using System;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using GameStateManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace wizard_game
{
    public class Door : GameEntity
    {



        public Room room;// Raum wo die tür hinführt
        public Door linkedDoor;
        public SpriteBatch _spriteBatch;
        public SiteEnum _site;
        int W_Height;
        int W_Width;
        public enum SiteEnum : int
        {

            Left = 0,
            Top = 1,
            Bottom = 2,
            Right = 3,

        }
        GameStateManagementGame gameInstance = GameStateManagementGame.Get();
        public Door(Point size, Color? color = null) : base(Vector2.Zero, size.X, size.Y, "")
        {

            //_spriteFont = gameInstance.Content.Load<SpriteFont>("Arial");
            W_Width = gameInstance.GraphicsDevice.Viewport.Width;
            W_Height = gameInstance.GraphicsDevice.Viewport.Height;
        }
        public Vector2 GetSpawnPoint()
        {
            switch (Site)
            {

                case SiteEnum.Left:
                    return position + new Vector2(width, 0);
                case SiteEnum.Top:
                    return position + new Vector2(0, height);
                case SiteEnum.Right:
                    return position + new Vector2(-width, 0);
                case SiteEnum.Bottom:
                    return position + new Vector2(0, -height);

                default:
                    return Vector2.Zero;
            }
        }
        public SiteEnum GetOppositeSite(SiteEnum? _site = null)
        {
            if (_site == null)
            {
                _site = Site;
            }
           //Debug.WriteLine("get o" + _site);
            switch (_site)
            {
                case SiteEnum.Top:
                    return SiteEnum.Bottom;

                case SiteEnum.Bottom:
                    return SiteEnum.Top;

                case SiteEnum.Left:
                    return SiteEnum.Right;

                case SiteEnum.Right:
                    return SiteEnum.Left;
                default:
                    return SiteEnum.Left;//should never happen

            }
        }

        public Vector2 Pos
        {
            get { return position; }
            set
            {

                this.position = value;
                hitBox.X = (int)value.X;
                hitBox.Y = (int)value.Y;
            }
        }
        public Vector2 Size
        {
            get { return new Vector2(width, height); }
            set
            {

                width = (int)value.X;
                height = (int)value.Y;
                hitBox.Size = value.ToPoint();
            }
        }
        public Point Center
        {
            get { return this.hitBox.Center; }

        }


        public void SetRoom(Room _room)
        {
            room = _room;
        }
        public void SetLinkedDoor(Door door)
        {
            linkedDoor = door;
        }

        public bool DetacteCollison(Rectangle _hitbox)
        {
            return hitBox.Intersects(_hitbox);

        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            //Debug.WriteLine("Call Override");

            float angle = 0; //90 Grad
            SpriteEffects effect = SpriteEffects.None;
            Vector2 origin = Vector2.Zero;
            //Pos = new Vector2(600, 600);
            Rectangle source = new Rectangle(0, 0, 80, 88); ;
            switch (Site)
            {
                case SiteEnum.Bottom:

                    effect = SpriteEffects.FlipVertically;
                    break;

                case SiteEnum.Left:
                    //angle = -(float)Math.PI / 2.0f; //-90 Grad
                    //effect = SpriteEffects.FlipHorizontally;
                    //origin = new Vector2(80 / 2, 95 / 2);//ist gedreht
                    source = new Rectangle(0, 0, 88, 80);
                    break;

                case SiteEnum.Right:
                    source = new Rectangle(0, 0, 88, 80);
                    effect = SpriteEffects.FlipHorizontally;
                    break;
            }
          //  Debug.WriteLine("door " + hitBox + angle + effect + origin);
            GameStateManagementGame._spriteBatch.Draw(sprite.texture,
            hitBox,
            source,
            Color.White,
            angle,
            origin,
            effect,
            1);

            if (GameStateManagementGame.mode == GameMode.DEBUG) drawDebugHitBox();
            //_spriteBatch.DrawString(_spriteFont, "TO " + linkedDoor.room.index, hitBox.Center.ToVector2(), Color.Black);
            //base.Draw(gameTime);
        }
        public SiteEnum Site
        {
            get { return _site; }
            set
            {
              //  Debug.WriteLine("set door to " + value);
                switch (value)
                {
                    case SiteEnum.Left: //left seite
                        spritename="Obstacles/door_left";
                        LoadSprite();
                        Pos = new Vector2(0, W_Height / 2);
                        Size = new Vector2(Size.Y, Size.X);//Rotate x and y
                        break;
                    case SiteEnum.Top: //top
                        Pos = new Vector2(W_Width / 2, 10);
                        spritename="Obstacles/door_top";
                        LoadSprite();
                        break;
                    case SiteEnum.Bottom: //bottom
                        Pos = new Vector2(W_Width / 2, W_Height - width - 20);
                        spritename="Obstacles/door_top";
                        LoadSprite();
                        break;
                    case SiteEnum.Right: //right seite
                        spritename="Obstacles/door_left";
                        LoadSprite();
                        Pos = new Vector2(W_Width - sprite.texture.Width, W_Height / 2);
                        Size = new Vector2(Size.Y, Size.X);//Rotate x and y

                        break;


                }
                _site = value;
            }
        }

    }
}