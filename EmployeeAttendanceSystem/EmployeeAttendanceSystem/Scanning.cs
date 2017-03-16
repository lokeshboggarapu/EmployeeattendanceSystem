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
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Text.RegularExpressions;
using System.Configuration;

namespace SCANNING_BARCODE_QRCODE_USING_WEBCAM
{

    
    public partial class Form3 : Form
    {
        
        

        SoundPlayer player;                                                                       //Initializing Sound player to play beep sound
        public Form3()
        {
            InitializeComponent();
        }
        private VideoCaptureDevice FinalFrame;                                                    //Initializing Video Capture Device
        private FilterInfoCollection CaptureDevice;
        public int i;

        private void FinalFrame_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            try
            {
                pictureBox1.Image = (Bitmap)eventArgs.Frame.Clone();                            //Code to clone the image in Picture box
            }
            catch
            {

            }
            // pictureBox1.Image = eventArgs.Frame.Clone() as Bitmap;
            // pictureBox1.Image = (Bitmap)eventArgs.Frame.Clone();   
        }
        
                            

                     
        private void button1_Click(object sender, EventArgs e)                                  //Button Click Event to select the camera
        {
            FinalFrame = new VideoCaptureDevice(CaptureDevice[comboBox1.SelectedIndex].MonikerString);  
            FinalFrame.NewFrame += new NewFrameEventHandler(FinalFrame_NewFrame);
            FinalFrame.Start();
            checkBox1.Checked = false;                                                         //After action has been completed it removes the check on the check button
        }
        private void WebcamScanner_Load(object sender, EventArgs e)                            //Starting Web cam Event  to start web cam on the system.
        {
            player = new SoundPlayer();
            CaptureDevice = new FilterInfoCollection(FilterCategory.VideoInputDevice);          //Initializing the capture device
            foreach (FilterInfo Device in CaptureDevice)
            {
                comboBox1.Items.Add(Device.Name);                                                //Adding Device Name in the combo box
            }
            comboBox1.SelectedIndex = 0;
            FinalFrame = new VideoCaptureDevice(CaptureDevice[comboBox1.SelectedIndex].MonikerString);
            FinalFrame.NewFrame += new NewFrameEventHandler(FinalFrame_NewFrame);
            FinalFrame.Start();                                                                    //To start the frame

            pictureBox1.Focus();
            timer2.Start();                                                                         //Starting timer after scanner has been loaded
        }
        private void button2_Click(object sender, EventArgs e)                                      //To start Scanning Manually
        {
            timer1.Enabled = true;                                                                    //To Enable the timer
            timer1.Start();                                                                           //To start the timer
        }

        private void button3_Click(object sender, EventArgs e)                                      //Quit Button click event 
        {
            timer1.Stop();                                                                         //To stop the timer 
            if (FinalFrame.IsRunning == true)
            {
                FinalFrame.Stop();                                                                   //To stop scanning from frame
                FinalFrame.NewFrame -= new NewFrameEventHandler(FinalFrame_NewFrame);
            }
            this.Close();                                                                             //To close the window
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)                         //Check Box Change Event function
        {
            if (checkBox1.Checked == true)                                                       //Check box is  checked
            {
                comboBox1.Visible = true;                                                        //Combo box will be visible
                button1.Visible = true;                                                          //select button will be visible
            }
            if (checkBox1.Checked == false)                                                        //Check box is  unchecked
            {
                comboBox1.Visible = false;                                                       //Combo box will  not be visible
                button1.Visible = false;                                                         //select button will not be visible
            }
        }
        public string decoded;
        private SqlConnection con;                                                                   //Initializing Sql Connection
        private SqlCommand cmd;                                                                       //Initializing Sql Command for queries
      //  private static string conStr = @"Data Source=LOKESH-VAIO\SQLEXPRESS;Initial Catalog=EmployeeAttendance;Integrated Security=false";
        private void timer1_Tick(object sender, EventArgs e)                                       //function for timer tick event
        {
            
            BarcodeReader Reader = new BarcodeReader();                                           //Object creation for barcodereader class
            Result result = Reader.Decode((Bitmap)pictureBox1.Image);                             //Storing result into result string
            if (result != null)
            {
                decoded = result.ToString().Trim();                                               //To remove spaces from string
                string[] lines = Regex.Split(decoded, "\r\n");                                    //To split the string based on new line characters
                string[] name = Regex.Split(lines[0], ":");                                       //split lines based on  colon     
                string Empname=name[1];
                string[] EID = Regex.Split(lines[2], ":");                                        //split lines based on  colon
                string EmpID = EID[1];
                if (decoded != "")                                                                //If we get something after decoding
                {
                    try
                    {
                        string s = ConfigurationManager.ConnectionStrings["orcl"].ConnectionString;         //Creating Connection string to connect to database
                  
                        using (con = new SqlConnection())
                        {
                            con.ConnectionString = s;
                            con.Open();                                                                           //Opening Connection to database
                            string query = "insert into employee_checkin(NAME,ID,CHECKINDTTIME,CREATEDDATE,CREATEDATE) values(@name,@id,@checkindttime,@createddate,@createdate)";        //Inserting to database using insert command
                            cmd = new SqlCommand(query, con);                                                       // Initializing command
                            cmd.CommandType = CommandType.Text;
                            MemoryStream stream = new MemoryStream();
                            cmd.Parameters.AddWithValue("@name", Empname);                                             //Adding value to the variable  Empname 
                            cmd.Parameters.AddWithValue("@id", Convert.ToInt64(EmpID));
                            cmd.Parameters.AddWithValue("@checkindttime", Convert.ToDateTime(System.DateTime.Now));
                            cmd.Parameters.AddWithValue("@createddate", Convert.ToDateTime(System.DateTime.Now));
                            cmd.Parameters.AddWithValue("@createdate", Convert.ToDateTime(System.DateTime.Now));
                            cmd.ExecuteNonQuery();                                                                    //Execute query
                            cmd.Dispose();                                                                             // Dispose Connection and query
                            con.Close();                                                                               //Closing Connection
                        }
                        
                    }
                    catch (Exception ex)                                                                              //Block to catch Exception
                    {
 MessageBox.Show(ex.ToString());

                        }
                    player.SoundLocation = "C:\\beep-01a.wav";                                                       //location where sound file is located
                    //C:\Users\lokesh\Documents\visual studio 2012\Projects\WindowsFormsApplication2\WindowsFormsApplication2\SoundFiles\beep-01a
                    player.Play();                                                                                   //command to play the sound file
                    timer1.Stop();                                                                                   //command to stop the timer
                    if(FinalFrame.IsRunning == true)                                                                //If final frame is running
                    {
                        FinalFrame.Stop();                                                                           //command to stop it
                        FinalFrame.NewFrame -= new NewFrameEventHandler(FinalFrame_NewFrame);                        



                        MessageBox.Show("Hola "+ Empname +" Checkedin Successfully");                               //Message Box to show Employee has checked in
                    }
                    this.Close();                                                                                   //To close the message box
                    player.Stop();                                                                                  //To stop the player sound
                }
            }
            button2.PerformClick();  
        }

        private void timer2_Tick(object sender, EventArgs e)                                                  //If the image is not yet scanned
        {
            i++;                                                                                               //Increment the loop
             
            if (pictureBox1.Image != null)                                                                       //if we have something in the picture box
            {
                timer1.Start();                                                                                  //To start the timer
                timer2.Stop();                                                                                    //To stop the timer    
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

    }
}
