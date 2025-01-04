

using System;

using GameStateManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;


namespace wizard_game
{
    class Enemy : Acteur
    {

        public enum EnemyState
        {
            ATTACKING,
            DIE,
            NORMAL
        }
        public Texture2D enemy_texture;
        private EnemyType e_type;
        private EnemyState e_state;
        public int expDrop;
        private bool drawHitBox = true;
        private int lineWidth = 2;
        private Color hitboxColor = Color.Purple;
        public Room room;
        public float currentSpeed;
        protected List<Vector2> path = new List<Vector2>();
        protected float playerViewDistance;
        protected Map map;
        Vector2 patroulliePoint1;
        Vector2 patroulliePoint2;
        public Enemy(int x, int y, Map _map, EnemyType type, string spriteName, Room room) : base(new Vector2(x, y), 27, 45, spriteName, true)
        {

            direction = new Vector2(0, -1);
            map = _map;
            e_type = type;
            setSpeed();
            this.room = room;

            //hab das in konstruktor verschoben weil es loadContent nicht mehr gibt. wir haben ja DrawableGameComponent rausgenommen
            //Load Content: Texture für Enemy
            LoadSprite(4, 4, 1, true);
            sprite.offset = new Vector2(width / 2, height / 2);
            sprite.origin = new Vector2(width / 2, height / 2);
            sprite.layerDepth = NEXT_LAYER_DEPTH;
            InitAnimations();
            enemy_texture = new Texture2D(GameStateManagementGame.Get().GraphicsDevice, 1, 1);
            enemy_texture.SetData(new Color[] { Color.Black });
            expDrop = 100;
            currentSpeed = 0;
            playerViewDistance = 300;
            patroulliePoint1 = position;
            patroulliePoint2 = CalculatePatroulliePoint2();
            Vector2 startPos = GenerateRandomPosition();
            SetPosition(startPos);
        }


        protected Vector2 GetNextPatroulliePoint()
        {
            if (Math.Abs((GetMidPos() - patroulliePoint1).Length()) < Math.Abs((GetMidPos() - patroulliePoint2).Length())) return patroulliePoint2;
            return patroulliePoint1;
        }


        // Animation setzen
        private void InitAnimations()
        {
            int[] animDown = { 0, 1, 2, 3 };
            int[] animLeft = { 4, 5, 6, 7 };
            int[] animRight = { 8, 9, 10, 11 };
            int[] animUp = { 12, 13, 14, 15 };
            int[] animIdleDown = { 0 };
            int[] animIdleLeft = { 4 };
            int[] animIdleRight = { 8 };
            int[] animIdleUp = { 12 };
            sprite.addAnimtaion(animDown, "down");
            sprite.addAnimtaion(animLeft, "left");
            sprite.addAnimtaion(animRight, "right");
            sprite.addAnimtaion(animUp, "up");
            sprite.addAnimtaion(animIdleDown, "idle_down");
            sprite.addAnimtaion(animIdleLeft, "idle_left");
            sprite.addAnimtaion(animIdleRight, "idle_right");
            sprite.addAnimtaion(animIdleUp, "idle_up");
            sprite.setAnimation("idle_right");
        }

        private bool IsObjectInFields(Vector2 test)
        {
            // Berechne die Positionen in den Feldern (Index in room.fields)
            if (!isValidPos(test))
            {
                return true;
            }
            int cordX = (int)test.X / 10;
            int cordY = (int)test.Y / 10;

            // Gibt zurück, ob das Feld auf true gesetzt ist
            return room.fields[cordX, cordY];

        }

        //Kollision mit Wall erkennen
        public bool DetacteCollison(Vector2 newPos)
        {

            return IsObjectInFields(newPos);
        }

        public bool detectCollisionWithRec(Rectangle rec)
        {
            if (map == null)
            {
                throw new InvalidOperationException("Map is not initialized.");
            }

            return map.DetacteCollison(rec, null, false);
        }

