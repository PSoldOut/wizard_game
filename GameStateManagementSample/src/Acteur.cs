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
        public static float NEXT_LAYER_DEPTH = 0.01f;
        public int health;
        protected SoundEffectInstance dieSound;
        protected SoundEffectInstance damageSound;
        protected Timer dieTimer;
        protected bool isDying;
        protected bool isTakingDamage;
        ParticleSystem particleSystem;
        public float speed = 0.24f;

        public int rangedExtraDamage = 0;
        public int rangedExtraVelocity = 0;
        public int meeleExtraDamage = 0;
        public int extraMaxHealth = 0;

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
            particleSystem = new ParticleSystem();
            speed = 0.24f;

            NEXT_LAYER_DEPTH += 0.01f;
            if (NEXT_LAYER_DEPTH >= 1.0f) NEXT_LAYER_DEPTH = 0.01f;
        }



        public abstract void Attack();





        public Vector2 GetMidPos()      //gibt die mitte vom Acteur, nicht den oberen linken rand
        {
            return new Vector2(position.X + width/2, position.Y + height/2);
        }



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
            particleSystem.Update(gameTime);
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


        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            particleSystem.Draw();
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
            particleSystem.AddBloodEffect(new Vector2(position.X+width/2, position.Y+height/2), 20);
            isTakingDamage = true;
            //particleSystem.AddMagicEffect(new Vector2(position.X+width/2, position.Y+height/2), 20);
            if (isDying) return;
            health-=damage;
            if (health <= 0) Die();
        }

    }
}