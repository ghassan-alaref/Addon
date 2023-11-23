using SAPbobsCOM;
using SAPbouiCOM;
using ST.KHCF.Customization.Forms.CCI;
using ST.KHCF.Customization.Logic.Classes;
using ST.KHCF.Customization.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection.Emit;
using ST.KHCF.Customization.MetaDataOperator;
using System.IO;
using System.Drawing;

namespace ST.KHCF.Customization.Forms.Patient
{
    internal class Frm_Patients_Card : Parent_Form
    {
        internal static SAPbobsCOM.Company company;
        internal static Application SBO_Application;
        internal static Parent_Form Form_Obj;

        //internal override string[] Get_Mondatory_Fields_List()
        //{
        //    List<string> Result = new List<string>();
        //    Result.AddRange(base.Get_Mondatory_Fields_List());
        //    Result.AddRange(new string[] { "9", "11", "13", "15", "17", "21", "23", "25", "27", "29", "39", "41", "43", "51", "52", "53", "77", "57", "80", "79", "73"});
        //    return Result.ToArray();  "9,11,13,15,17,21,23,25,27,29,39,41,43,51,52,53,77,57,80,79,73"
        //}

        //internal override string[] Get_Approval_Items_List()
        //{
        //    List<string> Result = new List<string>();
        //    Result.AddRange(base.Get_Approval_Items_List());
        //   // Result.AddRange(new string[] { "159", "160", "161", "162" });

        //    return Result.ToArray();
        //}

        internal override Depends_List[] Get_Depends_List_List()
        {
            List<Depends_List> Result = new List<Depends_List>();
            Result.AddRange(base.Get_Depends_List_List());
            //Result.Add(new Depends_List() { Item_ID = "133", Parent_Item_ID = "47", SQL = $@"SELECT T0.""Code"", T0.""Name"" FROM ""@ST_SUB_CHANNEL""  T0 WHERE T0.""U_ST_CHANNEL"" = '{{0}}'" });
            //Result.Add(new Depends_List() { Item_ID = "133", Parent_Item_ID = "131", SQL = $@"SELECT T0.""Code"", T0.""Name"" FROM ""@ST_SUB_CHANNEL""  T0 WHERE T0.""U_ST_CHANNEL"" = '002'" });
            return Result.ToArray();
        }

        //internal override string[] Get_Tab_Item_List()
        //{
        //    List<string> Result = new List<string>();
        //    Result.AddRange(base.Get_Tab_Item_List());
        //    Result.Add("3");
        //    Result.Add("4");
        //    Result.Add("93");
        //    Result.Add("19");
        //    Result.Add("85");
        //    Result.Add("86");  3,4,93,19,85,86

        //    return Result.ToArray();
        //}


        //internal override string[] Get_Fix_ReadOnly_Fields_List()
        //{
        //    List<string> Result = new List<string>();
        //    Result.AddRange(base.Get_Fix_ReadOnly_Fields_List());
        //    Result.AddRange(new string[] { "5", "152", "154", "7", "17", "29" }); "5,152,154,7,17,29"

        //    return Result.ToArray();
        //}

