using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Spearhead
{
    class Aphid : Enemy
    {
        public Aphid(Vector2 startPosition, ContentManager content, string formationType, string fireType, float fireRate, float pauseRate, string projectile)
            : base(startPosition, content, "Images/Aphid", 7.0f, 10, 3, 10, 250, formationType, fireType, fireRate, pauseRate, projectile)
        {
            shipType = "Aphid";
            offset1 = new Vector2(20, 36);
            offset2 = new Vector2(0,0);
            ProjectileVerticalSpeed = 20;
            ProjectileLateralSpeed = 0;
            ProjectileLateralDistance = 0;
            ProjectileUninterrupted = false;
        }
    }
}
