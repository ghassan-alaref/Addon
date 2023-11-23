using SAPbobsCOM;
using SAPbouiCOM;
using ST.KHCF.Customization.Logic.Classes;
using ST.KHCF.Customization.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection.Emit;
using System.IO;
using System.Text.RegularExpressions;

namespace ST.KHCF.Customization.Forms.Fundraising
{
    internal class Frm_Fundraising_Corporate_Card : Parent_Form
    {
        internal static SAPbobsCOM.Company company;
        internal static Application SBO_Application;
        internal static Parent_Form Form_Obj;
        internal static string[] Relations_Grid_IDs = new string[] { "330" };
        private static List<Utility.Item_UI> Grids_List = new List<Utility.Item_UI>();
        internal static Dictionary<string, string> Flags_List = new Dictionary<string, string>() {
            { "52" , "862"},
            { "53" , "863"},
            { "54" , "864"},
            { "55" , "865"},
            { "65" , "866"}
        };

        //internal override string[] Get_Mondatory_Fields_List()
        //{
        //    List<string> Result = new List<string>();
        //    Result.AddRange(base.Get_Mondatory_Fields_List());
        //    Result.AddRange(new string[] { "132","43","18","618","39","26"});  "132,43,18,618,39,26"

        //    return Result.ToArray();
        //}
        //internal override string[] Get_Fix_ReadOnly_Fields_List()
        //{
        //    List<string> Result = new List<string>();
        //    Result.AddRange(base.Get_Fix_ReadOnly_Fields_List());
        //   // Result.AddRange(new string[] { "5", "645", "7", "610", "624", "621", "705", "707", "709", "711", "713", "715", "717", "719", "721", "723", "725", "727", "729", "731", "733", "735", "737", "739", "741", "743", "745" });

        //    return Result.ToArray();
        //}

        //internal override string[] Get_Approval_Items_List()
        //{
        //    List<string> Result = new List<string>();
        //    Result.AddRange(base.Get_Approval_Items_List());
        //   // Result.AddRange(new string[] { "159", "160", "161", "162" });

        //    return Result.ToArray();
        //}
        //internal override string[] Get_Tab_Item_List()
        //{
        //    List<string> Result = new List<string>();
        //    Result.AddRange(base.Get_Tab_Item_List());
        //    Result.Add("3");
        //    Result.Add("4");
        //    Result.Add("19");
        //    Result.Add("35");  "3,4,19,35"
        //    //Result.Add("776");
        //    //Result.AddRange(new string[] { "777", "790", "791", "800" });

