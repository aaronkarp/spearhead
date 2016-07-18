#region File Description
//-----------------------------------------------------------------------------
// Shield.cs
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
    class Shield : Sprite
    {

        #region Fields
        public int Health;
        int frameOffset;
        public Vector2 textureOffset;
        public string ShieldType;
        public bool Active;
        PlayerShip PlayerShip;
        Rectangle sourceRect;
        #endregion

        public Shield(PlayerShip playerShip, ContentManager content, string shieldType, int health)
            : base(content, "Images/BasicShield")
        {
            ShieldType = shieldType;
            Health = health;
            Active = true;
            PlayerShip = playerShip;

            if (ShieldType == "Basic")
            {
                texture = content.Load<Texture2D>("Images/BasicShield");
                textureOffset = new Vector2(5, 5);
                frameOffset = texture.Width - (Health * 58);
            }
        }

        protected override void UpdateSprite(GameTime gameTime)
        {
            if (ShieldType == "Basic")
            {
                frameOffset = texture.Width - (Health * 58);
            }
            if (Health <= 0)
            {
                Active = false;
            }
            sourceRect = new Rectangle(frameOffset, 0, texture.Width / 3, texture.Height);
        }

        public override void DrawSprite(SpriteBatch batch)
        {
            batch.End();
            batch.Begin();
            batch.Draw(texture, Position, sourceRect, Color.White);
        }
    }
}
