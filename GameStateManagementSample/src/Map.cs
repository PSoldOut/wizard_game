using System;
using System.Collections.Generic;
using GameStateManagement;
using Manager;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace wizard_game
{
    public class Map
    {
        public Texture2D image;

        public Room activeRoom;
        public List<Room> rooms = new List<Room>();
        private bool[,] activeRoomFields = new bool[128, 72];
        public int roomIndex = 0;
        public static int level = 1;
        public int levelCount = 3;
        public int roomsCount = 7;
        GameStateManagementGame gameInstance = GameStateManagementGame.Get();
        public Texture2D background;

        public Map() {
        //    Debug.WriteLine("INIT NEW MAP############");

            generateRooms(roomsCount);
            background = AssetManager.GetTexture("bgd1");
         }




        public void generateRooms(int roomsCount)
        {
          //  Debug.WriteLine("Generate Room");
            Room backRoom;
            Room actualRoom = null;
            Point doorSize=new Point(80,95);
            for (int i = 0; i < roomsCount * levelCount; i++)
            {

                if (i == 0) //First room only to next room
                {

                    Room startRoom = new Room( i, this, i/roomsCount+1);
                    Room nextRoom = new Room(i + 1, this, (i+1)/roomsCount+1);

                    rooms.Add(startRoom);
                    rooms.Add(nextRoom);


                    Door doorToNext = new Door(doorSize);
                    Door doorBackFromNext = new Door(doorSize);



                    doorToNext.SetRoom(startRoom);
                    doorBackFromNext.SetRoom(nextRoom);


                    doorToNext.SetLinkedDoor(doorBackFromNext);
                    doorBackFromNext.SetLinkedDoor(doorToNext);


                    startRoom.SetDoor(doorToNext);

                    nextRoom.SetDoor(doorBackFromNext, doorToNext.GetOppositeSite());


                    activeRoom = rooms[0];
                    //backRoom = startRoom;
                    actualRoom = nextRoom;

                }
                else if ((i + 1) % roomsCount == 0)
                {
                    Room nextRoom = new Room(i + 1, this, (i+1)/roomsCount+1);
                    rooms.Add(nextRoom);

                    EndDoor doorToNext = new EndDoor(doorSize);
                    EndDoor doorBackFromNext = new EndDoor(doorSize);
                    doorToNext.SetFront(true);

                    doorToNext.SetRoom(actualRoom);
                    doorBackFromNext.SetRoom(nextRoom);

                    doorToNext.SetLinkedDoor(doorBackFromNext);
                    doorBackFromNext.SetLinkedDoor(doorToNext);


                    actualRoom.SetDoor(doorToNext);

                    nextRoom.SetDoor(doorBackFromNext, doorToNext.GetOppositeSite());
                    actualRoom = nextRoom;


                }
                else if (i + 1 < roomsCount * levelCount)
                {
                    Room nextRoom = new Room(i + 1, this, (i+1)/roomsCount+1);
                    rooms.Add(nextRoom);

                    Door doorToNext = new Door(doorSize);
                    Door doorBackFromNext = new Door(doorSize);


                    doorToNext.SetRoom(actualRoom);
                    doorBackFromNext.SetRoom(nextRoom);

                    doorToNext.SetLinkedDoor(doorBackFromNext);
                    doorBackFromNext.SetLinkedDoor(doorToNext);


                    actualRoom.SetDoor(doorToNext);

                    nextRoom.SetDoor(doorBackFromNext, doorToNext.GetOppositeSite());
                    actualRoom = nextRoom;


                }



            }
            for(int i =0 ;i<rooms.Count;i++)
            {
                rooms[i].BuildWalls();
            }
            rooms[0].SpawnActeur(Player.Get());
        }

        public void nextLevel()
        {
            level++;
            if (level == 1) background = AssetManager.GetTexture("bgd1");
            if (level == 2) background = AssetManager.GetTexture("castleBG");
            if (level == 3) background = AssetManager.GetTexture("towerBGKopie");
            if (level == 4) background = AssetManager.GetTexture("towerBGKopie");
        }

        public void previousLevel()
        {
            level--;
            if (level == 1) background = AssetManager.GetTexture("bgd1");
            if (level == 2) background = AssetManager.GetTexture("castleBG");
            if (level == 3) background = AssetManager.GetTexture("towerBGKopie");
            if (level == 4) background = AssetManager.GetTexture("towerBGKopie");
        }

        public void SameLevel()
        {
            if (level == 1) background = AssetManager.GetTexture("bgd1");
            if (level == 2) background = AssetManager.GetTexture("castleBG");
            if (level == 3) background = AssetManager.GetTexture("towerBGKopie");
            if (level == 4) background = AssetManager.GetTexture("towerBGKopie");
        }




        public void ReloadWalls()
        {
            activeRoom.ReloadWalls();
        }

        public bool DetacteCollison(Rectangle hitbox, Color[] playerImage, bool deleteBots = true)
        {
            return activeRoom == null ? false : activeRoom.DetacteCollison(hitbox, playerImage, deleteBots);
        }

        public Door DetacteCollisonDoor(Rectangle hitbox, bool deleteBots = true)
        {
            Door linkedDoor = activeRoom.DetacteCollisonDoor(hitbox, deleteBots);
            if (linkedDoor != null)
            {
                if (linkedDoor is EndDoor && Player.Get().coins < ((EndDoor)linkedDoor).goldNeeded) return null;
                if (linkedDoor is EndDoor) Player.Get().coins -= ((EndDoor)linkedDoor).goldNeeded;
                roomIndex = rooms.IndexOf(linkedDoor.room);
                activeRoom = linkedDoor.room;
                return linkedDoor;
            }
            return null;

        }
        public void DebugFieldsofRoom()
        {
            int xCount = activeRoom.fields.GetLength(0);
            int yCount = activeRoom.fields.GetLength(1);
            for (int col = 0; col < yCount; col++)
            {
                int trueCount = 0;
                int falseCount = 0;
                for (int row = 0; row < xCount; row++)
                {
                    if (activeRoom.fields[row, col])
                    {
                        trueCount++;
                    }
                    else
                    {
                        falseCount++;
                    }


                }

                Console.Write(string.Format(" Wall:{0} Air: {1}", trueCount, falseCount));
                Console.WriteLine("");
            }
        }

        public void Draw(GameTime gameTime)
        {
            Rectangle screenRectangle = new Rectangle(0, 0, 1280, 720);
            GameStateManagementGame._spriteBatch.Draw(background, screenRectangle, null, Color.White, 0, new Vector2(0,0), SpriteEffects.None, 1.0f);
            activeRoom.Draw(gameTime);

        }


        public void Update(GameTime gameTime)
        {
            activeRoom.Update(gameTime);
        }


        public Room GetActiveRoom()
        {
            return activeRoom;
        }





    }
}