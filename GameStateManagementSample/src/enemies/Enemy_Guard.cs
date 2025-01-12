using System;
using GameStateManagement;
using Microsoft.Xna.Framework;

namespace wizard_game
{
    class Enemy_Guard : Enemy
    {
        Timer attackTimer;

        bool canAttack = true;

        public Enemy_Guard(int x, int y, Map map, EnemyType type, Room room) : base(x, y, map, type, "spriteSheetEnemy_Guard", room)
        {
            setSpeed();
            direction = new Vector2(1, 0);
            currentAnimation = "idle_right";
            sprite.setAnimation(currentAnimation);
            attackTimer = new Timer(1, this);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (isDying) return;
            attackTimer.Update(gameTime);
            Move();
            float distance = caculateDistance();
            if (distance <= 150)
            {
                Attack();
            }
            else
            {
                SetEnemyState(EnemyState.NORMAL);
            }
            UpdateAnimation();
            sprite.setAnimation(currentAnimation);
        }

        //Bewegung des Gegners
        public new void Move()
        {
            chooseADirection();
            //direction.Normalize();
            position += direction * currentSpeed;
        }


        //wählen eine andere Richtung wenn Kollision auftritt
        public void chooseADirection()
        {
            Random random = new Random();
            int b = 0;
            currentSpeed = speed;
            Vector2 tmp = position + direction * currentSpeed;
            while ((DetacteCollison(tmp) || DetacteCollison(tmp + new Vector2(width,0)) || DetacteCollison(tmp + new Vector2(width, height)) || DetacteCollison(tmp+new Vector2(0, height))) && b < 30)
            {
                b++;
                int randomNumber = random.Next(1, 101);
                if (randomNumber % 4 == 0)
                {
                    direction.X = 1;
                    direction.Y = 0;

                }
                else if (randomNumber % 4 == 1)
                {
                    direction.X = 0;
                    direction.Y = 1;

                }
                else if (randomNumber % 4 == 2)
                {
                    direction.X = 0;
                    direction.Y = -1;

                }
                else
                {
                    direction.X = -1;
                    direction.Y = 0;

                }
                tmp = position + direction * currentSpeed;

            }
            if (b>= 30) currentSpeed = 0;
        }

        //Wwenn der Abstand klein ist, wird gegen Spieler kämpfen
        public override void Attack()

        {
            MoveToPlayer();
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
            float posX = position.X + width / 2 - Fireball.FIREBALL_WIDTH/2;
            float posY = position.Y + height / 2 - Fireball.FIREBALL_HEIGHT/2;

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

        public override void TimerCallback(Timer timer)
        {
            base.TimerCallback(timer);
            if (timer == attackTimer)
            {
                canAttack = true; // Cooldown beendet, Angriff wieder möglich
            }

        }
    }
}