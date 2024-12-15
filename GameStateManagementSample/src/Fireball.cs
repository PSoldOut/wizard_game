using System;
using System.Runtime.CompilerServices;
using GameStateManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace wizard_game
{

    class Fireball : Projectile
    {

        bool isAttacking;
        Timer timer;
        public Fireball(float x, float y, Enemy attacker) : base(new Vector2(x, y), 1, 1, "fireball", false, attacker)
        {
            sprite = new Sprite(GameStateManagementGame.Get().Content.Load<Texture2D>(spritename), 1, 1, 0.12f);
            sprite.SetScale(0.03f);
            timer = new Timer(1);
            isAttacking = false;
            speed = 100f;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            timer.Update(gameTime);
            if (timer.getSecondsRemaining() < 0.03)
            {
                Console.WriteLine("time over---");
                GameplayScreen.projectiles.Remove(this);
                foreach(Projectile p in GameplayScreen.projectiles){
                    if(p.position == this.position){
                        Console.WriteLine("nicht gelöscht");
                    }
                }
                return;
            }
            if (Math.Sqrt(Math.Pow(position.X - Player.Get().position.X, 2) + Math.Pow(position.Y - Player.Get().position.Y, 2)) < 1)
            {
                Console.WriteLine("player erreicht");
                GameplayScreen.projectiles.Remove(this);

                return;
            }
            if (Math.Abs(position.X - startPos.X) > 400 || Math.Abs(position.Y - startPos.Y) > 400)
            {
                Console.WriteLine("weit...");
                GameplayScreen.projectiles.Remove(this);
                return;
            }
            if (attacker is Enemy enemy && enemy.GetEnemyState() == Enemy.EnemyState.ATTACKING)
            {
                position += direction * speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
        }

        public void SetAttackstate(bool isAttacking)
        {
            timer.start();
            this.isAttacking = isAttacking;
        }

        public bool GetAttackState()
        {
            return isAttacking;
        }

        public Vector2 GetPos()
        {
            return position;
        }

        public new void SetDirection(Vector2 fireballDirection){
             Vector2 tmp = new Vector2(0,0);
            if(fireballDirection.X > 0){
                direction.X = 1;
            }else direction.X = -1;
             if(fireballDirection.Y > 0){
                direction.Y = 1;
            }else direction.Y = -1;
             direction = tmp;
        }


    }
}

