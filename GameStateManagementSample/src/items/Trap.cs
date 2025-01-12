using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using GameStateManagement;
using Manager;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using wizard_game;

namespace wizard_game
{

    class Trap : Item
    {

        public bool used;

        public Trap(int x, int y) : base(new Vector2(x, y), 32, 32, "trap", false)
        {
            effectSound = AssetManager.GetSoundInstance("hits/hit06.mp3");
            effectSound.Volume = GameStateManagementGame.soundSettings.GetVolumeForSound();
            LoadSprite(4, 2, 0.4f, true);
            sprite.animationSpeed = 70;
            width = sprite.frameWidth;
            height = sprite.frameHeight;
            sprite.offset = new Vector2(-35,-35);
            int[] snapAnim = { 0,1,2,3,4,5,6,7 };
            int[] idleAnim = { 0 };
            int[] usedAnim = {7};
            sprite.addAnimtaion(snapAnim, "snap");
            sprite.addAnimtaion(idleAnim, "idle");
            sprite.addAnimtaion(usedAnim, "used");
            sprite.setAnimation("idle");
            used = false;
            blinkEffect = false;
            sprite.layerDepth = 0.6f;
        }

        public void SetPos(Map map)
        {
            position = GenerateRandomPosition(map);
        }

        public override void Effect()
        {
            if (used) return;
            used = true;
            effectSound.Volume = GameStateManagementGame.soundSettings.GetVolumeForSound();
            effectSound.Play();
            sprite.setAnimation("snap");
            sprite.pushAnimation("used");
            
            Player.Get().takeDamage(1);
            //GameplayScreen.map.GetActiveRoom().items.Remove(this);
        }





        public bool detectCollisionWithRec(Rectangle rec, Map map)
        {
            if (map == null)
            {
                throw new InvalidOperationException("Map is not initialized.");
            }

            return map.DetacteCollison(rec, null, false);
        }

        public Vector2 GenerateRandomPosition(Map map)
        {
            Random random = new Random();
            Vector2 randomPosition;

            do
            {
 
                int randomX = random.Next(0, GameStateManagementGame.Get().graphics.GraphicsDevice.Viewport.Width - width);
                int randomY = random.Next(0, GameStateManagementGame.Get().graphics.GraphicsDevice.Viewport.Height - height);

                randomPosition = new Vector2(randomX, randomY);

                Rectangle rec = new Rectangle(randomX, randomY, width * 2, height * 2);

                if (!detectCollisionWithRec(rec, map))
                {
                    // Wenn keine Kollision, dann verlasse die Schleife
                    break;
                }

            } while (true);

            return randomPosition;
        }

    }
}