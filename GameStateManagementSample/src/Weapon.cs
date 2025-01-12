
using Microsoft.Xna.Framework;


namespace wizard_game
{
    abstract public class Weapon : Item
    {

        public enum WeaponName
        {
            SWORD, BOW, SPEAR, ROLE
        }

        public WeaponName name;
        public Vector2 equipedOffset;
        public float equipedRotation;

        public Vector2 equipedOffsetDown;
        public Vector2 equipedOffsetLeft;
        public Vector2 equipedOffsetRight;
        public Vector2 equipedOffsetUp;
        public int damage;


        public Weapon(Vector2 position, int width, int height, string spriteName, WeaponName weaponName) :
            base(position, width, height, spriteName)
        {
            this.name = weaponName;
            equipedOffset = new Vector2(0,0);
            equipedOffsetDown = new Vector2(0,0);
            equipedOffsetLeft = new Vector2(0,0);
            equipedOffsetRight = new Vector2(0,0);
            equipedOffsetUp = new Vector2(0,0);
            damage = 0;
        }


        public abstract void Attack(Acteur attacker);
        public abstract void SetEquippedLeft();
        public abstract void SetEquippedRight();
        public abstract void SetEquippedUp();
        public abstract void SetEquippedDown();
    }
}