using SAPbobsCOM;
using SAPbouiCOM;
using ST.KHCF.Customization.Logic.Classes;
using ST.KHCF.Customization.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;
using ST.KHCF.Customization.Forms.CCI;
using System.Runtime.CompilerServices;
using System.Drawing;

namespace ST.KHCF.Customization.Forms.Fundraising
{
    internal class Frm_Fundraising_Individual_Card : Parent_Form
    {
        internal static SAPbobsCOM.Company company;
        internal static Application SBO_Application;
        internal static Parent_Form Form_Obj;
        internal static string[] Relations_Grid_IDs = new string[] { "902","904" };
        private static List<Utility.Item_UI> Grids_List = new List<Utility.Item_UI>();
        internal static Dictionary<string, string> Flags_List = new Dictionary<string, string>() { 
            { "850" , "862"},
            { "851" , "863"},
            { "852" , "864"},
            { "853" , "865"},
            { "854" , "866"},
            { "855" , "867"},
            { "856" , "868"},
            { "857" , "869"},
            { "858" , "870"},
            { "859" , "871"},
            { "860" , "872"},
            { "873" , "875"}
        };

        internal override void Initialize_Form(Form form)
        {

            try
            {
                base.Initialize_Form(form);
                string SQL_Customer_Group = $@"SELECT ""GroupCode"" AS ""Code"", T0.""GroupName"" AS ""Name"" FROM OCRG T0 
WHERE  T0.""GroupType""  = 'C' AND T0.""U_ST_TYPE"" = 'I' AND U_ST_CUSTOMER_TYPE = 'F'";
                Helper.Utility.Fill_One_ComboBoxBySQL(company, form, "302", SQL_Customer_Group, true);

                string Fundraising_Department_ID = Configurations.Get_Fundraising_Department(company);
                string SQL_Account_Manager = $@"SELECT T0.""SlpCode"" AS ""Code"", T0.""SlpName"" AS ""Name"" 
FROM OSLP T0  INNER JOIN OHEM T1 ON T0.""SlpCode"" = T1.""salesPrson"" 
WHERE T1.""dept"" in ({Fundraising_Department_ID})";
                Helper.Utility.Fill_One_ComboBoxBySQL(company, form, "825", SQL_Account_Manager, true);

                Helper.Utility.Fill_One_ComboBoxBySQL(company, form, "826", SQL_Account_Manager, true);

                Grid Grd_Members = (Grid)form.Items.Item("138").Specific;
                ((ComboBoxColumn)Grd_Members.Columns.Item("ST_GENDER")).ValidValues.Add("M", "Male");
                ((ComboBoxColumn)Grd_Members.Columns.Item("ST_GENDER")).ValidValues.Add("F", "Female");
                ((ComboBoxColumn)Grd_Members.Columns.Item("ST_GENDER")).DisplayType = BoComboDisplayType.cdt_Description;
                Grd_Members.Columns.Item("SELECTED").AffectsFormMode = false;

                Helper.Utility.FillGridComboBoxForSQL(company, Grd_Members, "ST_CUSTOMER_GROUP", SQL_Customer_Group, true);
                string SQL_Relation_To_Father = $@"SELECT ""Code"", ""Name"" from ""@ST_RELATION_FATHER""";
                Helper.Utility.FillGridComboBoxForSQL(company, Grd_Members, "ST_RELATIONSHIP_TO_FATHER", SQL_Relation_To_Father, true);
                ((ComboBoxColumn)Grd_Members.Columns.Item("ST_REGION")).ValidValues.Add("L", "Local");
                ((ComboBoxColumn)Grd_Members.Columns.Item("ST_REGION")).ValidValues.Add("R", "Regional");
                ((ComboBoxColumn)Grd_Members.Columns.Item("ST_REGION")).ValidValues.Add("I", "International");
                ((ComboBoxColumn)Grd_Members.Columns.Item("ST_REGION")).DisplayType = BoComboDisplayType.cdt_Description;

                string SQL_Title = $@"SELECT T0.""Code"", T0.""Name"" FROM ""@ST_PREFIX"" T0";
                Helper.Utility.Fill_One_ComboBoxBySQL(company, form, "326", SQL_Title, true);




                ButtonCombo Btn_Cmb_Action = (ButtonCombo)form.Items.Item("163").Specific;
                Btn_Cmb_Action.ValidValues.Add("-", "Can Also");
                Btn_Cmb_Action.ValidValues.Add("R", "Remove");
                //Btn_Cmb_Action.ValidValues.Add("L", "Link");
                //Btn_Cmb_Action.ValidValues.Add("U", "Unlink");
                form.DataSources.UserDataSources.Item("163").Value = "-";

                ButtonCombo Btn_Print_Action = (ButtonCombo)form.Items.Item("165").Specific;
                Btn_Print_Action.ValidValues.Add("-", "Print");
                Btn_Print_Action.ValidValues.Add("L", "Label");
                Btn_Print_Action.ValidValues.Add("C", "Certificate");
                Btn_Print_Action.ValidValues.Add("T", "Tax Report");
                form.DataSources.UserDataSources.Item("165").Value = "-";

                form.Items.Item("164").Visible = false;
                form.Items.Item("163").AffectsFormMode = false;
                form.Items.Item("165").AffectsFormMode = false;
                //form.Items.Item("143").AffectsFormMode = false;
                //form.Items.Item("145").AffectsFormMode = false;
                form.Items.Item("901").TextStyle = 4;
                form.Items.Item("903").TextStyle = 4;

                form.Items.Item("164").Visible = false;
                // Matrix Mat_Cont = (Matrix)form.Items.Item("834").Specific;
                //Frm_CCI_Corporate_Member_Card.Fill_Position_ComboBox(Mat_Cont);

                Grd_Members.AutoResizeColumns();

                KHCF_Logic_Utility.Set_Individua_Fund_Chosse_From_List_Basic_Condition(form, "CFL_6");
                KHCF_Logic_Utility.Set_Corporate_Fund_Chosse_From_List_Basic_Condition(form, "CFL_PARENT_CORP");

                string SQL_User = $@"Select U_ST_HAVE_PATIENT_FLAG_ACCESS from OUSR WHERE USER_CODE = '{company.UserName}'";
                Recordset RC_User = Helper.Utility.Execute_Recordset_Query(company, SQL_User);
                if (RC_User.Fields.Item("U_ST_HAVE_PATIENT_FLAG_ACCESS").Value.ToString() != "Y")
                {
                    //form.Items.Item("855").Visible = false;
                    //form.Items.Item("Item_15").Visible = false;
                    //form.Items.Item(Flags_List["855"]).Visible = false;
                    //form.Items.Item("855").SetAutoManagedAttribute(BoAutoManagedAttr.ama_Visible, -1, BoModeVisualBehavior.mvb_False);
                    //form.Items.Item("Item_15").SetAutoManagedAttribute(BoAutoManagedAttr.ama_Visible, -1, BoModeVisualBehavior.mvb_False);
                    //form.Items.Item(Flags_List["855"]).SetAutoManagedAttribute(BoAutoManagedAttr.ama_Visible, -1, BoModeVisualBehavior.mvb_False);
                    //form.Items.Item("855").SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, -1, BoModeVisualBehavior.mvb_False);
                    //form.Items.Item(Flags_List["855"]).SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, -1, BoModeVisualBehavior.mvb_False);
                    //form.Items.Item("855").FromPane = 100;
                    //form.Items.Item("Item_15").FromPane = 100;
                    //form.Items.Item(Flags_List["855"]).FromPane = 100;
                    //form.Items.Item("855").ToPane = 100;
                    //form.Items.Item("Item_15").ToPane = 100;
                    //form.Items.Item(Flags_List["855"]).ToPane = 100;

                    string[] Patient_Items = { "855", "Item_15", "867", "873", "876", "875", "877", "878", "879" };
                    foreach (string OneItem in Patient_Items)
                    {
                        form.Items.Item(OneItem).FromPane = 100;
                        form.Items.Item(OneItem).ToPane = 100;
                    }
                    //form.Items.Item("855").FromPane = 100;
                    //form.Items.Item("Item_15").FromPane = 100;
                    //form.Items.Item(Flags_List["855"]).FromPane = 100;
                    //form.Items.Item("855").ToPane = 100;
                    //form.Items.Item("Item_15").ToPane = 100;
                    //form.Items.Item(Flags_List["855"]).ToPane = 100;

                }

                foreach (string OneItem in Relations_Grid_IDs)
                {
                    form.Items.Item(OneItem).SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 4, BoModeVisualBehavior.mvb_False);
                }
                Grids_List = Utility.Get_Grids_UI(form, Relations_Grid_IDs);
                Grids_List.AddRange(Utility.Get_Grids_UI(form, new string[] { "330" }));


                //DataTable DT_Departments = form.DataSources.DataTables.Item("DT_Departments");
                //Dictionary<string, string> Departments = new Dictionary<string, string>();
                //Departments.Add("LID", "LID");
                //Departments.Add("LCD", "LCD");
                //Departments.Add("IDD", "IDD");
                //Helper.Utility.FillGridForDictionary(DT_Departments, Departments, true);
                Fill_All_Relation_Tables(form);

                ((Grid)form.Items.Item("330").Specific).Columns.Item("Code").Visible = false;

                form.Items.Item("139").SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 2, BoModeVisualBehavior.mvb_False);
                form.Items.Item("140").SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 2, BoModeVisualBehavior.mvb_False);

                Matrix Matrix_Addr = (Matrix)form.Items.Item("20").Specific;
                Matrix_Addr.AutoResizeColumns();


