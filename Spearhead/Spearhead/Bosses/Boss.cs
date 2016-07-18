#region File Description
//-----------------------------------------------------------------------------
// Boss.cs
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
    /// This class provides the basis for all boss ships.
    /// </summary>

    class Boss
    {
        #region Fields
        public List<Component> Components; // A list of all of the components that constitute the boss
        public List<Component> Criticals; // A list of the components that have to be destroyed to destroy the boss
        public bool IsActive; // A flag to determine if the boss should be rendered
        public Vector2 Position; // The boss's position on the screen
        public bool IsDeployed; // A flag to indicate if the boss has reached its proper position on screen and should begin to take damage
        public ContentManager Content; // The content manager
        public bool Defeated; // A flag to indicate that the boss has been completely destroyed
        public bool Exploded; // A flag to indicate that the boss has exploded and its point value has been awarded to the player
        public TimeSpan DeathTime; // The time at which the entire boss was destroyed (or the time of the last explosion post-destruction)
        public TimeSpan ExplosionTime; // The time between explosions
        public int Explosions; // The number of explosions that have occurred
        public int ExplosionCount; // The number of explosions to show when the boss is destroyed completely
        public Vector2 TotalSize; // The overall size of the boss (typically the sum of its components)
        public int Value; // The number of points awarded to the player for defeating the boss
        public List<Projectile> Projectiles; // A master list of all projectiles fired by components
        #endregion

        public Boss(Vector2 startPosition, ContentManager content, int value)
        {
            Position = startPosition;
            Components = new List<Component>();
            Criticals = new List<Component>();
            Content = content;
            DeathTime = TimeSpan.Zero;
            ExplosionTime = TimeSpan.FromSeconds(.15f);
            Defeated = false;
            Exploded = false;
            Value = value;
            Explosions = 0;
            Projectiles = new List<Projectile>();
        }

        public void Update(GameTime gameTime)
        {
            UpdateBoss(gameTime);
            UpdateProjectiles(gameTime);
        }

        protected virtual void UpdateBoss(GameTime gameTime)
        {
        }

        public void UpdateProjectiles(GameTime gameTime)
        {
            for (int i = Projectiles.Count - 1; i >= 0; i--)
            {
                Projectiles[i].Update(gameTime);

                if (Projectiles[i].Active == false)
                {
                    Projectiles.RemoveAt(i);
                }
            }
        }

        public void Draw(SpriteBatch batch)
        {
            DrawBoss(batch);
            for (int i = Projectiles.Count - 1; i >= 0; i--)
            {
                Projectiles[i].Draw(batch);
            }
        }

        protected virtual void DrawBoss(SpriteBatch batch)
        {
        }
    }
}
