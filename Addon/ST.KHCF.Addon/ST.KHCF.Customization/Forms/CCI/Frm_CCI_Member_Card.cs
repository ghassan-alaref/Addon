using SAPbobsCOM;
using SAPbouiCOM;
using ST.KHCF.Customization.Forms.Patient;
using ST.KHCF.Customization.Logic;
using ST.KHCF.Customization.Logic.Classes;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using Form = SAPbouiCOM.Form;

namespace ST.KHCF.Customization.Forms.CCI
{
    internal class Frm_CCI_Member_Card : Parent_Form
    {
        internal static SAPbobsCOM.Company company;
        internal static SAPbouiCOM.Application SBO_Application;
        internal static Parent_Form Form_Obj;

   
        internal override Depends_List[] Get_Depends_List_List()
        {
            List<Depends_List> Result = new List<Depends_List>();
            Result.AddRange(base.Get_Depends_List_List());
            Result.Add(new Depends_List() { Item_ID = "133", Parent_Item_ID = "47", SQL = $@"SELECT T0.""Code"", T0.""Name"" FROM ""@ST_SUB_CHANNEL""  T0 WHERE T0.""U_ST_CHANNEL"" = '{{0}}'" });
            //Result.Add(new Depends_List() { Item_ID = "91", Parent_Item_ID = "143", SQL = $@"SELECT ""GroupCode"" AS ""Code"", T0.""GroupName"" AS ""Name"" FROM OCRG T0  WHERE U_ST_CUSTOMER_TYPE = 'C' AND T0.""GroupType""  = 'C'  AND T0.""U_ST_TYPE"" = '{{0}}' " });
            //Result.Add(new Depends_List() { Item_ID = "133", Parent_Item_ID = "131", SQL = $@"SELECT T0.""Code"", T0.""Name"" FROM ""@ST_SUB_CHANNEL""  T0 WHERE T0.""U_ST_CHANNEL"" = '002'" });
            return Result.ToArray();
        }

