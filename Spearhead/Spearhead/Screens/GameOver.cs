#region File Description
//-----------------------------------------------------------------------------
// GameOver.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.IO;
using System.IO.IsolatedStorage;
using System.Xml.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.GamerServices;
#endregion

namespace Spearhead
{
    class GameOver : PhoneMenuScreen
    {
        #region Fields
        SideScrollingBackground starLevel1;
        SideScrollingBackground starLevel2;
        SideScrollingBackground starLevel3;
        SideScrollingBackground maskLevel;
        List<SideScrollingBackground> backgrounds;
        TimeSpan GameOverStart;
        TimeSpan GameOverDuration;

        SpriteFont titleFont;

        #endregion

        #region Initialization
        public GameOver()
            : base("")
        {

            Button gameOverButton = new Button("Game Over");
            gameOverButton.Tapped += gameOverButton_Tapped;
            MenuButtons.Add(gameOverButton);
        }


        public override void Activate(bool instancePreserved)
        {

            if (content == null)
                content = new ContentManager(ScreenManager.Game.Services, "Content");

            GameOverStart = ScreenManager.GlobalGameTime;
            GameOverDuration = TimeSpan.FromSeconds(4f);

            // Scrolling Background layers
            backgrounds = new List<SideScrollingBackground>();
            starLevel1 = new SideScrollingBackground();
            starLevel2 = new SideScrollingBackground();
            starLevel3 = new SideScrollingBackground();
            maskLevel = new SideScrollingBackground();
            starLevel1.Initialize(content, ScreenManager.SpriteBatch, "Images/starfield_level1", 480, -2);
            starLevel2.Initialize(content, ScreenManager.SpriteBatch, "Images/starfield_level2", 480, -4);
            starLevel3.Initialize(content, ScreenManager.SpriteBatch, "Images/starfield_level3", 480, -8);
            maskLevel.Initialize(content, ScreenManager.SpriteBatch, "Images/starfield_mask", 480, -1);
            backgrounds.Add(starLevel1);
            backgrounds.Add(starLevel2);
            backgrounds.Add(starLevel3);
            backgrounds.Add(maskLevel);
            titleFont = ScreenManager.ScoreFont;

            base.Activate(instancePreserved);

        }
        #endregion

        void gameOverButton_Tapped(object sender, EventArgs e)
        {

            LoadingScreen.Load(ScreenManager, false, null, new BackgroundScreen(),
                                                           new PhoneMainMenuScreen());
        }

        protected override void OnCancel()
        {
            LoadingScreen.Load(ScreenManager, false, null, new BackgroundScreen(),
                                                           new PhoneMainMenuScreen());
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            foreach (SideScrollingBackground background in backgrounds)
            {
                background.Update(gameTime);
            }

            if (gameTime.TotalGameTime >= GameOverStart + GameOverDuration)
                LoadingScreen.Load(ScreenManager, false, null, new BackgroundScreen(),
                                                           new PhoneMainMenuScreen());

            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            foreach (SideScrollingBackground background in backgrounds)
            {
                background.Draw(gameTime);
            }

            spriteBatch.Begin();

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
