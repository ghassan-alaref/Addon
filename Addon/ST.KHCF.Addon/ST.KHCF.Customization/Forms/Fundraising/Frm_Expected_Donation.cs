using SAPbobsCOM;
using SAPbouiCOM;
using ST.KHCF.Customization.Logic;
using ST.KHCF.Customization.Logic.Classes;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ST.KHCF.Customization.Forms.Fundraising
{
    internal class Frm_Expected_Donations : Parent_Form
    {
        internal static SAPbobsCOM.Company company;
        internal static Application SBO_Application;
        internal static Parent_Form Form_Obj;
        internal static string[] Relations_Grid_IDs = new string[] { "72" };

        //internal override string[] Get_Mondatory_Fields_List()
        //{
        //    List<string> Result = new List<string>();
        //    Result.AddRange(base.Get_Mondatory_Fields_List());
        //    Result.AddRange(new string[] { "20","22"});

        //    return Result.ToArray();
        //}

        //internal override string[] Get_Fix_ReadOnly_Fields_List()
        //{
        //    List<string> Result = new List<string>();
        //    Result.AddRange(base.Get_Fix_ReadOnly_Fields_List());
        //    Result.AddRange(new string[] { "5", "9", "11", "152", "56", "42", "134" }); "5,9,11,152,56,42,134"

        //    return Result.ToArray();
        //}

        internal override bool Get_Is_Approval_Status(Form form)
        {
            return form.DataSources.DBDataSources.Item(0).GetValue("U_ST_FINANCE_CONFIRMATION", 0) == "Y";
        }

        //internal override string[] Get_Approval_Items_List()
        //{
        //    List<string> Result = new List<string>();
        //    Result.AddRange(base.Get_Approval_Items_List());
        //    Result.AddRange(new string[] { "70" });

        //    return Result.ToArray();
        //}

        internal override void Initialize_Form(Form form)
        {       
            base.Initialize_Form(form);
            Initializ_Expected_Form(form);
        }

        internal static void Initializ_Expected_Form(Form form)
        {
            string fundDepartments = Utility.Get_Configuration(company, "Fundraising_Department");
            string SQL_Account_Manager = $@"SELECT T0.""SlpCode"" AS ""Code"", T0.""SlpName"" AS ""Name"" FROM OSLP T0  INNER JOIN OHEM T1 ON T0.""SlpCode"" = T1.""salesPrson"" WHERE T1.""dept"" in ({fundDepartments})";
            Helper.Utility.Fill_One_ComboBoxBySQL(company, form, "36", SQL_Account_Manager, true);
            Helper.Utility.Fill_One_ComboBoxBySQL(company, form, "134", SQL_Account_Manager, true);

            string SQL_Cost_Center = $@"SELECT T0.""PrcCode"" AS ""Code"", T0.""PrcName"" AS ""Name"" FROM OPRC T0 ";
            Helper.Utility.Fill_One_ComboBoxBySQL(company, form, "32", SQL_Cost_Center, true);
         
            //string SQL_PaymentTerms = $@"SELECT T0.""GroupNum"" AS ""Code"",T0.""PymntGroup"" AS ""Name"" FROM OCTG T0";
            //Helper.Utility.Fill_One_ComboBoxBySQL(company, form, "95", SQL_PaymentTerms, true);

            SAPbouiCOM.ChooseFromList CFL_Orphans = form.ChooseFromLists.Item("CFL_Orphans");
            Conditions Orphans_Cons = CFL_Orphans.GetConditions();
            Condition Orphans_Con = Orphans_Cons.Add();
            Orphans_Con.Alias = "CardType";
            Orphans_Con.CondVal = "L";
            Orphans_Con.Operation = BoConditionOperation.co_EQUAL;
            Orphans_Con.Relationship = BoConditionRelationship.cr_AND;
            Orphans_Con = Orphans_Cons.Add();
            Orphans_Con.Alias = "U_ST_CUSTOMER_TYPE";
            Orphans_Con.CondVal = "C";
            Orphans_Con.Operation = BoConditionOperation.co_EQUAL;

            CFL_Orphans.SetConditions(Orphans_Cons);

            SAPbouiCOM.ChooseFromList CFL_Ambassador = form.ChooseFromLists.Item("CFL_Ambassador");
            Conditions Ambassador_Cons = CFL_Ambassador.GetConditions();
            Condition Ambassador_Con = Ambassador_Cons.Add();
            Ambassador_Con.Alias = "U_ST_AMBASSADOR_ADD_UPDATE";
            Ambassador_Con.CondVal = "Y";
            Ambassador_Con.Operation = BoConditionOperation.co_EQUAL;

            CFL_Ambassador.SetConditions(Ambassador_Cons);

            SAPbouiCOM.ChooseFromList CFL_Corp = form.ChooseFromLists.Item("CFL_AMB_CORP");
            Conditions Corp_Cons = CFL_Corp.GetConditions();
            Condition Corp_Con = Corp_Cons.Add();
            Corp_Con.Alias = "U_ST_IS_AMBASSADOR";
            Corp_Con.CondVal = "Y";
            Corp_Con.Operation = BoConditionOperation.co_EQUAL;

            CFL_Corp.SetConditions(Corp_Cons);

            SAPbouiCOM.ChooseFromList CFL_Donor = form.ChooseFromLists.Item("CFL_DONOR");
            Conditions Donor_Cons = CFL_Donor.GetConditions();
            Condition Donor_Con = Donor_Cons.Add();
            Donor_Con.Alias = "CardCode";
            Donor_Con.CondVal = "DON";
            Donor_Con.Operation = BoConditionOperation.co_START;
            CFL_Donor.SetConditions(Donor_Cons);

            DataTable DT_Departments = form.DataSources.DataTables.Item("DT_Departments");
            Dictionary<string, string> Departments = new Dictionary<string, string>();
            Departments.Add("LID", "LID");
            Departments.Add("LCD", "LCD");
            Departments.Add("IDD", "IDD");
            Helper.Utility.FillGridForDictionary(DT_Departments, Departments, true);
            ((Grid)form.Items.Item("72").Specific).Columns.Item("Code").Visible = false;

            Grid grid = (Grid)form.Items.Item("Item_26").Specific;
            grid.AutoResizeColumns();

            string canApproveSQL = $@"SELECT T0.""U_ST_CAN_CONFIRM_EXPECTED_DONATION"" FROM OUSR T0 WHERE T0.""USER_CODE"" = '{company.UserName}'";
            Recordset approveRC = Helper.Utility.Execute_Recordset_Query(company, canApproveSQL);
            if (approveRC.Fields.Item("U_ST_CAN_CONFIRM_EXPECTED_DONATION").Value.ToString() == "Y")
            {
                form.Items.Item("42").SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 3, BoModeVisualBehavior.mvb_True);
                ((Matrix)form.Items.Item("203").Specific).Columns.Item("FinCon").Editable = true;// .SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 3, BoModeVisualBehavior.mvb_True);
            }
            else
            {
                form.Items.Item("42").SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 3, BoModeVisualBehavior.mvb_False);
                ((Matrix)form.Items.Item("203").Specific).Columns.Item("FinCon").Editable = false;
                

            }
            string SQL_Booth = $@"SELECT T0.""empID"" AS ""Code"", CONCAT(T0.""firstName"",T0.""lastName"") AS ""Name"" FROM OHEM T0 WHERE T0.""dept"" = '3'";
            Helper.Utility.Fill_One_ComboBoxBySQL(company, form, "97", SQL_Booth, true);
            Matrix Mat_Lines = (Matrix)form.Items.Item("203").Specific;
            Mat_Lines.AutoResizeColumns();
            form.Items.Item("Item_17").Width = 113;

            form.Items.Item("1000").SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 4, BoModeVisualBehavior.mvb_False);
            //form.Items.Item("1000").SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 2, BoModeVisualBehavior.mvb_False);


            form.Items.Item("49").Click();
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
                    Form_Obj.UnSet_Mondatory_Fields_Color(form);
                    form.Items.Item("50").SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 4, BoModeVisualBehavior.mvb_False);
                    form.Items.Item("51").SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 4, BoModeVisualBehavior.mvb_False);
                    form.Items.Item("60").SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 4, BoModeVisualBehavior.mvb_False);
                    form.Items.Item("61").SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 4, BoModeVisualBehavior.mvb_False);
                    form.Items.Item("101").SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 4, BoModeVisualBehavior.mvb_False);
                    form.Items.Item("102").SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 4, BoModeVisualBehavior.mvb_False);
                    form.Items.Item("201").SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 4, BoModeVisualBehavior.mvb_False);
                    form.Items.Item("202").SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 4, BoModeVisualBehavior.mvb_False);
                    Form_Obj.UnSet_Mondatory_Fields_Color(form);
                    DisableTabButtons(form);
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
                    string targetId = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_TARGET",0);
                    string donorCard = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_DONOR_MASTER_RECORD", 0);
                    string donorName = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_DONATION_NAME", 0);
                    string docNumber = form.DataSources.DBDataSources.Item(0).GetValue("Code", 0);


                    string SQL = $@"SELECT T0.""U_ST_AREA_ID"", T0.""U_ST_SIGN_AR"", T0.""U_ST_SIGN_EN"" FROM ""@ST_TARGET_AREAS""  T0 WHERE T0.""Code"" ='{targetId}'";
                    Recordset RC = Helper.Utility.Execute_Recordset_Query(company, SQL);
                    if (RC.RecordCount > 0)
                    {
                        for (int i = 0; i < RC.RecordCount; i++)
                        {
                            string code = RC.Fields.Item("U_ST_AREA_ID").Value.ToString();
                            string updateSQL = $@"UPDATE ""@ST_NAMING"" T0 SET T0.""U_ST_STATUS"" = 'R', T0.""U_ST_SIGN_AR"" = '{RC.Fields.Item("U_ST_SIGN_AR").Value.ToString()}',T0.""U_ST_SIGN_EN"" = '{RC.Fields.Item("U_ST_SIGN_EN").Value.ToString()}',T0.""U_ST_DONOR_CARD"" = '{donorCard}',T0.""U_ST_NAMED_DONOR"" = '{donorName}',T0.""U_ST_EXP_DONATION"" = '{docNumber}' WHERE T0.""Code"" = '{code}'";
                            Recordset updateRC = Helper.Utility.Execute_Recordset_Query(company, updateSQL);
                            RC.MoveNext();
                        }
                    }
                    string machineSQL = $@"SELECT T0.""U_ST_MACHINE_ID"", T0.""U_ST_SIGN_AR"", T0.""U_ST_SIGN_EN"" FROM ""@ST_TARGET_MACHINES""  T0 WHERE T0.""Code"" ='{targetId}'";
                    Recordset machineRC = Helper.Utility.Execute_Recordset_Query(company, machineSQL);
                    if (machineRC.RecordCount > 0)
                    {
                        for (int i = 0; i < machineRC.RecordCount; i++)
                        {
                            string code = machineRC.Fields.Item("U_ST_MACHINE_ID").Value.ToString();
                            string updateSQL = $@"UPDATE ""@ST_MACHIN_DET"" T0 SET T0.""U_ST_MACHINE_STATUS"" = 'R' WHERE T0.""U_ST_MACHINE_ID"" = '{code}'";
                            Recordset updateRC = Helper.Utility.Execute_Recordset_Query(company, updateSQL);
                            machineRC.MoveNext();
                        }
                    }


                }

                if (BusinessObjectInfo.EventType == BoEventTypes.et_FORM_DATA_LOAD && !BusinessObjectInfo.BeforeAction)
                {
                    Form_Data_Load(BusinessObjectInfo);
                }

                string TableName = form.DataSources.DBDataSources.Item(0).TableName.Replace("@", "");
                UDO_Definition KHCF_Object = Objects_Logic.All_UDO_Definition.FirstOrDefault(O => O.Table_Name == TableName);
                SBO_Application.StatusBar.SetText("Loading", BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_None);
                if (Form_Obj.Set_ReadOnly(form, KHCF_Object))
                {
                    // return;
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

            if (!Form_Obj.Get_Is_Approval_Status(form))
            {
                Form_Obj.Send_Alert_For_Approve(UDO_Code);
            }

            Utility.Update_Relation_Table(company, form, UDO_Code, Relations_Grid_IDs, Form_Obj.KHCF_Object);

        }

        internal static void Form_Data_Load(BusinessObjectInfo businessObjectInfo)
        {
            Form form = SBO_Application.Forms.Item(businessObjectInfo.FormUID);

            //UDO_Definition obj = Objects_Logic.All_UDO_Definition.FirstOrDefault(o => o.Table_Name == "ST_EXPEC_DONATION");
            
            string UDO_Code = form.DataSources.DBDataSources.Item(0).GetValue("Code", 0);
            Utility.Load_All_Relation_Data(company, form, UDO_Code, Relations_Grid_IDs, Form_Obj.KHCF_Object);
            //Utility.Load_All_Relation_Data(company, form, UDO_Code, Relations_Grid_IDs, obj.KHCF_Object);

            if (string.IsNullOrEmpty(form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).GetValue("U_ST_TARGET", 0)))
            {
                form.Items.Item("1000").SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 1, BoModeVisualBehavior.mvb_True);
            }
            else
            {
                form.Items.Item("1000").SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 1, BoModeVisualBehavior.mvb_False);
            }


            Load_Actual_Donations(form);
            Form_Obj.Set_Fields(form);

        }

        //        internal static void Load_Activities(Form form, string Card_ID, Parent_Form Form_Obj_Data)
        //        {
        //            DataTable DT_Activities = form.DataSources.DataTables.Item("DT_ACTIVITIES");
        //            DT_Activities.Rows.Clear();
        //            string SQL_Activities = $@"SELECT T0.""ClgCode"", T0.""Recontact"", T0.""Details"", T0.""Duration"", T0.""Closed"", T0.""Notes"" 
        //FROM OCLG T0 WHERE T0.U_ST_KHCF_OBJECT_CODE = '{Card_ID}' AND  T0.U_ST_KHCF_OBJECT_TYPE = '{((int)Form_Obj_Data.KHCF_Object).ToString()}'";
        //            Recordset RC_Activitiesp = Helper.Utility.Execute_Recordset_Query(company, SQL_Activities);
        //            DT_Activities.Rows.Add(RC_Activitiesp.RecordCount);

        //            for (int i = 0; i < RC_Activitiesp.RecordCount; i++)
        //            {
        //                for (int J = 0; J < DT_Activities.Columns.Count; J++)
        //                {
        //                    string Col_Name = DT_Activities.Columns.Item(J).Name;
        //                    string UDF_Name;

        //                    UDF_Name = Col_Name;

        //                    DT_Activities.SetValue(Col_Name, i, RC_Activitiesp.Fields.Item(UDF_Name).Value);
        //                }
        //                RC_Activitiesp.MoveNext();
        //            }
        //            Grid Grd_Activities = (Grid)form.Items.Item("56").Specific;

        //            Grd_Activities.AutoResizeColumns();
        //        }

        //internal static void Load_Orphans(Form form, string Line_DataSource_Table)
        //{
        //    DataTable DT_Orphans_Details = form.DataSources.DataTables.Item("Orphans_Details");
        //    DT_Orphans_Details.Rows.Clear();
        //    int Rows_Count = form.DataSources.DBDataSources.Item(Line_DataSource_Table).Size;
        //    DT_Orphans_Details.Rows.Add(Rows_Count);
        //    for (int i = 0; i < Rows_Count; i++)
        //    {
        //        Set_Orphan_Data(form, DT_Orphans_Details, i, Line_DataSource_Table);
        //    }

        //    ((Matrix)form.Items.Item("52").Specific).LoadFromDataSource();
        //    ((Matrix)form.Items.Item("52").Specific).AutoResizeColumns();
        //}

        //internal static void Load_Patients(Form form, string Line_DataSource_Table)
        //{
        //    DataTable DT_Patients_Details = form.DataSources.DataTables.Item("Patients_Details");
        //    DT_Patients_Details.Rows.Clear();
        //    int Rows_Count = form.DataSources.DBDataSources.Item(Line_DataSource_Table).Size;
        //    DT_Patients_Details.Rows.Add(Rows_Count);
        //    for (int i = 0; i < Rows_Count; i++)
        //    {
        //        Set_Patient_Data(form, DT_Patients_Details, i, Line_DataSource_Table);
        //    }

        //    ((Matrix)form.Items.Item("62").Specific).LoadFromDataSource();
        //    ((Matrix)form.Items.Item("62").Specific).AutoResizeColumns();
        //}
        private static void Load_Actual_Donations(Form form)
        {
            string Code = form.DataSources.DBDataSources.Item(0).GetValue("Code", 0);

            if (!string.IsNullOrEmpty(Code))
            {
                DataTable DT_Actual = form.DataSources.DataTables.Item("DT_Actual");
                DT_Actual.Rows.Clear();
                string Actual_SQL = $@"SELECT T0.""Code"",T0.""U_ST_DONATION_DATE"",T0.""U_ST_DONATION_AMOUNT"" FROM ""@ST_ACTUAL_DONATIONS"" T0 WHERE T0.""U_ST_EXP_ID"" = '{Code}'";
                Recordset RC_Actual = Helper.Utility.Execute_Recordset_Query(company, Actual_SQL);
                int test = RC_Actual.RecordCount;
                for (int i = 0; i < RC_Actual.RecordCount; i++)
                {
                    DT_Actual.Rows.Add(1);
                    DT_Actual.SetValue("Code",i, RC_Actual.Fields.Item("Code").Value);
                    DT_Actual.SetValue("U_ST_DATE", i, RC_Actual.Fields.Item("U_ST_DONATION_DATE").Value);
                    DT_Actual.SetValue("U_ST_AMOUNT", i, RC_Actual.Fields.Item("U_ST_DONATION_AMOUNT").Value);
                    RC_Actual.MoveNext();
                }
            }
        }

        internal static void Set_Orphan_Data(Form form, DataTable DT_Orphans_Details, int DT_Row_Index, string Line_DataSource_Table)
        {
            string Code = form.DataSources.DBDataSources.Item(Line_DataSource_Table).GetValue("U_ST_ORPHANS_CODE", DT_Row_Index);
            string SQL = $@"SELECT T0.""CardName"", T0.U_ST_GENDER, T0.U_ST_DATE_OF_BIRTH, T0.U_ST_NATIONAL_ID 
FROM OCRD T0 WHERE T0.""CardCode"" = '{Code}'

UNION ALL

SELECT T0.U_ST_FULL_NAME_AR, T0.U_ST_GENDER, T0.U_ST_DATE_OF_BIRTH, T0.U_ST_NATIONAL_ID
FROM ""@ST_CCI_INDIV_CARD""  T0 WHERE T0.""Code"" = '{Code}'";
            Recordset RC = Helper.Utility.Execute_Recordset_Query(company, SQL);
            for (int J = 0; J < DT_Orphans_Details.Columns.Count; J++)
            {
                string Col_Name = DT_Orphans_Details.Columns.Item(J).Name;
                if (Col_Name == "SELECTED")
                {
                    continue;
                }
                if (Col_Name == "U_ST_ORPHANS_CODE")
                {
                    DT_Orphans_Details.SetValue(Col_Name, DT_Row_Index, Code);
                    continue;
                }
                DT_Orphans_Details.SetValue(Col_Name, DT_Row_Index, RC.Fields.Item(Col_Name).Value);
            }
        }

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
                form.DataSources.DBDataSources.Item(0).SetValue("U_ST_CREATOR", 0, company.UserName);
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
            string errorMsg = string.Empty;
            if (!Validate_Amount(out errorMsg))
            {
                throw new Custom_Exception(errorMsg);
                return false;
            }

            double Sum_Dep_Amount = 0;
            DataTable DT_Departments = form.DataSources.DataTables.Item("DT_Departments");
            for (int i = 0; i < DT_Departments.Rows.Count; i++)
            {
                if (DT_Departments.GetValue("SELECTED", i).ToString() == "Y")
                {
                    Sum_Dep_Amount += (double)DT_Departments.GetValue("U_ST_AMOUNT", i);
                }
            }

            if (double.Parse(form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).GetValue("U_ST_DONATION_AMOUNT", 0)) != Sum_Dep_Amount)
            {
                throw new Logic.Custom_Exception("The Donation Amount is not equal than sum Departments Amounts");
            }

            return true;
        }

        private static bool Validate_Amount(out string errorMsg)
        {
            errorMsg = string.Empty;
            Form form = SBO_Application.Forms.ActiveForm;
            if (!string.IsNullOrEmpty(form.DataSources.DBDataSources.Item(0).GetValue("U_ST_DONATION_AMOUNT", 0)) && !string.IsNullOrEmpty(form.DataSources.DBDataSources.Item(0).GetValue("U_ST_TARGET", 0)))
            {
                double donationAmount = Convert.ToDouble(form.DataSources.DBDataSources.Item(0).GetValue("U_ST_DONATION_AMOUNT", 0));
                string allocationId = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_TARGET", 0);
                string SQL = $@"SELECT T0.""U_ST_TOTAL"" FROM ""@ST_FUND_TARGET""T0 WHERE T0.""Code"" ='{allocationId}'";
                Recordset RC = Helper.Utility.Execute_Recordset_Query(company, SQL);
                if (!string.IsNullOrEmpty(RC.Fields.Item("U_ST_TOTAL").Value.ToString()))
                {
                    double allocatedAmount = Convert.ToDouble(RC.Fields.Item("U_ST_TOTAL").Value);
                    if (allocatedAmount != donationAmount)
                    {
                        errorMsg = "Donation Amount does not equal Allocated Amount";
                        return false;
                    }
                }
            }
            Matrix Mat = (Matrix)form.Items.Item("203").Specific;
            string checkValue = string.Empty;
            if (Mat.RowCount == 1)
            {
                EditText amountET = (EditText)Mat.Columns.Item("Amount").Cells.Item(1).Specific;
                checkValue = amountET.Value;
            }

            if (Mat.RowCount == 0 || checkValue.ToString() == "0.0")
            {
                return true;
            }
            if (!string.IsNullOrEmpty(form.DataSources.DBDataSources.Item(0).GetValue("U_ST_DONATION_AMOUNT", 0)))
            {
                double matrixAmount = 0;
                double donationAmount = Convert.ToDouble(form.DataSources.DBDataSources.Item(0).GetValue("U_ST_DONATION_AMOUNT", 0));
                for (int i = Mat.RowCount; i > 0; i--)
                {

                    EditText amountET = (EditText)Mat.Columns.Item("Amount").Cells.Item(i).Specific;
                    if (!string.IsNullOrEmpty(amountET.Value))
                    {
                        matrixAmount += Convert.ToDouble(amountET.Value);
                    }
                    
                }
                if (matrixAmount == donationAmount)
                {
                    return true;
                }
                else
                {
                    errorMsg = "Payment Schedule Amount does not Equal Donation Amount";
                    return false;
                }
            }
            else
            {
                errorMsg = "Donation Amount is not Filled";
                return false;
            }

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
                //Form_Obj.Set_ReadOnly(form, Form_Obj.UDO_Info);
                //form.DataSources.UserDataSources.Item("27").Value = "0";
                //form.DataSources.UserDataSources.Item("172").Value = "0";
                //string UDO_Code = form.DataSources.DBDataSources.Item(0).GetValue("Code", 0);

                //Utility.Fill_Relation_Grids(company, form, UDO_Code, Relations_Grid_IDs);
                DataTable DT_Departments = form.DataSources.DataTables.Item("DT_Departments");
                Dictionary<string, string> Departments = new Dictionary<string, string>();
                Departments.Add("LID", "LID");
                Departments.Add("LCD", "LCD");
                Departments.Add("IDD", "IDD");
                Helper.Utility.FillGridForDictionary(DT_Departments, Departments);
            }
            if (SBO_Application.Forms.ActiveForm.Mode != BoFormMode.fm_FIND_MODE)
            {
                Form_Obj.Set_Fields(SBO_Application.Forms.ActiveForm);
                Form form = SBO_Application.Forms.ActiveForm;
                if (form.DataSources.DBDataSources.Item(0).GetValue("U_ST_DONATION_TYPE", 0) == "C")
                {
                    form.Items.Item("96").BackColor = Color.Pink.R | (Color.Pink.G << 8) | (Color.Pink.B << 16);

                }
            }
            else
            {
                Form_Obj.UnSet_Mondatory_Fields_Color(SBO_Application.Forms.ActiveForm);
                DisableTabButtons(SBO_Application.Forms.ActiveForm);
            }

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
                if (pVal.ItemUID == "52" && pVal.ColUID == "CODE" && pVal.EventType == BoEventTypes.et_CHOOSE_FROM_LIST && !pVal.BeforeAction)
                {
                    Orphans_Choos_From_List(pVal, "@ST_FUND_EXP_DON_ORP");
                }
                if (pVal.ItemUID == "50" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Remove_Orphans_Selected_Lines(pVal);
                }
                if (pVal.ItemUID == "51" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Add_Orphans_Line(pVal, "@ST_FUND_EXP_DON_ORP");
                }
                if (pVal.ItemUID == "57" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Add_Activity(pVal);
                }
                if (pVal.ItemUID == "44" && pVal.EventType == BoEventTypes.et_CHOOSE_FROM_LIST && !pVal.BeforeAction)
                {
                    Select_Donor(pVal);
                }
                if (pVal.ItemUID == "502" && pVal.EventType == BoEventTypes.et_CHOOSE_FROM_LIST && !pVal.BeforeAction)
                {
                    Choose_Target_ID(pVal);
                }
                if (pVal.ItemUID == "95" && (pVal.EventType == BoEventTypes.et_CHOOSE_FROM_LIST || pVal.EventType == BoEventTypes.et_ITEM_PRESSED)  && pVal.BeforeAction)
                {
                    Form form = SBO_Application.Forms.Item(pVal.FormUID);
                    ComboBox typeCmb = (ComboBox)form.Items.Item("54").Specific;
                    EditText donorCodeET = (EditText)form.Items.Item("7").Specific;
                    string cardType = typeCmb.Selected.Value;
                    string card = donorCodeET.Value;
                    if (!string.IsNullOrEmpty(card))
                    {
                        SAPbouiCOM.ChooseFromList CFL_Recurring = form.ChooseFromLists.Item("CFL_Recurring");
                        Conditions recurring_Cons = CFL_Recurring.GetConditions();
                        if (recurring_Cons.Count == 0)
                        {
                            Condition recurring_Con = recurring_Cons.Add();
                            recurring_Con.Alias = "U_ST_CONTACT_CARD";
                            recurring_Con.CondVal = card;
                            recurring_Con.Operation = BoConditionOperation.co_EQUAL;
                            recurring_Con.Relationship = BoConditionRelationship.cr_AND;
                            recurring_Con = recurring_Cons.Add();
                            recurring_Con.Alias = "U_ST_CONTACT_TYPE";
                            recurring_Con.CondVal = cardType;
                            recurring_Con.Operation = BoConditionOperation.co_EQUAL;
                        }
                        else
                        {
                            recurring_Cons.Item(0).CondVal = card;
                            recurring_Cons.Item(1).CondVal = cardType;
                        }

                        CFL_Recurring.SetConditions(recurring_Cons);
                    }
                  
                }
                if (pVal.ItemUID == "95" && pVal.EventType == BoEventTypes.et_CHOOSE_FROM_LIST && !pVal.BeforeAction)
                {
                    Choose_Recurring_Donation(pVal);

                }
                if (pVal.ItemUID == "505" && pVal.EventType == BoEventTypes.et_CHOOSE_FROM_LIST && !pVal.BeforeAction)
                {
                    Choose_Activity_ID(pVal);
                }
                if (pVal.ItemUID == "305" && (pVal.EventType == BoEventTypes.et_CHOOSE_FROM_LIST || pVal.EventType == BoEventTypes.et_ITEM_PRESSED) && pVal.BeforeAction)
                {
                    Form form = SBO_Application.Forms.Item(pVal.FormUID);
                    string ambassador = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_AMBASSADOR", 0);
                    SAPbouiCOM.ChooseFromList CFL_Orphans = form.ChooseFromLists.Item("CFL_ACT");
                    Conditions Orphans_Cons = CFL_Orphans.GetConditions();
                    if (Orphans_Cons.Count > 0)
                    {
                        Orphans_Cons.Item(0).CondVal = ambassador;
                    }
                    else
                    {
                        Condition Orphans_Con = Orphans_Cons.Add();
                        Orphans_Con.Alias = "U_ST_CONTACT_CARD";
                        Orphans_Con.CondVal = ambassador;
                        Orphans_Con.Operation = BoConditionOperation.co_EQUAL;
                    }
                    CFL_Orphans.SetConditions(Orphans_Cons);
                }
                if (pVal.ItemUID == "305" && pVal.EventType == BoEventTypes.et_CHOOSE_FROM_LIST && !pVal.BeforeAction)
                {
                    Choose_Ambassador_ID(pVal);

                }
                if (pVal.ItemUID == "45" && pVal.EventType == BoEventTypes.et_CHOOSE_FROM_LIST && !pVal.BeforeAction)
                {
                    Choose_Behalf_Of(pVal);
                }
                if (pVal.ItemUID == "70" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Approve(pVal);
                }
                if (pVal.ItemUID == "71" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Convert_To_Actual(pVal);
                }
                if (pVal.ItemUID == "75" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Print(pVal);
                }

                if (pVal.ItemUID == "54" && pVal.EventType == BoEventTypes.et_COMBO_SELECT && !pVal.BeforeAction)
                {
                    Selected_Card_Type(pVal);
                }
                if (pVal.ItemUID == "20" && pVal.EventType == BoEventTypes.et_COMBO_SELECT && !pVal.BeforeAction)
                {
                    Selected_Donation_Type(pVal);
                }
                if (pVal.ItemUID == "302" && pVal.EventType == BoEventTypes.et_COMBO_SELECT && !pVal.BeforeAction)
                {
                    Selected_Ambassador_Type(pVal);
                }
                if (pVal.ItemUID == "7" && pVal.EventType == BoEventTypes.et_CHOOSE_FROM_LIST && !pVal.BeforeAction)
                {
                    Choose_Card_ID(pVal);
                }
                if (pVal.ItemUID == "43" && pVal.EventType == BoEventTypes.et_CHOOSE_FROM_LIST && !pVal.BeforeAction)
                {
                    Choose_Ambassador(pVal);
                }
                if ( pVal.ItemUID == "45" && pVal.EventType == BoEventTypes.et_CHOOSE_FROM_LIST && !pVal.BeforeAction)
                {
                    Choose_General_Item_ID(pVal);
                }

                if (pVal.ItemUID == "60" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Remove_Patient_Selected_Lines(pVal);
                }
                if (pVal.ItemUID == "61" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Add_Patient_Line(pVal, "@ST_FUND_EXP_DON_PAT");
                }
                if (pVal.ItemUID == "62" && pVal.ColUID == "CODE" && pVal.EventType == BoEventTypes.et_CHOOSE_FROM_LIST && !pVal.BeforeAction)
                {
                    Patient_Choos_From_List(pVal, "@ST_FUND_EXP_DON_PAT");
                }

                if (pVal.ItemUID == "101" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Remove_Naming_Selected_Lines(pVal);
                }
                if (pVal.ItemUID == "102" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Add_Naming_Line(pVal, "@ST_FUND_EXP_DON_NAM");
                }
                if (pVal.ItemUID == "103" && pVal.ColUID == "CODE" && pVal.EventType == BoEventTypes.et_CHOOSE_FROM_LIST && !pVal.BeforeAction)
                {
                    Naming_Choos_From_List(pVal, "@ST_FUND_EXP_DON_NAM");
                }

                if (pVal.ItemUID == "201" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Remove_Selected_Lines(pVal, "Payment_Dets","203");
                }
                if (pVal.ItemUID == "202" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Add_Line(pVal, "@ST_FUND_EXP_DON_PAY", "Payment_Dets","203","U_ST_DATE",true);
                }
                if (pVal.ItemUID == "203" && pVal.ColUID == "Activity" && pVal.EventType == BoEventTypes.et_CHOOSE_FROM_LIST && !pVal.BeforeAction)
                {
                    Activity_choose_From_List(pVal, "@ST_FUND_EXP_DON_PAY");
                }

                if (pVal.ItemUID == "1000" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    KHCF_Logic_Utility.Select_Allocation(pVal, Form_Obj, "502");
                }

            }
            catch (Exception ex)
            {
                Form form = SBO_Application.Forms.Item(pVal.FormUID);
                form.Freeze(false);
                SBO_Application.StatusBar.SetText(ex.Message, BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Error);
            }
        }

        internal static void Print(ItemEvent pVal)
        {
            List<Helper.Utility.Crystal_Report_Parameter> Result = new List<Helper.Utility.Crystal_Report_Parameter>();
            string Rpt_File = Utility.Get_Configuration(company, "Expected_Donation_Print", "Expected Donation Print Path", "");
            string Pdf_File_Name = Helper.Utility.Crystal_Report_Export(company, Rpt_File, Result.ToArray(), Helper.Utility.Export_File_Format.PDF, "", Utility.Get_Configuration(company, "Report_Output_Folder_Path", "Report Output Folder Path", ""));
            SBO_Application.StatusBar.SetText("Report has been Created Successfully at " + Pdf_File_Name, BoMessageTime.bmt_Long, BoStatusBarMessageType.smt_Success);
        }

        internal static void Patient_Choos_From_List(ItemEvent pVal, string Line_DataSource_Table)
        {
            SAPbouiCOM.Form form = Loader.SBO_Application.Forms.Item(pVal.FormUID);
            SAPbouiCOM.ChooseFromListEvent Choos_Event = (SAPbouiCOM.ChooseFromListEvent)pVal;

            if (Choos_Event.SelectedObjects == null || Choos_Event.SelectedObjects.Rows.Count == 0)
            {
                return;
            }
            Matrix Mat = (Matrix)form.Items.Item("62").Specific;
            DataTable DT_Patient_Details = form.DataSources.DataTables.Item("Patients_Details");

            string Code = Choos_Event.SelectedObjects.GetValue("Code", 0).ToString();
            int Index = pVal.Row - 1;
            form.DataSources.DBDataSources.Item(Line_DataSource_Table).SetValue("U_ST_PATIENTS_CODE", Index, Code);
            Set_Patient_Data(form, DT_Patient_Details, Index, Line_DataSource_Table);
            Mat.LoadFromDataSource();

            Mat.AutoResizeColumns();
            if (form.Mode == BoFormMode.fm_OK_MODE)
            {
                form.Mode = BoFormMode.fm_UPDATE_MODE;
            }

        }

        internal static void Set_Patient_Data(Form form, DataTable DT_Patient_Details, int DT_Row_Index, string Line_DataSource_Table)
        {
            string Code = form.DataSources.DBDataSources.Item(Line_DataSource_Table).GetValue("U_ST_PATIENTS_CODE", DT_Row_Index);
            string SQL = $@"SELECT T0.U_ST_FULL_ARABIC_NAME, T0.U_ST_GENDER, T0.U_ST_DATE_OF_BIRTH, T0.U_ST_NATIONAL_ID
FROM ""@ST_PATIENTS_CARD""  T0 WHERE T0.""Code"" ='{Code}'";
            Recordset RC = Helper.Utility.Execute_Recordset_Query(company, SQL);
            for (int J = 0; J < DT_Patient_Details.Columns.Count; J++)
            {
                string Col_Name = DT_Patient_Details.Columns.Item(J).Name;
                if (Col_Name == "SELECTED")
                {
                    continue;
                }
                if (Col_Name == "U_ST_PATIENTS_CODE")
                {
                    DT_Patient_Details.SetValue(Col_Name, DT_Row_Index, Code);
                    continue;
                }
                
                DT_Patient_Details.SetValue(Col_Name, DT_Row_Index, RC.Fields.Item(Col_Name).Value);
            }
        }

        internal static void Add_Patient_Line(ItemEvent pVal, string Line_DataSource_Table)
        {
            Form form = SBO_Application.Forms.Item(pVal.FormUID);
            DataTable DT_Patients_Details = form.DataSources.DataTables.Item("Patients_Details");
            form.Freeze(true);
            DBDataSource DS_Lines = form.DataSources.DBDataSources.Item(Line_DataSource_Table);
            Matrix Mat_Lines = (Matrix)form.Items.Item("62").Specific;
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
            if (DS_Lines.Size > DT_Patients_Details.Rows.Count)
            {
                DT_Patients_Details.Rows.Add();
            }
            Mat_Lines.LoadFromDataSource();
            //Set_Default_Value_Before_Adding(form);

            if (form.Mode == BoFormMode.fm_OK_MODE)
            {
                form.Mode = BoFormMode.fm_UPDATE_MODE;
            }
            form.Freeze(false);

        }

        internal static void Remove_Patient_Selected_Lines(ItemEvent pVal)
        {
            Form form = SBO_Application.Forms.Item(pVal.FormUID);
            DataTable DT_Patient_Details = form.DataSources.DataTables.Item("Patients_Details");
            Matrix Mat = (Matrix)form.Items.Item("62").Specific;
            int Count = Mat.RowCount;
            for (int i = Count; i > 0; i--)
            {
                //CheckBox Chk_Selected = (CheckBox)Mat.GetCellSpecific("SELECTED", i);
                CheckBox Chk_Selected = (CheckBox)Mat.Columns.Item("SELECTED").Cells.Item(i).Specific;
                if (Chk_Selected.Checked)
                {
                    Mat.DeleteRow(i);
                    DT_Patient_Details.Rows.Remove(i);
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

        internal static void Set_Naming_Data(Form form, DataTable DT_Patient_Details, int DT_Row_Index, string Line_DataSource_Table)
        {
            string Code = form.DataSources.DBDataSources.Item(Line_DataSource_Table).GetValue("U_ST_NAMING_CODE", DT_Row_Index);
            string SQL = $@"SELECT T0.U_ST_AREA_NAME, T0.U_ST_AREA_DESCRIPTION, T0.U_ST_AREA_NAMING_AMOUNT, T0.U_ST_NAMED_DONOR
FROM ""@ST_NAMING""  T0 WHERE T0.""Code"" ='{Code}'";
            Recordset RC = Helper.Utility.Execute_Recordset_Query(company, SQL);
            for (int J = 0; J < DT_Patient_Details.Columns.Count; J++)
            {
                string Col_Name = DT_Patient_Details.Columns.Item(J).Name;
                if (Col_Name == "SELECTED")
                {
                    continue;
                }
                if (Col_Name == "U_ST_NAMING_CODE")
                {
                    DT_Patient_Details.SetValue(Col_Name, DT_Row_Index, Code);
                    continue;
                }
                DT_Patient_Details.SetValue(Col_Name, DT_Row_Index, RC.Fields.Item(Col_Name).Value);
            }
        }

        internal static void Load_Naming(Form form, string Line_DataSource_Table)
        {
            DataTable DT_Patients_Details = form.DataSources.DataTables.Item("Naming_Details");
            DT_Patients_Details.Rows.Clear();
            int Rows_Count = form.DataSources.DBDataSources.Item(Line_DataSource_Table).Size;
            DT_Patients_Details.Rows.Add(Rows_Count);
            for (int i = 0; i < Rows_Count; i++)
            {
                Set_Naming_Data(form, DT_Patients_Details, i, Line_DataSource_Table);
            }

    ((Matrix)form.Items.Item("103").Specific).LoadFromDataSource();
            ((Matrix)form.Items.Item("103").Specific).AutoResizeColumns();
        }


        internal static void Add_Naming_Line(ItemEvent pVal, string Line_DataSource_Table)
        {
            Form form = SBO_Application.Forms.Item(pVal.FormUID);
            DataTable DT_Details = form.DataSources.DataTables.Item("Naming_Details");
            form.Freeze(true);
            DBDataSource DS_Lines = form.DataSources.DBDataSources.Item(Line_DataSource_Table);
            Matrix Mat_Lines = (Matrix)form.Items.Item("103").Specific;
            Mat_Lines.FlushToDataSource();
            int Count = DS_Lines.Size;
            if (Count == 1)
            {
                if (DS_Lines.GetValue("U_ST_NAMING_CODE", Count - 1) != "")
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
            if (DS_Lines.Size > DT_Details.Rows.Count)
            {
                DT_Details.Rows.Add();
            }
            Mat_Lines.LoadFromDataSource();
            //Set_Default_Value_Before_Adding(form);

            if (form.Mode == BoFormMode.fm_OK_MODE)
            {
                form.Mode = BoFormMode.fm_UPDATE_MODE;
            }
            form.Freeze(false);

        }

        internal static void Remove_Naming_Selected_Lines(ItemEvent pVal)
        {
            Form form = SBO_Application.Forms.Item(pVal.FormUID);
            DataTable DT_Patient_Details = form.DataSources.DataTables.Item("Naming_Details");
            Matrix Mat = (Matrix)form.Items.Item("103").Specific;
            int Count = Mat.RowCount;
            for (int i = Count; i > 0; i--)
            {
                //CheckBox Chk_Selected = (CheckBox)Mat.GetCellSpecific("SELECTED", i);
                CheckBox Chk_Selected = (CheckBox)Mat.Columns.Item("SELECTED").Cells.Item(i).Specific;
                if (Chk_Selected.Checked)
                {
                    Mat.DeleteRow(i);
                    DT_Patient_Details.Rows.Remove(i -1);
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

        internal static void Naming_Choos_From_List(ItemEvent pVal, string Line_DataSource_Table)
        {
            SAPbouiCOM.Form form = Loader.SBO_Application.Forms.Item(pVal.FormUID);
            SAPbouiCOM.ChooseFromListEvent Choos_Event = (SAPbouiCOM.ChooseFromListEvent)pVal;

            if (Choos_Event.SelectedObjects == null || Choos_Event.SelectedObjects.Rows.Count == 0)
            {
                return;
            }
            Matrix Mat = (Matrix)form.Items.Item("103").Specific;
            DataTable DT_Patient_Details = form.DataSources.DataTables.Item("Naming_Details");

            string Code = Choos_Event.SelectedObjects.GetValue("Code", 0).ToString();
            int Index = pVal.Row - 1;
            form.DataSources.DBDataSources.Item(Line_DataSource_Table).SetValue("U_ST_NAMING_CODE", Index, Code);
            Set_Naming_Data(form, DT_Patient_Details, Index, Line_DataSource_Table);
            Mat.LoadFromDataSource();

            Mat.AutoResizeColumns();
            if (form.Mode == BoFormMode.fm_OK_MODE)
            {
                form.Mode = BoFormMode.fm_UPDATE_MODE;
            }

        }

        internal static void Set_Machinery_Data(Form form, DataTable DT_Patient_Details, int DT_Row_Index, string Line_DataSource_Table)
        {
            string Code = form.DataSources.DBDataSources.Item(Line_DataSource_Table).GetValue("U_ST_MACHINERY_CODE", DT_Row_Index);
            string SQL = $@"SELECT T0.U_ST_MACHINE_NAME_AR, T0.U_ST_MACHINE_IMPACT_AR, T0.U_ST_MACHINE_DESCRIPTION_AR
FROM ""@ST_MACHINERY""  T0 WHERE T0.""Code"" ='{Code}'";
            Recordset RC = Helper.Utility.Execute_Recordset_Query(company, SQL);
            for (int J = 0; J < DT_Patient_Details.Columns.Count; J++)
            {
                string Col_Name = DT_Patient_Details.Columns.Item(J).Name;
                if (Col_Name == "SELECTED")
                {
                    continue;
                }
                if (Col_Name == "U_ST_MACHINERY_CODE")
                {
                    DT_Patient_Details.SetValue(Col_Name, DT_Row_Index, Code);
                    continue;
                }
                DT_Patient_Details.SetValue(Col_Name, DT_Row_Index, RC.Fields.Item(Col_Name).Value);
            }
        }


        internal static void Load_Machinery(Form form, string Line_DataSource_Table)
        {
            DataTable DT_Patients_Details = form.DataSources.DataTables.Item("Machinery_Details");
            DT_Patients_Details.Rows.Clear();
            int Rows_Count = form.DataSources.DBDataSources.Item(Line_DataSource_Table).Size;
            DT_Patients_Details.Rows.Add(Rows_Count);
            for (int i = 0; i < Rows_Count; i++)
            {
                Set_Machinery_Data(form, DT_Patients_Details, i, Line_DataSource_Table);
            }

    ((Matrix)form.Items.Item("203").Specific).LoadFromDataSource();
            ((Matrix)form.Items.Item("203").Specific).AutoResizeColumns();
        }


        internal static void Add_Machinery_Line(ItemEvent pVal, string Line_DataSource_Table)
        {
            Form form = SBO_Application.Forms.Item(pVal.FormUID);
            DataTable DT_Details = form.DataSources.DataTables.Item("Machinery_Details");
            form.Freeze(true);
            DBDataSource DS_Lines = form.DataSources.DBDataSources.Item(Line_DataSource_Table);
            Matrix Mat_Lines = (Matrix)form.Items.Item("203").Specific;
            Mat_Lines.FlushToDataSource();
            int Count = DS_Lines.Size;
            if (Count == 1)
            {
                if (DS_Lines.GetValue("U_ST_MACHINERY_CODE", Count - 1) != "")
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
            if (DS_Lines.Size > DT_Details.Rows.Count)
            {
                DT_Details.Rows.Add();
            }
            Mat_Lines.LoadFromDataSource();
            //Set_Default_Value_Before_Adding(form);

            if (form.Mode == BoFormMode.fm_OK_MODE)
            {
                form.Mode = BoFormMode.fm_UPDATE_MODE;
            }
            form.Freeze(false);

        }

        internal static void Remove_Machinery_Selected_Lines(ItemEvent pVal)
        {
            Form form = SBO_Application.Forms.Item(pVal.FormUID);
            DataTable DT_Patient_Details = form.DataSources.DataTables.Item("Machinery_Details");
            Matrix Mat = (Matrix)form.Items.Item("203").Specific;
            int Count = Mat.RowCount;
            for (int i = Count; i > 0; i--)
            {
                //CheckBox Chk_Selected = (CheckBox)Mat.GetCellSpecific("SELECTED", i);
                CheckBox Chk_Selected = (CheckBox)Mat.Columns.Item("SELECTED").Cells.Item(i).Specific;
                if (Chk_Selected.Checked)
                {
                    Mat.DeleteRow(i);
                    DT_Patient_Details.Rows.Remove(i -1);
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

        internal static void Machinery_Choos_From_List(ItemEvent pVal, string Line_DataSource_Table)
        {
            SAPbouiCOM.Form form = Loader.SBO_Application.Forms.Item(pVal.FormUID);
            SAPbouiCOM.ChooseFromListEvent Choos_Event = (SAPbouiCOM.ChooseFromListEvent)pVal;

            if (Choos_Event.SelectedObjects == null || Choos_Event.SelectedObjects.Rows.Count == 0)
            {
                return;
            }
            Matrix Mat = (Matrix)form.Items.Item("203").Specific;
            DataTable DT_Patient_Details = form.DataSources.DataTables.Item("Machinery_Details");

            string Code = Choos_Event.SelectedObjects.GetValue("Code", 0).ToString();
            int Index = pVal.Row - 1;
            form.DataSources.DBDataSources.Item(Line_DataSource_Table).SetValue("U_ST_MACHINERY_CODE", Index, Code);
            Set_Machinery_Data(form, DT_Patient_Details, Index, Line_DataSource_Table);
            Mat.LoadFromDataSource();

            Mat.AutoResizeColumns();
            if (form.Mode == BoFormMode.fm_OK_MODE)
            {
                form.Mode = BoFormMode.fm_UPDATE_MODE;
            }

        }

        private static void Convert_To_Actual(ItemEvent pVal)
        {
            Form form = SBO_Application.Forms.Item(pVal.FormUID);
            if (form.DataSources.DBDataSources.Item(0).GetValue("U_ST_FINANCE_CONFIRMATION", 0) != "Y")
            {
                throw new Logic.Custom_Exception("We can't convert to Actual Donation if we don't have Finance Confirmation");
            }
            EditText bpET = (EditText)form.Items.Item("44").Specific;
            string donorCode = bpET.Value;
            if (string.IsNullOrEmpty(donorCode))
            {
                throw new Logic.Custom_Exception("You cannot convert to actual, please flag the contact as donor");
            }
            //string MemberCard_Code = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_MEMBER_CARD", 0);
            //string X = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_END_DATE", 0);
            //DateTime Old_End_Date = DateTime.ParseExact(form.DataSources.DBDataSources.Item(0).GetValue("U_ST_END_DATE", 0), "yyyyMMdd", null);
            //bool Is_Past;
            //DateTime New_Renewal_StartDate = Membership.Get_New_Renewal_StartDate(company, Old_End_Date, out Is_Past);


            UDO_Definition KHCF_Actual_Object = Objects_Logic.All_UDO_Definition.FirstOrDefault(O => O.KHCF_Object ==  KHCF_Objects.Actual_Donations);
            Form Actual_Form = Loader.Open_UDO_Form(KHCF_Actual_Object.KHCF_Object);
            Actual_Form.Mode = BoFormMode.fm_ADD_MODE;
            Field_Definition[] Fields = Fields_Logic.All_Field_Definition.Where(F => F.KHCF_Object.ToString() == "Actual_Donations").ToArray();
            string[] expectedFields = Fields_Logic.All_Field_Definition.Where(F => F.KHCF_Object == Form_Obj.KHCF_Object).Select(F=>F.Field_Name).ToArray();
            Field_Definition[] jointFields = Fields.Where(F=> expectedFields.Contains(F.Field_Name)).ToArray();
            Actual_Form.Freeze(true);
            foreach (Field_Definition One_Field in jointFields)
            {
                Actual_Form.DataSources.DBDataSources.Item(0).SetValue(One_Field.Column_Name_In_DB, 0, form.DataSources.DBDataSources.Item(0).GetValue(One_Field.Column_Name_In_DB, 0));
            }
            Actual_Form.DataSources.DBDataSources.Item(0).SetValue("U_ST_EXP_ID", 0, form.DataSources.DBDataSources.Item(0).GetValue("Code",0));
            //int Index = 0;
            //int Rows_Count = form.DataSources.DBDataSources.Item("@ST_FUND_EXP_DON_ORP").Size;

            //DataTable DT_Orphans_Details = Actual_Form.DataSources.DataTables.Item("Orphans_Details");
            //DT_Orphans_Details.Rows.Add(Rows_Count);
            //for (int i = 0; i < Rows_Count; i++)
            //{
            //    string Orphans_Code = form.DataSources.DBDataSources.Item("@ST_FUND_EXP_DON_ORP").GetValue("U_ST_ORPHANS_CODE", 0);
            //    if (Orphans_Code == "")
            //    {
            //        return;
            //    }

            //    Actual_Form.DataSources.DBDataSources.Item("@ST_FUND_ACT_DON_ORP").InsertRecord(Index +1);
            //    Actual_Form.DataSources.DBDataSources.Item("@ST_FUND_ACT_DON_ORP").SetValue("U_ST_ORPHANS_CODE", Index, Orphans_Code);

            //    //DT_Orphans_Details.
            //    Set_Orphan_Data(Actual_Form, DT_Orphans_Details, Index, "@ST_FUND_ACT_DON_ORP");

            //    Index++;
            //}

            //Matrix Mat = (Matrix)Actual_Form.Items.Item("52").Specific;
            //Mat.LoadFromDataSource();


            DataTable DT_Departments = form.DataSources.DataTables.Item("DT_Departments");
            DataTable DT_Actual_Departments = Actual_Form.DataSources.DataTables.Item("DT_Departments");
            Dictionary<string, string> Departments = new Dictionary<string, string>();
            Departments.Add("LID", "LID");
            Departments.Add("LCD", "LCD");
            Departments.Add("IDD", "IDD");
            Helper.Utility.FillGridForDictionary(DT_Actual_Departments, Departments, true);
            for (int i = 0; i < DT_Departments.Rows.Count; i++)
            {
                string X = DT_Departments.GetValue("SELECTED", i).ToString();
                DT_Actual_Departments.SetValue("SELECTED", i, X);
            }

            Actual_Form.Freeze(false);
        }

        private static void Approve(ItemEvent pVal)
        {
            Form form = SBO_Application.Forms.Item(pVal.FormUID);
            string UDO_Code = form.DataSources.DBDataSources.Item(0).GetValue("Code", 0);

            Field_Data Fld = new Field_Data() { Field_Name = "U_ST_FINANCE_CONFIRMATION", Value = "Y" };
            Utility.Update_UDO(company, Form_Obj.UDO_Info, UDO_Code, new Field_Data[] { Fld });

            SBO_Application.StatusBar.SetText("Operation completed successfully!", BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Success);

        }

        internal static void Add_Activity(ItemEvent pVal)
        {
            Form form = SBO_Application.Forms.Item(pVal.FormUID);
            if (form.Mode != BoFormMode.fm_OK_MODE)
            {
                throw new Logic.Custom_Exception("We can add Activity if the form in OK Mode only");
            }
            string Code = form.DataSources.DBDataSources.Item(0).GetValue("Code", 0);
            string BP_Code = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_DONOR_MASTER_RECORD", 0);
            if (BP_Code == "")
            {
                throw new Logic.Custom_Exception("There is no BP Code(Donor Master Record )");
            }

            SBO_Application.ActivateMenuItem("2563");
            Form Frm_Activity = SBO_Application.Forms.ActiveForm;

            Form UDF_Form = SBO_Application.Forms.Item(Frm_Activity.UDFFormUID);
            System.Threading.Thread.Sleep(1000);
            //UDO_Definition UDO_Info = Logic.Objects_Logic.All_UDO_Definition.FirstOrDefault(U => U.KHCF_Object == KHCF_Objects.CCI_Member_Card);
            ((EditText)Frm_Activity.Items.Item("9").Specific).Value = BP_Code;
            Utility.Set_UDF_Value_on_Form(Code, UDF_Form, "U_ST_KHCF_OBJECT_CODE", true);
            Utility.Set_UDF_Value_on_Form(((int)Form_Obj.KHCF_Object).ToString(), UDF_Form, "U_ST_KHCF_OBJECT_TYPE", false);
            Frm_Activity.Items.Item("37").Click();
            // Frm_Payment.Items.Item("37").Click();
            //((EditText)Frm_Activity.Items.Item("13").Specific).Value = Payment_Amount.ToString();
            //System.Threading.Thread.Sleep(1000);
            //Frm_Activity.Items.Item("14").Click();
            //Frm_Activity.Items.Item("5").Enabled = false;
            //Frm_Activity.Items.Item("10").Enabled = false;
            //Frm_Activity.Items.Item("37").Enabled = false;
            //UDF_Form.Items.Item("U_ST_MEMBERSHIP_CODE").Enabled = false;

        }

        internal static void Choose_General_Item_ID(ItemEvent pVal)
        {
            //string Code = Utility.Chosse_From_List_For_Code_And_DBDataSource(pVal, pVal.ItemUID);
        }

        internal static void Choose_Card_ID(ItemEvent pVal)
        {
            string Code = Utility.Chosse_From_List_For_Code_And_DBDataSource(pVal, pVal.ItemUID);
            SAPbouiCOM.Form form = Loader.SBO_Application.Forms.Item(pVal.FormUID);
            string SQL = "";
            
            UDO_Definition UDO_Info;
            if (form.DataSources.DBDataSources.Item(0).GetValue("U_ST_CARD_TYPE", 0) == "I")
            {
                UDO_Info = Objects_Logic.All_UDO_Definition.FirstOrDefault(O => O.KHCF_Object == KHCF_Objects.Fundraising_Individual_Card);
                SQL = $@"SELECT T0.""U_ST_BP_CODE"",T0.U_ST_FULL_NAME_AR FROM ""@{UDO_Info.Table_Name}""  T0 WHERE T0.""Code"" = '{Code}'";
                Recordset RC = Helper.Utility.Execute_Recordset_Query(company, SQL);

                form.DataSources.DBDataSources.Item(0).SetValue("U_ST_DONOR_MASTER_RECORD", 0, RC.Fields.Item("U_ST_BP_CODE").Value.ToString());
                form.DataSources.DBDataSources.Item(0).SetValue("U_ST_DONATION_NAME", 0, RC.Fields.Item("U_ST_FULL_NAME_AR").Value.ToString());
            }
            else 
            {
                UDO_Info = Objects_Logic.All_UDO_Definition.FirstOrDefault(O => O.KHCF_Object == KHCF_Objects.Fundraising_Corporate_Card);
                SQL = $@"SELECT T0.""U_ST_BP_CODE"",T0.U_ST_COMPANY_ARABIC_NAME FROM ""@{UDO_Info.Table_Name}""  T0 WHERE T0.""Code"" = '{Code}'";
                Recordset RC = Helper.Utility.Execute_Recordset_Query(company, SQL);

                form.DataSources.DBDataSources.Item(0).SetValue("U_ST_DONOR_MASTER_RECORD", 0, RC.Fields.Item("U_ST_BP_CODE").Value.ToString());
                form.DataSources.DBDataSources.Item(0).SetValue("U_ST_DONATION_NAME", 0, RC.Fields.Item("U_ST_COMPANY_ARABIC_NAME").Value.ToString());
            }
            form.Refresh();

        }

        internal static void Select_Donor(ItemEvent pVal)
        {
            string Code = Chosse_From_List_For_Code_And_DBDataSource(pVal, pVal.ItemUID);
            if (Code == "")
            {
                return;
            }
            Form form = SBO_Application.Forms.Item(pVal.FormUID);
            string SQL = $@"SELECT T0.""CardName"" FROM OCRD T0 WHERE T0.""CardCode"" = '{Code}'";
            Recordset rs = Helper.Utility.Execute_Recordset_Query(company, SQL);
            form.DataSources.DBDataSources.Item(0).SetValue("U_ST_DONATION_NAME", 0, rs.Fields.Item("CardName").Value.ToString());
        }
        internal static string Chosse_From_List_For_Code_And_DBDataSource(SAPbouiCOM.ItemEvent pVal, string ItemUID, bool Is_User_DataSource = false, string DataSource_Tablename = "")
        {
            SAPbouiCOM.Form form = Loader.SBO_Application.Forms.Item(pVal.FormUID);
            SAPbouiCOM.ChooseFromListEvent Choos_Event = (SAPbouiCOM.ChooseFromListEvent)pVal;

            if (Choos_Event.SelectedObjects == null || Choos_Event.SelectedObjects.Rows.Count == 0)
            {
                return "";
            }

            string UDF_Name = Helper.Utility.Get_Item_DB_Datasource(form.Items.Item(ItemUID));
            string Code = Choos_Event.SelectedObjects.GetValue("CardCode", 0).ToString();
            if (Is_User_DataSource)
            {
                form.DataSources.UserDataSources.Item(UDF_Name).Value = Code;
            }
            else
            {
                string X;
                if (DataSource_Tablename == "")
                {
                    X = form.DataSources.DBDataSources.Item(0).TableName;
                }
                else
                {
                    X = DataSource_Tablename;
                }

                form.DataSources.DBDataSources.Item(X).SetValue(UDF_Name, 0, Code);
                //string Y = pVal.ItemUID;
                //((SAPbouiCOM.EditText)form.Items.Item(pVal.ItemUID).Specific).Value = Code;
            }


            return Code;
        }
        internal static void Selected_Card_Type(ItemEvent pVal)
        {
            SAPbouiCOM.Form form = Loader.SBO_Application.Forms.Item(pVal.FormUID);

            if (form.DataSources.DBDataSources.Item(0).GetValue("U_ST_CARD_TYPE", 0) == "I")
            {
                ((EditText)form.Items.Item("7").Specific).ChooseFromListUID = "CFL_INDIV_CARD";
                ((LinkedButton)form.Items.Item("55").Specific).LinkedObjectType = "ST_FUND_INDIV_CARD";
            }
            else if (form.DataSources.DBDataSources.Item(0).GetValue("U_ST_CARD_TYPE", 0) == "C")
            {
                ((EditText)form.Items.Item("7").Specific).ChooseFromListUID = "CFL_CORP_CARD";
                ((LinkedButton)form.Items.Item("55").Specific).LinkedObjectType = "ST_FUND_CORP_CARD";
            }
            else
            {
                //((EditText)form.Items.Item("145").Specific).ChooseFromListUID = "";
            }
            ((EditText)form.Items.Item("7").Specific).ChooseFromListAlias = "Code";
            form.DataSources.DBDataSources.Item(0).SetValue("U_ST_CONTACT_CARD", 0, "");
        }
        internal static void Selected_Donation_Type(ItemEvent pVal)
        {
            SAPbouiCOM.Form form = Loader.SBO_Application.Forms.Item(pVal.FormUID);

            if (form.DataSources.DBDataSources.Item(0).GetValue("U_ST_DONATION_TYPE", 0) == "C")
            {
                form.Items.Item("96").SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 3, BoModeVisualBehavior.mvb_True);
                form.Items.Item("96").BackColor = Color.Pink.R | (Color.Pink.G << 8) | (Color.Pink.B << 16);

            }
            else
            {
                ComboBox donationType = (ComboBox)form.Items.Item("96").Specific;
                donationType.Select("");
                form.Items.Item("96").BackColor = Color.FromKnownColor(KnownColor.White).ToArgb();
                form.Items.Item("96").SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 3, BoModeVisualBehavior.mvb_False);

            }

        }
        internal static void Selected_Ambassador_Type(ItemEvent pVal)
        {
            SAPbouiCOM.Form form = Loader.SBO_Application.Forms.Item(pVal.FormUID);

            if (form.DataSources.DBDataSources.Item(0).GetValue("U_ST_AMBASSADOR_TYPE", 0) == "I")
            {
                ((EditText)form.Items.Item("43").Specific).ChooseFromListUID = "CFL_Ambassador";
                ((LinkedButton)form.Items.Item("234").Specific).LinkedObjectType = "ST_FUND_INDIV_CARD";

            }
            else if (form.DataSources.DBDataSources.Item(0).GetValue("U_ST_AMBASSADOR_TYPE", 0) == "C")
            {
                ((EditText)form.Items.Item("43").Specific).ChooseFromListUID = "CFL_AMB_CORP";
                ((LinkedButton)form.Items.Item("234").Specific).LinkedObjectType = "ST_FUND_CORP_CARD";

            }
            else
            {
                //((EditText)form.Items.Item("145").Specific).ChooseFromListUID = "";
            }
      ((EditText)form.Items.Item("7").Specific).ChooseFromListAlias = "Code";
            form.DataSources.DBDataSources.Item(0).SetValue("U_ST_CONTACT_CARD", 0, "");
        }

        internal static void Orphans_Choos_From_List(ItemEvent pVal, string Line_DataSource_Table)
        {
            SAPbouiCOM.Form form = Loader.SBO_Application.Forms.Item(pVal.FormUID);
            //SAPbouiCOM.ChooseFromListEvent Choos_Event = (SAPbouiCOM.ChooseFromListEvent)pVal;

            //if (Choos_Event.SelectedObjects == null || Choos_Event.SelectedObjects.Rows.Count == 0)
            //{
            //    return;
            //}
            Matrix Mat = (Matrix)form.Items.Item("52").Specific;
            DataTable DT_Orphans_Details = form.DataSources.DataTables.Item("Orphans_Details");

            //string Code = Choos_Event.SelectedObjects.GetValue("CardCode", 0).ToString();
            int Index = pVal.Row - 1;
            //form.DataSources.DBDataSources.Item(Line_DataSource_Table).SetValue("U_ST_ORPHANS_CODE", Index, Code);

            //string Code = form.DataSources.DBDataSources.Item(Line_DataSource_Table).GetValue("U_ST_ORPHANS_CODE", Index);
            Set_Orphan_Data(form, DT_Orphans_Details, Index, Line_DataSource_Table);
            Mat.LoadFromDataSource();

            Mat.AutoResizeColumns();
        }

        internal static void Add_Orphans_Line(ItemEvent pVal, string Line_DataSource_Table)
        {
            Form form = SBO_Application.Forms.Item(pVal.FormUID);
            DataTable DT_Orphans_Details = form.DataSources.DataTables.Item("Orphans_Details");
            form.Freeze(true);
            DBDataSource DS_Lines = form.DataSources.DBDataSources.Item(Line_DataSource_Table);
            Matrix Mat_Lines = (Matrix)form.Items.Item("52").Specific;
            Mat_Lines.FlushToDataSource();
            int Count = DS_Lines.Size;
            if (Count == 1)
            {
                if (DS_Lines.GetValue("U_ST_ORPHANS_CODE", Count - 1) != "")
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
            DataTable DT_Orphans_Details = form.DataSources.DataTables.Item("Orphans_Details");
            Matrix Mat = (Matrix)form.Items.Item("52").Specific;
            int Count = Mat.RowCount;
            for (int i = Count; i > 0; i--)
            {
                //CheckBox Chk_Selected = (CheckBox)Mat.GetCellSpecific("SELECTED", i);
                CheckBox Chk_Selected = (CheckBox)Mat.Columns.Item("SELECTED").Cells.Item(i).Specific;
                if (Chk_Selected.Checked)
                {
                    Mat.DeleteRow(i);
                    DT_Orphans_Details.Rows.Remove(i);
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
        private static void Choose_Ambassador(ItemEvent pVal)
        {
            SAPbouiCOM.Form form = SBO_Application.Forms.Item(pVal.FormUID);
            ChooseFromListEvent Chos_Event = (ChooseFromListEvent)pVal;
            string Code = "";
            if ((Chos_Event.SelectedObjects == null ? false : Chos_Event.SelectedObjects.Rows.Count != 0))
            {
                if (Chos_Event.ChooseFromListUID == "CFL_Ambassador")
                {
                    Code = Chos_Event.SelectedObjects.GetValue("Code", 0).ToString();
                }
                else
                {
                    Code = Chos_Event.SelectedObjects.GetValue("Code", 0).ToString();
                }
                form.DataSources.DBDataSources.Item(0).SetValue("U_ST_AMBASSADOR", 0, Code);
                if (form.Mode == BoFormMode.fm_OK_MODE)
                {
                    form.Mode = BoFormMode.fm_UPDATE_MODE;
                }
            }
        }
        private static void Choose_Activity_ID(ItemEvent pVal)
        {
            SAPbouiCOM.Form form = SBO_Application.Forms.Item(pVal.FormUID);
            ChooseFromListEvent Chos_Event = (ChooseFromListEvent)pVal;
            string Code = "";
            if ((Chos_Event.SelectedObjects == null ? false : Chos_Event.SelectedObjects.Rows.Count != 0))
            {
                Code = Chos_Event.SelectedObjects.GetValue("Code", 0).ToString();
                form.DataSources.DBDataSources.Item(0).SetValue("U_ST_ACTIVITY_ID", 0, Code);
            }
            if (form.Mode == BoFormMode.fm_OK_MODE)
            {
                form.Mode = BoFormMode.fm_UPDATE_MODE;
            }
        }
        private static void Choose_Ambassador_ID(ItemEvent pVal)
        {
            SAPbouiCOM.Form form = SBO_Application.Forms.Item(pVal.FormUID);
            ChooseFromListEvent Chos_Event = (ChooseFromListEvent)pVal;
            string Code = "";
            if ((Chos_Event.SelectedObjects == null ? false : Chos_Event.SelectedObjects.Rows.Count != 0))
            {
                Code = Chos_Event.SelectedObjects.GetValue("Code", 0).ToString();
                form.DataSources.DBDataSources.Item(0).SetValue("U_ST_AMBASSADOR_ACT", 0, Code);
            }
            if (form.Mode == BoFormMode.fm_OK_MODE)
            {
                form.Mode = BoFormMode.fm_UPDATE_MODE;
            }
        }
        private static void Choose_Behalf_Of(ItemEvent pVal)
        {
            SAPbouiCOM.Form form = SBO_Application.Forms.Item(pVal.FormUID);
            ChooseFromListEvent Chos_Event = (ChooseFromListEvent)pVal;
            string Code = "";
            if ((Chos_Event.SelectedObjects == null ? false : Chos_Event.SelectedObjects.Rows.Count != 0))
            {
                Code = Chos_Event.SelectedObjects.GetValue("Code", 0).ToString();
                form.DataSources.DBDataSources.Item(0).SetValue("U_ST_ON_BEHALF_OF", 0, Code);
            }
            if (form.Mode == BoFormMode.fm_OK_MODE)
            {
                form.Mode = BoFormMode.fm_UPDATE_MODE;
            }
        }
        private static void Choose_Target_ID(ItemEvent pVal)
        {
            SAPbouiCOM.Form form = SBO_Application.Forms.Item(pVal.FormUID);
            ChooseFromListEvent Chos_Event = (ChooseFromListEvent)pVal;
            string Code = "";
            if ((Chos_Event.SelectedObjects == null ? false : Chos_Event.SelectedObjects.Rows.Count != 0))
            {
                Code = Chos_Event.SelectedObjects.GetValue("Code", 0).ToString();
                form.DataSources.DBDataSources.Item(0).SetValue("U_ST_TARGET", 0, Code);
            }
            if (form.Mode == BoFormMode.fm_OK_MODE)
            {
                form.Mode = BoFormMode.fm_UPDATE_MODE;
            }
        }
        private static void Choose_Recurring_Donation(ItemEvent pVal)
        {
            SAPbouiCOM.Form form = SBO_Application.Forms.Item(pVal.FormUID);
            ChooseFromListEvent Chos_Event = (ChooseFromListEvent)pVal;
            string Code = "";
            if ((Chos_Event.SelectedObjects == null ? false : Chos_Event.SelectedObjects.Rows.Count != 0))
            {
                Code = Chos_Event.SelectedObjects.GetValue("Code", 0).ToString();
                form.DataSources.DBDataSources.Item(0).SetValue("U_ST_RECURRING", 0, Code);
            }
            if (form.Mode == BoFormMode.fm_OK_MODE)
            {
                form.Mode = BoFormMode.fm_UPDATE_MODE;
            }
        }

        private static void DisableMatrixButtons(Form form)
        {
            List<string> GridBtnsIds = new List<string>() { "111", "110", "116", "115", "119", "118", "129", "128", "601", "600" };
            for (int i = 0; i < GridBtnsIds.Count; i++)
            {
                form.Items.Item(GridBtnsIds[i]).SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 4, BoModeVisualBehavior.mvb_False);

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

           // DT.SetValue("FinCon", Count, "N");
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
        private static void Activity_choose_From_List(ItemEvent pVal, string Line_DataSource_Table)
        {
            SAPbouiCOM.Form form = Loader.SBO_Application.Forms.Item(pVal.FormUID);
            SAPbouiCOM.ChooseFromListEvent Choos_Event = (SAPbouiCOM.ChooseFromListEvent)pVal;

            if (Choos_Event.SelectedObjects == null || Choos_Event.SelectedObjects.Rows.Count == 0)
            {
                return;
            }
            Matrix Mat = (Matrix)form.Items.Item("203").Specific;

            string Code = Choos_Event.SelectedObjects.GetValue("ClgCode", 0).ToString();
            int Index = pVal.Row - 1;
            form.DataSources.DBDataSources.Item(Line_DataSource_Table).SetValue("U_ST_ACTIVITY_ID", Index, Code);
            Mat.LoadFromDataSource();
            Mat.AutoResizeColumns();
            if (form.Mode == BoFormMode.fm_OK_MODE)
            {
                form.Mode = BoFormMode.fm_UPDATE_MODE;
            }
        }

        private static void DisableTabButtons(Form form)
        {
            List<string> GridBtnsIds = new List<string>() { "201", "202" };
            for (int i = 0; i < GridBtnsIds.Count; i++)
            {
                form.Items.Item(GridBtnsIds[i]).SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 4, BoModeVisualBehavior.mvb_False);

            }

        }

    }
}
