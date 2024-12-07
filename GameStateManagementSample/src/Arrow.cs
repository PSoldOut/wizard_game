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
        Vector2 startPos;

        public Arrow(int x, int y) : base(new Vector2(x, y), 10, 10, "arrow", true)
        {
            sprite = new Sprite(GameStateManagementGame.Get().Content.Load<Texture2D>(spritename), 1, 1, 0.12f);
            speed = 300f;
            startPos = position;
        }



        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            position += speed * direction * (float)gameTime.ElapsedGameTime.TotalSeconds;
            float opposite = Math.Abs(direction.X);
            float hipotenose = direction.Length();
            float alpha = (float)Math.Asin(opposite/hipotenose);
            if (direction.X >= 0) rotation = alpha;
            else rotation = -alpha;
            if (Math.Abs(position.X - startPos.X) >= 400 || Math.Abs(position.Y - startPos.Y) >= 400) GameplayScreen.projectiles.Remove(this);
        }
    }

}