        internal override void Initialize_Form(Form form)
        {
            //Code_value = "Frm_Patients_Card";
            //Desc_value = "Mandatary fields List For Patients Card ";
            //Man_fields =  "9,11,13,15,17,21,23,25,27,29,39,41,43,51,52,53,77,57,80,79,73";

            //File.AppendAllText("C:\\Users\\wsaleh\\Desktop\\Patient_TimeLog.txt", "Patients Card" + Environment.NewLine);
            //string startTimeExact = DateTime.Now.ToString("mm:ss:f");
            //DateTime startTime = DateTime.Now;
            //File.AppendAllText("C:\\Users\\wsaleh\\Desktop\\Patient_TimeLog.txt", "Start Time:" + startTimeExact + Environment.NewLine);
            base.Initialize_Form(form);
            //DateTime prev = Utility.Add_Time_Log("P", "base.Initialize Form", startTime);

            //Matrix Mat_Add = (Matrix)form.Items.Item("191").Specific;
            //base.Fill_Address_ComboBox(Mat_Add);
            //prev = Utility.Add_Time_Log("P", "Address", prev);
            Matrix Mat_Att = (Matrix)form.Items.Item("164").Specific;
            base.Fill_Attachment_ComboBox(Mat_Att);
            //prev = Utility.Add_Time_Log("P", "Attachments", prev);
            //            string SQL_Customer_Group = $@"SELECT ""GroupCode"" AS ""Code"", T0.""GroupName"" AS ""Name"" FROM OCRG T0 
            //WHERE  T0.""GroupType""  = 'C' AND T0.""U_ST_TYPE"" = 'I' AND U_ST_CUSTOMER_TYPE = 'C'";
            //            Helper.Utility.Fill_One_ComboBoxBySQL(company, form, "91", SQL_Customer_Group, true);

            //            string CCI_Department_ID = Configurations.Get_CCI_Department(company);
            //            string SQL_Account_Manager = $@"SELECT T0.""SlpCode"" AS ""Code"", T0.""SlpName"" AS ""Name"" 
            //FROM OSLP T0  INNER JOIN OHEM T1 ON T0.""SlpCode"" = T1.""salesPrson"" 
            //WHERE T1.""dept"" = {CCI_Department_ID}";
            //            Helper.Utility.Fill_One_ComboBoxBySQL(company, form, "137", SQL_Account_Manager, true);

            //            string Broker_Vendor_Group = Configurations.Get_Broker_Vendor_Group(company);
            //            string SQL_Broker = $@"SELECT T0.""CardCode"" AS ""Code"", T0.""CardName"" AS ""Name"" 
            //FROM OCRD T0 WHERE T0.""GroupCode"" = {Broker_Vendor_Group}";
            //            Helper.Utility.Fill_One_ComboBoxBySQL(company, form, "131", SQL_Broker, true);
            //            Helper.Utility.Fill_One_ComboBoxBySQL(company, form, "135", SQL_Broker, true);

            Grid Grd_Coverage_Request = (Grid)form.Items.Item("173").Specific;
            ((ComboBoxColumn)Grd_Coverage_Request.Columns.Item("U_ST_STATUS")).ValidValues.Add("O", "Open");
            ((ComboBoxColumn)Grd_Coverage_Request.Columns.Item("U_ST_STATUS")).ValidValues.Add("C", "Close");
            ((ComboBoxColumn)Grd_Coverage_Request.Columns.Item("U_ST_STATUS")).ValidValues.Add("S", "Stop");
            ((ComboBoxColumn)Grd_Coverage_Request.Columns.Item("U_ST_STATUS")).ValidValues.Add("L", "Cancel");
            ((ComboBoxColumn)Grd_Coverage_Request.Columns.Item("U_ST_STATUS")).DisplayType = BoComboDisplayType.cdt_Description;

            ((ComboBoxColumn)Grd_Coverage_Request.Columns.Item("U_ST_PATIENT_TYPE")).ValidValues.Add("C", "CCI");
            ((ComboBoxColumn)Grd_Coverage_Request.Columns.Item("U_ST_PATIENT_TYPE")).ValidValues.Add("G", "Goodwill");
            ((ComboBoxColumn)Grd_Coverage_Request.Columns.Item("U_ST_PATIENT_TYPE")).ValidValues.Add("O", "Other CCI Companies");
            ((ComboBoxColumn)Grd_Coverage_Request.Columns.Item("U_ST_PATIENT_TYPE")).DisplayType = BoComboDisplayType.cdt_Description;


            //prev = Utility.Add_Time_Log("P", "Combo box Column", prev);
            //Grd_Coverage_Request.Columns.Item("SELECTED").AffectsFormMode = false;

            //            Grid Grd_Membership = (Grid)form.Items.Item("154").Specific;
            //            string SQL_Cov = $@"SELECT ""Code"" , ""Name"" FROM ""@ST_COVERAGE""";
            //            Helper.Utility.FillGridComboBoxForSQL(company, Grd_Membership, "U_ST_COVERAGE", SQL_Cov);
            //            string SQL_Memeber_Customer_Group = $@"SELECT ""GroupCode"" AS ""Code"", T0.""GroupName"" AS ""Name"" FROM OCRG T0 
            //WHERE  T0.""GroupType""  = 'C' AND T0.""U_ST_TYPE"" = 'I'";
            //            Helper.Utility.FillGridComboBoxForSQL(company, Grd_Members, "ST_CUSTOMER_GROUP", SQL_Memeber_Customer_Group, true);

            //string SQL_Nationality = $@"SELECT T0.""Code"", T0.""Name"" FROM OCRY T0";
            //            Helper.Utility.FillGridComboBoxForSQL(company, Grd_Members, "ST_NATIONALITY", SQL_Nationality, true);
            //Helper.Utility.Fill_One_ComboBoxBySQL(company, form, "52", SQL_Nationality, true);
            //prev = Utility.Add_Time_Log("P", "Nationality", prev);
            ButtonCombo Btn_Cmb_Warrning = (ButtonCombo)form.Items.Item("180").Specific;
            Btn_Cmb_Warrning.ValidValues.Add("-", "You Can Also");
            Btn_Cmb_Warrning.ValidValues.Add("L", "Legal Report");
            Btn_Cmb_Warrning.ValidValues.Add("N", "National Cancer Registry Report");
            Btn_Cmb_Warrning.ValidValues.Add("R", "Remove");
            form.DataSources.UserDataSources.Item("180").Value = "-";
            form.Items.Item("180").AffectsFormMode = false;

            //prev = Utility.Add_Time_Log("P", "Combo Button", prev);
            ComboBox Cmb_CCI_Patient =(ComboBox) form.Items.Item("100").Specific;
            int CCI_Patient_Vendor_Group = Configurations.Get_CCI_Patient_Vendor_Group(company);
            Cmb_CCI_Patient.ValidValues.Add("-", "");
            Cmb_CCI_Patient.ValidValues.Add(CCI_Patient_Vendor_Group.ToString(), Utility.Get_BP_Group_Name(company, CCI_Patient_Vendor_Group));
            ComboBox Cmb_Other_CCI_Patient =(ComboBox) form.Items.Item("129").Specific;
            int CCI_Other_Patient_Vendor_Group = Configurations.Get_Other_CCI_Patient_Vendor_Group(company);
            Cmb_Other_CCI_Patient.ValidValues.Add("-", "");
            Cmb_Other_CCI_Patient.ValidValues.Add(CCI_Other_Patient_Vendor_Group.ToString(), Utility.Get_BP_Group_Name(company, CCI_Other_Patient_Vendor_Group));
            int CCI_Other_Companies_Vendor_Group = Configurations.Get_Other_CCI_Companies_Vendor_Group(company);
            string SQL_Other_CCI_Covered_By = $@"SELECT T0.""CardCode"" AS ""Code"", T0.""CardName"" AS ""Name"" 
            FROM OCRD T0 WHERE T0.""GroupCode"" = {CCI_Other_Companies_Vendor_Group}";
            Helper.Utility.Fill_One_ComboBoxBySQL(company, form, "131", SQL_Other_CCI_Covered_By, true);

            ComboBox Cmb_Goodwill = (ComboBox) form.Items.Item("91").Specific;
            int CCI_Goodwill = Configurations.Get_Other_Goodwill_Vendor_Group(company);
            Cmb_Goodwill.ValidValues.Add("-", "");
            Cmb_Goodwill.ValidValues.Add(CCI_Goodwill.ToString(), Utility.Get_BP_Group_Name(company, CCI_Goodwill));
            //prev = Utility.Add_Time_Log("P", "Valid Values", prev);
            form.Items.Item("3").Click();
            //prev = Utility.Add_Time_Log("P", "", startTime, true);
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


                Form form = Loader.SBO_Application.Forms.Item(BusinessObjectInfo.FormUID);
                //string TableName = form.DataSources.DBDataSources.Item(0).TableName.Replace("@", "");
                //UDO_Definition KHCF_Object = Objects_Logic.All_UDO_Definition.FirstOrDefault(O => O.Table_Name == TableName);
                //SBO_Application.StatusBar.SetText("Loading", BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_None);
                if (Form_Obj.Set_ReadOnly(form, Form_Obj.UDO_Info))
                {
                    // return;
                }

            }
            catch (Exception ex)
            {
                //SBO_Application.                SBO_Application.StatusBar.SetText(ex.Message, BoMessageTime.bmt_Short,  BoStatusBarMessageType.smt_Error);(ex.Message, BoMessageTime.bmt_Short, true);
                Loader.New_Msg = ex.Message;
                if (!BusinessObjectInfo.BeforeAction)
                {
                    Form form = Loader.SBO_Application.Forms.Item(BusinessObjectInfo.FormUID);
                    form.Mode = BoFormMode.fm_UPDATE_MODE;
                }
                BubbleEvent = false;
            }

            return BubbleEvent;
        }

        private static void Form_Data_Load(BusinessObjectInfo businessObjectInfo)
        {
            Form form = SBO_Application.Forms.Item(businessObjectInfo.FormUID);
            string Card_ID = form.DataSources.DBDataSources.Item(0).GetValue("Code", 0);

            Form_Obj.Load_Depends_Items(form);

            Load_Coverage_Request(form);

            //Load_Sub_Members(form, Card_ID);

            //Load_Memberships(form, Card_ID);
            //Load_Communication_Log(form, Card_ID);


            //Set_Parent_Link(form);
            //form.Items.Item("164").Visible = false;

            //  Check_Approval_Editable(form, Card_ID);

            Change_Items_Values_Cutomization("77", businessObjectInfo.FormUID, false, true, "", 0, true);
            Change_Items_Values_Cutomization("49", businessObjectInfo.FormUID, false, true, "", 0, true);
            Change_Items_Values_Cutomization("91", businessObjectInfo.FormUID, false, true, "", 0, true);
            Change_Items_Values_Cutomization("129", businessObjectInfo.FormUID, false, true, "", 0, true);
            Change_Items_Values_Cutomization("119", businessObjectInfo.FormUID, false, true, "", 0, true);

        }

