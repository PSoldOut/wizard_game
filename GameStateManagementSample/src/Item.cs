using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using GameStateManagement;
using Manager;

namespace wizard_game
{

    public abstract class Item : GameEntity
    {
        public enum State
        {
            ON_FLOOR, IN_INVENTORY, EQUIPPED
        }

        public Rectangle area;
        public State state;
        protected SoundEffectInstance effectSound;

        public Item(Vector2 position, int width, int height, string spriteName, bool hasCollision) : base(position, width, height, spriteName, hasCollision)
        {
            effectSound = AssetManager.GetSoundInstance("inventory_sound_effects/leather_inventory");
            this.area = new Rectangle((int)position.X, (int)position.Y, 50, 50);
            state = State.ON_FLOOR;
        }




        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            area.X = (int)position.X;
            area.Y = (int)position.Y;
        } 


        public virtual void Effect(){}


    }

}