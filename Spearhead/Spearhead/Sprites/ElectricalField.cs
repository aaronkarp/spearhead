using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Spearhead
{
    class ElectricalField
    {
        Texture2D ElectricalFieldTexture; // The texture/spritestrip of the electrical field
        public Animation ElectricalFieldAnimation; // The animation of the electrical field
        Boss Boss; // The boss currently using the electrical field attack
        public int Damage; // The damage done by the electrical attack
        public bool Active; // Whether the attack is currently active or not
        Vector2 Offset; // The offset from the boss's origin
        public string ImpactType; // The impact animation to use when the player is hit by the electrical field

        public ElectricalField(Boss boss, Vector2 offset, ContentManager content, string texturePath, string impactType)
        {
            Boss = boss;
            Offset = offset;
            Active = true;
            Damage = 5;
            ElectricalFieldTexture = content.Load<Texture2D>("Images/level-2-boss-electrical-field-spritesheet");
            ElectricalFieldAnimation = new Animation(ElectricalFieldTexture, Boss.Position + Offset, ElectricalFieldTexture.Width / 3, ElectricalFieldTexture.Height, 3, 70, Color.White, 1f, true);
            ImpactType = "small";
        }

        public void UpdateElectricalField(GameTime gameTime)
        {
            ElectricalFieldAnimation.animPosition = Boss.Position + Offset;
            ElectricalFieldAnimation.Update(gameTime);
        }

        public void DrawElectricalField(SpriteBatch spriteBatch)
        {
            ElectricalFieldAnimation.Draw(spriteBatch);
        }
    }
}
