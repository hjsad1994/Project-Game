using Project_Game.Entities;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Drawing;
using System.IO;

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
        public List<DroppedItem> DroppedItems { get; private set; } // Thêm danh sách DroppedItem
        public List<Tree> Trees { get; private set; }

        private Player player;
        private GameLogic gameLogic;

        // Singleton pattern
        private static GameObjectManager _instance;
        public static GameObjectManager Instance
        {
            get
            {
                if (_instance == null)
                    throw new Exception("GameObjectManager chưa được khởi tạo. Vui lòng khởi tạo nó trước khi sử dụng.");
                return _instance;
            }
        }

        public GameObjectManager(Player player)
        {
            this.player = player;
            Obstacles = new List<GameObject>();
            AnimatedObjects = new List<AnimatedObject>();
            Enemies = new List<TestEnemy>();
            Chickens = new List<Chicken>();
            Kapybaras = new List<Kapybara>();
            StaticObjects = new List<StaticObject>();
            DroppedItems = new List<DroppedItem>();
            Trees = new List<Tree>(); // Khởi tạo danh sách Trees
            _instance = this; // Thiết lập instance cho Singleton
        }

        public void SetGameLogic(GameLogic logic)
        {
            this.gameLogic = logic;
        }

        // Phương thức để thêm DroppedItem vào danh sách
        public void AddDroppedItem(DroppedItem item)
        {
            DroppedItems.Add(item);
            // Obstacles.Add(item); // Nếu muốn DroppedItem cũng là chướng ngại vật
        }

        // ... Các phương thức LoadMapX hiện tại ...

        public void LoadMap1()
        {
            ClearAll();

            Console.WriteLine("Loading Map 1...");
            List<Tree> loadedTrees;
            List<Chicken> loadedChickens;
            List<AnimatedObject> loadedAnimatedObjects;
            List<Kapybara> loadedKapybaras; // Thêm danh sách Kapybara

            // Cập nhật phương thức gọi LoadObjectsFromXml với tham số out mới
            var loadedObjects = ObjectLoader.LoadObjectsFromXml(
                "Assets/MapData/map1_objects.xml",
                out loadedChickens,
                out loadedAnimatedObjects,
                out loadedTrees,
                out loadedKapybaras // Thêm tham số out Kapybaras
            );

            // Thêm kẻ thù
            Enemies.AddRange(TestEnemy.CreateEnemies("Assets/Enemies/Skeleton_Swordman", 1, 700, 150));

            // Thêm các đối tượng vào danh sách tương ứng
            StaticObjects.AddRange(loadedObjects);
            AnimatedObjects.AddRange(loadedAnimatedObjects);
            Chickens.AddRange(loadedChickens);
            Trees.AddRange(loadedTrees);
            Kapybaras.AddRange(loadedKapybaras); // Thêm Kapybaras vào danh sách

            // Thêm cây vào danh sách Obstacles nếu chúng là chướng ngại vật
            Obstacles.AddRange(loadedObjects);
            Obstacles.AddRange(loadedTrees); // Thêm tất cả các cây vào Obstacles

            Console.WriteLine($"Map1 Loaded - StaticObjects Count: {StaticObjects.Count}, AnimatedObjects Count: {AnimatedObjects.Count}, Obstacles Count: {Obstacles.Count}, Chickens Count: {Chickens.Count}, Trees Count: {Trees.Count}, Kapybaras Count: {Kapybaras.Count}");

            // Thêm thông tin chi tiết về từng House
            foreach (var house in StaticObjects.OfType<House>())
            {
                Console.WriteLine($"[Info] House tại ({house.X}, {house.Y}) size ({house.Width}x{house.Height})");
            }

            player.SetObstacles(Obstacles); // Cập nhật Obstacles cho Player

            gameLogic.SetEnemies(Enemies.Cast<Enemy>().ToList());
        }

        public void LoadMap2()
        {
            ClearAll();

            Console.WriteLine("Loading Map 2...");
            List<Tree> loadedTrees;
            List<Chicken> loadedChickens;
            List<AnimatedObject> loadedAnimatedObjects;
            List<Kapybara> loadedKapybaras; // Thêm danh sách Kapybara

            var loadedObjects = ObjectLoader.LoadObjectsFromXml(
                "Assets/MapData/map2_objects.xml",
                out loadedChickens,
                out loadedAnimatedObjects,
                out loadedTrees,
                out loadedKapybaras // Thêm tham số out Kapybaras
            );

            StaticObjects.AddRange(loadedObjects);
            AnimatedObjects.AddRange(loadedAnimatedObjects);
            Chickens.AddRange(loadedChickens);
            Trees.AddRange(loadedTrees);
            Kapybaras.AddRange(loadedKapybaras); // Thêm Kapybaras vào danh sách

            // Thêm StaticObjects vào Obstacles
            Obstacles.AddRange(StaticObjects);
            // Nếu map2 có Trees, đã thêm vào Obstacles trong LoadMap1, nhưng ở map2 có thể có Trees riêng

            Console.WriteLine($"Map2 Loaded - StaticObjects Count: {StaticObjects.Count}, AnimatedObjects Count: {AnimatedObjects.Count}, Obstacles Count: {Obstacles.Count}, Chickens Count: {Chickens.Count}, Trees Count: {Trees.Count}, Kapybaras Count: {Kapybaras.Count}");

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
            List<Tree> loadedTrees;
            List<Chicken> loadedChickens;
            List<AnimatedObject> loadedAnimatedObjects;
            List<Kapybara> loadedKapybaras; // Thêm danh sách Kapybara

            var loadedObjects = ObjectLoader.LoadObjectsFromXml(
                "Assets/MapData/map3_objects.xml",
                out loadedChickens,
                out loadedAnimatedObjects,
                out loadedTrees,
                out loadedKapybaras // Thêm tham số out Kapybaras
            );

            StaticObjects.AddRange(loadedObjects);
            AnimatedObjects.AddRange(loadedAnimatedObjects);
            Chickens.AddRange(loadedChickens);
            Trees.AddRange(loadedTrees);
            Kapybaras.AddRange(loadedKapybaras); // Thêm Kapybaras vào danh sách

            Enemies.AddRange(TestEnemy.CreateEnemies("Assets/Enemies/Skeleton_Swordman", 3, 600, 200));

            // Thêm StaticObjects và AnimatedObjects vào Obstacles nếu cần
            Obstacles.AddRange(StaticObjects);
            // Thêm Trees vào Obstacles
            Obstacles.AddRange(loadedTrees);

            Console.WriteLine($"Map3 Loaded - StaticObjects Count: {StaticObjects.Count}, AnimatedObjects Count: {AnimatedObjects.Count}, Obstacles Count: {Obstacles.Count}, Chickens Count: {Chickens.Count}, Trees Count: {Trees.Count}, Kapybaras Count: {Kapybaras.Count}");

            player.SetObstacles(Obstacles); // Cập nhật Obstacles cho Player

            gameLogic.SetEnemies(Enemies.Cast<Enemy>().ToList());
        }

        public void LoadMap4()
        {
            ClearAll();

            Console.WriteLine("Loading Map 4...");
            List<Tree> loadedTrees;
            List<Chicken> loadedChickens;
            List<AnimatedObject> loadedAnimatedObjects;
            List<Kapybara> loadedKapybaras; // Thêm danh sách Kapybara

            var loadedObjects = ObjectLoader.LoadObjectsFromXml(
                "Assets/MapData/map4_objects.xml",
                out loadedChickens,
                out loadedAnimatedObjects,
                out loadedTrees,
                out loadedKapybaras // Thêm tham số out Kapybaras
            );

            StaticObjects.AddRange(loadedObjects);
            AnimatedObjects.AddRange(loadedAnimatedObjects);
            Chickens.AddRange(loadedChickens);
            Trees.AddRange(loadedTrees);
            Kapybaras.AddRange(loadedKapybaras); // Thêm Kapybaras vào danh sách

            // Thêm StaticObjects và AnimatedObjects vào Obstacles nếu cần
            Obstacles.AddRange(StaticObjects);
            Obstacles.AddRange(loadedTrees);

            Console.WriteLine($"Map4 Loaded - StaticObjects Count: {StaticObjects.Count}, AnimatedObjects Count: {AnimatedObjects.Count}, Obstacles Count: {Obstacles.Count}, Chickens Count: {Chickens.Count}, Trees Count: {Trees.Count}, Kapybaras Count: {Kapybaras.Count}");

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
            DroppedItems.Clear(); // Xoá DroppedItems khi tải bản đồ mới
            Trees.Clear(); // Thêm dòng này để xoá danh sách Trees
            Console.WriteLine("[Info] Cleared all game objects.");
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

            foreach (var droppedItem in DroppedItems)
            {
                droppedItem.Update(player);
            }

            foreach (var tree in Trees)
            {
                tree.UpdateTree();
            }
        }

        public void RenderAll(Graphics g)
        {
            // Vẽ StaticObjects
            foreach (var staticObj in StaticObjects)
            {
                staticObj.Draw(g);
            }

            // Vẽ AnimatedObjects
            foreach (var animatedObj in AnimatedObjects)
            {
                animatedObj.Draw(g);
            }

            // Vẽ DroppedItems
            foreach (var droppedItem in DroppedItems)
            {
                droppedItem.Draw(g);
            }

            // Vẽ Enemies
            foreach (var enemy in Enemies)
            {
                enemy.Draw(g);
            }

            // Vẽ Kapybaras
            foreach (var kapy in Kapybaras)
            {
                kapy.Draw(g);
            }

            // Vẽ các đối tượng khác như Player
            // (Assuming bạn đã có một Renderer để vẽ các đối tượng khác)
        }
    }
}
