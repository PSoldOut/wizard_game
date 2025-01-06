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

namespace wizard_game
{
    class HealthPotion : Item
    {
        int health = 1;

        public HealthPotion(int x, int y) : base(new Vector2(x, y), 25, 30, "healthPotion", false)
        {
            effectSound = AssetManager.GetSoundInstance("liveSound");
            effectSound.Volume = GameStateManagementGame.GetSoundVolume();
            LoadSprite(1,1,0.1f);

        }

        public override void Effect()
        {
            effectSound.Play();
            if (Player.Get().health <= Player.PLAYER_MAX_HEALTH - health + Player.Get().extraMaxHealth)
                Player.Get().health+=health;

            GameplayScreen.map.GetActiveRoom().items.Remove(this);
        }
    }

}