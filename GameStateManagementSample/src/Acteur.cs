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
using System.Runtime.CompilerServices;
using Manager;

namespace wizard_game
{
    public abstract class Acteur : GameEntity
    {
        public static int DEFAULT_MAX_HEALTH = 4;
        public int health;
        protected SoundEffectInstance dieSound;
        protected SoundEffectInstance damageSound;
        Timer dieTimer;
        protected bool isDying;
        protected bool isTakingDamage;

        public Acteur(Vector2 position, int width, int height, string spriteName, bool hasCollision) : base(position, width, height, spriteName, hasCollision)
        {
            health = DEFAULT_MAX_HEALTH;
            dieSound = ((SoundEffect) AssetManager.Get("monster_sfx_pack/monster-6")).CreateInstance();
            dieSound.Volume = GameStateManagementGame.GetSoundVolume();
            damageSound = ((SoundEffect) AssetManager.Get("monster_sfx_pack/monster-5")).CreateInstance();
            damageSound.Volume = GameStateManagementGame.GetSoundVolume();
            dieTimer = new Timer(1, this);
            isDying = false;
            isTakingDamage = false;
        }



        public abstract void Attack();


        public virtual void Die()
        {
            dieSound.Play();
            rotation = (float)(Math.PI/2);
            dieTimer.start();
            isDying = true;
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



        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            dieTimer.Update(gameTime);
            if (isTakingDamage)
            {
                Vector2 o = new Vector2(width/2, height/2);
                sprite.origin = o;
                float endRotation = 1;
                rotation = MathHelper.Lerp(rotation, endRotation, 0.4f);
                if (rotation <= endRotation + 0.05 && rotation >= endRotation - 0.05)
                {
                    isTakingDamage = false;
                    if (isDying) rotation = (float)(Math.PI/2);
                    else rotation = 0;
                }
            }
            
        }


        public override void TimerCallback(Timer timer)
        {
            if (timer == dieTimer)
            {
                isDying = false;
                GameplayScreen.acteurs.Remove(this);
            }
        }


        public virtual void takeDamage(int damage)
        {
            damageSound.Play();
            health-=damage;
            isTakingDamage = true;
            Console.WriteLine(health);
            if (health <= 0) Die();
        }

    }
}