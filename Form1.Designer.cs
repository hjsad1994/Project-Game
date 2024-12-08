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
            this.lblMovement = new System.Windows.Forms.Label();
            this.movementTimer = new System.Windows.Forms.Timer(this.components);
            this.healBar = new System.Windows.Forms.ProgressBar();
            this.gameOverTimer = new System.Windows.Forms.Timer(this.components);
            this.Test1 = new System.Windows.Forms.PictureBox();
            this.Test2 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.Test1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Test2)).BeginInit();
            this.SuspendLayout();
            // 
            // lblMovement
            // 
            this.lblMovement.AutoSize = true;
            this.lblMovement.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMovement.Location = new System.Drawing.Point(12, 477);
            this.lblMovement.Name = "lblMovement";
            this.lblMovement.Size = new System.Drawing.Size(127, 25);
            this.lblMovement.TabIndex = 0;
            this.lblMovement.Text = "Movement:";
            // 
            // movementTimer
            // 
            this.movementTimer.Enabled = true;
            this.movementTimer.Interval = 20;
            this.movementTimer.Tick += new System.EventHandler(this.TimerEvent);
            // 
            // healBar
            // 
            this.healBar.BackColor = System.Drawing.Color.Lime;
            this.healBar.ForeColor = System.Drawing.Color.LimeGreen;
            this.healBar.Location = new System.Drawing.Point(955, 21);
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
            // Test1
            // 
            this.Test1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.Test1.BackgroundImage = global::Project_Game.Properties.Resources.House_1_1;
            this.Test1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.Test1.Location = new System.Drawing.Point(274, 186);
            this.Test1.Name = "Test1";
            this.Test1.Size = new System.Drawing.Size(83, 118);
            this.Test1.TabIndex = 2;
            this.Test1.TabStop = false;
            // 
            // Test2
            // 
            this.Test2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.Test2.BackgroundImage = global::Project_Game.Properties.Resources.House_1_1;
            this.Test2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.Test2.Location = new System.Drawing.Point(734, 195);
            this.Test2.Name = "Test2";
            this.Test2.Size = new System.Drawing.Size(83, 118);
            this.Test2.TabIndex = 3;
            this.Test2.TabStop = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 561);
            this.Controls.Add(this.Test2);
            this.Controls.Add(this.Test1);
            this.Controls.Add(this.healBar);
            this.Controls.Add(this.lblMovement);
            this.DoubleBuffered = true;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form1";
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.FormPaintEvent);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.KeyIsDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.KeyIsUp);
            this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.FormMouseClick);
            ((System.ComponentModel.ISupportInitialize)(this.Test1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Test2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblMovement;
        private System.Windows.Forms.Timer movementTimer;
        private System.Windows.Forms.Timer gameOverTimer;
        private System.Windows.Forms.ProgressBar healBar;
        private System.Windows.Forms.PictureBox Test1;
        private System.Windows.Forms.PictureBox Test2;
    }
}

