using System;
using System.Collections.Generic;
using System.Drawing; // Đảm bảo đã thêm using này
using System.IO;
using System.Xml;

namespace Project_Game.Entities
{
    public static class ObjectLoader
    {
        public static List<StaticObject> LoadObjectsFromXml(string xmlFilePath, out List<Chicken> chickens, out List<AnimatedObject> animatedObjects, out List<Tree> trees)
        {
            List<StaticObject> objects = new List<StaticObject>();
            chickens = new List<Chicken>();
            animatedObjects = new List<AnimatedObject>();
            trees = new List<Tree>();

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

            Console.WriteLine($"[Info] Found {nodeList.Count} Object node(s), {treeNodeList.Count} Tree node(s), and {animatedNodeList.Count} AnimatedObject node(s) in {xmlFilePath}.");

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
                        Console.WriteLine($"[Info] Added House at ({x}, {y}) with image {fullPath}");
                    }
                    else
                    {
                        Console.WriteLine($"[Error] House image not found: {fullPath}");
                    }
                }
                else if (type == "Chicken")
                {
                    int minX = x - 50;
                    int maxX = x + 50;
                    chickens.Add(new Chicken("ChickenFromXML", x, y, minX, maxX));
                    Console.WriteLine($"[Info] Added Chicken at ({x}, {y}) with patrol range [{minX}, {maxX}]");
                }
                else
                {
                    string category = node.Attributes["category"]?.Value ?? "Objects";
                    string fullPath = Path.Combine("Assets", category, imageName);
                    if (File.Exists(fullPath))
                    {
                        objects.Add(new StaticObject(fullPath, x, y, width, height));
                        Console.WriteLine($"[Info] Added StaticObject at ({x}, {y}) with image {fullPath}");
                    }
                    else
                    {
                        Console.WriteLine($"[Error] StaticObject image not found: {fullPath}");
                    }
                }
            }

            // Xử lý các Tree
            // Phần xử lý Tree trong LoadObjectsFromXml
            foreach (XmlNode treeNode in treeNodeList)
            {
                string category = treeNode.Attributes["category"]?.Value;
                string imageName = treeNode.Attributes["imageName"]?.Value;
                int x = int.Parse(treeNode.Attributes["x"]?.Value ?? "0");
                int y = int.Parse(treeNode.Attributes["y"]?.Value ?? "0");
                int width = int.Parse(treeNode.Attributes["width"]?.Value ?? "50");   // Đọc width
                int height = int.Parse(treeNode.Attributes["height"]?.Value ?? "100"); // Đọc height

                Console.WriteLine($"[Debug] Tree node: category={category}, imageName={imageName}, x={x}, y={y}, width={width}, height={height}");

                // Giả sử các giai đoạn phát triển được đặt trong thư mục cụ thể
                List<Image> treeStages = new List<Image>();
                bool missingStage = false;
                for (int i = 1; i <= 3; i++)
                {
                    string treeImagePath = Path.Combine("Assets", category, $"Spruce_tree_{i:00}.png");
                    if (File.Exists(treeImagePath))
                    {
                        try
                        {
                            Image treeImage = Image.FromFile(treeImagePath);
                            treeStages.Add(treeImage);
                            Console.WriteLine($"[Info] Loaded tree stage {i} from {treeImagePath}");
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

                if (!missingStage && treeStages.Count == 3)
                {
                    // Tạo Tree với width và height
                    Tree tree = new Tree(x, y, treeStages, width, height, growthIntervalMilliseconds: 5000); // Thêm width và height
                    trees.Add(tree);
                    Console.WriteLine($"[Info] Added Tree at ({x}, {y}) with 3 stages and size ({width}x{height}).");
                }
                else
                {
                    Console.WriteLine($"[Warning] Tree at ({x}, {y}) not added due to missing stages.");
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
                    Console.WriteLine($"[Info] Added AnimatedObject of type {type} at ({x}, {y})");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[Error] Error creating AnimatedObject of type {type}: {ex.Message}");
                }
            }

            Console.WriteLine($"[Summary] Total objects loaded from XML: {objects.Count}, Chickens loaded: {chickens.Count}, Trees loaded: {trees.Count}, AnimatedObjects loaded: {animatedObjects.Count}");
            return objects;
        }
    }
}
