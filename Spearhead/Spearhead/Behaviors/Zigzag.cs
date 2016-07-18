using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Spearhead
{
    class Zigzag : Formation
    {
        string FireType;

        public Zigzag(ContentManager content, GameTime gameTime, string enemyType, int bonusValue, float speedMod, string fireType)
            : base(content, gameTime, enemyType, bonusValue, speedMod)
        {
            Content = content;
            GameTime = gameTime;
            EnemyType = enemyType;
            BonusValue = bonusValue;
            SpeedMod = speedMod;
            shipCount = 3;
            totalShips = shipCount;
            xPos = 0;
            xPos1 = 0;
            xPos2 = 0;
            yPos = 0;
            enemies = new List<Enemy>();
            Killed = 0;
            FireType = fireType;
        }

        public override void Reset()
        {
            CompleteWave = false;
            shipCount = 3;
            totalShips = shipCount;
            xPos = 0;
            xPos1 = 0;
            xPos2 = 0;
            yPos = 0;
            enemies = new List<Enemy>();
            Killed = 0;
        }

        public override void Update(GameTime gameTime)
        {
            if (shipCount == 3)
            {
                AddEnemies();
            }
            UpdateEnemies(gameTime);
        }

        public void AddEnemies()
        {
            int width;
            int height;
            if (EnemyType == "Rhino")
            {
                width = 53;
                height = 60;
                xPos = 240 - width / 2;
                xPos1 = xPos - width * 3;
                xPos2 = xPos + width * 3;
                yPos = 0 - height;
                Enemy enemy = new Rhino(new Vector2(xPos, yPos), Content, "Zigzag", FireType, 0.3f, 1f, "Images/rhino-shot");
                Enemy enemy1 = new Rhino(new Vector2(xPos1, yPos), Content, "Zigzag", FireType, 0.3f, 1f, "Images/rhino-shot");
                Enemy enemy2 = new Rhino(new Vector2(xPos2, yPos), Content, "Zigzag", FireType, 0.3f, 1f, "Images/rhino-shot");
                enemies.Add(enemy);
                enemies.Add(enemy1);
                enemies.Add(enemy2);
                shipCount = 0;
            }
            if (EnemyType == "Aphid")
            {
                width = 45;
                height = 26;
                xPos = 240 - width / 2;
                xPos1 = xPos - width * 3;
                xPos2 = xPos + width * 3;
                yPos = 0 - height;
                Enemy enemy = new Aphid(new Vector2(xPos, yPos), Content, "Zigzag", FireType, 0.2f, 0.75f, "Images/aphid-shot");
                Enemy enemy1 = new Aphid(new Vector2(xPos1, yPos), Content, "Zigzag", FireType, 0.2f, 0.75f, "Images/aphid-shot");
                Enemy enemy2 = new Aphid(new Vector2(xPos2, yPos), Content, "Zigzag", FireType, 0.2f, 0.75f, "Images/aphid-shot");
                enemies.Add(enemy);
                enemies.Add(enemy1);
                enemies.Add(enemy2);
                shipCount = 0;
            }
            if (EnemyType == "Hawk")
            {
                width = 52;
                height = 52;
                xPos = 240 - width / 2;
                xPos1 = xPos - width * 3;
                xPos2 = xPos + width * 3;
                yPos = 0 - height;
                Enemy enemy = new Hawk(new Vector2(xPos, yPos), Content, "Zigzag", FireType, 0.4f, 1.5f, "Images/hawk-shot");
                Enemy enemy1 = new Hawk(new Vector2(xPos1, yPos), Content, "Zigzag", FireType, 0.4f, 1.5f, "Images/hawk-shot");
                Enemy enemy2 = new Hawk(new Vector2(xPos2, yPos), Content, "Zigzag", FireType, 0.4f, 1.5f, "Images/hawk-shot");
                enemies.Add(enemy);
                enemies.Add(enemy1);
                enemies.Add(enemy2);
                shipCount = 0;
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
                CompleteWave = true;
            }
        }
    }
}