        private static void Load_Coverage_Request(Form form)
        {
            string Patient_Code = form.DataSources.DBDataSources.Item(0).GetValue("Code", 0);

            string SQL = $@"SELECT T0.""Code"", T0.U_ST_REQUEST_DATE, T0.U_ST_STATUS, T1.""Name"" AS ""SUPPORT_TYPE_NAME"", T0.U_ST_SUPPORT_AMOUNT, T0.U_ST_PATIENT_TYPE
FROM ""@ST_COVERAGE_REQUEST""  T0 INNER JOIN ""@ST_SUPPORT_TYPE"" T1 ON T0.U_ST_SUPPORT_TYPE = T1.""Code"" 
WHERE T0.""U_ST_PATIENT_CARD"" = '{Patient_Code}'";
            Recordset RC = Helper.Utility.Execute_Recordset_Query(company, SQL);
            DataTable DT_Coverage_Request = form.DataSources.DataTables.Item("DT_Coverage_Requests");
            DT_Coverage_Request.Rows.Clear();
            DT_Coverage_Request.Rows.Add(RC.RecordCount);

            for (int i = 0; i < RC.RecordCount; i++)
            {
                for (int J = 0; J < DT_Coverage_Request.Columns.Count; J++)
                {
                    string Col_Name = DT_Coverage_Request.Columns.Item(J).Name;

                    DT_Coverage_Request.SetValue(Col_Name, i, RC.Fields.Item(Col_Name).Value);
                }

                RC.MoveNext();
            }

            ((Grid)form.Items.Item("173").Specific).AutoResizeColumns();
        }

        private static void ADD_Update_UDO(BusinessObjectInfo businessObjectInfo)
        {
            Form form = SBO_Application.Forms.Item(businessObjectInfo.FormUID);
            System.Xml.XmlDocument XML_Doc = new System.Xml.XmlDocument();
            string XML_Text = businessObjectInfo.ObjectKey.Replace(" ", "").Replace("=", " = ");
            XML_Doc.LoadXml(XML_Text);

            string UDO_Code = XML_Doc.GetElementsByTagName("Code")[0].InnerText;
            string MemberCard_Code = form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).GetValue("U_ST_MEMBER_CARD", 0);
            if (MemberCard_Code != "")
            {
                string SQL_BP = $@"SELECT T1.""validFor"" , T0.U_ST_BP_CODE FROM ""@ST_CCI_INDIV_CARD""  T0 INNER JOIN OCRD T1 ON T0.U_ST_BP_CODE = ""CardCode"" 
WHERE T0.""Code"" = '{MemberCard_Code}'";
                Recordset RC_BP = Helper.Utility.Execute_Recordset_Query(company, SQL_BP);
                if (RC_BP.RecordCount != 0)
                {
                    if (RC_BP.Fields.Item("validFor").Value.ToString() == "Y")
                    {
                        BusinessPartners BP = (BusinessPartners)company.GetBusinessObject(BoObjectTypes.oBusinessPartners);
                        string BP_CardCode = RC_BP.Fields.Item("U_ST_BP_CODE").Value.ToString();
                        BP.GetByKey(BP_CardCode);

                        BP.Valid = BoYesNoEnum.tNO;
                        BP.Frozen = BoYesNoEnum.tYES;

                        if (BP.Update() != 0)
                        {
                            throw new Logic.Custom_Exception($"Error during Update the BP[{BP_CardCode}],[{company.GetLastErrorDescription()}]");
                        }
                    }
                }

                //UDO_Definition Membership_Info = Objects_Logic.All_UDO_Definition.FirstOrDefault(U => U.KHCF_Object == KHCF_Objects.Individual_Membership);
                //string Membership_Code = Utility.Get_Last_Individual_Membership_Per_Card(company, MemberCard_Code, Membership_Info);
                //Membership.Stop_MemberCard(company, Membership_Code, Membership_Info, DateTime.Today, $"Convert to Patient[{UDO_Code}]", false);
            }

            Create_BPs(form, UDO_Code);

        }

        private static void Create_BPs(Form form, string UDO_Code)
        {
            //string BP_Code = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_BP_CODE", 0);
            string BP_CCI_Code = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_CCI_BP_CODE", 0);
            string BP_Other_CCI_Code = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_OTHER_CCI_BP_CODE", 0);
            string BP_Woodwill_Code = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_GOODWILL_BP_CODE", 0);
            if (BP_CCI_Code != "" && BP_Other_CCI_Code != "" && BP_Woodwill_Code !="")
            {
                return;
            }

            KHCF_BP BP_Data = new KHCF_BP();
            string CCI_Patient_Vendor_Group_Text = form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).GetValue("U_ST_PATIENT_VENDOR_GROUP_MSH", 0);
            string CCI_Other_Patient_Vendor_Group_Text = form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).GetValue("U_ST_PATIENT_VENDOR_GROUP_CCI", 0);
            string Goodwill_Text = form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).GetValue("U_ST_PATIENT_VENDOR_GROUP_GW", 0);
            int CCI_Patient_Vendor_Group, CCI_Other_Patient_Vendor_Group, Goodwil_Group;
            int.TryParse(CCI_Patient_Vendor_Group_Text, out CCI_Patient_Vendor_Group);
            int.TryParse(CCI_Other_Patient_Vendor_Group_Text, out CCI_Other_Patient_Vendor_Group);
            int.TryParse(Goodwill_Text, out Goodwil_Group);

            if (!((CCI_Patient_Vendor_Group !=0 && BP_CCI_Code == "") || (CCI_Other_Patient_Vendor_Group != 0 && BP_Other_CCI_Code == "") || (Goodwil_Group !=0 && BP_Woodwill_Code =="")))
            {
                return;
            }

            BP_Data.CardName = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_FULL_ARABIC_NAME", 0);
            BP_Data.MemberCard_Code = UDO_Code;
            BP_Data.Is_Vendor = true;

            string SQL_Addr = $@"SELECT T0.""U_ST_ADDRESS_NAME"", T0.""U_ST_STREET"", T0.""U_ST_BLOCK"", T0.""U_ST_ZIP_CODE"", T0.""U_ST_CITY""
