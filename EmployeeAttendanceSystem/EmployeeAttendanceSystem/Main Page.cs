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


namespace WindowsFormsApplication3
{
    public partial class Form1 : MetroFramework.Forms.MetroForm                              //  Inheriting metro framework
    {

        
        public Form1()
        {
            InitializeComponent();                                                            //Initializing Form
        }

        private void metroTile1_Click(object sender, EventArgs e)                            //Click Event For Register page
        {
            using (Registration frm = new Registration())
            {
                frm.ShowDialog();                                                              //Showing Registration Page in the form of popup

            }
        }

        private void metroTile2_Click(object sender, EventArgs e)                            //Click Event For Scan QRcode page
        {

            
            using (SCANNING_BARCODE_QRCODE_USING_WEBCAM.Form3 frm1 = new SCANNING_BARCODE_QRCODE_USING_WEBCAM.Form3())
            {


                frm1.ShowDialog();                                                              //Showing QR Scan Page in the form of popup

            }
        }

        private void metroTile3_Click(object sender, EventArgs e)                             //Click Event For Report page
        {
            using (Reporting frm4 = new Reporting())
            {


                frm4.ShowDialog();                                                            //Showing Report Page in the form of popup

            }
        }
    }
}