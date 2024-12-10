using System;
using System.Diagnostics;
using GameStateManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace wizard_game
{
    class Enemy_Guard : Enemy
    {
        static int idEnemy;
        int blut = 10;

        public Enemy_Guard(int x, int y, Map map, EnemyType type, Room room) : base(x, y, map, type, "spriteSheetEnemy_Guard", room)
        {
            setSpeed();
            direction = new Vector2(1, 0);
            sprite.setAnimation("idle_right");
            idEnemy++;
        }


        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            Move();
            float distance = caculateDistance();
            if (distance <= 100)
            {
                Attack(distance);
            }
            UnloadFireball();
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

        //rechnen Abstand zwischen Player und Gegner
        public float caculateDistance()
        {
            float distanceX = position.X - Player.Get().position.X;
            float distanceY = position.Y - Player.Get().position.Y;
            return (float)Math.Sqrt(Math.Pow(distanceX, 2) + Math.Pow(distanceY, 2));
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
                    // TODO: rechts
                    direction.X = 1;
                    direction.Y = 0;
                    anim = "idle_right";
                }
                else if (randomNumber % 4 == 1)
                {
                    //TODO: unten
                    direction.X = 0;
                    direction.Y = 1;
                    anim = "idle_down";
                }
                else if (randomNumber % 4 == 2)
                {
                    //TODO: oben
                    direction.X = 0;
                    direction.Y = -1;
                    anim = "idle_up";
                }
                else
                {
                    //TODO: links
                    direction.X = -1;
                    direction.Y = 0;
                    anim = "idle_left";
                }
                tmp = position + direction * speed;
            }
            sprite.setAnimation(anim);
        }

        //Wwenn der Abstand klein ist, wird gegen Spieler kämpfen
        public void Attack(float distance)

        {
            MoveToPlayer();
            if (distance >= 0)
            {
                createFireBalls(position, Player.Get().position);
            }
            SetEnemyState(EnemyState.ATTACKING);
        }
        public void createFireBalls(Vector2 enemyPos, Vector2 playerPos)
        {
            // Erstelle die Position für den Fireball (in der Nähe des Feindes)
            float posX = position.X + width / 2; // Position anpassen
            float posY = position.Y + height / 2; // Position anpassen

            // Erstelle den Fireball
            Fireball fireball = new Fireball(posX, posY);
            fireball.SetDirection(direction);
            // Setze den Angriffszustand des Fireballs
            fireball.SetAttackstate(true);
            // Füge den Fireball der Projektilliste hinzu
            GameplayScreen.projectiles.Add(fireball);
        }

        public void UnloadFireball()
        {
            foreach (Projectile p in GameplayScreen.projectiles)
            {
                if (p is Fireball fireball)
                {
                    if(fireball.hitBox.Intersects(Player.Get().hitBox))
                    fireball.SetAttackstate(false);
                }
            }
        }

    }
}