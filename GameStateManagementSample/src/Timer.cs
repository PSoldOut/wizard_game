using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.Diagnostics;
using GameStateManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace wizard_game
{
    public class Timer : GameComponent
    {
        double secondsRemaining;
        double seconds;
        bool isRunning;

        public Timer(double seconds) : base(GameStateManagementGame.Get())
        {   
            this.seconds = seconds;
            this.secondsRemaining = seconds;
            isRunning = false;
        }

        public void start()
        {
            isRunning = true;
        }

        public void pause()
        {
            isRunning = false;
        }

        public void stop()
        {
            isRunning = false;
            secondsRemaining = seconds;
        }

        public double getSecondsRemaining()
        {
            return secondsRemaining;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            secondsRemaining-=gameTime.ElapsedGameTime.TotalSeconds;
            if (secondsRemaining <= 0) stop();
        }


    }

}