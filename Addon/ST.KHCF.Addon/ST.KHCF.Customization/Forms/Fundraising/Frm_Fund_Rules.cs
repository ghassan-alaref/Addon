using SAPbobsCOM;
using SAPbouiCOM;
using ST.KHCF.Customization.Logic;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ST.KHCF.Customization.Forms.Fundraising
{
    internal class Frm_Fund_Rules : Parent_Form
    {
        internal static SAPbobsCOM.Company company;
        internal static Application SBO_Application;
        internal static Parent_Form Form_Obj;
        internal static string[] Relations_Grid_IDs = new string[] { "71" };
        static int checkFirst = 0;
        //internal override string[] Get_Mondatory_Fields_List()
        //{
        //    List<string> Result = new List<string>();
        //    Result.AddRange(base.Get_Mondatory_Fields_List());
        //    Result.AddRange(new string[] { "14","15","16" });   "14,15,16"
        //    return Result.ToArray();
        //}

        //internal override string[] Get_Fix_ReadOnly_Fields_List()
        //{
        //    List<string> Result = new List<string>();
        //    Result.AddRange(base.Get_Fix_ReadOnly_Fields_List());
        //    Result.AddRange(new string[] { "5" });

        //    return Result.ToArray();
        //}

        internal override void Initialize_Form(Form form)
        {
            checkFirst = 0;
            //Desc_value = "Mandatary fields List For Goodwill Funds Card ";
            //Man_fields = "14,15,16";

            //File.AppendAllText("C:\\Users\\wsaleh\\Desktop\\Fund_TimeLog.txt", "FundRaising Goodwill Funds" + Environment.NewLine);
            //string startTimeExact = DateTime.Now.ToString("mm:ss:f");
            //DateTime startTime = DateTime.Now;
            //File.AppendAllText("C:\\Users\\wsaleh\\Desktop\\Fund_TimeLog.txt", "Start Time:" + startTimeExact + Environment.NewLine);
            base.Initialize_Form(form);
            //form.State = BoFormStateEnum.fs_Maximized;
            //DateTime prev = Utility.Add_Time_Log("F", "base.Initialize Form", startTime);
            Matrix Mat_Att = (Matrix)form.Items.Item("63").Specific;
            base.Fill_Attachment_ComboBox(Mat_Att);
            //prev = Utility.Add_Time_Log("F", "Attachment", prev);
           

            SAPbouiCOM.ChooseFromList CFL_Doner = form.ChooseFromLists.Item("CFL_Orphans");
            Conditions Doner_Cons = CFL_Doner.GetConditions();
            Condition Doner_Con = Doner_Cons.Add();
            Doner_Con.Alias = "U_ST_CUSTOMER_GROUP";
            Doner_Con.CondVal = "110";
            Doner_Con.Operation = BoConditionOperation.co_EQUAL;
            CFL_Doner.SetConditions(Doner_Cons);

            

            Matrix subNationalityMatrix = (Matrix)form.Items.Item("136").Specific;
            string SQL_Sub_Nationality = $@"SELECT T0.""Code"",T0.""Name"" FROM ""@ST_SUB_NATIONALITY"" T0";
            Helper.Utility.FillMatrixComboBoxForSQL(company, subNationalityMatrix, "CODE", SQL_Sub_Nationality);

            Matrix supportMatrix = (Matrix)form.Items.Item("139").Specific;
            string SQL_Support = $@"SELECT T0.""Code"",T0.""Name"" FROM ""@ST_SUPPORT_TYPE"" T0";
            Helper.Utility.FillMatrixComboBoxForSQL(company, supportMatrix, "CODE", SQL_Support);

            Matrix cancerMatrix = (Matrix)form.Items.Item("182").Specific;
            string SQL_Cancer_Type = $@"SELECT T0.""Code"",T0.""Name"" FROM ""@ST_CANCER_TYPE"" T0";
            Helper.Utility.FillMatrixComboBoxForSQL(company, cancerMatrix, "CODE", SQL_Cancer_Type);

            Matrix reportMatrix = (Matrix)form.Items.Item("162").Specific;
            string SQL_Report_Frequency = $@"SELECT T0.""Code"",T0.""Name"" FROM ""@ST_REPORTS_FREQUEN"" T0";
            Helper.Utility.FillMatrixComboBoxForSQL(company, reportMatrix, "FREQ", SQL_Report_Frequency);

            string SQL_Report_Type = $@"SELECT T0.""Code"",T0.""Name"" FROM ""@ST_REPORTS_REQUIRE"" T0";
            Helper.Utility.FillMatrixComboBoxForSQL(company, reportMatrix, "CODE", SQL_Report_Type);

            form.Items.Item("Item_36").Click();
            //prev = Utility.Add_Time_Log("F", "", startTime, true);
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
                Form form = SBO_Application.Forms.Item(BusinessObjectInfo.FormUID);
                if (form.Mode == BoFormMode.fm_FIND_MODE)
                {
                    Form_Obj.UnSet_Mondatory_Fields_Color(form);
                }
                SBO_Application.StatusBar.SetText("Loading", BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_None);

            }
            catch (Exception ex)
            {
                Loader.New_Msg = ex.Message;

                BubbleEvent = false;
            }

            return BubbleEvent;
        }

        private static void ADD_Update_UDO(BusinessObjectInfo businessObjectInfo)
        {
            Form form = SBO_Application.Forms.Item(businessObjectInfo.FormUID);
            System.Xml.XmlDocument XML_Doc = new System.Xml.XmlDocument();
            string XML_Text = businessObjectInfo.ObjectKey.Replace(" ", "").Replace("=", " = ");
            XML_Doc.LoadXml(XML_Text);
            string UDO_Code = XML_Doc.GetElementsByTagName("Code")[0].InnerText;

            Utility.Update_Relation_Table(company, form, UDO_Code, Relations_Grid_IDs, Form_Obj.KHCF_Object);

        }


        private static void Form_Data_Load(BusinessObjectInfo businessObjectInfo)
        {
            Form form = SBO_Application.Forms.Item(businessObjectInfo.FormUID);
            string UDO_Code = form.DataSources.DBDataSources.Item(0).GetValue("Code", 0);
            //string SQL_Invoice = $@"Select ""DocEntry""  AS ""Code"", ""DocNum"" AS ""Name"" FROM OINV Where ""DocEntry"" = 0{form.DataSources.DBDataSources.Item(0).GetValue("U_ST_INVOICE_NUMBER", 0)}";
            //Helper.Utility.Fill_One_ComboBoxBySQL(company, form, "87", SQL_Invoice, false);

            form.Freeze(true);
            Utility.Fill_Relation_Grids(company, form, UDO_Code, Relations_Grid_IDs);

            Utility.Load_All_Relation_Data(company, form, UDO_Code, Relations_Grid_IDs, Form_Obj.KHCF_Object);

            Form_Obj.Set_Fields(form);

            form.Freeze(false);
        }

        private static void Fill_Nationality_Grid(Form form, string Grd_ID)
        {
            Grid Grd = (Grid)form.Items.Item(Grd_ID).Specific;
            DataTable DT = Grd.DataTable;
            DT.Rows.Clear();
            string SQL = $@"Select ""Code"", ""Name"" FROM ""@{DT.UniqueID}""";
            Recordset RC = Helper.Utility.Execute_Recordset_Query(company, SQL);
            DT.Rows.Add(RC.RecordCount);

            for (int i = 0; i < RC.RecordCount; i++)
            {
                DT.SetValue("Code", i, RC.Fields.Item("Code").Value);
                DT.SetValue("Name", i, RC.Fields.Item("Name").Value);
                RC.MoveNext();
            }

            Grd.Columns.Item("Code").Visible = false;
            Grd.AutoResizeColumns();

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
                //form.DataSources.DBDataSources.Item(0).SetValue("U_ST_CREATOR", 0, company.UserName);
                //form.DataSources.DBDataSources.Item(0).SetValue("U_ST_PARENT_TYPE", 0, "N");
            }

        }

        private static bool Validate_Data(BusinessObjectInfo businessObjectInfo)
        {
            Form form = SBO_Application.Forms.Item(businessObjectInfo.FormUID);

            if (!Form_Obj.Validate_Data(form))
            {
                return false;
            }
            int row = -5;
            if (!Validate_Age(out row))
            {
                if (row != -5)
                {
                    throw new Custom_Exception($@"Age Tab at line number {row} Contains from value larger than to value ");
                }
                return false;
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
                if ((pVal.ItemUID == "30") && pVal.EventType == BoEventTypes.et_CHOOSE_FROM_LIST && !pVal.BeforeAction)
                {
                    Frm_Expected_Donations.Choose_General_Item_ID(pVal);
                }
                if (pVal.ItemUID == "100" && pVal.EventType == BoEventTypes.et_CHOOSE_FROM_LIST && !pVal.BeforeAction)
                {
                    Choose_Opportunity_ID(pVal);
                }
                if (pVal.ItemUID == "101" && pVal.EventType == BoEventTypes.et_CHOOSE_FROM_LIST && !pVal.BeforeAction)
                {
                    Choose_Opportunity_Name(pVal);
                }
                if (pVal.ItemUID == "60" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Add_Attachment(pVal);
                }
                if (pVal.ItemUID == "61" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Remove_Attachment(pVal);
                }
                if (pVal.ItemUID == "62" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Open_Attachment(pVal);
                }
                if (pVal.ItemUID == "112" && pVal.ColUID == "CODE" && pVal.EventType == BoEventTypes.et_CHOOSE_FROM_LIST && !pVal.BeforeAction)
                {
                    Patients_choose_From_List(pVal, "@ST_WON_GRANT_PATS");
                }
                if (pVal.ItemUID == "111" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Remove_Selected_Lines(pVal, "Patients_Details", "112");
                    //Remove_Patients_Selected_Lines(pVal);
                }
                if (pVal.ItemUID == "110" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Add_Line(pVal, "@ST_WON_GRANT_PATS", "Patients_Details", "112", "U_ST_PATIENT_CODE", true);
                    //Add_Patients_Line(pVal, "@ST_WON_GRANT_PATS");
                }
                if (pVal.ItemUID == "116" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Remove_Selected_Lines(pVal, "Wish_Details", "117");
                    //Remove_Wish_Selected_Lines(pVal);
                }
                if (pVal.ItemUID == "115" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Add_Line(pVal, "@ST_WON_GRANT_WISH", "Wish_Details", "117", "U_ST_WISH_CODE", true);
                    //Add_Wish_Line(pVal, "@ST_WON_GRANT_WISH");
                }
                if (pVal.ItemUID == "117" && pVal.ColUID == "CODE" && pVal.EventType == BoEventTypes.et_CHOOSE_FROM_LIST && !pVal.BeforeAction)
                {
                    Wish_choose_From_List(pVal, "@ST_WON_GRANT_WISH");
                }
                if (pVal.ItemUID == "120" && pVal.ColUID == "CODE" && pVal.EventType == BoEventTypes.et_CHOOSE_FROM_LIST && !pVal.BeforeAction)
                {
                    Orphans_Choos_From_List(pVal, "@ST_WON_GRANT_ORPH");
                }
                if (pVal.ItemUID == "119" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Remove_Selected_Lines(pVal, "Orphan_Details", "120");
                    //Remove_Orphans_Selected_Lines(pVal);
                }
                if (pVal.ItemUID == "118" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Add_Line(pVal, "@ST_WON_GRANT_ORPH", "Orphan_Details", "120", "U_ST_ORPHAN_CODE", true);
                    //Add_Orphans_Line(pVal, "@ST_WON_GRANT_ORPH");
                }
                if (pVal.ItemUID == "148" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Remove_Selected_Lines(pVal, "Participants_Details", "149");
                    //Remove_Participants_Selected_Lines(pVal);
                }
                if (pVal.ItemUID == "147" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Add_Line(pVal, "@ST_GOOD_WILL_PARTI", "Participants_Details", "149", "U_ST_PARTICIPANT_CODE", true);
                    //Add_Participants_Line(pVal, "@ST_GOOD_WILL_PARTI");
                }
                if (pVal.ItemUID == "149" && pVal.ColUID == "CODE" && pVal.EventType == BoEventTypes.et_CHOOSE_FROM_LIST && !pVal.BeforeAction)
                {
                    Participants_Choos_From_List(pVal, "@ST_GOOD_WILL_PARTI");
                }
                if (pVal.ItemUID == "151" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Remove_Selected_Lines(pVal, "Pledges_Details", "152");
                    //Remove_Pledges_Selected_Lines(pVal);
                }
                if (pVal.ItemUID == "150" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Add_Line(pVal, "@ST_PLEDGES_PATIENTS", "Pledges_Details", "152", "U_ST_PATIENTS_CODE", true);
                    //Add_Pledge_Line(pVal, "@ST_PLEDGES_PATIENTS");
                }
                if (pVal.ItemUID == "152" && pVal.ColUID == "CODE" && pVal.EventType == BoEventTypes.et_CHOOSE_FROM_LIST && !pVal.BeforeAction)
                {
                    Pledges_Choos_From_List(pVal, "@ST_PLEDGES_PATIENTS");
                }
                if (pVal.ItemUID == "161" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Remove_Selected_Lines(pVal, "Reports_Details", "162");
                    //Remove_Report_Selected_Lines(pVal);
                }
                if (pVal.ItemUID == "160" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Add_Line(pVal, "@ST_GOODWILL_REPORT", "Reports_Details", "162", "U_ST_REPORT_CODE", true);
                    //Add_Report_Line(pVal, "@ST_GOODWILL_REPORT");
                }
                if (pVal.ItemUID == "140" && pVal.EventType == BoEventTypes.et_CHOOSE_FROM_LIST && !pVal.BeforeAction)
                {
                    Choose_Creator(pVal);
                }
                if (pVal.ItemUID == "312" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Remove_Selected_Lines(pVal, "Nationality_Details", "313");
                    //Remove_Nationality_Selected_Lines(pVal);
                }
                if (pVal.ItemUID == "311" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Add_Line(pVal, "@ST_GOODWILL_NAT", "Nationality_Details", "313", "U_ST_COUNTRY_CODE", true);
                    //Add_Nationality_Line(pVal, "@ST_GOODWILL_NAT");
                }
                if (pVal.ItemUID == "313" && pVal.ColUID == "CODE" && pVal.EventType == BoEventTypes.et_CHOOSE_FROM_LIST && !pVal.BeforeAction)
                {
                    Nationality_choose_From_List(pVal, "@ST_GOODWILL_NAT");
                }
                if (pVal.ItemUID == "135" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Remove_Selected_Lines(pVal, "Sub_Details", "136");
                    //Remove_Sub_Nationality_Selected_Lines(pVal);
                }
                if (pVal.ItemUID == "134" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Add_Line(pVal, "@ST_GW_SUB_NAT", "Sub_Details", "136", "U_ST_SUB_CODE", true);
                    //Add_Sub_Nationality_Line(pVal, "@ST_GW_SUB_NAT");
                }
                if (pVal.ItemUID == "138" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Remove_Selected_Lines(pVal, "Support_Details", "139");
                    //Remove_Support_Selected_Lines(pVal);
                }
                if (pVal.ItemUID == "137" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Add_Line(pVal, "@ST_GW_SUP_TYPE", "Support_Details", "139", "U_ST_SUPPORT", true);
                    //Add_Support_Line(pVal, "@ST_GW_SUP_TYPE");
                }
                if (pVal.ItemUID == "181" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Remove_Selected_Lines(pVal, "Cancer_Details", "182");
                    //Remove_Cancer_Selected_Lines(pVal);
                }
                if (pVal.ItemUID == "180" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Add_Line(pVal, "@ST_GW_CAN_TYPE", "Cancer_Details", "182", "U_ST_CANCER_TYPE", true);
                    //Add_Cancer_Line(pVal, "@ST_GW_CAN_TYPE");
                }
                if (pVal.ItemUID == "124" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Add_Line(pVal, "@ST_FUND_RULE_AGE", "Age_Dets", "126", "U_ST_FROM", true);
                    //Add_Patients_Line(pVal, "@ST_WON_GRANT_PATS");
                }
                if (pVal.ItemUID == "125" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Remove_Selected_Lines(pVal, "Age_Dets", "126");
                    //Remove_Wish_Selected_Lines(pVal);
                }

            }
            catch (Exception ex)
            {
                Form form = SBO_Application.Forms.Item(pVal.FormUID);
                form.Freeze(false);
                SBO_Application.StatusBar.SetText(ex.Message, BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Error);
            }
        }

        //private static void SetRequiredFields(string type,Form form)
        //{
        //    try
        //    {
        //        form.Freeze(true);
        //        List<string> fund = new List<string>() { "14", "15", "16", "18", "20", "21", "24", "36", "37", "39", "40", "41", "43", "44", "55", "45", "46" };
        //        List<string> grant = new List<string>() { "100", "101", "104", "105", "106", "107", "108", "109", "Item_42", "Item_7", "Item_12" };
        //        List<string> pledge = new List<string>() { "140", "141", "142", "143", "145", "146", "Item_33", "Item_4", "Item_5" };

        //        if (type == "F")
        //        {
        //            for (int i = 0; i < fund.Count; i++)
        //            {
        //                form.Items.Item(fund[i]).SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 2, BoModeVisualBehavior.mvb_True);
        //                form.Items.Item(fund[i]).SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 3, BoModeVisualBehavior.mvb_True);
        //                if ((fund[i] == "15" || fund[i] == "16") && (form.Items.Item(fund[i]).BackColor == Color.FromKnownColor(KnownColor.White).ToArgb()))
        //                {
        //                    form.Items.Item(fund[i]).BackColor = Color.Pink.R | (Color.Pink.G << 8) | (Color.Pink.B << 16);
        //                }
        //            }
        //            for (int i = 0; i < grant.Count; i++)
        //            {
        //                form.Items.Item(grant[i]).SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 2, BoModeVisualBehavior.mvb_False);
        //                form.Items.Item(grant[i]).SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 3, BoModeVisualBehavior.mvb_False);

        //                if ((grant[i] == "105" || grant[i] == "106"))
        //                {
        //                    form.Items.Item(grant[i]).BackColor = Color.FromKnownColor(KnownColor.White).ToArgb();
        //                }
        //            }
        //            for (int i = 0; i < pledge.Count; i++)
        //            {
        //                form.Items.Item(pledge[i]).SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 2, BoModeVisualBehavior.mvb_False);
        //                form.Items.Item(pledge[i]).SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 3, BoModeVisualBehavior.mvb_False);

        //                if (pledge[i] == "143")
        //                {
        //                    form.Items.Item(pledge[i]).BackColor = Color.FromKnownColor(KnownColor.White).ToArgb();
        //                }

        //            }
        //        }
        //        else if (type == "G")
        //        {
        //            for (int i = 0; i < fund.Count; i++)
        //            {
        //                form.Items.Item(fund[i]).SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 2, BoModeVisualBehavior.mvb_True);
        //                form.Items.Item(fund[i]).SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 3, BoModeVisualBehavior.mvb_True);

        //                if ((fund[i] == "15" || fund[i] == "16") && (form.Items.Item(fund[i]).BackColor == Color.FromKnownColor(KnownColor.White).ToArgb()))
        //                {
        //                    form.Items.Item(fund[i]).BackColor = Color.Pink.R | (Color.Pink.G << 8) | (Color.Pink.B << 16);
        //                }
        //            }
        //            for (int i = 0; i < grant.Count; i++)
        //            {
        //                form.Items.Item(grant[i]).SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 2, BoModeVisualBehavior.mvb_True);
        //                form.Items.Item(grant[i]).SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 3, BoModeVisualBehavior.mvb_True);

        //                if ((grant[i] == "105" || grant[i] == "106") && (form.Items.Item(grant[i]).BackColor == Color.FromKnownColor(KnownColor.White).ToArgb()))
        //                {
        //                    form.Items.Item(grant[i]).BackColor = Color.Pink.R | (Color.Pink.G << 8) | (Color.Pink.B << 16);
        //                }
        //            }
        //            for (int i = 0; i < pledge.Count; i++)
        //            {
        //                form.Items.Item(pledge[i]).SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 2, BoModeVisualBehavior.mvb_False);
        //                form.Items.Item(pledge[i]).SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 3, BoModeVisualBehavior.mvb_False);

        //                if (pledge[i] == "143")
        //                {
        //                    form.Items.Item(pledge[i]).BackColor = Color.FromKnownColor(KnownColor.White).ToArgb();
        //                }
        //            }
        //        }
        //        else if (type == "P")
        //        {
        //            for (int i = 0; i < fund.Count; i++)
        //            {
        //                form.Items.Item(fund[i]).SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 2, BoModeVisualBehavior.mvb_True);
        //                form.Items.Item(fund[i]).SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 3, BoModeVisualBehavior.mvb_True);

        //                if ((fund[i] == "15" || fund[i] == "16") && (form.Items.Item(fund[i]).BackColor == Color.FromKnownColor(KnownColor.White).ToArgb()))
        //                {
        //                    form.Items.Item(fund[i]).BackColor = Color.Pink.R | (Color.Pink.G << 8) | (Color.Pink.B << 16);
        //                }
        //            }
        //            for (int i = 0; i < grant.Count; i++)
        //            {
        //                form.Items.Item(grant[i]).SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 2, BoModeVisualBehavior.mvb_False);
        //                form.Items.Item(grant[i]).SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 3, BoModeVisualBehavior.mvb_False);

        //                if ((grant[i] == "105" || grant[i] == "106"))
        //                {
        //                    form.Items.Item(grant[i]).BackColor = Color.FromKnownColor(KnownColor.White).ToArgb();
        //                }
        //            }
        //            for (int i = 0; i < pledge.Count; i++)
        //            {
        //                form.Items.Item(pledge[i]).SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 2, BoModeVisualBehavior.mvb_True);
        //                form.Items.Item(pledge[i]).SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 3, BoModeVisualBehavior.mvb_True);

        //                if (pledge[i] == "143" && (form.Items.Item(pledge[i]).BackColor == Color.FromKnownColor(KnownColor.White).ToArgb()))
        //                {
        //                    form.Items.Item(pledge[i]).BackColor = Color.Pink.R | (Color.Pink.G << 8) | (Color.Pink.B << 16);
        //                }
        //            }
        //        }
        //        form.Freeze(false);
        //    }
        //    catch (Exception ex)
        //    {
        //        form.Freeze(false);
        //    }

        //}

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
                //Form_Obj.Set_ReadOnly(form, Form_Obj.UDO_Info);
                //form.DataSources.UserDataSources.Item("27").Value = "0";
                //form.DataSources.UserDataSources.Item("172").Value = "0";
                string UDO_Code = form.DataSources.DBDataSources.Item(0).GetValue("Code", 0);
                Utility.Fill_Relation_Grids(company, form, UDO_Code, Relations_Grid_IDs);
            }
            if (SBO_Application.Forms.ActiveForm.Mode != BoFormMode.fm_FIND_MODE)
            {
                Form_Obj.Set_Fields(SBO_Application.Forms.ActiveForm);
            }
            if (SBO_Application.Forms.ActiveForm.Mode == BoFormMode.fm_FIND_MODE)
            {
                Form form = SBO_Application.Forms.ActiveForm;
                Form_Obj.UnSet_Mondatory_Fields_Color(SBO_Application.Forms.ActiveForm);
                DisableMatrixButtons(form);
            }

        }

        private static void DisableMatrixButtons(Form form)
        {
            List<string> GridBtnsIds = new List<string>() { "312", "311", "135", "134", "138", "137", "181", "180", "161", "160", "154", "153", "157", "156", "111", "110", "116", "115", "119", "118", "148", "147", "151", "150" };
            for (int i = 0; i < GridBtnsIds.Count; i++)
            {
                form.Items.Item(GridBtnsIds[i]).SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 4, BoModeVisualBehavior.mvb_False);

            }
        }

        private static void Add_Attachment(ItemEvent pVal)
        {
            Form form = SBO_Application.Forms.Item(pVal.FormUID);
            ST.Helper.Dialog.BrowseFile BF = new Helper.Dialog.BrowseFile(Helper.Dialog.DialogType.OpenFile);
            BF.ShowDialog();
            if (BF.FileName == "")
            {
                return;
            }
            form.Freeze(true);
            DBDataSource DS_Attachment = form.DataSources.DBDataSources.Item("@ST_FUND_RULE_ATT");
            Matrix Mat_Add = (Matrix)form.Items.Item("63").Specific;
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
            Mat_Add.LoadFromDataSource();
            Mat_Add.AutoResizeColumns();
            form.Freeze(false);
        }
        private static void Remove_Attachment(ItemEvent pVal)
        {
            Form form = SBO_Application.Forms.Item(pVal.FormUID);
            Form_Obj.Remove_Matrix_Row(form, "63");
        }
        private static void Open_Attachment(ItemEvent pVal)
        {
            Form form = SBO_Application.Forms.Item(pVal.FormUID);
            Matrix Mat = (Matrix)form.Items.Item("63").Specific;
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
        private static void Choose_Opportunity_ID(ItemEvent pVal)
        {
            SAPbouiCOM.Form form = SBO_Application.Forms.Item(pVal.FormUID);
            ChooseFromListEvent Chos_Event = (ChooseFromListEvent)pVal;
            string Code = "";
            if ((Chos_Event.SelectedObjects == null ? false : Chos_Event.SelectedObjects.Rows.Count != 0))
            {
                Code = Chos_Event.SelectedObjects.GetValue("OpprId", 0).ToString();
                form.DataSources.DBDataSources.Item(0).SetValue("U_ST_WG_OPPORTUNITY_ID", 0, Code);
                form.DataSources.DBDataSources.Item(0).SetValue("U_ST_WG_NAME", 0, Chos_Event.SelectedObjects.GetValue("CardName", 0).ToString());

            }
        }
        private static void Choose_Opportunity_Name(ItemEvent pVal)
        {
            SAPbouiCOM.Form form = SBO_Application.Forms.Item(pVal.FormUID);
            ChooseFromListEvent Chos_Event = (ChooseFromListEvent)pVal;
            string Code = "";
            if ((Chos_Event.SelectedObjects == null ? false : Chos_Event.SelectedObjects.Rows.Count != 0))
            {
                Code = Chos_Event.SelectedObjects.GetValue("CardName", 0).ToString();
                form.DataSources.DBDataSources.Item(0).SetValue("U_ST_WG_NAME", 0, Code);
                form.DataSources.DBDataSources.Item(0).SetValue("U_ST_WG_OPPORTUNITY_ID", 0, Chos_Event.SelectedObjects.GetValue("OpprId", 0).ToString());
            }
        }
        private static void Add_Patients_Line(ItemEvent pVal, string Line_DataSource_Table)
        {
            Form form = SBO_Application.Forms.Item(pVal.FormUID);
            DataTable DT_Orphans_Details = form.DataSources.DataTables.Item("Patients_Details");
            form.Freeze(true);
            DBDataSource DS_Lines = form.DataSources.DBDataSources.Item(Line_DataSource_Table);
            Matrix Mat_Lines = (Matrix)form.Items.Item("112").Specific;
            Mat_Lines.FlushToDataSource();
            int Count = DS_Lines.Size;
            if (Count == 1)
            {
                if (DS_Lines.GetValue("U_ST_PATIENT_CODE", Count - 1) != "")
                {
                    DS_Lines.InsertRecord(Count);
                }
                else
                {
                    Count = 0;

                    DS_Lines.InsertRecord(Count);
                    Mat_Lines.LoadFromDataSource();
                    Mat_Lines.DeleteRow(1);
                    Mat_Lines.FlushToDataSource();
                    Mat_Lines.LoadFromDataSource();
                }
            }
            else
            {
                DS_Lines.InsertRecord(Count);
            }
            if (DS_Lines.Size > DT_Orphans_Details.Rows.Count)
            {
                DT_Orphans_Details.Rows.Add();
            }
            Mat_Lines.LoadFromDataSource();
            //Set_Default_Value_Before_Adding(form);

            if (form.Mode == BoFormMode.fm_OK_MODE)
            {
                form.Mode = BoFormMode.fm_UPDATE_MODE;
            }
            form.Freeze(false);

        }

        private static void Remove_Patients_Selected_Lines(ItemEvent pVal)
        {
            Form form = SBO_Application.Forms.Item(pVal.FormUID);
            DataTable DT_Orphans_Details = form.DataSources.DataTables.Item("Patients_Details");
            Matrix Mat = (Matrix)form.Items.Item("112").Specific;
            int Count = Mat.RowCount;
            for (int i = Count; i > 0; i--)
            {
                //CheckBox Chk_Selected = (CheckBox)Mat.GetCellSpecific("SELECTED", i);
                CheckBox Chk_Selected = (CheckBox)Mat.Columns.Item("SELECTED").Cells.Item(i).Specific;
                if (Chk_Selected.Checked)
                {
                    Mat.DeleteRow(i);
                    DT_Orphans_Details.Rows.Remove(i - 1);
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
        private static void Patients_choose_From_List(ItemEvent pVal, string Line_DataSource_Table)
        {
            SAPbouiCOM.Form form = Loader.SBO_Application.Forms.Item(pVal.FormUID);
            SAPbouiCOM.ChooseFromListEvent Choos_Event = (SAPbouiCOM.ChooseFromListEvent)pVal;

            if (Choos_Event.SelectedObjects == null || Choos_Event.SelectedObjects.Rows.Count == 0)
            {
                return;
            }
            Matrix Mat = (Matrix)form.Items.Item("112").Specific;
            DataTable DT_Patients_Details = form.DataSources.DataTables.Item("Patients_Details");

            string Code = Choos_Event.SelectedObjects.GetValue("Code", 0).ToString();
            int Index = pVal.Row - 1;
            form.DataSources.DBDataSources.Item(Line_DataSource_Table).SetValue("U_ST_PATIENT_CODE", Index, Code);


            Set_Patient_Data(form, DT_Patients_Details, Index, Line_DataSource_Table);
            Mat.LoadFromDataSource();
            Mat.AutoResizeColumns();
            if (form.Mode == BoFormMode.fm_OK_MODE)
            {
                form.Mode = BoFormMode.fm_UPDATE_MODE;
            }
        }
        internal static void Set_Patient_Data(Form form, DataTable DT_Patient_Details, int DT_Row_Index, string Line_DataSource_Table)
        {
            string Code = form.DataSources.DBDataSources.Item(Line_DataSource_Table).GetValue("U_ST_PATIENT_CODE", DT_Row_Index);
            string SQL = $@"SELECT T0.U_ST_FULL_ARABIC_NAME, T0.U_ST_GENDER, T0.U_ST_DATE_OF_BIRTH, T0.U_ST_NATIONAL_ID
FROM ""@ST_PATIENTS_CARD""  T0 WHERE T0.""Code"" ='{Code}'";
            Recordset RC = Helper.Utility.Execute_Recordset_Query(company, SQL);
            string dob = string.Empty;
            string tempDate = RC.Fields.Item("U_ST_DATE_OF_BIRTH").Value.ToString();
            if (!string.IsNullOrEmpty(tempDate))
            {
                DateTime temp = Convert.ToDateTime(tempDate);
                dob = temp.ToString("dd/MM/yyyy");
            }
            form.DataSources.DBDataSources.Item(Line_DataSource_Table).SetValue("U_ST_NAME", DT_Row_Index, RC.Fields.Item("U_ST_FULL_ARABIC_NAME").Value.ToString());
            form.DataSources.DBDataSources.Item(Line_DataSource_Table).SetValue("U_ST_GENDER", DT_Row_Index, RC.Fields.Item("U_ST_GENDER").Value.ToString());
            form.DataSources.DBDataSources.Item(Line_DataSource_Table).SetValue("U_ST_DATE_OF_BIRTH", DT_Row_Index, dob);
            form.DataSources.DBDataSources.Item(Line_DataSource_Table).SetValue("U_ST_NATIONAL_ID", DT_Row_Index, RC.Fields.Item("U_ST_NATIONAL_ID").Value.ToString());
        }
        internal static void Add_Wish_Line(ItemEvent pVal, string Line_DataSource_Table)
        {
            Form form = SBO_Application.Forms.Item(pVal.FormUID);
            DataTable DT = form.DataSources.DataTables.Item("Wish_Details");
            form.Freeze(true);
            DBDataSource DS_Lines = form.DataSources.DBDataSources.Item(Line_DataSource_Table);
            Matrix Mat_Lines = (Matrix)form.Items.Item("117").Specific;
            Mat_Lines.FlushToDataSource();
            int Count = DS_Lines.Size;
            if (Count == 1)
            {
                if (DS_Lines.GetValue("U_ST_WISH_CODE", Count - 1) != "")
                {
                    DS_Lines.InsertRecord(Count);
                }
                else
                {
                    Count = 0;

                    DS_Lines.InsertRecord(Count);
                    Mat_Lines.LoadFromDataSource();
                    Mat_Lines.DeleteRow(1);
                    Mat_Lines.FlushToDataSource();
                    Mat_Lines.LoadFromDataSource();
                }
            }
            else
            {
                DS_Lines.InsertRecord(Count);
            }
            if (DS_Lines.Size > DT.Rows.Count)
            {
                DT.Rows.Add();
            }
            Mat_Lines.LoadFromDataSource();
            //Set_Default_Value_Before_Adding(form);

            if (form.Mode == BoFormMode.fm_OK_MODE)
            {
                form.Mode = BoFormMode.fm_UPDATE_MODE;
            }
            form.Freeze(false);

        }

        internal static void Remove_Wish_Selected_Lines(ItemEvent pVal)
        {
            Form form = SBO_Application.Forms.Item(pVal.FormUID);
            DataTable DT_Orphans_Details = form.DataSources.DataTables.Item("Wish_Details");
            Matrix Mat = (Matrix)form.Items.Item("117").Specific;
            int Count = Mat.RowCount;
            for (int i = Count; i > 0; i--)
            {
                //CheckBox Chk_Selected = (CheckBox)Mat.GetCellSpecific("SELECTED", i);
                CheckBox Chk_Selected = (CheckBox)Mat.Columns.Item("SELECTED").Cells.Item(i).Specific;
                if (Chk_Selected.Checked)
                {
                    Mat.DeleteRow(i);
                    DT_Orphans_Details.Rows.Remove(i - 1);
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

        private static void Wish_choose_From_List(ItemEvent pVal, string Line_DataSource_Table)
        {
            SAPbouiCOM.Form form = Loader.SBO_Application.Forms.Item(pVal.FormUID);
            SAPbouiCOM.ChooseFromListEvent Choos_Event = (SAPbouiCOM.ChooseFromListEvent)pVal;

            if (Choos_Event.SelectedObjects == null || Choos_Event.SelectedObjects.Rows.Count == 0)
            {
                return;
            }
            Matrix Mat = (Matrix)form.Items.Item("117").Specific;
            DataTable DT_Wish_Details = form.DataSources.DataTables.Item("Wish_Details");

            string Code = Choos_Event.SelectedObjects.GetValue("Code", 0).ToString();
            int Index = pVal.Row - 1;
            form.DataSources.DBDataSources.Item(Line_DataSource_Table).SetValue("U_ST_WISH_CODE", Index, Code);
            Set_Wish_Data(form, DT_Wish_Details, Index, Line_DataSource_Table);
            Mat.LoadFromDataSource();
            Mat.AutoResizeColumns();
        }
        internal static void Set_Wish_Data(Form form, DataTable DT_Orphans_Details, int DT_Row_Index, string Line_DataSource_Table)
        {
            string Code = form.DataSources.DBDataSources.Item(Line_DataSource_Table).GetValue("U_ST_WISH_CODE", DT_Row_Index);
            string SQL = $@"SELECT T0.U_ST_TYPE, T0.U_ST_SUB_TYPE, T0.""U_ST_REMARKS"" FROM ""@ST_DREAMS_COME_TRUE""  T0 WHERE T0.""Code"" = '{Code}'";
            Recordset RC = Helper.Utility.Execute_Recordset_Query(company, SQL);
            form.DataSources.DBDataSources.Item(Line_DataSource_Table).SetValue("U_ST_TYPE", DT_Row_Index, RC.Fields.Item("U_ST_TYPE").Value.ToString());
            form.DataSources.DBDataSources.Item(Line_DataSource_Table).SetValue("U_ST_SUB_TYPE", DT_Row_Index, RC.Fields.Item("U_ST_SUB_TYPE").Value.ToString());
            form.DataSources.DBDataSources.Item(Line_DataSource_Table).SetValue("U_ST_REMARKS", DT_Row_Index, RC.Fields.Item("U_ST_REMARKS").Value.ToString());
        }

        internal static void Add_Orphans_Line(ItemEvent pVal, string Line_DataSource_Table)
        {
            Form form = SBO_Application.Forms.Item(pVal.FormUID);
            DataTable DT_Orphans_Details = form.DataSources.DataTables.Item("Orphan_Details");
            form.Freeze(true);
            DBDataSource DS_Lines = form.DataSources.DBDataSources.Item(Line_DataSource_Table);
            Matrix Mat_Lines = (Matrix)form.Items.Item("120").Specific;
            Mat_Lines.FlushToDataSource();
            int Count = DS_Lines.Size;
            if (Count == 1)
            {
                if (DS_Lines.GetValue("U_ST_ORPHAN_CODE", Count - 1) != "")
                {
                    DS_Lines.InsertRecord(Count);
                }
                else
                {
                    Count = 0;

                    DS_Lines.InsertRecord(Count);
                    Mat_Lines.LoadFromDataSource();
                    Mat_Lines.DeleteRow(1);
                    Mat_Lines.FlushToDataSource();
                    Mat_Lines.LoadFromDataSource();
                }
            }
            else
            {
                DS_Lines.InsertRecord(Count);
            }
            if (DS_Lines.Size > DT_Orphans_Details.Rows.Count)
            {
                DT_Orphans_Details.Rows.Add();
            }
            Mat_Lines.LoadFromDataSource();
            //Set_Default_Value_Before_Adding(form);

            if (form.Mode == BoFormMode.fm_OK_MODE)
            {
                form.Mode = BoFormMode.fm_UPDATE_MODE;
            }
            form.Freeze(false);

        }

        internal static void Remove_Orphans_Selected_Lines(ItemEvent pVal)
        {
            Form form = SBO_Application.Forms.Item(pVal.FormUID);
            DataTable DT_Orphans_Details = form.DataSources.DataTables.Item("Orphan_Details");
            Matrix Mat = (Matrix)form.Items.Item("120").Specific;
            int Count = Mat.RowCount;
            for (int i = Count; i > 0; i--)
            {
                //CheckBox Chk_Selected = (CheckBox)Mat.GetCellSpecific("SELECTED", i);
                CheckBox Chk_Selected = (CheckBox)Mat.Columns.Item("SELECTED").Cells.Item(i).Specific;
                if (Chk_Selected.Checked)
                {
                    Mat.DeleteRow(i);
                    DT_Orphans_Details.Rows.Remove(i - 1);
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
        internal static void Orphans_Choos_From_List(ItemEvent pVal, string Line_DataSource_Table)
        {
            SAPbouiCOM.Form form = Loader.SBO_Application.Forms.Item(pVal.FormUID);
            SAPbouiCOM.ChooseFromListEvent Choos_Event = (SAPbouiCOM.ChooseFromListEvent)pVal;

            if (Choos_Event.SelectedObjects == null || Choos_Event.SelectedObjects.Rows.Count == 0)
            {
                return;
            }
            Matrix Mat = (Matrix)form.Items.Item("120").Specific;
            DataTable DT_Orphans_Details = form.DataSources.DataTables.Item("Orphan_Details");

            string Code = Choos_Event.SelectedObjects.GetValue("Code", 0).ToString();
            int Index = pVal.Row - 1;
            form.DataSources.DBDataSources.Item(Line_DataSource_Table).SetValue("U_ST_ORPHAN_CODE", Index, Code);
            Set_Orphan_Data(form, DT_Orphans_Details, Index, Line_DataSource_Table);
            Mat.LoadFromDataSource();

            Mat.AutoResizeColumns();
        }
        internal static void Set_Orphan_Data(Form form, DataTable DT_Orphans_Details, int DT_Row_Index, string Line_DataSource_Table)
        {
            string Code = form.DataSources.DBDataSources.Item(Line_DataSource_Table).GetValue("U_ST_ORPHAN_CODE", DT_Row_Index);
            string SQL = $@"SELECT T0.""U_ST_FULL_NAME_AR"", T0.U_ST_GENDER, T0.U_ST_DATE_OF_BIRTH, T0.U_ST_NATIONAL_ID FROM ""@ST_CCI_INDIV_CARD"" T0 WHERE T0.""Code"" = '{Code}'";
            Recordset RC = Helper.Utility.Execute_Recordset_Query(company, SQL);
            string dob = string.Empty;
            string tempDate = RC.Fields.Item("U_ST_DATE_OF_BIRTH").Value.ToString();
            if (!string.IsNullOrEmpty(tempDate))
            {
                DateTime temp = Convert.ToDateTime(tempDate);
                dob = temp.ToString("dd/MM/yyyy");
            }
            form.DataSources.DBDataSources.Item(Line_DataSource_Table).SetValue("U_ST_NAME", DT_Row_Index, RC.Fields.Item("U_ST_FULL_NAME_AR").Value.ToString());
            form.DataSources.DBDataSources.Item(Line_DataSource_Table).SetValue("U_ST_GENDER", DT_Row_Index, RC.Fields.Item("U_ST_GENDER").Value.ToString());
            form.DataSources.DBDataSources.Item(Line_DataSource_Table).SetValue("U_ST_DATE_OF_BIRTH", DT_Row_Index, dob);
            form.DataSources.DBDataSources.Item(Line_DataSource_Table).SetValue("U_ST_NATIONAL_ID", DT_Row_Index, RC.Fields.Item("U_ST_NATIONAL_ID").Value.ToString());
        }

        internal static void Add_Participants_Line(ItemEvent pVal, string Line_DataSource_Table)
        {
            Form form = SBO_Application.Forms.Item(pVal.FormUID);
            DataTable DT_Orphans_Details = form.DataSources.DataTables.Item("Participants_Details");
            form.Freeze(true);
            DBDataSource DS_Lines = form.DataSources.DBDataSources.Item(Line_DataSource_Table);
            Matrix Mat_Lines = (Matrix)form.Items.Item("149").Specific;
            Mat_Lines.FlushToDataSource();
            int Count = DS_Lines.Size;
            if (Count == 1)
            {
                if (DS_Lines.GetValue("Code", Count - 1) != "")
                {
                    DS_Lines.InsertRecord(Count);
                }
                else
                {
                    Count = 0;

                    DS_Lines.InsertRecord(Count);
                    Mat_Lines.LoadFromDataSource();
                    Mat_Lines.DeleteRow(1);
                    Mat_Lines.FlushToDataSource();
                    Mat_Lines.LoadFromDataSource();
                }
            }
            else
            {
                DS_Lines.InsertRecord(Count);
            }
            if (DS_Lines.Size > DT_Orphans_Details.Rows.Count)
            {
                DT_Orphans_Details.Rows.Add();
            }
            Mat_Lines.LoadFromDataSource();
            //Set_Default_Value_Before_Adding(form);

            if (form.Mode == BoFormMode.fm_OK_MODE)
            {
                form.Mode = BoFormMode.fm_UPDATE_MODE;
            }
            form.Freeze(false);

        }

        internal static void Remove_Participants_Selected_Lines(ItemEvent pVal)
        {
            Form form = SBO_Application.Forms.Item(pVal.FormUID);
            DataTable DT_Orphans_Details = form.DataSources.DataTables.Item("Participants_Details");
            Matrix Mat = (Matrix)form.Items.Item("149").Specific;
            int Count = Mat.RowCount;
            for (int i = Count; i > 0; i--)
            {
                //CheckBox Chk_Selected = (CheckBox)Mat.GetCellSpecific("SELECTED", i);
                CheckBox Chk_Selected = (CheckBox)Mat.Columns.Item("SELECTED").Cells.Item(i).Specific;
                if (Chk_Selected.Checked)
                {
                    Mat.DeleteRow(i);
                    DT_Orphans_Details.Rows.Remove(i - 1);
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
        internal static void Participants_Choos_From_List(ItemEvent pVal, string Line_DataSource_Table)
        {
            SAPbouiCOM.Form form = Loader.SBO_Application.Forms.Item(pVal.FormUID);
            SAPbouiCOM.ChooseFromListEvent Choos_Event = (SAPbouiCOM.ChooseFromListEvent)pVal;

            if (Choos_Event.SelectedObjects == null || Choos_Event.SelectedObjects.Rows.Count == 0)
            {
                return;
            }
            Matrix Mat = (Matrix)form.Items.Item("149").Specific;
            DataTable DT_Orphans_Details = form.DataSources.DataTables.Item("Participants_Details");

            string Code = Choos_Event.SelectedObjects.GetValue("Code", 0).ToString();
            int Index = pVal.Row - 1;
            form.DataSources.DBDataSources.Item(Line_DataSource_Table).SetValue("U_ST_PARTICIPANT_CODE", Index, Code);
            Set_Participants_Data(form, DT_Orphans_Details, Index, Line_DataSource_Table);
            Mat.LoadFromDataSource();

            Mat.AutoResizeColumns();
        }
        internal static void Set_Participants_Data(Form form, DataTable DT_Orphans_Details, int DT_Row_Index, string Line_DataSource_Table)
        {
            string Code = form.DataSources.DBDataSources.Item(Line_DataSource_Table).GetValue("U_ST_PARTICIPANT_CODE", DT_Row_Index);
            string SQL = $@"SELECT T0.""U_ST_FULL_NAME_AR"" FROM ""@ST_FUND_INDIV_CARD"" T0 WHERE T0.""Code"" = '{Code}'";
            Recordset RC = Helper.Utility.Execute_Recordset_Query(company, SQL);
            form.DataSources.DBDataSources.Item(Line_DataSource_Table).SetValue("U_ST_NAME", DT_Row_Index, RC.Fields.Item("U_ST_FULL_NAME_AR").Value.ToString());
        }

        internal static void Add_Pledge_Line(ItemEvent pVal, string Line_DataSource_Table)
        {
            Form form = SBO_Application.Forms.Item(pVal.FormUID);
            DataTable DT_Orphans_Details = form.DataSources.DataTables.Item("Pledges_Details");
            form.Freeze(true);
            DBDataSource DS_Lines = form.DataSources.DBDataSources.Item(Line_DataSource_Table);
            Matrix Mat_Lines = (Matrix)form.Items.Item("152").Specific;
            Mat_Lines.FlushToDataSource();
            int Count = DS_Lines.Size;
            if (Count == 1)
            {
                if (DS_Lines.GetValue("U_ST_PATIENTS_CODE", Count - 1) != "")
                {
                    DS_Lines.InsertRecord(Count);
                }
                else
                {
                    Count = 0;

                    DS_Lines.InsertRecord(Count);
                    Mat_Lines.LoadFromDataSource();
                    Mat_Lines.DeleteRow(1);
                    Mat_Lines.FlushToDataSource();
                    Mat_Lines.LoadFromDataSource();
                }
            }
            else
            {
                DS_Lines.InsertRecord(Count);
            }
            if (DS_Lines.Size > DT_Orphans_Details.Rows.Count)
            {
                DT_Orphans_Details.Rows.Add();
            }
            Mat_Lines.LoadFromDataSource();
            //Set_Default_Value_Before_Adding(form);

            if (form.Mode == BoFormMode.fm_OK_MODE)
            {
                form.Mode = BoFormMode.fm_UPDATE_MODE;
            }
            form.Freeze(false);

        }

        internal static void Remove_Pledges_Selected_Lines(ItemEvent pVal)
        {
            Form form = SBO_Application.Forms.Item(pVal.FormUID);
            DataTable DT_Orphans_Details = form.DataSources.DataTables.Item("Pledges_Details");
            Matrix Mat = (Matrix)form.Items.Item("152").Specific;
            int Count = Mat.RowCount;
            for (int i = Count; i > 0; i--)
            {
                //CheckBox Chk_Selected = (CheckBox)Mat.GetCellSpecific("SELECTED", i);
                CheckBox Chk_Selected = (CheckBox)Mat.Columns.Item("SELECTED").Cells.Item(i).Specific;
                if (Chk_Selected.Checked)
                {
                    Mat.DeleteRow(i);
                    DT_Orphans_Details.Rows.Remove(i - 1);
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
        internal static void Pledges_Choos_From_List(ItemEvent pVal, string Line_DataSource_Table)
        {
            SAPbouiCOM.Form form = Loader.SBO_Application.Forms.Item(pVal.FormUID);
            SAPbouiCOM.ChooseFromListEvent Choos_Event = (SAPbouiCOM.ChooseFromListEvent)pVal;

            if (Choos_Event.SelectedObjects == null || Choos_Event.SelectedObjects.Rows.Count == 0)
            {
                return;
            }
            Matrix Mat = (Matrix)form.Items.Item("152").Specific;
            DataTable DT_Orphans_Details = form.DataSources.DataTables.Item("Pledges_Details");

            string Code = Choos_Event.SelectedObjects.GetValue("Code", 0).ToString();
            int Index = pVal.Row - 1;
            form.DataSources.DBDataSources.Item(Line_DataSource_Table).SetValue("U_ST_PATIENTS_CODE", Index, Code);
            Set_Pledges_Data(form, DT_Orphans_Details, Index, Line_DataSource_Table);
            Mat.LoadFromDataSource();

            Mat.AutoResizeColumns();
        }
        internal static void Set_Pledges_Data(Form form, DataTable DT_Orphans_Details, int DT_Row_Index, string Line_DataSource_Table)
        {
            string Code = form.DataSources.DBDataSources.Item(Line_DataSource_Table).GetValue("U_ST_PATIENTS_CODE", DT_Row_Index);
            string SQL = $@"SELECT T0.""U_ST_FULL_NAME_AR"", T0.U_ST_GENDER, T0.U_ST_DATE_OF_BIRTH FROM ""@ST_FUND_INDIV_CARD"" T0 WHERE T0.""Code"" = '{Code}'";
            Recordset RC = Helper.Utility.Execute_Recordset_Query(company, SQL);
            string dob = string.Empty;
            string tempDate = RC.Fields.Item("U_ST_DATE_OF_BIRTH").Value.ToString();
            if (!string.IsNullOrEmpty(tempDate))
            {
                DateTime temp = Convert.ToDateTime(tempDate);
                dob = temp.ToString("dd/MM/yyyy");
            }
            form.DataSources.DBDataSources.Item(Line_DataSource_Table).SetValue("U_ST_NAME", DT_Row_Index, RC.Fields.Item("U_ST_FULL_NAME_AR").Value.ToString());
            form.DataSources.DBDataSources.Item(Line_DataSource_Table).SetValue("U_ST_GENDER", DT_Row_Index, RC.Fields.Item("U_ST_GENDER").Value.ToString());
            form.DataSources.DBDataSources.Item(Line_DataSource_Table).SetValue("U_ST_DATE_OF_BIRTH", DT_Row_Index, dob);

        }
        private static void Choose_Creator(ItemEvent pVal)
        {
            SAPbouiCOM.Form form = SBO_Application.Forms.Item(pVal.FormUID);
            ChooseFromListEvent Chos_Event = (ChooseFromListEvent)pVal;
            string Code = "";
            if ((Chos_Event.SelectedObjects == null ? false : Chos_Event.SelectedObjects.Rows.Count != 0))
            {
                Code = Chos_Event.SelectedObjects.GetValue("Code", 0).ToString();
                form.DataSources.DBDataSources.Item(0).SetValue("U_ST_PLEDGE_CREATOR", 0, Code);
            }
        }

        internal static void Add_Report_Line(ItemEvent pVal, string Line_DataSource_Table)
        {
            Form form = SBO_Application.Forms.Item(pVal.FormUID);
            DataTable DT_Orphans_Details = form.DataSources.DataTables.Item("Reports_Details");
            form.Freeze(true);
            DBDataSource DS_Lines = form.DataSources.DBDataSources.Item(Line_DataSource_Table);
            Matrix Mat_Lines = (Matrix)form.Items.Item("162").Specific;
            Mat_Lines.FlushToDataSource();
            int Count = DS_Lines.Size;
            if (Count == 1)
            {
                if (DT_Orphans_Details.GetValue("U_ST_REPORT_TYPE", Count - 1) != "")
                {
                    DS_Lines.InsertRecord(Count);
                }
                else
                {
                    Count = 0;

                    DS_Lines.InsertRecord(Count);
                    Mat_Lines.LoadFromDataSource();
                    Mat_Lines.DeleteRow(1);
                    Mat_Lines.FlushToDataSource();
                    Mat_Lines.LoadFromDataSource();
                }
            }
            else
            {
                DS_Lines.InsertRecord(Count);
            }
            if (DS_Lines.Size > DT_Orphans_Details.Rows.Count)
            {
                DT_Orphans_Details.Rows.Add();
            }
            Mat_Lines.LoadFromDataSource();
            //Set_Default_Value_Before_Adding(form);

            if (form.Mode == BoFormMode.fm_OK_MODE)
            {
                form.Mode = BoFormMode.fm_UPDATE_MODE;
            }
            form.Freeze(false);
        }

        internal static void Remove_Report_Selected_Lines(ItemEvent pVal)
        {
            Form form = SBO_Application.Forms.Item(pVal.FormUID);
            DataTable DT_Orphans_Details = form.DataSources.DataTables.Item("Reports_Details");
            Matrix Mat = (Matrix)form.Items.Item("162").Specific;
            int Count = Mat.RowCount;
            for (int i = Count; i > 0; i--)
            {
                //CheckBox Chk_Selected = (CheckBox)Mat.GetCellSpecific("SELECTED", i);
                CheckBox Chk_Selected = (CheckBox)Mat.Columns.Item("SELECTED").Cells.Item(i).Specific;
                if (Chk_Selected.Checked)
                {
                    Mat.DeleteRow(i);
                    DT_Orphans_Details.Rows.Remove(i - 1);
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

        internal static void Add_Nationality_Line(ItemEvent pVal, string Line_DataSource_Table)
        {
            Form form = SBO_Application.Forms.Item(pVal.FormUID);
            DataTable DT = form.DataSources.DataTables.Item("Nationality_Details");
            form.Freeze(true);
            DBDataSource DS_Lines = form.DataSources.DBDataSources.Item(Line_DataSource_Table);
            Matrix Mat_Lines = (Matrix)form.Items.Item("313").Specific;
            Mat_Lines.FlushToDataSource();
            int Count = DS_Lines.Size;
            if (Count == 1)
            {
                if (DS_Lines.GetValue("U_ST_COUNTRY_CODE", Count - 1) != "")
                {
                    DS_Lines.InsertRecord(Count);
                }
                else
                {
                    Count = 0;

                    DS_Lines.InsertRecord(Count);
                    Mat_Lines.LoadFromDataSource();
                    Mat_Lines.DeleteRow(1);
                    Mat_Lines.FlushToDataSource();
                    Mat_Lines.LoadFromDataSource();
                }
            }
            else
            {
                DS_Lines.InsertRecord(Count);
            }
            if (DS_Lines.Size > DT.Rows.Count)
            {
                DT.Rows.Add();
            }
            Mat_Lines.LoadFromDataSource();
            //Set_Default_Value_Before_Adding(form);

            if (form.Mode == BoFormMode.fm_OK_MODE)
            {
                form.Mode = BoFormMode.fm_UPDATE_MODE;
            }
            Mat_Lines.AutoResizeColumns();
            form.Freeze(false);

        }

        internal static void Remove_Nationality_Selected_Lines(ItemEvent pVal)
        {
            Form form = SBO_Application.Forms.Item(pVal.FormUID);
            DataTable DT_Orphans_Details = form.DataSources.DataTables.Item("Nationality_Details");
            Matrix Mat = (Matrix)form.Items.Item("313").Specific;
            int Count = Mat.RowCount;
            for (int i = Count; i > 0; i--)
            {
                //CheckBox Chk_Selected = (CheckBox)Mat.GetCellSpecific("SELECTED", i);
                CheckBox Chk_Selected = (CheckBox)Mat.Columns.Item("SELECTED").Cells.Item(i).Specific;
                if (Chk_Selected.Checked)
                {
                    Mat.DeleteRow(i);
                    DT_Orphans_Details.Rows.Remove(i - 1);
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

        private static void Nationality_choose_From_List(ItemEvent pVal, string Line_DataSource_Table)
        {
            SAPbouiCOM.Form form = Loader.SBO_Application.Forms.Item(pVal.FormUID);
            SAPbouiCOM.ChooseFromListEvent Choos_Event = (SAPbouiCOM.ChooseFromListEvent)pVal;

            if (Choos_Event.SelectedObjects == null || Choos_Event.SelectedObjects.Rows.Count == 0)
            {
                return;
            }
            Matrix Mat = (Matrix)form.Items.Item("313").Specific;
            DataTable DT_Wish_Details = form.DataSources.DataTables.Item("Nationality_Details");

            string Code = Choos_Event.SelectedObjects.GetValue("Code", 0).ToString();
            int Index = pVal.Row - 1;
            form.DataSources.DBDataSources.Item(Line_DataSource_Table).SetValue("U_ST_COUNTRY_CODE", Index, Code);
            Set_Nationality_Data(form, DT_Wish_Details, Index, Line_DataSource_Table);
            Mat.LoadFromDataSource();
            Mat.AutoResizeColumns();
        }
        internal static void Set_Nationality_Data(Form form, DataTable DT_Orphans_Details, int DT_Row_Index, string Line_DataSource_Table)
        {
            string Code = form.DataSources.DBDataSources.Item(Line_DataSource_Table).GetValue("U_ST_COUNTRY_CODE", DT_Row_Index);
            string SQL = $@"SELECT T0.""Name"" FROM OCRY  T0 WHERE T0.""Code"" = '{Code}'";
            Recordset RC = Helper.Utility.Execute_Recordset_Query(company, SQL);
            form.DataSources.DBDataSources.Item(Line_DataSource_Table).SetValue("U_ST_NAME", DT_Row_Index, RC.Fields.Item("Name").Value.ToString());

        }
        internal static void Add_Sub_Nationality_Line(ItemEvent pVal, string Line_DataSource_Table)
        {
            Form form = SBO_Application.Forms.Item(pVal.FormUID);
            DataTable DT_Orphans_Details = form.DataSources.DataTables.Item("Sub_Details");
            form.Freeze(true);
            DBDataSource DS_Lines = form.DataSources.DBDataSources.Item(Line_DataSource_Table);
            Matrix Mat_Lines = (Matrix)form.Items.Item("136").Specific;
            Mat_Lines.FlushToDataSource();
            int Count = DS_Lines.Size;
            if (Count == 1)
            {
                if (DT_Orphans_Details.GetValue("Name", Count - 1) != "")
                {
                    DS_Lines.InsertRecord(Count);
                }
                else
                {
                    Count = 0;

                    DS_Lines.InsertRecord(Count);
                    Mat_Lines.LoadFromDataSource();
                    Mat_Lines.DeleteRow(1);
                    Mat_Lines.FlushToDataSource();
                    Mat_Lines.LoadFromDataSource();
                }
            }
            else
            {
                DS_Lines.InsertRecord(Count);
            }
            if (DS_Lines.Size > DT_Orphans_Details.Rows.Count)
            {
                DT_Orphans_Details.Rows.Add();
            }
            Mat_Lines.LoadFromDataSource();
            //Set_Default_Value_Before_Adding(form);

            if (form.Mode == BoFormMode.fm_OK_MODE)
            {
                form.Mode = BoFormMode.fm_UPDATE_MODE;
            }
            form.Freeze(false);
        }

        internal static void Remove_Sub_Nationality_Selected_Lines(ItemEvent pVal)
        {
            Form form = SBO_Application.Forms.Item(pVal.FormUID);
            DataTable DT_Orphans_Details = form.DataSources.DataTables.Item("Sub_Details");
            Matrix Mat = (Matrix)form.Items.Item("136").Specific;
            int Count = Mat.RowCount;
            for (int i = Count; i > 0; i--)
            {
                //CheckBox Chk_Selected = (CheckBox)Mat.GetCellSpecific("SELECTED", i);
                CheckBox Chk_Selected = (CheckBox)Mat.Columns.Item("SELECTED").Cells.Item(i).Specific;
                if (Chk_Selected.Checked)
                {
                    Mat.DeleteRow(i);
                    DT_Orphans_Details.Rows.Remove(i - 1);
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

        internal static void Add_Support_Line(ItemEvent pVal, string Line_DataSource_Table)
        {
            Form form = SBO_Application.Forms.Item(pVal.FormUID);
            DataTable DT_Orphans_Details = form.DataSources.DataTables.Item("Support_Details");
            form.Freeze(true);
            DBDataSource DS_Lines = form.DataSources.DBDataSources.Item(Line_DataSource_Table);
            Matrix Mat_Lines = (Matrix)form.Items.Item("139").Specific;
            Mat_Lines.FlushToDataSource();
            int Count = DS_Lines.Size;
            if (Count == 1)
            {
                if (DT_Orphans_Details.GetValue("Name", Count - 1) != "")
                {
                    DS_Lines.InsertRecord(Count);
                }
                else
                {
                    Count = 0;

                    DS_Lines.InsertRecord(Count);
                    Mat_Lines.LoadFromDataSource();
                    Mat_Lines.DeleteRow(1);
                    Mat_Lines.FlushToDataSource();
                    Mat_Lines.LoadFromDataSource();
                }
            }
            else
            {
                DS_Lines.InsertRecord(Count);
            }
            if (DS_Lines.Size > DT_Orphans_Details.Rows.Count)
            {
                DT_Orphans_Details.Rows.Add();
            }
            Mat_Lines.LoadFromDataSource();
            //Set_Default_Value_Before_Adding(form);

            if (form.Mode == BoFormMode.fm_OK_MODE)
            {
                form.Mode = BoFormMode.fm_UPDATE_MODE;
            }
            form.Freeze(false);
        }

        internal static void Remove_Support_Selected_Lines(ItemEvent pVal)
        {
            Form form = SBO_Application.Forms.Item(pVal.FormUID);
            DataTable DT_Orphans_Details = form.DataSources.DataTables.Item("Support_Details");
            Matrix Mat = (Matrix)form.Items.Item("139").Specific;
            int Count = Mat.RowCount;
            for (int i = Count; i > 0; i--)
            {
                //CheckBox Chk_Selected = (CheckBox)Mat.GetCellSpecific("SELECTED", i);
                CheckBox Chk_Selected = (CheckBox)Mat.Columns.Item("SELECTED").Cells.Item(i).Specific;
                if (Chk_Selected.Checked)
                {
                    Mat.DeleteRow(i);
                    DT_Orphans_Details.Rows.Remove(i - 1);
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

        internal static void Add_Cancer_Line(ItemEvent pVal, string Line_DataSource_Table)
        {
            Form form = SBO_Application.Forms.Item(pVal.FormUID);
            DataTable DT_Orphans_Details = form.DataSources.DataTables.Item("Cancer_Details");
            form.Freeze(true);
            DBDataSource DS_Lines = form.DataSources.DBDataSources.Item(Line_DataSource_Table);
            Matrix Mat_Lines = (Matrix)form.Items.Item("182").Specific;
            Mat_Lines.FlushToDataSource();
            int Count = DS_Lines.Size;
            if (Count == 1)
            {
                if (DT_Orphans_Details.GetValue("Name", Count - 1) != "")
                {
                    DS_Lines.InsertRecord(Count);
                }
                else
                {
                    Count = 0;

                    DS_Lines.InsertRecord(Count);
                    Mat_Lines.LoadFromDataSource();
                    Mat_Lines.DeleteRow(1);
                    Mat_Lines.FlushToDataSource();
                    Mat_Lines.LoadFromDataSource();
                }
            }
            else
            {
                DS_Lines.InsertRecord(Count);
            }
            if (DS_Lines.Size > DT_Orphans_Details.Rows.Count)
            {
                DT_Orphans_Details.Rows.Add();
            }
            Mat_Lines.LoadFromDataSource();
            //Set_Default_Value_Before_Adding(form);

            if (form.Mode == BoFormMode.fm_OK_MODE)
            {
                form.Mode = BoFormMode.fm_UPDATE_MODE;
            }
            form.Freeze(false);
        }

        internal static void Remove_Cancer_Selected_Lines(ItemEvent pVal)
        {
            Form form = SBO_Application.Forms.Item(pVal.FormUID);
            DataTable DT_Orphans_Details = form.DataSources.DataTables.Item("Cancer_Details");
            Matrix Mat = (Matrix)form.Items.Item("182").Specific;
            int Count = Mat.RowCount;
            for (int i = Count; i > 0; i--)
            {
                //CheckBox Chk_Selected = (CheckBox)Mat.GetCellSpecific("SELECTED", i);
                CheckBox Chk_Selected = (CheckBox)Mat.Columns.Item("SELECTED").Cells.Item(i).Specific;
                if (Chk_Selected.Checked)
                {
                    Mat.DeleteRow(i);
                    DT_Orphans_Details.Rows.Remove(i - 1);
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

        internal static void Add_Line(ItemEvent pVal, string Line_DataSource_Table, string Datatable_Id, string Matrix_Id, string Code_Col, bool isDBTable)
        {
            Form form = SBO_Application.Forms.Item(pVal.FormUID);
            DataTable DT = form.DataSources.DataTables.Item(Datatable_Id);
            form.Freeze(true);
            DBDataSource DS_Lines = form.DataSources.DBDataSources.Item(Line_DataSource_Table);
            Matrix Mat_Lines = (Matrix)form.Items.Item(Matrix_Id).Specific;
            Mat_Lines.FlushToDataSource();
            int Count = DS_Lines.Size;
            if (isDBTable)
            {
                if (Count == 1)
                {
                    if (DS_Lines.GetValue(Code_Col, Count - 1) != "")
                    {
                        DS_Lines.InsertRecord(Count);
                    }
                    else
                    {
                        Count = 0;

                        DS_Lines.InsertRecord(Count);
                        Mat_Lines.LoadFromDataSource();
                        Mat_Lines.DeleteRow(1);
                        Mat_Lines.FlushToDataSource();
                        Mat_Lines.LoadFromDataSource();
                    }
                }
                else
                {
                    DS_Lines.InsertRecord(Count);
                }
                if (DS_Lines.Size > DT.Rows.Count)
                {
                    DT.Rows.Add();
                }
            }
            else
            {
                if (Count == 1)
                {
                    if (DT.GetValue(Code_Col, Count - 1) != "")
                    {
                        DS_Lines.InsertRecord(Count);
                    }
                    else
                    {
                        Count = 0;

                        DS_Lines.InsertRecord(Count);
                        Mat_Lines.LoadFromDataSource();
                        Mat_Lines.DeleteRow(1);
                        Mat_Lines.FlushToDataSource();
                        Mat_Lines.LoadFromDataSource();
                    }
                }
                else
                {
                    DS_Lines.InsertRecord(Count);
                }
                if (DS_Lines.Size > DT.Rows.Count)
                {
                    DT.Rows.Add();
                }

            }

            Mat_Lines.LoadFromDataSource();
            //Set_Default_Value_Before_Adding(form);

            if (form.Mode == BoFormMode.fm_OK_MODE)
            {
                form.Mode = BoFormMode.fm_UPDATE_MODE;
            }
            Mat_Lines.AutoResizeColumns();
            form.Freeze(false);

        }
        internal static void Remove_Selected_Lines(ItemEvent pVal, string Datatabe_Id, string Matrix_Id)
        {
            Form form = SBO_Application.Forms.Item(pVal.FormUID);
            DataTable DT_Orphans_Details = form.DataSources.DataTables.Item(Datatabe_Id);
            Matrix Mat = (Matrix)form.Items.Item(Matrix_Id).Specific;
            int Count = Mat.RowCount;
            for (int i = Count; i > 0; i--)
            {
                //CheckBox Chk_Selected = (CheckBox)Mat.GetCellSpecific("SELECTED", i);
                CheckBox Chk_Selected = (CheckBox)Mat.Columns.Item("SELECTED").Cells.Item(i).Specific;
                if (Chk_Selected.Checked)
                {
                    Mat.DeleteRow(i);
                    DT_Orphans_Details.Rows.Remove(i - 1);
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
        private static bool Validate_Age(out int rowIndex)
        {
            rowIndex = -5;
            Form form = SBO_Application.Forms.ActiveForm;
            Matrix Mat = (Matrix)form.Items.Item("126").Specific;
            if (Mat.RowCount == 0)
            {
                return true;
            }
            for (int i = Mat.RowCount; i > 0; i--)
            {
                EditText fromET = (EditText)Mat.Columns.Item("CODE").Cells.Item(i).Specific;
                EditText toET = (EditText)Mat.Columns.Item("TO").Cells.Item(i).Specific;
                //int from = Convert.ToInt32(fromET.Value);
                //int to = Convert.ToInt32(toET.Value);
                int from;
                int.TryParse( fromET.Value, out from);
                int to;
                int.TryParse(toET.Value, out to);
                if (from > to)
                {
                    rowIndex = i;
                    return false;
                }
                
            }
            return true;
        }
    }
}
