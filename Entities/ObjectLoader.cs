using System;
using System.IO;
using System.Xml;
using System.Collections.Generic;

namespace Project_Game.Entities
{
    public class ObjectLoader
    {
        public static List<StaticObject> LoadObjectsFromXml(string xmlFilePath, out List<Chicken> chickens)
        {
            List<StaticObject> objects = new List<StaticObject>();
            chickens = new List<Chicken>();

            if (!File.Exists(xmlFilePath))
            {
                Console.WriteLine($"XML file not found: {xmlFilePath}");
                return objects;
            }

            XmlDocument doc = new XmlDocument();
            doc.Load(xmlFilePath);

            XmlNodeList nodeList = doc.SelectNodes("/Objects/Object");
            Console.WriteLine($"Found {nodeList.Count} Object node(s) in {xmlFilePath}.");

            foreach (XmlNode node in nodeList)
            {
                string type = node.Attributes["type"]?.Value;
                string imageName = node.Attributes["imageName"]?.Value;
                int x = int.Parse(node.Attributes["x"]?.Value ?? "0");
                int y = int.Parse(node.Attributes["y"]?.Value ?? "0");
                int width = int.Parse(node.Attributes["width"]?.Value ?? "50");
                int height = int.Parse(node.Attributes["height"]?.Value ?? "50");

                Console.WriteLine($"Object node: type={type}, imageName={imageName}, x={x}, y={y}, width={width}, height={height}");

                if (type == "House")
                {
                    objects.Add(new House(imageName, x, y, width, height));
                }
                else if (type == "Chicken")
                {
                    // Giả sử Chicken có constructor: Chicken(string name, int startX, int startY, int minX, int maxX)
                    // Bạn phải quyết định minX, maxX như thế nào. Ví dụ đặt cứng:
                    int minX = x - 50;
                    int maxX = x + 50;
                    chickens.Add(new Chicken("ChickenFromXML", x, y, minX, maxX));
                }
                else
                {
                    // Các loại object khác (Fence, v.v.)
                    string category = node.Attributes["category"]?.Value;
                    if (string.IsNullOrEmpty(category))
                    {
                        category = "Objects";
                    }

                    string fullPath = Path.Combine("Assets", category, imageName);
                    objects.Add(new StaticObject(fullPath, x, y, width, height));
                }
            }

            Console.WriteLine($"Total objects loaded from XML: {objects.Count}, Chickens loaded: {chickens.Count}");
            return objects;
        }
    }
}
