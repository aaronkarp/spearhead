using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Spearhead
{
    class Projectile : Sprite
    {
        public Texture2D Texture;
        public Vector2 StartPosition;
        float VerticalSpeed;
        float LateralSpeed;
        int LateralDistance;
        public int Damage;
        public bool HurtPlayer;
        public bool Uninterrupted;
        bool vertical;
        public bool Active;
        public int Decay;
        public string ImpactType;

        public Projectile(Vector2 startPosition, ContentManager content, float verticalSpeed, float lateralSpeed, int lateralDistance, int damage, string texturePath, bool uninterrupted, bool hurtPlayer, int decay, string impactType)
            : base(content, texturePath)
        {
            Texture = content.Load<Texture2D>(texturePath);
            Position.X = startPosition.X;
            Position.Y = startPosition.Y;
            StartPosition = startPosition;
            VerticalSpeed = verticalSpeed;
            LateralSpeed = lateralSpeed;
            LateralDistance = lateralDistance;
            Damage = damage;
            HurtPlayer = hurtPlayer;
            Uninterrupted = uninterrupted;
            Active = true;
            vertical = false;
            Decay = decay;
            ImpactType = impactType;
        }

        public void Reset()
        {
            Position = StartPosition;
        }

        protected override void UpdateSprite(GameTime gameTime)
        {
            // Set the position of the projectile.
            // Positive Y values move down, negative Y values move up
            // Positive X values move right, negative X values move left
            Position.Y -= VerticalSpeed;
            Position.X += LateralSpeed;
            int verticalSpeedMod = 5;
            if (Decay != 0)
            {
                verticalSpeedMod = 1;
                if (LateralSpeed > 0)
                {
                    LateralSpeed -= Decay;
                    if (LateralSpeed < 0)
                        LateralSpeed = 0;
                }
                else
                {
                    LateralSpeed += Decay;
                    if (LateralSpeed > 0)
                        LateralSpeed = 0;
                }
            }

            // If the projectile is moving down the screen and has reached the bottom, make it inactive
            if ((VerticalSpeed < 0) && (Position.Y >= 800))
            {
                Active = false;
            }

            // If the projectile is moving up the screen and has reached the top, make it inactive
            if ((VerticalSpeed > 0) && (Position.Y <= 0))
            {
                Active = false;
            }

            // If the projectile is moving right and has passed beyond the right edge, make it inactive
            if (LateralSpeed > 0)
            {
                if (Position.X >= 480)
                {
                    Active = false;
                }

                if ((Position.X >= StartPosition.X + LateralDistance) && vertical == false)
                {
                    vertical = true;
                    LateralSpeed = 0;
                    VerticalSpeed *= verticalSpeedMod;
                }
            }

            // If the projectile is moving left and has passed beyond the left edge, make it inactive
            if (LateralSpeed < 0)
            {
                if (Position.X < 0)
                {
                    Active = false;
                }

                if ((Position.X <= StartPosition.X - LateralDistance) && vertical == false)
                {
                    vertical = true;
                    LateralSpeed = 0;
                    VerticalSpeed *= verticalSpeedMod;
                }
            }
        }

        public override void DrawSprite(SpriteBatch batch)
        {
            batch.Draw(Texture, Position, null, Color.White);
        }
    }
}
