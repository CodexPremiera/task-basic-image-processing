using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Linq.Expressions;
using AForge.Video;
using AForge.Video.DirectShow;
using static System.Net.Mime.MediaTypeNames;


namespace ImageProcessingApp01
{
    public partial class Form1 : Form
    {
        Bitmap loaded, processed, image1, image2, subtracted, vfxRes;
        BasicDIP dip;

        FilterInfoCollection fic;
        VideoCaptureDevice videoCam;

        int videoEffectsIndex = 0;
        String[] videoEffects =
        {
            "Subtract",
            "Pixel Copy",
            "GrayScale",
            "Invert",
            "Mirror Horizontal",
            "Mirror Vertical",
            "Sepia"
        };
        bool isProcessing = false;


        // INITIALIZATIONS

        public Form1()
        {
            dip = new BasicDIP();

            InitializeComponent();
            InitializeDefaultImage();
            InitializeVideoCameras();
            InitializeVideoEffects();
        }

        public void InitializeDefaultImage()
        {
            try
            {
                String defaultLoaded = Path.Combine(System.Windows.Forms.Application.StartupPath, "..\\..\\Static\\bear.jpg");
                String defaultImage1 = Path.Combine(System.Windows.Forms.Application.StartupPath, "..\\..\\Static\\foreground.jpg");
                String defaultImage2 = Path.Combine(System.Windows.Forms.Application.StartupPath, "..\\..\\Static\\background.jpg");

                if (File.Exists(defaultLoaded))
                    pictureBox1.Image = loaded = new Bitmap(defaultLoaded);

                if (File.Exists(defaultImage1))
                    image1PictureBox.Image = image1 = new Bitmap(defaultImage1);

                if (File.Exists(defaultImage2))
                    image2PictureBox.Image = image2 = new Bitmap(defaultImage2);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading default images: {ex.Message}");
            }
        }

        public void InitializeVideoCameras()
        {
            fic = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            videoCam = new VideoCaptureDevice();
            foreach (FilterInfo dev in fic)
            {
                comboBox1.Items.Add(dev.Name);
                comboBox1.SelectedIndex = 0;
            }
        }

        public void InitializeVideoEffects()
        {
            foreach (String s in videoEffects)
            {
                comboBox2.Items.Add(s);
            }
            comboBox2.SelectedIndex = 0;

            btnGetColor.BackColor = colorDialog1.Color;
        }



        // OPEN AND SAVE PHOTO

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



        // DIGITAL IMAGE PROCESSING ALGORITHMS

        // SUBTRACTION
        private void btnSubtract_Click(object sender, EventArgs e)
        {
            subtractedPictureBox.Image = subtracted = dip.subtract(image1, image2, colorDialog1.Color);
        }

        private void btnGetColor_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                btnGetColor.BackColor = colorDialog1.Color;
            }
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

        // HISTOGRAM
        private void histogramToolStripMenuItem_Click(object sender, EventArgs e)
        {
            processed = dip.histogram(loaded);
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
            
            dip.invert(loaded, processed, 0, 0, midX, midY);            // UPPER LEFT QUADRANT: INVERSION
            dip.grayscale(loaded, processed, midX, 0, width, midY);     // UPPER RIGHT QUADRANT: GRAYSCALE            
            dip.basicCopy(loaded, processed, 0, midY, midX, height);    // LOWER LEFT QUADRANT: BASIC COPY
            dip.mirrorY(loaded, processed, midX, midY, width, height);  // LOWER RIGHT QUADRANT: MIRROR Y

            pictureBox2.Image = processed;
        }

        
        // ADVANCE PAGE

        // ADD IMAGES
        private void image1PictureBox_Click(object sender, EventArgs e)
        {
            openImage1Dialog.ShowDialog();
        }
        private void openImage1Dialog_FileOk(object sender, CancelEventArgs e)
        {
            image1 = new Bitmap(openImage1Dialog.FileName);
            image1PictureBox.Image = image1;
        }

        private void image2PictureBox_Click(object sender, EventArgs e)
        {
            openImage2Dialog.ShowDialog();
        }
        private void openImage2Dialog_FileOk(object sender, CancelEventArgs e)
        {
            image2 = new Bitmap(openImage2Dialog.FileName);
            image2PictureBox.Image = image2;
        }


        // USE CAMERA
        private void stopCamera()
        {
            videoCam.SignalToStop();
            videoCam.WaitForStop();
        }

        private void useCamera()
        {
            if (checkBox1.Checked)
            {
                stopCamera();
                videoCam = new VideoCaptureDevice(fic[comboBox1.SelectedIndex].MonikerString);
                videoCam.NewFrame += FinalFrame_NewFrame;
                videoCam.Start();
            }
            else
            {
                stopCamera();
            }
        }

        private void FinalFrame_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            image1 = (Bitmap)eventArgs.Frame.Clone();

            switch (videoEffectsIndex)
            {
                case 0:
                    subtracted = dip.subtract(image1, image2, colorDialog1.Color);
                    break;
                case 1:
                    subtracted = dip.basicCopy(image1);
                    break;
                case 2:
                    subtracted = dip.grayscale(image1);
                    break;
                case 3:
                    subtracted = dip.invert(image1);
                    break;
                case 4:
                    subtracted = dip.mirrorX(image1);
                    break;
                case 5:
                    subtracted = dip.mirrorY(image1);
                    break;
                case 6:
                    subtracted = dip.sepia(image1);
                    break;
            }

            image1PictureBox.Image = image1;
            subtractedPictureBox.Image = subtracted;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            useCamera();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            useCamera();
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            videoEffectsIndex = comboBox2.SelectedIndex;
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            stopCamera();
            base.OnFormClosing(e);
        }

    }
}
