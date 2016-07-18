#region File Description
//-----------------------------------------------------------------------------
// LeftTurret.cs
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
    /// This class creates the left turret of the Rail Wing boss
    /// </summary>
    class LeftTurret : Component
    {
        #region Fields
        TimeSpan fireTime; // The time between shots
        TimeSpan previousFireTime; // The time the last shot was fired
        TimeSpan fireStopTime; // The length of pauses between firing
        TimeSpan previousFireStopTime; // The time the last pause began
        bool firing; // Whether the guns should be firing presently
        string projectileTexture; // The projectile texture
        Vector2 gunOffsetA; // The launch position of the first projectile as a distance from the component's origin
        Vector2 gunOffsetB; // The launch position of the second projectile as a distance from the component's origin
        Vector2 gunOffsetC; // The launch position of the third projectile as a distance from the component's origin
        float projectileVerticalSpeed; // The speed of the projectile vertically
        float projectileALateralSpeed; // The speed of the first projectile laterally
        float projectileBLateralSpeed; // The speed of the second projectile laterally
        float projectileCLateralSpeed; // The speed of the third projectile laterally
        int projectileLateralDistance; // The distance the projectile should travel laterally before losing lateral speed
        int gunDamage; // The damage done by the projectile
        int fireCount; // The number of rounds to fire before pausing
        bool projectileUninterrupted; // Whether the projectile is deactivated by collision
        ContentManager Content; // The Content Manager
        Level5Boss l5boss;
        #endregion

        public LeftTurret(Boss boss, Vector2 origin, Vector2 offset, ContentManager content, int health, int value, bool mirrored, bool collidable, bool critical)
            : base(boss, origin, offset, "Images/level-5-boss-wing-left-turret-spritesheet", content, health, value, mirrored, collidable, critical)
        {
            Texture = content.Load<Texture2D>("Images/level-5-boss-wing-left-turret-spritesheet");
            FrameSize = new Vector2(Texture.Width / 2, Texture.Height);
            Destroyed = false;
            ExplosionCount = 3;
            fireTime = TimeSpan.FromSeconds(.25f);
            previousFireTime = TimeSpan.Zero;
            fireStopTime = TimeSpan.FromSeconds(3.25f);
            previousFireStopTime = TimeSpan.Zero;
            firing = true;
            projectileTexture = "Images/level-3-boss-satellite-projectile";
            gunOffsetA = new Vector2(17, 43);
            gunOffsetB = new Vector2(28, 45);
            gunOffsetC = new Vector2(38, 43);
            projectileVerticalSpeed = 5f;
            projectileALateralSpeed = -2f;
            projectileBLateralSpeed = 0;
            projectileCLateralSpeed = 2f;
            projectileLateralDistance = 1000;
            gunDamage = 5;
            fireCount = 3;
            projectileUninterrupted = false;
            Content = content;
            l5boss = (Level5Boss)boss;
        }

        public override void UpdateComponent(GameTime gameTime, Level5Boss boss, string leftDoorStatus, string rightDoorStatus)
        {
            base.UpdateComponent(gameTime, boss);
            HitArea = new Rectangle((int)Position.X + 5, (int)Position.Y + 8, 50, 50);
            if (boss.IsDeployed && !Destroyed)
            {
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
            Projectile projectileA = new Projectile(position + gunOffsetA, content, projectileVerticalSpeed * -1, projectileALateralSpeed, projectileLateralDistance, gunDamage, projectileTexture, projectileUninterrupted, true, 0, "large");
            Projectile projectileB = new Projectile(position + gunOffsetB, content, projectileVerticalSpeed * -1, projectileBLateralSpeed, projectileLateralDistance, gunDamage, projectileTexture, projectileUninterrupted, true, 0, "large");
            Projectile projectileC = new Projectile(position + gunOffsetC, content, projectileVerticalSpeed * -1, projectileCLateralSpeed, projectileLateralDistance, gunDamage, projectileTexture, projectileUninterrupted, true, 0, "large");
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