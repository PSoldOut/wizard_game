#region File Description

//-----------------------------------------------------------------------------
// OptionsMenuScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------

#endregion File Description

using Microsoft.Xna.Framework.Media;

namespace GameStateManagement
{
    /// <summary>
    /// The options screen is brought up over the top of the main menu
    /// screen, and gives the user a chance to configure the game
    /// in various hopefully useful ways.
    /// </summary>
    internal class SoundOptionsMenuScreen : MenuScreen
    {
        #region Fields

        private MenuEntry musicVolumeEntry;
        private MenuEntry soundVolumeEntry;
        private MenuEntry masterVolumeEntry;
        

        #endregion Fields

        #region Initialization

        /// <summary>
        /// Constructor.
        /// </summary>
        public SoundOptionsMenuScreen()
            : base("Options")
        {
            // Create our menu entries.
            soundVolumeEntry = new MenuEntry("");
            musicVolumeEntry = new MenuEntry("");
            masterVolumeEntry = new MenuEntry("");

            SetMenuEntryText();

            MenuEntry back = new MenuEntry("Back");

            // Hook up menu event handlers.
            
            soundVolumeEntry.Selected += soundVolumeMenuEntrySelected;
            musicVolumeEntry.Selected += musicVolumeMenuEntrySelected;
            masterVolumeEntry.Selected += masterVolumeMenuEntrySelected;
            back.Selected += OnCancel;

            // Add entries to the menu.
            MenuEntries.Add(soundVolumeEntry);
            MenuEntries.Add(masterVolumeEntry);
            MenuEntries.Add(musicVolumeEntry);
            MenuEntries.Add(back);
        }

        /// <summary>
        /// Fills in the latest values for the options screen menu text.
        /// </summary>
        private void SetMenuEntryText()
        {
            soundVolumeEntry.Text = "Sound  Volume:  " + GameStateManagementGame.soundVolume;
            musicVolumeEntry.Text = "Music  Volume:  " + GameStateManagementGame.musicVolume;
            masterVolumeEntry.Text = "Master  Volume:  " + GameStateManagementGame.masterVolume;
        }

        #endregion Initialization

        #region Handle Input

        /// <summary>
        /// Event handler for when the Ungulate menu entry is selected.
        /// </summary>
        

        /// <summary>
        /// Event handler for when the Elf menu entry is selected.
        /// </summary>
        /// 

        private void musicVolumeMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            GameStateManagementGame.musicVolume = (GameStateManagementGame.musicVolume + 1) % 4;
            if (GameStateManagementGame.musicVolume == 0 || GameStateManagementGame.masterVolume == 0) MediaPlayer.Volume = 0;
            else MediaPlayer.Volume = GameStateManagementGame.musicVolume/8.0f + GameStateManagementGame.masterVolume/8.0f;
            SetMenuEntryText();
        }

        private void soundVolumeMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            GameStateManagementGame.soundVolume = (GameStateManagementGame.soundVolume + 1) % 4;
            SetMenuEntryText();
        }

        private void masterVolumeMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            GameStateManagementGame.masterVolume = (GameStateManagementGame.masterVolume+1) % 4;
            if (GameStateManagementGame.musicVolume == 0 || GameStateManagementGame.masterVolume == 0) MediaPlayer.Volume = 0;
            else MediaPlayer.Volume = GameStateManagementGame.musicVolume/8.0f + GameStateManagementGame.masterVolume/8.0f;
            SetMenuEntryText();
        }

        private void ElfMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            SetMenuEntryText();
        }

        #endregion Handle Input
    }
}