using System;
using System.IO;
using System.Xml;
using System.Collections.Generic;

namespace Project_Game.Entities
{
    public class ObjectLoader
    {
        public static List<StaticObject> LoadObjectsFromXml(string xmlFilePath)
        {
            List<StaticObject> objects = new List<StaticObject>();

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

                Console.WriteLine($"Loading Object: type={type}, imageName={imageName}, x={x}, y={y}, width={width}, height={height}");

                StaticObject obj = null;

                if (type == "House")
                {
                    // Chỉ truyền tên file. House constructor sẽ tự ghép "Assets/House/"
                    obj = new House(imageName, x, y, width, height);
                }
                else if (type == "Fence")
                {
                    obj = new Fence(imageName, x, y, width, height);
                }
                else
                {
                    string category = node.Attributes["category"]?.Value;
                    if (string.IsNullOrEmpty(category))
                    {
                        category = "Objects";
                    }

                    string fullPath = Path.Combine("Assets", category, imageName);
                    obj = new StaticObject(fullPath, x, y, width, height);
                }

                if (obj != null)
                    objects.Add(obj);
                else
                    Console.WriteLine("Failed to create object from XML node.");
            }

            Console.WriteLine($"Total objects loaded from XML: {objects.Count}");
            return objects;
        }
    }
}
