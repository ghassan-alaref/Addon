using SAPbobsCOM;
using ST.Helper.B1_Objects;
using ST.Helper.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Create_UDFs
{
    public partial class Frm_Main : Form, IMainForms
    {

        Company company;

        public Frm_Main()
        {
            InitializeComponent();

            Start();
        }

        private void Start()
        {

        }


        private Company GetConnectedCompany()
        {
            Company company = new Company();

            company.Server = Txt_Server.Text;
            company.LicenseServer = Txt_LicenseServer.Text;
            company.DbUserName = Txt_DB_Username.Text;
            company.DbPassword = Txt_DB_Password.Text;
            company.UserName = Txt_B1_Username.Text;
            company.Password = Txt_B1_Password.Text;
            company.CompanyDB = Txt_CompanyDB.Text;
            company.DbServerType = BoDataServerTypes.dst_HANADB;

            if (company.Connect() != 0)
            {
                string X = company.GetLastErrorDescription();
                SetLogMessage("Error during connecting with the company, " + company.GetLastErrorDescription(), MessageLogType.Error);
                return null;
            }
            else
            {
                SetLogMessage($"Connected with the database {company.CompanyDB}");
                return company;
            }
        }




        public void SetLogMessage(string LogMessage, MessageLogType MessageType = MessageLogType.Info)
        {
            Tmr_Clear_Message.Stop();
            Tmr_Clear_Message.Enabled = false;
            string Msg = DateTime.Now.ToString("yyyy-MM-dd:HH:mm:ss") + " : " + LogMessage + Environment.NewLine;
            if (MessageType == MessageLogType.Error)
            {
                Stb_Message.ForeColor = Color.Red;
                LogMessage = "**ERROR** " + LogMessage;
            }
            else
            {
                Stb_Message.ForeColor = Color.Green;
            }
            Stb_Message.Text = LogMessage;
            Txt_Log.Text += Msg;
            System.Windows.Forms.Application.DoEvents();
            Tmr_Clear_Message.Enabled = true;
            Tmr_Clear_Message.Start();
        }

        private void Btn_Connect_Click(object sender, EventArgs e)
        {
            try
            {
                //ST.Helper.Utility.Val(Program.Product_ID);
            }
            catch (Exception ex)
            {
                SetLogMessage(ex.Message, MessageLogType.Error);
                return;
            }
            ST.Helper.MetaDataOperater.UserFields.Main_Form = (IMainForms)this;
            SetLogMessage("Connecting with B1 Database");
            company = GetConnectedCompany();

            if (company != null)
            {
                try
                {
                    ST.KHCF.Customization.Logic.Objects_Logic.All_UDO_Definition = null;
                    ST.KHCF.Customization.Logic.Objects_Logic.All_UDT_Definition = null;
                    ST.KHCF.Customization.Logic.Fields_Logic.All_Field_Definition = null;
                    ST.KHCF.Customization.MetaDataOperator.Creator.CreateAll(company);
                }
                catch (Exception ex)
                {
                    SetLogMessage(ex.Message, MessageLogType.Error);
                }
            }
            else
            {
                return;
            }

            //if (key.)
            //{

            //}

            Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"SOFTWARE\ST_MetaData", true);
            if (key == null)
            {
                key = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(@"SOFTWARE\ST_MetaData");
            }
            key.SetValue("Server", Txt_Server.Text);
            key.SetValue("B1_Username", Txt_B1_Username.Text);
            key.SetValue("CompanyDB", Txt_CompanyDB.Text);
            key.SetValue("DB_Username", Txt_DB_Username.Text);
            key.SetValue("LicenseServer", Txt_LicenseServer.Text);
            key.SetValue("Save_Passwords", Chk_Save_Password.Checked);

            if (Chk_Save_Password.Checked == true)
            {
                key.SetValue("DB_Password", Txt_DB_Password.Text);
                key.SetValue("B1_Password", Txt_B1_Password.Text);
            }

            SetLogMessage("Done!");

        }


        internal static BoFieldTypes Get_B1_DataType(string Type_Text)
        {
            switch (Type_Text)
            {
                case "A":
                    return BoFieldTypes.db_Alpha;
                case "D":
                    return BoFieldTypes.db_Date;
                case "M":
                    return BoFieldTypes.db_Memo;
                case "N":
                    return BoFieldTypes.db_Numeric;
                case "B":
                    return BoFieldTypes.db_Float;
                default:
                    throw new Exception($"Data Type{Type_Text} is not supported");
            }
        }


        private void Frm_Main_FormClosed(object sender, FormClosedEventArgs e)
        {
            System.Windows.Forms.Application.Exit();
        }


        private void Frm_Main_Load(object sender, EventArgs e)
        {
            Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"SOFTWARE\ST_MetaData", true);

            try
            {
                Txt_Server.Text = key.GetValue("Server").ToString();
                Txt_B1_Username.Text = key.GetValue("B1_Username").ToString();
                Txt_CompanyDB.Text = key.GetValue("CompanyDB").ToString();
                Txt_DB_Username.Text = key.GetValue("DB_Username").ToString();
                Txt_LicenseServer.Text = key.GetValue("LicenseServer").ToString();
                bool Save_Password;
                bool.TryParse(key.GetValue("Save_Passwords").ToString(), out Save_Password);
                Chk_Save_Password.Checked = Save_Password;

                if (Chk_Save_Password.Checked == true)
                {
                    Txt_DB_Password.Text = key.GetValue("DB_Password").ToString();
                    Txt_B1_Password.Text = key.GetValue("B1_Password").ToString();
                }

            }
            catch { }

        }


        private void Tmr_Clear_Message_Tick(object sender, EventArgs e)
        {
            Stb_Message.Text = "";
            Tmr_Clear_Message.Enabled = false;
        }

   
    }
}
