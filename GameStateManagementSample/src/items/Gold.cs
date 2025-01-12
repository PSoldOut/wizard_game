using System;
using GameStateManagement;
using Manager;
using Microsoft.Xna.Framework;

namespace wizard_game
{

    class Gold : Item
    {

        public Gold(int x, int y) : base(new Vector2(x, y), 27, 27, "gold")
        {
            effectSound = AssetManager.GetSoundInstance("inventory_sound_effects/ring_inventory");
            effectSound.Volume = GameStateManagementGame.soundSettings.GetVolumeForSound();
            LoadSprite(1, 1, 0.4f);
            width = sprite.frameWidth;
            height = sprite.frameHeight;

        }

        public void SetPos(Map map)
        {
            position = GenerateRandomPosition(map);
        }

        public override void Effect()
        {
            effectSound.Play();
            // Player.Get().coins++;
            GameplayScreen.map.GetActiveRoom().items.Remove(this);
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