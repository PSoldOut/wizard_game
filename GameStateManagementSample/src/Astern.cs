using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Graphics.PackedVector;

namespace wizard_game
{
    class AStern
    {

        private List<NodeA> closedList;
        private List<NodeA> openList;


        public AStern()
        {
            closedList = new List<NodeA>();
            openList = new List<NodeA>();
        }

        public NodeA Start(NodeA startNode)
        {

            openList.Add(startNode);
            while (openList.Count > 0)
            {
                openList.Sort((n1,n2) => (n1.GetCost() + n1.getGoldCount()).CompareTo(n2.GetCost()
                +n2.getGoldCount()));
                NodeA currentNode = openList[0];
                openList.RemoveAt(0);

                // Zielüberprüfung (z.B. alle Goldmünzen aufgesammelt)
                if (currentNode.getGoldCount() == 5)
                {
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

          //  Console.WriteLine("No solution found.");
            return null;
        }
    }
}