#region File Description
//-----------------------------------------------------------------------------
// ScrollText.cs
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
    /// This class provides scrolling text (or other image)
    /// </summary>
    class ScrollText : Sprite
    {

        #region Fields
        ContentManager Content; // The content manager
        public Texture2D Texture; // The image of the scrolling text
        public float ScrollSpeed; // The speed at which the text scrolls
        public Vector2 StartPosition; // Where the text should start
        #endregion

        public ScrollText(Vector2 startPosition, ContentManager content, string texture, float scrollSpeed)
            : base(content, texture)
        {
            Content = content;
            Texture = content.Load<Texture2D>(texture);
            Position = startPosition;
            StartPosition = startPosition;
            ScrollSpeed = scrollSpeed;
        }

        protected override void UpdateSprite(GameTime gameTime)
        {
            Position.Y -= ScrollSpeed;
        }

        public override void DrawSprite(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Position, Color.White);
        }
    }
}
