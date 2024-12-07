using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;

using GameStateManagement;

namespace wizard_game
{
    public abstract class Acteur : GameEntity
    {
        protected SoundEffect dieSound;
        public Acteur(Vector2 position, int width, int height, string spriteName, bool hasCollision) : base(position, width, height, spriteName, hasCollision)
        {
            dieSound = GameStateManagementGame.Get().Content.Load<SoundEffect>("monster_sfx_pack/monster-6");
        }



        public abstract void Attack();


        public virtual void Die()
        {
            dieSound.Play();
            GameplayScreen.acteurs.Remove(this);
            if (this!=Player.Get())
            {
                int value = GameplayScreen.random.Next(2);
                switch(value)
                {
                    case 0:
                        GameplayScreen.items.Add(new HealthPotion((int)this.position.X, (int)this.position.Y));
                        break;
                    case 1:
                        GameplayScreen.items.Add(new Gold((int)this.position.X, (int)this.position.Y));
                        break;
                }
            }
        }

    }
}