#region File Description
//-----------------------------------------------------------------------------
// PlayerShip.cs
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
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace Spearhead
{
    /// <summary>
    /// This class provides the player's ship and all of its functionality.
    /// </summary>

    class PlayerShip : Sprite
    {
        #region Fields

        Vector2 StartPosition;
        ContentManager Content;
        public int Health;
        public bool Dead = false;
        public TimeSpan DeathTime;
        public TimeSpan DeathExplosionTime;
        public int ExplosionCount;
        Texture2D Texture;

        // These flags determine what powerups are in effect
        public string WeaponUp;
        public string SecondaryWeaponUp;
        public string ShieldUp;

        // This stores the shield status
        public Shield Shield;
        public int ShieldStrength;

        // The following variables define the current weapon
        public string ProjectileTexture;
        public TimeSpan FireTime;
        TimeSpan PreviousFireTime;
        public bool ProjectileUninterrupted; // Determines if the projectile should be deactivated upon hitting an enemy ship
        public int ProjectileVerticalSpeed; // The vertical speed of the projectile
        public int ProjectileLateralSpeed; // The lateral speed of the projectile (usually 0)
        public int ProjectileDamage; // Damage inflicted by the projectile
        public int Projectile1Offset; // Aligns the left projectile with the ship's left wing
        public int Projectile2Offset; // Aligns the right projectile with the ship's right wing
        public float ProjectileFireRate; // The refire rate in seconds
        public bool ProjectileAlternate; // Always false for primary weapons
        public int ProjectileLateralDistance; // Always zero for primary weapons
        public string ProjectileImpactType; // The type of impact sprite to use (large/small)

        // The following variables define the secondary weapon
        public string SecondaryProjectileTexture;
        public TimeSpan SecondaryFireTime;
        TimeSpan PreviousSecondaryFireTime;
        public string SecondaryProjectileDirection;
        public bool SecondaryProjectileUninterrupted; // Determines if the projectile should be deactivated on hitting an enemy ship
        public int SecondaryProjectileVerticalSpeed; // The vertical speed of the projectile
        public int SecondaryProjectileLateralSpeed; // The lateral speed of the projectile
        public int SecondaryProjectileDamage; // Damage inflicted by the projectile
        public int SecondaryProjectile1Offset; // Aligns the left projectile as needed
        public int SecondaryProjectile2Offset; // Aligns the right projectile as needed
        public int SecondaryProjectileVerticalOffset; // Aligns the projectiles vertically as needed
        public float SecondaryProjectileFireRate; // The refire rate in seconds
        public bool SecondaryProjectileAlternate; // Sets whether the projectiles should be fired simultaneously (false) or alternate left/right (true)
        public int SecondaryLateralDistance; // The distance from the firing location at which the projectiles should cease lateral motion
        public string SecondaryProjectileImpactType; // The type of impact sprite to use (large/small)

        public List<Projectile> projectiles;

        // Variables for post-death invincibility
        public TimeSpan InvincibleStart; // The time at which invincibility begins
        public TimeSpan InvincibleDuration; // How long invincibility should last
        public bool Invincible; // If the ship is currently invincible
        public bool BlinkOn; // A flag to make the ship blink to indicate invincibility
        public TimeSpan BlinkStart; // When the ship starts blinking
        public TimeSpan BlinkDuration; // How long the ship should appear/disappear per blink

        #endregion

        public PlayerShip(Vector2 startPosition, ContentManager content, string weaponUp, string secondaryWeaponUp, string shieldUp, int shieldStrength)
            : base(content, "Images/player_indicator")
        {
            Position.X = startPosition.X;
            Position.Y = startPosition.Y;
            StartPosition = startPosition;
            this.Content = content;
            Health = 100;
            WeaponUp = weaponUp;
            SecondaryWeaponUp = secondaryWeaponUp;
            ShieldUp = shieldUp;
            ShieldStrength = shieldStrength;

            if (WeaponUp == "Gatling")
            {
                ProjectileTexture = "Images/gatling-round";
                ProjectileUninterrupted = false;
                ProjectileVerticalSpeed = 20;
                ProjectileLateralSpeed = 0;
                ProjectileDamage = 5;
                Projectile1Offset = 2;
                Projectile2Offset = -4;
                ProjectileFireRate = .09f;
                ProjectileAlternate = false;
                ProjectileLateralDistance = 0;
                ProjectileImpactType = "small";
            }
            else if (WeaponUp == "Laser")
            {
                ProjectileTexture = "Images/laser-round";
                ProjectileUninterrupted = true;
                ProjectileVerticalSpeed = 30;
                ProjectileLateralSpeed = 0;
                ProjectileDamage = 10;
                Projectile1Offset = 22;
                Projectile2Offset = 600;
                ProjectileFireRate = .33f;
                ProjectileAlternate = false;
                ProjectileLateralDistance = 0;
                ProjectileImpactType = "small";
            }
            else
            {
                ProjectileTexture = "Images/player_shot_single";
                ProjectileUninterrupted = false;
                ProjectileVerticalSpeed = 20;
                ProjectileLateralSpeed = 0;
                ProjectileDamage = 7;
                Projectile1Offset = 0;
                Projectile2Offset = -6;
                ProjectileFireRate = .2f;
                ProjectileAlternate = false;
                ProjectileLateralDistance = 0;
                ProjectileImpactType = "small";
            }

            if (secondaryWeaponUp == "Missile")
            {
                SecondaryProjectileTexture = "Images/missile";
                SecondaryProjectileDirection = "Left";
                SecondaryProjectileUninterrupted = false;
                SecondaryProjectileVerticalSpeed = 5;
                SecondaryProjectileLateralSpeed = 10;
                SecondaryProjectileDamage = 15;
                SecondaryProjectile1Offset = 2;
                SecondaryProjectile2Offset = -4;
                SecondaryProjectileVerticalOffset = 30;
                SecondaryProjectileFireRate = .25f;
                SecondaryProjectileAlternate = true;
                SecondaryLateralDistance = 80;
                SecondaryProjectileImpactType = "large";
            }

            FireTime = TimeSpan.FromSeconds(ProjectileFireRate);
            SecondaryFireTime = TimeSpan.FromSeconds(SecondaryProjectileFireRate);
            projectiles = new List<Projectile>();
            DeathTime = TimeSpan.Zero;
            DeathExplosionTime = TimeSpan.FromSeconds(.3f);
            ExplosionCount = 0;
            Texture = content.Load<Texture2D>("Images/player_indicator");
            WeaponUp = weaponUp;
            ShieldUp = shieldUp;
            ShieldStrength = shieldStrength;
            Invincible = false;
            InvincibleStart = TimeSpan.Zero;
            InvincibleDuration = TimeSpan.FromSeconds(3f);
            BlinkOn = false;
            BlinkStart = TimeSpan.Zero;
            BlinkDuration = TimeSpan.FromSeconds(0.15f);
        }

        public void Reset()
        {
            Position = StartPosition;
            Health = 100;
            DeathTime = TimeSpan.Zero;
            Dead = false;
            ExplosionCount = 0;
            WeaponUp = "None";
            SecondaryWeaponUp = "None";
            ShieldUp = "None";
            ProjectileTexture = "Images/player_shot_single";
            ProjectileUninterrupted = false;
            ProjectileVerticalSpeed = 20;
            ProjectileLateralSpeed = 0;
            ProjectileDamage = 7;
            Projectile1Offset = 0;
            Projectile2Offset = -6;
            ProjectileFireRate = .2f;
            ProjectileAlternate = false;
            ProjectileLateralDistance = 0;
            FireTime = TimeSpan.FromSeconds(ProjectileFireRate);
            ProjectileImpactType = "small";
            
        }

        protected override void UpdateSprite(GameTime gameTime)
        {
            if (Dead == true && DeathTime == TimeSpan.Zero)
            {
                DeathTime = gameTime.TotalGameTime;
            }

            if ((gameTime.TotalGameTime - InvincibleStart > InvincibleDuration) && Invincible)
            {
                Invincible = false;
            }

            if (Invincible && BlinkOn)
            {
                if (gameTime.TotalGameTime - BlinkStart > BlinkDuration)
                {
                    BlinkOn = false;
                    BlinkStart = gameTime.TotalGameTime;
                }
            }
            else
            {
                if (gameTime.TotalGameTime - BlinkStart > BlinkDuration)
                {
                    BlinkOn = true;
                    BlinkStart = gameTime.TotalGameTime;
                }
            }

            if ((gameTime.TotalGameTime - PreviousFireTime > FireTime) && (Dead == false))
            {
                // Set the last weapon fire time to the current time
                PreviousFireTime = gameTime.TotalGameTime;

                // Add the projectile, but add it to the front and center of the player
                AddProjectile(new Vector2(this.Position.X + Projectile1Offset, this.Position.Y), Content);
                AddProjectile(new Vector2(this.Position.X + this.Width + Projectile2Offset, this.Position.Y), Content);
            }

            if (SecondaryWeaponUp != "None")
            {
                if ((gameTime.TotalGameTime - PreviousSecondaryFireTime > SecondaryFireTime) && (Dead == false))
                {
                    // Set the last weapon fire time to the current time
                    PreviousSecondaryFireTime = gameTime.TotalGameTime;

                    int offset = 0;

                    if (SecondaryProjectileAlternate)
                    {
                        if (SecondaryProjectileDirection == "Left")
                        {
                            SecondaryProjectileDirection = "Right";
                            offset = (int)Position.X + SecondaryProjectile1Offset;
                        }
                        else
                        {
                            SecondaryProjectileDirection = "Left";
                            offset = (int)Position.X + Width + SecondaryProjectile2Offset;
                        }

                        SecondaryProjectileLateralSpeed *= -1;
                    }

                    AddSecondaryProjectile(new Vector2(offset, this.Position.Y + SecondaryProjectileVerticalOffset), Content);
                }
            }

            if (Shield != null && Shield.Active)
            {
                Shield.Update(gameTime);
            }

            UpdateProjectiles(gameTime);
        }

        private void AddProjectile(Vector2 position, ContentManager content)
        {
            Projectile projectile = new Projectile(position, content, ProjectileVerticalSpeed, ProjectileLateralSpeed, ProjectileLateralDistance, ProjectileDamage, ProjectileTexture, ProjectileUninterrupted, false, 0, ProjectileImpactType);
            projectiles.Add(projectile);
        }

        private void AddSecondaryProjectile(Vector2 position, ContentManager content)
        {
            if (SecondaryProjectileAlternate)
            {
                Projectile projectile = new Projectile(position, content, SecondaryProjectileVerticalSpeed, SecondaryProjectileLateralSpeed, SecondaryLateralDistance, SecondaryProjectileDamage, SecondaryProjectileTexture, SecondaryProjectileUninterrupted, false, 0, SecondaryProjectileImpactType);
                projectiles.Add(projectile);
            }
            else
            {
                Projectile projectile = new Projectile(new Vector2(position.X + SecondaryProjectile1Offset, position.Y + SecondaryProjectileVerticalOffset), content, SecondaryProjectileVerticalSpeed, SecondaryProjectileLateralSpeed, SecondaryLateralDistance, SecondaryProjectileDamage, SecondaryProjectileTexture, SecondaryProjectileUninterrupted, false, 0, SecondaryProjectileImpactType);
                projectiles.Add(projectile);
                projectile = new Projectile(new Vector2(position.X + Width + SecondaryProjectile2Offset, position.Y + SecondaryProjectileVerticalOffset), content, SecondaryProjectileVerticalSpeed, SecondaryProjectileLateralSpeed * -1, SecondaryLateralDistance, SecondaryProjectileDamage, SecondaryProjectileTexture, SecondaryProjectileUninterrupted, false, 0, SecondaryProjectileImpactType);
                projectiles.Add(projectile);
            }
        }

        private void UpdateProjectiles(GameTime gameTime)
        {
            for (int i = projectiles.Count - 1; i >= 0; i--)
            {
                projectiles[i].Update(gameTime);

                if (projectiles[i].Active == false)
                {
                    projectiles.RemoveAt(i);
                }
            }
        }

        public override void DrawSprite(SpriteBatch batch)
        {
            batch.End();
            batch.Begin();
            if (BlinkOn)
                batch.Draw(Texture, Position, Color.White);
            if (Shield != null && Shield.Active)
            {
                Shield.Draw(batch);
            }
        }
    }
}