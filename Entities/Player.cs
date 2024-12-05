//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Windows.Forms;
//using System.Linq;
//using System.Drawing;

//public class Player
//{
//    public Image playerImage;
//    public List<string> playerMovements = new List<string>();
//    public List<string> idleMovements = new List<string>();
//    public int steps = 0;
//    public int slowDownFrameRate = 0;  // Giảm tốc độ animation khi di chuyển
//    public int idleFrameRate = 0;      // Giảm tốc độ animation khi idle
//    public bool goLeft, goRight, goUp, goDown;
//    public int playerX = 100;
//    public int playerY = 100;
//    public int playerHeight = 50;
//    public int playerWidth = 32;
//    public int playerSpeed = 6; // Tốc độ di chuyển của player
//    public string idleDirection = "Down";
//    public int currentHealth = 100;
//    public int maxHealth = 100;

//    public Player()
//    {
//        LoadPlayerImages();
//    }

//    private void LoadPlayerImages()
//    {
//        LoadMovementImages("MoveDown");
//        LoadIdleImages(idleDirection);
//        if (playerMovements.Count > 0)
//        {
//            playerImage = Image.FromFile(playerMovements[0]);
//        }
//    }

//    public void LoadMovementImages(string direction)
//    {
//        playerMovements.Clear();
//        string movementFolder = $"Char_MoveMent/{direction}";  // Lấy thư mục theo hướng
//        playerMovements = Directory.GetFiles(movementFolder, "*.png").ToList(); // Lấy các ảnh PNG trong thư mục
//    }

//    public void LoadIdleImages(string direction)
//    {
//        idleMovements.Clear();
//        string idleFolder = $"Char_Idle/{direction}";
//        idleMovements = Directory.GetFiles(idleFolder, "*.png").ToList();
//    }

//    public void ResetPlayer()
//    {
//        // Reset player trạng thái sau khi game reset
//        playerX = 100;
//        playerY = 100;
//        currentHealth = 100;  // Đặt lại máu về 100
//        this.playerSpeed = 6;  // Đặt lại tốc độ player
//        goLeft = goRight = goUp = goDown = false;
//        idleDirection = "Down";  // Đặt lại hướng idle mặc định
//        steps = 0;              // Reset số bước animation
//        slowDownFrameRate = 0;  // Reset frame rate chậm
//        idleFrameRate = 0;      // Reset idle frame rate
//    }

//    // Phương thức điều chỉnh tốc độ animation khi di chuyển
//    public void AnimatePlayer(int start, int end)
//    {
//        slowDownFrameRate += 1;  // Tăng dần tốc độ frame rate để tạo hiệu ứng chậm lại khi di chuyển
//        if (slowDownFrameRate == 4)  // Điều chỉnh tốc độ ở đây (ví dụ: mỗi 5 ticks mới thay đổi frame)
//        {
//            steps++;
//            slowDownFrameRate = 0;
//        }

//        // Đảm bảo rằng frame không vượt quá chỉ số
//        if (steps > end || steps < start)
//        {
//            steps = start;
//        }

//        playerImage = Image.FromFile(playerMovements[steps]);
//    }

//    // Phương thức điều chỉnh tốc độ animation khi idle (không di chuyển)
//    public void AnimateIdle(int start, int end)
//    {
//        if (idleMovements.Count > 0)
//        {
//            idleFrameRate++;  // Tăng dần frame rate cho trạng thái idle
//            if (idleFrameRate >= 5)  // Điều chỉnh tốc độ idle animation (mỗi 5 ticks mới thay đổi frame)
//            {
//                steps++;
//                if (steps > end)
//                {
//                    steps = start;
//                }
//                idleFrameRate = 0;
//            }
//            playerImage = Image.FromFile(idleMovements[steps % idleMovements.Count]);
//        }
//    }

