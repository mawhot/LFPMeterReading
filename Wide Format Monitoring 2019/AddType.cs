using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SQLite;
using System.IO;
using System.Configuration;

namespace Wide_Format_Monitoring_2019
{
    public partial class AddType : Form
    {
        public string ReturnValue1 { get; set; }
        public string ReturnValue2 { get; set; }
        private SQLiteConnection sql_con;
        private SQLiteCommand sql_cmd;
        private SQLiteDataAdapter DB;
        DataSet DataSetType = new DataSet();
        DataTable DataTableType = new DataTable();

        private void SetConnection()

        {
            string filesqlpath = AppDomain.CurrentDomain.BaseDirectory;
            sql_con = new SQLiteConnection("Data Source=" + filesqlpath + ConfigurationManager.AppSettings["database"]);
        }
        private void ListCommon()
        {
            
            SetConnection();
            sql_con.Open();
            sql_cmd = sql_con.CreateCommand();
            string CommandText1 = "Select * FROM library order by conversion"; 
            DB = new SQLiteDataAdapter(CommandText1, sql_con);
            DataSetType.Reset();
            DB.Fill(DataSetType);
            DataTableType = DataSetType.Tables[0];
            sql_con.Close();
            foreach(DataRow rowtype in DataTableType.Rows)
            {
                comboBoxCommon.Items.Add(rowtype["conversion"]);
            }
            
            
        }

        private void ExecuteQuery(string txtQuery)
        {
            SetConnection();
            sql_con.Open();
            sql_cmd = sql_con.CreateCommand();
            sql_cmd.CommandText = txtQuery;
            sql_cmd.ExecuteNonQuery();
            sql_con.Close();
        }
        public AddType(string TransfertType)
        {
            InitializeComponent();
            ListCommon();
            textBoxTypeDetected.Text = TransfertType;
        }

        private void AddType_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(textBoxNew.Text == "")
            {
                ExecuteQuery("INSERT INTO library (name,conversion) VALUES ('" + textBoxTypeDetected.Text + "','" + comboBoxCommon.SelectedItem.ToString() + "')");
                this.ReturnValue1 = comboBoxCommon.SelectedItem.ToString();
                
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                int n = 0;
                foreach (DataRow rowtype in DataTableType.Rows)
                {
                    if (textBoxNew.Text == rowtype["conversion"].ToString())
                    {
                        n = 1;
                        break;
                    }
                }
                if (n == 0)
                {
                    var val = checkBox1.Checked ? 1 : 0;
                    ExecuteQuery("INSERT INTO library (name,conversion) VALUES ('" + textBoxTypeDetected.Text + "','" + textBoxNew.Text + "')");
                    ExecuteQuery("INSERT INTO type (canon_name,comment,manufacturer,color) VALUES ('" + textBoxNew.Text + "','" + textBoxComment.Text + "','" + textBoxManufacturer.Text + "','" + val + "')");
                    this.ReturnValue1 = textBoxNew.Text;
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("This type already exist, please select it in the above list.");
                    textBoxNew.Text = "";

                    
                }
                    
                
            }

          

        }

        private void comboBoxCommon_SelectedIndexChanged(object sender, EventArgs e)
        {
           
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }
    }
}
