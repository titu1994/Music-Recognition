using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NAudio.Wave;

namespace MusicRecognition_Project
{
    public partial class WaveForm : Form
    {
        public double[,] highscores, recordpoints;
        public string filename;

        public WaveForm()
        {
            InitializeComponent();
        }

        private void LoadWavButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog o = new OpenFileDialog();
            o.Filter = @"Wave File (*.wav)|*.wav";
            o.InitialDirectory = Application.CommonAppDataPath + @"\WAV\";

            if (o.ShowDialog() != DialogResult.OK) return;

            customWaveViewer1.WaveStream = new WaveFileReader(o.FileName);
            customWaveViewer1.FitToSize();
        }

        private void LoadWav()
        {
            customWaveViewer1.WaveStream = new WaveFileReader(filename);
            customWaveViewer1.FitToSize();
        }
        
    }
}
