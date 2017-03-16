using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AForge;
using AForge.Video;
using System.Media;
using AForge.Video.DirectShow;
using ZXing;
using ZXing.QrCode;
using System.IO;

namespace WindowsFormApplication3
{
    public partial class Form3 : MetroFramework.Forms.MetroForm
    {
        SoundPlayer player;
        public Form3()
        {
            InitializeComponent();
        }
        private VideoCaptureDevice FinalFrame;
        private FilterInfoCollection CaptureDevice;
        public int i;

        private void FinalFrame_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            try
            {
                pictureBox1.Image = (Bitmap)eventArgs.Frame.Clone();
            }
            catch
            {

            }
            // pictureBox1.Image = eventArgs.Frame.Clone() as Bitmap;
            // pictureBox1.Image = (Bitmap)eventArgs.Frame.Clone();   
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FinalFrame = new VideoCaptureDevice(CaptureDevice[comboBox1.SelectedIndex].MonikerString);
            FinalFrame.NewFrame += new NewFrameEventHandler(FinalFrame_NewFrame);
            FinalFrame.Start();
            checkBox1.Checked = false;
        }
        private void WebcamScanner_Load(object sender, EventArgs e)
        {
            player = new SoundPlayer();
            CaptureDevice = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            foreach (FilterInfo Device in CaptureDevice)
            {
                comboBox1.Items.Add(Device.Name);
            }
            comboBox1.SelectedIndex = 0;
            FinalFrame = new VideoCaptureDevice(CaptureDevice[comboBox1.SelectedIndex].MonikerString);
            FinalFrame.NewFrame += new NewFrameEventHandler(FinalFrame_NewFrame);
            FinalFrame.Start();

            pictureBox1.Focus();
            timer2.Start();
        }
        private void button2_Click(object sender, EventArgs e)
        {
            timer1.Enabled = true;
            timer1.Start();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            timer1.Stop();
            if (FinalFrame.IsRunning == true)
            {
                FinalFrame.Stop();
                FinalFrame.NewFrame -= new NewFrameEventHandler(FinalFrame_NewFrame);
            }
            this.Close();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true)
            {
                comboBox1.Visible = true;
                button1.Visible = true;
            }
            if (checkBox1.Checked == false)
            {
                comboBox1.Visible = false;
                button1.Visible = false;
            }
        }
        public string decoded;
        private void timer1_Tick(object sender, EventArgs e)
        {
            BarcodeReader Reader = new BarcodeReader();
            Result result = Reader.Decode((Bitmap)pictureBox1.Image);
            if (result != null)
            {
                decoded = result.ToString().Trim();

                if (decoded != "")
                {
                    player.SoundLocation = "C:\\beep-01a.wav";
                    //C:\Users\lokesh\Documents\visual studio 2012\Projects\WindowsFormsApplication2\WindowsFormsApplication2\SoundFiles\beep-01a
                    player.Play();
                    timer1.Stop();
                    if (FinalFrame.IsRunning == true)
                    {
                        FinalFrame.Stop();
                        FinalFrame.NewFrame -= new NewFrameEventHandler(FinalFrame_NewFrame);
                        MessageBox.Show(decoded);
                    }
                    this.Close();
                    player.Stop();
                }
            }
            button2.PerformClick();
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            i++;

            if (pictureBox1.Image != null)
            {
                timer1.Start();
                timer2.Stop();
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

    }
}
