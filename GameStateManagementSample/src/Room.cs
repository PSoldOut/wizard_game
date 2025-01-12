using System;
using System.Collections.Generic;
using System.Diagnostics;
using GameStateManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace wizard_game
{
    public class Room
    {
        private List<Wall> walls = new List<Wall>();
        private List<Door> doors = new List<Door>();
        private List<Tile> tiles = new List<Tile>();
        private List<Tile> obstacles = new List<Tile>();
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
        public Gamestate[,] gamestate = new Gamestate[128, 72];
        int outerWallWidth = 70;
        int outerWallWidthSide = 85;

        public Map map;
        public List<Acteur> acteurs;
        public List<Item> items;
        public List<Projectile> projectiles;

        public bool isInitialized = false;
        static int goldCount = 7;
        private int level;
        private Dictionary<int, List<RoomEnemyConfig>> levelRoomConfigs = new Dictionary<int, List<RoomEnemyConfig>>
{
    // Level 1: Nur Prisoner und Knight
    { 1, new List<RoomEnemyConfig> {
        new RoomEnemyConfig(EnemyType.PRISONER, 1, 2),
        new RoomEnemyConfig(EnemyType.KNIGHT, 2, 3),
          new RoomEnemyConfig(EnemyType.GUARD, 0,1)
    }},
    // Level 2: Prisoner, Knight und Guard
    { 2, new List<RoomEnemyConfig> {
        new RoomEnemyConfig(EnemyType.PRISONER, 1, 2),
        new RoomEnemyConfig(EnemyType.KNIGHT, 2, 3),
        new RoomEnemyConfig(EnemyType.GUARD, 1, 2),
        new RoomEnemyConfig(EnemyType.DOUBLER, 0, 1),
    }},
    // Level 3: Doubler, Guard, Wizard und Magie
    { 3, new List<RoomEnemyConfig> {
        new RoomEnemyConfig(EnemyType.DOUBLER, 1, 2),
        new RoomEnemyConfig(EnemyType.GUARD, 2, 4),
        new RoomEnemyConfig(EnemyType.WIZARD, 1, 2),
        new RoomEnemyConfig(EnemyType.MAGIE, 1, 3)
    }}
};
        enum DirEnum : int
        {

            Left = 0,
            Top = 1,
            Bottom = 2,
            Right = 3,

        }
        enum Collison : int
        {
            None = 0,
            Border = 1,
            Side = 2,
            Front = 3,


        }
        public Room(int index, Map map, int level)
        {
            initGamestate();
            this.index = index;
            W_Width = gameInstance.GraphicsDevice.Viewport.Width;
            W_Height = gameInstance.GraphicsDevice.Viewport.Height;
            SideWalls();

            this.map = map;
            this.level = level;
            items = new List<Item>();
            acteurs = new List<Acteur>();
            projectiles = new List<Projectile>();

        }
        private void SideWalls()
        {
            wallBuilder(new Point(0, 0), new Point(W_Width, outerWallWidth), "Top").visible = false;//TopWall
            wallBuilder(new Point(0, 0), new Point(outerWallWidthSide, W_Height), "left").visible = false;//left wall
            wallBuilder(new Point(0, W_Height - outerWallWidth), new Point(W_Width, outerWallWidth), "Bottom").visible = false; //Bottom Wall
            wallBuilder(new Point(W_Width - outerWallWidthSide, 0), new Point(outerWallWidthSide, W_Height), "Right").visible = false;//Right wall
        }
        private Wall wallBuilder(Point pos, Point size, String id = null)
        {
            Wall wall = new Wall(this, pos, size, "Floors/Brickwall5");
            walls.Add(wall);
            setGamestateElement(new Vector2(pos.X, pos.Y), Gamestate.WALL);
            return wall;

        }


        public void init()
        {
            generateItems();
            generateEnemiesForRoom(level);
            isInitialized = true;
        }

        public void generateEnemiesForRoom(int level)
        {
            if (!levelRoomConfigs.ContainsKey(level))
            {
                throw new ArgumentException($"Keine Konfiguration für Level {level} gefunden.");
            }

            Random random = new Random();
            List<RoomEnemyConfig> configs = levelRoomConfigs[level];

            foreach (var config in configs)
            {
                // Zufällige Anzahl der Gegner für diesen Typ im Raum
                int count = random.Next(config.Min, config.Max + 1);

                for (int i = 0; i < count; i++)
                {
                    Enemy current = createEnemyByType(config.Type);
                    current.position = current.GenerateRandomPosition();
                    acteurs.Add(current);
                }
            }
        }
        private Enemy createEnemyByType(EnemyType type)
        {
            switch (type)
            {
                case EnemyType.GUARD:
                    return new Enemy_Guard(0, 0, map, EnemyType.GUARD, this);
                case EnemyType.KNIGHT:
                    return new Enemy_Knight(0, 0, map, EnemyType.KNIGHT, this);
                case EnemyType.DOUBLER:
                    return new Enemy_Doubler(0, 0, map, EnemyType.DOUBLER, this, 4);
                case EnemyType.PRISONER:
                    return new Enemy_prisoner(0, 0, map, EnemyType.PRISONER, this);
                case EnemyType.MAGIE:
                    return new Enemy_Magie(0, 0, map, EnemyType.MAGIE, this);
                case EnemyType.WIZARD:
                    return new Enemy_Wizard(0, 0, map, EnemyType.WIZARD, this);
                default:
                    throw new ArgumentException("Unbekannter EnemyType");
            }
        }



        public void generateEnemies()
        {
            if (GameStateManagementGame.mode != GameMode.TUTORIAL)
            {
                Enemy current = new Enemy_Guard(100, 200, map, EnemyType.GUARD, this);
                current.position = current.GenerateRandomPosition();
                acteurs.Add(current);

                current = new Enemy_Knight(100, 100, map, EnemyType.KNIGHT, this);
                current.position = current.GenerateRandomPosition();
                acteurs.Add(current);

                current = new Enemy_Doubler(200, 200, map, EnemyType.DOUBLER, this, 4);
                current.position = current.GenerateRandomPosition();
                acteurs.Add(current);

                current = new Enemy_prisoner(300, 300, map, EnemyType.PRISONER, this);
                current.position = current.GenerateRandomPosition();
                acteurs.Add(current);

                current = new Enemy_Magie(150, 200, map, EnemyType.MAGIE, this);
                current.position = current.GenerateRandomPosition();
                acteurs.Add(current);

                current = new Enemy_Wizard(150, 200, map, EnemyType.MAGIE, this);
                current.position = current.GenerateRandomPosition();
                acteurs.Add(current);
            }
        }

        public void generateEnemiesForAllRooms(int level)
        {
            for (int i = 1; i <= 5; i++) // Für 5 Räume
            {
                Console.WriteLine($"Generiere Gegner für Raum {i} in Level {level}...");
                generateEnemiesForRoom(level);
            }
        }


        public void generateItems()
        {
            if (GameStateManagementGame.mode != GameMode.TUTORIAL)
            {
                for (int i = 0; i < goldCount; i++)
                {
                    Gold g = new Gold(0, 0);
                    g.SetPos(map);
                    SpawnItem(g);
                    setGamestateElement(g.position, Gamestate.GOLD);
                }
                SpawnItem(new HealthPotion(GameplayScreen.rand.Next(GameStateManagementGame.Get().graphics.GraphicsDevice.Viewport.Width), GameplayScreen.rand.Next(GameStateManagementGame.Get().graphics.GraphicsDevice.Viewport.Height)));
                SpawnItem(new Sword(300, 100));
                SpawnItem(new Bow(100,100));
                SpawnItem(new Shoes(150,150));
                SpawnItem(new Role(200,200));
                SpawnItem(new Trap(400,200));
            }
        }



        public void SpawnActeur(Acteur acteur)
        {
            acteurs.Add(acteur);
            Rectangle oldRect = acteur.hitBox;
            acteur.hitBox = new Rectangle((int)acteur.position.X - 10, (int)acteur.position.Y - 10, acteur.width + 20, acteur.height + 20);
            while (acteur.DetacteCollison())
            {
                acteur.position = new Vector2(GameplayScreen.rand.Next(0, GameStateManagementGame.Get().graphics.PreferredBackBufferWidth), GameplayScreen.rand.Next(0, GameStateManagementGame.Get().graphics.PreferredBackBufferHeight));
                acteur.hitBox.X = (int)acteur.position.X - 10;
                acteur.hitBox.Y = (int)acteur.position.Y - 10;
            }
            acteur.hitBox = oldRect;
        }


        public void SpawnItem(Item item)
        {
            items.Add(item);
            Rectangle oldRect = item.hitBox;
            item.hitBox = new Rectangle((int)item.position.X - 10, (int)item.position.Y - 10, item.width + 20, item.height + 20);
            while (item.DetacteCollison())
            {
                item.position = new Vector2(GameplayScreen.rand.Next(0, GameStateManagementGame.Get().graphics.PreferredBackBufferWidth), GameplayScreen.rand.Next(0, GameStateManagementGame.Get().graphics.PreferredBackBufferHeight));
                item.hitBox.X = (int)item.position.X - 10;
                item.hitBox.Y = (int)item.position.Y - 10;
            }
            item.hitBox = oldRect;
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
            //debug
            /*for (int i = 0; i < 60; i++)
            {
                wallBuilder(new Point(i * 20, 500), new Point(20, 20));
            }*/

            for (int c = 0; c < 3; c++)
            {

                // Debug.WriteLine("new wall #######");
                int XLength = fields.GetLength(0);
                int YLength = fields.GetLength(1);
                int x;
                int y;
                DirEnum dir;
                do
                {
                    x = rnd.Next(XLength / 4, XLength / 4 * 3);
                    y = rnd.Next(YLength / 4, YLength / 4 * 3);

                    //x = 50;
                    //y = 50;
                    dir = (DirEnum)rnd.Next(0, 4);
                    //DirEnum dir = DirEnum.Top;
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
                } while (checkDoor(x, y));



                DirEnum dirOld = dir;
                bool changeDir = false;
                int iteraions = 30;
                int dirChanges = 1;
                int dirChangesMax = 3;

                for (int i = 0; i < iteraions; i++)
                {
                    if (changeDir || rnd.Next(i) > iteraions / (dirChangesMax / dirChanges))//|| rnd.Next(i) > iteraions / (dirChangesMax / dirChanges)
                    {

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

                    Collison res;
                    switch (dir)
                    {
                        case DirEnum.Right:
                            x += 2;
                            break;
                        case DirEnum.Left:
                            x -= 2;
                            break;
                        case DirEnum.Top:
                            y -= 2;
                            break;
                        case DirEnum.Bottom:
                            y += 2;
                            break;


                    }
                    res = CheckNeighbors(x, y, dir);
                    if (res == Collison.Border)
                    {
                        i = iteraions;
                        continue;
                    }
                    if (res == Collison.Side)
                    {
                        continue;
                    }
                    if (res == Collison.Front)
                    {
                        int blocksToSkip = 80;
                        switch (dir)
                        {
                            case DirEnum.Right:
                                x += blocksToSkip;
                                break;
                            case DirEnum.Left:
                                x -= blocksToSkip;
                                break;
                            case DirEnum.Top:
                                y -= blocksToSkip;
                                break;
                            case DirEnum.Bottom:
                                y += blocksToSkip;
                                break;

                        }
                        if (CheckBorder(x, y, dir))
                        {
                            i = iteraions;

                        }
                        continue;
                    }
                    wallBuilder(new Point(x * 10, y * 10), new Point(20, 20));

                }
            }
        }
        private bool CheckBorder(int x, int y, DirEnum dir)
        {
            x *= 10;
            y *= 10;
            switch (dir)
            {
                case DirEnum.Right:
                    if (x > W_Width - 80) return true;
                    break;
                case DirEnum.Left:
                    if (x < 80) return true;
                    break;
                case DirEnum.Top:
                    if (y < 80) return true;
                    break;
                case DirEnum.Bottom:
                    if (y > W_Height - 100) return true;
                    break;

            }
            return false;
        }
        public bool checkDoor(int x, int y)
        {
            Rectangle rectangle = new Rectangle(x * 10, y * 10, 100, 100);
            foreach (Door door in doors)
            {

                if (door.hitBox.Intersects(rectangle))
                {
                    Debug.WriteLine("intersect door");
                    return true;
                }
            }
            return false;
        }
        private Collison CheckNeighbors(int x, int y, DirEnum dir)
        {

            if (CheckBorder(x, y, dir))
            {
                return Collison.Border;
            }
            if (checkDoor(x, y))
            {
                return Collison.Border;
            }
            x *= 10;
            y *= 10;
            int frontBlocksCheck = 18;
            switch (dir)
            {
                case DirEnum.Right:
                    if (WallOverlap(new Point(x, y - 40), 40, 100))
                    {

                        return Collison.Side;
                    }
                    if (WallOverlap(new Point(x, y), frontBlocksCheck * 10, 20)) //in front
                    {
                        return Collison.Front;
                    }


                    break;
                case DirEnum.Left:
                    if (WallOverlap(new Point(x - 40, y - 40), 40, 100))
                    {

                        return Collison.Side;
                    }
                    if (WallOverlap(new Point(x - (frontBlocksCheck - 2) * 10, y), frontBlocksCheck * 10, 20)) //in front
                    {
                        return Collison.Front;
                    }
                    break;

                case DirEnum.Top:
                    if (WallOverlap(new Point(x - 40, y - 40), 100, 40))
                    {

                        return Collison.Side;
                    }
                    if (WallOverlap(new Point(x, y - (frontBlocksCheck - 2) * 10), 20, frontBlocksCheck * 10)) //in front
                    {
                        return Collison.Front;
                    }
                    break;


                case DirEnum.Bottom:
                    if (WallOverlap(new Point(x - 40, y), 100, 40))
                    {

                        return Collison.Side;
                    }
                    if (WallOverlap(new Point(x, y), 20, frontBlocksCheck * 10)) //in front
                    {
                        return Collison.Front;
                    }
                    break;

            }

            return Collison.None;
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
                        return true;
                    }


                }

            }
            return false;
        }
        public bool intersectObstacle(Point pos)
        {
            foreach (Tile obstacle in obstacles)
            {
                foreach (Wall wall in obstacle.walls)
                {
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

            for (int i = 0; i < items.Count; i++) items[i].Draw(gameTime);
            for (int i = 0; i < projectiles.Count; i++) projectiles[i].Draw(gameTime);
            for (int i = 0; i < acteurs.Count; i++) acteurs[i].Draw(gameTime);

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






        public void initGamestate()
        {
            for (int i = 0; i < gamestate.GetLength(0); i++)
            {
                for (int j = 0; j < gamestate.GetLength(1); j++)
                {
                    gamestate[i, j] = Gamestate.EMPTY;
                }
            }
        }
        public void PrintView()
        {
            for (int i = 0; i < gamestate.GetLength(0); i++)
            {
                for (int j = 0; j < gamestate.GetLength(1); j++)
                {
                    if (gamestate[i, j] == Gamestate.PLAYER || gamestate[i, j] == Gamestate.GOLD) { }
                }
            }
        }
        public void InitGamestate()
        {
            for (int i = 0; i < gamestate.GetLength(0); i++)
            {
                for (int j = 0; j < gamestate.GetLength(1); j++)
                {
                    gamestate[i, j] = Gamestate.EMPTY;
                }
            }
        }
        public void setGamestateElement(Vector2 pos, Gamestate state)
        {
            int cordX = (int)pos.X / 10;
            int cordY = (int)pos.Y / 10;
            gamestate[cordX, cordY] = state;
        }




        public void Update(GameTime gameTime)
        {
            for (int i = 0; i < items.Count; i++) items[i].Update(gameTime);
            for (int i = 0; i < projectiles.Count; i++) projectiles[i].Update(gameTime);
            for (int i = 0; i < acteurs.Count; i++) acteurs[i].Update(gameTime);
        }




    }
}