using System;
using Microsoft.Xna.Framework;
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
        private float blink = 0f;
        public bool blinkEffect;

        public Item(Vector2 position, int width, int height, string spriteName) : base(position, width, height, spriteName)
        {
            effectSound = AssetManager.GetSoundInstance("inventory_sound_effects/leather_inventory");
            effectSound.Volume = GameStateManagementGame.soundSettings.GetVolumeForSound();
            this.area = new Rectangle((int)position.X, (int)position.Y, 50, 50);
            state = State.ON_FLOOR;
            blinkEffect = true;
        }


        public override void RefreshVolume(float volumeForSound, float volumeForMusic)
        {
            base.RefreshVolume(volumeForSound, volumeForMusic);
            effectSound.Volume = volumeForSound;
        }




        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            area.X = (int)position.X;
            area.Y = (int)position.Y;

            if (this.state == State.ON_FLOOR && blinkEffect)
            {
                if (blink >= 1000*Math.PI) blink = 0;
                blink += 0.05f;
                sprite.color = new Color((int)((Math.Sin(blink) + 1.5f) / 2 * 255), (int)((Math.Sin(blink) + 1.5f) / 2 * 255), (int)((Math.Sin(blink) + 1.5f) / 2 * 255));
            }
        }


        public virtual void Effect(){}


    }

}