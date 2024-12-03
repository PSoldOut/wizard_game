using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;

namespace wizard_game
{
    class Enemy_prisoner : Enemy
    {
        Map map;
        public Enemy_prisoner(int x, int y, Map map, EnemyType type, Room room) : base(x, y, map, type, "spriteSheetEnemy_Prisoner", room)
        {
            setSpeed();
            this.map = map;
            direction = new Vector2(1, 0);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            Move();

            sprite.Update(gameTime);

        }

        //Bewegung des Gegners: wenn kein Objekt auf dem Weg zu Spieler gibt, dann verfolgt er spieler
        public new void Move()
        {
            if (!IsObjBetweenEnemyAndPlayer())
            {
                //Debug.WriteLine("wall between");
                follow();
            }
            else
            {

            }
        }

        //Den Spieler verfolgen
        public void follow()
        {
            if (hitBox.X < 1240 && hitBox.X >= 0 && hitBox.Y >= 0 && hitBox.Y < 1000)
            {
                MoveToPlayer();
                Vector2 test = position + direction * speed;

                if (!DetacteCollison(test))
                {
                    //  Debug.WriteLine("kollision");
                    direction.Normalize();
                    position = test;
                }
            }
        }

        //bewegen sich  nach der Richtung des Players
        public void MoveToPlayer()
        {
            //Debug.WriteLine("Attacking!!!!!!!!!!!!!!!!!!!!!!!!!");
            if (position.X < Player.Get().position.X)
            {
                direction.X = 1;
                direction.Y = 0;
                sprite.setAnimation("idle_right");
            }
            else if (position.Y > Player.Get().position.Y)
            {
                direction.Y = -1;
                direction.X = 0;
                sprite.setAnimation("idle_up");
            }
            else if (position.Y < Player.Get().position.Y)
            {
                direction.Y = 1;
                direction.X = 0;
                sprite.setAnimation("idle_down");
            }
            else
            {
                direction.X = -1;
                direction.Y = 0;
                sprite.setAnimation("idle_left");
            }
        }

        //überprüfen, ob Hindernisse (Wand) zw Player und Enemy vorliegen
        private bool IsObjBetweenEnemyAndPlayer()
{
    // Spieler- und Gegnerposition abrufen
    Vector2 playerPos = Player.Get().position;
    Vector2 enemyPos = position;
    bool[,] fields = room.fields; // Wände oder Hindernisse

    // Positionen in Raster-Koordinaten umrechnen (z. B. bei 10 Pixel pro Feld)
    int x1 = (int)enemyPos.X / 10;
    int y1 = (int)enemyPos.Y / 10;
    int x2 = (int)playerPos.X / 10;
    int y2 = (int)playerPos.Y / 10;

    if(x1 == x2){
        //Debug.WriteLine("x==");
       return DetectObjInDirectionY(x1, y1, y2, fields);
    }

     if(y1 == y2){
        //Debug.WriteLine("y==");
       return DetectObjInDirectionX(y1, x1, x2, fields);
    }

    // Bresenham-Algorithmus, um die Linie abzutasten
    int dx = Math.Abs(x2 - x1);
    int dy = Math.Abs(y2 - y1);
    int sx = x1 < x2 ? 1 : -1; // Schritt in x-Richtung
    int sy = y1 < y2 ? 1 : -1; // Schritt in y-Richtung
    int err = dx - dy;



    // Keine Wand auf der Linie gefunden
    return false;
}

public bool DetectObjInDirectionY (int x1, int y1, int y2, bool[,] fields){

    if(y1 <y2){
        for(int i = y1; i < y2; i++){
            if(fields[x1,i]){
                return true;
            }
        }
    }
    return false;
}

public bool DetectObjInDirectionX (int y1, int x1, int x2, bool[,] fields){

    if(x1 < x2){
        for(int i = x1; i < x2; i++){
            if(fields[i,y1]){
                return true;
            }
        }
    }
    return false;
}
    }
}