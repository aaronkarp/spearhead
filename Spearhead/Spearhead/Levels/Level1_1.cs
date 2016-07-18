#region File Description
//-----------------------------------------------------------------------------
// Level1_1.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Threading;
using System.IO;
using System.IO.IsolatedStorage;
using System.Xml.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Devices;
#endregion

namespace Spearhead
{
    /// <summary>
    /// This screen implements the game's first level
    /// </summary>
    class Level1_1 : GameScreen
    {
        #region Fields

        ContentManager content;
        GraphicsDevice graphics;
        ScrollingBackground starLevel1;
        ScrollingBackground starLevel2;
        ScrollingBackground starLevel3;
        ScrollingBackground maskLevel;
        List<ScrollingBackground> backgrounds;
        List<Formation> formations;

        PlayerShip playerShip;

        Formation Wave1;
        Formation Wave2;
        Formation Wave3;
        Formation Wave4;
        int currentWave;

        float ActiveTimer;

        PowerupCarrier Carrier;
        List<PowerupCarrier> carriers;
        Powerup Powerup;
        List<Powerup> powerups;

        Texture2D explosionTexture;
        Texture2D largeExplosionTexture;
        List<Animation> explosions;

        Texture2D smallImpactTexture;
        Texture2D largeImpactTexture;
        List<Animation> impacts;

        Hud Hud;
        Bonus Bonus;
        TimeSpan BonusStart;
        TimeSpan BonusLength;
        bool BonusDisplay = false;
        int LevelBonus;

        GetReady readyScreen;

        LevelComplete completeScreen;

        TimeSpan levelEnd; // The moment the player has successfully completed the level-end requirements
        TimeSpan levelEndDelay; // The delay between levelEnd and the triggering of the switch to the next level

        InputAction pauseAction;

        #endregion

        #region Initialization

        public Level1_1()
        {
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

            pauseAction = new InputAction(
                new Buttons[] { Buttons.Start, Buttons.Back },
                new Keys[] { Keys.Escape },
                true);  
        }

        public override void Activate(bool instancePreserved)
        {
            ScreenManager.Level = "1-1";
            LevelBonus = 1000;

            if (!instancePreserved)
            {
                if (content == null)
                    content = new ContentManager(ScreenManager.Game.Services, "Content");

                graphics = ScreenManager.GraphicsDevice;
                BonusLength = TimeSpan.FromSeconds(2.0f);
                explosions = new List<Animation>();
                impacts = new List<Animation>();
                formations = new List<Formation>();
                carriers = new List<PowerupCarrier>();
                powerups = new List<Powerup>();
                currentWave = 0;

                // Scrolling Background layers
                backgrounds = new List<ScrollingBackground>();
                starLevel1 = new ScrollingBackground();
                starLevel2 = new ScrollingBackground();
                starLevel3 = new ScrollingBackground();
                maskLevel = new ScrollingBackground();
                starLevel1.Initialize(content, ScreenManager.SpriteBatch, "Images/starfield_level1", 800, 2);
                starLevel2.Initialize(content, ScreenManager.SpriteBatch, "Images/starfield_level2", 800, 4);
                starLevel3.Initialize(content, ScreenManager.SpriteBatch, "Images/starfield_level3", 800, 8);
                maskLevel.Initialize(content, ScreenManager.SpriteBatch, "Images/starfield_mask", 800, 1);
                backgrounds.Add(starLevel1);
                backgrounds.Add(starLevel2);
                backgrounds.Add(starLevel3);
                backgrounds.Add(maskLevel);

                // Set up the player's ship, the "Get Ready" message and the "Level Complete" message
                // In Level 1-1, the player ship will never start with any powerups.
                playerShip = new PlayerShip(new Vector2(216, 600), content, "None", "None", "None", 0);
                readyScreen = new GetReady(content, ScreenManager, "Images/pixel", playerShip, ScreenManager.Level);
                completeScreen = new LevelComplete(content, ScreenManager.PlayerScore, "Images/Pixel", ScreenManager.Level, LevelBonus);

                // Initialize the level-end timer
                levelEnd = TimeSpan.Zero;
                levelEndDelay = TimeSpan.FromSeconds(4f);

                // Set up the HUD
                Hud = new Hud(content, "Images/pixel", playerShip);

                // Set up explosions
                explosionTexture = content.Load<Texture2D>("Images/explosion-1");
                largeExplosionTexture = content.Load<Texture2D>("Images/largeExplosion");

                // Set up impacts
                smallImpactTexture = content.Load<Texture2D>("Images/smallShotImpact");
                largeImpactTexture = content.Load<Texture2D>("Images/largeShotImpact");

                // Zero out ActiveTime and set IsPaused to false
                ScreenManager.ActiveTime = 0;
                ScreenManager.IsPaused = false;
            }
            else
            {
                ScreenManager.AddScreen(new PhonePauseScreen(), ControllingPlayer);
            }

            // Display a "Get Ready" screen at the start of the level
            readyScreen.StartTime = ScreenManager.GlobalGameTime;
            readyScreen.Active = true;
            ScreenManager.IsPaused = true;
            
#if WINDOWS_PHONE
            if (Microsoft.Phone.Shell.PhoneApplicationService.Current.State.ContainsKey("Score"))
            {
                // TODO: Implement state loading
            }
#endif
        }

