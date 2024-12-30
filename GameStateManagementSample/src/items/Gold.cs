using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using GameStateManagement;
using Manager;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using wizard_game;

namespace  wizard_game
{

    class Gold : Item
    {

        public Gold(int x, int y) : base(new Vector2(x, y), 27, 27, "gold", false)
        {
            effectSound = AssetManager.GetSoundInstance("inventory_sound_effects/ring_inventory");
            effectSound.Volume = GameStateManagementGame.GetSoundVolume();
            LoadSprite(1,1,0.4f);
            width = sprite.frameWidth;
            height = sprite.frameHeight;
        }

        public override void Effect()
        {
           effectSound.Play();
          // Player.Get().coins++;
           GameplayScreen.items.Remove(this);
        }


    }
}