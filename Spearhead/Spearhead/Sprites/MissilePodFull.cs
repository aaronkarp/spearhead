#region File Description
//-----------------------------------------------------------------------------
// MissilePodFull.cs
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
    /// This class creates a full missile tube graphic for the missile pod component
    /// </summary>
    class MissilePodFull : Component
    {
        #region Fields
        #endregion

        public MissilePodFull(Boss boss, Vector2 origin, Vector2 offset, ContentManager content, int health, int value, bool mirrored, bool collidable, bool critical)
            : base(boss, origin, offset, "Images/MissilePodFull", content, health, value, mirrored, collidable, critical)
        {
            FrameSize = new Vector2(80, 60);
            Texture = content.Load<Texture2D>("Images/MissilePodFull");
            Destroyed = false;
            Status = "Full";
            Damageable = false;
            ExplosionCount = 0;
        }

        public override void UpdateComponent(GameTime gameTime, Boss boss)
        {
            base.UpdateComponent(gameTime, boss);
            HitArea = new Rectangle((int)Position.X, (int)Position.Y, (int)(Texture.Width / 2), Texture.Height);
            // HitArea = new Rectangle((int)Position.X, (int)Position.Y, Texture.Width, Texture.Height);
            // TO DO: Add collision and firing routines
        }

        public override void DrawSprite(SpriteBatch batch)
        {
            // The missiles disappear when the missile pod is destroyed
            if (!Destroyed && Status == "Full")
            {
                batch.End();
                batch.Begin();
                batch.Draw(Texture, Position, color);
                if (color == Color.Red)
                    color = Color.White;
            }
        }
    }
}