#region File Description
//-----------------------------------------------------------------------------
// Jaw.cs
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
    /// This class creates the animated jaw boss component
    /// </summary>
    class Jaw : Component
    {
        #region Fields
        public int Travel; // The distance the jaw has moved independent of boss movement while opening and closing
        public int TravelSpeed; // The speed of the jaw when opening and closing
        TimeSpan fireSequenceDelay; // The time between firing of the large laser
        public TimeSpan PreviousFireSequence; // The time the last firing sequence ended
        TimeSpan openTime; // The time at which the jaw is fully open
        TimeSpan fireDelay; // The delay between the opening of the jaw and the large laser firing
        TimeSpan glowDelay; // The delay between the laser lens beginning to glow and the actual firing
        TimeSpan glowTime; // The time that the laser lens begins to glow
        TimeSpan fireTime; // The time at which the large laser is fired
        TimeSpan fireDuration; // The length of time the large laser should remain firing
        TimeSpan fireStopTime; // The time when the large laser stops firing
        TimeSpan closeDelay; // The delay between the large laser firing and the door closing
        Texture2D glowTexture; // The glow effect to be applied to the laser lens before firing
        public List<Projectile> Laser; // The large laser fired by the pod
        ContentManager Content; // The Content Manager
        public Projectile Projectile; // The giant beam projectile
        #endregion

        public Jaw(Boss boss, Vector2 origin, Vector2 offset, ContentManager content, int health, int value, bool mirrored, bool collidable, bool critical)
            : base(boss, origin, offset, "Images/level-4-boss-jaw", content, health, value, mirrored, collidable, critical)
        {
            FrameSize = new Vector2(152, 168);
            Texture = content.Load<Texture2D>("Images/level-4-boss-jaw");
            glowTexture = content.Load<Texture2D>("Images/level-4-boss-weapon-firing");
            Destroyed = false;
            HitArea = new Rectangle((int)Position.X + 44, (int)Position.Y + 56, 64, 62);
            Status = "Closed";
            TravelSpeed = 6;
            Travel = 0;
            fireSequenceDelay = TimeSpan.FromSeconds(5.0f);
            PreviousFireSequence = TimeSpan.Zero;
            openTime = TimeSpan.Zero;
            fireDelay = TimeSpan.FromSeconds(1.0f);
            glowDelay = TimeSpan.FromSeconds(1.0f);
            fireDuration = TimeSpan.FromSeconds(2.5f);
            glowTime = TimeSpan.Zero;
            fireTime = TimeSpan.Zero;
            fireStopTime = TimeSpan.Zero;
            closeDelay = TimeSpan.FromSeconds(1.75f);
            Laser = new List<Projectile>();
            Content = content;
            Damageable = false;
            ExplosionCount = 5;
        }

        public override void UpdateComponent(GameTime gameTime, Boss boss)
        {
            HitArea = new Rectangle((int)Position.X + 44, (int)Position.Y + 56, 64, 62);
            Position = Boss.Position + Offset;
            if (!Destroyed)
            {
                if (gameTime.TotalGameTime >= PreviousFireSequence + fireSequenceDelay && Status == "Closed")
                {
                    Status = "Opening";
                }
                if (Status == "Opening")
                {
                    Offset.Y += TravelSpeed;
                    Travel += TravelSpeed;
                    Damageable = true;
                }
                if (Status == "Closing")
                {
                    Offset.Y -= TravelSpeed;
                    Travel -= TravelSpeed;
                    Damageable = true;
                }
                if (Math.Abs(Travel) >= 48)
                {
                    if (Status == "Opening")
                    {
                        Status = "Open";
                        openTime = gameTime.TotalGameTime;
                        Travel = 0;
                        Damageable = true;
                    }
                    if (Status == "Closing")
                    {
                        Status = "Closed";
                        Travel = 0;
                        PreviousFireSequence = gameTime.TotalGameTime;
                        Damageable = false;
                    }
                }
                if (Status == "Open" && (gameTime.TotalGameTime - openTime > fireDelay))
                {
                    Status = "Glowing";
                    glowTime = gameTime.TotalGameTime;
                    Damageable = true;
                }
                if (Status == "Glowing" && (gameTime.TotalGameTime - glowTime > glowDelay))
                {
                    Status = "Firing";
                    fireTime = gameTime.TotalGameTime;
                    AddProjectile(Position, Content);
                    Damageable = true;
                }
                if (Status == "Firing")
                {
                    Projectile.Position.X = Position.X + 46;
                }
                if (Status == "Firing" && (gameTime.TotalGameTime - fireTime > fireDuration))
                {
                    Status = "Fired";
                    Projectile.Active = false;
                }
                if (Status == "Fired" && (gameTime.TotalGameTime - fireTime > closeDelay))
                {
                    Status = "Closing";
                    Damageable = true;
                }
                base.UpdateComponent(gameTime, boss);
                if (Destroyed)
                {
                    if (Projectile != null && Projectile.Active)
                    {
                        Status = "Closing";
                        Projectile.Active = false;
                    }
                }
            }
        }

        public void AddProjectile(Vector2 startPosition, ContentManager content)
        {
            startPosition = new Vector2(Position.X + 45, Position.Y + 74);
            int verticalSpeed = 0;
            int lateralSpeed = 0;
            int lateralDistance = 0;
            int damage = 2;
            string texturePath = "Images/giantbeam";
            int decay = 0;
            bool uninterrupted = true;
            bool hurtPlayer = true;
            Projectile = new Projectile(startPosition, content, verticalSpeed, lateralSpeed, lateralDistance, damage, texturePath, uninterrupted, hurtPlayer, decay, "large");
            Boss.Projectiles.Add(Projectile);
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
            if (Status == "Glowing")
                batch.Draw(glowTexture, new Vector2(Position.X + 45, Position.Y + 75), color);
            if (color == Color.Red)
                color = Color.White;
        }
    }
}