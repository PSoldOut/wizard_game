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
using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using wizard_game;
using Microsoft.Xna.Framework.Media;

#endregion Using Statements

namespace GameStateManagement
{
    /// <summary>
    /// Sample showing how to manage different game states, with transitions
    /// between menu screens, a loading screen, the game itself, and a pause
    /// menu. This main game class is extremely simple: all the interesting
    /// stuff happens in the ScreenManager component.
    /// </summary>
    public class GameStateManagementGame : Game
    {


        public static int musicVolume = 0;
        public static int soundVolume = 3;
        public static int masterVolume = 1;

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
        private Texture2D image;
        private Rectangle screenRectangle;
         
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
            if (musicVolume == 0 || masterVolume == 0) MediaPlayer.Volume = 0;
            else MediaPlayer.Volume = musicVolume/8.0f + masterVolume/8.0f;
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



        public static float GetSoundVolume()
        {
            if (soundVolume == 0 || masterVolume == 0) return 0.0f;
            else return soundVolume/8.0f + masterVolume/8.0f;
        }

        public static float GetMusicVolume()
        {
            if (musicVolume == 0 || masterVolume == 0) return 0.0f;
            else return musicVolume/8.0f + masterVolume/8.0f;
        }
       

        
        
    }


    

    

    /// <summary>
    /// The main entry point for the application.
    /// </summary>
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