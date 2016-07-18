#region File Description
//-----------------------------------------------------------------------------
// Formation.cs
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
using System.Collections.Generic;
#endregion

namespace Spearhead
{
    /// <summary>
    /// This is the basic formation class, which all formations extend.
    /// It doesn't really do anything on its own.
    /// </summary>
    class Formation
    {
        #region Fields
        public ContentManager Content;
        public GameTime GameTime;
        public string EnemyType;
        public int BonusValue;
        public bool CompleteWave;
        public float SpeedMod;
        public TimeSpan previousSpawnTime;
        public TimeSpan enemySpawnTime;
        public List<Enemy> enemies;
        public int shipCount;
        public int totalShips;
        public int xPos;
        public int xPos1;
        public int xPos2;
        public int yPos;
        public int Killed;

        public Random Random
        {
            get { return random; }
        }

        Random random = new Random();

        #endregion

        public Formation(ContentManager content, GameTime gameTime, string enemyType, int bonusValue, float speedMod)
        {
            Content = content;
            GameTime = gameTime;
            EnemyType = enemyType;
            BonusValue = bonusValue;
            SpeedMod = speedMod;
            CompleteWave = false;
        }

        public virtual void Update(GameTime gameTime)
        {
        }

        public virtual void Reset()
        {
        }
    }
}