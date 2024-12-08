using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

public class AnimationManager
{
    private List<string> frames = new List<string>(); // Các khung hình của animation
    private int currentFrameIndex = 0;
    private int frameRateCounter = 0;
    private int frameRateLimit;

    public Image CurrentFrame { get; private set; } // Khung hình hiện tại

    public AnimationManager(int frameRate = 5)
    {
        frameRateLimit = frameRate;
    }

    public void LoadFrames(string folderPath)
    {
        if (Directory.Exists(folderPath))
        {
            frames = new List<string>(Directory.GetFiles(folderPath, "*.png"));
            if (frames.Count > 0)
            {
                CurrentFrame = Image.FromFile(frames[0]);
                Console.WriteLine($"Loaded {frames.Count} frames from {folderPath}");
            }
            else
            {
                Console.WriteLine($"No frames found in {folderPath}");
            }
        }
        else
        {
            Console.WriteLine($"Folder not found: {folderPath}");
        }
    }


    public void UpdateAnimation()
    {
        if (frames.Count == 0) return;

        frameRateCounter++;
        if (frameRateCounter >= frameRateLimit)
        {
            currentFrameIndex = (currentFrameIndex + 1) % frames.Count;
            CurrentFrame = Image.FromFile(frames[currentFrameIndex]);
            frameRateCounter = 0;
            Console.WriteLine($"Switched to frame {currentFrameIndex} in animation.");
        }
    }


    public void ResetAnimation()
    {
        currentFrameIndex = 0;
        frameRateCounter = 0;
        if (frames.Count > 0)
        {
            CurrentFrame = Image.FromFile(frames[0]);
        }
    }

    public bool IsComplete()
    {
        return currentFrameIndex == frames.Count - 1 && frameRateCounter == 0;
    }
}
