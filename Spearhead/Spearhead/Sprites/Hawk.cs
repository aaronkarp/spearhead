using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Spearhead
{
    class Hawk : Enemy
    {
        public Hawk(Vector2 startPosition, ContentManager content, string formationType, string fireType, float fireRate, float pauseRate, string projectile)
            : base(startPosition, content, "Images/Hawk", 5f, 20, 10, 20, 500, formationType, fireType, fireRate, pauseRate, projectile)
        {
            shipType = "Hawk";
            offset1 = new Vector2(13, 52);
            offset2 = new Vector2(0, 0);
            ProjectileVerticalSpeed = 20;
            ProjectileLateralSpeed = 0;
            ProjectileLateralDistance = 0;
            ProjectileUninterrupted = false;
        }
    }
}
