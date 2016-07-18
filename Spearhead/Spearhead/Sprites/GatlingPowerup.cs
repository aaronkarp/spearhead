#region File Description
//-----------------------------------------------------------------------------
// GatlingPowerup.cs
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
    class GatlingPowerup : Powerup
    {

        #region Fields
        Rectangle sourceRect;
        #endregion

        public GatlingPowerup(Vector2 startPosition, ContentManager content)
            : base(startPosition, content, "Images/Powerups")
        {
            TextureOffset = 0;
            sourceRect = new Rectangle(0, TextureOffset, Texture.Width, Texture.Height / 5);
        }

        public override void Activate(ScreenManager screenManager, PlayerShip playerShip)
        {
            screenManager.CurrentWeapon = "Gatling";
            playerShip.ProjectileTexture = "Images/gatling-round";
            playerShip.ProjectileUninterrupted = false;
            playerShip.ProjectileVerticalSpeed = 20;
            playerShip.ProjectileLateralSpeed = 0;
            playerShip.ProjectileDamage = 5;
            playerShip.Projectile1Offset = 2;
            playerShip.Projectile2Offset = -4;
            playerShip.FireTime = TimeSpan.FromSeconds(.09f);
            playerShip.ProjectileImpactType = "small";
        }

        public override void DrawSprite(SpriteBatch batch)
        {
            batch.Draw(Texture, Position, sourceRect, color);
        }
    }
}