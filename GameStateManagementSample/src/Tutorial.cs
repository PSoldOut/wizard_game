using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.Diagnostics;
using System.Runtime.Serialization;
using GameStateManagement;
using Manager;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace wizard_game
{
    class Tutorial
    {
        private static Tutorial instance;
        Sprite backgroundSprite;
        float backgroundSpriteScaleX = 4.1f;
        float backgroundSpriteScaleY = 2.5f;
        string[] quests = new string[6];
        int currentQuest = 0;
        Vector2 fontOffset = new Vector2(23,15);
        int currenWeaponCount = 0;
        Enemy spawnedEnemy;
        int currentEnemyHealth=0;
        Timer endTimer = new Timer(5);
        int curretActeurCount = 4;


        private Tutorial()
        {
            backgroundSprite = new Sprite(AssetManager.GetTexture("panel_brown"), 1, 1, backgroundSpriteScaleX, backgroundSpriteScaleY, false);
            backgroundSprite.layerDepth = 0.01f;
            backgroundSprite.color = new Color(255,255,255,100);
            quests[0] = "Benutze  die  Tasten  W,  A,  S,  D\num  dich  zu  bewegen.\nProbiere  es  aus!";
            quests[1] = "Sehr  gut!  Jetzt  oeffne  mit  TAB\ndas  Skillmenu.  Druecke  1,  2  oder  3\num  je  eine  Fertigkeit  zu  verbessern.\nBedenke,  dass  das  Lernpunkte  kostet.";
            quests[2] = "Genau  so!  Jetzt  sammle  eine\nrumliegende  Waffen  ein,  indem  du\nueber  die  Waffe  laeufst.";
            quests[3] = "Super!  Jetzt  waehle  die  Waffe  mit  1\noder  2  aus  und  greife  einen  Gegner  an.\nBenutze  dazu  die  Leertaste.";
            quests[4] = "Meisterhaft!  Jetzt  sammle  die  Schriftrolle\n  ein  und  waehle  sie  mit  3  aus.\nDann  schiesse  mit  der  Leertaste\n  einen  Feuerball  ab  und  toete\ndamit  einen  Gegner.\nMit  Q  und  E  kannst  du\nFeuerbaelle  lenken.";
            quests[5] = "Hervorragend!\nDu  hast  das  Tutorial  erfolgreich  beendet.";
        }

        public static Tutorial Get()
        {
            if(instance == null) instance = new Tutorial();
            return instance;
        }

        public static void reset()
        {
            instance = new Tutorial();
        }

        public void Update(GameTime gameTime)
        {
            if (currentQuest == 0 && Player.Get().currentSpeed > 0) currentQuest++;
            else if (currentQuest == 1 && UI.Get().isInTabmenu && (Player.Get().rangedExtraDamage > 0 || Player.Get().extraMaxHealth > 0 || Player.Get().meeleExtraDamage > 0))
                {
                    currentQuest++;
                    GameplayScreen.SpawnItem(new Sword(400,400));
                    GameplayScreen.SpawnItem(new Bow(400,500));
                    currenWeaponCount = Player.Get().weapons.Count;
                }
            else if (currentQuest == 2 && Player.Get().weapons.Count > currenWeaponCount)
            {
                currentQuest++;
                spawnedEnemy = new Enemy_prisoner(400,400, Player.Get().map, EnemyType.PRISONER, Player.Get().map.GetActiveRoom());
                GameplayScreen.SpawnActeur(spawnedEnemy);
                currentEnemyHealth = spawnedEnemy.health;
            }
            else if (currentQuest == 3 && spawnedEnemy.health < currentEnemyHealth)
            {
                currentQuest++;
                spawnedEnemy = new Enemy_prisoner(400,400, Player.Get().map, EnemyType.PRISONER, Player.Get().map.GetActiveRoom());
                GameplayScreen.SpawnActeur(spawnedEnemy);
                curretActeurCount = GameplayScreen.acteurs.Count;
                GameplayScreen.SpawnItem(new Role(400,600));
            }

            else if (currentQuest == 4 && curretActeurCount > GameplayScreen.acteurs.Count)
            {   
                if (Player.Get().equippedWeapon is Role)
                {
                    currentQuest++;
                    endTimer.start();
                }
                else
                {
                    spawnedEnemy = new Enemy_prisoner(400,400, Player.Get().map, EnemyType.PRISONER, Player.Get().map.GetActiveRoom());
                    GameplayScreen.SpawnActeur(spawnedEnemy);
                    curretActeurCount = GameplayScreen.acteurs.Count;
                }
                
            }
            
            else if (!endTimer.isRunning && currentQuest == 5) 
            {
                ScreenManager screenManager = GameStateManagementGame.Get().screenManager;
                for (int i = screenManager.screens.Count-1; i >= 0; i--)
                {
                    screenManager.screens.Remove(screenManager.screens[i]);
                }
                screenManager.AddScreen(new BackgroundScreen(), null);
                screenManager.AddScreen(new MainMenuScreen(), null);
                Player.Get().reset();
            }

            endTimer.Update(gameTime);
            
        }

        public void Draw(GameTime gameTime)
        {
            GameStateManagementGame gsmg = GameStateManagementGame.Get();
            Vector2 posPanel = new Vector2(GameStateManagementGame.Get().graphics.PreferredBackBufferWidth/2-(int)(backgroundSprite.texture.Width*backgroundSpriteScaleX/2), gsmg.graphics.PreferredBackBufferHeight-260);
            backgroundSprite.Draw((int)posPanel.X, (int)posPanel.Y);
            GameStateManagementGame._spriteBatch.DrawString(AssetManager.GetFont("Arial"), quests[currentQuest], posPanel+fontOffset, Color.Wheat, 0, new Vector2(0,0), 1, SpriteEffects.None, 0.0f);
        }
    }
}