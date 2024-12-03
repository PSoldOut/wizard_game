using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;

namespace wizard_game
{
    class Enemy_Knight : Enemy
    {
        Map map;
        public Enemy_Knight(int x, int y, Map map, EnemyType type, Room room) : base(x, y, map, type, "spriteSheetEnemy_Knight", room)
        {
            setSpeed();
            this.map = map;
            direction = new Vector2(0, 0);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            Move();

            sprite.Update(gameTime);

        }

        //Bewegung des Gegners: Gold fressen
        public new void Move()
        {
/*
Gold gold = Gold[0];
follow(GoldPos)
*/
        }

//A* Gold zu finden
        public void follow(Vector2 pos)
        {
/*
*/
        }

        public new void Attack(){

        }





    }
}