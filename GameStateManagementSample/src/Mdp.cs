
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace wizard_game
{
    class Mdp
    {
        private Node Start;
        Node[,] value;
        Node[,] startValue;
        Node[,] copy;
        float gamma = 0.9f;
        float stepCost = -0.1f;
        Gamestate[,] view;
        Vector2 direction;
        public Mdp(Node start, Gamestate[,] view)
        {
            Start = start;
            this.view = view;
            value = new Node[view.GetLength(0), view.GetLength(1)];
        }

        //caculate the next direction to move
        public Vector2 nextDirection()
        {
            Vector2 direction = new Vector2(0, 0);
            init();
            startValue = copyMapValue();
            for (int i = 0; i < 50; i++)
            {
                direction = mapIterate();
            }
            return direction;
        }


        public Vector2 mapIterate()
        {
            Vector2 direction = new Vector2(0, 0);
            copy = copyMapValue();
            for (int i = 1; i < value.GetLength(0) - 1; i++)
            {
                for (int j = 1; j < value.GetLength(1) - 1; j++)
                {

                    if (view[i, j] != Gamestate.WALL)
                    {
                        Dictionary<Vector2, float> cost_action = new Dictionary<Vector2, float>
                    {
                        { new Vector2(1, 0), caculateNewValue(copy[i + 1, j]) },
                        { new Vector2(-1, 0), caculateNewValue(copy[i - 1, j]) },
                        { new Vector2(0, 1), caculateNewValue(copy[i, j + 1]) },
                        { new Vector2(0, -1), caculateNewValue(copy[i, j - 1]) }
                    };

                        float maxValue = float.NegativeInfinity;


                        foreach (KeyValuePair<Vector2, float> entry in cost_action)
                        {
                            if (entry.Value > maxValue)
                            {
                                maxValue = entry.Value;
                                if (i == Start.GetX() && j == Start.GetY())
                                {
                                    direction = entry.Key;
                                }
                            }
                        }

                        if (startValue[i, j].GetValue() < 0)
                        {
                            continue;
                        }

                        if (value[i, j].GetValue() < maxValue)
                            value[i, j].SetValue(maxValue);
                    }
                }
            }
            return direction;
        }


        public void init()
        {
            for (int i = 0; i < view.GetLength(0); i++)
            {
                for (int j = 0; j < view.GetLength(1); j++)
                {
                    value[i, j] = new Node(i, j);
                    value[i, j].SetValue(evaluateValue(i, j));
                }
            }
        }

        //copy the value of the current view
        public Node[,] copyMapValue()
        {
            Node[,] copy = new Node[value.GetLength(0), value.GetLength(1)];
            for (int i = 0; i < value.GetLength(0); i++)
            {
                for (int j = 0; j < value.GetLength(1); j++)
                {
                    Node k = new Node(i, j);
                    k.SetValue(value[i, j].GetValue());
                    copy[i, j] = k;
                }
            }
            return copy;
        }

        //Caculate value for the next field
        public float evaluateValue(int i, int j)
        {
            float tmp = 0f;
            if (!isValid(i, j))
            {
                tmp = -9999999.999f;
            }
            if (view[i, j] == Gamestate.WALL)
            {
                tmp = -10000;
            }
            else if (view[i, j] == Gamestate.EMPTY || view[i, j] == Gamestate.ITEM)
            {
                tmp = 0.1f;
            }
            else if (view[i, j] == Gamestate.GOLD)
            {
                tmp = 0.2f;
            }
            else if (view[i, j] == Gamestate.ENEMY)
            {
                tmp = -5;
            }
            else if (view[i, j] == Gamestate.PLAYER)
            {
                tmp = 10000;
            }

            return tmp;
        }

        //caculate the new value of the field based on the old one
        public float caculateNewValue(Node old)
        {
            return stepCost + gamma * old.GetValue();
        }

        public void printMap(int x, int y)
        {
            for (int i = 1; i < value.GetLength(0)-1; i++)
            {
                for (int j = 1; j < value.GetLength(1)-1; j++)
                {
                    if (view[i, j] == Gamestate.PLAYER)
                    {
                        Console.Write("Player-------------------"+ i+"-"+j);
                    }
                    if (x == i && y == j)
                    {
                        Console.Write("Enemy----.------------------" + value[i, j].GetValue());
                    }
                   Console.Write(value[i, j].GetValue() + "       ");
                }
                Console.WriteLine();
            }
            Console.WriteLine("----------------------");
        }


        //check if the field is valid (not outside the screen)
        public bool isValid(int i, int j)
        {
            return i >= 0 && j >= 0 && i < view.GetLength(0) && j < view.GetLength(1);
        }
    }
}