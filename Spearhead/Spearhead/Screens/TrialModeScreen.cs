#region File Description
//-----------------------------------------------------------------------------
// TrialModeScreen.cs
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
    class TrialModeScreen : PhoneMenuScreen
    {
        #region Fields
        SideScrollingBackground starLevel1;
        SideScrollingBackground starLevel2;
        SideScrollingBackground starLevel3;
        SideScrollingBackground maskLevel;
        List<SideScrollingBackground> backgrounds;
        Texture2D title;

        SpriteFont titleFont;
        Text copyrightText;

        int titleX;
        int titleY;

        #endregion

        #region Initialization
        public TrialModeScreen()
            : base("")
        {

            Button buyButton = new Button("Get Full Version");
            buyButton.Tapped += buyButton_Tapped;
            MenuButtons.Add(buyButton);

        }


        public override void Activate(bool instancePreserved)
        {

            if (content == null)
                content = new ContentManager(ScreenManager.Game.Services, "Content");

            title = content.Load<Texture2D>("Images/TrialMode");

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

            titleX = 0;
            titleY = 25;

            copyrightText = new Text(titleFont, "Copyright 2012 Showerhead Studios", new Vector2(0, 760), Color.White, Color.White, false, Text.Alignment.Horizontal, new Rectangle(0, 760, 480, 27));

            base.Activate(instancePreserved);

        }
        #endregion

        void buyButton_Tapped(object sender, EventArgs e)
        {
            Guide.ShowMarketplace(PlayerIndex.One);
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

            copyrightText.Draw(spriteBatch);

            spriteBatch.Draw(title, new Rectangle(titleX, titleY, title.Width, title.Height), Color.White);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
