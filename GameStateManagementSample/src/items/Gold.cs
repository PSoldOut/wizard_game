using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using GameStateManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using wizard_game;

namespace  wizard_game
{

    class Gold : Item
    {
        public Gold(int x, int y) : base(new Vector2(x, y), 27, 27, "gold", false)
        {
            sprite = new Sprite(GameStateManagementGame.Get().Content.Load<Texture2D>(spritename), 1, 1, 0.4f);
            width = sprite.frameWidth;
            height = sprite.frameHeight;
        }

        public override void Effect()
        {
           Player.Get().coins++;//für Enemy nichts
           GameplayScreen.items.Remove(this);
        }


    }
}