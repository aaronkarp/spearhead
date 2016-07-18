#region File Description
//-----------------------------------------------------------------------------
// MissilePodDoor.cs
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
    class MissilePodDoor : Component
    {
        #region Fields
        public int Travel; // The distance the door has moved independent of boss movement while opening and closing
        public int TravelSpeed; // The speed of the door when opening and closing
        TimeSpan openTime; // The time at which the door is fully open
        TimeSpan fireDelay; // The delay between the opening of the door and the missiles firing
        TimeSpan fireTime; // The time at which the missiles are fired
        TimeSpan closeDelay; // The delay between the missiles firing and the door closing
        public List<Projectile> Missiles; // The missiles fired by the pod
        ContentManager Content; // The Content Manager
        public Random Random;
        #endregion

        public MissilePodDoor(Boss boss, Vector2 origin, Vector2 offset, ContentManager content, int health, int value, bool mirrored, bool collidable, bool critical)
            : base(boss, origin, offset, "Images/MissilePodDoor", content, health, value, mirrored, collidable, critical)
        {
            FrameSize = new Vector2(80, 60);
            Texture = content.Load<Texture2D>("Images/MissilePodDoor");
            Destroyed = false;
            HitArea = new Rectangle((int)Position.X, (int)Position.Y, Texture.Width, Texture.Height);
            Status = "Closed";
            TravelSpeed = 4;
            Travel = 0;
            openTime = TimeSpan.Zero;
            fireDelay = TimeSpan.FromSeconds(1.0f);
            fireTime = TimeSpan.Zero;
            closeDelay = TimeSpan.FromSeconds(1.75f);
            Missiles = new List<Projectile>();
            Content = content;
            Random = new Random();
            Damageable = false;
            ExplosionCount = 0;
        }

        public override void UpdateComponent(GameTime gameTime, Boss boss)
        {
            HitArea = new Rectangle((int)Position.X, (int)Position.Y, Texture.Width, Texture.Height);
            if (!Destroyed)
            {
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
                if (Math.Abs(Travel) >= 52)
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
            startPosition += new Vector2(14, 18);
            int verticalSpeed = -15;
            int lateralSpeed = Random.Next(-30, 31);
            int lateralDistance = Random.Next(-400, -299);
            int damage = 20;
            string texturePath = "Images/BossMissile";
            int decay = 1;
            bool uninterrupted = false;
            bool hurtPlayer = true;
            if (startPosition.X + lateralDistance >= 480)
            {
                lateralDistance = Random.Next(440, 466) - (int)startPosition.X;
            }
            if (startPosition.X + lateralDistance <= 0)
            {
                lateralDistance = (int)startPosition.X - Random.Next(5, 21);
            }
            Projectile projectile1 = new Projectile(startPosition, content, verticalSpeed, lateralSpeed, lateralDistance, damage, texturePath, uninterrupted, hurtPlayer, decay, "large");
            startPosition += new Vector2(14, 0);
            lateralSpeed = Random.Next(-30, -9);
            lateralDistance = Random.Next(-200, -1);
            if (startPosition.X + lateralDistance >= 480)
            {
                lateralDistance = Random.Next(440, 466) - (int)startPosition.X;
            }
            if (startPosition.X + lateralDistance <= 0)
            {
                lateralDistance = (int)startPosition.X - Random.Next(5, 21);
            }
            Projectile projectile2 = new Projectile(startPosition, content, verticalSpeed, lateralSpeed, lateralDistance, damage, texturePath, uninterrupted, hurtPlayer, decay, "large");
            startPosition += new Vector2(14, 0);
            lateralSpeed = Random.Next(-30, -9);
            lateralDistance = Random.Next(1, 201);
            if (startPosition.X + lateralDistance >= 480)
            {
                lateralDistance = Random.Next(440, 466) - (int)startPosition.X;
            }
            if (startPosition.X + lateralDistance <= 0)
            {
                lateralDistance = (int)startPosition.X - Random.Next(5, 21);
            }
            Projectile projectile3 = new Projectile(startPosition, content, verticalSpeed, lateralSpeed, lateralDistance, damage, texturePath, uninterrupted, hurtPlayer, decay, "large");
            startPosition += new Vector2(14, 0);
            lateralSpeed = Random.Next(10, 31);
            lateralDistance = Random.Next(300, 401);
            if (startPosition.X + lateralDistance >= 480)
            {
                lateralDistance = Random.Next(440, 466) - (int)startPosition.X;
            }
            if (startPosition.X + lateralDistance <= 0)
            {
                lateralDistance = (int)startPosition.X - Random.Next(5, 21);
            }
            Projectile projectile4 = new Projectile(startPosition, content, verticalSpeed, lateralSpeed, lateralDistance, damage, texturePath, uninterrupted, hurtPlayer, decay, "large");
            startPosition += new Vector2(-42, 16);
            lateralSpeed = Random.Next(10, 31);
            lateralDistance = Random.Next(-400, -299);
            if (startPosition.X + lateralDistance >= 480)
            {
                lateralDistance = Random.Next(440, 466) - (int)startPosition.X;
            }
            if (startPosition.X + lateralDistance <= 0)
            {
                lateralDistance = (int)startPosition.X - Random.Next(5, 21);
            }
            Projectile projectile5 = new Projectile(startPosition, content, verticalSpeed, lateralSpeed, lateralDistance, damage, texturePath, uninterrupted, hurtPlayer, decay, "large");
            startPosition += new Vector2(14, 0);
            lateralSpeed = Random.Next(-30, -9);
            lateralDistance = Random.Next(-200, -1);
            if (startPosition.X + lateralDistance >= 480)
            {
                lateralDistance = Random.Next(440, 466) - (int)startPosition.X;
            }
            if (startPosition.X + lateralDistance <= 0)
            {
                lateralDistance = (int)startPosition.X - Random.Next(5, 21);
            }
            Projectile projectile6 = new Projectile(startPosition, content, verticalSpeed, lateralSpeed, lateralDistance, damage, texturePath, uninterrupted, hurtPlayer, decay, "large");
            startPosition += new Vector2(14, 0);
            lateralSpeed = Random.Next(10, 31);
            lateralDistance = Random.Next(1, 201);
            if (startPosition.X + lateralDistance >= 480)
            {
                lateralDistance = Random.Next(440, 466) - (int)startPosition.X;
            }
            if (startPosition.X + lateralDistance <= 0)
            {
                lateralDistance = (int)startPosition.X - Random.Next(5, 21);
            }
            Projectile projectile7 = new Projectile(startPosition, content, verticalSpeed, lateralSpeed, lateralDistance, damage, texturePath, uninterrupted, hurtPlayer, decay, "large");
            startPosition += new Vector2(14, 0);
            lateralSpeed = Random.Next(10, 31);
            lateralDistance = Random.Next(300, 401);
            if (startPosition.X + lateralDistance >= 480)
            {
                lateralDistance = Random.Next(440, 466) - (int)startPosition.X;
            }
            if (startPosition.X + lateralDistance <= 0)
            {
                lateralDistance = (int)startPosition.X - Random.Next(5, 21);
            }
            Projectile projectile8 = new Projectile(startPosition, content, verticalSpeed, lateralSpeed, lateralDistance, damage, texturePath, uninterrupted, hurtPlayer, decay, "large");
            Boss.Projectiles.Add(projectile1);
            Boss.Projectiles.Add(projectile2);
            Boss.Projectiles.Add(projectile3);
            Boss.Projectiles.Add(projectile4);
            Boss.Projectiles.Add(projectile5);
            Boss.Projectiles.Add(projectile6);
            Boss.Projectiles.Add(projectile7);
            Boss.Projectiles.Add(projectile8);
        }

        public override void DrawSprite(SpriteBatch batch)
        {
            // The door disappears when the missile pod is destroyed
            if (!Destroyed)
            {
                batch.End();
                batch.Begin();
                batch.Draw(Texture, Position, color);
                if (color == Color.Red)
                    color = Color.White;
            }
        }
    }
}