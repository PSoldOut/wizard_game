using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using GameStateManagement;
using Manager;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace wizard_game
{
    public class Wall : GameEntity
    {
        public Texture2D image;

        private bool mousePressed;
        private bool containsMouse = false;
        private Point startPointDrag;
        public bool visible = true;




        String idx;
        Room room; //Der raum in dem die wall ist

        public Wall(Room _room, Point _pos, Point _size, string spritename = null, string _id = "") :
        base(_pos.ToVector2(), _size.X, _size.Y, spritename, true)
        {

            idx = _id;
            room = _room;
            if (spritename == null)
            {

                image = new Texture2D(GameStateManagementGame.Get().GraphicsDevice, 1, 1);
                image.SetData(new Color[] { Color.Red });
                sprite = new Sprite(image, 1, 1, 1);
            }


            LoadSprite();
            setFields();
        }
        private void setFields()
        {
            //todo kann viel schneller laufen man kennt ja alle kordinaten
            //Debug.WriteLine("set fields" + width +" h" + height);
            for (int x = 0; x < width / 10; x++)
            {
                for (int y = 0; y < height / 10; y++)
                {
                    int cordX = (int)position.X / 10 + x;
                    int cordY = (int)position.Y / 10 + y;
                    room.fields[cordX, cordY] = true;
                    room.gamestate[cordX, cordY] = Gamestate.WALL;
                    //Debug.WriteLine("set x "+cordX +" Y" +cordY);
                }

            }
        }



        public override void Update(GameTime gameTime)
        {
            // Debug.WriteLineIf(idx.Contains("Tile"), "id " + idx);
            if (Mouse.GetState().LeftButton == ButtonState.Pressed && !mousePressed)
            {
                if (hitBox.Contains(Mouse.GetState().Position))
                {
                    containsMouse = true;
                    startPointDrag = hitBox.Location - Mouse.GetState().Position;
                }
                mousePressed = true;

            }
            if (Mouse.GetState().LeftButton == ButtonState.Released && mousePressed)
            {
                mousePressed = false;
                containsMouse = false;


            }
            if (mousePressed && containsMouse)
            {
                hitBox.X = Mouse.GetState().Position.X + startPointDrag.X;
                hitBox.Y = Mouse.GetState().Position.Y + startPointDrag.Y;

            }
            base.Update(gameTime);

        }




        public bool DetacteCollisonPixelPrecise(Rectangle otherHitbox, Color[] playerImage)
        {

            if (!hitBox.Intersects(otherHitbox))
            {
                return false;
            }


            int top = Math.Max(otherHitbox.Top, hitBox.Top);
            int bottom = Math.Min(otherHitbox.Bottom, hitBox.Bottom);
            int left = Math.Max(otherHitbox.Left, hitBox.Left);
            int right = Math.Min(otherHitbox.Right, hitBox.Right);
            for (int y = top; y < bottom; y++)
            {
                for (int x = left; x < right; x++)
                {

                    Color color1 = playerImage[x - otherHitbox.Left + (y - otherHitbox.Top) * otherHitbox.Width];
                    if (color1.A != 0)
                    {
                        return true;
                    }


                }
            }
            return false;
        }



        public bool DetacteCollison(Rectangle otherHitbox)
        {

            if (hitBox.Intersects(otherHitbox))
            {
                return true;
            }
            return false;
        }

        public override void Draw(GameTime gameTime)
        {
            //Debug.WriteLine("Call Override");
            if (!visible) return;
            GameStateManagementGame._spriteBatch.Draw(sprite.texture, hitBox, null, Color.White, 0.0f, new Vector2(0,0), SpriteEffects.None, 0.5f);
            if (GameStateManagementGame.mode == GameMode.DEBUG) drawDebugHitBox();
            //base.Draw(gameTime);
        }



    }

}