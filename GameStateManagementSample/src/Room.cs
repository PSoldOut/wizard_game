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
        GameStateManagementGame gameInstance = GameStateManagementGame.Get();
        bool enabled = true;
        public Gamestate[,] gamestate = new Gamestate[128, 72];
        public Room(int i)
        {
            index = i;
            W_Width = gameInstance.GraphicsDevice.Viewport.Width;
            W_Height = gameInstance.GraphicsDevice.Viewport.Height;
            wallBuilder(new Point(0, 0), new Point(W_Width, 10), "Top");//TopWall
            wallBuilder(new Point(0, 0), new Point(10, W_Height), "left");//left wall
            wallBuilder(new Point(0, W_Height - 10), new Point(W_Width, 10), "Bottom"); //Bottom Wall
            wallBuilder(new Point(W_Width - 10, 0), new Point(10, W_Height), "Right");//Right wall
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

            // Debug.WriteLine("Set door to " + door.Pos);
            doors.Add(door);
            if (usedSitesForDoor.Count == 1)//erste tür im raum
            {
                Debug.WriteLine(door.Center);
                Point size = new Point(0, 0);
                Point pos = new Point(0, 0);
                int wallLength = rnd.Next(3, 10) * 10;
                switch (site)
                {
                    case Door.SiteEnum.Left: //left seite

                        size = new Point(wallLength, 10);
                        pos = door.Center + new Point(-15, 30);
                        break;
                    case Door.SiteEnum.Top: //top
                        size = new Point(10, wallLength);
                        pos = door.Center + new Point(30, -15);
                        break;
                    case Door.SiteEnum.Bottom: //bottom
                        size = new Point(10, wallLength);
                        pos = door.Center + new Point(30, 15) - size;

                        break;
                    case Door.SiteEnum.Right: //right seite
                        size = new Point(wallLength, 10);
                        pos = door.Center + new Point(15, 30) - size;
                        break;


                }

                //Wall wall = wallBuilder(pos, size);
                generateObstacles(10);

            }
            return door.Site;


        }
        public void generateObstacles(int count)
        {
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
                if (wall.DetacteCollison(hitbox, playerImage))
                {
                    return true;
                }

            }
            foreach (Tile tile in tiles)
            {
                foreach (Wall wall in tile.walls)
                {
                    if (wall.DetacteCollison(hitbox, playerImage))
                    {
                        return true;
                    }

                }
            }
            foreach (Tile obstacle in obstacles)
            {
                foreach (Wall wall in obstacle.walls)
                {
                    if (wall.DetacteCollison(hitbox, playerImage))
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






    }
}