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
    public partial class DiscoverDevice : Form
    {
        List<string> culturelist = new List<string>();
        CultureInfo[] cultures = CultureInfo.GetCultures(CultureTypes.SpecificCultures);
        RegionInfo region;
        public string TransfertType { get; set; }
        private SQLiteConnection sql_con;
        private SQLiteCommand sql_cmd;
        private SQLiteDataAdapter DB;
        private string communitySNMP;
        
        private void SetConnection()
         
        {
            string filesqlpath = AppDomain.CurrentDomain.BaseDirectory;
            sql_con = new SQLiteConnection("Data Source=" + filesqlpath + ConfigurationManager.AppSettings["database"]);
        }

        public void LoadComboBox(string query)
        {
            SetConnection();
            sql_con.Open();

            DataSet DataSetLoad = new DataSet();
            DataTable DataTableLoad = new DataTable();
            sql_cmd = sql_con.CreateCommand();
           
            DB = new SQLiteDataAdapter(query, sql_con);
            DataSetLoad.Reset();
            DB.Fill(DataSetLoad);
            DataTableLoad = DataSetLoad.Tables[0];
            int TotalFleet = DataTableLoad.Rows.Count;
            sql_con.Close();
            foreach (DataRow rowLoad in DataTableLoad.Rows)
            {
                

            }
        }

        public void WriteToFile(string Message)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "\\Logs";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string filepath = AppDomain.CurrentDomain.BaseDirectory + "\\Logs\\DiscoveryLog_" + DateTime.Now.Date.ToShortDateString().Replace('/', '_') + ".txt";
            if (!File.Exists(filepath))
            {
                // Create a file to write to.   
                using (StreamWriter sw = File.CreateText(filepath))
                {
                    sw.WriteLine(Message);
                }
            }
            else
            {
                using (StreamWriter sw = File.AppendText(filepath))
                {
                    sw.WriteLine(Message);
                }
            }

        }
        public DiscoverDevice()
        {
            InitializeComponent();
           
        }

        private void DiscoverDevice_Load(object sender, EventArgs e)
        {
            SetConnection();
 comboBoxBuilding.Hide();
            comboBoxCity.Hide();
            comboBoxFloor.Hide();
            textBoxOther.Hide();
            foreach(CultureInfo culture in cultures)
            {
                region = new RegionInfo(culture.LCID);
                if (!(culturelist.Contains(region.EnglishName)))
                {
                    culturelist.Add(region.EnglishName);
                    comboBoxCountry.Items.Add(region.EnglishName);

                }
            }
        }
        public void ReadSNMP()
        {
            using (System.IO.StreamReader file =
                            new System.IO.StreamReader(AppDomain.CurrentDomain.BaseDirectory + "/community.txt", true))
            {
                communitySNMP = file.ReadToEnd();

            }
        }
            private void SnmpReading()
        {
       

                //LoadData();
                //WriteToFile(TotalFleet + "  Control Total when launch Monitor Fleet");
                ReadSNMP();
                // WriteToFile(communitySNMP + "   This is the community read");
               

                string IPAddressMonitor = textBoxIP.Text.ToString();

                    if (IPAddressMonitor != null)
                    {
                        // SNMP community name
                        OctetString community = new OctetString(communitySNMP);
                        // Define agent parameters class
                        AgentParameters param = new AgentParameters(community);
                        // Set SNMP version to 1 (or 2)
                        param.Version = SnmpVersion.Ver1;
                        // Construct the agent address object
                        // IpAddress class is easy to use here because
                        //  it will try to resolve constructor parameter if it doesn't
                        //  parse to an IP address
                        IpAddress agent = new IpAddress(IPAddressMonitor);
                        UdpTarget target = new UdpTarget((IPAddress)agent, 161, 2000, 1);
                        Pdu pdu = new Pdu(PduType.Get);
                       SetConnection();
                       sql_con.Open();

                       DataSet DataSetType0 = new DataSet();
                       DataTable DataTableType0 = new DataTable();
                       sql_cmd = sql_con.CreateCommand();
                       string CommandText = "Select * FROM oid WHERE id_type = 0";
                       DB = new SQLiteDataAdapter(CommandText, sql_con);
                       DataSetType0.Reset();
                       DB.Fill(DataSetType0);
                       DataTableType0 = DataSetType0.Tables[0];
                       int TotalFleet = DataTableType0.Rows.Count;
                       sql_con.Close();
                       foreach (DataRow rowType0 in DataTableType0.Rows)
                        {
                            pdu.VbList.Add(rowType0["oid"].ToString());
                            
                        }
                       
                        SnmpV1Packet result = null;
                        try
                        {
                            result = (SnmpV1Packet)target.Request(pdu, param) as SnmpV1Packet;
                        }
                        catch (Exception ex)
                        {
                            WriteToFile(DateTime.Now + " - " + ex);
                        }
                        finally
                        {
                            if (result == null)
                            {
                               
                            }
                            // If result is null then agent didn't reply or we couldn't parse the reply.
                            if (result != null)
                            {
                                // ErrorStatus other then 0 is an error returned by 
                                // the Agent - see SnmpConstants for error definitions
                                if (result.Pdu.ErrorStatus != 0)
                                {
                                    // agent reported an error with the request
                                    WriteToFile(DateTime.Now + "   " + IPAddressMonitor + "       " + result.Pdu.ErrorStatus + "   " + result.Pdu.ErrorIndex);
                                }
                                else
                                {
                                textBoxsysDescr.Text = result.Pdu.VbList[0].Value.ToString();
                            
                                textBoxsysUptime.Text = result.Pdu.VbList[1].Value.ToString();
                                textBoxsysContact.Text = result.Pdu.VbList[2].Value.ToString();
                                textBoxsysName.Text = result.Pdu.VbList[3].Value.ToString();
                                textBoxsysLocation.Text = result.Pdu.VbList[4].Value.ToString();


                            SetConnection();
                            sql_con.Open();

                            sql_cmd = sql_con.CreateCommand();
                            string CommandText1 = "Select * FROM library WHERE name= '"+ result.Pdu.VbList[0].Value.ToString()+"'";
                    
                            DB = new SQLiteDataAdapter(CommandText1, sql_con);
                            DataSetType0.Reset();
                            
                            DB.Fill(DataSetType0);
                            DataTableType0 = DataSetType0.Tables[0];
                            int TotalType = 0;
                            TotalType = DataTableType0.Rows.Count;
                            sql_con.Close();
                            if(TotalType == 0)
                            {
                                MessageBox.Show("This device type does not exist in our database.");
                                TransfertType = result.Pdu.VbList[0].Value.ToString();
                                using (var newtype = new AddType(TransfertType))
                                {
                                    var result1 = newtype.ShowDialog();
                                    if (result1 == DialogResult.OK)
                                    {
                                        string val = newtype.ReturnValue1;
                                        textBoxType.Text = val;
                                    }
                                }

                               
                            }

                            foreach (DataRow rowType0 in DataTableType0.Rows)
                            {
                                textBoxType.Text = rowType0["conversion"].ToString();

                            }
                        }
                                   
                                
                            }

                            target.Close();
                        }


                    }

                
            }

        private void btn_discover_Click(object sender, EventArgs e)
        {
            SnmpReading();
        }

      

        private void button2_Click(object sender, EventArgs e)
        {

        }
    }
}
