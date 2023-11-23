using SAPbobsCOM;
using SAPbouiCOM;
using ST.KHCF.Customization.Logic.Classes;
using ST.KHCF.Customization.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Drawing;
using System.IO;

namespace ST.KHCF.Customization.Forms.Fundraising
{
    internal class Frm_Won_Grants : Parent_Form
    {
        internal static SAPbobsCOM.Company company;
        internal static Application SBO_Application;
        internal static Parent_Form Form_Obj;
        internal static string[] Relations_Grid_IDs = new string[] { /*"68","50",*/ "202", "104", "206", "208", "210" };
        internal static string[] Potential_Items_List = new string[] { "37", "38", "7", "52", "14",/* "4",*/ "39", "60", "41", "42", "43", "44", "45", "46", "47", "48", "8", "15", "17", "24" };
        internal static string[] Won_Items_List = new string[] { "19", "26", "20", "27", "21", "28", "22", "29", /*"68", "50", "49", "51",*/ "66","65", "67"};
        internal static string[] Tab_Item_List = new string[] { "200", "100", "61", "62"};
        internal static DateTime prev = DateTime.Now;
        internal static DateTime startTime = DateTime.Now;
        private static List<Utility.Item_UI> Grids_List = new List<Utility.Item_UI>();

        //internal override string[] Get_Mondatory_Fields_List()
        //{
        //    List<string> Result = new List<string>();
        //    Result.AddRange(base.Get_Mondatory_Fields_List());
        //    Result.AddRange(new string[] { "15","24","25" });  
        //    return Result.ToArray();
        //}

        //internal override string[] Get_Fix_ReadOnly_Fields_List()
        //{
        //    List<string> Result = new List<string>();
        //    Result.AddRange(base.Get_Fix_ReadOnly_Fields_List());
        //    Result.AddRange(new string[] { "5"});

        //    return Result.ToArray();
        //}
        internal override void Initialize_Form(Form form)
        {
            base.Initialize_Form(form);
            Initializ_Won_Grants_Form(form);

            Matrix Mat = (Matrix)form.Items.Item("500").Specific;
            Mat.AutoResizeColumns();

        }

