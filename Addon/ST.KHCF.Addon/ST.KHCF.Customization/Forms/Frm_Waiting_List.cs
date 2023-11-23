using SAPbobsCOM;
using SAPbouiCOM;
using ST.KHCF.Customization.Logic;
using ST.KHCF.Customization.Logic.Classes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using DataTable = SAPbouiCOM.DataTable;
using Matrix = SAPbouiCOM.Matrix;

namespace ST.KHCF.Customization.Forms
{
    internal class Frm_Waiting_List
    {
        internal static SAPbobsCOM.Company company;
        internal static SAPbouiCOM.Application SBO_Application;

        internal static Form Create_Form()
        {
            var form_params = (FormCreationParams)SBO_Application.CreateObject(BoCreatableObjectType.cot_FormCreationParams);
            form_params.XmlData = Properties.Resources.Frm_Waiting_List;
            Form form;
            try 
            {
                form = SBO_Application.Forms.AddEx(form_params);
            }
            catch(Exception ex)
            {
                 form = SBO_Application.Forms.GetForm("ST_Waiting_List", 1);
            }

            //form.Visible = false;
            form.AutoManaged = true;
            form.SupportedModes = -1;
            form_params.Modality = BoFormModality.fm_Modal;

            //  Inv_Form_Data = Inv_Data;

            try
            {
                Initialize_Form(form);
            }
            catch (Exception ex)
            {
                SBO_Application.StatusBar.SetText(ex.Message, BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Error);
            }
            form.Visible = true;
            return form;
        }

        private static void Initialize_Form(Form form)
        {
            
            form.DataSources.UserDataSources.Item("1").Value = DateTime.Today.AddMonths(-1).ToString("yyyyMMdd");
            form.DataSources.UserDataSources.Item("2").Value = DateTime.Today.ToString("yyyyMMdd");
            form.DataSources.UserDataSources.Item("UD_12").Value = DateTime.Today.ToString("yyyyMMdd");
            form.DataSources.UserDataSources.Item("UD_14").Value = "Y";
           

            string N_query = $@"SELECT T0.""Code"", T0.""Name"" FROM OCRY T0";
            Helper.Utility.Fill_One_ComboBoxBySQL(company, form, "5", N_query, true);

            string CT_query = $@"SELECT DISTINCT T0.""Code"" , T0.""Name"" FROM ""@ST_CANCER_TYPE"" T0";
            Helper.Utility.Fill_One_ComboBoxBySQL(company, form, "6", CT_query, true);

            string SupT_query = $@"SELECT DISTINCT T0.""Code"" , T0.""Name"" FROM ""@ST_SUPPORT_TYPE"" T0";
            Helper.Utility.Fill_One_ComboBoxBySQL(company, form, "7", SupT_query, true);

            string TP_query = $@"SELECT DISTINCT T0.""Code"" , T0.""Name"" FROM ""@ST_TREATM_PLAN_DET"" T0";
            Helper.Utility.Fill_One_ComboBoxBySQL(company, form, "11", TP_query, true);

            string TT_query = $@"SELECT T0.""Code"", T0.""Name"" FROM ""KHCF_Y_TEST"".""@ST_TREATMENT_TYPE""  T0";
            Helper.Utility.Fill_One_ComboBoxBySQL(company, form, "15", TT_query, true);

            

            

            string sql_Query = $@"SELECT  T0.""U_ST_NAME"" FROM ""@ST_GOOD_WILL_FUNDS""  T0";


            DataTable DT_Result1 = form.DataSources.DataTables.Item("RESULT1");
            Recordset RC1 = Helper.Utility.Execute_Recordset_Query(company, sql_Query);

            int Count_All1 = RC1.RecordCount;
            int index = 0;
            if (Count_All1 > 0)
            {
                DT_Result1.Rows.Clear();
                DT_Result1.Rows.Add(Count_All1);
                while (!RC1.EoF)
                {
                    DT_Result1.SetValue("Code", index, RC1.Fields.Item("U_ST_NAME").Value.ToString());
                    DT_Result1.SetValue("Name", index, RC1.Fields.Item("U_ST_NAME").Value.ToString());
                    index++;

                    RC1.MoveNext();
                }

            }

        }

        internal static void SBO_Application_ItemEvent(string FormUID, ref ItemEvent pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            if (pVal.FormTypeEx != "ST_Waiting_List")
            {
                return;
            }
            try
            {
                
                if (pVal.EventType == BoEventTypes.et_FORM_VISIBLE && !pVal.BeforeAction) //|| pVal.EventType == BoEventTypes.et_FORM_DATA_LOAD)
                {
                   
                    Form form = SBO_Application.Forms.Item(pVal.FormUID);
                    if (form.Mode == BoFormMode.fm_FIND_MODE)
                    {
                        //  Form form = SBO_Application.Forms.Item(pVal.FormUID);
                        Add_Items(form, false);
                    }
                    else
                    {
                        Add_Items(form, true);
                    }
                    try {
                        Matrix Mat = (Matrix)form.Items.Item("Item_2").Specific;
                        Check_Visiblity(form, Mat);
                    }
                    catch(Exception ex) { }
                
                }  
                
                if (pVal.ItemUID == "12" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Preview(pVal);
                }
                if (pVal.ItemUID == "13" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Close(pVal);
                }
                if (pVal.ItemUID == "Item_8" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Add_List(pVal);
                } 
                if (pVal.ItemUID == "Item_6" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Update_List(pVal);
                }
                if (pVal.EventType == BoEventTypes.et_ITEM_PRESSED && pVal.ItemUID == "Item_2")
                {
                    if (pVal.ItemUID == "Item_2" && pVal.ColUID == "good")
                    {
                        Form form = SBO_Application.Forms.Item(pVal.FormUID);
                        form.Freeze(true);
                        ComboBox combo = null;
                        Matrix Mat = (Matrix)form.Items.Item("Item_2").Specific;
                        combo = (ComboBox) Mat.Columns.Item("good").Cells.Item(1).Specific;
                        string sql_Query = $@"SELECT  T0.""U_ST_NAME"" FROM ""@ST_GOOD_WILL_FUNDS""  T0";

                        while (combo.ValidValues.Count > 0) 
                        {
                            combo.ValidValues.Remove(0, BoSearchKey.psk_Index);
                        }
                        Recordset RC1 = Helper.Utility.Execute_Recordset_Query(company, sql_Query);

                        int Count_All1 = RC1.RecordCount;
                        //  int index = 0;
                        if (Count_All1 > 0)
                        {

                            while (!RC1.EoF)
                            {
                                combo.ValidValues.Add(RC1.Fields.Item(0).Value.ToString(), RC1.Fields.Item(0).Value.ToString());

                                RC1.MoveNext();
                            }

                        }
                        Mat.Columns.Item("good").DisplayDesc = true;



                    }
                }
                if (pVal.EventType == BoEventTypes.et_ITEM_PRESSED && pVal.ItemUID == "Item_7")
                {
                    Form form = SBO_Application.Forms.Item(pVal.FormUID);
                    Matrix Mat = (Matrix)form.Items.Item("Item_2").Specific;
                    string str = form.DataSources.UserDataSources.Item("UD_14").Value;
                    if (str == "Y")
                    {
                        form.DataSources.UserDataSources.Item("UD_14").Value = "N";
                        Check_Visiblity(form, Mat);
                    }
                    else
                    {
                        form.DataSources.UserDataSources.Item("UD_14").Value = "Y";
                        Check_Visiblity(form, Mat);
                    }
                }
            }
            catch (Exception ex)
            {
                Form form = SBO_Application.Forms.Item(pVal.FormUID);
                form.Freeze(false);
                SBO_Application.MessageBox(ex.Message);
                SBO_Application.StatusBar.SetText(ex.Message, BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Error);
            }
        }

