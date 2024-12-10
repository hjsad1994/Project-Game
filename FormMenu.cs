using System;
using System.Windows.Forms;

namespace Project_Game
{
    public partial class FormMenu : Form
    {
        public FormMenu()
        {
            InitializeComponent();
        }

        private void FormMenu_Load(object sender, EventArgs e)
        {
            // Thiết lập giao diện form menu
            this.Text = "Kute Fantasy - Menu";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = System.Drawing.Color.LightBlue;

            // Nút "Chơi"
            Button btnPlay = new Button
            {
                Text = "Chơi",
                Size = new System.Drawing.Size(200, 50),
                Location = new System.Drawing.Point(150, 100)
            };
            btnPlay.Click += BtnPlay_Click;
            this.Controls.Add(btnPlay);

            // Nút "Thoát"
            Button btnExit = new Button
            {
                Text = "Thoát",
                Size = new System.Drawing.Size(200, 50),
                Location = new System.Drawing.Point(150, 200)
            };
            btnExit.Click += BtnExit_Click;
            this.Controls.Add(btnExit);
        }
        private void BtnPlay_Click(object sender, EventArgs e)
        {
            // Mở Form1 (game) và ẩn form menu
            Form1 gameForm = new Form1();
            gameForm.Show();
            this.Hide();
        }

        private void BtnExit_Click(object sender, EventArgs e)
        {
            // Thoát chương trình
            Application.Exit();
        }
    }
}
