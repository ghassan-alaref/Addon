using SAPbobsCOM;
using SAPbouiCOM;
using ST.KHCF.Customization.Logic.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Text;
using System.Threading.Tasks;

namespace ST.KHCF.Customization.Forms
{
    internal class Frm_Cards_Actions_From_File
    {
        internal static SAPbobsCOM.Company company;
        internal static Application SBO_Application;
       static string[] Actions_Items = new string[] { "22", "12", "9", "15", "11", "18", "23", "24" };

        internal static Form Create_Form()
        {
            var form_params = (FormCreationParams)SBO_Application.CreateObject(BoCreatableObjectType.cot_FormCreationParams);
            form_params.XmlData = Properties.Resources.Frm_Cards_Actions_From_File;
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

            KHCF_Objects[] Supported_UDOs = new KHCF_Objects[] { KHCF_Objects.CCI_Member_Card, KHCF_Objects.Individual_Membership };
                //, KHCF_Objects.Corporate_Membership, KHCF_Objects.CCI_Corporate_Member_Card };
            ComboBox Cmb_Object_Type = (ComboBox)form.Items.Item("14").Specific;
            foreach (KHCF_Objects OneObj in Supported_UDOs)
            {
                UDO_Definition UDO_Info = Logic.Objects_Logic.All_UDO_Definition.FirstOrDefault(U => U.KHCF_Object == OneObj);
                Cmb_Object_Type.ValidValues.Add(((int)OneObj).ToString(), UDO_Info.Title);
            }
            Cmb_Object_Type.Select(0, BoSearchKey.psk_Index);
            Select_Object(form);


            //Grid Grd_Result = (Grid)form.Items.Item("16").Specific;
            //ComboBoxColumn Col_Apprv_Status = (ComboBoxColumn)Grd_Result.Columns.Item("Approval_Status");
            //ComboBox Cmb_Approve_Status = (ComboBox)form.Items.Item("4").Specific;
            //for (int i = 0; i < Cmb_Approve_Status.ValidValues.Count; i++)
            //{
            //    Col_Apprv_Status.ValidValues.Add(Cmb_Approve_Status.ValidValues.Item(i).Value, Cmb_Approve_Status.ValidValues.Item(i).Description);
            //}
            //Col_Apprv_Status.DisplayType = BoComboDisplayType.cdt_Description;

            //form.DataSources.UserDataSources.Item("4").Value = "-";
            //form.DataSources.UserDataSources.Item("8").Value = "-";
        }

