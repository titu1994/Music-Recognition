using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MusicRecognition_Project
{
    public partial class Frontend : Form, AudioManager.AudioManagerCallbacks
    {
        private DatabaseHandler db;
        private AudioManager audioManager;

        public Frontend()
        {
            InitializeComponent();
            db = new DatabaseHandler();
            db.InitDatabase();

            audioManager = new AudioManager(this);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);

            db.ReleaseDatabase();
        }

        public void DisplaySpectrum(double[,] highscores, double[,] recordPoints)
        {
            string filename = audioManager.CreateWAVFile(audioManager.GetAllSamples());

            WaveForm wf = new WaveForm();
            wf.filename = filename;
            wf.highscores = highscores;
            wf.recordpoints = recordPoints;
            wf.Show();
        }
    }
}
