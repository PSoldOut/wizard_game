using System;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using GameStateManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace wizard_game
{
    public class EndDoor : Door
    {
        public EndDoor(Point size, Color? color = null) : base(size, color) {}
    }

}