using Project_Game;
using Project_Game.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;

namespace Project_Game
{
    public class GameObjectManager
    {
        public List<TestEnemy> Enemies { get; private set; }
        public List<Chicken> Chickens { get; private set; }
        public List<Kapybara> Kapybaras { get; private set; }
        public List<GameObject> Obstacles { get; private set; }
        public List<AnimatedObject> AnimatedObjects { get; private set; }

        private Player player;
        private GameLogic gameLogic;

        public GameObjectManager(Player player)
        {
            this.player = player;
            Obstacles = new List<GameObject>();
            AnimatedObjects = new List<AnimatedObject>();
            Enemies = new List<TestEnemy>();
            Chickens = new List<Chicken>();
            Kapybaras = new List<Kapybara>();
        }

        public void SetGameLogic(GameLogic logic)
        {
            this.gameLogic = logic;
        }

        public void LoadMap1()
        {
            AnimatedObjects.Clear();
            Enemies.Clear();
            Chickens.Clear();
            Kapybaras.Clear();

            // Thêm 1 Kapybara
            Kapybaras.Add(new Kapybara("Kapybara1", 640, 450, 100, 200));

            // Thêm chickens
            Chickens.Add(new Chicken("Chicken1", 600, 300, 100, 200));
            Chickens.Add(new Chicken("Chicken2", 400, 200, 350, 450));

            gameLogic.SetEnemies(Enemies.Cast<Enemy>().ToList());
        }

        public void LoadMap2()
        {
            AnimatedObjects.Clear();
            Enemies.Clear();
            Chickens.Clear();
            Kapybaras.Clear();

            // Thêm logic cho Map2 nếu cần

            gameLogic.SetEnemies(Enemies.Cast<Enemy>().ToList());
        }

        public void LoadMap3()
        {
            AnimatedObjects.Clear();
            Enemies.Clear();
            Chickens.Clear();
            Kapybaras.Clear();

            AnimatedObjects.Add(new AnimatedObject("Flower_Grass_12_Anim_cuts", 200, 200, 40, 40, 12));
            AnimatedObjects.Add(new AnimatedObject("Flower_Grass_3_Anim_cuts", 600, 350, 40, 40, 12));
            Enemies = TestEnemy.CreateEnemies("Enemy/Skeleton_Swordman", 3, 700, 150);

            gameLogic.SetEnemies(Enemies.Cast<Enemy>().ToList());
        }

        public void LoadMap4()
        {
            AnimatedObjects.Clear();
            Enemies.Clear();
            Chickens.Clear();
            Kapybaras.Clear();

            AnimatedObjects.Add(new AnimatedObject("Flower_Grass_12_Anim_cuts", 250, 250, 35, 35, 9));
            AnimatedObjects.Add(new AnimatedObject("Flower_Grass_3_Anim_cuts", 650, 400, 35, 35, 9));

            gameLogic.SetEnemies(Enemies.Cast<Enemy>().ToList());
        }

        public void UpdateAll(Player player)
        {
            foreach (var enemy in Enemies)
            {
                enemy.Update(Obstacles, player);
            }

            foreach (var chicken in Chickens)
            {
                chicken.Update(player);
            }

            // Update Kapybaras
            foreach (var kapy in Kapybaras)
            {
                kapy.Update();
            }

            foreach (var animatedObj in AnimatedObjects)
            {
                animatedObj.Update();
            }
        }
    }
}
