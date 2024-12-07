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
    public string CurrentDirection { get; set; }

    public Image CurrentFrame { get; private set; } // Khung hình hiện tại

    public AnimationManager(int frameRate = 5)
    {
        frameRateLimit = frameRate; // Số lần lặp trước khi chuyển khung hình
    }

    // Tải các khung hình từ thư mục
    public void LoadFrames(string folderPath)
    {
        if (Directory.Exists(folderPath))
        {
            frames = new List<string>(Directory.GetFiles(folderPath, "*.png"));
            if (frames.Count > 0)
            {
                CurrentFrame = Image.FromFile(frames[0]); // Đặt khung hình đầu tiên
            }
        }
        else
        {
            throw new DirectoryNotFoundException($"Folder not found: {folderPath}");
        }
    }

    // Điều khiển hoạt ảnh
    public void UpdateAnimation()
    {
        if (frames.Count == 0) return;

        frameRateCounter++;
        if (frameRateCounter >= frameRateLimit)
        {
            currentFrameIndex = (currentFrameIndex + 1) % frames.Count;
            CurrentFrame = Image.FromFile(frames[currentFrameIndex]);
            frameRateCounter = 0;
        }
    }

    // Reset hoạt ảnh
    public void ResetAnimation()
    {
        currentFrameIndex = 0;
        frameRateCounter = 0;
        if (frames.Count > 0)
        {
            CurrentFrame = Image.FromFile(frames[0]);
        }
    }
}