                form.Items.Item("3").Click();

            }
            catch (Exception ex)
            {
                form.Freeze(false);
                SBO_Application.StatusBar.SetText($"Error during Initialize the form[{ex.Message}]", BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Error);
            }
        }

        private static void Fill_All_Relation_Tables(Form form)
        {
            DataTable DT_Departments = form.DataSources.DataTables.Item("DT_Departments");
            //if (DT_Departments.Rows.Count != 0)
            //{
            //    return;
            //}
            string UDO_Code = form.DataSources.DBDataSources.Item(0).GetValue("Code", 0);

            Utility.Fill_Relation_Grids(company, form, UDO_Code, Relations_Grid_IDs);

            DT_Departments.Rows.Clear();
            Dictionary<string, string> Departments = new Dictionary<string, string>();
            Departments.Add("LID", "LID");
            Departments.Add("LCD", "LCD");
            Departments.Add("IDD", "IDD");
            Helper.Utility.FillGridForDictionary(DT_Departments, Departments, true);

        }


        internal static bool SBO_Application_FormDataEvent(ref BusinessObjectInfo BusinessObjectInfo)
        {
            bool BubbleEvent = true;
            try
            {
                if (Form_Obj == null || BusinessObjectInfo.FormTypeEx != Form_Obj.Form_Type)
                {
                    return BubbleEvent;
                }
                Form form = SBO_Application.Forms.Item(BusinessObjectInfo.FormUID);
                if (form.Mode == BoFormMode.fm_FIND_MODE)
                {
                    form.Items.Item("139").SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 4, BoModeVisualBehavior.mvb_False);
                    form.Items.Item("140").SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 4, BoModeVisualBehavior.mvb_False);
                    form.Items.Item("31").SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 4, BoModeVisualBehavior.mvb_False);
                    form.Items.Item("32").SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 4, BoModeVisualBehavior.mvb_False);
                    //form.Items.Item("702").SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 4, BoModeVisualBehavior.mvb_False);
                    //form.Items.Item("703").SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 4, BoModeVisualBehavior.mvb_False);
                    Form_Obj.UnSet_Mondatory_Fields_Color(form);
                }
                Code_value = SBO_Application.Forms.ActiveForm.TypeEx.Replace("ST", "Frm");
                if ((BusinessObjectInfo.EventType == BoEventTypes.et_FORM_DATA_UPDATE || BusinessObjectInfo.EventType == BoEventTypes.et_FORM_DATA_ADD)
                    && BusinessObjectInfo.BeforeAction)
                {
                    BubbleEvent = Validate_Data(BusinessObjectInfo);
                    if (!BubbleEvent)
                    {
                        return BubbleEvent;
                    }
                    Before_Adding_UDO(BusinessObjectInfo);
                    //if (BusinessObjectInfo.EventType == BoEventTypes.et_FORM_DATA_UPDATE)
                    //{
                    //    ADD_Update_UDO(BusinessObjectInfo);
                    //}
                }

                if ((BusinessObjectInfo.EventType == BoEventTypes.et_FORM_DATA_UPDATE || BusinessObjectInfo.EventType == BoEventTypes.et_FORM_DATA_ADD) && BusinessObjectInfo.ActionSuccess && !BusinessObjectInfo.BeforeAction)
                {
                    //if (BusinessObjectInfo.EventType == BoEventTypes.et_FORM_DATA_ADD)
                    //{
                        ADD_Update_UDO(BusinessObjectInfo);
                    //}
                }

                if (BusinessObjectInfo.EventType == BoEventTypes.et_FORM_DATA_LOAD && !BusinessObjectInfo.BeforeAction)
                {
                    Form_Data_Load(BusinessObjectInfo);
                }

                string TableName = form.DataSources.DBDataSources.Item(0).TableName.Replace("@", "");
                UDO_Definition KHCF_Object = Objects_Logic.All_UDO_Definition.FirstOrDefault(O => O.Table_Name == TableName);
                SBO_Application.StatusBar.SetText("Loading", BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_None);
                if (Form_Obj.Set_ReadOnly(form, KHCF_Object))
                {
                    // return;
                }

            }
            catch (Exception ex)
            {
                Loader.New_Msg = ex.Message;
                if (!BusinessObjectInfo.BeforeAction)
                {
                    Form form = Loader.SBO_Application.Forms.Item(BusinessObjectInfo.FormUID);
                    form.Mode = BoFormMode.fm_UPDATE_MODE;
                    SBO_Application.StatusBar.SetText(ex.Message, BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Error);
                }
                BubbleEvent = false;
            }
            return BubbleEvent;
        }

        private static void Form_Data_Load(BusinessObjectInfo businessObjectInfo)
        {
            Form form = SBO_Application.Forms.Item(businessObjectInfo.FormUID);
            string Card_ID = form.DataSources.DBDataSources.Item(0).GetValue("Code", 0);

            string SQL_Sum_Donation = $@"SELECT SUM(T0.""U_ST_DONATION_AMOUNT"") 
FROM ""@ST_ACTUAL_DONATIONS""  T0 WHERE T0.""U_ST_CONTACT_CARD"" = '{Card_ID}' AND  T0.""U_ST_CARD_TYPE"" = 'I'";
            Recordset RC_Sum_Donation = Helper.Utility.Execute_Recordset_Query(company, SQL_Sum_Donation);
            form.DataSources.UserDataSources.Item("610").Value = RC_Sum_Donation.Fields.Item(0).Value.ToString();


               string SQL_Sum_Contact_Donation = $@"SELECT SUM(T0.""U_ST_DONATION_AMOUNT"") 
FROM ""@ST_ACTUAL_DONATIONS""  T0 WHERE  T0.""U_ST_CARD_TYPE"" = 'I' AND 
T0.""U_ST_AMBASSADOR"" in (SELECT ""Code"" FROM ""@ST_FUND_INDIV_CARD"" T0 
WHERE T0.""U_ST_FATHER"" = '{Card_ID}')";
            Recordset RC_Sum_ContactDonation = Helper.Utility.Execute_Recordset_Query(company, SQL_Sum_Contact_Donation);
            form.DataSources.UserDataSources.Item("612").Value = RC_Sum_ContactDonation.Fields.Item(0).Value.ToString();

            Fill_All_Relation_Tables(form);

            Load_Sub_Members(form, Card_ID);

            // Set_Parent_Link(form);
            //Set_Parent_Yellow_Arrow(form);

            form.Items.Item("164").Visible = false;

            string UDO_Code = form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).GetValue("Code", 0);
            Utility.Load_All_Relation_Data(company, form, UDO_Code, Relations_Grid_IDs, Form_Obj.KHCF_Object);
            Utility.Load_All_Relation_Data(company, form, UDO_Code, new string[] { "330" }, Form_Obj.KHCF_Object);

            Change_Items_Values_Cutomization("152", businessObjectInfo.FormUID, false, true, "", 0, true);
            Change_Items_Values_Cutomization("605", businessObjectInfo.FormUID, false, true, "", 0, true);
            Set_Flags_Visibility(form);

            Form_Obj.Set_Fields(form);
        }

        private static void Set_Flags_Visibility(Form form)
        {

            List<string> flags = new List<string> { "850", "851" };
            string BP_Code = form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).GetValue("U_ST_BP_CODE", 0);
            if (BP_Code != "")
            {
                form.Items.Item(flags[0]).SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 1, BoModeVisualBehavior.mvb_False);
                string SQL_BP = $@"SELECT T0.""CardType"" FROM OCRD T0 WHERE T0.""CardCode"" = '{BP_Code}'";
                Recordset RC_BP = Helper.Utility.Execute_Recordset_Query(company, SQL_BP);
                if (RC_BP.Fields.Item("CardType").Value.ToString() == "L")
                {
                    form.Items.Item(flags[1]).SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 1, BoModeVisualBehavior.mvb_True);
                }
                else
                {
                    form.Items.Item(flags[1]).SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 1, BoModeVisualBehavior.mvb_False);
                }
                return;
            }
            else
            {
                form.Items.Item(flags[0]).SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 1, BoModeVisualBehavior.mvb_True);
                form.Items.Item(flags[1]).SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 1, BoModeVisualBehavior.mvb_True);

            }
            //CheckBox leadCheckBox = (CheckBox)form.Items.Item(flags[0]).Specific;
            //CheckBox donorCheckBox = (CheckBox)form.Items.Item(flags[1]).Specific;

            //if (donorCheckBox.Checked || leadCheckBox.Checked)
            //{
            //    form.Items.Item(flags[1]).SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 1, BoModeVisualBehavior.mvb_False);
            //}
            //else if (leadCheckBox.Checked)
            //{
            //    form.Items.Item(flags[0]).SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 1, BoModeVisualBehavior.mvb_False);
            //}
            //else
            //{
            //    form.Items.Item(flags[0]).SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 1, BoModeVisualBehavior.mvb_True);
            //    form.Items.Item(flags[1]).SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 1, BoModeVisualBehavior.mvb_True);
            //}

        }

        private static void Set_Parent_Link(Form form)
        {
            //if (form.DataSources.DBDataSources.Item(0).GetValue("U_ST_PARENT_TYPE", 0) == "C")
            //{
            //    ((EditText)form.Items.Item("145").Specific).ChooseFromListUID = "CFL_PARENT_CORP";
            //}
            //else if (form.DataSources.DBDataSources.Item(0).GetValue("U_ST_PARENT_TYPE", 0) == "I")
            //{
            //    ((EditText)form.Items.Item("145").Specific).ChooseFromListUID = "CFL_PARENT_INDIV";
            //}
            //else
            //{
            //    //((EditText)form.Items.Item("145").Specific).ChooseFromListUID = "";
            //}
            //Set_Parent_Yellow_Arrow(form);
        }

        //private static void Set_Parent_Yellow_Arrow(Form form)
        //{
        //    if (form.DataSources.DBDataSources.Item(0).GetValue("U_ST_PARENT_TYPE", 0) == "C")
        //    {
        //        ((LinkedButton)form.Items.Item("182").Specific).LinkedObjectType = "ST_FUND_CORP_CARD";
        //    }
        //    else if (form.DataSources.DBDataSources.Item(0).GetValue("U_ST_PARENT_TYPE", 0) == "I")
        //    {
        //        ((LinkedButton)form.Items.Item("182").Specific).LinkedObjectType = "ST_FUND_INDIV_CARD";
        //    }

        //}

        private static void Load_Sub_Members(Form form, string Card_ID)
        {
            DataTable DT_Members = form.DataSources.DataTables.Item("MEMBERS");
            DT_Members.Rows.Clear();

            string SQL_Members = $@"SELECT *  FROM ""@ST_FUND_INDIV_CARD""  T0 
WHERE T0.""U_ST_FATHER"" = '{Card_ID}'";
            Recordset RC_Members = Helper.Utility.Execute_Recordset_Query(company, SQL_Members);
            DT_Members.Rows.Add(RC_Members.RecordCount);

            for (int i = 0; i < RC_Members.RecordCount; i++)
            {
                for (int J = 1; J < DT_Members.Columns.Count; J++)
                {
                    string Col_Name = DT_Members.Columns.Item(J).Name;
                    string UDF_Name;
                    if (Col_Name == "Code")
                    {
                        UDF_Name = Col_Name;
                    }
                    else
                    {
                        UDF_Name = "U_" + Col_Name;
                    }

                    DT_Members.SetValue(Col_Name, i, RC_Members.Fields.Item(UDF_Name).Value);
                }
                RC_Members.MoveNext();
            }
            Grid Grd_Members = (Grid)form.Items.Item("138").Specific;
            for (int i = 0; i < DT_Members.Rows.Count; i++)
            {
                for (int j = 2; j < Grd_Members.Columns.Count + 1; j++)
                {
                    Grd_Members.CommonSetting.SetCellEditable(i + 1, j, false);
                }
                //Grd_Members.CommonSetting.SetRowEditable(i + 1, false);
            }
            Grd_Members.AutoResizeColumns();
        }

        private static void ADD_Update_UDO(BusinessObjectInfo businessObjectInfo)
        {
            Form form = SBO_Application.Forms.Item(businessObjectInfo.FormUID);
            System.Xml.XmlDocument XML_Doc = new System.Xml.XmlDocument();
            string XML_Text = businessObjectInfo.ObjectKey.Replace(" ", "").Replace("=", " = ");
            XML_Doc.LoadXml(XML_Text);

            string UDO_Code = XML_Doc.GetElementsByTagName("Code")[0].InnerText;
            string UDO_Name = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_FULL_NAME_AR", 0);

            Add_Members(form, UDO_Code, UDO_Name);

            string BP_Code = Check_Flags_Logic(form, UDO_Code);
            form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).SetValue("U_ST_BP_CODE", 0, BP_Code);

            //if (!string.IsNullOrEmpty(BP_Code))
            //{
            //    UDO_Definition UDO_Info = Logic.Objects_Logic.All_UDO_Definition.FirstOrDefault(U => U.KHCF_Object == KHCF_Objects.Fundraising_Individual_Card);
            //    Utility.Update_BP(company, BP_Code, UDO_Code, UDO_Info);
            //}
            Utility.Update_Relation_Table(company, form, UDO_Code, Relations_Grid_IDs, Form_Obj.KHCF_Object);
            Utility.Update_Relation_Table(company, form, UDO_Code, new string[] { "330" }, Form_Obj.KHCF_Object);
      
            Set_Flags_Visibility(form);

            if (form.Mode == BoFormMode.fm_ADD_MODE)
            {
                Fill_All_Relation_Tables(form);
            }

        }

        private static string Check_Flags_Logic(Form form, string UDO_Code)
        {

            bool Is_Lead = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_LEAD_ADD_UPDATE", 0) == "Y";
            bool Is_Donor = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_DONOR_ADD_UPDATE", 0) == "Y";
            //bool Is_Anonymous = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_ANONYMOUS_ADD_UPDATE", 0) == "Y";
            //bool Is_Ambass = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_AMBASSADOR_ADD_UPDATE", 0) == "Y";
            //bool Is_Blacklist = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_BLACKLISTED_ADD_UPDATE", 0) == "Y";
            //bool Is_Pationt = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_PATIENT_ADD_UPDATE", 0) == "Y";
            //bool Is_Board = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_BOARD_MEMBER_ADD_UPDATE", 0) == "Y";
            //bool Is_VOLUNTEER = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_VOLUNTEER_ADD_UPDATE", 0) == "Y";
            //bool Is_SURVIVOR = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_SURVIVOR_ADD_UPDATE", 0) == "Y";
            //bool Is_DECEASED = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_DECEASED_UPDATE", 0) == "Y";
            //bool Is_INFLUENCER = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_INFLUENCER_ADD_UPDATE", 0) == "Y";

            //if (!(Is_Lead || Is_Doner || Is_Anonymous || Is_Ambass || Is_Blacklist || Is_Pationt || Is_Board || Is_VOLUNTEER || Is_SURVIVOR || Is_DECEASED || Is_INFLUENCER))
            //{
            //    return;
            //}
            if (!(Is_Lead || Is_Donor))
            {
                return "";
            }

            string BP_Code = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_BP_CODE", 0);
            if (string.IsNullOrEmpty(BP_Code))
            {
                KHCF_BP BP_Data = new KHCF_BP();
                BP_Data.BP_Group = int.Parse(form.DataSources.DBDataSources.Item(0).GetValue("U_ST_CUSTOMER_GROUP", 0));
                BP_Data.CardName = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_FULL_NAME_AR", 0);
                BP_Data.MemberCard_Code = UDO_Code;
                int.TryParse(form.DataSources.DBDataSources.Item(0).GetValue("U_ST_ACCOUNT_MANAGER", 0), out BP_Data.SalesPersonCode);

                if (Is_Donor == true)
                {
                    BP_Data.Is_Lead = false;
                }
                else if (Is_Lead == true && Is_Donor == false)
                {
                    BP_Data.Is_Lead = true;
                }
                else
                {
                    throw new Logic.Custom_Exception("We can't set the BP Type");
                }

                string SQL_Addr = $@"SELECT T0.""U_ST_ADDRESS_NAME"", T0.""U_ST_STREET"", T0.""U_ST_BLOCK"", T0.""U_ST_ZIP_CODE"", T0.""U_ST_CITY""
, T0.""U_ST_COUNTY"",  T0.""U_ST_STATE"", T0.""U_ST_BUILDING"", T0.""U_ST_ADDRESS_TYPE"", T0.""U_ST_ADDRESS_NAME_2""
, T0.""U_ST_ADDRESS_NAME_3"", T0.""U_ST_STREET_NO""
,(Select T1.""Code"" From OCRY T1 Where T1.""Name"" =U_ST_COUNTRY)As U_ST_COUNTRY 
FROM ""@ST_FUND_INDIV_ADDR"" T0 WHERE ""Code"" = '{UDO_Code}' ";
                Recordset RC_Addr = Helper.Utility.Execute_Recordset_Query(company, SQL_Addr);
                BP_Data.addresses = new List<BpAddress>();
                if (RC_Addr.RecordCount > 0)
                {
                    BP_Data.addresses = new List<BpAddress>();
                    for (int i = 0; i < RC_Addr.RecordCount; i++)
                    {
                        BpAddress address = new BpAddress();
                        address.Street = RC_Addr.Fields.Item("U_ST_STREET").Value.ToString();
                        address.AddressName = RC_Addr.Fields.Item("U_ST_ADDRESS_NAME").Value.ToString();
                        address.City = RC_Addr.Fields.Item("U_ST_CITY").Value.ToString();
                        address.AddressType = RC_Addr.Fields.Item("U_ST_ADDRESS_TYPE").Value.ToString();
                        address.Country = RC_Addr.Fields.Item("U_ST_COUNTRY").Value.ToString();
                        address.Block = RC_Addr.Fields.Item("U_ST_BLOCK").Value.ToString();
                        address.BuildingFloorRoom = RC_Addr.Fields.Item("U_ST_BUILDING").Value.ToString();
                        address.StreetNo = RC_Addr.Fields.Item("U_ST_STREET_NO").Value.ToString();
                        BP_Data.addresses.Add(address);
                        RC_Addr.MoveNext();
                    }
                }
                try
                {
                    company.StartTransaction();
                    BP_Code = Utility.Create_BP(company, BP_Data);
                    Field_Data Fld = new Field_Data() { Field_Name = "U_ST_BP_CODE", Value = BP_Code };
                    Utility.Update_UDO(company, Form_Obj.UDO_Info, UDO_Code, new Field_Data[] { Fld });
                    company.EndTransaction(BoWfTransOpt.wf_Commit);
                }
                catch (Exception)
                {
                    try
                    {
                        company.EndTransaction(BoWfTransOpt.wf_RollBack);
                    }
                    catch (Exception) { }
                    throw;
                }
            }
            Utility.Update_BP(company, BP_Code, UDO_Code, Form_Obj.UDO_Info, null, !Is_Donor);
            return BP_Code;
        }

        internal static void SBO_Application_MenuEvent(ref MenuEvent pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;

            if (pVal.BeforeAction)
                return;

            if (Form_Obj == null || SBO_Application.Forms.ActiveForm.TypeEx != Form_Obj.Form_Type)
            {
                return;
            }

            Code_value = SBO_Application.Forms.ActiveForm.TypeEx.Replace("ST", "Frm");
            if (pVal.MenuUID == "1282" || pVal.MenuUID == "1281")//Add || Find
            {
                
                Form form = SBO_Application.Forms.ActiveForm;
                try
                {
                    form.Freeze(true);
                    DataTable DT_Members = form.DataSources.DataTables.Item("MEMBERS");
                    DT_Members.Rows.Clear();

                    Fill_All_Relation_Tables(form);


                    string TableName = form.DataSources.DBDataSources.Item(0).TableName.Replace("@", "");
                    UDO_Definition KHCF_Object = Objects_Logic.All_UDO_Definition.FirstOrDefault(O => O.Table_Name == TableName);
                    if (Form_Obj.Set_ReadOnly(form, KHCF_Object))
                    {
                        // return;
                    }
                    //form.Freeze(false);

                    string Parent_ID = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_PARENT_ID", 0);
                    Grid grid = (Grid)form.Items.Item("138").Specific;
                    if (!string.IsNullOrEmpty(Parent_ID))
                    {
                        form.Items.Item("138").Enabled = false;
                        form.Items.Item("138").SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, -1, BoModeVisualBehavior.mvb_False);
                    }
                    else
                    {
                        form.Items.Item("138").Enabled = true;
                        form.Items.Item("138").SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, -1, BoModeVisualBehavior.mvb_True);
                    }
                }
                finally
                {
                    form.Freeze(false);
                }
            }
            if (SBO_Application.Forms.ActiveForm.Mode == BoFormMode.fm_FIND_MODE && Form_Obj != null)
            {
                if (SBO_Application.Forms.ActiveForm.TypeEx == Form_Obj.Form_Type)
                {
                    Form_Obj.UnSet_Mondatory_Fields_Color(SBO_Application.Forms.ActiveForm);
                }
            }
        }

        private static void Before_Adding_UDO(BusinessObjectInfo businessObjectInfo)
        {
            Form form = SBO_Application.Forms.Item(businessObjectInfo.FormUID);
            if (form.Mode == BoFormMode.fm_ADD_MODE)
            {
                Set_Default_Value_Before_Adding(form);
            }
        }
        private static void Set_Default_Value_Before_Adding(Form form)
        {
            if (form.Mode == BoFormMode.fm_ADD_MODE)
            {
                string New_UDO_Code = Utility.Get_New_UDO_Code(company, Form_Obj.KHCF_Object);
                form.DataSources.DBDataSources.Item(0).SetValue("Code", 0, New_UDO_Code);
                form.DataSources.DBDataSources.Item(0).SetValue("U_ST_CREATOR", 0, company.UserName);
                form.DataSources.DBDataSources.Item(0).SetValue("U_ST_CREATION_DATE", 0, DateTime.Now.ToString("yyyyMMdd"));
                form.DataSources.DBDataSources.Item(0).SetValue("U_ST_PARENT_TYPE", 0, "N");
            }
        }

        private static void Change_Items_Values_Cutomization(string ItemUID, string FormUID, bool BeforeAction, bool ItemChanged, string ColUID, int Row, bool OnLoad = false)
        {
            #region Names Fields

            string[] Arabic_Names_Items = new string[] { "9", "11", "13", "15" };

            if (Arabic_Names_Items.Contains(ItemUID))
            {
                SAPbouiCOM.Form form = SBO_Application.Forms.Item(FormUID);

                string FirstName = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_FIRST_NAME_AR", 0);
                string FatherName = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_FATHER_NAME_AR", 0);
                string MiddleName = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_MIDDLE_NAME_AR", 0);
                string SurName = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_SURNAME_AR", 0);
                string Full_Name = "";
                if (!Utility.Check_Text(FirstName) && !string.IsNullOrEmpty(FirstName))
                {
                    form.DataSources.DBDataSources.Item(0).SetValue("U_ST_FIRST_NAME_AR", 0, string.Empty);
                    form.DataSources.DBDataSources.Item(0).SetValue("U_ST_FULL_NAME_AR", 0, string.Empty);
                    form.Items.Item("9").Click();
                    //throw new Custom_Exception("Only Arabic Letters are allowed.");
                }
                else if (!Utility.Check_Text(FatherName) && !string.IsNullOrEmpty(FatherName))
                {
                    form.DataSources.DBDataSources.Item(0).SetValue("U_ST_FATHER_NAME_AR", 0, string.Empty);
                    form.DataSources.DBDataSources.Item(0).SetValue("U_ST_FULL_NAME_AR", 0, string.Empty);
                    form.Items.Item("11").Click();
                }
                else if (!Utility.Check_Text(MiddleName) && !string.IsNullOrEmpty(MiddleName))
                {
                    form.DataSources.DBDataSources.Item(0).SetValue("U_ST_MIDDLE_NAME_AR", 0, string.Empty);
                    form.DataSources.DBDataSources.Item(0).SetValue("U_ST_FULL_NAME_AR", 0, string.Empty);
                    form.Items.Item("13").Click();
                }
                else if (!Utility.Check_Text(SurName) && !string.IsNullOrEmpty(SurName))
                {
                    form.DataSources.DBDataSources.Item(0).SetValue("U_ST_SURNAME_AR", 0, string.Empty);
                    form.DataSources.DBDataSources.Item(0).SetValue("U_ST_FULL_NAME_AR", 0, string.Empty);
                    form.Items.Item("15").Click();
                }
                else
                    Full_Name = Utility.Get_Full_Name(FirstName, FatherName, MiddleName, SurName);

                form.DataSources.DBDataSources.Item(0).SetValue("U_ST_FULL_NAME_AR", 0, Full_Name);
            }

            string[] English_Names_Items = new string[] { "810", "803", "802", "811" };

            if (English_Names_Items.Contains(ItemUID))
            {
                SAPbouiCOM.Form form = SBO_Application.Forms.Item(FormUID);
                string FirstName = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_FIRST_NAME_EN", 0);
                string FatherName = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_FATHER_NAME_EN", 0);
                string MiddleName = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_MIDDLE_NAME_EN", 0);
                string SurName = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_SURNAME_EN", 0);
                string Full_Name = "";
                if (Utility.Check_Text(FirstName) && !string.IsNullOrEmpty(FirstName))
                {
                    form.DataSources.DBDataSources.Item(0).SetValue("U_ST_FIRST_NAME_EN", 0, string.Empty);
                    form.DataSources.DBDataSources.Item(0).SetValue("U_ST_FULL_NAME_EN", 0, string.Empty);
                    form.Items.Item("810").Click();
                    //throw new Custom_Exception("Only English Letters are allowed.");
                }
                else if (Utility.Check_Text(FatherName) && !string.IsNullOrEmpty(FatherName))
                {
                    form.DataSources.DBDataSources.Item(0).SetValue("U_ST_FATHER_NAME_EN", 0, string.Empty);
                    form.DataSources.DBDataSources.Item(0).SetValue("U_ST_FULL_NAME_EN", 0, string.Empty);
                    form.Items.Item("803").Click();
                }
                else if (Utility.Check_Text(MiddleName) && !string.IsNullOrEmpty(MiddleName))
                {
                    form.DataSources.DBDataSources.Item(0).SetValue("U_ST_MIDDLE_NAME_EN", 0, string.Empty);
                    form.DataSources.DBDataSources.Item(0).SetValue("U_ST_FULL_NAME_EN", 0, string.Empty);
                    form.Items.Item("802").Click();
                }
                else if (Utility.Check_Text(SurName) && !string.IsNullOrEmpty(SurName))
                {
                    form.DataSources.DBDataSources.Item(0).SetValue("U_ST_SURNAME_EN", 0, string.Empty);
                    form.DataSources.DBDataSources.Item(0).SetValue("U_ST_FULL_NAME_EN", 0, string.Empty);
                    form.Items.Item("811").Click();
                }
                else
                    Full_Name = Utility.Get_Full_Name(FirstName, FatherName, MiddleName, SurName);

                form.DataSources.DBDataSources.Item(0).SetValue("U_ST_FULL_NAME_EN", 0, Full_Name);
            }

            if (ItemUID == "138")
            {
                string[] Names_Cols = new string[] { "ST_FIRST_NAME_AR", "ST_FATHER_NAME_AR", "ST_MIDDLE_NAME_AR", "ST_SURNAME_AR" };
                if (Names_Cols.Contains(ColUID))
                {
                    SAPbouiCOM.Form form = SBO_Application.Forms.Item(FormUID);
                    DataTable DT_Members = form.DataSources.DataTables.Item("MEMBERS");
                    Grid Grd_Members = (Grid)form.Items.Item("138").Specific;
                    int RowIndex = Grd_Members.GetDataTableRowIndex(Row);

                    string FirstName = DT_Members.GetValue("ST_FIRST_NAME_AR", RowIndex).ToString();
                    string FatherName = DT_Members.GetValue("ST_FATHER_NAME_AR", RowIndex).ToString();
                    string MiddleName = DT_Members.GetValue("ST_MIDDLE_NAME_AR", RowIndex).ToString();
                    string SurName = DT_Members.GetValue("ST_SURNAME_AR", RowIndex).ToString();
                    string Full_Name = "";
                    if (!Utility.Check_Text(FirstName) && !string.IsNullOrEmpty(FirstName))
                    {
                        DT_Members.SetValue(ColUID, RowIndex, string.Empty);
                        Full_Name = string.Empty;
                    }
                    else if (!Utility.Check_Text(FatherName) && !string.IsNullOrEmpty(FatherName))
                    {
                        DT_Members.SetValue(ColUID, RowIndex, string.Empty);
                        Full_Name = string.Empty;
                    }
                    else if (!Utility.Check_Text(MiddleName) && !string.IsNullOrEmpty(MiddleName))
                    {
                        DT_Members.SetValue(ColUID, RowIndex, string.Empty);
                        Full_Name = string.Empty;
                    }
                    else if (!Utility.Check_Text(SurName) && !string.IsNullOrEmpty(SurName))
                    {
                        DT_Members.SetValue(ColUID, RowIndex, string.Empty);
                        Full_Name = string.Empty;
                    }
                    else
                        Full_Name = Utility.Get_Full_Name(FirstName, FatherName, MiddleName, SurName);

                    DT_Members.SetValue("ST_FULL_NAME_AR", RowIndex, Full_Name);
                }

                string[] Names_EN_Cols = new string[] { "ST_FIRST_NAME_EN", "ST_FATHER_NAME_EN", "ST_MIDDLE_NAME_EN", "ST_SURNAME_EN" };
                if (Names_EN_Cols.Contains(ColUID))
                {
                    SAPbouiCOM.Form form = SBO_Application.Forms.Item(FormUID);
                    DataTable DT_Members = form.DataSources.DataTables.Item("MEMBERS");
                    Grid Grd_Members = (Grid)form.Items.Item("138").Specific;
                    int RowIndex = Grd_Members.GetDataTableRowIndex(Row);

                    string FirstName = DT_Members.GetValue("ST_FIRST_NAME_EN", RowIndex).ToString();
                    string FatherName = DT_Members.GetValue("ST_FATHER_NAME_EN", RowIndex).ToString();
                    string MiddleName = DT_Members.GetValue("ST_MIDDLE_NAME_EN", RowIndex).ToString();
                    string SurName = DT_Members.GetValue("ST_SURNAME_EN", RowIndex).ToString();
                    string Full_Name = "";
                    if (Utility.Check_Text(FirstName) && !string.IsNullOrEmpty(FirstName))
                    {
                        DT_Members.SetValue(ColUID, RowIndex, string.Empty);
                        Full_Name = string.Empty;
                    }
                    else if (Utility.Check_Text(FatherName) && !string.IsNullOrEmpty(FatherName))
                    {
                        DT_Members.SetValue(ColUID, RowIndex, string.Empty);
                        Full_Name = string.Empty;
                    }
                    else if (Utility.Check_Text(MiddleName) && !string.IsNullOrEmpty(MiddleName))
                    {
                        DT_Members.SetValue(ColUID, RowIndex, string.Empty);
                        Full_Name = string.Empty;
                    }
                    else if (Utility.Check_Text(SurName) && !string.IsNullOrEmpty(SurName))
                    {
                        DT_Members.SetValue(ColUID, RowIndex, string.Empty);
                        Full_Name = string.Empty;
                    }
                    else
                        Full_Name = Utility.Get_Full_Name(FirstName, FatherName, MiddleName, SurName);

                    DT_Members.SetValue("ST_FULL_NAME_EN", RowIndex, Full_Name);
                }
            }

            #endregion

            if (ItemUID == "145")
            {
                Form form = SBO_Application.Forms.Item(FormUID);
                Choose_Parent_ID(form);
            }

            if (ItemUID == "27")
            {
                SAPbouiCOM.Form form = SBO_Application.Forms.Item(FormUID);
                string gender = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_GENDER", 0);
                if (!string.IsNullOrEmpty(gender))
                {
                    string SQL_Prefix = $@"SELECT T0.""Code"" AS ""Code"", T0.""Name"" AS ""Name"" 
                                           FROM ""@ST_PREFIX"" T0 WHERE T0.""U_ST_GENDER"" = '{gender}'";
                    Helper.Utility.Fill_One_ComboBoxBySQL(company, form, "326", SQL_Prefix, true);
                }
            }
            if (ItemUID == "605")
            {
                SAPbouiCOM.Form form = SBO_Application.Forms.Item(FormUID);
                string Value = form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).GetValue("U_ST_FATHER", 0);
                if (Value != "")
                {
                    form.Items.Item("601").BackColor = Color.Pink.R | (Color.Pink.G << 8) | (Color.Pink.B << 16); // Arabic Father Name
                }
                else
                {
                    form.Items.Item("601").BackColor = Color.FromKnownColor(KnownColor.White).ToArgb();
                }
            }

        }

        private static bool Validate_Data(BusinessObjectInfo businessObjectInfo)
        {
            Form form = SBO_Application.Forms.Item(businessObjectInfo.FormUID);
            if (!Form_Obj.Validate_Data(form))
            {
                return false;
            }
            //int errorIndex = CheckContacts(businessObjectInfo);
            //if (errorIndex >= 0)
            //{
            //    throw new Custom_Exception($@"Email address in line {errorIndex + 1} in contacts tab is invalid.");
            //}
            DataTable DT_Departments = form.DataSources.DataTables.Item("DT_Departments");

            //Utility.Check_Relation_Table_Mandatory(DT_Departments, "Departments");
            
            bool isEmpty = true;

            if (DT_Departments.Rows.Count > 0)
            {
                for (int i = 0; i < DT_Departments.Rows.Count; i++)
                {
                    if (DT_Departments.GetValue("SELECTED", i).ToString() == "Y")
                    {
                        isEmpty = false;
                    }
                }
            }

            if (isEmpty)
            {
                Loader.New_Msg = "Please select at least one Department";
                return false;
            }
            Member_Cards_UI.Check_Fundraising_Members(businessObjectInfo, "I");
            Member_Cards_UI.Check_Address(businessObjectInfo, "@ST_FUND_INDIV_ADDR");

            return true;
        }

        internal static void SBO_Application_ItemEvent(string FormUID, ref ItemEvent pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            if (Form_Obj == null || pVal.FormTypeEx != Form_Obj.Form_Type)
            {
                return;
            }
            try
            {
                if (!pVal.BeforeAction && pVal.ItemChanged)
                {
                    Change_Items_Values_Cutomization(pVal.ItemUID, pVal.FormUID, pVal.BeforeAction, pVal.ItemChanged, pVal.ColUID, pVal.Row);
                }
                if (pVal.ItemUID == "31" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Form form = SBO_Application.Forms.Item(pVal.FormUID);
                    Add_Address_Row(pVal);
                }
                if (pVal.ItemUID == "32" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Remove_Address_Row(pVal);
                }

                if (pVal.ItemUID == "139" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Form form = SBO_Application.Forms.Item(pVal.FormUID);
                    Add_Member_Row(pVal);
                }
                if (pVal.ItemUID == "140" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Remove_Member_Row(pVal);
                }

                if (pVal.ItemUID == "163" && (pVal.EventType == BoEventTypes.et_ITEM_PRESSED || pVal.EventType == BoEventTypes.et_COMBO_SELECT) && !pVal.BeforeAction)
                {
                    Run_Actions(pVal);
                }
                if (pVal.ItemUID == "165" && (pVal.EventType == BoEventTypes.et_ITEM_PRESSED || pVal.EventType == BoEventTypes.et_COMBO_SELECT) && !pVal.BeforeAction)
                {
                    Print_Actions(pVal, company);
                }
                if (pVal.ItemUID == "164" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Confirm_Link(pVal);
                }
                if (pVal.ItemUID == "143" && pVal.EventType == BoEventTypes.et_COMBO_SELECT && !pVal.BeforeAction)
                {
                    Select_Parent_Type(pVal);
                }
                //if (pVal.ItemUID == "703" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                //{
                //    Add_Contacts_Row(pVal);
                //}
                //if (pVal.ItemUID == "702" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                //{
                //    Remove_Contacts_Row(pVal);
                //}
                if (pVal.ItemUID == "20" && pVal.ColUID == "Country" && pVal.EventType == BoEventTypes.et_CHOOSE_FROM_LIST && !pVal.BeforeAction)
                {
                    Choose_From_List_Country_Matrix(pVal);
                }
                //if (pVal.ItemUID == "605" && pVal.EventType == BoEventTypes.et_CHOOSE_FROM_LIST && !pVal.BeforeAction)
                //{
                //    //Choose_Father(pVal);
                //    string s = Utility.Chosse_From_List_For_Code_And_DBDataSource(pVal, pVal.ItemUID);
                //    SAPbouiCOM.Form form = Loader.SBO_Application.Forms.Item(pVal.FormUID);
                //    ((EditText)form.Items.Item("605").Specific).Value = s;
                //}
                if (pVal.ItemUID == "138" && (pVal.ColUID == "ST_NATIONALITY" || pVal.ColUID == "ST_RESIDENCY") && pVal.EventType == BoEventTypes.et_CHOOSE_FROM_LIST && !pVal.BeforeAction)
                {
                    Choose_From_List_Country_Grid(pVal);
                }

                if (pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction && Flags_List.Keys.Contains(pVal.ItemUID))
                {
                    Flag_Click(pVal);
                }

                if (pVal.ItemUID == "841" && pVal.EventType == BoEventTypes.et_CLICK && !pVal.BeforeAction)
                {
                    //System.Threading.Thread.Sleep(1000);
                    string SQL_User = $@"Select U_ST_HAVE_PATIENT_FLAG_ACCESS from OUSR WHERE USER_CODE = '{company.UserName}'";
                    Recordset RC_User = Helper.Utility.Execute_Recordset_Query(company, SQL_User);
                    if (RC_User.Fields.Item("U_ST_HAVE_PATIENT_FLAG_ACCESS").Value.ToString() != "Y")
                    {
                        Form form = SBO_Application.Forms.Item(pVal.FormUID);
                        //form.Items.Item("855").Visible = false;
                        //form.Items.Item("Item_15").Visible = false;
                        //form.Items.Item(Flags_List["855"]).Visible = false;
                        form.Items.Item("855").SetAutoManagedAttribute(BoAutoManagedAttr.ama_Visible, -1, BoModeVisualBehavior.mvb_False);
                        form.Items.Item("Item_15").SetAutoManagedAttribute(BoAutoManagedAttr.ama_Visible, -1, BoModeVisualBehavior.mvb_False);
                        form.Items.Item(Flags_List["855"]).SetAutoManagedAttribute(BoAutoManagedAttr.ama_Visible, -1, BoModeVisualBehavior.mvb_False);
                    }
                }

                if ((pVal.ItemUID == "152" || pVal.ItemUID == "328") && pVal.EventType == BoEventTypes.et_CHOOSE_FROM_LIST && !pVal.BeforeAction)
                {
                    Choose_From_List_Nationality(pVal);
                }

                //if (pVal.ItemUID == "152" && pVal.EventType == BoEventTypes.et_CHOOSE_FROM_LIST && !pVal.BeforeAction)
                //{
                //    SAPbouiCOM.Form form = SBO_Application.Forms.Item(FormUID);
                //    string Name = form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).GetValue("U_ST_NATIONALITY", 0);
                //    if (Name == "Jordan")
                //    {
                //        form.Items.Item("11").BackColor = Color.Pink.R | (Color.Pink.G << 8) | (Color.Pink.B << 16); // Arabic Father Name
                //        form.Items.Item("13").BackColor = Color.Pink.R | (Color.Pink.G << 8) | (Color.Pink.B << 16); // Arabic Middle Name                                                                                            
                //    }
                //    else
                //    {
                //        form.Items.Item("11").BackColor = Color.FromKnownColor(KnownColor.White).ToArgb();
                //        form.Items.Item("13").BackColor = Color.FromKnownColor(KnownColor.White).ToArgb();
                //    }
                //}




                if (pVal.EventType == BoEventTypes.et_FORM_RESIZE && !pVal.BeforeAction)
                {
                    Form form = SBO_Application.Forms.Item(pVal.FormUID);
                    Utility.Resize_Grids(form, Grids_List);
                }

                if ( (pVal.ItemUID == "602" || pVal.ItemUID == "878" || pVal.ItemUID == "605") && pVal.EventType == BoEventTypes.et_CHOOSE_FROM_LIST && !pVal.BeforeAction)
                {
                    Utility.Chosse_From_List_For_Code_And_DBDataSource(pVal, pVal.ItemUID);
                }

                if ((pVal.ItemUID == "850" || pVal.ItemUID == "851") && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction) 
                {
                    Form form = SBO_Application.Forms.Item(pVal.FormUID);
                    Set_Flags_Visibility(form);
                }
            }
            catch (Exception ex)
            {
                if (!string.IsNullOrEmpty(pVal.FormUID) && pVal.EventType != BoEventTypes.et_FORM_UNLOAD)
                {
                    Form form = SBO_Application.Forms.Item(pVal.FormUID);
                    form.Freeze(false);
                    SBO_Application.StatusBar.SetText(ex.Message, BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Error);
                }
            }
        }

        private static void Flag_Click(ItemEvent pVal)
        {
            SAPbouiCOM.Form form = SBO_Application.Forms.Item(pVal.FormUID);
            string ItemId = pVal.ItemUID;

            string FieldName = Helper.Utility.Get_Item_DB_Datasource(form.Items.Item(ItemId));
            bool Value = ((CheckBox)form.Items.Item(ItemId).Specific).Checked;

            string Date_Item = Flags_List[ItemId];
            string Date_FieldName = Helper.Utility.Get_Item_DB_Datasource(form.Items.Item(Date_Item));
            if (Value == true)
            {
                form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).SetValue(Date_FieldName, 0, DateTime.Today.ToString("yyyyMMdd"));
            }
            else
            {
                form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).SetValue(Date_FieldName, 0, "");
            }

        }

        private static void Choose_From_List_Nationality(ItemEvent pVal)
        {
            SAPbouiCOM.Form form = SBO_Application.Forms.Item(pVal.FormUID);
            ChooseFromListEvent Chos_Event = (ChooseFromListEvent)pVal;
            if ((Chos_Event.SelectedObjects == null ? false : Chos_Event.SelectedObjects.Rows.Count != 0))
            {
                string Name = Chos_Event.SelectedObjects.GetValue("Name", 0).ToString();
                if (pVal.ItemUID == "152")
                {
                    form.DataSources.DBDataSources.Item(0).SetValue("U_ST_NATIONALITY", 0, Name);
                }
                else if (pVal.ItemUID == "328")
                {
                    form.DataSources.DBDataSources.Item(0).SetValue("U_ST_RESIDENCY", 0, Name);
                }
                else
                {
                    throw new Logic.Custom_Exception($"The Choose From List is not supported for the Item[{pVal.ItemUID}]");
                }


                if (form.Mode == BoFormMode.fm_OK_MODE)
                {
                    form.Mode = BoFormMode.fm_UPDATE_MODE;
                }
            }

        }
        private static void Choose_From_List_Country_Grid(ItemEvent pVal)
        {
            SAPbouiCOM.Form form = SBO_Application.Forms.Item(pVal.FormUID);
            ChooseFromListEvent Chos_Event = (ChooseFromListEvent)pVal;
            if ((Chos_Event.SelectedObjects == null ? false : Chos_Event.SelectedObjects.Rows.Count != 0))
            {
                Grid Grd = (Grid)form.Items.Item(pVal.ItemUID).Specific;
                string C = Chos_Event.SelectedObjects.GetValue("Name", 0).ToString();
                DataTable DT = Grd.DataTable;
                int Index = Grd.GetDataTableRowIndex(pVal.Row);
                DT.SetValue(pVal.ColUID, Index, C);
                if (form.Mode == BoFormMode.fm_OK_MODE)
                {
                    form.Mode = BoFormMode.fm_UPDATE_MODE;
                }
            }
        }
        private static void Choose_From_List_Country_Matrix(ItemEvent pVal)
        {
            SAPbouiCOM.Form form = SBO_Application.Forms.Item(pVal.FormUID);
            ChooseFromListEvent Chos_Event = (ChooseFromListEvent)pVal;
            if ((Chos_Event.SelectedObjects == null ? false : Chos_Event.SelectedObjects.Rows.Count != 0))
            {
                Matrix Mat_Line = (Matrix)form.Items.Item(pVal.ItemUID).Specific;
                string C = Chos_Event.SelectedObjects.GetValue("Name", 0).ToString();
                EditText Txt_Broker = (EditText)Mat_Line.GetCellSpecific(pVal.ColUID, pVal.Row);
                Txt_Broker.Value = C;
            }

        }
        

        private static void Choose_Parent_ID(Form form)
        {
            //string Code = Utility.Chosse_From_List_For_Code_And_DBDataSource(pVal, pVal.ItemUID);
            //Form form = SBO_Application.Forms.Item(pVal.FormUID);
            UDO_Definition UDO_Info = null;
            string Code = form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).GetValue("U_ST_PARENT_ID", 0);
            string Name_Field = "";
            if (form.DataSources.DBDataSources.Item(0).GetValue("U_ST_PARENT_TYPE", 0) == "C")
            {
                UDO_Info = Logic.Objects_Logic.All_UDO_Definition.FirstOrDefault(U => U.KHCF_Object == KHCF_Objects.Fundraising_Corporate_Card);
                Name_Field = "U_ST_COMMERCIAL_ARABIC_NAME";
            }
            else if (form.DataSources.DBDataSources.Item(0).GetValue("U_ST_PARENT_TYPE", 0) == "I")
            {
                UDO_Info = Logic.Objects_Logic.All_UDO_Definition.FirstOrDefault(U => U.KHCF_Object == KHCF_Objects.Fundraising_Individual_Card);
                Name_Field = "U_ST_FULL_NAME_AR";
            }

            string SQL = $@"SELECT {Name_Field} from ""@{UDO_Info.Table_Name}"" WHERE ""Code"" = '{Code}'";
            Recordset RC = Helper.Utility.Execute_Recordset_Query(company, SQL);
            form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).SetValue("U_ST_PARENT_NAME", 0, RC.Fields.Item(Name_Field).Value.ToString());
        }
        private static void Select_Parent_Type(ItemEvent pVal)
        {
            Form form = SBO_Application.Forms.Item(pVal.FormUID);
            Set_Parent_Link(form);
        }
        private static void Confirm_Link(ItemEvent pVal)
        {
            Form form = SBO_Application.Forms.Item(pVal.FormUID);
            string UDO_Code = form.DataSources.DBDataSources.Item(0).GetValue("Code", 0);
            UDO_Definition UDO_Info = Logic.Objects_Logic.All_UDO_Definition.FirstOrDefault(U => U.KHCF_Object == KHCF_Objects.Fundraising_Individual_Card);
            string Parent_Type = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_PARENT_TYPE", 0);
            string Parent_ID = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_PARENT_ID", 0);
            string Parent_Name = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_PARENT_NAME", 0);

            Logic.KHCF_Logic_Utility.Link(company, UDO_Code, Parent_Type, Parent_ID, Parent_Name, UDO_Info);

            SBO_Application.StatusBar.SetText("Done!", BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Success);
        }

        private static void Run_Actions(ItemEvent pVal)
        {
            Form form = SBO_Application.Forms.Item(pVal.FormUID);
            if (form.Mode != BoFormMode.fm_OK_MODE)
            {
                throw new Logic.Custom_Exception("We can run the action if the form in OK Mode only");
            }
            ButtonCombo Btn_Cmb_Warrning = (ButtonCombo)form.Items.Item("163").Specific;
            string Title = "";
            string Action_ID = form.DataSources.UserDataSources.Item("163").Value;
            if (Action_ID == "-")
                throw new Logic.Custom_Exception("Please select the action");
            else
            {
                Title = Utility.Get_Field_Configuration(company, form.TypeEx.Replace("ST","Frm") + "_" + Action_ID, "", "");
            }

            if (Title == "" || string.IsNullOrEmpty(Title))
                throw new Logic.Custom_Exception($"This Report [{Action_ID}] is not supported");
            //switch (Action_ID)
            //{
            //    case "-"://Can Also
            //        throw new Exception("Please select the action");
            //    case "R"://Remove
            //        Title = "Remove";
            //        break;
            //    case "L"://Link
            //        Title = "Link";
            //        break;
            //    case "U"://Unlink
            //        Title = "Unlink";
            //        break;
            //    default:
            //        throw new Exception($"This Report [{Action_ID}] is not supported");
            //}
            if (SBO_Application.MessageBox($"Are you sure want to {Title} the Card?", 1, "Yes", "No") != 1)
            {
                return;
            }
            string UDO_Code = form.DataSources.DBDataSources.Item(0).GetValue("Code", 0);
            UDO_Definition UDO_Info = Logic.Objects_Logic.All_UDO_Definition.FirstOrDefault(U => U.KHCF_Object == KHCF_Objects.Fundraising_Individual_Card);

            switch (Action_ID)
            {
                case "R"://Remove
                    KHCF_Logic_Utility.Remove_UDO_Entry(company, UDO_Code, UDO_Info);
                    SBO_Application.StatusBar.SetText($"The Card has been Removed successfully!", BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Success);
                    form.Mode = BoFormMode.fm_FIND_MODE;
                    break;
                case "L"://Link
                    if (form.DataSources.DBDataSources.Item(0).GetValue("U_ST_PARENT_TYPE", 0) != "N")
                    {
                        throw new Logic.Custom_Exception("We can link the Card if it is unlinked only");
                    }
                    form.Items.Item("143").SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, -1, BoModeVisualBehavior.mvb_True);
                    form.Items.Item("145").SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, -1, BoModeVisualBehavior.mvb_True);
                    SBO_Application.StatusBar.SetText("Please set the Parent ID", BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Warning);
                    form.Items.Item("145").Click();
                    form.Items.Item("164").Visible = true;
                    SBO_Application.StatusBar.SetText($"Done!", BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Success);
                    break;
                case "U"://Unlink
                    KHCF_Logic_Utility.Unlink(company, UDO_Code, form.DataSources.DBDataSources.Item(0).GetValue("U_ST_BP_Code", 0), UDO_Info);
                    SBO_Application.StatusBar.SetText($"The Card has been unlinked successfully!", BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Success);
                    break;
                default:
                    throw new Logic.Custom_Exception($"This Report [{Action_ID}] is not supported");
            }
        }
        private static void Print_Actions(ItemEvent pVal, SAPbobsCOM.Company oCompany)
        {
            Form form = SBO_Application.Forms.Item(pVal.FormUID);
            if (form.Mode != BoFormMode.fm_OK_MODE)
            {
                throw new Logic.Custom_Exception("We can run the action if the form in OK Mode only");
            }
            ButtonCombo Btn_Cmb_Warrning = (ButtonCombo)form.Items.Item("163").Specific;
            string Title = "";
            string Action_ID = form.DataSources.UserDataSources.Item("163").Value;
            switch (Action_ID)
            {
                case "-"://Can Also
                    throw new Logic.Custom_Exception("Please select the action");
                case "L"://Label
                    Title = "Label";
                    break;
                case "C"://Certificate
                    Title = "Certificate";
                    break;
                case "T"://Tax Report
                    Title = "Tax";
                    break;

                default:
                    throw new Logic.Custom_Exception($"This Report [{Action_ID}] is not supported");
            }
            if (SBO_Application.MessageBox($"Are you sure want to print {Title}?", 1, "Yes", "No") != 1)
            {
                return;
            }
            switch (Action_ID)
            {
                case "L"://Label
                    List<Helper.Utility.Crystal_Report_Parameter> Result = new List<Helper.Utility.Crystal_Report_Parameter>();
                    string Rpt_File = Utility.Get_Configuration(oCompany, "Indi_Fund_Label_Path", "Individual Fundraising Label Print Path", "");
                    string Pdf_File_Name = Helper.Utility.Crystal_Report_Export(oCompany, Rpt_File, Result.ToArray(), Helper.Utility.Export_File_Format.PDF, "", Utility.Get_Configuration(oCompany, "Report_Output_Folder_Path", "Report Output Folder Path",""));
                    SBO_Application.StatusBar.SetText("Report has been Created Successfully at " + Pdf_File_Name, BoMessageTime.bmt_Long, BoStatusBarMessageType.smt_Success);
                    break;
                case "C"://Certificate
                    List<Helper.Utility.Crystal_Report_Parameter> parameters = new List<Helper.Utility.Crystal_Report_Parameter>();
                    string Result_Rpt_File = Utility.Get_Configuration(oCompany, "Indi_Fund_Certificate_Path", "Individual Fundraising Certificate Print Path", "");
                    string Result_Pdf_File_Name = Helper.Utility.Crystal_Report_Export(oCompany, Result_Rpt_File, parameters.ToArray(), Helper.Utility.Export_File_Format.PDF, "", Utility.Get_Configuration(oCompany, "Report_Output_Folder_Path", "Report Output Folder Path", ""));
                    SBO_Application.StatusBar.SetText("Report has been Created Successfully at " + Result_Pdf_File_Name, BoMessageTime.bmt_Long, BoStatusBarMessageType.smt_Success);
                    break;
                case "T"://Tax Report
                    List<Helper.Utility.Crystal_Report_Parameter> tax_parameters = new List<Helper.Utility.Crystal_Report_Parameter>();
                    string tax_Rpt_File = Utility.Get_Configuration(oCompany, "Indi_Fund_Tax_Path", "Individual Fundraising Tax Print Path", "");
                    string tax_Pdf_File_Name = Helper.Utility.Crystal_Report_Export(oCompany, tax_Rpt_File, tax_parameters.ToArray(), Helper.Utility.Export_File_Format.PDF, "", Utility.Get_Configuration(oCompany, "Report_Output_Folder_Path", "Report Output Folder Path", ""));
                    SBO_Application.StatusBar.SetText("Report has been Created Successfully at " + tax_Pdf_File_Name, BoMessageTime.bmt_Long, BoStatusBarMessageType.smt_Success);
                    break;
                default:
                    throw new Logic.Custom_Exception($"This Report [{Action_ID}] is not supported");
            }
        }


        private static void Remove_Address_Row(ItemEvent pVal)
        {
            Form form = SBO_Application.Forms.Item(pVal.FormUID);
            Matrix Mat = (Matrix)form.Items.Item("20").Specific;
            int Count = Mat.RowCount;
            for (int i = Count; i > 0; i--)
            {
                //CheckBox Chk_Selected = (CheckBox)Mat.GetCellSpecific("SELECTED", i);
                CheckBox Chk_Selected = (CheckBox)Mat.Columns.Item("SELECTED").Cells.Item(i).Specific;
                if (Chk_Selected.Checked)
                {
                    Mat.DeleteRow(i);
                    // form.DataSources.DBDataSources.Item(1).RemoveRecord(Grid_Deleted_Row +1);
                    Mat.FlushToDataSource();
                    Mat.LoadFromDataSource();

                }
            }

        }
        private static void Add_Address_Row(ItemEvent pVal)
        {
            Form form = SBO_Application.Forms.Item(pVal.FormUID);
            form.Freeze(true);
            DBDataSource DS_Address = form.DataSources.DBDataSources.Item("@ST_FUND_INDIV_ADDR");
            Matrix Mat_Add = (Matrix)form.Items.Item("20").Specific;
            Mat_Add.FlushToDataSource();
            int Count = DS_Address.Size;
            if (Count == 1)
            {
                if (DS_Address.GetValue("U_ST_ADDRESS_NAME", Count - 1) != "")
                {
                    DS_Address.InsertRecord(Count);
                }
                else
                {
                    Count = 0;

                    Mat_Add.LoadFromDataSource();
                }
            }
            else
            {
                DS_Address.InsertRecord(Count);
            }

            DS_Address.SetValue("U_ST_ADDRESS_TYPE", Count, "B");

            Mat_Add.LoadFromDataSource();
            form.Freeze(false);

        }


        private static void Add_Members(Form form, string UDO_Code, string UDO_Name)
        {
            company.StartTransaction();
            try
            {
                DataTable DT_Members = form.DataSources.DataTables.Item("MEMBERS");
                List<string> Codes = new List<string>();
                Field_Definition[] Address_Fields = Logic.Fields_Logic.All_Field_Definition.Where(U => U.KHCF_Object == KHCF_Objects.Fundraising_Individual_Address).ToArray();
                for (int i = 0; i < DT_Members.Rows.Count; i++)
                {
                    if (DT_Members.GetValue("Code", i).ToString() == "")
                    {
                        //KHCF_BP BP = new KHCF_BP();
                        //BP.BP_Group = int.Parse(form.DataSources.DBDataSources.Item(0).GetValue("U_ST_CUSTOMER_GROUP", 0));
                        //BP.CardName = DT_Members.GetValue("ST_FULL_NAME_AR", 0).ToString();
                        //BP.FatherCode = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_BP_CODE", 0);
                        //if (form.DataSources.DBDataSources.Item(0).GetValue("U_ST_FATHER_TYPE", 0) == "P")
                        //{
                        //    BP.FatherType = BoFatherCardTypes.cPayments_sum;
                        //}
                        //else
                        //{
                        //    BP.FatherType = BoFatherCardTypes.cDelivery_sum;
                        //}

                        string BP_Code = "";
                        string New_UDO_Code = Utility.Get_New_UDO_Code(company, Form_Obj.KHCF_Object);
                        Codes.Add(New_UDO_Code);

                        SAPbobsCOM.CompanyService oCmpSrv = company.GetCompanyService();
                        SAPbobsCOM.GeneralService oGeneralService = oCmpSrv.GetGeneralService(form.BusinessObject.Type);
                        SAPbobsCOM.GeneralData oGeneralData = (SAPbobsCOM.GeneralData)oGeneralService.GetDataInterface(SAPbobsCOM.GeneralServiceDataInterfaces.gsGeneralData);

                        oGeneralData.SetProperty("Code", New_UDO_Code);
                        oGeneralData.SetProperty("U_ST_BP_CODE", BP_Code);
                        oGeneralData.SetProperty("U_ST_CREATOR", company.UserName);
                        //oGeneralData.SetProperty("U_ST_PARENT_ID", UDO_Code);
                        //oGeneralData.SetProperty("U_ST_PARENT_NAME", UDO_Name);
                        //oGeneralData.SetProperty("U_ST_PARENT_TYPE", "I");
                        oGeneralData.SetProperty("U_ST_FATHER", UDO_Code);
                        oGeneralData.SetProperty("U_ST_PARENT_NAME", UDO_Name);
                        //oGeneralData.SetProperty("U_ST_PARENT_TYPE", "I");
                        //oGeneralData.SetProperty("U_ST_CUSTOMER_GROUP", BP.BP_Group.ToString());
                        oGeneralData.SetProperty("U_ST_ACCOUNT_MANAGER", form.DataSources.DBDataSources.Item(0).GetValue("U_ST_ACCOUNT_MANAGER", 0));
                        oGeneralData.SetProperty("U_ST_SUB_ACCOUNT_MANAGER", form.DataSources.DBDataSources.Item(0).GetValue("U_ST_SUB_ACCOUNT_MANAGER", 0));
                        oGeneralData.SetProperty("U_ST_RESIDENCY", form.DataSources.DBDataSources.Item(0).GetValue("U_ST_RESIDENCY", 0));
                        //oGeneralData.SetProperty("U_ST_NATIONALITY", DT_Members.GetValue("ST_NATIONALITY", 0).ToString());
                        oGeneralData.SetProperty("U_ST_FULL_NAME_AR", DT_Members.GetValue("ST_FULL_NAME_AR", 0).ToString());
                        oGeneralData.SetProperty("U_ST_FULL_NAME_EN", DT_Members.GetValue("ST_FULL_NAME_EN", 0).ToString());

                        for (int j = 0; j < DT_Members.Columns.Count; j++)
                        {
                            string Col_Name = DT_Members.Columns.Item(j).Name;
                            if (Col_Name == "SELECTED" || Col_Name == "Code")
                            {
                                continue;
                            }
                            if (DT_Members.GetValue(Col_Name, i) != null)
                            {
                                oGeneralData.SetProperty($"U_{Col_Name}", DT_Members.GetValue(Col_Name, i));
                            }
                        }
                        SAPbobsCOM.GeneralDataCollection Address_Children = oGeneralData.Child("ST_FUND_INDIV_ADDR");
                        for (int J = 0; J < form.DataSources.DBDataSources.Item("@ST_FUND_INDIV_ADDR").Size; J++)
                        {
                            SAPbobsCOM.GeneralData oChild = Address_Children.Add();
                            foreach (var One_Add_Field in Address_Fields)
                            {
                                if (One_Add_Field.Is_Temp == true)
                                {
                                    continue;
                                }
                                oChild.SetProperty(One_Add_Field.Column_Name_In_DB, form.DataSources.DBDataSources.Item("@ST_FUND_INDIV_ADDR").GetValue(One_Add_Field.Column_Name_In_DB, J));
                            }
                        }
                        oGeneralService.Add(oGeneralData);
                        SBO_Application.StatusBar.SetText($"New Sub Member has been created with Code[{New_UDO_Code}]", BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Success);
                    }
                }
                company.EndTransaction(BoWfTransOpt.wf_Commit);
            }
            catch (Exception ex)
            {
                try
                {
                    company.EndTransaction(BoWfTransOpt.wf_RollBack);
                }
                catch (Exception)
                {
                }
                throw new Logic.Custom_Exception($"Error during add the new members[{ex.Message}]");
            }
        }
        private static void Remove_Member_Row(ItemEvent pVal)
        {
            Form form = SBO_Application.Forms.Item(pVal.FormUID);
            DataTable DT_Members = form.DataSources.DataTables.Item("MEMBERS");

            for (int i = DT_Members.Rows.Count - 1; i >= 0; i--)
            {
                if (DT_Members.GetValue("SELECTED", i).ToString() != "Y")
                {
                    continue;
                }
                string Sub_Code = DT_Members.GetValue("Code", i).ToString();
                if (Sub_Code != "")
                {
                    if (form.Mode != BoFormMode.fm_OK_MODE)
                    {
                        throw new Logic.Custom_Exception("We can unlink the member if the form in the OK mode only");
                    }
                    UDO_Definition UDO_Info = Logic.Objects_Logic.All_UDO_Definition.FirstOrDefault(U => U.KHCF_Object == KHCF_Objects.Fundraising_Individual_Card);
                    //string BP_Sub_Card = Utility.Get_BP_Code(company, Sub_Code, UDO_Info);
                    //KHCF_Logic_Utility.Unlink(company, Sub_Code, BP_Sub_Card, UDO_Info);
                    Field_Data Fld_Father = new Field_Data() { Field_Name = "U_ST_FATHER", Data_Type = BoFieldTypes.db_Alpha, Value = "" };
                    Field_Data Fld_Father_Relation = new Field_Data() { Field_Name = "U_ST_RELATIONSHIP_TO_FATHER", Data_Type = BoFieldTypes.db_Alpha, Value = "" };

                    Utility.Update_UDO(company, UDO_Info, Sub_Code, new Field_Data[] { Fld_Father, Fld_Father_Relation });

                    SBO_Application.StatusBar.SetText($"The Card[{Sub_Code}] has been unlinked successfully!", BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Success);
                }
                DT_Members.Rows.Remove(i);
            }
        }
        private static void Add_Member_Row(ItemEvent pVal)
        {
            Form form = SBO_Application.Forms.Item(pVal.FormUID);
            form.Freeze(true);
            DataTable DT_Members = form.DataSources.DataTables.Item("MEMBERS");

            DT_Members.Rows.Add();
            DT_Members.SetValue("ST_CUSTOMER_GROUP", DT_Members.Rows.Count - 1, form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).GetValue("U_ST_CUSTOMER_GROUP", 0).ToString());

            Grid Grd_Members = (Grid)form.Items.Item("138").Specific;
            for (int j = 2; j < Grd_Members.Columns.Count + 1; j++)
            {
                Grd_Members.CommonSetting.SetCellEditable(DT_Members.Rows.Count, j, true);
            }
            Grd_Members.Columns.Item("ST_FULL_NAME_AR").Editable = false;
            Grd_Members.Columns.Item("ST_FULL_NAME_EN").Editable = false;

            if (form.Mode == BoFormMode.fm_OK_MODE)
            {
                form.Mode = BoFormMode.fm_UPDATE_MODE;
            }
            form.Freeze(false);

        }

        //private static void Add_Contacts_Row(ItemEvent pVal)
        //{
        //    Form form = SBO_Application.Forms.Item(pVal.FormUID);
        //    form.Freeze(true);
        //    DBDataSource DS_Contacts = form.DataSources.DBDataSources.Item("@ST_FUND_INDIV_CONT");
        //    Matrix Mat_Cont = (Matrix)form.Items.Item("834").Specific;
        //    Mat_Cont.FlushToDataSource();
        //    int Count = DS_Contacts.Size;
        //    if (Count == 1)
        //    {
        //        if (DS_Contacts.GetValue("U_ST_POSITION", Count - 1) != "")
        //        {
        //            DS_Contacts.InsertRecord(Count);
        //        }
        //        else
        //        {
        //            Count = 0;
        //            DS_Contacts.InsertRecord(Count);
        //            Mat_Cont.LoadFromDataSource();
        //            Mat_Cont.DeleteRow(1);
        //            Mat_Cont.FlushToDataSource();
        //            Mat_Cont.LoadFromDataSource();
        //        }
        //    }
        //    else
        //    {
        //        DS_Contacts.InsertRecord(Count);
        //    }

        //    //DS_Contacts.SetValue("U_ST_ADDRESS_TYPE", Count, "S");
        //    //DS_Contacts.SetValue("U_ST_COUNTRY", Count, "JO");

        //    Mat_Cont.LoadFromDataSource();
        //    form.Freeze(false);

        //}
        //private static void Remove_Contacts_Row(ItemEvent pVal)
        //{
        //    Form form = SBO_Application.Forms.Item(pVal.FormUID);
        //    Form_Obj.Remove_Matrix_Row(form, "701");
        //}
        //private static int CheckContacts(BusinessObjectInfo businessObjectInfo)
        //{
        //    int result = -5;
        //    SAPbouiCOM.Form form = SBO_Application.Forms.Item(businessObjectInfo.FormUID);
        //    Matrix Mat_Contact = (Matrix)form.Items.Item("834").Specific;
        //    for (int i = 0; i < Mat_Contact.RowCount; i++)
        //    {
        //        SAPbouiCOM.EditText Email = (SAPbouiCOM.EditText)Mat_Contact.Columns.Item("E-Mail").Cells.Item(i + 1).Specific;
        //        bool isEmail = Regex.IsMatch(Email.Value, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase);
        //        if (!isEmail && !string.IsNullOrEmpty(Email.Value))
        //        {
        //            result = i;
        //            return result;
        //        }
        //    }
        //    return result;
        //}

        private static void Choose_Father(ItemEvent pVal)
        {
            Form form = SBO_Application.Forms.Item(pVal.FormUID);
            
            SAPbouiCOM.ChooseFromListEvent Choos_Event = (SAPbouiCOM.ChooseFromListEvent)pVal;

            if (Choos_Event.SelectedObjects == null || Choos_Event.SelectedObjects.Rows.Count == 0)
            {
                return;
            }

            string Code = Choos_Event.SelectedObjects.GetValue("Code", 0).ToString();

            form.DataSources.DBDataSources.Item("@ST_FUND_INDIV_CARD").SetValue("U_ST_FATHER", 0, Code);
            string s =form.DataSources.DBDataSources.Item("@ST_FUND_INDIV_CARD").GetValue("U_ST_FATHER", 0);

        }
    }
}
