using System;
using System.Windows.Forms;

namespace Project_Game.Entities
{
    public class GameOver
    {
        private bool hasGameOverMessageShown = false;
        private Timer gameOverTimer;
        private Action resetGameAction;
        private Action invalidateAction;
        private Action invalidateActionaa;

        public GameOver(Timer gameOverTimer, Action resetGameAction, Action invalidateAction)
        {
            this.gameOverTimer = gameOverTimer;
            this.resetGameAction = resetGameAction;
            this.invalidateAction = invalidateAction;
        }

        // Kiểm tra xem người chơi đã thua chưa
        public void CheckGameOver(int currentHealth, ProgressBar healBar)
        {
            if (currentHealth <= 0)
            {
                if (!hasGameOverMessageShown)
                {
                    hasGameOverMessageShown = true;
                    //MessageBox.Show("Người chơi thua cuộc!", "Thua cuộc", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Bắt đầu timer để reset game sau một khoảng thời gian
                    gameOverTimer.Start();
                }
            }
        }

        // Reset lại trạng thái sau khi game over
        public void ResetGame()
        {
            hasGameOverMessageShown = false;  // Đặt lại cờ để có thể hiển thị lại thông báo game over
            resetGameAction();  // Gọi phương thức reset game
            invalidateAction(); // Vẽ lại giao diện (invalidate)
        }
    }
}
