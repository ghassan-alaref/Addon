using SAPbouiCOM;
using ST.KHCF.Customization.Logic;
using ST.KHCF.Customization.Logic.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ST.KHCF.Customization.Forms.CCI
{
    internal class Frm_Set_Stop_Card_Data
    {
        internal static SAPbobsCOM.Company company;
        internal static Application SBO_Application;

        internal static Form Create_Form(string Individual_Cards, string Corporate_Card)
        {
            var form_params = (FormCreationParams)SBO_Application.CreateObject(BoCreatableObjectType.cot_FormCreationParams);
            form_params.XmlData = Properties.Resources.Frm_Set_Stop_Card_Data;
            var form = SBO_Application.Forms.AddEx(form_params);
            //form.Visible = false;
            form.AutoManaged = true;
            form.SupportedModes = -1;
            form_params.Modality = BoFormModality.fm_Modal;


            try
            {
                Initialize_Form(form, Individual_Cards, Corporate_Card);
            }
            catch (Exception ex)
            {
                SBO_Application.StatusBar.SetText(ex.Message, BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Error);
            }
            form.Visible = true;
            return form;
        }

        private static void Initialize_Form(Form form, string Individual_Cards, string Corporate_Card)
        {
            form.DataSources.UserDataSources.Item("4").Value = DateTime.Today.ToString("yyyyMMdd");
            form.DataSources.UserDataSources.Item("INDI_CARDS").Value = Individual_Cards;
            form.DataSources.UserDataSources.Item("CORP_CARD").Value = Corporate_Card;
        }

        internal static void SBO_Application_ItemEvent(string FormUID, ref ItemEvent pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            if (pVal.FormTypeEx != "ST_Set_Stop_Card_Data")
            {
                return;
            }
            try
            {

                if (pVal.ItemUID == "10" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Stop_All(pVal);
                }

                //if (pVal.FormTypeEx == "720" && pVal.ItemUID == "ST_ITM_STK" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                //{
                //    Run_Item_Stock(pVal);
                //}


            }
            catch (Exception ex)
            {
                SBO_Application.StatusBar.SetText(ex.Message, BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Error);
            }
        }

        private static void Stop_All(ItemEvent pVal)
        {
            Form form = SBO_Application.Forms.Item(pVal.FormUID);

            string Stop_Date_Text = form.DataSources.UserDataSources.Item("4").ValueEx;
            string Stop_Note = form.DataSources.UserDataSources.Item("6").Value;
            if (Stop_Date_Text == "")
            {
                throw new Logic.Custom_Exception("Please select the Stop Date");
            }
            if (Stop_Note =="")
            {
                throw new Logic.Custom_Exception("Please set the Stop Reason");
            }
            DateTime Stop_Date = DateTime.ParseExact(Stop_Date_Text, "yyyyMMdd", null);
            UDO_Definition UDO_Indiv_Membership_Info = Logic.Objects_Logic.All_UDO_Definition.FirstOrDefault(U => U.KHCF_Object == KHCF_Objects.Individual_Membership);
            UDO_Definition UDO_Corp_Membership_Info = Logic.Objects_Logic.All_UDO_Definition.FirstOrDefault(U => U.KHCF_Object == KHCF_Objects.Corporate_Membership);

            string Corp_Card = form.DataSources.UserDataSources.Item("CORP_CARD").Value;
            string Indiv_Cards = form.DataSources.UserDataSources.Item("INDI_CARDS").Value;

            if (Corp_Card != "")
            {
                Membership.Stop_MemberCard(company, Corp_Card, UDO_Corp_Membership_Info, Stop_Date, Stop_Note);
                SBO_Application.StatusBar.SetText($"The Card[{Corp_Card}] has been stopped", BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Success);
            }

            if (Indiv_Cards != "")
            {
                foreach (string OneCard in Indiv_Cards.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries))
                {
                    Membership.Stop_MemberCard(company, OneCard, UDO_Indiv_Membership_Info, Stop_Date, Stop_Note);
                    SBO_Application.StatusBar.SetText($"The Card[{OneCard}] has been stopped", BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Success);
                }
            }
            form.Close();
        }
    }
}
