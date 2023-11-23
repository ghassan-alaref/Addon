using SAPbobsCOM;
using SAPbouiCOM;
using ST.KHCF.Customization.Logic;
using ST.KHCF.Customization.Logic.Classes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ST.KHCF.Customization.Forms.CCI
{
    internal class Frm_CCI_Corporate_Member_Card : Parent_Form
    {
        internal static SAPbobsCOM.Company company;
        internal static Application SBO_Application;
        internal static Parent_Form Form_Obj;
        

        
        internal override Depends_List[] Get_Depends_List_List()
        {
            List<Depends_List> Result = new List<Depends_List>();
            Result.AddRange(base.Get_Depends_List_List());
            Result.Add(new Depends_List() { Item_ID = "133", Parent_Item_ID = "47", SQL = $@"SELECT T0.""Code"", T0.""Name"" FROM ""@ST_SUB_CHANNEL""  T0 WHERE T0.""U_ST_CHANNEL"" = '{{0}}'" });

            return Result.ToArray();
        }

        internal override void Initialize_Form(Form form)
        {
            base.Initialize_Form(form);
            Matrix Mat_Att = (Matrix)form.Items.Item("500").Specific;
            base.Fill_Attachment_ComboBox(Mat_Att);
            string SQL_Customer_Group = $@"SELECT ""GroupCode"" AS ""Code"", T0.""GroupName"" AS ""Name"" FROM OCRG T0 
WHERE  T0.""GroupType""  = 'C' AND T0.""U_ST_TYPE"" = 'C' AND U_ST_CUSTOMER_TYPE = 'C'";
            Helper.Utility.Fill_One_ComboBoxBySQL(company, form, "612", SQL_Customer_Group, true);
            
            string CCI_Department_ID = Configurations.Get_CCI_Department(company);
            string SQL_Account_Manager = $@"SELECT T0.""SlpCode"" AS ""Code"", T0.""SlpName"" AS ""Name"" 
FROM OSLP T0  INNER JOIN OHEM T1 ON T0.""SlpCode"" = T1.""salesPrson"" 
WHERE T1.""dept"" = {CCI_Department_ID}";
            Helper.Utility.Fill_One_ComboBoxBySQL(company, form, "137", SQL_Account_Manager, true);
            string Broker_Vendor_Group = Configurations.Get_Broker_Vendor_Group(company);
            SAPbouiCOM.ChooseFromList CFL_Broker = form.ChooseFromLists.Item("CFL_Broker");
            
            Conditions Broker_Cons = CFL_Broker.GetConditions();
            Condition Broker_Con = Broker_Cons.Add();
            Broker_Con.Alias = "GroupCode";
            Broker_Con.Operation = BoConditionOperation.co_EQUAL;
            Broker_Con.CondVal = Broker_Vendor_Group;
            CFL_Broker.SetConditions(Broker_Cons);
            Grid Grd_Membership = (Grid)form.Items.Item("624").Specific;
            string SQL_Cov = $@"SELECT ""Code"" , ""Name"" FROM ""@ST_COVERAGE"" Order By ""Code""";
            Helper.Utility.FillGridComboBoxForSQL(company, Grd_Membership, "U_ST_COVERAGE", SQL_Cov);
            string SQL_Currency = @"SELECT T0.""CurrCode"" AS ""Code"", T0.""CurrName"" As ""Name"" FROM OCRN T0";
            Helper.Utility.Fill_One_ComboBoxBySQL(company, form, "61", SQL_Currency, true);
            ComboBox Cmb_Currency = (ComboBox)form.Items.Item("61").Specific;
            Cmb_Currency.ValidValues.Add("##", "All Currencies");


            Matrix Mat_Add = (Matrix)form.Items.Item("20").Specific;
            Form_Obj.Fill_Address_ComboBox(Mat_Add);

            Matrix Mat_Cont = (Matrix)form.Items.Item("701").Specific;
            Fill_Position_ComboBox(Mat_Cont);


            Grid Grd_Members = (Grid)form.Items.Item("138").Specific;
            string SQL_Memeber_Customer_Group = $@"SELECT ""GroupCode"" AS ""Code"", T0.""GroupName"" AS ""Name"" FROM OCRG T0 
WHERE  T0.""GroupType""  = 'C' AND T0.""U_ST_TYPE"" = 'C' And U_ST_Customer_TYpe = 'C'";
            Helper.Utility.FillGridComboBoxForSQL(company, Grd_Members, "ST_CUSTOMER_GROUP", SQL_Memeber_Customer_Group, true);

            Grd_Members.Columns.Item("SELECTED").AffectsFormMode = false;
            ((ComboBoxColumn)Grd_Members.Columns.Item("ST_GENDER")).ValidValues.Add("M", "Male");
            ((ComboBoxColumn)Grd_Members.Columns.Item("ST_GENDER")).ValidValues.Add("F", "Female");
            ((ComboBoxColumn)Grd_Members.Columns.Item("ST_GENDER")).DisplayType = BoComboDisplayType.cdt_Description;
            ((ComboBoxColumn)Grd_Members.Columns.Item("ST_APPROVAL_STATUS")).ValidValues.Add("P", "Pending");
            ((ComboBoxColumn)Grd_Members.Columns.Item("ST_APPROVAL_STATUS")).ValidValues.Add("A", "Approved");
            ((ComboBoxColumn)Grd_Members.Columns.Item("ST_APPROVAL_STATUS")).ValidValues.Add("R", "Rejected");
            ((ComboBoxColumn)Grd_Members.Columns.Item("ST_APPROVAL_STATUS")).DisplayType = BoComboDisplayType.cdt_Description;
            Grd_Members.AutoResizeColumns();
            
            ((Matrix)form.Items.Item("20").Specific).AutoResizeColumns();
            ((Matrix)form.Items.Item("500").Specific).AutoResizeColumns();

            ButtonCombo Btn_Cmb_Action = (ButtonCombo)form.Items.Item("163").Specific;
            Btn_Cmb_Action.ValidValues.Add("-", "Can Also");
            Btn_Cmb_Action.ValidValues.Add("M", "Add/Renew Membership");
            Btn_Cmb_Action.ValidValues.Add("R", "Remove");
            form.DataSources.UserDataSources.Item("163").Value = "-";

            form.Items.Item("139").SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 2, BoModeVisualBehavior.mvb_False);
            form.Items.Item("140").SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 2, BoModeVisualBehavior.mvb_False);




            form.Items.Item("163").AffectsFormMode = false;
            form.Items.Item("3").Click();
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
                if ((BusinessObjectInfo.EventType == BoEventTypes.et_FORM_DATA_UPDATE || BusinessObjectInfo.EventType == BoEventTypes.et_FORM_DATA_ADD)
                    && BusinessObjectInfo.BeforeAction)
                {
                    string User_Role = Utility.Get_Current_User_Role(company);
                    if (string.IsNullOrEmpty(User_Role))
                    {
                        throw new Custom_Exception("You are Not autorized to Add or Update The Card.");
                    }
                    BubbleEvent = Validate_Data(BusinessObjectInfo);
                    if (!BubbleEvent)
                    {
                        return BubbleEvent;
                    }
                    Before_Adding_UDO(BusinessObjectInfo);
                    if (BusinessObjectInfo.EventType == BoEventTypes.et_FORM_DATA_UPDATE)
                    {
                        if (!ADD_Update_UDO(BusinessObjectInfo))
                        {
                            throw new Custom_Exception(Loader.New_Msg);
                        }
                    }
                }

                if ((BusinessObjectInfo.EventType == BoEventTypes.et_FORM_DATA_UPDATE || BusinessObjectInfo.EventType == BoEventTypes.et_FORM_DATA_ADD) && BusinessObjectInfo.ActionSuccess && !BusinessObjectInfo.BeforeAction)
                {
                    if (BusinessObjectInfo.EventType == BoEventTypes.et_FORM_DATA_ADD)
                    {
                        if (!ADD_Update_UDO(BusinessObjectInfo))
                        {
                            throw new Custom_Exception(Loader.New_Msg);
                        }   
                    }
                    if (BusinessObjectInfo.EventType == BoEventTypes.et_FORM_DATA_ADD && BusinessObjectInfo.ActionSuccess)
                    {
                        Approve(null);
                    }
                    if(BusinessObjectInfo.EventType == BoEventTypes.et_FORM_DATA_UPDATE)
                    {
                        Form currenctForm = SBO_Application.Forms.ActiveForm;
                        string UDO_Code = currenctForm.DataSources.DBDataSources.Item(0).GetValue("Code", 0);
                        string Name_Field = "";
                        string Parent_Fields = "";
                        if (Form_Obj.UDO_Info.KHCF_Object == KHCF_Objects.CCI_Corporate_Member_Card)
                        {
                            Name_Field = "U_ST_CORPORATE_ARABIC_NAME";
                        }
                        else
                        {
                            Name_Field = "U_ST_FULL_NAME_AR";
                            Parent_Fields = ", U_ST_PARENT_ID, U_ST_PARENT_TYPE";
                        }
                        string SQL_BP = $@"SELECT U_ST_BP_CODE, U_ST_CUSTOMER_GROUP,U_ST_CURRENCY, {Name_Field} {Parent_Fields} FROM ""@{Form_Obj.UDO_Info.Table_Name}"" WHERE ""Code"" = '{UDO_Code}' ";
                        Recordset RC_BP = Helper.Utility.Execute_Recordset_Query(company, SQL_BP);
                        string BP_Code = RC_BP.Fields.Item("U_ST_BP_CODE").Value.ToString();
                        Utility.Update_BP(company, BP_Code, UDO_Code, Form_Obj.UDO_Info);
                    }
                }

                if (BusinessObjectInfo.EventType == BoEventTypes.et_FORM_DATA_LOAD && !BusinessObjectInfo.BeforeAction)
                {
                    Form_Data_Load(BusinessObjectInfo);
                }
            }
            catch (Exception ex)
            {
                Loader.New_Msg = ex.Message;
                BubbleEvent = false;
            }
            return BubbleEvent;
        }

        private static void Form_Data_Load(BusinessObjectInfo businessObjectInfo)
        {
            Form form = SBO_Application.Forms.Item(businessObjectInfo.FormUID);
            try
            {
                SBO_Application.SetStatusBarMessage("Loading", BoMessageTime.bmt_Short, false);
                string Card_ID = form.DataSources.DBDataSources.Item(0).GetValue("Code", 0);

                Form_Obj.Load_Depends_Items(form);
                Form_Obj.Set_Fields(form);
                Member_Cards_UI.Load_Sub_Members(form, "C", Card_ID);
                //Load_Summary(form, Card_ID);
                Load_Memberships(form, Card_ID);
                Member_Cards_UI.Load_Communication_Log(form, "C", Card_ID);

                ((Matrix)form.Items.Item("20").Specific).AutoResizeColumns();
                ((Matrix)form.Items.Item("500").Specific).AutoResizeColumns();
            }
            catch (Exception ex)
            {
                //Console.WriteLine(ex.Message);
                form.Freeze(false);
                SBO_Application.StatusBar.SetText($@"Error during loading data[{ex.Message}]", BoMessageTime.bmt_Medium, BoStatusBarMessageType.smt_Error);
            }
        }

        //private static void Load_Summary(Form form, string Card_ID)
        //{

        //    string SQL = $@"call ST_MEMBERSHIP_SUMMARY('{Card_ID}')";
        //    Recordset RC = Helper.Utility.Execute_Recordset_Query(company, SQL);

        //    form.DataSources.UserDataSources.Item("705").Value = RC.Fields.Item("Number_of_Members").Value.ToString();
        //    form.DataSources.UserDataSources.Item("707").Value = RC.Fields.Item("Total_Premiums").Value.ToString();
        //    form.DataSources.UserDataSources.Item("709").Value = RC.Fields.Item("Total_Invoiced").Value.ToString();
        //    form.DataSources.UserDataSources.Item("711").Value = RC.Fields.Item("Number_of_Active_Members").Value.ToString();
        //    form.DataSources.UserDataSources.Item("713").Value = RC.Fields.Item("Total_Premiums_For_Active_Members").Value.ToString();
        //    form.DataSources.UserDataSources.Item("715").Value = RC.Fields.Item("Number_of_Stopped_Members").Value.ToString();
        //    form.DataSources.UserDataSources.Item("717").Value = RC.Fields.Item("Total_Premiums_for_Stopped_Members").Value.ToString();
        //    form.DataSources.UserDataSources.Item("719").Value = RC.Fields.Item("Number_of_Canceled_Members").Value.ToString();
        //    form.DataSources.UserDataSources.Item("721").Value = RC.Fields.Item("Total_Premiums_for_Canceled_Members").Value.ToString();
        //    form.DataSources.UserDataSources.Item("723").Value = RC.Fields.Item("Total_Number_of_Additions").Value.ToString();
        //    form.DataSources.UserDataSources.Item("725").Value = RC.Fields.Item("Total_Additions_Premiums").Value.ToString();
        //    form.DataSources.UserDataSources.Item("727").Value = RC.Fields.Item("Total_Net_Premiums").Value.ToString();
        //    form.DataSources.UserDataSources.Item("729").Value = RC.Fields.Item("U_ST_NUMBER_OF_EMPLOYEES_UNDER_60").Value.ToString();
        //    form.DataSources.UserDataSources.Item("731").Value = RC.Fields.Item("U_ST_NUMBER_OF_EMPLOYEES_ABOVE_60").Value.ToString();
        //    form.DataSources.UserDataSources.Item("733").Value = RC.Fields.Item("U_ST_NUMBER_OF_FAMILIES_UNDER_60").Value.ToString();
        //    form.DataSources.UserDataSources.Item("735").Value = RC.Fields.Item("U_ST_NUMBER_OF_FAMILIES_ABOVE_60").Value.ToString();
        //    form.DataSources.UserDataSources.Item("737").Value = RC.Fields.Item("U_ST_NUMBER_OF_RETIRED_UNDER_60").Value.ToString();
        //    form.DataSources.UserDataSources.Item("739").Value = RC.Fields.Item("U_ST_NUMBER_OF_RETIRED_ABOVE_60").Value.ToString();
        //    form.DataSources.UserDataSources.Item("741").Value = RC.Fields.Item("U_ST_NUMB_RET_FAMILIES_UNDER_60").Value.ToString();
        //    form.DataSources.UserDataSources.Item("743").Value = RC.Fields.Item("U_ST_NUMB_RET_FAMILIES_ABOVE_60").Value.ToString();
        //    form.DataSources.UserDataSources.Item("745").Value = RC.Fields.Item("U_ST_NUMBER_OF_STUDENTS").Value.ToString();

        //}

        private static void Load_Memberships(Form form, string Card_ID)
        {
            DataTable DT_Membership = form.DataSources.DataTables.Item("MEMBERSHIP");
            DT_Membership.Rows.Clear();
            //string SQL_Membership = $@"SELECT T0.""Code"" FROM ""@ST_CORP_MEMBERSHIP""  T0 WHERE T0.""U_ST_MEMBER_CARD"" ='{Card_ID}'";
            //Recordset RC_Membership = Helper.Utility.Execute_Recordset_Query(company, SQL_Membership);
            //string Membership_Code = RC_Membership.Fields.Item("Code").Value.ToString();
            //string SQL_Call_Procedure = $@"call ST_MEMBERSHIP_SUMMARY('{Membership_Code}')";
            //            string SQL_Membership = $@"SELECT T0.""Code"", T0.""U_ST_MEMBER_CARD"", T0.""U_ST_CREATION_DATE""
            //, T0.""U_ST_START_DATE"", T0.""U_ST_END_DATE"", T0.""U_ST_ACTIVE"", T0.U_ST_AUTOMATIC_RENEWAL , T0.U_ST_COVERAGE,T1.U_ST_CUSTOMER_GROUP
            //FROM ""@ST_CORP_MEMBERSHIP""  T0
            //JOIN ""@ST_CCI_CORP_CARD"" T1 ON T1.""Code"" = T0.""U_ST_MEMBER_CARD""
            //WHERE T0.""U_ST_MEMBER_CARD"" ='{Card_ID}'";

            string SQL_Membership = $@"SELECT T0.""Code"",  (Select Count(*) From   ""@ST_INDIV_MEMBERSHIP"" T2 inner join  ""@ST_CORP_MEMBERSHIP""  T3 ON T3.""Code"" = T2.""U_ST_PARENT_MEMBERSHIP_ID"" where T3.""U_ST_MEMBER_CARD"" ='{Card_ID}' 
and T2.""U_ST_PARENT_MEMBERSHIP_TYPE"" = 'C') As ""Number_of_Members"",  (Select Sum(U_ST_PREMIUM) From ""@ST_INDIV_MEMBERSHIP"" T2 inner join  ""@ST_CORP_MEMBERSHIP""  T3 ON T3.""Code"" = T2.""U_ST_PARENT_MEMBERSHIP_ID"" where T3.""U_ST_MEMBER_CARD"" ='{Card_ID}' 
and T2.""U_ST_PARENT_MEMBERSHIP_TYPE"" = 'C') As ""Total_Premiums"",  (Select Count(*) From ""@ST_INDIV_MEMBERSHIP"" T2 inner join  ""@ST_CORP_MEMBERSHIP""  T3 ON T3.""Code"" = T2.""U_ST_PARENT_MEMBERSHIP_ID"" where T3.""U_ST_MEMBER_CARD"" ='{Card_ID}'
And T2.""U_ST_ACTIVE""='Y' and T2.""U_ST_PARENT_MEMBERSHIP_TYPE"" = 'C') As ""Number_of_Active_Members"", (Select Sum(U_ST_PREMIUM) From   ""@ST_INDIV_MEMBERSHIP"" T2 inner join  ""@ST_CORP_MEMBERSHIP""  T3 ON T3.""Code"" = T2.""U_ST_PARENT_MEMBERSHIP_ID"" where T3.""U_ST_MEMBER_CARD"" ='{Card_ID}' and T2.""U_ST_PARENT_MEMBERSHIP_TYPE"" = 'C' And T2.""U_ST_ACTIVE""='Y') As ""Total_Premiums_For_Active_Members"",  (Select Count(*) From  ""@ST_INDIV_MEMBERSHIP"" T2 inner join  ""@ST_CORP_MEMBERSHIP""  T3 ON T3.""Code"" = T2.""U_ST_PARENT_MEMBERSHIP_ID"" where T3.""U_ST_MEMBER_CARD"" ='{Card_ID}' 
And T2.""U_ST_MEMBERSHIP_STATUS""='S' and T2.""U_ST_PARENT_MEMBERSHIP_TYPE"" = 'C') As ""Number_of_Stopped_Members"", (Select Sum(U_ST_PREMIUM) From ""@ST_INDIV_MEMBERSHIP"" T2 inner join  ""@ST_CORP_MEMBERSHIP""  T3 ON T3.""Code"" = T2.""U_ST_PARENT_MEMBERSHIP_ID"" where T3.""U_ST_MEMBER_CARD"" ='{Card_ID}' 
and T2.""U_ST_PARENT_MEMBERSHIP_TYPE"" = 'C' And T2.""U_ST_MEMBERSHIP_STATUS""='S') As ""Total_Premiums_for_Stopped_Members"",  (Select Count(*) From ""@ST_INDIV_MEMBERSHIP"" T2 inner join  ""@ST_CORP_MEMBERSHIP""  T3 ON T3.""Code"" = T2.""U_ST_PARENT_MEMBERSHIP_ID"" where T3.""U_ST_MEMBER_CARD"" ='{Card_ID}' And T2.""U_ST_MEMBERSHIP_STATUS""='C' and T2.""U_ST_PARENT_MEMBERSHIP_TYPE"" = 'C') As ""Number_of_Canceled_Members"", (Select Sum(U_ST_PREMIUM) From ""@ST_INDIV_MEMBERSHIP"" T2 inner join  ""@ST_CORP_MEMBERSHIP""  T3 ON T3.""Code"" = T2.""U_ST_PARENT_MEMBERSHIP_ID"" where T3.""U_ST_MEMBER_CARD"" ='{Card_ID}' and T2.""U_ST_PARENT_MEMBERSHIP_TYPE"" = 'C' And T2.""U_ST_MEMBERSHIP_STATUS""='C') As ""Total_Premiums_for_Canceled_Members"", 0 As ""Total_Number_of_Additions"" , 0 as ""Total_Additions_Premiums"" , 0 as ""Total_Net_Premiums"", T0.""U_ST_MEMBER_CARD"", T0.""U_ST_CREATION_DATE""
, T0.""U_ST_START_DATE"", T0.""U_ST_END_DATE"", T0.""U_ST_ACTIVE"", T0.U_ST_AUTOMATIC_RENEWAL , T0.U_ST_COVERAGE,T1.U_ST_CUSTOMER_GROUP
FROM ""@ST_CORP_MEMBERSHIP""  T0
JOIN ""@ST_CCI_CORP_CARD"" T1 ON T1.""Code"" = T0.""U_ST_MEMBER_CARD""
WHERE T0.""U_ST_MEMBER_CARD"" = '{Card_ID}'";

            Recordset RC_Membership_Procedure = Helper.Utility.Execute_Recordset_Query(company, SQL_Membership);
            DT_Membership.Rows.Add(RC_Membership_Procedure.RecordCount);

            for (int i = 0; i < RC_Membership_Procedure.RecordCount; i++)
            {
                for (int J = 0; J < DT_Membership.Columns.Count; J++)
                {
                    string Col_Name = string.Empty;
                    string UDF_Name = string.Empty;
                    try
                    {
                        Col_Name = DT_Membership.Columns.Item(J).Name;
                        UDF_Name = Col_Name;
                        double result = -1;
                        double.TryParse(RC_Membership_Procedure.Fields.Item(UDF_Name).Value.ToString(), out result);

                        if ((result != 0 && result != -1) && (Col_Name != "Code" && Col_Name != "U_ST_MEMBER_CARD" && Col_Name != "U_ST_COVERAGE"))
                        {
                            DT_Membership.SetValue(Col_Name, i, result.ToString("N03"));
                        }
                        else
                        {
                            DT_Membership.SetValue(Col_Name, i, RC_Membership_Procedure.Fields.Item(UDF_Name).Value);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(Col_Name + UDF_Name);
                    }
                }
                RC_Membership_Procedure.MoveNext();
            }
            Grid Grd_Membership = (Grid)form.Items.Item("624").Specific;

            Grd_Membership.AutoResizeColumns();

        }

