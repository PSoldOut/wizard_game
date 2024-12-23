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

    public class Role : Weapon
    {

        bool isAttacking;
        SoundEffect shootSound;
        Timer attackTimer;
        float attackSpeed = 1.0f;
        ParticleSystem particleSystem;
        public Role(int x, int y) : base(new Vector2(x, y), 55, 35, "role", false, WeaponName.ROLE)
        {
            shootSound = AssetManager.GetSound("fire");
            LoadSprite(1, 1, 0.04f, false);


            equipedOffsetDown = new Vector2(-10,15);
            equipedOffsetLeft = new Vector2(-10,15);
            equipedOffsetRight = new Vector2(0,15);
            equipedOffsetUp = new Vector2(-10,15);
            equipedOffset = new Vector2(0,0);

            isAttacking = false;
            attackTimer = new Timer(attackSpeed);
            particleSystem = new ParticleSystem(40);
        }



        public override void Attack(Acteur attacker)
        {
            if (attackTimer.isRunning) return;
            attackTimer.start();
            //shootSound.Play();
            Fireball fireball = new Fireball((int)(attacker.position.X+attacker.width/2), (int)(attacker.position.Y+attacker.height/2), attacker);
            GameplayScreen.projectiles.Add(fireball);
            fireball.SetDirection(Player.Get().GetDirection());
            fireball.SetAttackstate(true);
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
                particleSystem.AddMagicEffect(new Vector2(position.X+width/2, position.Y+height/2), 1, Color.AliceBlue);
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
            GameplayScreen.items.Remove(this);
            sprite.color = Color.White;
        }



        public override void SetEquippedDown()
        {
            equipedOffset = equipedOffsetDown;
            sprite.setFlippedY(false);
            sprite.scale = 0.03f;
        }


        public override void SetEquippedLeft()
        {
            equipedOffset = equipedOffsetLeft;
            sprite.setFlippedY(true);
            sprite.scale = 0.03f;
        }


        public override void SetEquippedRight()
        {
            equipedOffset = equipedOffsetRight;
            sprite.setFlippedY(false);
            sprite.scale = 0.03f;
        }


        public override void SetEquippedUp()
        {
            equipedOffset = equipedOffsetUp;
            sprite.setFlippedY(true);
            sprite.scale = 0.03f;
        }

    }

}