, T0.""U_ST_COUNTY"",  T0.""U_ST_STATE"", T0.""U_ST_BUILDING"", T0.""U_ST_ADDRESS_TYPE"", T0.""U_ST_ADDRESS_NAME_2""
, T0.""U_ST_ADDRESS_NAME_3"", T0.""U_ST_STREET_NO""
,(Select T1.""Code"" From OCRY T1 Where T1.""Name"" =U_ST_COUNTRY)As U_ST_COUNTRY 
FROM ""@ST_PATIENTS_ADDRESS"" T0 WHERE ""Code"" = '{UDO_Code}' ";
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
                    address.ZipCode = RC_Addr.Fields.Item("U_ST_ZIP_CODE").Value.ToString();
                    address.County = RC_Addr.Fields.Item("U_ST_COUNTY").Value.ToString();
                    address.State = RC_Addr.Fields.Item("U_ST_STATE").Value.ToString();
                    address.BuildingFloorRoom = RC_Addr.Fields.Item("U_ST_BUILDING").Value.ToString();
                    address.AddressName2 = RC_Addr.Fields.Item("U_ST_ADDRESS_NAME_2").Value.ToString();
                    address.AddressName3 = RC_Addr.Fields.Item("U_ST_ADDRESS_NAME_3").Value.ToString();
                    address.StreetNo = RC_Addr.Fields.Item("U_ST_STREET_NO").Value.ToString();

                    BP_Data.addresses.Add(address);

                    RC_Addr.MoveNext();
                }
            }




            try
            {
                //if (!((CCI_Patient_Vendor_Group != 0 && BP_CCI_Code == "") || (CCI_Other_Patient_Vendor_Group != 0 && BP_Other_CCI_Code == "") || (Goodwil_Group != 0 && BP_Woodwill_Code == "")))
                company.StartTransaction();
       
                if (CCI_Patient_Vendor_Group != 0 && BP_CCI_Code == "")
                {
                    BP_Data.BP_Group = CCI_Patient_Vendor_Group;
                    BP_Data.Control_Account = KHCF_Logic_Utility.Get_Control_Account_Per_BPGroup(company, BP_Data.BP_Group);
                    string BP_Code = Utility.Create_BP(company, BP_Data);
                    Field_Data Fld = new Field_Data() { Field_Name = "U_ST_CCI_BP_CODE", Value = BP_Code };
                    Utility.Update_UDO(company, Form_Obj.UDO_Info, UDO_Code, new Field_Data[] { Fld });
                    Utility.Update_BP(company, BP_Code, UDO_Code, Form_Obj.UDO_Info);
                    SBO_Application.StatusBar.SetText($"A new BP[{BP_Code}] has been created for CCI Patient.", BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Success);
                }
                if (CCI_Other_Patient_Vendor_Group != 0 && BP_Other_CCI_Code == "")
                {
                    BP_Data.BP_Group = CCI_Other_Patient_Vendor_Group;
                    BP_Data.Control_Account = KHCF_Logic_Utility.Get_Control_Account_Per_BPGroup(company, BP_Data.BP_Group);
                    string BP_Code = Utility.Create_BP(company, BP_Data);
                    Field_Data Fld = new Field_Data() { Field_Name = "U_ST_OTHER_CCI_BP_CODE", Value = BP_Code };
                    Utility.Update_UDO(company, Form_Obj.UDO_Info, UDO_Code, new Field_Data[] { Fld });
                    Utility.Update_BP(company, BP_Code, UDO_Code, Form_Obj.UDO_Info);
                    SBO_Application.StatusBar.SetText($"A new BP[{BP_Code}] has been created for Other CCI Patient.", BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Success);
                }
                if (Goodwil_Group != 0 && BP_Woodwill_Code == "")
                {
                    BP_Data.BP_Group = Goodwil_Group;
                    BP_Data.Control_Account = KHCF_Logic_Utility.Get_Control_Account_Per_BPGroup(company, BP_Data.BP_Group);
                    string BP_Code = Utility.Create_BP(company, BP_Data);
                    Field_Data Fld = new Field_Data() { Field_Name = "U_ST_GOODWILL_BP_CODE", Value = BP_Code };
                    Utility.Update_UDO(company, Form_Obj.UDO_Info, UDO_Code, new Field_Data[] { Fld });
                    Utility.Update_BP(company, BP_Code, UDO_Code, Form_Obj.UDO_Info);
                    SBO_Application.StatusBar.SetText($"A new BP[{BP_Code}] has been created for Goodwill.", BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Success);
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
                { }
                if (form.Mode == BoFormMode.fm_OK_MODE)
                {
                    form.Mode = BoFormMode.fm_UPDATE_MODE;
                }
                throw new Logic.Custom_Exception($@"Error during create the BPs[{ex.Message}]");
            }


            //if (form.Mode == BoFormMode.fm_UPDATE_MODE)
            //{
            // UDO_Definition UDO_Info = Logic.Objects_Logic.All_UDO_Definition.FirstOrDefault(U => U.KHCF_Object == KHCF_Objects.CCI_Member_Card);
            //}
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
                //int CCI_Patient_Vendor_Group = Configurations.Get_CCI_Patient_Vendor_Group(company);
                //form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).SetValue("U_ST_PATIENT_VENDOR_GROUP_MSH", 0, CCI_Patient_Vendor_Group.ToString());
                //int CCI_Other_Patient_Vendor_Group = Configurations.Get_Other_CCI_Patient_Vendor_Group(company);
                //form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).SetValue("U_ST_PATIENT_VENDOR_GROUP_CCI", 0, CCI_Other_Patient_Vendor_Group.ToString());
                //int CCI_Goodwill = Configurations.Get_Other_Goodwill_Vendor_Group(company);
                //form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).SetValue("U_ST_PATIENT_VENDOR_GROUP_GW", 0, CCI_Goodwill.ToString());

                //string TableName = form.DataSources.DBDataSources.Item(0).TableName.Replace("@", "");
                //UDO_Definition KHCF_Object = Objects_Logic.All_UDO_Definition.FirstOrDefault(O => O.Table_Name == TableName);
                if (Form_Obj.Set_ReadOnly(form, Form_Obj.UDO_Info))
                {
                    // return;
                }

            }
            if (SBO_Application.Forms.ActiveForm.Mode != BoFormMode.fm_FIND_MODE)
            {
                Form_Obj.Set_Fields(SBO_Application.Forms.ActiveForm);
            }
        }

        private static void Before_Adding_UDO(BusinessObjectInfo businessObjectInfo)
        {
            Form form = SBO_Application.Forms.Item(businessObjectInfo.FormUID);
            if (form.Mode == BoFormMode.fm_ADD_MODE)
            {
                KHCF_BP BP = new KHCF_BP();
                //BP.BP_Group = int.Parse(form.DataSources.DBDataSources.Item(0).GetValue("U_ST_CUSTOMER_GROUP", 0));
                //BP.CardName = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_FULL_NAME_AR", 0);
                // string BP_Code = Utility.Create_BP(company, BP);

                Set_Default_Value_Before_Adding(form);
            }


        }

        private static void Set_Default_Value_Before_Adding(Form form)
        {
            // form.DataSources.DBDataSources.Item(0).SetValue("U_ST_BP_CODE", 0, BP_Code);
            if (form.Mode == BoFormMode.fm_ADD_MODE)
            {
                string New_UDO_Code = Utility.Get_New_UDO_Code(company, Form_Obj.KHCF_Object);
                form.DataSources.DBDataSources.Item(0).SetValue("Code", 0, New_UDO_Code);
                form.DataSources.DBDataSources.Item(0).SetValue("U_ST_CREATOR", 0, company.UserName);
               // form.DataSources.DBDataSources.Item(0).SetValue("U_ST_PARENT_TYPE", 0, "N");
            }

        }

        private static void Change_Items_Values_Cutomization(string ItemUID, string FormUID, bool BeforeAction, bool ItemChanged, string ColUID, int Row, bool OnLoad = false)
        {
            #region Names Fields

            string[] Arabic_Names_Items = new string[] { "9", "11", "13", "15" };
            if (Arabic_Names_Items.Contains(ItemUID))
            {
                Form form = SBO_Application.Forms.Item(FormUID);
                string FirstName = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_FIRST_NAME_AR", 0);
                string FatherName = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_FATHER_NAME_AR", 0);
                string MiddleName = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_MIDDLE_NAME_AR", 0);
                string SurName = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_SURNAME_AR", 0);
                string Full_Name = Utility.Get_Full_Name(FirstName, FatherName, MiddleName, SurName);

                form.DataSources.DBDataSources.Item(0).SetValue("U_ST_FULL_ARABIC_NAME", 0, Full_Name);
            }

            string[] English_Names_Items = new string[] { "21", "23", "25", "27" };
            if (English_Names_Items.Contains(ItemUID))
            {
                Form form = SBO_Application.Forms.Item(FormUID);
                string FirstName = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_FIRST_NAME_EN", 0);
                string FatherName = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_FATHER_NAME_EN", 0);
                string MiddleName = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_MIDDLE_NAME_EN", 0);
                string SurName = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_SURNAME_EN", 0);
                string Full_Name = Utility.Get_Full_Name(FirstName, FatherName, MiddleName, SurName);

                form.DataSources.DBDataSources.Item(0).SetValue("U_ST_FULL_ENGLISH_NAME", 0, Full_Name);
            }



            #endregion

            if (Form_Obj.Get_Depends_Parent_Item_IDs_List().Contains(ItemUID))
            {
                Form form = SBO_Application.Forms.Item(FormUID);
                Form_Obj.Load_One_Depends_Parent_Item(form, ItemUID);
            }

            if (ItemUID == "43" || ItemUID == "53")
            {
                Form form = SBO_Application.Forms.Item(FormUID);
                DateTime Birth_Date, Diagnosis_Date;
                if (DateTime.TryParseExact(form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).GetValue("U_ST_DATE_OF_BIRTH", 0), "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out Birth_Date) == false)
                {
                    return;
                }
                if (DateTime.TryParseExact(form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).GetValue("U_ST_DIAGNOSIS_DATE", 0), "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out Diagnosis_Date) == false)
                {
                    return;
                }

                int Age = Diagnosis_Date.Year - Birth_Date.Year;
                form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).SetValue("U_ST_AGE_AT_DIAGNOSIS", 0, Age.ToString());
            }

            if (ItemUID == "77")
            {
                Form form = SBO_Application.Forms.Item(FormUID);
                string Diag_Status = form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).GetValue("U_ST_DIAGNOSIS_STATUS", 0);
                if (Diag_Status == Configurations.Get_Diagnosed_Cancer_Patient_Code(company))
                {
                    form.Items.Item("53").BackColor = Color.Pink.R | (Color.Pink.G << 8) | (Color.Pink.B << 16); 
                    form.Items.Item("57").BackColor = Color.Pink.R | (Color.Pink.G << 8) | (Color.Pink.B << 16); 
                    form.Items.Item("80").BackColor = Color.Pink.R | (Color.Pink.G << 8) | (Color.Pink.B << 16);
                }
                else
                {
                    form.Items.Item("53").BackColor = Color.FromKnownColor(KnownColor.White).ToArgb();
                    form.Items.Item("57").BackColor = Color.FromKnownColor(KnownColor.White).ToArgb();
                    form.Items.Item("80").BackColor = Color.FromKnownColor(KnownColor.White).ToArgb();
                }
            }
            if (ItemUID == "49")
            {
                Form form = SBO_Application.Forms.Item(FormUID);
                string Previous_Diagnosis_Date = form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).GetValue("U_ST_PREVIOUS_DIAGNOSIS_DATE", 0);
                if (Previous_Diagnosis_Date != "")
                {
                    form.Items.Item("55").BackColor = Color.Pink.R | (Color.Pink.G << 8) | (Color.Pink.B << 16); 
                    form.Items.Item("45").BackColor = Color.Pink.R | (Color.Pink.G << 8) | (Color.Pink.B << 16); 
                }
                else
                {
                    form.Items.Item("55").BackColor = Color.FromKnownColor(KnownColor.White).ToArgb();
                    form.Items.Item("45").BackColor = Color.FromKnownColor(KnownColor.White).ToArgb();
                }
            }
            if (ItemUID == "91")
            {
                Form form = SBO_Application.Forms.Item(FormUID);
                string Goodwill_Group = form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).GetValue("U_ST_PATIENT_VENDOR_GROUP_GW", 0);
                if (Goodwill_Group == "" || Goodwill_Group == "-")
                {
                    form.Items.Item("117").BackColor = Color.FromKnownColor(KnownColor.White).ToArgb();
                }
                else
                {
                    form.Items.Item("117").BackColor = Color.Pink.R | (Color.Pink.G << 8) | (Color.Pink.B << 16);
                    if (OnLoad == false)
                    {
                        form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).SetValue("U_ST_SUB_NATIONALITY", 0, form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).GetValue("U_ST_NATIONALITY", 0));
                    }
                }
            }
            if (ItemUID == "129")
            {
                Form form = SBO_Application.Forms.Item(FormUID);
                string U_ST_PATIENT_VENDOR_GROUP_CCI = form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).GetValue("U_ST_PATIENT_VENDOR_GROUP_CCI", 0);
                if (U_ST_PATIENT_VENDOR_GROUP_CCI == "" || U_ST_PATIENT_VENDOR_GROUP_CCI == "-")
                {
                    form.Items.Item("131").BackColor = Color.FromKnownColor(KnownColor.White).ToArgb();
                    form.Items.Item("134").BackColor = Color.FromKnownColor(KnownColor.White).ToArgb();
                }
                else
                {
                    form.Items.Item("131").BackColor = Color.Pink.R | (Color.Pink.G << 8) | (Color.Pink.B << 16); 
                    form.Items.Item("134").BackColor = Color.Pink.R | (Color.Pink.G << 8) | (Color.Pink.B << 16); 
                }
            }
            if (ItemUID == "119")
            {
                Form form = SBO_Application.Forms.Item(FormUID);
                string U_ST_REFUGEE = form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).GetValue("U_ST_REFUGEE", 0);
                if (U_ST_REFUGEE != "Y")
                {
                    form.Items.Item("126").BackColor = Color.FromKnownColor(KnownColor.White).ToArgb();
                }
                else
                {
                    form.Items.Item("126").BackColor = Color.Pink.R | (Color.Pink.G << 8) | (Color.Pink.B << 16); 
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

        private static bool Validate_Data(BusinessObjectInfo businessObjectInfo)
        {
            Form form = SBO_Application.Forms.Item(businessObjectInfo.FormUID);

            if (!Form_Obj.Validate_Data(form))
            {
                return false;
            }

            string New_Status = form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).GetValue("U_ST_ALLOW_CCI_MEMBERSHIP", 0);
            if (New_Status == "Y")
            {
                string Code = form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).GetValue("Code", 0);
                string Old_Status = "";
                if (Code != "")
                {
                    string SQL_Old_Status = $@"SELECT T0.U_ST_ALLOW_CCI_MEMBERSHIP FROM ""{Form_Obj.UDO_Database_Table_Name}""  T0 WHERE T0.""Code"" = '{Code}'";
                    Recordset RC_Old_Status = Helper.Utility.Execute_Recordset_Query(company, SQL_Old_Status);
                    Old_Status = RC_Old_Status.Fields.Item("U_ST_ALLOW_CCI_MEMBERSHIP").Value.ToString();
                }
                if (Old_Status != New_Status)
                {
                    string SQL_Auth = $@"SELECT T0.U_ST_CAN_PATIENT_ALLOW_CCI_MEMBERSHIP
FROM OUSR T0 WHERE T0.""USER_CODE"" = '{company.UserName}'";
                    Recordset RC_Auth = Helper.Utility.Execute_Recordset_Query(company, SQL_Auth);
                    string User_Can = RC_Auth.Fields.Item("U_ST_CAN_PATIENT_ALLOW_CCI_MEMBERSHIP").Value.ToString();
                    if (New_Status == "Y" && User_Can != "Y")
                    {
                        throw new Logic.Custom_Exception("The User don't have Authorization to change Allow CCI Membership");
                    }
                }

            }


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
                if (pVal.ItemUID == "32" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Remove_Address_Row(pVal);
                }
                if (pVal.ItemUID == "97" && pVal.EventType == BoEventTypes.et_CHOOSE_FROM_LIST && !pVal.BeforeAction)
                {
                    Choose_From_List_MemberCard(pVal);
                }
                if (pVal.ItemUID == "52" && pVal.EventType == BoEventTypes.et_CHOOSE_FROM_LIST && !pVal.BeforeAction)
                {
                    Choos_From_List_Nationality(pVal);
                }
                if (pVal.ItemUID == "117" && pVal.EventType == BoEventTypes.et_CHOOSE_FROM_LIST && !pVal.BeforeAction)
                {
                    Choos_From_List_Sub_Nationality(pVal);
                }
                if (!pVal.BeforeAction && pVal.ItemChanged == true)

                if (!pVal.BeforeAction && pVal.ItemChanged)
                {
                    Change_Items_Values_Cutomization(pVal.ItemUID, pVal.FormUID, pVal.BeforeAction, pVal.ItemChanged, pVal.ColUID, pVal.Row);
                }
                if (pVal.ItemUID == "161" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Add_Attachment(pVal);
                }
                if (pVal.ItemUID == "162" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Remove_Attachment(pVal);
                }
                if (pVal.ItemUID == "163" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Open_Attachment(pVal);
                }
                if (pVal.ItemUID == "171" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Create_Social_Study(pVal);
                }
                if (pVal.ItemUID == "180" && (pVal.EventType == BoEventTypes.et_ITEM_PRESSED || pVal.EventType == BoEventTypes.et_COMBO_SELECT) && !pVal.BeforeAction)
                {
                    Run_Report(pVal);
                }
                if (pVal.ItemUID == "191" && pVal.ColUID == "Country" && pVal.EventType == BoEventTypes.et_CHOOSE_FROM_LIST && !pVal.BeforeAction)
                {
                    Choose_From_List_Country_Matrix(pVal);
                }
                if (pVal.ItemUID == "158" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Create_Coverage_Request(pVal, "C");
                }
                if (pVal.ItemUID == "159" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Create_Coverage_Request(pVal, "G");
                }
                if (pVal.ItemUID == "157" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Create_Coverage_Request(pVal, "O");
                }

            }
            catch (Exception ex)
            {
                Form form = SBO_Application.Forms.Item(pVal.FormUID);
                form.Freeze(false);
                SBO_Application.StatusBar.SetText(ex.Message, BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Error);
            }
        }

        private static void Choos_From_List_Nationality(ItemEvent pVal)
        {
            SAPbouiCOM.Form form = SBO_Application.Forms.Item(pVal.FormUID);
            DateTime startTime = DateTime.Now;

            ChooseFromListEvent Chos_Event = (ChooseFromListEvent)pVal;
            if ((Chos_Event.SelectedObjects == null ? false : Chos_Event.SelectedObjects.Rows.Count != 0))
            {
                string Name = Chos_Event.SelectedObjects.GetValue("Name", 0).ToString();


                form.DataSources.DBDataSources.Item(0).SetValue("U_ST_NATIONALITY", 0, Name);
                if (Name == "Jordan")
                {
                    form.Items.Item("11").BackColor = Color.Pink.R | (Color.Pink.G << 8) | (Color.Pink.B << 16); // Arabic Father Name
                    form.Items.Item("13").BackColor = Color.Pink.R | (Color.Pink.G << 8) | (Color.Pink.B << 16); // Arabic Middle Name
                    form.Items.Item("33").BackColor = Color.Pink.R | (Color.Pink.G << 8) | (Color.Pink.B << 16); // National ID
                }
                else
                {
                    form.Items.Item("11").BackColor = Color.FromKnownColor(KnownColor.White).ToArgb();
                    form.Items.Item("13").BackColor = Color.FromKnownColor(KnownColor.White).ToArgb();
                    form.Items.Item("33").BackColor = Color.FromKnownColor(KnownColor.White).ToArgb();
                }

                if (form.Mode == BoFormMode.fm_OK_MODE)
                {
                    form.Mode = BoFormMode.fm_UPDATE_MODE;
                }
            }

        }
        private static void Choos_From_List_Sub_Nationality(ItemEvent pVal)
        {
            SAPbouiCOM.Form form = SBO_Application.Forms.Item(pVal.FormUID);

            ChooseFromListEvent Chos_Event = (ChooseFromListEvent)pVal;
            if ((Chos_Event.SelectedObjects == null ? false : Chos_Event.SelectedObjects.Rows.Count != 0))
            {
                string Name = Chos_Event.SelectedObjects.GetValue("Name", 0).ToString();
                form.DataSources.DBDataSources.Item(0).SetValue("U_ST_SUB_NATIONALITY", 0, Name);
                if (form.Mode == BoFormMode.fm_OK_MODE)
                {
                    form.Mode = BoFormMode.fm_UPDATE_MODE;
                }
            }

        }

        private static void Run_Report(ItemEvent pVal)
        {
            Form form = SBO_Application.Forms.Item(pVal.FormUID);
            if (form.Mode != BoFormMode.fm_OK_MODE)
            {
                throw new Logic.Custom_Exception("We can run the Report if the form in OK Mode only");
            }
           // ButtonCombo Btn_Cmb_Report = (ButtonCombo)form.Items.Item("180").Specific;
           // string Title = "";
            string Action_ID = form.DataSources.UserDataSources.Item("180").Value;
            string Rpt_File= "";
            string Action_Title = "";
            switch (Action_ID)
            {
                case "-"://Reports
                    throw new Logic.Custom_Exception("Please select the Action");
                case "L"://Legal
                    Rpt_File =Configurations.Get_Patient_Legal_Report_RPT_File_Path(company);
                    break;
                case "N"://National_Cancer_Registry
                    Rpt_File = Configurations.Get_Patient_National_Cancer_Registry_Report_RPT_File_Path(company);
                    break;
                case "R":
                     Action_Title = "Remove";
                    break;
                //case "U"://Unlink
                //    Title = "Unlink";
                //    break;
                default:
                    throw new Logic.Custom_Exception($"This Action [{Action_ID}] is not supported");
            }
            string Patient_Code = form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).GetValue("Code", 0);
            if (Rpt_File != "")
            {
                List<Helper.Utility.Crystal_Report_Parameter> Result = new List<Helper.Utility.Crystal_Report_Parameter>();
                Helper.Utility.Crystal_Report_Parameter Code_Par = new Helper.Utility.Crystal_Report_Parameter()
                {
                    Name = "Code",
                    Type = Helper.Utility.Crystal_Report_Parameter.DataType.String,
                    Value = Patient_Code
                };
                string Output_Folder = Configurations.Get_Report_Output_Folder_path(company);
                string Pdf_File_Name = Helper.Utility.Crystal_Report_Export(company, Rpt_File, Result.ToArray(), Helper.Utility.Export_File_Format.PDF, "", Output_Folder);
                System.Diagnostics.Process.Start(Pdf_File_Name);
            }
            if (SBO_Application.MessageBox($"Are you sure you want to {Action_Title}?", 1, "Yes", "No") != 1)
            {
                return;
            }
            string UDO_Code = form.DataSources.DBDataSources.Item(0).GetValue("Code", 0);
            UDO_Definition UDO_Info = Logic.Objects_Logic.All_UDO_Definition.FirstOrDefault(U => U.KHCF_Object == KHCF_Objects.Patients_Card);

            switch (Action_ID)
            {
                case "R":
                    string SQL_User = $@"Select U_ST_CAN_REMOVE_PATIENT from OUSR WHERE USER_CODE = '{company.UserName}'";
                    Recordset RC_User = Helper.Utility.Execute_Recordset_Query(company, SQL_User);
                    if (RC_User.Fields.Item("U_ST_CAN_REMOVE_PATIENT").Value.ToString() != "Y" )
                    {
                        throw new Logic.Custom_Exception("The User don't have authorization to remove the Patient");
                    }
                    string BP_CCI_Code = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_CCI_BP_CODE", 0);
                    string BP_Other_CCI_Code = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_OTHER_CCI_BP_CODE", 0);
                    string BP_Woodwill_Code = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_GOODWILL_BP_CODE", 0);

                    try
                    {
                        company.StartTransaction();
                        KHCF_Logic_Utility.Remove_UDO_Entry(company, UDO_Code, UDO_Info, false);

                        Utility.Remove_BP(company, BP_CCI_Code);
                        Utility.Remove_BP(company, BP_Other_CCI_Code);
                        Utility.Remove_BP(company, BP_Woodwill_Code);

                        company.EndTransaction(BoWfTransOpt.wf_Commit);
                        form.Mode = BoFormMode.fm_FIND_MODE;
                    }
                    catch (Exception ex)
                    {
                        try
                        {
                            company.EndTransaction(BoWfTransOpt.wf_RollBack);
                        }
                        catch (Exception)
                        { }

                        throw new Logic.Custom_Exception($"Error during remove Patient[{ex.Message}]");
                    }

                    break;
                default:
                    throw new Logic.Custom_Exception($"This Report [{Action_ID}] is not supported");

            }


            SBO_Application.StatusBar.SetText("Done!", BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Success);
        }

        private static void Create_Coverage_Request(ItemEvent pVal, string Type)
        {
            Form form = SBO_Application.Forms.Item(pVal.FormUID);

            if (form.Mode != BoFormMode.fm_OK_MODE)
            {
                throw new Logic.Custom_Exception("We can create the Coverage Request if the form in OK mode only");
            }

            string Patient_Code = form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).GetValue("Code", 0);
            Form Coverage_Request_Form = Loader.Open_UDO_Form(KHCF_Objects.Coverage_Request);
            Coverage_Request_Form.Mode = BoFormMode.fm_ADD_MODE;
            Coverage_Request_Form.DataSources.DBDataSources.Item("@ST_COVERAGE_REQUEST").SetValue("U_ST_PATIENT_CARD", 0, Patient_Code);
            Coverage_Request_Form.DataSources.DBDataSources.Item("@ST_COVERAGE_REQUEST").SetValue("U_ST_PATIENT_TYPE", 0, Type);
            if (Type == "C")
            {
                Coverage_Request_Form.DataSources.DBDataSources.Item("@ST_COVERAGE_REQUEST").SetValue("U_ST_PATIENT_VENDOR_CODE", 0, form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).GetValue("U_ST_CCI_BP_CODE", 0));
                Coverage_Request_Form.DataSources.DBDataSources.Item("@ST_COVERAGE_REQUEST").SetValue("U_ST_SUPPORT_AMOUNT", 0, form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).GetValue("U_ST_PREMIUM", 0));
            }
            if (Type == "G")
            {
                Coverage_Request_Form.DataSources.DBDataSources.Item("@ST_COVERAGE_REQUEST").SetValue("U_ST_PATIENT_VENDOR_CODE", 0, form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).GetValue("U_ST_GOODWILL_BP_CODE", 0));
            }
            if (Type == "O")
            {
                Coverage_Request_Form.DataSources.DBDataSources.Item("@ST_COVERAGE_REQUEST").SetValue("U_ST_PATIENT_VENDOR_CODE", 0, form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).GetValue("U_ST_OTHER_CCI_BP_CODE", 0));
            }
            Frm_Coverage_Request.Form_Obj.Set_Fields(Coverage_Request_Form);

            Frm_Coverage_Request.Set_Patien_Data(Coverage_Request_Form, Patient_Code);
        }

        private static void Create_Social_Study(ItemEvent pVal)
        {
            Form form = SBO_Application.Forms.Item(pVal.FormUID);

            if (form.Mode !=  BoFormMode.fm_OK_MODE)
            {
                throw new Logic.Custom_Exception("We can create the Social Study if the form in OK mode only");
            }

            string Patient_Code = form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).GetValue("Code", 0);
            string SQL_Social_Pationt = $@"SELECT T0.""Code"" FROM ""@ST_SOCIAL_STUDY""  T0 WHERE T0.U_ST_PATIENT_CARD_ID = '{Patient_Code}'";
            Recordset RC_Social_Patient = Helper.Utility.Execute_Recordset_Query(company, SQL_Social_Pationt);
            Form Social_Study_Form = Loader.Open_UDO_Form(KHCF_Objects.Social_Study);
            if (RC_Social_Patient.RecordCount !=0)
            {
                Social_Study_Form.Mode = BoFormMode.fm_FIND_MODE;
                ((EditText)Social_Study_Form.Items.Item("5").Specific).Value = RC_Social_Patient.Fields.Item("Code").Value.ToString();
                string f = Social_Study_Form.Items.Item("1").UniqueID;
                Social_Study_Form.Items.Item("1").Click();
                return;
            }
            string MemberCard_Code = form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).GetValue("U_ST_MEMBER_CARD", 0);
            Frm_Social_Study.Form_Obj.Set_Fields(Social_Study_Form);
            Social_Study_Form.Mode = BoFormMode.fm_ADD_MODE;
            Social_Study_Form.DataSources.DBDataSources.Item(0).SetValue("U_ST_PATIENT_CARD_ID", 0, Patient_Code);
            Social_Study_Form.DataSources.DBDataSources.Item(0).SetValue("U_ST_MEMBER_CARD", 0, MemberCard_Code);

            Frm_Social_Study.Select_Patient_Code(Social_Study_Form, Patient_Code);


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
            DBDataSource DS_Attachment = form.DataSources.DBDataSources.Item("@ST_PATIENTS_CRD_ATT");
            Matrix Mat_Add = (Matrix)form.Items.Item("164").Specific;
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

        private static void Remove_Attachment(ItemEvent pVal)
        {
            Form form = SBO_Application.Forms.Item(pVal.FormUID);

            Form_Obj.Remove_Matrix_Row(form, "164");

        }
        private static void Open_Attachment(ItemEvent pVal)
        {
            Form form = SBO_Application.Forms.Item(pVal.FormUID);

            Matrix Mat = (Matrix)form.Items.Item("164").Specific;
            for (int i = 0; i < Mat.RowCount; i++)
            {
                CheckBox Chk_Selected = (CheckBox)Mat.GetCellSpecific("SELECTED", i + 1);
                if (Chk_Selected.Checked)
                {
                    EditText Txt_FileName = (EditText)Mat.GetCellSpecific("FileName", i + 1);
                    System.Diagnostics.Process.Start(Txt_FileName.Value);
                }
            }


        }

        private static void Choose_From_List_MemberCard(ItemEvent pVal)
        {
            Form form = SBO_Application.Forms.Item(pVal.FormUID);
            string Code = Utility.Chosse_From_List_For_Code_And_DBDataSource(pVal, pVal.ItemUID, false, Form_Obj.UDO_Database_Table_Name);

            Select_MemberCard(form, Code);

            if (form.Mode == BoFormMode.fm_OK_MODE)
            {
                form.Mode = BoFormMode.fm_UPDATE_MODE;
            }
        }

        internal static void Select_MemberCard(Form form, string Code)
        {
            int CCI_Patient_Vendor_Group = Configurations.Get_CCI_Patient_Vendor_Group(company);
            form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).SetValue("U_ST_PATIENT_VENDOR_GROUP_MSH", 0, CCI_Patient_Vendor_Group.ToString());

            string SQL = $@"SELECT T1.*, T0.U_ST_START_DATE, T0.U_ST_END_DATE
