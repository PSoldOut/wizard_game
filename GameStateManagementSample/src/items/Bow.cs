using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.Diagnostics;
using GameStateManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace wizard_game
{

    public class Bow : Weapon
    {

        bool isAttacking;

        public Bow(int x, int y) : base(new Vector2(x, y), 20, 45, "bow", false, WeaponName.BOW)
        {
            sprite = new Sprite(GameStateManagementGame.Get().Content.Load<Texture2D>("bow"), 6, 4, 0.5f, true);
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
        }



        public override void Attack()
        {
            sprite.setAnimation("shoot");
            Arrow arrow = new Arrow((int)position.X+width/2, (int)position.Y+height/2);
            arrow.SetDirection(Player.Get().GetDirection());
        }



        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (isAttacking)
            {
                Console.WriteLine("attacking");
                
            }
            else
            {
                rotation = equipedRotation;
            }
        }



        public override void Effect()
        {
            Player.Get().AddWeapon(this);
            this.state = State.IN_INVENTORY;
            GameplayScreen.items.Remove(this);
        }



        public override void SetEquippedDown()
        {
            equipedOffset = equipedOffsetDown;
            sprite.setFlipped(false);
        }


        public override void SetEquippedLeft()
        {
            equipedOffset = equipedOffsetLeft;
            sprite.setFlipped(true);
        }


        public override void SetEquippedRight()
        {
            equipedOffset = equipedOffsetRight;
            sprite.setFlipped(false);
        }


        public override void SetEquippedUp()
        {
            equipedOffset = equipedOffsetUp;
            sprite.setFlipped(true);
        }

    }

}