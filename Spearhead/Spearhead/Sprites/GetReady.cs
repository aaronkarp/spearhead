using System.Collections.Generic;
using System;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Spearhead
{
    class GetReady
    {
        // The image representing the parallaxing background
        Texture2D texture;
        public TimeSpan StartTime;
        ScreenManager ScreenManager;
        TimeSpan DisplayTime;
        String LevelMessage;
        String ReadyMessage;
        Text LevelText;
        Text ReadyText;
        int readyHeight;
        int readyWidth;
        Color displayColor;
        Color textColor;
        Color shadowColor;
        public bool Active;
        SpriteFont font;
        PlayerShip PlayerShip;

        public GetReady(ContentManager content, ScreenManager screenManager, String texturePath, PlayerShip playerShip, string levelMessage)
        {
            texture = content.Load<Texture2D>(texturePath);
            font = content.Load<SpriteFont>("Fonts/squarefuture");
            PlayerShip = playerShip;
            StartTime = TimeSpan.Zero;
            ScreenManager = screenManager;
            DisplayTime = TimeSpan.FromSeconds(2.0f);
            LevelMessage = "LEVEL " + levelMessage;
            ReadyMessage = "GET READY";
            displayColor = Color.Black;
            textColor = Color.White;
            shadowColor = Color.CornflowerBlue;
            Active = false;
            readyHeight = 800;
            readyWidth = 480;
            LevelText = new Text(font, LevelMessage, new Vector2(readyWidth, readyHeight), textColor, shadowColor, Text.Alignment.Both, new Rectangle(0, 0, 480, 700));
            ReadyText = new Text(font, ReadyMessage, new Vector2(readyWidth, readyHeight), textColor, shadowColor, Text.Alignment.Both, new Rectangle(0, 100, 480, 700));
        }

        public void Update(GameTime gameTime)
        {
            if (StartTime != TimeSpan.Zero)
            {
                if (StartTime + DisplayTime < gameTime.TotalGameTime)
                {
                    Active = false;
                    ScreenManager.IsPaused = false;
                    if (PlayerShip.Dead)
                    {
                        PlayerShip.Reset();
                        PlayerShip.InvincibleStart = gameTime.TotalGameTime;
                        PlayerShip.Invincible = true;
                        PlayerShip.BlinkStart = gameTime.TotalGameTime;
                        PlayerShip.BlinkOn = true;
                    }
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (Active)
            {
                spriteBatch.Draw(texture, new Rectangle(0, 0, readyWidth, readyHeight), displayColor);
                LevelText.Draw(spriteBatch);
                ReadyText.Draw(spriteBatch);
            }
        }
    }
}
