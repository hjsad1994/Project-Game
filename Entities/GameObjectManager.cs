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
            ClearAll();

            Console.WriteLine("Loading Map 1...");

            List<Chicken> loadedChickens;
            List<AnimatedObject> loadedAnimatedObjects;
            var loadedObjects = ObjectLoader.LoadObjectsFromXml("Assets/MapData/map1_objects.xml", out loadedChickens, out loadedAnimatedObjects);
            Enemies.AddRange(TestEnemy.CreateEnemies("Assets/Enemies/Skeleton_Swordman", 3, 700, 150));

            StaticObjects.AddRange(loadedObjects);
            AnimatedObjects.AddRange(loadedAnimatedObjects);
            Chickens.AddRange(loadedChickens);

            Obstacles.AddRange(StaticObjects); // Nếu muốn house cản đường
          

            Console.WriteLine($"Map1 Loaded - StaticObjects Count: {StaticObjects.Count}, AnimatedObjects Count: {AnimatedObjects.Count}, Obstacles Count: {Obstacles.Count}, Chickens Count: {Chickens.Count}");

            player.SetObstacles(Obstacles); // Cập nhật Obstacles cho Player

            gameLogic.SetEnemies(Enemies.Cast<Enemy>().ToList());
        }

        public void LoadMap2()
        {
            ClearAll();

            Console.WriteLine("Loading Map 2...");

            // Nếu map2 có objects từ XML:
            // List<Chicken> loadedChickens;
            // List<AnimatedObject> loadedAnimatedObjects;
            // var loadedObjects = ObjectLoader.LoadObjectsFromXml("Assets/MapData/map2_objects.xml", out loadedChickens, out loadedAnimatedObjects);
            // StaticObjects.AddRange(loadedObjects);
            // AnimatedObjects.AddRange(loadedAnimatedObjects);
            // Chickens.AddRange(loadedChickens);

            // Nếu không có AnimatedObject trong map2, bạn có thể bỏ qua
            // Thêm StaticObjects vào Obstacles nếu cần (hiện tại StaticObjects đã được xóa nên Obstacles sẽ rỗng)
            Obstacles.AddRange(StaticObjects);
            // Nếu map2 không có AnimatedObject, không thêm vào Obstacles

            Console.WriteLine($"Map2 Loaded - StaticObjects Count: {StaticObjects.Count}, AnimatedObjects Count: {AnimatedObjects.Count}, Obstacles Count: {Obstacles.Count}, Chickens Count: {Chickens.Count}");

            player.SetObstacles(Obstacles); // Cập nhật Obstacles cho Player

            // Spawn quái cho map2 (nếu bạn muốn có quái ở map2)
            //Enemies.AddRange(TestEnemy.CreateEnemies("Assets/Enemies/Skeleton_Swordman", 3, 600, 200));

            // Cập nhật danh sách enemies trong gameLogic
            gameLogic.SetEnemies(Enemies.Cast<Enemy>().ToList());
        }

        public void LoadMap3()
        {
            ClearAll();

            Console.WriteLine("Loading Map 3...");

            // var loadedObjects = ObjectLoader.LoadObjectsFromXml("Assets/MapData/map3_objects.xml", out List<Chicken> loadedChickens, out List<AnimatedObject> loadedAnimatedObjects);
            // StaticObjects.AddRange(loadedObjects);
            // AnimatedObjects.AddRange(loadedAnimatedObjects);
            // Chickens.AddRange(loadedChickens);

            Enemies.AddRange(TestEnemy.CreateEnemies("Assets/Enemies/Skeleton_Swordman", 3, 600, 200));

            // Thêm StaticObjects và AnimatedObjects vào Obstacles nếu cần
            Obstacles.AddRange(StaticObjects);


            Console.WriteLine($"Map3 Loaded - StaticObjects Count: {StaticObjects.Count}, AnimatedObjects Count: {AnimatedObjects.Count}, Obstacles Count: {Obstacles.Count}, Chickens Count: {Chickens.Count}");

            player.SetObstacles(Obstacles); // Cập nhật Obstacles cho Player

            gameLogic.SetEnemies(Enemies.Cast<Enemy>().ToList());
        }

        public void LoadMap4()
        {
            ClearAll();

            Console.WriteLine("Loading Map 4...");

            // var loadedObjects = ObjectLoader.LoadObjectsFromXml("Assets/MapData/map4_objects.xml", out List<Chicken> loadedChickens, out List<AnimatedObject> loadedAnimatedObjects);
            // StaticObjects.AddRange(loadedObjects);
            // AnimatedObjects.AddRange(loadedAnimatedObjects);
            // Chickens.AddRange(loadedChickens);

            // Thêm StaticObjects và AnimatedObjects vào Obstacles nếu cần
            Obstacles.AddRange(StaticObjects);

            Console.WriteLine($"Map4 Loaded - StaticObjects Count: {StaticObjects.Count}, AnimatedObjects Count: {AnimatedObjects.Count}, Obstacles Count: {Obstacles.Count}, Chickens Count: {Chickens.Count}");

            player.SetObstacles(Obstacles); // Cập nhật Obstacles cho Player

            // Thêm quái cho map4 nếu cần
            //Enemies.AddRange(TestEnemy.CreateEnemies("Assets/Enemies/Skeleton_Swordman", 3, 600, 200));

            gameLogic.SetEnemies(Enemies.Cast<Enemy>().ToList());
        }

        /// <summary>
        /// Xoá tất cả các danh sách và reset lại.
        /// </summary>
        private void ClearAll()
        {
            AnimatedObjects.Clear();
            Enemies.Clear();
            Chickens.Clear();
            Kapybaras.Clear();
            StaticObjects.Clear();
            Obstacles.Clear();
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
