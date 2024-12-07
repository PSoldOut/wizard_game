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

namespace wizard_game
{
    class HealthPotion : Item
    {
        int health = 15; 
        SoundEffect effectSound;
        
        public HealthPotion(int x, int y) : base(new Vector2(x, y), 5, 5, "healthPotion", false)
        {
            effectSound = GameStateManagementGame.Get().Content.Load<SoundEffect>("liveSound");
            this.sprite = new Sprite(GameStateManagementGame.Get().Content.Load<Texture2D>(spritename), 1, 1, 0.1f);
        }

        public override void Effect()
        {
            effectSound.Play();
            if (Player.Get().health <= Player.MAX_HEALTH - health)
                Player.Get().health+=health;
            
            GameplayScreen.items.Remove(this);
        }
    }

}