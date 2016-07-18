using System.Collections.Generic;
using System;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Spearhead
{
    class ScrollingBackground : GameScreen
    {
        // The image representing the parallaxing background
        Texture2D texture;

        // An array of positions of the parallaxing background
        public Vector2[] positions;

        // The speed which the background is moving
        int speed;

        SpriteBatch SpriteBatch;

        public ScrollingBackground()
        {
        }

        public void Initialize(ContentManager content, SpriteBatch spriteBatch, String texturePath, int screenHeight, int speed)
        {
            SpriteBatch = spriteBatch;

            // Load the background texture we'll be using
            texture = content.Load<Texture2D>(texturePath);

            // Set the speed of the background
            this.speed = speed;

            // If we divide the screen with the texture height then we can determine the number of tiles needed
            // We add 1 to it so that we won't have a gap in the tiling
            positions = new Vector2[screenHeight / texture.Height + 1];

            // Set the initial positions of the parallaxing background
            for (int i = 0; i < positions.Length; i++)
            {
                // We need the tiles to be on top of each other to create a tiling effect
                positions[i] = new Vector2(0, i * texture.Height);
            }
        }

        public void Update(GameTime gameTime)
        {
            // Update the positions of the background
            for (int i = 0; i < positions.Length; i++)
            {
                // Update position of e screen by adding the speed
                positions[i].Y += speed;
                // If the speed has the background moving up
                if (speed <= 0)
                {
                    // Check the texture is out of view then put that texture at the top of the screen
                    if (positions[i].Y <= -texture.Height)
                    {
                        positions[i].Y = texture.Height * (positions.Length - 1) + gameTime.ElapsedGameTime.Milliseconds * speed;
                    }
                }

                // If the speed has the background moving down
                else
                {
                    // Check if the texture is out of view then position it at the bottom of the screen
                    if (positions[i].Y >= texture.Height * (positions.Length - 1))
                    {
                        positions[i].Y = -texture.Height + speed * 2;
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
