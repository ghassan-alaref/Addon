using SAPbobsCOM;
using SAPbouiCOM;
using ST.KHCF.Customization.Logic;
using ST.KHCF.Customization.Logic.Classes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ST.KHCF.Customization.Forms.Patient
{
    internal class Frm_Coverage_Request : Parent_Form
    {
        internal static SAPbobsCOM.Company company;
        internal static Application SBO_Application;
        internal static Parent_Form Form_Obj;

        private static string[] Accommodation_Items_List = new string[] { "26", "28", "64" };
        private static string[] Treatment_Items_List = new string[] {};
        private static string[] Transportation_List = new string[] { "41", "43", "45" };
        private static List<string> All_Type_Item_List = new List<string>();
        //internal override string[] Get_Mondatory_Fields_List()
        //{
        //    List<string> Result = new List<string>();
        //    Result.AddRange(base.Get_Mondatory_Fields_List());
        //    Result.AddRange(new string[] { "9", "13", "12", "19", "23", "25", "27", "43", "52", "39", "41", "51", "47", "133", "137", "127", "61" });
        //    return Result.ToArray();   "9,13,12,19,23,25,27,43,52,39,41,51,47,133,137,127, "61"
        //}

        //internal override string[] Get_Approval_Items_List()
        //{
        //    List<string> Result = new List<string>();
        //    Result.AddRange(base.Get_Approval_Items_List());
        //    // Result.AddRange(new string[] { "159", "160", "161", "162" });

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
        //    Result.Add("15");
        //    Result.Add("16");
        //    Result.Add("21");
        //    Result.Add("50"); 15,16,21,50
        //    //Result.Add("85");
        //    //Result.Add("86");

        //    return Result.ToArray();
        //}


        //internal override string[] Get_Fix_ReadOnly_Fields_List()
        //{
        //    List<string> Result = new List<string>();
        //    Result.AddRange(base.Get_Fix_ReadOnly_Fields_List());
        //    Result.AddRange(new string[] { "5", "11", "24"});  "5,11,24"

        //    return Result.ToArray();
        //}

        internal override void Initialize_Form(Form form)
        {
            //Code_value = "Frm_Coverage_Request";
            //Desc_value = "Mandatary fields List For Coverage Request ";
            //Man_fields = "9,13,12,19,23,25,27,43,52,39,41,51,47,133,137,127,61";

            //File.AppendAllText("
            //Desktop\\Patient_TimeLog.txt", "Patient Coverage Request" + Environment.NewLine);
            //string startTimeExact = DateTime.Now.ToString("mm:ss:f");
            //DateTime startTime = DateTime.Now;
            //File.AppendAllText("C:\\Users\\wsaleh\\Desktop\\Patient_TimeLog.txt", "Start Time:" + startTimeExact + Environment.NewLine);

            base.Initialize_Form(form);
            //DateTime prev = Utility.Add_Time_Log("P", "base.Initialize Form", startTime);

            Matrix Mat_Att = (Matrix)form.Items.Item("54").Specific;
            base.Fill_Attachment_ComboBox(Mat_Att);
            //prev = Utility.Add_Time_Log("P", "Attachments", prev);

            if (All_Type_Item_List.Count == 0)
            {
                All_Type_Item_List.AddRange(Accommodation_Items_List);
                All_Type_Item_List.AddRange(Treatment_Items_List);
                All_Type_Item_List.AddRange(Transportation_List);
            }

            Matrix Mat_Treat = (Matrix)form.Items.Item("77").Specific;
            string SQL_Plan = $@"SELECT *  FROM ""@ST_TREATMENT_PLAN""  T0";
            Helper.Utility.FillMatrixComboBoxForSQL(company, Mat_Treat, "PLAN", SQL_Plan, true);
            Mat_Treat.AutoResizeColumns();
            //form.Items.Item("71").AffectsFormMode = false;
            //form.Items.Item("73").AffectsFormMode = false;


            form.Items.Item("15").Click();
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
                SBO_Application.StatusBar.SetText("Loading", BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_None);
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
            form.Freeze(true);
            string Card_ID = form.DataSources.DBDataSources.Item(0).GetValue("Code", 0);

            Form_Obj.Load_Depends_Items(form);
            Set_Item_Type_Editable(form, true);

            if (form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).GetValue("U_ST_STATUS", 0) == "L")
            {
                //form.Items.Item("1").Enabled = false;
                //form.Items.Item("60").Enabled = false;
                form.Items.Item("1").SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, -1, BoModeVisualBehavior.mvb_False);
                form.Items.Item("60").SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, -1, BoModeVisualBehavior.mvb_False);
            }
            else
            {
                //form.Items.Item("1").Enabled = true;
                //form.Items.Item("60").Enabled = true;
                form.Items.Item("1").SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, -1, BoModeVisualBehavior.mvb_True);
                form.Items.Item("60").SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, -1, BoModeVisualBehavior.mvb_True);
            }

            string SQL_Remai = $@"SELECT SUM(T2.""DocTotal"" - T2.""PaidToDate"")
