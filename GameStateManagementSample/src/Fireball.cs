using System;
using System.Runtime.CompilerServices;
using GameStateManagement;
using Manager;
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
        Player player;
        Timer timer;
        public static int FIREBALL_WIDTH = 30;
        public static int FIREBALL_HEIGHT = 30;
        public Fireball(float x, float y, Acteur attacker) : base(new Vector2(x, y), FIREBALL_WIDTH, FIREBALL_HEIGHT, "fireball", false, attacker)
        {
            shootSound = AssetManager.GetSound("fire");
            sprite = new Sprite(GameStateManagementGame.Get().Content.Load<Texture2D>(spritename), 1, 1, 0.03f);
            sprite.offset = new Vector2(FIREBALL_WIDTH/2.0f, FIREBALL_HEIGHT/2.0f);
            sprite.origin = new Vector2(60/0.03f/2.0f, 60/0.03f/2.0f);          //die 60 ist ein bischen gefuscht
            damage = 2;
            timer = new Timer(3, this);
            isAttacking = false;
            speed = 250f;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            player = Player.Get();
            timer.Update(gameTime);
            timer.start();


            for (int i = 0; i < GameplayScreen.acteurs.Count; i++)
            {
                if (hitBox.Intersects(GameplayScreen.acteurs[i].hitBox) && GameplayScreen.acteurs[i] != attacker)
                {
                    hitSound.Stop();
                    hitSound.Play();
                    GameplayScreen.acteurs[i].takeDamage(damage);
                    GameplayScreen.projectiles.Remove(this);
                    return;
                }
            }


            if (DetacteCollison())
            {
                GameplayScreen.projectiles.Remove(this);
                return;
            }



            position += direction * speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            //Console.WriteLine("direction: " + direction);


            float opposite = direction.X;
            float adjecent = direction.Y;
            float alpha = (float)Math.Atan2(opposite,adjecent);
            rotation = -alpha;

         }




        public void SetAttackstate(bool isAttacking)
        {
            timer.start();
            shootSound.Play();
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
            // Vector2 tmp = new Vector2(0,0);
            //if(fireballDirection.X > 0){
            //    tmp.X = 1;
            //}else tmp.X = -1;
            // if(fireballDirection.Y > 0){
            //    tmp.Y = 1;
            //}else tmp.Y = -1;
            // direction = tmp;
            this.direction = fireballDirection;
        }


    }
}

