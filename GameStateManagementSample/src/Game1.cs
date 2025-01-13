#region File Description

//-----------------------------------------------------------------------------
// Game.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------

#endregion File Description

#region Using Statements

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using wizard_game;
using Microsoft.Xna.Framework.Media;
using System.Collections.Generic;
using wizard_game;
#endregion Using Statements

namespace GameStateManagement
{
    
    public class GameStateManagementGame : Game
    {


        public static SoundSettings soundSettings = new SoundSettings();
        
        public static GameMode mode = GameMode.DEBUG;

        public static Timer testTimer = new Timer(3);

        Song mainMenuSong;
        public struct InputState
        {
            public KeyboardState keyboadrState;
            public MouseState mouseState;
        }

        public readonly GraphicsDeviceManager graphics;
        public readonly ScreenManager screenManager;

        private static GameStateManagementGame instance;
        public static SpriteBatch _spriteBatch;

        // By preloading any assets used by UI rendering, we avoid framerate glitches
        // when they suddenly need to be loaded in the middle of a menu transition.
        private static readonly string[] preloadAssets =
        {
            "gradient",
        };



        /// <summary>
        /// The main game constructor.
        /// </summary>
        private GameStateManagementGame()
        {
            instance = this;
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            graphics = new GraphicsDeviceManager(this);
            // Create the screen manager component.
            screenManager = new ScreenManager(this);
            Components.Add(screenManager);

            // Activate the first screens.
            screenManager.AddScreen(new BackgroundScreen(), null);
            screenManager.AddScreen(new MainMenuScreen(), null);
        }

        //GameStateManagementGame is a singleton. you always get the instance of Player with Get()
        public static GameStateManagementGame Get()
        {
            if (instance == null) instance = new GameStateManagementGame();
            return instance;
        }

        protected override void Initialize()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;
            graphics.ApplyChanges();
            base.Initialize();
            if (soundSettings.GetMusicVolume() == 0 || soundSettings.GetMasterVolume() == 0) MediaPlayer.Volume = 0;
            else MediaPlayer.Volume = soundSettings.GetMusicVolume()/8.0f + soundSettings.GetMasterVolume()/8.0f;
            MediaPlayer.Play(mainMenuSong);
        }




        protected override void LoadContent()
        {
            mainMenuSong = Content.Load<Song>("mainMenuMusic");
            foreach (string asset in preloadAssets)
            {
                Content.Load<object>(asset);
            }
            base.LoadContent();
        }






        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.Black);
            _spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
            base.Draw(gameTime);
            _spriteBatch.End();




        }





    }






    internal static class Program
    {
        private static void Main()
        {
            using (GameStateManagementGame game = GameStateManagementGame.Get())
            {
                game.Run();
            }
        }
    }


}