using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

public class AnimationManager
{
    private List<Image> frames = new List<Image>(); // Các khung hình đã tải
    private int currentFrameIndex = 0;
    private int frameRateCounter = 0;
    private int frameRateLimit;

    public Image CurrentFrame { get; private set; } // Khung hình hiện tại

    public AnimationManager(int frameRate = 5)
    {
        frameRateLimit = frameRate;
    }

    /// <summary>
    /// Tải các khung hình từ thư mục.
    /// </summary>
    /// <param name="folderPath">Đường dẫn thư mục chứa khung hình.</param>
    public void LoadFrames(string folderPath)
    {
        if (Directory.Exists(folderPath))
        {
            frames.Clear(); // Xóa các khung hình cũ
            foreach (string file in Directory.GetFiles(folderPath, "*.png"))
            {
                frames.Add(Image.FromFile(file));
            }

            if (frames.Count > 0)
            {
                CurrentFrame = frames[0]; // Đặt khung hình đầu tiên
            }
            else
            {
                throw new Exception($"No frames found in folder: {folderPath}");
            }
        }
        else
        {
            throw new DirectoryNotFoundException($"Folder not found: {folderPath}");
        }
    }

    /// <summary>
    /// Cập nhật hoạt ảnh, chuyển sang khung hình tiếp theo nếu đủ điều kiện.
    /// </summary>
    public void UpdateAnimation()
    {
        if (frames.Count == 0) return;

        frameRateCounter++;
        if (frameRateCounter >= frameRateLimit)
        {
            currentFrameIndex = (currentFrameIndex + 1) % frames.Count;
            CurrentFrame = frames[currentFrameIndex];
            frameRateCounter = 0;
        }
    }

    /// <summary>
    /// Reset hoạt ảnh về khung hình đầu tiên.
    /// </summary>
    public void ResetAnimation()
    {
        currentFrameIndex = 0;
        frameRateCounter = 0;
        if (frames.Count > 0)
        {
            CurrentFrame = frames[0];
        }
    }

    /// <summary>
    /// Kiểm tra xem hoạt ảnh đã hoàn thành một chu kỳ hay chưa.
    /// </summary>
    /// <returns>Trả về true nếu hoàn thành một chu kỳ, ngược lại là false.</returns>
    public bool IsComplete()
    {
        return currentFrameIndex == frames.Count - 1 && frameRateCounter == 0;
    }

    /// <summary>
    /// Xóa dữ liệu và giải phóng tài nguyên của khung hình.
    /// </summary>
    public void Dispose()
    {
        foreach (var frame in frames)
        {
            frame.Dispose();
        }
        frames.Clear();
        CurrentFrame = null;
    }
}
