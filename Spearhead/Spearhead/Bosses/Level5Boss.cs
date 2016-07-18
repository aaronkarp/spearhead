#region File Description
//-----------------------------------------------------------------------------
// Level5Boss.cs
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
    /// This class creates and controls the Level 5 Boss, Rail Wing
    /// </summary>

    class Level5Boss : Boss
    {
        #region Fields
        public int XSpeed; // The boss's lateral speed
        public int YSpeed; // The boss's vertical speed
        public string Direction; // The direction the boss is currently traveling
        TimeSpan verticalStop; // The time at which the boss stops moving vertically
        TimeSpan pauseTime; // The time the boss pauses after the vertical move
        TimeSpan Phase1DeathTime; // The moment the phase 1 components are all destroyed
        TimeSpan Phase1DeathDelay; // The amount to time to wait before making phase 1 inactive
        public Wing Wing; // The Wing component
        public LeftRail LeftRail; // The Left Rail component
        public RightRail RightRail; // The Right Rail component
        public LeftTurret LeftTurret; // The Left Turret component
        public RightTurret RightTurret; // The Right Turret component
        public LeftMissiles LeftMissiles; // The Left Missile component
        public RightMissiles RightMissiles; // The Right Missile component
        public LeftMissileDoor LeftMissileDoor; // The Left Missile Door component
        public RightMissileDoor RightMissileDoor; // The Right Missile Door component
        public CenterFuselage CenterFuselage; // The Center Fuselage component
        public LeftWing LeftWing; // The Left Wing component
        public RightWing RightWing; // The Right Wing component
        public LeftThrusterPod LeftThrusterPod; // The Left Thruster Pod component
        public RightThrusterPod RightThrusterPod; // The Right Thruster Pod component
        public bool Phase1Active; // Whether or not the wing portion is currently active
        bool Phase1RunningDown; // Boolean to indicate state between Phase 1's destruction and its disappearance
        TimeSpan phase2Free; // The time that Phase 1 components disappear
        TimeSpan phase2Delay; // The delay before Phase 2 components start fighting
        #endregion

        public Level5Boss(Vector2 startPosition, ContentManager content, int value)
            : base(startPosition, content, value)
        {
            LeftThrusterPod = new LeftThrusterPod(this, startPosition, new Vector2(298, 10), content, 800, 750, false, true, true);
            RightThrusterPod = new RightThrusterPod(this, startPosition, new Vector2(94, 10), content, 800, 750, false, true, true);
            LeftWing = new LeftWing(this, startPosition, new Vector2(262, 68), content, 700, 500, false, true, true);
            RightWing = new RightWing(this, startPosition, new Vector2(154, 68), content, 700, 500, false, true, true);
            CenterFuselage = new CenterFuselage(this, startPosition, new Vector2(194, 0), content, 900, 1000, false, true, true);
            LeftRail = new LeftRail(this, startPosition, new Vector2(438, 156), content, 100, 0, false, false, false);
            RightRail = new RightRail(this, startPosition, new Vector2(18, 156), content, 100, 0, false, false, false);
            Wing = new Wing(this, startPosition, new Vector2(36, 46), content, 100, 0, false, false, false);
            LeftTurret = new LeftTurret(this, startPosition, new Vector2(285, 98), content, 750, 500, false, true, true);
            RightTurret = new RightTurret(this, startPosition, new Vector2(110, 98), content, 750, 500, false, true, true);
            LeftMissiles = new LeftMissiles(this, startPosition, new Vector2(348, 172), content, 500, 500, false, true, true);
            RightMissiles = new RightMissiles(this, startPosition, new Vector2(44, 172), content, 500, 500, false, true, true);
            LeftMissileDoor = new LeftMissileDoor(this, startPosition, new Vector2(348, 172), content, 100, 0, false, false, false);
            RightMissileDoor = new RightMissileDoor(this, startPosition, new Vector2(44, 172), content, 100, 0, false, false, false);
            Components = new List<Component>();
            Components.Add(LeftThrusterPod);
            Components.Add(RightThrusterPod);
            Components.Add(LeftWing);
            Components.Add(RightWing);
            Components.Add(CenterFuselage);
            Components.Add(LeftRail);
            Components.Add(RightRail);
            Components.Add(Wing);
            Components.Add(LeftTurret);
            Components.Add(RightTurret);
            Components.Add(LeftMissiles);
            Components.Add(RightMissiles);
            Components.Add(LeftMissileDoor);
            Components.Add(RightMissileDoor);

            for (int i = 0; i < Components.Count; i++)
            {
                if (Components[i].Critical)
                {
                    Criticals.Add(Components[i]);
                }
            }
            IsActive = true;
            IsDeployed = false;
            XSpeed = 0;
            YSpeed = 5;
            Direction = "Left";
            verticalStop = TimeSpan.Zero;
            pauseTime = TimeSpan.FromSeconds(1.75f);
            TotalSize = new Vector2(240, 153); // This only covers the main body area of the boss to prevent explosions appearing in empty space
            ExplosionCount = 20;
            DeathTime = TimeSpan.Zero;
            ExplosionTime = TimeSpan.FromSeconds(.2f);
            Content = content;
            Phase1Active = true;
            Phase1RunningDown = false;
            Phase1DeathTime = TimeSpan.Zero;
            Phase1DeathDelay = TimeSpan.FromSeconds(2f);
            phase2Free = TimeSpan.Zero;
            phase2Delay = TimeSpan.FromSeconds(1.3f);
        }

        protected override void UpdateBoss(GameTime gameTime)
        {
            if (Position.Y <= 150)
            {
                Position.Y += YSpeed;
                LeftMissileDoor.fireSequenceStart = gameTime.TotalGameTime;
                RightMissileDoor.fireSequenceStart = gameTime.TotalGameTime;
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
                        RightMissileDoor.fireSequenceStart = gameTime.TotalGameTime;
                        LeftMissileDoor.fireSequenceStart = gameTime.TotalGameTime;
                    }
                    if (!Phase1Active)
                    {
                        if (Position.X <= -102 && Direction == "Left")
                        {
                            XSpeed = 5;
                            Direction = "Right";
                        }
                        if (Position.X >= 110 && Direction == "Right")
                        {
                            XSpeed = -5;
                            Direction = "Left";
                        }
                    }
                    Position.X += XSpeed;
                }

                if (LeftMissiles.Destroyed && LeftTurret.Destroyed && RightTurret.Destroyed && RightMissiles.Destroyed && Phase1Active & !Phase1RunningDown)
                {
                    Phase1RunningDown = true;
                    Phase1DeathTime = gameTime.TotalGameTime;
                }

                if (Phase1RunningDown && gameTime.TotalGameTime >= Phase1DeathTime + Phase1DeathDelay && Phase1Active)
                {
                    if (phase2Free == TimeSpan.Zero)
                        phase2Free = gameTime.TotalGameTime;
                    if (gameTime.TotalGameTime >= phase2Free + phase2Delay && Phase1Active)
                    {
                        Phase1Active = false;
                        XSpeed = -5;
                        Direction = "Left";
                    }
                }
            }
            if (!Defeated)
            {
                for (int i = 0; i < Components.Count; i++)
                {
                    Components[i].UpdateComponent(gameTime, this, LeftMissileDoor.Status, RightMissileDoor.Status);
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
