using SAPbouiCOM;
using ST.KHCF.Customization.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SAPbobsCOM;
using ST.KHCF.Customization.Logic.Classes;
using System.Reflection;
using System.Windows.Forms;
using Form = SAPbouiCOM.Form;
using System.Reflection.Emit;

namespace ST.KHCF.Customization.Forms.CCI
{
    internal class Frm_Stop_Cancel_Children 
    {
        internal static SAPbobsCOM.Company company;
        internal static SAPbouiCOM.Application SBO_Application;

        internal static Form Create_Form()
        {
            var form_params = (FormCreationParams)SBO_Application.CreateObject(BoCreatableObjectType.cot_FormCreationParams);
            form_params.XmlData = Properties.Resources.Frm_Stop_Cancel_Children2;
            Form form;
            try
            {
                form = SBO_Application.Forms.AddEx(form_params);
            }
            catch (Exception ex)
            {
                form = SBO_Application.Forms.GetForm("ST_Stop_Cancel_Children", 1);
            }

            Grid Grd_Result = (Grid)form.Items.Item("Children").Specific;
            Grd_Result.AutoResizeColumns();
            form.AutoManaged = true;
            form.SupportedModes = -1;
            form_params.Modality = BoFormModality.fm_Modal;

            return form;

        }

        internal static void FillData(Form form)
        {
            try
            {
                Initialize_Form(form);
            }
            catch (Exception ex)
            {
                SBO_Application.StatusBar.SetText(ex.Message, BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Error);
            }
        }

