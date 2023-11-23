using SAPbobsCOM;
using SAPbouiCOM;
using ST.KHCF.Customization.Logic;
using ST.KHCF.Customization.Logic.Classes;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ST.KHCF.Customization.Forms.CCI
{
    class Frm_Membership_Renewal
    {
        internal static SAPbobsCOM.Company company;
        internal static Application SBO_Application;

        internal static Form Create_Form()
        {
            var form_params = (FormCreationParams)SBO_Application.CreateObject(BoCreatableObjectType.cot_FormCreationParams);
            form_params.XmlData = Properties.Resources.Frm_Membership_Renewal;
            var form = SBO_Application.Forms.AddEx(form_params);
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
            //File.AppendAllText("C:\\Users\\wsaleh\\Desktop\\CCI_TimeLog.txt", "CCI Membership Renewal" + Environment.NewLine);
            //string startTimeExact = DateTime.Now.ToString("mm:ss:f");
            //DateTime startTime = DateTime.Now;
            //File.AppendAllText("C:\\Users\\wsaleh\\Desktop\\CCI_TimeLog.txt", "Start Time:" + startTimeExact + Environment.NewLine);
            form.DataSources.UserDataSources.Item("4").Value = "I";
            //form.DataSources.UserDataSources.Item("6").Value = "-";
            //form.DataSources.UserDataSources.Item("8").Value = "-";

            Grid Grd_Result = (Grid)form.Items.Item("16").Specific;
            string SQL_Cov = $@"SELECT ""Code"" , ""Name"" FROM ""@ST_COVERAGE""";
            Helper.Utility.FillGridComboBoxForSQL(company, Grd_Result, "U_ST_COVERAGE", SQL_Cov);
            //DateTime prev = Utility.Add_Time_Log("C", "Coverage", startTime);
            //prev = Utility.Add_Time_Log("C", "", startTime, true);

            //ComboBoxColumn Col_Apprv_Status = (ComboBoxColumn)Grd_Result.Columns.Item("U_ST_APPROVAL_STATUS");
            //Col_Apprv_Status.ValidValues.Add("P", "Pending");
            //Col_Apprv_Status.ValidValues.Add("A", "Approved");
            //Col_Apprv_Status.ValidValues.Add("R", "Rejected");
            //Col_Apprv_Status.DisplayType = BoComboDisplayType.cdt_Description;
        }

        internal static void SBO_Application_ItemEvent(string FormUID, ref ItemEvent pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            if (pVal.FormTypeEx != "ST_Membership_Renewal")
            {
                return;
            }
            try
            {
                if (pVal.ItemUID == "18" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Filter(pVal);
                }
                if (pVal.ItemUID == "19" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Set_Approval(pVal, "R");
                }
                if (pVal.ItemUID == "20" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Set_Approval(pVal, "A");
                }
                if (pVal.ItemUID == "9" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Create_Renewal(pVal);
                }
                if (pVal.ItemUID == "23" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Select_Path(pVal);
                }
                if (pVal.ItemUID == "24" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Preview(pVal);
                }

            }
            catch (Exception ex)
            {
                Form form = SBO_Application.Forms.Item(FormUID);
                form.Freeze(false);
                SBO_Application.StatusBar.SetText(ex.Message, BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Error);
            }
        }

        private static void Preview(ItemEvent pVal)
        {
            Form form = SBO_Application.Forms.Item(pVal.FormUID);
            form.Freeze(true);
            // DataTable DT_Result = form.DataSources.DataTables.Item("RESULT");

            Membership.Renewal_Data[] Renewal_Data = Get_Renewal_Data_From_File(form);
            //string National_IDs_Text = string.Join("','", Renewal_Data.Select(R => R.NATIONAL_ID.Trim()));
            //National_IDs_Text = $"'{National_IDs_Text}'";

            string TableName = "";
            string CardMember_Table = "";
            string Full_Name = "";
            if (form.DataSources.UserDataSources.Item("4").Value == "I")
            {
                TableName = "ST_INDIV_MEMBERSHIP";
                CardMember_Table = "ST_CCI_INDIV_CARD";
                Full_Name = "U_ST_FULL_NAME_AR";
            }
            else
            {
                TableName = "ST_CORP_MEMBERSHIP";
                CardMember_Table = "ST_CCI_CORP_CARD";
                Full_Name = "U_ST_CORPORATE_ARABIC_NAME";
            }
            //string Renewal_Type = form.DataSources.UserDataSources.Item("6").Value;
            //string Approval_Status = form.DataSources.UserDataSources.Item("8").Value;

            string[] Renewal_Fields = new string[] { "NATIONAL_ID", "IS_ACCEPTED", "REASON", "NEW_COVERAGE", "CREATE_INV_AUTO" };

            DataTable DT_Membership = form.DataSources.DataTables.Item("RESULT");
            DT_Membership.Rows.Clear();

            DT_Membership.Rows.Add(Renewal_Data.Length);

            //for (int i = 0; i < RC_Membership.RecordCount; i++)
            //{
            int Index = 0;
            foreach (Membership.Renewal_Data One_Data in Renewal_Data)
            {

                string SQL_Result = $@"SELECT Top 1 T0.""Code"", T0.U_ST_MEMBER_CARD, T0.U_ST_CREATION_DATE, T1.U_ST_NATIONAL_ID
, T0.""U_ST_START_DATE"", T0.U_ST_END_DATE, T0.""U_ST_ACTIVE"", T1.U_ST_AUTOMATIC_RENEWAL , T0.U_ST_COVERAGE, T0.U_ST_APPROVAL_STATUS, T0.U_ST_APPROVAL_NOTE
,T1.{Full_Name} AS ""Name""
FROM ""@{TableName}"" T0 INNER JOIN ""@{CardMember_Table}""  T1 ON T0.U_ST_MEMBER_CARD = T1.""Code"" 
WHERE  T1.U_ST_NATIONAL_ID = '{One_Data.NATIONAL_ID}'
ORDER BY U_ST_END_DATE DESC
";
                Recordset RC_Membership = Helper.Utility.Execute_Recordset_Query(company, SQL_Result);
                if (RC_Membership.RecordCount == 0)
                {
                    throw new Logic.Custom_Exception($"There is no Membership data for the National ID[{One_Data.NATIONAL_ID}]");
                }
                for (int J = 0; J < DT_Membership.Columns.Count; J++)
                {
                    string Col_Name = DT_Membership.Columns.Item(J).Name;
                    string UDF_Name;
                    if (Renewal_Fields.Contains(Col_Name) || Col_Name == "Note")
                    {
                        continue;
                    }
                    UDF_Name = Col_Name;

                    DT_Membership.SetValue(Col_Name, Index, RC_Membership.Fields.Item(UDF_Name).Value);
                    string National_ID = RC_Membership.Fields.Item("U_ST_NATIONAL_ID").Value.ToString();

                    //Membership.Renewal_Data One_Data = Renewal_Data.FirstOrDefault(R => R.NATIONAL_ID == National_ID);
                    DT_Membership.SetValue("NATIONAL_ID", Index, One_Data.NATIONAL_ID);
                    DT_Membership.SetValue("REASON", Index, One_Data.REASON);
                    DT_Membership.SetValue("IS_ACCEPTED", Index, One_Data.IS_ACCEPTED);
                    DT_Membership.SetValue("NEW_COVERAGE", Index, One_Data.NEW_COVERAGE);
                    DT_Membership.SetValue("CREATE_INV_AUTO", Index, One_Data.CREATE_INV_AUTO);

                }
                Index++;
            }
            //    RC_Membership.MoveNext();
            //}
            Grid Grd_Membership = (Grid)form.Items.Item("16").Specific;

            Grd_Membership.AutoResizeColumns();
            form.Freeze(false);



        }

        private static Membership.Renewal_Data[] Get_Renewal_Data_From_File(Form form)
        {
            List<Membership.Renewal_Data> Result = new List<Membership.Renewal_Data>();

            string CSV_Delimiter = Configurations.Get_File_Delimiter(company);
            string File_Path = form.DataSources.UserDataSources.Item("22").Value;
            string[] Lines = System.IO.File.ReadAllLines(File_Path);

            if (Lines.Length <= 1)
            {
                throw new Logic.Custom_Exception($@"The File[{File_Path}] is Empty");
            }
            //string[] First_Line = Lines[0].Split(CSV_Delimiter.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            for (int i = 1; i < Lines.Length; i++)
            {
                string OneLine = Lines[i];
                string[] Split_Line = OneLine.Split(CSV_Delimiter.ToCharArray());
                Membership.Renewal_Data One_Data = new Membership.Renewal_Data();
                One_Data.NATIONAL_ID = Split_Line[0];
                One_Data.IS_ACCEPTED = Split_Line[1];
                One_Data.REASON = Split_Line[2];
                One_Data.NEW_COVERAGE = Split_Line[3];
                One_Data.CREATE_INV_AUTO = Split_Line[4];
                Result.Add(One_Data);
            }

            return Result.ToArray();
        }

        private static void Set_Approval(ItemEvent pVal, string Approval_Value)
        {
            Form form = SBO_Application.Forms.Item(pVal.FormUID);
            UDO_Definition UDO_Info;
            if (form.DataSources.UserDataSources.Item("4").Value == "I")
            {
                UDO_Info = Logic.Objects_Logic.All_UDO_Definition.FirstOrDefault(U => U.KHCF_Object == KHCF_Objects.Individual_Membership);
            }
            else
            {
                UDO_Info = Logic.Objects_Logic.All_UDO_Definition.FirstOrDefault(U => U.KHCF_Object == KHCF_Objects.Corporate_Membership);
            }
            DataTable DT_Result = form.DataSources.DataTables.Item("RESULT");

            SBO_Application.StatusBar.SetText("Begin!", BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Warning);
            form.Freeze(true);
            for (int i = 0; i < DT_Result.Rows.Count; i++)
            {
                if (DT_Result.GetValue("SELECTED", 0).ToString() != "Y")
                {
                    continue;
                }
                try
                {
                    string Membership_Code = DT_Result.GetValue("Code", 0).ToString();
                    if (DT_Result.GetValue("U_ST_APPROVAL_STATUS", i).ToString() != "P")
                    {
                        throw new Logic.Custom_Exception($"Membership[{Membership_Code}],the Approval status is not pending");
                    }
                    if (Approval_Value == "R")
                    {
                        Logic.Membership.Reject(company, Membership_Code, UDO_Info);
                    }
                    else
                    {
                        Logic.Membership.Approve(company, Membership_Code, UDO_Info);
                    }

                    SBO_Application.StatusBar.SetText($"The Membership[{Membership_Code}] has been Rejected successfully!", BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Success);
                }
                catch (Exception ex)
                {
                    SBO_Application.StatusBar.SetText(ex.Message, BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Error);
                    DT_Result.SetValue("Note", i, ex.Message);
                }

            }

            Grid Grd_Result = (Grid)form.Items.Item("16").Specific;


            Grd_Result.AutoResizeColumns();
            SBO_Application.StatusBar.SetText("Done!", BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Success);
            form.Freeze(false);


        }

        private static void Select_Path(ItemEvent pVal)
        {
            Form form = SBO_Application.Forms.Item(pVal.FormUID);

            ST.Helper.Dialog.BrowseFile BF = new Helper.Dialog.BrowseFile(Helper.Dialog.DialogType.OpenFile);
            BF.Filter = "CSV Files (*.csv)|*.csv";

            BF.ShowDialog();

            form.DataSources.UserDataSources.Item("22").Value = BF.FileName;

        }

        private static void Create_Renewal(ItemEvent pVal)
        {
            if(SBO_Application.MessageBox("Are you sure you want to run the process?" , 1, "Yes", "No") != 1)
            {
                return ;
            }

            System.Threading.Thread t = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(Create_Renewal_Thread));
            t.Start(pVal.FormUID);


        }

        private static void Create_Renewal_Thread(object FormUID)
        {
            Form form = SBO_Application.Forms.Item(FormUID);
            DataTable DT_Result = form.DataSources.DataTables.Item("RESULT");
            UDO_Definition UDO_Info;
            UDO_Definition UDO_Card_Info;
            if (form.DataSources.UserDataSources.Item("4").Value == "I")
            {
                UDO_Info = Logic.Objects_Logic.All_UDO_Definition.FirstOrDefault(U => U.KHCF_Object == KHCF_Objects.Individual_Membership);
                UDO_Card_Info = Logic.Objects_Logic.All_UDO_Definition.FirstOrDefault(U => U.KHCF_Object == KHCF_Objects.CCI_Member_Card);
            }
            else
            {
                UDO_Info = Logic.Objects_Logic.All_UDO_Definition.FirstOrDefault(U => U.KHCF_Object == KHCF_Objects.Corporate_Membership);
                UDO_Card_Info = Logic.Objects_Logic.All_UDO_Definition.FirstOrDefault(U => U.KHCF_Object == KHCF_Objects.CCI_Corporate_Member_Card);
            }

            SBO_Application.StatusBar.SetText("Begin!", BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Warning);
            form.Freeze(true);
            for (int i = 0; i < DT_Result.Rows.Count; i++)
            {
                try
                {
                    string Membership_Code = DT_Result.GetValue("Code", 0).ToString();
                    if (DT_Result.GetValue("IS_ACCEPTED", i).ToString() == "N")
                    {
                        string Memeber_Card_Code = DT_Result.GetValue("U_ST_MEMBER_CARD", i).ToString();
                        string Reason = DT_Result.GetValue("REASON", i).ToString();
                        Membership.Update_Card_Not_Renewal(company, Memeber_Card_Code, UDO_Card_Info, Reason);
                    }
                    else
                    {
                        List<Field_Data> Renewal_Fields = new List<Field_Data>();
                        //foreach (string OneField in Logic.Membership.Renewal_Editable_Fields)
                        //{
                        Field_Data OneData = new Field_Data() { Field_Name = "U_ST_COVERAGE", Value = DT_Result.GetValue("NEW_COVERAGE", i) };
                        Renewal_Fields.Add(OneData);
                        //}
                        bool Create_Invoice_Automatically = DT_Result.GetValue("CREATE_INV_AUTO", i).ToString() == "Y";
                        Logic.Membership.Create_Renewal(company, Membership_Code, UDO_Info, Renewal_Fields.ToArray(), Create_Invoice_Automatically,UDO_Info.Table_Name);

                        SBO_Application.StatusBar.SetText($"The Membership[{Membership_Code}] has been Renewal successfully!", BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Success);

                    }
                    DT_Result.SetValue("Note", i, "Done!");
                }
                catch (Exception ex)
                {
                    SBO_Application.StatusBar.SetText($"{ex.Message}, and all changes for this line will rollback", BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Error);
                    DT_Result.SetValue("Note", i, ex.Message);
                }
            }

            Grid Grd_Result = (Grid)form.Items.Item("16").Specific;


            Grd_Result.AutoResizeColumns();
            SBO_Application.StatusBar.SetText("Done!", BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Success);
            form.Freeze(false);
        }

        private static void Filter(ItemEvent pVal)
        {
            Form form = SBO_Application.Forms.Item(pVal.FormUID);
            form.Freeze(true);
            string TableName = "";
            string CardMember_Table = "";
            string Full_Name = "";
            if (form.DataSources.UserDataSources.Item("4").Value == "I")
            {
                TableName = "ST_INDIV_MEMBERSHIP";
                CardMember_Table = "ST_CCI_INDIV_CARD";
                Full_Name = "U_ST_FULL_NAME_AR";
            }
            else
            {
                TableName = "ST_CORP_MEMBERSHIP";
                CardMember_Table = "ST_CCI_CORP_CARD";
                Full_Name = "U_ST_CORPORATE_ARABIC_NAME";
            }
            string Renewal_Type = form.DataSources.UserDataSources.Item("6").Value;
            string Approval_Status = form.DataSources.UserDataSources.Item("8").Value;

            DataTable DT_Membership = form.DataSources.DataTables.Item("RESULT");
            DT_Membership.Rows.Clear();
            string SQL_Result = $@"SELECT T0.""Code"", T0.""U_ST_MEMBER_CARD"", T0.""U_ST_CREATION_DATE""
, T0.""U_ST_START_DATE"", T0.U_ST_END_DATE, T0.""U_ST_ACTIVE"", T0.U_ST_AUTOMATIC_RENEWAL , T0.U_ST_COVERAGE, T0.U_ST_APPROVAL_STATUS, T0.U_ST_APPROVAL_NOTE
,T1.{Full_Name} AS ""Name""
FROM ""@{TableName}"" T0 INNER JOIN ""@{CardMember_Table}""  T1 ON T0.U_ST_MEMBER_CARD = T1.""Code"" 
WHERE ADD_MONTHS(T0.U_ST_END_DATE, 3) >= CURRENT_DATE AND 
(T0.U_ST_AUTOMATIC_RENEWAL ='{Renewal_Type}' OR '{Renewal_Type}' = '-')
AND (T0.U_ST_APPROVAL_STATUS ='{Approval_Status}' OR '{Approval_Status}' = '-')";
            Recordset RC_Membership = Helper.Utility.Execute_Recordset_Query(company, SQL_Result);
            DT_Membership.Rows.Add(RC_Membership.RecordCount);

            for (int i = 0; i < RC_Membership.RecordCount; i++)
            {
                for (int J = 0; J < DT_Membership.Columns.Count; J++)
                {
                    string Col_Name = DT_Membership.Columns.Item(J).Name;
                    string UDF_Name;
                    if (Col_Name =="SELECTED" || Col_Name == "Note")
                    {
                        continue;
                    }
                    UDF_Name = Col_Name;

                    DT_Membership.SetValue(Col_Name, i, RC_Membership.Fields.Item(UDF_Name).Value);
                }
                RC_Membership.MoveNext();
            }
            Grid Grd_Membership = (Grid)form.Items.Item("16").Specific;

            Grd_Membership.AutoResizeColumns();
            form.Freeze(false);

        }


    }
}
