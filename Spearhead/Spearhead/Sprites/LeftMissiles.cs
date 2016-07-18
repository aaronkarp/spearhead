#region File Description
//-----------------------------------------------------------------------------
// LeftMissiles.cs
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
    /// This class creates the left missile bay component
    /// </summary>
    class LeftMissiles : Component
    {
        #region Fields
        Rectangle sourceRect;
        Level5Boss l5boss;
        #endregion

        public LeftMissiles(Boss boss, Vector2 origin, Vector2 offset, ContentManager content, int health, int value, bool mirrored, bool collidable, bool critical)
            : base(boss, origin, offset, "images/level-5-boss-wing-left-missiles-spritesheet", content, health, value, mirrored, collidable, critical)
        {
            Texture = content.Load<Texture2D>("Images/level-5-boss-wing-left-missiles-spritesheet");
            FrameSize = new Vector2(Texture.Width / 2, Texture.Height);
            Destroyed = false;
            Damageable = true;
            ExplosionCount = 0;
            sourceRect = new Rectangle(0, 0, (int)FrameSize.X, (int)FrameSize.Y);
            l5boss = (Level5Boss)boss;
        }

        public override void UpdateComponent(GameTime gameTime, Level5Boss boss, string leftDoorStatus, string rightDoorStatus)
        {
            base.UpdateComponent(gameTime, boss);
            if (leftDoorStatus == "Firing" || leftDoorStatus == "Closing" || Destroyed)
            {
                sourceRect.X = (int)FrameSize.X;
                sourceRect.Y = 0;
                sourceRect.Width = (int)FrameSize.X;
                sourceRect.Height = (int)FrameSize.Y;
            }
            else
            {
                sourceRect.X = 0;
                sourceRect.Y = 0;
                sourceRect.Width = (int)FrameSize.X;
                sourceRect.Height = (int)FrameSize.Y;
            }
            if (Destroyed)
                boss.LeftMissileDoor.Health = 0;
            HitArea = new Rectangle((int)Position.X, (int)Position.Y, (int)FrameSize.X, (int)FrameSize.Y);
        }

        public override void DrawSprite(SpriteBatch batch)
        {
            if (l5boss != null && l5boss.Phase1Active)
            {
                batch.End();
                batch.Begin();
                if (Texture != null && Position != null)
                    batch.Draw(Texture, Position, sourceRect, color);
                if (color == Color.Red)
                    color = Color.White;
            }
        }
    }
}