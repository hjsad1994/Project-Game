namespace Project_Game.Entities
{
    public class DropItemInfo
    {
        public string Name { get; set; }
        public string ImagePath { get; set; }
        public int Probability { get; set; } // Xác suất rơi ra (%)

        public DropItemInfo(string name, string imagePath, int probability)
        {
            Name = name;
            ImagePath = imagePath;
            Probability = probability;
        }
    }
}