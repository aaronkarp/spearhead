using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Spearhead
{
    class Rhino : Enemy
    {
        public Rhino(Vector2 startPosition, ContentManager content, string formationType, string fireType, float fireRate, float pauseRate, string projectile)
            : base(startPosition, content, "Images/Rhino", 4.0f, 15, 5, 10, 100, formationType, fireType, fireRate, pauseRate, projectile)
        {
            shipType = "Rhino";
            offset1 = new Vector2(10, 62);
            offset2 = new Vector2(35, 62);
            ProjectileVerticalSpeed = 20;
            ProjectileLateralSpeed = 0;
            ProjectileLateralDistance = 0;
            ProjectileUninterrupted = false;
        }
    }
}
