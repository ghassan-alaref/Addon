using SAPbobsCOM;
using SAPbouiCOM;
using ST.KHCF.Customization.Forms;
using ST.KHCF.Customization.Forms.CCI;
using ST.KHCF.Customization.Forms.Fundraising;
using ST.KHCF.Customization.Forms.Patient;
using ST.KHCF.Customization.Logic;
using ST.KHCF.Customization.Logic.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ST.KHCF.Customization
{
    public class Loader : Helper.Addon_Loader
    {

        internal static SAPbouiCOM.Application SBO_Application;
        internal static SAPbobsCOM.Company company;
        static List<string> All_Menues = new List<string>();
        private static bool Need_To_Change_Msg;
        internal static string New_Msg;
        public static DateTime prev = DateTime.Now;

        public Loader()
        {

            try
            {
                Set_SAP_Connection();
            }
            catch (Exception ex)
            {
                string Log_File = "Log.txt";

                System.IO.File.WriteAllText(Log_File, ex.ToString());
            }
            InitializeApplication();
            try 
            {
                Run();
            }
            catch(Exception ex) 
            {
                string Log_File = "Log.txt";
                System.IO.File.WriteAllText(Log_File, ex.ToString());
                SBO_Application.StatusBar.SetText(ex.Message, SAPbouiCOM.BoMessageTime.bmt_Medium, SAPbouiCOM.BoStatusBarMessageType.smt_Warning);
            }
            Frm_Memberships_Need_To_Active.Check_Membership_If_Need();
            SBO_Application.StatusBar.SetText(ModuleName + " Addon is ready.", SAPbouiCOM.BoMessageTime.bmt_Medium, SAPbouiCOM.BoStatusBarMessageType.smt_Success);
        }


        #region COR Method

        private void Run()
        {
            #region Set Company & SBO_Application
            Frm_Fundraising_Individual_Card.company = company;
            Frm_Patients_Card.company = company;
            Frm_Social_Study.company = company;
            Frm_Coverage_Request.company = company;
            Frm_Coverage_Transaction.company = company;
            Frm_Accommodation.company = company;
            Frm_Treatment_Plan_Details.company = company;
            Frm_Areas_and_Transportation_Types.company = company;
            Frm_Sales_Target.company = company;
            Frm_Commission_Rules.company = company;
            Frm_CCI_Member_Card.company = company;
            Frm_CCI_Corporate_Member_Card.company = company;
            Frm_Individual_Membership.company = company;
            Frm_Corporate_Membership.company = company;
            Frm_Coverage_Rules.company = company;
            Frm_Fundraising_Corporate_Card.company = company;
            Frm_Expected_Donations.company = company;
            Frm_Actual_Donations.company = company;
            Frm_Booth_Commission.company = company;
            Frm_Plaque_Wall.company = company;
            Frm_Tracking_Recognitions.company = company;
            Frm_Recommending_Recognitions.company = company;
            Frm_Naming.company = company;
            Frm_Machinery.company = company;
            Frm_Machinery_Installation_Det.company = company;
            Frm_Monthly_Giving.company = company;
            Frm_Goodwill_Funds.company = company;
            Frm_Grants.company = company;
            Frm_Potential_Grants.company = company;
            Frm_Won_Grants.company = company;
            Frm_Pledges.company = company;
            Frm_Dreams_Come_True.company = company;
            Frm_Patient_Activity.company = company;
            Frm_Communication_Log.company = company;
            Frm_Schools_Universites.company = company;
            Frm_Waiting_List.company = company;
            Frm_Stop_Cancel_Children.company = company;
            Frm_Fund_Target.company = company;
            Frm_Ambassador.company = company;


            Frm_Fundraising_Individual_Card.SBO_Application = SBO_Application;
            Frm_Patients_Card.SBO_Application = SBO_Application;
            Frm_Social_Study.SBO_Application = SBO_Application;
            Frm_Coverage_Request.SBO_Application = SBO_Application;
            Frm_Coverage_Transaction.SBO_Application = SBO_Application;
            Frm_Accommodation.SBO_Application = SBO_Application;
            Frm_Treatment_Plan_Details.SBO_Application = SBO_Application;
            Frm_Areas_and_Transportation_Types.SBO_Application = SBO_Application;
            Frm_Sales_Target.SBO_Application = SBO_Application;
            Frm_Commission_Rules.SBO_Application = SBO_Application;
            Frm_CCI_Member_Card.SBO_Application = SBO_Application;
            Frm_CCI_Corporate_Member_Card.SBO_Application = SBO_Application;
            Frm_Individual_Membership.SBO_Application = SBO_Application;
            Frm_Corporate_Membership.SBO_Application = SBO_Application;
            Frm_Schools_Universites.SBO_Application = SBO_Application;
            Frm_Coverage_Rules.SBO_Application = SBO_Application;
            Frm_Fundraising_Corporate_Card.SBO_Application = SBO_Application;
            Frm_Expected_Donations.SBO_Application = SBO_Application;
            Frm_Actual_Donations.SBO_Application = SBO_Application;
            Frm_Booth_Commission.SBO_Application = SBO_Application;
            Frm_Plaque_Wall.SBO_Application = SBO_Application;
            Frm_Tracking_Recognitions.SBO_Application = SBO_Application;
            Frm_Recommending_Recognitions.SBO_Application = SBO_Application;
            Frm_Naming.SBO_Application = SBO_Application;
            Frm_Machinery.SBO_Application = SBO_Application;
            Frm_Machinery_Installation_Det.SBO_Application = SBO_Application;
            Frm_Monthly_Giving.SBO_Application = SBO_Application;
            Frm_Goodwill_Funds.SBO_Application = SBO_Application;
            Frm_Grants.SBO_Application = SBO_Application;
            Frm_Potential_Grants.SBO_Application = SBO_Application;
            Frm_Won_Grants.SBO_Application = SBO_Application;
            Frm_Pledges.SBO_Application = SBO_Application;
            Frm_Dreams_Come_True.SBO_Application = SBO_Application;
            Frm_Patient_Activity.SBO_Application = SBO_Application;
            Frm_Communication_Log.SBO_Application = SBO_Application;
            Frm_Waiting_List.SBO_Application = SBO_Application;
            Frm_Stop_Cancel_Children.SBO_Application = SBO_Application;
            Frm_Ambassador.SBO_Application = SBO_Application;

            #endregion

            #region Set Item Event
            SBO_Application.ItemEvent += Parent_Form.SBO_Application_ItemEvent_For_Parent;
            SBO_Application.MenuEvent += Parent_Form.SBO_Application_MenuEvent_For_Parent;

            SBO_Application.ItemEvent += Frm_Fundraising_Individual_Card.SBO_Application_ItemEvent;
            SBO_Application.ItemEvent += Frm_Patients_Card.SBO_Application_ItemEvent;
            SBO_Application.ItemEvent += Frm_Social_Study.SBO_Application_ItemEvent;
            SBO_Application.ItemEvent += Frm_Coverage_Request.SBO_Application_ItemEvent;
            SBO_Application.ItemEvent += Frm_Schools_Universites.SBO_Application_ItemEvent;
            SBO_Application.ItemEvent += Frm_Coverage_Transaction.SBO_Application_ItemEvent;
            SBO_Application.ItemEvent += Frm_Accommodation.SBO_Application_ItemEvent;
            SBO_Application.ItemEvent += Frm_Treatment_Plan_Details.SBO_Application_ItemEvent;
            SBO_Application.ItemEvent += Frm_Areas_and_Transportation_Types.SBO_Application_ItemEvent;
            SBO_Application.ItemEvent += Frm_Sales_Target.SBO_Application_ItemEvent;
            SBO_Application.ItemEvent += Frm_Commission_Rules.SBO_Application_ItemEvent;
            SBO_Application.ItemEvent += Frm_CCI_Member_Card.SBO_Application_ItemEvent;
            SBO_Application.ItemEvent += Frm_CCI_Corporate_Member_Card.SBO_Application_ItemEvent;
            SBO_Application.ItemEvent += Frm_Individual_Membership.SBO_Application_ItemEvent;
            SBO_Application.ItemEvent += Frm_Corporate_Membership.SBO_Application_ItemEvent;
            SBO_Application.ItemEvent += Frm_Coverage_Rules.SBO_Application_ItemEvent;
            SBO_Application.ItemEvent += Frm_Fundraising_Corporate_Card.SBO_Application_ItemEvent;
            SBO_Application.ItemEvent += Frm_Expected_Donations.SBO_Application_ItemEvent;
            SBO_Application.ItemEvent += Frm_Actual_Donations.SBO_Application_ItemEvent;
            SBO_Application.ItemEvent += Frm_Booth_Commission.SBO_Application_ItemEvent;
            SBO_Application.ItemEvent += Frm_Plaque_Wall.SBO_Application_ItemEvent;
            SBO_Application.ItemEvent += Frm_Tracking_Recognitions.SBO_Application_ItemEvent;
            SBO_Application.ItemEvent += Frm_Recommending_Recognitions.SBO_Application_ItemEvent;
            SBO_Application.ItemEvent += Frm_Naming.SBO_Application_ItemEvent;
            SBO_Application.ItemEvent += Frm_Machinery.SBO_Application_ItemEvent;
            SBO_Application.ItemEvent += Frm_Machinery_Installation_Det.SBO_Application_ItemEvent;
            SBO_Application.ItemEvent += Frm_Monthly_Giving.SBO_Application_ItemEvent;
            SBO_Application.ItemEvent += Frm_Goodwill_Funds.SBO_Application_ItemEvent;
            SBO_Application.ItemEvent += Frm_Grants.SBO_Application_ItemEvent;
            SBO_Application.ItemEvent += Frm_Potential_Grants.SBO_Application_ItemEvent;
            SBO_Application.ItemEvent += Frm_Won_Grants.SBO_Application_ItemEvent;
            SBO_Application.ItemEvent += Frm_Pledges.SBO_Application_ItemEvent;
            SBO_Application.ItemEvent += Frm_Fund_Rules.SBO_Application_ItemEvent;
            SBO_Application.ItemEvent += Frm_Dreams_Come_True.SBO_Application_ItemEvent;
            SBO_Application.ItemEvent += Frm_Patient_Activity.SBO_Application_ItemEvent;
            SBO_Application.ItemEvent += Frm_Communication_Log.SBO_Application_ItemEvent;
            SBO_Application.ItemEvent += Frm_Waiting_List.SBO_Application_ItemEvent;
            SBO_Application.ItemEvent += Frm_Ambassador.SBO_Application_ItemEvent;
            SBO_Application.ItemEvent += Frm_Fund_Target.SBO_Application_ItemEvent;
            SBO_Application.ItemEvent += Frm_Stop_Cancel_Children.SBO_Application_ItemEvent;
            #endregion

            //Frm_Individual_Card.company = company;
            Forms.Frm_Individual_Card.SBO_Application = SBO_Application;
            Frm_Set_Installment.company = company;
            Frm_Set_Installment.SBO_Application = SBO_Application;
            Frm_Set_Stop_Card_Data.company = company;
            Frm_Set_Stop_Card_Data.SBO_Application = SBO_Application;
            Frm_Revenue_Realization.company = company;
            Frm_Revenue_Realization.SBO_Application = SBO_Application;
            Frm_Cards_List.company = company;
            Frm_Cards_List.SBO_Application = SBO_Application;
            Frm_Cards_Actions_From_File.company = company;
            Frm_Cards_Actions_From_File.SBO_Application = SBO_Application;
            Frm_Import_Data.company = company;
            Frm_Import_Data.SBO_Application = SBO_Application;
            Frm_Membership_Renewal.company = company;
            Frm_Membership_Renewal.SBO_Application = SBO_Application;
            Frm_Memberships_Need_To_Active.company = company;
            Frm_Memberships_Need_To_Active.SBO_Application = SBO_Application;
            Frm_Fund_Target.SBO_Application = SBO_Application;
            Frm_Fund_Rules.company = company;
            Frm_Fund_Rules.SBO_Application = SBO_Application;
            System_Forms.company = company;
            System_Forms.SBO_Application = SBO_Application;
            Frm_System_Time_Sheet.company = company;
            Frm_System_Time_Sheet.SBO_Application = SBO_Application;

            SBO_Application.AppEvent += new SAPbouiCOM._IApplicationEvents_AppEventEventHandler(SBO_Application_AppEvent);
            SBO_Application.MenuEvent += SBO_Application_MenuEvent;
            SBO_Application.StatusBarEvent += SBO_Application_StatusBarEvent;

            SBO_Application.RightClickEvent += Forms.Frm_Individual_Card.SBO_Application_RightClickEvent;
            //SBO_Application.MenuEvent += Forms.Frm_Individual_Card.SBO_Application_MenuEvent;
            SBO_Application.ItemEvent += Frm_Individual_Card.SBO_Application_ItemEvent;
            SBO_Application.ItemEvent += Frm_Set_Installment.SBO_Application_ItemEvent;
            SBO_Application.ItemEvent += Frm_Set_Stop_Card_Data.SBO_Application_ItemEvent;
            SBO_Application.ItemEvent += Frm_Revenue_Realization.SBO_Application_ItemEvent;
            SBO_Application.ItemEvent += Frm_Memberships_Need_To_Active.SBO_Application_ItemEvent;
            SBO_Application.ItemEvent += Frm_Cards_List.SBO_Application_ItemEvent;
            SBO_Application.ItemEvent += Frm_Cards_Actions_From_File.SBO_Application_ItemEvent;
            SBO_Application.ItemEvent += Frm_Import_Data.SBO_Application_ItemEvent;
            SBO_Application.ItemEvent += Frm_Membership_Renewal.SBO_Application_ItemEvent;
            SBO_Application.ItemEvent += System_Forms.SBO_Application_ItemEvent;
            SBO_Application.ItemEvent += Frm_System_Time_Sheet.SBO_Application_ItemEvent;
            SBO_Application.FormDataEvent += SBO_Application_FormDataEvent;

            SBO_Application.MenuEvent += Frm_CCI_Member_Card.SBO_Application_MenuEvent;
            SBO_Application.MenuEvent += Frm_CCI_Corporate_Member_Card.SBO_Application_MenuEvent;
            SBO_Application.MenuEvent += Frm_Individual_Membership.SBO_Application_MenuEvent;
            SBO_Application.MenuEvent += Frm_Corporate_Membership.SBO_Application_MenuEvent;
            SBO_Application.MenuEvent += Frm_Fund_Rules.SBO_Application_MenuEvent;
            SBO_Application.MenuEvent += Frm_Goodwill_Funds.SBO_Application_MenuEvent;
            SBO_Application.MenuEvent += Frm_Fund_Target.SBO_Application_MenuEvent;
            SBO_Application.MenuEvent += Frm_Patient_Activity.SBO_Application_MenuEvent;
            SBO_Application.MenuEvent += Frm_Won_Grants.SBO_Application_MenuEvent;
            SBO_Application.MenuEvent += Frm_Patients_Card.SBO_Application_MenuEvent;
            SBO_Application.MenuEvent += Frm_Social_Study.SBO_Application_MenuEvent;
            SBO_Application.MenuEvent += Frm_Expected_Donations.SBO_Application_MenuEvent;
            SBO_Application.MenuEvent += Frm_Dreams_Come_True.SBO_Application_MenuEvent;
            SBO_Application.MenuEvent += Frm_Actual_Donations.SBO_Application_MenuEvent;
            SBO_Application.MenuEvent += Frm_Commission_Rules.SBO_Application_MenuEvent;
            SBO_Application.MenuEvent += Frm_Coverage_Rules.SBO_Application_MenuEvent;
            SBO_Application.MenuEvent += Frm_Sales_Target.SBO_Application_MenuEvent;
            SBO_Application.MenuEvent += Frm_Booth_Commission.SBO_Application_MenuEvent;
            SBO_Application.MenuEvent += Frm_Grants.SBO_Application_MenuEvent;
            SBO_Application.MenuEvent += Frm_Machinery.SBO_Application_MenuEvent;
            SBO_Application.MenuEvent += Frm_Machinery_Installation_Det.SBO_Application_MenuEvent;
            SBO_Application.MenuEvent += Frm_Monthly_Giving.SBO_Application_MenuEvent;
            SBO_Application.MenuEvent += Frm_Naming.SBO_Application_MenuEvent;
            SBO_Application.MenuEvent += Frm_Plaque_Wall.SBO_Application_MenuEvent;
            SBO_Application.MenuEvent += Frm_Pledges.SBO_Application_MenuEvent;
            SBO_Application.MenuEvent += Frm_Potential_Grants.SBO_Application_MenuEvent;
            SBO_Application.MenuEvent += Frm_Recommending_Recognitions.SBO_Application_MenuEvent;
            SBO_Application.MenuEvent += Frm_Schools_Universites.SBO_Application_MenuEvent;
            SBO_Application.MenuEvent += Frm_Tracking_Recognitions.SBO_Application_MenuEvent;
            SBO_Application.MenuEvent += Frm_Accommodation.SBO_Application_MenuEvent;
            SBO_Application.MenuEvent += Frm_Areas_and_Transportation_Types.SBO_Application_MenuEvent;
            SBO_Application.MenuEvent += Frm_Coverage_Request.SBO_Application_MenuEvent;
            SBO_Application.MenuEvent += Frm_Coverage_Transaction.SBO_Application_MenuEvent;
            SBO_Application.MenuEvent += Frm_Treatment_Plan_Details.SBO_Application_MenuEvent;
            SBO_Application.MenuEvent += Frm_Fundraising_Individual_Card.SBO_Application_MenuEvent;
            SBO_Application.MenuEvent += Frm_Communication_Log.SBO_Application_MenuEvent;
            SBO_Application.MenuEvent += Frm_Ambassador.SBO_Application_MenuEvent;
            SBO_Application.MenuEvent += Frm_Fundraising_Corporate_Card.SBO_Application_MenuEvent;

        }
        private static void SBO_Application_StatusBarEvent(string Text, BoStatusBarMessageType messageType)
        {
            if (Need_To_Change_Msg)
            {
                Need_To_Change_Msg = false;
                SBO_Application.MessageBox(New_Msg);
                New_Msg = string.Empty;
            }
        }

        private void SBO_Application_FormDataEvent(ref SAPbouiCOM.BusinessObjectInfo BusinessObjectInfo, out bool BubbleEvent)
        {
            BubbleEvent = Frm_CCI_Member_Card.SBO_Application_FormDataEvent(ref BusinessObjectInfo);
            if (!BubbleEvent)
            {
                Need_To_Change_Msg = true;
                return;
            }
            BubbleEvent = Frm_Individual_Membership.SBO_Application_FormDataEvent(ref BusinessObjectInfo);
            if (!BubbleEvent)
            {
                Need_To_Change_Msg = true;
                return;
            }
            BubbleEvent = Frm_Corporate_Membership.SBO_Application_FormDataEvent(ref BusinessObjectInfo);
            if (!BubbleEvent)
            {
                Need_To_Change_Msg = true;
                return;
            }

            BubbleEvent = Parent_Form.SBO_Application_FormDataEvent_For_Parent(ref BusinessObjectInfo);
            if (!BubbleEvent)
            {
                Need_To_Change_Msg = true;
                return;
            }
            BubbleEvent = Frm_CCI_Corporate_Member_Card.SBO_Application_FormDataEvent(ref BusinessObjectInfo);
            if (!BubbleEvent)
            {
                Need_To_Change_Msg = true;
                return;
            }
            BubbleEvent = Frm_Communication_Log.SBO_Application_FormDataEvent(ref BusinessObjectInfo);
            if (!BubbleEvent)
            {
                Need_To_Change_Msg = true;
                return;
            }
            BubbleEvent = System_Forms.SBO_Application_FormDataEvent(ref BusinessObjectInfo);
            if (!BubbleEvent)
            {
                Need_To_Change_Msg = true;
                return;
            }
            BubbleEvent = Frm_System_Time_Sheet.SBO_Application_FormDataEvent(ref BusinessObjectInfo);
            if (!BubbleEvent)
            {
                Need_To_Change_Msg = true;
                return;
            }
            BubbleEvent = Frm_Coverage_Rules.SBO_Application_FormDataEvent(ref BusinessObjectInfo);
            if (!BubbleEvent)
            {
                Need_To_Change_Msg = true;
                return;
            }
            BubbleEvent = Frm_Sales_Target.SBO_Application_FormDataEvent(ref BusinessObjectInfo);
            if (!BubbleEvent)
            {
                Need_To_Change_Msg = true;
                return;
            }
            BubbleEvent = Frm_Commission_Rules.SBO_Application_FormDataEvent(ref BusinessObjectInfo);
            if (!BubbleEvent)
            {
                Need_To_Change_Msg = true;
                return;
            }
            BubbleEvent = Frm_Fundraising_Individual_Card.SBO_Application_FormDataEvent(ref BusinessObjectInfo);
            if (!BubbleEvent)
            {
                Need_To_Change_Msg = true;
                return;
            }
            BubbleEvent = Frm_Fundraising_Corporate_Card.SBO_Application_FormDataEvent(ref BusinessObjectInfo);
            if (!BubbleEvent)
            {
                Need_To_Change_Msg = true;
                return;
            }
            BubbleEvent = Frm_Expected_Donations.SBO_Application_FormDataEvent(ref BusinessObjectInfo);
            if (!BubbleEvent)
            {
                Need_To_Change_Msg = true;
                return;
            }
            BubbleEvent = Frm_Actual_Donations.SBO_Application_FormDataEvent(ref BusinessObjectInfo);
            if (!BubbleEvent)
            {
                Need_To_Change_Msg = true;
                return;
            }
            BubbleEvent = Frm_Tracking_Recognitions.SBO_Application_FormDataEvent(ref BusinessObjectInfo);
            if (!BubbleEvent)
            {
                Need_To_Change_Msg = true;
                return;
            }
            BubbleEvent = Frm_Naming.SBO_Application_FormDataEvent(ref BusinessObjectInfo);
            if (!BubbleEvent)
            {
                Need_To_Change_Msg = true;
                return;
            }
            BubbleEvent = Frm_Recommending_Recognitions.SBO_Application_FormDataEvent(ref BusinessObjectInfo);
            if (!BubbleEvent)
            {
                Need_To_Change_Msg = true;
                return;
            }
            BubbleEvent = Frm_Pledges.SBO_Application_FormDataEvent(ref BusinessObjectInfo);
            if (!BubbleEvent)
            {
                Need_To_Change_Msg = true;
                return;
            }
            BubbleEvent = Frm_Plaque_Wall.SBO_Application_FormDataEvent(ref BusinessObjectInfo);
            if (!BubbleEvent)
            {
                Need_To_Change_Msg = true;
                return;
            }
            BubbleEvent = Frm_Machinery.SBO_Application_FormDataEvent(ref BusinessObjectInfo);
            if (!BubbleEvent)
            {
                Need_To_Change_Msg = true;
                return;
            }
            BubbleEvent = Frm_Machinery_Installation_Det.SBO_Application_FormDataEvent(ref BusinessObjectInfo);
            if (!BubbleEvent)
            {
                Need_To_Change_Msg = true;
                return;
            }
            BubbleEvent = Frm_Booth_Commission.SBO_Application_FormDataEvent(ref BusinessObjectInfo);
            if (!BubbleEvent)
            {
                Need_To_Change_Msg = true;
                return;
            }
            BubbleEvent = Frm_Grants.SBO_Application_FormDataEvent(ref BusinessObjectInfo);
            if (!BubbleEvent)
            {
                Need_To_Change_Msg = true;
                return;
            }
            BubbleEvent = Frm_Potential_Grants.SBO_Application_FormDataEvent(ref BusinessObjectInfo);
            if (!BubbleEvent)
            {
                Need_To_Change_Msg = true;
                return;
            }
            BubbleEvent = Frm_Monthly_Giving.SBO_Application_FormDataEvent(ref BusinessObjectInfo);
            if (!BubbleEvent)
            {
                Need_To_Change_Msg = true;
                return;
            }
            BubbleEvent = Frm_Won_Grants.SBO_Application_FormDataEvent(ref BusinessObjectInfo);
            if (!BubbleEvent)
            {
                Need_To_Change_Msg = true;
                return;
            }
            BubbleEvent = Frm_Goodwill_Funds.SBO_Application_FormDataEvent(ref BusinessObjectInfo);
            if (!BubbleEvent)
            {
                Need_To_Change_Msg = true;
                return;
            }
            BubbleEvent = Frm_Fund_Rules.SBO_Application_FormDataEvent(ref BusinessObjectInfo);
            if (!BubbleEvent)
            {
                Need_To_Change_Msg = true;
                return;
            }
            BubbleEvent = Frm_Fund_Target.SBO_Application_FormDataEvent(ref BusinessObjectInfo);
            if (!BubbleEvent)
            {
                Need_To_Change_Msg = true;
                return;
            }
            BubbleEvent = Frm_Dreams_Come_True.SBO_Application_FormDataEvent(ref BusinessObjectInfo);
            if (!BubbleEvent)
            {
                Need_To_Change_Msg = true;
                return;
            }
            BubbleEvent = Frm_Patient_Activity.SBO_Application_FormDataEvent(ref BusinessObjectInfo);
            if (!BubbleEvent)
            {
                Need_To_Change_Msg = true;
                return;
            }
            BubbleEvent = Frm_Patients_Card.SBO_Application_FormDataEvent(ref BusinessObjectInfo);
            if (!BubbleEvent)
            {
                Need_To_Change_Msg = true;
                return;
            }
            BubbleEvent = Frm_Treatment_Plan_Details.SBO_Application_FormDataEvent(ref BusinessObjectInfo);
            if (!BubbleEvent)
            {
                Need_To_Change_Msg = true;
                return;
            }
            BubbleEvent = Frm_Accommodation.SBO_Application_FormDataEvent(ref BusinessObjectInfo);
            if (!BubbleEvent)
            {
                Need_To_Change_Msg = true;
                return;
            }
            BubbleEvent = Frm_Social_Study.SBO_Application_FormDataEvent(ref BusinessObjectInfo);
            if (!BubbleEvent)
            {
                Need_To_Change_Msg = true;
                return;
            }
            BubbleEvent = Frm_Coverage_Transaction.SBO_Application_FormDataEvent(ref BusinessObjectInfo);
            if (!BubbleEvent)
            {
                Need_To_Change_Msg = true;
                return;
            }
            BubbleEvent = Frm_Coverage_Request.SBO_Application_FormDataEvent(ref BusinessObjectInfo);
            if (!BubbleEvent)
            {
                Need_To_Change_Msg = true;
                return;
            }
            BubbleEvent = Frm_Areas_and_Transportation_Types.SBO_Application_FormDataEvent(ref BusinessObjectInfo);
            if (!BubbleEvent)
            {
                Need_To_Change_Msg = true;
                return;
            }
            BubbleEvent = Frm_Schools_Universites.SBO_Application_FormDataEvent(ref BusinessObjectInfo);
            if (!BubbleEvent)
            {
                Need_To_Change_Msg = true;
                return;
            }
            BubbleEvent = Frm_Ambassador.SBO_Application_FormDataEvent(ref BusinessObjectInfo);
            if (!BubbleEvent)
            {
                Need_To_Change_Msg = true;
                return;
            }


            //BubbleEvent = Forms.Item_MasterData_Form.SBO_Application_FormDataEvent(ref BusinessObjectInfo);
            //if (BubbleEvent == false)
            //{
            //    return;
            //}
        }

        void SBO_Application_AppEvent(SAPbouiCOM.BoAppEventTypes EventType)
        {
            switch (EventType)
            {
                case SAPbouiCOM.BoAppEventTypes.aet_CompanyChanged:
                    break;
                case SAPbouiCOM.BoAppEventTypes.aet_FontChanged:
                    break;
                case SAPbouiCOM.BoAppEventTypes.aet_LanguageChanged:
                    //AddMenuItems();
                    break;
                case SAPbouiCOM.BoAppEventTypes.aet_ServerTerminition:
                    Terminate();
                    //SBO_Application.StatusBar.SetText(ModuleName + " Addon will be close", SAPbouiCOM.BoMessageTime.bmt_Medium, SAPbouiCOM.BoStatusBarMessageType.smt_None);
                    Environment.Exit(0);
                    break;
                case SAPbouiCOM.BoAppEventTypes.aet_ShutDown:
                    Terminate();
                    //SBO_Application.StatusBar.SetText(ModuleName + " Addon will be close", SAPbouiCOM.BoMessageTime.bmt_Medium, SAPbouiCOM.BoStatusBarMessageType.smt_None);
                    Environment.Exit(0);
                    break;
                default:
                    break;
            }
        }


        internal void Install()
        {

        }

        internal void LanguageChanged()
        {

        }

        internal string ModuleGuid
        {
            get { return "SkyTech KHCF Add-On"; }
        }

        internal string ModuleInfoLink
        {
            get { return "https://www.e-skytech.com/"; }
        }

        public override string ModuleName
        {
            //get { return "SkyTech Item Stock Transaction"; }
            get { return "SkyTech KHCF Add-On"; }
        }
        public override string Addon_ID
        {
            get { return "0014"; }
        }
        public override string Configuration_UDT_Table_Name
        {
            get { return "ST_KHCF_CONFIG"; }
        }

        internal string ModuleVersion
        {
            get { return "1.0.0.0"; }
        }
        public override bool Has_License
        {
            get { return false; }
        }


        internal void Terminate()
        {
            //var menuExist = SBO_Application.Menus.Item("3072").SubMenus.Exists("ST_STOCK_TRANS");
            //if (menuExist)
            //    SBO_Application.Menus.Item("3072").SubMenus.RemoveEx("ST_STOCK_TRANS");
        }

        #endregion


        #region Connection with SBO

        private void SetApplication()
        {
            SAPbouiCOM.SboGuiApi SboGuiApi = null;
            string sConnectionString = null;
            SboGuiApi = new SAPbouiCOM.SboGuiApi();
            sConnectionString = "";
            try
            {
                sConnectionString = System.Convert.ToString(Environment.GetCommandLineArgs().GetValue(1).ToString().Trim());
            }
            catch (Exception ex) { sConnectionString = System.Convert.ToString("0030002C0030002C00530041005000420044005F00440061007400650076002C0050004C006F006D0056004900490056"); }
            // sConnectionString = System.Convert.ToString(Environment.GetCommandLineArgs().GetValue(1).ToString().Trim());
            //sConnectionString = System.Convert.ToString(Environment.GetCommandLineArgs().GetValue(1).ToString().Trim());
            SboGuiApi.Connect(sConnectionString);
            SBO_Application = SboGuiApi.GetApplication(-1);
        }

        private int SetConnectionContext()
        {
            int setConnectionContextReturn = 0;
            string sCookie = null;
            string sConnectionContext = null;

            SAPbobsCOM.Company company = new SAPbobsCOM.Company();
            sCookie = company.GetContextCookie();
            sConnectionContext = SBO_Application.Company.GetConnectionContext(sCookie);
            if (company.Connected)
            {
                company.Disconnect();
            }
            setConnectionContextReturn = company.SetSboLoginContext(sConnectionContext);

            Loader.company = company;
            return setConnectionContextReturn;
        }

        private void Set_SAP_Connection()
        {
            SetApplication();

            if (!(SetConnectionContext() == 0))
            {
                SBO_Application.MessageBox("Failed setting a connection to DI API", 1, "Ok", "", "");
                System.Environment.Exit(0); //  Terminating the Add-On Application
            }

            if ((company.Connect() != 0))
            {
                int Error_Code = company.GetLastErrorCode();
                SBO_Application.MessageBox($"Failed connecting to the company's Data Base [{company.GetLastErrorDescription()}]", 1, "Ok", "", "");
                System.Environment.Exit(0); //  Terminating the Add-On Application
            }
            else
            {
                PreInstall();
            }
        }

        #endregion

        internal void PreInstall()
        {

        }


        void InitializeApplication()
        {
            try
            {

                Initialize_Client();
                Logic.General_Logic.Initialize();
                Terminate();
                CreateMenu();
                Set_Default_Configuration();
                SBO_Application.StatusBar.SetText(ModuleName + " Add-on is ready!", SAPbouiCOM.BoMessageTime.bmt_Medium, SAPbouiCOM.BoStatusBarMessageType.smt_Success);
            }
            catch (Exception ex)
            {
                SBO_Application.StatusBar.SetText(ex.ToString(), SAPbouiCOM.BoMessageTime.bmt_Medium, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
            }

        }

        private void Set_Default_Configuration()
        {
            //Utility.Get_Configuration(company, "File_Delimiter", "File Delimiter", ";");
            Configurations.Set_Default_Configuration(company);
        }

        private void CreateMenu()
        {

            try
            {
                this.CreateMenus();

                var sapModules = SBO_Application.Menus.Item("43520");
                var menuExist = sapModules.SubMenus.Exists("ST_KHCF");
                //if (menuExist)
                //    sapModules.SubMenus.RemoveEx("ST_KHCF");
                //if (menuExist == true)
                //{
                //    return;
                //}

                var Main_Folder_Menu = Helper.Utility.Add_Menu(sapModules.SubMenus, "ST_KHCF", "KHCF", BoMenuType.mt_POPUP, 20);
                foreach (KHCF_Modules One_Module in Enum.GetValues(typeof(KHCF_Modules)))
                {
                    var Module_UDO = Helper.Utility.Add_Menu(Main_Folder_Menu.SubMenus, "ST_MOD" + One_Module.ToString(),One_Module.ToString(), BoMenuType.mt_POPUP, 1);
                    UDO_Definition[] ModuleMain_Objects = Logic.Objects_Logic.All_UDO_Definition.Where(O => O.UDO_Modules == One_Module && O.Type == Object_Type.UDO_Main).ToArray();
                    foreach (UDO_Definition One_UDO in ModuleMain_Objects)
                    {
                        string Menu_ID = One_UDO.KHCF_Object.ToString();
                        if (One_UDO.KHCF_Object == KHCF_Objects.Fund_Target)
                        {
                            continue;
                        }
                         All_Menues.Add(Menu_ID);
                        //if (menuExist == true)
                        //{
                        //    continue;
                        //}
                        // Module_UDO.SubMenus.Add(Menu_ID, "ST_" + One_UDO.Title, BoMenuType.mt_STRING, 0);
                        Helper.Utility.Add_Menu(Module_UDO.SubMenus, Menu_ID,One_UDO.Title, BoMenuType.mt_STRING, 0);
                        


                    }
                    //if (menuExist == true)
                    //{
                    //    return;
                    //}

                    if (One_Module == KHCF_Modules.CCI)
                    {
                        Helper.Utility.Add_Menu(Module_UDO.SubMenus, "ST_Revenue_Realization", "Revenue Realization", BoMenuType.mt_STRING, 20);
                        Helper.Utility.Add_Menu(Module_UDO.SubMenus, "ST_Cards_List", "Cards WorkFlow", BoMenuType.mt_STRING, 20);
                        Helper.Utility.Add_Menu(Module_UDO.SubMenus, "ST_Cards_Actions_From_File", "Cards Actions From File", BoMenuType.mt_STRING, 20);
                        Helper.Utility.Add_Menu(Module_UDO.SubMenus, "ST_Membership_Renewal", "Membership Renewal", BoMenuType.mt_STRING, 20);
                        Helper.Utility.Add_Menu(Module_UDO.SubMenus, "ST_Memberships_Need_To_Active", "Pre-Payment to AR Invoice", BoMenuType.mt_STRING, 20);
                        //Module_UDO.SubMenus.Add("ST_Revenue_Realization", "ST_Revenue Realization", BoMenuType.mt_STRING, 20);
                        //Module_UDO.SubMenus.Add("ST_Cards_List", "ST_Cards Approval", BoMenuType.mt_STRING, 20);
                        //Module_UDO.SubMenus.Add("ST_Cards_Actions_From_File", "ST_Cards Actions From File", BoMenuType.mt_STRING, 20);
                        //Module_UDO.SubMenus.Add("ST_Membership_Renewal", "ST_Membership Renewal", BoMenuType.mt_STRING, 20);
                        //Module_UDO.SubMenus.Add("ST_Memberships_Need_To_Active", "ST_Memberships Need To Active", BoMenuType.mt_STRING, 20);
                    }
                    if (One_Module == KHCF_Modules.Patient)
                    {
                        UDO_Definition UDO_Good_Will = Objects_Logic.All_UDO_Definition.FirstOrDefault(U => U.KHCF_Object == KHCF_Objects.Goodwill_Funds);
                        Helper.Utility.Add_Menu(Module_UDO.SubMenus, "ST_P" + UDO_Good_Will.KHCF_Object.ToString(), UDO_Good_Will.Title, BoMenuType.mt_STRING, 20);
                        Helper.Utility.Add_Menu(Module_UDO.SubMenus, "ST_Waiting_List", "Waiting List", BoMenuType.mt_STRING, 20);
                    }
                }
                //var Menu_Utilities = Main_Folder_Menu.SubMenus.Add("ST_Utilities", "ST_Utilities", BoMenuType.mt_POPUP, 10);

                var Menu_Utilities = Helper.Utility.Add_Menu(Main_Folder_Menu.SubMenus, "ST_Utilities", "Utilities", BoMenuType.mt_POPUP, 10);
                Helper.Utility.Add_Menu(Menu_Utilities.SubMenus, "ST_Import_Data", "Import Data", BoMenuType.mt_STRING, 20);
                //var myModule = Folder_Menu.SubMenus.Add("ST_QR_DOC_GE", "ST_QR Document Generation", SAPbouiCOM.BoMenuType.mt_STRING, 50);
                // Folder_Menu.SubMenus.Add("ST_FRM_OB_DEFIN", "ST_Objects Definition", SAPbouiCOM.BoMenuType.mt_STRING, 50);
            }
            catch (Exception) { }

        }

        void SBO_Application_MenuEvent(ref SAPbouiCOM.MenuEvent pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;

            if (pVal.BeforeAction)
                return;

            if (All_Menues.Contains(pVal.MenuUID))
            {
                string KHCF_Objects_Text = pVal.MenuUID.Replace("ST_", "");
                //Forms.Parent_Form Form_Obj = new Forms.CCI.Frm_CCI_Member_Card();
                try
                {
                    KHCF_Objects KHCF_Object = (KHCF_Objects)Enum.Parse(typeof(KHCF_Objects), KHCF_Objects_Text);
                    Open_UDO_Form(KHCF_Object);
                }
                catch (Exception ex)
                {
                    SBO_Application.StatusBar.SetText(ex.Message, BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Error);
                    return;
                }
            }
            if (pVal.MenuUID == "ST_PGoodwill_Funds" )
            {          
                Open_UDO_Form( KHCF_Objects.Goodwill_Funds);
            }

            if (pVal.MenuUID == "ST_Revenue_Realization")
            {
                Frm_Revenue_Realization.Create_Form();
            }
            if (pVal.MenuUID == "ST_Cards_List")
            {
                Frm_Cards_List.Create_Form();
            }
            if (pVal.MenuUID == "ST_Cards_Actions_From_File")
            {
                Frm_Cards_Actions_From_File.Create_Form();
            }
            if (pVal.MenuUID == "ST_Import_Data")
            {
                Frm_Import_Data.Create_Form();
            }
            if (pVal.MenuUID == "ST_Membership_Renewal")
            {
                Frm_Membership_Renewal.Create_Form();
            }
            if (pVal.MenuUID == "ST_Memberships_Need_To_Active")
            {
                Frm_Memberships_Need_To_Active.Create_Form(false);
            }  
            if (pVal.MenuUID == "ST_Waiting_List")
            {
                Frm_Waiting_List.Create_Form();
            }
            if (pVal.MenuUID == "ST_Stop_Cancel_Children")
            {
                Frm_Stop_Cancel_Children.Create_Form();
            }


        }

        internal static Form Open_UDO_Form(KHCF_Objects KHCF_Object)
        {
             //prev = DateTime.Now;
            //KHCF_Objects KHCF_Object = Objects_Logic.All_UDO_Definition.FirstOrDefault(O => O.Table_Name == KHCF_Objects_Text).KHCF_Object;
            //Utility.Add_Time_Log("C", "Start of Open UDO Method", prev);

            Forms.Parent_Form Form_Obj = Get_Form_Object(KHCF_Object.ToString());
            Form_Obj.KHCF_Object = KHCF_Object;
            string XML_Form = "";
            string KHCF_Objects_Text = "ST_" + KHCF_Object.ToString();

            switch (KHCF_Objects_Text)
            {
                case "ST_CCI_Member_Card":
                    Frm_CCI_Member_Card.Form_Obj = Form_Obj;
                    XML_Form = Properties.Resources.Frm_CCI_Member_Card;
                    break;
                case "ST_CCI_Corporate_Member_Card":
                    Frm_CCI_Corporate_Member_Card.Form_Obj = Form_Obj;
                    XML_Form = Properties.Resources.Frm_Corporate__Member_Card;
                    break;
                case "ST_Individual_Membership":
                    Frm_Individual_Membership.Form_Obj = Form_Obj;
                    XML_Form = Properties.Resources.Frm_CCI_Individual_Membership;
                    break;
                case "ST_Corporate_Membership":
                    Frm_Corporate_Membership.Form_Obj = Form_Obj;
                    XML_Form = Properties.Resources.Frm_CCI_Corporate_Membership;
                    break;
                case "ST_Communication_Log":
                    Frm_Communication_Log.Form_Obj = Form_Obj;
                    XML_Form = Properties.Resources.Frm_Communication_Log;
                    break;
                case "ST_Sales_Target":
                    Frm_Sales_Target.Form_Obj = Form_Obj;
                    XML_Form = Properties.Resources.Frm_Sales_Target;
                    break;
                case "ST_Commission_Rules":
                    Frm_Commission_Rules.Form_Obj = Form_Obj;
                    XML_Form = Properties.Resources.Frm_Commission_Rules;
                    break;
                case "ST_Coverage_Rules":
                    Frm_Coverage_Rules.Form_Obj = Form_Obj;
                    XML_Form = Properties.Resources.Frm_Coverage_Rules;
                    break;
                case "ST_Fundraising_Corporate_Card":
                    Frm_Fundraising_Corporate_Card.Form_Obj = Form_Obj;
                    XML_Form = Properties.Resources.Frm_Fundraising_Corporate_Card;
                    break;
                case "ST_Fundraising_Individual_Card":
                    Frm_Fundraising_Individual_Card.Form_Obj = Form_Obj;
                    XML_Form = Properties.Resources.Frm_Fundraising_Individual_Card;
                    break;
                case "ST_Expected_Donations":
                    Frm_Expected_Donations.Form_Obj = Form_Obj;
                    XML_Form = Properties.Resources.Frm_Expected_Donations;
                    break;
                case "ST_Actual_Donations":
                    Frm_Actual_Donations.Form_Obj = Form_Obj;
                    XML_Form = Properties.Resources.Frm_Actual_Donations;
                    break;
                case "ST_Tracking_Recognitions":
                    Frm_Tracking_Recognitions.Form_Obj = Form_Obj;
                    XML_Form = Properties.Resources.Frm_Tracing_Recognitions;
                    break;
                case "ST_Naming":
                    Frm_Naming.Form_Obj = Form_Obj;
                    XML_Form = Properties.Resources.Frm_Naming;
                    break;
                case "ST_Machinery":
                    Frm_Machinery.Form_Obj = Form_Obj;
                    XML_Form = Properties.Resources.Frm_Machinery;
                    break;
                case "ST_Machinery_Installation_Det":
                    Frm_Machinery_Installation_Det.Form_Obj = Form_Obj;
                    XML_Form = Properties.Resources.Frm_Machinery_Installation_Det;
                    break;
                case "ST_Monthly_Giving":
                    Frm_Monthly_Giving.Form_Obj = Form_Obj;
                    XML_Form = Properties.Resources.Frm_Monthly_Giving;
                    break;
                case "ST_Plaque_Wall":
                    Frm_Plaque_Wall.Form_Obj = Form_Obj;
                    XML_Form = Properties.Resources.Frm_Plaque_Wall;
                    break;
                case "ST_Pledges":
                    Frm_Pledges.Form_Obj = Form_Obj;
                    XML_Form = Properties.Resources.Frm_Pledges;
                    break;
                case "ST_Booth_Commission":
                    Frm_Booth_Commission.Form_Obj = Form_Obj;
                    XML_Form = Properties.Resources.Frm_Booth_Commission;
                    break;
                case "ST_Recommending_Recognitions":
                    Frm_Recommending_Recognitions.Form_Obj = Form_Obj;
                    XML_Form = Properties.Resources.Frm_Recommending_Recognitions;
                    break;
                case "ST_Grants":
                    Frm_Grants.Form_Obj = Form_Obj;
                    XML_Form = Properties.Resources.Frm_Grants;
                    break;
                case "ST_Goodwill_Funds":
                    Frm_Goodwill_Funds.Form_Obj = Form_Obj;
                    XML_Form = Properties.Resources.Frm_Goodwill_Funds;
                    break;
                case "ST_Dreams_Come_True":
                    Frm_Dreams_Come_True.Form_Obj = Form_Obj;
                    XML_Form = Properties.Resources.Frm_Dreams_Come_True;
                    break;
                case "ST_Patient_Activity":
                    Frm_Patient_Activity.Form_Obj = Form_Obj;
                    XML_Form = Properties.Resources.Frm_Patient_Activity;
                    break;
                case "ST_Potential_Grants":
                    Frm_Potential_Grants.Form_Obj = Form_Obj;
                    XML_Form = Properties.Resources.Frm_Potential_Grants;
                    break;
                case "ST_Won_Grants":
                    Frm_Won_Grants.Form_Obj = Form_Obj;
                    XML_Form = Properties.Resources.Frm_Won_Grants;
                    break;
                case "ST_Treatment_Plan_Details":
                    Frm_Treatment_Plan_Details.Form_Obj = Form_Obj;
                    XML_Form = Properties.Resources.Frm_Treatment_Plan_Details;
                    break;
                case "ST_Accommodation":
                    Frm_Accommodation.Form_Obj = Form_Obj;
                    XML_Form = Properties.Resources.Frm_Accommodation;
                    break;
                case "ST_Patients_Card":
                    Frm_Patients_Card.Form_Obj = Form_Obj;
                    XML_Form = Properties.Resources.Frm_Patients_Card;
                    break;
                case "ST_Social_Study":
                    Frm_Social_Study.Form_Obj = Form_Obj;
                    XML_Form = Properties.Resources.Frm_ST_SOCIAL_STUDY;
                    break;
                case "ST_Coverage_Request":
                    Frm_Coverage_Request.Form_Obj = Form_Obj;
                    XML_Form = Properties.Resources.Frm_Coverage_Request;
                    break;
                case "ST_Coverage_Transaction":
                    Frm_Coverage_Transaction.Form_Obj = Form_Obj;
                    XML_Form = Properties.Resources.Frm_Coverage_Transaction;
                    break;
                case "ST_Areas_and_Transportation_Types":
                    Frm_Areas_and_Transportation_Types.Form_Obj = Form_Obj;
                    XML_Form = Properties.Resources.Frm_Areas_and_transportation_type;
                    break;
                case "ST_Schools_Universites":
                    Frm_Schools_Universites.Form_Obj = Form_Obj;
                    XML_Form = Properties.Resources.Frm_Schools_Universites;
                    break;
                case "ST_Ambassador":
                    Frm_Ambassador.Form_Obj = Form_Obj;
                    XML_Form = Properties.Resources.Frm_Ambassador;
                    break;
                case "ST_Fund_Target":
                    Frm_Fund_Target.Form_Obj = Form_Obj;
                    XML_Form = Properties.Resources.Frm_Fund_Target;
                    break;
                case "ST_Fund_Rules":
                    Frm_Fund_Rules.Form_Obj = Form_Obj;
                    XML_Form = Properties.Resources.Frm_Fund_Rules;
                    break;

                default:
                    throw new Logic.Custom_Exception($"The Menu[{KHCF_Objects_Text}] is not supported");
            }
            Form_Obj.company_For_Parent_Form = company;
            Form_Obj.SBO_Application_For_Parent_Form = SBO_Application;
            return Form_Obj.Create_Form(SBO_Application, XML_Form);
        }

        internal static Forms.Parent_Form Get_Form_Object(string KHCF_Objects_Text)
        {
            KHCF_Objects KHCF_Object = (KHCF_Objects)Enum.Parse(typeof(KHCF_Objects), KHCF_Objects_Text);
            UDO_Definition UDO = Objects_Logic.All_UDO_Definition.FirstOrDefault(O => O.KHCF_Object == KHCF_Object);
            string NameSpace = $"ST.KHCF.Customization.Forms.{UDO.UDO_Modules.ToString()}" ;
            Type CAType = Type.GetType(NameSpace + ".Frm_" + KHCF_Objects_Text);
            Forms.Parent_Form Result = (Forms.Parent_Form)Activator.CreateInstance(CAType);

            return Result;
        }

        public override void Initialize_Client()
        {
            Helper.Utility.Initialize_Client(company, SBO_Application, this);

        }
    }

}
