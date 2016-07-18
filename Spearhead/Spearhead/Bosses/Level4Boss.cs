#region File Description
//-----------------------------------------------------------------------------
// Level4Boss.cs
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
    /// This class creates and controls the Level 4 Boss, Death's Head
    /// </summary>

    class Level4Boss : Boss
    {
        #region Fields
        public int XSpeed; // The boss's lateral speed
        public int YSpeed; // The boss's vertical speed
        public string Direction; // The direction the boss is currently traveling
        TimeSpan verticalStop; // The time at which the boss stops moving vertically
        TimeSpan pauseTime; // The time the boss pauses after the vertical move
        Component skull; // The main body of the boss
        Component leftEye; // The boss's left eye
        Component rightEye; // The boss's right eye
        Component blades; // The decorative blades near the skull's jaw
        public Jaw Jaw; // The boss's jaw
        TimeSpan eyeFireTime; // The time between eye shots
        TimeSpan previousEyeFireTime; // The time the last eye shot was fired
        TimeSpan eyeFireStopTime; // The length of pauses between firing the eye shots
        TimeSpan previousEyeFireStopTime; // The time the last eye pause began
        bool eyeFiring; // Whether the eyes should be firing presently
        string eyeProjectileTexture; // The projectile texture for the eyes
        Vector2 leftEyeOffset; // The launch position of shots from the left eye as a distance from the component's origin
        Vector2 rightEyeOffset; // The launch position of shots from the right eye as a distance from the component's origin
        float eyeProjectileVerticalSpeed; // The speed of the first left eye projectile vertically
        float eyeProjectileALateralSpeed; // The speed of the first left projectile laterally
        float eyeProjectileBLateralSpeed; // The speed of the second left projectile laterally
        float eyeProjectileCLateralSpeed; // The speed of the third left projectile laterally
        int eyeProjectileLateralDistance; // The distance the railgun (right) projectile should travel laterally before losing lateral speed
        int eyeDamage; // The damage done by the railgun (right) projectile
        int eyeFireCount; // The number of railgun (right) rounds to fire before pausing
        bool eyeProjectileUninterrupted; // Whether the railgun (right) projectile is deactivated by collision
        int eyeProjectileDecay; // The rate of decay in the lateral movement of projectiles
        #endregion

        public Level4Boss(Vector2 startPosition, ContentManager content, int value)
            : base(startPosition, content, value)
        {
            blades = new Blades(this, Position, new Vector2(4, 226), Content, 100, 0, false, false, false);
            Jaw = new Jaw(this, Position, new Vector2(68, 174), Content, 750, 2000, false, true, true);
            skull = new Skull(this, Position, new Vector2(0, 0), Content, 100, 0, false, false, false);
            leftEye = new LeftEye(this, Position, new Vector2(144, 186), Content, 750, 2000, false, true, true);
            rightEye = new RightEye(this, Position, new Vector2(68, 186), Content, 750, 2000, false, true, true);
            Components = new List<Component>();
            Components.Add(blades);
            Components.Add(Jaw);
            Components.Add(skull);
            Components.Add(leftEye);
            Components.Add(rightEye);

            for (int i = 0; i < Components.Count; i++)
            {
                if (Components[i].Critical)
                {
                    Criticals.Add(Components[i]);
                }
            }
            IsActive = true;
            IsDeployed = false;
            XSpeed = -3;
            YSpeed = 5;
            Direction = "Left";
            verticalStop = TimeSpan.Zero;
            pauseTime = TimeSpan.FromSeconds(1.75f);
            TotalSize = new Vector2(310, 215); // This only covers the main body area of the boss to prevent explosions appearing in empty space
            ExplosionCount = 12;
            DeathTime = TimeSpan.Zero;
            ExplosionTime = TimeSpan.FromSeconds(.3f);
            eyeFireTime = TimeSpan.FromSeconds(.55f);
            previousEyeFireTime = TimeSpan.Zero;
            eyeFireStopTime = TimeSpan.FromSeconds(3f);
            eyeFiring = true;
            eyeProjectileTexture = "Images/level-3-boss-satellite-projectile";
            leftEyeOffset = new Vector2(174, 220);
            rightEyeOffset = new Vector2(102, 220);
            eyeProjectileVerticalSpeed = 10f;
            eyeProjectileALateralSpeed = 0;
            eyeProjectileBLateralSpeed = -8f;
            eyeProjectileCLateralSpeed = 8f;
            eyeProjectileLateralDistance = 1000;
            eyeDamage = 6;
            eyeProjectileDecay = 0;
            eyeFireCount = 2;
            eyeProjectileUninterrupted = false;
            Content = content;
        }

        protected override void UpdateBoss(GameTime gameTime)
        {
            if (Position.Y <= 150)
            {
                Position.Y += YSpeed;
                Jaw.PreviousFireSequence = gameTime.TotalGameTime;
                previousEyeFireTime = gameTime.TotalGameTime;
            }
            else
            {
                if (YSpeed != 0)
                {
                    YSpeed = 0;
                    verticalStop = gameTime.TotalGameTime;
                }

                if (gameTime.TotalGameTime - pauseTime > verticalStop)
                {
                    if (!IsDeployed)
                    {
                        IsDeployed = true;
                        Jaw.PreviousFireSequence = gameTime.TotalGameTime;
                        previousEyeFireTime = gameTime.TotalGameTime;
                    }
                    if (Position.X <= 0 && Direction == "Left")
                    {
                        XSpeed = 3;
                        Direction = "Right";
                    }
                    if (Position.X >= 170 && Direction == "Right")
                    {
                        XSpeed = -3;
                        Direction = "Left";
                    }
                    Position.X += XSpeed;
                }

                if (IsDeployed)
                {
                    if (Jaw != null && Jaw.Status != "Firing")
                    {
                        if (eyeFireCount == 0)
                        {
                            eyeFiring = false;
                            previousEyeFireStopTime = gameTime.TotalGameTime;
                            eyeFireCount = 3;
                        }

                        if (gameTime.TotalGameTime - previousEyeFireStopTime > eyeFireStopTime)
                        {
                            eyeFiring = true;
                        }

                        if (eyeFiring)
                        {
                            if (gameTime.TotalGameTime - previousEyeFireTime > eyeFireTime)
                            {
                                previousEyeFireTime = gameTime.TotalGameTime;
                                AddProjectile(Position, Content);
                                eyeFireCount--;
                            }
                        }
                    }
                }
            }
            if (!Defeated)
            {
                for (int i = 0; i < Components.Count; i++)
                {
                    Components[i].UpdateComponent(gameTime, this);
                }
            }
        }

        public void AddProjectile(Vector2 position, ContentManager content)
        {
            if (!leftEye.Destroyed)
            {
                Projectile projectileA = new Projectile(position + leftEyeOffset, content, eyeProjectileVerticalSpeed * -1, eyeProjectileALateralSpeed, eyeProjectileLateralDistance, eyeDamage, eyeProjectileTexture, eyeProjectileUninterrupted, true, eyeProjectileDecay, "small");
                Projectile projectileB = new Projectile(position + leftEyeOffset, content, eyeProjectileVerticalSpeed * -1, eyeProjectileBLateralSpeed, eyeProjectileLateralDistance, eyeDamage, eyeProjectileTexture, eyeProjectileUninterrupted, true, eyeProjectileDecay, "small");
                Projectile projectileC = new Projectile(position + leftEyeOffset, content, eyeProjectileVerticalSpeed * -1, eyeProjectileCLateralSpeed, eyeProjectileLateralDistance, eyeDamage, eyeProjectileTexture, eyeProjectileUninterrupted, true, eyeProjectileDecay, "small");
                Projectiles.Add(projectileA);
                Projectiles.Add(projectileB);
                Projectiles.Add(projectileC);
            }

            if (!rightEye.Destroyed)
            {
                Projectile projectileD = new Projectile(position + rightEyeOffset, content, eyeProjectileVerticalSpeed * -1, eyeProjectileALateralSpeed, eyeProjectileLateralDistance, eyeDamage, eyeProjectileTexture, eyeProjectileUninterrupted, true, eyeProjectileDecay, "small");
                Projectile projectileE = new Projectile(position + rightEyeOffset, content, eyeProjectileVerticalSpeed * -1, eyeProjectileBLateralSpeed, eyeProjectileLateralDistance, eyeDamage, eyeProjectileTexture, eyeProjectileUninterrupted, true, eyeProjectileDecay, "small");
                Projectile projectileF = new Projectile(position + rightEyeOffset, content, eyeProjectileVerticalSpeed * -1, eyeProjectileCLateralSpeed, eyeProjectileLateralDistance, eyeDamage, eyeProjectileTexture, eyeProjectileUninterrupted, true, eyeProjectileDecay, "small");
                Projectiles.Add(projectileD);
                Projectiles.Add(projectileE);
                Projectiles.Add(projectileF);
            }
        }

        protected override void DrawBoss(SpriteBatch batch)
        {
            for (int i = 0; i < Components.Count; i++)
            {
                Components[i].DrawSprite(batch);
            }
        }
    }
}
