using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Spearhead
{
    class Bonus
    {
        string BonusString;
        Text BonusText;
        SpriteFont BonusFont;

        public Bonus(ContentManager content, string bonusFont, string bonusString)
        {
            BonusFont = content.Load<SpriteFont>(bonusFont);
            BonusString = "WAVE BONUS: " + bonusString;
            BonusText = new Text(BonusFont, BonusString, new Vector2(480, 800), Color.White, Text.Alignment.Both, new Rectangle(0, 0, 480, 800));
        }

        public void Draw(SpriteBatch batch)
        {
            BonusText.Draw(batch);
        }
    }
}
