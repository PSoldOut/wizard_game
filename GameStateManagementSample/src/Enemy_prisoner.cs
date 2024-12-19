using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using GameStateManagement;
using Manager;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace wizard_game
{
    class Enemy_prisoner : Enemy
    {
        Map map;
        NodeA startNode = null;
        NodeA solutionNode = null;
        List<Vector2> path = new List<Vector2>();
        int cordXInGamestate;
        int cordYInGamestate;
        Gamestate[,] currentView;

        public Enemy_prisoner(int x, int y, Map map, EnemyType type, Room room) : base(x, y, map, type, "spriteSheetEnemy_Prisoner", room)
        {
            dieSound = AssetManager.GetSoundInstance("monster_sfx_pack/monster-6");     //sterbesound
            dieSound.Volume = GameStateManagementGame.GetSoundVolume();
            setSpeed();
            //speed = 0.5f;
            this.map = map;
            direction = new Vector2(1, 0);

            using (StreamWriter writer = new StreamWriter("C:\\Users\\Philipp\\Desktop\\gitProjects\\wizard_game\\GameStateManagementSample\\src\\test.txt"))
            {
                currentView = room.gamestate;
                for (int row = 0; row < currentView.GetLength(1); row++) // Erste Dimension: Zeilen
                {
                    for (int col = 0; col < currentView.GetLength(0); col++) // Zweite Dimension: Spalten
                    {
                        Gamestate state = currentView[col, row];
                        writer.Write("\t" + state + "\t");
                    }
                    writer.WriteLine();
                }
                //throw new Exception("fertig");
            }
            
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            //if (isDying) return;
            //if (path.Count ==0)
            //{
            //    cordXInGamestate = (int)position.X / 10;
            //    cordYInGamestate = (int)position.Y / 10;
            //    currentView = room.gamestate;
            //    startNode = new NodeA(currentView, cordXInGamestate, cordYInGamestate, null, EnemyAction.NONE, 0);
            //    solutionNode = AStar(startNode);
            //    while(solutionNode != null)
            //    {
            //        path.Add(new Vector2(solutionNode.GetX()*10, solutionNode.GetY()*10));
            //        solutionNode = solutionNode.GetParent();
            //    }
            //}
            //
            //if (path.Count >=1 && moveToTarget(path[path.Count-1])) path.RemoveAt(path.Count-1);
            //Console.WriteLine("count: " + path.Count() + "nextTarget:" + path[path.Count-1]);

        }

        //Bewegung des Gegners: wenn kein Objekt auf dem Weg zu Spieler gibt, dann verfolgt er spieler
        public new void Move()
        {
            if (!IsObjBetweenEnemyAndPlayer())
            {
                follow();
            }
        }


        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            foreach(Vector2 p in path)
            {
                drawPoint(p);
            }
        }

        //Den Spieler verfolgen
        public void follow()
        {
            if (hitBox.X < 1240 && hitBox.X >= 0 && hitBox.Y >= 0 && hitBox.Y < 1000)
            {
                MoveToPlayer();
                direction.Normalize();
                Vector2 test = position + direction * speed;

                if (!DetacteCollison(test))
                {
                    // Debug.WriteLine("no kollision");
                    position = test;
                }

//                 if (caculateDistance() < 50)
//                 {
//                     dieSound = GameStateManagementGame.Get().Content.Load<SoundEffect>("battle_sound_effects/hit01").CreateInstance();
//                     dieSound.Play();
//    //Wird aber kontinuierlich displayed
//                 }
//                 else
//                 {
                    dieSound = GameStateManagementGame.Get().Content.Load<SoundEffect>("monster_sfx_pack/monster-6").CreateInstance();
                // }
            }
        }

        //bewegen sich  nach der Richtung des Players
        public new void MoveToPlayer()

        {

            float deltaX = Player.Get().position.X - position.X;
            float deltaY = Player.Get().position.Y - position.Y;

            // Bestimme, ob Bewegung entlang X oder Y priorisiert wird
            if (Math.Abs(deltaX) > Math.Abs(deltaY)) // Bewegung entlang der X-Achse
            {
                if (deltaX > 0)
                {
                    direction.X = 1; // nach rechts
                    sprite.setAnimation("right");
                }
                else
                {
                    direction.X = -1; // nach links
                    sprite.setAnimation("left");
                }
                //  direction.Y = 0; // Nur entlang der X-Achse bewegen
            }
            else // Bewegung entlang der Y-Achse
            {
                if (deltaY > 0)
                {
                    direction.Y = 1; // nach unten
                    sprite.setAnimation("down");
                }
                else
                {
                    direction.Y = -1; // nach oben
                    sprite.setAnimation("up");
                }
                //  direction.X = 0; // Nur entlang der Y-Achse bewegen
            }

            //Debug.WriteLine(direction + " direction");
        }



        //überprüfen, ob Hindernisse (Wand) zw Player und Enemy vorliegen
        private bool IsObjBetweenEnemyAndPlayer()
        {
            Vector2 playerPos = Player.Get().position;
            Vector2 enemyPos = position;
            bool[,] fields = room.fields;

            // Positionen in Raster-Koordinaten umrechnen (z. B. bei 10 Pixel pro Feld)
            int x1 = (int)enemyPos.X / 10;
            int y1 = (int)enemyPos.Y / 10;
            int x2 = (int)playerPos.X / 10;
            int y2 = (int)playerPos.Y / 10;

            if (x1 == x2)
            {
                return DetectObjInDirectionY(x1, y1, y2, fields);
            }

            else if (y1 == y2)
            {

                return DetectObjInDirectionX(y1, x1, x2, fields);
            }

            else
            {
                int dx = Math.Abs(x2 - x1);
                int dy = Math.Abs(y2 - y1);
                int sx = x1 < x2 ? 1 : -1; // Schritt in x-Richtung
                int sy = y1 < y2 ? 1 : -1; // Schritt in y-Richtung

                for (int i = 0; i < dx; i++)
                {
                    if (fields[x1 + i * sx, y1 + (int)(dy / dx) * sy])
                    {
                        return true;
                    }
                }
            }
            //-----------------------------------------------------------
            
            // Positionen in Raster-Koordinaten umrechnen (z. B. bei 10 Pixel pro Feld)
            x1 = (int)(enemyPos.X + width) / 10;
            y1 = (int)(enemyPos.Y + height) / 10;
            x2 = (int)(playerPos.X + Player.Get().GetWidth()) / 10;
            y2 = (int)(playerPos.Y + Player.Get().GetHeight()) / 10;

            if (x1 == x2)
            {
                return DetectObjInDirectionY(x1, y1, y2, fields);
            }

            else if (y1 == y2)
            {

                return DetectObjInDirectionX(y1, x1, x2, fields);
            }

            else
            {
                int dx = Math.Abs(x2 - x1);
                int dy = Math.Abs(y2 - y1);
                int sx = x1 < x2 ? 1 : -1; // Schritt in x-Richtung
                int sy = y1 < y2 ? 1 : -1; // Schritt in y-Richtung

                for (int i = 0; i < dx; i++)
                {
                    if (fields[x1 + i * sx, y1 + (int)(dy / dx) * sy])
                    {
                        return true;
                    }
                }
            
            }

            //------------------------------------------------------
            // Positionen in Raster-Koordinaten umrechnen (z. B. bei 10 Pixel pro Feld)
            x1 = (int)(enemyPos.X + width) / 10;
            y1 = (int)enemyPos.Y / 10;
            x2 = (int)(playerPos.X + Player.Get().GetWidth()) / 10;
            y2 = (int)(playerPos.Y + Player.Get().GetHeight()) / 10;

            if (x1 == x2)
            {
                return DetectObjInDirectionY(x1, y1, y2, fields);
            }

            else if (y1 == y2)
            {

                return DetectObjInDirectionX(y1, x1, x2, fields);
            }

            else
            {
                int dx = Math.Abs(x2 - x1);
                int dy = Math.Abs(y2 - y1);
                int sx = x1 < x2 ? 1 : -1; // Schritt in x-Richtung
                int sy = y1 < y2 ? 1 : -1; // Schritt in y-Richtung

                for (int i = 0; i < dx; i++)
                {
                    if (fields[x1 + i * sx, y1 + (int)(dy / dx) * sy])
                    {
                        return true;
                    }
                }
            
            }



            //-----------------------------------------------------
            // Positionen in Raster-Koordinaten umrechnen (z. B. bei 10 Pixel pro Feld)
            x1 = (int)enemyPos.X / 10;
            y1 = (int)(enemyPos.Y + height) / 10;
            x2 = (int)(playerPos.X + Player.Get().GetWidth()) / 10;
            y2 = (int)(playerPos.Y + Player.Get().GetHeight()) / 10;

            if (x1 == x2)
            {
                return DetectObjInDirectionY(x1, y1, y2, fields);
            }

            else if (y1 == y2)
            {

                return DetectObjInDirectionX(y1, x1, x2, fields);
            }

            else
            {
                int dx = Math.Abs(x2 - x1);
                int dy = Math.Abs(y2 - y1);
                int sx = x1 < x2 ? 1 : -1; // Schritt in x-Richtung
                int sy = y1 < y2 ? 1 : -1; // Schritt in y-Richtung

                for (int i = 0; i < dx; i++)
                {
                    if (fields[x1 + i * sx, y1 + (int)(dy / dx) * sy])
                    {
                        return true;
                    }
                }
            
            }
            return false;
        }

        public bool DetectObjInDirectionY(int x1, int y1, int y2, bool[,] fields)
        {

            if (y1 < y2)
            {
                for (int i = y1; i < y2; i++)
                {
                    if (fields[x1, i])
                    {
                        return true;
                    }
                }
            }
            else
            {
                for (int i = y2; i < y1; i++)
                {
                    if (fields[x1, i])
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool DetectObjInDirectionX(int y1, int x1, int x2, bool[,] fields)
        {

            if (x1 < x2)
            {
                for (int i = x1; i < x2; i++)
                {
                    if (fields[i, y1])
                    {
                        return true;
                    }
                }
            }
            else
            {
                for (int i = x2; i < x1; i++)
                {
                    if (fields[i, y1])
                    {
                        return true;
                    }
                }
            }
            return false;
        }



    }
}