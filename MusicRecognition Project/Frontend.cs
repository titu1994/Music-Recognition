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
    public partial class Frontend : Form
    {
        private DatabaseHandler db;

        public Frontend()
        {
            InitializeComponent();
            db.InitDatabase();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);

            db.ReleaseDatabase();
        }
    }
}
