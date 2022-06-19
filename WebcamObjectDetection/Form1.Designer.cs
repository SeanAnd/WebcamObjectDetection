
namespace WebcamObjectDetection
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.startBtn = new System.Windows.Forms.Button();
            this.cameraBox = new System.Windows.Forms.PictureBox();
            this.deviceDropdown = new System.Windows.Forms.ComboBox();
            this.recordingTimer = new System.Windows.Forms.Timer(this.components);
            this.staticFpsLabel = new System.Windows.Forms.Label();
            this.fpsLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.cameraBox)).BeginInit();
            this.SuspendLayout();
            // 
            // startBtn
            // 
            this.startBtn.Location = new System.Drawing.Point(457, 657);
            this.startBtn.Name = "startBtn";
            this.startBtn.Size = new System.Drawing.Size(94, 29);
            this.startBtn.TabIndex = 0;
            this.startBtn.Text = "Start";
            this.startBtn.UseVisualStyleBackColor = true;
            this.startBtn.Click += new System.EventHandler(this.startBtn_Click);
            // 
            // cameraBox
            // 
            this.cameraBox.Location = new System.Drawing.Point(12, 12);
            this.cameraBox.Name = "cameraBox";
            this.cameraBox.Size = new System.Drawing.Size(982, 605);
            this.cameraBox.TabIndex = 1;
            this.cameraBox.TabStop = false;
            // 
            // deviceDropdown
            // 
            this.deviceDropdown.FormattingEnabled = true;
            this.deviceDropdown.Location = new System.Drawing.Point(426, 623);
            this.deviceDropdown.Name = "deviceDropdown";
            this.deviceDropdown.Size = new System.Drawing.Size(151, 28);
            this.deviceDropdown.TabIndex = 2;
            // 
            // recordingTimer
            // 
            this.recordingTimer.Tick += new System.EventHandler(this.recordingTimer_Tick);
            // 
            // staticFpsLabel
            // 
            this.staticFpsLabel.AutoSize = true;
            this.staticFpsLabel.Location = new System.Drawing.Point(58, 645);
            this.staticFpsLabel.Name = "staticFpsLabel";
            this.staticFpsLabel.Size = new System.Drawing.Size(35, 20);
            this.staticFpsLabel.TabIndex = 3;
            this.staticFpsLabel.Text = "FPS:";
            // 
            // fpsLabel
            // 
            this.fpsLabel.AutoSize = true;
            this.fpsLabel.Location = new System.Drawing.Point(100, 645);
            this.fpsLabel.Name = "fpsLabel";
            this.fpsLabel.Size = new System.Drawing.Size(17, 20);
            this.fpsLabel.TabIndex = 4;
            this.fpsLabel.Text = "0";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1006, 721);
            this.Controls.Add(this.fpsLabel);
            this.Controls.Add(this.staticFpsLabel);
            this.Controls.Add(this.deviceDropdown);
            this.Controls.Add(this.cameraBox);
            this.Controls.Add(this.startBtn);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.cameraBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button startBtn;
        private System.Windows.Forms.PictureBox cameraBox;
        private System.Windows.Forms.ComboBox deviceDropdown;
        private System.Windows.Forms.Timer recordingTimer;
        private System.Windows.Forms.Label staticFpsLabel;
        private System.Windows.Forms.Label fpsLabel;
    }
}

