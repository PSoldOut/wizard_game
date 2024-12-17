#region File Description

//-----------------------------------------------------------------------------
// GameplayScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------

#endregion File Description

#region Using Statements

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Threading;
using wizard_game;
using Microsoft.Xna.Framework.Media;

#endregion Using Statements

namespace GameStateManagement
{
    /// <summary>
    /// This screen implements the actual game logic. It is just a
    /// placeholder to get the idea across: you'll probably want to
    /// put some more interesting gameplay in here!
    /// </summary>
    internal class GameplayScreen : GameScreen
    {
        Song level1Song;
        public static Texture2D goldTexture;
        public static Random rand;
        public static Map map;


        bool mousePressed = false;
        private int fps;
        private SpriteFont spriteFont;


        private ContentManager content;
        private SpriteFont gameFont;

        public static Random random = new Random();

        private float pauseAlpha;

        static int goldCount = 5;

        public static List<Acteur> acteurs;
        public static List<Item> items;
        public static List<Projectile> projectiles;
        public Texture2D background;

        UI ui;


        public GameplayScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
            rand = new Random();

            ui = new UI();

            items = new List<Item>();
            acteurs = new List<Acteur>();
            projectiles = new List<Projectile>();
            map = new Map();
            Room room = map.GetActiveRoom();
            for (int i = 0; i < goldCount; i++)
            {
                Gold g = new Gold(rand.Next(GameStateManagementGame.Get().graphics.GraphicsDevice.Viewport.Height), rand.Next(GameStateManagementGame.Get().graphics.GraphicsDevice.Viewport.Height));
                room.setGamestateElement(g.position, Gamestate.GOLD);
                items.Add(g);
            }
            items.Add(new HealthPotion(rand.Next(GameStateManagementGame.Get().graphics.GraphicsDevice.Viewport.Height), rand.Next(GameStateManagementGame.Get().graphics.GraphicsDevice.Viewport.Height)));
            items.Add(new Sword(300, 100));
            items.Add(new Bow(200, 100));
            //---------------------------------------




            //map.Initialize();

            acteurs.Add(Player.Get());
            acteurs.Add(new Enemy_Guard(10, 10, map, EnemyType.GUARD, map.GetActiveRoom()));
            acteurs.Add(new Enemy_Knight(500,500, map, EnemyType.KNIGHT, map.GetActiveRoom()));
            acteurs.Add(new Enemy_Doubler(400,300, map, EnemyType.DOUBLER, map.GetActiveRoom(), 4));
            acteurs.Add(new Enemy_prisoner(400, 400, map, EnemyType.PRISONER, map.GetActiveRoom()));


        }



        public override void LoadContent()
        {
            if (content == null)
            {
                content = new ContentManager(ScreenManager.Game.Services, "Content");
            }
            level1Song = content.Load<Song>("level1Music");
            gameFont = content.Load<SpriteFont>("gamefont");
            goldTexture = content.Load<Texture2D>("gold");
            spriteFont = content.Load<SpriteFont>("Arial");
            background = content.Load<Texture2D>("Floors/Brickwall5");

            // once the load has finished, we use ResetElapsedTime to tell the game's
            // timing mechanism that we have just finished a very long frame, and that
            // it should not try to catch up.
            MediaPlayer.Play(level1Song);
            MediaPlayer.Volume = GameStateManagementGame.GetMusicVolume();
            ScreenManager.Game.ResetElapsedTime();
        }




        public override void UnloadContent()
        {
            content.Unload();
        }




        /// <summary>
        /// Updates the state of the game. This method checks the GameScreen.IsActive
        /// property, so the game will stop updating when the pause menu is active,
        /// or if you tab away to a different application.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {

            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
            if (!otherScreenHasFocus)
            {
                if (gameTime.ElapsedGameTime.Milliseconds > 0)
                {
                    fps = 1000 / gameTime.ElapsedGameTime.Milliseconds;
                }

                if (Mouse.GetState().LeftButton == ButtonState.Released && mousePressed)
                {
                    mousePressed = false;

                }
                ui.Update(gameTime);
                //map.Update(gameTime);
                for (int i = 0; i < items.Count; i++) items[i].Update(gameTime);
                for (int i = 0; i < projectiles.Count; i++) projectiles[i].Update(gameTime);
                for (int i = 0; i < acteurs.Count; i++) acteurs[i].Update(gameTime);


            }

        }

        /// <summary>
        /// Lets the game respond to player input. Unlike the Update method,
        /// this will only be called when the gameplay screen is active.
        /// </summary>
        public override void HandleInput(InputState input)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            // Look up inputs for the active player profile.
            int playerIndex = (int)ControllingPlayer.Value;
            GamePadState gamePadState = input.CurrentGamePadStates[playerIndex];

            // The game pauses either if the user presses the pause button, or if
            // they unplug the active gamepad. This requires us to keep track of
            // whether a gamepad was ever plugged in, because we don't want to pause
            // on PC if they are playing with a keyboard and have no gamepad at all!
            bool gamePadDisconnected = !gamePadState.IsConnected &&
                                       input.GamePadWasConnected[playerIndex];

            if (input.IsPauseGame(ControllingPlayer) || gamePadDisconnected)
            {
                ScreenManager.AddScreen(new PauseMenuScreen(), ControllingPlayer);

            }

            Player.Get().HandleInput(input);
        }





        public override void Draw(GameTime gameTime)
        {
            if (!this.IsActive) // todo spiel vielleich im hintergrunt anzeigen.
            {
                return;
            }
            ScreenManager.GraphicsDevice.Clear(ClearOptions.Target, Color.CornflowerBlue, 0, 0);
            Rectangle screenRectangle = new Rectangle(0, 0, 1280, 720);
            GameStateManagementGame._spriteBatch.Draw(background, screenRectangle, null, Color.White, 0, new Vector2(0,0), SpriteEffects.None, 1.0f);
            GameStateManagementGame._spriteBatch.DrawString(spriteFont, "FPS: " + fps.ToString(), new Vector2(20, GameStateManagementGame.Get().graphics.PreferredBackBufferHeight-40), Color.Wheat);
            GameStateManagementGame._spriteBatch.DrawString(spriteFont, "Room: " + map.roomIndex.ToString(), new Vector2(120, GameStateManagementGame.Get().graphics.PreferredBackBufferHeight-40), Color.Wheat);

            for (int i = 0; i < items.Count; i++) items[i].Draw(gameTime);
            for (int i = 0; i < projectiles.Count; i++) projectiles[i].Draw(gameTime);
            for (int i = 0; i < acteurs.Count; i++) acteurs[i].Draw(gameTime);
            map.Draw(gameTime);
            ui.Draw(gameTime);
            base.Draw(gameTime);
        }



        protected bool IsAnyInput()
        {
            MouseState ms = Mouse.GetState();
            if (Keyboard.GetState().GetPressedKeys().Length > 0 ||
                ms.LeftButton == ButtonState.Pressed ||
                ms.RightButton == ButtonState.Pressed ||
                ms.MiddleButton == ButtonState.Pressed ||
                ms.MiddleButton == ButtonState.Released ||
                ms.RightButton == ButtonState.Released ||
                ms.LeftButton == ButtonState.Released) return true;
            return false;
        }

    }
}