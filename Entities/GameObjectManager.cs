using Project_Game.Entities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Xml;

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
        public List<Ore> Ores { get; private set; } // Thêm danh sách Ores

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
            Ores = new List<Ore>(); // Khởi tạo danh sách Ores
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

        // Phương thức để thêm Ore vào danh sách
        public void AddOre(Ore ore)
        {
            Ores.Add(ore);
            Obstacles.Add(ore); // Thêm quặng vào danh sách vật cản để người chơi và kẻ thù tránh va chạm
            Console.WriteLine($"[Info] Thêm quặng tại ({ore.X}, {ore.Y}) vào trò chơi.");
        }

        public void RemoveOre(Ore ore)
        {
            if (Ores.Contains(ore))
            {
                Ores.Remove(ore);
                Obstacles.Remove(ore);
                Console.WriteLine($"[Info] Quặng tại ({ore.X}, {ore.Y}) đã được loại bỏ khỏi trò chơi.");

                // Cập nhật danh sách Obstacles cho Player
                player.SetObstacles(Obstacles);
            }
            else
            {
                Console.WriteLine($"[Warning] Cố gắng loại bỏ quặng không tồn tại tại ({ore.X}, {ore.Y}).");
            }
        }

        // ... Các phương thức LoadMapX hiện tại ...

        public void LoadMap1()
        {
            ClearAll();

            Console.WriteLine("Loading Map 1...");
            List<Tree> loadedTrees;
            List<Chicken> loadedChickens;
            List<AnimatedObject> loadedAnimatedObjects;
            List<Kapybara> loadedKapybaras;
            List<Ore> loadedOres; // Thêm danh sách Ores

            // Cập nhật phương thức gọi LoadObjectsFromXml với tham số out mới
            var loadedObjects = ObjectLoader.LoadObjectsFromXml(
                "Assets/MapData/map1_objects.xml",
                out loadedChickens,
                out loadedAnimatedObjects,
                out loadedTrees,
                out loadedKapybaras,
                out loadedOres // Thêm tham số out Ores
            );

            // Thêm kẻ thù
            Enemies.AddRange(TestEnemy.CreateEnemies("Assets/Enemies/Skeleton_Swordman", 1, 700, 150));

            // Thêm các đối tượng vào danh sách tương ứng
            StaticObjects.AddRange(loadedObjects);
            AnimatedObjects.AddRange(loadedAnimatedObjects);
            Chickens.AddRange(loadedChickens);
            Trees.AddRange(loadedTrees);
            Kapybaras.AddRange(loadedKapybaras);
            Ores.AddRange(loadedOres); // Thêm Ores từ XML

            // Thêm cây và các đối tượng static vào danh sách Obstacles
            Obstacles.AddRange(loadedObjects);
            Obstacles.AddRange(loadedTrees);
            Obstacles.AddRange(loadedOres); // Thêm Ores vào Obstacles

            Console.WriteLine($"Map1 Loaded - StaticObjects Count: {StaticObjects.Count}, AnimatedObjects Count: {AnimatedObjects.Count}, Obstacles Count: {Obstacles.Count}, Chickens Count: {Chickens.Count}, Trees Count: {Trees.Count}, Kapybaras Count: {Kapybaras.Count}, Ores Count: {Ores.Count}");

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
            List<Kapybara> loadedKapybaras;
            List<Ore> loadedOres; // Thêm danh sách Ores

            var loadedObjects = ObjectLoader.LoadObjectsFromXml(
                "Assets/MapData/map2_objects.xml",
                out loadedChickens,
                out loadedAnimatedObjects,
                out loadedTrees,
                out loadedKapybaras,
                out loadedOres // Thêm tham số out Ores
            );

            StaticObjects.AddRange(loadedObjects);
            AnimatedObjects.AddRange(loadedAnimatedObjects);
            Chickens.AddRange(loadedChickens);
            Trees.AddRange(loadedTrees);
            Kapybaras.AddRange(loadedKapybaras);
            Ores.AddRange(loadedOres); // Thêm Ores từ XML

            // Thêm quặng vào Map2
            // Nếu bạn đã khai báo Ores trong XML, không cần thêm thủ công nữa
            // Nhưng nếu muốn thêm thêm Ores, bạn có thể làm điều đó ở đây
            /*
            Ore ore3 = new Ore(250, 250, Path.Combine("Assets", "Items", "Ore", "ore.png"));
            Ore ore4 = new Ore(550, 450, Path.Combine("Assets", "Items", "Ore", "ore.png"));
            AddOre(ore3);
            AddOre(ore4);
            */

            // Thêm StaticObjects vào Obstacles
            Obstacles.AddRange(StaticObjects);
            Obstacles.AddRange(loadedTrees);
            Obstacles.AddRange(loadedOres); // Thêm Ores vào Obstacles

            Console.WriteLine($"Map2 Loaded - StaticObjects Count: {StaticObjects.Count}, AnimatedObjects Count: {AnimatedObjects.Count}, Obstacles Count: {Obstacles.Count}, Chickens Count: {Chickens.Count}, Trees Count: {Trees.Count}, Kapybaras Count: {Kapybaras.Count}, Ores Count: {Ores.Count}");

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
            List<Kapybara> loadedKapybaras;
            List<Ore> loadedOres; // Thêm danh sách Ores

            var loadedObjects = ObjectLoader.LoadObjectsFromXml(
                "Assets/MapData/map3_objects.xml",
                out loadedChickens,
                out loadedAnimatedObjects,
                out loadedTrees,
                out loadedKapybaras,
                out loadedOres // Thêm tham số out Ores
            );

            StaticObjects.AddRange(loadedObjects);
            AnimatedObjects.AddRange(loadedAnimatedObjects);
            Chickens.AddRange(loadedChickens);
            Trees.AddRange(loadedTrees);
            Kapybaras.AddRange(loadedKapybaras);
            Ores.AddRange(loadedOres); // Thêm Ores từ XML

            // Thêm quặng vào Map3
            // Nếu bạn đã khai báo Ores trong XML, không cần thêm thủ công nữa
            /*
            Ore ore5 = new Ore(400, 300, Path.Combine("Assets", "Items", "Ore", "ore.png"));
            AddOre(ore5);
            */

            // Thêm kẻ thù
            Enemies.AddRange(TestEnemy.CreateEnemies("Assets/Enemies/Skeleton_Swordman", 3, 600, 200));

            // Thêm StaticObjects và AnimatedObjects vào Obstacles nếu cần
            Obstacles.AddRange(StaticObjects);
            Obstacles.AddRange(loadedTrees);
            Obstacles.AddRange(loadedOres); // Thêm Ores vào Obstacles

            Console.WriteLine($"Map3 Loaded - StaticObjects Count: {StaticObjects.Count}, AnimatedObjects Count: {AnimatedObjects.Count}, Obstacles Count: {Obstacles.Count}, Chickens Count: {Chickens.Count}, Trees Count: {Trees.Count}, Kapybaras Count: {Kapybaras.Count}, Ores Count: {Ores.Count}");

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
            List<Kapybara> loadedKapybaras;
            List<Ore> loadedOres; // Thêm danh sách Ores

            var loadedObjects = ObjectLoader.LoadObjectsFromXml(
                "Assets/MapData/map4_objects.xml",
                out loadedChickens,
                out loadedAnimatedObjects,
                out loadedTrees,
                out loadedKapybaras,
                out loadedOres // Thêm tham số out Ores
            );

            StaticObjects.AddRange(loadedObjects);
            AnimatedObjects.AddRange(loadedAnimatedObjects);
            Chickens.AddRange(loadedChickens);
            Trees.AddRange(loadedTrees);
            Kapybaras.AddRange(loadedKapybaras);
            Ores.AddRange(loadedOres); // Thêm Ores từ XML

            // Thêm quặng vào Map4
            // Nếu bạn đã khai báo Ores trong XML, không cần thêm thủ công nữa
            /*
            Ore ore6 = new Ore(350, 350, Path.Combine("Assets", "Items", "Ore", "ore.png"));
            AddOre(ore6);
            */

            // Thêm StaticObjects và AnimatedObjects vào Obstacles nếu cần
            Obstacles.AddRange(StaticObjects);
            Obstacles.AddRange(loadedTrees);
            Obstacles.AddRange(loadedOres); // Thêm Ores vào Obstacles

            Console.WriteLine($"Map4 Loaded - StaticObjects Count: {StaticObjects.Count}, AnimatedObjects Count: {AnimatedObjects.Count}, Obstacles Count: {Obstacles.Count}, Chickens Count: {Chickens.Count}, Trees Count: {Trees.Count}, Kapybaras Count: {Kapybaras.Count}, Ores Count: {Ores.Count}");

            player.SetObstacles(Obstacles); // Cập nhật Obstacles cho Player

            // Thêm quặng cho map4 nếu cần
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
            Ores.Clear(); // Xoá danh sách Ores
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

            foreach (var ore in Ores)
            {
                // Quặng không có cập nhật liên tục trừ khi bạn muốn thêm logic nào đó
                // Nếu có, hãy thêm ở đây
            }

            // Loại bỏ các đối tượng đã bị đánh dấu để loại bỏ
            RemoveDestroyedObjects();
        }

        public void RenderAll(Graphics g)
        {
            // Vẽ StaticObjects
            foreach (var staticObj in StaticObjects)
            {
                staticObj.Draw(g);
            }

            // Vẽ Trees
            foreach (var tree in Trees)
            {
                tree.Draw(g);
            }

            // Vẽ Ores
            foreach (var ore in Ores)
            {
                ore.Draw(g);
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

        private void RemoveDestroyedObjects()
        {
            // Loại bỏ các đối tượng Static đã bị đánh dấu
            StaticObjects.RemoveAll(o => o is IRemovable removable && removable.ShouldRemove);

            // Loại bỏ các AnimatedObjects đã bị đánh dấu
            AnimatedObjects.RemoveAll(o => o is IRemovable removable && removable.ShouldRemove);

            // Loại bỏ các Enemies đã bị đánh dấu
            Enemies.RemoveAll(e => e is IRemovable removable && removable.ShouldRemove);

            // Loại bỏ các Chickens đã bị đánh dấu
            Chickens.RemoveAll(c => c is IRemovable removable && removable.ShouldRemove);

            // Loại bỏ các Kapybaras đã bị đánh dấu
            Kapybaras.RemoveAll(k => k is IRemovable removable && removable.ShouldRemove);

            // Loại bỏ các DroppedItems đã bị đánh dấu
            DroppedItems.RemoveAll(d => d is IRemovable removable && removable.ShouldRemove);

            // Loại bỏ các Trees đã bị đánh dấu
            Trees.RemoveAll(t => t is IRemovable removable && removable.ShouldRemove);

            // Loại bỏ các Ores đã bị đánh dấu
            Ores.RemoveAll(o => o is IRemovable removable && removable.ShouldRemove);
        }
    }
}
