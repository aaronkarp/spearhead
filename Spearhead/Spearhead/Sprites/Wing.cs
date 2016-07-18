#region File Description
//-----------------------------------------------------------------------------
// Wing.cs
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
    /// This class creates the wing boss component
    /// </summary>
    class Wing : Component
    {
        #region Fields
        ContentManager Content; // The Content Manager
        Level5Boss l5boss;
        #endregion

        public Wing(Boss boss, Vector2 origin, Vector2 offset, ContentManager content, int health, int value, bool mirrored, bool collidable, bool critical)
            : base(boss, origin, offset, "Images/level-5-boss-wing", content, health, value, mirrored, collidable, critical)
        {
            Texture = content.Load<Texture2D>("Images/level-5-boss-wing");
            FrameSize = new Vector2(Texture.Width, Texture.Height);
            Damageable = false;
            Destroyed = false;
            HitArea = new Rectangle((int)Position.X, (int)Position.Y, (int)(Texture.Width / 2), Texture.Height);
            ExplosionCount = 4;
            Content = content;
            l5boss = (Level5Boss)boss;
        }

        public override void UpdateComponent(GameTime gameTime, Level5Boss boss, string leftDoorStatus, string rightDoorStatus)
        {
            base.UpdateComponent(gameTime, boss);
            HitArea = new Rectangle((int)Position.X, (int)Position.Y, (int)Texture.Width, (int)Texture.Height);
        }

        public override void DrawSprite(SpriteBatch batch)
        {
            if (l5boss != null && l5boss.Phase1Active)
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
}