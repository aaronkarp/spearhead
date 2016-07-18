using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Spearhead
{
    class Sprite
    {
        public Vector2 Position;
        public delegate void CollisionDelegate();

        public Texture2D texture;
        public Color color = Color.White;

        public Sprite(ContentManager content, string assetName)
        {
            texture = content.Load<Texture2D>(assetName);
        }

        public void Update(GameTime gameTime)
        {
            UpdateSprite(gameTime);
        }

        public void Update(GameTime gameTime, int playerScore, string level, int playerLives)
        {
            UpdateSprite(gameTime, playerScore, level, playerLives);
        }

        protected virtual void UpdateSprite(GameTime gameTime)
        {
        }

        protected virtual void UpdateSprite(GameTime gameTime, int playerScore, string level, int playerLives)
        {
        }

        public void Draw(SpriteBatch batch)
        {
            DrawSprite(batch);
        }

        public virtual void DrawSprite(SpriteBatch batch)
        {
        }

        public bool IsCollidingWith(Sprite spriteToCheck)
        {
            return CollisionRectangle.Intersects(spriteToCheck.CollisionRectangle);
        }

        public Rectangle CollisionRectangle
        {
            get
            {
                return new Rectangle((int)Position.X, (int)Position.Y, Width, Height);
            }
        }

        public int Height
        {
            get { return texture.Height; }
        }

        public int Width
        {
            get { return texture.Width; }
        }
    }
}