        public override void Deactivate()
        {
#if WINDOWS_PHONE
            // TODO: Implement state saving (save score, health, lives, powerup state, and level)
#endif
            base.Deactivate();
        }

        public override void Unload()
        {
            content.Unload();
#if WINDOWS_PHONE
            // TODO: Implement state removal.
#endif
        }

        #endregion

        #region Update and Draw

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            ActiveTimer = (float)Math.Round((double)ScreenManager.ActiveTime, 1);

            if (IsActive)
            {
                if (!playerShip.Dead)
                    // If the player ship has been destroyed, stop scrolling the backgrounds
                {
                    for (int i = 0; i < backgrounds.Count; i++)
                    {
                        backgrounds[i].Update(gameTime);
                    }
                }

                playerShip.Update(gameTime);
                UpdateFormations(gameTime);
                UpdateCarriers(gameTime);
                UpdatePowerups(gameTime);
                if (readyScreen == null || !readyScreen.Active)
                {
                    for (int i = 0; i < formations.Count; i++)
                    {
                        formations[i].Update(gameTime);
                    }

                    for (int i = 0; i < carriers.Count; i++)
                    {
                        carriers[i].Update(gameTime);
                    }
                }
                UpdateCollision();
                UpdateExplosions(gameTime);
                Hud.Update(gameTime, ScreenManager.PlayerScore, ScreenManager.Level, ScreenManager.PlayerLives);
                
                // Handle displaying the bonus message
                if (BonusStart + BonusLength > gameTime.TotalGameTime)
                {
                    BonusDisplay = true;
                }
                else
                {
                    BonusDisplay = false;
                }

                // Display explosions when the player dies
                if (playerShip.Dead)
                {
                    if (playerShip.DeathTime + playerShip.DeathExplosionTime < gameTime.TotalGameTime)
                    {
                        playerShip.DeathTime = gameTime.TotalGameTime;
                        if (playerShip.ExplosionCount <= 4)
                        {
                            playerShip.ExplosionCount++;
                            AddExplosion(new Vector2(playerShip.Position.X + Random.Next(10, 38), playerShip.Position.Y + Random.Next(0, 80)), explosionTexture, 80, 80, 10, 45, "small");
                        }
                        else if (readyScreen.Active == false)
                        {
                            readyScreen.StartTime = gameTime.TotalGameTime;
                            readyScreen.Active = true;
                            ScreenManager.IsPaused = true;
                            formations[currentWave - 1].Reset();
                            ScreenManager.CurrentShield = "None";
                            ScreenManager.CurrentShieldStrength = 0;
                            ScreenManager.CurrentWeapon = "None";
                            ScreenManager.CurrentSecondary = "None";
                        }
                    }
                    if (ScreenManager.PlayerLives == 0)
                    {
                        LoadingScreen.Load(ScreenManager, true, PlayerIndex.One, new GameOver());
                        ScreenManager.RemoveScreen(this);
                    }
                }

                if (readyScreen != null && readyScreen.Active)
                {
                    readyScreen.Update(gameTime);
                }

                if (completeScreen != null)
                {
                    if (completeScreen.Active)
                    {
                        completeScreen.Update(gameTime);
                    }
                    if (completeScreen.Finished)
                    {
                        LoadingScreen.Load(ScreenManager, true, PlayerIndex.One, new Level1_2());
                        ScreenManager.RemoveScreen(this);
                    }
                }
            }
        }

