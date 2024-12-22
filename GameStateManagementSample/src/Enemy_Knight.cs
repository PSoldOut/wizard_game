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
           Gold gold = null;
        private NodeA solutionNode;
        private List<Vector2> pathToGold = new List<Vector2>();
        private NodeA startNode;
        Gamestate[,] currentView;
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
            if (isDying) return;
            //  CheckForItems();

            // room.PrintView();
            // return;
            if (pathToGold.Count == 0)
            {
                cordXInGamestate = (int)position.X / prop;
                cordYInGamestate = (int)position.Y / prop;
                currentView = room.gamestate;
                startNode = new NodeA(currentView, cordXInGamestate, cordYInGamestate, null, EnemyAction.NONE, 0);
                solutionNode = AStar(startNode);
                //Wenn es noch Gold gibt
                while (solutionNode != null)
                {
                  //  Console.WriteLine(solutionNode.GetX() + "---" + solutionNode.GetY());
                    pathToGold.Add(new Vector2(solutionNode.GetX() * 10, solutionNode.GetY() * 10));
                    solutionNode = solutionNode.GetParent();
                }
            }

            if (pathToGold.Count >= 1 && moveToTarget(pathToGold[^1]))
            {
                pathToGold.RemoveAt(pathToGold.Count - 1);
                if (pathToGold.Count > 0)
                {
                     Console.WriteLine($"count: {pathToGold.Count}, nextTarget: {pathToGold[^1]}");
                }
                else
                {
                    gold?.Effect();
                    //    Console.WriteLine("count: 0, nextTarget: None");
                }
            }

        }

        public NodeA AStar(NodeA startNode)
        {
            List<NodeA> closedList = new List<NodeA>();
            List<NodeA> openList = [startNode];
            Vector2 target = new Vector2(0, 0);
            foreach (Item item in GameplayScreen.items)
            {
                if (item is Gold gold)
                {
                    target = item.position / 10;
                    this.gold = gold;
                    break;
                }
            }
    Console.WriteLine(target);
            while (openList.Count > 0)
            {
                //    Console.WriteLine(openList.Count);
                openList.Sort((n1, n2) => (n1.GetCost() + Math.Abs((int)target.X - n1.GetX()) + Math.Abs((int)target.Y - n1.GetY())).CompareTo(
                                          n2.GetCost() + Math.Abs((int)target.X - n2.GetX()) + Math.Abs((int)target.Y - n2.GetY())));
                NodeA currentNode = openList[0];
                openList.RemoveAt(0);

                if (Math.Abs(currentNode.GetX() - (int)target.X) < 2 && Math.Abs(currentNode.GetY() - (int)target.Y) < 2)
                {
                    currentView[currentNode.GetX(), currentNode.GetY()] = Gamestate.EMPTY;
                    //  Console.WriteLine("found solution!");
                    return currentNode;
                }

                // Wenn der aktuelle Knoten noch nicht verarbeitet wurde, füge ihn der Closed-List hinzu
                if (!closedList.Contains(currentNode))
                {
                    closedList.Add(currentNode);

                    // Expandieren der Nachfolgerknoten
                    var successors = currentNode.Expand();
                    foreach (var successor in successors)
                    {
                        if (!openList.Contains(successor))
                        {
                            openList.Add(successor);
                        }
                    }
                }
            }

            //    Console.WriteLine("No solution found.");
            return null;
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            foreach (Vector2 p in pathToGold)
            {
                drawPoint(p);
            }
        }


    }
}