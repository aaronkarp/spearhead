#region File Description
//-----------------------------------------------------------------------------
// Skull.cs
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
    /// This class creates the skull boss component
    /// </summary>
    class Skull : Component
    {
        #region Fields
        ContentManager Content; // The Content Manager
        #endregion

        public Skull(Boss boss, Vector2 origin, Vector2 offset, ContentManager content, int health, int value, bool mirrored, bool collidable, bool critical)
            : base(boss, origin, offset, "Images/level-4-boss-skull", content, health, value, mirrored, collidable, critical)
        {
            Texture = content.Load<Texture2D>("Images/level-4-boss-skull");
            FrameSize = new Vector2(Texture.Width, Texture.Height);
            Damageable = false;
            Destroyed = false;
            HitArea = new Rectangle((int)Position.X, (int)Position.Y, (int)(Texture.Width / 2), Texture.Height);
            ExplosionCount = 4;
            Content = content;
        }

        public override void UpdateComponent(GameTime gameTime, Boss boss)
        {
            base.UpdateComponent(gameTime, boss);
            HitArea = new Rectangle((int)Position.X, (int)Position.Y, (int)Texture.Width, (int)Texture.Height);
        }

        public override void DrawSprite(SpriteBatch batch)
        {
            batch.End();
            batch.Begin();
            if (Texture != null && Position != null)
                batch.Draw(Texture, Position, color);
            if (color == Color.Red)
                color = Color.White;
        }
    }
}