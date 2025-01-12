
using Microsoft.Xna.Framework;
using GameStateManagement;
using Microsoft.Xna.Framework.Audio;
using Manager;

namespace wizard_game
{

    public abstract class Projectile : GameEntity
    {
        public static int DEFAULT_DAMAGE = 1;
        protected float speed;
        protected int damage;
        public Acteur attacker;
        protected Vector2 startPos;

        protected SoundEffectInstance hitSound;

        public Projectile(Vector2 pos, int width, int height, string spritename, Acteur attacker) : base(pos, width, height, spritename)
        {
            hitSound = AssetManager.GetSoundInstance("hits/hit03.mp3");
            hitSound.Volume = GameStateManagementGame.soundSettings.GetVolumeForSound();
            this.attacker = attacker;
            startPos = position;
            damage = DEFAULT_DAMAGE;
        }
    }

}