        public Vector2 GenerateRandomPosition()
        {
            Random random = new Random();
            Vector2 randomPosition;

            do
            {
                // Generiere eine zufällige Position innerhalb des Bildschirms
                int randomX = random.Next(0, GameStateManagementGame.Get().graphics.GraphicsDevice.Viewport.Width - width);
                int randomY = random.Next(0, GameStateManagementGame.Get().graphics.GraphicsDevice.Viewport.Height - height);

                // Die linke obere Ecke des Gegners
                randomPosition = new Vector2(randomX, randomY);

                // Erstelle ein Rechteck, das den Gegner darstellt
                Rectangle rec = new Rectangle(randomX, randomY, width, height );

                // Prüfe, ob das Rechteck keine Kollision hat
                if (!detectCollisionWithRec(rec))
                {
                    // Wenn keine Kollision, dann verlasse die Schleife
                    break;
                }

            } while (true);

            return randomPosition;
        }
        public bool DetacteCollison()
        {
            Color[] data = sprite.GetCurrentColorData();
            return map.DetacteCollison(hitBox, data, false);
        }

        //wenn Kollision kommt, dann bleibe
        public void DetacteCollisonX(Vector2 posOld)
        {

            hitBox.X = (int)position.X;
            if (DetacteCollison(posOld))
            {
                position.X = posOld.X;
            }
            hitBox.X = (int)position.X;

        }
        public void DetacteCollisonY(Vector2 posOld)
        {
            hitBox.Y = (int)position.Y;
            if (DetacteCollison(posOld))
            {
                position.Y = posOld.Y;
            }
            hitBox.Y = (int)position.Y;
        }

        //TODO: move()
        public override void Update(GameTime gameTime)
        {

            base.Update(gameTime);
            Move();
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            if (GameStateManagementGame.mode == GameMode.DEBUG)
            {
                foreach (Vector2 v in path)
                {
                    drawPoint(v);
                }
            }
        }

        // Je nach EnemyType wird doe Geschwindigkeit gesetzt
        public void setSpeed()
        {
            switch (e_type)
            {
                case EnemyType.WIZARD:
                    speed = 2.3f;
                    break;
                case EnemyType.GUARD:
                    speed = 1.3f;
                    break;
                case EnemyType.SKELETON:
                    speed = 0.3f;
                    break;
                case EnemyType.PRISONER:
                    speed = 1.5f;
                    break;
                case EnemyType.DOUBLER:
                    speed = 0.9f;
                    break;
                case EnemyType.KNIGHT:
                    speed = 3f;
                    break;
                default:
                    speed = 0.7f;
                    break;
            }
        }

        public void SetEnemyState(EnemyState enemyState)
        {
            e_state = enemyState;
        }

        public EnemyState GetEnemyState()
        {
            return e_state;
        }

        public void Move()
        {

        }


        //target is in pixel
        public List<Vector2> calculatePathToTarget(Vector2 target)
        {
            List<Vector2> p = new List<Vector2>();
            target /= 10;
            int cordXInGamestate = (int)(position.X + width / 2) / 10;
            int cordYInGamestate = (int)(position.Y + height / 2) / 10;
            Gamestate[,] currentView = room.gamestate;
            NodeA startNode = new NodeA(currentView, cordXInGamestate, cordYInGamestate, null, EnemyAction.NONE, 0);
            NodeA solutionNode = AStar(startNode, target);
            while (solutionNode != null)
            {
                p.Add(new Vector2(solutionNode.GetX() * 10, solutionNode.GetY() * 10));
                solutionNode = solutionNode.GetParent();
            }
            return p;
        }


        //rechnen Abstand zwischen Player und Gegner
        public float caculateDistance()
        {
            float distanceX = position.X + width / 2 - Player.Get().position.X - Player.Get().width / 2;
            float distanceY = position.Y + height / 2 - Player.Get().position.Y - Player.Get().height / 2;
            return (float)Math.Sqrt(Math.Pow(distanceX, 2) + Math.Pow(distanceY, 2));
        }

