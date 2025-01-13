using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using wizard_game;

class Tile
{
    public List<Wall> walls = new List<Wall>();
    public enum tileType
    {
        LeftCornerTop,
        RightCornerTop,
        LeftCornerBottom,
        RightCornerBottom,
        Rock

    }
    public Dictionary<string, Point> sizes = new Dictionary<string, Point>();
    public Tile( Room _room, tileType _type, Point startPoint)
    {
        switch (_type)
        {
            case tileType.LeftCornerTop:

                walls.Add(new Wall(_room, startPoint, new Point(10, 100)));
                walls.Add(new Wall(_room, startPoint, new Point(100, 10)));
                break;
            case tileType.RightCornerTop:

                walls.Add(new Wall( _room, startPoint, new Point(100, 10)));
                startPoint = new Point(startPoint.X + 100, startPoint.Y);
                walls.Add(new Wall( _room, startPoint, new Point(10, 100)));
                break;
            case tileType.LeftCornerBottom:
                walls.Add(new Wall( _room, startPoint, new Point(10, 100)));
                startPoint = new Point(startPoint.X, startPoint.Y + 100);
                walls.Add(new Wall( _room, startPoint, new Point(100, 10)));
                break;

            case tileType.RightCornerBottom:
                Point _startPoint = new Point(startPoint.X, startPoint.Y + 100);
                walls.Add(new Wall( _room, _startPoint, new Point(100 + 10, 10)));

                _startPoint = new Point(startPoint.X + 100, startPoint.Y);
                walls.Add(new Wall( _room, _startPoint, new Point(10, 100)));

                break;
            case tileType.Rock:
                walls.Add(new Wall( _room, startPoint, new Point(50, 50),"Obstacles/Rock_MOSSY_BIG"));
                break;


        }



    }
    public void Draw(GameTime gameTime)
    {
        foreach (Wall wall in walls)
        {
            wall.Draw(gameTime);
        }

    }

}