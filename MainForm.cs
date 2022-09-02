using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Newtonsoft.Json;
using System.Runtime.Serialization;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.IO.Ports;


namespace FinTriageGUI
{

    public partial class MainFrm : Form
    {
       
       private string id { get; set; }
       private string tmp { get; set; }

        public bool onClick = true;
	

       public void UsersInfos()
       {
                imgProcessing.Visible = true;
                using (HttpClient client = new HttpClient())
                {
                    var APIurl = new Uri("https://test.ucnmtriage.com/updateform/temperature");

                    var userdatas = new userData
                    {
                        id = this.id,
                        temperature = this.tmp

                    };
                    
                    var toJson = JsonConvert.SerializeObject(userdatas);
                    var payload = new StringContent(toJson, Encoding.UTF8, "application/json");

                    var result = client.PutAsync(APIurl, payload).Result;
                    var json = result.Content.ReadAsStringAsync().Result;
                    
                    try
                    {
                        var dataInfo = JsonConvert.DeserializeObject<Root>(json);

                        if (dataInfo.message.Equals("student has symptoms"))
                        {
                            imgProcessing.Visible = false;
                            MainTab.SelectedIndex = 5;                                                                                                                                                                                                                                                                                                                                                                                                                                                                                           
                            timerSymptoms.Enabled = true;
                            serialPort.Close();
                            this.id = String.Empty;
                            this.tmp = String.Empty;
                            txtGetID.Text = String.Empty;
                            txtGetTmp.Text = String.Empty;
                            //timerCntr.Enabled = true;                      
                        }

                       else if (dataInfo.data.dataUser.Equals("Unregistered"))
                       {
                            imgProcessing.Visible = false;
                            MainTab.SelectedIndex = 2;
                            timerError.Enabled = true;
                            serialPort.Close();
                            this.id = String.Empty;
                            this.tmp = String.Empty;
                            txtGetID.Text = String.Empty;
                            txtGetTmp.Text = String.Empty;
                            //timerCntr.Enabled = true;
                       }
                       else if (double.Parse(dataInfo.data.temperature) >= 37.5)
                       {
                                imgProcessing.Visible = false;
                                lblTemp.Text = dataInfo.data.temperature.ToString();
                                MainTab.SelectedIndex = 4;
                                timerTmp.Enabled = true;
                                serialPort.Close();
                                this.id = String.Empty;
                                this.tmp = String.Empty;
                                txtGetID.Text = String.Empty;
                                txtGetTmp.Text = String.Empty;
                                //timerCntr.Enabled = true;                       
                       }
                       else
                       {
                                imgProcessing.Visible = false;
                                MainTab.SelectedIndex = 1;
                                timerSuccess.Enabled = true;
                            
                                txtFname.Text = dataInfo.data.firstname;
                                txtLname.Text = dataInfo.data.lastname;
                                txtMiddleName.Text = dataInfo.data.mi;
                                txtClassification.Text = dataInfo.data.classfication;
                                txtDate.Text = dataInfo.data.date;
                                txtStatus.Text = dataInfo.data.dataUser;
                                txtTemperature.Text = dataInfo.data.temperature;
                                txtNoTimes.Text = dataInfo.data.timesIn.ToString();

                                serialPort.WriteLine("8000");
                                serialPort.Close();

                                this.id = String.Empty;
                                this.tmp = String.Empty;
                                txtGetID.Text = String.Empty;
                                txtGetTmp.Text = String.Empty;                       
                       }
    
                    }
                    catch (HttpRequestException err)
                    {
                        Console.WriteLine(err.Message);
                    }                

                }
         
       }

        public MainFrm()
        {
            InitializeComponent();          
        }
      
        private void MainFrm_Load(object sender, EventArgs e)
        {
           
            this.WindowState = FormWindowState.Maximized;        
            MainTab.Appearance = TabAppearance.FlatButtons;
            MainTab.ItemSize = new Size(0, 1);
            MainTab.SizeMode = TabSizeMode.Fixed;
            // testing();
            //MainTab.SelectedIndex = 5;
        }