//    public void Move()
//    {
//        // Cập nhật vị trí của player và hướng di chuyển
//        if (goLeft)
//        {
//            playerX -= playerSpeed;  // Di chuyển sang trái
//            idleDirection = "Left";  // Cập nhật hướng idle
//            LoadMovementImages("MoveLeft");
//            AnimatePlayer(0, 5);  // Di chuyển từ frame 0 đến 5
//        }
//        else if (goRight)
//        {
//            playerX += playerSpeed;  // Di chuyển sang phải
//            idleDirection = "Right";  // Cập nhật hướng idle
//            LoadMovementImages("MoveRight");
//            AnimatePlayer(0, 5);  // Di chuyển từ frame 0 đến 5
//        }
//        else if (goUp)
//        {
//            playerY -= playerSpeed;  // Di chuyển lên
//            idleDirection = "Up";  // Cập nhật hướng idle
//            LoadMovementImages("MoveUp");
//            AnimatePlayer(0, 5);  // Di chuyển từ frame 0 đến 5
//        }
//        else if (goDown)
//        {
//            playerY += playerSpeed;  // Di chuyển xuống
//            idleDirection = "Down";  // Cập nhật hướng idle
//            LoadMovementImages("MoveDown");
//            AnimatePlayer(0, 5);  // Di chuyển từ frame 0 đến 5
//        }
//        else
//        {
//            LoadMovementImages(idleDirection);  // Nếu không di chuyển, chỉ tải ảnh idle
//            AnimateIdle(0, 5);  // Animation idle nếu không di chuyển
//        }
//    }

//    public void ResetMovementFlags()
//    {
//        goLeft = false;
//        goRight = false;
//        goUp = false;
//        goDown = false;
//    }

//    public void KeyDownHandler(KeyEventArgs e)
//    {
//        if (e.KeyCode == Keys.Left)
//        {
//            goLeft = true;
//        }
//        else if (e.KeyCode == Keys.Right)
//        {
//            goRight = true;
//        }
//        else if (e.KeyCode == Keys.Up)
//        {
//            goUp = true;
//        }
//        else if (e.KeyCode == Keys.Down)
//        {
//            goDown = true;
//        }
//    }

//    public void KeyUpHandler(KeyEventArgs e)
//    {
//        if (e.KeyCode == Keys.Left)
//        {
//            goLeft = false;
//        }
//        else if (e.KeyCode == Keys.Right)
//        {
//            goRight = false;
//        }
//        else if (e.KeyCode == Keys.Up)
//        {
//            goUp = false;
//        }
//        else if (e.KeyCode == Keys.Down)
//        {
//            goDown = false;
//        }
//    }
//}
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Linq;
using System.Drawing;

public class Player
{
    public Image playerImage;
    public List<string> playerMovements = new List<string>();
    public List<string> idleMovements = new List<string>();
    public int steps = 0;
    public int slowDownFrameRate = 0;  // Giảm tốc độ animation khi di chuyển
    public int idleFrameRate = 0;      // Giảm tốc độ animation khi idle
    public bool goLeft, goRight, goUp, goDown;
    public int playerX = 100;
    public int playerY = 100;
    public int playerHeight = 50;
    public int playerWidth = 32;
    public int playerSpeed = 6; // Tốc độ di chuyển của player
    public string idleDirection = "Down";
    public int currentHealth = 100;
    public int maxHealth = 100;

    private List<PictureBox> obstacles;  // Danh sách chứa các vật cản (PictureBox)

    public Player(List<PictureBox> obstacles)
    {
        this.obstacles = obstacles;  // Khởi tạo danh sách vật cản
        LoadPlayerImages();
    }

    private void LoadPlayerImages()
    {
        LoadMovementImages("MoveDown");
        LoadIdleImages(idleDirection);
        if (playerMovements.Count > 0)
        {
            playerImage = Image.FromFile(playerMovements[0]);
        }
    }

    public void LoadMovementImages(string direction)
    {
        playerMovements.Clear();
        string movementFolder = $"Char_MoveMent/{direction}";  // Lấy thư mục theo hướng
        playerMovements = Directory.GetFiles(movementFolder, "*.png").ToList(); // Lấy các ảnh PNG trong thư mục
    }

    public void LoadIdleImages(string direction)
    {
        idleMovements.Clear();
        string idleFolder = $"Char_Idle/{direction}";
        idleMovements = Directory.GetFiles(idleFolder, "*.png").ToList();
    }

    public void ResetPlayer()
    {
        // Reset player trạng thái sau khi game reset
        playerX = 100;
        playerY = 100;
        currentHealth = 100;  // Đặt lại máu về 100
        this.playerSpeed = 6;  // Đặt lại tốc độ player
        goLeft = goRight = goUp = goDown = false;
        idleDirection = "Down";  // Đặt lại hướng idle mặc định
        steps = 0;              // Reset số bước animation
        slowDownFrameRate = 0;  // Reset frame rate chậm
        idleFrameRate = 0;      // Reset idle frame rate
    }

