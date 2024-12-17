using System;
using System.Runtime.CompilerServices;
using GameStateManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace wizard_game
{

    class Fireball : Projectile
    {

        bool isAttacking;
        SoundEffect shootSound;
        Timer timer;
        public Fireball(float x, float y, Enemy attacker) : base(new Vector2(x, y), 1, 1, "fireball", false, attacker)
        {
            shootSound = GameStateManagementGame.Get().Content.Load<SoundEffect>("battle_sound_effects/Bow");
            sprite = new Sprite(GameStateManagementGame.Get().Content.Load<Texture2D>(spritename), 1, 1, 0.12f);
            sprite.SetScale(0.03f);
            timer = new Timer(1, this);
            isAttacking = false;
            speed = 100f;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            timer.Update(gameTime);
            timer.start();
            if (timer.getSecondsRemaining() < 0.03)
            {

                return;
            }
            if (Math.Sqrt(Math.Pow(position.X - Player.Get().position.X, 2) + Math.Pow(position.Y - Player.Get().position.Y, 2)) < 100)
            {
                //Console.WriteLine("player erreicht");
                shootSound.Play();
                GameplayScreen.projectiles.Remove(this);

                return;
            }
            if (Math.Abs(position.X - startPos.X) > 400 || Math.Abs(position.Y - startPos.Y) > 400)
            {
                //Console.WriteLine("weit...");
                GameplayScreen.projectiles.Remove(this);
                return;
            }

            position += direction * speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            //Console.WriteLine("direction: " + direction);
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

        public override void TimerCallback(Timer timer)
        {
            base.TimerCallback(timer);
            GameplayScreen.projectiles.Remove(this);
        }

        public new void SetDirection(Vector2 fireballDirection){
             Vector2 tmp = new Vector2(0,0);
            if(fireballDirection.X > 0){
                tmp.X = 1;
            }else tmp.X = -1;
             if(fireballDirection.Y > 0){
                tmp.Y = 1;
            }else tmp.Y = -1;
             direction = tmp;
        }


    }
}