        internal static void SBO_Application_ItemEvent(string FormUID, ref ItemEvent pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            if (pVal.FormTypeEx != "ST_Cards_Actions_From_File")
            {
                return;
            }
            try
            {
                if (pVal.ItemUID == "6" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Preview(pVal);
                }
                if (pVal.ItemUID == "5" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Select_Path(pVal);
                }
                if (pVal.ItemUID == "22" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    System.Threading.Thread t = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(Unlink));
                    t.Start(pVal.FormUID);
                    //Unlink(pVal);
                }
                if (pVal.ItemUID == "12" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    System.Threading.Thread t = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(Remove));
                    t.Start(pVal.FormUID);
                    //Remove(pVal);
                }
                if (pVal.ItemUID == "11" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    System.Threading.Thread t = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(Link_Cards));
                    t.Start(pVal.FormUID);
                    //Link_Cards(pVal);
                }
                if (pVal.ItemUID == "18" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    System.Threading.Thread t = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(Cancel_Memberships));
                    t.Start(pVal.FormUID);
                    //Cancel_Memberships(pVal);
                }
                if (pVal.ItemUID == "24" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    System.Threading.Thread t = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(Stop_Memberships));
                    t.Start(pVal.FormUID);
                    //Cancel_Memberships(pVal);
                }
                if (pVal.ItemUID == "23" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    System.Threading.Thread t = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(Stop_Memberships));
                    t.Start(pVal.FormUID);
                    //Cancel_Memberships(pVal);
                }
                if (pVal.ItemUID == "14" && pVal.EventType == BoEventTypes.et_COMBO_SELECT && !pVal.BeforeAction)
                {
                    Form form = SBO_Application.Forms.Item(pVal.FormUID);
                    Select_Object(form);
                }
                if (pVal.ItemUID == "10" && pVal.EventType == BoEventTypes.et_CHOOSE_FROM_LIST && !pVal.BeforeAction)
                {
                    string Code = Utility.Chosse_From_List_For_Code_And_DBDataSource(pVal, pVal.ItemUID, true);

                }


            }
            catch (Exception ex)
            {
                SBO_Application.StatusBar.SetText(ex.Message, BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Error);
            }
        }

        private static void Cancel_Memberships(object obj)
        {
            if (SBO_Application.MessageBox("Are you sure you want to Cancel the Memberships", 1, "Yes", "No") != 1)
            {
                return;
            }
            try
            {
                Form form = SBO_Application.Forms.Item(obj.ToString());
                Change_Buttons_Enabled(form, false);
                DataTable DT_Result = form.DataSources.DataTables.Item("RESULT");
                UDO_Definition UDO_Info = Logic.Objects_Logic.All_UDO_Definition.FirstOrDefault(U => U.KHCF_Object == KHCF_Objects.Individual_Membership);
                for (int i = 0; i < DT_Result.Rows.Count; i++)
                {
                    if (DT_Result.GetValue("SELECTED", i).ToString() != "Y")
                    {
                        continue;
                    }

                    try
                    {
                        string UDO_Card_Code = DT_Result.GetValue("Code", i).ToString();
                        string Membership_Code = Utility.Get_Last_Individual_Membership_Per_Card(company, UDO_Card_Code, UDO_Info);
                        Logic.Membership.Cancel_Individual_Membership(company, Membership_Code, UDO_Info);

                        SBO_Application.StatusBar.SetText($"The membership[{UDO_Card_Code}] has been Canceled successfully!", BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Success);
                        DT_Result.SetValue("Note", i, "Done");
                    }
                    catch (Exception ex)
                    {
                        DT_Result.SetValue("Note", i, ex.Message);
                    }
                }
                Change_Buttons_Enabled(form, true);

            }
            catch (Exception ex)
            {
                Form form = SBO_Application.Forms.Item(obj.ToString());
                Change_Buttons_Enabled(form, true);
                SBO_Application.StatusBar.SetText(ex.Message, BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Error);
            }
        }

        private static void Change_Buttons_Enabled(Form form, bool New_Value)
        {
            foreach (string OneItem in Actions_Items)
            {
                form.Items.Item(OneItem).Enabled = New_Value;
            }

        }

        private static void Stop_Memberships(object obj)
        {
            if (SBO_Application.MessageBox("Are you sure you want to Stop the Memberships", 1, "Yes", "No") != 1)
            {
                return;
            }
            try
            {
                Form form = SBO_Application.Forms.Item(obj.ToString());
                Change_Buttons_Enabled(form, false);
                DataTable DT_Result = form.DataSources.DataTables.Item("RESULT");
                UDO_Definition UDO_Info = Logic.Objects_Logic.All_UDO_Definition.FirstOrDefault(U => U.KHCF_Object == KHCF_Objects.Individual_Membership);
                string Stop_Date_Text = form.DataSources.UserDataSources.Item("26").ValueEx;
                string Stop_Note = form.DataSources.UserDataSources.Item("28").ValueEx;
                if (Stop_Date_Text == "")
                {
                    throw new Logic.Custom_Exception("Please set the Stop Date");
                }
                if (Stop_Note == "")
                {
                    throw new Logic.Custom_Exception("Please set the Stop Note");
                }
                DateTime Stop_Date = DateTime.ParseExact(Stop_Date_Text, "yyyyMMdd", null);
                for (int i = 0; i < DT_Result.Rows.Count; i++)
                {
                    if (DT_Result.GetValue("SELECTED", i).ToString() != "Y")
                    {
                        continue;
                    }

                    try
                    {
                        string UDO_Card_Code = DT_Result.GetValue("Code", i).ToString();
                        string Membership_Code = Utility.Get_Last_Individual_Membership_Per_Card(company, UDO_Card_Code, UDO_Info);
                        Logic.Membership.Stop_Individual_Membership(company, Membership_Code, UDO_Info, Stop_Date, Stop_Note);

                        SBO_Application.StatusBar.SetText($"The membership[{Membership_Code}] has been Stop successfully!", BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Success);
                        DT_Result.SetValue("Note", i, "Done");
                    }
                    catch (Exception ex)
                    {
                        DT_Result.SetValue("Note", i, ex.Message);
                    }
                }
                Change_Buttons_Enabled(form, true);

            }
            catch (Exception ex)
            {
                Form form = SBO_Application.Forms.Item(obj.ToString());
                Change_Buttons_Enabled(form, true);
                SBO_Application.StatusBar.SetText(ex.Message, BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Error);

            }
        }

        private static void Stop_Unlink_Memberships(object obj)
        {
            if (SBO_Application.MessageBox("Are you sure you want to Stop and Unlink the Member Cards", 1, "Yes", "No") != 1)
            {
                return;
            }
            try
            {
                Form form = SBO_Application.Forms.Item(obj.ToString());
                Change_Buttons_Enabled(form, false);
                DataTable DT_Result = form.DataSources.DataTables.Item("RESULT");
                UDO_Definition UDO_Info = Logic.Objects_Logic.All_UDO_Definition.FirstOrDefault(U => U.KHCF_Object == KHCF_Objects.Individual_Membership);
                string Stop_Date_Text = form.DataSources.UserDataSources.Item("26").ValueEx;
                string Stop_Note = form.DataSources.UserDataSources.Item("28").ValueEx;
                if (Stop_Date_Text == "")
                {
                    throw new Logic.Custom_Exception("Please set the Stop Date");
                }
                if (Stop_Note == "")
                {
                    throw new Logic.Custom_Exception("Please set the Stop Note");
                }
                DateTime Stop_Date = DateTime.ParseExact(Stop_Date_Text, "yyyyMMdd", null);
                for (int i = 0; i < DT_Result.Rows.Count; i++)
                {
                    if (DT_Result.GetValue("SELECTED", i).ToString() != "Y")
                    {
                        continue;
                    }

                    try
                    {
                        string UDO_Card_Code = DT_Result.GetValue("Code", i).ToString();
                        string Membership_Code = Utility.Get_Last_Individual_Membership_Per_Card(company, UDO_Card_Code, UDO_Info);
                        string BP_Code = DT_Result.GetValue("BP_Code", i).ToString();
                        company.StartTransaction();
                        Logic.Membership.Stop_Individual_Membership(company, Membership_Code, UDO_Info, Stop_Date, Stop_Note, false);
                        Logic.KHCF_Logic_Utility.Unlink(company, UDO_Card_Code, BP_Code, UDO_Info, false);

                        company.EndTransaction(BoWfTransOpt.wf_Commit);
                        SBO_Application.StatusBar.SetText($"The Member Card[{UDO_Card_Code}] has been Stop and Unlink successfully!", BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Success);
                        DT_Result.SetValue("Note", i, "Done");
                    }
                    catch (Exception ex)
                    {
                        try
                        {
                            company.EndTransaction(BoWfTransOpt.wf_RollBack);
                        }
                        catch (Exception)
                        { }

                        DT_Result.SetValue("Note", i, ex.Message);
                    }
                }
                Change_Buttons_Enabled(form, true);

            }
            catch (Exception ex)
            {
                Form form = SBO_Application.Forms.Item(obj.ToString());
                Change_Buttons_Enabled(form, true);
                SBO_Application.StatusBar.SetText(ex.Message, BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Error);

            }
        }

        private static void Select_Object(Form form)
        {
            KHCF_Objects Obj = (KHCF_Objects)int.Parse(form.DataSources.UserDataSources.Item("14").Value);
            form.Freeze(true);
            foreach (string OneItem in Actions_Items)
            {
                form.Items.Item(OneItem).Visible = false;
            }
            string[] Visible_Items = null;
            switch (Obj)
            {
                case KHCF_Objects.CCI_Member_Card:
                //case KHCF_Objects.CCI_Corporate_Member_Card:
                    Visible_Items = new string[] { "22", "12", "9", "15", "10", "11", "23" };
                    break;
                case KHCF_Objects.Individual_Membership:
               // case KHCF_Objects.Corporate_Membership:
                    Visible_Items = new string[] { "18", "24" };
                    break;
                default:
                    throw new Logic.Custom_Exception($"The Object[{Obj}] is not supported");
            }

            foreach (string OneItem in Visible_Items)
            {
                form.Items.Item(OneItem).Visible = true;
            }

            form.Freeze(false);
        }

        private static void Link_Cards(object obj)
        {

            if (SBO_Application.MessageBox("Are you sure you want to Link the selected Cards", 1, "Yes", "No") != 1)
            {
                return;
            }
            try
            {
                Form form = SBO_Application.Forms.Item(obj.ToString());
                Change_Buttons_Enabled(form, false);
                DataTable DT_Result = form.DataSources.DataTables.Item("RESULT");
                UDO_Definition UDO_Info = Logic.Objects_Logic.All_UDO_Definition.FirstOrDefault(U => U.KHCF_Object == KHCF_Objects.CCI_Member_Card);
                for (int i = 0; i < DT_Result.Rows.Count; i++)
                {
                    if (DT_Result.GetValue("SELECTED", i).ToString() != "Y")
                    {
                        continue;
                    }

                    try
                    {
                        string UDO_Code = DT_Result.GetValue("Code", i).ToString();

                        bool hasValidMembership = Utility.IndvCardHasValidMembership(company, UDO_Code);

                        if (!hasValidMembership)
                        {
                            string Parent_Type = "C";
                            string Parent_ID = form.DataSources.UserDataSources.Item("10").Value;
                            UDO_Definition Parent_UDO_Info = null;
                            string Name_Field = "";
                            Parent_UDO_Info = Logic.Objects_Logic.All_UDO_Definition.FirstOrDefault(U => U.KHCF_Object == KHCF_Objects.CCI_Corporate_Member_Card);
                            Name_Field = "U_ST_CORPORATE_ARABIC_NAME";
                            string SQL = $@"SELECT {Name_Field} from ""@{Parent_UDO_Info.Table_Name}"" WHERE ""Code"" = '{Parent_ID}'";
                            Recordset RC = Helper.Utility.Execute_Recordset_Query(company, SQL);
                            string Parent_Name = RC.Fields.Item(Name_Field).Value.ToString();
                            Logic.KHCF_Logic_Utility.Link(company, UDO_Code, Parent_Type, Parent_ID, Parent_Name, UDO_Info);

                            SBO_Application.StatusBar.SetText($"Card[{UDO_Code}] has been Linked successfully.", BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Success);
                            DT_Result.SetValue("Note", i, "Operation completed successfully");
                        }
                        else
                            DT_Result.SetValue("Note", i, $"Cannot complete the action. Card[{UDO_Code}] is related with valid membership(s) in the time being.");
                    }
                    catch (Exception ex)
                    {
                        DT_Result.SetValue("Note", i, ex.Message);
                    }
                }
                Change_Buttons_Enabled(form, true);

            }
            catch (Exception ex)
            {
                Form form = SBO_Application.Forms.Item(obj.ToString());
                Change_Buttons_Enabled(form, true);
                SBO_Application.StatusBar.SetText(ex.Message, BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Error);

            }
        }

        private static void Remove(object obj)
        {
            if (SBO_Application.MessageBox("Are you sure you want to Remove the selected Cards", 1, "Yes", "No") != 1)
            {
                return;
            }
            try
            {
                Form form = SBO_Application.Forms.Item(obj.ToString());
                Change_Buttons_Enabled(form, false);
                DataTable DT_Result = form.DataSources.DataTables.Item("RESULT");
                KHCF_Objects Obj = (KHCF_Objects)int.Parse(form.DataSources.UserDataSources.Item("14").Value);
                UDO_Definition UDO_Info = Logic.Objects_Logic.All_UDO_Definition.FirstOrDefault(U => U.KHCF_Object == KHCF_Objects.CCI_Member_Card);
                for (int i = 0; i < DT_Result.Rows.Count; i++)
                {
                    if (DT_Result.GetValue("SELECTED", i).ToString() != "Y")
                    {
                        continue;
                    }

                    try
                    {
                        string UDO_Code = DT_Result.GetValue("Code", i).ToString();
                        //string BP_Code = DT_Result.GetValue("BP_Code", i).ToString();
                        Logic.KHCF_Logic_Utility.Remove_UDO_Entry(company, UDO_Code, UDO_Info);

                        SBO_Application.StatusBar.SetText($"The Card[{UDO_Code}] has been Removed successfully!", BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Success);
                        DT_Result.SetValue("Note", i, "Done");
                    }
                    catch (Exception ex)
                    {
                        DT_Result.SetValue("Note", i, ex.Message);
                    }
                }
                Change_Buttons_Enabled(form, true);

            }
            catch (Exception ex)
            {
                Form form = SBO_Application.Forms.Item(obj.ToString());
                Change_Buttons_Enabled(form, true);
                SBO_Application.StatusBar.SetText(ex.Message, BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Error);

            }
        }

        private static void Preview(ItemEvent pVal)
        {
            Form form = SBO_Application.Forms.Item(pVal.FormUID);
            form.Freeze(true);
            DataTable DT_Result = form.DataSources.DataTables.Item("RESULT");

            string[] National_IDs = Get_National_IDs_From_File(form);
            string National_IDs_Text = string.Join("','", National_IDs);
            National_IDs_Text = $"'{National_IDs_Text}'";

//            string SQL = $@"SELECT T0.""Code"", T0.U_ST_FULL_NAME_AR, T0.U_ST_BP_CODE, T0.U_ST_PARENT_ID, T0.U_ST_NATIONAL_ID, T0.U_ST_TEL1
//, T0.U_ST_APPROVAL_STATUS
//FROM ""@ST_CCI_INDIV_CARD"" T0 
//WHERE  T0.U_ST_NATIONAL_ID in ({National_IDs_Text})";
            string SQL = $@"SELECT T0.""Code"", T0.U_ST_FULL_NAME_AR, T0.U_ST_BP_CODE, T0.U_ST_PARENT_ID, T0.U_ST_NATIONAL_ID, T0.U_ST_TEL1
, T0.U_ST_APPROVAL_STATUS
FROM ""@ST_CCI_INDIV_CARD"" T0 
WHERE  T0.""Code"" in ({National_IDs_Text})";

            Recordset RC = Helper.Utility.Execute_Recordset_Query(company, SQL);
            DT_Result.Rows.Clear();
            DT_Result.Rows.Add(RC.RecordCount);

            for (int i = 0; i < RC.RecordCount; i++)
            {
                DT_Result.SetValue("SELECTED", i, "Y");
                DT_Result.SetValue("Code", i, RC.Fields.Item("Code").Value);
                DT_Result.SetValue("Name", i, RC.Fields.Item("U_ST_FULL_NAME_AR").Value);
                DT_Result.SetValue("BP_Code", i, RC.Fields.Item("U_ST_BP_CODE").Value);
                DT_Result.SetValue("National_ID", i, RC.Fields.Item("U_ST_NATIONAL_ID").Value);
                DT_Result.SetValue("Parent_Code", i, RC.Fields.Item("U_ST_PARENT_ID").Value);
                DT_Result.SetValue("Tel", i, RC.Fields.Item("U_ST_TEL1").Value);


                RC.MoveNext();
            }
            ((Grid)form.Items.Item("16").Specific).AutoResizeColumns();
            form.Freeze(false);

        }

        private static void Unlink(object obj)
        {
            if (SBO_Application.MessageBox("Are you sure you want to Unlink the selected Cards", 1, "Yes", "No") != 1)
            {
                return;
            }
            Form form = SBO_Application.Forms.Item(obj.ToString());
            try
            {
                form.Freeze(true);
                Change_Buttons_Enabled(form, false);
                DataTable DT_Result = form.DataSources.DataTables.Item("RESULT");
                UDO_Definition UDO_Info = Logic.Objects_Logic.All_UDO_Definition.FirstOrDefault(U => U.KHCF_Object == KHCF_Objects.CCI_Member_Card);
                for (int i = 0; i < DT_Result.Rows.Count; i++)
                {
                    if (DT_Result.GetValue("SELECTED", i).ToString() != "Y")
                    {
                        continue;
                    }

                    try
                    {
                        string UDO_Code = DT_Result.GetValue("Code", i).ToString();

                        bool hasValidMembership = Utility.IndvCardHasValidMembership(company, UDO_Code);

                        if (!hasValidMembership)
                        {
                            string BP_Code = DT_Result.GetValue("BP_Code", i).ToString();
                            Logic.KHCF_Logic_Utility.Unlink(company, UDO_Code, BP_Code, UDO_Info);
                            SBO_Application.StatusBar.SetText($"The Card[{UDO_Code}] has been Unlinked successfully", BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Success);
                            DT_Result.SetValue("Note", i, "Operation completed successfully.");
                        }
                        else
                            DT_Result.SetValue("Note", i, $"Cannot complete the action. Card[{UDO_Code}] is related with valid membership(s) in the time being.");
                    }
                    catch (Exception ex)
                    {
                        DT_Result.SetValue("Note", i, ex.Message);
                    }
                }
                Change_Buttons_Enabled(form, true);

            }
            catch (Exception ex)
            {
                form = SBO_Application.Forms.Item(obj.ToString());
                Change_Buttons_Enabled(form, true);
                SBO_Application.StatusBar.SetText(ex.Message, BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Error);
            }
            finally
            {
                ((Grid)form.Items.Item("16").Specific).AutoResizeColumns();
                form.Freeze(false);
            }
        }

        private static string[] Get_National_IDs_From_File(Form form)
        {
            List<string> Result = new List<string>();

            string CSV_Delimiter = Configurations.Get_File_Delimiter(company);
            string File_Path = form.DataSources.UserDataSources.Item("4").Value;
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

                Result.Add(Split_Line[0]);
            }

            return Result.ToArray();
        }

        private static void Select_Path(ItemEvent pVal)
        {
            Form form = SBO_Application.Forms.Item(pVal.FormUID);

            ST.Helper.Dialog.BrowseFile BF = new Helper.Dialog.BrowseFile(Helper.Dialog.DialogType.OpenFile);
            BF.Filter = "CSV Files (*.csv)|*.csv";

            BF.ShowDialog();

            form.DataSources.UserDataSources.Item("4").Value = BF.FileName;

        }

    }
}
