using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Spearhead
{
    class SweepUp : Formation
    {
        string direction;
        string FireType;

        public SweepUp(ContentManager content, GameTime gameTime, string enemyType, int bonusValue, float speedMod, string fireType)
            : base(content, gameTime, enemyType, bonusValue, speedMod)
        {
            Content = content;
            GameTime = gameTime;
            EnemyType = enemyType;
            BonusValue = bonusValue;
            CompleteWave = false;
            SpeedMod = speedMod;
            previousSpawnTime = TimeSpan.Zero;
            enemySpawnTime = TimeSpan.FromSeconds(2.5f);
            shipCount = 10;
            totalShips = shipCount;
            enemies = new List<Enemy>();
            Killed = 0;
            direction = "Right";
            FireType = fireType;
        }

        public override void Reset()
        {
            CompleteWave = false;
            previousSpawnTime = TimeSpan.Zero;
            shipCount = 10;
            totalShips = shipCount;
            enemies = new List<Enemy>();
            Killed = 0;
            direction = "Right";
        }

        public override void Update(GameTime gameTime)
        {
            if (shipCount > 0)
            {
                if (gameTime.TotalGameTime - previousSpawnTime > enemySpawnTime)
                {
                    previousSpawnTime = gameTime.TotalGameTime;
                    AddEnemy();
                }
            }
            UpdateEnemies(gameTime);
        }

        public void AddEnemy()
        {
            Enemy enemy;
            if (direction == "Right")
            {
                xPos = -60;
                direction = "Left";
            }
            else
            {
                xPos = 490;
                direction = "Right";
            }
            yPos = Random.Next(0, 75);
            Vector2 position = new Vector2(xPos, yPos);

            if (EnemyType == "Rhino")
            {
                enemy = new Rhino(position, Content, "SweepUp", FireType, 0.3f, 1f, "Images/rhino-shot");
                enemies.Add(enemy);
            }

            if (EnemyType == "Aphid")
            {
                enemy = new Aphid(position, Content, "SweepUp", FireType, 0.2f, 0.75f, "Images/aphid-shot");
                enemies.Add(enemy);
            }

            if (EnemyType == "Hawk")
            {
                enemy = new Hawk(position, Content, "SweepUp", FireType, 0.4f, 1.5f, "Images/hawk-shot");
                enemies.Add(enemy);
            }

            shipCount--;
        }

        public void UpdateEnemies(GameTime gameTime)
        {
            if (enemies.Count > 0)
            {
                for (int i = enemies.Count - 1; i >= 0; i--)
                {
                    enemies[i].Update(gameTime);

                    if (enemies[i].Active == false)
                    {
                        enemies.RemoveAt(i);
                    }
                }
            }
            else
            {
                if (shipCount == 0)
                    CompleteWave = true;
            }
        }
    }
}
