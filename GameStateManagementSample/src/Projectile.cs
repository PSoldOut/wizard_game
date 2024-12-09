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
using Microsoft.Xna.Framework.Audio;

namespace wizard_game
{

    public abstract class Projectile : GameEntity
    {
        public static int DEFAULT_DAMAGE = 1;
        protected float speed;
        protected int damage;
        protected Acteur attacker;
        protected Vector2 startPos;

        protected SoundEffectInstance hitSound;

        public Projectile(Vector2 pos, int width, int height, string spritename, bool hasCollision, Acteur attacker) : base(pos, width, height, spritename, hasCollision)
        {
            hitSound = GameStateManagementGame.Get().Content.Load<SoundEffect>("hits/hit03.mp3").CreateInstance();
            GameplayScreen.projectiles.Add(this);
            this.attacker = attacker;
            startPos = position;
            damage = DEFAULT_DAMAGE;
        }
    }

}