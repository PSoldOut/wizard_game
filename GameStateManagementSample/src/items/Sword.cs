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
    public class Sword : MeeleWeapon
    {

        private float startRotation;
        private float endRotation;
        float distance;
        float damageRadius;
        bool isAttacking;


        float animationRotation;

        public Sword(int x, int y) : base(new Vector2(x, y), 20, 5, "sword", false, WeaponName.SWORD)
        {
            sprite = new Sprite(GameStateManagementGame.Get().Content.Load<Texture2D>(spritename), 1, 1, 0.16f);
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

        }


        public override void Effect()
        {
            Player.Get().AddWeapon(this);
            this.state = State.IN_INVENTORY;
            GameplayScreen.items.Remove(this);
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
        }

        public override void Attack(Acteur attacker)
        {
            
            isAttacking = true;
            Rectangle damageArea = new Rectangle((int)(position.X + (Player.Get().GetWidth()/2) + Player.Get().GetDirection().X * distance), (int)(position.Y + (Player.Get().GetHeight()/2) + Player.Get().GetDirection().Y * distance), (int)damageRadius, (int)damageRadius);
            //Console.WriteLine("playerPos X" + Player.Get().position.X);
            //Console.WriteLine("playerPos Y" + Player.Get().position.Y);
            //Console.WriteLine("damageArea X: " + damageArea.X);
            //Console.WriteLine("damageArea Y: " + damageArea.Y);

            for (int i = 0; i < GameplayScreen.acteurs.Count; i++)
            {
               if (attacker.damageArea.Intersects(GameplayScreen.acteurs[i].hitBox) && attacker != GameplayScreen.acteurs[i])
               {
                    GameplayScreen.acteurs[i].Die();
               }
            }

            
        }


        public override void SetEquippedDown()
        {
            equipedOffset = equipedOffsetDown;
        }


        public override void SetEquippedLeft()
        {
            equipedOffset = equipedOffsetLeft;
        }


        public override void SetEquippedRight()
        {
            equipedOffset = equipedOffsetRight;
        }


        public override void SetEquippedUp()
        {
            equipedOffset = equipedOffsetUp;
        }



    }

}