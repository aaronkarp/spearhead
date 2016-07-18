#region File Description
//-----------------------------------------------------------------------------
// PhoneMainMenuScreen.cs
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
    class PhoneMainMenuScreen : PhoneMenuScreen
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

        private const string StateFilename = "SpearheadSavegame.xml";
        private const string OptionsFilename = "SpearheadOptions.xml";
        string continueLevel;
        int continueScore;
        bool vibrationOn;
        bool isTrialMode;

        #endregion

        #region Initialization
        public PhoneMainMenuScreen()
            : base("")
        {

            Button playButton = new Button("New Game");
            playButton.Tapped += playButton_Tapped;
            MenuButtons.Add(playButton);
            isTrialMode = Guide.IsTrialMode;

            vibrationOn = true;

            using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (storage.FileExists(StateFilename))
                {
                    using (IsolatedStorageFileStream stream = storage.OpenFile(StateFilename, FileMode.Open))
                    {
                        XDocument doc = XDocument.Load(stream);
                        foreach (XElement saveElem in doc.Root.Elements("SaveData"))
                        {
                            continueLevel = saveElem.Attribute("Level").Value;
                            continueScore = (int)saveElem.Attribute("Score");
                        }
                    }
                }
                if (storage.FileExists(OptionsFilename))
                {
                    using (IsolatedStorageFileStream stream = storage.OpenFile(OptionsFilename, FileMode.Open))
                    {
                        XDocument doc = XDocument.Load(stream);
                        foreach (XElement optElem in doc.Root.Elements("OptionsData"))
                        {
                            if (optElem.Attribute("Vibration").Value == "true")
                                vibrationOn = true;
                            else
                                vibrationOn = false;
                        }
                    }
                }
            }

            if (continueLevel != null)
            {
                Button continueButton = new Button("Continue Game");
                continueButton.Tapped += continueButton_Tapped;
                MenuButtons.Add(continueButton);
            }

            Button optionsButton = new Button("Options");
            optionsButton.Tapped += optionsButton_Tapped;
            MenuButtons.Add(optionsButton);

            if (isTrialMode)
            {
                Button fullModeButton = new Button("Get Full Version");
                fullModeButton.Tapped += fullModeButton_Tapped;
                MenuButtons.Add(fullModeButton);
            }
        }


        public override void Activate(bool instancePreserved)
        {

            if (content == null)
                content = new ContentManager(ScreenManager.Game.Services, "Content");

            title = content.Load<Texture2D>("Images/title");

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

            titleX = -596;
            titleY = 250;

            ScreenManager.VibrationOn = vibrationOn;

            copyrightText = new Text(titleFont, "Copyright 2012 Showerhead Studios", new Vector2(0, 760), Color.White, Color.White, false, Text.Alignment.Horizontal, new Rectangle(0, 760, 480, 27));

            base.Activate(instancePreserved);

        }
        #endregion

        void playButton_Tapped(object sender, EventArgs e)
        {
            // When the "Play" button is tapped, we load the GameplayScreen
            LoadingScreen.Load(ScreenManager, true, PlayerIndex.One, new Level1_1());
        }

        void continueButton_Tapped(object sender, EventArgs e)
        {
            LoadingScreen.Load(ScreenManager, true, PlayerIndex.One, new Level1_2());
            switch (continueLevel)
            {
                case "1-2":
                    ScreenManager.PlayerScore = continueScore;
                    LoadingScreen.Load(ScreenManager, true, PlayerIndex.One, new Level1_2());
                    break;
                case "1-3":
                    ScreenManager.PlayerScore = continueScore;
                    LoadingScreen.Load(ScreenManager, true, PlayerIndex.One, new Level1_3());
                    break;
                case "2-1":
                    ScreenManager.PlayerScore = continueScore;
                    LoadingScreen.Load(ScreenManager, true, PlayerIndex.One, new Level2_1());
                    break;
                case "2-2":
                    ScreenManager.PlayerScore = continueScore;
                    LoadingScreen.Load(ScreenManager, true, PlayerIndex.One, new Level2_2());
                    break;
                case "2-3":
                    ScreenManager.PlayerScore = continueScore;
                    LoadingScreen.Load(ScreenManager, true, PlayerIndex.One, new Level2_3());
                    break;
                case "3-1":
                    ScreenManager.PlayerScore = continueScore;
                    LoadingScreen.Load(ScreenManager, true, PlayerIndex.One, new Level3_1());
                    break;
                case "3-2":
                    ScreenManager.PlayerScore = continueScore;
                    LoadingScreen.Load(ScreenManager, true, PlayerIndex.One, new Level3_2());
                    break;
                case "3-3":
                    ScreenManager.PlayerScore = continueScore;
                    LoadingScreen.Load(ScreenManager, true, PlayerIndex.One, new Level3_3());
                    break;
                case "4-1":
                    ScreenManager.PlayerScore = continueScore;
                    LoadingScreen.Load(ScreenManager, true, PlayerIndex.One, new Level4_1());
                    break;
                case "4-2":
                    ScreenManager.PlayerScore = continueScore;
                    LoadingScreen.Load(ScreenManager, true, PlayerIndex.One, new Level4_2());
                    break;
                case "4-3":
                    ScreenManager.PlayerScore = continueScore;
                    LoadingScreen.Load(ScreenManager, true, PlayerIndex.One, new Level4_3());
                    break;
                case "5-1":
                    ScreenManager.PlayerScore = continueScore;
                    LoadingScreen.Load(ScreenManager, true, PlayerIndex.One, new Level5_1());
                    break;
                case "5-2":
                    ScreenManager.PlayerScore = continueScore;
                    LoadingScreen.Load(ScreenManager, true, PlayerIndex.One, new Level5_2());
                    break;
                case "5-3":
                    ScreenManager.PlayerScore = continueScore;
                    LoadingScreen.Load(ScreenManager, true, PlayerIndex.One, new Level5_3());
                    break;
            }
        }

        void fullModeButton_Tapped(object sender, EventArgs e)
        {
            Guide.ShowMarketplace(PlayerIndex.One);
        }

        void optionsButton_Tapped(object sender, EventArgs e)
        {
            LoadingScreen.Load(ScreenManager, true, PlayerIndex.One, new OptionsScreen());
        }

        void sfxButton_Tapped(object sender, EventArgs e)
        {
            BooleanButton button = sender as BooleanButton;

            // In a real game, you'd want to store away the value of 
            // the button to turn off sounds here. :)
        }

        void musicButton_Tapped(object sender, EventArgs e)
        {
            BooleanButton button = sender as BooleanButton;

            // In a real game, you'd want to store away the value of 
            // the button to turn off music here. :)
        }

        protected override void OnCancel()
        {
            ScreenManager.Game.Exit();
            base.OnCancel();
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            foreach (SideScrollingBackground background in backgrounds)
            {
                background.Update(gameTime);
            }

            if (titleX < 44)
            {
                titleX += 32;
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
