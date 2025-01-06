using System.Data.Common;
using System.IO.Pipes;
using Microsoft.Xna.Framework;

namespace wizard_game
{
    class Enemy_Wizard : Enemy
    {
        int prop = 10;
        Map map;
        Node startNode;
        public Enemy_Wizard(int x, int y, Map map, EnemyType type, Room room) : base(x, y, map, type, "spriteSheetEnemy_Wizard", room)
        {
            setSpeed();
            this.map = map;
            direction = new Vector2(0, 0);
        }


         public override void Update(GameTime gameTime)
        {

            base.Update(gameTime);
            startNode = new Node((int)position.X/prop, (int) position.Y/prop);
            Mdp mdp = new Mdp(startNode,room.gamestate);
         //   direction = mdp.nextDirection();
          //  mdp.printMap((int)position.X/prop, (int)position.Y/prop);
            if(direction.X == 1){
            sprite.setAnimation("idle_right");
            }else if(direction.X == -1){
                sprite.setAnimation("idle_left");
            }else if(direction.Y == 1){
                sprite.setAnimation("idle_down");
            }else{
                sprite.setAnimation("idle_up");
            }
        //    position += direction*speed;
            // MoveToPlayer();
            //  direction.Normalize();
            //     Vector2 test = position + direction * speed;

            //     if (!DetacteCollison(test))
            //     {
            //         // Debug.WriteLine("no kollision");
            //         position = test;
            //     }
            sprite.Update(gameTime);
        }
    }
}