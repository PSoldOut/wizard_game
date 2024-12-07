using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using GameStateManagement;

namespace wizard_game
{
    public abstract class Acteur : GameEntity
    {

        public Acteur(Vector2 position, int width, int height, string spriteName, bool hasCollision) : base(position, width, height, spriteName, hasCollision)
        {

        }



        public abstract void Attack();


        public void Die()
        {
            GameplayScreen.acteurs.Remove(this);
        }

    }
}