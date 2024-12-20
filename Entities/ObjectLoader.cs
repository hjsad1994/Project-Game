using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Xml;

namespace Project_Game.Entities
{
    public static class ObjectLoader
    {
        public static List<StaticObject> LoadObjectsFromXml(string xmlFilePath,
            out List<Chicken> chickens,
            out List<AnimatedObject> animatedObjects,
            out List<Tree> trees,
            out List<Kapybara> kapybaras,
            out List<Ore> ores) // Thêm danh sách Ores
        {
            List<StaticObject> objects = new List<StaticObject>();
            chickens = new List<Chicken>();
            animatedObjects = new List<AnimatedObject>();
            trees = new List<Tree>();
            kapybaras = new List<Kapybara>();
            ores = new List<Ore>(); // Khởi tạo danh sách Ores

            if (!File.Exists(xmlFilePath))
            {
                Console.WriteLine($"[Error] XML file not found: {xmlFilePath}");
                return objects;
            }

            XmlDocument doc = new XmlDocument();
            doc.Load(xmlFilePath);

            XmlNodeList nodeList = doc.SelectNodes("/Objects/Object");
            XmlNodeList treeNodeList = doc.SelectNodes("/Objects/Tree");
            XmlNodeList animatedNodeList = doc.SelectNodes("/Objects/AnimatedObject");
            XmlNodeList kapybaraNodeList = doc.SelectNodes("/Objects/Kapybara");
            XmlNodeList oreNodeList = doc.SelectNodes("/Objects/Ore"); // Thêm xử lý Ore

            Console.WriteLine($"[Info] Found {nodeList.Count} Object node(s), {treeNodeList.Count} Tree node(s), {animatedNodeList.Count} AnimatedObject node(s), {kapybaraNodeList.Count} Kapybara node(s), và {oreNodeList.Count} Ore node(s) trong {xmlFilePath}.");

            // Xử lý các StaticObject và Chicken
            foreach (XmlNode node in nodeList)
            {
                string type = node.Attributes["type"]?.Value;
                string imageName = node.Attributes["imageName"]?.Value;
                int x = int.Parse(node.Attributes["x"]?.Value ?? "0");
                int y = int.Parse(node.Attributes["y"]?.Value ?? "0");
                int width = int.Parse(node.Attributes["width"]?.Value ?? "50");
                int height = int.Parse(node.Attributes["height"]?.Value ?? "50");

                Console.WriteLine($"[Debug] Object node: type={type}, imageName={imageName}, x={x}, y={y}, width={width}, height={height}");

                if (type == "House")
                {
                    string category = node.Attributes["category"]?.Value ?? "Houses";
                    string fullPath = Path.Combine("Assets", category, imageName);
                    if (File.Exists(fullPath))
                    {
                        objects.Add(new House(fullPath, x, y, width, height));
                        Console.WriteLine($"[Info] Added House tại ({x}, {y}) với image {fullPath}");
                    }
                    else
                    {
                        Console.WriteLine($"[Error] House image not found: {fullPath}");
                    }
                }
                else if (type == "Chicken")
                {
                    // Bạn có thể thêm các thuộc tính khác nếu cần, như tên
                    chickens.Add(new Chicken("ChickenFromXML", x, y, x - 50, x + 50));
                    Console.WriteLine($"[Info] Added Chicken tại ({x}, {y}) với patrol range [{x - 50}, {x + 50}]");
                }
                else
                {
                    string category = node.Attributes["category"]?.Value ?? "Objects";
                    string fullPath = Path.Combine("Assets", category, imageName);
                    if (File.Exists(fullPath))
                    {
                        objects.Add(new StaticObject(fullPath, x, y, width, height));
                        Console.WriteLine($"[Info] Added StaticObject tại ({x}, {y}) với image {fullPath}");
                    }
                    else
                    {
                        Console.WriteLine($"[Error] StaticObject image not found: {fullPath}");
                    }
                }
            }

            // Xử lý các Tree
            foreach (XmlNode treeNode in treeNodeList)
            {
                string category = treeNode.Attributes["category"]?.Value; // e.g., "Trees/Spruce_tree"
                if (string.IsNullOrEmpty(category))
                {
                    Console.WriteLine($"[Error] Tree category is missing.");
                    continue;
                }

                int x = int.Parse(treeNode.Attributes["x"]?.Value ?? "0");
                int y = int.Parse(treeNode.Attributes["y"]?.Value ?? "0");

                Console.WriteLine($"[Debug] Tree node: category={category}, x={x}, y={y}");

                // Giả sử các giai đoạn phát triển được đặt trong thư mục cụ thể
                List<TreeStage> treeStages = new List<TreeStage>();
                bool missingStage = false;

                foreach (XmlNode stageNode in treeNode.SelectNodes("Stage"))
                {
                    string imageName = stageNode.Attributes["imageName"]?.Value;
                    int stageWidth = int.Parse(stageNode.Attributes["width"]?.Value ?? "10");
                    int stageHeight = int.Parse(stageNode.Attributes["height"]?.Value ?? "10");

                    if (string.IsNullOrEmpty(imageName))
                    {
                        Console.WriteLine($"[Error] Stage imageName is missing for Tree tại ({x}, {y}).");
                        missingStage = true;
                        break;
                    }

                    string[] categoryParts = category.Split('/');
                    if (categoryParts.Length < 2)
                    {
                        Console.WriteLine($"[Error] Invalid category format for Tree: {category}");
                        missingStage = true;
                        break;
                    }
                    string treeType = categoryParts[1]; // e.g., "Spruce_tree"
                    string treeImagePath = Path.Combine("Assets", category, imageName);

                    if (File.Exists(treeImagePath))
                    {
                        try
                        {
                            Image treeImage = Image.FromFile(treeImagePath);
                            treeStages.Add(new TreeStage(treeImage, stageWidth, stageHeight));
                            Console.WriteLine($"[Info] Loaded tree stage với image {treeImagePath} và size ({stageWidth}x{stageHeight}).");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"[Error] Error loading tree image {treeImagePath}: {ex.Message}");
                            missingStage = true;
                            break;
                        }
                    }
                    else
                    {
                        Console.WriteLine($"[Error] Tree image not found: {treeImagePath}");
                        missingStage = true;
                        break;
                    }
                }

                if (!missingStage && treeStages.Count > 0)
                {
                    Tree tree = new Tree(x, y, treeStages, growthIntervalMilliseconds: 5000); // 5 giây để phát triển
                    trees.Add(tree);
                    Console.WriteLine($"[Info] Added Tree tại ({x}, {y}) với {treeStages.Count} stages.");
                }
                else
                {
                    Console.WriteLine($"[Warning] Tree tại ({x}, {y}) không được thêm vào do thiếu stages.");
                }
            }

