using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    class UI
    {
        private static UI instance;
        Sprite heartSprite;
        Sprite goldSprite;
        SpriteFont spriteFont;

        float upperOffsetY = 21;
        float bottomOffsetY = 40;
        bool isInTabmenu;

        private UI()
        {
            heartSprite = new Sprite(AssetManager.GetTexture("heart"), 1, 1, 1, false);
            heartSprite.layerDepth = 0.3f;

            goldSprite = new Sprite(AssetManager.GetTexture("gold"), 1, 1, 1, false);
            goldSprite.layerDepth = 0.3f;
            spriteFont = AssetManager.GetFont("Arial");
            isInTabmenu = false;
        }


        public static UI Get()
        {
            if (instance == null) instance = new UI();
            return instance;
        }


        public void toggleTabmenu()
        {
            isInTabmenu = !isInTabmenu;
        }



        public void Update(GameTime gameTime)
        {
            if (isInTabmenu)
            {
                Console.WriteLine("in tabmenu");
            }
        }

        public void Draw(GameTime gameTime)
        {
            GameStateManagementGame gsmg = GameStateManagementGame.Get();
            for (int i = 0; i < Player.Get().health; i++)
            {
                heartSprite.Draw(15 + i * 45, 15);
            }
            GameStateManagementGame._spriteBatch.DrawString(spriteFont, "" + Player.Get().coins, new Vector2(gsmg.graphics.PreferredBackBufferWidth-90, upperOffsetY), Color.Wheat);
            goldSprite.Draw(gsmg.graphics.PreferredBackBufferWidth-80, 0);
            GameStateManagementGame._spriteBatch.DrawString(spriteFont, "Rank: " + Player.Get().rank + "          exp: " + Player.Get().exp + "/" + Player.current_rank_exp_needed, new Vector2(gsmg.graphics.PreferredBackBufferWidth/2 - 120, gsmg.graphics.PreferredBackBufferHeight-bottomOffsetY), Color.Wheat, 0, new Vector2(0,0), 1.0f, SpriteEffects.None, 0.0f);
            GameStateManagementGame._spriteBatch.DrawString(spriteFont, "FPS: " + GameplayScreen.fps.ToString(), new Vector2(20, gsmg.graphics.PreferredBackBufferHeight-bottomOffsetY), Color.Wheat, 0, new Vector2(0,0), 1.0f, SpriteEffects.None, 0.0f);
            GameStateManagementGame._spriteBatch.DrawString(spriteFont, "Room: " + GameplayScreen.map.roomIndex.ToString(), new Vector2(120, gsmg.graphics.PreferredBackBufferHeight-bottomOffsetY), Color.Wheat, 0, new Vector2(0,0), 1.0f, SpriteEffects.None, 0.0f);

        }
    }

}