        private static void Preview(ItemEvent pVal)
        {
            Form form = SBO_Application.Forms.Item(pVal.FormUID);
            if (form.Mode == BoFormMode.fm_FIND_MODE)
            {
                Add_Items(form, false);
                
                string waiting_query = $@"select * from ""@ST_Waiting_List"" T0";
                string condition = string.Empty;
                if (form.DataSources.UserDataSources.Item("UD_13").Value == "")
                {
                    throw new Logic.Custom_Exception("Please set the Code");
                }

                else
                {
                    if (!string.IsNullOrEmpty(form.DataSources.UserDataSources.Item("UD_13").Value.ToString()))
                    {
                        condition = $@" Where T0.""Code""='{form.DataSources.UserDataSources.Item("UD_13").Value.ToString()}'";
                    }
                    if (!string.IsNullOrEmpty(form.DataSources.UserDataSources.Item("3").Value.ToString()) &&
                        !string.IsNullOrEmpty(form.DataSources.UserDataSources.Item("4").Value.ToString()))
                    {
                        int begin_year = Convert.ToInt32(form.DataSources.UserDataSources.Item("3").Value.ToString());
                        DateTime Begin_Age1 = DateTime.Now.AddYears(begin_year * -1);
                        int end_year = Convert.ToInt32(form.DataSources.UserDataSources.Item("4").Value.ToString());
                        DateTime End_Age1 = DateTime.Now.AddYears(end_year * -1);
                        if (string.IsNullOrEmpty(condition))
                        {
                            condition += "where ";
                            condition += $@" T0.""U_ST_Age"" between to_date('{Begin_Age1.ToString("MM/dd/yyyy")}', 'MM/DD/YYYY') and to_date('{End_Age1.ToString("MM/dd/yyyy")}', 'MM/DD/YYYY')";
                        }
                        else { condition += $@" And T0.""U_ST_Age"" between to_date('{Begin_Age1.ToString("MM/dd/yyyy")}', 'MM/DD/YYYY') and to_date('{End_Age1.ToString("MM/dd/yyyy")}', 'MM/DD/YYYY')"; }
                    }
                    if (!string.IsNullOrEmpty(form.DataSources.UserDataSources.Item("1").Value.ToString()) &&
                       !string.IsNullOrEmpty(form.DataSources.UserDataSources.Item("2").Value.ToString()))
                    {

                        DateTime Begin_Date1 = DateTime.ParseExact(form.DataSources.UserDataSources.Item("1").ValueEx, "yyyyMMdd", null);

                        DateTime End_Date1 = DateTime.ParseExact(form.DataSources.UserDataSources.Item("1").ValueEx, "yyyyMMdd", null);
                        if (string.IsNullOrEmpty(condition))
                        {
                            condition += "where ";
                            condition += $@" T0.""U_ST_Request_d"" between to_date('{Begin_Date1.ToString("MM/dd/yyyy")}', 'MM/DD/YYYY') and to_date('{End_Date1.ToString("MM/dd/yyyy")}', 'MM/DD/YYYY')";
                        }
                        else { condition += $@" And T0.""U_ST_Request_d"" between to_date('{Begin_Date1.ToString("MM/dd/yyyy")}', 'MM/DD/YYYY') and to_date('{End_Date1.ToString("MM/dd/yyyy")}', 'MM/DD/YYYY')"; }
                    }

                    if (!string.IsNullOrEmpty(form.DataSources.UserDataSources.Item("5").Value.ToString()))
                    {
                        if (string.IsNullOrEmpty(condition))
                        {
                            condition += "where ";
                            condition += $@" T0.""U_ST_National"" = '{form.DataSources.UserDataSources.Item("5").Value}'";
                        }
                        else
                        {
                            condition += $@" And T0.""U_ST_National"" = '{form.DataSources.UserDataSources.Item("5").Value}'";
                        }
                    }
                    if (!string.IsNullOrEmpty(form.DataSources.UserDataSources.Item("6").Value.ToString()))
                    {
                        if (string.IsNullOrEmpty(condition))
                        {
                            condition += " where";
                            condition += $@" T0.""U_ST_C_Type"" = '{form.DataSources.UserDataSources.Item("6")}'";
                        }
                        else
                            condition += $@" And T0.""U_ST_C_Type"" = '{form.DataSources.UserDataSources.Item("6")}'";
                    }

                    if (string.IsNullOrEmpty(condition))
                        waiting_query += " " + condition;
                    Recordset RC = Helper.Utility.Execute_Recordset_Query(company, waiting_query);
                    DataTable DT_Result = form.DataSources.DataTables.Item("RESULT");
                    int Count_All = RC.RecordCount;
                    int index = 0;
                    Matrix Mat = (Matrix)form.Items.Item("Item_2").Specific;

                    //DT_Result.ExecuteQuery(SQL);

                    if (Count_All > 0)
                    {
                        DT_Result.Rows.Clear();
                        DT_Result.Rows.Add(Count_All);
                        while (!RC.EoF)
                        {

                            DT_Result.SetValue("DocEntry", index, RC.Fields.Item("DocEntry").Value.ToString());
                            DT_Result.SetValue("M_Number", index, RC.Fields.Item("U_ST_M_Number").Value.ToString());
                            DT_Result.SetValue("P_Name", index, RC.Fields.Item("U_ST_P_Name").Value.ToString());
                            DT_Result.SetValue("National", index, RC.Fields.Item("U_ST_National").Value.ToString());
                            DT_Result.SetValue("Age", index, RC.Fields.Item("U_ST_Age").Value.ToString());
                            DT_Result.SetValue("C_Type", index, RC.Fields.Item("U_ST_C_Type").Value.ToString());
                            DT_Result.SetValue("P_Status", index, RC.Fields.Item("U_ST_P_Status").Value.ToString());
                            DT_Result.SetValue("A_Cost", index, RC.Fields.Item("U_ST_A_Cost").Value.ToString());
                            DT_Result.SetValue("R_Support", index, RC.Fields.Item("U_ST_R_Support").Value.ToString());
                            DT_Result.SetValue("A_Amount", index, RC.Fields.Item("U_ST_A_Amount").Value.ToString());
                            DT_Result.SetValue("T_Amount", index, RC.Fields.Item("U_ST_T_Amount").Value.ToString());
                            DT_Result.SetValue("R_Amount", index, RC.Fields.Item("U_ST_R_Amount").Value.ToString());
                            DT_Result.SetValue("P_Party", index, RC.Fields.Item("U_ST_P_Party").Value.ToString());
                            DT_Result.SetValue("P_Amount", index, RC.Fields.Item("U_ST_P_Amount").Value.ToString());
                            DT_Result.SetValue("Request_d", index, RC.Fields.Item("U_ST_Request_d").Value.ToString());
                            DT_Result.SetValue("User1", index, RC.Fields.Item("U_ST_User1").Value.ToString());
                            DT_Result.SetValue("good", index, RC.Fields.Item("U_ST_good").Value.ToString());
                            DT_Result.SetValue("amount1", index, RC.Fields.Item("U_ST_amount1").Value.ToString());
                            DT_Result.SetValue("remark1", index, RC.Fields.Item("U_ST_remark1").Value.ToString());
                            DT_Result.SetValue("User2", index, RC.Fields.Item("U_ST_User2").Value.ToString());
                            DT_Result.SetValue("good2", index, RC.Fields.Item("U_ST_good2").Value.ToString());
                            DT_Result.SetValue("amount2", index, RC.Fields.Item("U_ST_amount2").Value.ToString());
                            DT_Result.SetValue("remark2", index, RC.Fields.Item("U_ST_remark2").Value.ToString());
                            DT_Result.SetValue("User3", index, RC.Fields.Item("U_ST_User3").Value.ToString());
                            DT_Result.SetValue("good3", index, RC.Fields.Item("U_ST_good3").Value.ToString());
                            DT_Result.SetValue("amount3", index, RC.Fields.Item("U_ST_amount3").Value.ToString());
                            DT_Result.SetValue("remark3", index, RC.Fields.Item("U_ST_remark3").Value.ToString());
                            DT_Result.SetValue("User4", index, RC.Fields.Item("U_ST_User4").Value.ToString());
                            DT_Result.SetValue("good4", index, RC.Fields.Item("U_ST_good4").Value.ToString());
                            DT_Result.SetValue("amount4", index, RC.Fields.Item("U_ST_amount4").Value.ToString());
                            DT_Result.SetValue("remark4", index, RC.Fields.Item("U_ST_remark4").Value.ToString());
                            DT_Result.SetValue("User5", index, RC.Fields.Item("U_ST_User5").Value.ToString());
                            DT_Result.SetValue("good5", index, RC.Fields.Item("U_ST_good5").Value.ToString());
                            DT_Result.SetValue("amount5", index, RC.Fields.Item("U_ST_amount5").Value.ToString());
                            DT_Result.SetValue("remark5", index, RC.Fields.Item("U_ST_remark5").Value.ToString());

                            index++;

                            RC.MoveNext();

                        }



                        
                        if (!DT_Result.IsEmpty)
                        {
                            var columns = Mat.Columns;

                            columns.Item("Code").DataBind.Bind("RESULT", "DocEntry");
                            columns.Item("M_Number").DataBind.Bind("RESULT", "M_Number");
                            columns.Item("P_Name").DataBind.Bind("RESULT", "P_Name");
                            columns.Item("National").DataBind.Bind("RESULT", "National");
                            columns.Item("C_Type").DataBind.Bind("RESULT", "C_Type");
                            columns.Item("P_Status").DataBind.Bind("RESULT", "P_Status");
                            columns.Item("A_Cost").DataBind.Bind("RESULT", "A_Cost");
                            columns.Item("R_Support").DataBind.Bind("RESULT", "R_Support");
                            columns.Item("A_Amount").DataBind.Bind("RESULT", "A_Amount");
                            columns.Item("T_Amount").DataBind.Bind("RESULT", "T_Amount");
                            columns.Item("R_Amount").DataBind.Bind("RESULT", "R_Amount");
                            columns.Item("P_Party").DataBind.Bind("RESULT", "P_Party");
                            columns.Item("P_Amount").DataBind.Bind("RESULT", "P_Amount");
                            columns.Item("Request_d").DataBind.Bind("RESULT", "Request_d");
                            columns.Item("User1").DataBind.Bind("RESULT", "User1");


                            Mat.LoadFromDataSource();
                        }

                            Check_Visiblity(form, Mat);
                        SBO_Application.StatusBar.SetText("Done!", BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Success);
                        form.Freeze(false);


                    }

                    


                    SBO_Application.StatusBar.SetText("Done!", BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Success);
                    form.Freeze(false);
                }

            }
            else
            {
                DateTime Begin_Date = new DateTime(2000, 1, 1);
                DateTime End_Date = new DateTime(2000, 1, 1);
                DateTime Begin_Age, End_Age;
                string Nationality = string.Empty, Cancer_type = string.Empty, Support_type = string.Empty, Treatment_plan = string.Empty, Treatment_type = string.Empty;
                double Support_amount = 0, Patient_amount = 0, Actual_coverage = 0;

                string SQL = $@"SELECT T0.""DocEntry"" As ""Code"",T1.""U_ST_MEDICAL_NUMBER"",T1.""U_ST_FULL_ARABIC_NAME"",T4.""Name"" as ""Nationality"",T1.""U_ST_DATE_OF_BIRTH"" ,T3.""Name"" as ""Cancer Type"" , 
                CASE WHEN T0.""U_ST_PATIENT_STATUS""= 'N' THEN  'New' WHEN T0.""U_ST_PATIENT_STATUS"" = 'E' THEN 'existed' END AS ""Patient_Status"" ,
                    T0.""U_ST_SUPPORT_AMOUNT"", T0.""U_ST_TOTAL_REQUESTED_FOR_THE_WHOLE_PERIOD"" ,  T1.""U_ST_COVERAGE"" , T1.""U_ST_COVERAGE_CCI"" , T3.""U_ST_AVERAGE_COST"" , T0.""U_ST_REQUESTED_SUPPORT"",
                T1.""U_ST_PREVIOUS_DIAGNOSIS_PLACE"" ,T6.""U_ST_PREVIOUS_COVERAGE_PARTY"", T6.""U_ST_PREVIOUS_COVERAGE_AMOUNT"",T0.""U_ST_REQUEST_DATE""  FROM ""@ST_COVERAGE_REQUEST""  T0 inner join
                ""@ST_PATIENTS_CARD"" T1 on T1.""Code"" = T0.""U_ST_PATIENT_CARD"" inner join ""@ST_SUPPORT_TYPE""  T2 on  T0.""U_ST_SUPPORT_TYPE"" = T2.""Code"" inner join ""@ST_CANCER_TYPE""  T3  on T3.""Code"" = T1.""U_ST_CANCER_TYPE"" inner join OCRY T4 on T1.""U_ST_NATIONAL_ID"" = T4.""Code"" inner join ""@ST_TREATMENT_PLAN"" T5 on T5.""Code"" = T0.""U_ST_TREATMENT_PLAN"" inner join ""@ST_SOCIAL_STUDY""  T6 on T1.""Code""=T6.""U_ST_PATIENT_CARD_ID""
                WHERE   T0.""U_ST_STATUS""  = 'O' 
                And  T0.""U_ST_PATIENT_TYPE""  Like '%G%'";
               // SQL = $@"SELECT T0.""Name"" FROM ""@ST_STEER_COMM_USERS""  T0";
                //SQL = $@"SELECT T0.""DocEntry"" As ""Code"",T1.""U_ST_MEDICAL_NUMBER"",T1.""U_ST_FULL_ARABIC_NAME"",T1.""U_ST_DATE_OF_BIRTH"" ,T3.""Name"" as ""Nationality"" ,T3.""Name"" as ""Cancer Type"" , 
                //    CASE WHEN T0.""U_ST_PATIENT_STATUS""= 'N' THEN  'New' WHEN T0.""U_ST_PATIENT_STATUS"" = 'E' THEN 'existed' END AS ""Patient_Status"" ,
                //        T0.""U_ST_SUPPORT_AMOUNT"", T0.""U_ST_TOTAL_REQUESTED_FOR_THE_WHOLE_PERIOD"" ,  T1.""U_ST_COVERAGE"" , T1.""U_ST_COVERAGE_CCI"" , T3.""U_ST_AVERAGE_COST"" , T0.""U_ST_REQUESTED_SUPPORT"",
                //    T1.""U_ST_PREVIOUS_DIAGNOSIS_PLACE"" ,T0.""U_ST_SUPPORT_AMOUNT"" as ""U_ST_PREVIOUS_COVERAGE_PARTY"", T0.""U_ST_SUPPORT_AMOUNT"" as ""U_ST_PREVIOUS_COVERAGE_AMOUNT"",T0.""U_ST_REQUEST_DATE""  FROM ""@ST_COVERAGE_REQUEST""  T0 inner join
                //    ""@ST_PATIENTS_CARD"" T1 on T1.""Code"" = T0.""U_ST_PATIENT_CARD"" inner join ""@ST_CANCER_TYPE""  T3  on T3.""Code"" = T1.""U_ST_CANCER_TYPE""";


                try
                {
                    form.Freeze(true);
                    if (form.DataSources.UserDataSources.Item("1").Value == "" || form.DataSources.UserDataSources.Item("2").Value == "")
                    {
                        throw new Logic.Custom_Exception("Please set the Date range");
                    }

                    else
                    {
                        Begin_Date = DateTime.ParseExact(form.DataSources.UserDataSources.Item("1").ValueEx, "yyyyMMdd", null);
                        End_Date = DateTime.ParseExact(form.DataSources.UserDataSources.Item("2").ValueEx, "yyyyMMdd", null);

                        // SQL += $@" And T0.""U_ST_REQUEST_DATE"" between to_date('{Begin_Date.ToString("MM/dd/yyyy")}', 'MM/DD/YYYY') and to_date('{End_Date.ToString("MM/dd/yyyy")}', 'MM/DD/YYYY')";

                        if (!string.IsNullOrEmpty(form.DataSources.UserDataSources.Item("3").Value.ToString()) &&
                            !string.IsNullOrEmpty(form.DataSources.UserDataSources.Item("4").Value.ToString()))
                        {
                            int begin_year = Convert.ToInt32(form.DataSources.UserDataSources.Item("3").Value.ToString());
                            Begin_Age = DateTime.Now.AddYears(begin_year * -1);
                            int end_year = Convert.ToInt32(form.DataSources.UserDataSources.Item("4").Value.ToString());
                            End_Age = DateTime.Now.AddYears(end_year * -1);

                            SQL += $@" And T1.""U_ST_DATE_OF_BIRTH"" between to_date('{Begin_Age.ToString("MM/dd/yyyy")}', 'MM/DD/YYYY') and to_date('{End_Age.ToString("MM/dd/yyyy")}', 'MM/DD/YYYY')";
                        }

                        if (!string.IsNullOrEmpty(form.DataSources.UserDataSources.Item("5").Value.ToString()))
                        {
                            Nationality = form.DataSources.UserDataSources.Item("5").Value;

                            SQL += $@" And T1.""U_ST_NATIONAL_ID"" = '{Nationality}'";
                        }
                        if (!string.IsNullOrEmpty(form.DataSources.UserDataSources.Item("6").Value.ToString()))
                        {
                            Cancer_type = form.DataSources.UserDataSources.Item("6").Value;

                            SQL += $@" And T1.""U_ST_CANCER_TYPE"" = '{Cancer_type}'";
                        }

                        if (!string.IsNullOrEmpty(form.DataSources.UserDataSources.Item("7").Value.ToString()))
                        {
                            Support_type = form.DataSources.UserDataSources.Item("7").Value;

                            SQL += $@" And T0.""U_ST_SUPPORT_TYPE"" = '{Support_type}'";
                        }

                        if (!string.IsNullOrEmpty(form.DataSources.UserDataSources.Item("11").Value.ToString()))
                        {
                            Treatment_plan = form.DataSources.UserDataSources.Item("11").Value;

                            if (Treatment_plan != "-")
                                SQL += $@" And T0.""U_ST_TREATMENT_PLAN"" = '{Treatment_plan}'";
                        }

                        if (!string.IsNullOrEmpty(form.DataSources.UserDataSources.Item("8").Value.ToString()))
                        {
                            Support_amount = Convert.ToDouble(form.DataSources.UserDataSources.Item("8").Value);

                            if (Support_amount != 0)
                                SQL += $@" And  T0.""U_ST_SUPPORT_AMOUNT"" = '{Support_amount}'";
                        }
                        if (!string.IsNullOrEmpty(form.DataSources.UserDataSources.Item("9").Value.ToString()))
                        {
                            Patient_amount = Convert.ToDouble(form.DataSources.UserDataSources.Item("9").Value);

                            if (Patient_amount != 0)
                                SQL += $@" And  T0.""U_ST_TOTAL_REQUESTED_FOR_THE_WHOLE_PERIOD"" = '{Patient_amount}'";
                        }
                        if (!string.IsNullOrEmpty(form.DataSources.UserDataSources.Item("10").Value.ToString()))
                        {
                            Actual_coverage = Convert.ToDouble(form.DataSources.UserDataSources.Item("10").Value);

                            if (Actual_coverage != 0)
                                SQL += $@" And  T0.""U_ST_SUPPORT_AMOUNT"" = '{Support_amount}'";
                        }
                        if (!string.IsNullOrEmpty(form.DataSources.UserDataSources.Item("15").Value.ToString()))
                        {
                            Treatment_type = form.DataSources.UserDataSources.Item("15").Value;

                            if (Treatment_type != "-")
                                SQL += $@" And T0.""U_ST_TREATMENT_TYPE"" = '{Treatment_type}'";
                        }

                        Recordset RC = Helper.Utility.Execute_Recordset_Query(company, SQL);
                        DataTable DT_Result = form.DataSources.DataTables.Item("RESULT");
                        int Count_All = RC.RecordCount;
                        int index = 0;
                        Matrix Mat = (Matrix)form.Items.Item("Item_2").Specific;

                        //DT_Result.ExecuteQuery(SQL);

                        if (Count_All > 0)
                        {
                            DT_Result.Rows.Clear();
                            DT_Result.Rows.Add(Count_All);
                            while (!RC.EoF)
                            {
                                //DT_Result.SetValue("DocEntry", index, RC.Fields.Item("DocEntry").Value.ToString());
                                //DT_Result.SetValue("M_Number", index, RC.Fields.Item("U_ST_MEDICAL_NUMBER").Value.ToString());
                                //DT_Result.SetValue("P_Name", index, RC.Fields.Item("U_ST_FULL_ARABIC_NAME").Value.ToString());
                                //DT_Result.SetValue("National", index, RC.Fields.Item("Nationality").Value.ToString());
                                //DT_Result.SetValue("C_Type", index, RC.Fields.Item("Cancer Type").Value.ToString());
                                //DT_Result.SetValue("A_Cost", index, RC.Fields.Item("U_ST_AVERAGE_COST").Value.ToString());
                                //DT_Result.SetValue("R_Support", index, RC.Fields.Item("U_ST_REQUESTED_SUPPORT").Value.ToString());
                                //DT_Result.SetValue("A_Amount", index, RC.Fields.Item("U_ST_COVERAGE").Value.ToString());
                                //DT_Result.SetValue("T_Amount", index, RC.Fields.Item("U_ST_TOTAL_REQUESTED_FOR_THE_WHOLE_PERIOD").Value.ToString());
                                //DT_Result.SetValue("R_Amount", index, RC.Fields.Item("U_ST_COVERAGE_CCI").Value.ToString());
                                //DT_Result.SetValue("P_Party", index, RC.Fields.Item("U_ST_PREVIOUS_COVERAGE_PARTY").Value.ToString());
                                //DT_Result.SetValue("P_Amount", index, RC.Fields.Item("U_ST_PREVIOUS_COVERAGE_AMOUNT").Value.ToString());


                                DT_Result.SetValue("DocEntry", index, RC.Fields.Item("Code").Value.ToString());
                                DT_Result.SetValue("M_Number", index, RC.Fields.Item("U_ST_MEDICAL_NUMBER").Value.ToString());
                                DT_Result.SetValue("P_Name", index, RC.Fields.Item("U_ST_FULL_ARABIC_NAME").Value.ToString());
                                DT_Result.SetValue("National", index, RC.Fields.Item("Nationality").Value.ToString());
                                DT_Result.SetValue("Age", index, RC.Fields.Item("U_ST_DATE_OF_BIRTH").Value.ToString());
                                DT_Result.SetValue("C_Type", index, RC.Fields.Item("Cancer Type").Value.ToString());
                                DT_Result.SetValue("P_Status", index, RC.Fields.Item("Patient_Status").Value.ToString());
                                DT_Result.SetValue("A_Cost", index, RC.Fields.Item("U_ST_COVERAGE").Value.ToString());
                                DT_Result.SetValue("R_Support", index, RC.Fields.Item("U_ST_COVERAGE").Value.ToString());
                                DT_Result.SetValue("A_Amount", index, RC.Fields.Item("U_ST_COVERAGE").Value.ToString());
                                DT_Result.SetValue("T_Amount", index, RC.Fields.Item("U_ST_COVERAGE").Value.ToString());
                                DT_Result.SetValue("R_Amount", index, RC.Fields.Item("U_ST_COVERAGE_CCI").Value.ToString());
                                DT_Result.SetValue("P_Party", index, RC.Fields.Item("U_ST_COVERAGE").Value.ToString());
                                DT_Result.SetValue("P_Amount", index, RC.Fields.Item("U_ST_COVERAGE").Value.ToString());
                                DT_Result.SetValue("Request_d", index, RC.Fields.Item("U_ST_REQUEST_DATE").Value.ToString());
                                string user1_query1 = $@"SELECT T0.""Name"" FROM ""@ST_STEER_COMM_USERS""  T0";
                                Recordset RC_User11 = Helper.Utility.Execute_Recordset_Query(company, user1_query1);
                                if (RC_User11.RecordCount == 5)
                                {
                                    DT_Result.SetValue("User1", index, RC_User11.Fields.Item("Name").Value.ToString());

                                    RC_User11.MoveNext();
                                    DT_Result.SetValue("User2", index, RC_User11.Fields.Item("Name").Value.ToString());

                                    RC_User11.MoveNext();
                                    DT_Result.SetValue("User3", index, RC_User11.Fields.Item("Name").Value.ToString());

                                    RC_User11.MoveNext();
                                    DT_Result.SetValue("User4", index, RC_User11.Fields.Item("Name").Value.ToString());

                                    RC_User11.MoveNext();
                                    DT_Result.SetValue("User5", index, RC_User11.Fields.Item("Name").Value.ToString());

                                }
                                else
                                {
                                    DT_Result.SetValue("User1", index, " ");
                                    DT_Result.SetValue("User2", index, " ");
                                    DT_Result.SetValue("User3", index, " ");
                                    DT_Result.SetValue("User4", index, " ");
                                    DT_Result.SetValue("User5", index, " ");
                                }
                                DT_Result.SetValue("good", index, "");
                                DT_Result.SetValue("good1", index, "");
                                DT_Result.SetValue("good2", index, "");
                                DT_Result.SetValue("good3", index, "");
                                DT_Result.SetValue("amount1", index, "0");
                                DT_Result.SetValue("remark1", index, "   ");
                                DT_Result.SetValue("amount3", index, "0");
                                DT_Result.SetValue("remark3", index, "   ");
                                DT_Result.SetValue("amount4", index, "0");
                                DT_Result.SetValue("remark4", index, "   ");
                                DT_Result.SetValue("amount5", index, "0");
                                DT_Result.SetValue("remark5", index, "   ");
                                index++;

                                RC.MoveNext();

                            }



                            int k = 0;
                            if (!DT_Result.IsEmpty)
                            {
                                var columns = Mat.Columns;

                                columns.Item("Code").DataBind.Bind("RESULT", "DocEntry");
                                columns.Item("M_Number").DataBind.Bind("RESULT", "M_Number");
                                columns.Item("P_Name").DataBind.Bind("RESULT", "P_Name");
                                columns.Item("National").DataBind.Bind("RESULT", "National");
                                columns.Item("C_Type").DataBind.Bind("RESULT", "C_Type");
                                columns.Item("P_Status").DataBind.Bind("RESULT", "P_Status");
                                columns.Item("A_Cost").DataBind.Bind("RESULT", "A_Cost");
                                columns.Item("R_Support").DataBind.Bind("RESULT", "R_Support");
                                columns.Item("A_Amount").DataBind.Bind("RESULT", "A_Amount");
                                columns.Item("T_Amount").DataBind.Bind("RESULT", "T_Amount");
                                columns.Item("R_Amount").DataBind.Bind("RESULT", "R_Amount");
                                columns.Item("P_Party").DataBind.Bind("RESULT", "P_Party");
                                columns.Item("P_Amount").DataBind.Bind("RESULT", "P_Amount");
                                columns.Item("Request_d").DataBind.Bind("RESULT", "Request_d");
                                columns.Item("User1").DataBind.Bind("RESULT", "User1");

                                while (k < DT_Result.Rows.Count)
                                {
                                    SAPbouiCOM.ComboBox cmb = (ComboBox)columns.Item("good").Cells.Item(k).Specific;

                                    string Good_query = $@"SELECT  T0.""U_ST_NAME"" FROM ""@ST_GOOD_WILL_FUNDS""  T0";

                                    Recordset RC1 = Helper.Utility.Execute_Recordset_Query(company, Good_query);

                                    int Count_All1 = RC1.RecordCount;
                                    //  int index = 0;
                                    if (Count_All1 > 0)
                                    {

                                        while (!RC1.EoF)
                                        {
                                            cmb.ValidValues.Add(RC1.Fields.Item(0).Value.ToString(), RC1.Fields.Item(0).Value.ToString());

                                            RC1.MoveNext();
                                        }

                                    }
                                    k++;
                                }
                                Mat.LoadFromDataSource();
                            }

                                Check_Visiblity(form, Mat);
                            SBO_Application.StatusBar.SetText("Done!", BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Success);
                            form.Freeze(false);


                        }

                        
                          // Check_Visiblity(form, Mat);

                        //int k1 = 0;
                        //while (k1 < DT_Result.Rows.Count) 
                        //{
                        //    SAPbouiCOM.ComboBox cmb = (ComboBox)columns1.Item(17).Cells.Item(k1).Specific;

                        //    string Good_query = $@"SELECT  T0.""U_ST_NAME"" FROM ""@ST_GOOD_WILL_FUNDS""  T0";

                        //    Recordset RC1 = Helper.Utility.Execute_Recordset_Query(company, Good_query);

                        //    int Count_All1 = RC1.RecordCount;
                        //    //  int index = 0;
                        //    if (Count_All1 > 0)
                        //    {

                        //        while (!RC1.EoF)
                        //        {
                        //            cmb.ValidValues.Add(RC1.Fields.Item(0).Value.ToString(), RC1.Fields.Item(0).Value.ToString());

                        //            RC1.MoveNext();
                        //        }

                        //    }
                        //    k1++;
                        //}


                        SBO_Application.StatusBar.SetText("Done!", BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Success);
                        form.Freeze(false);
                    }
                }
                catch (Exception ex)
                {
                    SBO_Application.SetStatusBarMessage(ex.Message, BoMessageTime.bmt_Short, true);
                }
                finally
                {
                    form.Freeze(false);
                }
            }
         
        }
        private static void Close(ItemEvent pVal)
        {
            if (SBO_Application.MessageBox("Are you sure you want to Close the Form?", 1, "Yes", "No") == 1)
            {
                Form form = SBO_Application.Forms.Item(pVal.FormUID);
                form.Close();

            }

        }

