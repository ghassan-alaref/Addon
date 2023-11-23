using SAPbobsCOM;
using SAPbouiCOM;
using ST.KHCF.Customization.Logic.Classes;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ST.KHCF.Customization.Forms
{
    class Frm_Cards_List
    {
        internal static SAPbobsCOM.Company company;
        internal static Application SBO_Application;

        internal static Form Create_Form()
        {
            var form_params = (FormCreationParams)SBO_Application.CreateObject(BoCreatableObjectType.cot_FormCreationParams);
            form_params.XmlData = Properties.Resources.Frm_Cards_List;
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

            Grid Grd_Result = (Grid)form.Items.Item("16").Specific;
            ComboBoxColumn Col_Apprv_Status = (ComboBoxColumn)Grd_Result.Columns.Item("Approval_Status");
           // ComboBoxColumn Col_Customer_Group = (ComboBoxColumn)Grd_Result.Columns.Item("Customer_Group");
            string SQL_Memeber_Customer_Group = $@"SELECT ""GroupCode"" AS ""Code"", T0.""GroupName"" AS ""Name"" FROM OCRG T0 
WHERE  T0.""GroupType""  = 'C'  AND U_ST_CUSTOMER_TYPE = 'C'";
            Helper.Utility.FillGridComboBoxForSQL(company, Grd_Result, "Customer_Group", SQL_Memeber_Customer_Group, true);
            
            string SQL_Channel = $@"SELECT T0.""Code"", T0.""Name"" FROM ""@ST_CHANNEL""  T0";
            Helper.Utility.FillGridComboBoxForSQL(company, Grd_Result, "Channel", SQL_Channel, true);

            ComboBox Cmb_Approve_Status = (ComboBox)form.Items.Item("4").Specific;
            for (int i = 0; i < Cmb_Approve_Status.ValidValues.Count; i++)
            {
                Col_Apprv_Status.ValidValues.Add(Cmb_Approve_Status.ValidValues.Item(i).Value, Cmb_Approve_Status.ValidValues.Item(i).Description);
            }
            Col_Apprv_Status.DisplayType = BoComboDisplayType.cdt_Description;
            ComboBoxColumn Card_Type = (ComboBoxColumn)Grd_Result.Columns.Item("CARD_TYPE");
            Card_Type.ValidValues.Add("I", "Individual");
            Card_Type.ValidValues.Add("C", "Corporate");
            Card_Type.ValidValues.Add("N", "None");
            //Card_Type.DisplayType = BoComboDisplayType.cdt_Description;

            form.DataSources.UserDataSources.Item("4").Value = "P";
            form.DataSources.UserDataSources.Item("8").Value = "-";
        }

        internal static void SBO_Application_ItemEvent(string FormUID, ref ItemEvent pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            if (pVal.FormTypeEx != "ST_Cards_List")
            {
                return;
            }
            try
            {
                if (pVal.ItemUID == "18" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Filter(pVal);
                }
                if (pVal.ItemUID == "20" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Approve(pVal);
                }
                if (pVal.ItemUID == "19" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Reject(pVal);
                }
                //if (pVal.ItemUID == "22" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                //{
                //    Unlink(pVal);
                //}

            }
            catch (Exception ex)
            {
                Form form = SBO_Application.Forms.Item(pVal.FormUID);
                form.Freeze(false);
                SBO_Application.StatusBar.SetText(ex.Message, BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Error);
            }
        }

        private static void Reject(ItemEvent pVal)
        {
            if (SBO_Application.MessageBox("Are you sure you want to Reject the selected Cards", 1, "Yes", "No") != 1)
            {
                return;
            }
            Form form = SBO_Application.Forms.Item(pVal.FormUID);
            DataTable DT_Result = form.DataSources.DataTables.Item("RESULT");
            UDO_Definition UDO_Indiv_Info = Logic.Objects_Logic.All_UDO_Definition.FirstOrDefault(U => U.KHCF_Object == KHCF_Objects.CCI_Member_Card);
            UDO_Definition UDO_Corp_Info = Logic.Objects_Logic.All_UDO_Definition.FirstOrDefault(U => U.KHCF_Object == KHCF_Objects.CCI_Corporate_Member_Card);
            for (int i = 0; i < DT_Result.Rows.Count; i++)
            {
                if (DT_Result.GetValue("SELECTED", i).ToString() != "Y")
                {
                    continue;
                }

                try
                {
                    string UDO_Code = DT_Result.GetValue("Code", i).ToString();
                    string Card_Type = DT_Result.GetValue("CARD_TYPE", i).ToString();
                    string SQL_Parent = "";
                    Recordset rc_Parent;
                    if (Card_Type == "I")
                    {
                        Logic.KHCF_Approval.Reject_MemberCard(company, UDO_Code, "", UDO_Indiv_Info);
                        SQL_Parent = $@"Select T0.""Code"",T0.""U_ST_APPROVAL_STATUS"" from ""@ST_CCI_INDIV_CARD"" T0 Where T0.""U_ST_PARENT_ID""='{UDO_Code}'";
                    }
                    else if (Card_Type == "C")
                    {
                        Logic.KHCF_Approval.Reject_MemberCard(company, UDO_Code, "", UDO_Corp_Info);
                        SQL_Parent = $@"Select T0.""Code"",T0.""U_ST_APPROVAL_STATUS"" from ""@ST_CCI_CORP_CARD"" T0 Where T0.""U_ST_PARENT_ID""='{UDO_Code}'";
                    }
                    else
                    {
                        throw new Logic.Custom_Exception($"The Card Type[{Card_Type}] is not supported");
                    }

                    rc_Parent = Helper.Utility.Execute_Recordset_Query(company, SQL_Parent);
                    while (!rc_Parent.EoF)
                    {
                        string Child_Code = rc_Parent.Fields.Item("Code").Value.ToString();
                        Logic.KHCF_Approval.Reject_MemberCard(company, Child_Code, $"Approved From Parent [{Child_Code}]", UDO_Indiv_Info);
                        rc_Parent.MoveNext();
                    }

                    SBO_Application.StatusBar.SetText($"The Card[{UDO_Code}] has been Rejected successfully!", BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Success);
                    DT_Result.SetValue("Note", i, "Done");
                    //DT_Result.Rows.Remove(i);
                }
                catch (Exception ex)
                {
                    SBO_Application.StatusBar.SetText($"Error when Approve The Card [{ex.Message}]", BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Error);
                    DT_Result.SetValue("Note", i, ex.Message);
                }
            }

            Filter(pVal);
        }

        private static void Approve(ItemEvent pVal)
        {
            if (SBO_Application.MessageBox("Are you sure you want to Approve the selected Cards", 1, "Yes", "No") != 1)
            {
                return;
            }
            Form form = SBO_Application.Forms.Item(pVal.FormUID);
            DataTable DT_Result = form.DataSources.DataTables.Item("RESULT");
            UDO_Definition UDO_Indiv_Info = Logic.Objects_Logic.All_UDO_Definition.FirstOrDefault(U => U.KHCF_Object == KHCF_Objects.CCI_Member_Card);
            UDO_Definition UDO_Corp_Info = Logic.Objects_Logic.All_UDO_Definition.FirstOrDefault(U => U.KHCF_Object == KHCF_Objects.CCI_Corporate_Member_Card);
            for (int i = 0; i < DT_Result.Rows.Count; i++)
            {
                if (DT_Result.GetValue("SELECTED", i).ToString() != "Y")
                {
                    continue;
                }

                try
                {
                    string UDO_Code = DT_Result.GetValue("Code", i).ToString();
                    string Card_Type = DT_Result.GetValue("CARD_TYPE", i).ToString();
                  
                    if (Card_Type == "I")
                    {
                        Logic.KHCF_Approval.Approve_MemberCard(company, UDO_Code, "", UDO_Indiv_Info);

                    }
                    else if (Card_Type == "C")
                    {
                        Logic.KHCF_Approval.Approve_MemberCard(company, UDO_Code, "", UDO_Corp_Info);
                        
                    }
                    else
                    {
                        throw new Logic.Custom_Exception($"The Card Type[{Card_Type}] is not supported");
                    }

                   
                    SBO_Application.StatusBar.SetText($"The Card[{UDO_Code}] has been approved successfully!", BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Success);
                    DT_Result.SetValue("Note", i, "Done");
                   // DT_Result.Rows.Remove(i);
                }
                catch (Exception ex)
                {
                    SBO_Application.StatusBar.SetText($"Error when Approve The Card [{ex.Message}]", BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Error);
                  //  DT_Result.SetValue("Note", i, ex.Message);
                }
            }
            Filter(pVal);
            ((Grid)form.Items.Item("16").Specific).AutoResizeColumns();



        }

        private static void Filter(ItemEvent pVal)
        {
            UDO_Definition UDO_Info = Logic.Objects_Logic.All_UDO_Definition.FirstOrDefault(U => U.KHCF_Object == KHCF_Objects.CCI_Member_Card);
            //  string Current_Role = Utility.Get_Current_User_Role(Loader.company);
            bool Can_Approve = Utility.User_Can_Approve(Loader.company, Loader.company.UserName, UDO_Info);
            if (!Can_Approve)
            {
                throw new Logic.Custom_Exception("The user has not Approval Role");
            }
            Form form = SBO_Application.Forms.Item(pVal.FormUID);
            form.Freeze(true);
            DataTable DT_Result = form.DataSources.DataTables.Item("RESULT");

            string Approval_Status = form.DataSources.UserDataSources.Item("4").ValueEx;
            string Card_ID = form.DataSources.UserDataSources.Item("6").ValueEx;
            string Member_Type = form.DataSources.UserDataSources.Item("8").ValueEx;
            string Parent_ID = form.DataSources.UserDataSources.Item("21").ValueEx;
            string National_ID = form.DataSources.UserDataSources.Item("11").ValueEx;
            string Tel = form.DataSources.UserDataSources.Item("13").ValueEx;
            string Name = form.DataSources.UserDataSources.Item("15").ValueEx;
            string SQL =string.Empty;
            // AND (IFNULL(T0.U_ST_PARENT_TYPE,'-') = '{Parent_Type}' OR '{Parent_Type}' = '-')

            if (Member_Type == "I")
            {
                SQL = $@"SELECT T0.""Code"", T0.""U_ST_FULL_NAME_AR"", 'I' AS ""CARD_TYPE"", T0.""U_ST_BP_CODE"", IFNULL(T0.""U_ST_PARENT_ID"",''), T0.""U_ST_NATIONAL_ID"", T0.""U_ST_TEL1""
,T0.""U_ST_APPROVAL_STATUS"",T0.""U_ST_CUSTOMER_GROUP"",T0.""CreateDate"",T0.""U_ST_CREATOR"",T0.""U_ST_FIRST_NAME_AR"",T0.""U_ST_FATHER_NAME_AR"",T0.""U_ST_MIDDLE_NAME_AR"",
T0.""U_ST_SURNAME_AR"",T0.""U_ST_FIRST_NAME_EN"",T0.""U_ST_FATHER_NAME_EN"",T0.""U_ST_MIDDLE_NAME_EN"",T0.""U_ST_SURNAME_EN"",T0.""U_ST_PARENT_TYPE"",T0.""U_ST_PARENT_ID"",T0.""U_ST_PARENT_NAME"",
T0.""U_ST_PERSONAL_ID"",T0.""U_ST_PASSPORT_ID"",T0.""U_ST_TEL2"",T0.""U_ST_DATE_OF_BIRTH"",T0.""U_ST_GENDER"",T0.""U_ST_NATIONALITY"",T0.""U_ST_EMAIL"",T0.""U_ST_TITLE"",
T0.""U_ST_VIP"",T0.""U_ST_VIP_RELATED_TO"",T0.""U_ST_ACCOUNT_MANAGER"",T0.""U_ST_BROKER1"",T0.""U_ST_BROKER2"",T0.""U_ST_CHANNEL"",T0.""U_ST_SUB_CHANNEL"",T0.""U_ST_RESIDENCY"",T0.""U_ST_CURRENCY"",T0.""U_ST_PREFIX"",T0.""U_ST_APPROVAL_NOTE"",'','','','','','',''
FROM ""@ST_CCI_INDIV_CARD""  T0
WHERE (IFNULL(T0.U_ST_APPROVAL_STATUS, 'P') = '{Approval_Status}' OR  '{Approval_Status}' = '-') 
AND (T0.""Code"" LIKE '%{Card_ID}%')
AND (T0.U_ST_PARENT_ID = '{Parent_ID}' OR '{Parent_ID}'= '') AND (IFNULL(T0.U_ST_NATIONAL_ID,'') LIKE '%{National_ID}%')
AND IFNULL(T0.U_ST_TEL1,'') LIKE '%{Tel}%' AND UPPER(IFNULL(T0.U_ST_FULL_NAME_AR,'')) LIKE '%{Name.ToUpper().Replace("*", "%")}%' 
AND (IFNULL(T0.""U_ST_PARENT_ID"",0)=0 OR (Select T1.""U_ST_APPROVAL_STATUS"" From ""@ST_CCI_INDIV_CARD""  T1 where T1.""Code"" = T0.""U_ST_PARENT_ID"")='A')";
            }
            else if (Member_Type == "C")
            {
                SQL = $@"SELECT T0.""Code"", T0.""U_ST_CORPORATE_ARABIC_NAME"", 'C' AS ""CARD_TYPE"", T0.""U_ST_BP_CODE"",T0.""U_ST_NATIONAL_ID"",T0.""U_ST_TEL1"", T0.""U_ST_APPROVAL_STATUS""
,T0.""U_ST_CUSTOMER_GROUP"",T0.""U_ST_CREATION_DATE"",T0.""U_ST_CREATOR"",TO_VARCHAR(T0.""U_ST_CORPORATE_NATIONAL_ID""),'',T0.""U_ST_TEL_2"",T0.""U_ST_EMAIL"",T0.""U_ST_ACCOUNT_MANAGER"",T0.""U_ST_BROKER"",T0.""U_ST_CHANNEL"",T0.""U_ST_SUB_CHANNEL"",T0.""U_ST_CURRENCY"",T0.""U_ST_APPROVAL_NOTE"",T0.""U_ST_CORPORATE_ENGLISH_NAME""
,T0.""U_ST_INSURANCE_COMPANY"",T0.""U_ST_INSURANCE_END_DATE"",T0.""U_ST_MOBILE_1"",T0.""U_ST_MOBILE_2"",T0.""U_ST_SECTOR"",T0.""U_ST_WEBSITE"",
T0.""U_ST_GENERAL_MANAGER"",T0.""U_ST_PERSONAL_ID"", T0.""U_ST_PASSPORT_ID"", T0.""U_ST_GENDER""
FROM ""@ST_CCI_CORP_CARD""  T0
WHERE(IFNULL(T0.U_ST_APPROVAL_STATUS, 'P') = '{Approval_Status}' OR  '{Approval_Status}' = '-')
AND(T0.""Code"" LIKE '%{Card_ID}%')  AND(IFNULL(T0.U_ST_NATIONAL_ID, '') LIKE '%{National_ID}%')
AND IFNULL(T0.U_ST_TEL1,'') LIKE '%{Tel}%' AND UPPER(IFNULL(T0.U_ST_CORPORATE_ARABIC_NAME,'')) LIKE '%{Name.ToUpper().Replace("*", "%")}%'
";
            }
            else
            {
                
                    SQL = $@"SELECT T0.""Code"", T0.""U_ST_FULL_NAME_AR"", 'I' AS ""CARD_TYPE"", T0.""U_ST_BP_CODE"", IFNULL(T0.""U_ST_PARENT_ID"",''), T0.""U_ST_NATIONAL_ID"", T0.""U_ST_TEL1""
,T0.""U_ST_APPROVAL_STATUS"",T0.""U_ST_CUSTOMER_GROUP"",T0.""CreateDate"",T0.""U_ST_CREATOR"",T0.""U_ST_FIRST_NAME_AR"",T0.""U_ST_FATHER_NAME_AR"",T0.""U_ST_MIDDLE_NAME_AR"",
T0.""U_ST_SURNAME_AR"",T0.""U_ST_FIRST_NAME_EN"",T0.""U_ST_FATHER_NAME_EN"",T0.""U_ST_MIDDLE_NAME_EN"",T0.""U_ST_SURNAME_EN"",T0.""U_ST_PARENT_TYPE"",T0.""U_ST_PARENT_ID"",T0.""U_ST_PARENT_NAME"",
T0.""U_ST_PERSONAL_ID"",T0.""U_ST_PASSPORT_ID"",T0.""U_ST_TEL2"",T0.""U_ST_DATE_OF_BIRTH"",T0.""U_ST_GENDER"",T0.""U_ST_NATIONALITY"",T0.""U_ST_EMAIL"",T0.""U_ST_TITLE"",
T0.""U_ST_VIP"",T0.""U_ST_VIP_RELATED_TO"",T0.""U_ST_ACCOUNT_MANAGER"",T0.""U_ST_BROKER1"",T0.""U_ST_BROKER2"",T0.""U_ST_CHANNEL"",T0.""U_ST_SUB_CHANNEL"",T0.""U_ST_RESIDENCY"",T0.""U_ST_CURRENCY"",T0.""U_ST_PREFIX"",T0.""U_ST_APPROVAL_NOTE"",'','','','','','',''
FROM ""@ST_CCI_INDIV_CARD""  T0
WHERE (IFNULL(T0.U_ST_APPROVAL_STATUS, 'P') = '{Approval_Status}' OR  '{Approval_Status}' = '-') 
AND (T0.""Code"" LIKE '%{Card_ID}%')
AND (T0.U_ST_PARENT_ID = '{Parent_ID}' OR '{Parent_ID}'= '') AND (IFNULL(T0.U_ST_NATIONAL_ID,'') LIKE '%{National_ID}%')
AND IFNULL(T0.U_ST_TEL1,'') LIKE '%{Tel}%' AND UPPER(IFNULL(T0.U_ST_FULL_NAME_AR,'')) LIKE '%{Name.ToUpper().Replace("*", "%")}%' 
AND (IFNULL(T0.""U_ST_PARENT_ID"",0)=0 OR (Select T1.""U_ST_APPROVAL_STATUS"" From ""@ST_CCI_INDIV_CARD""  T1 where T1.""Code"" = T0.""U_ST_PARENT_ID"")='A')

UNION ALL 

SELECT T0.""Code"", T0.""U_ST_CORPORATE_ARABIC_NAME"", 'C' AS ""CARD_TYPE"", T0.""U_ST_BP_CODE"", '', '', T0.""U_ST_TEL1"", T0.""U_ST_APPROVAL_STATUS""
,T0.""U_ST_CUSTOMER_GROUP"",T0.""U_ST_CREATION_DATE"",T0.""U_ST_CREATOR"",'','','','','','','','','','','',TO_VARCHAR(T0.""U_ST_CORPORATE_NATIONAL_ID""),'',T0.""U_ST_TEL_2"",'','','',T0.""U_ST_EMAIL"",'','',T0.""U_ST_ACCOUNT_MANAGER"",T0.""U_ST_BROKER"",'',T0.""U_ST_CHANNEL"",T0.""U_ST_SUB_CHANNEL"",'',T0.""U_ST_CURRENCY"",'',T0.""U_ST_APPROVAL_NOTE"",T0.""U_ST_CORPORATE_ENGLISH_NAME""
,T0.""U_ST_INSURANCE_COMPANY"",T0.""U_ST_INSURANCE_END_DATE"",T0.""U_ST_MOBILE_1"",T0.""U_ST_MOBILE_2"",T0.""U_ST_SECTOR"",T0.""U_ST_WEBSITE"",
T0.""U_ST_GENERAL_MANAGER""
FROM ""@ST_CCI_CORP_CARD""  T0 
WHERE (IFNULL(T0.U_ST_APPROVAL_STATUS, 'P') = '{Approval_Status}' OR  '{Approval_Status}' = '-') 
AND (T0.""Code"" LIKE '%{Card_ID}%')  AND (IFNULL(T0.U_ST_NATIONAL_ID,'') LIKE '%{National_ID}%')
AND IFNULL(T0.U_ST_TEL1,'') LIKE '%{Tel}%' AND UPPER(IFNULL(T0.U_ST_CORPORATE_ARABIC_NAME,'')) LIKE '%{Name.ToUpper().Replace("*", "%")}%'
";
                
            }
            

            Recordset RC = Helper.Utility.Execute_Recordset_Query(company, SQL);
            DT_Result.Rows.Clear();
            DT_Result.Rows.Add(RC.RecordCount);
            int Count = 0;
          
            RC = Helper.Utility.Execute_Recordset_Query(company, SQL);
            DT_Result.Rows.Clear();
            DT_Result.Rows.Add(RC.RecordCount);
            for (int i = 0; i < RC.RecordCount; i++)
            {
                // DT_Result.SetValue("SELECTED", i, "Y");
                DT_Result.SetValue("Code", i, RC.Fields.Item("Code").Value);
                if (Member_Type != "C")
                {
                    DT_Result.SetValue("Name", i, RC.Fields.Item("U_ST_FULL_NAME_AR").Value);
                }
                else
                {
                    DT_Result.SetValue("Name", i, RC.Fields.Item("U_ST_CORPORATE_ARABIC_NAME").Value);
                }
                DT_Result.SetValue("CARD_TYPE", i, RC.Fields.Item("CARD_TYPE").Value);
                DT_Result.SetValue("BP_Code", i, RC.Fields.Item("U_ST_BP_CODE").Value);
                DT_Result.SetValue("National_ID", i, RC.Fields.Item("U_ST_NATIONAL_ID").Value);
                DT_Result.SetValue("Approval_Status", i, RC.Fields.Item("U_ST_APPROVAL_STATUS").Value);
                if (Member_Type != "C")
                {
                    DT_Result.SetValue("Parent_Code", i, RC.Fields.Item("U_ST_PARENT_ID").Value);
                }
                else
                {
                    DT_Result.SetValue("Parent_Code", i,string.Empty);
                }
                DT_Result.SetValue("Tel", i, RC.Fields.Item("U_ST_TEL1").Value);
                DT_Result.SetValue("Customer_Group", i, RC.Fields.Item("U_ST_CUSTOMER_GROUP").Value);
                  //U_ST_CREATOR
                if (Member_Type != "C")
                {
                    //DT_Result.SetValue("Name", i, RC.Fields.Item("U_ST_FULL_NAME_AR").Value);
                    DT_Result.SetValue("Create_Date", i, RC.Fields.Item("CreateDate").Value);
                    DT_Result.SetValue("First_Name_AR", i, RC.Fields.Item("U_ST_FIRST_NAME_AR").Value);
                    DT_Result.SetValue("Father_Name_AR", i, RC.Fields.Item("U_ST_FATHER_NAME_AR").Value);
                    DT_Result.SetValue("Middle_Name_AR", i, RC.Fields.Item("U_ST_MIDDLE_NAME_AR").Value);
                    DT_Result.SetValue("Surname_AR", i, RC.Fields.Item("U_ST_SURNAME_AR").Value);
                    DT_Result.SetValue("First_Name_EN", i, RC.Fields.Item("U_ST_FIRST_NAME_EN").Value);
                    DT_Result.SetValue("Father_Name_EN", i, RC.Fields.Item("U_ST_FATHER_NAME_EN").Value);
                    DT_Result.SetValue("Middle_Name_EN", i, RC.Fields.Item("U_ST_MIDDLE_NAME_EN").Value);
                    DT_Result.SetValue("Surname_EN", i, RC.Fields.Item("U_ST_SURNAME_EN").Value);
                    DT_Result.SetValue("Parent_Type", i, RC.Fields.Item("U_ST_PARENT_TYPE").Value);
                    DT_Result.SetValue("Parent_ID", i, RC.Fields.Item("U_ST_PARENT_ID").Value);
                    DT_Result.SetValue("Parent_Name", i, RC.Fields.Item("U_ST_PARENT_NAME").Value);
                    DT_Result.SetValue("Personal_ID", i, RC.Fields.Item("U_ST_PERSONAL_ID").Value);
                    DT_Result.SetValue("Passport_ID", i, RC.Fields.Item("U_ST_PASSPORT_ID").Value);
                    DT_Result.SetValue("Tel2", i, RC.Fields.Item("U_ST_TEL2").Value);
                }
                else
                {
                    DT_Result.SetValue("Create_Date", i, RC.Fields.Item("U_ST_CREATOR").Value);
                    DT_Result.SetValue("First_Name_AR", i, string.Empty);
                    DT_Result.SetValue("Father_Name_AR", i, string.Empty);
                    DT_Result.SetValue("Middle_Name_AR", i, string.Empty);
                    DT_Result.SetValue("Surname_AR", i, string.Empty);
                    DT_Result.SetValue("First_Name_EN", i, string.Empty);
                    DT_Result.SetValue("Father_Name_EN", i, string.Empty);
                    DT_Result.SetValue("Middle_Name_EN", i, string.Empty);
                    DT_Result.SetValue("Surname_EN", i, string.Empty);
                    DT_Result.SetValue("Parent_Type", i, string.Empty);
                    DT_Result.SetValue("Parent_ID", i, string.Empty);
                    DT_Result.SetValue("Parent_Name", i, string.Empty);
                    DT_Result.SetValue("Personal_ID", i, string.Empty);
                    DT_Result.SetValue("Passport_ID", i, string.Empty);
                    DT_Result.SetValue("Tel2", i, string.Empty);
                }


                if (Member_Type != "C")
                {
                    DT_Result.SetValue("DOB", i, RC.Fields.Item("U_ST_DATE_OF_BIRTH").Value);
                }
                else
                {
                    DT_Result.SetValue("DOB", i, string.Empty);
                }
                DT_Result.SetValue("Gender", i, RC.Fields.Item("U_ST_GENDER").Value);
                if (Member_Type != "C")
                {
                    DT_Result.SetValue("Nationality", i, RC.Fields.Item("U_ST_NATIONALITY").Value);
                }
                else
                {
                    DT_Result.SetValue("Nationality", i, string.Empty);
                }
                DT_Result.SetValue("Email", i, RC.Fields.Item("U_ST_EMAIL").Value);
                if (Member_Type != "C")
                {
                    DT_Result.SetValue("Title", i, RC.Fields.Item("U_ST_TITLE").Value);
                    DT_Result.SetValue("VIP", i, RC.Fields.Item("U_ST_VIP").Value);
                    DT_Result.SetValue("VIP_Related_To", i, RC.Fields.Item("U_ST_VIP_RELATED_TO").Value);
                }
                else
                {
                    DT_Result.SetValue("Title", i,string.Empty);
                    DT_Result.SetValue("VIP", i, string.Empty);
                    DT_Result.SetValue("VIP_Related_To", i, string.Empty);
                }
                DT_Result.SetValue("Account_Manager", i, RC.Fields.Item("U_ST_ACCOUNT_MANAGER").Value);
                if (Member_Type != "C")
                {
                    DT_Result.SetValue("Broker1", i, RC.Fields.Item("U_ST_BROKER1").Value);
                    DT_Result.SetValue("Broker2", i, RC.Fields.Item("U_ST_BROKER2").Value);
                }
                else
                {
                    DT_Result.SetValue("Broker1", i, RC.Fields.Item("U_ST_BROKER").Value);
                    DT_Result.SetValue("Broker2", i, string.Empty);
                }
                string v = RC.Fields.Item("U_ST_CHANNEL").Value.ToString();
                try
                {
                    DT_Result.SetValue("Channel", i, RC.Fields.Item("U_ST_CHANNEL").Value);
                }
                catch (Exception ex)
                {
                    DT_Result.SetValue("Channel", i, string.Empty);
                }
                
                DT_Result.SetValue("Sub_Channel", i, RC.Fields.Item("U_ST_SUB_CHANNEL").Value);
                if (Member_Type != "C")
                {
                    DT_Result.SetValue("Residency", i, RC.Fields.Item("U_ST_RESIDENCY").Value);
                    if (RC.Fields.Item("U_ST_CURRENCY").Value.ToString() == "##")
                    {
                        DT_Result.SetValue("Currency", i, "All Currencies");
                    }
                    else
                    {
                        DT_Result.SetValue("Currency", i, RC.Fields.Item("U_ST_CURRENCY").Value);
                    }
                    DT_Result.SetValue("Prefix", i, RC.Fields.Item("U_ST_PREFIX").Value);
                }
                else
                {
                    DT_Result.SetValue("Residency", i, string.Empty);
                    DT_Result.SetValue("Currency", i, string.Empty);
                    DT_Result.SetValue("Prefix", i, string.Empty);
                }
                    
                DT_Result.SetValue("Approval_Note", i, RC.Fields.Item("U_ST_APPROVAL_NOTE").Value);

               // }

                RC.MoveNext();
            }
            ((Grid)form.Items.Item("16").Specific).AutoResizeColumns();
            form.Freeze(false);

        }



    }
}