        //    return Result.ToArray();
        //}
        internal override void Initialize_Form(Form form)
        {
            //Code_value = "Frm_Fundraising_Corporate_Card";
            //Desc_value = "Mandatary fields List For Fundraising Corporate Card ";
            //Man_fields = "132,43,18,618,39,26";

            //File.AppendAllText("C:\\Users\\wsaleh\\Desktop\\Fund_TimeLog.txt", "FundRaising Corporate Card" + Environment.NewLine);
            //string startTimeExact = DateTime.Now.ToString("mm:ss:f");
            //DateTime startTime = DateTime.Now;
            //File.AppendAllText("C:\\Users\\wsaleh\\Desktop\\Fund_TimeLog.txt", "Start Time:" + startTimeExact + Environment.NewLine);
            base.Initialize_Form(form);
            //DateTime prev = Utility.Add_Time_Log("F", "base.Initialize Form", startTime);
            //Matrix Mat_Add = (Matrix)form.Items.Item("20").Specific;
            //base.Fill_Address_ComboBox(Mat_Add);
            //prev = Utility.Add_Time_Log("F", "Address", prev);
           // Matrix Mat_Att = (Matrix)form.Items.Item("500").Specific;
            //base.Fill_Attachment_ComboBox(Mat_Att);
            //prev = Utility.Add_Time_Log("F", "Attachment", prev);
            string SQL_Customer_Group = $@"SELECT ""GroupCode"" AS ""Code"", T0.""GroupName"" AS ""Name"" FROM OCRG T0 
WHERE  T0.""GroupType""  = 'C' AND T0.""U_ST_TYPE"" = 'C' AND U_ST_CUSTOMER_TYPE = 'F'";
            Helper.Utility.Fill_One_ComboBoxBySQL(company, form, "43", SQL_Customer_Group, true);
            //prev = Utility.Add_Time_Log("F", "Customer Group", prev);
            Grid Grd_Members = (Grid)form.Items.Item("138").Specific;
            ((ComboBoxColumn)Grd_Members.Columns.Item("ST_GENDER")).ValidValues.Add("M", "Male");
            ((ComboBoxColumn)Grd_Members.Columns.Item("ST_GENDER")).ValidValues.Add("F", "Female");
            ((ComboBoxColumn)Grd_Members.Columns.Item("ST_GENDER")).DisplayType = BoComboDisplayType.cdt_Description;
            Grd_Members.Columns.Item("SELECTED").AffectsFormMode = false;

            string SQL_Customer_Indiv_Group = $@"SELECT ""GroupCode"" AS ""Code"", T0.""GroupName"" AS ""Name"" FROM OCRG T0 
WHERE  T0.""GroupType""  = 'C' AND T0.""U_ST_TYPE"" = 'I' AND U_ST_CUSTOMER_TYPE = 'F'";
            string SQL_Branch = $@"SELECT T0.""Code"", T0.""Name"" FROM ""@ST_BRANCH"" T0";
            string SQL_Job_Title = $@"SELECT T0.""Code"", T0.""Name"" FROM ""@ST_JOB_TITLE"" T0";

            Helper.Utility.FillGridComboBoxForSQL(company, Grd_Members, "ST_CUSTOMER_GROUP", SQL_Customer_Indiv_Group, true);
            Helper.Utility.FillGridComboBoxForSQL(company, Grd_Members, "ST_BRANCH", SQL_Branch, true);
            Helper.Utility.FillGridComboBoxForSQL(company, Grd_Members, "ST_JOB_TITLE", SQL_Job_Title, true);

            string SQL_Relation_To_Father = $@"SELECT ""Code"", ""Name"" from ""@ST_RELATION_FATHER""";
            Helper.Utility.FillGridComboBoxForSQL(company, Grd_Members, "ST_RELATIONSHIP_TO_FATHER", SQL_Relation_To_Father, true);
            ((ComboBoxColumn)Grd_Members.Columns.Item("ST_REGION")).ValidValues.Add("L", "Local");
            ((ComboBoxColumn)Grd_Members.Columns.Item("ST_REGION")).ValidValues.Add("R", "Regional");
            ((ComboBoxColumn)Grd_Members.Columns.Item("ST_REGION")).ValidValues.Add("I", "International");
            ((ComboBoxColumn)Grd_Members.Columns.Item("ST_REGION")).DisplayType = BoComboDisplayType.cdt_Description;


            DataTable DT_Departments = form.DataSources.DataTables.Item("DT_Departments");
            Dictionary<string, string> Departments = new Dictionary<string, string>();
            Departments.Add("LID", "LID");
            Departments.Add("LCD", "LCD");
            Departments.Add("IDD", "IDD");
            Helper.Utility.FillGridForDictionary(DT_Departments, Departments, true);
            ((Grid)form.Items.Item("330").Specific).Columns.Item("Code").Visible = false;
           
                       
            Grd_Members.Columns.Item("SELECTED").AffectsFormMode = false;

            Grd_Members.AutoResizeColumns();
            ((Matrix)form.Items.Item("20").Specific).AutoResizeColumns();

            Grids_List = Utility.Get_Grids_UI(form, Relations_Grid_IDs);

            form.Items.Item("3").Click();
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
                    form.Items.Item("31").SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 4, BoModeVisualBehavior.mvb_False);
                    form.Items.Item("32").SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 4, BoModeVisualBehavior.mvb_False);
                    form.Items.Item("139").SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 4, BoModeVisualBehavior.mvb_False);
                    form.Items.Item("140").SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 4, BoModeVisualBehavior.mvb_False);
                    form.Items.Item("702").SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 4, BoModeVisualBehavior.mvb_False);
                    form.Items.Item("703").SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 4, BoModeVisualBehavior.mvb_False);
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
                }

                if ((BusinessObjectInfo.EventType == BoEventTypes.et_FORM_DATA_UPDATE || BusinessObjectInfo.EventType == BoEventTypes.et_FORM_DATA_ADD) && BusinessObjectInfo.ActionSuccess && !BusinessObjectInfo.BeforeAction)
                {
                    ADD_Update_UDO(BusinessObjectInfo);
                }

                if (BusinessObjectInfo.EventType == BoEventTypes.et_FORM_DATA_LOAD && !BusinessObjectInfo.BeforeAction)
                {
                    Form_Data_Load(BusinessObjectInfo);
                }

                //Form form = Loader.SBO_Application.Forms.Item(BusinessObjectInfo.FormUID);
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
                //SBO_Application.                SBO_Application.StatusBar.SetText(ex.Message, BoMessageTime.bmt_Short,  BoStatusBarMessageType.smt_Error);(ex.Message, BoMessageTime.bmt_Short, true);
                Loader.New_Msg = ex.Message;

                BubbleEvent = false;
            }

            return BubbleEvent;
        }

        private static void Form_Data_Load(BusinessObjectInfo businessObjectInfo)
        {
            Form form = SBO_Application.Forms.Item(businessObjectInfo.FormUID);
            string Card_ID = form.DataSources.DBDataSources.Item(0).GetValue("Code", 0);
            string SQL_Sum_Donation = $@"SELECT SUM(T0.""U_ST_DONATION_AMOUNT"") 
FROM ""@ST_ACTUAL_DONATIONS""  T0 WHERE T0.""U_ST_CONTACT_CARD"" = '{Card_ID}' AND  T0.""U_ST_CARD_TYPE"" = 'C'";
            Recordset RC_Sum_Donation = Helper.Utility.Execute_Recordset_Query(company, SQL_Sum_Donation);
            form.DataSources.UserDataSources.Item("610").Value = RC_Sum_Donation.Fields.Item(0).Value.ToString();


            string SQL_Sum_Contact_Donation = $@"SELECT SUM(T0.""U_ST_DONATION_AMOUNT"") 
FROM ""@ST_ACTUAL_DONATIONS""  T0 WHERE  T0.""U_ST_CARD_TYPE"" = 'C' AND 
T0.""U_ST_AMBASSADOR"" in (SELECT ""Code"" FROM ""@ST_FUND_INDIV_CARD"" T0 
WHERE T0.""U_ST_FATHER"" = '{Card_ID}')";
            Recordset RC_Sum_ContactDonation = Helper.Utility.Execute_Recordset_Query(company, SQL_Sum_Contact_Donation);
            form.DataSources.UserDataSources.Item("612").Value = RC_Sum_ContactDonation.Fields.Item(0).Value.ToString();

            Load_Sub_Members(form, Card_ID);

            string UDO_Code = form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).GetValue("Code", 0);
            Utility.Load_All_Relation_Data(company, form, UDO_Code, Relations_Grid_IDs, Form_Obj.KHCF_Object);

            ((Matrix)form.Items.Item("20").Specific).AutoResizeColumns();
            ((Matrix)form.Items.Item("500").Specific).AutoResizeColumns();

            Set_Flags_Visibility(form);

            Form_Obj.Set_Fields(form);

        }

        private static void Load_Sub_Members(Form form, string Card_ID)
        {
            DataTable DT_Members = form.DataSources.DataTables.Item("MEMBERS");
            DT_Members.Rows.Clear();

            string SQL_Members = $@"SELECT *  FROM ""@ST_FUND_INDIV_CARD""  T0 
WHERE T0.""U_ST_PARENT_ID"" = '{Card_ID}' and T0.""U_ST_PARENT_TYPE"" = 'C'";
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
            string UDO_Name = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_COMMERCIAL_ARABIC_NAME", 0);

            Add_Members(form, UDO_Code, UDO_Name);

            Check_Flags_Logic(form, UDO_Code);

            string BP_Code = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_BP_CODE", 0);
            Utility.Update_Relation_Table(company, form, UDO_Code, Relations_Grid_IDs, Form_Obj.KHCF_Object);

            Set_Flags_Visibility(form);

        }

        private static void Check_Flags_Logic(Form form, string UDO_Code)
        {
            string X = form.DataSources.DBDataSources.Item(0).TableName;
            bool Is_Lead = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_IS_LEAD", 0) == "Y";
            bool Is_Donor = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_IS_DONOR", 0) == "Y";
           
            if (!(Is_Lead || Is_Donor ))
            {
                return;
            }
             
            string BP_Code = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_BP_CODE", 0);
            if (BP_Code == "")
            {

                KHCF_BP BP = new KHCF_BP();

                BP.BP_Group = int.Parse(form.DataSources.DBDataSources.Item(0).GetValue("U_ST_CUSTOMER_GROUP", 0));
                BP.CardName = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_COMPANY_ARABIC_NAME", 0);
                BP.MemberCard_Code = UDO_Code;
                if (Is_Donor == true)
                {
                    BP.Is_Lead = false;
                }
                else if (Is_Lead == true && Is_Donor == false)
                {
                    BP.Is_Lead = true;
                }
                else
                {
                    throw new Logic.Custom_Exception("We can't set the BP Type");
                }

                string SQL_Addr = $@"SELECT T0.""U_ST_ADDRESS_NAME"", T0.""U_ST_STREET"", T0.""U_ST_BLOCK"", T0.""U_ST_ZIP_CODE"", T0.""U_ST_CITY""
, T0.""U_ST_COUNTY"",  T0.""U_ST_STATE"", T0.""U_ST_BUILDING"", T0.""U_ST_ADDRESS_TYPE"", T0.""U_ST_ADDRESS_NAME_2""
, T0.""U_ST_ADDRESS_NAME_3"", T0.""U_ST_STREET_NO""
,(Select T1.""Code"" From OCRY T1 Where T1.""Name"" =U_ST_COUNTRY)As U_ST_COUNTRY 
FROM ""@ST_FUND_CORP_ADDR"" T0 WHERE ""Code"" = '{UDO_Code}' ";
                Recordset RC_Addr = Helper.Utility.Execute_Recordset_Query(company, SQL_Addr);
                BP.addresses = new List<BpAddress>();
                if (RC_Addr.RecordCount > 0)
                {
                    BP.addresses = new List<BpAddress>();
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
                        BP.addresses.Add(address);
                        RC_Addr.MoveNext();
                    }
                }
                try
                {
                    company.StartTransaction();
                    BP_Code = Utility.Create_BP(company, BP);
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
                    catch (Exception)
                    { }
                    throw;
                }
            }

            Utility.Update_BP(company, BP_Code, UDO_Code, Form_Obj.UDO_Info, null , !Is_Donor);


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
                        UDO_Definition UDO_Info= Logic.Objects_Logic.All_UDO_Definition.FirstOrDefault(U => U.KHCF_Object == KHCF_Objects.Fundraising_Individual_Card);
                        string New_UDO_Code = Utility.Get_New_UDO_Code(company, UDO_Info.KHCF_Object);
                        Codes.Add(New_UDO_Code);

                        SAPbobsCOM.CompanyService oCmpSrv = company.GetCompanyService();
                        SAPbobsCOM.GeneralService oGeneralService = oCmpSrv.GetGeneralService("ST_FUND_INDIV_CARD");
                        SAPbobsCOM.GeneralData oGeneralData = (SAPbobsCOM.GeneralData)oGeneralService.GetDataInterface(SAPbobsCOM.GeneralServiceDataInterfaces.gsGeneralData);

                        oGeneralData.SetProperty("Code", New_UDO_Code);
                        oGeneralData.SetProperty("U_ST_BP_CODE", BP_Code);
                        oGeneralData.SetProperty("U_ST_PARENT_ID", UDO_Code);
                        //oGeneralData.SetProperty("U_ST_PARENT_NAME", UDO_Name);
                        oGeneralData.SetProperty("U_ST_PARENT_TYPE", "C");
                        oGeneralData.SetProperty("U_ST_FATHER", UDO_Code);
                        oGeneralData.SetProperty("U_ST_CORPORATE", UDO_Code);
                        oGeneralData.SetProperty("U_ST_PARENT_NAME", UDO_Name);
                        //oGeneralData.SetProperty("U_ST_PARENT_TYPE", "I");
                        //oGeneralData.SetProperty("U_ST_CUSTOMER_GROUP", BP.BP_Group.ToString());
                        oGeneralData.SetProperty("U_ST_DEPARTMENT", form.DataSources.DBDataSources.Item(0).GetValue("U_ST_DEPARTMENT", 0));
                        //oGeneralData.SetProperty("U_ST_SUB_ACCOUNT_MANAGER", form.DataSources.DBDataSources.Item(0).GetValue("U_ST_SUB_ACCOUNT_MANAGER", 0));
                        //oGeneralData.SetProperty("U_ST_RESIDENCY", form.DataSources.DBDataSources.Item(0).GetValue("U_ST_RESIDENCY", 0));
                        //oGeneralData.SetProperty("U_ST_NATIONALITY", DT_Members.GetValue("ST_NATIONALITY", 0).ToString());
                        oGeneralData.SetProperty("U_ST_FULL_NAME_AR", DT_Members.GetValue("ST_FULL_NAME_AR", 0).ToString());
                        oGeneralData.SetProperty("U_ST_FULL_NAME_EN", DT_Members.GetValue("ST_FULL_NAME_EN", 0).ToString());
                        oGeneralData.SetProperty("U_ST_EMAIL", DT_Members.GetValue("ST_EMAIL", 0).ToString());
                        oGeneralData.SetProperty("U_ST_JOB_TITLE", DT_Members.GetValue("ST_JOB_TITLE", 0).ToString());

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
                        for (int J = 0; J < form.DataSources.DBDataSources.Item("@ST_FUND_CORP_ADDR").Size; J++)
                        {
                            SAPbobsCOM.GeneralData oChild = Address_Children.Add();
                            foreach (var One_Add_Field in Address_Fields)
                            {
                                if (One_Add_Field.Is_Temp == true)
                                {
                                    continue;
                                }
                                oChild.SetProperty(One_Add_Field.Column_Name_In_DB, form.DataSources.DBDataSources.Item("@ST_FUND_CORP_ADDR").GetValue(One_Add_Field.Column_Name_In_DB, J));
                            }
                        }
                        oGeneralService.Add(oGeneralData);
                        Utility.Update_Relation_Table(company, form, New_UDO_Code, new string[] { "330" }, KHCF_Objects.Fundraising_Individual_Card);
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
                DataTable DT_Members = form.DataSources.DataTables.Item("MEMBERS");
                DT_Members.Rows.Clear();

                DataTable DT_Departments = form.DataSources.DataTables.Item("DT_Departments");
                Dictionary<string, string> Departments = new Dictionary<string, string>();
                Departments.Add("LID", "LID");
                Departments.Add("LCD", "LCD");
                Departments.Add("IDD", "IDD");
                Helper.Utility.FillGridForDictionary(DT_Departments, Departments, true);
                SBO_Application.Forms.ActiveForm.DefButton = "1";

            }
            if (SBO_Application.Forms.ActiveForm.Mode != BoFormMode.fm_FIND_MODE)
            {
                Form_Obj.Set_Fields(SBO_Application.Forms.ActiveForm);               
            }
            else
            {
                Form_Obj.UnSet_Mondatory_Fields_Color(SBO_Application.Forms.ActiveForm);
                DisableTabButtons(SBO_Application.Forms.ActiveForm);
            }
        }

        private static void Before_Adding_UDO(BusinessObjectInfo businessObjectInfo)
        {
            Form form = SBO_Application.Forms.Item(businessObjectInfo.FormUID);
            DataTable DT_Members = form.DataSources.DataTables.Item("MEMBERS");
            int X = DT_Members.Rows.Count;
            //throw new Exception("fasfaf");
            KHCF_BP BP = new KHCF_BP();
            //BP.BP_Group = int.Parse(form.DataSources.DBDataSources.Item(0).GetValue("U_ST_CUSTOMER_GROUP", 0));
            //BP.CardName = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_CORPORATE_ARABIC_NAME", 0);
            //string BP_Code = Utility.Create_BP(company, BP);

            Set_Default_Value_Before_Adding(form);


        }

        private static void Set_Default_Value_Before_Adding(Form form)
        {
            //form.DataSources.DBDataSources.Item(0).SetValue("U_ST_BP_CODE", 0, BP_Code);
            if (form.Mode == BoFormMode.fm_ADD_MODE)
            {
                string New_UDO_Code = Utility.Get_New_UDO_Code(company, Form_Obj.KHCF_Object);
                form.DataSources.DBDataSources.Item(0).SetValue("Code", 0, New_UDO_Code);
                form.DataSources.DBDataSources.Item(0).SetValue("U_ST_CREATOR", 0, company.UserName);
                //form.DataSources.DBDataSources.Item(0).SetValue("U_ST_PARENT_TYPE", 0, "C");
            }

            string Att_Folder = Configurations.Get_Attachment_Folder(company);
            for (int i = 0; i < form.DataSources.DBDataSources.Item("@ST_FUND_CORP_ATT").Size; i++)
            {
                if (form.DataSources.DBDataSources.Item("@ST_FUND_CORP_ATT").GetValue("LineId", i) == "-1")
                {
                    string User_File_Path = form.DataSources.DBDataSources.Item("@ST_FUND_CORP_ATT").GetValue("U_ST_FILE_NAME", i);
                    string FileName = System.IO.Path.GetFileName(User_File_Path);
                    if (!System.IO.Directory.Exists(Att_Folder))
                    {
                        System.IO.Directory.CreateDirectory(Att_Folder);
                    }
                    string New_Path = System.IO.Path.Combine(Att_Folder, DateTime.Now.ToString("yyyyMMdd_HHmmss_") + FileName);
                    System.IO.File.Copy(User_File_Path, New_Path);
                    form.DataSources.DBDataSources.Item("@ST_FUND_CORP_ATT").SetValue("U_ST_FILE_NAME", i, New_Path);
                    form.DataSources.DBDataSources.Item("@ST_FUND_CORP_ATT").SetValue("LineId", i, "");
                    Loader.SBO_Application.StatusBar.SetText($"The File [{User_File_Path}] has been copied to the Addon attachment folder", BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Warning);
                }

            }


        }

        private static void Change_Items_Values_Cutomization(string ItemUID, string FormUID, bool BeforeAction, bool ItemChanged, string ColUID, int Row, bool OnLoad = false)
        {
            #region Names Fields

            string[] Arabic_Names_Items = new string[] { "18", "12" };
            if (Arabic_Names_Items.Contains(ItemUID))
            {
                SAPbouiCOM.Form form = SBO_Application.Forms.Item(FormUID);

                string CommericalName = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_COMMERCIAL_ARABIC_NAME", 0);
                string CompanyName = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_COMPANY_ARABIC_NAME", 0);
               
                if (!Utility.Check_Text(CommericalName) && !string.IsNullOrEmpty(CommericalName))
                {
                    form.DataSources.DBDataSources.Item(0).SetValue("U_ST_COMMERCIAL_ARABIC_NAME", 0, string.Empty);
                    form.Items.Item("12").Click(); 
                }
                else if (!Utility.Check_Text(CompanyName) && !string.IsNullOrEmpty(CompanyName))
                {
                    form.DataSources.DBDataSources.Item(0).SetValue("U_ST_COMPANY_ARABIC_NAME", 0, string.Empty);
                    form.Items.Item("18").Click();
                    
                }

            }

            string[] English_Names_Items = new string[] { "13", "618" };

            if (English_Names_Items.Contains(ItemUID))
            {
                SAPbouiCOM.Form form = SBO_Application.Forms.Item(FormUID);
                string ComericalName = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_COMMERCIAL_ENGLISH_NAME", 0);
                string CompanyName = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_COMPANY_ENGLISH_NAME", 0);
               
                if (Utility.Check_Text(ComericalName) && !string.IsNullOrEmpty(CompanyName))
                {
                    form.DataSources.DBDataSources.Item(0).SetValue("U_ST_COMMERCIAL_ENGLISH_NAME", 0, string.Empty);
                    form.Items.Item("13").Click();
                }
                else if (Utility.Check_Text(CompanyName) && !string.IsNullOrEmpty(CompanyName))
                {
                    form.DataSources.DBDataSources.Item(0).SetValue("U_ST_COMPANY_ENGLISH_NAME", 0, string.Empty);
                    form.Items.Item("618").Click();
                }
               
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
            //if (ItemUID == "138")
            //{
            //    string[] Names_Cols = new string[] { "ST_FIRST_NAME_AR", "ST_FATHER_NAME_AR", "ST_MIDDLE_NAME_AR", "ST_SURNAME_AR" };
            //    if (Names_Cols.Contains(ColUID))
            //    {
            //        Form form = SBO_Application.Forms.Item(FormUID);
            //        DataTable DT_Members = form.DataSources.DataTables.Item("MEMBERS");
            //        Grid Grd_Members = (Grid)form.Items.Item("138").Specific;
            //        int Index = Grd_Members.GetDataTableRowIndex(Row);

            //        string FirstName = DT_Members.GetValue("ST_FIRST_NAME_AR", Index).ToString();
            //        string FatherName = DT_Members.GetValue("ST_FATHER_NAME_AR", Index).ToString();
            //        string MiddleName = DT_Members.GetValue("ST_MIDDLE_NAME_AR", Index).ToString();
            //        string SurName = DT_Members.GetValue("ST_SURNAME_AR", Index).ToString();
            //        string Full_Name = Utility.Get_Full_Name(FirstName, FatherName, MiddleName, SurName);

            //        DT_Members.SetValue("ST_FULL_NAME_AR", Index, Full_Name);
            //    }
            //    string[] Names_EN_Cols = new string[] { "ST_FIRST_NAME_EN", "ST_FATHER_NAME_EN", "ST_MIDDLE_NAME_EN", "ST_SURNAME_EN" };
            //    if (Names_EN_Cols.Contains(ColUID))
            //    {
            //        Form form = SBO_Application.Forms.Item(FormUID);
            //        DataTable DT_Members = form.DataSources.DataTables.Item("MEMBERS");
            //        Grid Grd_Members = (Grid)form.Items.Item("138").Specific;
            //        int Index = Grd_Members.GetDataTableRowIndex(Row);

            //        string FirstName = DT_Members.GetValue("ST_FIRST_NAME_EN", Index).ToString();
            //        string FatherName = DT_Members.GetValue("ST_FATHER_NAME_EN", Index).ToString();
            //        string MiddleName = DT_Members.GetValue("ST_MIDDLE_NAME_EN", Index).ToString();
            //        string SurName = DT_Members.GetValue("ST_SURNAME_EN", Index).ToString();
            //        string Full_Name = Utility.Get_Full_Name(FirstName, FatherName, MiddleName, SurName);

            //        DT_Members.SetValue("ST_FULL_NAME_EN", Index, Full_Name);
            //    }


            //}

            #endregion



        }

        private static bool Validate_Data(BusinessObjectInfo businessObjectInfo)
        {
            Form form = SBO_Application.Forms.Item(businessObjectInfo.FormUID);
            int checkContacts = CheckContacts(businessObjectInfo);
            if (checkContacts != -5)
            {
                throw new Custom_Exception($@"Email Address on line {checkContacts + 1} in the Personal Contacts tab is invalid");
            }

            if (!Form_Obj.Validate_Data(form))
            {
                return false;
            }

            SAPbouiCOM.DataTable DT_Departments = form.DataSources.DataTables.Item("DT_Departments");
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
            Check_Contacts(form);
            Member_Cards_UI.Check_Address(businessObjectInfo, "@ST_FUND_CORP_ADDR");

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
                if (pVal.ItemUID == "31" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Form form = SBO_Application.Forms.Item(pVal.FormUID);
                    Add_Address_Row(pVal);
                }
                if ((pVal.ItemUID == "52" || pVal.ItemUID == "53") && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Form form = SBO_Application.Forms.Item(pVal.FormUID);
                    Set_Flags_Visibility(form);
                }
                if (pVal.ItemUID == "32" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Remove_Address_Row(pVal);
                }
                if (pVal.ItemUID == "139" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Form form = SBO_Application.Forms.Item(pVal.FormUID);
                    Grid Grd_Members = (Grid)form.Items.Item("138").Specific;
                    Add_Member_Row(pVal);
                }
                if (pVal.ItemUID == "703" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Add_Contacts_Row(pVal);
                }
                if (pVal.ItemUID == "140" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Remove_Member_Row(pVal);
                }
                if (pVal.ItemUID == "502" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Add_Attachment(pVal);
                }
                if (pVal.ItemUID == "503" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Remove_Attachment(pVal);
                }
                if (pVal.ItemUID == "504" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Open_Attachment(pVal);
                }
                if (!pVal.BeforeAction && pVal.ItemChanged)
                {
                    Change_Items_Values_Cutomization(pVal.ItemUID, pVal.FormUID, pVal.BeforeAction, pVal.ItemChanged, pVal.ColUID, pVal.Row);
                }
                if (pVal.ItemUID == "163" && (pVal.EventType == BoEventTypes.et_ITEM_PRESSED || pVal.EventType == BoEventTypes.et_COMBO_SELECT) && !pVal.BeforeAction)
                {
                    Run_Actions(pVal);
                }
                if (pVal.ItemUID == "20" && pVal.ColUID == "Country" && pVal.EventType == BoEventTypes.et_CHOOSE_FROM_LIST && !pVal.BeforeAction)
                {
                    Choose_From_List_Country_Matrix(pVal);
                }
                if (pVal.ItemUID == "138" && (pVal.ColUID == "ST_NATIONALITY" || pVal.ColUID == "ST_RESIDENCY") && pVal.EventType == BoEventTypes.et_CHOOSE_FROM_LIST && !pVal.BeforeAction)
                {
                    Choose_From_List_Country_Grid(pVal);
                }
                if (pVal.ItemUID == "87" && pVal.EventType == BoEventTypes.et_CHOOSE_FROM_LIST && !pVal.BeforeAction)
                {
                    Choose_Country(pVal);
                }
                //if (pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction && Flags_List.Keys.Contains(pVal.ItemUID))
                //{
                //   // Flag_Click(pVal);
                //}

            }
            catch (Exception ex)
            {
                SBO_Application.StatusBar.SetText(ex.Message, BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Error);
            }
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
            switch (Action_ID)
            {
                case "-"://Can Also
                    throw new Logic.Custom_Exception("Please select the action");
                case "R"://Remove
                    Title = "Remove";
                    break;
                //case "L"://Link
                //    Title = "Link";
                //    break;
                //case "U"://Unlink
                //    Title = "Unlink";
                //    break;
                case "RE"://Renewal All
                    Title = "Renew All Cards";
                    break;
                case "RS"://Renewal and selected childes
                    Title = "Renew Card and the selected Sub-Members";
                    break;
                case "SA"://Stop All
                    Title = "Stop the main Card and all Sub-Members";
                    break;
                case "SS"://Renewal and selected childes
                    Title = "Stop Card and selected Sub-Members";
                    break;
                case "M":
                    Title = "Create Membership";
                    break;
                default:
                    throw new Logic.Custom_Exception($"This Functionality [{Action_ID}] is not supported");
            }
            if (SBO_Application.MessageBox($"Are you sure want to {Title}?", 1, "Yes", "No") != 1)
            {
                return;
            }
            string UDO_Code = form.DataSources.DBDataSources.Item(0).GetValue("Code", 0);
            //UDO_Definition UDO_Info = Logic.Objects_Logic.All_UDO_Definition.FirstOrDefault(U => U.KHCF_Object == KHCF_Objects.CCI_Member_Card);

            switch (Action_ID)
            {
                case "R"://Remove
                    KHCF_Logic_Utility.Remove_UDO_Entry(company, UDO_Code,Form_Obj.UDO_Info);
                    break;
                //case "RE"://Renewal All
                //    Renewal_Membership(form, true, true);
                //    break;
                //case "RS"://Renewal and selected childes
                //    Renewal_Membership(form, false, true);
                //    break;
                //case "SA"://Stop All
                //    Stop_Membership(form, UDO_Info, true, true);
                //    break;
                //case "SS"://Stop and selected childes
                //    Stop_Membership(form, UDO_Info, false, true);
                //    break;
                //case "M"://Create Membership
                //    Create_Membership(form);
                //    break;
                default:
                    throw new Logic.Custom_Exception($"This Functionality [{Action_ID}] is not supported");
            }
        }

        private static void Add_Contacts_Row(ItemEvent pVal)
        {
            Form form = SBO_Application.Forms.Item(pVal.FormUID);
            form.Freeze(true);
            DBDataSource DS_Contacts = form.DataSources.DBDataSources.Item("@ST_FUND_CORP_CONT");
            Matrix Mat_Cont = (Matrix)form.Items.Item("701").Specific;
            Mat_Cont.FlushToDataSource();
            int Count = DS_Contacts.Size;
            if (Count == 1)
            {
                if (DS_Contacts.GetValue("U_ST_NAME", Count - 1) != "")
                {
                    DS_Contacts.InsertRecord(Count);
                }
                else
                {
                    Count = 0;
                    DS_Contacts.InsertRecord(Count);
                    Mat_Cont.LoadFromDataSource();
                    Mat_Cont.DeleteRow(1);
                    Mat_Cont.FlushToDataSource();
                    Mat_Cont.LoadFromDataSource();
                }
            }
            else
            {
                DS_Contacts.InsertRecord(Count);
            }

            //DS_Contacts.SetValue("U_ST_ADDRESS_TYPE", Count, "S");
            //DS_Contacts.SetValue("U_ST_COUNTRY", Count, "JO");

            Mat_Cont.LoadFromDataSource();
            form.Freeze(false);

        }

        private static void Open_Attachment(ItemEvent pVal)
        {
            Form form = SBO_Application.Forms.Item(pVal.FormUID);

            Matrix Mat = (Matrix)form.Items.Item("500").Specific;
            for (int i = 0; i < Mat.RowCount; i++)
            {
                CheckBox Chk_Selected = (CheckBox)Mat.GetCellSpecific("SELECTED", i + 1);
                if (Chk_Selected.Checked)
                {
                    EditText Txt_FileName = (EditText)Mat.GetCellSpecific("FileName", i + 1);
                    System.Diagnostics.Process.Start(Txt_FileName.Value);
                }
            }
            // Form_Obj.Remove_Matrix_Row(form, "500");

        }

        private static void Remove_Attachment(ItemEvent pVal)
        {
            Form form = SBO_Application.Forms.Item(pVal.FormUID);
            //Matrix Mat = (Matrix)form.Items.Item("500").Specific;
            //int Count = Mat.RowCount;
            //for (int i = Count; i > 0; i--)
            //{
            //    //CheckBox Chk_Selected = (CheckBox)Mat.GetCellSpecific("SELECTED", i);
            //    CheckBox Chk_Selected = (CheckBox)Mat.Columns.Item("SELECTED").Cells.Item(i).Specific;
            //    if (Chk_Selected.Checked == true)
            //    {
            //        Mat.DeleteRow(i);
            //        // form.DataSources.DBDataSources.Item(1).RemoveRecord(Grid_Deleted_Row +1);
            //        Mat.FlushToDataSource();
            //        Mat.LoadFromDataSource();
            //    }
            //}
            Form_Obj.Remove_Matrix_Row(form, "500");

        }

        private static void Remove_Address_Row(ItemEvent pVal)
        {
            Form form = SBO_Application.Forms.Item(pVal.FormUID);
            //Matrix Mat = (Matrix)form.Items.Item("20").Specific;
            //int Count = Mat.RowCount;
            //for (int i = Count; i > 0; i--)
            //{
            //    //CheckBox Chk_Selected = (CheckBox)Mat.GetCellSpecific("SELECTED", i);
            //    CheckBox Chk_Selected = (CheckBox)Mat.Columns.Item("SELECTED").Cells.Item(i).Specific;
            //    if (Chk_Selected.Checked == true)
            //    {
            //        Mat.DeleteRow(i);
            //        // form.DataSources.DBDataSources.Item(1).RemoveRecord(Grid_Deleted_Row +1);
            //        Mat.FlushToDataSource();
            //        Mat.LoadFromDataSource();
            //    }
            //}
            Form_Obj.Remove_Matrix_Row(form, "20");

        }

        private static void Add_Attachment(ItemEvent pVal)
        {
            Form form = SBO_Application.Forms.Item(pVal.FormUID);

            ST.Helper.Dialog.BrowseFile BF = new Helper.Dialog.BrowseFile(Helper.Dialog.DialogType.OpenFile);
            // BF.Filter = "CSV Files (*.csv)|*.csv";

            BF.ShowDialog();
            if (BF.FileName == "")
            {
                return;
            }
            form.Freeze(true);
            DBDataSource DS_Attachment = form.DataSources.DBDataSources.Item("@ST_FUND_CORP_ATT");
            Matrix Mat_Add = (Matrix)form.Items.Item("500").Specific;
            Mat_Add.FlushToDataSource();
            int Count = DS_Attachment.Size;
            if (Count == 1)
            {
                if (DS_Attachment.GetValue("U_ST_FILE_NAME", Count - 1) != "")
                {
                    DS_Attachment.InsertRecord(Count);
                }
                else
                {
                    Count = 0;

                    Mat_Add.LoadFromDataSource();
                }
            }
            else
            {
                DS_Attachment.InsertRecord(Count);
            }

            DS_Attachment.SetValue("U_ST_FILE_NAME", Count, BF.FileName);
            DS_Attachment.SetValue("LineId", Count, "-1");

            //DS_Address.SetValue("U_ST_COUNTRY", Count, "JO");

            Mat_Add.LoadFromDataSource();
            Mat_Add.AutoResizeColumns();
            form.Freeze(false);

        }

        private static void Remove_Member_Row(ItemEvent pVal)
        {
            Form form = SBO_Application.Forms.Item(pVal.FormUID);
            DataTable DT_Members = form.DataSources.DataTables.Item("MEMBERS");

            for (int i = 0; i < DT_Members.Rows.Count; i++)
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
                    string BP_Sub_Card = Utility.Get_BP_Code(company, Sub_Code, UDO_Info);
                    KHCF_Logic_Utility.Unlink(company, Sub_Code, BP_Sub_Card, UDO_Info);
                    SBO_Application.StatusBar.SetText($"The Card[{Sub_Code}] has been unlinked successfully!", BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Success);
                }

                DT_Members.Rows.Remove(i);
            }
        }

        private static void Add_Member_Row(ItemEvent pVal)
        {
            Form form = SBO_Application.Forms.Item(pVal.FormUID);
            DataTable DT_Members = form.DataSources.DataTables.Item("MEMBERS");

            for (int i = 0; i < DT_Members.Rows.Count; i++)
            {
                if (string.IsNullOrEmpty(DT_Members.GetValue("ST_CUSTOMER_GROUP", i).ToString()))
                {
                    DT_Members.Rows.Remove(i);
                }
            }

            DT_Members.Rows.Add();
            DT_Members.SetValue("ST_GENDER", DT_Members.Rows.Count - 1, "M");

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
        }

        private static void Add_Address_Row(ItemEvent pVal)
        {
            Form form = SBO_Application.Forms.Item(pVal.FormUID);
            form.Freeze(true);
            DBDataSource DS_Address = form.DataSources.DBDataSources.Item("@ST_FUND_CORP_ADDR");
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

            DS_Address.SetValue("U_ST_ADDRESS_TYPE", Count, "S");
            DS_Address.SetValue("U_ST_COUNTRY", Count, "Jordan");

            Mat_Add.LoadFromDataSource();
            form.Freeze(false);

        }

        private static int CheckContacts(BusinessObjectInfo businessObjectInfo)
        {
            int result = -5;
            SAPbouiCOM.Form form = SBO_Application.Forms.Item(businessObjectInfo.FormUID);
            Matrix Mat_Contact = (Matrix)form.Items.Item("36").Specific;
            for (int i = 0; i < Mat_Contact.RowCount; i++)
            {
                SAPbouiCOM.EditText Email = (SAPbouiCOM.EditText)Mat_Contact.Columns.Item("E-Mail").Cells.Item(i + 1).Specific;
                bool isEmail = Regex.IsMatch(Email.Value, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase);
                if (!isEmail && !string.IsNullOrEmpty(Email.Value))
                {
                    result = i;
                    return result;
                }
            }
            return result;
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

        private static void DisableTabButtons(Form form)
        {
            List<string> GridBtnsIds = new List<string>() { "31","32" , "139", "140" };
            for (int i = 0; i < GridBtnsIds.Count; i++)
            {
                form.Items.Item(GridBtnsIds[i]).SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 4, BoModeVisualBehavior.mvb_False);

            }

        }

        private static void Check_Contacts(Form form)
        {
            DataTable DT_Members = form.DataSources.DataTables.Item("MEMBERS");
            int Count = DT_Members.Rows.Count;
            SAPbouiCOM.Matrix Mat_Add = (SAPbouiCOM.Matrix)form.Items.Item("20").Specific;
            if ((Count == 1 && string.IsNullOrEmpty(DT_Members.GetValue("ST_CUSTOMER_GROUP", Count - 1).ToString())))
            {
                throw new Custom_Exception($"Please add at least one address at the addresses tab.");
            }
            if (Count == 0)
            {
                throw new Custom_Exception($"Please add at least one Related Contact at the Related Contact tab.");
            }
        }

        private static void Set_Flags_Visibility(Form form)
        {

            List<string> flags = new List<string> { "52", "53" };
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
        private static void Choose_Country(ItemEvent pVal)
        {
            SAPbouiCOM.Form form = SBO_Application.Forms.Item(pVal.FormUID);
            ChooseFromListEvent Chos_Event = (ChooseFromListEvent)pVal;
            string Code = "";
            if ((Chos_Event.SelectedObjects == null ? false : Chos_Event.SelectedObjects.Rows.Count != 0))
            {
                Code = Chos_Event.SelectedObjects.GetValue("Name", 0).ToString();
                form.DataSources.DBDataSources.Item(0).SetValue("U_ST_COUNTRY", 0, Code);
            }
        }

    }
}