        private static void Add_List(ItemEvent pVal)
        {
            Form form = SBO_Application.Forms.Item(pVal.FormUID);
            UserTable UDT_Waiting_List = company.UserTables.Item("ST_Waiting_List");
            DataTable DT_Result = form.DataSources.DataTables.Item("RESULT");
            string c_date=form.DataSources.UserDataSources.Item("UD_12").Value;

            for (int i = 0; i < DT_Result.Rows.Count; i++)
            {
                UDT_Waiting_List.Code = Helper.Utility.getCode("@ST_Waiting_List", company);
                UDT_Waiting_List.Name = UDT_Waiting_List.Code;
                UDT_Waiting_List.UserFields.Fields.Item("ST_CREATION_DATE").Value = c_date;

                for (int J = 0; J < DT_Result.Columns.Count; J++)
                {
                    UDT_Waiting_List.UserFields.Fields.Item("U_ST_" + DT_Result.Columns.Item(J).Name).Value = DT_Result.GetValue(J, i);
                }
               
                if (UDT_Waiting_List.Add() != 0)
                {
                    throw new Logic.Custom_Exception("Error during add entry to the Change Log");
                }
                Add_Items(form, false);
                if(isUser())
                {
                    Update_Items(form, true); 
                }
            }
        }

        private static void Update_List(ItemEvent pVal)
        {
            Form form = SBO_Application.Forms.Item(pVal.FormUID);
            UserTable UDT_Waiting_List = company.UserTables.Item("ST_Waiting_List");
            DataTable DT_Result = form.DataSources.DataTables.Item("RESULT");
            string c_date = form.DataSources.UserDataSources.Item("UD_12").Value;

            for (int i = 0; i < DT_Result.Rows.Count; i++)
            {
                UDT_Waiting_List.Code = form.DataSources.UserDataSources.Item("UD_13").Value;
                UDT_Waiting_List.Name = UDT_Waiting_List.Code;
                UDT_Waiting_List.UserFields.Fields.Item("ST_CREATION_DATE").Value = c_date;

                for (int J = 0; J < DT_Result.Columns.Count; J++)
                {
                    UDT_Waiting_List.UserFields.Fields.Item("U_ST_" + DT_Result.Columns.Item(J).Name).Value = DT_Result.GetValue(J, i);
                }

                if (UDT_Waiting_List.Update() != 0)
                {
                    throw new Logic.Custom_Exception("Error during add entry to the Change Log");
                }
                Add_Items(form, false);
                if (isUser())
                {
                    Update_Items(form, true);
                }
            }
        }
        private static void Add_Items(Form form,bool visible)
        {

            try
            {
                Item Btn_add = form.Items.Item("Item_8");//, BoFormItemTypes.it_BUTTON);
                Btn_add.Visible = visible;               

            }
            catch (Exception ex)
            {
               // SBO_Application.StatusBar.SetText(ex.Message, BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Error);
            }
        }
        private static void Update_Items(Form form,bool visible)
        {

            try
            {
                Item Btn_update = form.Items.Item("Item_6");//, BoFormItemTypes.it_BUTTON);
                Btn_update.Visible = visible;               

            }
            catch (Exception ex)
            {
               // SBO_Application.StatusBar.SetText(ex.Message, BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Error);
            }
        }

