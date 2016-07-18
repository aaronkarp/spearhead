#region File Description
//-----------------------------------------------------------------------------
// Powerup.cs
//
// Spearhead
// Copyright (C) Showerhead Studios. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace Spearhead
{
    class Powerup : Sprite
    {

        #region Fields
        protected Texture2D Texture;
        public bool Active;
        float EnemyMoveSpeed;
        protected int TextureOffset;
        #endregion

        public Powerup(Vector2 startPosition, ContentManager content, string texturePath)
            : base(content, texturePath)
        {
            Texture = content.Load<Texture2D>(texturePath);
            Position.X = startPosition.X;
            Position.Y = startPosition.Y;
            Active = true;
            EnemyMoveSpeed = 3.0f;
        }

        protected override void UpdateSprite(GameTime gameTime)
        {
            Position.Y += EnemyMoveSpeed;
            // (Math.Sin((speed) / time adjustment) * amplitude) + center adjustment
            Position.X = (int)((float)Math.Sin((Position.Y) / 35) * 225) + 220;

            if (Position.Y > 800)
            {
                Active = false;
            }
        }

        public virtual void Activate(ScreenManager screenManager, PlayerShip playerShip)
        {
        }
    }
}
