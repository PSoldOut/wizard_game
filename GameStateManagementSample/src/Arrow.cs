using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.Diagnostics;
using GameStateManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace wizard_game
{
    public class Arrow : Projectile
    {

        public Arrow(int x, int y, Acteur attacker) : base(new Vector2(x, y), 10, 10, "arrow", true, attacker)
        {
            sprite = new Sprite(GameStateManagementGame.Get().Content.Load<Texture2D>(spritename), 1, 1, 0.12f);
            speed = 300f;
        }



        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (Math.Abs(position.X-startPos.X) > 400 || Math.Abs(position.Y - startPos.Y) > 400)
            {
                GameplayScreen.projectiles.Remove(this);
                return;
            }

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

            position += speed * direction * (float)gameTime.ElapsedGameTime.TotalSeconds;
            float opposite = Math.Abs(direction.X);
            float adjecent = Math.Abs(direction.Y);
            float alpha = (float)Math.Atan2(opposite,adjecent);

            if (direction.X >= 0) rotation = alpha;
            else rotation = -alpha;

            if (Math.Abs(position.X-startPos.X) > 400 || Math.Abs(position.Y - startPos.Y) > 400) GameplayScreen.projectiles.Remove(this);
        }
    }

}