, T0.U_ST_COVERAGE, T0.U_ST_WAITING_PERIOD, T0.U_ST_PREMIUM 
FROM ""@ST_INDIV_MEMBERSHIP""  T0 INNER JOIN ""@ST_CCI_INDIV_CARD"" T1 ON T0.U_ST_MEMBER_CARD = T1.""Code"" WHERE T1.""Code"" = '{Code}'";
            Recordset RC = Helper.Utility.Execute_Recordset_Query(company, SQL);
            if (RC.Fields.Item("U_ST_PARENT_TYPE").Value.ToString() == "C")
            {
                form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).SetValue("U_ST_MEMBERSHIP_TYPE", 0, "C");
            }
            else
            {
                form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).SetValue("U_ST_MEMBERSHIP_TYPE", 0, "I");
            }
            // form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).SetValue("", 0, RC.Fields.Item("U_ST_PARENT_ID").Value.ToString());
            form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).SetValue("U_ST_CORPORATE_NAME", 0, RC.Fields.Item("U_ST_PARENT_NAME").Value.ToString());
            DateTime StartDate = (DateTime)RC.Fields.Item("U_ST_START_DATE").Value;
            form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).SetValue("U_ST_START_DATE", 0, StartDate.ToString("yyyyMMdd"));
            form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).SetValue("U_ST_END_DATE", 0, ((DateTime)RC.Fields.Item("U_ST_END_DATE").Value).ToString("yyyyMMdd"));
            form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).SetValue("U_ST_WAITING_PERIOD_END_DATE", 0, RC.Fields.Item("U_ST_WAITING_PERIOD").Value.ToString());
            form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).SetValue("U_ST_PREMIUM", 0, RC.Fields.Item("U_ST_PREMIUM").Value.ToString());
            string SQL_Coverage_Amount = $@"SELECT T0.""U_ST_TREATMENT_LIMIT"", T0.""U_ST_TRANSPORTATION_LIMIT"" FROM ""@ST_COVERAGE""  T0 WHERE T0.""Code"" ='{RC.Fields.Item("U_ST_COVERAGE").Value.ToString()}'";
            Recordset RC_Coverage_Amount = Helper.Utility.Execute_Recordset_Query(company, SQL_Coverage_Amount);
            double Coverge_Amount = (double)RC_Coverage_Amount.Fields.Item("U_ST_TREATMENT_LIMIT").Value +  (double)RC_Coverage_Amount.Fields.Item("U_ST_TRANSPORTATION_LIMIT").Value;
            form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).SetValue("U_ST_COVERAGE_MEMBERSHIP", 0, Coverge_Amount.ToString());

            string[] MemberCard_Fields = new string[] { "U_ST_FIRST_NAME_AR", "U_ST_FATHER_NAME_AR", "U_ST_MIDDLE_NAME_AR", "U_ST_SURNAME_AR"
            ,  "U_ST_FIRST_NAME_EN", "U_ST_FATHER_NAME_EN", "U_ST_MIDDLE_NAME_EN", "U_ST_SURNAME_EN", "U_ST_NATIONAL_ID", "U_ST_PERSONAL_ID", "U_ST_PASSPORT_ID", "U_ST_TEL1", "U_ST_TEL2"
            , "U_ST_DATE_OF_BIRTH", "U_ST_GENDER", "U_ST_NATIONALITY"};

            foreach (string OneField in MemberCard_Fields)
            {
                string Value;
                if (RC.Fields.Item(OneField).Type == BoFieldTypes.db_Date)
                {
                    Value = ((DateTime)RC.Fields.Item(OneField).Value).ToString("yyyyMMdd");
                }
                else
                {
                    Value = RC.Fields.Item(OneField).Value.ToString();
                }
                form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).SetValue(OneField, 0, Value);
            }
            form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).SetValue("U_ST_FULL_ARABIC_NAME", 0, RC.Fields.Item("U_ST_FULL_NAME_AR").Value.ToString());
            form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).SetValue("U_ST_FULL_ENGLISH_NAME", 0, RC.Fields.Item("U_ST_FULL_NAME_EN").Value.ToString());

            if (RC.Fields.Item("U_ST_PARENT_TYPE").Value.ToString() == "C")
            {
                //string SQL_Corp = $@"call ST_MEMBERSHIP_SUMMARY('{RC.Fields.Item("U_ST_PARENT_ID").Value.ToString()}')";
                //Recordset RC_Corp = Helper.Utility.Execute_Recordset_Query(company, SQL_Corp);
                //form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).SetValue("U_ST_ACTIVE_CORPORATE_SUBSCRIBERS", 0, RC_Corp.Fields.Item("Number_of_Active_Members").Value.ToString());
                //form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).SetValue("U_ST_CORPORATE_TOTAL_REVENUE", 0, RC_Corp.Fields.Item("Total_Net_Premiums").Value.ToString());
            }

            Field_Definition[] Add_Fields = Logic.Fields_Logic.All_Field_Definition.Where(F => F.KHCF_Object == KHCF_Objects.CCI_Member_Card_Address).ToArray();
            DBDataSource DB_Address = form.DataSources.DBDataSources.Item("@ST_PATIENTS_ADDRESS");
            string SQL_Add = $@"SELECT *  FROM ""@ST_CCI_INDIV_ADDR""  T0 WHERE T0.""Code"" ='{Code}'";
            Recordset RC_Add = Helper.Utility.Execute_Recordset_Query(company, SQL_Add);
            int Address_Index = 0;
            for (int J = 0; J < RC_Add.RecordCount; J++)
            {
                if (DB_Address.GetValue("U_ST_ADDRESS_NAME", Address_Index) != "")
                {
                    DB_Address.InsertRecord(Address_Index);
                    Address_Index++;
                }
                foreach (Field_Definition One_Field in Add_Fields)
                {
                    DB_Address.SetValue(One_Field.Column_Name_In_DB, Address_Index, RC_Add.Fields.Item(One_Field.Column_Name_In_DB).Value.ToString());
                }

                RC_Add.MoveNext();
            }

           ((Matrix)form.Items.Item("191").Specific).LoadFromDataSource();

        }

        private static void Remove_Address_Row(ItemEvent pVal)
        {
            Form form = SBO_Application.Forms.Item(pVal.FormUID);
            Matrix Mat = (Matrix)form.Items.Item("191").Specific;
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
            DBDataSource DS_Address = form.DataSources.DBDataSources.Item("@ST_PATIENTS_ADDRESS");
            Matrix Mat_Add = (Matrix)form.Items.Item("191").Specific;
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
           // DS_Address.SetValue("U_ST_COUNTRY", Count, "Jordan");

            Mat_Add.LoadFromDataSource();
            form.Freeze(false);

        }


    }
}
