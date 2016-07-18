#region File Description
//-----------------------------------------------------------------------------
// DualGunPod.cs
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
    /// This class creates the dual gun pod boss component
    /// </summary>
    class DualGunPod : Component
    {
        #region Fields
        TimeSpan railGunFireTime; // The time between railgun (right) shots
        TimeSpan previousRailGunFireTime; // The time the last shot was fired from the railgun (right)
        TimeSpan railGunFireStopTime; // The length of pauses between firing the railgun (right)
        TimeSpan previousRailGunFireStopTime; // The time the last railgun (right) pause began
        bool railGunFiring; // Whether the railgun (right) should be firing presently
        string railGunProjectileTextureA; // The projectile texture for the railgun (right)
        string railGunProjectileTextureB; // The alternate projectile texture for the railgun (right)
        Vector2 railGunOffset; // The launch position of each railgun (right) projectile as a distance from the component's origin
        float railGunProjectileVerticalSpeed; // The speed of the railgun (right) projectile vertically
        float railGunProjectileLateralSpeed; // The speed of the railgun (right) projectile laterally
        int railGunProjectileLateralDistance; // The distance the railgun (right) projectile should travel laterally before losing lateral speed
        int railGunDamage; // The damage done by the railgun (right) projectile
        int railGunFireCount; // The number of railgun (right) rounds to fire before pausing
        bool railGunProjectileUninterrupted; // Whether the railgun (right) projectile is deactivated by collision
        TimeSpan cannonFireTime; // The time between cannon (left) shots
        TimeSpan previousCannonFireTime; // The time the last shot was fired from the cannon (left)
        TimeSpan cannonFireStopTime; // The length of pauses between firing the cannon (left)
        TimeSpan previousCannonFireStopTime; // The time the last cannon (left) pause began
        bool cannonFiring; // Whether the cannon (left) should be firing presently
        string cannonProjectileTexture; // The projectile texture for the cannon (left)
        Vector2 cannonOffset; // The launch position of each cannon (left) projectile as a distance from the component's origin
        float cannonProjectileVerticalSpeed; // The speed of the cannon (left) projectile vertically
        float cannonProjectileLateralSpeed; // The speed of the cannon (left) projectile laterally
        int cannonProjectileLateralDistance; // The distance the cannon (left) projectile should travel laterally before losing lateral speed
        int cannonDamage; // The damage done by the cannon (left) projectile
        int cannonFireCount; // The number of cannon (left) rounds to fire before pausing
        bool cannonProjectileUninterrupted; // Whether the cannon (left) projectile is deactivated by collision
        ContentManager Content; // The Content Manager
        #endregion

        public DualGunPod(Boss boss, Vector2 origin, Vector2 offset, ContentManager content, int health, int value, bool mirrored, bool collidable, bool critical)
            : base(boss, origin, offset, "Images/DualGunPod", content, health, value, mirrored, collidable, critical)
        {
            FrameSize = new Vector2(80, 300);
            Texture = content.Load<Texture2D>("Images/DualGunPod");
            Destroyed = false;
            HitArea = new Rectangle((int)Position.X, (int)Position.Y, (int)(Texture.Width / 2), Texture.Height);
            ExplosionCount = 4;
            railGunFireTime = TimeSpan.FromSeconds(.1f);
            previousRailGunFireTime = TimeSpan.Zero;
            railGunFireStopTime = TimeSpan.FromSeconds(1.5f);
            previousRailGunFireStopTime = TimeSpan.Zero;
            railGunFiring = true;
            railGunProjectileTextureA = "Images/DualGunRoundSmallA";
            railGunProjectileTextureB = "Images/DualGunRoundSmallB";
            railGunOffset = new Vector2(64, 276);
            railGunProjectileVerticalSpeed = 25f;
            railGunProjectileLateralSpeed = 0;
            railGunProjectileLateralDistance = 0;
            railGunDamage = 5;
            railGunFireCount = 20;
            railGunProjectileUninterrupted = false;
            cannonFireTime = TimeSpan.FromSeconds(.5f);
            previousCannonFireTime = TimeSpan.Zero;
            cannonFireStopTime = TimeSpan.FromSeconds(4.0f);
            previousCannonFireStopTime = TimeSpan.Zero;
            cannonFiring = true;
            cannonProjectileTexture = "Images/DualGunRoundLarge";
            cannonOffset = new Vector2(25, 247);
            cannonProjectileVerticalSpeed = 15f;
            cannonProjectileLateralSpeed = 0;
            cannonProjectileLateralDistance = 0;
            cannonDamage = 40;
            cannonFireCount = 3;
            cannonProjectileUninterrupted = false;
            Content = content;
        }

        public override void UpdateComponent(GameTime gameTime, Boss boss)
        {
            base.UpdateComponent(gameTime, boss);
            HitArea = new Rectangle((int)Position.X, (int)Position.Y, (int)(Texture.Width / 2), Texture.Height);

            if (Boss.IsDeployed && !Destroyed)
            {
                if (railGunFireCount == 0)
                {
                    railGunFiring = false;
                    previousRailGunFireStopTime = gameTime.TotalGameTime;
                    railGunFireCount = 20;
                }

                if (gameTime.TotalGameTime - previousRailGunFireStopTime > railGunFireStopTime)
                {
                    railGunFiring = true;
                }

                if (cannonFireCount == 0)
                {
                    cannonFiring = false;
                    previousCannonFireStopTime = gameTime.TotalGameTime;
                    cannonFireCount = 3;
                }

                if (gameTime.TotalGameTime - previousCannonFireStopTime > cannonFireStopTime)
                {
                    cannonFiring = true;
                }

                if (railGunFiring)
                {
                    if (gameTime.TotalGameTime - previousRailGunFireTime > railGunFireTime)
                    {
                        previousRailGunFireTime = gameTime.TotalGameTime;
                        AddRailGunProjectile(Position + railGunOffset, Content);
                        railGunFireCount--;
                    }
                }

                if (cannonFiring)
                {
                    if (gameTime.TotalGameTime - previousCannonFireTime > cannonFireTime)
                    {
                        previousCannonFireTime = gameTime.TotalGameTime;
                        AddProjectile(Position + cannonOffset, Content);
                        cannonFireCount--;
                    }
                }
            }
        }

        public void AddProjectile(Vector2 position, ContentManager content)
        {
            Projectile projectile = new Projectile(position, content, cannonProjectileVerticalSpeed * -1, cannonProjectileLateralSpeed, cannonProjectileLateralDistance, cannonDamage, cannonProjectileTexture, cannonProjectileUninterrupted, true, 0, "large");
            Boss.Projectiles.Add(projectile);
        }

        public void AddRailGunProjectile(Vector2 position, ContentManager content)
        {
            string texture;
            if (railGunFireCount % 2 == 0)
                texture = railGunProjectileTextureA;
            else
                texture = railGunProjectileTextureB;
            Projectile projectile = new Projectile(position, content, railGunProjectileVerticalSpeed * -1, railGunProjectileLateralSpeed, railGunProjectileLateralDistance, railGunDamage, texture, railGunProjectileUninterrupted, true, 0, "small");
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