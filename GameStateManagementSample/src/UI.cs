using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    class UI
    {
        Sprite heartSprite;
        Sprite goldSprite;
        SpriteFont spriteFont;

        public UI()
        {
            heartSprite = new Sprite(GameStateManagementGame.Get().Content.Load<Texture2D>("heart"), 1, 1, 1, false);
            heartSprite.layerDepth = 0.3f;
            goldSprite = new Sprite(GameStateManagementGame.Get().Content.Load<Texture2D>("gold"), 1, 1, 1, false);
            goldSprite.layerDepth = 0.3f;
            spriteFont = GameStateManagementGame.Get().Content.Load<SpriteFont>("Arial");
        }
        


        public void Update(GameTime gameTime)
        {

        }

        public void Draw(GameTime gameTime)
        {
            for (int i = 0; i < Player.Get().health; i++)
            {
                heartSprite.Draw(15 + i * 45, 15);
            }
            
            GameStateManagementGame._spriteBatch.DrawString(spriteFont, "" + Player.Get().coins, new Vector2(GameStateManagementGame.Get().graphics.PreferredBackBufferWidth-90, 21), Color.Wheat);

            goldSprite.Draw(GameStateManagementGame.Get().graphics.PreferredBackBufferWidth-80, 0);

        }
    }

}