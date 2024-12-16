namespace Project_Game
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.movementTimer = new System.Windows.Forms.Timer(this.components);
            this.healBar = new System.Windows.Forms.ProgressBar();
            this.gameOverTimer = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // movementTimer
            // 
            this.movementTimer.Enabled = true;
            this.movementTimer.Interval = 16;
            this.movementTimer.Tick += new System.EventHandler(this.TimerEvent);
            // 
            // healBar
            // 
            this.healBar.BackColor = System.Drawing.Color.Lime;
            this.healBar.ForeColor = System.Drawing.Color.LimeGreen;
            this.healBar.Location = new System.Drawing.Point(500, 24);
            this.healBar.Name = "healBar";
            this.healBar.Size = new System.Drawing.Size(158, 23);
            this.healBar.TabIndex = 1;
            this.healBar.Value = 100;
            // 
            // gameOverTimer
            // 
            this.gameOverTimer.Interval = 4000;
            this.gameOverTimer.Tick += new System.EventHandler(this.GameOverTimer_Tick);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(784, 591);
            this.Controls.Add(this.healBar);
            this.DoubleBuffered = true;
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(800, 630);
            this.MinimumSize = new System.Drawing.Size(800, 630);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Tag = "test";
            this.Text = "Form1";
            this.TopMost = true;
            this.TransparencyKey = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.Load += new System.EventHandler(this.Form1_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.FormPaintEvent);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.KeyIsDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.KeyIsUp);
            this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.FormMouseClick);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Timer movementTimer;
        private System.Windows.Forms.Timer gameOverTimer;
        private System.Windows.Forms.ProgressBar healBar;
    }
}

