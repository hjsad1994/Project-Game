﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

public class AnimationManager
{
    private List<Image> frames = new List<Image>();
    private int currentFrameIndex = 0;
    private int frameRateCounter = 0;
    private int frameRateLimit;
    private bool isComplete = false;

    public Image CurrentFrame { get; private set; }

    public AnimationManager(int frameRate = 5)
    {
        frameRateLimit = frameRate;
    }

    public void LoadFrames(string folderPath)
    {
        if (!Directory.Exists(folderPath))
        {
            throw new DirectoryNotFoundException($"Folder not found: {folderPath}");
        }

        var filePaths = Directory.GetFiles(folderPath, "*.png");
        frames.Clear();

        foreach (var filePath in filePaths)
        {
            try
            {
                using (Image img = Image.FromFile(filePath))
                {
                    Bitmap bmp = new Bitmap(img);
                    frames.Add(bmp);
                }
            }
            catch (OutOfMemoryException)
            {
                // Log ra tên file gây lỗiii
                Console.WriteLine($"Không thể load ảnh từ {filePath}. File có thể hỏng hoặc không phải ảnh.");
            }
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
        }
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
}
