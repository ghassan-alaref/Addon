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

namespace ST.KHCF.Customization.Forms.Patient
{
    internal class Frm_Social_Study : Parent_Form
    {
        internal static SAPbobsCOM.Company company;
        internal static Application SBO_Application;
        internal static Parent_Form Form_Obj;

        //internal override string[] Get_Mondatory_Fields_List()
        //{
        //    List<string> Result = new List<string>();
        //    Result.AddRange(base.Get_Mondatory_Fields_List());
        //    Result.AddRange(new string[] { "13", "17"});
        //    return Result.ToArray();
        //}

        //internal override string[] Get_Approval_Items_List()
        //{
        //    List<string> Result = new List<string>();
        //    Result.AddRange(base.Get_Approval_Items_List());
        //    // Result.AddRange(new string[] { "159", "160", "161", "162" });

        //    return Result.ToArray();
        //}

        internal override Depends_List[] Get_Depends_List_List()
        {
            List<Depends_List> Result = new List<Depends_List>();
            Result.AddRange(base.Get_Depends_List_List());
            //Result.Add(new Depends_List() { Item_ID = "133", Parent_Item_ID = "47", SQL = $@"SELECT T0.""Code"", T0.""Name"" FROM ""@ST_SUB_CHANNEL""  T0 WHERE T0.""U_ST_CHANNEL"" = '{{0}}'" });
            //Result.Add(new Depends_List() { Item_ID = "133", Parent_Item_ID = "131", SQL = $@"SELECT T0.""Code"", T0.""Name"" FROM ""@ST_SUB_CHANNEL""  T0 WHERE T0.""U_ST_CHANNEL"" = '002'" });
            return Result.ToArray();
        }

        //internal override string[] Get_Tab_Item_List()
        //{
        //    List<string> Result = new List<string>();
        //    Result.AddRange(base.Get_Tab_Item_List());
        //    Result.Add("3");
        //    Result.Add("4");
        //    Result.Add("93");
        //    Result.Add("127");  3,4,93,127
        //    //Result.Add("85");
        //    //Result.Add("86");

        //    return Result.ToArray();
        //}


        //internal override string[] Get_Fix_ReadOnly_Fields_List()
        //{
        //    List<string> Result = new List<string>();
        //    Result.AddRange(base.Get_Fix_ReadOnly_Fields_List());
        //    Result.AddRange(new string[] { "5", "136",  "7" }); "5,136,7"

        //    return Result.ToArray();
        //}

