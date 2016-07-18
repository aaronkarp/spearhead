using System.Collections.Generic;
using System;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Spearhead
{
    class LevelComplete
    {
        // The image representing the parallaxing background
        Texture2D texture;
        public TimeSpan StartTime;
        TimeSpan DisplayTime;
        String CompleteMessage;
        String BonusMessage;
        Text CompleteText;
        Text BonusText;
        int readyHeight;
        int readyWidth;
        Color displayColor;
        Color textColor;
        Color shadowColor;
        public bool Active;
        SpriteFont font;
        int PlayerScore;
        public bool Finished;

        public LevelComplete(ContentManager content, int playerScore, String texturePath, String levelMessage, int levelBonus)
        {
            PlayerScore = playerScore;
            PlayerScore += levelBonus;
            texture = content.Load<Texture2D>(texturePath);
            font = content.Load<SpriteFont>("Fonts/squarefuture");
            StartTime = TimeSpan.Zero;
            DisplayTime = TimeSpan.FromSeconds(4.0f);
            CompleteMessage = "LEVEL " + levelMessage + " COMPLETE";
            BonusMessage = "BONUS: " + levelBonus.ToString();
            displayColor = Color.Black;
            textColor = Color.White;
            shadowColor = Color.CornflowerBlue;
            Active = false;
            readyHeight = 800;
            readyWidth = 480;
            CompleteText = new Text(font, CompleteMessage, new Vector2(readyWidth, readyHeight), textColor, shadowColor, Text.Alignment.Both, new Rectangle(0, 0, 480, 700));
            BonusText = new Text(font, BonusMessage, new Vector2(readyWidth, readyHeight), textColor, shadowColor, Text.Alignment.Both, new Rectangle(0, 100, 480, 700));
            Finished = false;
        }

        public void Update(GameTime gameTime)
        {
            if (StartTime != TimeSpan.Zero)
            {
                if (StartTime + DisplayTime < gameTime.TotalGameTime)
                {
                    Active = false;
                    Finished = true;
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (Active)
            {
                spriteBatch.Draw(texture, new Rectangle(0, 0, readyWidth, readyHeight), displayColor);
                CompleteText.Draw(spriteBatch);
                BonusText.Draw(spriteBatch);
            }
        }
    }
}
