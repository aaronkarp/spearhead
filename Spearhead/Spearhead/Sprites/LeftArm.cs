#region File Description
//-----------------------------------------------------------------------------
// LeftArm.cs
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
    /// This class creates the left arm of the Pincer Wing boss
    /// </summary>
    class LeftArm : Component
    {
        #region Fields
        TimeSpan fireTime; // The time between  shots
        TimeSpan previousFireTime; // The time the last shot was fired
        TimeSpan fireStopTime; // The length of pauses between firing
        public TimeSpan previousFireStopTime; // The time the last pause began
        public bool Firing; // Whether the gun should be firing presently
        string projectileTexture; // The projectile texture
        Vector2 gunOffset; // The launch position of each projectile as a distance from the component's origin
        float projectileAVerticalSpeed; // The speed of the first projectile vertically
        float projectileALateralSpeed; // The speed of the first projectile laterally
        float projectileBVerticalSpeed; // The speed of the second projectile vertically
        float projectileBLateralSpeed; // The speed of the second projectile laterally
        float projectileCVerticalSpeed; // The speed of the third projectile vertically
        float projectileCLateralSpeed; // The speed of the third projectile laterally
        int projectileLateralDistance; // The distance the projectile should travel laterally before losing lateral speed
        int gunDamage; // The damage done by the projectile
        int fireCount; // The number of rounds to fire before pausing
        bool projectileUninterrupted; // Whether the projectile is deactivated by collision
        ContentManager Content; // The Content Manager
        #endregion

        public LeftArm(Boss boss, Vector2 origin, Vector2 offset, ContentManager content, int health, int value, bool mirrored, bool collidable, bool critical)
            : base(boss, origin, offset, "Images/level-2-boss-left-arm-spritesheet", content, health, value, mirrored, collidable, critical)
        {
            FrameSize = new Vector2(84, 272);
            Texture = content.Load<Texture2D>("Images/level-2-boss-left-arm-spritesheet");
            Destroyed = false;
            HitArea = new Rectangle((int)Position.X, (int)Position.Y, (int)(Texture.Width / 2), Texture.Height);
            ExplosionCount = 3;
            fireTime = TimeSpan.FromSeconds(.25f);
            previousFireTime = TimeSpan.Zero;
            fireStopTime = TimeSpan.FromSeconds(1.5f);
            previousFireStopTime = TimeSpan.Zero;
            Firing = true;
            projectileTexture = "Images/level-3-boss-satellite-projectile";
            gunOffset = new Vector2(52, 236);
            projectileAVerticalSpeed = 10f;
            projectileALateralSpeed = 0;
            projectileBVerticalSpeed = 15f;
            projectileBLateralSpeed = -5f;
            projectileCVerticalSpeed = 15f;
            projectileCLateralSpeed = -10f;
            projectileLateralDistance = 480;
            gunDamage = 5;
            fireCount = 3;
            projectileUninterrupted = false;
            Content = content;
        }

        public override void UpdateComponent(GameTime gameTime, Boss boss)
        {
            base.UpdateComponent(gameTime, boss);
            HitArea = new Rectangle((int)Position.X, (int)Position.Y, (int)(Texture.Width / 2), Texture.Height);

            if (Boss.IsDeployed && !Destroyed)
            {
                if (!MasterSafety)
                {
                    if (fireCount == 0)
                    {
                        Firing = false;
                        previousFireStopTime = gameTime.TotalGameTime;
                        fireCount = 3;
                    }

                    if (gameTime.TotalGameTime - previousFireStopTime > fireStopTime)
                    {
                        Firing = true;
                    }

                    if (Firing)
                    {
                        if (gameTime.TotalGameTime - previousFireTime > fireTime)
                        {
                            previousFireTime = gameTime.TotalGameTime;
                            AddProjectile(Position + gunOffset, Content);
                            fireCount--;
                        }
                    }
                }
            }

            if (Destroyed)
            {
                MasterSafety = false;
            }
        }

        public void AddProjectile(Vector2 position, ContentManager content)
        {
            Projectile projectileA = new Projectile(position, content, projectileAVerticalSpeed * -1, projectileALateralSpeed, projectileLateralDistance, gunDamage, projectileTexture, projectileUninterrupted, true, 0, "small");
            Projectile projectileB = new Projectile(position, content, projectileBVerticalSpeed * -1, projectileBLateralSpeed, projectileLateralDistance, gunDamage, projectileTexture, projectileUninterrupted, true, 0, "small");
            Projectile projectileC = new Projectile(position, content, projectileCVerticalSpeed * -1, projectileCLateralSpeed, projectileLateralDistance, gunDamage, projectileTexture, projectileUninterrupted, true, 0, "small");
            Boss.Projectiles.Add(projectileA);
            Boss.Projectiles.Add(projectileB);
            Boss.Projectiles.Add(projectileC);
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