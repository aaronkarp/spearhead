#region File Description
//-----------------------------------------------------------------------------
// OptionsScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

using System;
using System.IO;
using System.IO.IsolatedStorage;
using System.Xml.Linq;

namespace Spearhead
{
    /// <summary>
    /// A screen where users can configure the game's options
    /// </summary>
    class OptionsScreen : PhoneMenuScreen
    {
        private const string OptionsFilename = "SpearheadOptions.xml";
        bool vibrationSave;

        public OptionsScreen()
            : base("Options")
        {
            vibrationSave = true;
            using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (storage.FileExists(OptionsFilename))
                {
                    using (IsolatedStorageFileStream stream = storage.OpenFile(OptionsFilename, FileMode.Open))
                    {
                        XDocument doc = XDocument.Load(stream);
                        foreach (XElement optElem in doc.Root.Elements("OptionsData"))
                        {
                            vibrationSave = (bool)optElem.Attribute("Vibration");
                        }
                    }
                }
            }

            Button exitButton = new Button("Exit");
            exitButton.Tapped += exitButton_Tapped;
            MenuButtons.Add(exitButton);

            BooleanButton vibrationButton = new BooleanButton("Vibration", vibrationSave);
            vibrationButton.Tapped += vibrationButton_Tapped;
            MenuButtons.Add(vibrationButton);
        }

        /// <summary>
        /// The "Exit" button handler uses the LoadingScreen to take the user out to the main menu.
        /// </summary>
        void exitButton_Tapped(object sender, EventArgs e)
        {
            // Save options to Isolated Storage
            using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                XDocument doc = new XDocument();
                XElement root = new XElement("Options");
                doc.Add(root);
                root.Add(new XElement("OptionsData", new XAttribute("Vibration", ScreenManager.VibrationOn)));

                using (IsolatedStorageFileStream stream = storage.CreateFile(OptionsFilename))
                {
                    doc.Save(stream);
                }
            }
            LoadingScreen.Load(ScreenManager, false, null, new BackgroundScreen(), new PhoneMainMenuScreen());
        }

        void vibrationButton_Tapped(object sender, EventArgs e)
        {
            BooleanButton button = sender as BooleanButton;
            ScreenManager.VibrationOn = button.value;
        }

        protected override void OnCancel()
        {
            // Save options to Isolated Storage
            using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                XDocument doc = new XDocument();
                XElement root = new XElement("Options");
                doc.Add(root);
                root.Add(new XElement("OptionsData", new XAttribute("Vibration", ScreenManager.VibrationOn)));

                using (IsolatedStorageFileStream stream = storage.CreateFile(OptionsFilename))
                {
                    doc.Save(stream);
                }
            }
            LoadingScreen.Load(ScreenManager, false, null, new BackgroundScreen(), new PhoneMainMenuScreen());
        }
    }
}