        internal override void Initialize_Form(SAPbouiCOM.Form form)
        {
            base.Initialize_Form(form);
            Matrix Mat_Att = (Matrix)form.Items.Item("500").Specific;
            base.Fill_Attachment_ComboBox(Mat_Att);
            string SQL_Customer_Group = $@"SELECT ""GroupCode"" AS ""Code"", T0.""GroupName"" AS ""Name"" FROM OCRG T0 
WHERE  T0.""GroupType""  = 'C' AND T0.""U_ST_TYPE"" = 'I' AND U_ST_CUSTOMER_TYPE = 'C'";
            Helper.Utility.Fill_One_ComboBoxBySQL(company, form, "91", SQL_Customer_Group, true);

            string CCI_Department_ID = Configurations.Get_CCI_Department(company);
            string SQL_Account_Manager = $@"SELECT T0.""SlpCode"" AS ""Code"", T0.""SlpName"" AS ""Name"" 
FROM OSLP T0  INNER JOIN OHEM T1 ON T0.""SlpCode"" = T1.""salesPrson"" 
WHERE T1.""dept"" = {CCI_Department_ID}";
            Helper.Utility.Fill_One_ComboBoxBySQL(company, form, "137", SQL_Account_Manager, true);
            
            string Broker_Vendor_Group = Configurations.Get_Broker_Vendor_Group(company);
            SAPbouiCOM.ChooseFromList CFL_Broker = form.ChooseFromLists.Item("CFL_Broker");
            SAPbouiCOM.ChooseFromList CFL_Broker2 = form.ChooseFromLists.Item("CFL_Broker2");

            Conditions Broker_Cons = CFL_Broker.GetConditions();
            Condition Broker_Con = Broker_Cons.Add();
            Broker_Con.Alias = "GroupCode";
            //Donor_Con.CondVal = "NULL";
            Broker_Con.Operation = BoConditionOperation.co_EQUAL;
            Broker_Con.CondVal = Broker_Vendor_Group;
            CFL_Broker.SetConditions(Broker_Cons);
            
            Conditions Broker_Cons2 = CFL_Broker2.GetConditions();
            Condition Broker_Con2 = Broker_Cons2.Add();
            Broker_Con2.Alias = "GroupCode";
            Broker_Con2.Operation = BoConditionOperation.co_EQUAL;
            Broker_Con2.CondVal = Broker_Vendor_Group;
            CFL_Broker2.SetConditions(Broker_Cons2);

            Grid Grd_Members = (Grid)form.Items.Item("138").Specific;
            Grd_Members.Columns.Item("SELECTED").AffectsFormMode = false;
            ((ComboBoxColumn)Grd_Members.Columns.Item("ST_GENDER")).ValidValues.Add("M", "Male");
            ((ComboBoxColumn)Grd_Members.Columns.Item("ST_GENDER")).ValidValues.Add("F", "Female");
            ((ComboBoxColumn)Grd_Members.Columns.Item("ST_APPROVAL_STATUS")).ValidValues.Add("P", "Pending");
            ((ComboBoxColumn)Grd_Members.Columns.Item("ST_APPROVAL_STATUS")).ValidValues.Add("A", "Approved");
            ((ComboBoxColumn)Grd_Members.Columns.Item("ST_APPROVAL_STATUS")).ValidValues.Add("R", "Rejected");
            ((ComboBoxColumn)Grd_Members.Columns.Item("ST_APPROVAL_STATUS")).DisplayType = BoComboDisplayType.cdt_Description;

            Grid Grd_Membership = (Grid)form.Items.Item("154").Specific;
            string SQL_Cov = $@"SELECT ""Code"" , ""Name"" FROM ""@ST_COVERAGE"" order By ""Code""";
            Helper.Utility.FillGridComboBoxForSQL(company, Grd_Membership, "U_ST_COVERAGE", SQL_Cov);
            //prev = Utility.Add_Time_Log("C", "Coverage", prev);

            string SQL_Memeber_Customer_Group = $@"SELECT ""GroupCode"" AS ""Code"", T0.""GroupName"" AS ""Name"" FROM OCRG T0 
WHERE  T0.""GroupType""  = 'C' AND T0.""U_ST_TYPE"" = 'I' AND U_ST_CUSTOMER_TYPE = 'C'";
            Helper.Utility.FillGridComboBoxForSQL(company, Grd_Members, "ST_CUSTOMER_GROUP", SQL_Memeber_Customer_Group, true);

//            string SQL_Memeber_Customer_Group2 = $@"SELECT ""GroupCode"" AS ""Code"", T0.""GroupName"" AS ""Name"" FROM OCRG T0 
//WHERE  T0.""GroupType""  = 'C'  AND U_ST_CUSTOMER_TYPE = 'C'";
//            Helper.Utility.FillGridComboBoxForSQL(company, Grd_Membership, "U_ST_CUSTOMER_GROUP", SQL_Memeber_Customer_Group2, true);

            //((ComboBoxColumn)Grd_Membership.Columns.Item("U_ST_CUSTOMER_GROUP")).DisplayType = BoComboDisplayType.cdt_Description;

            ((ComboBoxColumn)Grd_Membership.Columns.Item("U_ST_MEMBERSHIP_STATUS")).ValidValues.Add("N", "New");
            ((ComboBoxColumn)Grd_Membership.Columns.Item("U_ST_MEMBERSHIP_STATUS")).ValidValues.Add("R", "Renew");
            ((ComboBoxColumn)Grd_Membership.Columns.Item("U_ST_MEMBERSHIP_STATUS")).ValidValues.Add("P", "Past to Renew");
            ((ComboBoxColumn)Grd_Membership.Columns.Item("U_ST_MEMBERSHIP_STATUS")).ValidValues.Add("C", "Canceled");
            ((ComboBoxColumn)Grd_Membership.Columns.Item("U_ST_MEMBERSHIP_STATUS")).ValidValues.Add("S", "Stopped");
            ((ComboBoxColumn)Grd_Membership.Columns.Item("U_ST_MEMBERSHIP_STATUS")).DisplayType = BoComboDisplayType.cdt_Description;
            ((ComboBoxColumn)Grd_Membership.Columns.Item("U_ST_PARENT_TYPE")).ValidValues.Add("", "None");
            ((ComboBoxColumn)Grd_Membership.Columns.Item("U_ST_PARENT_TYPE")).ValidValues.Add("-", "None");
            ((ComboBoxColumn)Grd_Membership.Columns.Item("U_ST_PARENT_TYPE")).ValidValues.Add("N", "None");
            ((ComboBoxColumn)Grd_Membership.Columns.Item("U_ST_PARENT_TYPE")).ValidValues.Add("I", "Individual");
            ((ComboBoxColumn)Grd_Membership.Columns.Item("U_ST_PARENT_TYPE")).ValidValues.Add("C", "Corporate");
            ((ComboBoxColumn)Grd_Membership.Columns.Item("U_ST_PARENT_TYPE")).DisplayType = BoComboDisplayType.cdt_Description;

            string SQL_Currency = @"SELECT T0.""CurrCode"" AS ""Code"", T0.""CurrName"" As ""Name"" FROM OCRN T0";
            Helper.Utility.Fill_One_ComboBoxBySQL(company, form, "61", SQL_Currency, true);
            SAPbouiCOM.ComboBox comboBox = (SAPbouiCOM.ComboBox)form.Items.Item("61").Specific;
            comboBox.ValidValues.Add("##", "All Currencies");

            ButtonCombo Btn_Cmb_Action = (ButtonCombo)form.Items.Item("163").Specific;
            Btn_Cmb_Action.ValidValues.Add("-", "Can Also");
            Btn_Cmb_Action.ValidValues.Add("M", "Add/Renew Membership");
            Btn_Cmb_Action.ValidValues.Add("L", "Link");
            Btn_Cmb_Action.ValidValues.Add("U", "Unlink");
            Btn_Cmb_Action.ValidValues.Add("P", "Convert to Patient");
            Btn_Cmb_Action.ValidValues.Add("R", "Remove");
            form.DataSources.UserDataSources.Item("163").Value = "-";

            form.Items.Item("164").Visible = false;
            form.Items.Item("163").AffectsFormMode = false;
            form.Items.Item("143").AffectsFormMode = false;
            form.Items.Item("145").AffectsFormMode = false;
            bool v = form.Items.Item("159").Visible;
            ((EditText)form.Items.Item("610").Specific).IsPassword = true;
            Grd_Members.AutoResizeColumns();
            form.Items.Item("139").SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 2, BoModeVisualBehavior.mvb_False);
            form.Items.Item("140").SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 2, BoModeVisualBehavior.mvb_False);

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
                    if(string.IsNullOrEmpty(User_Role)) 
                    {
                        throw new Custom_Exception("You are Not autorized to Add or Update The Card.");
                    }
                    BubbleEvent = Validate_Data(BusinessObjectInfo);
                    if (!BubbleEvent)
                    {
                        return BubbleEvent;
                    }
                    string key = BusinessObjectInfo.ObjectKey;
                    Before_Adding_UDO(BusinessObjectInfo);
                    if (BusinessObjectInfo.EventType == BoEventTypes.et_FORM_DATA_UPDATE)
                    {
                        if (!ADD_Update_UDO(BusinessObjectInfo))
                        {
                            throw new Custom_Exception(Loader.New_Msg);
                        }
                    }

                }
                if ((BusinessObjectInfo.EventType == BoEventTypes.et_FORM_DATA_ADD || BusinessObjectInfo.EventType == BoEventTypes.et_FORM_DATA_UPDATE) 
                    && BusinessObjectInfo.ActionSuccess && !BusinessObjectInfo.BeforeAction)
                {
                    if (BusinessObjectInfo.EventType == BoEventTypes.et_FORM_DATA_ADD)
                    {
                        if (!ADD_Update_UDO(BusinessObjectInfo))
                        {
                            throw new Custom_Exception(Loader.New_Msg);
                        }
                        else
                            Approve(null, BusinessObjectInfo);
                    }
                    if (BusinessObjectInfo.EventType == BoEventTypes.et_FORM_DATA_UPDATE)
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
                SAPbouiCOM.Form form = Loader.SBO_Application.Forms.Item(BusinessObjectInfo.FormUID);
                string TableName = form.DataSources.DBDataSources.Item(0).TableName.Replace("@", "");
                UDO_Definition KHCF_Object = Objects_Logic.All_UDO_Definition.FirstOrDefault(O => O.Table_Name == TableName);
                SBO_Application.StatusBar.SetText("Loading", BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_None);

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
            SAPbouiCOM.Form form = SBO_Application.Forms.Item(businessObjectInfo.FormUID);
            string str = businessObjectInfo.FormTypeEx;
            SBO_Application.StatusBar.SetText("Loading", BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Warning);
            string MemberCard_Code = form.DataSources.DBDataSources.Item(0).GetValue("Code", 0);
            string U_ST_APPROVAL_STATUS = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_APPROVAL_STATUS", 0);
            
            string Card_ID = form.DataSources.DBDataSources.Item(0).GetValue("Code", 0);
            string Parent_Type = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_PARENT_TYPE", 0);
            if (!string.IsNullOrEmpty(Parent_Type))
            {
                LinkedButton Yar_Parent = (LinkedButton)form.Items.Item("182").Specific;
                if (Parent_Type == "C")
                {
                    string SQL_Customer_Group = $@"SELECT ""GroupCode"" AS ""Code"", T0.""GroupName"" AS ""Name"" FROM OCRG T0 
WHERE  T0.""GroupType""  = 'C' AND T0.""U_ST_TYPE"" = '{Parent_Type}' AND U_ST_CUSTOMER_TYPE = 'C'";
                    Helper.Utility.Fill_One_ComboBoxBySQL(company, form, "91", SQL_Customer_Group, true);
                    Yar_Parent.LinkedObjectType = "ST_CCI_CORP_CARD";
                }
                else if (Parent_Type != "N")
                {
                    string SQL_P = $@"SELECT T0.""U_ST_PARENT_ID"", T0.""U_ST_PARENT_TYPE"", T0.""U_ST_PARENT_NAME"" FROM ""@ST_CCI_INDIV_CARD""  T0 WHERE T0.""Code"" ='{MemberCard_Code}'";
                    Recordset RC_P = Helper.Utility.Execute_Recordset_Query(company, SQL_P);
                    Yar_Parent.LinkedObjectType = "ST_CCI_INDIV_CARD";
                    if (RC_P.RecordCount > 0)
                    {
                        string Parent_IDd = RC_P.Fields.Item("U_ST_PARENT_ID").Value.ToString();
                        string Parent_Name = RC_P.Fields.Item("U_ST_PARENT_NAME").Value.ToString();

                        form.DataSources.DBDataSources.Item(0).SetValue("U_ST_PARENT_NAME", 0, Parent_Name);
                        form.DataSources.DBDataSources.Item(0).SetValue("U_ST_PARENT_ID", 0, Parent_IDd);
                    }
                }
                else
                {
                    string SQL_Customer_Group = $@"SELECT ""GroupCode"" AS ""Code"", T0.""GroupName"" AS ""Name"" FROM OCRG T0 
WHERE  T0.""GroupType""  = 'C' AND T0.""U_ST_TYPE"" = 'I' AND U_ST_CUSTOMER_TYPE = 'C'";
                    Helper.Utility.Fill_One_ComboBoxBySQL(company, form, "91", SQL_Customer_Group, true);
                    Yar_Parent.LinkedObjectType = "ST_CCI_INDIV_CARD";
                }
            }

            Form_Obj.Load_Depends_Items(form);
            Form_Obj.Set_Fields(form);
            Member_Cards_UI.Load_Sub_Members(form,"I", Card_ID);
            Load_Memberships(form, Card_ID);
            Member_Cards_UI.Load_Communication_Log(form, "I", Card_ID);

            form.Items.Item("164").Visible = false;
            form.DataSources.UserDataSources.Item("185").Value = "";
        }

        private static void Set_Parent_Link(SAPbouiCOM.Form form)
        {
            LinkedButton Yar_Parent = (LinkedButton)form.Items.Item("182").Specific;
            string Parent_Type = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_PARENT_TYPE", 0);
            if (Parent_Type == "C")
            {
                Yar_Parent.LinkedObjectType = "ST_CCI_CORP_CARD";
                form.DataSources.DBDataSources.Item(0).SetValue("U_ST_PARENT_ID", 0, "");
                form.DataSources.DBDataSources.Item(0).SetValue("U_ST_PARENT_NAME", 0, "");
                form.DataSources.UserDataSources.Item("33").Value = "";
                form.Items.Item("145").Enabled = true;
                form.Items.Item("164").Visible = true;

            }
            else if (Parent_Type == "I")
            {
                form.Items.Item("145").Enabled = true;
                form.Items.Item("164").Visible = true;

                form.DataSources.DBDataSources.Item(0).SetValue("U_ST_PARENT_ID", 0, "");
                form.DataSources.DBDataSources.Item(0).SetValue("U_ST_PARENT_NAME", 0, "");
                form.DataSources.UserDataSources.Item("33").Value = "";
                Yar_Parent.LinkedObjectType = "ST_CCI_INDIV_CARD";

                string SQL_Parent = $@"SELECT ""GroupCode"" AS ""Code"", T0.""GroupName"" AS ""Name"" FROM OCRG T0 
WHERE  T0.""GroupType""  = 'C' AND T0.""U_ST_TYPE"" = 'I' AND U_ST_CUSTOMER_TYPE = 'C'";
                Recordset RC_Parent_CFL = Helper.Utility.Execute_Recordset_Query(company, SQL_Parent);
                SAPbouiCOM.ChooseFromList CFL_OCRD = form.ChooseFromLists.Item("CFL_4");

            }
            else if (Parent_Type == "N")
            {
                form.DataSources.DBDataSources.Item(0).SetValue("U_ST_PARENT_ID", 0, "");
                form.DataSources.DBDataSources.Item(0).SetValue("U_ST_PARENT_NAME", 0, "");
                form.Items.Item("145").Enabled = false;
                form.Items.Item("164").Visible = false;
            }

            if (Parent_Type == "C")
            {
                string SQL_Customer_Group = $@"SELECT ""GroupCode"" AS ""Code"", T0.""GroupName"" AS ""Name"" FROM OCRG T0 
WHERE  T0.""GroupType""  = 'C' AND T0.""U_ST_TYPE"" = '{Parent_Type}' AND U_ST_CUSTOMER_TYPE = 'C'";
                Helper.Utility.Fill_One_ComboBoxBySQL(company, form, "91", SQL_Customer_Group, true);
                Yar_Parent.LinkedObjectType = "ST_CCI_CORP_CARD";
            }
            else
            {
                string SQL_Customer_Group = $@"SELECT ""GroupCode"" AS ""Code"", T0.""GroupName"" AS ""Name"" FROM OCRG T0 
WHERE  T0.""GroupType""  = 'C' AND T0.""U_ST_TYPE"" = 'I' AND U_ST_CUSTOMER_TYPE = 'C'";
                Helper.Utility.Fill_One_ComboBoxBySQL(company, form, "91", SQL_Customer_Group, true);
                Yar_Parent.LinkedObjectType = "ST_CCI_INDIV_CARD";
            }
            form.DataSources.DBDataSources.Item(0).SetValue("U_ST_CUSTOMER_GROUP", 0, "");
        }

        private static void Load_Memberships(SAPbouiCOM.Form form, string Card_ID)
        {
            DataTable DT_Membership = form.DataSources.DataTables.Item("MEMBERSHIP");
            DT_Membership.Rows.Clear();
            string SQL_Membership = $@"
SELECT T0.""Code"", T0.""U_ST_MEMBER_CARD"", T0.""U_ST_MEMBERSHIP_STATUS"", T0.""U_ST_CREATION_DATE"",
 ifnull(T0.""U_ST_PARENT_MEMBERSHIP_TYPE"", '-') AS ""U_ST_PARENT_TYPE"", 
 CASE WHEN IFNULL(T1.""U_ST_MEMBER_NAME"",'') = '' THEN T2.""U_ST_CORPORATE_NAME"" ELSE T1.""U_ST_MEMBER_NAME"" END AS ""U_ST_PARENT_NAME"", 
 T0.""U_ST_PARENT_MEMBERSHIP_ID"" AS ""U_ST_PARENT_ID"", 
 T0.""U_ST_START_DATE"", T0.""U_ST_END_DATE"", T0.""U_ST_ACTIVE"", T0.U_ST_AUTOMATIC_RENEWAL , T0.U_ST_COVERAGE,T0.U_ST_AGE,T0.U_ST_PREMIUM, 
 T0.""U_ST_CUSTOMER_GROUP"" 
 FROM ""@ST_INDIV_MEMBERSHIP""  T0
 INNER JOIN ""@ST_CCI_INDIV_CARD"" T3 ON ( T3.""Code"" = T0.""U_ST_MEMBER_CARD"" )
 LEFT OUTER JOIN ""@ST_INDIV_MEMBERSHIP"" T1 ON ( T1.""Code"" = T0.""U_ST_PARENT_MEMBERSHIP_ID"" AND T0.""U_ST_PARENT_MEMBERSHIP_TYPE"" = 'I' )
 LEFT OUTER JOIN ""@ST_CORP_MEMBERSHIP"" T2 ON ( T2.""Code"" = T0.""U_ST_PARENT_MEMBERSHIP_ID"" AND T0.""U_ST_PARENT_MEMBERSHIP_TYPE"" = 'C' )
 WHERE T0.""U_ST_MEMBER_CARD"" ='{Card_ID}'
";
            Recordset RC_Membership = Helper.Utility.Execute_Recordset_Query(company, SQL_Membership);
            DT_Membership.Rows.Add(RC_Membership.RecordCount);

            for (int i = 0; i < RC_Membership.RecordCount; i++)
            {
                for (int J = 0; J < DT_Membership.Columns.Count; J++)
                {
                    string Col_Name = DT_Membership.Columns.Item(J).Name;
                    string UDF_Name;

                    UDF_Name = Col_Name;
                    if (Col_Name == "U_ST_PREMIUM")
                    {
                        double Premium = Convert.ToDouble(RC_Membership.Fields.Item(UDF_Name).Value.ToString());
                        DT_Membership.SetValue(Col_Name, i, Premium.ToString("N03"));
                    }
                    else
                    {
                        string X = RC_Membership.Fields.Item(UDF_Name).Value.ToString();
                        DT_Membership.SetValue(Col_Name, i, RC_Membership.Fields.Item(UDF_Name).Value);
                    }
                }
                RC_Membership.MoveNext();
            }
            Grid Grd_Membership = (Grid)form.Items.Item("154").Specific;

            Grd_Membership.AutoResizeColumns();

        }

        private static bool ADD_Update_UDO(BusinessObjectInfo businessObjectInfo)
        {
            SAPbouiCOM.Form form = SBO_Application.Forms.Item(businessObjectInfo.FormUID);
            System.Xml.XmlDocument XML_Doc = new System.Xml.XmlDocument();
            string XML_Text = businessObjectInfo.ObjectKey.Replace(" ", "").Replace("=", " = ");
            XML_Doc.LoadXml(XML_Text);

            string UDO_Code = XML_Doc.GetElementsByTagName("Code")[0].InnerText;
            string UDO_Name = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_FULL_NAME_AR", 0);

            if(!Add_Members(form, UDO_Code, UDO_Name))
            {
                return false;
            }

            if (form.DataSources.DBDataSources.Item(0).GetValue("U_ST_APPROVAL_STATUS", 0) == "P" && form.Mode != BoFormMode.fm_ADD_MODE)
            {
                Form_Obj.Send_Alert_For_Approve(UDO_Code);
            }
            else
            {
                string BP_Code = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_BP_CODE", 0);
                UDO_Definition UDO_Info = Logic.Objects_Logic.All_UDO_Definition.FirstOrDefault(U => U.KHCF_Object == KHCF_Objects.CCI_Member_Card);
                if(!string.IsNullOrEmpty(BP_Code))
                    Member_Cards_UI.Update_Card_BP(form, UDO_Code, UDO_Info);
            }
            return true;
        }

        private static bool Add_Members(SAPbouiCOM.Form form, string UDO_Code, string UDO_Name)
        {
            company.StartTransaction();
            try
            {
                Field_Definition[] Address_Fields = Logic.Fields_Logic.All_Field_Definition.Where(U => U.KHCF_Object == KHCF_Objects.CCI_Member_Card_Address).ToArray();

                DataTable DT_Members = form.DataSources.DataTables.Item("MEMBERS");
                List<string> Codes = new List<string>();
                for (int i = 0; i < DT_Members.Rows.Count; i++)
                {
                    string national_id = DT_Members.GetValue("ST_NATIONAL_ID", i).ToString();
                    string nationality = DT_Members.GetValue("ST_NATIONALITY", i).ToString();
                    if ((national_id.ToString().Length != 10 || string.IsNullOrEmpty(national_id)) && (nationality =="JO" || nationality =="Jordan"))
                    {
                        throw new Custom_Exception($"National id in Row {i} Should be 10 digit");
                    }
                    if (DT_Members.GetValue("Code", i).ToString() == "" && DT_Members.GetValue("ST_GENDER", i).ToString() != "")
                    {
                        KHCF_BP BP = new KHCF_BP();
                        BP.BP_Group = int.Parse(form.DataSources.DBDataSources.Item(0).GetValue("U_ST_CUSTOMER_GROUP", 0));
                        BP.CardName = DT_Members.GetValue("ST_FULL_NAME_AR", 0).ToString();
                        BP.FatherCode = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_BP_CODE", 0);
                        if (form.DataSources.DBDataSources.Item(0).GetValue("U_ST_FATHER_TYPE", 0) == "P")
                        {
                            BP.FatherType = BoFatherCardTypes.cPayments_sum;
                        }
                        else
                        {
                            BP.FatherType = BoFatherCardTypes.cDelivery_sum;
                        }
                        BP.Email = ((EditText)(form.Items.Item("94").Specific)).Value.ToString();
                        BP.Mobile = ((EditText)(form.Items.Item("185").Specific)).Value.ToString();
                        string tel1 = ((EditText)(form.Items.Item("39").Specific)).Value.ToString();
                        string BP_Code = "";

                        string New_UDO_Code = Utility.Get_New_UDO_Code(company, Form_Obj.KHCF_Object);
                        Codes.Add(New_UDO_Code);

                        SAPbobsCOM.CompanyService oCmpSrv = company.GetCompanyService();
                        SAPbobsCOM.GeneralService oGeneralService = oCmpSrv.GetGeneralService(form.BusinessObject.Type);
                        SAPbobsCOM.GeneralData oGeneralData = (SAPbobsCOM.GeneralData)oGeneralService.GetDataInterface(SAPbobsCOM.GeneralServiceDataInterfaces.gsGeneralData);

                        oGeneralData.SetProperty("Code", New_UDO_Code);
                        oGeneralData.SetProperty("U_ST_BP_CODE", BP_Code);
                        oGeneralData.SetProperty("U_ST_PARENT_ID", UDO_Code);
                        oGeneralData.SetProperty("U_ST_PARENT_NAME", UDO_Name);
                        oGeneralData.SetProperty("U_ST_CREATOR", company.UserName);
                        oGeneralData.SetProperty("U_ST_PARENT_TYPE", "I");
                        oGeneralData.SetProperty("U_ST_CUSTOMER_GROUP", BP.BP_Group.ToString());
                        oGeneralData.SetProperty("U_ST_CHANNEL", form.DataSources.DBDataSources.Item(0).GetValue("U_ST_CHANNEL", 0));
                        oGeneralData.SetProperty("U_ST_SUB_CHANNEL", form.DataSources.DBDataSources.Item(0).GetValue("U_ST_SUB_CHANNEL", 0));
                        oGeneralData.SetProperty("U_ST_BROKER1", form.DataSources.DBDataSources.Item(0).GetValue("U_ST_BROKER1", 0));
                        oGeneralData.SetProperty("U_ST_BROKER2", form.DataSources.DBDataSources.Item(0).GetValue("U_ST_BROKER2", 0));
                        oGeneralData.SetProperty("U_ST_ACCOUNT_MANAGER", form.DataSources.DBDataSources.Item(0).GetValue("U_ST_ACCOUNT_MANAGER", 0));
                        oGeneralData.SetProperty("U_ST_CURRENCY", form.DataSources.DBDataSources.Item(0).GetValue("U_ST_CURRENCY", 0));
                        oGeneralData.SetProperty("U_ST_RESIDENCY", form.DataSources.DBDataSources.Item(0).GetValue("U_ST_RESIDENCY", 0));
                        oGeneralData.SetProperty("U_ST_TEL2", form.DataSources.DBDataSources.Item(0).GetValue("U_ST_TEL2", 0));
                        oGeneralData.SetProperty("U_ST_EMAIL", BP.Email);
                        oGeneralData.SetProperty("U_ST_MOBILE", BP.Mobile);
                        oGeneralData.SetProperty("U_ST_TEL1", tel1);
                        oGeneralData.SetProperty("U_ST_PREFIX", form.DataSources.DBDataSources.Item(0).GetValue("U_ST_PREFIX", 0));
                        oGeneralData.SetProperty("U_ST_TITLE", form.DataSources.DBDataSources.Item(0).GetValue("U_ST_TITLE", 0));

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

                        SAPbobsCOM.GeneralDataCollection Address_Children = oGeneralData.Child("ST_CCI_INDIV_ADDR");
                        for (int J = 0; J < form.DataSources.DBDataSources.Item("@ST_CCI_INDIV_ADDR").Size; J++)
                        {
                            SAPbobsCOM.GeneralData oChild = Address_Children.Add();
                            foreach (var One_Add_Field in Address_Fields)
                            {
                                if (One_Add_Field.Is_Temp == true)
                                {
                                    continue;
                                }
                                oChild.SetProperty(One_Add_Field.Column_Name_In_DB, form.DataSources.DBDataSources.Item("@ST_CCI_INDIV_ADDR").GetValue(One_Add_Field.Column_Name_In_DB, J));
                            }
                        }
                        oGeneralService.Add(oGeneralData);
                    }
                }
                company.EndTransaction(BoWfTransOpt.wf_Commit);
                if (form.Mode != BoFormMode.fm_ADD_MODE)
                {
                    for (int i = 0; i < Codes.Count; i++)
                    {
                        UDO_Definition UDO_Info_Approve = Logic.Objects_Logic.All_UDO_Definition.FirstOrDefault(U => U.KHCF_Object == KHCF_Objects.CCI_Member_Card);
                        KHCF_Approval.Approve_MemberCard(company, Codes[i], "", UDO_Info_Approve);
                    }
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
                    SAPbouiCOM.Form form = SBO_Application.Forms.ActiveForm;
                    DataTable DT_Members = form.DataSources.DataTables.Item("MEMBERS");
                    DT_Members.Rows.Clear();
                   
                    string TableName = form.DataSources.DBDataSources.Item(0).TableName.Replace("@", "");

                    string Parent_ID = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_PARENT_ID", 0);

                    if (pVal.MenuUID == "1282")
                    {
                        form.DataSources.DBDataSources.Item(0).SetValue("U_ST_APPROVAL_STATUS", 0, "A");
                    }
                    string U_ST_APPROVAL_STATUS = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_APPROVAL_STATUS", 0);
                    string Code = form.DataSources.DBDataSources.Item(0).GetValue("Code", 0);

                    if (!string.IsNullOrEmpty(Parent_ID))
                    {
                        form.Items.Item("138").Enabled = false;
                        Grid grid = (Grid)form.Items.Item("138").Specific;
                        form.Items.Item("138").SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, -1, BoModeVisualBehavior.mvb_False);
                    }
                    else
                    {
                        form.Items.Item("138").Enabled = true;
                        Grid grid = (Grid)form.Items.Item("138").Specific;
                        form.Items.Item("138").SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, -1, BoModeVisualBehavior.mvb_True);
                    }
                    form.Items.Item("164").Visible = false;
                  
                    form.DataSources.UserDataSources.Item("185").Value = "";

                }
                if (pVal.MenuUID == "1282")
                {
                    Form_Obj.Set_Fields(SBO_Application.Forms.ActiveForm);
                }
                if (SBO_Application.Forms.ActiveForm.Mode == BoFormMode.fm_OK_MODE)
                {
                    SAPbouiCOM.Form form = SBO_Application.Forms.ActiveForm;
                    bool disable = false;
                    Grid membersGrid = (Grid)form.Items.Item("154").Specific;
                    int Count = membersGrid.Rows.Count;
                    for (int i = 0; i < Count; i++)
                    {
                        string isActive = membersGrid.DataTable.GetValue("U_ST_ACTIVE", i).ToString();
                        if (isActive == "Y")
                        {
                            disable = true;
                        }
                    }
                    //if (disable)
                    //{
                    //    ButtonCombo Btn_Cmb_Action = (ButtonCombo)form.Items.Item("163").Specific;
                    //    if (Btn_Cmb_Action.ValidValues.Count == 10)
                    //    {
                    //        Btn_Cmb_Action.ValidValues.Remove("M");
                    //    }
                    //}

                }
                else if (SBO_Application.Forms.ActiveForm.Mode == BoFormMode.fm_FIND_MODE && Form_Obj!=null)
                {
                    if (SBO_Application.Forms.ActiveForm.TypeEx == Form_Obj.Form_Type)
                    {
                        Form_Obj.UnSet_Mondatory_Fields_Color(SBO_Application.Forms.ActiveForm);
                    }
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
            SAPbouiCOM.Form form = SBO_Application.Forms.Item(businessObjectInfo.FormUID);
            if (form.Mode == BoFormMode.fm_ADD_MODE)
            {
                KHCF_BP BP = new KHCF_BP();
                BP.BP_Group = int.Parse(form.DataSources.DBDataSources.Item(0).GetValue("U_ST_CUSTOMER_GROUP", 0));
                BP.CardName = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_FULL_NAME_AR", 0);
            }

            bool Need_To_Approve = false;
            if (form.Mode == BoFormMode.fm_ADD_MODE)
            {
                string New_UDO_Code = Utility.Get_New_UDO_Code(company, Form_Obj.KHCF_Object);
                form.DataSources.DBDataSources.Item(0).SetValue("Code", 0, New_UDO_Code);
                form.DataSources.DBDataSources.Item(0).SetValue("U_ST_CREATOR", 0, company.UserName);
                form.DataSources.DBDataSources.Item(0).SetValue("U_ST_PARENT_TYPE", 0, "N");
                //Need_To_Approve = true;
            }
            else
            {
               // string Member_Card = form.DataSources.DBDataSources.Item(0).GetValue("Code", 0);
               // string sql_Member = $@"SELECT *  FROM ""@ST_INDIV_MEMBERSHIP""  T0 WHERE T0.""U_ST_MEMBER_CARD"" ='{Member_Card}'";
               // Recordset RC_Ship = Helper.Utility.Execute_Recordset_Query(company, sql_Member);
               // if (RC_Ship.RecordCount > 0)
               // { 
               // string SQL = $@"Select * from ""{Form_Obj.UDO_Database_Table_Name}"" where ""Code"" = '{Member_Card}'";
               // Recordset RC = Helper.Utility.Execute_Recordset_Query(company, SQL);

               ////string[] Fields = Configurations.Get_Individual_Card_Fields_For_Approval(company);
               //     string t1 = Fields[0]; string t2 = Fields[1];
               // foreach (string OneField in Fields)
               // {
               //     if (form.DataSources.DBDataSources.Item(0).GetValue(OneField, 0) != RC.Fields.Item(OneField).Value.ToString())
               //     {
               //         Need_To_Approve = true;
               //     }
               // }
               // }
            }

            if (Need_To_Approve)
            {
                form.DataSources.DBDataSources.Item(0).SetValue("U_ST_APPROVAL_STATUS", 0, "P");
            }

        }

        private static void Change_Items_Values_Cutomization(string ItemUID, string FormUID, bool BeforeAction, bool ItemChanged, string ColUID, int Row, bool OnLoad = false)
        {
            #region Names Fields

            string[] Arabic_Names_Items = new string[] { "9", "11", "13", "15" };

            if (Arabic_Names_Items.Contains(ItemUID))
            {
                SAPbouiCOM.Form form = SBO_Application.Forms.Item(FormUID);
                
                string FirstName = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_FIRST_NAME_AR", 0);
                string FatherName = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_FATHER_NAME_AR", 0);
                string MiddleName = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_MIDDLE_NAME_AR", 0);
                string SurName = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_SURNAME_AR", 0);
                string Full_Name = "";
                if (!Utility.Check_Text(FirstName) && !string.IsNullOrEmpty(FirstName))
                {
                    form.DataSources.DBDataSources.Item(0).SetValue("U_ST_FIRST_NAME_AR", 0,string.Empty);
                    form.DataSources.DBDataSources.Item(0).SetValue("U_ST_FULL_NAME_AR", 0, string.Empty);
                    form.Items.Item("9").Click();    
                    //throw new Custom_Exception("Only Arabic Letters are allowed.");
                }
                else if (!Utility.Check_Text(FatherName) && !string.IsNullOrEmpty(FatherName))
                {
                    form.DataSources.DBDataSources.Item(0).SetValue("U_ST_FATHER_NAME_AR", 0, string.Empty);
                    form.DataSources.DBDataSources.Item(0).SetValue("U_ST_FULL_NAME_AR", 0, string.Empty);
                    form.Items.Item("11").Click();
                }
                else if (!Utility.Check_Text(MiddleName) && !string.IsNullOrEmpty(MiddleName))
                {
                    form.DataSources.DBDataSources.Item(0).SetValue("U_ST_MIDDLE_NAME_AR", 0, string.Empty);
                    form.DataSources.DBDataSources.Item(0).SetValue("U_ST_FULL_NAME_AR", 0, string.Empty);
                    form.Items.Item("13").Click();
                }
                else if (!Utility.Check_Text(SurName) && !string.IsNullOrEmpty(SurName))
                {
                    form.DataSources.DBDataSources.Item(0).SetValue("U_ST_SURNAME_AR", 0, string.Empty);
                    form.DataSources.DBDataSources.Item(0).SetValue("U_ST_FULL_NAME_AR", 0, string.Empty);
                    form.Items.Item("15").Click();
                }
                else
                    Full_Name = Utility.Get_Full_Name(FirstName, FatherName, MiddleName, SurName);

                form.DataSources.DBDataSources.Item(0).SetValue("U_ST_FULL_NAME_AR", 0, Full_Name);
            }

            string[] English_Names_Items = new string[] { "21", "23", "25", "27" };

            if (English_Names_Items.Contains(ItemUID))
            {
                SAPbouiCOM.Form form = SBO_Application.Forms.Item(FormUID);
                string FirstName = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_FIRST_NAME_EN", 0);
                string FatherName = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_FATHER_NAME_EN", 0);
                string MiddleName = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_MIDDLE_NAME_EN", 0);
                string SurName = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_SURNAME_EN", 0);
                string Full_Name = "";
                if (Utility.Check_Text(FirstName) && !string.IsNullOrEmpty(FirstName))
                {
                    form.DataSources.DBDataSources.Item(0).SetValue("U_ST_FIRST_NAME_EN", 0, string.Empty);
                    form.DataSources.DBDataSources.Item(0).SetValue("U_ST_FULL_NAME_EN", 0, string.Empty);
                    form.Items.Item("21").Click();
                    //throw new Custom_Exception("Only English Letters are allowed.");
                }
                else if (Utility.Check_Text(FatherName) && !string.IsNullOrEmpty(FatherName))
                {
                    form.DataSources.DBDataSources.Item(0).SetValue("U_ST_FATHER_NAME_EN", 0, string.Empty);
                    form.DataSources.DBDataSources.Item(0).SetValue("U_ST_FULL_NAME_EN", 0, string.Empty);
                    form.Items.Item("23").Click();
                }
                else if (Utility.Check_Text(MiddleName) && !string.IsNullOrEmpty(MiddleName))
                {
                    form.DataSources.DBDataSources.Item(0).SetValue("U_ST_MIDDLE_NAME_EN", 0, string.Empty);
                    form.DataSources.DBDataSources.Item(0).SetValue("U_ST_FULL_NAME_EN", 0, string.Empty);
                    form.Items.Item("25").Click();
                }
                else if (Utility.Check_Text(SurName) && !string.IsNullOrEmpty(SurName))
                {
                    form.DataSources.DBDataSources.Item(0).SetValue("U_ST_SURNAME_EN", 0, string.Empty);
                    form.DataSources.DBDataSources.Item(0).SetValue("U_ST_FULL_NAME_EN", 0, string.Empty);
                    form.Items.Item("27").Click();
                }
                else
                    Full_Name = Utility.Get_Full_Name(FirstName, FatherName, MiddleName, SurName);

                form.DataSources.DBDataSources.Item(0).SetValue("U_ST_FULL_NAME_EN", 0, Full_Name);
            }

            if (ItemUID == "138")
            {
                string[] Names_Cols = new string[] { "ST_FIRST_NAME_AR", "ST_FATHER_NAME_AR", "ST_MIDDLE_NAME_AR", "ST_SURNAME_AR" };
                if (Names_Cols.Contains(ColUID))
                {
                    SAPbouiCOM.Form form = SBO_Application.Forms.Item(FormUID);
                    DataTable DT_Members = form.DataSources.DataTables.Item("MEMBERS");
                    Grid Grd_Members = (Grid)form.Items.Item("138").Specific;
                    int RowIndex = Grd_Members.GetDataTableRowIndex(Row);

                    string FirstName = DT_Members.GetValue("ST_FIRST_NAME_AR", RowIndex).ToString();
                    string FatherName = DT_Members.GetValue("ST_FATHER_NAME_AR", RowIndex).ToString();
                    string MiddleName = DT_Members.GetValue("ST_MIDDLE_NAME_AR", RowIndex).ToString();
                    string SurName = DT_Members.GetValue("ST_SURNAME_AR", RowIndex).ToString();
                    string Full_Name = "";
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
                    SAPbouiCOM.Form form = SBO_Application.Forms.Item(FormUID);
                    DataTable DT_Members = form.DataSources.DataTables.Item("MEMBERS");
                    Grid Grd_Members = (Grid)form.Items.Item("138").Specific;
                    int RowIndex = Grd_Members.GetDataTableRowIndex(Row);

                    string FirstName = DT_Members.GetValue("ST_FIRST_NAME_EN", RowIndex).ToString();
                    string FatherName = DT_Members.GetValue("ST_FATHER_NAME_EN", RowIndex).ToString();
                    string MiddleName = DT_Members.GetValue("ST_MIDDLE_NAME_EN", RowIndex).ToString();
                    string SurName = DT_Members.GetValue("ST_SURNAME_EN", RowIndex).ToString();
                    string Full_Name = "";
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



            if (ItemUID == "91")
            {
                SAPbouiCOM.Form form = SBO_Application.Forms.Item(FormUID);
                DataTable DT_Members = form.DataSources.DataTables.Item("MEMBERS");
                string Group_type = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_CUSTOMER_GROUP", 0);
                if (DT_Members.Rows.Count == 1 && string.IsNullOrEmpty(DT_Members.GetValue("ST_APPROVAL_STATUS", 0).ToString()))
                    return;
                for (int i = 0; i < DT_Members.Rows.Count; i++)
                {
                    DT_Members.SetValue("ST_CUSTOMER_GROUP", i,Group_type);
                }
             }

            if (Form_Obj.Get_Depends_Parent_Item_IDs_List().Contains(ItemUID))
            {
                SAPbouiCOM.Form form = SBO_Application.Forms.Item(FormUID);
                Form_Obj.Load_One_Depends_Parent_Item(form, ItemUID);
            }

            if (ItemUID == "156") // Residency
            {
                SAPbouiCOM.Form form = SBO_Application.Forms.Item(FormUID);
                string Residency = form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).GetValue("U_ST_RESIDENCY", 0);

                string SQL_Country = $@"SELECT T0.U_ST_TEL_COUNTRY_CODE FROM OCRY T0 WHERE T0.""Name"" = '{Residency}'";
                Recordset RC_Country = Helper.Utility.Execute_Recordset_Query(company, SQL_Country);

                string countryCode = RC_Country.Fields.Item("U_ST_TEL_COUNTRY_CODE").Value.ToString();
              
                form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).SetValue("U_ST_MOBILE", 0, $"{countryCode}");
                form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).SetValue("U_ST_TEL1", 0, $"{countryCode}");
            }
            #endregion

            #region Gender Prefix
            if (ItemUID == "51")
            {
                SAPbouiCOM.Form form = SBO_Application.Forms.Item(FormUID);
                string gender = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_GENDER", 0);
                if (!string.IsNullOrEmpty(gender))
                {
                    string SQL_Prefix = $@"SELECT T0.""Code"" AS ""Code"", T0.""Name"" AS ""Name"" 
                                           FROM ""@ST_PREFIX"" T0 WHERE T0.""U_ST_GENDER"" = '{gender}'";
                    Helper.Utility.Fill_One_ComboBoxBySQL(company, form, "181", SQL_Prefix, true);
                }
            }
            #endregion

            if (ItemUID == "145")
            {
                SAPbouiCOM.Form form = SBO_Application.Forms.Item(FormUID);
                Choose_Parent(form);
            }
        }

        private static bool Validate_Data(BusinessObjectInfo businessObjectInfo)
        {
            SAPbouiCOM.Form form = SBO_Application.Forms.Item(businessObjectInfo.FormUID);

            if (!Form_Obj.Validate_Data(form))
            {
                return false;
            }
            string Nationality = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_NATIONALITY", 0);
            string Email = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_EMAIL", 0);
            bool isEmail = Regex.IsMatch(Email, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase);

            if (Nationality == "Jordan")
            {
                string National_Id = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_NATIONAL_ID", 0);
                bool res = true;
                if ((string.IsNullOrEmpty(National_Id) || National_Id.Count() != 10))
                {
                    throw new Logic.Custom_Exception($"The National ID Field is not Filled or is not 10 Digits Long");
                }
                if (string.IsNullOrEmpty(form.DataSources.DBDataSources.Item(0).GetValue("U_ST_FATHER_NAME_AR", 0)))
                {
                    throw new Logic.Custom_Exception($"Arabic Father Name field is Required.");
                }
                if (string.IsNullOrEmpty(form.DataSources.DBDataSources.Item(0).GetValue("U_ST_MIDDLE_NAME_AR", 0)))
                {
                    throw new Logic.Custom_Exception($"Arabic Middle Name field is Required.");
                }
                //if (string.IsNullOrEmpty(form.DataSources.DBDataSources.Item(0).GetValue("U_ST_FATHER_NAME_EN", 0)))
                //{
                //    throw new Logic.Custom_Exception($"English Father Name field is Required.");
                //}
                //if (string.IsNullOrEmpty(form.DataSources.DBDataSources.Item(0).GetValue("U_ST_MIDDLE_NAME_EN", 0)))
                //{
                //    throw new Logic.Custom_Exception($"English Middle Name field is Required.");
                //}
                
                if (!res) return false;
            }
            if (Nationality != "Jordan")
            {
                string personalID = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_PERSONAL_ID", 0);
                string passport = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_PASSPORT_ID", 0);
                if( string.IsNullOrEmpty(personalID) && string.IsNullOrEmpty(passport))
                {
                    throw new Logic.Custom_Exception($"Personal ID or Passport is Required in case of NON Jordanians.");
                }
            }
            if (!isEmail && !string.IsNullOrEmpty(Email))
            {
                throw new Logic.Custom_Exception($"The Email is not in Correct Format");
            }
           
            if (form.Mode == BoFormMode.fm_UPDATE_MODE && form.DataSources.DBDataSources.Item(0).GetValue("U_ST_APPROVAL_STATUS", 0) != "P")
            {
                bool approvalResult = Member_Cards_UI.Process_Individual_Update_Approval(form, Form_Obj);
                if (!approvalResult) return false;
            }

            Member_Cards_UI.Check_Members(businessObjectInfo,"I");
            Member_Cards_UI.Check_Address(businessObjectInfo, "@ST_CCI_INDIV_ADDR");

            return true;
        }

        internal static void SBO_Application_ItemEvent(string FormUID, ref ItemEvent pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            //Code_value = SBO_Application.Forms.ActiveForm.TypeEx.Replace("ST", "Frm"); 
            if (Form_Obj == null || pVal.FormTypeEx != Form_Obj.Form_Type )
            {
                return;
            }
            try
            {
                
                if (!pVal.BeforeAction && pVal.ItemChanged)
                {
                    Change_Items_Values_Cutomization(pVal.ItemUID, pVal.FormUID, pVal.BeforeAction, pVal.ItemChanged, pVal.ColUID, pVal.Row);
                }
                if (pVal.ItemUID == "31" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    SAPbouiCOM.Form form = SBO_Application.Forms.Item(pVal.FormUID);
                    DateTime startTime = DateTime.Now;
                    Matrix Mat_Add = (Matrix)form.Items.Item("20").Specific;
                    //if (Mat_Add.Columns.Item("Country").ValidValues.Count == 0)
                    //{
                    //    Form_Obj.Fill_Address_ComboBox(Mat_Add);
                    //    // DateTime prev = Utility.Add_Time_Log("C", "New Fill Address", startTime);
                    //}
                    //string SQL = $@"SELECT T0.""Code"", T0.""Name"" FROM ""@ST_CITY_AREA""  T0";
                    //Recordset RC = (Recordset)company.GetBusinessObject(BoObjectTypes.BoRecordset);
                    //RC.DoQuery(SQL);
                    //if (Mat_Add.Columns.Item("City").ValidValues.Count == 0)
                    //{
                    //    for (int i = 0; i < RC.RecordCount; i++)
                    //    {
                    //        Mat_Add.Columns.Item("City").ValidValues.Add(RC.Fields.Item("Code").Value.ToString(), RC.Fields.Item("Name").Value.ToString());
                    //        RC.MoveNext();
                    //    }
                    //}
                    Member_Cards_UI.Add_Address_Row(pVal,"I");
                    if(form.Mode == BoFormMode.fm_OK_MODE)
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
                    SAPbouiCOM.Form form = SBO_Application.Forms.Item(pVal.FormUID);
                    DateTime startTime = DateTime.Now;
                    if (string.IsNullOrEmpty(form.DataSources.DBDataSources.Item(0).GetValue("U_ST_PARENT_ID", 0)))
                    {
                        Member_Cards_UI.Add_Member_Row(pVal);
                        if (form.Mode == BoFormMode.fm_OK_MODE)
                            form.Mode = BoFormMode.fm_UPDATE_MODE;
                    }
                    else
                        throw new Custom_Exception($"Error in Add Sub Members, Card [{form.DataSources.DBDataSources.Item(0).GetValue("Code", 0)}] has Parent ");
                }
                if (pVal.ItemUID == "140" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    SAPbouiCOM.Form form = SBO_Application.Forms.Item(pVal.FormUID);
                    Member_Cards_UI.Remove_Member_Row(pVal);
                    if (form.Mode == BoFormMode.fm_OK_MODE)
                        form.Mode = BoFormMode.fm_UPDATE_MODE;
                }
                if (pVal.ItemUID == "159" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Approve(pVal,null);
                    SBO_Application.Menus.Item("1304").Activate();
                }
                if (pVal.ItemUID == "160" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Reject(pVal);
                }
                if (pVal.ItemUID == "163" && (pVal.EventType == BoEventTypes.et_ITEM_PRESSED || pVal.EventType == BoEventTypes.et_COMBO_SELECT) && !pVal.BeforeAction)
                {
                    Run_Actions(pVal);
                }
                if (pVal.ItemUID == "164" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Confirm_Link(pVal);
                }
                if (pVal.ItemUID == "170" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Member_Cards_UI.Add_Communication_Log(pVal,"I");
                }
                if (pVal.ItemUID == "Item_2" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Remove_Communication_Row(pVal);
                }
                if ((pVal.ItemUID == "52"|| pVal.ItemUID == "156") && pVal.EventType == BoEventTypes.et_CHOOSE_FROM_LIST && !pVal.BeforeAction)
                {
                    SAPbouiCOM.Form form = SBO_Application.Forms.Item(pVal.FormUID);
                    DateTime startTime = DateTime.Now;
                   
                    ChooseFromListEvent Chos_Event = (ChooseFromListEvent)pVal;
                    if ((Chos_Event.SelectedObjects == null ? false : Chos_Event.SelectedObjects.Rows.Count != 0))
                    {
                        string Name = Chos_Event.SelectedObjects.GetValue("Name", 0).ToString();
                       
                        if (pVal.ItemUID == "52")
                        {
                            form.DataSources.DBDataSources.Item(0).SetValue("U_ST_NATIONALITY", 0, Name);
                            if (Name == "Jordan")
                            {
                                form.Items.Item("11").BackColor = Color.Pink.R | (Color.Pink.G << 8) | (Color.Pink.B << 16); // Arabic Father Name
                                form.Items.Item("13").BackColor = Color.Pink.R | (Color.Pink.G << 8) | (Color.Pink.B << 16); // Arabic Middle Name
                                //form.Items.Item("23").BackColor = Color.Pink.R | (Color.Pink.G << 8) | (Color.Pink.B << 16); // English Father Name
                                //form.Items.Item("25").BackColor = Color.Pink.R | (Color.Pink.G << 8) | (Color.Pink.B << 16); // English Middle Name
                                form.Items.Item("33").BackColor = Color.Pink.R | (Color.Pink.G << 8) | (Color.Pink.B << 16); // National ID
                            }
                            else
                            {
                                form.Items.Item("11").BackColor =  Color.FromKnownColor(KnownColor.White).ToArgb();
                                form.Items.Item("13").BackColor = Color.FromKnownColor(KnownColor.White).ToArgb();
                                //form.Items.Item("23").BackColor = Color.FromKnownColor(KnownColor.White).ToArgb();
                                //form.Items.Item("25").BackColor = Color.FromKnownColor(KnownColor.White).ToArgb();
                                form.Items.Item("33").BackColor = Color.FromKnownColor(KnownColor.White).ToArgb();
                            }
                        }
                        else
                        {
                            form.DataSources.DBDataSources.Item(0).SetValue("U_ST_RESIDENCY", 0, Name);
                            string Residency = form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).GetValue("U_ST_RESIDENCY", 0);

                            string SQL_Country = $@"SELECT T0.U_ST_TEL_COUNTRY_CODE FROM OCRY T0 WHERE T0.""Name"" = '{Residency}'";
                            Recordset RC_Country = Helper.Utility.Execute_Recordset_Query(company, SQL_Country);

                            string countryCode = RC_Country.Fields.Item("U_ST_TEL_COUNTRY_CODE").Value.ToString();
                            form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).SetValue("U_ST_MOBILE", 0, $"{countryCode}");
                            form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).SetValue("U_ST_TEL1", 0, $"{countryCode}");
                        }
                        if (form.Mode == BoFormMode.fm_OK_MODE)
                        {
                            form.Mode = BoFormMode.fm_UPDATE_MODE;
                        }
                    }
                }
        
                if (pVal.ItemUID == "143" && pVal.EventType == BoEventTypes.et_COMBO_SELECT && !pVal.BeforeAction)
                {
                    Select_Parent_Type(pVal);
                }
                if (pVal.ItemUID == "145" && pVal.EventType == BoEventTypes.et_FORMAT_SEARCH_COMPLETED && !pVal.BeforeAction)
                {
                    SAPbouiCOM.Form form = SBO_Application.Forms.Item(pVal.FormUID);
                    Choose_Parent(form);
                    string code = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_BP_Code", 0);
                }
                if (pVal.ItemUID == "20" && pVal.ColUID == "Country" && pVal.EventType == BoEventTypes.et_CHOOSE_FROM_LIST && !pVal.BeforeAction)
                {
                    Choose_From_List_Country_Matrix(pVal);
                }
                if (pVal.ItemUID == "131" && pVal.EventType == BoEventTypes.et_CHOOSE_FROM_LIST && !pVal.BeforeAction)
                {
                    Choose_Broker(pVal,1);
                }
                if (pVal.ItemUID == "135" && pVal.EventType == BoEventTypes.et_CHOOSE_FROM_LIST && !pVal.BeforeAction)
                {
                    Choose_Broker(pVal,2);
                }

                if (pVal.ItemUID == "138" && (pVal.ColUID == "ST_NATIONALITY" || pVal.ColUID == "ST_RESIDENCY") && pVal.EventType == BoEventTypes.et_CHOOSE_FROM_LIST && !pVal.BeforeAction)
                {
                    Choose_From_List_Country_Grid(pVal);
                }
                if (pVal.ItemUID == "502" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Member_Cards_UI.Add_Attachment(pVal,"I");
                    SAPbouiCOM.Form form = SBO_Application.Forms.Item(pVal.FormUID);
                    if (form.Mode == BoFormMode.fm_OK_MODE)
                        form.Mode = BoFormMode.fm_UPDATE_MODE;
                }
                if (pVal.ItemUID == "503" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Member_Cards_UI.Remove_Attachment(pVal);
                    SAPbouiCOM.Form form = SBO_Application.Forms.Item(pVal.FormUID);
                    if (form.Mode == BoFormMode.fm_OK_MODE)
                        form.Mode = BoFormMode.fm_UPDATE_MODE;
                }
                if (pVal.ItemUID == "504" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Member_Cards_UI.Open_Attachment(pVal);
                }
            }
            catch (Exception ex)
            {
                if (!string.IsNullOrEmpty(pVal.FormUID) && pVal.EventType != BoEventTypes.et_FORM_UNLOAD)
                {
                    SAPbouiCOM.Form form = SBO_Application.Forms.Item(pVal.FormUID);
                    form.Freeze(false);
                    SBO_Application.StatusBar.SetText(ex.Message, BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Error);
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

        private static void Remove_Communication_Row(SAPbouiCOM.ItemEvent pVal)
        {
            SAPbouiCOM.Form form = Loader.SBO_Application.Forms.Item(pVal.FormUID);
            SAPbouiCOM.DataTable DT_Members = form.DataSources.DataTables.Item("Communication_Log");

            for (int i = 0; i < DT_Members.Rows.Count; i++)
            {
                if (DT_Members.GetValue("SELECTED", i).ToString() != "Y")
                {
                    continue;
                }
                string Log_Code = DT_Members.GetValue("Code", i).ToString();
                if (Log_Code != "")
                {
                    UDO_Definition UDF_Lof_Info = Logic.Objects_Logic.All_UDO_Definition.FirstOrDefault(U => U.KHCF_Object == KHCF_Objects.Communication_Log);
                    KHCF_Logic_Utility.Remove_UDO_Entry(company, Log_Code, UDF_Lof_Info);
                    //kh Remove_UDO_Entry
                }
                DT_Members.Rows.Remove(i);
            }
            //   SBO_Application.Menus.Item("1304").Activate();
        }

        private static void Choose_Parent_ID(ItemEvent pVal)
        {
            string Code = Utility.Chosse_From_List_For_Code_And_DBDataSource(pVal, pVal.ItemUID);
            //return;
            SAPbouiCOM.Form form = SBO_Application.Forms.Item(pVal.FormUID);
            UDO_Definition UDO_Info = null;
            string Name_Field = "";
            if (form.DataSources.DBDataSources.Item(0).GetValue("U_ST_PARENT_TYPE", 0) == "C")
            {
                UDO_Info = Logic.Objects_Logic.All_UDO_Definition.FirstOrDefault(U => U.KHCF_Object == KHCF_Objects.CCI_Corporate_Member_Card);
                Name_Field = "U_ST_CORPORATE_ARABIC_NAME";
            }
            else if (form.DataSources.DBDataSources.Item(0).GetValue("U_ST_PARENT_TYPE", 0) == "I")
            {
                UDO_Info = Logic.Objects_Logic.All_UDO_Definition.FirstOrDefault(U => U.KHCF_Object == KHCF_Objects.CCI_Member_Card);
                Name_Field = "U_ST_FULL_NAME_AR";
            }
            else 
            {               
                throw new Custom_Exception("Please Select Parent Type");
            }
            string SQL = $@"SELECT {Name_Field} ,""U_ST_CUSTOMER_GROUP"" from ""@{UDO_Info.Table_Name}"" WHERE ""Code"" = '{Code}'";
            Recordset RC = Helper.Utility.Execute_Recordset_Query(company, SQL);
            string parent_Name = RC.Fields.Item(Name_Field).Value.ToString();
            string Customer_Group = RC.Fields.Item("U_ST_CUSTOMER_GROUP").Value.ToString();
            form.DataSources.DBDataSources.Item(0).SetValue("U_ST_PARENT_NAME", 0, parent_Name);
            form.DataSources.DBDataSources.Item(0).SetValue("U_ST_PARENT_ID", 0, Code);
            form.DataSources.DBDataSources.Item(0).SetValue("U_ST_CUSTOMER_GROUP", 0, Customer_Group);
            string code = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_BP_Code", 0);
            if (form.Mode == BoFormMode.fm_OK_MODE)
                form.Mode = BoFormMode.fm_UPDATE_MODE;


        }

        private static void Choose_Parent(Form form)
        {
            //string Code = Utility.Chosse_From_List_For_Code_And_DBDataSource(pVal, pVal.ItemUID);
            //return;
            //  SAPbouiCOM.Form form = SBO_Application.Forms.Item(pVal.FormUID);
            string Code = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_PARENT_ID", 0);
            UDO_Definition UDO_Info = null;
            string Name_Field = "";
            if (form.DataSources.DBDataSources.Item(0).GetValue("U_ST_PARENT_TYPE", 0) == "C")
            {
                UDO_Info = Logic.Objects_Logic.All_UDO_Definition.FirstOrDefault(U => U.KHCF_Object == KHCF_Objects.CCI_Corporate_Member_Card);
                Name_Field = "U_ST_CORPORATE_ARABIC_NAME";
            }
            else if (form.DataSources.DBDataSources.Item(0).GetValue("U_ST_PARENT_TYPE", 0) == "I")
            {
                UDO_Info = Logic.Objects_Logic.All_UDO_Definition.FirstOrDefault(U => U.KHCF_Object == KHCF_Objects.CCI_Member_Card);
                Name_Field = "U_ST_FULL_NAME_AR";
            }
            else
            {
                throw new Custom_Exception("Please Select Parent Type");
            }
            string SQL = $@"SELECT {Name_Field} ,""U_ST_CUSTOMER_GROUP"" from ""@{UDO_Info.Table_Name}"" WHERE ""Code"" = '{Code}'";
            Recordset RC = Helper.Utility.Execute_Recordset_Query(company, SQL);
            string parent_Name = RC.Fields.Item(Name_Field).Value.ToString();
            string Customer_Group = RC.Fields.Item("U_ST_CUSTOMER_GROUP").Value.ToString();
            form.DataSources.DBDataSources.Item(0).SetValue("U_ST_PARENT_NAME", 0, parent_Name);
            form.DataSources.DBDataSources.Item(0).SetValue("U_ST_PARENT_ID", 0, Code);
            form.DataSources.DBDataSources.Item(0).SetValue("U_ST_CUSTOMER_GROUP", 0, Customer_Group);
            string code = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_BP_Code", 0);
            if (form.Mode == BoFormMode.fm_OK_MODE)
                form.Mode = BoFormMode.fm_UPDATE_MODE;


        }

        private static void Choose_Parent_ID_By_BP(ItemEvent pVal)
        {
            
            //return;
            SAPbouiCOM.Form form = SBO_Application.Forms.Item(pVal.FormUID);
            ChooseFromListEvent Chos_Event = (ChooseFromListEvent)pVal;
            string Code = "";
            if ((Chos_Event.SelectedObjects == null ? false : Chos_Event.SelectedObjects.Rows.Count != 0))
            {
                string c = Chos_Event.SelectedObjects.GetValue("CardCode", 0).ToString();
                Code = c;// form.DataSources.UserDataSources.Item("33").Value;
            }
            UDO_Definition UDO_Info = null;
            string Name_Field = "";
            if (form.DataSources.DBDataSources.Item(0).GetValue("U_ST_PARENT_TYPE", 0) == "C")
            {
                UDO_Info = Logic.Objects_Logic.All_UDO_Definition.FirstOrDefault(U => U.KHCF_Object == KHCF_Objects.CCI_Corporate_Member_Card);
                Name_Field = "U_ST_CORPORATE_ARABIC_NAME";
            }
            else if (form.DataSources.DBDataSources.Item(0).GetValue("U_ST_PARENT_TYPE", 0) == "I")
            {
                UDO_Info = Logic.Objects_Logic.All_UDO_Definition.FirstOrDefault(U => U.KHCF_Object == KHCF_Objects.CCI_Member_Card);
                Name_Field = "U_ST_FULL_NAME_AR";
            }
            else 
            {
                throw new Custom_Exception("Please Select Parent Type");
            }
            string SQL_BP = $@"SELECT ""Code"" from ""@{UDO_Info.Table_Name}"" WHERE ""U_ST_BP_CODE"" = '{Code}'";
            Recordset RC_BP = Helper.Utility.Execute_Recordset_Query(company, SQL_BP);
            string Card_Num = RC_BP.Fields.Item("Code").Value.ToString();
            string SQL = $@"SELECT ""{Name_Field}"" ,""U_ST_CUSTOMER_GROUP"" from ""@{UDO_Info.Table_Name}"" WHERE ""Code"" = '{Card_Num}'";
            Recordset RC = Helper.Utility.Execute_Recordset_Query(company, SQL);
            string parent_Name = RC.Fields.Item(Name_Field).Value.ToString();
            string Customer_Group = RC.Fields.Item("U_ST_CUSTOMER_GROUP").Value.ToString();
            form.DataSources.DBDataSources.Item(0).SetValue("U_ST_PARENT_NAME", 0, parent_Name);
            form.DataSources.DBDataSources.Item(0).SetValue("U_ST_PARENT_ID", 0, Card_Num);
            form.DataSources.DBDataSources.Item(0).SetValue("U_ST_CUSTOMER_GROUP", 0, Customer_Group);
            string code = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_BP_Code", 0);

            if (form.Mode == BoFormMode.fm_OK_MODE)
                form.Mode = BoFormMode.fm_UPDATE_MODE;
        }

        private static void Choose_Broker(ItemEvent pVal,int Borker_Number)
        {
            SAPbouiCOM.Form form = SBO_Application.Forms.Item(pVal.FormUID);
            ChooseFromListEvent Chos_Event = (ChooseFromListEvent)pVal;
            string Code = "";
            if ((Chos_Event.SelectedObjects == null ? false : Chos_Event.SelectedObjects.Rows.Count != 0))
            {
                 Code = Chos_Event.SelectedObjects.GetValue("CardCode", 0).ToString();
                string SQL_Name = $@"Select T0.""CardName"" From OCRD T0 Where T0.""CardCode""='{Code}'";
                Recordset RC_Broker = Helper.Utility.Execute_Recordset_Query(company, SQL_Name);
                if (Borker_Number==1)
                 form.DataSources.DBDataSources.Item(0).SetValue("U_ST_BROKER1", 0, RC_Broker.Fields.Item("CardName").Value.ToString());
                else
                  form.DataSources.DBDataSources.Item(0).SetValue("U_ST_BROKER2", 0, RC_Broker.Fields.Item("CardName").Value.ToString());
            }
         

        }

        private static void Select_Parent_Type(ItemEvent pVal)
        {
            SAPbouiCOM.Form form = SBO_Application.Forms.Item(pVal.FormUID);
            Set_Parent_Link(form);
        }

        private static void Confirm_Link(ItemEvent pVal)
        {
            SAPbouiCOM.Form form = SBO_Application.Forms.Item(pVal.FormUID);
            string UDO_Code = form.DataSources.DBDataSources.Item(0).GetValue("Code", 0);
            string Sql_Members = $@"select * from ""@ST_CCI_INDIV_CARD"" T0 WHERE T0.""U_ST_PARENT_ID""='{UDO_Code}' AND T0.""U_ST_PARENT_TYPE"" = 'I' ";
            Recordset RC_Mem = Helper.Utility.Execute_Recordset_Query(company, Sql_Members);
            if (RC_Mem.RecordCount > 0)
            {
                SBO_Application.StatusBar.SetText($"The member already has sub members. You cannot link him/her with a parent", BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Error);
            }
            else 
            {
                UDO_Definition UDO_Info = Logic.Objects_Logic.All_UDO_Definition.FirstOrDefault(U => U.KHCF_Object == KHCF_Objects.CCI_Member_Card);
                string Parent_Type = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_PARENT_TYPE", 0);
                string Parent_ID = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_PARENT_ID", 0);
                string Parent_Name = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_PARENT_NAME", 0);

                Logic.KHCF_Logic_Utility.Link(company, UDO_Code, Parent_Type, Parent_ID, Parent_Name, UDO_Info);
                // SBO_Application.Menus.Item("1304").Activate();
                SBO_Application.StatusBar.SetText("Done!", BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Success);
            }
           
        }

        private static void Run_Actions(ItemEvent pVal)
        {
            SAPbouiCOM.Form form = SBO_Application.Forms.Item(pVal.FormUID);
            if (form.Mode != BoFormMode.fm_OK_MODE)
            {
                throw new Logic.Custom_Exception("This action can only run in OK mode.");
            }
            //ButtonCombo Btn_Cmb_Warrning = (ButtonCombo)form.Items.Item("163").Specific;
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

            if (Title != "create the membership")
            {
                if (SBO_Application.MessageBox($"Are you sure you want to {Title} the Card?", 1, "Yes", "No") != 1)
                {
                    return;
                }
            }
            else if (SBO_Application.MessageBox($"Are you sure you want to {Title}?", 1, "Yes", "No") != 1)
            {
                return;
            }
            string UDO_Code = form.DataSources.DBDataSources.Item(0).GetValue("Code", 0);
            UDO_Definition UDO_Info = Logic.Objects_Logic.All_UDO_Definition.FirstOrDefault(U => U.KHCF_Object == KHCF_Objects.CCI_Member_Card);

            switch (Action_ID)
            {
                case "R"://Remove
                    string sql_Member = $@"SELECT T0.""Code""  FROM ""@ST_INDIV_MEMBERSHIP""  T0 WHERE T0.""U_ST_MEMBER_CARD"" ='{UDO_Code}'";
                    Recordset RC_Ship = Helper.Utility.Execute_Recordset_Query(company, sql_Member);
                    if (RC_Ship.RecordCount == 0)
                    {
                        KHCF_Logic_Utility.Remove_UDO_Entry(company, UDO_Code, UDO_Info);

                        string SQL_Parent2 = $@"Select T0.""Code"" from ""@ST_CCI_INDIV_CARD"" T0 Where T0.""U_ST_PARENT_ID""='{UDO_Code}' AND T0.""U_ST_PARENT_TYPE"" = 'I'";
                        Recordset rc_Parent = Helper.Utility.Execute_Recordset_Query(company, SQL_Parent2);
                        while (!rc_Parent.EoF)
                        {
                            string Child_Code = rc_Parent.Fields.Item("Code").Value.ToString();
                            KHCF_Logic_Utility.Remove_UDO_Entry(company, Child_Code, Form_Obj.UDO_Info);
                            rc_Parent.MoveNext();
                        }
                        SBO_Application.StatusBar.SetText($"The Card [{UDO_Code}] has been removed successfully!", BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Success);
                        form.Mode = BoFormMode.fm_FIND_MODE;
                        Form_Obj.UnSet_Mondatory_Fields_Color(form);
                    }
                    else
                        SBO_Application.StatusBar.SetText($"Card [{UDO_Code}] cannot be removed because there are memberships", BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Error);
                    break;
                case "L"://Link
                    if (form.DataSources.DBDataSources.Item(0).GetValue("U_ST_PARENT_TYPE", 0) != "N" && !string.IsNullOrEmpty(form.DataSources.DBDataSources.Item(0).GetValue("U_ST_PARENT_ID", 0)))
                    {
                        throw new Logic.Custom_Exception("You have to unlink the current parent first");
                    }
                    string SQL_Parent = $@"Select T0.""Code"" From ""@ST_CCI_INDIV_CARD"" T0 Where T0.""U_ST_PARENT_ID""='{UDO_Code}' AND T0.""U_ST_PARENT_TYPE"" = 'I'";
                    Recordset RC_Parent = Helper.Utility.Execute_Recordset_Query(company,SQL_Parent);
                    if (RC_Parent.RecordCount>0)
                    {
                        throw new Logic.Custom_Exception("You cannot unlink a parent from sub members on parent card. Please go to sub members to unlink");
                    }
                    form.Items.Item("143").SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, -1, BoModeVisualBehavior.mvb_True);

                    form.Items.Item("145").SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, -1, BoModeVisualBehavior.mvb_True);
                    SBO_Application.StatusBar.SetText("Please set the Parent ID", BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Warning);
                    form.DataSources.DBDataSources.Item(0).SetValue("U_ST_PARENT_TYPE", 0,"N");
                    Set_Parent_Link(form);
                    //form.Items.Item("Item_6").Click();
                    form.Items.Item("164").Visible = true;
                    break;
                case "U"://Unlink
                    string sql_Membership = $@"SELECT T0.""Code"" FROM ""@ST_INDIV_MEMBERSHIP"" T0 WHERE T0.""U_ST_MEMBERSHIP_STATUS"" IN ('N','R','P') 
AND CURRENT_DATE BETWEEN T0.""U_ST_START_DATE"" AND T0.""U_ST_END_DATE"" AND T0.""U_ST_MEMBER_CARD"" ='{UDO_Code}'";
                    Recordset RC_Mem_Ship = Helper.Utility.Execute_Recordset_Query(company, sql_Membership);
                    if (RC_Mem_Ship.RecordCount > 0)
                    {
                        throw new Custom_Exception("You cannot unlink the card due to active memberships.");
                    }
                    KHCF_Logic_Utility.Unlink(company, UDO_Code, form.DataSources.DBDataSources.Item(0).GetValue("U_ST_BP_Code", 0), UDO_Info);
                    SBO_Application.StatusBar.SetText($"The Card has been unlinked successfully!", BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Success);
                    break;
                case "M"://Create Membership
                    if (form.DataSources.DBDataSources.Item(0).GetValue("U_ST_APPROVAL_STATUS", 0).ToString() != "A")
                        throw new Custom_Exception("You cannot create membership. Card is not approved.");
                    Create_Membership(form);
                    break;
                case "P"://Create Patient
                    //string sql_Mem = $@"SELECT T0.""Code"" FROM ""@ST_INDIV_MEMBERSHIP""  T0 where T0.""U_ST_MEMBERSHIP_STATUS""<>'S' AND T0.""U_ST_MEMBER_CARD"" ='{UDO_Code}'";
                    //Recordset RC_Mem = Helper.Utility.Execute_Recordset_Query(company, sql_Mem);
                    //if (RC_Mem.RecordCount > 0)
                    //{
                    //    throw new Custom_Exception("Card cannot be converted to patient. Related memberships are active.");
                    //}
                    string sql_Mem = $@"SELECT COUNT(*) FROM ""@ST_INDIV_MEMBERSHIP""  T0 where T0.""U_ST_MEMBER_CARD"" ='{UDO_Code}'";
                    Recordset RC_Mem = Helper.Utility.Execute_Recordset_Query(company, sql_Mem);
                    if ((int)RC_Mem.Fields.Item(0).Value == 0)
                    {
                        throw new Custom_Exception("Card cannot be converted to Patient. There isn't any related Memberships.");
                    }
                    Create_Patient(form);
                    break;
                default:
                    throw new Logic.Custom_Exception($"This Report [{Action_ID}] is not supported");
            }
        }

        private static void Create_Patient(SAPbouiCOM.Form form)
        {
            string UDO_Code = form.DataSources.DBDataSources.Item(0).GetValue("Code", 0);

            SAPbouiCOM.Form Patient_Form = Loader.Open_UDO_Form(KHCF_Objects.Patients_Card);
            Patient_Form.Mode = BoFormMode.fm_ADD_MODE;
            Patient_Form.DataSources.DBDataSources.Item(0).SetValue("U_ST_MEMBER_CARD", 0, UDO_Code);
          Frm_Patients_Card.Form_Obj.Set_Fields(Patient_Form);
            Frm_Patients_Card.Select_MemberCard(Patient_Form, UDO_Code);
        }

        private static void Stop_Membership(SAPbouiCOM.Form form, UDO_Definition UDO_Info, bool All_Childs, bool Renewal_Parent)
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
            if (Renewal_Parent)
            {
                string Parent_Code = form.DataSources.DBDataSources.Item(0).GetValue("Code", 0);
                All_Cards += $"{Parent_Code},";
               // Membership.Stop_MemberCard(company, Parent_Code, UDO_Membership_Info);
            }
            foreach (string OneCard in Childs_Cards)
            {
                All_Cards += $"{OneCard},";
                // Membership.Stop_MemberCard(company, OneCard, UDO_Membership_Info);
            }

            Frm_Set_Stop_Card_Data.Create_Form(All_Cards, "");
        }

        private static void Create_Membership(SAPbouiCOM.Form form)
        {
            string UDO_Code = form.DataSources.DBDataSources.Item(0).GetValue("Code", 0);

            Frm_Individual_Membership.Create_Membership_for_MemberCard(UDO_Code);
        }

        private static void Reject(ItemEvent pVal)
        {
            SAPbouiCOM.Form form = SBO_Application.Forms.Item(pVal.FormUID);
            string UDO_Code = form.DataSources.DBDataSources.Item(0).GetValue("Code", 0);
            string Approval_Note = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_APPROVAL_NOTE", 0);
            string Creator_Code = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_CREATOR", 0);
            string Parent_ID = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_PARENT_ID", 0);

            if (string.IsNullOrEmpty(Approval_Note))
            {
                throw new Custom_Exception($@"Please Fill Reject Reason");
            }
            if (!string.IsNullOrEmpty(Parent_ID)) // check Parent Approval Status
            {
                string SQL_Status_Parent = $@"Select T0.""U_ST_APPROVAL_STATUS"" from ""@ST_CCI_INDIV_CARD"" T0 Where T0.""Code"" = '{Parent_ID}'";
                Recordset RC_Status_Parent = Helper.Utility.Execute_Recordset_Query(company, SQL_Status_Parent);
                if (RC_Status_Parent.RecordCount > 0)
                {
                    string Approval_Status = RC_Status_Parent.Fields.Item("U_ST_APPROVAL_STATUS").Value.ToString();
                    if (Approval_Status != "A")
                        throw new Custom_Exception($@"You cannot Reject Sub member [{UDO_Code}] where the parent is pending");
                }
            }


            Logic.KHCF_Approval.Reject_MemberCard(company, UDO_Code, Approval_Note, Form_Obj.UDO_Info);
            Form_Obj.Send_Alert_To_Creator(UDO_Code, "Reject", Creator_Code);

           // Check Children
            string SQL_Parent = $@"Select T0.""Code"",T0.""U_ST_APPROVAL_STATUS"",T0.""U_ST_CREATOR"" from ""@ST_CCI_INDIV_CARD"" T0 
Where T0.""U_ST_PARENT_ID""='{UDO_Code}' AND T0.""U_ST_PARENT_TYPE"" = 'I' And T0.""U_ST_APPROVAL_STATUS"" = 'P' ";
            Recordset rc_Parent = Helper.Utility.Execute_Recordset_Query(company, SQL_Parent);
            while (!rc_Parent.EoF)
            {
                string Child_Code = rc_Parent.Fields.Item("Code").Value.ToString();
                string Creator = rc_Parent.Fields.Item("U_ST_CREATOR").Value.ToString();
                Logic.KHCF_Approval.Reject_MemberCard(company, Child_Code, $"Rejected From Parent [{UDO_Code}]", Form_Obj.UDO_Info);
                Form_Obj.Send_Alert_To_Creator(Child_Code, "Reject", Creator);
                rc_Parent.MoveNext();
            }

            //SBO_Application.Menus.Item("1304").Activate();
            SBO_Application.StatusBar.SetText("Operation completed successfully!", BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Success);
            SBO_Application.Menus.Item("1304").Activate();
        }

        private static void Approve(ItemEvent pVal, BusinessObjectInfo businessObjectInfo)
        {
            SAPbouiCOM.Form form = null;
            if (businessObjectInfo == null)
            {
                form = SBO_Application.Forms.Item(pVal.FormUID);
            }
            else
            {
                form = SBO_Application.Forms.Item(businessObjectInfo.FormUID);
            }
          
            string UDO_Code = form.DataSources.DBDataSources.Item(0).GetValue("Code", 0);
            string Approval_Note = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_APPROVAL_NOTE", 0);
            string Creator_Code = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_CREATOR", 0);
            string Parent_ID = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_PARENT_ID", 0);
            if (!string.IsNullOrEmpty(Parent_ID)) // check Parent Approval Status
            {
                string SQL_Status_Parent = $@"Select T0.""U_ST_APPROVAL_STATUS"" from ""@ST_CCI_INDIV_CARD"" T0 Where T0.""Code"" = '{Parent_ID}'";
                Recordset RC_Status_Parent = Helper.Utility.Execute_Recordset_Query(company, SQL_Status_Parent);
                if (RC_Status_Parent.RecordCount > 0)
                {
                    string Approval_Status = RC_Status_Parent.Fields.Item("U_ST_APPROVAL_STATUS").Value.ToString();
                    if (Approval_Status != "A")
                        throw new Custom_Exception($@"Sub-member [{UDO_Code}] cannot be approved because the parent level card is not approved yet.");
                }
            }
            Logic.KHCF_Approval.Approve_MemberCard(company, UDO_Code, Approval_Note, Form_Obj.UDO_Info);
            if (businessObjectInfo == null)
                Form_Obj.Send_Alert_To_Creator(UDO_Code, "Approve", Creator_Code);
            
            //Check Children
            string SQL_Parent = $@"Select T0.""Code"",T0.""U_ST_APPROVAL_STATUS"",T0.""U_ST_CREATOR"" from ""@ST_CCI_INDIV_CARD"" T0 
Where T0.""U_ST_PARENT_ID""='{UDO_Code}' AND T0.""U_ST_PARENT_TYPE"" = 'I' And T0.""U_ST_APPROVAL_STATUS"" = 'P' ";
            Recordset rc_Parent = Helper.Utility.Execute_Recordset_Query(company,SQL_Parent);
            while (!rc_Parent.EoF)
            {
                string Child_Code = rc_Parent.Fields.Item("Code").Value.ToString();
                string Creator = rc_Parent.Fields.Item("U_ST_CREATOR").Value.ToString();
                Logic.KHCF_Approval.Approve_MemberCard(company, Child_Code, $"Approved From Parent [{UDO_Code}]", Form_Obj.UDO_Info);
                if (businessObjectInfo == null)
                    Form_Obj.Send_Alert_To_Creator(Child_Code, "Approve", Creator);
                rc_Parent.MoveNext();
            }
            
            SBO_Application.StatusBar.SetText("Operation completed successfully!", BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Success);
            
        }
        
    }
}
