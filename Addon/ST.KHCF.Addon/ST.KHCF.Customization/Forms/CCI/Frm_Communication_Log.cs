using SAPbouiCOM;
using ST.KHCF.Customization.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SAPbobsCOM;
using ST.KHCF.Customization.Logic.Classes;

namespace ST.KHCF.Customization.Forms.CCI
{
    internal class Frm_Communication_Log : Parent_Form
    {
        internal static SAPbobsCOM.Company company;
        internal static Application SBO_Application;
        internal static Parent_Form Form_Obj;

        //internal override string[] Get_Fix_ReadOnly_Fields_List()
        //{
        //    List<string> Result = new List<string>();
        //    Result.AddRange(base.Get_Fix_ReadOnly_Fields_List());
        //    Result.AddRange(new string[] { "5", "6", "22" , "172"}); "5,6,22,172"

        //    return Result.ToArray();
        //}
        //internal override string[] Get_Mondatory_Fields_List()
        //{
        //    List<string> Result = new List<string>();
        //    Result.AddRange(base.Get_Mondatory_Fields_List());
        //    Result.AddRange(new string[] { "22" });
        //    return Result.ToArray();
        //}

        internal override Depends_List[] Get_Depends_List_List()
        {
            List<Depends_List> Result = new List<Depends_List>();
            Result.AddRange(base.Get_Depends_List_List());
            Result.Add(new Depends_List() { Item_ID = "16", Parent_Item_ID = "14", SQL = $@"SELECT T0.""Code"", T0.""Name"" FROM ""@ST_COMM_SUB_STATUS""  T0 WHERE T0.""U_ST_STATUS"" = '{{0}}'" });

            return Result.ToArray();
        }

        internal override void Initialize_Form(Form form)
        {
            
            //Desc_value = "Mandatary fields List For Communication Log";
            //Man_fields = "22";

            base.Initialize_Form(form);

            form.Items.Item("172").SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 2, BoModeVisualBehavior.mvb_True);
            form.Items.Item("24").SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 2, BoModeVisualBehavior.mvb_True);


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
                Form form = Loader.SBO_Application.Forms.Item(BusinessObjectInfo.FormUID);
                if ((BusinessObjectInfo.EventType == BoEventTypes.et_FORM_DATA_UPDATE || BusinessObjectInfo.EventType == BoEventTypes.et_FORM_DATA_ADD)
    && !BusinessObjectInfo.BeforeAction)
                {
                   // form.DataSources.DBDataSources.Item(0).SetValue("U_ST_NUMBER_CARD_TYPE", 0, "I");
                   // form.DataSources.DBDataSources.Item(0).SetValue("U_ST_AUTOMATIC_DATE", 0, DateTime.Today.ToString("yyyyMMdd"));
                   //string Time= form.DataSources.DBDataSources.Item(0).GetValue("U_ST_TIME", 0);
                   // form.DataSources.DBDataSources.Item(0).SetValue("U_ST_TIME", 0, DateTime.Now.ToString("HH:mm:ss"));
                }

                //if ((BusinessObjectInfo.EventType == BoEventTypes.et_FORM_DATA_UPDATE || BusinessObjectInfo.EventType == BoEventTypes.et_FORM_DATA_ADD) && BusinessObjectInfo.ActionSuccess && !BusinessObjectInfo.BeforeAction)
                //{
                //    ADD_Update_UDO(BusinessObjectInfo);
                //}

                //if (BusinessObjectInfo.EventType == BoEventTypes.et_FORM_DATA_LOAD && !BusinessObjectInfo.BeforeAction)
                //{
                //    Form_Data_Load(BusinessObjectInfo);
                //}
                if (BusinessObjectInfo.EventType == BoEventTypes.et_FORM_DATA_LOAD && !BusinessObjectInfo.BeforeAction)
                {
                    SBO_Application.StatusBar.SetText("Loading", BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_None);
                    Form_Obj.Load_One_Depends_Parent_Item(form, "14", false);

                }


