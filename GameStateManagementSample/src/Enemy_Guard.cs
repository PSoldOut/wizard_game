using System;
using System.Diagnostics;
using GameStateManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace wizard_game
{
    class Enemy_Guard : Enemy
    {
        Timer attackTimer;

        bool canAttack = true;
        static int idEnemy;
        int blut = 10;

        public Enemy_Guard(int x, int y, Map map, EnemyType type, Room room) : base(x, y, map, type, "spriteSheetEnemy_Guard", room)
        {
            setSpeed();
            direction = new Vector2(1, 0);
            sprite.setAnimation("idle_right");
            idEnemy++;
            attackTimer = new Timer(1, this);
        }


        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
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
            //UnloadFireball();
            sprite.Update(gameTime);

        }

        //Bewegung des Gegners
        public new void Move()
        {
            //  if(hitBox.X < 1240 && hitBox.X >= 0 && hitBox.Y >= 0 && hitBox.Y < 1000){
            chooseADirection();
            direction.Normalize();
            position += direction * speed;
        }

        //wählen eine andere Richtung wenn Kollision auftritt
        public void chooseADirection()
        {
            Random random = new Random();

            string anim = "idle_down";
            Vector2 tmp = position + direction * speed;
            while (DetacteCollison(tmp))
            {
                int randomNumber = random.Next(1, 101);
                if (randomNumber % 4 == 0)
                {
                    direction.X = 1;
                    direction.Y = 0;
                    anim = "idle_right";
                }
                else if (randomNumber % 4 == 1)
                {
                    direction.X = 0;
                    direction.Y = 1;
                    anim = "idle_down";
                }
                else if (randomNumber % 4 == 2)
                {
                    direction.X = 0;
                    direction.Y = -1;
                    anim = "idle_up";
                }
                else
                {
                    direction.X = -1;
                    direction.Y = 0;
                    anim = "idle_left";
                }
                tmp = position + direction * speed;
            }
            sprite.setAnimation(anim);
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
            float posX = position.X + width / 2;
            float posY = position.Y + height / 2;

            // Berechne die Richtung des Fireballs zum Spieler
            Vector2 playerPosition = Player.Get().position;
            Vector2 fireballDirection = playerPosition - new Vector2(posX, posY);
            fireballDirection.Normalize(); // Richtung normalisieren

            // Erstelle den Fireball
            Fireball fireball = new Fireball(posX, posY, this);
            fireball.SetDirection(fireballDirection);
            fireball.SetAttackstate(true);
            GameplayScreen.projectiles.Add(fireball);
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