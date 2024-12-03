using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.Diagnostics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using GameStateManagement;

namespace wizard_game
{

    public abstract class GameEntity
    {

        protected Sprite sprite;
        protected Vector2 direction;
        public Vector2 position;
        protected float rotation;
        protected int width;
        protected int height;
        public Rectangle hitBox;
        protected string spritename;
        bool hasCollision;

        private bool drawHitBox = true;
        private int lineWidth = 2;
        private Color hitboxColor = Color.Purple;
        public Texture2D image_hitbox;

        public GameEntity(Vector2 position, int width, int height, string spritename = null, bool hasCollision = true)
        {
            //Debug.WriteLine("init new game entity" + position.ToString() + " w" +width + " h "+height+" name"+spritename);
            this.position = position;
            this.width = width;
            this.height = height;
            this.spritename = spritename;
            this.hasCollision = hasCollision;
            hitBox = new Rectangle((int)position.X, (int)position.Y, width, height);
            image_hitbox = new Texture2D(GameStateManagementGame.Get().GraphicsDevice, 1, 1);
            image_hitbox.SetData(new Color[] { Color.Red });
        }

        public virtual void OnInput(GameStateManagementGame.InputState input)
        {
        }


        public virtual void Draw(GameTime gameTime)
        {
            sprite.rotation = rotation;
            sprite.Draw((int)position.X, (int)position.Y);
            if (drawHitBox)
            {
                drawDebugHitBox();
            }

        }
        public virtual void drawDebugHitBox()
        {
            GameStateManagementGame._spriteBatch.Draw(image_hitbox, new Rectangle(hitBox.X, hitBox.Y, lineWidth, hitBox.Height + lineWidth), hitboxColor);
            GameStateManagementGame._spriteBatch.Draw(image_hitbox, new Rectangle(hitBox.X, hitBox.Y, hitBox.Width + lineWidth, lineWidth), hitboxColor);
            GameStateManagementGame._spriteBatch.Draw(image_hitbox, new Rectangle(hitBox.X + hitBox.Width, hitBox.Y, lineWidth, hitBox.Height + lineWidth), hitboxColor);
            GameStateManagementGame._spriteBatch.Draw(image_hitbox, new Rectangle(hitBox.X, hitBox.Y + hitBox.Height, hitBox.Width + lineWidth, lineWidth), hitboxColor);
        }

        public virtual void Update(GameTime gameTime)
        {
            sprite.Update(gameTime);
            sprite.rotation = rotation;
            hitBox.X = (int)position.X;
            hitBox.Y = (int)position.Y;
        }

        public Vector2 GetDirection()
        {
            return direction;
        }

        public int GetWidth()
        {
            return width;
        }

        public int GetHeight()
        {
            return height;
        }


        public void SetDirection(Vector2 direciton)
        {
            this.direction = direciton;
        }




    }

}