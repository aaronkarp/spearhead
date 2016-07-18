#region File Description
//-----------------------------------------------------------------------------
// Enemy.cs
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
    /// This class provides the basis for enemy ships.
    /// </summary>
    class Enemy : Sprite
    {

        #region Fields
        ContentManager Content; // The content manager
        public Texture2D Texture; // The image of the enemy ship
        public string shipType; // The ship's type
        public float EnemyMoveSpeed; // The speed at which the enemy moves
        public bool Active; // Whether the enemy should be considered in updates
        public int Health; // The enemy's current health
        public int GunDamage; // The damage the enemy's guns do to the player's ship
        public int RamDamage; // The damage done to the player's ship by colliding with the enemy
        public int Value; // The score awarded for destroying the enemy
        public string FormationType; // The type of formation the enemy is part of
        public Vector2 StartPosition; // Where the enemy should be spawned
        string direction; // The lateral direction the enemy is traveling
        Random Random; // A multipurpose random
        int leftRandom; // A random starting point on the left
        int rightRandom; // A random starting point on the right
        TimeSpan stopTime; // The time at which the enemy stops moving
        TimeSpan restartTime; // The time until the enemy begins moving again
        bool moving; // A boolean to show if the ship is moving or not
        bool restarted; // A boolean to show if the ship has started moving after stopping
        public Vector2 offset1; // A Vector2 value providing the offset for projectiles
        public Vector2 offset2; // A Vector2 value providing the offset for a second projectile to be fired per cycle
        public int ProjectileVerticalSpeed; // The vertical speed of the enemy projectile
        public int ProjectileLateralSpeed; // The lateral speed of the enemy projectile
        public int ProjectileLateralDistance; // The distance a projectile should travel laterally before becoming vertical-only
        public bool ProjectileUninterrupted; // A flag to determine if the enemy projectile should continue to exist after hitting the player's ship
        string FireType; // The firing pattern the enemy should use
        public string ProjectileTexture; // The texture for the enemy's projectiles
        TimeSpan fireTime; // The timespan between shots from the enemy
        TimeSpan previousFireTime; // The time when the last projectile was fired
        TimeSpan fireStopTime; // The timespan between bursts in burst fire mode
        TimeSpan previousFireStopTime; // The time the last burst ended
        public List<Projectile> projectiles; // A list of active projectiles fired by the enemy
        bool firing; // A boolean to distinguish between firing and non-firing states
        int fireCount; // The number of rounds fired since the last non-firing state
        public bool Destroyed; // Boolean to mark the enemy as destroyed without making it inactive in order to preserve active projectiles
        public bool Exploded; // Boolean to mark the enemy as exploded to stop explosion animations and scoring
        #endregion

        public Enemy(Vector2 startPosition, ContentManager content, string texture, float enemyMoveSpeed, int health, int gundamage, int ramdamage, int value, string formationType, string fireType, float fireRate, float pauseRate, string projectile)
            : base(content, texture)
        {
            Content = content;
            Texture = content.Load<Texture2D>(texture);
            Position = startPosition;
            StartPosition = startPosition;
            EnemyMoveSpeed = enemyMoveSpeed;
            Health = health;
            GunDamage = gundamage;
            RamDamage = ramdamage;
            Value = value;
            Active = true;
            FormationType = formationType;
            direction = "Left";
            Random = new Random();
            leftRandom = Random.Next(150, 300);
            rightRandom = Random.Next(100, 250);
            stopTime = TimeSpan.Zero;
            restartTime = TimeSpan.FromSeconds(1.5f);
            moving = true;
            restarted = false;
            FireType = fireType;
            ProjectileTexture = projectile;
            previousFireTime = TimeSpan.Zero;
            fireTime = TimeSpan.FromSeconds(fireRate);
            previousFireStopTime = TimeSpan.Zero;
            fireStopTime = TimeSpan.FromSeconds(pauseRate);
            projectiles = new List<Projectile>();
            firing = true;
            fireCount = 0;
            Destroyed = false;
            Exploded = false;
        }

        protected override void UpdateSprite(GameTime gameTime)
        {
            if (moving == true)
                Position.Y += EnemyMoveSpeed;

            if (FormationType == "SweepUp" || FormationType == "SweepDown" || FormationType == "SweepHold")
            {
                int lateralModifier = 1;
                int verticalModifier = 1;
                if (FormationType == "SweepUp")
                {
                    verticalModifier = -1;
                }
                if (StartPosition.X == -60)
                {
                    direction = "Right";
                    lateralModifier = 1;
                }
                else
                {
                    direction = "Left";
                    lateralModifier = -1;
                }

                if ((direction == "Right" && (Position.X < StartPosition.X + leftRandom)) || (direction == "Left" && (Position.X > StartPosition.X - rightRandom)))
                {
                    // directionModifier * (int)(Math.Pow(Position.Y * Amplitude, 2) * Amplitude reduction) / speed reduction
                    Position.X += (lateralModifier * (int)(Math.Pow(Position.Y * 0.5, 2) * 0.001)) / 2;
                }
                else
                {
                    if (moving == true && restarted == false)
                    {
                        stopTime = gameTime.TotalGameTime;
                        moving = false;
                    }
                }
                if (moving == false)
                {
                    if (stopTime + restartTime < gameTime.TotalGameTime && FormationType != "SweepHold")
                    {
                        moving = true;
                        restarted = true;
                        EnemyMoveSpeed = 20 * verticalModifier;
                    }
                }
            }

            if (FormationType == "Zigzag")
            {
                if (Position.X >= StartPosition.X + Texture.Width * 2)
                    direction = "Right";
                if (Position.X <= StartPosition.X - Texture.Width * 2)
                    direction = "Left";
                if (direction == "Right")
                {
                    Position.X -= EnemyMoveSpeed;
                }
                else
                {
                    Position.X += EnemyMoveSpeed;
                }
            }

            // If the enemy's health is less than or equal to zero, mark it as destroyed.
            // If its projectiles are all inactive, mark it inactive as well.
            if (Health <= 0)
            {
                Destroyed = true;
                firing = false;
                moving = false;
                if (projectiles.Count == 0)
                    Active = false;
            }

            // If the enemy is beyond the bottom or top of the screen, set its active flag to false
            if (Position.Y > 800 || (FormationType == "SweepUp" && Position.Y < -50 && restarted == true))
            {
                Active = false;
            }

            if (FireType == "Burst")
            {
                if (FormationType == "SweepUp" || FormationType == "SweepDown" || FormationType == "SweepHold")
                {
                    if (moving == false && restarted == false)
                    {
                        if (firing == true)
                        {
                            if (gameTime.TotalGameTime - previousFireTime > fireTime)
                            {
                                previousFireTime = gameTime.TotalGameTime;
                                AddProjectile(Position + offset1, Content);
                                if (shipType == "Rhino")
                                    AddProjectile(Position + offset2, Content);
                                fireCount++;
                                if (fireCount >= 3)
                                {
                                    firing = false;
                                    previousFireStopTime = gameTime.TotalGameTime;
                                }
                            }
                        }
                        else
                        {
                            if (gameTime.TotalGameTime - previousFireStopTime > fireStopTime)
                            {
                                firing = true;
                                fireCount = 0;
                            }
                        }
                    }
                }
                else
                {
                    if (firing == true)
                    {
                        if (gameTime.TotalGameTime - previousFireTime > fireTime)
                        {
                            previousFireTime = gameTime.TotalGameTime;
                            AddProjectile(Position + offset1, Content);
                            if (shipType == "Rhino")
                                AddProjectile(Position + offset2, Content);
                            fireCount++;
                            if (fireCount >= 3)
                            {
                                firing = false;
                                previousFireStopTime = gameTime.TotalGameTime;
                            }
                        }
                    }
                    else
                    {
                        if (gameTime.TotalGameTime - previousFireStopTime > fireStopTime)
                        {
                            firing = true;
                            fireCount = 0;
                        }
                    }
                }
            }
            else if (FireType != "None")
            {
                if (firing == true)
                {
                    if (gameTime.TotalGameTime - previousFireTime > fireTime)
                    {
                        previousFireTime = gameTime.TotalGameTime;
                        AddProjectile(Position + offset1, Content);
                        if (shipType == "Rhino")
                            AddProjectile(Position + offset2, Content);
                    }
                }
            }

            UpdateProjectiles(gameTime);
        }

        public override void DrawSprite(SpriteBatch spriteBatch)
        {
            if (!Destroyed)
                spriteBatch.Draw(Texture, Position, color);
            if (color == Color.Red)
                color = Color.White;
            for (int i = projectiles.Count - 1; i >= 0; i--)
            {
                projectiles[i].Draw(spriteBatch);
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

        private void AddProjectile(Vector2 position, ContentManager content)
        {
            Projectile projectile = new Projectile(position, content, ProjectileVerticalSpeed * - 1, ProjectileLateralSpeed, ProjectileLateralDistance, GunDamage, ProjectileTexture, ProjectileUninterrupted, true, 0, "small");
            projectiles.Add(projectile);
        }
    }
}
