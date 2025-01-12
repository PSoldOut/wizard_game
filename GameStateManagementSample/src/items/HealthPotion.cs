
using GameStateManagement;
using Manager;
using Microsoft.Xna.Framework;

namespace wizard_game
{
    class HealthPotion : Item
    {
        int health = 1;

        public HealthPotion(int x, int y) : base(new Vector2(x, y), 25, 30, "healthPotion")
        {
            effectSound = AssetManager.GetSoundInstance("liveSound");
            effectSound.Volume = GameStateManagementGame.soundSettings.GetVolumeForSound();
            LoadSprite(1,1,0.1f);

        }

        public override void Effect()
        {
            effectSound.Play();
            if (Player.Get().health <= Player.PLAYER_MAX_HEALTH - health + Player.Get().extraMaxHealth)
                Player.Get().health+=health;

            GameplayScreen.map.GetActiveRoom().items.Remove(this);
        }
    }

}