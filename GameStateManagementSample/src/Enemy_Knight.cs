using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using GameStateManagement;
using Microsoft.Xna.Framework;

namespace wizard_game
{
    class Enemy_Knight : Enemy
    {
        int prop = 10;
        Map map;
        private NodeA solutionNode;
        private LinkedList<NodeA> pathToGold;
        private NodeA startNode;
        Gamestate[,] currentView;
        List<Vector2> goldPositions;
        int cordXInGamestate;
        int cordYInGamestate;
        public Enemy_Knight(int x, int y, Map map, EnemyType type, Room room) : base(x, y, map, type, "spriteSheetEnemy_Knight", room)
        {
            setSpeed();
            this.map = map;
            direction = new Vector2(0, 0);

        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            CheckForItems();

           // room.PrintView();
            // return;
            cordXInGamestate = (int)position.X / prop;
            cordYInGamestate = (int)position.Y / prop;
            currentView = room.gamestate;
            pathToGold = new LinkedList<NodeA>();
            //Wenn es noch Gold gibt
            if (solutionNode == null)
            {
              //  Console.WriteLine("solution ist null");
                startNode = new NodeA(currentView, cordXInGamestate, cordYInGamestate, null, EnemyAction.NONE, 0);
                AStern search = new AStern();
                //solutionNode = search.Start(startNode);
                solutionNode = AStar(startNode);
           //     Console.Write("solution was found");
                //throw new Exception("jej  X:" + solutionNode.GetX() + "  Y:" + solutionNode.GetY());
                NodeA tmp = solutionNode;
                while (tmp != null)
                {
                    pathToGold.AddLast(tmp);
                    tmp = tmp.GetParent();
                }
            }
            if (solutionNode != null)
            {
             //   Console.WriteLine("solution is not null");
                while (pathToGold.Count > 0)
                {
                    NodeA n = pathToGold.Last.Value;
                    pathToGold.RemoveLast();
                    if (n.Equals(startNode))
                    {
                        n = pathToGold.Last.Value;
                        pathToGold.RemoveLast();
                    }
                    position += n.GetDirectionVector() * speed;
                    return;
                }
            }

            sprite.Update(gameTime);

        }


        public new void Attack()
        {

        }

        public void CheckForItems()
        {
            foreach (Item item in GameplayScreen.items)
            {
                if (item.state == Item.State.ON_FLOOR && item.area.Intersects(hitBox))
                {
                    item.Effect();
                    room.setGamestateElement(position, Gamestate.EMPTY);

                    break;
                }

            }
        }




        public NodeA AStar(NodeA startNode)
        {
            List<NodeA> closedList = new List<NodeA>();
            List<NodeA> openList = [startNode];
            while (openList.Count > 0)
            {
            //    Console.WriteLine(openList.Count);
                openList.Sort((n1,n2) => (n1.GetCost() + Math.Abs((int)(Player.Get().position.X/10) - n1.GetX()) + Math.Abs((int)(Player.Get().position.Y/10) - n1.GetY())).CompareTo(
                                          n2.GetCost() + Math.Abs((int)(Player.Get().position.X/10) - n2.GetX()) + Math.Abs((int)(Player.Get().position.Y/10) - n2.GetY())));
                NodeA currentNode = openList[0];
                openList.RemoveAt(0);

                if (Math.Abs(currentNode.GetX() - (int)(Player.Get().position.X/10)) < 2 && Math.Abs(currentNode.GetY() - (int)(Player.Get().position.Y/10)) < 2)
                {
                  //  Console.WriteLine("found solution!");
                    return currentNode;
                }



                // Wenn der aktuelle Knoten noch nicht verarbeitet wurde, füge ihn der Closed-List hinzu
                if (!closedList.Contains(currentNode))
                {
                    closedList.Add(currentNode);

                    // Expandieren der Nachfolgerknoten
                    var successors = currentNode.Expand();
                    foreach (var successor in successors){
                        if(!openList.Contains(successor)){
                            openList.Add(successor);
                        }
                    }

                }
            }

        //    Console.WriteLine("No solution found.");
            return null;
        }



    }
}