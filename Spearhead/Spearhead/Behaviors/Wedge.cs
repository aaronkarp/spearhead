using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Spearhead
{
    class Wedge : Formation
    {
        string FireType;

        public Wedge(ContentManager content, GameTime gameTime, string enemyType, int bonusValue, float speedMod, string fireType)
            : base(content, gameTime, enemyType, bonusValue, speedMod)
        {
            Content = content;
            GameTime = gameTime;
            EnemyType = enemyType;
            BonusValue = bonusValue;
            CompleteWave = false;
            SpeedMod = speedMod;
            previousSpawnTime = TimeSpan.Zero;
            enemySpawnTime = TimeSpan.FromSeconds(0.2f);
            shipCount = 5;
            totalShips = shipCount;
            xPos = 214;
            xPos1 = 214;
            xPos2 = 214;
            yPos = 0;
            enemies = new List<Enemy>();
            Killed = 0;
            FireType = fireType;
        }

        public override void Reset()
        {
            CompleteWave = false;
            previousSpawnTime = TimeSpan.Zero;
            shipCount = 5;
            totalShips = shipCount;
            xPos = 214;
            xPos1 = 214;
            xPos2 = 214;
            yPos = 0;
            enemies = new List<Enemy>();
            Killed = 0;
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
            Enemy enemy1;
            Enemy enemy2;
            if (shipCount == 5)
            {
                Vector2 position = new Vector2(xPos, yPos);
                if (EnemyType == "Rhino")
                {
                    enemy = new Rhino(position, Content, "Wedge", FireType, 0.3f, 1f, "Images/rhino-shot");
                    enemies.Add(enemy);
                }
                if (EnemyType == "Aphid")
                {
                    enemy = new Aphid(position, Content, "Wedge", FireType, 0.2f, 0.75f, "Images/aphid-shot");
                    enemies.Add(enemy);
                }
                if (EnemyType == "Hawk")
                {
                    enemy = new Hawk(position, Content, "Wedge", FireType, 0.4f, 1.5f, "Images/hawk-shot");
                    enemies.Add(enemy);
                }
                shipCount--;
            }
            else
            {
                xPos1 -= 85;
                xPos2 += 85;
                if (EnemyType == "Rhino")
                {
                    enemy1 = new Rhino(new Vector2(xPos1, yPos), Content, "Wedge", FireType, 0.3f, 1f, "Images/rhino-shot");
                    enemy2 = new Rhino(new Vector2(xPos2, yPos), Content, "Wedge", FireType, 0.3f, 1f, "Images/rhino-shot");
                    enemies.Add(enemy1);
                    enemies.Add(enemy2);
                }
                if (EnemyType == "Aphid")
                {
                    enemy1 = new Aphid(new Vector2(xPos1, yPos), Content, "Wedge", FireType, 0.2f, 0.75f, "Images/aphid-shot");
                    enemy2 = new Aphid(new Vector2(xPos2, yPos), Content, "Wedge", FireType, 0.2f, 0.75f, "Images/aphid-shot");
                    enemies.Add(enemy1);
                    enemies.Add(enemy2);
                }
                if (EnemyType == "Hawk")
                {
                    enemy1 = new Hawk(new Vector2(xPos1, yPos), Content, "Wedge", FireType, 0.4f, 1.5f, "Images/hawk-shot");
                    enemy2 = new Hawk(new Vector2(xPos2, yPos), Content, "Wedge", FireType, 0.4f, 1.5f, "Images/hawk-shot");
                    enemies.Add(enemy1);
                    enemies.Add(enemy2);
                }
                shipCount -= 2;
            }
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