                string TableName = form.DataSources.DBDataSources.Item(0).TableName.Replace("@", "");
                UDO_Definition KHCF_Object = Objects_Logic.All_UDO_Definition.FirstOrDefault(O => O.Table_Name == TableName);
                SBO_Application.StatusBar.SetText("Loading", BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_None);
                if (Form_Obj.Set_ReadOnly(form, KHCF_Object))
                {
                    // return;
                }

            }
            catch (Exception ex)
            {
                //SBO_Application.                SBO_Application.StatusBar.SetText(ex.Message, BoMessageTime.bmt_Short,  BoStatusBarMessageType.smt_Error);(ex.Message, BoMessageTime.bmt_Short, true);
                Loader.New_Msg = ex.Message;

                BubbleEvent = false;
            }

            return BubbleEvent;
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
            // form.DataSources.DBDataSources.Item(0).SetValue("U_ST_BP_CODE", 0, BP_Code);
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
                if (pVal.ItemUID == "14" && pVal.EventType == BoEventTypes.et_COMBO_SELECT && !pVal.BeforeAction)
                {
                    Form form = SBO_Application.Forms.Item(pVal.FormUID);
                    Form_Obj.Load_One_Depends_Parent_Item(form, pVal.ItemUID);
                }
                if (pVal.ItemUID == "24" && pVal.EventType == BoEventTypes.et_COMBO_SELECT && !pVal.BeforeAction)
                {
                    Form form = SBO_Application.Forms.Item(pVal.FormUID);
                    Set_Merber_Card_Type(form);
                }

                if (pVal.ItemUID == "172" && pVal.EventType == BoEventTypes.et_CHOOSE_FROM_LIST && !pVal.BeforeAction)
                {
                    Utility.Chosse_From_List_For_Code_And_DBDataSource(pVal, pVal.ItemUID);
                }

                if (pVal.ItemUID == "1" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Form form = SBO_Application.Forms.Item(pVal.FormUID);

                    if (form.Mode == BoFormMode.fm_ADD_MODE)
                    {
                        string Time = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_TIME", 0);
                        form.DataSources.DBDataSources.Item(0).SetValue("U_ST_AUTOMATIC_DATE", 0, DateTime.Today.ToString("yyyyMMdd"));
                        form.DataSources.DBDataSources.Item(0).SetValue("U_ST_TIME", 0, DateTime.Now.ToString("HH:mm:ss"));
                        Set_Merber_Card_Type(form);

                    }                }

            }
            catch (Exception ex)
            {
                SBO_Application.StatusBar.SetText(ex.Message, BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Error);
            }
        }

        private static void Set_Merber_Card_Type(Form form)
        {

            LinkedButton Yar_Parent = (LinkedButton)form.Items.Item("173").Specific;
            if (form.DataSources.DBDataSources.Item(0).GetValue("U_ST_NUMBER_CARD_TYPE", 0) == "C")
            {
                ((EditText)form.Items.Item("172").Specific).ChooseFromListUID = "ST_CCI_CORP_CARD";
                Yar_Parent.LinkedObjectType = "ST_CCI_CORP_CARD";
            }
            else if (form.DataSources.DBDataSources.Item(0).GetValue("U_ST_NUMBER_CARD_TYPE", 0) == "I")
            {
                ((EditText)form.Items.Item("172").Specific).ChooseFromListUID = "CHS_Indiv_Member_Cards";
                Yar_Parent.LinkedObjectType = "ST_CCI_INDIV_CARD";
            }

            ((EditText)form.Items.Item("172").Specific).ChooseFromListAlias = "Code";
            form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).SetValue("U_ST_MEMBER_CARD_CODE", 0, "");
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
            if (pVal.MenuUID == "1282")//Add || Find
            {
                SAPbouiCOM.Form form = SBO_Application.Forms.ActiveForm;
                form.DataSources.DBDataSources.Item(0).SetValue("U_ST_NUMBER_CARD_TYPE", 0, "I");
                form.DataSources.DBDataSources.Item(0).SetValue("U_ST_AUTOMATIC_DATE", 0, DateTime.Today.ToString("yyyyMMdd"));
                form.DataSources.DBDataSources.Item(0).SetValue("U_ST_TIME", 0, DateTime.Now.ToString("HH:mm:ss"));
                Set_Merber_Card_Type(form);

            }

            if (SBO_Application.Forms.ActiveForm.Mode != BoFormMode.fm_FIND_MODE)
            {
                Code_value = SBO_Application.Forms.ActiveForm.TypeEx.Replace("ST", "Frm");
                Form_Obj.Set_Fields(SBO_Application.Forms.ActiveForm);
            }
            if (pVal.MenuUID == "1282")
            {
                SAPbouiCOM.Form form = SBO_Application.Forms.ActiveForm;
                form.Items.Item("172").SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 2, BoModeVisualBehavior.mvb_True);
                form.Items.Item("24").SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 2, BoModeVisualBehavior.mvb_True);
            }
        }
        
    }
}
