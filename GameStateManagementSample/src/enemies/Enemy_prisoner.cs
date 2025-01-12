
using System.IO;
using System.Threading.Tasks;
using GameStateManagement;
using Manager;
using Microsoft.Xna.Framework;

namespace wizard_game
{
    class Enemy_prisoner : Enemy
    {
        Gamestate[,] currentView;

        Timer attackTimer;
        bool aStarThreadIsRunning = false;
        bool isAttacking;
        bool canAttack;
        Timer aStarTimer;
        float attackRange;

        public Enemy_prisoner(int x, int y, Map map, EnemyType type, Room room) : base(x, y, map, type, "spriteSheetEnemy_Prisoner", room)
        {
            dieSound = AssetManager.GetSoundInstance("monster_sfx_pack/monster-6");     //sterbesound
            dieSound.Volume = GameStateManagementGame.soundSettings.GetVolumeForSound();
            setSpeed();
            this.map = map;
            direction = new Vector2(1, 0);
            attackTimer = new Timer(1.5f, this);
            isAttacking = false;
            canAttack = true;
            aStarTimer = new Timer(1, this);
            aStarTimer.start();
            attackRange = 40;

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

            }
        }



        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            currentSpeed = 0;
            if (isDying) return;
            aStarTimer.Update(gameTime);
            attackTimer.Update(gameTime);
            if (path.Count ==0 && !aStarThreadIsRunning)
            {
                aStarThreadIsRunning = true;
                Task.Run(()=>
                {
                    path = calculatePathToTarget(Player.Get().GetMidPos());
                    aStarThreadIsRunning = false;                  
                });
                aStarTimer.start();

            }
            if (path.Count >=1 && moveToTarget(path[path.Count-1])) path.RemoveAt(path.Count-1);




            if ((Player.Get().GetMidPos() - GetMidPos()).Length() < attackRange) Attack();

            if (isAttacking)
            {
                Vector2 o = new Vector2(width/2, height/2);
                sprite.origin = o;
                float endRotation = 1;
                rotation = MathHelper.Lerp(rotation, endRotation, 0.4f);
                if (rotation <= endRotation + 0.05 && rotation >= endRotation - 0.05)
                {
                    rotation = 0;
                    isAttacking = false;
                    if ((Player.Get().GetMidPos() - GetMidPos()).Length() < attackRange) Player.Get().takeDamage(1);
                }
            }


