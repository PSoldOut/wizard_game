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
using Microsoft.Xna.Framework.Audio;
using Manager;

namespace wizard_game
{

    public abstract class GameEntity
    {

        public Sprite sprite;
        protected Vector2 direction;
        public Vector2 position;
        protected float rotation;
        public int width;
        public int height;
        public Rectangle hitBox;
        protected string spritename;
        bool hasCollision;

        private bool drawHitBox = false;
        public int lineWidth = 2;
        public Color hitboxColor = Color.Purple;
        public Texture2D image_hitbox;

        public Rectangle damageArea;
        public Vector2 damageOffset;
        public int damageDistance = 30;

        SoundEffectInstance openGateSound;


        public GameEntity(Vector2 position, int width, int height, string spritename = null, bool hasCollision = true)
        {

            openGateSound = AssetManager.GetSoundInstance("Horror_Sound_Library/Gate_Open_00");
            openGateSound.Volume = GameStateManagementGame.GetSoundVolume();

            //Debug.WriteLine("init new game entity" + position.ToString() + " w" +width + " h "+height+" name"+spritename);
            this.position = position;
            this.width = width;
            this.height = height;
            this.spritename = spritename;

            this.hasCollision = hasCollision;
            hitBox = new Rectangle((int)position.X, (int)position.Y, width, height);
            image_hitbox = new Texture2D(GameStateManagementGame.Get().GraphicsDevice, 1, 1);
            image_hitbox.SetData(new Color[] { Color.Red });

            damageOffset = new Vector2(damageDistance, 0);
            damageArea = new Rectangle((int)(position.X+damageOffset.X), (int)(position.Y+damageOffset.Y), 27, 45);
        }
        public void LoadSprite(int hFrames=1, int vFrames=1, float scale=1,bool isAnimated=false)
        {
            if(spritename!="" && spritename !=null)
            {
                sprite = new Sprite(AssetManager.GetTexture(spritename), hFrames, vFrames, scale,isAnimated);
            }
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




        protected bool DetacteCollison()
        {
            Door spawnDoor = GameplayScreen.map.DetacteCollisonDoor(hitBox);
            if (spawnDoor != null)
            {
                position = new Vector2(spawnDoor.GetSpawnPoint().X, spawnDoor.GetSpawnPoint().Y);
                hitBox.X = (int)position.X;
                hitBox.Y = (int)position.Y;
                //openGateSound.Play();
                return false;
            }
            Color[] data = sprite.GetCurrentColorData();
            return GameplayScreen.map.DetacteCollison(hitBox, data, false);
        }



        public void DetacteCollisonX(Vector2 posOld)
        {
            hitBox.X = (int)position.X;
            if (DetacteCollison())
            {
                position.X = posOld.X;
            }
            hitBox.X = (int)position.X;
        }



        public void DetacteCollisonY(Vector2 posOld)
        {
            hitBox.Y = (int)position.Y;
            if (DetacteCollison())
            {
                position.Y = posOld.Y;
            }
            hitBox.Y = (int)position.Y;
        }



        public virtual void TimerCallback(Timer timer){}
    }

}