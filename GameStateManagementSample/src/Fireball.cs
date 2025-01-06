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
        Timer timer;
        public static int FIREBALL_WIDTH = 30;
        public static int FIREBALL_HEIGHT = 30;


        Vector2 acceleration;
        Vector2 velocity;
        float maxForce = 0.7f;
        Acteur target;
        bool hasTarget = false;

        public Fireball(float x, float y, Acteur attacker) : base(new Vector2(x, y), FIREBALL_WIDTH, FIREBALL_HEIGHT, "fireball", false, attacker)
        {
            shootSound = AssetManager.GetSound("fire");
            sprite = new Sprite(GameStateManagementGame.Get().Content.Load<Texture2D>(spritename), 1, 1, 0.03f);
            sprite.offset = new Vector2(FIREBALL_WIDTH/2.0f, FIREBALL_HEIGHT/2.0f);
            sprite.origin = new Vector2(60/0.03f/2.0f, 60/0.03f/2.0f);          //die 60 ist ein bischen gefuscht
            damage = 2;
            timer = new Timer(3.5, this);
            isAttacking = false;
            speed = 250f;
            acceleration = new Vector2(0,0);
            velocity = new Vector2(0,0);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            timer.Update(gameTime);
            timer.start();
            


            for (int i = 0; i < GameplayScreen.map.GetActiveRoom().acteurs.Count; i++)
            {
                if (hitBox.Intersects(GameplayScreen.map.GetActiveRoom().acteurs[i].hitBox) && GameplayScreen.map.GetActiveRoom().acteurs[i] != attacker)
                {
                    hitSound.Stop();
                    hitSound.Play();
                    GameplayScreen.map.GetActiveRoom().acteurs[i].takeDamage(damage);
                    GameplayScreen.map.GetActiveRoom().projectiles.Remove(this);
                    return;
                }
            }


            if (DetacteCollison())
            {
                GameplayScreen.map.GetActiveRoom().projectiles.Remove(this);
                return;
            }


            if (hasTarget)
            {
                Vector2 desired = target.GetMidPos() - position;
                desired.Normalize();
                desired = desired * speed;
                Vector2 steer = desired - velocity;
                steer.Normalize();
                if (steer.Length() > maxForce)
                {  
                    steer.Normalize();
                    steer*=maxForce;
                }
                velocity += steer * 8;
                if (velocity.Length() > speed)
                {
                    velocity.Normalize();
                    velocity*=speed;
                }
                position += velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
            } else
            {
                position += direction * speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            //Console.WriteLine("direction: " + direction);


            float opposite = velocity.X;
            float adjecent = velocity.Y;
            float alpha = (float)Math.Atan2(opposite,adjecent);
            rotation = -alpha;

        }


        public void SetTarget(Acteur target)
        {
            this.target = target;
            hasTarget = true;
        }


        public float caculateDistance()
        {
            float distanceX = position.X + width/2 - Player.Get().position.X - Player.Get().width/2;
            float distanceY = position.Y +height/2 - Player.Get().position.Y - Player.Get().height/2;
            return (float)Math.Sqrt(Math.Pow(distanceX, 2) + Math.Pow(distanceY, 2));
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
            GameplayScreen.map.GetActiveRoom().projectiles.Remove(this);
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
            fireballDirection.Normalize();
            this.direction = fireballDirection;
            this.velocity = fireballDirection * speed;
        }


    }
}

