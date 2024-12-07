using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using GameStateManagement;
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
        SoundEffect effectSound;

        public Gold(int x, int y) : base(new Vector2(x, y), 27, 27, "gold", false)
        {
            effectSound = GameStateManagementGame.Get().Content.Load<SoundEffect>("inventory_sound_effects/ring_inventory");
            sprite = new Sprite(GameStateManagementGame.Get().Content.Load<Texture2D>(spritename), 1, 1, 0.4f);
            width = sprite.frameWidth;
            height = sprite.frameHeight;
        }

        public override void Effect()
        {
           effectSound.Play();
           Player.Get().coins++;
           GameplayScreen.items.Remove(this);
        }

        
    }
}