#region File Description
//-----------------------------------------------------------------------------
// RightWing.cs
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
    /// This class creates the left wing of the Rail Wing boss
    /// </summary>
    class RightWing : Component
    {
        #region Fields
        TimeSpan fireTime; // The time between shots
        TimeSpan previousFireTime; // The time the last shot was fired
        TimeSpan fireStopTime; // The length of pauses between firing
        TimeSpan previousFireStopTime; // The time the last pause began
        bool firing; // Whether the guns should be firing presently
        string projectileTexture; // The projectile texture
        Vector2 gunOffset; // The launch position of the first projectile as a distance from the component's origin
        float projectileVerticalSpeed; // The speed of the projectile vertically
        float projectileLateralSpeed; // The speed of the first projectile laterally
        int projectileLateralDistance; // The distance the projectile should travel laterally before losing lateral speed
        int gunDamage; // The damage done by the projectile
        int fireCount; // The number of rounds to fire before pausing
        bool projectileUninterrupted; // Whether the projectile is deactivated by collision
        ContentManager Content; // The Content Manager
        Level5Boss l5Boss;
        #endregion

        public RightWing(Boss boss, Vector2 origin, Vector2 offset, ContentManager content, int health, int value, bool mirrored, bool collidable, bool critical)
            : base(boss, origin, offset, "Images/level-5-boss-ship-right-wing-spritesheet", content, health, value, mirrored, collidable, critical)
        {
            Texture = content.Load<Texture2D>("Images/level-5-boss-ship-right-wing-spritesheet");
            FrameSize = new Vector2(Texture.Width / 2, Texture.Height);
            Destroyed = false;
            ExplosionCount = 3;
            fireTime = TimeSpan.FromSeconds(.15f);
            previousFireTime = TimeSpan.Zero;
            fireStopTime = TimeSpan.FromSeconds(3.25f);
            previousFireStopTime = TimeSpan.Zero;
            firing = true;
            projectileTexture = "Images/DualGunRoundSmallB";
            gunOffset = new Vector2(14, 104);
            projectileVerticalSpeed = 25f;
            projectileLateralSpeed = 0;
            projectileLateralDistance = 1000;
            gunDamage = 5;
            fireCount = 8;
            projectileUninterrupted = false;
            Content = content;
            l5Boss = (Level5Boss)boss;
            Damageable = false;
        }

        public override void UpdateComponent(GameTime gameTime, Level5Boss boss, string leftDoorStatus, string rightDoorStatus)
        {
            base.UpdateComponent(gameTime, boss);
            HitArea = new Rectangle((int)Position.X + 9, (int)Position.Y, 10, 104);
            if (boss.IsDeployed && !Destroyed && !boss.Phase1Active)
            {
                Damageable = true;
                if (fireCount == 0)
                {
                    firing = false;
                    previousFireStopTime = gameTime.TotalGameTime;
                    fireCount = 3;
                }

                if (gameTime.TotalGameTime - previousFireStopTime > fireStopTime)
                {
                    firing = true;
                }

                if (firing)
                {
                    if (gameTime.TotalGameTime - previousFireTime > fireTime)
                    {
                        // Set the last weapon fire time to the current time
                        previousFireTime = gameTime.TotalGameTime;

                        AddProjectile(Position, Content);
                        fireCount--;
                    }
                }
            }
        }

        public void AddProjectile(Vector2 position, ContentManager content)
        {
            Projectile projectileA = new Projectile(position + gunOffset, content, projectileVerticalSpeed * -1, projectileLateralSpeed, projectileLateralDistance, gunDamage, projectileTexture, projectileUninterrupted, true, 0, "small");
            Boss.Projectiles.Add(projectileA);
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