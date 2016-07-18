#region File Description
//-----------------------------------------------------------------------------
// AssaultDrone.cs
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
    /// This class creates an assault drone ship that will be orbited by satellites
    /// </summary>
    class AssaultDrone : Component
    {
        #region Fields
        TimeSpan fireTime; // The time between shots
        TimeSpan previousFireTime; // The time the last shot was fired
        TimeSpan fireStopTime; // The length of pauses between firing
        TimeSpan previousFireStopTime; // The time the last pause began
        bool firing; // Whether the guns should be firing presently
        string projectileTextureA; // The projectile texture
        string projectileTextureB; // The alternate projectile texture
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

        public AssaultDrone(Boss boss, Vector2 origin, Vector2 offset, ContentManager content, int health, int value, bool mirrored, bool collidable, bool critical)
            : base(boss, origin, offset, "Images/level-3-boss", content, health, value, mirrored, collidable, critical)
        {
            Texture = content.Load<Texture2D>("Images/level-3-boss");
            FrameSize = new Vector2(Texture.Width, Texture.Height);
            Destroyed = false;
            ExplosionCount = 3;
            fireTime = TimeSpan.FromSeconds(.1f);
            previousFireTime = TimeSpan.Zero;
            fireStopTime = TimeSpan.FromSeconds(1.75f);
            previousFireStopTime = TimeSpan.Zero;
            firing = true;
            projectileTextureA = "Images/DualGunRoundSmallA";
            projectileTextureB = "Images/DualGunRoundSmallB";
            gunOffset = new Vector2(20, 135);
            gunOffset2 = new Vector2(52, 0);
            projectileVerticalSpeed = 10f;
            projectileLateralSpeed = 0;
            projectileLateralDistance = 0;
            gunDamage = 5;
            fireCount = 3;
            projectileUninterrupted = false;
            Content = content;
        }

        public override void UpdateComponent(GameTime gameTime, Boss boss)
        {
            base.UpdateComponent(gameTime, boss);
            HitArea = new Rectangle((int)Position.X, (int)Position.Y, (int)Texture.Width, 100);

            if (boss.IsDeployed && !Destroyed)
            {
                if (fireCount == 0)
                {
                    firing = false;
                    previousFireStopTime = gameTime.TotalGameTime;
                    fireCount = 5;
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
                        AddProjectile(Position + gunOffset, Content);
                        AddProjectile(Position + gunOffset + gunOffset2, Content);
                        fireCount--;

                        //AddProjectile(Position + gunOffset, Content);
                        //fireCount--;
                    }
                }
            }
        }

        public void AddProjectile(Vector2 position, ContentManager content)
        {
            string texture;
            if (fireCount % 2 == 0)
                texture = projectileTextureA;
            else
                texture = projectileTextureB;
            Projectile projectile = new Projectile(position, content, projectileVerticalSpeed * -1, projectileLateralSpeed, projectileLateralDistance, gunDamage, texture, projectileUninterrupted, true, 0, "large");
            Boss.Projectiles.Add(projectile);
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