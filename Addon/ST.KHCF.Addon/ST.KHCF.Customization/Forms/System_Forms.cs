using SAPbobsCOM;
using SAPbouiCOM;
using ST.KHCF.Customization.Forms.CCI;
using ST.KHCF.Customization.Logic;
using ST.KHCF.Customization.Logic.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace ST.KHCF.Customization.Forms
{
    internal class System_Forms
    {
        internal static SAPbobsCOM.Company company;
        internal static Application SBO_Application;

        internal static void SBO_Application_RightClickEvent(ref ContextMenuInfo eventInfo, out bool BubbleEvent)
        {
            BubbleEvent = true;
            try
            {

            }
            catch (Exception ex)
            {
                SBO_Application.SetStatusBarMessage(ex.Message, BoMessageTime.bmt_Medium, true);
            }
        }

        internal static void SBO_Application_ItemEvent(string FormUID, ref ItemEvent pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;

            //if (pVal.FormTypeEx != "720")//Goods Issue form
            //{
            //    return;
            //}

            try 
            {
                if (pVal.FormTypeEx == "134")// BP Form
                {
                    if (pVal.EventType == BoEventTypes.et_FORM_LOAD && !pVal.BeforeAction)
                    {
                        Form form = SBO_Application.Forms.Item(pVal.FormUID);
                        Add_BP_Items(form);
                    }

                    if (pVal.ItemUID == "ST_CON_LED" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                    {
                        Form form = SBO_Application.Forms.Item(pVal.FormUID);
                        if (form.Mode != BoFormMode.fm_OK_MODE)
                        {
                            throw new Logic.Custom_Exception("We convert the Lead to Customer in OK mode only");
                        }

                        string CardCode = form.DataSources.DBDataSources.Item(0).GetValue("CardCode", 0);
                        Convert_Lead_To_MemberCard_Form(CardCode);
                    }


                }

                if (pVal.FormTypeEx == "179")// Credit Note Form
                {
                    if (pVal.EventType == BoEventTypes.et_FORM_LOAD && !pVal.BeforeAction)
                    {
                        Form form = SBO_Application.Forms.Item(pVal.FormUID);
                        //Add_Credit_Note_Items(form);
                    }

                    if (pVal.ItemUID == "ST_CAN_REL" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                    {
                        Form form = SBO_Application.Forms.Item(pVal.FormUID);
                        if (form.Mode != BoFormMode.fm_OK_MODE)
                        {
                            throw new Logic.Custom_Exception("We Create the Revenue Realization cancellation in OK mode only");
                        }

                        int DocEntry = int.Parse(form.DataSources.DBDataSources.Item(0).GetValue("DocEntry", 0));
                        int DocNum = int.Parse(form.DataSources.DBDataSources.Item(0).GetValue("DocEntry", 0));
                        //Create_Revenue_Realization_Cancellation(DocEntry, DocNum);
                    }
                }
                if (pVal.FormTypeEx == "141")// AP Invoice Form
                {
                    if (pVal.EventType == BoEventTypes.et_FORM_LOAD && !pVal.BeforeAction)
                    {
                        Form form = SBO_Application.Forms.Item(pVal.FormUID);
                        Add_AP_Invoice_Items(form);
                        Add_Marketing_Items(form, "86", "46", "OPCH");
                    }

                    if (pVal.ItemUID == "ST_CRT_JE" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                    {
                        Form form = SBO_Application.Forms.Item(pVal.FormUID);
                        if (form.Mode != BoFormMode.fm_OK_MODE)
                        {
                            throw new Logic.Custom_Exception("We Create the Journal Entry in OK mode only");
                        }

                        int DocEntry = int.Parse(form.DataSources.DBDataSources.Item(0).GetValue("DocEntry", 0));
                        int DocNum = int.Parse(form.DataSources.DBDataSources.Item(0).GetValue("DocEntry", 0));
                        string Patient_BP_Code = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_PATIENT_CODE", 0);
                        Create_KFCC_Invoice_JE(DocEntry, DocNum, Patient_BP_Code);
                    }
                }

                if (pVal.FormTypeEx == "1320000022")// Campaign Form
                {
                    if (pVal.EventType == BoEventTypes.et_FORM_LOAD && !pVal.BeforeAction)
                    {
                        Form form = SBO_Application.Forms.Item(pVal.FormUID);
                        Add_Campaign_Items(form);
                    }

                    if (pVal.ItemUID == "ST_CMBPROG" && pVal.EventType == BoEventTypes.et_COMBO_SELECT && !pVal.BeforeAction)
                    {
                        Form form = SBO_Application.Forms.Item(pVal.FormUID);

                        Check_Campaign_Column_Visibility(form);
                    }

                }
       
                if (pVal.FormTypeEx == "1470000002")// Bin Location Master Data Form
                {
                    if (pVal.EventType == BoEventTypes.et_FORM_LOAD && !pVal.BeforeAction)
                    {
                        Form form = SBO_Application.Forms.Item(pVal.FormUID);
                        Add_Bin_Location_Items(form);
                    }
                }

                if (pVal.FormTypeEx == "63")// Item Group Form
                {
                    if (pVal.EventType == BoEventTypes.et_FORM_LOAD && !pVal.BeforeAction)
                    {
                        Form form = SBO_Application.Forms.Item(pVal.FormUID);
                        Add_Item_Group_Items(form);
                    }

                }

                if (pVal.FormTypeEx == "150")// Item Master Data Form
                {
                    if (pVal.EventType == BoEventTypes.et_FORM_LOAD && !pVal.BeforeAction)
                    {
                        Form form = SBO_Application.Forms.Item(pVal.FormUID);
                        Add_Item_Items(form);
                    }

                }

                if (pVal.FormTypeEx == "651")
                {
                    if(pVal.ItemUID == "U_ST_CONTACT_TYPE" && pVal.EventType == BoEventTypes.et_COMBO_SELECT && !pVal.BeforeAction)
                    {
                        Form form = SBO_Application.Forms.Item(pVal.FormUID);
                        EditText Donor_ID = (EditText) form.Items.Item("U_ST_CONTACT_CARD").Specific;
                        ComboBox Combo_type = (ComboBox)form.Items.Item("U_ST_CONTACT_TYPE").Specific;
                        if (Combo_type.Value == "I")
                        {
                            Donor_ID.ChooseFromListUID = "CFL_Donor";
                        }
                        else if (Combo_type.Value == "C")
                        {
                            Donor_ID.ChooseFromListUID = "CFL_CORP";
                        }
                    }
                }
                //if (pVal.FormTypeEx == "711") // Project Form
                //{
                //    if (pVal.EventType == BoEventTypes.et_FORM_LOAD && !pVal.BeforeAction)
                //    {
                //        Form form = SBO_Application.Forms.Item(pVal.FormUID);
                //       // Add_Project_Item(form);
                //    }

                //}

                if (pVal.FormTypeEx == "60090")// Invoice For Donation
                {
                    if (pVal.EventType == BoEventTypes.et_FORM_DATA_ADD && pVal.ActionSuccess)
                    {
                        Form form = SBO_Application.Forms.Item(pVal.FormUID);
                        string Code = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_ACTUAL_DONATION_CODE", 0);
                        if (!string.IsNullOrEmpty(Code))
                        {
                            string SQL_Entry = $@"SELECT T0.""DocEntry"" FROM OINV T0 WHERE T0.""U_ST_ACTUAL_DONATION_CODE"" ='{Code}'";
                            Recordset RC = Helper.Utility.Execute_Recordset_Query(company, SQL_Entry);
                            if (RC.RecordCount > 0)
                            {
                                string Doc_Entry = RC.Fields.Item(0).Value.ToString();
                                UDO_Definition UDO_Info = Logic.Objects_Logic.All_UDO_Definition.FirstOrDefault(U => U.KHCF_Object == KHCF_Objects.Actual_Donations);
                                Field_Data Fld = new Field_Data() { Field_Name = "U_ST_INVOICE_NUMBER", Value = Doc_Entry };
                                Utility.Update_UDO(company,UDO_Info, Code, new Field_Data[] { Fld });
                            }
                        }
                        //Add_Credit_Note_Items(form);
                    }

                }

                if (pVal.FormTypeEx == "133")// Invoice Form
                {
                    if (pVal.EventType == BoEventTypes.et_FORM_LOAD && !pVal.BeforeAction)
                    {
                        Form form = SBO_Application.Forms.Item(pVal.FormUID);
                        Add_Marketing_Items(form, "86", "46", "OINV");
                    }
                }

                if (pVal.FormTypeEx == "170")// IncomingPayment Form
                {
                    if (pVal.EventType == BoEventTypes.et_FORM_LOAD && !pVal.BeforeAction)
                    {
                        Form form = SBO_Application.Forms.Item(pVal.FormUID);
                        Add_Marketing_Items(form, "53", "52", "ORCT");
                    }
                }

                if (pVal.FormTypeEx == "720")// Goods Issue Form
                {
                    if (pVal.EventType == BoEventTypes.et_FORM_LOAD && !pVal.BeforeAction)
                    {
                        Form form = SBO_Application.Forms.Item(pVal.FormUID);
                        Add_Marketing_Items(form, "20", "3", "OIGE");
                    }
                }

                if (pVal.FormTypeEx == "149") // Sales Quotation
                {
                    if (pVal.EventType == BoEventTypes.et_FORM_LOAD && !pVal.BeforeAction)
                    {
                        Form form = SBO_Application.Forms.Item(pVal.FormUID);
                        Add_Marketing_Items(form, "86", "46", "OQUT");
                    }
                }

                if (pVal.FormTypeEx == "142")// A/P Invoice Form
                {
                    if (pVal.EventType == BoEventTypes.et_FORM_LOAD && !pVal.BeforeAction)
                    {
                        Form form = SBO_Application.Forms.Item(pVal.FormUID);
                        Add_Marketing_Items(form, "86", "46", "OPOR");
                    }
                }

                if (pVal.FormTypeEx == "18000002")// Warehouse sub level Form
                {
                    if (pVal.EventType == BoEventTypes.et_FORM_LOAD && !pVal.BeforeAction)
                    {
                        Form form = SBO_Application.Forms.Item(pVal.FormUID);
                        //ADD_Bin_Sub_Location(form);
                    }
                }

                if (pVal.ItemUID == "ST_CAN_REL" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Form form = SBO_Application.Forms.Item(pVal.FormUID);
                    if (form.Mode != BoFormMode.fm_OK_MODE)
                    {
                        throw new Logic.Custom_Exception("We Create the Revenue Realization cancellation in OK mode only");
                    }

                    int DocEntry = int.Parse(form.DataSources.DBDataSources.Item(0).GetValue("DocEntry", 0));
                    int DocNum = int.Parse(form.DataSources.DBDataSources.Item(0).GetValue("DocEntry", 0));
                    //Create_Revenue_Realization_Cancellation(DocEntry, DocNum);
                }
            }
            catch (Exception ex)
            {
                SBO_Application.StatusBar.SetText(ex.Message, BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Error);
            }

        }

        private static void Check_Campaign_Column_Visibility(Form form)
        {
            Helper.MetaDataOperater.UserFields.Field_Definition[] All_Line_UDFs_List = Helper.MetaDataOperater.UserTable.Get_Fields_List(company, "CPN1");
            string[] UDF_Names = All_Line_UDFs_List.Select(U => "U_" + U.Field_Name).ToArray();

            string Program = form.DataSources.DBDataSources.Item("OCPN").GetValue("U_ST_PROGRAM", 0);
            string SQL_Program = $@"SELECT T0.""U_ST_VISIBLE_UDFS"" FROM ""@ST_PROGRAM_LEVEL1""  T0 WHERE T0.""Code"" = '{Program}' ";
            Recordset RC_Program = Helper.Utility.Execute_Recordset_Query(company, SQL_Program);

            string Program_UDFs_Text = RC_Program.Fields.Item("U_ST_VISIBLE_UDFS").Value.ToString();
            string[] Program_UDFs_List = Program_UDFs_Text.Split(",".ToCharArray());

            Matrix Mat_Lines = (Matrix)form.Items.Item("1320000034").Specific;

            foreach (string OneUDF in UDF_Names)
            {
                if (Program_UDFs_List.Contains(OneUDF))
                {
                    Mat_Lines.Columns.Item(OneUDF).Visible = true;
                }
                else
                {
                    Mat_Lines.Columns.Item(OneUDF).Visible = false;
                }
            }

        }

        private static void Create_KFCC_Invoice_JE(int DocEntry, int DocNum, string Patient_Code)
        {
            KHCF_Logic_Utility.Create_KFCC_Invoice_JE(company, DocEntry, DocNum, Patient_Code);
            SBO_Application.StatusBar.SetText($"the KFCC Journal Entry has been created successfully!", BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Success);
        }

        internal static void Create_Revenue_Realization_Cancellation(int DocEntry, int DocNum)
        {
            if (SBO_Application.MessageBox("Are you sure you want to create the Revenue Realization Cancellation Journal Entry?", 1, "Yes", "No") != 1)
            {
                return;
            }
            string SQL_Base_Num = $@"SELECT T0.""BaseRef"" FROM RIN1 T0 WHERE T0.""DocEntry"" = {DocEntry}";
            Recordset RC_Base_Num = Helper.Utility.Execute_Recordset_Query(company, SQL_Base_Num);
            int Base_Inv_DocNum = (int)RC_Base_Num.Fields.Item("BaseRef").Value;
            KHCF_Logic_Utility.Create_Revenue_Realization_Cancellation(company, DocEntry, DocNum, Base_Inv_DocNum);
            SBO_Application.StatusBar.SetText($"the Cancellation Journal Entry has been created successfully!", BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Success);

        }

        internal static void Convert_Lead_To_MemberCard_Form(string CardCode)
        {
            /// Form form = SBO_Application.Forms.Item(formUID);
            BusinessPartners BP = (BusinessPartners)company.GetBusinessObject(BoObjectTypes.oBusinessPartners);
            BP.GetByKey(CardCode);

            Convert_Lead_To_MemberCard_Validation(BP);


            if (SBO_Application.MessageBox("Are you sure you want to convert the BP to customer?", 1, "Yes", "No") != 1)
            {
                return;
            }

            int BP_Group = BP.GroupCode;
            string SQL_Group = $@"SELECT T1.""U_ST_TYPE"", T1.""U_ST_CUSTOMER_TYPE"" FROM ""OCRG""  T1 
WHERE ""GroupCode"" = {BP_Group}";
            Recordset RC_Group = Helper.Utility.Execute_Recordset_Query(company, SQL_Group);
            bool Is_Corp = RC_Group.Fields.Item("U_ST_TYPE").Value.ToString() == "C";
            Form Member_Card_Form = null;
            try
            {
                //Logic.Create_Goods_Receipt_From_GoodsIssue(company, Doc);
                KHCF_Objects KHCF_Object;
                string UDO_Address;
                //string UDO_Contact
                string UDO_Contact;
                if (Is_Corp)
                {
                    KHCF_Object = KHCF_Objects.CCI_Corporate_Member_Card;
                    UDO_Address = "@ST_CCI_CORP_ADDR";
                    UDO_Contact = "@ST_CCI_CORP_CONT";
                }
                else
                {
                    KHCF_Object = KHCF_Objects.CCI_Member_Card;
                    UDO_Address = "@ST_CCI_INDIV_ADDR";
                    UDO_Contact = "@ST_CCI_INDIV_CONT";
                }
                UDO_Definition UDO_Info = Logic.Objects_Logic.All_UDO_Definition.FirstOrDefault(U => U.KHCF_Object == KHCF_Object);

                Member_Card_Form = Loader.Open_UDO_Form(KHCF_Object);
                Member_Card_Form.Mode = BoFormMode.fm_ADD_MODE;
                Member_Card_Form.Freeze(true);
                if (Is_Corp == true)
                {
                    Frm_CCI_Corporate_Member_Card.Form_Obj.Set_Fields(Member_Card_Form);
                }
                else
                {
                    Frm_CCI_Member_Card.Form_Obj.Set_Fields(Member_Card_Form);
                }
                //CompanyService oCmpSrv = company.GetCompanyService();
                //string UDO_Code = Utility.Get_UDO_Type_ID(UDO_Info.KHCF_Object);
                //GeneralService oGeneralService = oCmpSrv.GetGeneralService(UDO_Code);
                //GeneralDataParams oGeneralParams = (GeneralDataParams)oGeneralService.GetDataInterface(GeneralServiceDataInterfaces.gsGeneralDataParams);
                //string UDO_Entry_Code = Utility.Get_New_UDO_Code(company, UDO_Info.KHCF_Object);
                ////oGeneralParams.SetProperty("Code", UDO_Entry_Code);
                //// GeneralData oGeneralData = oGeneralService.GetByParams(oGeneralParams);
                //GeneralData oGeneralData = (GeneralData)oGeneralService.GetDataInterface(SAPbobsCOM.GeneralServiceDataInterfaces.gsGeneralData);
                //oGeneralData.SetProperty("Code", UDO_Entry_Code);
                //oGeneralData.SetProperty("U_ST_CREATOR", company.UserName);
                //oGeneralData.SetProperty("U_ST_CREATION_DATE", DateTime.Today.ToString("yyyyMMdd"));
                //oGeneralData.SetProperty("U_ST_APPROVAL_STATUS", "A");

                //Frm_Member_Card.DataSources.DBDataSources.Item(0).SetValue("", 0 )
                Member_Card_Form.DataSources.DBDataSources.Item(0).SetValue("U_ST_ACCOUNT_MANAGER", 0, BP.SalesPersonCode.ToString());// BP.UserFields.Fields.Item("U_ST_ACCOUNT_MANAGER").Value);
                Member_Card_Form.DataSources.DBDataSources.Item(0).SetValue("U_ST_CHANNEL", 0, BP.UserFields.Fields.Item("U_ST_CHANNEL").Value.ToString());
                Member_Card_Form.DataSources.DBDataSources.Item(0).SetValue("U_ST_SUB_CHANNEL", 0, BP.UserFields.Fields.Item("U_ST_SUB_CHANNEL").Value.ToString());

                Member_Card_Form.DataSources.DBDataSources.Item(0).SetValue("U_ST_EMAIL", 0, BP.EmailAddress);

                Member_Card_Form.DataSources.DBDataSources.Item(0).SetValue("U_ST_TEL1", 0, BP.Phone1);
                Member_Card_Form.DataSources.DBDataSources.Item(0).SetValue("U_ST_CURRENCY", 0, BP.Currency);
                Member_Card_Form.DataSources.DBDataSources.Item(0).SetValue("U_ST_BP_CODE", 0, BP.CardCode);
                Member_Card_Form.DataSources.DBDataSources.Item(0).SetValue("U_ST_CUSTOMER_GROUP", 0, BP.GroupCode.ToString());
                if (Is_Corp)
                {
                    Member_Card_Form.DataSources.DBDataSources.Item(0).SetValue("U_ST_SECTOR", 0, BP.UserFields.Fields.Item("U_ST_MAIN_SECTOR").Value.ToString());
                    Member_Card_Form.DataSources.DBDataSources.Item(0).SetValue("U_ST_CORPORATE_ARABIC_NAME", 0, BP.UserFields.Fields.Item("CardName").Value.ToString());
                    Member_Card_Form.DataSources.DBDataSources.Item(0).SetValue("U_ST_CORPORATE_ENGLISH_NAME", 0, BP.UserFields.Fields.Item("CardFName").Value.ToString());
                    Member_Card_Form.DataSources.DBDataSources.Item(0).SetValue("U_ST_GENERAL_MANAGER", 0, BP.UserFields.Fields.Item("U_ST_GENERAL_MANAGER").Value.ToString());
                    Member_Card_Form.DataSources.DBDataSources.Item(0).SetValue("U_ST_BROKER", 0, BP.UserFields.Fields.Item("U_ST_BROKER1").Value.ToString());
                    Member_Card_Form.DataSources.DBDataSources.Item(0).SetValue("U_ST_CORPORATE_NATIONAL_ID", 0, BP.UserFields.Fields.Item("U_ST_CORPORATE_NATIONAL_ID").Value.ToString());
                    Member_Card_Form.DataSources.DBDataSources.Item(0).SetValue("U_ST_MOBILE_1", 0, BP.Cellular);
                    Member_Card_Form.DataSources.DBDataSources.Item(0).SetValue("U_ST_TEL_2", 0, BP.Phone2);
                    Member_Card_Form.DataSources.DBDataSources.Item(0).SetValue("U_ST_REMARK", 0, BP.FreeText);
                    Member_Card_Form.DataSources.DBDataSources.Item(0).SetValue("U_ST_MOBILE_2", 0, BP.Fax);

                }
                else
                {

                    Member_Card_Form.DataSources.DBDataSources.Item(0).SetValue("U_ST_PARENT_TYPE", 0, "N");
                    Member_Card_Form.DataSources.DBDataSources.Item(0).SetValue("U_ST_NATIONAL_ID", 0, BP.UserFields.Fields.Item("U_ST_NATIONAL_ID").Value.ToString());
                    Member_Card_Form.DataSources.DBDataSources.Item(0).SetValue("U_ST_FIRST_NAME_AR", 0, BP.UserFields.Fields.Item("U_ST_FIRST_NAME_AR").Value.ToString());
                    Member_Card_Form.DataSources.DBDataSources.Item(0).SetValue("U_ST_FATHER_NAME_AR", 0, BP.UserFields.Fields.Item("U_ST_FATHER_NAME_AR").Value.ToString());
                    Member_Card_Form.DataSources.DBDataSources.Item(0).SetValue("U_ST_MIDDLE_NAME_AR", 0, BP.UserFields.Fields.Item("U_ST_MIDDLE_NAME_AR").Value.ToString());
                    Member_Card_Form.DataSources.DBDataSources.Item(0).SetValue("U_ST_SURNAME_AR", 0, BP.UserFields.Fields.Item("U_ST_SURNAME_AR").Value.ToString());
                    Member_Card_Form.DataSources.DBDataSources.Item(0).SetValue("U_ST_FULL_NAME_AR", 0, Utility.Get_Full_Name(BP.UserFields.Fields.Item("U_ST_FIRST_NAME_AR").Value.ToString(), BP.UserFields.Fields.Item("U_ST_FATHER_NAME_AR").Value.ToString(), BP.UserFields.Fields.Item("U_ST_MIDDLE_NAME_AR").Value.ToString(), BP.UserFields.Fields.Item("U_ST_SURNAME_AR").Value.ToString()));
                    Member_Card_Form.DataSources.DBDataSources.Item(0).SetValue("U_ST_FIRST_NAME_EN", 0, BP.UserFields.Fields.Item("U_ST_FIRST_NAME_EN").Value.ToString());
                    Member_Card_Form.DataSources.DBDataSources.Item(0).SetValue("U_ST_FATHER_NAME_EN", 0, BP.UserFields.Fields.Item("U_ST_FATHER_NAME_EN").Value.ToString());
                    Member_Card_Form.DataSources.DBDataSources.Item(0).SetValue("U_ST_MIDDLE_NAME_EN", 0, BP.UserFields.Fields.Item("U_ST_MIDDLE_NAME_EN").Value.ToString());
                    Member_Card_Form.DataSources.DBDataSources.Item(0).SetValue("U_ST_SURNAME_EN", 0, BP.UserFields.Fields.Item("U_ST_SURNAME_EN").Value.ToString());
                    Member_Card_Form.DataSources.DBDataSources.Item(0).SetValue("U_ST_FULL_NAME_EN", 0, Utility.Get_Full_Name(BP.UserFields.Fields.Item("U_ST_FIRST_NAME_EN").Value.ToString(), BP.UserFields.Fields.Item("U_ST_FATHER_NAME_EN").Value.ToString(), BP.UserFields.Fields.Item("U_ST_MIDDLE_NAME_EN").Value.ToString(), BP.UserFields.Fields.Item("U_ST_SURNAME_EN").Value.ToString()));
                    Member_Card_Form.DataSources.DBDataSources.Item(0).SetValue("U_ST_DATE_OF_BIRTH", 0, ((DateTime)BP.UserFields.Fields.Item("U_ST_DATE_OF_BIRTH").Value).ToString("yyyyMMdd"));
                    Member_Card_Form.DataSources.DBDataSources.Item(0).SetValue("U_ST_GENDER", 0, BP.UserFields.Fields.Item("U_ST_GENDER").Value.ToString());
                    Member_Card_Form.DataSources.DBDataSources.Item(0).SetValue("U_ST_NATIONALITY", 0, BP.UserFields.Fields.Item("U_ST_NATIONALITY").Value.ToString());
                    Member_Card_Form.DataSources.DBDataSources.Item(0).SetValue("U_ST_RESIDENCY", 0, BP.UserFields.Fields.Item("U_ST_RESIDENCY").Value.ToString());

                    string s = BP.UserFields.Fields.Item("U_ST_RESIDENCY").Value.ToString();
                    Member_Card_Form.DataSources.DBDataSources.Item(0).SetValue("U_ST_TITLE", 0, BP.UserFields.Fields.Item("U_ST_JOB_TITLE").Value.ToString());
                    Member_Card_Form.DataSources.DBDataSources.Item(0).SetValue("U_ST_PERSONAL_ID", 0, BP.UserFields.Fields.Item("U_ST_PERSONAL_ID").Value.ToString());
                    Member_Card_Form.DataSources.DBDataSources.Item(0).SetValue("U_ST_PASSPORT_ID", 0, BP.UserFields.Fields.Item("U_ST_PASSPORT_ID").Value.ToString());
                    Member_Card_Form.DataSources.DBDataSources.Item(0).SetValue("U_ST_BROKER1", 0, BP.UserFields.Fields.Item("U_ST_BROKER1").Value.ToString());
                    Member_Card_Form.DataSources.DBDataSources.Item(0).SetValue("U_ST_TEL2", 0, BP.Phone2);
                    Member_Card_Form.DataSources.DBDataSources.Item(0).SetValue("U_ST_MOBILE", 0, BP.Cellular);

                }

                //SAPbobsCOM.GeneralDataCollection oChildren = oGeneralData.Child(UDO_Address);
                DBDataSource DB_Address = Member_Card_Form.DataSources.DBDataSources.Item(UDO_Address);
                int Address_Index = 0;
                for (int J = 0; J < BP.Addresses.Count; J++)
                {
                    BP.Addresses.SetCurrentLine(J);
                    if (BP.Addresses.AddressName == "")
                    {
                        continue;
                    }
                    if (DB_Address.GetValue("U_ST_ADDRESS_NAME", Address_Index) != "")
                    {
                        DB_Address.InsertRecord(Address_Index);
                        Address_Index++;
                    }

                    DB_Address.SetValue("U_ST_ADDRESS_NAME", Address_Index, BP.Addresses.AddressName);
                    DB_Address.SetValue("U_ST_STREET", Address_Index, BP.Addresses.Street);
                    DB_Address.SetValue("U_ST_BLOCK", Address_Index, BP.Addresses.Block);
                    DB_Address.SetValue("U_ST_ZIP_CODE", Address_Index, BP.Addresses.ZipCode);
                    DB_Address.SetValue("U_ST_CITY", Address_Index, BP.Addresses.City);
                    DB_Address.SetValue("U_ST_COUNTY", Address_Index, BP.Addresses.County);
                    string Country_Name = Utility.Get_Country_Name(company,BP.Addresses.Country);
                    DB_Address.SetValue("U_ST_COUNTRY", Address_Index, Country_Name);
                    DB_Address.SetValue("U_ST_STATE", Address_Index, BP.Addresses.State);
                    DB_Address.SetValue("U_ST_BUILDING", Address_Index, BP.Addresses.BuildingFloorRoom);
                    DB_Address.SetValue("U_ST_ADDRESS_TYPE", Address_Index, BP.Addresses.AddressType == BoAddressType.bo_ShipTo ? "S" : "B");
                    DB_Address.SetValue("U_ST_ADDRESS_NAME_2", Address_Index, BP.Addresses.AddressName2);
                    DB_Address.SetValue("U_ST_ADDRESS_NAME_3", Address_Index, BP.Addresses.AddressName3);
                    DB_Address.SetValue("U_ST_STREET_NO", Address_Index, BP.Addresses.StreetNo);

                }

                ((Matrix)Member_Card_Form.Items.Item("20").Specific).LoadFromDataSource();

                /* 
                 SELECT T0."U_ST_CONTACT_ID", T0."U_ST_NAME", T0."U_ST_POSITION", T0."U_ST_ADDRESS", T0."U_ST_TELEPHONE_1"
                , T0."U_ST_TELEPHONE_2", T0."U_ST_MOBILE_PHONE", T0."U_ST_FAX", T0."U_ST_E_MAIL", T0."U_ST_E_MAIL_GROUP",
                T0."U_ST_PAGERU_ST_PAGER", T0."U_ST_REMARKS_1", T0."U_ST_REMARKS_2", T0."U_ST_PASSWORD", T0."U_ST_COUNTRY", T0."U_ST_GENDER", 
                T0."U_ST_PROFESSION", T0."U_ST_CITY_OF_BIRTH", T0."U_ST_CONNECTED_ADDRESS" FROM "KHCFSANDBOX"."@ST_CCI_INDIV_CONT"  T0
                 */
                int Contact_Index = 0;
                if (Is_Corp == true)
                {
                    DBDataSource DB_Contact = Member_Card_Form.DataSources.DBDataSources.Item(UDO_Contact);
                    for (int J = 0; J < BP.ContactEmployees.Count; J++)
                    {
                        BP.ContactEmployees.SetCurrentLine(J);
                        if (BP.ContactEmployees.Name == "")
                        {
                            continue;
                        }
                        if (DB_Contact.GetValue("U_ST_NAME", Contact_Index) != "")
                        {
                            DB_Contact.InsertRecord(Contact_Index);
                            Contact_Index++;
                        }

                        //DB_Contact.InsertRecord(J);
                        //oChild.SetProperty("U_ST_CONTACT_ID", BP.ContactEmployees.id);
                        DB_Contact.SetValue("U_ST_NAME", Contact_Index, BP.ContactEmployees.Name);
                        DB_Contact.SetValue("U_ST_POSITION", Contact_Index, BP.ContactEmployees.Position);
                        DB_Contact.SetValue("U_ST_ADDRESS", Contact_Index, BP.ContactEmployees.Address);
                        DB_Contact.SetValue("U_ST_TELEPHONE_1", Contact_Index, BP.ContactEmployees.Phone1);
                        DB_Contact.SetValue("U_ST_TELEPHONE_2", Contact_Index, BP.ContactEmployees.Phone2);
                        DB_Contact.SetValue("U_ST_MOBILE_PHONE", Contact_Index, BP.ContactEmployees.MobilePhone);
                        DB_Contact.SetValue("U_ST_FAX", Contact_Index, BP.ContactEmployees.Fax);
                        DB_Contact.SetValue("U_ST_E_MAIL", Contact_Index, BP.ContactEmployees.E_Mail);
                        DB_Contact.SetValue("U_ST_E_MAIL_GROUP", Contact_Index, BP.ContactEmployees.EmailGroupCode);
                        DB_Contact.SetValue("U_ST_PAGER", Contact_Index, BP.ContactEmployees.Pager);
                        DB_Contact.SetValue("U_ST_REMARKS_1", Contact_Index, BP.ContactEmployees.Remarks1);
                        DB_Contact.SetValue("U_ST_REMARKS_2", Contact_Index, BP.ContactEmployees.Remarks2);
                        DB_Contact.SetValue("U_ST_PASSWORD", Contact_Index, BP.ContactEmployees.Password);
                        DB_Contact.SetValue("U_ST_COUNTRY", Contact_Index, BP.ContactEmployees.ForeignCountry);
                        DB_Contact.SetValue("U_ST_GENDER", Contact_Index, BP.ContactEmployees.Gender == BoGenderTypes.gt_Male ? "M" : "F");
                        DB_Contact.SetValue("U_ST_PROFESSION", Contact_Index, BP.ContactEmployees.Profession);
                        DB_Contact.SetValue("U_ST_CITY_OF_BIRTH", Contact_Index, BP.ContactEmployees.CityOfBirth);
                        DB_Contact.SetValue("U_ST_CONNECTED_ADDRESS", Contact_Index, BP.ContactEmployees.ConnectedAddressName);
                    }
                     ((Matrix)Member_Card_Form.Items.Item("701").Specific).LoadFromDataSource();
                }
                //GeneralDataParams New_Entry = oGeneralService.Add(oGeneralData);

                //BP.UserFields.Fields.Item("U_ST_MEMBER_CARD").Value = NewCode;
                //BP.CardType = BoCardTypes.cCustomer;
                //if (BP.Update() != 0)
                //{
                //    throw new Logic.Custom_Exception($"Error during update the BP[{company.GetLastErrorDescription()}]");
                //}

                if (Is_Corp == true)
                {
                    Frm_CCI_Corporate_Member_Card.Form_Obj.Load_Depends_Items(Member_Card_Form);
                }
                else
                {
                    Frm_CCI_Member_Card.Form_Obj.Load_Depends_Items(Member_Card_Form);
                }


                Member_Card_Form.Freeze(false);

            }
            catch (Exception ex)
            {
                if (Member_Card_Form != null)
                {
                    Member_Card_Form.Freeze(false);
                }
                throw new Logic.Custom_Exception($"Error during Process the convert the Lead to Member Card[{ex.Message}]");
                //SBO_Application.StatusBar.SetText($"Error during Process the convert the BP to customer[{ex.Message}], All Changes is rollbaked.");
                //SBO_Application.MessageBox($"Error during Process the Create Goods Receipt Logic[{ex.Message}], All Changes is rollbaked");
            }


        }

        internal static string Convert_Lead_To_MemberCard(string CardCode, bool Is_Automatically)
        {
            /// Form form = SBO_Application.Forms.Item(formUID);
            BusinessPartners BP = (BusinessPartners)company.GetBusinessObject(BoObjectTypes.oBusinessPartners);
            BP.GetByKey(CardCode);

            Convert_Lead_To_MemberCard_Validation(BP);

            if (Is_Automatically == false)
            {
                if (SBO_Application.MessageBox("Are you sure you want to convert the BP to customer?", 1, "Yes", "No") != 1)
                {
                    return "";
                }
            }
            int BP_Group = BP.GroupCode;
            string SQL_Group = $@"SELECT T1.""U_ST_TYPE"", T1.""U_ST_CUSTOMER_TYPE"" FROM ""OCRG""  T1 
WHERE ""GroupCode"" = {BP_Group}";
            Recordset RC_Group = Helper.Utility.Execute_Recordset_Query(company, SQL_Group);
            bool Is_Corp = RC_Group.Fields.Item("U_ST_TYPE").Value.ToString() == "C";
            try
            {
                if (Is_Automatically == false)
                {
                    company.StartTransaction();
                    SBO_Application.StatusBar.SetText("Please wait the Convert the BP to customer Process", BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Warning);
                }             
                //Logic.Create_Goods_Receipt_From_GoodsIssue(company, Doc);
                KHCF_Objects KHCF_Object;
                string UDO_Address;
                //string UDO_Contact
                    string UDO_Contact;
                if (Is_Corp)
                {
                    KHCF_Object = KHCF_Objects.CCI_Corporate_Member_Card;
                    UDO_Address = "ST_CCI_CORP_ADDR";
                    UDO_Contact = "ST_CCI_CORP_CONT";
                }
                else
                {
                    KHCF_Object = KHCF_Objects.CCI_Member_Card;
                    UDO_Address = "ST_CCI_INDIV_ADDR";
                    UDO_Contact = "ST_CCI_INDIV_CONT";
                }
                UDO_Definition UDO_Info = Logic.Objects_Logic.All_UDO_Definition.FirstOrDefault(U => U.KHCF_Object == KHCF_Object);

                CompanyService oCmpSrv = company.GetCompanyService();
                string UDO_Code = Utility.Get_UDO_Type_ID(UDO_Info.KHCF_Object);
                GeneralService oGeneralService = oCmpSrv.GetGeneralService(UDO_Code);
                GeneralDataParams oGeneralParams = (GeneralDataParams)oGeneralService.GetDataInterface(GeneralServiceDataInterfaces.gsGeneralDataParams);
                string UDO_Entry_Code = Utility.Get_New_UDO_Code(company, UDO_Info.KHCF_Object);
                //oGeneralParams.SetProperty("Code", UDO_Entry_Code);
                // GeneralData oGeneralData = oGeneralService.GetByParams(oGeneralParams);
                GeneralData oGeneralData = (GeneralData)oGeneralService.GetDataInterface(SAPbobsCOM.GeneralServiceDataInterfaces.gsGeneralData);
                oGeneralData.SetProperty("Code", UDO_Entry_Code);
                oGeneralData.SetProperty("U_ST_CREATOR", company.UserName);
                oGeneralData.SetProperty("U_ST_CREATION_DATE", DateTime.Today.ToString("yyyyMMdd"));
                oGeneralData.SetProperty("U_ST_APPROVAL_STATUS", "A");

                oGeneralData.SetProperty("U_ST_ACCOUNT_MANAGER", BP.SalesPersonCode.ToString());// BP.UserFields.Fields.Item("U_ST_ACCOUNT_MANAGER").Value);
                oGeneralData.SetProperty("U_ST_CHANNEL", BP.UserFields.Fields.Item("U_ST_CHANNEL").Value);
                oGeneralData.SetProperty("U_ST_SUB_CHANNEL", BP.UserFields.Fields.Item("U_ST_SUB_CHANNEL").Value);
                
                oGeneralData.SetProperty("U_ST_EMAIL", BP.EmailAddress);
                
                oGeneralData.SetProperty("U_ST_TEL1", BP.Phone1);
                oGeneralData.SetProperty("U_ST_CURRENCY", BP.Currency);
                oGeneralData.SetProperty("U_ST_BP_CODE", BP.CardCode);
                oGeneralData.SetProperty("U_ST_CUSTOMER_GROUP", BP.GroupCode.ToString());
                if (Is_Corp)
                {
                    oGeneralData.SetProperty("U_ST_SECTOR", BP.UserFields.Fields.Item("U_ST_MAIN_SECTOR").Value);
                    oGeneralData.SetProperty("U_ST_CORPORATE_ARABIC_NAME", BP.UserFields.Fields.Item("CardName").Value);
                    oGeneralData.SetProperty("U_ST_CORPORATE_ENGLISH_NAME", BP.UserFields.Fields.Item("CardFName").Value);
                    oGeneralData.SetProperty("U_ST_GENERAL_MANAGER", BP.UserFields.Fields.Item("U_ST_GENERAL_MANAGER").Value);
                    oGeneralData.SetProperty("U_ST_BROKER", BP.UserFields.Fields.Item("U_ST_BROKER1").Value);
                    oGeneralData.SetProperty("U_ST_CORPORATE_NATIONAL_ID", BP.UserFields.Fields.Item("U_ST_CORPORATE_NATIONAL_ID").Value);
                    oGeneralData.SetProperty("U_ST_MOBILE_1", BP.Cellular);
                    oGeneralData.SetProperty("U_ST_TEL_2", BP.Phone2);
                    oGeneralData.SetProperty("U_ST_REMARK", BP.FreeText);
                    oGeneralData.SetProperty("U_ST_MOBILE_2", BP.Fax);

                }
                else
                {

                    oGeneralData.SetProperty("U_ST_PARENT_TYPE", "N");
                    oGeneralData.SetProperty("U_ST_NATIONAL_ID", BP.UserFields.Fields.Item("U_ST_NATIONAL_ID").Value);
                    oGeneralData.SetProperty("U_ST_FIRST_NAME_AR", BP.UserFields.Fields.Item("U_ST_FIRST_NAME_AR").Value);
                    oGeneralData.SetProperty("U_ST_FATHER_NAME_AR", BP.UserFields.Fields.Item("U_ST_FATHER_NAME_AR").Value);
                    oGeneralData.SetProperty("U_ST_MIDDLE_NAME_AR", BP.UserFields.Fields.Item("U_ST_MIDDLE_NAME_AR").Value);
                    oGeneralData.SetProperty("U_ST_SURNAME_AR", BP.UserFields.Fields.Item("U_ST_SURNAME_AR").Value);
                    oGeneralData.SetProperty("U_ST_FULL_NAME_AR", Utility.Get_Full_Name(BP.UserFields.Fields.Item("U_ST_FIRST_NAME_AR").Value.ToString(), BP.UserFields.Fields.Item("U_ST_FATHER_NAME_AR").Value.ToString(), BP.UserFields.Fields.Item("U_ST_MIDDLE_NAME_AR").Value.ToString(), BP.UserFields.Fields.Item("U_ST_SURNAME_AR").Value.ToString()));
                    oGeneralData.SetProperty("U_ST_FIRST_NAME_EN", BP.UserFields.Fields.Item("U_ST_FIRST_NAME_EN").Value);
                    oGeneralData.SetProperty("U_ST_FATHER_NAME_EN", BP.UserFields.Fields.Item("U_ST_FATHER_NAME_EN").Value);
                    oGeneralData.SetProperty("U_ST_MIDDLE_NAME_EN", BP.UserFields.Fields.Item("U_ST_MIDDLE_NAME_EN").Value);
                    oGeneralData.SetProperty("U_ST_SURNAME_EN", BP.UserFields.Fields.Item("U_ST_SURNAME_EN").Value);
                    oGeneralData.SetProperty("U_ST_FULL_NAME_EN", Utility.Get_Full_Name(BP.UserFields.Fields.Item("U_ST_FIRST_NAME_EN").Value.ToString(), BP.UserFields.Fields.Item("U_ST_FATHER_NAME_EN").Value.ToString(), BP.UserFields.Fields.Item("U_ST_MIDDLE_NAME_EN").Value.ToString(), BP.UserFields.Fields.Item("U_ST_SURNAME_EN").Value.ToString()));
                    oGeneralData.SetProperty("U_ST_DATE_OF_BIRTH", BP.UserFields.Fields.Item("U_ST_DATE_OF_BIRTH").Value);
                    oGeneralData.SetProperty("U_ST_GENDER", BP.UserFields.Fields.Item("U_ST_GENDER").Value);
                    oGeneralData.SetProperty("U_ST_NATIONALITY", BP.UserFields.Fields.Item("U_ST_NATIONALITY").Value);
                    oGeneralData.SetProperty("U_ST_RESIDENCY", BP.UserFields.Fields.Item("U_ST_RESIDENCY").Value);

                    string s = BP.UserFields.Fields.Item("U_ST_RESIDENCY").Value.ToString();
                    oGeneralData.SetProperty("U_ST_TITLE", BP.UserFields.Fields.Item("U_ST_JOB_TITLE").Value);
                    oGeneralData.SetProperty("U_ST_PERSONAL_ID", BP.UserFields.Fields.Item("U_ST_PERSONAL_ID").Value);
                    oGeneralData.SetProperty("U_ST_PASSPORT_ID", BP.UserFields.Fields.Item("U_ST_PASSPORT_ID").Value);
                    oGeneralData.SetProperty("U_ST_BROKER1", BP.UserFields.Fields.Item("U_ST_BROKER1").Value);
                    oGeneralData.SetProperty("U_ST_TEL2", BP.Phone2);
                    oGeneralData.SetProperty("U_ST_MOBILE", BP.Cellular);

                }

                SAPbobsCOM.GeneralDataCollection oChildren = oGeneralData.Child(UDO_Address);
                for (int J = 0; J < BP.Addresses.Count; J++)
                {
                    BP.Addresses.SetCurrentLine(J);
                    if (BP.Addresses.AddressName == "")
                    {
                        continue;
                    }
                    GeneralData oChild = oChildren.Add();

                    oChild.SetProperty("U_ST_ADDRESS_NAME", BP.Addresses.AddressName);
                    oChild.SetProperty("U_ST_STREET", BP.Addresses.Street);
                    oChild.SetProperty("U_ST_BLOCK", BP.Addresses.Block);
                    oChild.SetProperty("U_ST_ZIP_CODE", BP.Addresses.ZipCode);
                    oChild.SetProperty("U_ST_CITY", BP.Addresses.City);
                    oChild.SetProperty("U_ST_COUNTY", BP.Addresses.County);
                    oChild.SetProperty("U_ST_COUNTRY", BP.Addresses.Country);
                    oChild.SetProperty("U_ST_STATE", BP.Addresses.State);
                    oChild.SetProperty("U_ST_BUILDING", BP.Addresses.BuildingFloorRoom);
                    oChild.SetProperty("U_ST_ADDRESS_TYPE", BP.Addresses.AddressType == BoAddressType.bo_ShipTo ? "S" : "B");
                    oChild.SetProperty("U_ST_ADDRESS_NAME_2", BP.Addresses.AddressName2);
                    oChild.SetProperty("U_ST_ADDRESS_NAME_3", BP.Addresses.AddressName3);
                    oChild.SetProperty("U_ST_STREET_NO", BP.Addresses.StreetNo);

                }


                /* 
                 SELECT T0."U_ST_CONTACT_ID", T0."U_ST_NAME", T0."U_ST_POSITION", T0."U_ST_ADDRESS", T0."U_ST_TELEPHONE_1"
                , T0."U_ST_TELEPHONE_2", T0."U_ST_MOBILE_PHONE", T0."U_ST_FAX", T0."U_ST_E_MAIL", T0."U_ST_E_MAIL_GROUP",
                T0."U_ST_PAGERU_ST_PAGER", T0."U_ST_REMARKS_1", T0."U_ST_REMARKS_2", T0."U_ST_PASSWORD", T0."U_ST_COUNTRY", T0."U_ST_GENDER", 
                T0."U_ST_PROFESSION", T0."U_ST_CITY_OF_BIRTH", T0."U_ST_CONNECTED_ADDRESS" FROM "KHCFSANDBOX"."@ST_CCI_INDIV_CONT"  T0
                 */
                SAPbobsCOM.GeneralDataCollection oChildren1 = oGeneralData.Child(UDO_Contact);
                for (int J = 0; J < BP.ContactEmployees.Count; J++)
                {
                    BP.ContactEmployees.SetCurrentLine(J);
                    if (BP.ContactEmployees.Name == "")
                    {
                        continue;
                    }
                    GeneralData oChild = oChildren1.Add();

                    //oChild.SetProperty("U_ST_CONTACT_ID", BP.ContactEmployees.id);
                    oChild.SetProperty("U_ST_NAME", BP.ContactEmployees.Name);
                    oChild.SetProperty("U_ST_POSITION", BP.ContactEmployees.Position);
                    oChild.SetProperty("U_ST_ADDRESS", BP.ContactEmployees.Address);
                    oChild.SetProperty("U_ST_TELEPHONE_1", BP.ContactEmployees.Phone1);
                    oChild.SetProperty("U_ST_TELEPHONE_2", BP.ContactEmployees.Phone2);
                    oChild.SetProperty("U_ST_MOBILE_PHONE", BP.ContactEmployees.MobilePhone);
                    oChild.SetProperty("U_ST_FAX", BP.ContactEmployees.Fax);
                    oChild.SetProperty("U_ST_E_MAIL", BP.ContactEmployees.E_Mail);
                    oChild.SetProperty("U_ST_E_MAIL_GROUP", BP.ContactEmployees.EmailGroupCode);
                    oChild.SetProperty("U_ST_PAGER", BP.ContactEmployees.Pager);
                    oChild.SetProperty("U_ST_REMARKS_1", BP.ContactEmployees.Remarks1);
                    oChild.SetProperty("U_ST_REMARKS_2", BP.ContactEmployees.Remarks2);
                    oChild.SetProperty("U_ST_PASSWORD", BP.ContactEmployees.Password);
                    oChild.SetProperty("U_ST_COUNTRY", BP.ContactEmployees.ForeignCountry);
                    oChild.SetProperty("U_ST_GENDER", BP.ContactEmployees.Gender == BoGenderTypes.gt_Male ? "M": "F");
                    oChild.SetProperty("U_ST_PROFESSION", BP.ContactEmployees.Profession);
                    oChild.SetProperty("U_ST_CITY_OF_BIRTH", BP.ContactEmployees.CityOfBirth);
                    oChild.SetProperty("U_ST_CONNECTED_ADDRESS", BP.ContactEmployees.ConnectedAddressName);
                   
                   

                }

                GeneralDataParams New_Entry = oGeneralService.Add(oGeneralData);

                string NewCode = New_Entry.GetProperty("Code").ToString();

                BP.UserFields.Fields.Item("U_ST_MEMBER_CARD").Value = NewCode;
                BP.CardType = BoCardTypes.cCustomer;
                if (BP.Update() != 0)
                {
                    throw new Logic.Custom_Exception($"Error during update the BP[{company.GetLastErrorDescription()}]");
                }

                if (Is_Automatically == false)
                {
                    company.EndTransaction(BoWfTransOpt.wf_Commit);
                    SBO_Application.StatusBar.SetText("Create convert the BP to customer Process completed successfully!", BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Success);
                }
                return NewCode;
            }
            catch (Exception ex)
            {
                try
                {
                    company.EndTransaction(BoWfTransOpt.wf_RollBack);
                }
                catch (Exception)
                { }
                throw new Logic.Custom_Exception($"Error during Process the convert the BP to customer[{ex.Message}]");
                //SBO_Application.StatusBar.SetText($"Error during Process the convert the BP to customer[{ex.Message}], All Changes is rollbaked.");
                //SBO_Application.MessageBox($"Error during Process the Create Goods Receipt Logic[{ex.Message}], All Changes is rollbaked");
            }


        }

        private static void Convert_Lead_To_MemberCard_Validation(BusinessPartners BP)
        {
            if (BP.UserFields.Fields.Item("U_ST_MEMBER_CARD").Value.ToString() != "")
            {
                throw new Logic.Custom_Exception($"The BP already linked with the Member Card [{BP.UserFields.Fields.Item("U_ST_MEMBER_CARD").Value.ToString()}]");
            }

            string SQL_Opp = $@"SELECT T0.""Name"", T0.""Status"" FROM OOPR T0 WHERE T0.""CardCode"" ='{BP.CardCode}'";
            Recordset RC_Opp = Helper.Utility.Execute_Recordset_Query(company, SQL_Opp);
            if(RC_Opp.RecordCount== 0)
            {
                throw new Logic.Custom_Exception($"We can't convert the lead to Member card because there is no Sales Opportunity for the BP[{BP.CardCode}].");
            }

            if (RC_Opp.Fields.Item("Name").Value.ToString() != "Enrollment")
            {
                throw new Logic.Custom_Exception($"We can't convert the lead to Member card because the Sales Opportunity type is not [Enrollment].");
            }

            if (RC_Opp.Fields.Item("Status").Value.ToString() != "W")
            {
                throw new Logic.Custom_Exception($"We can't convert the lead to Member card because the Sales Opportunity status is not [Won].");
            }

        }

        internal static bool SBO_Application_FormDataEvent(ref BusinessObjectInfo BusinessObjectInfo)
        {
            bool BubbleEvent = true;
            //if (BusinessObjectInfo.FormTypeEx != "720")//Goods Issue form
            //{
            //    return BubbleEvent;
            //}
            try
            {
                if (BusinessObjectInfo.FormTypeEx == "134")
                {
                    //if (BusinessObjectInfo.EventType == BoEventTypes.et_FORM_DATA_ADD && !BusinessObjectInfo.BeforeAction && BusinessObjectInfo.ActionSuccess)
                    //{

                    //    string XML = BusinessObjectInfo.ObjectKey;
                    //    System.Xml.XmlDocument XML_Doc = new System.Xml.XmlDocument();
                    //    XML_Doc.LoadXml(XML);
                    //    string DocEntry = XML_Doc.GetElementsByTagName("DocEntry")[0].InnerText;

                    //    Convert_Lead_To_MemberCard(BusinessObjectInfo.FormUID, int.Parse(DocEntry));
                    //}

                    if (BusinessObjectInfo.EventType == BoEventTypes.et_FORM_DATA_LOAD && !BusinessObjectInfo.BeforeAction)
                    {
                        Form form = SBO_Application.Forms.Item(BusinessObjectInfo.FormUID);
                        Add_BP_Items(form);
                        Check_BP_Visibility(BusinessObjectInfo);
                    }
                    if ((BusinessObjectInfo.EventType == BoEventTypes.et_FORM_DATA_UPDATE || BusinessObjectInfo.EventType == BoEventTypes.et_FORM_DATA_ADD)
                        && BusinessObjectInfo.BeforeAction)
                    {
                        BubbleEvent = Validate_Data(BusinessObjectInfo);
                        if (!BubbleEvent)
                        {
                            return BubbleEvent;
                        }
                    }
                }

                if (BusinessObjectInfo.FormTypeEx == "1320000022")
                {

                    if (BusinessObjectInfo.EventType == BoEventTypes.et_FORM_DATA_LOAD && !BusinessObjectInfo.BeforeAction)
                    {
                        Form form = SBO_Application.Forms.Item(BusinessObjectInfo.FormUID);
                        Add_Campaign_Items(form);
                        Check_Campaign_Column_Visibility(form);
                    }

                }

                if (BusinessObjectInfo.FormTypeEx == "179")
                {

                    if (BusinessObjectInfo.EventType == BoEventTypes.et_FORM_DATA_LOAD && !BusinessObjectInfo.BeforeAction)
                    {
                        Form form = SBO_Application.Forms.Item(BusinessObjectInfo.FormUID);
                        //Add_Credit_Note_Items(form);
                       // Check_Credit_Note_Visibility(BusinessObjectInfo);
                    }

                    if (BusinessObjectInfo.EventType == BoEventTypes.et_FORM_DATA_ADD && !BusinessObjectInfo.BeforeAction && BusinessObjectInfo.ActionSuccess)
                    {
                        //string XML = BusinessObjectInfo.ObjectKey;
                        //System.Xml.XmlDocument XML_Doc = new System.Xml.XmlDocument();
                        //XML_Doc.LoadXml(XML);
                        //string DocEntry = XML_Doc.GetElementsByTagName("DocEntry")[0].InnerText;

                        //string SQL = $@"SELECT T0.""DocNum"" FROM ORIN T0 WHERE ""DocEntry"" = {DocEntry}";
                        //Recordset RC = Helper.Utility.Execute_Recordset_Query(company, SQL);
                        //Create_Revenue_Realization_Cancellation(int.Parse(DocEntry), (int)RC.Fields.Item("DocNum").Value);
                    }

                }

                if (BusinessObjectInfo.FormTypeEx == "141")
                {

                    if (BusinessObjectInfo.EventType == BoEventTypes.et_FORM_DATA_LOAD && !BusinessObjectInfo.BeforeAction)
                    {
                        Form form = SBO_Application.Forms.Item(BusinessObjectInfo.FormUID);
                        Add_AP_Invoice_Items(form);
                        Check_AP_Invoice_Visibility(BusinessObjectInfo);
                    }

                    if (BusinessObjectInfo.EventType == BoEventTypes.et_FORM_DATA_ADD && !BusinessObjectInfo.BeforeAction && BusinessObjectInfo.ActionSuccess)
                    {
                        string XML = BusinessObjectInfo.ObjectKey;
                        System.Xml.XmlDocument XML_Doc = new System.Xml.XmlDocument();
                        XML_Doc.LoadXml(XML);
                        string DocEntry = XML_Doc.GetElementsByTagName("DocEntry")[0].InnerText;

                        string SQL = $@"SELECT T0.""DocNum"", T0.U_ST_PATIENT_CODE FROM OPCH T0 WHERE ""DocEntry"" = {DocEntry}";
                        Recordset RC = Helper.Utility.Execute_Recordset_Query(company, SQL);
                        string Patient_Code = RC.Fields.Item("U_ST_PATIENT_CODE").Value.ToString();
                        if (Patient_Code != "")
                        {
                            Create_KFCC_Invoice_JE(int.Parse(DocEntry), (int)RC.Fields.Item("DocNum").Value, Patient_Code);
                        }
                    }

                }

                if (BusinessObjectInfo.FormTypeEx == "170") //Incoming Payment
                {
                    if (BusinessObjectInfo.EventType == BoEventTypes.et_FORM_DATA_ADD && !BusinessObjectInfo.BeforeAction && BusinessObjectInfo.ActionSuccess)
                    {
//                        string XML = BusinessObjectInfo.ObjectKey;
//                        System.Xml.XmlDocument XML_Doc = new System.Xml.XmlDocument();
//                        XML_Doc.LoadXml(XML);
//                        string DocEntry = XML_Doc.GetElementsByTagName("DocEntry")[0].InnerText;

//                        string SQL = $@"SELECT T0.""DocNum"", T0.U_ST_MEMBERSHIP_CODE, T0.U_ST_MEMBERSHIP_TYPE, T0.""DocDate""
//FROM ORCT T0 WHERE ""DocEntry"" = {DocEntry}";
//                        Recordset RC = Helper.Utility.Execute_Recordset_Query(company, SQL);
//                        string Membership_Code = RC.Fields.Item("U_ST_MEMBERSHIP_CODE").Value.ToString();
//                        if (Membership_Code != "")
//                        {
//                            string Membership_Type = RC.Fields.Item("U_ST_MEMBERSHIP_TYPE").Value.ToString();
//                            UDO_Definition UDO_Info;
//                            if (Membership_Type == "I")
//                            {
//                                UDO_Info = Logic.Objects_Logic.All_UDO_Definition.FirstOrDefault(U => U.KHCF_Object == KHCF_Objects.Individual_Membership);
//                            }
//                            else
//                            {
//                                UDO_Info = Logic.Objects_Logic.All_UDO_Definition.FirstOrDefault(U => U.KHCF_Object == KHCF_Objects.Corporate_Membership);
//                            }

//                            Field_Data Fld_Date = new Field_Data() { Field_Name = "U_ST_COLLECTION_DATE", Value = (DateTime)RC.Fields.Item("U_ST_COLLECTION_DATE").Value };
//                            Field_Data Fld_Number = new Field_Data() { Field_Name = "U_ST_RECEIPT_VOUCHER_NUMBER", Value = (DateTime)RC.Fields.Item("DocNum").Value };
//                            Utility.Update_UDO(company, UDO_Info, Membership_Code, new Field_Data[] { Fld_Date, Fld_Number });

//                        }


                    }

                }

                if (BusinessObjectInfo.FormTypeEx == "60090")// Invoice For Donation
                {
                    if (BusinessObjectInfo.EventType == BoEventTypes.et_FORM_DATA_ADD && BusinessObjectInfo.ActionSuccess)
                    {
                        Form form = SBO_Application.Forms.Item(BusinessObjectInfo.FormUID);
                        string Code = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_ACTUAL_DONATION_CODE", 0);
                        if (!string.IsNullOrEmpty(Code))
                        {
                            string SQL_Entry = $@"SELECT T0.""DocEntry"" FROM OINV T0 WHERE T0.""U_ST_ACTUAL_DONATION_CODE"" ='{Code}'";
                            Recordset RC = Helper.Utility.Execute_Recordset_Query(company, SQL_Entry);
                            if (RC.RecordCount > 0)
                            {
                                string Doc_Entry = RC.Fields.Item(0).Value.ToString();
                                UDO_Definition UDO_Info = Logic.Objects_Logic.All_UDO_Definition.FirstOrDefault(U => U.KHCF_Object == KHCF_Objects.Actual_Donations);
                                Field_Data Fld = new Field_Data() { Field_Name = "U_ST_INVOICE_NUMBER", Value = Doc_Entry };
                                Utility.Update_UDO(company, UDO_Info, Code, new Field_Data[] { Fld });
                            }
                        }
                        //Add_Credit_Note_Items(form);
                        form.Close();
                    }
                    
                }

                if (BusinessObjectInfo.FormTypeEx == "320")
                {
                    if ((BusinessObjectInfo.EventType == BoEventTypes.et_FORM_DATA_UPDATE || BusinessObjectInfo.EventType == BoEventTypes.et_FORM_DATA_ADD) && !BusinessObjectInfo.BeforeAction && BusinessObjectInfo.ActionSuccess)
                    {
                        Form form = SBO_Application.Forms.Item(BusinessObjectInfo.FormUID);
                        string Status = form.DataSources.DBDataSources.Item(0).GetValue("Status", 0);
                        string Code = form.DataSources.DBDataSources.Item(0).GetValue("OpprId", 0);
                        if (!string.IsNullOrEmpty(Code) && (Status == "W" || Status == "L"))
                        {
                            string SQL_Entry = $@"SELECT T0.""Code"" FROM ""@ST_WON_GRANTS"" T0 WHERE T0.""U_ST_OPPORTUNITY_ID"" ='{Code}'";
                            Recordset RC = Helper.Utility.Execute_Recordset_Query(company, SQL_Entry);
                            for(int i =0;i<RC.RecordCount;i++)
                            {
                                string Grant_Code = RC.Fields.Item(0).Value.ToString();
                                UDO_Definition UDO_Info = Logic.Objects_Logic.All_UDO_Definition.FirstOrDefault(U => U.KHCF_Object == KHCF_Objects.Won_Grants);
                                Field_Data Fld = new Field_Data() { Field_Name = "U_ST_STATUS", Value = Status };
                                Utility.Update_UDO(company, UDO_Info, Grant_Code, new Field_Data[] { Fld });
                                RC.MoveNext();
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Loader.New_Msg = ex.Message;

                BubbleEvent = false;
            }
            return BubbleEvent;

        }

        private static bool Validate_Data(BusinessObjectInfo businessObjectInfo)
        {
              Form form = SBO_Application.Forms.Item(businessObjectInfo.FormUID);
            if (form.DataSources.DBDataSources.Item(0).GetValue("CardType", 0) == "C")
            {
                throw new Exception("We can Update Lead BP only in System Form.");
            }
            return true;
        }

        private static void Check_AP_Invoice_Visibility(BusinessObjectInfo businessObjectInfo)
        {
            Form form = SBO_Application.Forms.Item(businessObjectInfo.FormUID);
            Item Btn_GR_GEN = form.Items.Item("ST_CRT_JE");
            //string DocEntry = form.DataSources.DBDataSources.Item(0).GetValue("DocEntry", 0);

            string KFCH_Vendor_Code = Configurations.Get_KFCH_Vendor_Code(company);
            if (form.DataSources.DBDataSources.Item(0).GetValue("CardCode", 0) != KFCH_Vendor_Code)
            {
                Btn_GR_GEN.Visible = false;
                return;
            }
            if (form.DataSources.DBDataSources.Item(0).GetValue("U_ST_PATIENT_CODE", 0) == "")
            {
                Btn_GR_GEN.Visible = false;
                return;
            }
            Btn_GR_GEN.Visible = true;


        }

        private static void Add_AP_Invoice_Items(Form form)
        {
            try
            {

                Item Btn_Revenue_Realization = form.Items.Add("ST_CRT_JE", BoFormItemTypes.it_BUTTON);

                Item Btn_Cancel = form.Items.Item("2");
                Btn_Revenue_Realization.Top = Btn_Cancel.Top;
                Btn_Revenue_Realization.Width = Convert.ToInt32(Math.Round(Btn_Cancel.Width * 1.5));
                Btn_Revenue_Realization.Height = Btn_Cancel.Height;
                Btn_Revenue_Realization.Left = (Btn_Cancel.Width * 4) + 12;
                Btn_Revenue_Realization.LinkTo = "2";

                //Btn_Def_Exp.SetAutoManagedAttribute(BoAutoManagedAttr.ama_Visible, -1, BoModeVisualBehavior.mvb_False);
                // Btn_Def_Exp.SetAutoManagedAttribute(BoAutoManagedAttr.ama_Visible, 1, BoModeVisualBehavior.mvb_True);
                Btn_Revenue_Realization.Visible = false;
                ((Button)Btn_Revenue_Realization.Specific).Caption = "Create JE";

            }
            catch (Exception)
            {

            }

        }

        private static void Check_Credit_Note_Visibility(BusinessObjectInfo businessObjectInfo)
        {
            Form form = SBO_Application.Forms.Item(businessObjectInfo.FormUID);
            Item Btn_GR_GEN = form.Items.Item("ST_CAN_REL");
            string DocEntry = form.DataSources.DBDataSources.Item(0).GetValue("DocEntry", 0);

//            string SQL = $@"SELECT T0.U_ST_REVENUE_REALIZATION_JE 
//FROM RIN1 T0 INNER JOIN INV1 T1 ON  T0.""BaseEntry"" = T1.""DocEntry"" AND T0.""BaseType"" = 13 AND T0.""BaseLine"" = T1.""LineNum"" 
//WHERE T0.""DocEntry"" = {DocEntry} AND  T0.""U_ST_REVENUE_REALIZATION_JE"" <> 0 ";
            string SQL = $@"SELECT T0.U_ST_REVENUE_REALIZATION_JE 
FROM RIN1 T0 
WHERE T0.""DocEntry"" = {DocEntry} AND  T0.""U_ST_REVENUE_REALIZATION_JE"" <> 0 AND IFNULL(T0.U_ST_CANCEL_REVENUE_REALIZATION_JE,0) = 0";
            Recordset RC = Helper.Utility.Execute_Recordset_Query(company, SQL);
            if (RC.RecordCount != 0)
            {
                Btn_GR_GEN.Visible = true;
            }
            else
            {
                Btn_GR_GEN.Visible = false;
            }

        }

        private static void Add_Credit_Note_Items(Form form)
        {
            try
            {

                Item Btn_Revenue_Realization = form.Items.Add("ST_CAN_REL", BoFormItemTypes.it_BUTTON);

                Item Btn_Cancel = form.Items.Item("2");
                Btn_Revenue_Realization.Top = Btn_Cancel.Top;
                Btn_Revenue_Realization.Width = Convert.ToInt32(Math.Round(Btn_Cancel.Width * 2.5));
                Btn_Revenue_Realization.Height = Btn_Cancel.Height;
                Btn_Revenue_Realization.Left = (Btn_Cancel.Width * 4) + 12;
                Btn_Revenue_Realization.LinkTo = "2";

                //Btn_Def_Exp.SetAutoManagedAttribute(BoAutoManagedAttr.ama_Visible, -1, BoModeVisualBehavior.mvb_False);
                // Btn_Def_Exp.SetAutoManagedAttribute(BoAutoManagedAttr.ama_Visible, 1, BoModeVisualBehavior.mvb_True);
                Btn_Revenue_Realization.Visible = false;
                ((Button)Btn_Revenue_Realization.Specific).Caption = "Cancel Revenue Realization";

            }
            catch (Exception)
            {

            }

        }

        private static void Check_BP_Visibility(BusinessObjectInfo businessObjectInfo)
        {
            Form form = SBO_Application.Forms.Item(businessObjectInfo.FormUID);
            Item Btn_GR_GEN = form.Items.Item("ST_CON_LED");
            string CardType = form.DataSources.DBDataSources.Item(0).GetValue("CardType", 0);


            if (CardType == "L")
            {
            string SQL = $@"Select U_ST_CAN_CONVERT_LEAD_TO_CUSTOMER from OUSR 
WHERE USER_CODE = '{company.UserName}'";

            Recordset RC = Helper.Utility.Execute_Recordset_Query(company, SQL);
                if (RC.Fields.Item(0).Value.ToString() == "Y")
                {
                Btn_GR_GEN.Visible = true;
                }
                else
                {
                    Btn_GR_GEN.Visible = false;
                }
            }
            else
            {
                Btn_GR_GEN.Visible = false;
            }
        }

        private static void Add_BP_Items(Form form)
        {

            try
            {

                Item Btn_GR_GEN = form.Items.Add("ST_CON_LED", BoFormItemTypes.it_BUTTON);

                Item Btn_Cancel = form.Items.Item("2");
                Btn_GR_GEN.Top = Btn_Cancel.Top;
                Btn_GR_GEN.Width = Convert.ToInt32(Math.Round(Btn_Cancel.Width * 2.5));
                Btn_GR_GEN.Height = Btn_Cancel.Height;
                Btn_GR_GEN.Left = (Btn_Cancel.Width * 4) + 12;
                Btn_GR_GEN.LinkTo = "2";

                //Btn_Def_Exp.SetAutoManagedAttribute(BoAutoManagedAttr.ama_Visible, -1, BoModeVisualBehavior.mvb_False);
                // Btn_Def_Exp.SetAutoManagedAttribute(BoAutoManagedAttr.ama_Visible, 1, BoModeVisualBehavior.mvb_True);
                Btn_GR_GEN.Visible = false;
                ((Button)Btn_GR_GEN.Specific).Caption = "Convert Lead to Member Card";

            }
            catch (Exception)
            {

            }
        }

        private static void Add_Campaign_Items(Form form)
        {

            try
            {
                Item Cmb_Program_Item = form.Items.Add("ST_CMBPROG", BoFormItemTypes.it_COMBO_BOX);

                Item Txt_Campaign_Name = form.Items.Item("1320000012");
                Cmb_Program_Item.Height = Txt_Campaign_Name.Height;
                Cmb_Program_Item.Top = Txt_Campaign_Name.Top - Cmb_Program_Item.Height - 1;
                Cmb_Program_Item.Width = Txt_Campaign_Name.Width;
                Cmb_Program_Item.Left = Txt_Campaign_Name.Left;
                Cmb_Program_Item.LinkTo = Txt_Campaign_Name.UniqueID;

                ComboBox Cmb_Program = (ComboBox)Cmb_Program_Item.Specific;
                Cmb_Program.DataBind.SetBound(true, "OCPN", "U_ST_PROGRAM");
                string SQL = $@"SELECT T0.""Code"", T0.""Name"" FROM ""@ST_PROGRAM_TOOL""  T0";
                Helper.Utility.Fill_One_ComboBoxBySQL(company, form, Cmb_Program_Item.UniqueID, SQL, true);
                Cmb_Program_Item.DisplayDesc = true;

                Item Lbl_Program = form.Items.Add("ST_LBLPROG", BoFormItemTypes.it_STATIC);
                Item Lbl_Campaign_Name = form.Items.Item("1320000011");
                Lbl_Program.Height = Lbl_Campaign_Name.Height;
                Lbl_Program.Top = Lbl_Campaign_Name.Top - Lbl_Campaign_Name.Height - 1;
                Lbl_Program.Width = Lbl_Campaign_Name.Width;
                Lbl_Program.Left = Lbl_Campaign_Name.Left;
                Lbl_Program.LinkTo = Cmb_Program_Item.UniqueID;

                ((StaticText)Lbl_Program.Specific).Caption = "Program";

                //here 

                Item Lbl_OCC = form.Items.Add("ST_LBLOCN", BoFormItemTypes.it_STATIC);
                Item Txt_Campaign_Remark = form.Items.Item("1320000020");
                Lbl_OCC.Height = Txt_Campaign_Remark.Height;
                Lbl_OCC.Top = Txt_Campaign_Remark.Top + Txt_Campaign_Remark.Height + 1;
                Lbl_OCC.Width = Txt_Campaign_Remark.Width;
                Lbl_OCC.Left = Txt_Campaign_Remark.Left;

                Item Cmb_Occian_Item = form.Items.Add("ST_CMBOCN", BoFormItemTypes.it_COMBO_BOX);

               // Item Txt_Campaign_Remark = form.Items.Item("1320000020");
                Cmb_Occian_Item.Height = Lbl_OCC.Height;
                Cmb_Occian_Item.Top = Lbl_OCC.Top + Cmb_Occian_Item.Height + 1;
                Cmb_Occian_Item.Width = Lbl_OCC.Width;
                Cmb_Occian_Item.Left = Lbl_OCC.Left;
                Cmb_Occian_Item.LinkTo = Txt_Campaign_Remark.UniqueID;

                ComboBox Cmb_Occian = (ComboBox)Cmb_Program_Item.Specific;
                Cmb_Occian.DataBind.SetBound(true, "OCPN", "U_ST_OCC");
                string SQL2 = $@"SELECT T0.""Code"", T0.""Name"" FROM ""@ST_OCCASION""  T0";
                Helper.Utility.Fill_One_ComboBoxBySQL(company, form, Cmb_Occian_Item.UniqueID, SQL2, true);
                Cmb_Occian_Item.DisplayDesc = true;

               
                Lbl_OCC.LinkTo = Cmb_Occian_Item.UniqueID;

                ((StaticText)Lbl_OCC.Specific).Caption = "occasion ";



            }
            catch (Exception ex)
            {

            }
        }

        private static void Add_Bin_Location_Items(Form form)
        {

            try
            {
                Item Cmb_Program_Item = form.Items.Add("ST_IN_DATE", BoFormItemTypes.it_EDIT);
                
                EditText Cmb_Program_Item_edit = (EditText)Cmb_Program_Item.Specific;
                Cmb_Program_Item_edit.DataBind.SetBound(true, "OBIN", "U_ST_IN_DATE");
                Item Txt_Bracnh = form.Items.Item("1470000017");
                Cmb_Program_Item.Height = Txt_Bracnh.Height;
                Cmb_Program_Item.Top = Txt_Bracnh.Top;
                Cmb_Program_Item.Width = Txt_Bracnh.Width;
                Cmb_Program_Item.Left = Txt_Bracnh.Left + Cmb_Program_Item.Width + 10;
                Cmb_Program_Item.LinkTo = Txt_Bracnh.UniqueID;

                EditText c = (EditText)Cmb_Program_Item.Specific;
                c.DataBind.SetBound(true, "OBIN", "U_ST_IN_DATE");

                Item Lbl_Program = form.Items.Add("ST_LBLINS", BoFormItemTypes.it_STATIC);
                Item Lbl_Branch_Name = form.Items.Item("1470000015");
                Lbl_Program.Height = Lbl_Branch_Name.Height;
                Lbl_Program.Top = Lbl_Branch_Name.Top;
                Lbl_Program.Width = Lbl_Branch_Name.Width;
                Lbl_Program.Left = Lbl_Branch_Name.Left + Lbl_Program.Width + 10 ;
                Lbl_Program.LinkTo = Cmb_Program_Item.UniqueID;
                

                ((StaticText)Lbl_Program.Specific).Caption = "Installation Date";

            }
            catch (Exception ex)
            {

            }
        }

        private static void Add_Item_Group_Items(Form form)
        {

            try
            {
                Item Cmb_Program_Item = form.Items.Add("ST_HP_SHOP", BoFormItemTypes.it_COMBO_BOX);

                Item Txt_Bracnh = form.Items.Item("124");
                Cmb_Program_Item.Height = Txt_Bracnh.Height;
                Cmb_Program_Item.Top = Txt_Bracnh.Top - Cmb_Program_Item.Height - 5;
                Cmb_Program_Item.Width = Txt_Bracnh.Width;
                Cmb_Program_Item.Left = Txt_Bracnh.Left;
                Cmb_Program_Item.LinkTo = Txt_Bracnh.UniqueID;
                ComboBox c = (ComboBox) Cmb_Program_Item.Specific;
                c.ValidValues.Add("Y", "Yes");
                c.ValidValues.Add("N", "No");
                c.DataBind.SetBound(true, "OITB", "U_ST_HP_SHOP");
                Cmb_Program_Item.DisplayDesc = true;

                Item Lbl_Program = form.Items.Add("ST_LBLHPSH", BoFormItemTypes.it_STATIC);
                Item Lbl_Branch_Name = form.Items.Item("123");
                Lbl_Program.Height = Lbl_Branch_Name.Height;
                Lbl_Program.Top = Lbl_Branch_Name.Top - Lbl_Program.Height - 5;
                Lbl_Program.Width = Lbl_Branch_Name.Width;
                Lbl_Program.Left = Lbl_Branch_Name.Left;
                Lbl_Program.LinkTo = Cmb_Program_Item.UniqueID;


                ((StaticText)Lbl_Program.Specific).Caption = "Hope Shop";

            }
            catch (Exception ex)
            {

            }
        }

        private static void Add_Item_Items(Form form)
        {

            try
            {
                //Add item Type Combo Box

                Item Lbl_Program = form.Items.Add("ST_LBTYPE", BoFormItemTypes.it_STATIC);
                Item Lbl_Branch_Name = form.Items.Item("114");
                Item Lbl = form.Items.Item("115");
                Lbl_Program.Height = Lbl.Height;
                Lbl_Program.Top = Lbl_Branch_Name.Top;
                Lbl_Program.Width = Lbl_Branch_Name.Width;
                Lbl_Program.Left = Lbl_Branch_Name.Left + Lbl_Program.Width + 20;
                Lbl_Program.FromPane = 6; Lbl_Program.ToPane = 6;

                Item Cmb_Program_Item = form.Items.Add("ST_ITM_TYP", BoFormItemTypes.it_COMBO_BOX);

                Item Txt_Bracnh = form.Items.Item("114");
                Cmb_Program_Item.Height = Txt_Bracnh.Height;
                Cmb_Program_Item.Top = Txt_Bracnh.Top;
                Cmb_Program_Item.Width = Txt_Bracnh.Width;
                Cmb_Program_Item.Left = Txt_Bracnh.Left + Txt_Bracnh.Width + Lbl_Program.Width + 1;
                Cmb_Program_Item.LinkTo = Txt_Bracnh.UniqueID;
                ComboBox c = (ComboBox)Cmb_Program_Item.Specific;
                string SQL = $@"SELECT T0.""Code"", T0.""Name"" FROM ""@ST_ITEM_TYPE""  T0";
                Helper.Utility.Fill_One_ComboBoxBySQL(company, form, Cmb_Program_Item.UniqueID, SQL, true);
                c.DataBind.SetBound(true, "OITM", "U_ST_ITM_TYP");
                Cmb_Program_Item.FromPane = 6; Cmb_Program_Item.ToPane = 6;

                Cmb_Program_Item.DisplayDesc = true;
                //c.DisplayDesc = true;

                Lbl_Program.LinkTo = Cmb_Program_Item.UniqueID;
                ((StaticText)Lbl_Program.Specific).Caption = "Item Type";

                //Add Item Details
                Item Lbl_Program2 = form.Items.Add("ST_LBDET", BoFormItemTypes.it_STATIC);
                Item Lbl_Branch_Name2 = form.Items.Item("186");
                Item Lbl2 = form.Items.Item("185");
                Lbl_Program2.Height = Lbl2.Height;
                Lbl_Program2.Top = Lbl_Branch_Name2.Top;
                Lbl_Program2.Width = Lbl_Branch_Name2.Width;
                Lbl_Program2.Left = Lbl_Branch_Name2.Left + Lbl_Program2.Width + 20;
                Lbl_Program2.FromPane = 6; Lbl_Program2.ToPane = 6;

                Item Cmb_Program_Item2 = form.Items.Add("ST_ITM_DET", BoFormItemTypes.it_COMBO_BOX);

                Item Txt_Bracnh2 = form.Items.Item("186");
                Cmb_Program_Item2.Height = Txt_Bracnh2.Height;
                Cmb_Program_Item2.Top = Txt_Bracnh2.Top;
                Cmb_Program_Item2.Width = Txt_Bracnh2.Width;
                Cmb_Program_Item2.Left = Txt_Bracnh2.Left + Txt_Bracnh2.Width + Lbl_Program2.Width + 1;
                Cmb_Program_Item2.LinkTo = Txt_Bracnh2.UniqueID;
                ComboBox c2 = (ComboBox)Cmb_Program_Item2.Specific;
                string SQL2 = $@"SELECT T0.""Code"", T0.""Name"" FROM ""@ST_ITEM_DETAILS""  T0";
                Helper.Utility.Fill_One_ComboBoxBySQL(company, form, Cmb_Program_Item2.UniqueID, SQL2, true);
                c2.DataBind.SetBound(true, "OITM", "U_ST_ITM_DET");

                Cmb_Program_Item2.DisplayDesc = true;
                Cmb_Program_Item2.FromPane = 6; Cmb_Program_Item2.ToPane = 6;
                //c.DisplayDesc = true;

                Lbl_Program2.LinkTo = Cmb_Program_Item2.UniqueID;
                ((StaticText)Lbl_Program2.Specific).Caption = "Item Details";

                //Add item themes
                Item Lbl_Program3 = form.Items.Add("ST_LBTHM", BoFormItemTypes.it_STATIC);
                Item Lbl_Branch_Name3 = form.Items.Item("35");
                Item Lbl3 = form.Items.Item("32");
                Lbl_Program3.Height = Lbl3.Height;
                Lbl_Program3.Top = Lbl_Branch_Name3.Top;
                Lbl_Program3.Width = Lbl_Branch_Name3.Width;
                Lbl_Program3.Left = Lbl_Branch_Name3.Left + Lbl_Program3.Width + 20;
                Lbl_Program3.FromPane = 6; Lbl_Program3.ToPane = 6;

                Item Cmb_Program_Item3 = form.Items.Add("ST_ITM_THM", BoFormItemTypes.it_COMBO_BOX);

                Item Txt_Bracnh3 = form.Items.Item("35");
                Cmb_Program_Item3.Height = Txt_Bracnh3.Height;
                Cmb_Program_Item3.Top = Txt_Bracnh3.Top;
                Cmb_Program_Item3.Width = Txt_Bracnh3.Width;
                Cmb_Program_Item3.Left = Txt_Bracnh3.Left + Txt_Bracnh3.Width + Lbl_Program3.Width + 1;
                Cmb_Program_Item3.LinkTo = Txt_Bracnh3.UniqueID;
                ComboBox c3 = (ComboBox)Cmb_Program_Item3.Specific;
                string SQL3 = $@"SELECT T0.""Code"", T0.""Name"" FROM ""@ST_ITEM_THEME""  T0";
                Helper.Utility.Fill_One_ComboBoxBySQL(company, form, Cmb_Program_Item3.UniqueID, SQL3, true);
                c3.DataBind.SetBound(true, "OITM", "U_ST_ITM_THM");

                Cmb_Program_Item3.DisplayDesc = true;
                //c.DisplayDesc = true;
                Lbl_Program3.LinkTo = Cmb_Program_Item3.UniqueID;
                Cmb_Program_Item3.FromPane = 6; Cmb_Program_Item3.ToPane = 6;

                Lbl_Program3.LinkTo = Cmb_Program_Item3.UniqueID;
                ((StaticText)Lbl_Program3.Specific).Caption = "Item Themes";

            }
            catch (Exception ex)
            {

            }
        }

        private static void Add_Marketing_Items(Form form,string Label_ID,string Text_ID,string Table_Name)
        {
            try
            {
                Item Lbl_Semester = form.Items.Add("ST_LBTSEM", BoFormItemTypes.it_STATIC);
                Item Lbl_Doc_Date = form.Items.Item(Label_ID);
                Lbl_Semester.Height = Lbl_Doc_Date.Height;
                Lbl_Semester.Top = Lbl_Doc_Date.Top + Lbl_Doc_Date.Height + 1;
                Lbl_Semester.Width = Lbl_Doc_Date.Width;
                Lbl_Semester.Left = Lbl_Doc_Date.Left;

                Item Semster_ITem = form.Items.Add("ST_SEM", BoFormItemTypes.it_EDIT);

                Item Txt_Date = form.Items.Item(Text_ID);
                Semster_ITem.Height = Txt_Date.Height;
                Semster_ITem.Top = Txt_Date.Top + Txt_Date.Height + 1; 
                Semster_ITem.Width = Txt_Date.Width;
                Semster_ITem.Left = Txt_Date.Left;
                Semster_ITem.LinkTo = Txt_Date.UniqueID;
                EditText c = (EditText)Semster_ITem.Specific;
               
                c.DataBind.SetBound(true, Table_Name, "U_ST_SEMESTER");

                Lbl_Semester.LinkTo = Semster_ITem.UniqueID;
                ((StaticText)Lbl_Semester.Specific).Caption = "Semester";
                //c.DisplayDesc = true;

                //Faculity
                Item Lbl_Faculity = form.Items.Add("ST_LBTFAC", BoFormItemTypes.it_STATIC);
                Lbl_Faculity.Height =Lbl_Semester.Height;
                Lbl_Faculity.Top = Lbl_Semester.Top + Lbl_Semester.Height + 1;
                Lbl_Faculity.Width = Lbl_Semester.Width;
                Lbl_Faculity.Left = Lbl_Semester.Left;

                Item Faculity_ITem = form.Items.Add("ST_FAC", BoFormItemTypes.it_EDIT);

                Faculity_ITem.Height = Semster_ITem.Height;
                Faculity_ITem.Top = Semster_ITem.Top + Semster_ITem.Height + 1;
                Faculity_ITem.Width = Semster_ITem.Width;
                Faculity_ITem.Left = Semster_ITem.Left;
                Faculity_ITem.LinkTo = Semster_ITem.UniqueID;
                EditText c2 = (EditText)Faculity_ITem.Specific;

                c2.DataBind.SetBound(true, Table_Name, "U_ST_FACULTY");

                Lbl_Faculity.LinkTo = Faculity_ITem.UniqueID;
                ((StaticText)Lbl_Faculity.Specific).Caption = "Faculity";

                //GPA
                Item Lbl_GPA = form.Items.Add("ST_LBTGPA", BoFormItemTypes.it_STATIC);
                Lbl_GPA.Height = Lbl_Faculity.Height;
                Lbl_GPA.Top = Lbl_Faculity.Top + Lbl_Faculity.Height + 1;
                Lbl_GPA.Width = Lbl_Faculity.Width;
                Lbl_GPA.Left = Lbl_Faculity.Left;

                Item GPA_ITem = form.Items.Add("ST_GPA", BoFormItemTypes.it_EDIT);

                GPA_ITem.Height = Faculity_ITem.Height;
                GPA_ITem.Top = Faculity_ITem.Top + Faculity_ITem.Height + 1;
                GPA_ITem.Width = Faculity_ITem.Width;
                GPA_ITem.Left = Faculity_ITem.Left;
                GPA_ITem.LinkTo = Faculity_ITem.UniqueID;
                EditText c3 = (EditText)GPA_ITem.Specific;

                c3.DataBind.SetBound(true, Table_Name, "U_ST_GPA");

                Lbl_GPA.LinkTo = GPA_ITem.UniqueID;
                ((StaticText)Lbl_GPA.Specific).Caption = "GPA";
            }
            catch (Exception ex)
            { }
        }

        private static void ADD_Bin_Sub_Location(Form form)
        {
            Matrix Mat_Lines = (Matrix)form.Items.Item("10000001").Specific;
            Mat_Lines.Columns.Add("10000009", BoFormItemTypes.it_EDIT);
            Mat_Lines.Columns.Item("10000010").Visible = true;// Add("U_ST_JOIN_DATE", BoFormItemTypes.it_EDIT);
            //Mat_Lines.Columns.Add("ST_D_CODE", BoFormItemTypes.it_EDIT);
        }



    }
}
