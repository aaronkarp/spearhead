#region File Description
//-----------------------------------------------------------------------------
// LaserPowerup.cs
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
    class LaserPowerup : Powerup
    {

        #region Fields
        Rectangle sourceRect;
        #endregion

        public LaserPowerup(Vector2 startPosition, ContentManager content)
            : base(startPosition, content, "Images/Powerups")
        {
            TextureOffset = 27;
            sourceRect = new Rectangle(0, TextureOffset, Texture.Width, Texture.Height / 5);
        }

        public override void Activate(ScreenManager screenManager, PlayerShip playerShip)
        {
            screenManager.CurrentWeapon = "Laser";
            playerShip.ProjectileTexture = "Images/laser-round";
            playerShip.ProjectileUninterrupted = true;
            playerShip.ProjectileVerticalSpeed = 30;
            playerShip.ProjectileLateralSpeed = 0;
            playerShip.ProjectileDamage = 10;
            playerShip.Projectile1Offset = 22;
            playerShip.Projectile2Offset = 600;
            playerShip.FireTime = TimeSpan.FromSeconds(.33f);
            playerShip.ProjectileImpactType = "small";
        }

        public override void DrawSprite(SpriteBatch batch)
        {
            batch.Draw(Texture, Position, sourceRect, color);
        }
    }
}