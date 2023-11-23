using SAPbobsCOM;
using SAPbouiCOM;
using ST.KHCF.Customization.Logic.Classes;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace ST.KHCF.Customization.Forms.CCI
{
    internal class Frm_Memberships_Need_To_Active
    {
        internal static SAPbobsCOM.Company company;
        internal static Application SBO_Application;

        internal static Form Create_Form(bool Close_If_Empty)
        {
            var form_params = (FormCreationParams)SBO_Application.CreateObject(BoCreatableObjectType.cot_FormCreationParams);
            form_params.XmlData = Properties.Resources.Frm_Memberships_Need_To_Active;
            var form = SBO_Application.Forms.AddEx(form_params);
            //form.Visible = false;
            form.AutoManaged = true;
            form.SupportedModes = -1;
            form_params.Modality = BoFormModality.fm_Modal;

            //  Inv_Form_Data = Inv_Data;

            try
            {
                bool Need_To_Close = Initialize_Form(form, Close_If_Empty);
                if (Need_To_Close)
                {
                    form.Close();
                    return null;
                }
            }
            catch (Exception ex)
            {
                SBO_Application.StatusBar.SetText(ex.Message, BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Error);
            }
            form.Visible = true;
            return form;
        }

        private static bool Initialize_Form(Form form, bool Close_If_Empty)
        {
            Grid Grd = (Grid)form.Items.Item("4").Specific;
            Grd.Columns.Item("CREATE_INV_AUTO").Visible = false;
            Grd.Columns.Item("U_ST_PREVIOUS_MEMBERSHIP_CODE").Visible = false;
            Filter(form);
            DataTable DT_Result = form.DataSources.DataTables.Item("RESULT");
            if (Close_If_Empty == false)
            {
                return false;
            }
            if (DT_Result.Rows.Count == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        internal static void SBO_Application_ItemEvent(string FormUID, ref ItemEvent pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            if (pVal.FormTypeEx != "ST_Memberships_Need_To_Active")
            {
                return;
            }
            try
            {
                if (pVal.ItemUID == "3" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Run(pVal);
                }
                if (pVal.ItemUID == "9" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Form form = SBO_Application.Forms.Item(FormUID);
                    Filter(form);
                }

            }
            catch (Exception ex)
            {
                Form form = SBO_Application.Forms.Item(FormUID);
                form.Freeze(false);
                SBO_Application.StatusBar.SetText(ex.Message, BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Error);
            }
        }

        private static void Run(ItemEvent pVal)
        {
            string SQL_User = $@"SELECT T0.U_ST_CAN_ACTIVE_MEMBERSHIP FROM OUSR T0 WHERE T0.""USER_CODE"" = '{company.UserName}'";
            Recordset RC_User = Helper.Utility.Execute_Recordset_Query(company, SQL_User);
            if (RC_User.Fields.Item("U_ST_CAN_ACTIVE_MEMBERSHIP").Value.ToString() == "N")
            {
                throw new Logic.Custom_Exception("You are not authorizer to do this action");
            }

            if (SBO_Application.MessageBox("Are you sure you want to run the process?", 1, "Yes", "No") != 1)
            {
                return;
            }

            System.Threading.Thread t = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(Run_Thread));
            t.Start(pVal.FormUID);


        }

        private static void Run_Thread(object FormUID)
        {
            Form form = SBO_Application.Forms.Item(FormUID);
            DataTable DT_Result = form.DataSources.DataTables.Item("RESULT");
            UDO_Definition UDO_Info = Logic.Objects_Logic.All_UDO_Definition.FirstOrDefault(U => U.KHCF_Object == KHCF_Objects.Individual_Membership);

            SBO_Application.StatusBar.SetText("Begin!", BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Warning);
            form.Freeze(true);
            for (int i = 0; i < DT_Result.Rows.Count; i++)
            {
                if (DT_Result.GetValue("SELECTED", i).ToString() != "Y")
                {
                    continue;
                }
                try
                {
                    company.StartTransaction();

                    string Membership_Code = DT_Result.GetValue("Code", i).ToString();
                    string Prev_Membership_Code = DT_Result.GetValue("U_ST_PREVIOUS_MEMBERSHIP_CODE", i).ToString();
                    //bool Create_Invoice_Automatically = DT_Result.GetValue("CREATE_INV_AUTO", i).ToString() == "Y";

                    Logic.Membership.Active(company, Membership_Code, Prev_Membership_Code, UDO_Info, true);
                    DT_Result.SetValue("Note", i, "Done!");

                    company.EndTransaction(BoWfTransOpt.wf_Commit);
                    SBO_Application.StatusBar.SetText($"The Membership[{Membership_Code}] has been Activated successfully!", BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Success);
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
                    DT_Result.SetValue("Note", i, ex.Message);
                    DT_Result.SetValue("SELECTED", 0, "N");
                    SBO_Application.SetStatusBarMessage(ex.Message, BoMessageTime.bmt_Short, true);
                }

            }

            Grid Grd_Result = (Grid)form.Items.Item("4").Specific;


            Grd_Result.AutoResizeColumns();
            SBO_Application.StatusBar.SetText("Done!", BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Success);
            form.Freeze(false);
        }



        private static void Filter(Form form)
        {
            //File.AppendAllText("C:\\Users\\wsaleh\\Desktop\\CCI_TimeLog.txt", "CCI Members to Activate" + Environment.NewLine);
            string startTimeExact = DateTime.Now.ToString("mm:ss:f");
            DateTime startTime = DateTime.Now;
           // File.AppendAllText("C:\\Users\\wsaleh\\Desktop\\CCI_TimeLog.txt", "Start Time:" + startTimeExact + Environment.NewLine);
            form.Freeze(true);
            string From_Date_Text = form.DataSources.UserDataSources.Item("6").ValueEx;
            string To_Date_Text = form.DataSources.UserDataSources.Item("8").ValueEx;
            if (From_Date_Text == "")
            {
                From_Date_Text = "20000101";
            }
            if (To_Date_Text == "")
            {
                To_Date_Text = "21000101";
            }
            DataTable DT_Result = form.DataSources.DataTables.Item("RESULT");


            string SQL = $@"SELECT 'Y' AS ""SELECTED"", T0.""Code"", T0.""U_ST_MEMBER_CARD"", T1.""U_ST_FULL_NAME_AR"", T0.""U_ST_START_DATE""
, T0.""U_ST_END_DATE"", T2.""Name"" AS ""COVERAGE"" ,T0.U_ST_PREVIOUS_MEMBERSHIP_CODE, 'Y' AS CREATE_INV_AUTO, T3.""DocEntry"" as DOWN_DOCENTRY, T3.""DocTotal"", T3.""PaidToDate""
FROM ""@ST_INDIV_MEMBERSHIP""  T0 
INNER JOIN ""@ST_CCI_INDIV_CARD"" T1 ON T0.U_ST_MEMBER_CARD = T1.""Code"" 
INNER JOIN ODPI T3 ON T3.""DocEntry"" = T0.U_ST_INVOICE_NUMBER
LEFT JOIN ""@ST_COVERAGE"" T2 ON T0.U_ST_COVERAGE = T2.""Code"" 
WHERE T0.""U_ST_APPROVAL_STATUS"" = 'A' 
AND T3.""PaidToDate"" <> 0
AND U_ST_MEMBERSHIP_STATUS in ('N', 'R', 'P') AND U_ST_DOCUMENT_TYPE = '203' AND U_ST_INVOICE_NUMBER <> ''
AND T3.""DocDate"" Between '{From_Date_Text}' AND '{To_Date_Text}'";

            Recordset RC = Helper.Utility.Execute_Recordset_Query(company, SQL);
            DT_Result.Rows.Clear();
            DT_Result.Rows.Add(RC.RecordCount);

            for (int i = 0; i < RC.RecordCount; i++)
            {
                for (int J = 0; J < DT_Result.Columns.Count; J++)
                {
                    string Col_Name = DT_Result.Columns.Item(J).Name;
                    if (Col_Name == "Note")
                    {
                        continue;
                    }
                    DT_Result.SetValue(Col_Name, i, RC.Fields.Item(Col_Name).Value);
                }
                RC.MoveNext();
            }
            ((Grid)form.Items.Item("4").Specific).AutoResizeColumns();
            form.Freeze(false);
           // DateTime prev = Utility.Add_Time_Log("C", "Grid", startTime);
           // prev = Utility.Add_Time_Log("C", "", startTime, true);

        }

        internal static void Check_Membership_If_Need()
        {
            string SQL_User  = $@"SELECT T0.U_ST_CAN_ACTIVE_MEMBERSHIP FROM OUSR T0 WHERE T0.""USER_CODE"" = '{company.UserName}'";
            Recordset RC_User = Helper.Utility.Execute_Recordset_Query(company, SQL_User);
            if (RC_User.Fields.Item("U_ST_CAN_ACTIVE_MEMBERSHIP").Value.ToString() == "N")
            {
                return;
            }
            string SQL = $@"SELECT COUNT(*) FROM ""@ST_INDIV_MEMBERSHIP""  T0 
WHERE T0.U_ST_APPROVAL_STATUS = 'A' AND  IFNULL(T0.U_ST_ACTIVE ,'N') = 'N' 
AND  T0.U_ST_START_DATE >= CURRENT_DATE";

            Recordset RC = Helper.Utility.Execute_Recordset_Query(company, SQL);
            if ((int)RC.Fields.Item(0).Value != 0)
            {
                Create_Form(true);
            }
        }
    }
}
