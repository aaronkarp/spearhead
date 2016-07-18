#region File Description
//-----------------------------------------------------------------------------
// Fuselage.cs
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
    /// This class creates the fuselage of the Pincer Wing
    /// </summary>
    class Fuselage : Component
    {
        #region Fields
        TimeSpan fireTime; // The time between shots
        TimeSpan previousFireTime; // The time the last shot was fired
        TimeSpan fireStopTime; // The length of pauses between firing
        public TimeSpan previousFireStopTime; // The time the last pause began
        public bool Firing; // Whether the guns should be firing presently
        string projectileTexture; // The projectile texture
        Vector2 gunOffset; // The launch position of each projectile as a distance from the component's origin
        Vector2 gunOffset2; // The distance between the two guns
        float projectileVerticalSpeed; // The speed of the projectile vertically
        float projectileLateralSpeed; // The speed of the projectile laterally
        int projectileLateralDistance; // The distance the projectile should travel laterally before losing lateral speed
        int gunDamage; // The damage done by the projectile
        int fireCount; // The number of rounds to fire before pausing
        bool projectileUninterrupted; // Whether the projectile is deactivated by collision
        ContentManager Content; // The Content Manager
        #endregion

        public Fuselage(Boss boss, Vector2 origin, Vector2 offset, ContentManager content, int health, int value, bool mirrored, bool collidable, bool critical)
            : base(boss, origin, offset, "Images/level-2-boss-fuselage-spritesheet", content, health, value, mirrored, collidable, critical)
        {
            Texture = content.Load<Texture2D>("Images/level-2-boss-fuselage-spritesheet");
            FrameSize = new Vector2(Texture.Width / 2, Texture.Height);
            Destroyed = false;
            ExplosionCount = 3;
            fireTime = TimeSpan.FromSeconds(.25f);
            previousFireTime = TimeSpan.Zero;
            fireStopTime = TimeSpan.FromSeconds(1.25f);
            previousFireStopTime = TimeSpan.Zero;
            Firing = true;
            projectileTexture = "Images/laser-round";
            gunOffset = new Vector2(24, 230);
            gunOffset2 = new Vector2(24, 0);
            projectileVerticalSpeed = 20f;
            projectileLateralSpeed = 0;
            projectileLateralDistance = 0;
            gunDamage = 3;
            fireCount = 2;
            projectileUninterrupted = true;
            Content = content;
        }

        public override void UpdateComponent(GameTime gameTime, Boss boss)
        {
            base.UpdateComponent(gameTime, boss);
            HitArea = new Rectangle((int)Position.X, (int)Position.Y, (int)Texture.Width, 155);

            if (boss.IsDeployed && !Destroyed)
            {
                if (!MasterSafety)
                {
                    if (fireCount == 0)
                    {
                        Firing = false;
                        previousFireStopTime = gameTime.TotalGameTime;
                        fireCount = 2;
                    }

                    if (gameTime.TotalGameTime - previousFireStopTime > fireStopTime)
                    {
                        Firing = true;
                    }

                    if (Firing)
                    {
                        if (gameTime.TotalGameTime - previousFireTime > fireTime)
                        {
                            // Set the last weapon fire time to the current time
                            previousFireTime = gameTime.TotalGameTime;
                            AddProjectile(Position + gunOffset, Content);
                            AddProjectile(Position + gunOffset + gunOffset2, Content);
                            fireCount--;
                        }
                    }
                }
            }
        }

        public void AddProjectile(Vector2 position, ContentManager content)
        {
            Projectile projectile = new Projectile(position, content, projectileVerticalSpeed * -1, projectileLateralSpeed, projectileLateralDistance, gunDamage, projectileTexture, projectileUninterrupted, true, 0, "large");
            Boss.Projectiles.Add(projectile);
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