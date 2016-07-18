#region File Description
//-----------------------------------------------------------------------------
// RightEye.cs
//
// Spearhead
// Copyright (C) Showerhead Studios. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace Spearhead
{
    /// <summary>
    /// This class creates the right eye boss component
    /// </summary>
    class RightEye : Component
    {
        #region Fields
        ContentManager Content; // The Content Manager
        #endregion

        public RightEye(Boss boss, Vector2 origin, Vector2 offset, ContentManager content, int health, int value, bool mirrored, bool collidable, bool critical)
            : base(boss, origin, offset, "Images/level-4-boss-right-eye", content, health, value, mirrored, collidable, critical)
        {
            Texture = content.Load<Texture2D>("Images/level-4-boss-right-eye");
            FrameSize = new Vector2(Texture.Width / 2, Texture.Height);
            Damageable = true;
            Destroyed = false;
            HitArea = new Rectangle((int)Position.X + 14, (int)Position.Y + 13, 59, 37);
            ExplosionCount = 3;
            Content = content;
        }

        public override void UpdateComponent(GameTime gameTime, Boss boss)
        {
            base.UpdateComponent(gameTime, boss);
            HitArea = new Rectangle((int)Position.X + 14, (int)Position.Y + 13, 59, 37);
        }

        public override void DrawSprite(SpriteBatch batch)
        {
            Rectangle sourceRect;
            if (Destroyed && Explosions >= 1)
                sourceRect = new Rectangle((int)FrameSize.X, 0, (int)FrameSize.X, (int)FrameSize.Y);
            else
                sourceRect = new Rectangle(0, 0, (int)FrameSize.X, (int)FrameSize.Y);
            batch.End();
            batch.Begin();
            if (Texture != null && Position != null)
                batch.Draw(Texture, Position, sourceRect, color);
            if (color == Color.Red)
                color = Color.White;
        }
    }
}