using System.Collections.Generic;
using System;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Spearhead
{
    class SideScrollingBackground : GameScreen
    {
        // The image representing the parallaxing background
        Texture2D texture;

        // An array of positions of the parallaxing background
        Vector2[] positions;

        // The speed which the background is moving
        int speed;

        SpriteBatch SpriteBatch;

        public SideScrollingBackground()
        {
        }

        public void Initialize(ContentManager content, SpriteBatch spriteBatch, String texturePath, int screenWidth, int speed)
        {
            SpriteBatch = spriteBatch;

            // Load the background texture we'll be using
            texture = content.Load<Texture2D>(texturePath);

            // Set the speed of the background
            this.speed = speed;

            // If we divide the screen with the texture width then we can determine the number of tiles needed
            // We add 1 to it so that we won't have a gap in the tiling
            positions = new Vector2[screenWidth / texture.Width + 1];

            // Set the initial positions of the parallaxing background
            for (int i = 0; i < positions.Length; i++)
            {
                // We need the tiles to be next to each other to create a tiling effect
                positions[i] = new Vector2(i * texture.Width, 0);
            }
        }

        public void Update(GameTime gameTime)
        {
            // Update the positions of the background
            for (int i = 0; i < positions.Length; i++)
            {
                // Update position of the screen by adding the speed
                positions[i].X += speed;
                // If the speed has the background moving left
                if (speed <= 0)
                {
                    // Check the texture is out of view then put that texture at the right side of the screen
                    if (positions[i].X <= -texture.Width)
                    {
                        positions[i].X = texture.Width * (positions.Length - 1);
                    }
                }

                // If the speed has the background moving right
                else
                {
                    // Check if the texture is out of view then position it at the left side of the screen
                    if (positions[i].X >= texture.Width * (positions.Length - 1))
                    {
                        positions[i].X = -texture.Width;
                    }
                }
            }
        }

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch.Begin();
            for (int i = 0; i < positions.Length; i++)
            {
                SpriteBatch.Draw(texture, positions[i], Color.White);
            }
            SpriteBatch.End();
        }
    }
}