            // Xử lý các AnimatedObject
            foreach (XmlNode animNode in animatedNodeList)
            {
                string type = animNode.Attributes["type"]?.Value;
                string spriteSheetFolderPath = animNode.Attributes["spriteSheetFolderPath"]?.Value;
                int x = int.Parse(animNode.Attributes["x"]?.Value ?? "0");
                int y = int.Parse(animNode.Attributes["y"]?.Value ?? "0");
                int width = int.Parse(animNode.Attributes["width"]?.Value ?? "50");
                int height = int.Parse(animNode.Attributes["height"]?.Value ?? "50");
                int frameRate = int.Parse(animNode.Attributes["frameRate"]?.Value ?? "5");

                Console.WriteLine($"[Debug] AnimatedObject node: type={type}, spriteSheetFolderPath={spriteSheetFolderPath}, x={x}, y={y}, width={width}, height={height}, frameRate={frameRate}");

                try
                {
                    AnimatedObject animatedObj = new AnimatedObject(spriteSheetFolderPath, x, y, width, height, frameRate);
                    animatedObjects.Add(animatedObj);
                    Console.WriteLine($"[Info] Added AnimatedObject of type {type} tại ({x}, {y})");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[Error] Error creating AnimatedObject of type {type}: {ex.Message}");
                }
            }

