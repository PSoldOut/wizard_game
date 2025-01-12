
using Microsoft.Xna.Framework;

namespace wizard_game
{
    public abstract class MeeleWeapon : Weapon
    {
        public MeeleWeapon(Vector2 position, int width, int height, string spriteName, WeaponName weaponName) :
            base(position, width, height, spriteName, weaponName)
        {

        }
    }
}