#region File Description
//-----------------------------------------------------------------------------
// RightMissileDoor.cs
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
    /// This class creates an animated door for the missile pod component
    /// </summary>
    class RightMissileDoor : Component
    {
        #region Fields
        public int Travel; // The distance the door has moved independent of boss movement while opening and closing
        public int TravelSpeed; // The speed of the door when opening and closing
        public TimeSpan fireSequenceStart; // The beginning of the firing sequence, when the door starts to open
        public TimeSpan fireSequenceDelay; // The delay between firing sequences
        TimeSpan openTime; // The time at which the door is fully open
        TimeSpan fireDelay; // The delay between the opening of the door and the missiles firing
        TimeSpan fireTime; // The time at which the missiles are fired
        TimeSpan closeDelay; // The delay between the missiles firing and the door closing
        public List<Projectile> Missiles; // The missiles fired by the pod
        ContentManager Content; // The Content Manager
        Level5Boss l5boss;
        #endregion

        public RightMissileDoor(Boss boss, Vector2 origin, Vector2 offset, ContentManager content, int health, int value, bool mirrored, bool collidable, bool critical)
            : base(boss, origin, offset, "Images/level-5-boss-wing-right-missile-doors-spritesheet", content, health, value, mirrored, collidable, critical)
        {
            Texture = content.Load<Texture2D>("Images/level-5-boss-wing-right-missile-doors-spritesheet");
            FrameSize = new Vector2(Texture.Width / 2, Texture.Height);
            Destroyed = false;
            HitArea = new Rectangle((int)Position.X, (int)Position.Y, Texture.Width / 2, Texture.Height);
            Status = "Closed";
            TravelSpeed = -1;
            Travel = 0;
            fireSequenceStart = TimeSpan.Zero;
            fireSequenceDelay = TimeSpan.FromSeconds(5f);
            openTime = TimeSpan.Zero;
            fireDelay = TimeSpan.FromSeconds(1.0f);
            fireTime = TimeSpan.Zero;
            closeDelay = TimeSpan.FromSeconds(1.75f);
            Missiles = new List<Projectile>();
            Content = content;
            Damageable = false;
            ExplosionCount = 2;
            l5boss = (Level5Boss)boss;
        }

        public override void UpdateComponent(GameTime gameTime, Level5Boss boss, string leftDoorStatus, string rightDoorStatus)
        {
            HitArea = new Rectangle((int)Position.X, (int)Position.Y, Texture.Width, Texture.Height);
            if (!Destroyed)
            {
                if (gameTime.TotalGameTime - fireSequenceDelay >= fireSequenceStart && Status == "Closed")
                {
                    Status = "Opening";
                    boss.RightMissiles.Damageable = true;
                    fireSequenceStart = gameTime.TotalGameTime;
                }
                if (Status == "Opening")
                {
                    Offset.Y -= TravelSpeed;
                    Travel -= TravelSpeed;
                }
                if (Status == "Closing")
                {
                    Offset.Y += TravelSpeed;
                    Travel += TravelSpeed;
                }
                if (Math.Abs(Travel) >= 9)
                {
                    if (Status == "Opening")
                    {
                        Status = "Open";
                        openTime = gameTime.TotalGameTime;
                        Travel = 0;
                    }
                    if (Status == "Closing")
                    {
                        Status = "Closed";
                        boss.RightMissiles.Damageable = false;
                        Travel = 0;
                    }
                }
                if (Status == "Open" && (gameTime.TotalGameTime - openTime > fireDelay))
                {
                    Status = "Firing";
                    fireTime = gameTime.TotalGameTime;
                    AddProjectile(Position, Content);
                }
                if (Status == "Firing" && (gameTime.TotalGameTime - fireTime > closeDelay))
                {
                    Status = "Closing";
                }
                base.UpdateComponent(gameTime, boss);
            }
        }

        public void AddProjectile(Vector2 startPosition, ContentManager content)
        {
            startPosition += new Vector2(10, 4);
            int verticalSpeed = -15;
            int lateralSpeed = -10;
            int lateralDistance = 40;
            int damage = 10;
            string texturePath = "Images/BossMissile2";
            int decay = 2;
            bool uninterrupted = false;
            bool hurtPlayer = true;
            Projectile projectile1 = new Projectile(startPosition, content, verticalSpeed, lateralSpeed, lateralDistance, damage, texturePath, uninterrupted, hurtPlayer, decay, "large");
            startPosition += new Vector2(14, 2);
            lateralSpeed = -5;
            lateralDistance = 20;
            Projectile projectile2 = new Projectile(startPosition, content, verticalSpeed, lateralSpeed, lateralDistance, damage, texturePath, uninterrupted, hurtPlayer, decay, "large");
            startPosition += new Vector2(14, 2);
            lateralSpeed = 5;
            lateralDistance = 20;
            Projectile projectile3 = new Projectile(startPosition, content, verticalSpeed, lateralSpeed, lateralDistance, damage, texturePath, uninterrupted, hurtPlayer, decay, "large");
            startPosition += new Vector2(14, 2);
            lateralSpeed = 10;
            lateralDistance = 40;
            Projectile projectile4 = new Projectile(startPosition, content, verticalSpeed, lateralSpeed, lateralDistance, damage, texturePath, uninterrupted, hurtPlayer, decay, "large");
            Boss.Projectiles.Add(projectile1);
            Boss.Projectiles.Add(projectile2);
            Boss.Projectiles.Add(projectile3);
            Boss.Projectiles.Add(projectile4);
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
                batch.Draw(Texture, Position, sourceRect, color);
                if (color == Color.Red)
                    color = Color.White;
            }
        }
    }
}