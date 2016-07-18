#region File Description
//-----------------------------------------------------------------------------
// Satellite.cs
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
    class Satellite : Component
    {
        #region Fields
        ContentManager Content; // The Content Manager
        public float Angle; // The current angle of rotation
        public float OrbitSpeed; // The number of degrees to rotate per frame
        public int Radius; // The distance at which the satellite orbits the center
        public string ProjectileTexture; // The projectile fired by the satellite
        int damage; // The damage done by the projectile
        #endregion

        public Satellite(Boss boss, Vector2 origin, float angle, float orbitSpeed, int radius, ContentManager content, int health, int value, bool mirrored, bool collidable, bool critical)
            : base(boss, origin, new Vector2(110,110), "Images/level-3-boss-satellite", content, health, value, mirrored, collidable, critical)
        {
            Origin = origin;
            Texture = content.Load<Texture2D>("Images/level-3-boss-satellite");
            FrameSize = new Vector2(Texture.Width, Texture.Height);
            Destroyed = false;
            HitArea = new Rectangle((int)Position.X + 5, (int)Position.Y + 5, Texture.Width - 5, Texture.Height - 5);
            Content = content;
            Damageable = true;
            ExplosionCount = 1;
            Angle = angle;
            OrbitSpeed = orbitSpeed;
            Radius = radius;
            Independent = true;
            ProjectileTexture = "Images/level-3-boss-satellite-projectile";
            damage = 10;
        }

        public override void UpdateComponent(GameTime gameTime, Boss boss)
        {
            if (!Destroyed)
            {
                if (Angle == 360)
                    Angle = 0;
                Angle += OrbitSpeed;
                double rad;
                rad = Angle * (Math.PI / 180);
                Position.X = (int)(Boss.Position.X + 18 + Radius * Math.Cos(rad)) + 10;
                Position.Y = (int)(Boss.Position.Y + 36 + Radius * Math.Sin(rad)) + 10;
                if (Angle % 45 == 0 && Boss.IsDeployed)
                    AddProjectile(new Vector2(Position.X + 20, Position.Y + 20), Content);
                base.UpdateComponent(gameTime, boss);
                HitArea.X = (int)Position.X + 5;
                HitArea.Y = (int)Position.Y + 5;
                HitArea.Width = Texture.Width - 10;
                HitArea.Height = Texture.Height - 10;
            }
        }

        public void AddProjectile(Vector2 startPosition, ContentManager content)
        {
            if (Angle == 0)
            {
                Projectile projectile = new Projectile(startPosition, content, 0, 10, 480, damage, ProjectileTexture, false, true, 0, "small");
                Boss.Projectiles.Add(projectile);
            }

            if (Angle == 45)
            {
                Projectile projectile = new Projectile(startPosition, content, -10, 10, 480, damage, ProjectileTexture, false, true, 0, "small");
                Boss.Projectiles.Add(projectile);
            }

            if (Angle == 90)
            {
                Projectile projectile = new Projectile(startPosition, content, -10, 0, 480, damage, ProjectileTexture, false, true, 0, "small");
                Boss.Projectiles.Add(projectile);
            }

            if (Angle == 135)
            {
                Projectile projectile = new Projectile(startPosition, content, -10, -10, 480, damage, ProjectileTexture, false, true, 0, "small");
                Boss.Projectiles.Add(projectile);
            }

            if (Angle == 180)
            {
                Projectile projectile = new Projectile(startPosition, content, 0, -10, 480, damage, ProjectileTexture, false, true, 0, "small");
                Boss.Projectiles.Add(projectile);
            }

            if (Angle == 225)
            {
                Projectile projectile = new Projectile(startPosition, content, 10, -10, 480, damage, ProjectileTexture, false, true, 0, "small");
                Boss.Projectiles.Add(projectile);
            }

            if (Angle == 270)
            {
                Projectile projectile = new Projectile(startPosition, content, 10, 0, 480, damage, ProjectileTexture, false, true, 0, "small");
                Boss.Projectiles.Add(projectile);
            }

            if (Angle == 315)
            {
                Projectile projectile = new Projectile(startPosition, content, 10, 10, 480, damage, ProjectileTexture, false, true, 0, "small");
                Boss.Projectiles.Add(projectile);
            }
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