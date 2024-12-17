using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using GameStateManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
namespace wizard_game
{


    public class Room
    {
        private List<Wall> walls = new List<Wall>();
        private List<Door> doors = new List<Door>();
        private List<Tile> tiles = new List<Tile>();
        private List<Tile> obstacles = new List<Tile>();
        //private List<Bot> bots = new List<Bot>();
        // private List<Bot> botsToDelete = new List<Bot>();
        public Texture2D image;

        public int index;
        private Color[] wallColors = new Color[] { Color.Red, Color.Orange, Color.Yellow, Color.Green, Color.Blue, Color.Indigo, Color.Purple };
        static private Random rnd = new Random(Guid.NewGuid().GetHashCode());
        int W_Height;
        int W_Width;
        private List<Door.SiteEnum> usedSitesForDoor = new List<Door.SiteEnum>();
        public bool[,] fields = new bool[128, 72];
        public bool[,] fields_Walls = new bool[128, 72];
        GameStateManagementGame gameInstance = GameStateManagementGame.Get();
        bool enabled = true;
        public Gamestate[,] gamestate = new Gamestate[128, 72];
        int outerWallWidth = 20;
        enum DirEnum : int
        {

            Left = 0,
            Top = 1,
            Bottom = 2,
            Right = 3,

        }
        public Room(int i)
        {
            index = i;
            W_Width = gameInstance.GraphicsDevice.Viewport.Width;
            W_Height = gameInstance.GraphicsDevice.Viewport.Height;
            SideWalls();
            BuildWalls();
            Debug.WriteLine("build walls");
        }
        private void SideWalls()
        {
            wallBuilder(new Point(0, 0), new Point(W_Width, outerWallWidth), "Top");//TopWall
            wallBuilder(new Point(0, 0), new Point(outerWallWidth, W_Height), "left");//left wall
            wallBuilder(new Point(0, W_Height - outerWallWidth), new Point(W_Width, outerWallWidth), "Bottom"); //Bottom Wall
            wallBuilder(new Point(W_Width - outerWallWidth, 0), new Point(outerWallWidth, W_Height), "Right");//Right wall
        }
        private Wall wallBuilder(Point pos, Point size, String id = null)
        {
            //Debug.WriteLine(string.Format("Build wall with POS {0} and Size {1}", pos, size));
            Wall wall = new Wall(this, pos, size, "Floors/Brickwall5");
            walls.Add(wall);
            return wall;

        }

        public Door.SiteEnum? SetDoor(Door door, Door.SiteEnum? _site = null)
        {

            //set door position

            Door.SiteEnum site;
            if (_site != null)
            {
                site = (Door.SiteEnum)_site;
                usedSitesForDoor.Add(site);
            }
            else
            {
                //site = (Door.SiteEnum)1;

                //usedSitesForDoor.Add(site);

                while (true)
                {
                    site = (Door.SiteEnum)rnd.NextInt64(3);
                    if (!usedSitesForDoor.Contains(site))
                    {
                        usedSitesForDoor.Add(site);
                        break;
                    }

                }

            }
            door.Site = site;

            doors.Add(door);
            return door.Site;

        }
        public void BuildWalls()
        {
            for (int c = 0; c < 3; c++)
            {

                Debug.WriteLine("new wall #######");
                int XLength = fields.GetLength(0);
                int YLength = fields.GetLength(1);
                int x = rnd.Next(XLength / 4, XLength / 4 * 3);
                int y = rnd.Next(YLength / 4, YLength / 4 * 3);

                //x = 50;
                //y = 50;
                DirEnum dir = (DirEnum)rnd.Next(0, 4);
                //DirEnum dir = DirEnum.Right;
                switch (dir)
                {
                    case DirEnum.Right:
                        x = 0;
                        break;
                    case DirEnum.Left:
                        x = XLength - outerWallWidth / 10;

                        break;
                    case DirEnum.Top:
                        y = YLength - outerWallWidth / 10;

                        break;
                    case DirEnum.Bottom:
                        y = 0;
                        break;

                }
                Debug.WriteLine("start dir " + dir + " x" + x + " y:" + y);


                DirEnum dirOld = dir;
                bool changeDir = false;
                int iteraions = 50;
                int dirChanges = 1;
                int dirChangesMax = 5;
                for (int i = 0; i < iteraions; i++)
                {
                    //Debug.WriteLine((x, y));
                    if (changeDir || rnd.Next(i) > iteraions / (dirChangesMax / dirChanges))
                    {
                        //Debug.WriteLine("change dir");

                        if (changeDir)
                        {
                            changeDir = false;
                        }


                        while (true)
                        {
                            dir = (DirEnum)rnd.Next(0, 4);
                            //nicht in die gegenrichtung
                            if (dirOld == DirEnum.Left && dir == DirEnum.Right) continue;
                            if (dirOld == DirEnum.Right && dir == DirEnum.Left) continue;
                            if (dirOld == DirEnum.Top && dir == DirEnum.Bottom) continue;
                            if (dirOld == DirEnum.Bottom && dir == DirEnum.Top) continue;

                            if (dir != dirOld)
                            {
                                Debug.WriteLine("Change dir form " + dirOld + " to " + dir);
                                dirOld = dir;
                                if (!changeDir && dirChangesMax > dirChanges)
                                {
                                    dirChanges++;
                                }

                                break;
                            }


                        }

                    }
                    if (dirChanges >= dirChangesMax)
                    {
                        break;
                    }


                    switch (dir)
                    {
                        case DirEnum.Right:
                            x += 2;

                            if (CheckNeighbors(x, y, dir))
                            {
                                Debug.WriteLine("CheckNeighbors for right" + " x " + x + " y " + y);
                                x -= 2;
                                changeDir = true;
                                continue;
                            }


                            break;
                        case DirEnum.Left:
                            x -= 2;

                            if (CheckNeighbors(x, y, dir))
                            {
                                Debug.WriteLine("CheckNeighbors for left" + " x " + x + " y " + y);
                                x += 2;
                                changeDir = true;
                                continue;
                            }

                            break;
                        case DirEnum.Top:
                            y -= 2;

                            if (CheckNeighbors(x, y, dir))
                            {
                                Debug.WriteLine("CheckNeighbors for top" + " x " + x + " y " + y);
                                y += 2;
                                changeDir = true;
                                continue;
                            }


                            break;
                        case DirEnum.Bottom:
                            y += 2;

                            if (CheckNeighbors(x, y, dir))
                            {
                                Debug.WriteLine("CheckNeighbors for bott" + " x " + x + " y " + y);
                                y -= 2;
                                changeDir = true;
                                continue;
                            }

                            break;


                    }
                    //Debug.WriteLine(("create wall att ", new Point(x * 10, y * 10)));
                    wallBuilder(new Point(x * 10, y * 10), new Point(20, 20));

                }


                Debug.WriteLine(("max", XLength, YLength));
            }
        }
        private bool CheckNeighbors(int x, int y, DirEnum dir)
        {
            x *= 10;
            y *= 10;
            switch (dir)
            {
                case DirEnum.Right:
                    if (WallOverlap(new Point(x, y), 80, 20)) return true;//checkt right

                    if (WallOverlap(new Point(x, y - 40), 40, 120)) return true;//front
                    break;
                case DirEnum.Left:
                    if (WallOverlap(new Point(x - 60, y), 80, 20)) return true;//check left

                    if (WallOverlap(new Point(x - 80, y - 40), 40, 120)) return true;//front
                    break;
                case DirEnum.Top:
                    if (WallOverlap(new Point(x, y), 80, 20)) return true;

                    if (WallOverlap(new Point(x - 40, y - 80), 120, 40)) return true;//front

                    break;
                case DirEnum.Bottom:
                    if (WallOverlap(new Point(x, y), 80, 20)) return true;

                    if (WallOverlap(new Point(x - 40, y), 40, 120)) return true;//front
                    break;
            }
            //Debug.WriteLine("neibor not overlap");
            return false;
        }

