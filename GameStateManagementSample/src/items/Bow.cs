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
using Microsoft.Xna.Framework.Media;

namespace wizard_game
{

    public class Bow : Weapon
    {

        bool isAttacking;
        SoundEffect shootSound;
        Timer attackTimer;
        float attackSpeed = 1.0f;
        ParticleSystem particleSystem;
        public Bow(int x, int y) : base(new Vector2(x, y), 20, 45, "bow", false, WeaponName.BOW)
        {
            shootSound = AssetManager.GetSound("battle_sound_effects/Bow");
            LoadSprite(6, 4, 0.5f, true);

            int[] shootAnim = {0,1,2,3,4,5,6,7,8,9,10,11,23,22,21,20,19,18,17,16,15,14,13,12};
            int[] idleAnim = {0};
            sprite.addAnimtaion(shootAnim, "shoot");
            sprite.addAnimtaion(idleAnim, "idle");
            sprite.animationSpeed = 2;
            sprite.setAnimation("idle");

            equipedOffsetDown = new Vector2(10,10);
            equipedOffsetLeft = new Vector2(-10,0);
            equipedOffsetRight = new Vector2(10,0);
            equipedOffsetUp = new Vector2(-10,0);
            equipedOffset = new Vector2(0,0);

            isAttacking = false;
            attackTimer = new Timer(attackSpeed);
            particleSystem = new ParticleSystem(40);
        }



        public override void Attack(Acteur attacker)
        {
            if (attackTimer.isRunning) return;
            attackTimer.start();
            shootSound.Play();
            sprite.setAnimation("shoot");
            Arrow arrow = new Arrow((int)(attacker.position.X+attacker.width/2), (int)(attacker.position.Y+attacker.height/2), attacker);
            GameplayScreen.map.GetActiveRoom().projectiles.Add(arrow);
            arrow.SetDirection(Player.Get().GetDirection());
        }



        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            attackTimer.Update(gameTime);
            if (isAttacking)
            {
                Console.WriteLine("attacking");

            }
            else
            {
                rotation = equipedRotation;
            }
            if (this.state == State.ON_FLOOR)
            {
                particleSystem.AddMagicEffect(new Vector2(position.X+width/2, position.Y+height), 1, Color.AliceBlue);
                particleSystem.Update(gameTime);
            }
        }



        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            if (this.state == State.ON_FLOOR) particleSystem.Draw();
        }



        public override void Effect()
        {
            effectSound.Play();
            Player.Get().AddWeapon(this);
            this.state = State.IN_INVENTORY;
            GameplayScreen.map.GetActiveRoom().items.Remove(this);
            sprite.color = Color.White;
        }



        public override void SetEquippedDown()
        {
            equipedOffset = equipedOffsetDown;
            sprite.setFlippedY(false);
        }


        public override void SetEquippedLeft()
        {
            equipedOffset = equipedOffsetLeft;
            sprite.setFlippedY(true);
        }


        public override void SetEquippedRight()
        {
            equipedOffset = equipedOffsetRight;
            sprite.setFlippedY(false);
        }


        public override void SetEquippedUp()
        {
            equipedOffset = equipedOffsetUp;
            sprite.setFlippedY(true);
        }

    }

}