    // Phương thức điều chỉnh tốc độ animation khi di chuyển
    public void AnimatePlayer(int start, int end)
    {
        slowDownFrameRate += 1;  // Tăng dần tốc độ frame rate để tạo hiệu ứng chậm lại khi di chuyển
        if (slowDownFrameRate == 4)  // Điều chỉnh tốc độ ở đây (ví dụ: mỗi 5 ticks mới thay đổi frame)
        {
            steps++;
            slowDownFrameRate = 0;
        }

        // Đảm bảo rằng frame không vượt quá chỉ số
        if (steps > end || steps < start)
        {
            steps = start;
        }

        playerImage = Image.FromFile(playerMovements[steps]);
    }

    // Phương thức điều chỉnh tốc độ animation khi idle (không di chuyển)
    public void AnimateIdle(int start, int end)
    {
        if (idleMovements.Count > 0)
        {
            idleFrameRate++;  // Tăng dần frame rate cho trạng thái idle
            if (idleFrameRate >= 5)  // Điều chỉnh tốc độ idle animation (mỗi 5 ticks mới thay đổi frame)
            {
                steps++;
                if (steps > end)
                {
                    steps = start;
                }
                idleFrameRate = 0;
            }
            playerImage = Image.FromFile(idleMovements[steps % idleMovements.Count]);
        }
    }

    // Kiểm tra va chạm với các PictureBox
    private bool CheckCollisionWithObstacles(int newX, int newY)
    {
        Rectangle playerRect = new Rectangle(newX, newY, playerWidth, playerHeight);

        // Kiểm tra va chạm với từng PictureBox
        foreach (var obstacle in obstacles)
        {
            Rectangle obstacleRect = new Rectangle(obstacle.Location.X, obstacle.Location.Y, obstacle.Width, obstacle.Height);
            if (playerRect.IntersectsWith(obstacleRect))
            {
                return true;  // Có va chạm, không di chuyển
            }
        }
        return false;  // Không có va chạm, có thể di chuyển
    }

    public void Move()
    {
        // Cập nhật vị trí của player và hướng di chuyển
        if (goLeft)
        {
            if (!CheckCollisionWithObstacles(playerX - playerSpeed, playerY))
            {
                playerX -= playerSpeed;  // Di chuyển sang trái
                idleDirection = "Left";  // Cập nhật hướng idle
                LoadMovementImages("MoveLeft");
                AnimatePlayer(0, 5);  // Di chuyển từ frame 0 đến 5
            }
        }
        else if (goRight)
        {
            if (!CheckCollisionWithObstacles(playerX + playerSpeed, playerY))
            {
                playerX += playerSpeed;  // Di chuyển sang phải
                idleDirection = "Right";  // Cập nhật hướng idle
                LoadMovementImages("MoveRight");
                AnimatePlayer(0, 5);  // Di chuyển từ frame 0 đến 5
            }
        }
        else if (goUp)
        {
            if (!CheckCollisionWithObstacles(playerX, playerY - playerSpeed))
            {
                playerY -= playerSpeed;  // Di chuyển lên
                idleDirection = "Up";  // Cập nhật hướng idle
                LoadMovementImages("MoveUp");
                AnimatePlayer(0, 5);  // Di chuyển từ frame 0 đến 5
            }
        }
        else if (goDown)
        {
            if (!CheckCollisionWithObstacles(playerX, playerY + playerSpeed))
            {
                playerY += playerSpeed;  // Di chuyển xuống
                idleDirection = "Down";  // Cập nhật hướng idle
                LoadMovementImages("MoveDown");
                AnimatePlayer(0, 5);  // Di chuyển từ frame 0 đến 5
            }
        }
        else
        {
            LoadMovementImages(idleDirection);  // Nếu không di chuyển, chỉ tải ảnh idle
            AnimateIdle(0, 5);  // Animation idle nếu không di chuyển
        }
    }

    public void ResetMovementFlags()
    {
        goLeft = false;
        goRight = false;
        goUp = false;
        goDown = false;
    }

    public void KeyDownHandler(KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Left)
        {
            goLeft = true;
        }
        else if (e.KeyCode == Keys.Right)
        {
            goRight = true;
        }
        else if (e.KeyCode == Keys.Up)
        {
            goUp = true;
        }
        else if (e.KeyCode == Keys.Down)
        {
            goDown = true;
        }
    }

    public void KeyUpHandler(KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Left)
        {
            goLeft = false;
        }
        else if (e.KeyCode == Keys.Right)
        {
            goRight = false;
        }
        else if (e.KeyCode == Keys.Up)
        {
            goUp = false;
        }
        else if (e.KeyCode == Keys.Down)
        {
            goDown = false;
        }
    }
}
