
using System;
using System.Threading.Tasks;
using GameStateManagement;
using Manager;
using Microsoft.Xna.Framework;

namespace wizard_game
{
    class Enemy_Wizard : Enemy
    {
        public static int MAX_DOUBLING = 4;

        int doubling;
        ParticleSystem particleSystem;
        Timer attackTimer;
        float attackRangeFist;
        float attaclRangeFireball;
        bool aStarThreadIsRunning = false;
        bool isAttacking;
        bool canAttack;
        Timer aStarTimer;
        private const double visibleTime = 1000;
        private const double invisibleTime = 2000;
        private bool isVisible = true;
        private double visibilityTimer = 0;


        public Enemy_Wizard(int x, int y, Map map, EnemyType type, Room room, int doubling) : base(x, y, map, type, "spriteSheetEnemy_Wizard", room)
        {
            expDrop = 1000;
            particleSystem = new ParticleSystem(40);
            this.doubling = doubling;
            health = doubling * 16;
            particleSystem.AddMagicEffect(new Vector2(position.X+width/2, position.Y+height/2), 20, Color.Coral, 0.4f);
            this.map = map;
            direction = new Vector2(1, 0);
            attackTimer = new Timer(1.5f, this);
            isAttacking = false;
            canAttack = true;
            speed = 2f;
            attackRangeFist = 40;
            attaclRangeFireball = 300;
            currentAnimation = "idle_left";
            aStarTimer = new Timer(1, this);
            aStarTimer.start();
            playerViewDistance = 800;
        }



        


        public override void takeDamage(int damage)
        {
            if (doubling > 0)
            {
                doubling--;
                int rX = GameplayScreen.rand.Next(-40, 40)*2;
                int rY = GameplayScreen.rand.Next(-40, 40)*2;
                GameplayScreen.map.GetActiveRoom().acteurs.Add(new Enemy_Wizard((int)position.X + rX, (int)position.Y + rY, map, EnemyType.DOUBLER, map.GetActiveRoom(), doubling));
            }
            base.takeDamage(damage);   
        }


        public void AttackFist()
        {
            if (!isAttacking && canAttack)
            {
                isAttacking = true;
                canAttack = false;
                attackTimer.start();
            }

        }

        public override void Attack()
        {
            if (canAttack)
            {
                createFireBalls();
            }

            SetEnemyState(EnemyState.ATTACKING);
            canAttack = false;
            attackTimer.start();
        }
        public void createFireBalls()
        {
            // Erstelle die Position für den Fireball (in der Nähe des Feindes)
            float posX = position.X + width / 2 - Fireball.FIREBALL_WIDTH / 2;
            float posY = position.Y + height / 2 - Fireball.FIREBALL_HEIGHT / 2;

            // Berechne die Richtung des Fireballs zum Spieler
            Vector2 playerPosition = Player.Get().position;
            Vector2 fireballDirection = playerPosition - position;
            fireballDirection.Normalize(); // Richtung normalisieren

            // Erstelle den Fireball
            Fireball fireball = new Fireball(posX, posY, this);
            fireball.SetDirection(fireballDirection);
            fireball.SetTarget(Player.Get());
            fireball.SetAttackstate(true);
            GameplayScreen.map.GetActiveRoom().projectiles.Add(fireball);
        }


        public override void Draw(GameTime gameTime)
        {
            particleSystem.Draw();
            if (isVisible)
            {
                base.Draw(gameTime);
            }
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
            


            


            
            if ((Player.Get().GetMidPos() - GetMidPos()).Length() < attackRangeFist) AttackFist();
            else if ((Player.Get().GetMidPos() - GetMidPos()).Length() < attaclRangeFireball) Attack();

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
                    if ((Player.Get().GetMidPos() - GetMidPos()).Length() < attackRangeFist) Player.Get().takeDamage(4);
                }
            }


            UpdateAnimation();
            sprite.setAnimation(currentAnimation);


            visibilityTimer += gameTime.ElapsedGameTime.TotalMilliseconds;

            if (isVisible && visibilityTimer >= visibleTime)
            {
                isVisible = false;
                visibilityTimer = 0;
            }
            else if (!isVisible && visibilityTimer >= invisibleTime)
            {

                isVisible = true;
                visibilityTimer = 0;
            }

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
