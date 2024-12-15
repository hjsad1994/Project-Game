using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace Project_Game.Entities
{
    public class AnimationManager
    {
        private List<Image> frames = new List<Image>();
        private List<string> frameNames = new List<string>(); // Danh sách tên khung hình
        private int currentFrameIndex = 0;
        private int frameRateCounter = 0;
        private int frameRateLimit;
        private bool isComplete = false;

        public Image CurrentFrame { get; private set; }
        private Dictionary<string, List<Image>> cachedFrames = new Dictionary<string, List<Image>>();

        public AnimationManager(int frameRate = 5)
        {
            frameRateLimit = frameRate;
        }

        public void LoadFrames(string folderPath, string state = "default")
        {
            if (cachedFrames.ContainsKey(folderPath))
            {
                frames = cachedFrames[folderPath];
                CurrentFrame = frames[0];
                currentFrameIndex = 0; // Reset frame index
                frameRateCounter = 0;   // Reset frame rate counter
                Console.WriteLine($"Loaded cached frames from: {folderPath}");
                return;
            }

            if (!Directory.Exists(folderPath))
            {
                Console.WriteLine($"Folder not found: {folderPath}");
                throw new DirectoryNotFoundException($"Folder not found: {folderPath}");
            }

            var filePaths = Directory.GetFiles(folderPath, "*.png");
            frames.Clear();
            frameNames.Clear(); // Reset danh sách tên khung hình

            Console.WriteLine($"Loading frames from: {folderPath}");

            foreach (var filePath in filePaths)
            {
                try
                {
                    using (Image img = Image.FromFile(filePath))
                    {
                        Bitmap bmp = new Bitmap(img);
                        frames.Add(bmp);
                        frameNames.Add(Path.GetFileName(filePath)); // Thêm tên khung hình
                        Console.WriteLine($"Loaded frame: {Path.GetFileName(filePath)}");
                    }
                }
                catch (OutOfMemoryException)
                {
                    Console.WriteLine($"Cannot load image from {filePath}. The file may be corrupted or not an image.");
                }
            }

            if (frames.Count > 0)
            {
                CurrentFrame = frames[0];
                currentFrameIndex = 0;      // Reset frame index
                frameRateCounter = 0;       // Reset frame rate counter
                cachedFrames[folderPath] = new List<Image>(frames); // Cache the loaded frames
                Console.WriteLine($"Total frames loaded from {folderPath}: {frames.Count}");
            }
            else
            {
                Console.WriteLine($"No frames loaded from {folderPath}");
            }
        }

        public void UpdateAnimation(bool loop = true)
        {
            if (frames.Count == 0) return;

            frameRateCounter++;
            if (frameRateCounter >= frameRateLimit)
            {
                frameRateCounter = 0;
                currentFrameIndex++;

                if (currentFrameIndex >= frames.Count)
                {
                    if (loop)
                    {
                        currentFrameIndex = 0;
                    }
                    else
                    {
                        currentFrameIndex = frames.Count - 1;
                        isComplete = true;
                    }
                }

                CurrentFrame = frames[currentFrameIndex];
                Console.WriteLine($"Animation Update: Frame {currentFrameIndex + 1}/{frames.Count} - {frameNames[currentFrameIndex]}");
            }
        }

        public void ResetAnimation()
        {
            currentFrameIndex = 0;
            frameRateCounter = 0;
            isComplete = false;
            if (frames.Count > 0)
            {
                CurrentFrame = frames[0];
                Console.WriteLine("Animation reset to first frame.");
            }
            Console.WriteLine("Animation reset.");
        }

        public bool IsComplete()
        {
            return isComplete;
        }

        public int GetFrameCount()
        {
            return frames.Count;
        }

        public Image GetCurrentFrame()
        {
            return CurrentFrame;
        }

        public string GetCurrentFrameName()
        {
            if (currentFrameIndex < frameNames.Count)
                return frameNames[currentFrameIndex];
            return string.Empty;
        }

        public void SetFrames(List<Image> newFrames)
        {
            frames = newFrames;
            frameNames.Clear(); // Reset danh sách tên khung hình

            // Nếu bạn có thông tin tên khung hình, hãy thêm vào đây
            // Giả sử tên khung hình không cần thiết trong SetFrames
            currentFrameIndex = 0;
            frameRateCounter = 0;
            CurrentFrame = frames.Count > 0 ? frames[0] : null;
            isComplete = false;
            Console.WriteLine("Animation frames set.");
        }
        public void ClearCache()
        {
            cachedFrames.Clear();
            Console.WriteLine("Animation cache cleared.");
        }
    }
}
