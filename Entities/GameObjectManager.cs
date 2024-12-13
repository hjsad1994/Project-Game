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

            // Khởi tạo Obstacle từ form
            // Lưu ý: Bạn phải truyền Test1, Test2 từ Form1 qua hoặc có cách load obstacles khác.
            // Tạm thời bạn sẽ load cứng hoặc nhận tham số.

            // Chỉ ví dụ: Sau này bạn có thể truyền form control
        }

        public void SetGameLogic(GameLogic logic)
        {
            this.gameLogic = logic;
        }

        // Load cho từng map
        public void LoadMap1()
        {
            AnimatedObjects.Clear();
            Enemies.Clear();
            Chickens.Clear();

            // Thêm animated objects map 1
            //AnimatedObjects.Add(new AnimatedObject("Flower_Grass_12_Anim_cuts", 100, 100, 50, 25, 8));
            //AnimatedObjects.Add(new AnimatedObject("Flower_Grass_3_Anim_cuts", 400, 250, 50, 25, 8));
            //AnimatedObjects.Add(new AnimatedObject("Flower_Grass_3_Anim_cuts", 500, 250, 50, 25, 8));
            //AnimatedObjects.Add(new AnimatedObject("Flower_Grass_3_Anim_cuts", 600, 250, 50, 25, 8));

            // Add chickens, enemies nếu cần
            Chickens.Add(new Chicken("Chicken1", 600, 300, 100, 200));
            Chickens.Add(new Chicken("Chicken2", 400, 200, 350, 450));

            gameLogic.SetEnemies(Enemies.Cast<Enemy>().ToList());
        }

        public void LoadMap2()
        {
            AnimatedObjects.Clear();
            Enemies.Clear();
            Chickens.Clear();

          //  AnimatedObjects.Add(new AnimatedObject("Flower_Grass_12_Anim_cuts", 150, 120, 30, 30, 10));
//AnimatedObjects.Add(new AnimatedObject("Flower_Grass_3_Anim_cuts", 500, 300, 30, 30, 10));

            gameLogic.SetEnemies(Enemies.Cast<Enemy>().ToList());
        }

        public void LoadMap3()
        {
            AnimatedObjects.Clear();
            Enemies.Clear();
            Chickens.Clear();

            AnimatedObjects.Add(new AnimatedObject("Flower_Grass_12_Anim_cuts", 200, 200, 40, 40, 12));
            AnimatedObjects.Add(new AnimatedObject("Flower_Grass_3_Anim_cuts", 600, 350, 40, 40, 12));

            // Thêm enemies
            Enemies = TestEnemy.CreateEnemies("Enemy/Skeleton_Swordman", 3, 700, 150);

            gameLogic.SetEnemies(Enemies.Cast<Enemy>().ToList());
        }

        public void LoadMap4()
        {
            AnimatedObjects.Clear();
            Enemies.Clear();
            Chickens.Clear();

            AnimatedObjects.Add(new AnimatedObject("Flower_Grass_12_Anim_cuts", 250, 250, 35, 35, 9));
            AnimatedObjects.Add(new AnimatedObject("Flower_Grass_3_Anim_cuts", 650, 400, 35, 35, 9));

            gameLogic.SetEnemies(Enemies.Cast<Enemy>().ToList());
        }

        public void UpdateAll(Player player)
        {
            // Update logic cho enemies, chicken, animated object
            foreach (var enemy in Enemies)
            {
                enemy.Update(Obstacles, player);
            }

            foreach (var chicken in Chickens)
            {
                chicken.Update(player);
            }

            foreach (var animatedObj in AnimatedObjects)
            {
                animatedObj.Update();
            }
        }
    }
}
