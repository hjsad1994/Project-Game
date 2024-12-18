using Project_Game.Entities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Project_Game.Entities
{
    public class MapManager
    {
        private Image bgMap1;
        private Image bgMap2;
        private Image bgMap3;
        private Image bgMap4;
        private Image currentBg;

        private int currentMap = 1;

        private Rectangle map1ToMap2Zone = new Rectangle(780, 270, 10, 35);
        private Rectangle map2ToMap1Zone = new Rectangle(0, 310, 2, 32);
        private Rectangle map1ToMap3Zone = new Rectangle(375, 590, 50, 5);
        private Rectangle map3ToMap1Zone = new Rectangle(430, 100, 50, 5);
        private Rectangle map1ToMap4Zone = new Rectangle(0, 215, 5, 40);
        private Rectangle map4ToMap1Zone = new Rectangle(780, 295, 10, 40);

        private bool isTransitioning = false;

        private GameObjectManager objectManager;
        private Player player;

        public MapManager(Player player, GameObjectManager objectManager)
        {
            this.player = player;
            this.objectManager = objectManager;
            LoadMaps();
            currentBg = bgMap4; // default start map
        }

        private void LoadMaps()
        {
            try
            {
                bgMap1 = Image.FromFile("base.png");
                bgMap2 = Image.FromFile("farm2.png");
                bgMap3 = Image.FromFile("dungeon.png");
                bgMap4 = Image.FromFile("farm.png");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading map images: {ex.Message}");
                Environment.Exit(1);
            }
        }

        public Image CurrentBg => currentBg;

        public void UpdateMap()
        {
            if (isTransitioning) return;

            Rectangle playerRect = new Rectangle(player.playerX, player.playerY, player.playerWidth, player.playerHeight);
        //    Console.WriteLine($"Player Rect: {playerRect}");

            if (currentMap == 1 && map1ToMap2Zone.IntersectsWith(playerRect))
            {
                isTransitioning = true;
                SwitchMap(2, new System.Drawing.Point(30, 310));
            }
            else if (currentMap == 2 && map2ToMap1Zone.IntersectsWith(playerRect))
            {
                isTransitioning = true;
                SwitchMap(1, new System.Drawing.Point(740, 270));
            }
            else if (currentMap == 1 && map1ToMap3Zone.IntersectsWith(playerRect))
            {
                isTransitioning = true;
                SwitchMap(3, new System.Drawing.Point(435, 120));
            }
            else if (currentMap == 3 && map3ToMap1Zone.IntersectsWith(playerRect))
            {
                isTransitioning = true;
                SwitchMap(1, new System.Drawing.Point(385, 550));
            }
            else if (currentMap == 1 && map1ToMap4Zone.IntersectsWith(playerRect))
            {
                isTransitioning = true;
                SwitchMap(4, new System.Drawing.Point(752, 292));
            }
            else if (currentMap == 4 && map4ToMap1Zone.IntersectsWith(playerRect))
            {
                isTransitioning = true;
                SwitchMap(1, new System.Drawing.Point(50, 220));
            }
        }

        //public void SwitchMap(int newMap, Point targetPosition)
        //{
        //    currentMap = newMap;
        //    Console.WriteLine($"Switching to Map {newMap}.");

        //    if (currentMap == 1)
        //    {
        //        currentBg = bgMap1;
        //        objectManager.LoadMap1();
        //    }
        //    else if (currentMap == 2)
        //    {
        //        currentBg = bgMap2;
        //        objectManager.LoadMap2();
        //    }
        //    else if (currentMap == 3)
        //    {
        //        currentBg = bgMap3;
        //        objectManager.LoadMap3();
        //    }
        //    else if (currentMap == 4)
        //    {
        //        currentBg = bgMap4;
        //        objectManager.LoadMap4();
        //    }

        //    player.playerX = targetPosition.X;
        //    player.playerY = targetPosition.Y;
        //    Console.WriteLine($"Player positioned at Map {currentMap} entrance: ({player.playerX}, {player.playerY})");

        //    isTransitioning = false;
        //}
        public void SwitchMap(int newMap, Point targetPosition)
        {
            currentMap = newMap;
            Console.WriteLine($"Switching to Map {newMap}.");

            if (currentMap == 1)
            {
                currentBg = bgMap1;
                objectManager.LoadMap1();
            }
            else if (currentMap == 2)
            {
                currentBg = bgMap2;
                objectManager.LoadMap2();
            }
            else if (currentMap == 3)
            {
                currentBg = bgMap3;
                objectManager.LoadMap3();
            }
            else if (currentMap == 4)
            {
                currentBg = bgMap4;
                objectManager.LoadMap4();
            }

            player.playerX = targetPosition.X;
            player.playerY = targetPosition.Y;
            Console.WriteLine($"Player positioned at Map {currentMap} entrance: ({player.playerX}, {player.playerY})");

            isTransitioning = false;
        }

        public int CurrentMap => currentMap;

        public void ResetMap()
        {
            // Khi reset game, có thể reset lại map về map 1
            currentMap = 1;
            currentBg = bgMap1;
        }
    }
}