        private static bool isUser()
        {
            bool result = false;
            string user1_query1 = $@"SELECT T0.""Name"" FROM ""@ST_STEER_COMM_USERS""  T0";
            Recordset RC_User11 = Helper.Utility.Execute_Recordset_Query(company, user1_query1);
            string current_user = company.UserName;
            if (RC_User11.RecordCount == 5)
            {
                while (!RC_User11.EoF)
                {
                    if (RC_User11.Fields.Item("Name").Value.ToString().Equals(current_user)) 
                    {
                        result = true;
                        break;
                    }
                    RC_User11.MoveNext();
                }

            }

            return result;
        }

        private static void Check_Visiblity(Form form,Matrix Mat)
        {
           // string v = company.UserName;
            
            if (form.DataSources.UserDataSources.Item("UD_14").Value.Equals("N") && isUser()) 
            {
                foreach (Column col in Mat.Columns)
                {
                    col.Visible = true;
                }
            }
            else 
            {
            string user1_query = $@"SELECT T0.""Name"" FROM ""@ST_STEER_COMM_USERS""  T0";
            Recordset RC_User1 = Helper.Utility.Execute_Recordset_Query(company, user1_query);
            bool find = false;
            if (RC_User1.RecordCount == 5)
            {
                string current_user = company.UserName.ToString();

                //((Button)Btn_GR_GEN.Specific)
                string check_value = form.DataSources.UserDataSources.Item("UD_14").Value;
                if (RC_User1.Fields.Item("Name").Value.ToString().Equals(current_user))
                {
                    find = true;
                    if (check_value=="Y")
                    {
                        Column colU1 = Mat.Columns.Item("User2");//.Cells.Item(pVal.Row).Specific;
                        colU1.Visible = false;

                        Column colU2 = Mat.Columns.Item("User3");//.Cells.Item(pVal.Row).Specific;
                        colU2.Visible = false;

                        Column colU3 = Mat.Columns.Item("User4");//.Cells.Item(pVal.Row).Specific;
                        colU3.Visible = false;

                        Column colU4 = Mat.Columns.Item("User5");//.Cells.Item(pVal.Row).Specific;
                        colU4.Visible = false;

                        Column col = Mat.Columns.Item("good2");//.Cells.Item(pVal.Row).Specific;
                        col.Visible = false;

                        Column col2 = Mat.Columns.Item("good3");//.Cells.Item(pVal.Row).Specific;
                        col2.Visible = false;

                        Column col3 = Mat.Columns.Item("good4");//.Cells.Item(pVal.Row).Specific;
                        col3.Visible = false;

                        Column col4 = Mat.Columns.Item("good5");//.Cells.Item(pVal.Row).Specific;
                        col4.Visible = false;

                        Column colA1 = Mat.Columns.Item("amount2");//.Cells.Item(pVal.Row).Specific;
                        colA1.Visible = false;

                        Column colA2 = Mat.Columns.Item("amount3");//.Cells.Item(pVal.Row).Specific;
                        colA2.Visible = false;

                        Column colA3 = Mat.Columns.Item("amount4");//.Cells.Item(pVal.Row).Specific;
                        colA3.Visible = false;

                        Column colA4 = Mat.Columns.Item("amount5");//.Cells.Item(pVal.Row).Specific;
                        colA4.Visible = false;

                        Column colR1 = Mat.Columns.Item("remark2");//.Cells.Item(pVal.Row).Specific;
                        colR1.Visible = false;

                        Column colR2 = Mat.Columns.Item("remark3");//.Cells.Item(pVal.Row).Specific;
                        colR2.Visible = false;

                        Column colR3 = Mat.Columns.Item("remark4");//.Cells.Item(pVal.Row).Specific;
                        colR3.Visible = false;

                        Column colR4 = Mat.Columns.Item("remark5");//.Cells.Item(pVal.Row).Specific;
                        colR4.Visible = false;
                    }

                }

                RC_User1.MoveNext();
                if (RC_User1.Fields.Item("Name").Value.ToString().Equals(current_user))// && (((CheckBox)check_hide.Specific).Checked))
                {
                    if (check_value == "Y")
                    {
                        find = true;
                        Column colU1 = Mat.Columns.Item("User1");//.Cells.Item(pVal.Row).Specific;
                        colU1.Visible = false;

                        Column colU2 = Mat.Columns.Item("User3");//.Cells.Item(pVal.Row).Specific;
                        colU2.Visible = false;

                        Column colU3 = Mat.Columns.Item("User4");//.Cells.Item(pVal.Row).Specific;
                        colU3.Visible = false;

                        Column col = Mat.Columns.Item("good");//.Cells.Item(pVal.Row).Specific;
                        col.Visible = false;

                        Column col2 = Mat.Columns.Item("good3");//.Cells.Item(pVal.Row).Specific;
                        col2.Visible = false;

                        Column col3 = Mat.Columns.Item("good4");//.Cells.Item(pVal.Row).Specific;
                        col3.Visible = false;

                        Column colA1 = Mat.Columns.Item("amount1");//.Cells.Item(pVal.Row).Specific;
                        colA1.Visible = false;

                        Column colA2 = Mat.Columns.Item("amount3");//.Cells.Item(pVal.Row).Specific;
                        colA2.Visible = false;

                        Column colA3 = Mat.Columns.Item("amount4");//.Cells.Item(pVal.Row).Specific;
                        colA3.Visible = false;

                        Column colR1 = Mat.Columns.Item("remark1");//.Cells.Item(pVal.Row).Specific;
                        colR1.Visible = false;

                        Column colR2 = Mat.Columns.Item("remark3");//.Cells.Item(pVal.Row).Specific;
                        colR2.Visible = false;

                        Column colR3 = Mat.Columns.Item("remark4");//.Cells.Item(pVal.Row).Specific;
                        colR3.Visible = false;

                    }
                }

                RC_User1.MoveNext();
                if (RC_User1.Fields.Item("Name").Value.ToString().Equals(current_user))// && (((CheckBox)check_hide.Specific).Checked))
                {
                    find = true;
                    if (check_value == "Y")
                    {
                        Column colU1 = Mat.Columns.Item("User1");//.Cells.Item(pVal.Row).Specific;
                        colU1.Visible = false;

                        Column colU2 = Mat.Columns.Item("User2");//.Cells.Item(pVal.Row).Specific;
                        colU2.Visible = false;

                        Column colU3 = Mat.Columns.Item("User4");//.Cells.Item(pVal.Row).Specific;
                        colU3.Visible = false;

                        Column colU4 = Mat.Columns.Item("User5");//.Cells.Item(pVal.Row).Specific;
                        colU4.Visible = false;

                        Column col = Mat.Columns.Item("good");//.Cells.Item(pVal.Row).Specific;
                        col.Visible = false;

                        Column col2 = Mat.Columns.Item("good2");//.Cells.Item(pVal.Row).Specific;
                        col2.Visible = false;

                        Column col3 = Mat.Columns.Item("good4");//.Cells.Item(pVal.Row).Specific;
                        col3.Visible = false;

                        Column col4 = Mat.Columns.Item("good5");//.Cells.Item(pVal.Row).Specific;
                        col4.Visible = false;

                        Column colA1 = Mat.Columns.Item("amount1");//.Cells.Item(pVal.Row).Specific;
                        colA1.Visible = false;

                        Column colA2 = Mat.Columns.Item("amount2");//.Cells.Item(pVal.Row).Specific;
                        colA2.Visible = false;

                        Column colA3 = Mat.Columns.Item("amount4");//.Cells.Item(pVal.Row).Specific;
                        colA3.Visible = false;

                        Column colA4 = Mat.Columns.Item("amount5");//.Cells.Item(pVal.Row).Specific;
                        colA4.Visible = false;

                        Column colR1 = Mat.Columns.Item("remark1");//.Cells.Item(pVal.Row).Specific;
                        colR1.Visible = false;

                        Column colR2 = Mat.Columns.Item("remark2");//.Cells.Item(pVal.Row).Specific;
                        colR2.Visible = false;

                        Column colR3 = Mat.Columns.Item("remark4");//.Cells.Item(pVal.Row).Specific;
                        colR3.Visible = false;

                        Column colR4 = Mat.Columns.Item("remark5");//.Cells.Item(pVal.Row).Specific;
                        colR4.Visible = false;
                    }
                }
                RC_User1.MoveNext();
                if (RC_User1.Fields.Item("Name").Value.ToString().Equals(current_user))// && (((CheckBox)check_hide.Specific).Checked))
                {
                    find = true;
                    if (check_value == "Y")
                    {
                        Column colU1 = Mat.Columns.Item("User1");//.Cells.Item(pVal.Row).Specific;
                        colU1.Visible = false;

                        Column colU2 = Mat.Columns.Item("User2");//.Cells.Item(pVal.Row).Specific;
                        colU2.Visible = false;

                        Column colU3 = Mat.Columns.Item("User3");//.Cells.Item(pVal.Row).Specific;
                        colU3.Visible = false;

                        Column colU4 = Mat.Columns.Item("User5");//.Cells.Item(pVal.Row).Specific;
                        colU4.Visible = false;

                        Column col = Mat.Columns.Item("good");//.Cells.Item(pVal.Row).Specific;
                        col.Visible = false;

                        Column col2 = Mat.Columns.Item("good2");//.Cells.Item(pVal.Row).Specific;
                        col2.Visible = false;

                        Column col3 = Mat.Columns.Item("good3");//.Cells.Item(pVal.Row).Specific;
                        col3.Visible = false;

                        Column col4 = Mat.Columns.Item("good5");//.Cells.Item(pVal.Row).Specific;
                        col4.Visible = false;

                        Column colA1 = Mat.Columns.Item("amount1");//.Cells.Item(pVal.Row).Specific;
                        colA1.Visible = false;

                        Column colA2 = Mat.Columns.Item("amount2");//.Cells.Item(pVal.Row).Specific;
                        colA2.Visible = false;

                        Column colA3 = Mat.Columns.Item("amount3");//.Cells.Item(pVal.Row).Specific;
                        colA3.Visible = false;

                        Column colA4 = Mat.Columns.Item("amount5");//.Cells.Item(pVal.Row).Specific;
                        colA4.Visible = false;

                        Column colR1 = Mat.Columns.Item("remark1");//.Cells.Item(pVal.Row).Specific;
                        colR1.Visible = false;

                        Column colR2 = Mat.Columns.Item("remark2");//.Cells.Item(pVal.Row).Specific;
                        colR2.Visible = false;

                        Column colR3 = Mat.Columns.Item("remark3");//.Cells.Item(pVal.Row).Specific;
                        colR3.Visible = false;

                        Column colR4 = Mat.Columns.Item("remark5");//.Cells.Item(pVal.Row).Specific;
                        colR4.Visible = false;

                    }

                }
                RC_User1.MoveNext();
                if (RC_User1.Fields.Item("Name").Value.ToString().Equals(current_user))// && (((CheckBox)check_hide.Specific).Checked))
                {
                    find = true;
                    if (check_value == "Y")
                    {
                        Column colU1 = Mat.Columns.Item("User1");//.Cells.Item(pVal.Row).Specific;
                        colU1.Visible = false;

                        Column colU2 = Mat.Columns.Item("User2");//.Cells.Item(pVal.Row).Specific;
                        colU2.Visible = false;

                        Column colU3 = Mat.Columns.Item("User3");//.Cells.Item(pVal.Row).Specific;
                        colU3.Visible = false;

                        Column colU4 = Mat.Columns.Item("User4");//.Cells.Item(pVal.Row).Specific;
                        colU4.Visible = false;

                        Column col = Mat.Columns.Item("good");//.Cells.Item(pVal.Row).Specific;
                        col.Visible = false;

                        Column col2 = Mat.Columns.Item("good2");//.Cells.Item(pVal.Row).Specific;
                        col2.Visible = false;

                        Column col3 = Mat.Columns.Item("good3");//.Cells.Item(pVal.Row).Specific;
                        col3.Visible = false;

                        Column col4 = Mat.Columns.Item("good4");//.Cells.Item(pVal.Row).Specific;
                        col4.Visible = false;

                        Column colA1 = Mat.Columns.Item("amount1");//.Cells.Item(pVal.Row).Specific;
                        colA1.Visible = false;

                        Column colA2 = Mat.Columns.Item("amount2");//.Cells.Item(pVal.Row).Specific;
                        colA2.Visible = false;

                        Column colA3 = Mat.Columns.Item("amount3");//.Cells.Item(pVal.Row).Specific;
                        colA3.Visible = false;

                        Column colA4 = Mat.Columns.Item("amount4");//.Cells.Item(pVal.Row).Specific;
                        colA4.Visible = false;

                        Column colR1 = Mat.Columns.Item("remark1");//.Cells.Item(pVal.Row).Specific;
                        colR1.Visible = false;

                        Column colR2 = Mat.Columns.Item("remark2");//.Cells.Item(pVal.Row).Specific;
                        colR2.Visible = false;

                        Column colR3 = Mat.Columns.Item("remark3");//.Cells.Item(pVal.Row).Specific;
                        colR3.Visible = false;

                        Column colR4 = Mat.Columns.Item("remark4");//.Cells.Item(pVal.Row).Specific;
                        colR4.Visible = false;
                    }

                }
                Mat.AutoResizeColumns();


            }

            if (!find)
            {
                Mat = (Matrix)form.Items.Item("Item_2").Specific;

                Column colU = Mat.Columns.Item("User1");//.Cells.Item(pVal.Row).Specific;
                colU.Visible = false;

                Column colU1 = Mat.Columns.Item("User2");//.Cells.Item(pVal.Row).Specific;
                colU1.Visible = false;

                Column colU2 = Mat.Columns.Item("User3");//.Cells.Item(pVal.Row).Specific;
                colU2.Visible = false;

                Column colU3 = Mat.Columns.Item("User4");//.Cells.Item(pVal.Row).Specific;
                colU3.Visible = false;

                Column colU4 = Mat.Columns.Item("User5");//.Cells.Item(pVal.Row).Specific;
                colU4.Visible = false;


                Column co1l = Mat.Columns.Item("good");//.Cells.Item(pVal.Row).Specific;
                co1l.Visible = false;

                Column col = Mat.Columns.Item("good2");//.Cells.Item(pVal.Row).Specific;
                col.Visible = false;

                Column col2 = Mat.Columns.Item("good3");//.Cells.Item(pVal.Row).Specific;
                col2.Visible = false;

                Column col3 = Mat.Columns.Item("good4");//.Cells.Item(pVal.Row).Specific;
                col3.Visible = false;

                Column col4 = Mat.Columns.Item("good5");//.Cells.Item(pVal.Row).Specific;
                col4.Visible = false;

                Column colA11 = Mat.Columns.Item("amount1");//.Cells.Item(pVal.Row).Specific;
                colA11.Visible = false;


                Column colA1 = Mat.Columns.Item("amount2");//.Cells.Item(pVal.Row).Specific;
                colA1.Visible = false;

                Column colA2 = Mat.Columns.Item("amount3");//.Cells.Item(pVal.Row).Specific;
                colA2.Visible = false;

                Column colA3 = Mat.Columns.Item("amount4");//.Cells.Item(pVal.Row).Specific;
                colA3.Visible = false;

                Column colA4 = Mat.Columns.Item("amount5");//.Cells.Item(pVal.Row).Specific;
                colA4.Visible = false;

                Column colR11 = Mat.Columns.Item("remark1");//.Cells.Item(pVal.Row).Specific;
                colR11.Visible = false;

                Column colR1 = Mat.Columns.Item("remark2");//.Cells.Item(pVal.Row).Specific;
                colR1.Visible = false;

                Column colR2 = Mat.Columns.Item("remark3");//.Cells.Item(pVal.Row).Specific;
                colR2.Visible = false;

                Column colR3 = Mat.Columns.Item("remark4");//.Cells.Item(pVal.Row).Specific;
                colR3.Visible = false;

                Column colR4 = Mat.Columns.Item("remark5");//.Cells.Item(pVal.Row).Specific;
                colR4.Visible = false;

                Mat.AutoResizeColumns();
            }
            }
        }
    }
}