        public override void HandleInput(GameTime gameTime, InputState input)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            int playerIndex = (int)ControllingPlayer.Value;

            KeyboardState keyboardState = input.CurrentKeyboardStates[playerIndex];
            GamePadState gamePadState = input.CurrentGamePadStates[playerIndex];

            bool gamePadDisconnected = !gamePadState.IsConnected && input.GamePadWasConnected[playerIndex];

            PlayerIndex player;
            if (pauseAction.Evaluate(input, ControllingPlayer, out player) || gamePadDisconnected)
            {
#if WINDOWS_PHONE
                ScreenManager.IsPaused = true;
                ScreenManager.AddScreen(new PhonePauseScreen(), ControllingPlayer);
#else
                ScreenManager.AddScreen(new PauseMenuScreen(), ControllingPlayer);
#endif
            }
            else
            {
                // Otherwise move the player position.
                Vector2 movement = Vector2.Zero;

                if (keyboardState.IsKeyDown(Keys.Left))
                    movement.X--;

                if (keyboardState.IsKeyDown(Keys.Right))
                    movement.X++;

                if (keyboardState.IsKeyDown(Keys.Up))
                    movement.Y--;

                if (keyboardState.IsKeyDown(Keys.Down))
                    movement.Y++;

                Vector2 thumbstick = gamePadState.ThumbSticks.Left;

                movement.X += thumbstick.X;
                movement.Y -= thumbstick.Y;
                float moveSpeed = 1f;

                if (input.TouchState.Count > 0)
                {
                    Vector2 touchPosition = input.TouchState[0].Position;
                    Vector2 direction = touchPosition - (playerShip.Position + new Vector2(24, 117));
                    if (Math.Abs(direction.X) <= 12 && Math.Abs(direction.Y) <= 58)
                    {
                     //   movement = new Vector2(0, 0);
                        moveSpeed = 8f;
                    }
                    else if ((Math.Abs(direction.X) > 12 && Math.Abs(direction.X) <= 24) && (Math.Abs(direction.Y) > 58 && Math.Abs(direction.Y) <= 117))
                    {
                        moveSpeed = 16f;
                    }
                    else
                    {
                        direction.Normalize();
                        movement += direction;
                        moveSpeed = 21f;
                    }
                }

                Vector2 prePosition = new Vector2(playerShip.Position.X, playerShip.Position.Y);

                if (movement.Length() > 1)
                    movement.Normalize();
                prePosition += movement * moveSpeed;
                prePosition.X = (int)prePosition.X;
                prePosition.Y = (int)prePosition.Y;
                playerShip.Position = prePosition;
                if (playerShip.Shield != null)
                    playerShip.Shield.Position = playerShip.Position - playerShip.Shield.textureOffset;
            }

        }


        /// <summary>
        /// Draws the gameplay screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            ScreenManager.GraphicsDevice.Clear(ClearOptions.Target,
                                               Color.Black, 0, 0);

            // Our player and enemy are both actually just text strings.
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            for (int i = 0; i < backgrounds.Count; i++)
            {
                backgrounds[i].Draw(gameTime);
            }

            spriteBatch.Begin();

            if (playerShip.ExplosionCount <= 2)
            {
                playerShip.Draw(spriteBatch);
            }

            for (int i = playerShip.projectiles.Count - 1; i >= 0; i--)
            {
                playerShip.projectiles[i].Draw(spriteBatch);
            }

            for (int i = 0; i < formations.Count; i++)
            {
                for (int j = 0; j < formations[i].enemies.Count; j++)
                {
                    formations[i].enemies[j].Draw(spriteBatch);
                }
            }

            for (int i = 0; i < carriers.Count; i++)
            {
                carriers[i].Draw(spriteBatch);
            }

            for (int i = 0; i < powerups.Count; i++)
            {
                powerups[i].Draw(spriteBatch);
            }

            for (int i = explosions.Count - 1; i >= 0; i--)
            {
                explosions[i].Draw(spriteBatch);
            }

            for (int i = impacts.Count - 1; i >= 0; i--)
            {
                impacts[i].Draw(spriteBatch);
            }

            Hud.Draw(spriteBatch);

            if (BonusDisplay && Bonus != null)
            {
                Bonus.Draw(spriteBatch);
            }

            if (readyScreen != null && readyScreen.Active)
            {
                readyScreen.Draw(spriteBatch);
            }

            if (completeScreen != null && completeScreen.Active)
            {
                completeScreen.Draw(spriteBatch);
            }

            spriteBatch.End();

        }

        public void AddExplosion(Vector2 position, Texture2D explosionTexture, int frameWidth, int frameHeight, int frameCount, int frameTime, string size)
        {
            Animation explosion = new Animation(explosionTexture, position, frameWidth, frameHeight, frameCount, frameTime, Color.White, 1f, false);
            explosions.Add(explosion);
            if (size == "small" && ScreenManager.VibrationOn)
            {
                VibrateController.Default.Start(TimeSpan.FromMilliseconds(100));
            }
            else if (size == "large" && ScreenManager.VibrationOn)
            {
                VibrateController.Default.Start(TimeSpan.FromMilliseconds(200));
            }
        }

        public void AddImpact(Vector2 position, Texture2D impactTexture, int frameWidth, int frameHeight, int frameCount, int frameTime)
        {
            Animation impact = new Animation(impactTexture, position, frameWidth, frameHeight, frameCount, frameTime, Color.White, 1f, false);
            impacts.Add(impact);
        }

        public void UpdateCarriers(GameTime gameTime)
        {
        // Handle powerup carrier destruction and powerup distribution
            for (int i = 0; i < carriers.Count; i++)
            {
                if (carriers[i].Health <= 0 && carriers[i].Active)
                {
                    AddExplosion(new Vector2(carriers[i].Position.X + carriers[i].Width / 2, carriers[i].Position.Y + carriers[i].Height / 2), explosionTexture, 80, 80, 10, 45, "small");
                    carriers[i].Active = false;
                    string type = carriers[i].DeployType(Random);
                    if (type == "Gatling")
                        Powerup = new GatlingPowerup(carriers[i].Position, content);
                    if (type == "Laser")
                        Powerup = new LaserPowerup(carriers[i].Position, content);
                    if (type == "Missile")
                        Powerup = new MissilePowerup(carriers[i].Position, content);
                    if (type == "Shield")
                        Powerup = new ShieldPowerup(carriers[i].Position, content);
                    if (type == "Health")
                        Powerup = new HealthPowerup(carriers[i].Position, content);
                    powerups.Add(Powerup);
                    carriers.RemoveAt(i);
                }
            }
        }

        public void UpdatePowerups(GameTime gameTime)
        {
            for (int i = 0; i < powerups.Count; i++)
            {
                powerups[i].Update(gameTime);
            }
        }

        public void UpdateFormations(GameTime gameTime)
        {
            if (ActiveTimer >= 3.0 && Wave1 == null)
            {
                Wave1 = new Wedge(content, gameTime, "Rhino", 500, 1f, "None");

                formations.Add(Wave1);
                currentWave = 1;
            }

            if (ActiveTimer >= 9.5 && Wave2 == null)
            {
                Wave2 = new Wedge(content, gameTime, "Aphid", 600, 1f, "None");
                formations.Add(Wave2);
                currentWave = 2;
            }
            
            if (ActiveTimer >= 16.0 && Wave3 == null)
            {
                Wave3 = new SweepHold(content, gameTime, "Aphid", 700, 1f, "Burst");
                formations.Add(Wave3);
                currentWave = 3;
            }

            if (ActiveTimer > 15.0 && Carrier == null)
            {
                Vector2 startPosition = new Vector2(0, -36);
                Carrier = new PowerupCarrier(startPosition, content, "Powerup");
                carriers.Add(Carrier);
            }

            if (ActiveTimer >= 24.0 && Wave4 == null)
            {
                Wave4 = new Zigzag(content, gameTime, "Rhino", 700, 1f, "Auto");
                formations.Add(Wave4);
                currentWave = 4;
            }

            if (Wave4 != null && Wave4.CompleteWave && Wave3 != null && Wave3.CompleteWave && Wave2 != null && Wave2.CompleteWave && Wave1 != null && Wave1.CompleteWave)
            {
                if (levelEnd == TimeSpan.Zero)
                    levelEnd = gameTime.TotalGameTime;
                if (gameTime.TotalGameTime >= levelEnd + levelEndDelay)
                {
                    if (completeScreen.Active == false)
                    {
                        completeScreen.StartTime = gameTime.TotalGameTime;
                        completeScreen.Active = true;
                    }
                }
            }

            // Add explosions where appropriate
            for (int i = 0; i < formations.Count; i++)
            {
                if (!formations[i].CompleteWave)
                {
                    for (int j = formations[i].enemies.Count - 1; j >= 0; j--)
                    {
                        if (formations[i].enemies[j].Health <= 0 && !formations[i].enemies[j].Exploded)
                        {
                            AddExplosion(new Vector2(formations[i].enemies[j].Position.X + formations[i].enemies[j].Width / 2, formations[i].enemies[j].Position.Y + formations[i].enemies[j].Height / 2), explosionTexture, 80, 80, 10, 45, "small");
                            ScreenManager.PlayerScore += formations[i].enemies[j].Value;
                            formations[i].enemies[j].Exploded = true;
                            formations[i].Killed += 1;
                        }
                    }

                    // If the player has destroyed all of the ships in the wave, display a bonus notification and award them the bonus
                    if (formations[i].Killed == formations[i].totalShips)
                    {
                        Bonus = new Bonus(content, "Fonts/squarefuture_small", formations[i].BonusValue.ToString());
                        BonusStart = gameTime.TotalGameTime;
                        ScreenManager.PlayerScore += formations[i].BonusValue;
                        formations[i].Killed = 0;
                    }
                }
            }
        }

        private void UpdateExplosions(GameTime gameTime)
        {
            for (int i = explosions.Count - 1; i >= 0; i--)
            {
                explosions[i].Update(gameTime);
                if (!explosions[i].Active)
                {
                    explosions.RemoveAt(i);
                }
            }

            for (int i = impacts.Count - 1; i >= 0; i--)
            {
                impacts[i].Update(gameTime);
                if (!impacts[i].Active)
                {
                    impacts.RemoveAt(i);
                }
            }
        }

        private void UpdateCollision()
        {
            Rectangle rectangle1;
            Rectangle rectangle2;
            Rectangle rectangle3;

            rectangle1 = new Rectangle((int)playerShip.Position.X, (int)playerShip.Position.Y, playerShip.Width, playerShip.Height);

            for (int i = powerups.Count - 1; i >= 0; i--)
            {
                if (!playerShip.Dead)
                {
                    rectangle2 = new Rectangle((int)powerups[i].Position.X, (int)powerups[i].Position.Y, powerups[i].Width, powerups[i].Height);
                    if (rectangle1.Intersects(rectangle2))
                    {
                        powerups[i].Activate(ScreenManager, playerShip);
                        powerups[i].Active = false;
                        powerups.RemoveAt(i);
                    }
                }
            }

            for (int i = formations.Count - 1; i >= 0; i--)
            {
                if (!playerShip.Dead)
                {
                    for (int j = formations[i].enemies.Count - 1; j >= 0; j--)
                    {
                        if (!formations[i].enemies[j].Destroyed)
                        {
                            rectangle2 = new Rectangle((int)formations[i].enemies[j].Position.X, (int)formations[i].enemies[j].Position.Y, formations[i].enemies[j].Width, formations[i].enemies[j].Height);
                            if (rectangle1.Intersects(rectangle2))
                            {
                                if (!playerShip.Invincible)
                                {
                                    if (playerShip.Shield != null && playerShip.Shield.Active == true)
                                    {
                                        playerShip.Shield.Health--;
                                    }
                                    else
                                    {
                                        playerShip.Health -= formations[i].enemies[j].RamDamage;
                                    }
                                }
                                formations[i].enemies[j].Health = 0;

                                if (playerShip.Health <= 0)
                                {
                                    playerShip.Dead = true;
                                    ScreenManager.PlayerLives--;
                                }
                            }
                        }
                        for (int k = formations[i].enemies[j].projectiles.Count - 1; k >= 0; k--)
                        {

                            int frameWidth = 8;
                            int frameHeight = 8;
                            int frameCount = 3;
                            int frameTime = 70;
                            Texture2D impactTexture = smallImpactTexture;

                            if (formations[i].enemies[j].projectiles[k].ImpactType == "large")
                            {
                                frameWidth = 16;
                                frameHeight = 16;
                                frameCount = 4;
                                frameTime = 70;
                                impactTexture = largeImpactTexture;
                            }

                            rectangle3 = new Rectangle((int)formations[i].enemies[j].projectiles[k].Position.X, (int)formations[i].enemies[j].projectiles[k].Position.Y, formations[i].enemies[j].projectiles[k].Width, formations[i].enemies[j].projectiles[k].Height);
                            if (rectangle1.Intersects(rectangle3))
                            {
                                if (playerShip.Shield != null && playerShip.Shield.Active == true)
                                {
                                    playerShip.Shield.Health--;
                                }
                                else
                                {
                                    if (!playerShip.Invincible)
                                        playerShip.Health -= formations[i].enemies[j].GunDamage;
                                }

                                if (formations[i].enemies[j].projectiles[k].Uninterrupted == false)
                                {
                                    formations[i].enemies[j].projectiles[k].Active = false;
                                }

                                if (playerShip.Health <= 0)
                                {
                                    playerShip.Dead = true;
                                    ScreenManager.PlayerLives--;
                                }

                                if (!playerShip.Invincible)
                                    AddImpact(new Vector2((formations[i].enemies[j].projectiles[k].Position.X - ((formations[i].enemies[j].projectiles[k].Texture.Width - impactTexture.Width) / 2)), formations[i].enemies[j].projectiles[k].Position.Y + formations[i].enemies[j].projectiles[k].Texture.Height), impactTexture, frameWidth, frameHeight, frameCount, frameTime);
                            }
                        }
                    }
                }

                for (int j = 0; j < playerShip.projectiles.Count; j++)
                {

                    int frameWidth = 8;
                    int frameHeight = 8;
                    int frameCount = 3;
                    int frameTime = 70;
                    Texture2D impactTexture = smallImpactTexture;

                    if (playerShip.projectiles[j].ImpactType == "large")
                    {
                        frameWidth = 16;
                        frameHeight = 16;
                        frameCount = 4;
                        frameTime = 70;
                        impactTexture = largeImpactTexture;
                    }

                    for (int w = formations[i].enemies.Count - 1; w >= 0; w--)
                    {
                        rectangle1 = new Rectangle((int)playerShip.projectiles[j].Position.X, (int)playerShip.projectiles[j].Position.Y, playerShip.projectiles[j].Width, playerShip.projectiles[j].Height);
                        rectangle2 = new Rectangle((int)formations[i].enemies[w].Position.X, (int)formations[i].enemies[w].Position.Y, formations[i].enemies[w].Width, formations[i].enemies[w].Height);

                        if (rectangle1.Intersects(rectangle2))
                        {
                            formations[i].enemies[w].Health -= playerShip.projectiles[j].Damage;
                            if (playerShip.projectiles[j].Uninterrupted == false)
                            {
                                playerShip.projectiles[j].Active = false;
                            }
                            AddImpact(new Vector2((playerShip.projectiles[j].Position.X - ((playerShip.projectiles[j].Texture.Width - impactTexture.Width) / 2)), playerShip.projectiles[j].Position.Y), impactTexture, frameWidth, frameHeight, frameCount, frameTime);
                        }
                    }

                    for (int k = 0; k < carriers.Count; k++)
                    {
                        rectangle1 = new Rectangle((int)playerShip.projectiles[j].Position.X, (int)playerShip.projectiles[j].Position.Y, playerShip.projectiles[j].Width, playerShip.projectiles[j].Height);
                        rectangle2 = new Rectangle((int)carriers[k].Position.X, (int)carriers[k].Position.Y, carriers[k].Width, carriers[k].Height);

                        if (rectangle1.Intersects(rectangle2))
                        {
                            carriers[k].Health -= playerShip.projectiles[j].Damage;
                            if (playerShip.projectiles[j].Uninterrupted == false)
                            {
                                playerShip.projectiles[j].Active = false;
                            }
                            AddImpact(new Vector2((playerShip.projectiles[j].Position.X - ((playerShip.projectiles[j].Texture.Width - impactTexture.Width) / 2)), playerShip.projectiles[j].Position.Y), impactTexture, frameWidth, frameHeight, frameCount, frameTime);
                        }
                    }
                }
            }
        }

        #endregion
    }
}