            // Xử lý các Kapybara
            foreach (XmlNode kapyNode in kapybaraNodeList)
            {
                string name = kapyNode.Attributes["name"]?.Value ?? "Kapybara";
                int x = int.Parse(kapyNode.Attributes["x"]?.Value ?? "0");
                int y = int.Parse(kapyNode.Attributes["y"]?.Value ?? "0");
                int minX = int.Parse(kapyNode.Attributes["minX"]?.Value ?? (x - 50).ToString());
                int maxX = int.Parse(kapyNode.Attributes["maxX"]?.Value ?? (x + 50).ToString());
                int width = int.Parse(kapyNode.Attributes["width"]?.Value ?? "60"); // Thêm thuộc tính width
                int height = int.Parse(kapyNode.Attributes["height"]?.Value ?? "50"); // Thêm thuộc tính height

                Console.WriteLine($"[Debug] Kapybara node: name={name}, x={x}, y={y}, minX={minX}, maxX={maxX}, width={width}, height={height}");

                try
                {
                    Kapybara kapy = new Kapybara(name, x, y, minX, maxX)
                    {
                        Width = width,    // Đặt Width theo XML
                        Height = height   // Đặt Height theo XML
                    };
                    kapybaras.Add(kapy);
                    Console.WriteLine($"[Info] Added Kapybara tên {name} tại ({x}, {y}) với patrol range [{minX}, {maxX}] và kích thước ({width}x{height}).");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[Error] Error creating Kapybara tên {name}: {ex.Message}");
                }
            }

            // Xử lý các Ore
            foreach (XmlNode oreNode in oreNodeList)
            {
                string imageName = oreNode.Attributes["imageName"]?.Value;
                int x = int.Parse(oreNode.Attributes["x"]?.Value ?? "0");
                int y = int.Parse(oreNode.Attributes["y"]?.Value ?? "0");
                int width = int.Parse(oreNode.Attributes["width"]?.Value ?? "32");
                int height = int.Parse(oreNode.Attributes["height"]?.Value ?? "32");

                Console.WriteLine($"[Debug] Ore node: imageName={imageName}, x={x}, y={y}, width={width}, height={height}");

                if (string.IsNullOrEmpty(imageName))
                {
                    Console.WriteLine($"[Error] Ore imageName is missing at ({x}, {y}).");
                    continue;
                }

                string oreImagePath = Path.Combine("Assets", "Items", "Ore", imageName);

                // Đọc các DropItem nếu có
                List<DropItemInfo> dropItems = new List<DropItemInfo>();
                XmlNodeList dropItemNodes = oreNode.SelectNodes("DropItems/DropItem");
                foreach (XmlNode dropItemNode in dropItemNodes)
                {
                    string dropItemName = dropItemNode.Attributes["name"]?.Value ?? "Ore";
                    string dropItemImageName = dropItemNode.Attributes["imageName"]?.Value ?? "ore.png";
                    int probability = int.Parse(dropItemNode.Attributes["probability"]?.Value ?? "100");

                    string dropItemImagePath = Path.Combine("Assets", "Items", "Ore", dropItemImageName);
                    if (File.Exists(dropItemImagePath))
                    {
                        dropItems.Add(new DropItemInfo(dropItemName, dropItemImagePath, probability));
                        Console.WriteLine($"[Info] Added DropItem: {dropItemName} với probability {probability}% tại {dropItemImagePath}");
                    }
                    else
                    {
                        Console.WriteLine($"[Error] DropItem image not found: {dropItemImagePath}");
                    }
                }

                if (File.Exists(oreImagePath))
                {
                    try
                    {
                        Ore ore = new Ore(x, y, oreImagePath, dropItems)
                        {
                            Width = width,
                            Height = height
                        };
                        ores.Add(ore);
                        Console.WriteLine($"[Info] Added Ore tại ({x}, {y}) với image {oreImagePath}, có {dropItems.Count} DropItems và size ({width}x{height}).");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[Error] Error creating Ore tại ({x}, {y}): {ex.Message}");
                    }
                }
                else
                {
                    Console.WriteLine($"[Error] Ore image not found: {oreImagePath}");
                }
            }

            Console.WriteLine($"[Summary] Tổng số objects đã tải từ XML: {objects.Count}, Chickens đã tải: {chickens.Count}, Trees đã tải: {trees.Count}, AnimatedObjects đã tải: {animatedObjects.Count}, Kapybaras đã tải: {kapybaras.Count}, và Ores đã tải: {ores.Count}");
            return objects;
        }
    }
}
