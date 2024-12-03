using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using wizard_game;

class Node
{
    private int x ;
    private int y ;
    private Vector2 direction;
    float initValue;
    float currentValue;
    public Node(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public int GetX()
    {
        return x;
    }

    public void SetX(int x)
    {
        this.x = x;
    }

    public int GetY()
    {
        return y;
    }

    public void SetY(int y)
    {
        this.y = y;
    }

    public float GetValue()
    {
        return currentValue;
    }

    public void SetValue(float value)
    {
        currentValue = value;
    }

    public Microsoft.Xna.Framework.Vector2 GetDirection()
    {
        return direction;
    }

    public void SetDirection(Microsoft.Xna.Framework.Vector2 direction)
    {
        this.direction = direction;
    }

    public override bool Equals(object o)
    {
        if (this == o) return true;

        if (o == null || GetType() != o.GetType()) return false;

        Node node = (Node)o;

        if (GetX() != node.GetX() || GetY() != node.GetY())
            return false;
        return true;
    }

    public float getInitValue()
    {
        return initValue;
    }

    public void setInitValue(float initValue)
    {
        this.initValue = initValue;
    }


    public override int GetHashCode()
    {
        return x.GetHashCode() ^ y.GetHashCode();
    }

    // public LinkedList<Node> expand()
    // {

    //     //TO DO: alle Nachfolger des Nodes
    //     LinkedList<Node> nachfolger = new LinkedList<Node>();
    //     if (View[x + 1, y] == Gamestate.PLAYER || View[x + 1, y] == Gamestate.EMPTY)
    //     {
    //         Gamestate[,] copiedView = copyView(View);
    //         copiedView[x, y] = Gamestate.EMPTY;
    //         copiedView[x + 1, y] = Gamestate.EMPTY;
    //         Node tmp = new Node(copiedView, x + 1, y, this, EnemyAction.GO_EAST, Kosten + 1);

    //         nachfolger.AddLast(tmp);
    //     }

    //     if (View[x - 1, y] == Gamestate.PLAYER || View[x - 1, y] == Gamestate.EMPTY)
    //     {
    //         Gamestate[,] copiedView = copyView(View);
    //         copiedView[x, y] = Gamestate.EMPTY;
    //         copiedView[x - 1, y] = Gamestate.EMPTY;
    //         Node tmp = new Node(copiedView, x - 1, y, this, EnemyAction.GO_WEST, Kosten + 1);

    //         nachfolger.AddLast(tmp);
    //     }

    //     if (View[x, y + 1] == Gamestate.PLAYER || View[x, y + 1] == Gamestate.EMPTY)
    //     {

    //         Gamestate[,] copiedView = copyView(View);
    //         copiedView[x, y] = Gamestate.EMPTY;
    //         copiedView[x, y + 1] = Gamestate.EMPTY;
    //         Node tmp = new Node(copiedView, x, y + 1, this, EnemyAction.GO_SOUTH, Kosten + 1);

    //         nachfolger.AddLast(tmp);
    //     }

    //     if (View[x, y - 1] == Gamestate.PLAYER || View[x, y - 1] == Gamestate.EMPTY)
    //     {
    //         Gamestate[,] copiedView = copyView(View);
    //         copiedView[x, y] = Gamestate.EMPTY;
    //         copiedView[x, y - 1] = Gamestate.EMPTY;
    //         Node tmp = new Node(copiedView, x, y - 1, this, EnemyAction.GO_NORTH, Kosten + 1);

    //         nachfolger.AddLast(tmp);
    //     }
    //     return nachfolger;
    // }

    // private Gamestate[,] copyView(Gamestate[,] view)
    // {
    //     //TO DO: implementieren copyView
    //     throw new NotImplementedException();
    // }
}

