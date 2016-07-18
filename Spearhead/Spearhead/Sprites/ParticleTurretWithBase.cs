#region File Description
//-----------------------------------------------------------------------------
// ParticleTurretWithBase.cs
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
    /// This class creates an SDF-1-inspired particle turret with a base for other components
    /// </summary>
    class ParticleTurretWithBase : Component
    {
        #region Fields
        string fireDirection; // Which of the two guns is currently firing, "right" or "left"
        TimeSpan fireTime; // The time between shots
        TimeSpan previousFireTime; // The time the last shot was fired
        TimeSpan fireStopTime; // The length of pauses between firing
        TimeSpan previousFireStopTime; // The time the last pause began
        bool firing; // Whether the guns should be firing presently
        string projectileTexture; // The projectile texture
        Vector2 gunOffset; // The launch position of each projectile as a distance from the component's origin
        float projectileVerticalSpeed; // The speed of the projectile vertically
        float projectileLateralSpeed; // The speed of the projectile laterally
        int projectileLateralDistance; // The distance the projectile should travel laterally before losing lateral speed
        int gunDamage; // The damage done by the projectile
        int fireCount; // The number of rounds to fire before pausing
        bool projectileUninterrupted; // Whether the projectile is deactivated by collision
        ContentManager Content; // The Content Manager
        #endregion

        public ParticleTurretWithBase(Boss boss, Vector2 origin, Vector2 offset, ContentManager content, int health, int value, bool mirrored, bool collidable, bool critical)
            : base(boss, origin, offset, "Images/ParticleTurretWithBase", content, health, value, mirrored, collidable, critical)
        {
            FrameSize = new Vector2(150, 174);
            Texture = content.Load<Texture2D>("Images/ParticleTurretWithBase");
            Destroyed = false;
            ExplosionCount = 3;
            fireDirection = "right";
            fireTime = TimeSpan.FromSeconds(.25f);
            previousFireTime = TimeSpan.Zero;
            fireStopTime = TimeSpan.FromSeconds(3.25f);
            previousFireStopTime = TimeSpan.Zero;
            firing = true;
            projectileTexture = "Images/ParticleRound";
            gunOffset = new Vector2(80, 140);
            projectileVerticalSpeed = 10f;
            projectileLateralSpeed = 0;
            projectileLateralDistance = 0;
            gunDamage = 30;
            fireCount = 6;
            projectileUninterrupted = false;
            Content = content;
        }

        public override void UpdateComponent(GameTime gameTime, Boss boss)
        {
            base.UpdateComponent(gameTime, boss);
            HitArea = new Rectangle((int)Position.X, (int)Position.Y, (int)(Texture.Width / 2), 100);
            if (boss.IsDeployed && !Destroyed)
            {
                if (fireCount == 0)
                {
                    firing = false;
                    previousFireStopTime = gameTime.TotalGameTime;
                    fireCount = 6;
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

                        if (fireDirection == "Left")
                        {
                            fireDirection = "Right";
                            gunOffset.X = 80;
                        }
                        else
                        {
                            fireDirection = "Left";
                            gunOffset.X = 44;
                        }

                        AddProjectile(Position + gunOffset, Content);
                        fireCount--;
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