        public bool WallOverlap(Point position, int width, int height)
        {
            for (int x = 0; x < width / 10; x++)
            {
                for (int y = 0; y < height / 10; y++)
                {
                    int cordX = (int)position.X / 10 + x;
                    int cordY = (int)position.Y / 10 + y;
                    if (cordX >= fields.GetLength(0) || cordX < 0) return true;
                    if (cordY >= fields.GetLength(1) || cordY < 0) return true;
                    if (fields[cordX, cordY])
                    {
                        //Debug.WriteLine("inersect at" + position);
                        return true;
                    }


                }

            }
            return false;
        }
        public void generateObstacles(int count)
        {
            return;
            int generated = 0;
            while (generated != count)
            {
                Point rockPos = new Point(rnd.Next(10, 120) * 10, rnd.Next(10, 60) * 10);
                if (intersectObstacle(rockPos))
                {
                    Debug.WriteLine("Rocks intersect we draw new");
                    continue;
                }
                //Debug.WriteLine("Draw " + generated + " Pos: " + rockPos);
                Tile rock = new Tile(this, Tile.tileType.Rock, rockPos);
                obstacles.Add(rock);
                generated++;
            }

        }
        public bool intersectObstacle(Point pos)
        {
            foreach (Tile obstacle in obstacles)
            {
                foreach (Wall wall in obstacle.walls)
                {
                    //TODO größe flexile
                    Rectangle hitBox = new Rectangle(pos, new Point(50, 50));
                    if (wall.hitBox.Intersects(hitBox))
                    {
                        return true;
                    }

                }
            }
            return false;
        }
        public bool DetacteCollison(Rectangle hitbox, Color[] playerImage, bool deleteBots = true)
        {
            foreach (Wall wall in walls)
            {
                if (wall.DetacteCollison(hitbox))
                {
                    return true;
                }

            }
            foreach (Tile tile in tiles)
            {
                foreach (Wall wall in tile.walls)
                {
                    if (wall.DetacteCollison(hitbox))
                    {
                        return true;
                    }

                }
            }
            foreach (Tile obstacle in obstacles)
            {
                foreach (Wall wall in obstacle.walls)
                {
                    if (wall.DetacteCollison(hitbox))
                    {
                        return true;
                    }

                }
            }




            return false;
        }

        public Door DetacteCollisonDoor(Rectangle hitbox, bool deleteBots = true)
        {
            foreach (Door door in doors)
            {
                if (door.DetacteCollison(hitbox))
                {
                    // Debug.WriteLine("rl index");
                    // Debug.WriteLine(door.linkedDoor.room.index);
                    // Debug.WriteLine("r index");
                    // Debug.WriteLine(door.room.index);
                    return door.linkedDoor;

                }

            }
            return null;

        }
        public void Draw(GameTime gameTime)
        {
            foreach (Wall wall in walls)
            {
                wall.Draw(gameTime);
            }
            foreach (Tile obstacle in obstacles)
            {
                obstacle.Draw(gameTime);
            }
            foreach (Door door in doors)
            {
                door.Draw(gameTime);
            }

        }
        //debug
        public void ReloadWalls()
        {
            walls.Clear();
            fields = new bool[128, 72];
            fields_Walls = new bool[128, 72];
            SideWalls();
            BuildWalls();
        }






    }
}