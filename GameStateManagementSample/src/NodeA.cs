using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using wizard_game;

namespace wizard_game{
class NodeA
{
    private int x;
    private int y;
    private int cost;
    private int goldCount;
    private NodeA parent;
    private EnemyAction nextAction;
    private float currentValue;
    private Gamestate[,] view;

    public NodeA(Gamestate[,] view, int x, int y, NodeA parent, EnemyAction direction, int stepCost)
    {
        this.view = view;
        this.x = x;
        this.y = y;
        this.parent = parent;
        this.nextAction = direction;
        this.cost = stepCost;
       setGoldCount(getGoldCount());
        this.currentValue = 0;
    }

    // Getter und Setter
    public int GetX() => x;
    public void SetX(int x) => this.x = x;

    public int GetY() => y;
    public void SetY(int y) => this.y = y;

    public float GetValue() => currentValue;
    public void SetValue(float value) => currentValue = value;

    public NodeA GetParent() => parent;

    public Vector2 GetDirectionVector() {
        switch(nextAction){
            case EnemyAction.GO_EAST:
            return new Vector2(1, 0);
            case EnemyAction.GO_WEST:
            return new Vector2(-1, 0);
            case EnemyAction.GO_SOUTH:
            return new Vector2(0, 1);
            default:
            return new Vector2(0, -1);
    }
    }


    // Equals und GetHashCode, um Vergleiche zu erleichtern
    public override bool Equals(object obj)
    {
        if (this == obj) return true;

        if (obj == null || GetType() != obj.GetType()) return false;

        NodeA other = (NodeA)obj;
        return x == other.x && y == other.y;
    }

    public override int GetHashCode() => x.GetHashCode() ^ y.GetHashCode();

    // Nachfolger erstellen
    public LinkedList<NodeA> Expand()
    {
   //     Console.WriteLine("expand");
        LinkedList<NodeA> successors = new LinkedList<NodeA>();

        // Prüfen und Hinzufügen der möglichen Nachbarn (Bewegungsrichtungen)
        AddSuccessor(successors, x + 1, y, EnemyAction.GO_SOUTH);
        AddSuccessor(successors, x - 1, y, EnemyAction.GO_NORTH);
        AddSuccessor(successors, x, y + 1, EnemyAction.GO_EAST);
        AddSuccessor(successors, x, y - 1, EnemyAction.GO_WEST);

        return successors;
    }

    private void AddSuccessor(LinkedList<NodeA> successors, int newX, int newY, EnemyAction action)
    {
    //    Console.WriteLine(view[newX, newY]+ "-x-"+newX+"-y-"+newY);
        if (IsInBounds(newX, newY) && view[newX, newY] != Gamestate.WALL)
        {
            Gamestate[,] copiedView = CopyView(view);
            copiedView[x, y] = Gamestate.EMPTY;
            copiedView[newX, newY] = Gamestate.ENEMY;

            NodeA successor = new NodeA(copiedView, newX, newY, this, action, cost + 1);
        //  Console.WriteLine(successor.GetX() + "-" + successor.GetY() + "-"+ successor.GetCost());
            successors.AddLast(successor);
        }
    }

    private bool IsInBounds(int newX, int newY)
    {
        return newX >= 0 && newX < view.GetLength(0) && newY >= 0 && newY < view.GetLength(1);
    }

    // Kopie des aktuellen View-Arrays erstellen
    private Gamestate[,] CopyView(Gamestate[,] originalView)
    {
        int rows = originalView.GetLength(0);
        int cols = originalView.GetLength(1);
        Gamestate[,] copiedView = new Gamestate[rows, cols];

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                copiedView[i, j] = originalView[i, j];
            }
        }

        return copiedView;
    }

    public void setGoldCount(int count){
        goldCount = count;
    }

    public int getGoldCount(){
        int goldCount = 0;
		for (int i = 0; i < view.GetLength(0); i++) {
			for(int j = 0; j < view.GetLength(1); j++) {
				if(view[i, j] == Gamestate.GOLD) {
                  //  Console.WriteLine("Gold "+i + "-"+j);
					goldCount++;
				}
			}
		}
        Console.WriteLine("---------------------------------------------");
		return goldCount;
    }

    public int GetCost(){
        return cost;
    }
}
}
