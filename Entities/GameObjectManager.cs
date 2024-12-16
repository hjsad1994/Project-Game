using Project_Game.Entities;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Project_Game
{
    public class GameObjectManager
    {
        public List<StaticObject> StaticObjects { get; private set; }
        public List<AnimatedObject> AnimatedObjects { get; private set; }
        public List<TestEnemy> Enemies { get; private set; }
        public List<Chicken> Chickens { get; private set; }
        public List<Kapybara> Kapybaras { get; private set; }
        public List<GameObject> Obstacles { get; private set; }

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
            StaticObjects = new List<StaticObject>();
        }

        public void SetGameLogic(GameLogic logic)
        {
            this.gameLogic = logic;
        }

        public void LoadMap1()
        {
            // Xóa dữ liệu cũ
            AnimatedObjects.Clear();
            Enemies.Clear();
            Chickens.Clear();
            Kapybaras.Clear();
            StaticObjects.Clear();

            // Thêm gà thủ công để test
            Chickens.Add(new Chicken("Chicken1", 150, 200, 100, 200));

            // Thêm nhà thủ công
        //    StaticObjects.Add(new House("House_3_1.png", 300, 200, 100, 100));

            // Load objects từ XML
            var loadedObjects = ObjectLoader.LoadObjectsFromXml("Assets/MapData/map1_objects.xml");
            StaticObjects.AddRange(loadedObjects);

            // Spawn quái kiểu cũ
         //   Enemies.AddRange(TestEnemy.CreateEnemies("Assets/Enemies/Skeleton_Swordman", 3, 700, 150));

            // Debug số lượng static object
            Console.WriteLine($"After loading map1: StaticObjects={StaticObjects.Count}, Enemies={Enemies.Count}, Chickens={Chickens.Count}");

            // Cập nhật danh sách enemies trong gameLogic
            gameLogic.SetEnemies(Enemies.Cast<Enemy>().ToList());
        }

        public void LoadMap2()
        {
            AnimatedObjects.Clear();
            Enemies.Clear();
            Chickens.Clear();
            Kapybaras.Clear();
            StaticObjects.Clear();

            var loadedObjects = ObjectLoader.LoadObjectsFromXml("Assets/MapData/map2_objects.xml");
            StaticObjects.AddRange(loadedObjects);

            gameLogic.SetEnemies(Enemies.Cast<Enemy>().ToList());
        }

        public void LoadMap3()
        {
            AnimatedObjects.Clear();
            Enemies.Clear();
            Chickens.Clear();
            Kapybaras.Clear();
            StaticObjects.Clear();

            // var loadedObjects = ObjectLoader.LoadObjectsFromXml("Assets/MapData/map3_objects.xml");
            // StaticObjects.AddRange(loadedObjects);

            gameLogic.SetEnemies(Enemies.Cast<Enemy>().ToList());
        }

        public void LoadMap4()
        {
            AnimatedObjects.Clear();
            Enemies.Clear();
            Chickens.Clear();
            Kapybaras.Clear();
            StaticObjects.Clear();

            // var loadedObjects = ObjectLoader.LoadObjectsFromXml("Assets/MapData/map4_objects.xml");
            // StaticObjects.AddRange(loadedObjects);

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
