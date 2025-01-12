using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.Diagnostics;
using GameStateManagement;
using Manager;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace wizard_game
{
    public class Sword : MeeleWeapon
    {

        private float startRotation;
        private float endRotation;
        float distance;
        float damageRadius;
        bool isAttacking;

        SoundEffectInstance swishSound;
        SoundEffectInstance hitSound;
        float animationRotation;
        ParticleSystem particleSystem;

        public Sword(int x, int y) : base(new Vector2(x, y), 20, 20, "sword", WeaponName.SWORD)
        {
            hitSound = AssetManager.GetSoundInstance("hits/hit02.mp3");
            hitSound.Volume = GameStateManagementGame.soundSettings.GetVolumeForSound();

            swishSound = AssetManager.GetSoundInstance("battle_sound_effects/swish_2");
            swishSound.Volume = GameStateManagementGame.soundSettings.GetVolumeForSound();
            LoadSprite(1,1,0.16f);
            equipedOffsetRight = new Vector2(26,23);
            equipedOffsetLeft = new Vector2(-2, 23);
            equipedOffsetDown = new Vector2(18,33);
            equipedOffsetUp = new Vector2(4,12);
            equipedOffset = equipedOffsetRight;
            equipedRotation = -(float)Math.PI * 0.25f;
            endRotation = (float)Math.PI - 0.2f;
            sprite.origin = new Vector2(80,120);
            isAttacking = false;
            distance = 20;
            damageRadius = 100;
            damage = 1;
            particleSystem = new ParticleSystem(40);
        }


        public override void Effect()
        {
            effectSound.Play();
            Player.Get().AddWeapon(this);
            this.state = State.IN_INVENTORY;
            GameplayScreen.map.GetActiveRoom().items.Remove(this);
            sprite.color = Color.White;
        }


        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (isAttacking)
            {
                Console.WriteLine("attacking");
                rotation = MathHelper.Lerp(rotation, endRotation, 0.3f);
                if (rotation <= endRotation + 0.01 && rotation >= endRotation - 0.01)
                {
                    isAttacking = false;
                    sprite.rotation = startRotation;
                }
            }
            else
            {
                rotation = equipedRotation;
            }
            if (this.state == State.ON_FLOOR)
            {
                particleSystem.AddMagicEffect(new Vector2(position.X+width/2, position.Y+height/2), 1, Color.AliceBlue, 0.6f);
                particleSystem.Update(gameTime);
            }
        }


        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            if (this.state == State.ON_FLOOR) particleSystem.Draw();
        }


        public override void Attack(Acteur attacker)
        {

            if (!isAttacking)
            {
                isAttacking = true;
                swishSound.Stop();
                swishSound.Play();

                for (int i = 0; i < GameplayScreen.map.GetActiveRoom().acteurs.Count; i++)
                {
                    if (attacker.damageArea.Intersects(GameplayScreen.map.GetActiveRoom().acteurs[i].hitBox) && attacker != GameplayScreen.map.GetActiveRoom().acteurs[i])
                    {
                            hitSound.Stop();
                            hitSound.Play();
                            GameplayScreen.map.GetActiveRoom().acteurs[i].takeDamage(damage + attacker.meeleExtraDamage);
                    }
                }
            }
            
        }


        public override void SetEquippedDown()
        {
            if (isAttacking) return;
            equipedOffset = equipedOffsetDown;
            rotation = equipedRotation;
        }


        public override void SetEquippedLeft()
        {
            if (isAttacking) return;
            equipedOffset = equipedOffsetLeft;
            rotation = equipedRotation;
        }


        public override void SetEquippedRight()
        {
            if (isAttacking) return;
            equipedOffset = equipedOffsetRight;
            rotation = equipedRotation;
        }


        public override void SetEquippedUp()
        {
            if (isAttacking) return;
            equipedOffset = equipedOffsetUp;
            rotation = equipedRotation;
        }



    }

}