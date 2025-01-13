using System;
using GameStateManagement;
using Manager;
using Microsoft.Xna.Framework;

namespace wizard_game
{

    class Gold : Item
    {

        public Gold(int x, int y) : base(new Vector2(x, y), 27, 27, "gold")
        {
            effectSound = AssetManager.GetSoundInstance("inventory_sound_effects/ring_inventory");
            effectSound.Volume = GameStateManagementGame.soundSettings.GetVolumeForSound();
            LoadSprite(1, 1, 0.4f);
            width = sprite.frameWidth;
            height = sprite.frameHeight;

        }

        public void SetPos(Map map)
        {
            position = GenerateRandomPosition(map);
        }

        public override void Effect()
        {
            effectSound.Play();
            // Player.Get().coins++;
            GameplayScreen.map.GetActiveRoom().items.Remove(this);
        }



        

    }
}