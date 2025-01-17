using Microsoft.Xna.Framework;
using Manager;
using GameStateManagement;
using System.Threading.Tasks;
namespace wizard_game
{
    class Enemy_Magie : Enemy
    {
        private double visibilityTimer = 0;
        Timer attackTimer;
        bool aStarThreadIsRunning = false;
        private bool isVisible = true;
        private  bool canAttack = true;
        private const double visibleTime = 1000;
        private const double invisibleTime = 2000;
        bool isAttacking = false;
        Timer aStarTimer;
        float attackRange;

        public Enemy_Magie(int x, int y, Map map, EnemyType type, Room room) : base(x, y, map, type, "spriteSheetEnemy_Wizard", room)
        {
            dieSound = AssetManager.GetSoundInstance("monster_sfx_pack/monster-6");     //sterbesound
            dieSound.Volume = GameStateManagementGame.soundSettings.GetVolumeForSound();
            sprite.color = Color.Yellow;
            setSpeed();
            this.map = map;
            direction = new Vector2(1, 0);
            attackTimer = new Timer(1.5f, this);
            isAttacking = false;
            canAttack = true;
            aStarTimer = new Timer(1, this);
            aStarTimer.start();
            attackRange = 40;
             expDrop = 150;
        }



        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            currentSpeed = 0;
            if (isDying) return;
            aStarTimer.Update(gameTime);
            attackTimer.Update(gameTime);
            if (path.Count ==0 && !aStarThreadIsRunning)
            {
                aStarThreadIsRunning = true;
                Task.Run(()=>
                {
                    path = calculatePathToTarget(Player.Get().GetMidPos());
                    aStarThreadIsRunning = false;                  
                });
                aStarTimer.start();

            }
            if (path.Count >=1 && moveToTarget(path[path.Count-1])) path.RemoveAt(path.Count-1);




            if ((Player.Get().GetMidPos() - GetMidPos()).Length() < attackRange) Attack();

            if (isAttacking)
            {
                Vector2 o = new Vector2(width/2, height/2);
                sprite.origin = o;
                float endRotation = 1;
                rotation = MathHelper.Lerp(rotation, endRotation, 0.4f);
                if (rotation <= endRotation + 0.05 && rotation >= endRotation - 0.05)
                {
                    rotation = 0;
                    isAttacking = false;
                    if ((Player.Get().GetMidPos() - GetMidPos()).Length() < attackRange) Player.Get().takeDamage(1);
                }
            }


            UpdateAnimation();
            sprite.setAnimation(currentAnimation);
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
            if (timer == attackTimer) canAttack = true;
            else if (timer == aStarTimer && !aStarThreadIsRunning)
            {                
                aStarThreadIsRunning = true;
                Task.Run(()=>
                {
                    path = calculatePathToTarget(Player.Get().GetMidPos());
                    aStarThreadIsRunning = false;                  
                });
                aStarTimer.start();
            }

        }
    }
}
