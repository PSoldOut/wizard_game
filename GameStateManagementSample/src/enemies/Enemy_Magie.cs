using Microsoft.Xna.Framework;

namespace wizard_game
{
    class Enemy_Magie : Enemy
    {
        private double visibilityTimer = 0;
        Timer attackTimer;
        private bool isVisible = true;
         private  bool canAttack = true;
        private const double visibleTime = 1000;
        private const double invisibleTime = 2000;

        public Enemy_Magie(int x, int y, Map map, EnemyType type, Room room)
            : base(x, y, map, type, "spriteSheetEnemy_Wizard", room)
        {
            setSpeed();
            this.map = map;
             attackTimer = new Timer(1, this);
        }



        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            attackTimer.Update(gameTime);
            if(caculateDistance() < 20){
                Attack();
            }
            visibilityTimer += gameTime.ElapsedGameTime.TotalMilliseconds;

            if (isVisible && visibilityTimer >= visibleTime)
            {
                isVisible = false;
                visibilityTimer = 0;
            }
            else if (!isVisible && visibilityTimer >= invisibleTime)
            {

                isVisible = true;
                visibilityTimer = 0;
            }

            if (isVisible)
            {
                sprite.Update(gameTime);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            if (isVisible)
            {
                base.Draw(gameTime);
            }
        }

        public new void Attack(){
             if (canAttack)
            {
                Player.Get().takeDamage(1);
            }

            SetEnemyState(EnemyState.ATTACKING);
            canAttack = false;
            attackTimer.start();

        }

         public override void TimerCallback(Timer timer)
        {
            base.TimerCallback(timer);
            if (timer == attackTimer)
            {
                canAttack = true;
            }

        }
    }
}
