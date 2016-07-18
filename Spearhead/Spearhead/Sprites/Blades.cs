#region File Description
//-----------------------------------------------------------------------------
// Blades.cs
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
    /// This class creates the decorative blades for the Death's Head boss
    /// </summary>
    class Blades : Component
    {
        #region Fields
        ContentManager Content; // The Content Manager
        #endregion

        public Blades(Boss boss, Vector2 origin, Vector2 offset, ContentManager content, int health, int value, bool mirrored, bool collidable, bool critical)
            : base(boss, origin, offset, "Images/level-4-boss-blades", content, health, value, mirrored, collidable, critical)
        {
            Texture = content.Load<Texture2D>("Images/level-4-boss-blades");
            FrameSize = new Vector2(Texture.Width, Texture.Height);
            Damageable = false;
            Destroyed = false;
            HitArea = new Rectangle((int)Position.X, (int)Position.Y, Texture.Width, Texture.Height);
            ExplosionCount = 0;
            Content = content;
        }

        public override void UpdateComponent(GameTime gameTime, Boss boss)
        {
            base.UpdateComponent(gameTime, boss);
        }

        public override void DrawSprite(SpriteBatch batch)
        {
            batch.Draw(Texture, Position, color);
            if (color == Color.Red)
                color = Color.White;
        }
    }
}