using System;
using GameStateManagement;
using Manager;
using Microsoft.Xna.Framework;

namespace wizard_game
{

    class Plant : Item
    {

        public Plant(int x, int y) : base(new Vector2(x, y), 27, 27, "plant")
        {
            effectSound = AssetManager.GetSoundInstance("inventory_sound_effects/ring_inventory");
            effectSound.Volume = GameStateManagementGame.soundSettings.GetVolumeForSound();
            LoadSprite(1, 1, 1.4f);
            sprite.layerDepth = 0.7f;
            width = sprite.frameWidth;
            height = sprite.frameHeight;
            blinkEffect = false;

        }

        public void SetPos(Map map)
        {
            position = GenerateRandomPosition(map);
        }

        public override void Effect()
        {
            
        }



        

    }
}