        internal static void Initializ_Won_Grants_Form(Form form)
        {
            KHCF_Logic_Utility.Set_Corporate_Fund_Chosse_From_List_Basic_Condition(form, "CFL_FUN_CORP");

            SAPbouiCOM.ChooseFromList CFL_Opp = form.ChooseFromLists.Item("CFL_SalesOpportunities_ID");
            SAPbouiCOM.Conditions CFL_Opp_Cons = CFL_Opp.GetConditions();
            SAPbouiCOM.Condition CFL_Opp_Con = CFL_Opp_Cons.Add();
            CFL_Opp_Con.Alias = "CardCode";
            CFL_Opp_Con.CondVal = "";
            CFL_Opp_Con.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL;

            CFL_Opp.SetConditions(CFL_Opp_Cons);


            form.Items.Item("201").TextStyle = 4;
            form.Items.Item("203").TextStyle = 4;
            form.Items.Item("205").TextStyle = 4;
            form.Items.Item("207").TextStyle = 4;
            form.Items.Item("209").TextStyle = 4;
            form.Items.Item("101").TextStyle = 4;
            form.Items.Item("103").TextStyle = 4;
            //form.Items.Item("49").TextStyle = 4;
            //form.Items.Item("51").TextStyle = 4;

            string SQL_Customer_Group = $@"SELECT ""GroupCode"" AS ""Code"", T0.""GroupName"" AS ""Name"" FROM OCRG T0 
WHERE  T0.""GroupType""  = 'C' AND T0.""U_ST_TYPE"" = 'C' AND U_ST_CUSTOMER_TYPE = 'F'";
            Helper.Utility.Fill_One_ComboBoxBySQL(company, form, "53", SQL_Customer_Group, true);

            string Fundraising_Department_ID = Configurations.Get_Fundraising_Department(company);
//            string SQL_Account_Manager = $@"SELECT T0.""SlpCode"" AS ""Code"", T0.""SlpName"" AS ""Name"" 
//FROM OSLP T0  INNER JOIN OHEM T1 ON T0.""SlpCode"" = T1.""salesPrson"" 
//WHERE T1.""dept"" in ({Fundraising_Department_ID})";
            string SQL_Account_Manager = $@"SELECT T0.""SlpCode"" AS ""Code"", T0.""SlpName"" AS ""Name"" FROM OSLP T0  ";
            Helper.Utility.Fill_One_ComboBoxBySQL(company, form, "38", SQL_Account_Manager, true);

            string SQL_Req = $@"SELECT T0.""Code"",T0.""Name""  FROM ""@ST_REPORTS_REQUIRE""  T0";
            string SQL_Freq = $@"SELECT T0.""Code"",T0.""Name""  FROM ""@ST_REPORTS_FREQUEN""  T0";
            Matrix Mat = (Matrix)form.Items.Item("500").Specific;
            Helper.Utility.FillMatrixComboBoxForSQL(company, Mat, "Req", SQL_Req);
            Helper.Utility.FillMatrixComboBoxForSQL(company, Mat, "Freq", SQL_Freq);

            string[] Buttons_Item_IDs = new string[] { "220", "221", "66", "65" };
            foreach (string OneItem in Buttons_Item_IDs)
            {
                form.Items.Item(OneItem).SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 4, BoModeVisualBehavior.mvb_False);
            }
            foreach (string OneItem in Relations_Grid_IDs)
            {
                form.Items.Item(OneItem).SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 4, BoModeVisualBehavior.mvb_False);
            }
            form.Items.Item("204").SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 4, BoModeVisualBehavior.mvb_False);
            form.Items.Item("67").SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 4, BoModeVisualBehavior.mvb_False);
           // form.Items.Item("500").SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 4, BoModeVisualBehavior.mvb_False);
            form.Items.Item("Item_2").SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 4, BoModeVisualBehavior.mvb_False);
            form.Items.Item("Item_3").SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 4, BoModeVisualBehavior.mvb_False);

            DataTable DT_Departments = form.DataSources.DataTables.Item("DT_Departments");
            Dictionary<string, string> Departments = new Dictionary<string, string>();
            Departments.Add("LID", "LID");
            Departments.Add("LCD", "LCD");
            Departments.Add("IDD", "IDD");
            Helper.Utility.FillGridForDictionary(DT_Departments, Departments, true);
            ((Grid)form.Items.Item("102").Specific).Columns.Item("Code").Visible = false;
            ((Grid)form.Items.Item("102").Specific).AutoResizeColumns();
            Grids_List = Utility.Get_Grids_UI(form, Relations_Grid_IDs);
            Grids_List.AddRange(Utility.Get_Grids_UI(form, new string[] { "204", "61", "67", "102", "207","209" }));

            form.Items.Item("1000").SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 4, BoModeVisualBehavior.mvb_False);
            //form.Items.Item("1000").SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 2, BoModeVisualBehavior.mvb_False);

            form.Items.Item("200").Click();
            // prev = Utility.Add_Time_Log("F", "", startTime, true);
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
                    form.Items.Item("65").SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 4, BoModeVisualBehavior.mvb_False);
                    form.Items.Item("66").SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 4, BoModeVisualBehavior.mvb_False);
                    form.Items.Item("70").SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 4, BoModeVisualBehavior.mvb_False);
                    form.Items.Item("71").SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 4, BoModeVisualBehavior.mvb_False);
                    form.Items.Item("72").SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 4, BoModeVisualBehavior.mvb_False);
                    form.Items.Item("73").SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 4, BoModeVisualBehavior.mvb_False);
                    form.Items.Item("Item_2").SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 4, BoModeVisualBehavior.mvb_False);
                    form.Items.Item("Item_3").SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 4, BoModeVisualBehavior.mvb_False);
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

                if (!BusinessObjectInfo.BeforeAction)
                {
                    //Form form = Loader.SBO_Application.Forms.Item(BusinessObjectInfo.FormUID);
                    string TableName = form.DataSources.DBDataSources.Item(0).TableName.Replace("@", "");
                    UDO_Definition KHCF_Object = Objects_Logic.All_UDO_Definition.FirstOrDefault(O => O.Table_Name == TableName);
                    if (Form_Obj.Set_ReadOnly(form, KHCF_Object))
                    {
                        // return;
                    }

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
                form.Freeze(true);
                Fill_All_Relation_Tables();
                Select_Sales_Oppertunities(form, "");
                Set_Status_Visiblity(form);
            }
            if (SBO_Application.Forms.ActiveForm.Mode != BoFormMode.fm_FIND_MODE)
            {
                Form_Obj.Set_Fields(SBO_Application.Forms.ActiveForm);
            }
            else
            {
                Form_Obj.UnSet_Mondatory_Fields_Color(SBO_Application.Forms.ActiveForm);
                Form form = SBO_Application.Forms.ActiveForm;
                form.Items.Item("Item_2").SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 4, BoModeVisualBehavior.mvb_False);
                form.Items.Item("Item_3").SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 4, BoModeVisualBehavior.mvb_False);

            }
        }

        private static void Fill_All_Relation_Tables()
        {
            Form form = SBO_Application.Forms.ActiveForm;
            string UDO_Code = form.DataSources.DBDataSources.Item(0).GetValue("Code", 0);

            Utility.Fill_Relation_Grids(company, form, UDO_Code, Relations_Grid_IDs);

            DataTable DT_Departments = form.DataSources.DataTables.Item("DT_Departments");
            Dictionary<string, string> Departments = new Dictionary<string, string>();
            Departments.Add("LID", "LID");
            Departments.Add("LCD", "LCD");
            Departments.Add("IDD", "IDD");
            Helper.Utility.FillGridForDictionary(DT_Departments, Departments, true);



        }

        private static void ADD_Update_UDO(BusinessObjectInfo businessObjectInfo)
        {
            Form form = SBO_Application.Forms.Item(businessObjectInfo.FormUID);
            System.Xml.XmlDocument XML_Doc = new System.Xml.XmlDocument();
            string XML_Text = businessObjectInfo.ObjectKey.Replace(" ", "").Replace("=", " = ");
            XML_Doc.LoadXml(XML_Text);

            string UDO_Code = XML_Doc.GetElementsByTagName("Code")[0].InnerText;

            Utility.Update_Relation_Table(company, form, UDO_Code, Relations_Grid_IDs, Form_Obj.KHCF_Object);
            Utility.Update_Relation_Table(company, form, UDO_Code, new string[] { "102" }, Form_Obj.KHCF_Object);

        }

        internal static void Form_Data_Load(BusinessObjectInfo businessObjectInfo)
        {
            Form form = SBO_Application.Forms.Item(businessObjectInfo.FormUID);
            form.Freeze(true);
            string UDO_Code = form.DataSources.DBDataSources.Item(0).GetValue("Code", 0);

            if (form.DataSources.DataTables.Item("ST_REPORTS_REQUIRE").Rows.Count == 0)
            {
                Utility.Fill_Relation_Grids(company, form, UDO_Code, Relations_Grid_IDs);
                DataTable DT_Departments = form.DataSources.DataTables.Item("DT_Departments");
                Dictionary<string, string> Departments = new Dictionary<string, string>();
                Departments.Add("LID", "LID");
                Departments.Add("LCD", "LCD");
                Departments.Add("IDD", "IDD");
                Helper.Utility.FillGridForDictionary(DT_Departments, Departments, true);
                ((Grid)form.Items.Item("102").Specific).AutoResizeColumns();
            }
            Utility.Load_All_Relation_Data(company, form, UDO_Code, Relations_Grid_IDs, Form_Obj.KHCF_Object);
            Utility.Load_All_Relation_Data(company, form, UDO_Code, new string[] {"102"}, Form_Obj.KHCF_Object);

            Set_Sales_Oppertunities_CFL_Condation(form);

            Select_Sales_Oppertunities(form, form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).GetValue("U_ST_OPPORTUNITY_ID", 0));

            Set_Status_Visiblity(form);
            form.Freeze(false);
            Form_Obj.Set_Fields(form);
            if (form.DataSources.DBDataSources.Item(0).GetValue("U_ST_STATUS", 0) == "W" || form.DataSources.DBDataSources.Item(0).GetValue("U_ST_STATUS", 0) == "L")
            {
                form.Items.Item("36").SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 3, BoModeVisualBehavior.mvb_False);

            }
            else
            {
                form.Items.Item("36").SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 3, BoModeVisualBehavior.mvb_True);
            }
            //Load_Orphans(form, "@ST_WON_GRANT_ORPH");

            //Load_Patients(form, "@ST_WON_GRANT_PATS");
            //Load_Wish(form, "@ST_WON_GRANT_WISH");

        }

        private static void Set_Sales_Oppertunities_CFL_Condation(Form form)
        {
            string Fun_Code = form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).GetValue("U_ST_ENTITY", 0);

            string SQL_BP = $@"SELECT T0.U_ST_BP_CODE FROM ""@ST_FUND_CORP_CARD""  T0 WHERE T0.""Code"" = '{Fun_Code}'";
            Recordset RC_BP = Helper.Utility.Execute_Recordset_Query(company, SQL_BP);
            string BP_CardCode= RC_BP.Fields.Item("U_ST_BP_CODE").Value.ToString();

            SAPbouiCOM.ChooseFromList CFL_Opp = form.ChooseFromLists.Item("CFL_SalesOpportunities_ID");
            SAPbouiCOM.Conditions CFL_Opp_Cons = CFL_Opp.GetConditions();
            int X = CFL_Opp_Cons.Count;
            SAPbouiCOM.Condition CFL_Opp_Con = CFL_Opp_Cons.Item(0);
            //CFL_Opp_Con.Alias = "CardCode";
            CFL_Opp_Con.CondVal = BP_CardCode;
           // CFL_Opp_Con.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL;

            CFL_Opp.SetConditions(CFL_Opp_Cons);

        }

        private static void Set_Status_Visiblity(Form form)
        {
            //form.Freeze(true);
            if (form.Mode == BoFormMode.fm_FIND_MODE)
            {
                Form_Obj.Change_Visiblity(form, Potential_Items_List, true);
                Form_Obj.Change_Visiblity(form, Won_Items_List, true);
                form.Freeze(false);
                return;
            }
            string Status = form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).GetValue("U_ST_STATUS", 0);
            if (Status == "N")
            {
                Form_Obj.Change_Visiblity(form, Potential_Items_List, false);                
                Form_Obj.Change_Visiblity(form, Won_Items_List, false);
                form.Items.Item("500").SetAutoManagedAttribute(BoAutoManagedAttr.ama_Visible, -1, BoModeVisualBehavior.mvb_False);
                form.Items.Item("Item_2").SetAutoManagedAttribute(BoAutoManagedAttr.ama_Visible, -1, BoModeVisualBehavior.mvb_False);
                form.Items.Item("Item_3").SetAutoManagedAttribute(BoAutoManagedAttr.ama_Visible, -1, BoModeVisualBehavior.mvb_False);
            }
            else if (Status == "P")
            {
                Form_Obj.Change_Visiblity(form, Potential_Items_List, true);
                Form_Obj.Change_Visiblity(form, Won_Items_List, false);
                form.Items.Item("500").SetAutoManagedAttribute(BoAutoManagedAttr.ama_Visible, -1, BoModeVisualBehavior.mvb_False);
                form.Items.Item("Item_2").SetAutoManagedAttribute(BoAutoManagedAttr.ama_Visible, -1, BoModeVisualBehavior.mvb_False);
                form.Items.Item("Item_3").SetAutoManagedAttribute(BoAutoManagedAttr.ama_Visible, -1, BoModeVisualBehavior.mvb_False);
            }
            else
            {
                Form_Obj.Change_Visiblity(form, Potential_Items_List, true);
                Form_Obj.Change_Visiblity(form, Won_Items_List, true);
                form.Items.Item("500").SetAutoManagedAttribute(BoAutoManagedAttr.ama_Visible, -1, BoModeVisualBehavior.mvb_True);
                form.Items.Item("Item_2").SetAutoManagedAttribute(BoAutoManagedAttr.ama_Visible, -1, BoModeVisualBehavior.mvb_True);
                form.Items.Item("Item_3").SetAutoManagedAttribute(BoAutoManagedAttr.ama_Visible, -1, BoModeVisualBehavior.mvb_True);
            }
            form.Freeze(false);
        }

        // internal static void Load_Wish(Form form, string Line_DataSource_Table)
        // {
        //     DataTable DT_Orphans_Details = form.DataSources.DataTables.Item("Wish_Details");
        //     DT_Orphans_Details.Rows.Clear();
        //     int Rows_Count = form.DataSources.DBDataSources.Item(Line_DataSource_Table).Size;
        //     DT_Orphans_Details.Rows.Add(Rows_Count);
        //     for (int i = 0; i < Rows_Count; i++)
        //     {
        //         Set_Wish_Data(form, DT_Orphans_Details, i, Line_DataSource_Table);
        //     }

        //     ((Matrix)form.Items.Item("33").Specific).LoadFromDataSource();
        //     ((Matrix)form.Items.Item("33").Specific).AutoResizeColumns();
        // }

        // internal static void Load_Orphans(Form form, string Line_DataSource_Table)
        // {
        //     DataTable DT_Orphans_Details = form.DataSources.DataTables.Item("Orphan_Details");
        //     DT_Orphans_Details.Rows.Clear();
        //     int Rows_Count = form.DataSources.DBDataSources.Item(Line_DataSource_Table).Size;
        //     DT_Orphans_Details.Rows.Add(Rows_Count);
        //     for (int i = 0; i < Rows_Count; i++)
        //     {
        //         Set_Orphan_Data(form, DT_Orphans_Details, i, Line_DataSource_Table);
        //     }

        //     ((Matrix)form.Items.Item("100").Specific).LoadFromDataSource();
        //     ((Matrix)form.Items.Item("100").Specific).AutoResizeColumns();
        // }
        // internal static void Load_Patients(Form form, string Line_DataSource_Table)
        // {
        //     DataTable DT_Orphans_Details = form.DataSources.DataTables.Item("Patients_Details");
        //     DT_Orphans_Details.Rows.Clear();
        //     int Rows_Count = form.DataSources.DBDataSources.Item(Line_DataSource_Table).Size;
        //     DT_Orphans_Details.Rows.Add(Rows_Count);
        //     for (int i = 0; i < Rows_Count; i++)
        //     {
        //         Set_Patient_Data(form, DT_Orphans_Details, i, Line_DataSource_Table);
        //     }

        //((Matrix)form.Items.Item("101").Specific).LoadFromDataSource();
        //     ((Matrix)form.Items.Item("101").Specific).AutoResizeColumns();
        // }
        //        internal static void Set_Wish_Data(Form form, DataTable DT_Orphans_Details, int DT_Row_Index, string Line_DataSource_Table)
        //        {
        //            string Code = form.DataSources.DBDataSources.Item(Line_DataSource_Table).GetValue("U_ST_WISH_CODE", DT_Row_Index);
        //            string SQL = $@"SELECT T0.U_ST_TYPE, T0.U_ST_SUB_TYPE, T0.""U_ST_REMARKS"" FROM ""@ST_DREAMS_COME_TRUE""  T0 WHERE T0.""Code"" = '{Code}'";
        //            Recordset RC = Helper.Utility.Execute_Recordset_Query(company, SQL);
        //            for (int J = 0; J < DT_Orphans_Details.Columns.Count; J++)
        //            {
        //                string Col_Name = DT_Orphans_Details.Columns.Item(J).Name;
        //                if (Col_Name == "SELECTED")
        //                {
        //                    continue;
        //                }
        //                if (Col_Name == "U_ST_WISH_CODE")
        //                {
        //                    DT_Orphans_Details.SetValue(Col_Name, DT_Row_Index, Code);
        //                    continue;
        //                }
        //                string X = RC.Fields.Item(Col_Name).Value.ToString();
        //                DT_Orphans_Details.SetValue(Col_Name, DT_Row_Index, RC.Fields.Item(Col_Name).Value);
        //            }
        //        }

        //        internal static void Set_Orphan_Data(Form form, DataTable DT_Orphans_Details, int DT_Row_Index, string Line_DataSource_Table)
        //        {
        //            string Code = form.DataSources.DBDataSources.Item(Line_DataSource_Table).GetValue("U_ST_ORPHAN_CODE", DT_Row_Index);
        //            string SQL = $@"SELECT T0.""CardName"", T0.U_ST_GENDER, T0.U_ST_DATE_OF_BIRTH, T0.U_ST_NATIONAL_ID FROM OCRD T0 WHERE T0.""CardCode"" = '{Code}'";
        //            Recordset RC = Helper.Utility.Execute_Recordset_Query(company, SQL);
        //            for (int J = 0; J < DT_Orphans_Details.Columns.Count; J++)
        //            {
        //                string Col_Name = DT_Orphans_Details.Columns.Item(J).Name;
        //                if (Col_Name == "SELECTED")
        //                {
        //                    continue;
        //                }
        //                if (Col_Name == "U_ST_ORPHAN_CODE")
        //                {
        //                    DT_Orphans_Details.SetValue(Col_Name, DT_Row_Index, Code);
        //                    continue;
        //                }
        //                string X = RC.Fields.Item(Col_Name).Value.ToString();
        //                DT_Orphans_Details.SetValue(Col_Name, DT_Row_Index, RC.Fields.Item(Col_Name).Value);
        //            }
        //        }
        //        internal static void Set_Patient_Data(Form form, DataTable DT_Patient_Details, int DT_Row_Index, string Line_DataSource_Table)
        //        {
        //            string Code = form.DataSources.DBDataSources.Item(Line_DataSource_Table).GetValue("U_ST_PATIENT_CODE", DT_Row_Index);
        //            string SQL = $@"SELECT T0.U_ST_FULL_ARABIC_NAME, T0.U_ST_GENDER, T0.U_ST_DATE_OF_BIRTH, T0.U_ST_NATIONAL_ID
        //FROM ""@ST_PATIENTS_CARD""  T0 WHERE T0.""Code"" ='{Code}'";
        //            Recordset RC = Helper.Utility.Execute_Recordset_Query(company, SQL);
        //            for (int J = 0; J < DT_Patient_Details.Columns.Count; J++)
        //            {
        //                string Col_Name = DT_Patient_Details.Columns.Item(J).Name;
        //                if (Col_Name == "SELECTED")
        //                {
        //                    continue;
        //                }
        //                if (Col_Name == "U_ST_PATIENT_CODE")
        //                {
        //                    DT_Patient_Details.SetValue(Col_Name, DT_Row_Index, Code);
        //                    continue;
        //                }
        //                string X = RC.Fields.Item(Col_Name).Value.ToString();
        //                DT_Patient_Details.SetValue(Col_Name, DT_Row_Index, RC.Fields.Item(Col_Name).Value);
        //            }
        //        }

        private static void Before_Adding_UDO(BusinessObjectInfo businessObjectInfo)
        {
            Form form = SBO_Application.Forms.Item(businessObjectInfo.FormUID);
            if (form.Mode == BoFormMode.fm_ADD_MODE)
            {
                Set_Default_Value_Before_Adding(form);
            }
        }

        internal static void Set_Default_Value_Before_Adding(Form form)
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

            string New_Status = form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).GetValue("U_ST_STATUS", 0);
            string New_Status_Text;
            if (New_Status == "N")
            {
                New_Status_Text = "New";
            }
            else if (New_Status == "P")
            {
                New_Status_Text = "Potential";
            }
            else if (New_Status == "W")
            {
                New_Status_Text = "Won";
            }
            else if (New_Status == "L")
            {
                New_Status_Text = "Lost";
            }
            else
            {
                throw new Logic.Custom_Exception($"The new status[{New_Status}] is not supported");
            }
            string Code = form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).GetValue("Code", 0);
            string Old_Status = "";
            if (Code != "")
            {
                string SQL_Old_Status = $@"SELECT T0.U_ST_STATUS FROM ""{Form_Obj.UDO_Database_Table_Name}""  T0 WHERE T0.""Code"" = '{Code}'";
                Recordset RC_Old_Status = Helper.Utility.Execute_Recordset_Query(company, SQL_Old_Status);
                Old_Status = RC_Old_Status.Fields.Item("U_ST_STATUS").Value.ToString();
            }
            if (Old_Status == "W")
            {
                if (Old_Status != New_Status)
                {
                    throw new Logic.Custom_Exception($"We can't change the status from [Won] to [{New_Status_Text}]");
                }
            }

            if (Old_Status == "P")
            {
                if (New_Status == "N")
                {
                    throw new Logic.Custom_Exception($"We can't change the status from [Potential] to [{New_Status_Text}]");
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
                if (pVal.ItemUID == "500" && pVal.ColUID == "Act" && pVal.EventType == BoEventTypes.et_CHOOSE_FROM_LIST && !pVal.BeforeAction)
                {
                    Choose_From_List_Activity_Matrix(pVal);
                }
                if (pVal.ItemUID == "65" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Add_Amount_Row(pVal);
                }
                if (pVal.ItemUID == "Item_2" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Add_Req_Row(pVal);
                }
                if (pVal.ItemUID == "Item_3" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Remove_Req_Row(pVal);
                }
                if (pVal.ItemUID == "66" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Remove_Amount_Row(pVal);
                }
                if (pVal.ItemUID == "220" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Add_Expected_Post_Dates_Row(pVal);
                }
                if (pVal.ItemUID == "221" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Remove_Expected_Post_Dates_Row(pVal);
                }
                if (pVal.ItemUID == "36" && pVal.EventType == BoEventTypes.et_COMBO_SELECT && !pVal.BeforeAction)
                {
                    Form form = SBO_Application.Forms.Item(pVal.FormUID);
                    form.Freeze(true);
                    Set_Status_Visiblity(form);
                }

                if ((pVal.ItemUID == "14") && pVal.EventType == BoEventTypes.et_CHOOSE_FROM_LIST && !pVal.BeforeAction)
                {
                    SalesOpportunities_Choos_From_List(pVal);
                }
                if (pVal.ItemUID == "32" && pVal.EventType == BoEventTypes.et_CHOOSE_FROM_LIST && !pVal.BeforeAction)
                {
                    Choose_From_List_Entry(pVal);
                }
                if ((pVal.ItemUID == "55" || pVal.ItemUID == "57") && pVal.EventType == BoEventTypes.et_CHOOSE_FROM_LIST && !pVal.BeforeAction)
                {
                    Choose_From_List_UDO(pVal);
                }
                if (pVal.EventType == BoEventTypes.et_ITEM_PRESSED && Tab_Item_List.Contains(pVal.ItemUID) && !pVal.BeforeAction)
                {
                    Form form = SBO_Application.Forms.Item(pVal.FormUID);
                    Set_Status_Visiblity(form);
                }
                if (pVal.EventType == BoEventTypes.et_ITEM_PRESSED && Tab_Item_List.Contains(pVal.ItemUID) && pVal.BeforeAction)
                {
                    Form form = SBO_Application.Forms.Item(pVal.FormUID);
                    form.Freeze(true);
                }
                if (pVal.EventType == BoEventTypes.et_FORM_RESIZE && !pVal.BeforeAction)
                {
                    Form form = SBO_Application.Forms.Item(pVal.FormUID);
                    Utility.Resize_Grids(form, Grids_List);
                }

                if (pVal.ItemUID == "1000" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    KHCF_Logic_Utility.Select_Allocation(pVal, Form_Obj, "55");
                }

            }
            catch (Exception ex)
            {
                Form form = SBO_Application.Forms.Item(pVal.FormUID);
                form.Freeze(false);
                SBO_Application.StatusBar.SetText(ex.Message, BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Error);
            }
        }

        private static void Choose_From_List_UDO(ItemEvent pVal)
        {
            string Code = Utility.Chosse_From_List_For_Code_And_DBDataSource(pVal, pVal.ItemUID);
        }

        private static void Choose_From_List_Entry(ItemEvent pVal)
        {
            Form form = SBO_Application.Forms.Item(pVal.FormUID);
            DataTable DT;
            string Code= Utility.Chosse_From_List_For_Code_And_DBDataSource(pVal, pVal.ItemUID, out DT);
            if (Code == "")
            {
                return;
            }
            form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).SetValue("U_ST_ENTITY_TYPE", 0, DT.GetValue("U_ST_CUSTOMER_GROUP", 0).ToString());
            form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).SetValue("U_ST_ENTITY_NAME", 0, DT.GetValue("U_ST_COMPANY_ARABIC_NAME", 0).ToString());

            Set_Sales_Oppertunities_CFL_Condation(form);

            if (form.Mode == BoFormMode.fm_OK_MODE)
            {
                form.Mode = BoFormMode.fm_UPDATE_MODE;
            }

        }

        private static void Remove_Amount_Row(ItemEvent pVal)
        {
            Form form = SBO_Application.Forms.Item(pVal.FormUID);
            Matrix Mat = (Matrix)form.Items.Item("67").Specific;
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

        private static void Remove_Req_Row(ItemEvent pVal)
        {
            Form form = SBO_Application.Forms.Item(pVal.FormUID);
            Matrix Mat = (Matrix)form.Items.Item("500").Specific;
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


        private static void Add_Amount_Row(ItemEvent pVal)
        {
            Form form = SBO_Application.Forms.Item(pVal.FormUID);
            form.Freeze(true);
            DBDataSource DS_Amount = form.DataSources.DBDataSources.Item("@ST_WON_GRANT_AMOUNT");
            Matrix Mat = (Matrix)form.Items.Item("67").Specific;
            Mat.FlushToDataSource();
            int Count = DS_Amount.Size;
            if (Count == 1)
            {
                if (DS_Amount.GetValue("U_ST_AMOUNT", Count - 1) != "" && double.Parse(DS_Amount.GetValue("U_ST_AMOUNT", Count - 1)) != 0)
                {
                    DS_Amount.InsertRecord(Count);
                }
                else
                {
                    Count = 0;

                    DS_Amount.InsertRecord(Count);
                    Mat.LoadFromDataSource();
                    Mat.DeleteRow(1);
                    Mat.FlushToDataSource();
                    Mat.LoadFromDataSource();
                }
            }
            else
            {
                DS_Amount.InsertRecord(Count);
            }

            Mat.LoadFromDataSource();

            if (form.Mode == BoFormMode.fm_OK_MODE)
            {
                form.Mode = BoFormMode.fm_UPDATE_MODE;
            }

            form.Freeze(false);

        }

        private static void Remove_Expected_Post_Dates_Row(ItemEvent pVal)
        {
            Form form = SBO_Application.Forms.Item(pVal.FormUID);
            Matrix Mat = (Matrix)form.Items.Item("204").Specific;
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

        private static void Add_Expected_Post_Dates_Row(ItemEvent pVal)
        {
            Form form = SBO_Application.Forms.Item(pVal.FormUID);
            form.Freeze(true);
            Matrix Mat_Add = (Matrix)form.Items.Item("204").Specific;
            Mat_Add.AddRow();
            if (form.Mode == BoFormMode.fm_OK_MODE)
            {
                form.Mode = BoFormMode.fm_UPDATE_MODE;
            }
            Mat_Add.FlushToDataSource();
            int Last_Row_Index = form.DataSources.DBDataSources.Item("@ST_WON_GRANT_POS_DT").Size;
            form.DataSources.DBDataSources.Item("@ST_WON_GRANT_POS_DT").SetValue("U_ST_DATE", Last_Row_Index -1, "");
            Mat_Add.LoadFromDataSource();

            if (form.Mode == BoFormMode.fm_OK_MODE)
            {
                form.Mode = BoFormMode.fm_UPDATE_MODE;
            }

            form.Freeze(false);

        }


        //private static void Add_Patients_Line(ItemEvent pVal, string Line_DataSource_Table)
        //{
        //    Form form = SBO_Application.Forms.Item(pVal.FormUID);
        //    DataTable DT_Orphans_Details = form.DataSources.DataTables.Item("Patients_Details");
        //    form.Freeze(true);
        //    DBDataSource DS_Lines = form.DataSources.DBDataSources.Item(Line_DataSource_Table);
        //    Matrix Mat_Lines = (Matrix)form.Items.Item("101").Specific;
        //    Mat_Lines.FlushToDataSource();
        //    int Count = DS_Lines.Size;
        //    if (Count == 1)
        //    {
        //        if (DS_Lines.GetValue("U_ST_PATIENT_CODE", Count - 1) != "")
        //        {
        //            DS_Lines.InsertRecord(Count);
        //        }
        //        else
        //        {
        //            Count = 0;

        //            DS_Lines.InsertRecord(Count);
        //            Mat_Lines.LoadFromDataSource();
        //            Mat_Lines.DeleteRow(1);
        //            Mat_Lines.FlushToDataSource();
        //            Mat_Lines.LoadFromDataSource();
        //        }
        //    }
        //    else
        //    {
        //        DS_Lines.InsertRecord(Count);
        //    }
        //    if (DS_Lines.Size > DT_Orphans_Details.Rows.Count)
        //    {
        //        DT_Orphans_Details.Rows.Add();
        //    }
        //    Mat_Lines.LoadFromDataSource();
        //    //Set_Default_Value_Before_Adding(form);

        //    if (form.Mode == BoFormMode.fm_OK_MODE)
        //    {
        //        form.Mode = BoFormMode.fm_UPDATE_MODE;
        //    }
        //    form.Freeze(false);

        //}

        //private static void Remove_Patients_Selected_Lines(ItemEvent pVal)
        //{
        //    Form form = SBO_Application.Forms.Item(pVal.FormUID);
        //    DataTable DT_Orphans_Details = form.DataSources.DataTables.Item("Patients_Details");
        //    Matrix Mat = (Matrix)form.Items.Item("101").Specific;
        //    int Count = Mat.RowCount;
        //    for (int i = Count; i > 0; i--)
        //    {
        //        //CheckBox Chk_Selected = (CheckBox)Mat.GetCellSpecific("SELECTED", i);
        //        CheckBox Chk_Selected = (CheckBox)Mat.Columns.Item("SELECTED").Cells.Item(i).Specific;
        //        if (Chk_Selected.Checked)
        //        {
        //            Mat.DeleteRow(i);
        //            DT_Orphans_Details.Rows.Remove(i);
        //            // form.DataSources.DBDataSources.Item(1).RemoveRecord(Grid_Deleted_Row +1);
        //            Mat.FlushToDataSource();
        //            Mat.LoadFromDataSource();

        //        }
        //    }
        //    if (form.Mode == BoFormMode.fm_OK_MODE)
        //    {
        //        form.Mode = BoFormMode.fm_UPDATE_MODE;
        //    }
        //}

        //private static void Patients_choose_From_List(ItemEvent pVal, string Line_DataSource_Table)
        //{
        //    SAPbouiCOM.Form form = Loader.SBO_Application.Forms.Item(pVal.FormUID);
        //    SAPbouiCOM.ChooseFromListEvent Choos_Event = (SAPbouiCOM.ChooseFromListEvent)pVal;

        //    if (Choos_Event.SelectedObjects == null || Choos_Event.SelectedObjects.Rows.Count == 0)
        //    {
        //        return;
        //    }
        //    Matrix Mat = (Matrix)form.Items.Item("101").Specific;
        //    DataTable DT_Patients_Details = form.DataSources.DataTables.Item("Patients_Details");

        //    string Code = Choos_Event.SelectedObjects.GetValue("Code", 0).ToString();
        //    int Index = pVal.Row - 1;
        //    form.DataSources.DBDataSources.Item(Line_DataSource_Table).SetValue("U_ST_PATIENT_CODE", Index, Code);


        //    Set_Patient_Data(form, DT_Patients_Details, Index, Line_DataSource_Table);
        //    Mat.LoadFromDataSource();
        //    Mat.AutoResizeColumns();
        //    if (form.Mode == BoFormMode.fm_OK_MODE)
        //    {
        //        form.Mode = BoFormMode.fm_UPDATE_MODE;
        //    }
        //}

        //private static void Wish_choose_From_List(ItemEvent pVal, string Line_DataSource_Table)
        //{
        //    SAPbouiCOM.Form form = Loader.SBO_Application.Forms.Item(pVal.FormUID);
        //    SAPbouiCOM.ChooseFromListEvent Choos_Event = (SAPbouiCOM.ChooseFromListEvent)pVal;

        //    if (Choos_Event.SelectedObjects == null || Choos_Event.SelectedObjects.Rows.Count == 0)
        //    {
        //        return;
        //    }
        //    Matrix Mat = (Matrix)form.Items.Item("101").Specific;
        //    DataTable DT_Wish_Details = form.DataSources.DataTables.Item("Wish_Details");

        //    string Code = Choos_Event.SelectedObjects.GetValue("Code", 0).ToString();
        //    int Index = pVal.Row - 1;
        //    form.DataSources.DBDataSources.Item(Line_DataSource_Table).SetValue("U_ST_WISH_CODE", Index, Code);
        //    Set_Wish_Data(form, DT_Wish_Details, Index, Line_DataSource_Table);
        //    Mat.LoadFromDataSource();
        //    Mat.AutoResizeColumns();
        //}


        internal static void SalesOpportunities_Choos_From_List(ItemEvent pVal)
        {
            SAPbouiCOM.Form form = Loader.SBO_Application.Forms.Item(pVal.FormUID);
            SAPbouiCOM.ChooseFromListEvent Choos_Event = (SAPbouiCOM.ChooseFromListEvent)pVal;

            if (Choos_Event.SelectedObjects == null || Choos_Event.SelectedObjects.Rows.Count == 0)
            {
                return;
            }
            string Oppr_ID = Choos_Event.SelectedObjects.GetValue("OpprId", 0).ToString();
            form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).SetValue("U_ST_OPPORTUNITY_ID", 0, Oppr_ID);

            Select_Sales_Oppertunities(form, Oppr_ID);

            if (form.Mode == BoFormMode.fm_OK_MODE)
            {
                form.Mode = BoFormMode.fm_UPDATE_MODE;
            }

        }

        private static void Select_Sales_Oppertunities(Form form, string Oppr_ID)
        {

            string SQL = $@"SELECT T0.""Name"", T0.""MaxSumLoc"", T0.""U_ST_SUBMISSION_DEADLINE"", T0.""U_ST_ANNOUNCEMENT_DEADLINE""
, T0.""U_ST_SUBMITTED"", T0.""U_ST_SUBMISSION_NOTES"", T0.""OpenDate"", T0.""CloseDate"" , T0.""SlpCode""
FROM OOPR T0 WHERE T0.""OpprId"" = 0{Oppr_ID}";
            Recordset RC = Helper.Utility.Execute_Recordset_Query(company, SQL);

            //form.DataSources.UserDataSources.Item("11").ValueEx = RC.Fields.Item("Name").Value.ToString();
            form.DataSources.UserDataSources.Item("60").ValueEx = RC.Fields.Item("MaxSumLoc").Value.ToString();
            form.DataSources.UserDataSources.Item("42").ValueEx = Utility.Get_Date_Datasource_ValueEX((DateTime)RC.Fields.Item("U_ST_SUBMISSION_DEADLINE").Value);
            form.DataSources.UserDataSources.Item("42").ValueEx = Utility.Get_Date_Datasource_ValueEX((DateTime)RC.Fields.Item("U_ST_SUBMISSION_DEADLINE").Value);
            form.DataSources.UserDataSources.Item("44").ValueEx = Utility.Get_Date_Datasource_ValueEX((DateTime)RC.Fields.Item("U_ST_ANNOUNCEMENT_DEADLINE").Value);
            form.DataSources.UserDataSources.Item("46").ValueEx = RC.Fields.Item("U_ST_SUBMITTED").Value.ToString();
            form.DataSources.UserDataSources.Item("48").ValueEx = RC.Fields.Item("U_ST_SUBMISSION_NOTES").Value.ToString();
            form.DataSources.UserDataSources.Item("15").ValueEx = Utility.Get_Date_Datasource_ValueEX((DateTime) RC.Fields.Item("OpenDate").Value);
            form.DataSources.UserDataSources.Item("24").ValueEx = Utility.Get_Date_Datasource_ValueEX((DateTime)RC.Fields.Item("CloseDate").Value);
            form.DataSources.UserDataSources.Item("38").ValueEx = RC.Fields.Item("SlpCode").Value.ToString();

        }




        //internal static void Add_Orphans_Line(ItemEvent pVal, string Line_DataSource_Table)
        //{
        //    Form form = SBO_Application.Forms.Item(pVal.FormUID);
        //    DataTable DT_Orphans_Details = form.DataSources.DataTables.Item("Orphan_Details");
        //    form.Freeze(true);
        //    DBDataSource DS_Lines = form.DataSources.DBDataSources.Item(Line_DataSource_Table);
        //    Matrix Mat_Lines = (Matrix)form.Items.Item("100").Specific;
        //    Mat_Lines.FlushToDataSource();
        //    int Count = DS_Lines.Size;
        //    if (Count == 1)
        //    {
        //        if (DS_Lines.GetValue("U_ST_ORPHAN_CODE", Count - 1) != "")
        //        {
        //            DS_Lines.InsertRecord(Count);
        //        }
        //        else
        //        {
        //            Count = 0;

        //            DS_Lines.InsertRecord(Count);
        //            Mat_Lines.LoadFromDataSource();
        //            Mat_Lines.DeleteRow(1);
        //            Mat_Lines.FlushToDataSource();
        //            Mat_Lines.LoadFromDataSource();
        //        }
        //    }
        //    else
        //    {
        //        DS_Lines.InsertRecord(Count);
        //    }
        //    if (DS_Lines.Size > DT_Orphans_Details.Rows.Count)
        //    {
        //        DT_Orphans_Details.Rows.Add();
        //    }
        //    Mat_Lines.LoadFromDataSource();
        //    //Set_Default_Value_Before_Adding(form);

        //    if (form.Mode == BoFormMode.fm_OK_MODE)
        //    {
        //        form.Mode = BoFormMode.fm_UPDATE_MODE;
        //    }
        //    form.Freeze(false);

        //}

        //internal static void Remove_Orphans_Selected_Lines(ItemEvent pVal)
        //{
        //    Form form = SBO_Application.Forms.Item(pVal.FormUID);
        //    DataTable DT_Orphans_Details = form.DataSources.DataTables.Item("Orphan_Details");
        //    Matrix Mat = (Matrix)form.Items.Item("100").Specific;
        //    int Count = Mat.RowCount;
        //    for (int i = Count; i > 0; i--)
        //    {
        //        //CheckBox Chk_Selected = (CheckBox)Mat.GetCellSpecific("SELECTED", i);
        //        CheckBox Chk_Selected = (CheckBox)Mat.Columns.Item("SELECTED").Cells.Item(i).Specific;
        //        if (Chk_Selected.Checked)
        //        {
        //            Mat.DeleteRow(i);
        //            DT_Orphans_Details.Rows.Remove(i);
        //            // form.DataSources.DBDataSources.Item(1).RemoveRecord(Grid_Deleted_Row +1);
        //            Mat.FlushToDataSource();
        //            Mat.LoadFromDataSource();

        //        }
        //    }
        //    if (form.Mode == BoFormMode.fm_OK_MODE)
        //    {
        //        form.Mode = BoFormMode.fm_UPDATE_MODE;
        //    }

        //}

        //internal static void Add_Wish_Line(ItemEvent pVal, string Line_DataSource_Table)
        //{
        //    Form form = SBO_Application.Forms.Item(pVal.FormUID);
        //    DataTable DT = form.DataSources.DataTables.Item("Wish_Details");
        //    form.Freeze(true);
        //    DBDataSource DS_Lines = form.DataSources.DBDataSources.Item(Line_DataSource_Table);
        //    Matrix Mat_Lines = (Matrix)form.Items.Item("33").Specific;
        //    Mat_Lines.FlushToDataSource();
        //    int Count = DS_Lines.Size;
        //    if (Count == 1)
        //    {
        //        if (DS_Lines.GetValue("U_ST_WISH_CODE", Count - 1) != "")
        //        {
        //            DS_Lines.InsertRecord(Count);
        //        }
        //        else
        //        {
        //            Count = 0;

        //            DS_Lines.InsertRecord(Count);
        //            Mat_Lines.LoadFromDataSource();
        //            Mat_Lines.DeleteRow(1);
        //            Mat_Lines.FlushToDataSource();
        //            Mat_Lines.LoadFromDataSource();
        //        }
        //    }
        //    else
        //    {
        //        DS_Lines.InsertRecord(Count);
        //    }
        //    if (DS_Lines.Size > DT.Rows.Count)
        //    {
        //        DT.Rows.Add();
        //    }
        //    Mat_Lines.LoadFromDataSource();
        //    //Set_Default_Value_Before_Adding(form);

        //    if (form.Mode == BoFormMode.fm_OK_MODE)
        //    {
        //        form.Mode = BoFormMode.fm_UPDATE_MODE;
        //    }
        //    form.Freeze(false);

        //}

        //internal static void Remove_Wish_Selected_Lines(ItemEvent pVal)
        //{
        //    Form form = SBO_Application.Forms.Item(pVal.FormUID);
        //    DataTable DT_Orphans_Details = form.DataSources.DataTables.Item("Orphan_Details");
        //    Matrix Mat = (Matrix)form.Items.Item("100").Specific;
        //    int Count = Mat.RowCount;
        //    for (int i = Count; i > 0; i--)
        //    {
        //        //CheckBox Chk_Selected = (CheckBox)Mat.GetCellSpecific("SELECTED", i);
        //        CheckBox Chk_Selected = (CheckBox)Mat.Columns.Item("SELECTED").Cells.Item(i).Specific;
        //        if (Chk_Selected.Checked)
        //        {
        //            Mat.DeleteRow(i);
        //            DT_Orphans_Details.Rows.Remove(i);
        //            // form.DataSources.DBDataSources.Item(1).RemoveRecord(Grid_Deleted_Row +1);
        //            Mat.FlushToDataSource();
        //            Mat.LoadFromDataSource();

        //        }
        //    }
        //    if (form.Mode == BoFormMode.fm_OK_MODE)
        //    {
        //        form.Mode = BoFormMode.fm_UPDATE_MODE;
        //    }

        //}

        private static void Choose_From_List_Activity_Matrix(ItemEvent pVal)
        {
            SAPbouiCOM.Form form = SBO_Application.Forms.Item(pVal.FormUID);
            ChooseFromListEvent Chos_Event = (ChooseFromListEvent)pVal;
            if ((Chos_Event.SelectedObjects == null ? false : Chos_Event.SelectedObjects.Rows.Count != 0))
            {
                Matrix Mat_Line = (Matrix)form.Items.Item(pVal.ItemUID).Specific;
                string C = Chos_Event.SelectedObjects.GetValue("ClgCode", 0).ToString();
                try
                {
                    if (form.Mode == BoFormMode.fm_OK_MODE)
                    {
                        form.Mode = BoFormMode.fm_UPDATE_MODE;
                    }

                    EditText Txt_Broker = (EditText)Mat_Line.GetCellSpecific(pVal.ColUID, pVal.Row);
                    Txt_Broker.Value = C;
                }
                catch (Exception ex) { }
            }

        }

        private static void Add_Req_Row(ItemEvent pVal)
        {
            Form form = SBO_Application.Forms.Item(pVal.FormUID);
            form.Freeze(true);
            DBDataSource DS_Amount = form.DataSources.DBDataSources.Item("@ST_GRANT_REPORTS");
            Matrix Mat = (Matrix)form.Items.Item("500").Specific;
            Mat.FlushToDataSource();
            int Count = DS_Amount.Size;
            if (Count == 1)
            {
                if (DS_Amount.GetValue("U_ST_REPORT_REQ", Count - 1) != "" && DS_Amount.GetValue("U_ST_REPORT_FREQ", Count - 1) != "")
                {
                    DS_Amount.InsertRecord(Count);
                }
                else
                {
                    Count = 0;

                    DS_Amount.InsertRecord(Count);
                    Mat.LoadFromDataSource();
                    Mat.DeleteRow(1);
                    Mat.FlushToDataSource();
                    Mat.LoadFromDataSource();
                }
            }
            else
            {
                DS_Amount.InsertRecord(Count);
            }

            Mat.LoadFromDataSource();
            Mat.AutoResizeColumns();

            if (form.Mode == BoFormMode.fm_OK_MODE)
            {
                form.Mode = BoFormMode.fm_UPDATE_MODE;
            }

            form.Freeze(false);

        }

    }
}
