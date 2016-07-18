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
    class MissilePowerup : Powerup
    {

        #region Fields
        Rectangle sourceRect;
        #endregion

        public MissilePowerup(Vector2 startPosition, ContentManager content)
            : base(startPosition, content, "Images/Powerups")
        {
            TextureOffset = 54;
            sourceRect = new Rectangle(0, TextureOffset, Texture.Width, Texture.Height / 5);
        }

        public override void Activate(ScreenManager screenManager, PlayerShip playerShip)
        {
            screenManager.CurrentSecondary = "Missile";
            playerShip.SecondaryWeaponUp = "Missile";
            playerShip.SecondaryProjectileTexture = "Images/missile";
            playerShip.SecondaryProjectileDirection = "Left";
            playerShip.SecondaryProjectileUninterrupted = false;
            playerShip.SecondaryProjectileVerticalSpeed = 5;
            playerShip.SecondaryProjectileLateralSpeed = 10;
            playerShip.SecondaryProjectileDamage = 15;
            playerShip.SecondaryProjectile1Offset = 2;
            playerShip.SecondaryProjectile2Offset = -4;
            playerShip.SecondaryProjectileVerticalOffset = 30;
            playerShip.SecondaryFireTime = TimeSpan.FromSeconds(.25f);
            playerShip.SecondaryProjectileAlternate = true;
            playerShip.SecondaryLateralDistance = 80;
            playerShip.SecondaryProjectileImpactType = "large";
        }

        public override void DrawSprite(SpriteBatch batch)
        {
            batch.Draw(Texture, Position, sourceRect, Color.White);
        }
    }
}