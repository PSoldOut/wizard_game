using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameStateManagement;
using Microsoft.Xna.Framework;

namespace wizard_game
{
    class Enemy_Knight : Enemy
    {
        Gold gold = null;
        private NodeA solutionNode;
        private NodeA startNode;
        Gamestate[,] currentView;
        bool aStarThreadIsRunning = false;
        int cordXInGamestate;
        int cordYInGamestate;
        private HashSet<Gold> checkedGold = new HashSet<Gold>();

        public Enemy_Knight(int x, int y, Map map, EnemyType type, Room room) : base(x, y, map, type, "spriteSheetEnemy_Knight", room)
        {
            setSpeed();
            this.map = map;
            direction = new Vector2(0, 0);
            expDrop = 50;
        }


        

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (isDying) return;
            if (path.Count == 0 && !aStarThreadIsRunning)
            {
                aStarThreadIsRunning = true;
                Task.Run(() =>
                {
                    cordXInGamestate = (int)(position.X + width / 2) / 10;
                    cordYInGamestate = (int)(position.Y + height / 2) / 10;
                    currentView = room.gamestate;
                    startNode = new NodeA(currentView, cordXInGamestate, cordYInGamestate, null, EnemyAction.NONE, 0);
                    solutionNode = AStar(startNode);
                    if (solutionNode == null)
                    {
                        aStarThreadIsRunning = false;
                        checkedGold.Add(gold);
                        return; // Thread beenden, wenn kein Pfad gefunden wurde
                    }
                    //Wenn es noch Gold gibt
                    while (solutionNode != null)
                    {
                        path.Add(new Vector2(solutionNode.GetX() * 10, solutionNode.GetY() * 10));
                        solutionNode = solutionNode.GetParent();
                    }
                    aStarThreadIsRunning = false;
                });

            }

            if (path.Count >= 1 && moveToTarget(path[^1]))
            {
                path.RemoveAt(path.Count - 1);
                if (path.Count == 0)
                {
                    Console.WriteLine(Vector2.Distance(position, gold.position));
                }
                if (path.Count == 0)
                {
                    gold?.Effect();
                    gold = null;
                }


            }

        }

        public void SearchForGold()
        {
            for (int i = 0; i < GameplayScreen.map.GetActiveRoom().items.Count; i++)
            {
                Item item = GameplayScreen.map.GetActiveRoom().items[i];
                Rectangle rec = new Rectangle((int)item.position.X, (int)item.position.Y, item.width, item.height);

                if (item is Gold gold && !checkedGold.Contains(gold))
                {
                    if (!gold.detectCollisionWithRec(rec, map))
                    {
                        this.gold = gold;
                        return;
                    }
                    else
                    {
                        checkedGold.Add(gold);
                    }
                }
                else continue;
            }
        }

        public NodeA AStar(NodeA startNode)
        {
            List<NodeA> closedList = new List<NodeA>();
            List<NodeA> openList = [startNode];
            SearchForGold();
            if (gold != null)
            {
                Vector2 target = new Vector2(gold.position.X + gold.width / 2, gold.position.Y + gold.height / 2) / 10;
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

            }
            return null;
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

        }
    }
}