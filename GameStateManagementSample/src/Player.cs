using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.Diagnostics;
using GameStateManagement;
using Manager;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace wizard_game
{

    public class Player : Acteur
    {
        public const int RANK_2_EXP_NEEDED = 500;  //die exp die nötig sind um von rang 1 auf rang 2 zu kommen
        public const float RANK_EXP_MULTIPLIER = 0.5f; //der faktor mit dem current_rank_exp multiplioziert wird und auf sich selbst addiert wird
        public static int current_rank_exp_needed = RANK_2_EXP_NEEDED; //die exp die aktuell nötig sind um ein level aufzusteigen
        
        private static Player instance;
        public const int PLAYER_MAX_HEALTH = 8;
        public float speed = 0.24f;
        public float currentSpeed;
        public string currentAnimation;
        public int id;
        private Vector2 destination;
        private Map map;
        private List<Weapon> weapons;
        private Weapon equippedWeapon;
        public int coins;
        float stepsSpeed = 0.1f;
        Timer stepsTimer;
        public int rank;
        public int exp;
        public int lp;  //lernpunkte



        SoundEffectInstance inventorySound;
        SoundEffectInstance stepsSound;
        private Player(int x, int y, Map _map) : base(new Vector2(x, y), 27, 45, "spriteSheetPlayer", true)
        {
            stepsSound = AssetManager.GetSoundInstance("footsteps/step_lth1");
            inventorySound = AssetManager.GetSoundInstance("inventory_sound_effects/cloth-inventory");
            
            LoadSprite(4, 4, 1, true);
            sprite.offset = new Vector2(width/2, height/2);
            sprite.origin = new Vector2(width/2, height/2);
            InitAnimations();
            direction = new Vector2(0, -1);
            this.weapons = new List<Weapon>();
            health = PLAYER_MAX_HEALTH;
            map = _map;
            currentSpeed = 0;
            stepsTimer = new Timer(stepsSpeed, this);
            rank = 1;
            exp = 0;
            lp = 20;
            
        }


        //Player is a singleton. you always get the instance of Player with Get()
        public static Player Get()
        {
            if (instance == null)
            {
                instance = new Player(500, 100, GameplayScreen.map);
                return instance;
            }
            else return instance;
        }





        //creating animations by defining the order of frame indices
        //adding the animations to the sprite
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
            currentAnimation = "idle_down";
            sprite.setAnimation(currentAnimation);
        }




        //adding the weapon only if this type is not already carried by the player
        public void AddWeapon(Weapon weapon)
        {
            foreach (Weapon w in weapons)
            {
                if (w.name == weapon.name) return;
            }
            weapons.Add(weapon);
        }



        public void PickupExp(int exp)
        {
            this.exp += exp;
            if (this.exp >= current_rank_exp_needed)
            {
                this.exp = this.exp -  current_rank_exp_needed;
                rank++;
                CalcCurrentRankExp();
                lp++;
            }
        }



        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (isDying) return;
            damageArea.X = (int)(position.X+damageOffset.X);
            damageArea.Y = (int)(position.Y+damageOffset.Y);

            CheckForItems();

            direction.Normalize();
            Room room = map.GetActiveRoom();
            Vector2 oldPos = position;
            //Zustand als Empty
           // room.setGamestateElement(position, Gamestate.EMPTY);
            position = position + direction * currentSpeed * (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            //Zustand als Player
         //   room.setGamestateElement(position, Gamestate.PLAYER);
            DetacteCollisonX(oldPos);
            DetacteCollisonY(oldPos);
            sprite.setAnimation(currentAnimation);
            
            if (equippedWeapon != null) equippedWeapon.Update(gameTime);
            //keeping the position of the weapons by the player
            foreach (Weapon w in weapons)
            {
                w.position = position;
                w.hitBox.X = (int)position.X;
                w.hitBox.Y = (int)position.Y;
                w.area.X = (int)position.X;
                w.area.Y = (int)position.Y;
            }
            //setting the position of the equipped weapon
            if (equippedWeapon != null)
            {
                equippedWeapon.position = position + equippedWeapon.equipedOffset;
            }

            if (currentSpeed > 0) stepsTimer.start();
            else stepsTimer.pause();

            stepsTimer.Update(gameTime);
        }



        public void HandleInput(InputState inputState)
        {
            currentAnimation = "idle_down";
            currentSpeed = 0;
            Vector2 nextDir = new Vector2(0, 0);

            KeyboardState keyboardState = inputState.CurrentKeyboardStates[0];
            if (keyboardState.IsKeyDown(Keys.W)) nextDir += WalkUp();
            if (keyboardState.IsKeyDown(Keys.A)) nextDir += WalkLeft();
            if (keyboardState.IsKeyDown(Keys.S)) nextDir += WalkDown();
            if (keyboardState.IsKeyDown(Keys.D)) nextDir += WalkRight();
            //debug
            if (inputState.IsNewKeyPress(Keys.R))
            {
                map.ReloadWalls();
            }
            if (nextDir.Length() != 0) direction = nextDir;

            //the correct idle animation is determined when the player is not moving. in which side is the player looking?
            if (currentSpeed == 0)
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

            //inputState.IsNewKeyPress(Keys.Space);
            if (inputState.IsNewKeyPress(Keys.Space)) Attack();

            //switching weapons
            
           
            if (inputState.IsNewKeyPress(Keys.D1))
            {
                if (UI.Get().isInTabmenu && lp > 0)
                {
                    extraMaxHealth++;
                    lp--;
                }
                else
                {
                    inventorySound.Play();
                    EquipWeapon(Weapon.WeaponName.SWORD);
                }
                
            }
            if (inputState.IsNewKeyPress(Keys.D2))
            {
                if (UI.Get().isInTabmenu && lp > 0)
                {
                    rangedExtraDamage++;
                    rangedExtraVelocity++;
                    lp--;
                }
                else
                {
                    inventorySound.Play();
                    EquipWeapon(Weapon.WeaponName.BOW);
                }
            }

            if (inputState.IsNewKeyPress(Keys.D3))
            {
                if (UI.Get().isInTabmenu && lp > 0)
                {
                    meeleExtraDamage++;
                    lp--;
                }
            }
            
            if (inputState.IsNewKeyPress(Keys.R))
            {
                int x = GameplayScreen.rand.Next(0, GameStateManagementGame.Get().graphics.PreferredBackBufferWidth);
                int y = GameplayScreen.rand.Next(0, GameStateManagementGame.Get().graphics.PreferredBackBufferHeight);
                position = new Vector2(x, y);
            }
            if (inputState.IsNewKeyPress(Keys.K))
            {
                takeDamage(1);
            }
            if (inputState.IsNewKeyPress(Keys.Tab))
            {
                UI.Get().toggleTabmenu();
            }
        }




        // returns the direction vector
        private Vector2 WalkUp()
        {
            if (equippedWeapon != null) equippedWeapon.sprite.layerDepth = 0.6f;
            damageOffset.X = 0;
            damageOffset.Y = -damageDistance;
            currentSpeed = speed;
            DetacteCollisonY(position);
            currentAnimation = "up";
            //let the weapon know in which direciton the player is looking
            if (equippedWeapon != null) equippedWeapon.SetEquippedUp();
            return new Vector2(0, -1);
        }


        private Vector2 WalkDown()
        {
            if (equippedWeapon != null) equippedWeapon.sprite.layerDepth = 0.4f;
            damageOffset.X = 0;
            damageOffset.Y = damageDistance;
            currentSpeed = speed;
            DetacteCollisonY(position);
            currentAnimation = "down";
            if (equippedWeapon != null) equippedWeapon.SetEquippedDown();
            return new Vector2(0, 1);
        }


        private Vector2 WalkLeft()
        {
            if (equippedWeapon != null) equippedWeapon.sprite.layerDepth = 0.6f;
            damageOffset.X = -damageDistance;
            damageOffset.Y = 0;
            currentSpeed = speed;
            DetacteCollisonX(position);
            currentAnimation = "left";
            if (equippedWeapon != null) equippedWeapon.SetEquippedLeft();
            return new Vector2(-1, 0);
        }


        private Vector2 WalkRight()
        {
            if (equippedWeapon != null) equippedWeapon.sprite.layerDepth = 0.4f;
            damageOffset.X = damageDistance;
            damageOffset.Y = 0;
            currentSpeed = speed;
            DetacteCollisonX(position);
            currentAnimation = "right";
            if (equippedWeapon != null) equippedWeapon.SetEquippedRight();
            return new Vector2(1, 0);
        }



        private void EquipWeapon(Weapon.WeaponName weaponName)
        {
            foreach (Weapon w in weapons)
            {
                if (w.name == weaponName)
                {
                    if (equippedWeapon != null && equippedWeapon.name == weaponName)
                    {
                        if (equippedWeapon != null) equippedWeapon.state = Item.State.IN_INVENTORY;
                        equippedWeapon = null;
                        return;
                    }
                    if (equippedWeapon != null) equippedWeapon.state = Item.State.IN_INVENTORY;
                    equippedWeapon = w;
                    equippedWeapon.state = Item.State.EQUIPPED;
                    if (equippedWeapon != null)
                    {
                        if (direction.Y < 0)
                        {
                            equippedWeapon.SetEquippedUp();
                            equippedWeapon.sprite.layerDepth = 0.6f;
                            if (direction.X < 0) { equippedWeapon.SetEquippedLeft(); equippedWeapon.sprite.layerDepth = 0.6f; }
                            else if (direction.X > 0) { equippedWeapon.SetEquippedRight(); equippedWeapon.sprite.layerDepth = 0.4f; }
                        }
                        else if (direction.Y > 0)
                        {
                            equippedWeapon.SetEquippedDown(); equippedWeapon.sprite.layerDepth = 0.4f;
                            if (direction.X > 0) { equippedWeapon.SetEquippedRight(); equippedWeapon.sprite.layerDepth = 0.4f; }
                        }
                        else
                        {
                            if (direction.X < 0) { equippedWeapon.SetEquippedLeft(); equippedWeapon.sprite.layerDepth = 0.6f; }
                            else { equippedWeapon.SetEquippedRight(); equippedWeapon.sprite.layerDepth = 0.4f; }
                        }
                        equippedWeapon.position = position + equippedWeapon.equipedOffset;
                        return;
                    }

                }
            }
        }



        public override void Attack()
        {
            if (equippedWeapon != null) equippedWeapon.Attack(this);
        }


        public override void OnInput(GameStateManagementGame.InputState input)
        {
        }


        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            if (equippedWeapon != null) equippedWeapon.Draw(gameTime);
            //GameStateManagementGame._spriteBatch.Draw(image_hitbox, new Rectangle(damageArea.X, damageArea.Y, lineWidth, damageArea.Height + lineWidth), hitboxColor);
            //GameStateManagementGame._spriteBatch.Draw(image_hitbox, new Rectangle(damageArea.X, damageArea.Y, damageArea.Width + lineWidth, lineWidth), hitboxColor);
            //GameStateManagementGame._spriteBatch.Draw(image_hitbox, new Rectangle(damageArea.X + damageArea.Width, damageArea.Y, lineWidth, damageArea.Height + lineWidth), hitboxColor);
            //GameStateManagementGame._spriteBatch.Draw(image_hitbox, new Rectangle(damageArea.X, damageArea.Y + damageArea.Height, damageArea.Width + lineWidth, lineWidth), hitboxColor);


        }



        public void CalcCurrentRankExp()
        {
            current_rank_exp_needed += (int)(current_rank_exp_needed * RANK_EXP_MULTIPLIER);
        }



        public void CheckForItems()
        {
            foreach (Item item in GameplayScreen.items)
            {
                if (item.state == Item.State.ON_FLOOR && item.area.Intersects(hitBox))
                {
                    item.Effect();
                    map.GetActiveRoom().setGamestateElement(item.position, Gamestate.EMPTY);
                    break;
                }

            }
        }




        public void CheckDestinationReached()
        {
            if (hitBox.Contains(destination))
            {
                Console.WriteLine("dest reached");
                //destinationSet = false;

            }
        }


        public void reset()
        {
            weapons = new List<Weapon>();
            position = new Vector2(400,400);
            health = 2;
            coins = 0;
            rotation = 0;
            lp = 20;
            extraMaxHealth = 0;
            meeleExtraDamage = 0;
            rangedExtraDamage = 0;
            rangedExtraVelocity = 0;
        }


        public override void Die()
        {
            base.Die();
            sprite.setAnimation("idle_right");
            equippedWeapon = null;
        }


        public override void TimerCallback(Timer timer)
        {
            ScreenManager screenManager = GameStateManagementGame.Get().screenManager;
            if (timer == stepsTimer) stepsSound.Play();
            if (timer == dieTimer)
            {
                isDying = false;
                for (int i = screenManager.screens.Count-1; i >= 0; i--)
                {
                    screenManager.screens.Remove(screenManager.screens[i]);
                }
                screenManager.AddScreen(new BackgroundScreen(), null);
                screenManager.AddScreen(new MainMenuScreen(), null);
                reset();
            }
        }

    }
}