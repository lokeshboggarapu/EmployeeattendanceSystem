using System;
using System.Collections.Generic;
using System.IO;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Reflection;
using iTextSharp.text.pdf;
using iTextSharp.text;
using Excel = Microsoft.Office.Interop.Excel; 


namespace WindowsFormsApplication3
{
    public partial class Reporting : MetroFramework.Forms.MetroForm
    {
        public Reporting()
        {
            InitializeComponent();
        }
        private SqlConnection con;                                                                                                                                         //Initializing Sql Connection
        private void button1_Click(object sender, EventArgs e)                                                                                                             //Report Button click event to populate data into datagrid
        {
            try
            {
                string s = ConfigurationManager.ConnectionStrings["orcl"].ConnectionString;                                                                               //Creating Connection string to connect to database

                using (con = new SqlConnection())
                {
                    con.ConnectionString = s;
                    con.Open();                                                                                                                                             //Opening Connection to database
                    DataTable dt = new DataTable();                                                                                                                        //Creating the new datatable
                    if (textBox1.Text.Length > 0)                                                                                                                          //To get data based on name
                    {
                        SqlDataAdapter da = new SqlDataAdapter("Select * from Employee_Checkin where (name like '%" + textBox1.Text + "%' or name='') ", con);             //Query to get data based on name
                        da.Fill(dt);                                                                                                                                       //Filling data into datatable
                    }
                    else if (textBox2.Text.Length > 0)                                                                                                                     //To get data based on name or id
                    {
                        SqlDataAdapter da = new SqlDataAdapter("Select * from Employee_Checkin where (id like '%" + textBox2.Text + "%' or name='') ", con);               //Query to get data based on name
                        da.Fill(dt);                                                                                                                                       //Filling data into datatable
                    }
                    else if (textBox1.Text.Length > 0 && textBox2.Text.Length > 0)                                                                                                                          // To get data based on name and id                                                                                          
                    { 
                        SqlDataAdapter da = new SqlDataAdapter("Select * from Employee_Checkin where (name like '%" + textBox2.Text + "%' or name='') and (  id= '" + textBox2.Text + " ' ) ", con);        //Query to get data based on name and id
                        da.Fill(dt);                                                                                                                                                                        //Filling data into datatable
                    }
                    else if ((dateTimePicker1.Text.Length > 0 && dateTimePicker2.Text.Length > 0))                                                                                                          // To get data based on checkin and checkout timings
                    {
                        SqlDataAdapter da = new SqlDataAdapter("Select * from Employee_Checkin where createdate between '" + dateTimePicker1.Value.ToString("yyyy-MM-dd") + "' and '" + dateTimePicker2.Value.ToString("yyyy-MM-dd") + "' ", con);        //Query to get data based on on checkin and checkout timings
                        da.Fill(dt);                                                                                                                                                                         //Filling data into datatable
                    }
                    else if ((dateTimePicker1.Text.Length > 0 && dateTimePicker2.Text.Length > 0) && textBox1.Text.Length > 0)                                                                                                                            // To get data based on checkin and checkout timings and name
                    {
                        SqlDataAdapter da = new SqlDataAdapter("Select * from Employee_Checkin where createdate between '" + dateTimePicker1.Value.ToString("yyyy-MM-dd") + "' and '" + dateTimePicker2.Value.ToString("yyyy-MM-dd") + "' and (  name= '" + textBox1.Text + " ' ) ", con);    //Query to get data based on on checkin and checkout timings and name
                        da.Fill(dt);                                                                                                                                                                        //Filling data into datatable
                    }
                    else if ((dateTimePicker1.Text.Length > 0 && dateTimePicker2.Text.Length > 0) && textBox2.Text.Length > 0)                                                                                // To get data based on checkin and checkout timings and ID
                    {
                        SqlDataAdapter da = new SqlDataAdapter("Select * from Employee_Checkin where createdate between '" + dateTimePicker1.Value.ToString("yyyy-MM-dd") + "' and '" + dateTimePicker2.Value.ToString("yyyy-MM-dd") + "' and (  id= '" + textBox2.Text + " ' ) ", con);     //Query to get data based on on checkin and checkout timings and ID
                        da.Fill(dt);                                                                                                                                                                      //Filling data into datatable
                    }
                    dataGridView1.DataSource = dt;                                                                  //datasource from dt datatable for gridview
                    con.Close();                                                                                    //Closing Connection
                }

            }

            catch( Exception ex)                                                                                  //Exception Block
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void button2_Click(object sender, EventArgs e)  //Function to export data to PDF
        {
            
                PdfPTable pdfTable = new PdfPTable(dataGridView1.ColumnCount);         //Counting columns that has to be exported to pdf
                pdfTable.DefaultCell.Padding = 3;                                       //Giving Default cell padding to 3
                pdfTable.WidthPercentage = 30;                                          //Giving width percentage to 30
                pdfTable.HorizontalAlignment = Element.ALIGN_LEFT;                      //Aligning to left
                pdfTable.DefaultCell.BorderWidth = 1;                                     //Giving Default border width to 1

                //Adding Header row
                foreach (DataGridViewColumn column in dataGridView1.Columns)                //Getting data from each column of data grid  
                {
                    PdfPCell cell = new PdfPCell(new Phrase(column.HeaderText));            //creating new object for pdf cell class
                    cell.BackgroundColor = new iTextSharp.text.BaseColor(240, 240, 240);    
                    pdfTable.AddCell(cell);                                                 //Adding new cell to the table
                }


                foreach (DataGridViewRow row in dataGridView1.Rows)                          //Adding DataRow
                {
                    foreach (DataGridViewCell cell in row.Cells)
                    {
                        if (cell.Value != null)                                               //If we have some data in the cell
                        {
                            pdfTable.AddCell(cell.Value.ToString());                          //then it is going to add cell in the pdf table
                        }
                    }
                }
                string date = Convert.ToString(DateTime.Now);                               //Converting datetime to string
                //Exporting to PDF
                string folderPath = "C:\\PDFs\\";                                             //Saving pdf to the path 
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);                                    //If path does not exists create the path and folder
                }
                using (FileStream stream = new FileStream(folderPath + ".pdf", FileMode.Create))   //using file stream to load into pdf and adding .pdf format type
                {
                    Document pdfDoc = new Document(PageSize.A2, 10f, 10f, 10f, 0f);
                    PdfWriter.GetInstance(pdfDoc, stream);                                        //Creating instance for pdf writer
                    pdfDoc.Open();                                                                //OPening Pdfdoc
                    pdfDoc.Add(pdfTable);                                                         //Adding Pdf doc to the pdf table        
                    pdfDoc.Close();                                                               //Closing the pdf doc
                    stream.Close();                                                               //Closing the stream
                }
        }

