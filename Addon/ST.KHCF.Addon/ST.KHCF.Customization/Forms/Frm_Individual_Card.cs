using SAPbouiCOM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ST.KHCF.Customization.Forms
{
    internal class Frm_Individual_Card
    {
        internal static SAPbobsCOM.Company company;
        internal static Application SBO_Application;

        internal static void SBO_Application_RightClickEvent(ref ContextMenuInfo eventInfo, out bool BubbleEvent)
        {
            BubbleEvent = true;
            try
            {
                //Form form = SBO_Application.Forms.ActiveForm;
                //if (form.TypeEx == "134" && form.Mode == BoFormMode.fm_OK_MODE)                {


                //}
            }
            catch (Exception ex)
            {
                SBO_Application.StatusBar.SetText(ex.Message, BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Error);
            }
        }

        internal static void SBO_Application_ItemEvent(string FormUID, ref ItemEvent pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;

            try
            {
                //if (pVal.FormTypeEx == "720" && pVal.EventType == BoEventTypes.et_FORM_LOAD && !pVal.BeforeAction)
                //{
                //    Add_Generate_Run_Items(pVal);
                //}

                //if (pVal.FormTypeEx == "720" && pVal.ItemUID == "ST_ITM_STK" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                //{
                //    Run_Item_Stock(pVal);
                //}


            }
            catch (Exception ex)
            {
                               SBO_Application.StatusBar.SetText(ex.Message, BoMessageTime.bmt_Short,  BoStatusBarMessageType.smt_Error);
            }
        }


    }
}
