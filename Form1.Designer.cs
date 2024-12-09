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
            this.Test2 = new System.Windows.Forms.PictureBox();
            this.Test1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.Test2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Test1)).BeginInit();
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
            this.movementTimer.Interval = 33;
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
            // Test2
            // 
            this.Test2.Location = new System.Drawing.Point(424, 196);
            this.Test2.Name = "Test2";
            this.Test2.Size = new System.Drawing.Size(100, 50);
            this.Test2.TabIndex = 2;
            this.Test2.TabStop = false;
            // 
            // Test1
            // 
            this.Test1.Location = new System.Drawing.Point(616, 196);
            this.Test1.Name = "Test1";
            this.Test1.Size = new System.Drawing.Size(100, 50);
            this.Test1.TabIndex = 3;
            this.Test1.TabStop = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(946, 547);
            this.Controls.Add(this.Test1);
            this.Controls.Add(this.Test2);
            this.Controls.Add(this.healBar);
            this.Controls.Add(this.lblMovement);
            this.DoubleBuffered = true;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form1";
            this.TransparencyKey = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.FormPaintEvent);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.KeyIsDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.KeyIsUp);
            this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.FormMouseClick);
            ((System.ComponentModel.ISupportInitialize)(this.Test2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Test1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblMovement;
        private System.Windows.Forms.Timer movementTimer;
        private System.Windows.Forms.Timer gameOverTimer;
        private System.Windows.Forms.ProgressBar healBar;
        private System.Windows.Forms.PictureBox Test2;
        private System.Windows.Forms.PictureBox Test1;
    }
}

