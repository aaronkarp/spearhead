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
    class HealthPowerup : Powerup
    {

        #region Fields
        Rectangle sourceRect;
        #endregion

        public HealthPowerup(Vector2 startPosition, ContentManager content)
            : base(startPosition, content, "Images/Powerups")
        {
            TextureOffset = 108;
            sourceRect = new Rectangle(0, TextureOffset, Texture.Width, Texture.Height / 5);
        }

        public override void Activate(ScreenManager screenManager, PlayerShip playerShip)
        {
            playerShip.Health += 25;
            if (playerShip.Health > 100)
                playerShip.Health = 100;
        }

        public override void DrawSprite(SpriteBatch batch)
        {
            batch.Draw(Texture, Position, sourceRect, color);
        }
    }
}