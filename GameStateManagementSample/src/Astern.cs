using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics.PackedVector;

namespace wizard_game
{
    class AStern
    {

        private List<NodeA> closedList;
        private List<NodeA> openList;
    Vector2 positionGold;

        public AStern(Vector2 positionGold)
        {
            closedList = new List<NodeA>();
            openList = new List<NodeA>();
            this.positionGold = positionGold;
        }

        public NodeA Start(NodeA startNode)
        {
            if(positionGold.X < 0) return null;
            openList.Add(startNode);
            while (openList.Count > 0)
            {
                 openList.Sort((n1,n2) => (n1.GetCost() + Math.Abs((int)positionGold.X - n1.GetX()) + Math.Abs((int)positionGold.Y - n1.GetY())).CompareTo(
                                          n2.GetCost() + Math.Abs((int)positionGold.X - n2.GetX()) + Math.Abs((int)positionGold.Y - n2.GetY())));
                NodeA currentNode = openList[0];
                openList.RemoveAt(0);

                if (Math.Abs(currentNode.GetX() - (int)positionGold.X) < 2 && Math.Abs(currentNode.GetY() - (int)positionGold.Y) < 2)
                {
                   Console.WriteLine("found solution!");
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

            Console.WriteLine("No solution found.");
            return null;
        }
    }
}