//        private static void Load_Sub_Members(Form form, string Card_ID)
//        {
//            form.Freeze(true);
//            try
//            {
//                DataTable DT_Members = form.DataSources.DataTables.Item("MEMBERS");
//                DT_Members.Rows.Clear();
//                string Txt_Filter = form.DataSources.UserDataSources.Item("651").Value.ToUpper();
//                string SQL_Members = $@"SELECT TOP 30 * FROM ""@ST_CCI_INDIV_CARD""  T0 
//WHERE T0.""U_ST_PARENT_ID"" = '{Card_ID}' AND  T0.""U_ST_PARENT_TYPE"" = 'C'
//AND (UPPER(IFNULL(T0.U_ST_FULL_NAME_AR,'')) LIKE '%{Txt_Filter}%' OR UPPER(IFNULL(T0.U_ST_NATIONAL_ID,'')) LIKE '%{Txt_Filter}%')
//";
//                Recordset RC_Members = Helper.Utility.Execute_Recordset_Query(company, SQL_Members);
//                DT_Members.Rows.Add(RC_Members.RecordCount);

//                for (int i = 0; i < RC_Members.RecordCount; i++)
//                {
//                    for (int J = 1; J < DT_Members.Columns.Count; J++)
//                    {
//                        string Col_Name = DT_Members.Columns.Item(J).Name;
//                        string UDF_Name;
//                        if (Col_Name == "Code")
//                        {
//                            UDF_Name = Col_Name;
//                        }
//                        else
//                        {
//                            UDF_Name = "U_" + Col_Name;
//                        }
//                        if (Col_Name == "SELECTED")
//                        {
//                            continue;
//                        }
//                        try
//                        {
//                            if (!string.IsNullOrEmpty(RC_Members.Fields.Item(UDF_Name).Value.ToString()) && !string.IsNullOrEmpty(Col_Name))
//                            {
//                                DT_Members.SetValue(Col_Name, i, RC_Members.Fields.Item(UDF_Name).Value);
//                            }
//                            else
//                            {
//                                DT_Members.SetValue(Col_Name, i, "");
//                            }
//                        }
//                        catch (Exception ex)
//                        {
//                            throw new Custom_Exception(ex.Message);
//                        }
//                    }
//                    RC_Members.MoveNext();
//                }
//                Grid Grd_Members = (Grid)form.Items.Item("138").Specific;
//                for (int i = 0; i < DT_Members.Rows.Count; i++)
//                {
//                    for (int j = 2; j < Grd_Members.Columns.Count + 1; j++)
//                    {
//                        Grd_Members.CommonSetting.SetCellEditable(i + 1, j, false);
//                    }
//                }
//                Grd_Members.AutoResizeColumns();
//            }
//            catch (Exception ex)
//            {
//                throw new Logic.Custom_Exception($"Error during loading the Sub Members[{ex.Message}]");
//            } 
//            finally
//            {
//                form.Freeze(false);
//            }
            
