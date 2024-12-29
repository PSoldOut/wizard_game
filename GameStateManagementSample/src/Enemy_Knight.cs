using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
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
        bool aStarThreadIsRunning = false;
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
            if (pathToGold.Count == 0 && !aStarThreadIsRunning)
            {
                aStarThreadIsRunning = true;
                Task.Run(() =>
                {
                    cordXInGamestate = (int)(position.X + width/2) / 10;
                    cordYInGamestate = (int)(position.Y + height/2) / 10;
                    currentView = room.gamestate;
                    startNode = new NodeA(currentView, cordXInGamestate, cordYInGamestate, null, EnemyAction.NONE, 0);
                    solutionNode = AStar(startNode);
                    //Wenn es noch Gold gibt
                    while (solutionNode != null)
                    {
                        pathToGold.Add(new Vector2(solutionNode.GetX() * 10, solutionNode.GetY() * 10));
                        solutionNode = solutionNode.GetParent();
                    }
                    aStarThreadIsRunning = false;
                });

            }

            if (pathToGold.Count >= 1 && moveToTarget(pathToGold[^1]))
            {
                pathToGold.RemoveAt(pathToGold.Count - 1);
                if (pathToGold.Count > 0)
                {
                }
                else
                {
                    gold?.Effect();//soll angepasst werden:gold nicht zu player hinzugefügt
                }
            }

        }

        public new NodeA AStar(NodeA startNode)
        {
            List<NodeA> closedList = new List<NodeA>();
            List<NodeA> openList = [startNode];
            Vector2 target = new Vector2(0, 0);
            foreach (Item item in GameplayScreen.items)
            {
                if (item is Gold gold && currentView[(int)item.position.X/10, (int)item.position.Y/10]!= Gamestate.WALL)
                {
                    target =  new Vector2(gold.position.X + gold.width/2, gold.position.Y + gold.height/2)/10;
                    this.gold = gold;
                    break;
                }
            }

            while (openList.Count > 0)
            {
                openList.Sort((n1, n2) => (n1.GetCost() + Math.Abs((int)target.X - n1.GetX()) + Math.Abs((int)target.Y - n1.GetY())).CompareTo(
                                          n2.GetCost() + Math.Abs((int)target.X - n2.GetX()) + Math.Abs((int)target.Y - n2.GetY())));
                NodeA currentNode = openList[0];
                openList.RemoveAt(0);

                if (Math.Abs(currentNode.GetX() - (int)target.X) < 0.5 && Math.Abs(currentNode.GetY() - (int)target.Y) < 0.5)
                {
                    currentView[currentNode.GetX(), currentNode.GetY()] = Gamestate.EMPTY;
                    return currentNode;
                }

                if (!closedList.Contains(currentNode))
                {
                    closedList.Add(currentNode);
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