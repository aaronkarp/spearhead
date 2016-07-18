#region File Description
//-----------------------------------------------------------------------------
// ShieldPowerup.cs
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
    class ShieldPowerup : Powerup
    {

        #region Fields
        Rectangle sourceRect;
        ContentManager Content;
        #endregion

        public ShieldPowerup(Vector2 startPosition, ContentManager content)
            : base(startPosition, content, "Images/Powerups")
        {
            TextureOffset = 81;
            sourceRect = new Rectangle(0, TextureOffset, Texture.Width, Texture.Height / 5);
            Content = content;
        }

        public override void Activate(ScreenManager screenManager, PlayerShip playerShip)
        {
            screenManager.CurrentShield = "Basic";
            playerShip.ShieldUp = "Basic";
            playerShip.ShieldStrength = 3;
            playerShip.Shield = new Shield(playerShip, Content, "Basic", 3);
        }

        public override void DrawSprite(SpriteBatch batch)
        {
            batch.Draw(Texture, Position, sourceRect, Color.White);
        }
    }
}