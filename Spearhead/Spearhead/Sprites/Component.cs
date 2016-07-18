#region File Description
//-----------------------------------------------------------------------------
// Component.cs
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
    /// This class provides the basis for components of boss characters.
    /// </summary>
    class Component : Sprite
    {
        #region Fields
        public Boss Boss; // The boss to which this component belongs
        ContentManager Content; // The content manager
        public Texture2D Texture; // The sprite sheet of the component
        public Vector2 Origin; // The boss's origin
        public Vector2 Offset; // The component's offset from the boss's origin
        public Vector2 FrameSize; // The size of one frame of the sprite sheet
        public Rectangle HitArea; // The vulnerable area of the component
// TODO: Create a list-based HitArea system to allow for more complex collision detection (v2?)
        public int Health; // The component's remaining health
        public bool IsActive; // A flag to determine if the component should be rendered
        public bool Destroyed; // A flag to mark the component as destroyed without making it inactive in order to preserve active projectiles
        public bool Exploded; // A flag to mark the component as exploded to stop explosion animations and scoring
        public bool Mirrored; // A flag to determine if the sprite should be rendered as a mirror image of itself
        public bool Collidable; // A flag to mark whether the component should be considered in collison detection
        public bool Critical; // A flag to mark whether destroying the component is necessary to destroy the boss
        public int Value; // The component's point value
        public string Status; // The status of the component (used for complex animation sequences)
        public bool Damageable; // Some components can only be damaged in specific circumstances - this switch determines vulnerability
        public TimeSpan DeathTime; // The time at which the component was destroyed (or the last explosion occurred post-destruction)
        public TimeSpan ExplosionTime; // The time between explosions
        public int Explosions; // The number of explosions that have occurred
        public int ExplosionCount; // The number of explosions to show when the component explodes
        public bool Independent; // A switch to determine if the component handles all of its movement in its subclass (true) or relies on the movement code in this class (false)
        public bool MasterSafety; // A switch to prevent component guns from firing when a major weapon is active
        #endregion

        public Component(Boss boss, Vector2 origin, Vector2 offset, string texture, ContentManager content, int health, int value, bool mirrored, bool collidable, bool critical)
            : base(content, texture)
        {
            Boss = boss;
            Origin = origin;
            Offset = offset;
            Content = content;
            Health = health;
            Value = value;
            Mirrored = mirrored;
// TODO: Is Collidable necessary?
            Collidable = collidable;
            Critical = critical;
            IsActive = true;
            Destroyed = false;
            Exploded = false;
            Position = origin + offset;
            Damageable = true;
            DeathTime = TimeSpan.Zero;
            ExplosionTime = TimeSpan.FromSeconds(.4f);
            Explosions = 0;
            Independent = false;
            MasterSafety = false;
        }

        public virtual void UpdateComponent(GameTime gameTime, Boss boss)
        {
            if (Independent == false)
                Position = boss.Position + Offset;
            if (Health <= 0)
            {
                Destroyed = true;
            }
        }

        public virtual void UpdateComponent(GameTime gameTime, Level5Boss boss, string leftDoorStatus, string rightDoorStatus)
        {
            if (Independent == false)
                Position = boss.Position + Offset;
            if (Health <= 0)
            {
                Destroyed = true;
            }
        }
    }
}
