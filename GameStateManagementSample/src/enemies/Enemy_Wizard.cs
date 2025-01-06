using System.Data.Common;
using System.IO.Pipes;
using System.Threading.Tasks;
using GameStateManagement;
using Manager;
using Microsoft.Xna.Framework;

namespace wizard_game
{
    class Enemy_Wizard : Enemy
    {
        int prop = 10;

        bool aStarThreadIsRunning = false;
        bool isAttacking;
       bool canAttack = true;
       Timer attackTimer;
        Timer aStarTimer;
        public string currentAnimation;
        float attackRange;
        public Enemy_Wizard(int x, int y, Map map, EnemyType type, Room room) : base(x, y, map, type, "spriteSheetEnemy_Wizard", room)
        {
             dieSound = AssetManager.GetSoundInstance("monster_sfx_pack/monster-6");     //sterbesound
            dieSound.Volume = GameStateManagementGame.GetSoundVolume();
            setSpeed();
            this.map = map;
            direction = new Vector2(1, 0);
            isAttacking = false;
            canAttack = true;
            aStarTimer = new Timer(1, this);
            aStarTimer.start();
            attackRange = 250;
             attackTimer = new Timer(1, this);
        }



        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            currentSpeed = 0;
            if (isDying) return;
             attackTimer.Update(gameTime);
            aStarTimer.Update(gameTime);
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

        }

         public void UpdateAnimation()
        {

            if (currentSpeed == 0 && path.Count == 0)
            {
                if (direction.Y < 0)
                {
                    currentAnimation = "idle_up";
                    if (direction.X < 0) currentAnimation = "idle_left";
                    else if (direction.X > 0) currentAnimation = "idle_right";
                }
                else if (direction.Y > 0)
                {
                    currentAnimation = "idle_down";
                    if (direction.X > 0) currentAnimation = "idle_right";
                }
                else
                {
                    if (direction.X < 0) currentAnimation = "idle_left";
                    else currentAnimation = "idle_right";
                }
            }

            if (currentSpeed > 0)
            {
                if (direction.Y < 0)
                {
                    currentAnimation = "up";
                    if (direction.X < 0) currentAnimation = "left";
                    else if (direction.X > 0) currentAnimation = "right";
                }
                else if (direction.Y > 0)
                {
                    currentAnimation = "down";
                    if (direction.X > 0) currentAnimation = "right";
                }
                else
                {
                    if (direction.X < 0) currentAnimation = "left";
                    else currentAnimation = "right";
                }
            }
        }

      public override void Attack()

        {
            if (canAttack)
            {
                createFireBalls();
            }

            SetEnemyState(EnemyState.ATTACKING);
            canAttack = false;
            attackTimer.start();
        }
        public void createFireBalls()
        {
            // Erstelle die Position für den Fireball (in der Nähe des Feindes)
            float posX = position.X + width / 2 - Fireball.FIREBALL_WIDTH/2;
            float posY = position.Y + height / 2 - Fireball.FIREBALL_HEIGHT/2;

            // Berechne die Richtung des Fireballs zum Spieler
            Vector2 playerPosition = Player.Get().position;
            Vector2 fireballDirection = playerPosition - position;
            fireballDirection.Normalize(); // Richtung normalisieren

            // Erstelle den Fireball
            Fireball fireball = new Fireball(posX, posY, this);
            fireball.SetDirection(fireballDirection);
            fireball.SetTarget(Player.Get());
            fireball.SetAttackstate(true);
            GameplayScreen.map.GetActiveRoom().projectiles.Add(fireball);
        }

          public override void TimerCallback(Timer timer)
        {
            base.TimerCallback(timer);
            if (timer == attackTimer)
            {
                canAttack = true; // Cooldown beendet, Angriff wieder möglich
            }

        }
    }
}