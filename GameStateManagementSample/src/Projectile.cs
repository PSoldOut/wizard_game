using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace wizard_game
{

    public abstract class Projectile : GameEntity
    {
        protected float speed;
        protected int damage;
        public Vector2 startPosition;

        public Projectile(Vector2 pos, int width, int height, string spritename, bool hasCollision) : base(pos, width, height, spritename, hasCollision)
        {
            this.startPosition = pos;
        }
    }

}