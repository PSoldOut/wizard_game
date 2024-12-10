

using System;
using System.Diagnostics;
using GameStateManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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
        public float speed;
        private EnemyType e_type;
        private EnemyState e_state;

        private bool drawHitBox = true;
        private int lineWidth = 2;
        private Color hitboxColor = Color.Purple;
        public Room room;

        // private Rectangle rect { get; set; }
        protected Map map;
        public Enemy(int x, int y, Map _map, EnemyType type, string spriteName, Room room) : base(new Vector2(x, y), 27, 45, spriteName, true)
        {
            direction = new Vector2(0, -1);
            map = _map;
            e_type = type;
            setSpeed();
            this.room = room;

            //hab das in konstruktor verschoben weil es loadContent nicht mehr gibt. wir haben ja DrawableGameComponent rausgenommen
            //Load Content: Texture für Enemy
            sprite = new Sprite(GameStateManagementGame.Get().Content.Load<Texture2D>(spritename), 4, 4, 1, true);
            InitAnimations();
            enemy_texture = new Texture2D(GameStateManagementGame.Get().GraphicsDevice, 1, 1);
            enemy_texture.SetData(new Color[] { Color.Black });
            //hitBox = getNextRect(position);
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



        // Die neue Hitbox bei der Bewegung
        public Rectangle getNextRect(Vector2 position)
        {
            return new Rectangle((int)position.X, (int)position.Y, enemy_texture.Width,
                                                 enemy_texture.Height);
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