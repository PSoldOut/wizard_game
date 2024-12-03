
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace wizard_game
{
    public abstract class MeeleWeapon : Weapon
    {
        public MeeleWeapon(Vector2 position, int width, int height, string spriteName, bool hasCollision, WeaponName weaponName) :
            base(position, width, height, spriteName, hasCollision, weaponName)
        {

        }
    }
}