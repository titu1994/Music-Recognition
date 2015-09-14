namespace MusicRecognition_Project
{
    partial class WaveForm
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
            this.LoadWavButton = new System.Windows.Forms.Button();
            this.customWaveViewer1 = new MusicRecognition_Project.CustomWaveViewer();
            this.SuspendLayout();
            // 
            // LoadWavButton
            // 
            this.LoadWavButton.Location = new System.Drawing.Point(12, 3);
            this.LoadWavButton.Name = "LoadWavButton";
            this.LoadWavButton.Size = new System.Drawing.Size(75, 23);
            this.LoadWavButton.TabIndex = 0;
            this.LoadWavButton.Text = "Load WAV";
            this.LoadWavButton.UseVisualStyleBackColor = true;
            this.LoadWavButton.Click += new System.EventHandler(this.LoadWavButton_Click);
            // 
            // customWaveViewer1
            // 
            this.customWaveViewer1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.customWaveViewer1.Location = new System.Drawing.Point(0, 45);
            this.customWaveViewer1.Name = "customWaveViewer1";
            this.customWaveViewer1.SamplesPerPixel = 128;
            this.customWaveViewer1.Size = new System.Drawing.Size(784, 216);
            this.customWaveViewer1.StartPosition = ((long)(0));
            this.customWaveViewer1.TabIndex = 2;
            this.customWaveViewer1.WaveStream = null;
            // 
            // WaveForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(784, 261);
            this.Controls.Add(this.customWaveViewer1);
            this.Controls.Add(this.LoadWavButton);
            this.Name = "WaveForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "WaveForm";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button LoadWavButton;
        private CustomWaveViewer customWaveViewer1;
    }
}