        private void copyAlltoClipboard()   //copying Data to clipboard function 
        {
            dataGridView1.SelectAll();        //Selecting the datagrid view
            DataObject dataObj = dataGridView1.GetClipboardContent();  //Getting data from datagridview
            if (dataObj != null)
                Clipboard.SetDataObject(dataObj);
        }

        private void button3_Click(object sender, EventArgs e)           //Button to export it to excel
        {
            copyAlltoClipboard();                                        //Calling function Copytoclipboard
            Microsoft.Office.Interop.Excel.Application xlexcel;           //To create excel
            Microsoft.Office.Interop.Excel.Workbook xlWorkBook;           //To create Workbook
            Microsoft.Office.Interop.Excel.Worksheet xlWorkSheet;         //TO create Work Sheet
            object misValue = System.Reflection.Missing.Value; 
            xlexcel = new Excel.Application();                           //Creeating new xlexcel object  for excel class
            xlexcel.Visible = true;                                        //To make visible to true
            xlWorkBook = xlexcel.Workbooks.Add(misValue);
            xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1); //To add items to excel sheet
            Excel.Range CR = (Excel.Range)xlWorkSheet.Cells[1, 1];
            CR.Select();
            xlWorkSheet.PasteSpecial(CR, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, true);//To paste the items which are on clip board.          

        }

        

    }
}
