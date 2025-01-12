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
    public class Timer
    {
        double secondsRemaining;
        double seconds;
        public bool isRunning;
        public List<GameEntity> listeners;

        

        public Timer(double seconds)
        {
            listeners = new List<GameEntity>();
            this.seconds = seconds;
            this.secondsRemaining = seconds;
            isRunning = false;
            
        }

        public Timer(double seconds, GameEntity listener)
        {
            listeners = new List<GameEntity>();
            listeners.Add(listener);
            this.seconds = seconds;
            this.secondsRemaining = seconds;
            isRunning = false;
            
        }


        public void addListener(GameEntity l)
        {
            listeners.Add(l);
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
            foreach(GameEntity g in listeners)
            {
                if (g!= null) g.TimerCallback(this);
            }
            
        }

        public double getSecondsRemaining()
        {
            return secondsRemaining;
        }

        public void Update(GameTime gameTime)
        {
            if (isRunning) secondsRemaining -=gameTime.ElapsedGameTime.TotalSeconds;
            if (secondsRemaining <= 0) stop();
        }


    }

}