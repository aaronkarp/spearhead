#region File Description
//-----------------------------------------------------------------------------
// Level1Boss.cs
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
    /// This class creates and controls the Level 1 Boss, Combat Frame 01
    /// </summary>

    class Level1Boss : Boss
    {
        #region Fields
        public int XSpeed; // The boss's lateral speed
        public int YSpeed; // The boss's vertical speed
        public string Direction; // The direction the boss is currently traveling
        TimeSpan verticalStop; // The time at which the boss stops moving vertically
        TimeSpan pauseTime; // The time the boss pauses after the vertical move
        Component leftModule; // The left component of the boss, a Dual Gun Pod
        Component centerModule; // The center component of the boss, a Particle Turret with Base
        Component rightModule; // The right component of the boss, a Missile Pod
        Component missilePodFull; // The graphic of full missile tubes within the Missile Pod
        public Component missilePodDoor; // The missile pod door
        TimeSpan fireSequenceStart; // The time of the start of the last firing sequence
        TimeSpan fireSequenceDelay; // The delay between firing sequences
        #endregion

        public Level1Boss(Vector2 startPosition, ContentManager content, int value)
            : base(startPosition, content, value)
        {
            Components = new List<Component>();
            leftModule = new DualGunPod(this, startPosition, new Vector2(0, 0), content, 500, 500, false, true, true);
            centerModule = new ParticleTurretWithBase(this, startPosition, new Vector2(80, 40), content, 750, 500, false, true, true);
            rightModule = new MissilePod(this, startPosition, new Vector2(230, 20), content, 500, 500, false, true, true);
            missilePodFull = new MissilePodFull(this, startPosition, new Vector2(244, 120), content, 100, 0, false, false, false);
            missilePodDoor = new MissilePodDoor(this, startPosition, new Vector2(230, 102), content, 100, 0, false, false, false);
            Components.Add(leftModule);
            Components.Add(centerModule);
            Components.Add(rightModule);
            Components.Add(missilePodFull);
            Components.Add(missilePodDoor);
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
            fireSequenceStart = TimeSpan.Zero;
            fireSequenceDelay = TimeSpan.FromSeconds(5f);
            TotalSize = new Vector2(310, 215); // This only covers the main body area of the boss to prevent explosions appearing in empty space
            ExplosionCount = 12;
            DeathTime = TimeSpan.Zero;
            ExplosionTime = TimeSpan.FromSeconds(.3f);
        }

        protected override void UpdateBoss(GameTime gameTime)
        {
            if (Position.Y <= 150)
            {
                Position.Y += YSpeed;
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
                        fireSequenceStart = gameTime.TotalGameTime;
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
                    if (gameTime.TotalGameTime - fireSequenceDelay >= fireSequenceStart && missilePodDoor.Status == "Closed")
                    {
                        missilePodDoor.Status = "Opening";
                        rightModule.Damageable = true;
                        fireSequenceStart = gameTime.TotalGameTime;
                    }
                }
                if ((missilePodDoor.Status == "Firing" || missilePodDoor.Status == "Closing") && missilePodFull.Status == "Full")
                {
                    missilePodFull.Status = "Empty";
                }
                if ((missilePodDoor.Status == "Opening" || missilePodDoor.Status == "Open" || missilePodDoor.Status == "Closed") && missilePodFull.Status == "Empty")
                {
                    missilePodFull.Status = "Full";
                    if (missilePodDoor.Status == "Closed")
                        rightModule.Damageable = false;
                }

                if (rightModule.Destroyed == true)
                {
                    if (rightModule.Explosions >= 1)
                    {
                        missilePodDoor.Destroyed = true;
                        missilePodFull.Destroyed = true;
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

        protected override void DrawBoss(SpriteBatch batch)
        {
            for (int i = 0; i < Components.Count; i++)
            {
                Components[i].DrawSprite(batch);
            }
        }
    }
}