            UpdateAnimation();
            sprite.setAnimation(currentAnimation);

        }




        


        public override void Attack()
        {
            base.Attack();
            if (!isAttacking && canAttack)
            {
                isAttacking = true;
                canAttack = false;
                attackTimer.start();
            }

        }



        //überprüfen, ob Hindernisse (Wand) zw Player und Enemy vorliegen
        // private bool IsObjBetweenEnemyAndPlayer()
        // {
        //     Vector2 playerPos = Player.Get().position;
        //     Vector2 enemyPos = position;
        //     bool[,] fields = room.fields;

        //     // Positionen in Raster-Koordinaten umrechnen (z. B. bei 10 Pixel pro Feld)
        //     int x1 = (int)enemyPos.X / 10;
        //     int y1 = (int)enemyPos.Y / 10;
        //     int x2 = (int)playerPos.X / 10;
        //     int y2 = (int)playerPos.Y / 10;

        //     if (x1 == x2)
        //     {
        //         return DetectObjInDirectionY(x1, y1, y2, fields);
        //     }

        //     else if (y1 == y2)
        //     {

        //         return DetectObjInDirectionX(y1, x1, x2, fields);
        //     }

        //     else
        //     {
        //         int dx = Math.Abs(x2 - x1);
        //         int dy = Math.Abs(y2 - y1);
        //         int sx = x1 < x2 ? 1 : -1; // Schritt in x-Richtung
        //         int sy = y1 < y2 ? 1 : -1; // Schritt in y-Richtung

        //         for (int i = 0; i < dx; i++)
        //         {
        //             if (fields[x1 + i * sx, y1 + (int)(dy / dx) * sy])
        //             {
        //                 return true;
        //             }
        //         }
        //     }
        //     //-----------------------------------------------------------

        //     // Positionen in Raster-Koordinaten umrechnen (z. B. bei 10 Pixel pro Feld)
        //     x1 = (int)(enemyPos.X + width) / 10;
        //     y1 = (int)(enemyPos.Y + height) / 10;
        //     x2 = (int)(playerPos.X + Player.Get().GetWidth()) / 10;
        //     y2 = (int)(playerPos.Y + Player.Get().GetHeight()) / 10;

        //     if (x1 == x2)
        //     {
        //         return DetectObjInDirectionY(x1, y1, y2, fields);
        //     }

        //     else if (y1 == y2)
        //     {

        //         return DetectObjInDirectionX(y1, x1, x2, fields);
        //     }

        //     else
        //     {
        //         int dx = Math.Abs(x2 - x1);
        //         int dy = Math.Abs(y2 - y1);
        //         int sx = x1 < x2 ? 1 : -1; // Schritt in x-Richtung
        //         int sy = y1 < y2 ? 1 : -1; // Schritt in y-Richtung

        //         for (int i = 0; i < dx; i++)
        //         {
        //             if (fields[x1 + i * sx, y1 + (int)(dy / dx) * sy])
        //             {
        //                 return true;
        //             }
        //         }

        //     }

        //     //------------------------------------------------------
        //     // Positionen in Raster-Koordinaten umrechnen (z. B. bei 10 Pixel pro Feld)
        //     x1 = (int)(enemyPos.X + width) / 10;
        //     y1 = (int)enemyPos.Y / 10;
        //     x2 = (int)(playerPos.X + Player.Get().GetWidth()) / 10;
        //     y2 = (int)(playerPos.Y + Player.Get().GetHeight()) / 10;

        //     if (x1 == x2)
        //     {
        //         return DetectObjInDirectionY(x1, y1, y2, fields);
        //     }

        //     else if (y1 == y2)
        //     {

        //         return DetectObjInDirectionX(y1, x1, x2, fields);
        //     }

        //     else
        //     {
        //         int dx = Math.Abs(x2 - x1);
        //         int dy = Math.Abs(y2 - y1);
        //         int sx = x1 < x2 ? 1 : -1; // Schritt in x-Richtung
        //         int sy = y1 < y2 ? 1 : -1; // Schritt in y-Richtung

        //         for (int i = 0; i < dx; i++)
        //         {
        //             if (fields[x1 + i * sx, y1 + (int)(dy / dx) * sy])
        //             {
        //                 return true;
        //             }
        //         }

        //     }



        //     //-----------------------------------------------------
        //     // Positionen in Raster-Koordinaten umrechnen (z. B. bei 10 Pixel pro Feld)
        //     x1 = (int)enemyPos.X / 10;
        //     y1 = (int)(enemyPos.Y + height) / 10;
        //     x2 = (int)(playerPos.X + Player.Get().GetWidth()) / 10;
        //     y2 = (int)(playerPos.Y + Player.Get().GetHeight()) / 10;

        //     if (x1 == x2)
        //     {
        //         return DetectObjInDirectionY(x1, y1, y2, fields);
        //     }

        //     else if (y1 == y2)
        //     {

        //         return DetectObjInDirectionX(y1, x1, x2, fields);
        //     }

        //     else
        //     {
        //         int dx = Math.Abs(x2 - x1);
        //         int dy = Math.Abs(y2 - y1);
        //         int sx = x1 < x2 ? 1 : -1; // Schritt in x-Richtung
        //         int sy = y1 < y2 ? 1 : -1; // Schritt in y-Richtung

        //         for (int i = 0; i < dx; i++)
        //         {
        //             if (fields[x1 + i * sx, y1 + (int)(dy / dx) * sy])
        //             {
        //                 return true;
        //             }
        //         }

        //     }
        //     return false;
        // }

        // public bool DetectObjInDirectionY(int x1, int y1, int y2, bool[,] fields)
        // {

        //     if (y1 < y2)
        //     {
        //         for (int i = y1; i < y2; i++)
        //         {
        //             if (fields[x1, i])
        //             {
        //                 return true;
        //             }
        //         }
        //     }
        //     else
        //     {
        //         for (int i = y2; i < y1; i++)
        //         {
        //             if (fields[x1, i])
        //             {
        //                 return true;
        //             }
        //         }
        //     }
        //     return false;
        // }

        // public bool DetectObjInDirectionX(int y1, int x1, int x2, bool[,] fields)
        // {

        //     if (x1 < x2)
        //     {
        //         for (int i = x1; i < x2; i++)
        //         {
        //             if (fields[i, y1])
        //             {
        //                 return true;
        //             }
        //         }
        //     }
        //     else
        //     {
        //         for (int i = x2; i < x1; i++)
        //         {
        //             if (fields[i, y1])
        //             {
        //                 return true;
        //             }
        //         }
        //     }
        //     return false;
        // }



        public override void TimerCallback(Timer timer)
        {
            base.TimerCallback(timer);
            if (timer == attackTimer) canAttack = true;
            else if (timer == aStarTimer && !aStarThreadIsRunning)
            {                
                aStarThreadIsRunning = true;
                Task.Run(()=>
                {
                    path = calculatePathToTarget(Player.Get().GetMidPos());
                    aStarThreadIsRunning = false;                  
                });
                aStarTimer.start();
            }
        }

    }
}