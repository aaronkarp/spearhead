#region File Description
//-----------------------------------------------------------------------------
// Level2Boss.cs
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
    /// This class creates and controls the Level 2 Boss, Pincer Wing
    /// </summary>

    class Level2Boss : Boss
    {
        #region Fields
        public int XSpeed; // The boss's lateral speed
        public int PreviousXSpeed; // A copy of the lateral speed to be reapplied after the "rushing" behavior
        public int YSpeed; // The boss's vertical speed
        public string Direction; // The direction the boss is currently traveling
        TimeSpan verticalStop; // The time at which the boss stops moving vertically
        TimeSpan pauseTime; // The time the boss pauses after the vertical move
        public Fuselage Fuselage; // The main body of the ship
        public LeftArm LeftArm; // One of the orbiting satellites
        public RightArm RightArm; // One of the orbiting satellites
        public ElectricalField electricalField; // The electrical field attack
        TimeSpan electricalFieldStartTime; // The time the electrical field attack begins
        TimeSpan electricalFieldDuration; // The time the electrical field remains active
        TimeSpan previousElectricalField; // The time the last electrical field attack ended
        TimeSpan electricalFieldAttackDelay; // The time between uses of the electrical field attack
        TimeSpan rushDelay; // The time between the start of the electrical field attack and the beginning of the "rushing" behavior
        int electricalFieldRushSpeed; // The speed of the "rushing" behavior of the electrical field attack
        bool rushing; // Whether the ship is currently engaged in the "rushing" behavior
        #endregion

        public Level2Boss(Vector2 startPosition, ContentManager content, int value)
            : base(startPosition, content, value)
        {
            Fuselage = new Fuselage(this, startPosition, new Vector2(84, 0), content, 1000, 750, false, true, true);
            RightArm = new RightArm(this, startPosition, new Vector2(0, 20), content, 500, 750, false, true, true);
            LeftArm = new LeftArm(this, startPosition, new Vector2(162, 20), content, 500, 750, false, true, true);
            Components = new List<Component>();
            Components.Add(Fuselage);
            Components.Add(RightArm);
            Components.Add(LeftArm);
            electricalFieldStartTime = TimeSpan.Zero;
            previousElectricalField = TimeSpan.Zero;
            electricalFieldAttackDelay = TimeSpan.FromSeconds(7f);
            electricalFieldDuration = TimeSpan.FromSeconds(5f);
            rushDelay = TimeSpan.FromSeconds(.5f);
            electricalFieldRushSpeed = 15;
            rushing = false;

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
            TotalSize = new Vector2(132, 115); // This only covers the main body area of the boss to prevent explosions appearing in empty space
            ExplosionCount = 6;
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
                        previousElectricalField = gameTime.TotalGameTime;
                    }
                    if (Position.X <= 1 && Direction == "Left")
                    {
                        XSpeed = 6;
                        PreviousXSpeed = 6;
                        Direction = "Right";
                    }
                    if (Position.X >= 233 && Direction == "Right")
                    {
                        XSpeed = -6;
                        PreviousXSpeed = -6;
                        Direction = "Left";
                    }
                    Position.X += XSpeed;
                }

                if (IsDeployed)
                {
                    if (!RightArm.Destroyed || !LeftArm.Destroyed)
                    {
                        if (gameTime.TotalGameTime >= previousElectricalField + electricalFieldAttackDelay)
                            if (electricalField == null)
                            {
                                electricalField = new ElectricalField(this, new Vector2(121, 256), Content, "Images/level-2-boss-electrical-field-spritesheet", "small");
                                electricalFieldStartTime = gameTime.TotalGameTime;
                                previousElectricalField = gameTime.TotalGameTime;
                                RightArm.MasterSafety = true;
                                LeftArm.MasterSafety = true;
                                Fuselage.MasterSafety = true;
                            }
                            else
                            {
                                electricalField.Active = true;
                                electricalFieldStartTime = gameTime.TotalGameTime;
                                previousElectricalField = gameTime.TotalGameTime;
                                RightArm.MasterSafety = true;
                                LeftArm.MasterSafety = true;
                                Fuselage.MasterSafety = true;
                            }
                    }

                    if (electricalField != null && (((RightArm.Destroyed || LeftArm.Destroyed) && electricalField.Active) || (electricalField.Active && gameTime.TotalGameTime >= electricalFieldStartTime + electricalFieldDuration)))
                    {
                        electricalField.Active = false;
                        previousElectricalField = gameTime.TotalGameTime;
                        RightArm.MasterSafety = false;
                        LeftArm.MasterSafety = false;
                        Fuselage.MasterSafety = false;
                        RightArm.previousFireStopTime = gameTime.TotalGameTime;
                        LeftArm.previousFireStopTime = gameTime.TotalGameTime;
                        Fuselage.previousFireStopTime = gameTime.TotalGameTime;
                        rushing = false;
                    }

                    if (electricalField != null && electricalField.Active && rushing == false && (gameTime.TotalGameTime >= electricalFieldStartTime + rushDelay))
                    {
                        rushing = true;
                    }

                    if (rushing && Position.Y <= 500)
                        Position.Y += electricalFieldRushSpeed;

                    if (Position.Y >= 500 && rushing)
                    {
                        XSpeed = 0;
                    }

                    if (!rushing && Position.Y > 155)
                    {
                        Position.Y -= electricalFieldRushSpeed;
                        XSpeed = PreviousXSpeed;
                    }

                }
            }
            if (!Defeated)
            {
                for (int i = 0; i < Components.Count; i++)
                {
                    Components[i].UpdateComponent(gameTime, this);
                }
                if (electricalField != null)
                    electricalField.UpdateElectricalField(gameTime);
            }
        }

        protected override void DrawBoss(SpriteBatch batch)
        {
            for (int i = 0; i < Components.Count; i++)
            {
                Components[i].DrawSprite(batch);
            }
            if (electricalField != null && electricalField.Active)
                electricalField.DrawElectricalField(batch);
        }
    }
}
