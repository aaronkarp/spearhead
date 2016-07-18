#region File Description
//-----------------------------------------------------------------------------
// Ending.cs
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
    /// This screen implements the ending of the game
    /// </summary>
    class Ending : GameScreen
    {
        #region Fields

        ContentManager content;
        GraphicsDevice graphics;
        ScrollingBackground starLevel1;
        ScrollingBackground starLevel2;
        ScrollingBackground starLevel3;
        ScrollingBackground maskLevel;
        ScrollText EndingText;
        List<ScrollingBackground> backgrounds;

        InputAction pauseAction;

        #endregion

        #region Initialization

        public Ending()
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
            if (!instancePreserved)
            {
                if (content == null)
                    content = new ContentManager(ScreenManager.Game.Services, "Content");

                graphics = ScreenManager.GraphicsDevice;

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

                EndingText = new ScrollText(new Vector2(0, 850), content, "Images/EndText", 4f);

                // Zero out ActiveTime and set IsPaused to false
                ScreenManager.ActiveTime = 0;
                ScreenManager.IsPaused = false;
            }
            else
            {
                ScreenManager.AddScreen(new PhonePauseScreen(), ControllingPlayer);
            }

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

            if (IsActive)
            {
                if (EndingText.Position.Y < -1350)
                {
                    LoadingScreen.Load(ScreenManager, false, null, new BackgroundScreen(), new PhoneMainMenuScreen());
                    ScreenManager.RemoveScreen(this);
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
            EndingText.Draw(spriteBatch);
            spriteBatch.End();

        }

        #endregion
    }
}