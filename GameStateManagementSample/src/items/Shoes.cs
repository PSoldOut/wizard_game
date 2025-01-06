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
    class Shoes : Item
    {
        float speed = 0.07f;
        ParticleSystem particleSystem;

        public Shoes(int x, int y) : base(new Vector2(x, y), 35, 30, "shoes", false)
        {
            effectSound = AssetManager.GetSoundInstance("liveSound");
            effectSound.Volume = GameStateManagementGame.GetSoundVolume();
            particleSystem = new ParticleSystem(40);
            LoadSprite(1,1,0.05f);
        }


        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            particleSystem.AddMagicEffect(new Vector2(position.X+width/2, position.Y+height/2), 1, Color.AliceBlue);
            particleSystem.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            particleSystem.Draw();
        }

        public override void Effect()
        {
            effectSound.Play();
            Player.Get().speed += speed;
            GameplayScreen.map.GetActiveRoom().items.Remove(this);
        }
    }

}