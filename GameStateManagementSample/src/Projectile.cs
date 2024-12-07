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

    public abstract class Projectile : GameEntity
    {
        protected float speed;
        protected int damage;
        protected Acteur attacker;
        protected Vector2 startPos;

        public Projectile(Vector2 pos, int width, int height, string spritename, bool hasCollision, Acteur attacker) : base(pos, width, height, spritename, hasCollision)
        {
            GameplayScreen.projectiles.Add(this);
            this.attacker = attacker;
            startPos = position;
        }
    }

}