        public bool isWall(int x, int y)
        {
            return false;
        }

        public bool isValidPos(Vector2 pos)
        {
            return pos.X >= 0 && pos.Y >= 0 && pos.X < 1280 - enemy_texture.Width && pos.Y < 780 - enemy_texture.Height;
        }

        public override void Attack()
        {

        }




        public bool moveToTarget(Vector2 target)
        {
            if ((target - GetMidPos()).Length() < 20) return true;
            direction = target - GetMidPos();
            direction.Normalize();
            //Console.WriteLine("direction:" + direction);
            position += direction * speed;
            currentSpeed = speed;
            return false;
        }


        protected Vector2 CalculatePatroulliePoint2()
        {
            //calculating pattroulliePoint2
            Rectangle oldRect = hitBox;
            Vector2 oldPosition = position;
            Vector2 result;
            position = new Vector2(GameplayScreen.rand.Next(0, GameStateManagementGame.Get().graphics.PreferredBackBufferWidth), GameplayScreen.rand.Next(0, GameStateManagementGame.Get().graphics.PreferredBackBufferHeight));
            hitBox.X = (int)position.X - 10;
            hitBox.Y = (int)position.Y - 10;
            while (DetacteCollison())
            {
                position = new Vector2(GameplayScreen.rand.Next(0, GameStateManagementGame.Get().graphics.PreferredBackBufferWidth), GameplayScreen.rand.Next(0, GameStateManagementGame.Get().graphics.PreferredBackBufferHeight));
                hitBox.X = (int)position.X - 10;
                hitBox.Y = (int)position.Y - 10;
            }
            result = position;
            position = oldPosition;
            hitBox = oldRect;
            return result;
        }


        public NodeA AStar(NodeA startNode, Vector2 target)
        {
            List<NodeA> closedList = new List<NodeA>();
            List<NodeA> openList = [startNode];

            while (openList.Count > 0)
            {
                //    Console.WriteLine(openList.Count);
                openList.Sort((n1, n2) => (n1.GetCost() + Math.Abs((int)target.X - n1.GetX()) + Math.Abs((int)target.Y - n1.GetY())).CompareTo(
                                          n2.GetCost() + Math.Abs((int)target.X - n2.GetX()) + Math.Abs((int)target.Y - n2.GetY())));
                NodeA currentNode = openList[0];
                openList.RemoveAt(0);

                if (Math.Abs(currentNode.GetX() - (int)target.X) < 2 && Math.Abs(currentNode.GetY() - (int)target.Y) < 2)
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







        public override void Die()
        {
            base.Die();
            Player.Get().PickupExp(expDrop);
            particleSystem.AddExpEffect(GetMidPos(), Color.Wheat, expDrop.ToString());

        }



        public void MoveToPlayer()
        {
            float deltaX = Player.Get().position.X - position.X;
            float deltaY = Player.Get().position.Y - position.Y;

            // Bestimme, ob Bewegung entlang X oder Y priorisiert wird
            if (Math.Abs(deltaX) > Math.Abs(deltaY)) // Bewegung entlang der X-Achse
            {
                if (deltaX > 0)
                {
                    direction.X = 1; // nach rechts
                    sprite.setAnimation("idle_right");
                }
                else
                {
                    direction.X = -1; // nach links
                    sprite.setAnimation("idle_left");
                }
                direction.Y = 0; // Nur entlang der X-Achse bewegen
            }
            else // Bewegung entlang der Y-Achse
            {
                if (deltaY > 0)
                {
                    direction.Y = 1; // nach unten
                    sprite.setAnimation("idle_down");
                }
                else
                {
                    direction.Y = -1; // nach oben
                    sprite.setAnimation("idle_up");
                }
                direction.X = 0; // Nur entlang der Y-Achse bewegen
            }

            //Debug.WriteLine(direction + " direction");
        }

    }
}