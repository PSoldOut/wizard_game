using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using GameStateManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace wizard_game
{
    class Enemy_prisoner : Enemy
    {
        Map map;
        public Enemy_prisoner(int x, int y, Map map, EnemyType type, Room room) : base(x, y, map, type, "spriteSheetEnemy_Prisoner", room)
        {
            dieSound = GameStateManagementGame.Get().Content.Load<SoundEffect>("monster_sfx_pack/monster-6").CreateInstance();
            dieSound.Volume = GameStateManagementGame.GetSoundVolume();
            setSpeed();
            this.map = map;
            direction = new Vector2(1, 0);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            Move();

            sprite.Update(gameTime);

        }

        //Bewegung des Gegners: wenn kein Objekt auf dem Weg zu Spieler gibt, dann verfolgt er spieler
        public new void Move()
        {
            if (!IsObjBetweenEnemyAndPlayer() && !isDying)
            {
                follow();
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
                int sx = x1 < x2 ? 1 : -1;
                int sy = y1 < y2 ? 1 : -1;
                if (dx < dy)
                {
                    for (int i = 0; i < dx; i++)
                    {
                        if (fields[x1 + i * sx, y1 + (int)(dy / dx) * sy])
                        {
                            return true;
                        }
                    }
                }
                // Keine Wand auf der Linie gefunden
                return false;
            }
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