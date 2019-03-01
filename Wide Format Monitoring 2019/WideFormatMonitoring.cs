using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;
using System.Data.SQLite;
using SnmpSharpNet;
using System.IO;
using System.Net;
using System.Collections;
using System.Diagnostics;
using System.Globalization;

namespace Wide_Format_Monitoring_2019
{

   
    public partial class Form1 : Form
    { TreeNode parentNode = null;
        private SQLiteConnection sql_con;
        private SQLiteCommand sql_cmd;
        private SQLiteDataAdapter DB;
        private string communitySNMP;
        DataSet DataSetLoad = new DataSet();
        DataTable DataTableLoad = new DataTable();

        private void SetConnection()

        {
            string filesqlpath = AppDomain.CurrentDomain.BaseDirectory;
            sql_con = new SQLiteConnection("Data Source=" + filesqlpath + ConfigurationManager.AppSettings["database"]);
        }

 
           

     

        public Form1()
        {
            InitializeComponent();
           
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DiscoverDevice discover = new DiscoverDevice();
            discover.Show();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            TreeNode parentNode= new TreeNode();
            parentNode = null;
            ViewTree(0, parentNode);
                
        }

        
        private void ViewTree(int level, TreeNode parentNode)
        {
            SetConnection();
            sql_con.Open();
            DataTable dt = new DataTable();
            DataSet ds = new DataSet();
            DataTable dt1 = new DataTable();
            DataSet ds1 = new DataSet();
            sql_cmd = sql_con.CreateCommand();
            string query = "SELECT * FROM department where id_parent="+ level;
            DB = new SQLiteDataAdapter(query, sql_con);
            ds.Reset();
            DB.Fill(ds);
            dt = ds.Tables[0];
            int TotalFleet = dt.Rows.Count;
           
            sql_con.Close();
            TreeNode childNode = new TreeNode();
            childNode = null;

            foreach (DataRow dr in dt.Rows)
            {
                 
            if (parentNode==null) {
            

                childNode = treeViewList.Nodes.Add(dr["department"].ToString());
                  
                }
                else
                {
                   
                    childNode = parentNode.Nodes.Add(dr["department"].ToString());
                }
                
                ViewTree(Convert.ToInt32(dr["id_department"].ToString()), childNode);
                TotalFleet = TotalFleet - 1;

            }


        }

        private void treeViewList_AfterSelect(object sender, TreeViewEventArgs e)
        {

        }
        

    }

    }