        internal override void Initialize_Form(Form form)
        {
            //Code_value = "Frm_Social_Study";
            //Desc_value = "Mandatary fields List For Social Study ";
            //Man_fields =  "13,17";

            //File.AppendAllText("C:\\Users\\wsaleh\\Desktop\\Patient_TimeLog.txt", "Patient Social Study" + Environment.NewLine);
            //string startTimeExact = DateTime.Now.ToString("mm:ss:f");
            //DateTime startTime = DateTime.Now;
            //File.AppendAllText("C:\\Users\\wsaleh\\Desktop\\Patient_TimeLog.txt", "Start Time:" + startTimeExact + Environment.NewLine);
            base.Initialize_Form(form);
            //DateTime prev = Utility.Add_Time_Log("P", "base.Initialize Form", startTime);
            Matrix Mat_Add = (Matrix)form.Items.Item("94").Specific;
            base.Fill_Address_ComboBox(Mat_Add);
            //prev = Utility.Add_Time_Log("P", "Address", prev);
            Matrix Mat_Att = (Matrix)form.Items.Item("144").Specific;
            base.Fill_Attachment_ComboBox(Mat_Att);

            ButtonCombo Btn_Cmb_Warrning = (ButtonCombo)form.Items.Item("156").Specific;
            Btn_Cmb_Warrning.ValidValues.Add("-", "You Can Also");
            Btn_Cmb_Warrning.ValidValues.Add("R", "Remove");
            form.DataSources.UserDataSources.Item("156").Value = "-";
            form.Items.Item("156").AffectsFormMode = false;


            form.Items.Item("152").SetAutoManagedAttribute(BoAutoManagedAttr.ama_Visible, -1, BoModeVisualBehavior.mvb_False);
            form.Items.Item("152").SetAutoManagedAttribute(BoAutoManagedAttr.ama_Visible, 4, BoModeVisualBehavior.mvb_True);
            form.Items.Item("153").SetAutoManagedAttribute(BoAutoManagedAttr.ama_Visible, -1, BoModeVisualBehavior.mvb_False);
            form.Items.Item("153").SetAutoManagedAttribute(BoAutoManagedAttr.ama_Visible, 4, BoModeVisualBehavior.mvb_True);
            form.Items.Item("154").SetAutoManagedAttribute(BoAutoManagedAttr.ama_Visible, -1, BoModeVisualBehavior.mvb_False);
            form.Items.Item("154").SetAutoManagedAttribute(BoAutoManagedAttr.ama_Visible, 4, BoModeVisualBehavior.mvb_True);

            Matrix Mat_Prev_Supp = (Matrix)form.Items.Item("173").Specific;
            string SQL_Prev_Supp = $@"SELECT T0.""Code"", T0.""Name"" FROM ""@ST_SUPPORT_TYPE"" T0";
            //Recordset RC_Prev_Supp = (Recordset)company.GetBusinessObject(BoObjectTypes.BoRecordset);
            //RC_Prev_Supp.DoQuery(SQL_Prev_Supp);
            Helper.Utility.FillMatrixComboBoxForSQL(company, Mat_Prev_Supp, "TYPE", SQL_Prev_Supp, true);

            form.Items.Item("170").TextStyle = 4;
            form.Items.Item("128").TextStyle = 4;
            form.Items.Item("130").TextStyle = 4;

            form.Items.Item("3").Click();
            //prev = Utility.Add_Time_Log("P", "", startTime, true);
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


                Form form = Loader.SBO_Application.Forms.Item(BusinessObjectInfo.FormUID);
                //string TableName = form.DataSources.DBDataSources.Item(0).TableName.Replace("@", "");
                //UDO_Definition KHCF_Object = Objects_Logic.All_UDO_Definition.FirstOrDefault(O => O.Table_Name == TableName);
                SBO_Application.StatusBar.SetText("Loading", BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_None);
                if (Form_Obj.Set_ReadOnly(form, Form_Obj.UDO_Info))
                {
                    // return;
                }

            }
            catch (Exception ex)
            {
                //SBO_Application.                SBO_Application.StatusBar.SetText(ex.Message, BoMessageTime.bmt_Short,  BoStatusBarMessageType.smt_Error);(ex.Message, BoMessageTime.bmt_Short, true);
                Loader.New_Msg = ex.Message;
                if (!BusinessObjectInfo.BeforeAction)
                {
                    Form form = Loader.SBO_Application.Forms.Item(BusinessObjectInfo.FormUID);
                    form.Mode = BoFormMode.fm_UPDATE_MODE;
                }
                BubbleEvent = false;
            }

            return BubbleEvent;
        }

        private static void Form_Data_Load(BusinessObjectInfo businessObjectInfo)
        {
            Form form = SBO_Application.Forms.Item(businessObjectInfo.FormUID);
            string Card_ID = form.DataSources.DBDataSources.Item(0).GetValue("Code", 0);

            Form_Obj.Load_Depends_Items(form);

            //Load_Sub_Members(form, Card_ID);

            //Load_Memberships(form, Card_ID);
            //Load_Communication_Log(form, Card_ID);


            //Set_Parent_Link(form);
            //form.Items.Item("164").Visible = false;

            //  Check_Approval_Editable(form, Card_ID);

            Change_Items_Values_Cutomization("17", businessObjectInfo.FormUID, false, true, "", 0, true);
            Change_Items_Values_Cutomization("51", businessObjectInfo.FormUID, false, true, "", 0, true);
            Change_Items_Values_Cutomization("52", businessObjectInfo.FormUID, false, true, "", 0, true);
            Change_Items_Values_Cutomization("125", businessObjectInfo.FormUID, false, true, "", 0, true);
            Change_Items_Values_Cutomization("91", businessObjectInfo.FormUID, false, true, "", 0, true);

        }




