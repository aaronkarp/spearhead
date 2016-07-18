using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Spearhead
{
    class Hud : Sprite
    {
        Texture2D Texture;
        Texture2D lifeIndicator;
        PlayerShip PlayerShip;
        Rectangle hudBox;
        Rectangle hudLine;
        Rectangle outerBox;
        Rectangle innerBox;
        Rectangle healthBox;
        Color statusColor;
        Color warnColor;
        Color critColor;
        Color baseColor;
        TimeSpan blinkTime;
        TimeSpan previousBlinkTime;
        SpriteFont hudFont;
        String healthString;
        Text healthText;
        String scoreString;
        Text scoreText;
        String levelString;
        Text levelText;
        String livesString;
        Text livesText;

        public Hud(ContentManager content, string texturePath, PlayerShip playerShip)
            : base(content, texturePath)
        {
            Texture = content.Load<Texture2D>(texturePath);
            lifeIndicator = content.Load<Texture2D>("Images/player_small");
            hudFont = content.Load<SpriteFont>("Fonts/squarefuture_small");
            scoreString = "SCORE: 0";
            scoreText = new Text(hudFont, scoreString, new Vector2(12, 9), Color.White);
            healthString = "HULL:";
            healthText = new Text(hudFont, healthString, new Vector2(12, 38), Color.White);
            levelString = "LEVEL ";
            levelText = new Text(hudFont, levelString, new Vector2(322, 9), Color.White);
            livesString = " ";
            livesText = new Text(hudFont, livesString, new Vector2(455, 38), Color.White);
            hudBox = new Rectangle(0, 0, 480, 65);
            hudLine = new Rectangle(0, 65, 480, 1);
            outerBox = new Rectangle(105, 31, 204, 28);
            innerBox = new Rectangle(106, 32, 202, 26);
            healthBox = new Rectangle(107, 33, 200, 24);
            statusColor = Color.Lime;
            warnColor = Color.Yellow;
            critColor = Color.Red;
            baseColor = new Color(61, 61, 61, 255);
            PlayerShip = playerShip;
            previousBlinkTime = TimeSpan.Zero;
            blinkTime = TimeSpan.FromSeconds(.5f);
        }

        public void Reset()
        {
        }

        protected override void UpdateSprite(GameTime gameTime, int playerScore, string level, int lives)
        {

            scoreText.ChangeText("Score: " + playerScore.ToString());
            levelText.ChangeText("LEVEL " + level);
            livesText.ChangeText(lives.ToString());
            // Change the health display's color based on ship health
            if (PlayerShip.Health <= 100 && PlayerShip.Health >= 66)
            {
                statusColor = Color.Lime;
            }

            if (PlayerShip.Health < 66 && PlayerShip.Health > 33)
            {
                statusColor = Color.Yellow;
            }

            if (PlayerShip.Health < 33)
            {
                statusColor = critColor;

                // If ship health is critical, flash the health display as a warning
                if (PlayerShip.Health <= 25)
                {
                    if (gameTime.TotalGameTime - previousBlinkTime > blinkTime)
                    {
                        previousBlinkTime = gameTime.TotalGameTime;

                        if (statusColor.R == 255)
                        {
                            statusColor = Color.White;
                        }
                        else
                        {
                            statusColor = critColor;
                        }
                    }
                }

                if (PlayerShip.Health <= 10)
                {
                    blinkTime = TimeSpan.FromSeconds(.25f);
                }
            }

            healthBox = new Rectangle(107, 33, PlayerShip.Health * 2, 24);
        }

        public override void DrawSprite(SpriteBatch batch)
        {
            batch.Draw(Texture, hudBox, baseColor);
            batch.Draw(Texture, hudLine, Color.White);
            scoreText.Draw(batch);
            healthText.Draw(batch);
            levelText.Draw(batch);
            livesText.Draw(batch);
            batch.Draw(lifeIndicator, new Rectangle(423, 32, 16, 27), Color.White);
            batch.Draw(Texture, outerBox, Color.White);
            batch.Draw(Texture, innerBox, baseColor);
            batch.Draw(Texture, healthBox, statusColor);
        }
    }
}
