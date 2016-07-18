#region File Description
//-----------------------------------------------------------------------------
// Level3_3.cs
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
    /// This screen implements the game's ninth level, the third boss fight
    /// </summary>
    class Level3_3 : GameScreen
    {
        #region Fields

        ContentManager content;
        GraphicsDevice graphics;
        ScrollingBackground groundLevel1;
        ScrollingBackground groundLevel2;
        ScrollingBackground groundLevel3;
        ScrollingBackground groundLevel4;
        ScrollingBackground zeroLevel;
        List<ScrollingBackground> backgrounds;

        PlayerShip playerShip;
        string currentWeapon;
        string currentSecondary;
        string currentShield;
        int currentShieldStrength;
        public Boss Boss;

        float ActiveTimer;

        PowerupCarrier Carrier;
        PowerupCarrier Carrier2;
        PowerupCarrier Carrier3;
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
        int LevelBonus;

        GetReady readyScreen;

        LevelComplete completeScreen;

        TimeSpan levelEnd; // The moment the player has successfully completed the level-end requirements
        TimeSpan levelEndDelay; // The delay between levelEnd and the triggering of the switch to the next level

        InputAction pauseAction;

        private const string StateFilename = "SpearheadSavegame.xml";

        #endregion

        #region Initialization

        public Level3_3()
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
            ScreenManager.Level = "3-3";
            LevelBonus = 4500;

            if (!instancePreserved)
            {
                if (content == null)
                    content = new ContentManager(ScreenManager.Game.Services, "Content");

                graphics = ScreenManager.GraphicsDevice;
                explosions = new List<Animation>();
                impacts = new List<Animation>();
                carriers = new List<PowerupCarrier>();
                powerups = new List<Powerup>();

                // Scrolling Background layers
                backgrounds = new List<ScrollingBackground>();
                groundLevel1 = new ScrollingBackground();
                groundLevel2 = new ScrollingBackground();
                groundLevel3 = new ScrollingBackground();
                groundLevel4 = new ScrollingBackground();
                zeroLevel = new ScrollingBackground();
                groundLevel1.Initialize(content, ScreenManager.SpriteBatch, "Images/level-3-background-1", 800, 3);
                groundLevel2.Initialize(content, ScreenManager.SpriteBatch, "Images/level-3-background-2", 800, 3);
                groundLevel3.Initialize(content, ScreenManager.SpriteBatch, "Images/level-3-background-3", 800, 6);
                groundLevel4.Initialize(content, ScreenManager.SpriteBatch, "Images/level-3-background-4", 800, 10);
                zeroLevel.Initialize(content, ScreenManager.SpriteBatch, "Images/level-3-background-0", 800, 3);
                backgrounds.Add(zeroLevel);
                backgrounds.Add(groundLevel1);
                backgrounds.Add(groundLevel2);
                backgrounds.Add(groundLevel3);
                backgrounds.Add(groundLevel4);

                // Read in powerup states from ScreenManager
                currentShield = ScreenManager.CurrentShield;
                currentShieldStrength = ScreenManager.CurrentShieldStrength;
                currentWeapon = ScreenManager.CurrentWeapon;
                currentSecondary = ScreenManager.CurrentSecondary;

                // Set up the player's ship, the "Get Ready" message and the "Level Complete" message
                playerShip = new PlayerShip(new Vector2(216, 600), content, currentWeapon, currentSecondary, currentShield, currentShieldStrength);
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

            // Save game information to Isolated Storage
            using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                XDocument doc = new XDocument();
                XElement root = new XElement("SaveGame");
                doc.Add(root);
                string saveLevel = ScreenManager.Level;
                int saveScore = ScreenManager.PlayerScore;
                root.Add(new XElement("SaveData", new XAttribute("Level", saveLevel), new XAttribute("Score", saveScore)));

                using (IsolatedStorageFileStream stream = storage.CreateFile(StateFilename))
                {
                    doc.Save(stream);
                }
            }

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

                if (ActiveTimer >= 2.0 && Carrier == null)
                {
                    Vector2 startPosition = new Vector2(0, -36);
                    Carrier = new PowerupCarrier(startPosition, content, "Powerup");
                    carriers.Add(Carrier);
                }

                if (ActiveTimer >= 20.0 && Carrier2 == null)
                {
                    Vector2 startPosition = new Vector2(0, -36);
                    Carrier2 = new PowerupCarrier(startPosition, content, "Powerup");
                    carriers.Add(Carrier2);
                }

                if (ActiveTimer >= 30.0 && Carrier3 == null)
                {
                    Vector2 startPosition = new Vector2(0, -36);
                    Carrier3 = new PowerupCarrier(startPosition, content, "Powerup");
                    carriers.Add(Carrier3);
                }

                if (ActiveTimer >= 3.0 && Boss == null)
                {
                    Boss = new Level3Boss(new Vector2(185, -150), content, 8000);
                }

                playerShip.Update(gameTime);
                UpdateCarriers(gameTime);
                UpdatePowerups(gameTime);
                if (readyScreen == null || !readyScreen.Active)
                {

                    for (int i = 0; i < carriers.Count; i++)
                    {
                        carriers[i].Update(gameTime);
                    }
                }

                if (Boss != null)
                {
                    Boss.Update(gameTime);
                }

                UpdateBoss(gameTime);
                UpdateCollision();
                UpdateExplosions(gameTime);
                Hud.Update(gameTime, ScreenManager.PlayerScore, ScreenManager.Level, ScreenManager.PlayerLives);

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
                        LoadingScreen.Load(ScreenManager, true, PlayerIndex.One, new Level4_1());
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

            for (int i = 0; i < carriers.Count; i++)
            {
                carriers[i].Draw(spriteBatch);
            }

            for (int i = 0; i < powerups.Count; i++)
            {
                powerups[i].Draw(spriteBatch);
            }

            if (Boss != null)
            {
                Boss.Draw(spriteBatch);
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

        public void UpdateBoss(GameTime gameTime)
        {
            if (Boss != null)
            {
                for (int i = 0; i < Boss.Components.Count; i++)
                {
                    if (Boss.Components[i].Destroyed && !Boss.Components[i].Exploded)
                    {
                        Boss.Components[i].Exploded = true;
                        ScreenManager.PlayerScore += Boss.Components[i].Value;
                        Boss.Components[i].DeathTime = gameTime.TotalGameTime;
                        if (Boss.Components[i].Critical)
                        {
                            Boss.Criticals.Remove(Boss.Components[i]);
                        }
                    }

                    if (Boss.Components[i].Exploded)
                    {
                        if (Boss.Components[i].DeathTime + Boss.Components[i].ExplosionTime < gameTime.TotalGameTime)
                        {
                            if (Boss.Components[i].Explosions < Boss.Components[i].ExplosionCount)
                            {
                                Boss.Components[i].Explosions++;
                                Boss.Components[i].DeathTime = gameTime.TotalGameTime;
                                Vector2 location = Vector2.Zero;
                                location.X = Random.Next((int)Boss.Components[i].Position.X, (int)(Boss.Components[i].Position.X + Boss.Components[i].Texture.Width / 2));
                                location.Y = Random.Next((int)Boss.Components[i].Position.Y, (int)(Boss.Components[i].Position.Y + Boss.Components[i].Texture.Height));
                                AddExplosion(location, largeExplosionTexture, 120, 120, 11, 60, "large");
                            }
                        }
                    }

                    if (Boss.Defeated && !Boss.Exploded)
                    {
                        Boss.DeathTime = gameTime.TotalGameTime;
                        Boss.Exploded = true;
                        ScreenManager.PlayerScore += Boss.Value;
                    }

                    if (Boss.Exploded)
                    {
                        if (Boss.DeathTime + Boss.ExplosionTime < gameTime.TotalGameTime)
                        {
                            if (Boss.Explosions <= Boss.ExplosionCount)
                            {
                                Boss.Explosions++;
                                Boss.DeathTime = gameTime.TotalGameTime;
                                Vector2 location = Vector2.Zero;
                                location.X = Random.Next((int)Boss.Position.X, (int)(Boss.TotalSize.X + Boss.Position.X));
                                location.Y = Random.Next((int)Boss.Position.Y, (int)(Boss.TotalSize.Y + Boss.Position.Y));
                                AddExplosion(location, largeExplosionTexture, 120, 120, 11, 60, "large");
                            }
                        }
                    }

                    if (Boss.Criticals.Count == 0)
                    {
                        Boss.Defeated = true;
                    }

                    if (Boss.Defeated)
                    {
                        if (completeScreen.Active == false)
                        {
                            completeScreen.StartTime = gameTime.TotalGameTime;
                            completeScreen.Active = true;
                        }
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

            if (!playerShip.Dead)
            {
                if (playerShip.Health <= 0)
                {
                    playerShip.Dead = true;
                    ScreenManager.PlayerLives--;
                }

                if (Boss != null)
                {
                    for (int k = Boss.Projectiles.Count - 1; k >= 0; k--)
                    {
                        rectangle2 = new Rectangle((int)Boss.Projectiles[k].Position.X, (int)Boss.Projectiles[k].Position.Y, (int)Boss.Projectiles[k].Width, (int)Boss.Projectiles[k].Height);
                        if (rectangle1.Intersects(rectangle2))
                        {
                            if (playerShip.Shield != null && playerShip.Shield.Active == true)
                            {
                                if(!playerShip.Invincible)
                                    playerShip.Shield.Health--;
                            }
                            else
                            {
                                if (!playerShip.Invincible)
                                    playerShip.Health -= Boss.Projectiles[k].Damage;
                            }

                            int frameWidth = 8;
                            int frameHeight = 8;
                            int frameCount = 3;
                            int frameTime = 70;
                            Texture2D impactTexture = smallImpactTexture;

                            if (Boss.Projectiles[k].ImpactType == "large")
                            {
                                frameWidth = 16;
                                frameHeight = 16;
                                frameCount = 4;
                                frameTime = 70;
                                impactTexture = largeImpactTexture;
                            }

                            if (Boss.Projectiles[k].Uninterrupted == false)
                            {
                                Boss.Projectiles[k].Active = false;
                            }

                            if (!playerShip.Invincible)
                                AddImpact(new Vector2((Boss.Projectiles[k].Position.X - ((Boss.Projectiles[k].Texture.Width - impactTexture.Width) / 2)), Boss.Projectiles[k].Position.Y + Boss.Projectiles[k].Texture.Height), impactTexture, frameWidth, frameHeight, frameCount, frameTime);
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

                    if (Boss != null)
                    {
                        if (Boss.IsDeployed)
                        {
                            for (int l = 0; l < Boss.Components.Count; l++)
                            {
                                rectangle1 = new Rectangle((int)playerShip.projectiles[j].Position.X, (int)playerShip.projectiles[j].Position.Y, playerShip.projectiles[j].Width, playerShip.projectiles[j].Height);
                                rectangle2 = new Rectangle((int)Boss.Components[l].HitArea.X, (int)Boss.Components[l].HitArea.Y, (int)Boss.Components[l].HitArea.Width, (int)Boss.Components[l].HitArea.Height);

                                if (rectangle1.Intersects(rectangle2))
                                {
                                    if (Boss.Components[l].Damageable && Boss.Components[l].Health > 0) // Only subtract from the component's health if it can take damage
                                        Boss.Components[l].Health -= playerShip.projectiles[j].Damage;
                                    if (playerShip.projectiles[j].Uninterrupted == false)
                                    {
                                        if (Boss.Components[l].Damageable && !Boss.Components[l].Destroyed)
                                        {
                                            playerShip.projectiles[j].Active = false;
                                            Boss.Components[l].color = Color.Red;
                                        }
                                    }
                                    if (Boss.Components[l].Damageable && !Boss.Components[l].Destroyed)
                                        AddImpact(new Vector2((playerShip.projectiles[j].Position.X - ((playerShip.projectiles[j].Texture.Width - impactTexture.Width) / 2)), playerShip.projectiles[j].Position.Y), impactTexture, frameWidth, frameHeight, frameCount, frameTime);
                                }
                            }
                        }
                    }
                }
            }
        }

        #endregion
    }
}