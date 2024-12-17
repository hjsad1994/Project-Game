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

        // Singleton pattern (nếu bạn đã triển khai)
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
            var loadedObjects = ObjectLoader.LoadObjectsFromXml("Assets/MapData/map1_objects.xml", out loadedChickens, out loadedAnimatedObjects, out loadedTrees);
            Enemies.AddRange(TestEnemy.CreateEnemies("Assets/Enemies/Skeleton_Swordman", 1, 700, 150));

            StaticObjects.AddRange(loadedObjects);
            AnimatedObjects.AddRange(loadedAnimatedObjects);
            Chickens.AddRange(loadedChickens);

            // Thêm cây vào danh sách Trees với vị trí cố định
            List<Image> treeStages = new List<Image>();
            for (int i = 1; i <= 3; i++)
            {
                string treeImagePath = Path.Combine("Assets", "Tree", "Spruce_tree", $"Spruce_tree_{i:00}.png");
                if (File.Exists(treeImagePath))
                {
                    try
                    {
                        Image treeImage = Image.FromFile(treeImagePath);
                        treeStages.Add(treeImage);
                        Console.WriteLine($"Loaded tree stage {i} from {treeImagePath}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error loading tree image {treeImagePath}: {ex.Message}");
                    }
                }
                else
                {
                    Console.WriteLine($"Tree image not found: {treeImagePath}");
                }
            }

            // Kiểm tra xem đã tải đủ 3 giai đoạn chưa
            if (treeStages.Count != 3)
            {
                Console.WriteLine($"Error: Expected 3 tree stages, but loaded {treeStages.Count}.");
                // Bạn có thể xử lý lỗi ở đây, chẳng hạn như bỏ qua việc thêm cây hoặc sử dụng hình ảnh mặc định
            }
            else
            {
                // Tạo cây tại vị trí cố định, ví dụ: (200, 300) với width=50 và height=100
                int treeWidth = 25; // Bạn có thể thay đổi giá trị này theo ý muốn
                int treeHeight = 50; // Bạn có thể thay đổi giá trị này theo ý muốn

                Tree tree1 = new Tree(300, 350, treeStages, treeWidth, treeHeight, growthIntervalMilliseconds: 5000); // 5 giây để phát triển
                Trees.Add(tree1);
                Obstacles.Add(tree1); // Nếu bạn muốn cây cũng là chướng ngại vật
                Console.WriteLine($"Added Tree at (200, 300) with size ({treeWidth}x{treeHeight})");

            }

            Obstacles.AddRange(StaticObjects); // Thêm StaticObjects vào Obstacles

            Console.WriteLine($"Map1 Loaded - StaticObjects Count: {StaticObjects.Count}, AnimatedObjects Count: {AnimatedObjects.Count}, Obstacles Count: {Obstacles.Count}, Chickens Count: {Chickens.Count}, Trees Count: {Trees.Count}");

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

            // Vẽ Enemies, Chickens, Kapybaras, Player, etc.
            // (Assuming bạn đã có một Renderer để vẽ các đối tượng khác)
        }
    }
}
