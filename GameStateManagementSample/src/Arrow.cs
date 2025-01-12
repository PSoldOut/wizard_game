using System;
using GameStateManagement;
using Manager;
using Microsoft.Xna.Framework;

namespace wizard_game
{
    public class Arrow : Projectile
    {

        public Arrow(int x, int y, Acteur attacker) : base(new Vector2(x, y), 5, 5, "arrow", attacker)
        {
            sprite = new Sprite(AssetManager.GetTexture(spritename), 1, 1, 0.12f);
            sprite.origin = new Vector2(sprite.texture.Width/2, sprite.texture.Height/2);
            sprite.offset = new Vector2(0, 2.5f);
            sprite.FlipX();
            speed = 400f;
        }


        public override void RefreshVolume(float volumeForSound, float volumeForMusic)
        {
            base.RefreshVolume(volumeForSound, volumeForMusic);
            hitSound.Volume = volumeForSound;
        }



        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (Math.Abs(position.X-startPos.X) > 400 || Math.Abs(position.Y - startPos.Y) > 400)
            {
                GameplayScreen.map.GetActiveRoom().projectiles.Remove(this);
                return;
            }

            for (int i = 0; i < GameplayScreen.map.GetActiveRoom().acteurs.Count; i++)
            {
                if (hitBox.Intersects(GameplayScreen.map.GetActiveRoom().acteurs[i].hitBox) && GameplayScreen.map.GetActiveRoom().acteurs[i] != attacker)
                {
                    hitSound.Stop();
                    hitSound.Play();
                    GameplayScreen.map.GetActiveRoom().acteurs[i].takeDamage(damage + attacker.rangedExtraDamage);
                    GameplayScreen.map.GetActiveRoom().projectiles.Remove(this);
                    return;
                }
            }
            if (DetacteCollison())
            {
                GameplayScreen.map.GetActiveRoom().projectiles.Remove(this);
                return;
            }

            position += (speed + attacker.rangedExtraVelocity) * direction * (float)gameTime.ElapsedGameTime.TotalSeconds;
            float opposite = direction.X;
            float adjecent = direction.Y;
            float alpha = (float)Math.Atan2(opposite,adjecent);
            rotation = -alpha;

            if (Math.Abs(position.X-startPos.X) > 400 || Math.Abs(position.Y - startPos.Y) > 400) GameplayScreen.map.GetActiveRoom().projectiles.Remove(this);
        }
    }

}