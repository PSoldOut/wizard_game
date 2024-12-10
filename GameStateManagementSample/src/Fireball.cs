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
        public Fireball(float x, float y) : base(new Vector2(x, y), 1, 1, "fireball", false)
        {
            sprite = new Sprite(GameStateManagementGame.Get().Content.Load<Texture2D>(spritename), 1, 1, 0.12f);
            sprite.SetScale(0.005f);
            isAttacking = false;
            speed = 10f;
        }

        public override void Draw(GameTime gameTime)
        {
            if (isAttacking)
            {
                base.Draw(gameTime);
            }

        }

        public override void Update(GameTime gameTime)
        {
            if (isAttacking)
            {
                base.Update(gameTime);
                position += direction * speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                float opposite = Math.Abs(direction.X);
                float hipotenose = direction.Length();
                float alpha = (float)Math.Asin(opposite / hipotenose);
                if (direction.X >= 0) rotation = alpha;
                else rotation = -alpha;
            }
        }

        public void SetAttackstate(bool isAttacking)
        {
            this.isAttacking = isAttacking;
        }

        public Vector2 GetPos()
        {
            return position;
        }


    }
}

