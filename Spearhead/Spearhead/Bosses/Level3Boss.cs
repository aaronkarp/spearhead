#region File Description
//-----------------------------------------------------------------------------
// Level3Boss.cs
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
    /// This class creates and controls the Level 3 Boss, Satellite Assault Drone
    /// </summary>

    class Level3Boss : Boss
    {
        #region Fields
        public int XSpeed; // The boss's lateral speed
        public int YSpeed; // The boss's vertical speed
        public string Direction; // The direction the boss is currently traveling
        TimeSpan verticalStop; // The time at which the boss stops moving vertically
        TimeSpan pauseTime; // The time the boss pauses after the vertical move
        public Component mainBody; // The main body of the ship
        Component satellite1; // One of the orbiting satellites
        Component satellite2; // One of the orbiting satellites
        Component satellite3; // One of the orbiting satellites
        Component satellite4; // One of the orbiting satellites
        Component satellite5; // One of the orbiting satellites
        Component satellite6; // One of the orbiting satellites
        Component satellite7; // One of the orbiting satellites
        Component satellite8; // One of the orbiting satellites
        TimeSpan fireSequenceStart; // The time of the start of the last firing sequence
        TimeSpan fireSequenceDelay; // The delay between firing sequences
        #endregion

        public Level3Boss(Vector2 startPosition, ContentManager content, int value)
            : base(startPosition, content, value)
        {
            mainBody = new AssaultDrone(this, startPosition, new Vector2(0, 0), Content, 900, 1000, false, true, true);
            satellite1 = new Satellite(this, new Vector2(startPosition.X + 49, startPosition.Y + 67), 0, 2.5f, 110, Content, 150, 250, false, true, false);
            satellite2 = new Satellite(this, new Vector2(startPosition.X + 49, startPosition.Y + 67), 45, 2.5f, 110, Content, 150, 250, false, true, false);
            satellite3 = new Satellite(this, new Vector2(startPosition.X + 49, startPosition.Y + 67), 90, 2.5f, 110, Content, 150, 250, false, true, false);
            satellite4 = new Satellite(this, new Vector2(startPosition.X + 49, startPosition.Y + 67), 135, 2.5f, 110, Content, 150, 250, false, true, false);
            satellite5 = new Satellite(this, new Vector2(startPosition.X + 49, startPosition.Y + 67), 180, 2.5f, 110, Content, 150, 250, false, true, false);
            satellite6 = new Satellite(this, new Vector2(startPosition.X + 49, startPosition.Y + 67), 225, 2.5f, 110, Content, 150, 250, false, true, false);
            satellite7 = new Satellite(this, new Vector2(startPosition.X + 49, startPosition.Y + 67), 270, 2.5f, 110, Content, 150, 250, false, true, false);
            satellite8 = new Satellite(this, new Vector2(startPosition.X + 49, startPosition.Y + 67), 315, 2.5f, 110, Content, 150, 250, false, true, false);
            Components = new List<Component>();
            Components.Add(mainBody);
            Components.Add(satellite1);
            Components.Add(satellite2);
            Components.Add(satellite3);
            Components.Add(satellite4);
            Components.Add(satellite5);
            Components.Add(satellite6);
            Components.Add(satellite7);
            Components.Add(satellite8);
            
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
            TotalSize = new Vector2(96, 136); // This only covers the main body area of the boss to prevent explosions appearing in empty space
            ExplosionCount = 7;
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
                    if (Position.X <= 82 && Direction == "Left")
                    {
                        XSpeed = 3;
                        Direction = "Right";
                    }
                    if (Position.X >= 302 && Direction == "Right")
                    {
                        XSpeed = -3;
                        Direction = "Left";
                    }
                    Position.X += XSpeed;
                }

                if (IsDeployed)
                {
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
