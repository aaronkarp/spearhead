#region File Description
//-----------------------------------------------------------------------------
// MissilePod.cs
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
    /// This class creates the missile pod component
    /// NOTE: This does not create the pod door or filled missile tube components
    /// </summary>
    class MissilePod : Component
    {
        #region Fields
        #endregion

        public MissilePod(Boss boss, Vector2 origin, Vector2 offset, ContentManager content, int health, int value, bool mirrored, bool collidable, bool critical)
            : base(boss, origin, offset, "images/MissilePod", content, health, value, mirrored, collidable, critical)
        {
            FrameSize = new Vector2(80, 250);
            Texture = content.Load<Texture2D>("Images/MissilePod");
            Destroyed = false;
            Damageable = false;
            ExplosionCount = 1;
        }

        public override void UpdateComponent(GameTime gameTime, Boss boss)
        {
            base.UpdateComponent(gameTime, boss);
            HitArea = new Rectangle((int)Position.X, (int)Position.Y + 82, 80, 60);
            // TO DO: Add collision and firing routines
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