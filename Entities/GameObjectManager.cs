using Project_Game.Entities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace Project_Game
{
    public class GameObjectManager
    {
        public List<StaticObject> StaticObjects { get; private set; }
        public List<AnimatedObject> AnimatedObjects { get; private set; }
        public List<Enemy> Enemies { get; private set; }
        public List<Chicken> Chickens { get; private set; }
        public List<Kapybara> Kapybaras { get; private set; }
        public List<GameObject> Obstacles { get; private set; }
        public List<DroppedItem> DroppedItems { get; private set; }
        public List<Tree> Trees { get; private set; }
        public List<Ore> Ores { get; private set; }

        // Danh sách Bullet mới thêm
        public List<Bullet> Bullets { get; private set; }

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
            Enemies = new List<Enemy>();
            Chickens = new List<Chicken>();
            Kapybaras = new List<Kapybara>();
            StaticObjects = new List<StaticObject>();
            DroppedItems = new List<DroppedItem>();
            Trees = new List<Tree>();
            Ores = new List<Ore>();
            Bullets = new List<Bullet>(); // Khởi tạo danh sách Bullets
            _instance = this; // Thiết lập instance cho Singleton
        }

        public void SetGameLogic(GameLogic logic)
        {
            this.gameLogic = logic;
        }

        // Phương thức thêm DroppedItem
        public void AddDroppedItem(DroppedItem item)
        {
            DroppedItems.Add(item);
            Console.WriteLine($"[Info] Thêm DroppedItem: {item.Item.Name} tại ({item.X}, {item.Y}) vào trò chơi.");
        }

        // Phương thức thêm Ore
        public void AddOre(Ore ore)
        {
            Ores.Add(ore);
            Obstacles.Add(ore);
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

        // Thêm phương thức để thêm Bullet
        public void AddGameObject(Bullet bullet)
        {
            Bullets.Add(bullet);
            Console.WriteLine($"[Info] Thêm Bullet tại ({bullet.X}, {bullet.Y}) vào trò chơi.");
        }

        public void LoadMap1()
        {
            ClearAll();

            Console.WriteLine("Loading Map 1...");
            List<Tree> loadedTrees;
            List<Chicken> loadedChickens;
            List<AnimatedObject> loadedAnimatedObjects;
            List<Kapybara> loadedKapybaras;
            List<Ore> loadedOres;

            var loadedObjects = ObjectLoader.LoadObjectsFromXml(
                "Assets/MapData/map1_objects.xml",
                out loadedChickens,
                out loadedAnimatedObjects,
                out loadedTrees,
                out loadedKapybaras,
                out loadedOres
            );

            Enemies.AddRange(TestEnemy.CreateEnemies("Assets\\Enemies\\Skeleton_Swordman", 1, 700, 150));
            Enemies.AddRange(SkeletonMage.CreateEnemies("Assets\\Enemies\\Skeleton_Mage", 1, 300, 150));


            StaticObjects.AddRange(loadedObjects);
            AnimatedObjects.AddRange(loadedAnimatedObjects);
            Chickens.AddRange(loadedChickens);
            Trees.AddRange(loadedTrees);
            Kapybaras.AddRange(loadedKapybaras);
            Ores.AddRange(loadedOres);

            Obstacles.AddRange(loadedObjects);
            Obstacles.AddRange(loadedTrees);
            Obstacles.AddRange(loadedOres);

            Console.WriteLine($"Map1 Loaded - StaticObjects Count: {StaticObjects.Count}, AnimatedObjects Count: {AnimatedObjects.Count}, Obstacles Count: {Obstacles.Count}, Chickens Count: {Chickens.Count}, Trees Count: {Trees.Count}, Kapybaras Count: {Kapybaras.Count}, Ores Count: {Ores.Count}");

            foreach (var house in StaticObjects.OfType<House>())
            {
                Console.WriteLine($"[Info] House tại ({house.X}, {house.Y}) size ({house.Width}x{house.Height})");
            }

            player.SetObstacles(Obstacles);
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
            List<Ore> loadedOres;

            var loadedObjects = ObjectLoader.LoadObjectsFromXml(
                "Assets/MapData/map2_objects.xml",
                out loadedChickens,
                out loadedAnimatedObjects,
                out loadedTrees,
                out loadedKapybaras,
                out loadedOres
            );

            StaticObjects.AddRange(loadedObjects);
            AnimatedObjects.AddRange(loadedAnimatedObjects);
            Chickens.AddRange(loadedChickens);
            Trees.AddRange(loadedTrees);
            Kapybaras.AddRange(loadedKapybaras);
            Ores.AddRange(loadedOres);

            Obstacles.AddRange(StaticObjects);
            Obstacles.AddRange(loadedTrees);
            Obstacles.AddRange(loadedOres);

            Console.WriteLine($"Map2 Loaded - StaticObjects Count: {StaticObjects.Count}, AnimatedObjects Count: {AnimatedObjects.Count}, Obstacles Count: {Obstacles.Count}, Chickens Count: {Chickens.Count}, Trees Count: {Trees.Count}, Kapybaras Count: {Kapybaras.Count}, Ores Count: {Ores.Count}");

            player.SetObstacles(Obstacles);

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
            List<Ore> loadedOres;

            var loadedObjects = ObjectLoader.LoadObjectsFromXml(
                "Assets/MapData/map3_objects.xml",
                out loadedChickens,
                out loadedAnimatedObjects,
                out loadedTrees,
                out loadedKapybaras,
                out loadedOres
            );

            StaticObjects.AddRange(loadedObjects);
            AnimatedObjects.AddRange(loadedAnimatedObjects);
            Chickens.AddRange(loadedChickens);
            Trees.AddRange(loadedTrees);
            Kapybaras.AddRange(loadedKapybaras);
            Ores.AddRange(loadedOres);

            Enemies.AddRange(TestEnemy.CreateEnemies("Assets/Enemies/Skeleton_Swordman", 3, 600, 200));

            Obstacles.AddRange(StaticObjects);
            Obstacles.AddRange(loadedTrees);
            Obstacles.AddRange(loadedOres);

            Console.WriteLine($"Map3 Loaded - StaticObjects Count: {StaticObjects.Count}, AnimatedObjects Count: {AnimatedObjects.Count}, Obstacles Count: {Obstacles.Count}, Chickens Count: {Chickens.Count}, Trees Count: {Trees.Count}, Kapybaras Count: {Kapybaras.Count}, Ores Count: {Ores.Count}");

            player.SetObstacles(Obstacles);
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
            List<Ore> loadedOres;

            var loadedObjects = ObjectLoader.LoadObjectsFromXml(
                "Assets/MapData/map4_objects.xml",
                out loadedChickens,
                out loadedAnimatedObjects,
                out loadedTrees,
                out loadedKapybaras,
                out loadedOres
            );

            StaticObjects.AddRange(loadedObjects);
            AnimatedObjects.AddRange(loadedAnimatedObjects);
            Chickens.AddRange(loadedChickens);
            Trees.AddRange(loadedTrees);
            Ores.AddRange(loadedOres);

            Obstacles.AddRange(StaticObjects);
            Obstacles.AddRange(loadedTrees);
            Obstacles.AddRange(loadedOres);

            Console.WriteLine($"Map4 Loaded - StaticObjects Count: {StaticObjects.Count}, AnimatedObjects Count: {AnimatedObjects.Count}, Obstacles Count: {Obstacles.Count}, Chickens Count: {Chickens.Count}, Trees Count: {Trees.Count}, Kapybaras Count: {Kapybaras.Count}, Ores Count: {Ores.Count}");

            player.SetObstacles(Obstacles);
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
            DroppedItems.Clear();
            Trees.Clear();
            Ores.Clear();
            Bullets.Clear();
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
            foreach (var bullet in Bullets.ToList())
            {
                bullet.Update();
                bullet.CheckHit(player);
                if (bullet.ShouldRemove)
                {
                    Bullets.Remove(bullet);
                }
            }
            foreach (var kapy in Kapybaras)
            {
                kapy.Update();
            }

            foreach (var animatedObj in AnimatedObjects)
            {
                animatedObj.Update();
            }

            foreach (var droppedItem in DroppedItems.ToList())
            {
                droppedItem.Update(player);
                if (droppedItem.ShouldRemove)
                {
                    DroppedItems.Remove(droppedItem);
                    Console.WriteLine($"[Info] DroppedItem {droppedItem.Item.Name} tại ({droppedItem.X}, {droppedItem.Y}) đã được loại bỏ.");
                }
            }

            foreach (var tree in Trees)
            {
                tree.UpdateTree();
            }

            foreach (var ore in Ores.ToList())
            {
                if (ore.ShouldRemove)
                {
                    Ores.Remove(ore);
                    Obstacles.Remove(ore);
                    Console.WriteLine($"[Info] Quặng tại ({ore.X}, {ore.Y}) đã được loại bỏ khỏi trò chơi.");
                }
            }

            // Cập nhật Bullet
            foreach (var bullet in Bullets.ToList())
            {
                bullet.Update();
                bullet.CheckHit(player);
                if (bullet.ShouldRemove)
                {
                    Bullets.Remove(bullet);
                    Console.WriteLine("[Info] Bullet đã bị loại bỏ.");
                }
            }

            RemoveDestroyedObjects();
        }

        public void RenderAll(Graphics g)
        {
            foreach (var staticObj in StaticObjects)
            {
                staticObj.Draw(g);
            }
            foreach (var bullet in Bullets)
            {
                bullet.Draw(g);
            }
            foreach (var tree in Trees)
            {
                tree.Draw(g);
            }

            foreach (var ore in Ores)
            {
                ore.Draw(g);
            }

            foreach (var animatedObj in AnimatedObjects)
            {
                animatedObj.Draw(g);
            }

            foreach (var droppedItem in DroppedItems)
            {
                droppedItem.Draw(g);
            }

            foreach (var enemy in Enemies)
            {
                enemy.Draw(g);
            }
            foreach (var kapy in Kapybaras)
            {
                kapy.Draw(g);
            }

            // Vẽ Bullet
            foreach (var bullet in Bullets)
            {
                bullet.Draw(g);
            }
        }

        private void RemoveDestroyedObjects()
        {
            StaticObjects.RemoveAll(o => o is IRemovable removable && removable.ShouldRemove);
            AnimatedObjects.RemoveAll(o => o is IRemovable removable && removable.ShouldRemove);
            Enemies.RemoveAll(e => e is IRemovable removable && removable.ShouldRemove);
            Chickens.RemoveAll(c => c is IRemovable removable && removable.ShouldRemove);
            Kapybaras.RemoveAll(k => k is IRemovable removable && removable.ShouldRemove);
            DroppedItems.RemoveAll(d => d is IRemovable removable && removable.ShouldRemove);
            Trees.RemoveAll(t => t is IRemovable removable && removable.ShouldRemove);
            Ores.RemoveAll(o => o is IRemovable removable && removable.ShouldRemove);
            Bullets.RemoveAll(b => b is IRemovable removable && removable.ShouldRemove);
        }
    }
}