FROM ""@ST_COVERAGE_REQUEST"" T0 INNER JOIN ""@ST_COVERAGE_TRANS"" T1 ON T0.""Code"" = T1.U_ST_COVERAGE_REQUEST_CODE
INNER JOIN OPCH T2 ON T1.""Code"" = T2.U_ST_COVERAGE_TRANSACTION_CODE
WHERE T0.""Code"" = '{Card_ID}'";
            Recordset RC_Remai = Helper.Utility.Execute_Recordset_Query(company, SQL_Remai);
            form.DataSources.UserDataSources.Item("72").Value = RC_Remai.Fields.Item(0).Value.ToString();

            form.Freeze(false);
        }
        private static void ADD_Update_UDO(BusinessObjectInfo businessObjectInfo)
        {
            Form form = SBO_Application.Forms.Item(businessObjectInfo.FormUID);
            System.Xml.XmlDocument XML_Doc = new System.Xml.XmlDocument();
            string XML_Text = businessObjectInfo.ObjectKey.Replace(" ", "").Replace("=", " = ");
            XML_Doc.LoadXml(XML_Text);

            string UDO_Code = XML_Doc.GetElementsByTagName("Code")[0].InnerText;
            //string UDO_Name = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_FULL_NAME_AR", 0);


            //if (form.DataSources.DBDataSources.Item(0).GetValue("U_ST_APPROVAL_STATUS", 0) == "P")
            //{
            //    Form_Obj.Send_Alert_For_Approve(UDO_Code);
            //}
            //else
            //{
            //    string BP_Code = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_BP_CODE", 0);
            //    UDO_Definition UDO_Info = Logic.Objects_Logic.All_UDO_Definition.FirstOrDefault(U => U.KHCF_Object == KHCF_Objects.CCI_Member_Card);
            //    Utility.Update_BP(company, BP_Code, UDO_Code, UDO_Info);
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
                form.DataSources.DBDataSources.Item( Form_Obj.UDO_Database_Table_Name).SetValue("Code", 0, New_UDO_Code);
                //form.DataSources.DBDataSources.Item(0).SetValue("U_ST_CREATOR", 0, company.UserName);
                // form.DataSources.DBDataSources.Item(0).SetValue("U_ST_PARENT_TYPE", 0, "N");
            }

        }
        private static void Change_Items_Values_Cutomization(string ItemUID, string FormUID, bool BeforeAction, bool ItemChanged, string ColUID, int Row, bool OnLoad = false)
        {
            #region Names Fields


            #endregion

            if (ItemUID == "26" || ItemUID == "28")
            {
                Form form = SBO_Application.Forms.Item(FormUID);
                DateTime Accommodation_Start_Date;
                DateTime.TryParseExact(form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).GetValue("U_ST_ACCOMMODATION_START_DATE", 0), "yyyyMMdd", null, System.Globalization.DateTimeStyles.None,  out Accommodation_Start_Date);
                DateTime Accommodation_End_Date;
                DateTime.TryParseExact(form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).GetValue("U_ST_ACCOMMODATION_END_DATE", 0), "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out Accommodation_End_Date);
                int Accommodation_Days;
                if (Accommodation_End_Date.Year < 2000 || Accommodation_Start_Date.Year < 2000)
                {
                    Accommodation_Days = -1;
                }
                else
                {
                    Accommodation_Days = (Accommodation_End_Date - Accommodation_Start_Date).Days;
                }
                form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).SetValue("U_ST_ACCOMMODATION_NUMBER_OF_DAYS", 0, Accommodation_Days.ToString());

            }


            if (Form_Obj.Get_Depends_Parent_Item_IDs_List().Contains(ItemUID))
            {
                Form form = SBO_Application.Forms.Item(FormUID);
                Form_Obj.Load_One_Depends_Parent_Item(form, ItemUID);
            }

        }
        private static bool Validate_Data(BusinessObjectInfo businessObjectInfo)
        {
            Form form = SBO_Application.Forms.Item(businessObjectInfo.FormUID);

            if (!Form_Obj.Validate_Data(form))
            {
                return false;
            }

            string New_Status = form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).GetValue("U_ST_STATUS", 0);
            if (New_Status == "L")
            {
                string Code = form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).GetValue("Code", 0);
                string Old_Status = "";
                if (Code != "")
                {
                    string SQL_Old_Status = $@"SELECT T0.U_ST_STATUS FROM ""{Form_Obj.UDO_Database_Table_Name}""  T0 WHERE T0.""Code"" = '{Code}'";
                    Recordset RC_Old_Status = Helper.Utility.Execute_Recordset_Query(company, SQL_Old_Status);
                    Old_Status = RC_Old_Status.Fields.Item("U_ST_STATUS").Value.ToString();
                }
                if (Old_Status != New_Status)
                {
                    string SQL_Auth = $@"SELECT T0.U_ST_CAN_CANCEL_COVERAGE_REQUEST
FROM OUSR T0 WHERE T0.""USER_CODE"" = '{company.UserName}'";
                    Recordset RC_Auth = Helper.Utility.Execute_Recordset_Query(company, SQL_Auth);
                    string User_Can = RC_Auth.Fields.Item("U_ST_CAN_CANCEL_COVERAGE_REQUEST").Value.ToString();
                    if (New_Status == "L" && User_Can != "Y")
                    {
                        throw new Logic.Custom_Exception("The User don't have Authorization to Cancel the Coverage Request");
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

                if (!pVal.BeforeAction && pVal.ItemChanged)
                {
                    Change_Items_Values_Cutomization(pVal.ItemUID, pVal.FormUID, pVal.BeforeAction, pVal.ItemChanged, pVal.ColUID, pVal.Row);
                }
                if (pVal.ItemUID == "51" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Add_Attachment(pVal);
                }
                if (pVal.ItemUID == "52" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Remove_Attachment(pVal);
                }
                if (pVal.ItemUID == "53" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Open_Attachment(pVal);
                }
                if (pVal.ItemUID == "60" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Create_Coverage_Transaction(pVal);
                }
                //if (pVal.ItemUID == "74" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                //{
                //    Stop_Request(pVal);
                //}
                if (pVal.ItemUID == "12" && pVal.EventType == BoEventTypes.et_COMBO_SELECT && !pVal.BeforeAction)
                {
                    Form form = SBO_Application.Forms.Item(pVal.FormUID);
                    form.Freeze(true);
                    Set_Item_Type_Editable(form, false);
                    form.Freeze(false);
                }
                if (pVal.ItemUID == "10" && pVal.EventType == BoEventTypes.et_CHOOSE_FROM_LIST && !pVal.BeforeAction)
                {
                    Choose_Patient_From_List(pVal);
                }

                if (pVal.ItemUID == "76" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Add_Line(pVal);
                }
                if (pVal.ItemUID == "75" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Remove_Selected_Lines(pVal);
                }


            }
            catch (Exception ex)
            {
                Form form = SBO_Application.Forms.Item(pVal.FormUID);
                form.Freeze(false);
                SBO_Application.StatusBar.SetText(ex.Message, BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Error);
            }
        }
        //private static void Stop_Request(ItemEvent pVal)
        //{
        //    Form form = SBO_Application.Forms.Item(pVal.FormUID);

        //    if (form.Mode != BoFormMode.fm_OK_MODE)
        //    {
        //        throw new Exception("We can Stop the Coverage Request if the form in OK mode only");
        //    }

        //    if (SBO_Application.MessageBox("Are you sure you want to Stop the coverage Request?", 1 , "Yes", "No"  ) != 1)
        //    {
        //        return;
        //    }
        //    string UDO_Code = form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).GetValue("Code", 0);
        //    string Stop_Reason = form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).GetValue("U_ST_STOP_REASON", 0);
        //    if (Stop_Reason == "-")
        //    {
        //        throw new Exception("Please select the Stop Reason");
        //    }
        //    string Stop_Date_Text = form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).GetValue("U_ST_STOP_DATE", 0);
        //    if (Stop_Date_Text == "")
        //    {
        //        throw new Exception("Please select the Stop Date");
        //    }

        //    KHCF_Logic_Utility.Stop_Coverage_Request(company, UDO_Code, Stop_Reason, DateTime.ParseExact(Stop_Date_Text, "yyyyMMdd", null));
        //}
        private static void Create_Coverage_Transaction(ItemEvent pVal)
        {
            Form form = SBO_Application.Forms.Item(pVal.FormUID);

            if (form.Mode != BoFormMode.fm_OK_MODE)
            {
                throw new Logic.Custom_Exception("We can create the Coverage Request if the form in OK mode only");
            }

            string Coverage_Request_Code = form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).GetValue("Code", 0);
            Form Coverage_Transaction_Form = Loader.Open_UDO_Form(KHCF_Objects.Coverage_Transaction);
            Coverage_Transaction_Form.Mode = BoFormMode.fm_ADD_MODE;
            Coverage_Transaction_Form.DataSources.DBDataSources.Item("@ST_COVERAGE_TRANS").SetValue("U_ST_COVERAGE_REQUEST_CODE", 0, Coverage_Request_Code);

            Frm_Coverage_Transaction.Set_Coverage_Request_Data(Coverage_Transaction_Form, Coverage_Request_Code);
        }
        private static void Choose_Patient_From_List(ItemEvent pVal)
        {
            string Patient_Code= Utility.Chosse_From_List_For_Code_And_DBDataSource(pVal, pVal.ItemUID, false, Form_Obj.UDO_Database_Table_Name);
            Form form = SBO_Application.Forms.Item(pVal.FormUID);

            //Set_Patien_Data(form, Patient_Code);

        }
        internal static void Set_Patien_Data(Form form, string Patient_Code)
        {
            string SQL = $@"SELECT T0.U_ST_FULL_ARABIC_NAME
FROM ""@ST_PATIENTS_CARD""  T0 WHERE T0.""Code"" ='{Patient_Code}'";
            Recordset RC = Helper.Utility.Execute_Recordset_Query(company, SQL);
            form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).SetValue("U_ST_PATIENT_NAME", 0 , RC.Fields.Item("U_ST_FULL_ARABIC_NAME").Value.ToString());
            //            string Patient_Type = "";
            //            if (RC.Fields.Item("U_ST_MEMBER_CARD").Value.ToString() != "")
            //            {
            //                Patient_Type = "C";
            //            }
            //            else if (RC.Fields.Item("U_ST_PATIENT_VENDOR_GROUP_GW").Value.ToString() != "" && RC.Fields.Item("U_ST_PATIENT_VENDOR_GROUP_GW").Value.ToString() != "-")
            //            {
            //                Patient_Type = "G";
            //            }
            //            else if (RC.Fields.Item("U_ST_PATIENT_VENDOR_GROUP_CCI").Value.ToString() != "" && RC.Fields.Item("U_ST_PATIENT_VENDOR_GROUP_CCI").Value.ToString() != "-")
            //            {
            //                Patient_Type = "O";
            //            }

            //            if (Patient_Type =="")
            //            {
            //                throw new Logic.Custom_Exception("The Patient type not found");
            //            }

            //            form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).SetValue("U_ST_PATIENT_TYPE", 0, Patient_Type);



        }
        private static void Set_Item_Type_Editable(Form form, bool On_Load)
        {
            form.Freeze(true);

            foreach (string item in All_Type_Item_List)
            {
                Utility.Set_Item_Editable(form, item, false, true);

                if (On_Load == false)
                {
                    string Field_Name = Helper.Utility.Get_Item_DB_Datasource(form.Items.Item(item));
                    Field_Definition Field_Def = Logic.Fields_Logic.All_Field_Definition.FirstOrDefault(F => F.KHCF_Object == KHCF_Objects.Coverage_Request && F.Column_Name_In_DB == Field_Name);
                    if (Field_Def == null)
                    {
                        throw new Logic.Custom_Exception($"The Field[{Field_Name}] is not supported");
                    }
                    if (Field_Def.Data_Type == BoFieldTypes.db_Float || Field_Def.Data_Type == BoFieldTypes.db_Numeric)
                    {
                        form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).SetValue(Field_Name, 0, "0");
                    }
                    else
                    {
                        form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).SetValue(Field_Name, 0, "");
                    }

                }
            }
            if (On_Load == false)
            {
                form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).SetValue("U_ST_ACCOMMODATION_NUMBER_OF_DAYS", 0, "0");
            }
            Utility.Set_Item_Editable(form, "75", false, true);
            Utility.Set_Item_Editable(form, "76", false, true);

            string Support_Type = form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).GetValue("U_ST_SUPPORT_TYPE", 0);

            switch (Support_Type)
            {
                case "A":
                    foreach (string item in Accommodation_Items_List)
                    {
                        Utility.Set_Item_Editable(form, item, true, true);
                    }
                    form.Items.Item("15").Click();
                    break;
                case "T":
                    foreach (string item in Treatment_Items_List)
                    {
                        Utility.Set_Item_Editable(form, item, true, true);
                    }
                    Utility.Set_Item_Editable(form, "75", true, true);
                    Utility.Set_Item_Editable(form, "76", true, true);

                    form.Items.Item("16").Click();
                    break;
                case "P":
                    foreach (string item in Transportation_List)
                    {
                        Utility.Set_Item_Editable(form, item, true, true);
                    }
                    form.Items.Item("21").Click();
                    break;
                default:
                    break;
            }

            form.Freeze(false);
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
            DBDataSource DS_Attachment = form.DataSources.DBDataSources.Item("@ST_COVER_REQ_ATT");
            Matrix Mat_Add = (Matrix)form.Items.Item("54").Specific;
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

            Form_Obj.Remove_Matrix_Row(form, "54");

        }
        private static void Open_Attachment(ItemEvent pVal)
        {
            Form form = SBO_Application.Forms.Item(pVal.FormUID);

            Matrix Mat = (Matrix)form.Items.Item("54").Specific;
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

        private static void Add_Line(ItemEvent pVal)
        {
            Form form = SBO_Application.Forms.Item(pVal.FormUID);
            //form.Freeze(true);
            DBDataSource DS_Lines = form.DataSources.DBDataSources.Item("@ST_COVER_REQ_TREAT");
            Matrix Mat_Lines = (Matrix)form.Items.Item("77").Specific;
            Mat_Lines.AddRow();
            Mat_Lines.ClearRowData(Mat_Lines.RowCount);
            if (form.Mode == BoFormMode.fm_OK_MODE)
            {
                form.Mode = BoFormMode.fm_UPDATE_MODE;
            }
            return;
  

        }

        private static void Remove_Selected_Lines(ItemEvent pVal)
        {
            Form form = SBO_Application.Forms.Item(pVal.FormUID);
            Matrix Mat = (Matrix)form.Items.Item("77").Specific;
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
            if (form.Mode == BoFormMode.fm_OK_MODE)
            {
                form.Mode = BoFormMode.fm_UPDATE_MODE;
            }

        }

    }
}
