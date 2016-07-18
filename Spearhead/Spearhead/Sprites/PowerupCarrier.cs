#region File Description
//-----------------------------------------------------------------------------
// PowerupCarrier.cs
//
// Spearhead
// Copyright (C) Showerhead Studios. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace Spearhead
{
    class PowerupCarrier : Enemy
    {

        public PowerupCarrier(Vector2 startPosition, ContentManager content, string formationType)
            : base(startPosition, content, "Images/Powerup-Carrier", 3.0f, 5, 0, 0, 0, formationType, "Burst", 0, 0, "None")
        {
        }

        protected override void UpdateSprite(GameTime gameTime)
        {

            Position.Y += EnemyMoveSpeed;
            // (Math.Sin((speed) / time adjustment) * amplitude) + center adjustment
            Position.X = (int)((float)Math.Sin((Position.Y) / 35) * 225) + 220;

            // If the powerup is beyond the bottom of the screen or has been destroyed, set its active flag to false
            if (Position.Y > 800 || Health <= 0)
            {
                Active = false;
            }
        }

        public string DeployType(Random random)
        {
            int selector = random.Next(1, 6);
            switch (selector)
            {
                case 1:
                    return "Gatling";
                case 2:
                    return "Laser";
                case 3:
                    return "Missile";
                case 4:
                    return "Shield";
                case 5:
                    return "Health";
            }
            return "Health";
            
        }
    }
}