//        }

        private static bool ADD_Update_UDO(BusinessObjectInfo businessObjectInfo)
        {
            Form form = SBO_Application.Forms.Item(businessObjectInfo.FormUID);
            System.Xml.XmlDocument XML_Doc = new System.Xml.XmlDocument();
            string XML_Text = businessObjectInfo.ObjectKey.Replace(" ", "").Replace("=", " = ");
            XML_Doc.LoadXml(XML_Text);

            string UDO_Code = XML_Doc.GetElementsByTagName("Code")[0].InnerText;
            string UDO_Name = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_CORPORATE_ARABIC_NAME", 0);

            if (!Add_Members(form, UDO_Code, UDO_Name))
            {
                return false;
            }

            if (form.DataSources.DBDataSources.Item(0).GetValue("U_ST_APPROVAL_STATUS", 0) == "P" && form.Mode!=BoFormMode.fm_ADD_MODE)
            {
                Form_Obj.Send_Alert_For_Approve(UDO_Code);
            }
            else
            {
                string BP_Code = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_BP_CODE", 0);
                UDO_Definition UDO_Info = Logic.Objects_Logic.All_UDO_Definition.FirstOrDefault(U => U.KHCF_Object == KHCF_Objects.CCI_Corporate_Member_Card);
                if (!string.IsNullOrEmpty(BP_Code))
                    Member_Cards_UI.Update_Card_BP(form, UDO_Code, UDO_Info);
            }
            return true;
        }

        private static void Reject(ItemEvent pVal)
        {
            SAPbouiCOM.Form form = SBO_Application.Forms.Item(pVal.FormUID);
            string UDO_Code = form.DataSources.DBDataSources.Item(0).GetValue("Code", 0);
            string Approval_Note = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_APPROVAL_NOTE", 0);

            Logic.KHCF_Approval.Reject_MemberCard(company, UDO_Code, Approval_Note, Form_Obj.UDO_Info);
            SBO_Application.StatusBar.SetText("Operation completed successfully!", BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Success);
            SBO_Application.Menus.Item("1304").Activate();
        }

        private static void Approve(ItemEvent pVal)
        {
            SAPbouiCOM.Form form = null;
            if (pVal != null)
                form = SBO_Application.Forms.Item(pVal.FormUID);
            else
                form = SBO_Application.Forms.ActiveForm;
            string UDO_Code = form.DataSources.DBDataSources.Item(0).GetValue("Code", 0);
            string Approval_Note = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_APPROVAL_NOTE", 0);

            Logic.KHCF_Approval.Approve_MemberCard(company, UDO_Code, Approval_Note, Form_Obj.UDO_Info);
            string SQL_Parent = $@"Select T0.""Code"",T0.""U_ST_APPROVAL_STATUS"" from ""@ST_CCI_INDIV_CARD"" T0 Where T0.""U_ST_PARENT_ID""='{UDO_Code}'";
            Recordset rc_Parent = Helper.Utility.Execute_Recordset_Query(company, SQL_Parent);
            while (!rc_Parent.EoF)
            {
                string Child_Code = rc_Parent.Fields.Item("Code").Value.ToString();
                UDO_Definition UDO_Info = Logic.Objects_Logic.All_UDO_Definition.FirstOrDefault(U => U.KHCF_Object == KHCF_Objects.CCI_Member_Card);
                Logic.KHCF_Approval.Approve_MemberCard(company, Child_Code, $"Approved From Parent [{UDO_Code}]", UDO_Info);
                rc_Parent.MoveNext();
            }


            SBO_Application.StatusBar.SetText("Operation completed successfully!", BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Success);
            //SBO_Application.Menus.Item("1304").Activate();

        }

        private static bool Add_Members(Form form, string UDO_Code, string UDO_Name)
        {
            company.StartTransaction();
            try
            {
                List<string> Codes = new List<string>();
                DataTable DT_Members = form.DataSources.DataTables.Item("MEMBERS");
                for (int i = 0; i < DT_Members.Rows.Count; i++)
                {
                    if (DT_Members.GetValue("Code", i).ToString() == "" && DT_Members.GetValue("ST_GENDER", i).ToString() != "")
                    {
                        KHCF_BP BP = new KHCF_BP();
                        BP.BP_Group = int.Parse(form.DataSources.DBDataSources.Item(0).GetValue("U_ST_CUSTOMER_GROUP", 0));
                        BP.CardName = DT_Members.GetValue("ST_FULL_NAME_AR", 0).ToString();
                        BP.FatherCode = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_BP_CODE", 0);
                        BP.Mobile = DT_Members.GetValue("ST_MOBILE", 0).ToString();
                        string title = DT_Members.GetValue("ST_TITLE", 0).ToString();
                        string prefix = DT_Members.GetValue("ST_PREFIX", 0).ToString();
                        if (form.DataSources.DBDataSources.Item(0).GetValue("U_ST_FATHER_TYPE", 0) == "P")
                        {
                            BP.FatherType = BoFatherCardTypes.cPayments_sum;
                        }
                        else
                        {
                            BP.FatherType = BoFatherCardTypes.cDelivery_sum;
                        }
                        BP.Email = ((EditText)(form.Items.Item("888").Specific)).Value.ToString();
                        string tel1 = ((EditText)(form.Items.Item("39").Specific)).Value.ToString();
                        string BP_Code = "";

                        string New_UDO_Code = Utility.Get_New_UDO_Code(company, KHCF_Objects.CCI_Member_Card);

                        CompanyService oCmpSrv = company.GetCompanyService();
                        GeneralService oGeneralService = oCmpSrv.GetGeneralService("ST_CCI_INDIV_CARD");
                        GeneralData oGeneralData = (GeneralData)oGeneralService.GetDataInterface(GeneralServiceDataInterfaces.gsGeneralData);

                        oGeneralData.SetProperty("Code", New_UDO_Code);
                        oGeneralData.SetProperty("U_ST_BP_CODE", BP_Code);
                        oGeneralData.SetProperty("U_ST_PARENT_ID", UDO_Code);
                        oGeneralData.SetProperty("U_ST_PARENT_NAME", UDO_Name);
                        oGeneralData.SetProperty("U_ST_PARENT_TYPE", "C");
                        oGeneralData.SetProperty("U_ST_CUSTOMER_GROUP", BP.BP_Group.ToString());
                        oGeneralData.SetProperty("U_ST_CHANNEL", form.DataSources.DBDataSources.Item(0).GetValue("U_ST_CHANNEL", 0));
                        oGeneralData.SetProperty("U_ST_SUB_CHANNEL", form.DataSources.DBDataSources.Item(0).GetValue("U_ST_SUB_CHANNEL", 0));
                        oGeneralData.SetProperty("U_ST_BROKER1", form.DataSources.DBDataSources.Item(0).GetValue("U_ST_BROKER", 0));
                        oGeneralData.SetProperty("U_ST_ACCOUNT_MANAGER", form.DataSources.DBDataSources.Item(0).GetValue("U_ST_ACCOUNT_MANAGER", 0));
                        oGeneralData.SetProperty("U_ST_CURRENCY", form.DataSources.DBDataSources.Item(0).GetValue("U_ST_CURRENCY", 0));
                        oGeneralData.SetProperty("U_ST_CREATOR", company.UserName);
                        oGeneralData.SetProperty("U_ST_MOBILE", BP.Mobile);
                        oGeneralData.SetProperty("U_ST_TEL1", tel1);
                        oGeneralData.SetProperty("U_ST_EMAIL", BP.Email);
                        oGeneralData.SetProperty("U_ST_CREATION_DATE", DateTime.Today);
                        oGeneralData.SetProperty("U_ST_PREFIX", title);
                        oGeneralData.SetProperty("U_ST_TITLE", prefix);

                        for (int j = 0; j < DT_Members.Columns.Count; j++)
                        {
                            string Col_Name = DT_Members.Columns.Item(j).Name;
                            if (Col_Name == "SELECTED" || Col_Name == "Code")
                            {
                                continue;
                            }
                            if (DT_Members.GetValue(Col_Name, i) != null)
                            {
                                oGeneralData.SetProperty($"U_{Col_Name}", DT_Members.GetValue(Col_Name, i));
                            }

                        }

                        Field_Definition[] Addr_Fields = Logic.Fields_Logic.All_Field_Definition.Where(A => A.KHCF_Object == KHCF_Objects.CCI_Member_Card_Address).ToArray();
                        SAPbobsCOM.GeneralDataCollection oChildren = oGeneralData.Child("ST_CCI_INDIV_ADDR");
                        for (int J = 0; J < form.DataSources.DBDataSources.Item("@ST_CCI_CORP_ADDR").Size; J++)
                        {
                            SAPbobsCOM.GeneralData oChild = oChildren.Add();
                            foreach (Field_Definition OneField in Addr_Fields)
                            {
                                if (OneField.Field_Name == "ST_SELECTED")
                                {
                                    continue;
                                }
                                oChild.SetProperty(OneField.Column_Name_In_DB, form.DataSources.DBDataSources.Item("@ST_CCI_CORP_ADDR").GetValue(OneField.Column_Name_In_DB, J));
                            }
                        }
                        oGeneralService.Add(oGeneralData);
                        Codes.Add(New_UDO_Code);
                    }
                }
                company.EndTransaction(BoWfTransOpt.wf_Commit);
                for (int i = 0; i < Codes.Count; i++)
                {
                    UDO_Definition UDO_Info_Approve = Logic.Objects_Logic.All_UDO_Definition.FirstOrDefault(U => U.KHCF_Object == KHCF_Objects.CCI_Member_Card);
                    KHCF_Approval.Approve_MemberCard(company, Codes[i], "", UDO_Info_Approve);
                }
                return true;
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
                Loader.New_Msg = $"Error during adding new members[{ ex.Message }]";
                return false;
            }
        }

        internal static void SBO_Application_MenuEvent(ref MenuEvent pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;

            try
            {
                if (pVal.BeforeAction)
                    return;

                if (Form_Obj == null || SBO_Application.Forms.ActiveForm.TypeEx != Form_Obj.Form_Type)
                {
                    return;
                }
                if (pVal.MenuUID == "1282" || pVal.MenuUID == "1281")//Add || Find
                {
                    Form form = SBO_Application.Forms.ActiveForm;
                    DataTable DT_Members = form.DataSources.DataTables.Item("MEMBERS");
                    DT_Members.Rows.Clear();

                }
                if (SBO_Application.Forms.ActiveForm.Mode != BoFormMode.fm_FIND_MODE)
                {
                    Code_value = SBO_Application.Forms.ActiveForm.TypeEx.Replace("ST", "Frm");
                    Form_Obj.Set_Fields(SBO_Application.Forms.ActiveForm);
                }
                else if (SBO_Application.Forms.ActiveForm.Mode == BoFormMode.fm_FIND_MODE && Form_Obj != null)
                {
                    if (SBO_Application.Forms.ActiveForm.TypeEx == Form_Obj.Form_Type)
                        Form_Obj.UnSet_Mondatory_Fields_Color(SBO_Application.Forms.ActiveForm);
                }
            }
            catch (Exception ex)
            {
                SBO_Application.SetStatusBarMessage(ex.Message, BoMessageTime.bmt_Short, true);
                return;
            }
        }

        private static void Before_Adding_UDO(BusinessObjectInfo businessObjectInfo)
        {
            Form form = SBO_Application.Forms.Item(businessObjectInfo.FormUID);
            DataTable DT_Members = form.DataSources.DataTables.Item("MEMBERS");
            int X = DT_Members.Rows.Count;
            //throw new Exception("fasfaf");
            KHCF_BP BP = new KHCF_BP();
            BP.BP_Group = int.Parse(form.DataSources.DBDataSources.Item(0).GetValue("U_ST_CUSTOMER_GROUP", 0));
            BP.CardName = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_CORPORATE_ARABIC_NAME", 0);
            //string BP_Code = Utility.Create_BP(company, BP);

            Set_Default_Value_Before_Adding(form);


        }

        private static void Set_Default_Value_Before_Adding(Form form)
        {
            //form.DataSources.DBDataSources.Item(0).SetValue("U_ST_BP_CODE", 0, BP_Code);
            if (form.Mode == BoFormMode.fm_ADD_MODE)
            {
                string New_UDO_Code = Utility.Get_New_UDO_Code(company, Form_Obj.KHCF_Object);
                form.DataSources.DBDataSources.Item(0).SetValue("Code", 0, New_UDO_Code);
                form.DataSources.DBDataSources.Item(0).SetValue("U_ST_CREATOR", 0, company.UserName);
                form.DataSources.DBDataSources.Item(0).SetValue("U_ST_CREATION_DATE", 0, DateTime.Today.ToString("yyyyMMdd"));
                //form.DataSources.DBDataSources.Item(0).SetValue("U_ST_PARENT_TYPE", 0, "C");
            }

            string Att_Folder = Configurations.Get_Attachment_Folder(company);
            for (int i = 0; i < form.DataSources.DBDataSources.Item("@ST_CCI_CORP_ATT").Size; i++)
            {
                if (form.DataSources.DBDataSources.Item("@ST_CCI_CORP_ATT").GetValue("LineId", i) == "-1")
                {
                    string User_File_Path = form.DataSources.DBDataSources.Item("@ST_CCI_CORP_ATT").GetValue("U_ST_FILE_NAME", i);
                    string FileName = System.IO.Path.GetFileName(User_File_Path);
                    if (!System.IO.Directory.Exists(Att_Folder))
                    {
                        System.IO.Directory.CreateDirectory(Att_Folder);
                    }
                    string New_Path = System.IO.Path.Combine(Att_Folder, DateTime.Now.ToString("yyyyMMdd_HHmmss_") + FileName);
                    System.IO.File.Copy(User_File_Path, New_Path);
                    form.DataSources.DBDataSources.Item("@ST_CCI_CORP_ATT").SetValue("U_ST_FILE_NAME", i, New_Path);
                    form.DataSources.DBDataSources.Item("@ST_CCI_CORP_ATT").SetValue("LineId", i, "");
                    Loader.SBO_Application.StatusBar.SetText($"The File [{User_File_Path}] has been copied to the Addon attachment folder", BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Warning);
                }
                
            }


        }

        private static void Change_Items_Values_Cutomization(string ItemUID, string FormUID, bool BeforeAction, bool ItemChanged, string ColUID, int Row, bool OnLoad = false)
        {
            #region Names Fields

            if (ItemUID == "616" || ItemUID == "618")
            {
                Form form = SBO_Application.Forms.Item(FormUID);
                string Arabic_Name = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_CORPORATE_ARABIC_NAME", 0);
                string English_Name = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_CORPORATE_ENGLISH_NAME", 0);
                if (!Utility.Check_Text(Arabic_Name) && !string.IsNullOrEmpty(Arabic_Name))
                {
                    throw new Custom_Exception("Only Arabic letters are allowed in Arabic Corporate Name field.");
                }
                if (Utility.Check_Text(English_Name) && !string.IsNullOrEmpty(English_Name))
                {
                    throw new Custom_Exception("Only English letters are allowed in English Corporate Name field.");
                }
            }
            if (ItemUID == "138")
            {
                string[] Names_Cols = new string[] { "ST_FIRST_NAME_AR", "ST_FATHER_NAME_AR", "ST_MIDDLE_NAME_AR", "ST_SURNAME_AR" };
                if (Names_Cols.Contains(ColUID))
                {
                    Form form = SBO_Application.Forms.Item(FormUID);
                    DataTable DT_Members = form.DataSources.DataTables.Item("MEMBERS");
                    Grid Grd_Members = (Grid)form.Items.Item("138").Specific;
                    int Index = Grd_Members.GetDataTableRowIndex(Row);
                    int RowIndex = Grd_Members.GetDataTableRowIndex(Row);

                    string Full_Name;
                    string FirstName = DT_Members.GetValue("ST_FIRST_NAME_AR", Index).ToString();
                    string FatherName = DT_Members.GetValue("ST_FATHER_NAME_AR", Index).ToString();
                    string MiddleName = DT_Members.GetValue("ST_MIDDLE_NAME_AR", Index).ToString();
                    string SurName = DT_Members.GetValue("ST_SURNAME_AR", Index).ToString();
                    if (!Utility.Check_Text(FirstName) && !string.IsNullOrEmpty(FirstName))
                    {
                        DT_Members.SetValue(ColUID, RowIndex, string.Empty);
                        Full_Name = string.Empty;
                    }
                    else if (!Utility.Check_Text(FatherName) && !string.IsNullOrEmpty(FatherName))
                    {
                        DT_Members.SetValue(ColUID, RowIndex, string.Empty);
                        Full_Name = string.Empty;
                    }
                    else if (!Utility.Check_Text(MiddleName) && !string.IsNullOrEmpty(MiddleName))
                    {
                        DT_Members.SetValue(ColUID, RowIndex, string.Empty);
                        Full_Name = string.Empty;
                    }
                    else if (!Utility.Check_Text(SurName) && !string.IsNullOrEmpty(SurName))
                    {
                        DT_Members.SetValue(ColUID, RowIndex, string.Empty);
                        Full_Name = string.Empty;
                    }
                    else
                        Full_Name = Utility.Get_Full_Name(FirstName, FatherName, MiddleName, SurName);

                    DT_Members.SetValue("ST_FULL_NAME_AR", RowIndex, Full_Name);
                }
                string[] Names_EN_Cols = new string[] { "ST_FIRST_NAME_EN", "ST_FATHER_NAME_EN", "ST_MIDDLE_NAME_EN", "ST_SURNAME_EN" };
                if (Names_EN_Cols.Contains(ColUID))
                {
                    Form form = SBO_Application.Forms.Item(FormUID);
                    DataTable DT_Members = form.DataSources.DataTables.Item("MEMBERS");
                    Grid Grd_Members = (Grid)form.Items.Item("138").Specific;
                    int Index = Grd_Members.GetDataTableRowIndex(Row);
                    int RowIndex = Grd_Members.GetDataTableRowIndex(Row);

                    string Full_Name;
                    string FirstName = DT_Members.GetValue("ST_FIRST_NAME_EN", Index).ToString();
                    string FatherName = DT_Members.GetValue("ST_FATHER_NAME_EN", Index).ToString();
                    string MiddleName = DT_Members.GetValue("ST_MIDDLE_NAME_EN", Index).ToString();
                    string SurName = DT_Members.GetValue("ST_SURNAME_EN", Index).ToString();
                    if (Utility.Check_Text(FirstName) && !string.IsNullOrEmpty(FirstName))
                    {
                        DT_Members.SetValue(ColUID, RowIndex, string.Empty);
                        Full_Name = string.Empty;
                    }
                    else if (Utility.Check_Text(FatherName) && !string.IsNullOrEmpty(FatherName))
                    {
                        DT_Members.SetValue(ColUID, RowIndex, string.Empty);
                        Full_Name = string.Empty;
                    }
                    else if (Utility.Check_Text(MiddleName) && !string.IsNullOrEmpty(MiddleName))
                    {
                        DT_Members.SetValue(ColUID, RowIndex, string.Empty);
                        Full_Name = string.Empty;
                    }
                    else if (Utility.Check_Text(SurName) && !string.IsNullOrEmpty(SurName))
                    {
                        DT_Members.SetValue(ColUID, RowIndex, string.Empty);
                        Full_Name = string.Empty;
                    }
                    else
                        Full_Name = Utility.Get_Full_Name(FirstName, FatherName, MiddleName, SurName);

                    DT_Members.SetValue("ST_FULL_NAME_EN", RowIndex, Full_Name);
                }
            }

            #endregion

            if (ItemUID == "612")
            {
                SAPbouiCOM.Form form = SBO_Application.Forms.Item(FormUID);
                string Customer_Group = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_CUSTOMER_GROUP", 0);
                DataTable DT_Members = form.DataSources.DataTables.Item("MEMBERS");
                if (DT_Members.Rows.Count == 1 && string.IsNullOrEmpty(DT_Members.GetValue("ST_APPROVAL_STATUS", 0).ToString()))
                    return;
                for (int i = 0; i < DT_Members.Rows.Count; i++)
                {
                    DT_Members.SetValue("ST_CUSTOMER_GROUP", i, Customer_Group);
                }
            }
            if (Form_Obj.Get_Depends_Parent_Item_IDs_List().Contains(ItemUID))
            {
                Form form = SBO_Application.Forms.Item(FormUID);
                Form_Obj.Load_One_Depends_Parent_Item(form, ItemUID);
            }
            if (ItemUID == "888")
            {
                SAPbouiCOM.Form form = SBO_Application.Forms.Item(FormUID);
                string Email = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_EMAIL", 0);
                bool isEmail = Regex.IsMatch(Email, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase);

                if (!isEmail && !string.IsNullOrEmpty(Email))
                {
                    form.Items.Item("888").Click();
                    form.Items.Item("888").Click();
                    throw new Logic.Custom_Exception($"The Email is not a correct format.");
                }
            }

            if (ItemUID == "701")
            {
                SAPbouiCOM.Form form = SBO_Application.Forms.Item(FormUID);
                DBDataSource DS_Address = form.DataSources.DBDataSources.Item("@ST_CCI_CORP_CONT");
                Matrix Mat_Add = (Matrix)form.Items.Item("701").Specific;

                for (int i = 0; i < Mat_Add.RowCount; i++)
                {
                    SAPbouiCOM.EditText Email = (SAPbouiCOM.EditText)Mat_Add.Columns.Item("E-Mail").Cells.Item(i + 1).Specific;
                    bool isEmail = Regex.IsMatch(Email.Value, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase);
                    if (!isEmail && !string.IsNullOrEmpty(Email.Value))
                    {
                        SBO_Application.StatusBar.SetText($"Email is not in correct format in Contacts Tab at row [{i+1}]", BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Error);
                    }
                }
            }
        }

        private static bool Validate_Data(BusinessObjectInfo businessObjectInfo)
        {
            Form form = SBO_Application.Forms.Item(businessObjectInfo.FormUID);

            if (!Form_Obj.Validate_Data(form))
            {
                return false;
            }
            if (form.Mode == BoFormMode.fm_UPDATE_MODE && form.DataSources.DBDataSources.Item(0).GetValue("U_ST_APPROVAL_STATUS", 0) != "P")
            {
                bool approvalResult = Member_Cards_UI.Process_Corporate_Update_Approval(form, Form_Obj);
                if (!approvalResult) return false;
            }

            string Mobile1 = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_MOBILE_1", 0);
            string Mobile2 = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_MOBILE_2", 0);
            string Tel1 = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_TEL1", 0);
            string Tel2 = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_TEL_2", 0);
            string CorpNationalID = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_CORPORATE_NATIONAL_ID", 0);

            string PhoneRegExpression = @"^[\+]?[(]?[0-9]{3}[)]?[-\s\.]?[0-9]{3}[-\s\.]?[0-9]{4,6}$";
            //bool checkMobile1 = Regex.IsMatch(Mobile1, PhoneRegExpression , RegexOptions.IgnoreCase);
            //bool checkMobile2 = Regex.IsMatch(Mobile2, PhoneRegExpression, RegexOptions.IgnoreCase);
            //bool checkTel1 = Regex.IsMatch(Tel1, PhoneRegExpression, RegexOptions.IgnoreCase);
            //bool checkTel2 = Regex.IsMatch(Tel2, PhoneRegExpression, RegexOptions.IgnoreCase);

            if(!Utility.IsDigitsOnly(CorpNationalID) && CorpNationalID.Length != 9)
                throw new Custom_Exception("Corporate National ID must be 9 digts.");

            //if (!checkMobile1 || !checkTel1 || (!string.IsNullOrEmpty(Mobile2) && !checkMobile2) 
            //    || (!string.IsNullOrEmpty(Tel2) && !checkTel2))
            //    throw new Custom_Exception(@"Please check the corporate's phones format.");

            string Arabic_Name = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_CORPORATE_ARABIC_NAME", 0);
            string English_Name = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_CORPORATE_ENGLISH_NAME", 0);

            if (!string.IsNullOrEmpty(Arabic_Name) && !Utility.Check_Text(Arabic_Name))
                throw new Custom_Exception("Only Arabic letters are allowed in Arabic Corporate Name");

            if (!string.IsNullOrEmpty(English_Name) && Utility.Check_Text(English_Name))
                throw new Custom_Exception("Only English letters are allowed in English Corporate Name");

            string Email = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_EMAIL", 0);
            bool isEmail = Regex.IsMatch(Email, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase);
            if (!isEmail && !string.IsNullOrEmpty(Email))
                throw new Logic.Custom_Exception($"The Email is not in Correct Format");

            DBDataSource DS_Contacts = form.DataSources.DBDataSources.Item("@ST_CCI_CORP_CONT");
            Matrix Mat_Contacts = (Matrix)form.Items.Item("701").Specific;
            for (int i = 0; i < Mat_Contacts.RowCount; i++)
            {
                SAPbouiCOM.EditText ContactEmailEditText = (SAPbouiCOM.EditText)Mat_Contacts.Columns.Item("E-Mail").Cells.Item(i + 1).Specific;
                isEmail = Regex.IsMatch(ContactEmailEditText.Value, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase);

                if (!isEmail && !string.IsNullOrEmpty(ContactEmailEditText.Value))
                {
                    throw new Logic.Custom_Exception($"The Email is not in Correct Format in Contact Tab at row [{i + 1}]");
                }
            }

            Member_Cards_UI.Check_Members(businessObjectInfo, "C");
            Member_Cards_UI.Check_Address(businessObjectInfo, "@ST_CCI_CORP_ADDR");

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
                    DateTime startTime = DateTime.Now;
                    Form form = SBO_Application.Forms.Item(pVal.FormUID);
                    Matrix Mat_Add = (Matrix)form.Items.Item("20").Specific;
                    //if (Mat_Add.Columns.Item("Country").ValidValues.Count == 0)
                    //{
                    //       Form_Obj.Fill_Address_ComboBox(Mat_Add);
                    //DateTime prev = Utility.Add_Time_Log("C", "New Fill Address", startTime);
                    //}
                    Member_Cards_UI.Add_Address_Row(pVal, "C");
                    if (form.Mode == BoFormMode.fm_OK_MODE)
                        form.Mode = BoFormMode.fm_UPDATE_MODE;
                }

                if (pVal.ItemUID == "32" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    SAPbouiCOM.Form form = SBO_Application.Forms.Item(pVal.FormUID);
                    Member_Cards_UI.Remove_Address_Row(pVal);
                    if (form.Mode == BoFormMode.fm_OK_MODE)
                        form.Mode = BoFormMode.fm_UPDATE_MODE;
                }
                if (pVal.ItemUID == "139" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Form form = SBO_Application.Forms.Item(pVal.FormUID);
                    Member_Cards_UI.Add_Member_Row(pVal);
                }
                if (pVal.ItemUID == "703" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Add_Contacts_Row(pVal);
                }
                if (pVal.ItemUID == "702" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Remove_Contacts_Row(pVal);
                }
                if (pVal.ItemUID == "140" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Member_Cards_UI.Remove_Member_Row(pVal);
                }
                if (pVal.ItemUID == "178" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Form form = SBO_Application.Forms.Item(pVal.FormUID);
                    Renewal_Membership(form, false, false);
                }
                if (pVal.ItemUID == "502" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Member_Cards_UI.Add_Attachment(pVal,"C");
                }
                if (pVal.ItemUID == "503" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Member_Cards_UI.Remove_Attachment(pVal);
                }
                if (pVal.ItemUID == "504" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Member_Cards_UI.Open_Attachment(pVal);
                }
                if (pVal.ItemUID == "620" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Member_Cards_UI.Add_Communication_Log(pVal,"C");
                }
                if (pVal.ItemUID == "652" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Filter_Members(pVal);
                }
                if (!pVal.BeforeAction && pVal.ItemChanged)
                {
                    Change_Items_Values_Cutomization(pVal.ItemUID, pVal.FormUID, pVal.BeforeAction, pVal.ItemChanged, pVal.ColUID, pVal.Row);
                }
                if (pVal.ItemUID == "163" && (pVal.EventType == BoEventTypes.et_ITEM_PRESSED || pVal.EventType == BoEventTypes.et_COMBO_SELECT) && !pVal.BeforeAction)
                {
                    Run_Actions(pVal);
                } 
              
                if (pVal.ItemUID == "127" && pVal.EventType == BoEventTypes.et_CHOOSE_FROM_LIST && !pVal.BeforeAction)
                {
                    Choose_From_List_Broker(pVal);
                }
                if (pVal.ItemUID == "138" && (pVal.ColUID == "ST_NATIONALITY" || pVal.ColUID == "ST_RESIDENCY") && pVal.EventType == BoEventTypes.et_CHOOSE_FROM_LIST && !pVal.BeforeAction)
                {
                    Choose_From_List_Country_Grid(pVal);
                }
                if (pVal.ItemUID == "20" && pVal.ColUID == "Country" && pVal.EventType == BoEventTypes.et_CHOOSE_FROM_LIST && !pVal.BeforeAction)
                {
                    Choose_From_List_Country_Matrix(pVal);
                }
                if (pVal.ItemUID == "159" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Approve(pVal);
                }
                if (pVal.ItemUID == "160" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Reject(pVal);
                }

            }
            catch (Exception ex)
            {
                SBO_Application.StatusBar.SetText(ex.Message, BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Error);
            }
        }

        private static void Choose_From_List_Broker(ItemEvent pVal)
        {
            SAPbouiCOM.Form form = SBO_Application.Forms.Item(pVal.FormUID);
            ChooseFromListEvent Chos_Event = (ChooseFromListEvent)pVal;
            if ((Chos_Event.SelectedObjects == null ? false : Chos_Event.SelectedObjects.Rows.Count != 0))
            {
                string C = Chos_Event.SelectedObjects.GetValue("CardCode", 0).ToString();
                form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).SetValue("U_ST_BROKER", 0, C);
                if (form.Mode == BoFormMode.fm_OK_MODE)
                {
                    form.Mode = BoFormMode.fm_UPDATE_MODE;
                }
            }
        }

        private static void Choose_From_List_Country_Grid(ItemEvent pVal)
        {
            SAPbouiCOM.Form form = SBO_Application.Forms.Item(pVal.FormUID);
            ChooseFromListEvent Chos_Event = (ChooseFromListEvent)pVal;
            if ((Chos_Event.SelectedObjects == null ? false : Chos_Event.SelectedObjects.Rows.Count != 0))
            {
                Grid Grd = (Grid)form.Items.Item(pVal.ItemUID).Specific;
                string C = Chos_Event.SelectedObjects.GetValue("Name", 0).ToString();
                DataTable DT = Grd.DataTable;
                int Index = Grd.GetDataTableRowIndex(pVal.Row);

                DT.SetValue(pVal.ColUID, Index, C);
                if (form.Mode == BoFormMode.fm_OK_MODE)
                {
                    form.Mode = BoFormMode.fm_UPDATE_MODE;
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

        private static void Renewal_Membership(Form form, bool All_Childs, bool Renewal_Parent)
        {
            //UDO_Definition UDO_Info = Logic.Objects_Logic.All_UDO_Definition.FirstOrDefault(U => U.KHCF_Object == KHCF_Objects.CCI_Member_Card);

            //List<string> Childs_Cards = new List<string>();
            //UDO_Definition UDO_Membership_Info = Logic.Objects_Logic.All_UDO_Definition.FirstOrDefault(U => U.KHCF_Object == KHCF_Objects.Individual_Membership);
            //DataTable DT_Members = form.DataSources.DataTables.Item("MEMBERS");
            //for (int i = 0; i < DT_Members.Rows.Count; i++)
            //{
            //    if (All_Childs  || DT_Members.GetValue("SELECTED", i).ToString() == "Y")
            //    {
            //        Childs_Cards.Add(DT_Members.GetValue("Code", i).ToString());
            //    }
            //}
            //if (Renewal_Parent)
            //{
            //    string Parent_Code = form.DataSources.DBDataSources.Item(0).GetValue("Code", 0);
            //    UDO_Definition UDO_Parent_Membership_Info = Logic.Objects_Logic.All_UDO_Definition.FirstOrDefault(U => U.KHCF_Object == KHCF_Objects.Corporate_Membership);
            //    Membership.Create_Renewal_MemberCard(company, Parent_Code, UDO_Parent_Membership_Info);
            //}
            //foreach (string OneCard in Childs_Cards)
            //{
            //    Membership.Create_Renewal_MemberCard(company, OneCard, UDO_Membership_Info);
            //}

        }

        private static void Filter_Members(ItemEvent pVal)
        {
            Form form = SBO_Application.Forms.Item(pVal.FormUID);
            string Card_ID = form.DataSources.DBDataSources.Item(0).GetValue("Code", 0);
            string searchPhrase = form.DataSources.DBDataSources.Item(0).GetValue("651", 0);

            Member_Cards_UI.Load_Sub_Members(form, "C", Card_ID,searchPhrase);
        }

        private static void Run_Actions(ItemEvent pVal)
        {
            Form form = SBO_Application.Forms.Item(pVal.FormUID);
            if (form.Mode != BoFormMode.fm_OK_MODE)
            {
                throw new Logic.Custom_Exception("This action can only run in OK mode.");
            }

            string Approval_Status = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_APPROVAL_STATUS", 0);
            //if (Approval_Status=="P")
            //{
            //    throw new Logic.Custom_Exception("We can run the action if the Card is Approved only");
            //}
            ButtonCombo Btn_Cmb_Warrning = (ButtonCombo)form.Items.Item("163").Specific;
            string Title = "";
            string Action_ID = form.DataSources.UserDataSources.Item("163").Value;
            if (Action_ID == "-")
                throw new Logic.Custom_Exception("Please select the action");
            else 
            {
                Title = Utility.Get_Field_Configuration(company, form.TypeEx.Replace("ST","Frm") + "_" + Action_ID, "", "");
            }
            
            if (Title == "" || string.IsNullOrEmpty(Title))
                throw new Logic.Custom_Exception($"The Action [{Action_ID}] is not supported");
            
            //switch (Action_ID)
            //{
            //    case "-"://Can Also
            //        throw new Exception("Please select the action");
            //    case "R"://Remove
            //        Title = "Remove";
            //        break;
            //    //case "L"://Link
            //    //    Title = "Link";
            //    //    break;
            //    //case "U"://Unlink
            //    //    Title = "Unlink";
            //    //    break;
            //    case "RE"://Renewal All
            //        Title = "Renewal All Cards";
            //        break;
            //    case "RS"://Renewal and selected childes
            //        Title = "Renewal Card and selected childes";
            //        break;
            //    case "SA"://Stop All
            //        Title = "Stop All Cards";
            //        break;
            //    case "SS"://Renewal and selected childes
            //        Title = "Stop Card and selected childes";
            //        break;
            //    case "M":
            //        Title = "Create Membership";
            //        break;
            //    default:
            //        throw new Exception($"This Report [{Action_ID}] is not supported");
            //}
            if (SBO_Application.MessageBox($"Are you sure you want to {Title} the Card?", 1, "Yes", "No") != 1)
            {
                return;
            }
            string UDO_Code = form.DataSources.DBDataSources.Item(0).GetValue("Code", 0);
            UDO_Definition UDO_Info = Form_Obj.UDO_Info;// Logic.Objects_Logic.All_UDO_Definition.FirstOrDefault(U => U.KHCF_Object == KHCF_Objects.CCI_Member_Card);

            switch (Action_ID)
            {
                case "R"://Remove
                    KHCF_Logic_Utility.Remove_UDO_Entry(company, UDO_Code, UDO_Info);
                    // SBO_Application.Menus.Item("1304").Activate();
                    form.Mode = BoFormMode.fm_FIND_MODE;
                    Form_Obj.UnSet_Mondatory_Fields_Color(form);
                    break;
                case "RE"://Renewal All
                    Renewal_Membership(form, true, true);
                    break;
                case "RS"://Renewal and selected childes
                    Renewal_Membership(form, false, true);
                    break;
                case "SA"://Stop All
                    Stop_Membership(form, UDO_Info, true, true);
                    break;
                case "SS"://Stop and selected childes
                    Stop_Membership(form, UDO_Info, false, true);
                    break;
                case "M"://Create Membership
                    Create_Membership(form);
                    break;
                default:
                    throw new Logic.Custom_Exception($"This Report [{Action_ID}] is not supported");
            }
        }

        private static void Stop_Membership(Form form, UDO_Definition UDO_Info, bool All_Childs, bool Renewal_Parent)
        {
            List<string> Childs_Cards = new List<string>();
            DataTable DT_Members = form.DataSources.DataTables.Item("MEMBERS");
            for (int i = 0; i < DT_Members.Rows.Count; i++)
            {
                if (All_Childs || DT_Members.GetValue("SELECTED", i).ToString() == "Y")
                {
                    Childs_Cards.Add(DT_Members.GetValue("Code", i).ToString());
                }
            }
            string All_Cards = "";
            string Parent_Code = "";
            if (Renewal_Parent)
            {
                 Parent_Code = form.DataSources.DBDataSources.Item(0).GetValue("Code", 0);
                ///All_Cards += $"{Parent_Code},";
                // Membership.Stop_MemberCard(company, Parent_Code, UDO_Membership_Info);
            }
            foreach (string OneCard in Childs_Cards)
            {
                All_Cards += $"{OneCard},";
                // Membership.Stop_MemberCard(company, OneCard, UDO_Membership_Info);
            }

            Frm_Set_Stop_Card_Data.Create_Form(All_Cards, Parent_Code);
        }

        private static void Remove_Contacts_Row(ItemEvent pVal)
        {
            Form form = SBO_Application.Forms.Item(pVal.FormUID);
            Form_Obj.Remove_Matrix_Row(form, "701");
        }
        
        private static void Create_Membership(Form form)
        {
            string UDO_Code = form.DataSources.DBDataSources.Item(0).GetValue("Code", 0);
            if (form.DataSources.DBDataSources.Item(0).GetValue("U_ST_APPROVAL_STATUS", 0) == "p")
                throw new Custom_Exception($"You can`t Create Membership,Card [{UDO_Code}] is pending!");
            Frm_Corporate_Membership.Create_Membership_for_MemberCard(UDO_Code);
        }

        private static void Add_Contacts_Row(ItemEvent pVal)
        {
            Form form = SBO_Application.Forms.Item(pVal.FormUID);
            DBDataSource DS_Contacts = form.DataSources.DBDataSources.Item("@ST_CCI_CORP_CONT");
            Matrix Mat_Cont = (Matrix)form.Items.Item("701").Specific;
            Mat_Cont.AddRow();
            Mat_Cont.ClearRowData(Mat_Cont.RowCount);
            if (form.Mode == BoFormMode.fm_OK_MODE)
            {
                form.Mode = BoFormMode.fm_UPDATE_MODE;
            }
            return;
            Mat_Cont.FlushToDataSource();
            int Count = DS_Contacts.Size;
            if (Count == 1)
            {
                if (DS_Contacts.GetValue("U_ST_CONTACT_ID", Count - 1) != "")
                {
                    DS_Contacts.InsertRecord(Count);
                }
                else
                {
                    Count = 0;
                    Mat_Cont.LoadFromDataSource();
                }
            }
            else
            {
                DS_Contacts.InsertRecord(Count);
            }
            //DS_Contacts.SetValue("U_ST_ADDRESS_TYPE", Count, "S");
            //DS_Contacts.SetValue("U_ST_COUNTRY", Count, "JO");
            Mat_Cont.LoadFromDataSource();
        }

        internal static void Fill_Position_ComboBox(Matrix Mat_Contacts)
        {
            string SQL = $@"SELECT T0.""Code"", T0.""U_ST_ARABIC_NAME"" FROM ""@ST_JOB_TITLE"" T0";
            Recordset RC = (Recordset)company.GetBusinessObject(BoObjectTypes.BoRecordset);
            RC = (Recordset)company.GetBusinessObject(BoObjectTypes.BoRecordset);
            RC.DoQuery(SQL);
            for (int i = 0; i < RC.RecordCount; i++)
            {
                Mat_Contacts.Columns.Item("Position").ValidValues.Add(RC.Fields.Item("Code").Value.ToString(), RC.Fields.Item("U_ST_ARABIC_NAME").Value.ToString());
                RC.MoveNext();
            }
            string[] Visibale_Columns = new string[] {  "E-Mail", "Mobile", "Tel_1", "Position", "ST_NAME" , "SELECTED", "#" };
            for (int i = 0; i < Mat_Contacts.Columns.Count; i++)
            {
                string Name = Mat_Contacts.Columns.Item(i).UniqueID;
                if (Visibale_Columns.Contains(Name))
                {
                    Mat_Contacts.Columns.Item(i).Visible= true;
                }
                else
                {
                    Mat_Contacts.Columns.Item(i).Visible= false;
                }
            }


        }

    }
}