        private static void Initialize_Form(Form form)
        {
            string UDO_Code = form.DataSources.UserDataSources.Item("UD_2").Value;
            string SQL_Membership = $@"SELECT T0.""Code"",T0.""U_ST_MEMBER_NAME"", T0.""U_ST_START_DATE"", T0.""U_ST_END_DATE"", 
CASE WHEN T0.""U_ST_MEMBERSHIP_STATUS"" = 'C' Then 'Canceled' When  T0.""U_ST_MEMBERSHIP_STATUS"" = 'N' then 'New' When  T0.""U_ST_MEMBERSHIP_STATUS"" = 'P' then 'Past Renew' 
When  T0.""U_ST_MEMBERSHIP_STATUS"" = 'R' Then 'Renew' End As U_ST_MEMBERSHIP_STATUS FROM ""@ST_INDIV_MEMBERSHIP""  T0 WHERE T0.""U_ST_MEMBERSHIP_STATUS"" in ('N','R','P') And T0.""U_ST_APPROVAL_STATUS"" ='A' AND T0.""U_ST_PARENT_MEMBERSHIP_ID"" = '{UDO_Code}'";
            
            Grid Grd_Result = (Grid)form.Items.Item("Children").Specific;
            string Type = form.DataSources.UserDataSources.Item("Type").Value;
            
            Recordset RC_Child = Helper.Utility.Execute_Recordset_Query(company, SQL_Membership);
            if (RC_Child.RecordCount != 0)
            {
                DataTable DT_Result = form.DataSources.DataTables.Item("Children");              
                DT_Result.Rows.Clear();
                DT_Result.Rows.Add(RC_Child.RecordCount);

                for(int index=0;index<RC_Child.RecordCount;index++)
                {
                    DateTime Start_Date = Convert.ToDateTime(RC_Child.Fields.Item("U_ST_START_DATE").Value.ToString());
                    DateTime End_Date = Convert.ToDateTime(RC_Child.Fields.Item("U_ST_END_DATE").Value.ToString());
                    DT_Result.SetValue("SELECTED",index, "Y");
                    DT_Result.SetValue("Code", index, RC_Child.Fields.Item("Code").Value.ToString());
                    DT_Result.SetValue("ST_MEMBER_NAME", index, RC_Child.Fields.Item("U_ST_MEMBER_NAME").Value.ToString());
                    DT_Result.SetValue("ST_START_DATE", index,Start_Date.ToString("yyyyMMdd"));
                    DT_Result.SetValue("ST_END_DATE", index, End_Date.ToString("yyyyMMdd"));
                    DT_Result.SetValue("ST_MEMBERSHIP_STATUS", index, RC_Child.Fields.Item("U_ST_MEMBERSHIP_STATUS").Value.ToString());
                    if (Type == "S")
                    {
                        DT_Result.SetValue("ST_STOP_NOTE", index, form.DataSources.UserDataSources.Item("Note").Value);
                        DateTime StopDate;
                        if (DateTime.TryParseExact(form.DataSources.UserDataSources.Item("Stop").ValueEx, "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out StopDate))
                        {
                            DT_Result.SetValue("ST_STOP_DATE", index, StopDate);
                        }
                    }
                    RC_Child.MoveNext();

                }
            }
            if (Type != "S")
            {
                Grd_Result.Columns.Item("ST_STOP_NOTE").Visible = false;
                Grd_Result.Columns.Item("ST_STOP_DATE").Visible = false;
            }

            Grd_Result.AutoResizeColumns();
            SBO_Application.StatusBar.SetText("Done", BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Success);
        }

        internal static void SBO_Application_ItemEvent(string FormUID, ref ItemEvent pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            if (pVal.FormTypeEx != "ST_Stop_Cancel_Children")
            {
                return;
            }
            try
            {
                if (pVal.ItemUID == "Item_3" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Run(pVal);
                }
            }
            catch (Exception ex)
            {
                SBO_Application.StatusBar.SetText(ex.Message, BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Error);
            }
        }

        private static void Run(ItemEvent pVal)
        {
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
            DataTable DT_Result = form.DataSources.DataTables.Item("Children");

            SBO_Application.StatusBar.SetText("Begin!", BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Warning);
            form.Freeze(true);
            UDO_Definition UDO_Info = Logic.Objects_Logic.All_UDO_Definition.FirstOrDefault(U => U.KHCF_Object == KHCF_Objects.Individual_Membership);
            string Title = "";
            string Type = form.DataSources.UserDataSources.Item("Type").Value;
            if (Type == "S")
                Title = "Stopped";
            else if (Type == "C")
                Title = "Canceled";
            else if (Type == "L")
                Title = "Closed";
            else if (Type == "R")
                Title = "Removed";
            for (int i = 0; i < DT_Result.Rows.Count; i++)
            {
                if (DT_Result.GetValue("SELECTED", i).ToString() != "Y")
                {
                    continue;
                }
                try
                {
                    string UDO_Code = DT_Result.GetValue("Code", i).ToString();
                    if (Type == "S")
                    {
                        if (string.IsNullOrEmpty(DT_Result.GetValue("ST_STOP_NOTE", i).ToString()) || string.IsNullOrEmpty(DT_Result.GetValue("ST_STOP_NOTE", i).ToString()))
                            throw new Custom_Exception($@"you should fill stop Date and note at row [{i}]");

                        Membership.Stop_Individual_Membership(company, UDO_Code, UDO_Info, (DateTime)DT_Result.GetValue("ST_STOP_DATE", i), DT_Result.GetValue("ST_STOP_NOTE", i).ToString());
                    }
                    else if (Type == "C")
                    {
                        Membership.Cancel_Individual_Membership(company, UDO_Code, UDO_Info);
                    }
                    else if (Type == "L")
                    {
                        Membership.Close_Individual_Membership(company, UDO_Code, UDO_Info);
                    }
                    else if (Type == "R")
                    {
                        Membership.Remove(company, UDO_Code, UDO_Info);
                    }

                    SBO_Application.StatusBar.SetText($"The Membership [{UDO_Code}] has been {Title} successfully!", BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Success);
                    DT_Result.SetValue("Note", i, "Done");
                }
                catch (Exception ex)
                {
                    SBO_Application.StatusBar.SetText(ex.Message, BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Error);
                    DT_Result.SetValue("Note", i, ex.Message);
                }
                finally
                {
                    form.Freeze(false);
                }
 
                SBO_Application.StatusBar.SetText("Done!", BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Success);
            }

        }
        
    }

}
