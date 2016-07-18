#region Using Statements
using System;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
#endregion

namespace Spearhead
{
    class Splash : GameScreen
    {
        #region Fields

        ContentManager content;
        private int splashAlpha = 0;
        Texture2D splashScreen;
        Rectangle screenPos = new Rectangle(0, 0, 480, 800);

        bool menuActive = false;

        #endregion

        #region Initialization

        public Splash()
        {
            TransitionOnTime = TimeSpan.FromSeconds(0.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
        }

        public override void Activate(bool instancePreserved)
        {
            if (!instancePreserved)
            {
                if (content == null)
                    content = new ContentManager(ScreenManager.Game.Services, "Content");

                splashScreen = content.Load<Texture2D>("Images/splash-screen");
                
                ScreenManager.Game.ResetElapsedTime();
            }
        }

        public override void Deactivate()
        {
            base.Deactivate();
        }

        public override void Unload()
        {
            content.Unload();
        }

        #endregion

        #region Update and Draw

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, false);

            if (splashAlpha < 255)
            {
                splashAlpha += 2;
            }

            if (gameTime.TotalGameTime.Seconds > 5 && menuActive == false)
            {
                menuActive = true;
                ScreenManager.AddScreen(new PhoneMainMenuScreen(), null);
                ScreenManager.RemoveScreen(this);
            }
                
        }

        public override void HandleInput(GameTime gameTime, InputState input)
        {
            if (input.TouchState.Count > 0)
            {
                menuActive = true;
                ScreenManager.AddScreen(new PhoneMainMenuScreen(), null);
                ScreenManager.RemoveScreen(this);
            }
            base.HandleInput(gameTime, input);
        }

        public override void Draw(GameTime gameTime)
        {
            ScreenManager.GraphicsDevice.Clear(ClearOptions.Target, Color.White, 0, 0);
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            spriteBatch.Begin();
            spriteBatch.Draw(splashScreen, screenPos, new Color(255, 255, 255, splashAlpha));
            spriteBatch.End();
        }

        #endregion

    }
}
