using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using GameStateManagement;
using Manager;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

using GameStateManagement;
using System.Runtime.CompilerServices;

namespace wizard_game
{

    class Enemy_Doubler : Enemy
    {
        public static int MAX_DOUBLING = 4;

        int doubling;
        ParticleSystem particleSystem;
        Timer attackTimer;
        float attackRange;
        bool aStarThreadIsRunning = false;
        bool isAttacking;
        bool canAttack;
        Timer aStarTimer;


        public Enemy_Doubler(int x, int y, Map map, EnemyType type, Room room, int doubling) : base(x, y, map, type, "doubler", room)
        {
            particleSystem = new ParticleSystem(40);
            this.doubling = doubling;
            health = doubling;
            particleSystem.AddMagicEffect(new Vector2(position.X+width/2, position.Y+height/2), 20, Color.Coral, 0.4f);
            this.map = map;
            direction = new Vector2(1, 0);
            attackTimer = new Timer(1.5f, this);
            isAttacking = false;
            canAttack = true;
            speed = 2f;
            attackRange = 40;
            currentAnimation = "idle_left";
            aStarTimer = new Timer(1, this);
            aStarTimer.start();
        }



        


        public override void takeDamage(int damage)
        {
            if (doubling > 0)
            {
                doubling--;
                int rX = GameplayScreen.rand.Next(-40, 40)*2;
                int rY = GameplayScreen.rand.Next(-40, 40)*2;
                GameplayScreen.map.GetActiveRoom().acteurs.Add(new Enemy_Doubler((int)position.X + rX, (int)position.Y + rY, map, EnemyType.DOUBLER, map.GetActiveRoom(), doubling));
            }
            base.takeDamage(damage);   
        }









        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            particleSystem.Update(gameTime);
            currentSpeed = 0;
            aStarTimer.Update(gameTime);
            if (isDying) return;
            attackTimer.Update(gameTime);
            if (path.Count == 0 && !aStarThreadIsRunning)
            {                
                if (Math.Abs((GetMidPos() - Player.Get().GetMidPos()).Length()) <= playerViewDistance)
                {
                    aStarThreadIsRunning = true;
                    Task.Run(()=>
                    {
                        path = calculatePathToTarget(Player.Get().GetMidPos());
                        aStarThreadIsRunning = false;                  
                    });
                    aStarTimer.start();
                }
                else
                {
                    aStarThreadIsRunning = true;
                    Task.Run(()=>
                    {
                        path = calculatePathToTarget(GetNextPatroulliePoint());
                        aStarThreadIsRunning = false;                  
                    });
                    aStarTimer.start();
                }

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



        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            particleSystem.Draw();
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
            }
        }

        


        




        public override void Die()
        {
            base.Die();
            sprite.setAnimation("idle_right");
        }



        //überprüfen, ob Hindernisse (Wand) zw Player und Enemy vorliegen
        private bool IsObjBetweenEnemyAndPlayer()
        {
            // Spieler- und Gegnerposition abrufen
            Vector2 playerPos = Player.Get().position;
            Vector2 enemyPos = position;
            bool[,] fields = room.fields; // Wände oder Hindernisse

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
            }else{
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
            }else{
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





        public override void TimerCallback(Timer timer)
        {
            base.TimerCallback(timer);
            if (timer == attackTimer) canAttack = true;
            else if (timer == aStarTimer && !aStarThreadIsRunning)
            {
                if (Math.Abs((GetMidPos() - Player.Get().GetMidPos()).Length()) <= playerViewDistance)
                {               
                    aStarThreadIsRunning = true;
                    Task.Run(()=>
                    {
                        path = calculatePathToTarget(Player.Get().GetMidPos());
                        aStarThreadIsRunning = false;                  
                    });
                }
                aStarTimer.start();

            }
        }


    }
}