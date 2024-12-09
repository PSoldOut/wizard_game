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

    
}

