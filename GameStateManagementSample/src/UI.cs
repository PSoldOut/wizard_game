
using GameStateManagement;
using Manager;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace wizard_game
{
    class UI
    {
        private static UI instance;
        Sprite heartSprite;
        Sprite goldSprite;
        Sprite tabmenuSprite;
        Sprite bowSprite;
        Sprite swordSprite;
        SpriteFont spriteFont;

        float upperOffsetY = 21;
        float bottomOffsetY = 40;
        public bool isInTabmenu;
        float tabmenuSpriteScale = 3.5f;
        public bool won;

        private UI()
        {
            won = false;
            heartSprite = new Sprite(AssetManager.GetTexture("heart"), 1, 1, 1, false);
            heartSprite.layerDepth = 0.2f;

            bowSprite = new Sprite(AssetManager.GetTexture("bow"), 6, 4, 1, false);
            bowSprite.layerDepth = 0.2f;
            bowSprite.currentFrame = 1;

            swordSprite = new Sprite(AssetManager.GetTexture("sword"), 1, 1, 1, false);
            swordSprite.layerDepth = 0.2f;
            swordSprite.SetScale(0.25f);

            goldSprite = new Sprite(AssetManager.GetTexture("gold"), 1, 1, 1, false);
            goldSprite.layerDepth = 0.3f;
            spriteFont = AssetManager.GetFont("Arial");
            tabmenuSprite = new Sprite(AssetManager.GetTexture("panel_brown"), 1, 1, tabmenuSpriteScale, false);
            tabmenuSprite.layerDepth = 0.3f;
    
            tabmenuSprite.color = new Color(255,255,255,200);
            isInTabmenu = false;
        }


        public static UI Get()
        {
            if (instance == null) instance = new UI();
            return instance;
        }


        public static void reset()
        {
            instance = new UI();
        }


        public void toggleTabmenu()
        {
            isInTabmenu = !isInTabmenu;
        }



        public void Update(GameTime gameTime)
        {
            
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
            if (isInTabmenu)
            {
                Vector2 posPanel = new Vector2(GameStateManagementGame.Get().graphics.PreferredBackBufferWidth/2-(int)(tabmenuSprite.texture.Width*tabmenuSpriteScale/2), GameStateManagementGame.Get().graphics.PreferredBackBufferHeight/2-(int)(tabmenuSprite.texture.Height*tabmenuSpriteScale/2)-70);
                tabmenuSprite.Draw((int)posPanel.X, (int)posPanel.Y);
                GameStateManagementGame._spriteBatch.DrawString(spriteFont, "LP: " + Player.Get().lp, new Vector2(posPanel.X + tabmenuSprite.texture.Width * tabmenuSpriteScale/2 - 15, posPanel.Y+tabmenuSprite.texture.Height * tabmenuSpriteScale/8), Color.Wheat, 0, new Vector2(0,0), 1.0f, SpriteEffects.None, 0.0f);

                heartSprite.SetScale(2);
                heartSprite.Draw((int)(posPanel.X + tabmenuSprite.texture.Width * tabmenuSpriteScale * (1.0f/3.0f) - heartSprite.texture.Width*heartSprite.scaleX -15), (int)(posPanel.Y+tabmenuSprite.texture.Height * tabmenuSpriteScale/3));
                heartSprite.SetScale(1);

                bowSprite.Draw((int)(posPanel.X + tabmenuSprite.texture.Width * tabmenuSpriteScale * (2.0f/3.0f) - bowSprite.texture.Width*bowSprite.scaleX/6)-15, (int)(posPanel.Y+tabmenuSprite.texture.Height * tabmenuSpriteScale/3 - 10));

                swordSprite.Draw((int)(posPanel.X + tabmenuSprite.texture.Width * tabmenuSpriteScale * (3.0f/3.0f) - swordSprite.texture.Width*swordSprite.scaleX - 30), (int)(posPanel.Y+tabmenuSprite.texture.Height * tabmenuSpriteScale/3));
                GameStateManagementGame._spriteBatch.DrawString(spriteFont, "MaxHP:"+Player.Get().extraMaxHealth.ToString(), new Vector2(posPanel.X + tabmenuSprite.texture.Width * tabmenuSpriteScale * (1.0f/3.0f) - 90, posPanel.Y + +tabmenuSprite.texture.Height * tabmenuSpriteScale/1.4f), Color.Wheat, 0, new Vector2(0,0), 1.0f, SpriteEffects.None, 0.0f);
                GameStateManagementGame._spriteBatch.DrawString(spriteFont, "Ranged:"+Player.Get().rangedExtraDamage.ToString(), new Vector2(posPanel.X + tabmenuSprite.texture.Width * tabmenuSpriteScale * (2.0f/3.0f) - 90, posPanel.Y + +tabmenuSprite.texture.Height * tabmenuSpriteScale/1.4f), Color.Wheat, 0, new Vector2(0,0), 1.0f, SpriteEffects.None, 0.0f);
                GameStateManagementGame._spriteBatch.DrawString(spriteFont, "Meele:"+Player.Get().meeleExtraDamage.ToString(), new Vector2(posPanel.X + tabmenuSprite.texture.Width * tabmenuSpriteScale * (3.0f/3.0f) - 100, posPanel.Y + +tabmenuSprite.texture.Height * tabmenuSpriteScale/1.4f), Color.Wheat, 0, new Vector2(0,0), 1.0f, SpriteEffects.None, 0.0f);
            }

            if (won)
            {
                GameStateManagementGame._spriteBatch.DrawString(AssetManager.GetFont("Arial"), "Du hast gewonnen!", new Vector2(GameStateManagementGame.Get().graphics.PreferredBackBufferWidth/2-20,GameStateManagementGame.Get().graphics.PreferredBackBufferHeight/2), Color.Wheat);
            }

        }
    }

}