        private static void ADD_Update_UDO(BusinessObjectInfo businessObjectInfo)
        {
            Form form = SBO_Application.Forms.Item(businessObjectInfo.FormUID);
            System.Xml.XmlDocument XML_Doc = new System.Xml.XmlDocument();
            string XML_Text = businessObjectInfo.ObjectKey.Replace(" ", "").Replace("=", " = ");
            XML_Doc.LoadXml(XML_Text);

            string UDO_Code = XML_Doc.GetElementsByTagName("Code")[0].InnerText;
            //string UDO_Name = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_FULL_NAME_AR", 0);


            //if (form.DataSources.DBDataSources.Item(0).GetValue("U_ST_APPROVAL_STATUS", 0) == "P")
            //{
            //    Form_Obj.Send_Alert_For_Approve(UDO_Code);
            //}
            //else
            //{
            //    string BP_Code = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_BP_CODE", 0);
            //    UDO_Definition UDO_Info = Logic.Objects_Logic.All_UDO_Definition.FirstOrDefault(U => U.KHCF_Object == KHCF_Objects.CCI_Member_Card);
            //    Utility.Update_BP(company, BP_Code, UDO_Code, UDO_Info);
            //}

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
                if (form.TypeEx != Form_Obj.Form_Type)
                {
                    return;
                }
                //int CCI_Patient_Vendor_Group = Configurations.Get_CCI_Patient_Vendor_Group(company);
                //form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).SetValue("U_ST_PATIENT_VENDOR_GROUP_MSH", 0, CCI_Patient_Vendor_Group.ToString());
                //int CCI_Other_Patient_Vendor_Group = Configurations.Get_Other_CCI_Patient_Vendor_Group(company);
                //form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).SetValue("U_ST_PATIENT_VENDOR_GROUP_CCI", 0, CCI_Other_Patient_Vendor_Group.ToString());
                //int CCI_Goodwill = Configurations.Get_Other_Goodwill_Vendor_Group(company);
                //form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).SetValue("U_ST_PATIENT_VENDOR_GROUP_GW", 0, CCI_Goodwill.ToString());

                //string TableName = form.DataSources.DBDataSources.Item(0).TableName.Replace("@", "");
                //UDO_Definition KHCF_Object = Objects_Logic.All_UDO_Definition.FirstOrDefault(O => O.Table_Name == TableName);
                if (Form_Obj.Set_ReadOnly(form, Form_Obj.UDO_Info))
                {
                    // return;
                }
                Change_Items_Values_Cutomization("17", form.UniqueID, false, true, "", 0, true);
                Change_Items_Values_Cutomization("51", form.UniqueID, false, true, "", 0, true);
                Change_Items_Values_Cutomization("52", form.UniqueID, false, true, "", 0, true);
                Change_Items_Values_Cutomization("125", form.UniqueID, false, true, "", 0, true);
                Change_Items_Values_Cutomization("91", form.UniqueID, false, true, "", 0, true);

            }
            if (SBO_Application.Forms.ActiveForm.Mode != BoFormMode.fm_FIND_MODE)
            {
                Form_Obj.Set_Fields(SBO_Application.Forms.ActiveForm);
            }
        }
        private static void Before_Adding_UDO(BusinessObjectInfo businessObjectInfo)
        {
            Form form = SBO_Application.Forms.Item(businessObjectInfo.FormUID);
            if (form.Mode == BoFormMode.fm_ADD_MODE)
            {
                KHCF_BP BP = new KHCF_BP();
                //BP.BP_Group = int.Parse(form.DataSources.DBDataSources.Item(0).GetValue("U_ST_CUSTOMER_GROUP", 0));
                //BP.CardName = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_FULL_NAME_AR", 0);
                // string BP_Code = Utility.Create_BP(company, BP);

                Set_Default_Value_Before_Adding(form);
            }


        }

        private static void Set_Default_Value_Before_Adding(Form form)
        {
            // form.DataSources.DBDataSources.Item(0).SetValue("U_ST_BP_CODE", 0, BP_Code);
            if (form.Mode == BoFormMode.fm_ADD_MODE)
            {
                string New_UDO_Code = Utility.Get_New_UDO_Code(company, Form_Obj.KHCF_Object);
                form.DataSources.DBDataSources.Item(0).SetValue("Code", 0, New_UDO_Code);
                form.DataSources.DBDataSources.Item(0).SetValue("U_ST_CREATOR", 0, company.UserName);
                // form.DataSources.DBDataSources.Item(0).SetValue("U_ST_PARENT_TYPE", 0, "N");
            }

        }

        private static void Change_Items_Values_Cutomization(string ItemUID, string FormUID, bool BeforeAction, bool ItemChanged, string ColUID, int Row, bool OnLoad = false)
        {
            #region Names Fields


            #endregion


            if (Form_Obj.Get_Depends_Parent_Item_IDs_List().Contains(ItemUID))
            {
                Form form = SBO_Application.Forms.Item(FormUID);
                Form_Obj.Load_One_Depends_Parent_Item(form, ItemUID);
            }

            if (ItemUID == "17")
            {
                Form form = SBO_Application.Forms.Item(FormUID);
                string Status = form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).GetValue("U_ST_STATUS", 0);
                if (Status == "C")
                {
                    form.Items.Item("15").BackColor = Color.Pink.R | (Color.Pink.G << 8) | (Color.Pink.B << 16);
                }
                else
                {
                    form.Items.Item("15").BackColor = Color.FromKnownColor(KnownColor.White).ToArgb();
                }
            }

            if (ItemUID == "51")
            {
                Form form = SBO_Application.Forms.Item(FormUID);
                string Value = form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).GetValue("U_ST_RESIDENCE_OWNERSHIP", 0);
                if (Value == "007")
                {
                    form.Items.Item("152").SetAutoManagedAttribute(BoAutoManagedAttr.ama_Visible, -1, BoModeVisualBehavior.mvb_True);
                }
                else
                {
                    form.Items.Item("152").SetAutoManagedAttribute(BoAutoManagedAttr.ama_Visible, -1, BoModeVisualBehavior.mvb_False);
                    form.Items.Item("152").SetAutoManagedAttribute(BoAutoManagedAttr.ama_Visible, 4, BoModeVisualBehavior.mvb_True);
                }
            }
            if (ItemUID == "52")
            {
                Form form = SBO_Application.Forms.Item(FormUID);
                string Value = form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).GetValue("U_ST_RESIDENCE_TYPE", 0);
                if (Value == "006")
                {
                    form.Items.Item("153").SetAutoManagedAttribute(BoAutoManagedAttr.ama_Visible, -1, BoModeVisualBehavior.mvb_True);
                }
                else
                {
                    form.Items.Item("153").SetAutoManagedAttribute(BoAutoManagedAttr.ama_Visible, -1, BoModeVisualBehavior.mvb_False);
                    form.Items.Item("153").SetAutoManagedAttribute(BoAutoManagedAttr.ama_Visible, 4, BoModeVisualBehavior.mvb_True);
                }
            }

            if (ItemUID == "125")
            {
                Form form = SBO_Application.Forms.Item(FormUID);
                string Value = form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).GetValue("U_ST_INCOME_SOURCE", 0);
                if (Value == "005")
                {
                    form.Items.Item("154").SetAutoManagedAttribute(BoAutoManagedAttr.ama_Visible, -1, BoModeVisualBehavior.mvb_True);
                }
                else
                {
                    form.Items.Item("154").SetAutoManagedAttribute(BoAutoManagedAttr.ama_Visible, -1, BoModeVisualBehavior.mvb_False);
                    form.Items.Item("154").SetAutoManagedAttribute(BoAutoManagedAttr.ama_Visible, 4, BoModeVisualBehavior.mvb_True);
                }
            }
            if (ItemUID == "91")
            {
                Form form = SBO_Application.Forms.Item(FormUID);
                string Value = form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).GetValue("U_ST_PREVIOUS_COVERAGE_PARTY", 0);
                if (Value == "007")
                {
                    form.Items.Item("155").SetAutoManagedAttribute(BoAutoManagedAttr.ama_Visible, -1, BoModeVisualBehavior.mvb_True);
                }
                else
                {
                    form.Items.Item("155").SetAutoManagedAttribute(BoAutoManagedAttr.ama_Visible, -1, BoModeVisualBehavior.mvb_False);
                    form.Items.Item("155").SetAutoManagedAttribute(BoAutoManagedAttr.ama_Visible, 4, BoModeVisualBehavior.mvb_True);
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
            string New_Status = form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).GetValue("U_ST_STATUS", 0);
            if (New_Status == "C" || New_Status == "R")
            {
                string Code = form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).GetValue("Code", 0);
                string Old_Status = "";
                if (Code != "")
                {
                    string SQL_Old_Status = $@"SELECT T0.U_ST_STATUS FROM ""@ST_SOCIAL_STUDY""  T0 WHERE T0.""Code"" = '{Code}'";
                    Recordset RC_Old_Status = Helper.Utility.Execute_Recordset_Query(company, SQL_Old_Status);
                    Old_Status = RC_Old_Status.Fields.Item("U_ST_STATUS").Value.ToString();
                }
                if (Old_Status != New_Status)
                {
                    string SQL_Auth = $@"SELECT T0.U_ST_CAN_COMPLETE_PATIENT_SOCIAL_STUDY, T0.U_ST_CAN_REJECT_PATIENT_SOCIAL_STUDY 
FROM OUSR T0 WHERE T0.""USER_CODE"" = '{company.UserName}'";
                    Recordset RC_Auth = Helper.Utility.Execute_Recordset_Query(company, SQL_Auth);
                    string Complete_Auth = RC_Auth.Fields.Item("U_ST_CAN_COMPLETE_PATIENT_SOCIAL_STUDY").Value.ToString();
                    string Rej_Auth = RC_Auth.Fields.Item("U_ST_CAN_REJECT_PATIENT_SOCIAL_STUDY").Value.ToString();
                    if (New_Status == "C" && Complete_Auth != "Y")
                    {
                        throw new Logic.Custom_Exception("The User don't have Authorization to complete the Social Study");
                    }
                    if (New_Status == "R" && Rej_Auth != "Y")
                    {
                        throw new Logic.Custom_Exception("The User don't have Authorization to reject the Social Study");
                    }
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
                if (pVal.ItemUID == "117" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Add_Address_Row(pVal);
                }
                if (pVal.ItemUID == "118" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Remove_Address_Row(pVal);
                }
                if (pVal.ItemUID == "134" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Add_Special_needs_Row(pVal);
                }
                if (pVal.ItemUID == "135" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Remove_Special_needs_Row(pVal);
                }
                if (pVal.ItemUID == "132" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Add_Health_Issue_Row(pVal);
                }
                if (pVal.ItemUID == "133" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Remove_Health_Issue_Row(pVal);
                }
                if (pVal.ItemUID == "172" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Add_Previous_Support_Row(pVal);
                }
                if (pVal.ItemUID == "171" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Remove_Previous_Support_Row(pVal);
                }
                if (pVal.ItemUID == "9" && pVal.EventType == BoEventTypes.et_CHOOSE_FROM_LIST && !pVal.BeforeAction)
                {
                    Choose_From_List_Patient(pVal);
                }
                if (pVal.ItemUID == "11" && pVal.EventType == BoEventTypes.et_CHOOSE_FROM_LIST && !pVal.BeforeAction)
                {
                    Choose_From_List_MemberCard(pVal);
                }

                if (!pVal.BeforeAction && pVal.ItemChanged)
                {
                    Change_Items_Values_Cutomization(pVal.ItemUID, pVal.FormUID, pVal.BeforeAction, pVal.ItemChanged, pVal.ColUID, pVal.Row);
                }
                if (pVal.ItemUID == "141" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Add_Attachment(pVal);
                }
                if (pVal.ItemUID == "142" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Remove_Attachment(pVal);
                }
                if (pVal.ItemUID == "143" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Open_Attachment(pVal);
                }
                if (pVal.ItemUID == "94" && pVal.ColUID == "Country" && pVal.EventType == BoEventTypes.et_CHOOSE_FROM_LIST && !pVal.BeforeAction)
                {
                    Choose_From_List_Country_Matrix(pVal);
                }
                if (pVal.ItemUID == "156" && (pVal.EventType == BoEventTypes.et_ITEM_PRESSED || pVal.EventType == BoEventTypes.et_COMBO_SELECT) && !pVal.BeforeAction)
                {
                    Run_Action(pVal);
                }

            }
            catch (Exception ex)
            {
                Form form = SBO_Application.Forms.Item(pVal.FormUID);
                form.Freeze(false);
                SBO_Application.StatusBar.SetText(ex.Message, BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Error);
            }
        }

        private static void Run_Action(ItemEvent pVal)
        {
            SAPbouiCOM.Form form = SBO_Application.Forms.Item(pVal.FormUID);
            if (form.Mode != BoFormMode.fm_OK_MODE)
            {
                throw new Logic.Custom_Exception("This action can only run in OK mode.");
            }
            //ButtonCombo Btn_Cmb_Warrning = (ButtonCombo)form.Items.Item("163").Specific;
            string Title = "";
            string Action_ID = form.DataSources.UserDataSources.Item("156").Value;
            if (Action_ID == "-")
                throw new Logic.Custom_Exception("Please select the action");
            else
            {
                Title = Utility.Get_Field_Configuration(company, form.TypeEx.Replace("ST", "Frm") + "_" + Action_ID, "", "");
            }

            if (Title == "" || string.IsNullOrEmpty(Title))
                throw new Logic.Custom_Exception($"The Action [{Action_ID}] is not supported");

            if (SBO_Application.MessageBox($"Are you sure you want to {Title}?", 1, "Yes", "No") != 1)
            {
                return;
            }
            string UDO_Code = form.DataSources.DBDataSources.Item(0).GetValue("Code", 0);
            UDO_Definition UDO_Info = Logic.Objects_Logic.All_UDO_Definition.FirstOrDefault(U => U.KHCF_Object == KHCF_Objects.Social_Study);

            switch (Action_ID)
            {
                case "R"://Remove
                    string SQL_User = $@"Select U_ST_CAN_REMOVE_SOCIAL_STUDY from OUSR WHERE USER_CODE = '{company.UserName}'";
                    Recordset RC_User = Helper.Utility.Execute_Recordset_Query(company, SQL_User);
                    if (RC_User.Fields.Item("U_ST_CAN_REMOVE_SOCIAL_STUDY").Value.ToString() != "Y")
                    {
                        throw new Logic.Custom_Exception("The User don't have authorization to remove the Social Study");
                    }
                    KHCF_Logic_Utility.Remove_UDO_Entry(company, UDO_Code, UDO_Info, false);
                    form.Mode = BoFormMode.fm_FIND_MODE;
                    break;    
                default:
                    throw new Logic.Custom_Exception($"This Report [{Action_ID}] is not supported");
            }
            SBO_Application.StatusBar.SetText("Done!", BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Success);

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

        private static void Add_Attachment(ItemEvent pVal)
        {
            Form form = SBO_Application.Forms.Item(pVal.FormUID);

            ST.Helper.Dialog.BrowseFile BF = new Helper.Dialog.BrowseFile(Helper.Dialog.DialogType.OpenFile);
            // BF.Filter = "CSV Files (*.csv)|*.csv";

            BF.ShowDialog();
            if (BF.FileName == "")
            {
                return;
            }
            form.Freeze(true);
            DBDataSource DS_Attachment = form.DataSources.DBDataSources.Item("@ST_SOCIAL_STUDY_ATT");
            Matrix Mat_Add = (Matrix)form.Items.Item("144").Specific;
            Mat_Add.FlushToDataSource();
            int Count = DS_Attachment.Size;
            if (Count == 1)
            {
                if (DS_Attachment.GetValue("U_ST_FILE_NAME", Count - 1) != "")
                {
                    DS_Attachment.InsertRecord(Count);
                }
                else
                {
                    Count = 0;

                    Mat_Add.LoadFromDataSource();
                }
            }
            else
            {
                DS_Attachment.InsertRecord(Count);
            }

            DS_Attachment.SetValue("U_ST_FILE_NAME", Count, BF.FileName);
            DS_Attachment.SetValue("LineId", Count, "-1");

            //DS_Address.SetValue("U_ST_COUNTRY", Count, "JO");

            Mat_Add.LoadFromDataSource();
            Mat_Add.AutoResizeColumns();
            form.Freeze(false);

        }

        private static void Remove_Attachment(ItemEvent pVal)
        {
            Form form = SBO_Application.Forms.Item(pVal.FormUID);

            Form_Obj.Remove_Matrix_Row(form, "144");

        }
        private static void Open_Attachment(ItemEvent pVal)
        {
            Form form = SBO_Application.Forms.Item(pVal.FormUID);

            Matrix Mat = (Matrix)form.Items.Item("144").Specific;
            for (int i = 0; i < Mat.RowCount; i++)
            {
                CheckBox Chk_Selected = (CheckBox)Mat.GetCellSpecific("SELECTED", i + 1);
                if (Chk_Selected.Checked)
                {
                    EditText Txt_FileName = (EditText)Mat.GetCellSpecific("FileName", i + 1);
                    System.Diagnostics.Process.Start(Txt_FileName.Value);
                }
            }


        }
        private static void Choose_From_List_Patient(ItemEvent pVal)
        {
            Form form = SBO_Application.Forms.Item(pVal.FormUID);
            string Code = Utility.Chosse_From_List_For_Code_And_DBDataSource(pVal, pVal.ItemUID, false, Form_Obj.UDO_Database_Table_Name);

            Select_Patient_Code(form, Code);
        }

        internal static void Select_Patient_Code(Form form, string Patient_Code)
        {
            string SQL_MemberCard = $@"SELECT T0.U_ST_MEMBER_CARD FROM ""@ST_PATIENTS_CARD""  T0 WHERE T0.""Code"" = '{Patient_Code}'";
            Recordset RC_MemberCard = Helper.Utility.Execute_Recordset_Query(company, SQL_MemberCard);

            form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).SetValue("U_ST_MEMBER_CARD", 0, RC_MemberCard.Fields.Item("U_ST_MEMBER_CARD").Value.ToString());

            Field_Definition[] Add_Fields = Logic.Fields_Logic.All_Field_Definition.Where(F => F.KHCF_Object == KHCF_Objects.Social_Study_Address).ToArray();
            DBDataSource DB_Address = form.DataSources.DBDataSources.Item("@ST_SOCIAL_STUDY_ADD");
            string SQL_Add = $@"SELECT *  FROM ""@ST_PATIENTS_ADDRESS""  T0 WHERE T0.""Code"" ='{Patient_Code}'";
            Recordset RC_Add = Helper.Utility.Execute_Recordset_Query(company, SQL_Add);
            int Address_Index = 0;
            for (int J = 0; J < RC_Add.RecordCount; J++)
            {
                if (DB_Address.GetValue("U_ST_ADDRESS_NAME", Address_Index) != "")
                {
                    DB_Address.InsertRecord(Address_Index);
                    Address_Index++;
                }
                foreach (Field_Definition One_Field in Add_Fields)
                {
                    DB_Address.SetValue(One_Field.Column_Name_In_DB, Address_Index, RC_Add.Fields.Item(One_Field.Column_Name_In_DB).Value.ToString());
                }

                RC_Add.MoveNext();
            }

           ((Matrix)form.Items.Item("94").Specific).LoadFromDataSource();


        }

        private static void Choose_From_List_MemberCard(ItemEvent pVal)
        {
            Form form = SBO_Application.Forms.Item(pVal.FormUID);
            string Code = Utility.Chosse_From_List_For_Code_And_DBDataSource(pVal, pVal.ItemUID, false, Form_Obj.UDO_Database_Table_Name);

        }

        private static void Remove_Address_Row(ItemEvent pVal)
        {
            Form form = SBO_Application.Forms.Item(pVal.FormUID);
            Matrix Mat = (Matrix)form.Items.Item("94").Specific;
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

        }

        private static void Add_Address_Row(ItemEvent pVal)
        {
            Form form = SBO_Application.Forms.Item(pVal.FormUID);
            form.Freeze(true);
            DBDataSource DS_Address = form.DataSources.DBDataSources.Item("@ST_SOCIAL_STUDY_ADD");
            Matrix Mat_Add = (Matrix)form.Items.Item("94").Specific;
            Mat_Add.FlushToDataSource();
            int Count = DS_Address.Size;
            if (Count == 1)
            {
                if (DS_Address.GetValue("U_ST_ADDRESS_NAME", Count - 1) != "")
                {
                    DS_Address.InsertRecord(Count);
                }
                else
                {
                    Count = 0;

                    Mat_Add.LoadFromDataSource();
                }
            }
            else
            {
                DS_Address.InsertRecord(Count);
            }

            DS_Address.SetValue("U_ST_ADDRESS_TYPE", Count, "S");
            //DS_Address.SetValue("U_ST_COUNTRY", Count, "JO");

            Mat_Add.LoadFromDataSource();
            form.Freeze(false);

        }

        private static void Remove_Special_needs_Row(ItemEvent pVal)
        {
            Form form = SBO_Application.Forms.Item(pVal.FormUID);
            Matrix Mat = (Matrix)form.Items.Item("129").Specific;
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

        }

        private static void Add_Special_needs_Row(ItemEvent pVal)
        {
            Form form = SBO_Application.Forms.Item(pVal.FormUID);
            form.Freeze(true);
            DBDataSource DS_Address = form.DataSources.DBDataSources.Item("@ST_SOCIAL_SPEC_NED");
            Matrix Mat_Add = (Matrix)form.Items.Item("129").Specific;
            Mat_Add.FlushToDataSource();
            int Count = DS_Address.Size;
            if (Count == 1)
            {
                if (DS_Address.GetValue("U_ST_AGE", Count - 1) != "")
                {
                    DS_Address.InsertRecord(Count);
                }
                else
                {
                    Count = 0;

                    DS_Address.InsertRecord(Count);
                    Mat_Add.LoadFromDataSource();
                    Mat_Add.DeleteRow(1);
                    Mat_Add.FlushToDataSource();
                    Mat_Add.LoadFromDataSource();
                }
            }
            else
            {
                DS_Address.InsertRecord(Count);
            }


            Mat_Add.LoadFromDataSource();
            form.Freeze(false);

        }
        private static void Remove_Health_Issue_Row(ItemEvent pVal)
        {
            Form form = SBO_Application.Forms.Item(pVal.FormUID);
            Matrix Mat = (Matrix)form.Items.Item("131").Specific;
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

        }

        private static void Add_Health_Issue_Row(ItemEvent pVal)
        {
            Form form = SBO_Application.Forms.Item(pVal.FormUID);
            form.Freeze(true);
            DBDataSource DS_Address = form.DataSources.DBDataSources.Item("@ST_SOCIAL_HLTH_ISU");
            Matrix Mat_Add = (Matrix)form.Items.Item("131").Specific;
            Mat_Add.FlushToDataSource();
            int Count = DS_Address.Size;
            if (Count == 1)
            {
                if (DS_Address.GetValue("U_ST_AGE", Count - 1) != "")
                {
                    DS_Address.InsertRecord(Count);
                }
                else
                {
                    Count = 0;
                    DS_Address.InsertRecord(Count);
                    Mat_Add.LoadFromDataSource();
                    Mat_Add.DeleteRow(1);
                    Mat_Add.FlushToDataSource();

                    Mat_Add.LoadFromDataSource();
                }
            }
            else
            {
                DS_Address.InsertRecord(Count);
            }


            Mat_Add.LoadFromDataSource();
            form.Freeze(false);

        }


        private static void Remove_Previous_Support_Row(ItemEvent pVal)
        {
            Form form = SBO_Application.Forms.Item(pVal.FormUID);
            Matrix Mat = (Matrix)form.Items.Item("173").Specific;
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

        }

        private static void Add_Previous_Support_Row(ItemEvent pVal)
        {
            Form form = SBO_Application.Forms.Item(pVal.FormUID);
            form.Freeze(true);
            //DBDataSource DS_Address = form.DataSources.DBDataSources.Item("@@ST_SOCIAL_PREV_SUP");
            Matrix Mat_Add = (Matrix)form.Items.Item("173").Specific;
            //Mat_Add.FlushToDataSource();
            //int Count = DS_Address.Size;
            Mat_Add.AddRow();
            if (form.Mode == BoFormMode.fm_OK_MODE)
            {
                form.Mode = BoFormMode.fm_UPDATE_MODE;
            }

            //if (Count == 1)
            //{
            //    if (DS_Address.GetValue("U_ST_TYPE_OF_PREVIOUS_SUPPORT_FROM_KHCF", Count - 1) != "")
            //    {
            //        DS_Address.InsertRecord(Count);
            //    }
            //    else
            //    {
            //        Count = 0;
            //        DS_Address.InsertRecord(Count);
            //        Mat_Add.LoadFromDataSource();
            //        Mat_Add.DeleteRow(1);
            //        Mat_Add.FlushToDataSource();

            //        Mat_Add.LoadFromDataSource();
            //    }
            //}
            //else
            //{
            //    DS_Address.InsertRecord(Count);
            //}


            //Mat_Add.LoadFromDataSource();
            form.Freeze(false);

        }

    }
}
