using SAPbobsCOM;
using SAPbouiCOM;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ST.KHCF.Customization.Forms.Fundraising
{
    internal class Frm_Goodwill_Funds : Parent_Form
    {
        internal static SAPbobsCOM.Company company;
        internal static Application SBO_Application;
        internal static Parent_Form Form_Obj;
        //internal override string[] Get_Mondatory_Fields_List()
        //{
        //    List<string> Result = new List<string>();
        //    Result.AddRange(base.Get_Mondatory_Fields_List());
        //    Result.AddRange(new string[] { "14","15","16" });   "14,15,16"
        //    return Result.ToArray();
        //}

        //internal override string[] Get_Fix_ReadOnly_Fields_List()
        //{
        //    List<string> Result = new List<string>();
        //    Result.AddRange(base.Get_Fix_ReadOnly_Fields_List());
        //    Result.AddRange(new string[] { "5" });

        //    return Result.ToArray();
        //}

        internal override void Initialize_Form(Form form)
        {
       
            base.Initialize_Form(form);
            form.Items.Item("Item_1").Click();

            form.Items.Item("1000").SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 4, BoModeVisualBehavior.mvb_False);
            //form.Items.Item("1000").SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 2, BoModeVisualBehavior.mvb_False);

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
                    //ADD_Update_UDO(BusinessObjectInfo);
                }
                if (BusinessObjectInfo.EventType == BoEventTypes.et_FORM_DATA_LOAD && !BusinessObjectInfo.BeforeAction)
                {
                    Form_Data_Load(BusinessObjectInfo);
                }
                Form form = SBO_Application.Forms.Item(BusinessObjectInfo.FormUID);
                if (form.Mode == BoFormMode.fm_FIND_MODE)
                {
                    Form_Obj.UnSet_Mondatory_Fields_Color(form);
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
        }


        private static void Form_Data_Load(BusinessObjectInfo businessObjectInfo)
        {
            Form form = SBO_Application.Forms.Item(businessObjectInfo.FormUID);
            string UDO_Code = form.DataSources.DBDataSources.Item(0).GetValue("Code", 0);

            Form_Obj.Set_Fields(form);

        }

        private static void Before_Adding_UDO(BusinessObjectInfo businessObjectInfo)
        {
            Form form = SBO_Application.Forms.Item(businessObjectInfo.FormUID);
            if (form.Mode == BoFormMode.fm_ADD_MODE)
            {
                Set_Default_Value_Before_Adding(form);
            }
        }

        private static void Set_Default_Value_Before_Adding(Form form)
        {
            if (form.Mode == BoFormMode.fm_ADD_MODE)
            {
                string New_UDO_Code = Utility.Get_New_UDO_Code(company, Form_Obj.KHCF_Object);
                form.DataSources.DBDataSources.Item(0).SetValue("Code", 0, New_UDO_Code);
            }

        }

        private static bool Validate_Data(BusinessObjectInfo businessObjectInfo)
        {
            Form form = SBO_Application.Forms.Item(businessObjectInfo.FormUID);

            if (!Form_Obj.Validate_Data(form))
            {
                return false;
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
                if (pVal.ItemUID == "67" && pVal.EventType == BoEventTypes.et_CHOOSE_FROM_LIST && !pVal.BeforeAction)
                {
                    Choose_Rule_ID(pVal);
                }
                if (pVal.ItemUID == "68" && pVal.EventType == BoEventTypes.et_CHOOSE_FROM_LIST && !pVal.BeforeAction)
                {
                    Choose_Target_ID(pVal);
                }
                if (pVal.ItemUID == "126" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Add_Attachment(pVal);
                }
                if (pVal.ItemUID == "66" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Remove_Attachment(pVal);
                }
                if (pVal.ItemUID == "64" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Open_Attachment(pVal);
                }
                if (pVal.ItemUID == "1000" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Logic.KHCF_Logic_Utility.Select_Allocation(pVal, Form_Obj, "68");
                }

            }
            catch (Exception ex)
            {
                Form form = SBO_Application.Forms.Item(pVal.FormUID);
                form.Freeze(false);
                SBO_Application.StatusBar.SetText(ex.Message, BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Error);
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
                string UDO_Code = form.DataSources.DBDataSources.Item(0).GetValue("Code", 0);
            }
            if (SBO_Application.Forms.ActiveForm.Mode != BoFormMode.fm_FIND_MODE)
            {
                Form_Obj.Set_Fields(SBO_Application.Forms.ActiveForm);
            }
            else
            {
                Form_Obj.UnSet_Mondatory_Fields_Color(SBO_Application.Forms.ActiveForm);
            }

        }
        private static void Choose_Rule_ID(ItemEvent pVal)
        {
            SAPbouiCOM.Form form = SBO_Application.Forms.Item(pVal.FormUID);
            ChooseFromListEvent Chos_Event = (ChooseFromListEvent)pVal;
            string Code = "";
            if ((Chos_Event.SelectedObjects == null ? false : Chos_Event.SelectedObjects.Rows.Count != 0))
            {
                Code = Chos_Event.SelectedObjects.GetValue("Code", 0).ToString();
                form.DataSources.DBDataSources.Item(0).SetValue("U_ST_FUND_RULE", 0, Code);
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
                form.DataSources.DBDataSources.Item(0).SetValue("U_ST_FUND_TARGET", 0, Code);
            }
        }
        private static void Add_Attachment(ItemEvent pVal)
        {
            Form form = SBO_Application.Forms.Item(pVal.FormUID);
            ST.Helper.Dialog.BrowseFile BF = new Helper.Dialog.BrowseFile(Helper.Dialog.DialogType.OpenFile);
            BF.ShowDialog();
            if (BF.FileName == "")
            {
                return;
            }
            form.Freeze(true);
            DBDataSource DS_Attachment = form.DataSources.DBDataSources.Item("@ST_GOOD_WILL_ATT");
            Matrix Mat_Add = (Matrix)form.Items.Item("65").Specific;
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
            Mat_Add.LoadFromDataSource();
            Mat_Add.AutoResizeColumns();
            form.Freeze(false);
        }
        private static void Remove_Attachment(ItemEvent pVal)
        {
            Form form = SBO_Application.Forms.Item(pVal.FormUID);
            Form_Obj.Remove_Matrix_Row(form, "65");
        }
        private static void Open_Attachment(ItemEvent pVal)
        {
            Form form = SBO_Application.Forms.Item(pVal.FormUID);
            Matrix Mat = (Matrix)form.Items.Item("65").Specific;
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
    }
}
