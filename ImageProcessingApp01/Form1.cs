using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImageProcessingApp01
{
    public partial class Form1 : Form
    {
        Bitmap loaded;
        Bitmap processed;
        BasicDIP dip;
      
        public Form1()
        {
            dip = new BasicDIP();

            InitializeComponent();
            InitializeDefaultImage();
        }

        public void InitializeDefaultImage()
        {
            String defaultImage = System.IO.Path.Combine(Application.StartupPath, "..\\..\\Static\\bear.jpg");
            if (System.IO.File.Exists(defaultImage))
            {
                loaded = new Bitmap(defaultImage);
                pictureBox1.Image = loaded;
            }
            else
            {
                return;
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();

        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            loaded = new Bitmap(openFileDialog1.FileName);
            pictureBox1.Image = loaded;
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileDialog1.ShowDialog();
        }

        private void saveFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            processed.Save(saveFileDialog1.FileName);

        }


        // BASIC COPY
        private void pixelCopyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            processed = new Bitmap(loaded.Width, loaded.Height);
            dip.basicCopy(loaded, processed);
            pictureBox2.Image = processed;
        }

        // GRAYSCALING
        private void grayscaleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            processed = new Bitmap(loaded.Width, loaded.Height);
            dip.grayscale(loaded, processed);
            pictureBox2.Image = processed;
        }

        // INVERSION
        private void inversionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            processed = new Bitmap(loaded.Width, loaded.Height);
            dip.invert(loaded, processed);
            pictureBox2.Image = processed;
        }
        // MIRROR Y
        private void mirrorVerticalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            processed = new Bitmap(loaded.Width, loaded.Height);
            dip.mirrorY(loaded, processed);
            pictureBox2.Image = processed;
        }
        // MIRROR X
        private void mirrorHorizontalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            processed = new Bitmap(loaded.Width, loaded.Height);
            dip.mirrorX(loaded, processed);
            pictureBox2.Image = processed;
        }

        // SEPIA 
        private void sepiaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            processed = new Bitmap(loaded.Width, loaded.Height);
            dip.sepia(loaded, processed);
            pictureBox2.Image = processed;
        }

        // PUZZLIZE
        private void puzzlizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int width = loaded.Width;
            int height = loaded.Height;
            int midX = width / 2;
            int midY = height / 2;

            processed = new Bitmap(loaded.Width, loaded.Height);

            // UPPER LEFT QUADRANT: INVERSION
            dip.invert(loaded, processed, 0, 0, midX, midY);

            // UPPER RIGHT QUADRANT: GRAYSCALE
            dip.grayscale(loaded, processed, midX, 0, width, midY);

            // LOWER LEFT QUADRANT: BASIC COPY
            dip.basicCopy(loaded, processed, 0, midY, midX, height);

            // LOWER RIGHT QUADRANT: MIRROR Y
            dip.mirrorY(loaded, processed, midX, midY, width, height);


            pictureBox2.Image = processed;
        }

        private void histogramToolStripMenuItem_Click(object sender, EventArgs e)
        {
            processed = dip.histogram(loaded);
            pictureBox2.Image = processed;
        }
    }
}
