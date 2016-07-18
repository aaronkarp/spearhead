#region File Description
//-----------------------------------------------------------------------------
// RightRail.cs
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
    /// This class creates the left rail boss component
    /// </summary>
    class RightRail : Component
    {
        #region Fields
        ContentManager Content; // The Content Manager
        Animation RightRailAnimation; // The animation of the wheels rolling
        Level5Boss l5boss;
        #endregion

        public RightRail(Boss boss, Vector2 origin, Vector2 offset, ContentManager content, int health, int value, bool mirrored, bool collidable, bool critical)
            : base(boss, origin, offset, "Images/level-5-boss-wing-right-rail-unit-spritesheet", content, health, value, mirrored, collidable, critical)
        {
            Texture = content.Load<Texture2D>("Images/level-5-boss-wing-right-rail-unit-spritesheet");
            FrameSize = new Vector2(Texture.Width / 3, Texture.Height);
            Damageable = false;
            Destroyed = false;
            HitArea = new Rectangle((int)Position.X, (int)Position.Y, (int)(Texture.Width / 2), Texture.Height);
            ExplosionCount = 4;
            Content = content;
            l5boss = (Level5Boss)boss;
            RightRailAnimation = new Animation(Texture, boss.Position + offset, (int)FrameSize.X, (int)FrameSize.Y, 2, 70, Color.White, 1f, true);
        }

        public override void UpdateComponent(GameTime gameTime, Level5Boss boss, string leftDoorStatus, string rightDoorStatus)
        {
            base.UpdateComponent(gameTime, boss, leftDoorStatus, rightDoorStatus);
            HitArea = new Rectangle((int)Position.X, (int)Position.Y, (int)Texture.Width / 3, (int)Texture.Height);
            RightRailAnimation.animPosition = boss.Position + Offset;
            RightRailAnimation.Update(gameTime);
        }

        public override void DrawSprite(SpriteBatch batch)
        {
            if (l5boss != null && l5boss.Phase1Active)
            {
                batch.End();
                batch.Begin();
                if (RightRailAnimation != null && Position != null)
                    RightRailAnimation.Draw(batch);
            }
        }
    }
}