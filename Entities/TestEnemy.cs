using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace Project_Game.Entities
{
    public class TestEnemy : Enemy
    {
        // Constructor cho TestEnemy, thay đổi đường dẫn baseFolderPath thành "test"
        public TestEnemy() : base("Char_MoveMent")
        {
        }

        // Ghi đè phương thức LoadEnemyImages nếu cần cấu hình riêng
        protected override void LoadEnemyImages(string baseFolderPath)
        {
            // Sử dụng đường dẫn "test" cho các hình ảnh
            //baseFolderPath = "Char_MoveMent";
            LoadEnemyDirectionImages("Left", enemyMovementsLeft, baseFolderPath);
            LoadEnemyDirectionImages("Right", enemyMovementsRight, baseFolderPath);
            LoadEnemyDirectionImages("Up", enemyMovementsUp, baseFolderPath);
            LoadEnemyDirectionImages("Down", enemyMovementsDown, baseFolderPath);

            if (enemyMovementsDown.Count > 0)
            {
                enemyImage = Image.FromFile(enemyMovementsDown[0]);
            }
        }

        // Bạn có thể ghi đè các phương thức khác như AnimateEnemy, Move nếu cần hành vi khác
        public override void AnimateEnemy(int start, int end)
        {
            base.AnimateEnemy(start, end);
            // Thêm logic hoạt ảnh đặc biệt cho TestEnemy (nếu cần)
        }

        public override void Move(int playerX, int playerY, int screenWidth, int screenHeight, List<GameObject> obstacles)
        {
            base.Move(playerX, playerY, screenWidth, screenHeight, obstacles);
            // Thêm logic di chuyển riêng cho TestEnemy (nếu cần)
        }
    }
}
