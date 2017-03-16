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
using Zen.Barcode;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Configuration;
 
namespace WindowsFormsApplication3
{
    public partial class Registration : MetroFramework.Forms.MetroForm                                  //  Inheriting metro framework
    {
        private SqlConnection con;                                                               //Initializing Sql Connection
        private SqlCommand cmd;                                                                  //Initializing Sql Command for queries
       
       // private SqlDataTable dr;
        public Registration()
        {
            InitializeComponent();
            generateautonumber();
        }



        private void button1_Click(object sender, EventArgs e)                                       //Click Event For Register button
        {
            try
            {




                Zen.Barcode.CodeQrBarcodeDraw barcode = Zen.Barcode.BarcodeDrawFactory.CodeQr;        //Initializing QR code
                String Qrcode = null;
                Qrcode += "Name:" + textBox1.Text + "\r\n";                                            //Taking input Values to Convert it to QR Code
                Qrcode += "Age:" + textBox2.Text + "\r\n";
                Qrcode += "EmpId:" + textBox3.Text + "\r\n";
                string totalQRcode = Qrcode;                                                           //Converting QR code to string 
                pictureBox1.Image = barcode.Draw(totalQRcode, 50);                                     //Embeding QR code image into picturebox 
                string name = Convert.ToString(textBox1.Text);
                int age = Convert.ToInt32(textBox2.Text);
                int EmpID = Convert.ToInt32(textBox3.Text);

               using( con = new SqlConnection(@"Data Source=LOKESH-VAIO\SQLEXPRESS;Initial Catalog=EmployeeAttendance;Integrated Security=True;"))      //Creating Connection string to connect to database
               {
                con.Open();                                                                                        //Opening Connection to database
              
                string query = "insert into employee(NAME,AGE,ID,IMAGE) values(@name,@age,@ID,@pic)";               //Inserting to database using insert command
                cmd = new SqlCommand(query, con);                                                                   // Initializing command
                cmd.CommandType = CommandType.Text;
                MemoryStream stream = new MemoryStream();                                                            //Creating memory stream object
                pictureBox1.Image.Save(stream, System.Drawing.Imaging.ImageFormat.Jpeg);                             //Saving image from picture box
                byte[] pic = stream.ToArray();
                SqlParameter imageParameter = new SqlParameter("@pic", SqlDbType.Image);                             //Saving image into image format into database
                //imageParameter.Value = DBNull.Value;
                //cmd.Parameters.Add(imageParameter);
                cmd.Parameters.AddWithValue("@pic", SqlDbType.Image).Value = pic;                                      //Adding value to the variable  pic 
                cmd.Parameters.AddWithValue("@name", name);
                cmd.Parameters.AddWithValue("@age", Convert.ToInt32(age));
                cmd.Parameters.AddWithValue("@ID", Convert.ToInt32(EmpID));
               // cmd.Parameters.AddWithValue("@pic", "pic");
                cmd.ExecuteNonQuery();                                                                                 //Execute query 
                cmd.Dispose();                                                                                         // Dispose Connection and query
                con.Close();                                                                                           //Closing Connection
            }
            }
            catch (Exception ex)                                                                                        //Block to catch Exception
            {
                MessageBox.Show(ex.ToString());
            }



        }

        private void generateautonumber()                                                                                   //Generate Employee ID function  
        {

            SqlConnection congen = new SqlConnection(ConfigurationManager.ConnectionStrings["orcl"].ToString());             //Creating Connection string to connect to database
            congen.Open();                                                                                                   //Opening Connection to database
       
            string query = "Select count(ID) from employee";                                                                  //selecting from  database using insert command
            SqlCommand cmdgen = new SqlCommand(query, congen);
            
            cmdgen.CommandType = CommandType.Text;
            int i =Convert.ToInt32(cmdgen.ExecuteScalar()) ;
            congen.Close();                                                                                                    //Closing Connection
            i++;
            textBox3.Text = Convert.ToString(i);

        }

    }
}