        private void pnlInfo_AutoSizeChanged(object sender, EventArgs e)
        {
            pnlInfo.Location = new Point(
               this.ClientSize.Width / 2 - this.pnlInfo.Size.Width / 2,
               this.ClientSize.Height / 2 - this.pnlInfo.Size.Height / 2);
            this.pnlInfo.Anchor = AnchorStyles.None;
        }
        private void pnlTempError_AutoSizeChanged(object sender, EventArgs e)
        {
            pnlTempError.Location = new Point(
                this.ClientSize.Width / 2 - this.pnlInfo.Size.Width / 2,
                this.ClientSize.Height / 2 - this.pnlInfo.Size.Height / 2);
            this.pnlTempError.Anchor = AnchorStyles.None;
        }

        private void pnlMainImg_AutoSizeChanged(object sender, EventArgs e)
        {
            pnlMainImg.Location = new Point(
                this.ClientSize.Width / 2 - this.pnlInfo.Size.Width / 2,
                this.ClientSize.Height / 2 - this.pnlInfo.Size.Height / 2);
            this.pnlMainImg.Anchor = AnchorStyles.None;
        }
        private void pnlSymptoms_AutoSizeChanged(object sender, EventArgs e)
        {
            pnlSymptoms.Location = new Point(
                    this.ClientSize.Width / 2 - this.pnlSymptoms.Width / 2,
                    this.ClientSize.Height / 2 - this.pnlSymptoms.Height / 2
                );
            this.pnlSymptoms.Anchor = AnchorStyles.None;
        }


        private void timerError_Tick(object sender, EventArgs e)
        {
            if(timerError.Enabled)
            {
                MainTab.SelectedIndex = 0;
                timerError.Enabled = false;
                timerCntr.Enabled = true;
            }
        }

        private void timerSuccess_Tick(object sender, EventArgs e)
        {
          
            if(timerSuccess.Enabled)
            {
                MainTab.SelectedIndex = 3;
                timerDisplay.Enabled = true;
                timerSuccess.Enabled = false;             
            }
        }
  
        private void timerCntr_Tick(object sender, EventArgs e)
        {

           
            if (timerCntr.Enabled)
            {          
                serialPort.Open();             

                try
                {                                   
                    string data = serialPort.ReadLine();
                    string[] infos = data.Split('|');
                        
                    txtGetID.Text = infos[0].Remove(infos[0].Length-1,1);
                    txtGetTmp.Text = infos[1];                                    
                }
                catch (Exception ie)
                {
                    MessageBox.Show(ie.Message);
                    serialPort.Close();
                }

                if (txtGetID.Text.Equals(String.Empty) && txtGetTmp.Text.Equals(String.Empty))
                {
                      MainTab.SelectedIndex = 0;
                }
                else
                {                   
                    timerCntr.Enabled = false;
                    btnSubmit.PerformClick();
                }
            }
        }

        private void timerSymptoms_Tick(object sender, EventArgs e)
        {
            if (timerSymptoms.Enabled)
            {
                MainTab.SelectedIndex = 0;
                timerSymptoms.Enabled = false;
                timerCntr.Enabled = true;
            }
        }

        private void timerDisplay_Tick(object sender, EventArgs e)
        {
            if (timerDisplay.Enabled)
            {
                MainTab.SelectedIndex = 0;
                timerDisplay.Enabled = false;
                timerCntr.Enabled = true;
            }
        }

        private void timerTmp_Tick(object sender, EventArgs e)
        {
            if(timerTmp.Enabled)
            {
                MainTab.SelectedIndex = 0;
                timerTmp.Enabled = false;
                timerCntr.Enabled = true;
            }
        }
       
        private void btnSubmit_Click(object sender, EventArgs e)
        {
              this.id = txtGetID.Text; this.tmp = txtGetTmp.Text;
              UsersInfos();
        }
    }
}
