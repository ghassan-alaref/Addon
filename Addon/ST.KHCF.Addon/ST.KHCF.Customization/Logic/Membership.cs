using SAPbobsCOM;
using ST.KHCF.Customization.Forms;
using ST.KHCF.Customization.Logic.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ST.KHCF.Customization.Logic
{
    internal class Membership
    {

        internal static string[] Renewal_Editable_Fields = new string[] { "U_ST_COVERAGE" };

        internal class Premium_Data
        {
            internal double Premium_Amount;
            internal double Discount_Percentage;
            internal double Discount_Value;
            internal int Waiting_Period;
            internal double Net_Amount;
            internal DateTime StartDate;
        }

        internal class Renewal_Data
        {
            internal string NATIONAL_ID;
            internal string IS_ACCEPTED;
            internal string REASON;
            internal string NEW_COVERAGE;
            internal string CREATE_INV_AUTO;
        }

        internal static Premium_Data Calculate_Premium(Company company, string Membership_Code)
        {
            string SQL_Memship = $@"SELECT T0.""Code"", T2.U_ST_TYPE, T1.U_ST_CUSTOMER_GROUP, T1.U_ST_BP_CODE, T0.U_ST_CHANNEL, T1.U_ST_BROKER1
, T0.U_ST_SUB_CHANNEL, T0.U_ST_MEMBERSHIP_STATUS, T0.U_ST_COVERAGE, T1.U_ST_DATE_OF_BIRTH, T1.U_ST_GENDER, T1.U_ST_NATIONALITY 
, T0.U_ST_START_DATE, T0.U_ST_END_DATE, T1.U_ST_RESIDENCY, T0.U_ST_AUTOMATIC_RENEWAL, T0.U_ST_MEMBER_CARD, T1.U_ST_PARENT_TYPE, T1.U_ST_PARENT_ID
FROM ""@ST_INDIV_MEMBERSHIP""  T0 
INNER JOIN ""@ST_CCI_INDIV_CARD""  T1 ON T1.""Code"" = T0.U_ST_MEMBER_CARD 
INNER JOIN OCRG T2 ON T2.""GroupCode"" = T1.U_ST_CUSTOMER_GROUP 
WHERE T0.""Code"" = '{Membership_Code}'";

            Recordset RC_Memship = Helper.Utility.Execute_Recordset_Query(company, SQL_Memship);
            if (RC_Memship.RecordCount == 0)
            {
                throw new Logic.Custom_Exception($"There is no Membership[{Membership_Code}] in the database.");
            }
            DateTime StartDate = (DateTime)RC_Memship.Fields.Item("U_ST_START_DATE").Value;
            DateTime EndDate = (DateTime)RC_Memship.Fields.Item("U_ST_END_DATE").Value;
            int Age = (StartDate - ((DateTime)RC_Memship.Fields.Item("U_ST_DATE_OF_BIRTH").Value)).Days / 365;

            string Group_Type, Customer_Group, Customer, Channel, Sub_Channel, Broker, Membership_Status;
            Group_Type = RC_Memship.Fields.Item("U_ST_PARENT_TYPE").Value.ToString();
            if (Group_Type != "C")
            {
                Group_Type = "I";
            }
            if (Group_Type == "I")
            {
                Customer_Group = RC_Memship.Fields.Item("U_ST_CUSTOMER_GROUP").Value.ToString();
                Customer = RC_Memship.Fields.Item("U_ST_BP_CODE").Value.ToString();
                Channel = RC_Memship.Fields.Item("U_ST_CHANNEL").Value.ToString();
                Sub_Channel = RC_Memship.Fields.Item("U_ST_SUB_CHANNEL").Value.ToString();
                Broker = RC_Memship.Fields.Item("U_ST_BROKER1").Value.ToString();
            }
            else
            {
                string SQL_Parent = $@"SELECT T0.U_ST_BP_CODE, T0.U_ST_CUSTOMER_GROUP, T0.U_ST_CHANNEL, T0.U_ST_SUB_CHANNEL, T0.U_ST_BROKER 
FROM ""@ST_CCI_CORP_CARD""  T0 WHERE T0.""Code"" = '{RC_Memship.Fields.Item("U_ST_PARENT_ID").Value}'";
                Recordset RC_Parent = Helper.Utility.Execute_Recordset_Query(company, SQL_Parent);
                Customer_Group = RC_Parent.Fields.Item("U_ST_CUSTOMER_GROUP").Value.ToString();
                Customer = RC_Parent.Fields.Item("U_ST_BP_CODE").Value.ToString();
                Channel = RC_Parent.Fields.Item("U_ST_CHANNEL").Value.ToString();
                Sub_Channel = RC_Parent.Fields.Item("U_ST_SUB_CHANNEL").Value.ToString();
                Broker = RC_Parent.Fields.Item("U_ST_BROKER").Value.ToString();
            }

            Membership_Status = RC_Memship.Fields.Item("U_ST_MEMBERSHIP_STATUS").Value.ToString();
            string SQL_Without_Customer = "";

            string sql_nationality = $@"Select ""Code"" From OCRY Where ""Name""='{RC_Memship.Fields.Item("U_ST_NATIONALITY").Value.ToString()}'";
            Recordset RC_Nationality = Helper.Utility.Execute_Recordset_Query(company, sql_nationality);
            RC_Nationality.DoQuery(sql_nationality);
            string Membership_Nationality = RC_Memship.Fields.Item("U_ST_NATIONALITY").Value.ToString();

            string SQL_Residency = $@"Select ""Code"" From OCRY Where ""Name""='{RC_Memship.Fields.Item("U_ST_RESIDENCY").Value}'";
            Recordset RC_Residency = Helper.Utility.Execute_Recordset_Query(company, SQL_Residency);
            RC_Residency.DoQuery(SQL_Residency);
            string Membership_Residency = RC_Memship.Fields.Item("U_ST_RESIDENCY").Value.ToString();

            if (RC_Nationality.RecordCount > 0)
                Membership_Nationality = RC_Nationality.Fields.Item("Code").Value.ToString();

            if (RC_Residency.RecordCount > 0)
                Membership_Residency = RC_Residency.Fields.Item("Code").Value.ToString();

           
            SQL_Without_Customer = $@"SELECT T1.U_ST_PREMIUM, T1.U_ST_DISCOUNT_PERCENTAGE, T1.U_ST_DISCOUNT_VALUE, T1.U_ST_WAITING_PERIOD
FROM ""@ST_COVERAGE_RULES""  T0 INNER JOIN ""@ST_COVERAGE_RULES_L""  T1 ON T0.""Code"" = T1.""Code"" 
WHERE T0.U_ST_CUSTOMER_GROUP_TYPE = '{Group_Type}' AND  T0.U_ST_CUSTOMER_GROUP = '{Customer_Group}' 
AND T0.U_ST_ACTIVE = 'Y'
AND T0.U_ST_CHANNEL = '{Channel}'  
AND (T0.U_ST_SUB_CHANNEL = '{Sub_Channel}' OR IFNULL(T0.U_ST_SUB_CHANNEL,'') = '') 
AND (T0.U_ST_BROKER = '{Broker}' OR IFNULL(T0.U_ST_BROKER,'') = '') 
AND  T1.U_ST_MEMBERSHIP_STATUS = '{Membership_Status}' AND T1.U_ST_COVERAGE = '{RC_Memship.Fields.Item("U_ST_COVERAGE").Value}'
AND ({Age} BETWEEN  T1.U_ST_AGE_FROM AND  T1.U_ST_AGE_TO) AND  (T1.U_ST_GENDER = '{RC_Memship.Fields.Item("U_ST_GENDER").Value}' OR T1.U_ST_GENDER = 'B') 
AND  (T1.U_ST_NATIONALITY = '{Membership_Nationality}' OR ('{Membership_Nationality}' <> 'JO' AND T1.U_ST_NATIONALITY = 'NJ')) 
AND  (T1.U_ST_RESIDENCY = '{Membership_Residency}' OR IFNULL(T1.U_ST_RESIDENCY,'') = '' OR ('{Membership_Residency}' <> 'JO' AND T1.U_ST_RESIDENCY ='OJ' ))
AND ('{((DateTime)RC_Memship.Fields.Item("U_ST_START_DATE").Value).ToString("yyyyMMdd")}' BETWEEN  T1.U_ST_START_DATE AND  T1.U_ST_END_DATE)";
 


            string SQL_With_Customer = $@"{SQL_Without_Customer}
AND T0.U_ST_CUSTOMER = '{Customer}' ";
            Recordset RC_Prem = Helper.Utility.Execute_Recordset_Query(company, SQL_With_Customer);
            if (RC_Prem.RecordCount == 0)
            {
                RC_Prem = Helper.Utility.Execute_Recordset_Query(company, SQL_Without_Customer);
                if (RC_Prem.RecordCount == 0)
                {
                    throw new Logic.Custom_Exception($"There is no Coverage Rule for the Membership[{Membership_Code}]");
                }
            }

            double Days_Percentage;
            if (StartDate.AddYears(1).AddDays(-1) == EndDate)
            {
                Days_Percentage = 1;
            }
            else
            {
                TimeSpan date_Calc = EndDate - StartDate;
                double Days_Total = date_Calc.Days;
                if (Days_Total == 364)
                {
                    Days_Total++;
                }
                Days_Percentage = Days_Total / 365;
            }

            Premium_Data Result = new Premium_Data();
            Result.Premium_Amount = (double)RC_Prem.Fields.Item("U_ST_PREMIUM").Value * Days_Percentage;
            Result.Discount_Percentage = (double)RC_Prem.Fields.Item("U_ST_DISCOUNT_PERCENTAGE").Value;
            Result.Discount_Value = (double)RC_Prem.Fields.Item("U_ST_DISCOUNT_VALUE").Value * Days_Percentage;
            Result.Waiting_Period = (int)RC_Prem.Fields.Item("U_ST_WAITING_PERIOD").Value;
            Result.StartDate = StartDate;
            if (RC_Memship.Fields.Item("U_ST_MEMBERSHIP_STATUS").Value.ToString() == "R")
            {
                string MemberCard_Code = RC_Memship.Fields.Item("U_ST_MEMBER_CARD").Value.ToString();
                string Current_Membership_Code = RC_Memship.Fields.Item("Code").Value.ToString();

                string SQL_Last_Membership = $@"SELECT TOP 1 T0.""U_ST_START_DATE"", T0.""U_ST_WAITING_PERIOD"" FROM ""@ST_INDIV_MEMBERSHIP"" T0 
WHERE T0.U_ST_MEMBER_CARD = '{MemberCard_Code}' 
AND T0.""Code"" <> '{Current_Membership_Code}' ANd T0.U_ST_MEMBERSHIP_STATUS NOT IN ('C')
ORDER BY T0.U_ST_END_DATE DESC ";

                Recordset RC_Last_Membership = Helper.Utility.Execute_Recordset_Query(company, SQL_Last_Membership);
                if (RC_Last_Membership.RecordCount == 0)
                {
                    throw new Logic.Custom_Exception($"Cannot calculate the Premium. There are No valid memberships before this one.");
                }
                DateTime Last_Membership_Start_Date = (DateTime)RC_Last_Membership.Fields.Item("U_ST_START_DATE").Value;
                int Last_Membership_Waiting_Period = (int)RC_Last_Membership.Fields.Item("U_ST_WAITING_PERIOD").Value;
                //int Rest_Months = Convert.ToInt32(Math.Round((DateTime.Today - Prev_Start_Date).TotalDays / 30, MidpointRounding.AwayFromZero));
                int Last_Current_Memberships_Months_Diff = Convert.ToInt32(Math.Round((StartDate - Last_Membership_Start_Date).TotalDays / 30, MidpointRounding.AwayFromZero));
                int New_Waiting_Period = Last_Membership_Waiting_Period - Last_Current_Memberships_Months_Diff;
                if (New_Waiting_Period > 0)
                {
                    Result.Waiting_Period = New_Waiting_Period;
                }
                else
                {
                    Result.Waiting_Period = 0;
                }
            }
            Result.Net_Amount = Result.Premium_Amount - Result.Discount_Value;

            return Result;
        }

        internal static Inoice_Data Update_Premium(Company company, string Membership_Code, UDO_Definition UDO_Info, int Payment_Terms, string Currency, bool Is_One_Installment)
        {
            Inoice_Data Inv_Data = new Inoice_Data();
            Premium_Data Premium = Membership.Calculate_Premium(company, Membership_Code);
            Inv_Data.Source_Code = Membership_Code;
            Inv_Data.Premium_Amount = Premium.Premium_Amount;
            Inv_Data.Discount_Value = Premium.Discount_Value;
            Inv_Data.Discount_Percentage = Premium.Discount_Percentage;
            Inv_Data.Waiting_Period = Premium.Waiting_Period;
            Inv_Data.Is_One_Installment = Is_One_Installment;
            Inv_Data.Payment_Terms = Payment_Terms;
            Inv_Data.Currency =Currency;


            List<Field_Data> Premium_Field_Data = new List<Field_Data>();
            Premium_Field_Data.Add(new Field_Data() { Field_Name = "U_ST_PREMIUM", Value = Premium.Premium_Amount });
            Premium_Field_Data.Add(new Field_Data() { Field_Name = "U_ST_DISCOUNT_PERCENTAGE", Value = Premium.Discount_Percentage });
            Premium_Field_Data.Add(new Field_Data() { Field_Name = "U_ST_DISCOUNT_VALUE", Value = Premium.Discount_Value });
            Premium_Field_Data.Add(new Field_Data() { Field_Name = "U_ST_WAITING_PERIOD", Value = Premium.Waiting_Period });
            Utility.Update_UDO(company, UDO_Info, Membership_Code, Premium_Field_Data.ToArray());

            return Inv_Data;
        }

        internal static void Renewal_Children(Company company, string Member_Card_Code, string Parent_Membership_ID, UDO_Definition UDO_Info)
        {
            UDO_Definition TempUdoInfo = new UDO_Definition();
            if(UDO_Info.KHCF_Object == KHCF_Objects.Corporate_Membership)
                TempUdoInfo.KHCF_Object = KHCF_Objects.CCI_Corporate_Member_Card;

            if (UDO_Info.KHCF_Object == KHCF_Objects.Individual_Membership)
                TempUdoInfo.KHCF_Object = KHCF_Objects.CCI_Member_Card;

            string[] Children_Card_Codes = KHCF_Logic_Utility.Get_Children_MemberCard_Codes(company, Member_Card_Code, TempUdoInfo);
            try
            {
                company.StartTransaction();
                foreach (string One_Child in Children_Card_Codes)
                {
                    TempUdoInfo.KHCF_Object = KHCF_Objects.Individual_Membership;
                    TempUdoInfo.Table_Name = "ST_INDIV_MEMBERSHIP";
                    if (UDO_Info.KHCF_Object == KHCF_Objects.Corporate_Membership)
                        Membership.Create_Renewal_MemberCard(company, One_Child, TempUdoInfo, "C", Parent_Membership_ID);
                    else
                        Membership.Create_Renewal_MemberCard(company, One_Child, TempUdoInfo, "I", Parent_Membership_ID);
                }
                company.EndTransaction(BoWfTransOpt.wf_Commit);
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
                throw new Exception($"Error during Renewal the children of the Member Card[{Member_Card_Code}],[{ex.Message}]");
            }
        }

        internal static int Create_Invoice(Company company, string Membership_Code, UDO_Definition UDO_Info)
        {
            Inoice_Data Inv_Data = new Inoice_Data();
            Premium_Data Premium = Membership.Calculate_Premium(company, Membership_Code);
            Inv_Data.Source_Code = Membership_Code;
            Inv_Data.Premium_Amount = Premium.Premium_Amount;
            Inv_Data.Discount_Value = Premium.Discount_Value;
            Inv_Data.Discount_Percentage = Premium.Discount_Percentage;
            Inv_Data.Waiting_Period = Premium.Waiting_Period;
     
            List<Field_Data> Premium_Field_Data = new List<Field_Data>();
            Premium_Field_Data.Add(new Field_Data() { Field_Name = "U_ST_PREMIUM", Value = Premium.Premium_Amount });
            Premium_Field_Data.Add(new Field_Data() { Field_Name = "U_ST_DISCOUNT_PERCENTAGE", Value = Premium.Discount_Percentage });
            Premium_Field_Data.Add(new Field_Data() { Field_Name = "U_ST_DISCOUNT_VALUE", Value = Premium.Discount_Value });
            Premium_Field_Data.Add(new Field_Data() { Field_Name = "U_ST_WAITING_PERIOD", Value = Premium.Waiting_Period });
            DateTime waitingPeriodDate = Premium.StartDate.AddMonths(Premium.Waiting_Period);
            Premium_Field_Data.Add(new Field_Data() { Field_Name = "U_ST_WAITING_PERIOD_DATE", Value = waitingPeriodDate });

            Utility.Update_UDO(company, UDO_Info, Membership_Code, Premium_Field_Data.ToArray());

            Inv_Data.Is_One_Installment = true;
            string type = string.Empty;
            int NewEntry = Membership.Create_Invoice(company, Inv_Data, UDO_Info, out type);

            return NewEntry;
        }

        internal static int Create_Invoice(Company company, Inoice_Data Inv_Data, UDO_Definition UDO_Info, out string type, Documents DownPayment_Doc = null, bool Is_Credit_Note = false)
        {
            string SQL_Mem_Ship = $@"SELECT T0.U_ST_START_DATE, T1.U_ST_BP_CODE, T2.U_ST_UNEARNED_REVENUE_ACCOUNT , T1.""CreateDate"",T1.""U_ST_FULL_NAME_AR""
, T3.""Name"" AS ""Coverage_Name"", T0.U_ST_PREMIUM, T0.U_ST_CURRENCY, T0.U_ST_PARENT_MEMBERSHIP_TYPE, T0.U_ST_PARENT_MEMBERSHIP_ID
, T0.U_ST_INVOICE_NUMBER, U_ST_DOCUMENT_TYPE, U_ST_DISCOUNT_VALUE, T0.U_ST_MEMBERSHIP_STATUS, T0.U_ST_INSTALLMENT_TYPE
FROM ""@{UDO_Info.Table_Name}""  T0 INNER JOIN ""@ST_CCI_INDIV_CARD""  T1 ON T0.U_ST_MEMBER_CARD = T1.""Code"" 
INNER JOIN ""@ST_INV_ACCOUNT_MAPP"" T2 ON T0.U_ST_COVERAGE = T2.""U_ST_COVERAGE""  AND T1.U_ST_CUSTOMER_GROUP = T2.U_ST_CUSTOMER_GROUP_CODE
INNER JOIN ""@ST_COVERAGE"" T3 ON T0.U_ST_COVERAGE = T3.""Code"" 
WHERE T0.""Code"" = '{Inv_Data.Source_Code}'";
            Recordset RC_Mem_Ship = Helper.Utility.Execute_Recordset_Query(company, SQL_Mem_Ship);
            string Invoic_Entry = RC_Mem_Ship.Fields.Item("U_ST_INVOICE_NUMBER").Value.ToString();
            string Invoic_Type = RC_Mem_Ship.Fields.Item("U_ST_DOCUMENT_TYPE").Value.ToString();
            Inv_Data.Premium_Amount = (double)RC_Mem_Ship.Fields.Item("U_ST_PREMIUM").Value;
            Inv_Data.Currency = RC_Mem_Ship.Fields.Item("U_ST_CURRENCY").Value.ToString();
            if (Is_Credit_Note == false)
            {
                if (Invoic_Entry != "" && Invoic_Entry != "0" && Invoic_Type == "13")
                {
                    throw new Logic.Custom_Exception($@"The Membership[{Inv_Data.Source_Code}] already has the Invoice Entry[{Invoic_Entry}]");
                }
                if ((new string[] { "S", "C" }).Contains(RC_Mem_Ship.Fields.Item("U_ST_MEMBERSHIP_STATUS").Value.ToString()))
                {
                    throw new Logic.Custom_Exception($"According the Status, We can't create Invoice for the Membership[{Inv_Data.Source_Code}]");
                }
            }

            DateTime StartDate = (DateTime)RC_Mem_Ship.Fields.Item("U_ST_START_DATE").Value;
            Documents Doc;
            string GL_Account;
            bool IsInvoice = StartDate <= DateTime.Today || DownPayment_Doc != null;
            if (IsInvoice || Is_Credit_Note == true)
            {
                if (Is_Credit_Note == false)
                {
                    type = "Invoice";
                    Doc = (Documents)company.GetBusinessObject(BoObjectTypes.oInvoices);
                }
                else
                {
                    type = "Credit Note";
                    Doc = (Documents)company.GetBusinessObject(BoObjectTypes.oCreditNotes);
                }
                DateTime D = (DateTime)RC_Mem_Ship.Fields.Item("U_ST_START_DATE").Value;
                Doc.DocDate = (DateTime)RC_Mem_Ship.Fields.Item("U_ST_START_DATE").Value;
                GL_Account = RC_Mem_Ship.Fields.Item("U_ST_UNEARNED_REVENUE_ACCOUNT").Value.ToString();
                if (GL_Account == "")
                {
                    throw new Logic.Custom_Exception($"Invoice cannot be created. No GL Account in the configuration for the coverage[{RC_Mem_Ship.Fields.Item("Coverage_Name").Value.ToString()}].");
                }
                if (DownPayment_Doc != null)
                {
                    int X = DownPayment_Doc.DocEntry;
                    DownPaymentsToDraw dpToDraw = Doc.DownPaymentsToDraw;
                    dpToDraw.DocEntry = DownPayment_Doc.DocEntry;
                    dpToDraw.AmountToDraw = DownPayment_Doc.PaidToDate;
                }
                if (Inv_Data.Premium_Amount == 0)
                {
                    throw new Exception($"The {type} cannot be created. Premium must be greater than Zero.");
                }
            }
            else
            {
                if (Invoic_Entry != "" && Invoic_Entry != "0")
                {
                    throw new Logic.Custom_Exception($@"The Membership[{Inv_Data.Source_Code}] already has the Down Payment Entry[{Invoic_Entry}]");
                }
                type = "Down Payment";
                Doc = (Documents)company.GetBusinessObject(BoObjectTypes.oDownPayments);
                Doc.DocDate = DateTime.Now;
                GL_Account = Configurations.Get_Down_Payment(company);
                Doc.DownPaymentType = DownPaymentTypeEnum.dptInvoice;
            }

            double Exchange_Rate = 1;
            if (Inv_Data.Currency != "JOD")
            {
                Exchange_Rate = 1 / Utility.Get_Exchange_Rate(company, Inv_Data.Currency, Doc.DocDate);
                Doc.DocCurrency = Inv_Data.Currency;
            }

            Doc.DocType = BoDocumentTypes.dDocument_Service;
            Doc.TaxDate = (DateTime)RC_Mem_Ship.Fields.Item("CreateDate").Value;
            if (Inv_Data.Is_One_Installment)
            {
                Doc.DocDueDate = DateTime.Today;
            }
            else
            {
                string Pay_Terms_Text = RC_Mem_Ship.Fields.Item("U_ST_INSTALLMENT_TYPE").Value.ToString();
                if (Pay_Terms_Text != "" && Pay_Terms_Text != "0")
                {
                    Doc.PaymentGroupCode = int.Parse(Pay_Terms_Text);
                }
            }
            Doc.CardCode = RC_Mem_Ship.Fields.Item("U_ST_BP_CODE").Value.ToString();
            Doc.JournalMemo = RC_Mem_Ship.Fields.Item("U_ST_FULL_NAME_AR").Value.ToString();
            Doc.Comments = RC_Mem_Ship.Fields.Item("U_ST_FULL_NAME_AR").Value.ToString();
            Doc.Lines.AccountCode = GL_Account;
            Doc.Lines.ItemDescription = RC_Mem_Ship.Fields.Item("Coverage_Name").Value.ToString();
            double Main_Amount;

            if (DownPayment_Doc != null)
            {
                Main_Amount = DownPayment_Doc.DocTotal;
            }
            else
            {
                Main_Amount = Inv_Data.Premium_Amount * Exchange_Rate;
            }
            Doc.Lines.LineTotal = Main_Amount;
            Doc.Lines.Add();

            if (IsInvoice || Is_Credit_Note)
            {
                double Discount = (double)RC_Mem_Ship.Fields.Item("U_ST_DISCOUNT_VALUE").Value;
                if (Discount != 0)
                {
                    Doc.Lines.AccountCode = Configurations.Get_GL_Account_For_Discount(company);
                    Doc.Lines.ItemDescription = RC_Mem_Ship.Fields.Item("Coverage_Name").Value.ToString();
                   // Doc.Lines.LineTotal = (-1) * Inv_Data.Discount_Value * Exchange_Rate;
                    Doc.Lines.LineTotal = (-1) * Discount * Exchange_Rate;
                    Doc.Lines.Add();
                }

                if (RC_Mem_Ship.Fields.Item("U_ST_PARENT_MEMBERSHIP_TYPE").Value.ToString() == "C")
                {
                    string SQL_Parent_Discount = $@"SELECT T0.U_ST_DISCOUNT_PERCENTAGE
FROM ""@ST_CORP_MEMBERSHIP""  T0 WHERE T0.""Code"" = '{RC_Mem_Ship.Fields.Item("U_ST_PARENT_MEMBERSHIP_ID").Value}'";
                    Recordset RC_Parent_Discount = Helper.Utility.Execute_Recordset_Query(company, SQL_Parent_Discount);
                    double Parent_Discount = (double)RC_Parent_Discount.Fields.Item("U_ST_DISCOUNT_PERCENTAGE").Value;
                    if (Parent_Discount != 0)
                    {
                        Doc.Lines.AccountCode = Configurations.Get_GL_Account_For_Discount(company);
                        Doc.Lines.ItemDescription = RC_Mem_Ship.Fields.Item("Coverage_Name").Value.ToString();
                        // Doc.Lines.LineTotal = (-1) * Inv_Data.Discount_Value * Exchange_Rate;
                        Doc.Lines.LineTotal = (-1) * Main_Amount * (Parent_Discount / 100);
                        Doc.Lines.FreeText = $"Parent Membership[{RC_Mem_Ship.Fields.Item("U_ST_PARENT_MEMBERSHIP_ID").Value}] discount";
                        Doc.Lines.Add();
                    }
                }

            }

            Doc.UserFields.Fields.Item("U_ST_MEMBERSHIP_CODE").Value = Inv_Data.Source_Code;

            if (Doc.Add() != 0)
            {
                string X = company.GetLastErrorDescription();
                throw new Logic.Custom_Exception($"{type} cannot be created for the Membership[{Inv_Data.Source_Code}],[{company.GetLastErrorDescription()}]");
            }

            string NewEntry = "";

            try
            {
                company.GetNewObjectCode(out NewEntry);

                if (Is_Credit_Note == false)
                {
                    Field_Data Fld_Status = new Field_Data() { Field_Name = "U_ST_INVOICE_NUMBER", Value = NewEntry };
                    string DocType;
                    if (IsInvoice == true)
                    {
                        DocType = "13";
                    }
                    else
                    {
                        DocType = ((int)BoObjectTypes.oDownPayments).ToString();
                    }
                    Field_Data Fld_DocType = new Field_Data() { Field_Name = "U_ST_DOCUMENT_TYPE", Value = DocType };
                    Utility.Update_UDO(company, UDO_Info, Inv_Data.Source_Code, new Field_Data[] { Fld_Status, Fld_DocType });
                }
            }
            catch (Exception ex)
            {
                throw new Logic.Custom_Exception($"Error during updating the Membership[{Inv_Data.Source_Code}] after creating the Invoice[{NewEntry}][{ex.Message}]");
            }

            return int.Parse(NewEntry);

        }

        internal static void Cancel_Individual_Membership(Company company, string UDO_Code, UDO_Definition UDO_Info)
        {
            string SQL = $@"Select U_ST_CAN_CANCEL_MEMBERSHIP from OUSR 
WHERE USER_CODE = '{company.UserName}'";
            Recordset RC = Helper.Utility.Execute_Recordset_Query(company, SQL);
            bool Can_Cancel = RC.Fields.Item(0).Value.ToString() == "Y";
            if (!Can_Cancel)
            {
                throw new Logic.Custom_Exception("You are not authorized to Cancel the Membership");
            }
            Field_Data Status = Utility.Get_Field_Value(company, UDO_Info, UDO_Code, "U_ST_MEMBERSHIP_STATUS");
            if (!(new string[] { "N", "R", "P" }).Contains(Status.Value.ToString()))
            {
                throw new Logic.Custom_Exception($"According the Status, We can't cancel the Membership[{UDO_Code}]");
            }

            try
            {
                company.StartTransaction();
                Documents Doc = null;
                if (UDO_Info.Table_Name == "ST_INDIV_MEMBERSHIP")
                {
                    Doc = Utility.Get_Membership_Invoice(company, UDO_Code);
                }
                else if (UDO_Info.Table_Name == "ST_CORP_MEMBERSHIP")
                {
                    Doc = Utility.Get_Corporate_Invoice(company, UDO_Code);
                }
                    
                if (Doc != null)
                {
                    if (Doc.DocumentStatus == BoStatus.bost_Close)
                    {
                        throw new Logic.Custom_Exception($"We can't cancel the Invoice [{Doc.DocNum}] because it is closed");
                    }
                    if (Doc != null)
                    {
                        if (Doc.DocObjectCode == BoObjectTypes.oInvoices)
                        {
                            Documents Doc_Cancellation = Doc.CreateCancellationDocument();
                            if (Doc_Cancellation == null)
                            {
                                throw new Logic.Custom_Exception($"Error during cancel the Document[{Doc.DocNum}][{company.GetLastErrorDescription()}]");
                            }
                            if (Doc_Cancellation.Add() != 0)
                            {
                                throw new Logic.Custom_Exception($"Error during cancel the Document[{Doc.DocNum}][{company.GetLastErrorDescription()}]");
                            }
                        }
                        else if (Doc.DocObjectCode == BoObjectTypes.oDownPayments)
                        {
                            if (Doc.PaidToDate != 0)
                            {
                                throw new Logic.Custom_Exception("The Down Payment is already related to a Payment, please cancel all related payments and try again");
                            }
                            //Documents Credit_Note_Doc = Helper.Utility.Copy_Document(company, Doc, BoObjectTypes.oCreditCards);
                            Documents Credit_Note_Doc = Utility.Copy_Document(company, Doc, BoObjectTypes.oCreditNotes);
                        }
                    }
                }
                Field_Data Fld_Status = new Field_Data() { Field_Name = "U_ST_MEMBERSHIP_STATUS", Value = "C" };
                Field_Data Fld_Active = new Field_Data() { Field_Name = "U_ST_ACTIVE", Value = "N" };
                Utility.Update_UDO(company, UDO_Info, UDO_Code, new Field_Data[] { Fld_Status, Fld_Active });

                //                string SQL_Revenue_Real = $@"SELECT DISTINCT T0.U_ST_REVENUE_REALIZATION_JE 
                //FROM INV1 T0  INNER JOIN OINV T1 ON T0.""DocEntry"" = T1.""DocEntry"" 
                //WHERE T1.U_ST_MEMBERSHIP_CODE = '{UDO_Code}'";
                if (Doc != null)
                {

                    string SQL_Revenue_Real = $@"SELECT T0.""TransId"" FROM OJDT T0 WHERE T0.U_ST_REVENUE_REALIZATION_INV_NUM = {Doc.DocNum}";

                    Recordset RC_Revenue_Real = Helper.Utility.Execute_Recordset_Query(company, SQL_Revenue_Real);
                    for (int i = 0; i < RC_Revenue_Real.RecordCount; i++)
                    {
                        JournalEntries JE = (JournalEntries)company.GetBusinessObject(BoObjectTypes.oJournalEntries);
                        int JE_ID = (int)RC_Revenue_Real.Fields.Item("TransId").Value;
                        if (JE_ID != 0)
                        {
                            if (!JE.GetByKey(JE_ID))
                            {
                                throw new Logic.Custom_Exception($"The JE[{JE_ID}] is not existing for the Membership[{UDO_Code}]");
                            }
                            if (JE.Cancel() != 0)
                            {
                                throw new Logic.Custom_Exception($"Error during cancel the JE[{JE_ID}][{company.GetLastErrorDescription()}]");
                            }
                        }
                        RC_Revenue_Real.MoveNext();
                    }
                }
                company.EndTransaction(BoWfTransOpt.wf_Commit);
            }
            catch (Exception ex)
            {
                try
                {
                    company.EndTransaction(BoWfTransOpt.wf_RollBack);
                }
                catch (Exception)
                { }
                throw new Logic.Custom_Exception($"Error during Cancel the Membership[{UDO_Code}][{ex.Message}]");
            }


        }

        internal static void Close_Individual_Membership(Company company, string UDO_Code, UDO_Definition UDO_Info)
        {
            try
            {
                company.StartTransaction();
                Documents Doc_Inv = Utility.Get_Membership_Invoice(company, UDO_Code);
                if (Doc_Inv != null)
                {
                    Create_Rest_Credit_Note(company, UDO_Code, UDO_Info, Doc_Inv, DateTime.Today);
                }

                Field_Data Fld_Status = new Field_Data() { Field_Name = "U_ST_MEMBERSHIP_STATUS", Value = "C" };
                Field_Data Fld_Active = new Field_Data() { Field_Name = "U_ST_ACTIVE", Value = "N" };
                Utility.Update_UDO(company, UDO_Info, UDO_Code, new Field_Data[] { Fld_Status, Fld_Active });




                company.EndTransaction(BoWfTransOpt.wf_Commit);
            }
            catch (Exception ex)
            {
                try
                {
                    company.EndTransaction(BoWfTransOpt.wf_RollBack);
                }
                catch (Exception)
                { }
                throw new Logic.Custom_Exception($"Error during Cancel the Membership[{UDO_Code}][{ex.Message}]");
            }

        }

        private static Documents Create_Rest_Credit_Note(Company company, string UDO_Code, UDO_Definition UDO_Info, Documents Doc_Inv, DateTime Stop_Date)
        {
            string SQL_Memb = $@"SELECT T0.U_ST_START_DATE, T0.U_ST_END_DATE, U_ST_PREMIUM 
FROM ""@{UDO_Info.Table_Name}""  T0 
WHERE T0.""Code"" = '{UDO_Code}'";
            Recordset RC_Memb = Helper.Utility.Execute_Recordset_Query(company, SQL_Memb);
            DateTime StartDate = (DateTime)RC_Memb.Fields.Item("U_ST_START_DATE").Value;
            DateTime EndDate = (DateTime)RC_Memb.Fields.Item("U_ST_END_DATE").Value;
            int Duration_Days = (EndDate - StartDate).Days;
            int Taken_Days = (Stop_Date - StartDate).Days;
            //if (StartDate >= DateTime.Today)
            //{
            //    Taken_Days = Duration_Days;
            //}
            //else
            //{
            //    Taken_Days = (EndDate - StartDate).Days;
            //}

            double Taken_Rate = (double)Taken_Days / Duration_Days;

            Documents Doc_Mem = (Documents)company.GetBusinessObject(BoObjectTypes.oCreditNotes);

            Doc_Mem.CardCode = Doc_Inv.CardCode;
            Doc_Mem.CardName = Doc_Inv.CardName;
            Doc_Mem.DocDate = Doc_Mem.TaxDate = Stop_Date;
            Doc_Mem.DiscountPercent = Doc_Inv.DiscountPercent;
            Doc_Mem.DocType = BoDocumentTypes.dDocument_Service;
            for (int i = 0; i < Doc_Inv.Lines.Count; i++)
            {
                Doc_Inv.Lines.SetCurrentLine(i);
                Doc_Mem.Lines.ItemDescription = Doc_Inv.Lines.ItemDescription;
                Doc_Mem.Lines.AccountCode = Doc_Inv.Lines.AccountCode;
                double XX = Doc_Inv.Lines.LineTotal - Doc_Inv.Lines.LineTotal * Taken_Rate;
                Doc_Mem.Lines.LineTotal = XX;// Doc_Inv.Lines.LineTotal - Doc_Inv.Lines.LineTotal * Taken_Rate;

                Doc_Mem.Lines.Add();
            }
            //int c1 = Doc_Mem.Lines.Count;
            Doc_Mem.UserFields.Fields.Item("U_ST_MEMBERSHIP_CODE").Value = UDO_Code;

            if (Doc_Mem.Add() != 0)
            {
                string X = company.GetLastErrorDescription();
                throw new Logic.Custom_Exception($@"Error during create the Credit Note[{company.GetLastErrorDescription()}]");
            }

            double Actual_Prem = (double)RC_Memb.Fields.Item("U_ST_PREMIUM").Value * Taken_Rate;
            Field_Data Fld_Actual_Prem = new Field_Data() { Field_Name = "U_ST_ACTUAL_PERIOD_PREMIUM", Value = Actual_Prem };

            Utility.Update_UDO(company, UDO_Info, UDO_Code, new Field_Data[] { Fld_Actual_Prem });


            string New_Entry;
            company.GetNewObjectCode(out New_Entry);

            Doc_Mem.GetByKey(int.Parse(New_Entry));

            return Doc_Mem;
        }

        internal static DateTime Get_New_Renewal_StartDate(Company company, DateTime Old_End_Date, out bool Is_Past,DateTime? StartDate = null)
        {
            Is_Past = false;
            DateTime CurrentDate = StartDate == null ? DateTime.Now : StartDate.Value;
            int AllowanceDays = Configurations.Get_Renewal_Days_for_Start_Date(company, true);
            int AllowanceMonths = Configurations.Get_Past_Allowance_Months(company, true);

            if (Old_End_Date.AddMonths(AllowanceMonths) < CurrentDate)
            {
                Is_Past = true;
                return CurrentDate;
            }
            else
            {
                return Old_End_Date.AddDays(AllowanceDays);
            }

            //if (Old_End_Date >= CurrentDate)
            //{
            //    return Old_End_Date.AddDays(AllowanceDays);
            //}
            //else if (Old_End_Date.AddMonths(AllowanceMonths) >= CurrentDate)
            //{
            //    return Old_End_Date.AddDays(AllowanceDays);
            //}
            //else 
            //throw new Logic.Custom_Exception($"The Old end Date[{Old_End_Date}] is not supported to get the New Renewal Start Date");
        }

        internal static void Reject(Company company, string Membership_Code, UDO_Definition UDO_Info)
        {
            Field_Data Fld_Status = new Field_Data() { Field_Name = "U_ST_APPROVAL_STATUS", Value = "R" };
            Utility.Update_UDO(company, UDO_Info, Membership_Code, new Field_Data[] { Fld_Status });

            string Created_UserCode = KHCF_Logic_Utility.Get_Created_UserCode(company, Membership_Code, UDO_Info);
            string Message = $@"The {UDO_Info.Title}[{Membership_Code}] is Rejected";
            Helper.Utility.SendAlertMessage(company, Membership_Code, Message, Created_UserCode, $"{UDO_Info.Title}[{Membership_Code}] is Rejected", "KHCF Object Code", $"{UDO_Info.Title} [{Membership_Code}]", Utility.Get_UDO_Type_ID(UDO_Info.KHCF_Object));

        }

        internal static void Approve(Company company, string Membership_Code, UDO_Definition UDO_Info)
        {
            Field_Data[] Membership_Fields = Utility.Get_UDO_Data(company, Membership_Code, UDO_Info);
            DateTime StartDate = 
                Convert.ToDateTime(
                (
                    from a in Membership_Fields
                    where a.Field_Name == "U_ST_START_DATE"
                    select a.Value
                ).FirstOrDefault());

            Field_Data Fld_Status = new Field_Data() { Field_Name = "U_ST_APPROVAL_STATUS", Value = "A" };
            Field_Data Fld_Membership_Status = new Field_Data() { Field_Name = "U_ST_MEMBERSHIP_STATUS", Value = "R" };
            Utility.Update_UDO(company, UDO_Info, Membership_Code, new Field_Data[] { Fld_Status, Fld_Membership_Status });

            if (UDO_Info.KHCF_Object == KHCF_Objects.Individual_Membership)
            {
                Premium_Data Premium = Membership.Calculate_Premium(company, Membership_Code);
                Field_Data Fld_Membership_Waiting_Period = new Field_Data() { Field_Name = "U_ST_WAITING_PERIOD", Value = Premium.Waiting_Period, Data_Type = BoFieldTypes.db_Numeric };
                Field_Data Fld_Membership_Waiting_Period_Date = new Field_Data() { Field_Name = "U_ST_WAITING_PERIOD_DATE", Value = StartDate.AddMonths(Premium.Waiting_Period), Data_Type = BoFieldTypes.db_Date };
                Utility.Update_UDO(company, UDO_Info, Membership_Code, new Field_Data[] { Fld_Membership_Waiting_Period, Fld_Membership_Waiting_Period_Date });
            }
            string Created_UserCode = KHCF_Logic_Utility.Get_Created_UserCode(company, Membership_Code, UDO_Info);
            string Message = $@"The {UDO_Info.Title}[{Membership_Code}] is Renewed from Past Successfully.";
            Helper.Utility.SendAlertMessage(company, Membership_Code, Message, Created_UserCode, $"{UDO_Info.Title}[{Membership_Code}] is Approved", "KHCF Object Code", $"{UDO_Info.Title} [{Membership_Code}]", Utility.Get_UDO_Type_ID(UDO_Info.KHCF_Object));
        }

        internal static void Create_Renewal(Company company, string Membership_Code, UDO_Definition UDO_Info, Field_Data[] Renewal_Fields,
             bool Create_Invoice_Automatically, string MemberCardType ,string Parent_Membership_ID = null)
        {
            string SQL_Data = $@"Select * FROM ""@{UDO_Info.Table_Name}"" WHERE ""Code"" = '{Membership_Code}'";
            Recordset RC_Data = Helper.Utility.Execute_Recordset_Query(company, SQL_Data);

            Field_Definition[] Membership_Fields = Fields_Logic.All_Field_Definition.Where(F => F.KHCF_Object == UDO_Info.KHCF_Object).ToArray();
            List<Field_Data> Renewal_Data = new List<Field_Data>();
            foreach (Field_Definition OneField in Membership_Fields)
            {
                Renewal_Data.Add(new Field_Data() { Field_Name = OneField.Column_Name_In_DB, Value = RC_Data.Fields.Item(OneField.Column_Name_In_DB).Value });
            }
            DateTime Old_End_Date = (DateTime)Renewal_Data.FirstOrDefault(F => F.Field_Name == "U_ST_END_DATE").Value;
            string Old_Is_Active = Renewal_Data.FirstOrDefault(F => F.Field_Name == "U_ST_ACTIVE").Value.ToString();
            string CardNumber_Code = Renewal_Data.FirstOrDefault(F => F.Field_Name == "U_ST_MEMBER_CARD").Value.ToString();
            bool Is_Past;
            DateTime New_Renewal_StartDate = Membership.Get_New_Renewal_StartDate(company, Old_End_Date, out Is_Past);
            DateTime New_Renewal_EndDate = New_Renewal_StartDate.AddMonths(Configurations.Get_Renewal_Month_for_End_Date(company, true)).AddDays(-1);
            New_Renewal_EndDate.AddDays(-1);

            foreach (Field_Data OneField in Renewal_Fields)
            {
                if (OneField.Value.ToString() != "")
                {
                    Renewal_Data.FirstOrDefault(F => F.Field_Name == OneField.Field_Name).Value = OneField.Value;
                }
            }

            string CardMember_Table = "";
            if (UDO_Info.KHCF_Object == KHCF_Objects.Individual_Membership)
            {
                CardMember_Table = "ST_CCI_INDIV_CARD";
            }
            else
            {
                CardMember_Table = "ST_CCI_CORP_CARD";
            }

            string SQL_Card = $@"SELECT T0.U_ST_DATE_OF_BIRTH, T0.U_ST_CHANNEL, T0.U_ST_SUB_CHANNEL FROM ""@{CardMember_Table}""  T0 WHERE T0.""Code"" = '{CardNumber_Code}'";
            Recordset RC_Card = Helper.Utility.Execute_Recordset_Query(company, SQL_Card);
            int Age = (New_Renewal_StartDate - ((DateTime)RC_Card.Fields.Item("U_ST_DATE_OF_BIRTH").Value)).Days / 365;
            Renewal_Data.FirstOrDefault(F => F.Field_Name == "U_ST_AGE").Value = Age;
            Renewal_Data.FirstOrDefault(F => F.Field_Name == "U_ST_INVOICE_NUMBER").Value = "";
            Renewal_Data.FirstOrDefault(F => F.Field_Name == "U_ST_START_DATE").Value = New_Renewal_StartDate;
            Renewal_Data.FirstOrDefault(F => F.Field_Name == "U_ST_END_DATE").Value = New_Renewal_EndDate;
            Renewal_Data.FirstOrDefault(F => F.Field_Name == "U_ST_CREATOR").Value = company.UserName;
            Renewal_Data.FirstOrDefault(F => F.Field_Name == "U_ST_CREATION_DATE").Value = DateTime.Today;

            if (Is_Past)
            {
                Renewal_Data.FirstOrDefault(F => F.Field_Name == "U_ST_MEMBERSHIP_STATUS").Value = "P";
            }
            else
            {
                Renewal_Data.FirstOrDefault(F => F.Field_Name == "U_ST_MEMBERSHIP_STATUS").Value = "R";
            }

            Renewal_Data.FirstOrDefault(F => F.Field_Name == "U_ST_PREVIOUS_MEMBERSHIP_CODE").Value = Membership_Code;

            if (Parent_Membership_ID != null)
                Renewal_Data.FirstOrDefault(F => F.Field_Name == "U_ST_PARENT_MEMBERSHIP_ID").Value = Parent_Membership_ID;

            Renewal_Data.FirstOrDefault(F => F.Field_Name == "U_ST_PARENT_MEMBERSHIP_TYPE").Value = MemberCardType;

            Renewal_Data.FirstOrDefault(F => F.Field_Name == "U_ST_APPROVAL_STATUS").Value = "A";

            if (Old_Is_Active == "Y")
            {
                Renewal_Data.FirstOrDefault(F => F.Field_Name == "U_ST_ACTIVE").Value = "N";
            }
            else
            {
                Renewal_Data.FirstOrDefault(F => F.Field_Name == "U_ST_ACTIVE").Value = "Y";
            }

            string New_Membership_Code = Utility.Add_UDO_Entry(company, UDO_Info, Renewal_Data.ToArray());

            Premium_Data Prem_Data = Calculate_Premium(company, New_Membership_Code);
            List<Field_Data> Premium_Field_Data = new List<Field_Data>();
            Premium_Field_Data.Add(new Field_Data() { Field_Name = "U_ST_PREMIUM", Value = Prem_Data.Premium_Amount });
            Premium_Field_Data.Add(new Field_Data() { Field_Name = "U_ST_DISCOUNT_PERCENTAGE", Value = Prem_Data.Discount_Percentage });
            Premium_Field_Data.Add(new Field_Data() { Field_Name = "U_ST_DISCOUNT_VALUE", Value = Prem_Data.Discount_Value });
            Premium_Field_Data.Add(new Field_Data() { Field_Name = "U_ST_WAITING_PERIOD", Value = Prem_Data.Waiting_Period });

            Utility.Update_UDO(company, UDO_Info, New_Membership_Code, Premium_Field_Data.ToArray());

            if (Create_Invoice_Automatically)
            {
                int NewEntry = Membership.Create_Invoice(company, New_Membership_Code, UDO_Info);
            }

        }

        internal static void Create_Renewal_MemberCard(Company company, string Parent_Code, UDO_Definition UDO_Membership_Info, string MemberCardType, string Parent_Membership_ID = null)
        {
            string Membership_Code = Utility.Get_Last_Individual_Membership_Per_Card(company, Parent_Code, UDO_Membership_Info);
            Field_Data[] Membership_Data = new Field_Data[] { };// = Utility.Get_UDO_Data(company, Membership_Code, UDO_Membership_Info);
            Create_Renewal(company, Membership_Code, UDO_Membership_Info, Membership_Data, false, MemberCardType , Parent_Membership_ID);
        }

        internal static void Update_Card_Not_Renewal(Company company, string Memeber_Card_Code, UDO_Definition UDO_Card_Info, string Reason)
        {
            Field_Data Fld_Note = new Field_Data() { Field_Name = "U_ST_RENEWAL_NOTE", Value = Reason };
            Field_Data Fld_Auto = new Field_Data() { Field_Name = "U_ST_AUTOMATIC_RENEWAL", Value = "N" };

            Utility.Update_UDO(company, UDO_Card_Info, Memeber_Card_Code, new Field_Data[] { Fld_Note, Fld_Auto });

        }

        internal static void Convert_Past_to_Renew(Company company, string Membership_Code, Parent_Form Form_Obj, UDO_Definition UDO_Info = null)
        {
            Field_Data Fld_Status = new Field_Data() { Field_Name = "U_ST_APPROVAL_STATUS", Value = "P" };

            Utility.Update_UDO(company, UDO_Info == null ? Form_Obj.UDO_Info : UDO_Info, Membership_Code, new Field_Data[] { Fld_Status });

            Form_Obj.Send_Alert_For_Approve(Membership_Code);
        }

        internal static void Active(Company company, string Membership_Code, string Prev_Membership_Code, UDO_Definition UDO_Info, bool Create_Invoice_Automatically)
        {
            //Field_Data Fld_Status = new Field_Data() { Field_Name = "U_ST_ACTIVE", Value = "N" };
            //if (Prev_Membership_Code != "")
            //{
            //    Utility.Update_UDO(company, UDO_Info, Prev_Membership_Code, new Field_Data[] { Fld_Status });
            //}
            //Fld_Status.Value = "Y";
            //Utility.Update_UDO(company, UDO_Info, Membership_Code, new Field_Data[] { Fld_Status });

            //Premium_Data Prem_Data = Calculate_Premium(company, Membership_Code);
            //List<Field_Data> Premium_Field_Data = new List<Field_Data>();
            //Premium_Field_Data.Add(new Field_Data() { Field_Name = "U_ST_PREMIUM", Value = Prem_Data.Premium_Amount });
            //Premium_Field_Data.Add(new Field_Data() { Field_Name = "U_ST_DISCOUNT_PERCENTAGE", Value = Prem_Data.Discount_Percentage });
            //Premium_Field_Data.Add(new Field_Data() { Field_Name = "U_ST_DISCOUNT_VALUE", Value = Prem_Data.Discount_Value });
            //Premium_Field_Data.Add(new Field_Data() { Field_Name = "U_ST_WAITING_PERIOD", Value = Prem_Data.Waiting_Period });

            //Utility.Update_UDO(company, UDO_Info, Membership_Code, Premium_Field_Data.ToArray());

            string SQL_Membership = $@"Select U_ST_INVOICE_NUMBER, U_ST_CURRENCY from ""@ST_INDIV_MEMBERSHIP"" where ""Code"" = '{Membership_Code}'";
            Recordset RC_Membership = Helper.Utility.Execute_Recordset_Query(company, SQL_Membership);
            int Downpayment_Entry = int.Parse(RC_Membership.Fields.Item("U_ST_INVOICE_NUMBER").Value.ToString());
            Documents Downpayment_Doc = (Documents)company.GetBusinessObject(BoObjectTypes.oDownPayments);
            Downpayment_Doc.GetByKey(Downpayment_Entry);
            Inoice_Data Inv_Data = new Inoice_Data();
            Inv_Data.Source_Code = Membership_Code;
            Inv_Data.Currency = RC_Membership.Fields.Item("U_ST_CURRENCY").Value.ToString();

            string Type;
            Create_Invoice(company, Inv_Data, UDO_Info, out Type, Downpayment_Doc);
            //}
        }

        internal static void Remove(Company company, string UDO_Code, UDO_Definition UDO_Info)
        {
            KHCF_Logic_Utility.Remove_UDO_Entry(company, UDO_Code, UDO_Info);
        }

        internal static void Stop_MemberCard(Company company, string Member_Card_Code, UDO_Definition UDO_Membership_Info, DateTime Stop_Date, string Stop_Note, bool Need_Permission = true)
        {
            string SQL_User = $@"Select U_ST_CAN_STOP_CARD from OUSR 
WHERE USER_CODE = '{company.UserName}'";
            Recordset RC_User = Helper.Utility.Execute_Recordset_Query(company, SQL_User);
            if (Need_Permission)
            {
                if (RC_User.Fields.Item("U_ST_CAN_STOP_CARD").Value.ToString() != "Y")
                {
                    throw new Logic.Custom_Exception("The user can't stop the Cards");
                }
            }

            try
            {
                company.StartTransaction();

                string Membership_Code = Utility.Get_Last_Individual_Membership_Per_Card(company, Member_Card_Code, UDO_Membership_Info);
                Field_Data Fld_Status = new Field_Data() { Field_Name = "U_ST_MEMBERSHIP_STATUS", Value = "S" };
                Field_Data Fld_Stop_Date = new Field_Data() { Field_Name = "U_ST_STOP_DATE", Value = Stop_Date };
                Field_Data Fld_Stop_Note = new Field_Data() { Field_Name = "U_ST_STOP_NOTE", Value = Stop_Note };

                Utility.Update_UDO(company, UDO_Membership_Info, Membership_Code, new Field_Data[] { Fld_Status, Fld_Stop_Date, Fld_Stop_Note });

                //                string SQL_Membership = $@"SELECT T0.U_ST_START_DATE, T0.U_ST_END_DATE, T0.U_ST_INVOICE_NUMBER, T0.U_ST_STOP_DATE 
                //FROM ""@{UDO_Membership_Info.Table_Name}""  T0 
                //WHERE T0.""Code"" = '{Membership_Code}'";
                //                Recordset RC_Membership = Helper.Utility.Execute_Recordset_Query(company, SQL_Membership);
                //                DateTime Start_Date = (DateTime)RC_Membership.Fields.Item("U_ST_START_DATE").Value;
                //                DateTime End_Date = (DateTime)RC_Membership.Fields.Item("U_ST_END_DATE").Value;
                Documents Doc_Inv = Utility.Get_Membership_Invoice(company, Membership_Code);
                if (Doc_Inv != null)
                {
                    Documents Doc_Mem = Create_Rest_Credit_Note(company, Membership_Code, UDO_Membership_Info, Doc_Inv, Stop_Date);

                    KHCF_Logic_Utility.Create_Revenue_Realization_Cancellation(company, Doc_Mem.DocEntry, Doc_Mem.DocNum, Doc_Inv.DocNum);

                }


                company.EndTransaction(BoWfTransOpt.wf_Commit);


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
                throw new Logic.Custom_Exception($"Error during stop the Member Card[{Member_Card_Code}],[{ex.Message}] ");
            }

        }

        internal static void Stop_Individual_Membership(Company company, string UDO_Code, UDO_Definition UDO_Info, DateTime StopDate, string Stop_Note, bool With_Transaction = true)
        {
            string SQL = $@"Select U_ST_CAN_STOP_MEMBERSHIP from OUSR WHERE USER_CODE = '{company.UserName}'";
            Recordset RC = Helper.Utility.Execute_Recordset_Query(company, SQL);
            bool Can_Stop = RC.Fields.Item(0).Value.ToString() == "Y";
            if (!Can_Stop)
            {
                throw new Logic.Custom_Exception("You are not authorized to stop the Membership");
            }
            if (Stop_Note == "")
            {
                throw new Logic.Custom_Exception($"Cannot stop the membership[{UDO_Code}]. Stop reason is empty.");
            }
            Field_Data Status = Utility.Get_Field_Value(company, UDO_Info, UDO_Code, "U_ST_MEMBERSHIP_STATUS");
            if (!(new string[] { "N", "R", "P" }).Contains(Status.Value.ToString()))
            {
                throw new Logic.Custom_Exception($"According to the membership[{UDO_Code}] current status, it cannot be stopped.");
            }

            try
            {
                if (With_Transaction == true)
                {
                    company.StartTransaction();
                }
                Field_Data Fld_Status = new Field_Data() { Field_Name = "U_ST_MEMBERSHIP_STATUS", Value = "S" };
                Field_Data Fld_Active = new Field_Data() { Field_Name = "U_ST_ACTIVE", Value = "N" };
                Field_Data Fld_StopDate = new Field_Data() { Field_Name = "U_ST_STOP_DATE", Value = StopDate };
                Field_Data Fld_Stop_Note = new Field_Data() { Field_Name = "U_ST_STOP_NOTE", Value = Stop_Note };

                Utility.Update_UDO(company, UDO_Info, UDO_Code, new Field_Data[] { Fld_Status, Fld_Active, Fld_StopDate, Fld_Stop_Note });
                Documents Doc_Inv = null;

                if (UDO_Info.Table_Name == "ST_INDIV_MEMBERSHIP")
                    Doc_Inv = Utility.Get_Membership_Invoice(company, UDO_Code);
                else if (UDO_Info.Table_Name == "ST_CORP_MEMBERSHIP")
                    Doc_Inv = Utility.Get_Corporate_Invoice(company, UDO_Code);


                //if (Doc_Inv == null)
                //    throw new Custom_Exception("This membership has No invoices. Please remove it.");

                if (Doc_Inv != null)
                {
                    Field_Data Start_Date = Utility.Get_Field_Value(company, UDO_Info, UDO_Code, "U_ST_START_DATE");
                    Field_Data End_Date = Utility.Get_Field_Value(company, UDO_Info, UDO_Code, "U_ST_END_DATE");
                    DateTime startdate = Convert.ToDateTime(Start_Date.Value.ToString());
                    DateTime enddate = Convert.ToDateTime(End_Date.Value.ToString());

                    //if (!(StopDate <= enddate && startdate < StopDate && StopDate <= DateTime.Now))
                    if (!(StopDate <= enddate && startdate < StopDate ))
                    {
                        throw new Custom_Exception("Stop date should be between the start and the end dates of the membership and should not be in the future.");
                    }
                    if (Doc_Inv.DocObjectCodeEx == "13")
                    {
                        Documents Doc_Mem = Create_Rest_Credit_Note(company, UDO_Code, UDO_Info, Doc_Inv, StopDate);
                        KHCF_Logic_Utility.Create_Revenue_Realization_Cancellation(company, Doc_Mem.DocEntry, Doc_Mem.DocNum, Doc_Inv.DocNum);
                    }
                    else
                    {
                        Inoice_Data Inv_Data = new Inoice_Data();
                        Inv_Data.Source_Code = UDO_Code;
                        Inv_Data.Is_One_Installment = true;

                        string Type;
                        Create_Invoice(company, Inv_Data, UDO_Info, out Type, null, true);
                    }
                }
                if (With_Transaction == true)
                {
                    company.EndTransaction(BoWfTransOpt.wf_Commit);
                }
            }
            catch (Exception ex)
            {
                try
                {
                    if (With_Transaction == true)
                    {
                        company.EndTransaction(BoWfTransOpt.wf_RollBack);
                    }
                }
                catch (Exception)
                { }
                throw new Logic.Custom_Exception($"Error during Stop the Membership[{UDO_Code}][{ex.Message}]");
            }
        }

        internal static void Create_Child_MemberShip(Company company, string Memeber_Code, UDO_Definition UDO_Info, string Membership_Parent_ID,string Parent_Type, string Children_Card)
        {
            string tableName;
            if (Parent_Type == "C")
                tableName = "ST_CORP_MEMBERSHIP";
            else
                tableName = "ST_INDIV_MEMBERSHIP";

            string SQL_Membership_Parent = $@"SELECT * FROM ""@{tableName}"" T0 where T0.""Code""='{Membership_Parent_ID}'";
            Recordset RC_Parent_MemberShip = Helper.Utility.Execute_Recordset_Query(company, SQL_Membership_Parent);
            string SQL_Child_Card = $@"Select * from ""@ST_CCI_INDIV_CARD"" T0 where T0.""Code""='{Children_Card}'";
            Recordset RC__Child_Card = Helper.Utility.Execute_Recordset_Query(company, SQL_Child_Card);
            Field_Data[] Membership_Fields = new Field_Data[18];
            string New_Code = string.Empty;
            if (RC_Parent_MemberShip.RecordCount > 0)
            {
                DateTime StartDate = Convert.ToDateTime(RC_Parent_MemberShip.Fields.Item("U_ST_START_DATE").Value.ToString());
                int Age = (StartDate - ((DateTime)RC__Child_Card.Fields.Item("U_ST_DATE_OF_BIRTH").Value)).Days / 365;
                string Coverage_Rule = RC_Parent_MemberShip.Fields.Item("U_ST_COVERAGE").Value.ToString();

                Membership_Fields[0] = new Field_Data() { Field_Name = "U_ST_CREATION_DATE", Value = RC_Parent_MemberShip.Fields.Item("U_ST_CREATION_DATE").Value };
                Membership_Fields[1] = new Field_Data() { Field_Name = "U_ST_MEMBERSHIP_STATUS", Value = RC_Parent_MemberShip.Fields.Item("U_ST_MEMBERSHIP_STATUS").Value };
                Membership_Fields[2] = new Field_Data() { Field_Name = "U_ST_START_DATE", Value = RC_Parent_MemberShip.Fields.Item("U_ST_START_DATE").Value };
                Membership_Fields[3] = new Field_Data() { Field_Name = "U_ST_END_DATE", Value = RC_Parent_MemberShip.Fields.Item("U_ST_END_DATE").Value };
                Membership_Fields[4] = new Field_Data() { Field_Name = "U_ST_MEMBER_CARD", Value = Children_Card };
                Membership_Fields[5] = new Field_Data() { Field_Name = "U_ST_MEMBER_NAME", Value = RC__Child_Card.Fields.Item("U_ST_FULL_NAME_AR").Value };
                if (Parent_Type != "C")
                {
                    Membership_Fields[6] = new Field_Data() { Field_Name = "U_ST_CHANNEL", Value = RC_Parent_MemberShip.Fields.Item("U_ST_CHANNEL").Value };
                    Membership_Fields[7] = new Field_Data() { Field_Name = "U_ST_SUB_CHANNEL", Value = RC_Parent_MemberShip.Fields.Item("U_ST_SUB_CHANNEL").Value };
                    Membership_Fields[8] = new Field_Data() { Field_Name = "U_ST_CURRENCY", Value = RC_Parent_MemberShip.Fields.Item("U_ST_CURRENCY").Value };
                    Membership_Fields[9] = new Field_Data() { Field_Name = "U_ST_PAYMENT_METHOD", Value = RC_Parent_MemberShip.Fields.Item("U_ST_PAYMENT_METHOD").Value };
                }
                else
                {
                    Membership_Fields[6] = new Field_Data() { Field_Name = "U_ST_CHANNEL", Value = RC__Child_Card.Fields.Item("U_ST_CHANNEL").Value };
                    Membership_Fields[7] = new Field_Data() { Field_Name = "U_ST_SUB_CHANNEL", Value = RC__Child_Card.Fields.Item("U_ST_SUB_CHANNEL").Value };
                    Membership_Fields[8] = new Field_Data() { Field_Name = "U_ST_CURRENCY", Value = RC__Child_Card.Fields.Item("U_ST_CURRENCY").Value };
                    Membership_Fields[9] = new Field_Data() { Field_Name = "U_ST_PAYMENT_METHOD", Value = "" };
                }
               
                Membership_Fields[10] = new Field_Data() { Field_Name = "U_ST_ACCOUNT_MANAGER", Value = RC_Parent_MemberShip.Fields.Item("U_ST_ACCOUNT_MANAGER").Value };
                Membership_Fields[11] = new Field_Data() { Field_Name = "U_ST_INSTALLMENT_TYPE", Value = RC_Parent_MemberShip.Fields.Item("U_ST_INSTALLMENT_TYPE").Value };
                
                Membership_Fields[12] = new Field_Data() { Field_Name = "U_ST_CREATOR", Value = RC_Parent_MemberShip.Fields.Item("U_ST_CREATOR").Value };
                Membership_Fields[13] = new Field_Data() { Field_Name = "U_ST_COVERAGE", Value = Coverage_Rule };
                Membership_Fields[14] = new Field_Data() { Field_Name = "U_ST_PARENT_MEMBERSHIP_ID", Value = Membership_Parent_ID };
                Membership_Fields[15] = new Field_Data() { Field_Name = "U_ST_AGE", Value = Age.ToString() };
                if(Parent_Type != "C")
                 Membership_Fields[16] = new Field_Data() { Field_Name = "U_ST_EMPLOYEE_ID", Value = RC_Parent_MemberShip.Fields.Item("U_ST_EMPLOYEE_ID").Value };
                else
                    Membership_Fields[16] = new Field_Data() { Field_Name = "U_ST_EMPLOYEE_ID", Value = "185" };
                Membership_Fields[17] = new Field_Data() { Field_Name = "U_ST_PARENT_MEMBERSHIP_TYPE", Value = Parent_Type };


                if(Parent_Type == "C")
                {
                    DateTime Corp_Membership_End_Date = (DateTime)RC_Parent_MemberShip.Fields.Item("U_ST_END_DATE").Value;
                    DateTime Corp_Membership_Start_Date = (DateTime)RC_Parent_MemberShip.Fields.Item("U_ST_START_DATE").Value;
                    // Discussing the The age, start date, end date and membership status in case the individual member has memberships before
                    UDO_Definition Membership_UDO_Info = Logic.Objects_Logic.All_UDO_Definition.FirstOrDefault(U => U.KHCF_Object == KHCF_Objects.Individual_Membership);
                    string SQL_Last_Child_Membership = $@"SELECT top 1 T0.""Code"" , T0.""U_ST_END_DATE"" FROM ""@{Membership_UDO_Info.Table_Name}""  T0  
 WHERE T0.U_ST_MEMBER_CARD = '{Children_Card}' AND T0.""U_ST_MEMBERSHIP_STATUS"" <> 'C'  
 ORDER BY U_ST_END_DATE DESC ";
                    Recordset RC_Last_Child_Membership = Helper.Utility.Execute_Recordset_Query(company, SQL_Last_Child_Membership);

                    if (RC_Last_Child_Membership.RecordCount > 0)
                    {
                        DateTime Old_End_Date = (DateTime)RC_Last_Child_Membership.Fields.Item("U_ST_END_DATE").Value;
                        if (Old_End_Date <= Corp_Membership_End_Date && Old_End_Date >= Corp_Membership_Start_Date)
                            throw new Custom_Exception($@"Cannot renew membership for individual card [{Children_Card}]. Last membership is overlapping with the corporate membership.");

                        bool Is_Past;
                        DateTime New_Renewal_StartDate = Membership.Get_New_Renewal_StartDate(company, Old_End_Date, out Is_Past,Corp_Membership_Start_Date);
                        DateTime New_Renewal_EndDate = New_Renewal_StartDate.AddMonths(Configurations.Get_Renewal_Month_for_End_Date(company, true)).AddDays(-1);
                        New_Renewal_EndDate.AddDays(-1);

                        Age = (New_Renewal_StartDate - ((DateTime)RC__Child_Card.Fields.Item("U_ST_DATE_OF_BIRTH").Value)).Days / 365;

                        Field_Data FldAge = new Field_Data() { Field_Name = "U_ST_AGE", Value = Age.ToString() };
                        Field_Data FldStartDate = new Field_Data() { Field_Name = "U_ST_START_DATE", Value = New_Renewal_StartDate };
                        Field_Data FldEndDate = new Field_Data() { Field_Name = "U_ST_END_DATE", Value = New_Renewal_EndDate };
                        Field_Data FldMembershipStatus = new Field_Data() { Field_Name = "U_ST_MEMBERSHIP_STATUS", Value = string.Empty };

                        if (Is_Past)
                        {
                            FldMembershipStatus.Value = "P";
                        }
                        else
                        {
                            FldMembershipStatus.Value = "R";
                        }

                        Membership_Fields[15] = FldAge; Membership_Fields[2] = FldStartDate; Membership_Fields[3] = FldEndDate; Membership_Fields[1] = FldMembershipStatus;
                    }

                    // discussing the regular and client types of corporates; determining the end dates in each one
                    
                    DateTime Membership_StartDate = (DateTime)Membership_Fields[2].Value;
                    string Corp_Type = RC_Parent_MemberShip.Fields.Item("U_ST_CORPORATE_TYPE").Value.ToString();
                    if (Corp_Type == "R")
                    {
                        Membership_Fields[3].Value = Corp_Membership_End_Date;
                    }
                    if (Corp_Type == "C")
                    {
                        Membership_Fields[3].Value = Membership_StartDate.AddMonths(12).AddDays(-1);
                    }

                }

                New_Code = Utility.Add_UDO_Entry(company, UDO_Info, Membership_Fields, null);
                if (Parent_Type == "I")
                {
                    Field_Data Fld = new Field_Data() { Field_Name= "U_ST_APPLICATION_NUMBER" , Value = RC_Parent_MemberShip.Fields.Item("U_ST_APPLICATION_NUMBER").Value };
                    Utility.Update_UDO(company, UDO_Info, New_Code, new Field_Data[] { Fld });
                }
            }

        }

        internal static void IndividualCardApprovalInfo(Company company, string Member_Card_Code, out string LastMembershipInvoice, out bool HasFinancialImpact)
        {
            LastMembershipInvoice = null;
            HasFinancialImpact = false;


            string SQL_Membership_With_No_Invoice = $@"
SELECT TOP 1 T0.""U_ST_INVOICE_NUMBER"" FROM ""@ST_INDIV_MEMBERSHIP"" T0 WHERE T0.""U_ST_MEMBER_CARD"" ='{Member_Card_Code}' 
 AND T0.""U_ST_MEMBERSHIP_STATUS"" <> 'C' 
 ORDER BY T0.""U_ST_END_DATE"" DESC
";

            Recordset RC_With_Invoice = Helper.Utility.Execute_Recordset_Query(company, SQL_Membership_With_No_Invoice);
            if (RC_With_Invoice.RecordCount > 0)
            {
                LastMembershipInvoice = RC_With_Invoice.Fields.Item(0).Value.ToString();
            }
            else if (RC_With_Invoice.RecordCount == 0)
            {
                LastMembershipInvoice = "NO-MEMBERSHIPS";
            }

            string SQL_Member = $@"
SELECT COUNT(*) FROM ""@ST_INDIV_MEMBERSHIP"" T0 WHERE T0.""U_ST_MEMBER_CARD"" ='{Member_Card_Code}' 
AND T0.""U_ST_INVOICE_NUMBER"" IS NOT NULL AND T0.""U_ST_INVOICE_NUMBER"" <> '' 
";
            Recordset RC_Memberships = Helper.Utility.Execute_Recordset_Query(company, SQL_Member);
            if (RC_Memberships.RecordCount > 0)
            {
                int number_Of_memberships = Convert.ToInt32(RC_Memberships.Fields.Item(0).Value);
                if (number_Of_memberships > 0)
                    HasFinancialImpact = true;
            }
        }

        internal static void CorporateCardApprovalInfo(Company company, string Corporate_Card_Code, out string LastMembershipInvoice, out bool HasFinancialImpact)
        {
            LastMembershipInvoice = null;
            HasFinancialImpact = false;


            string SQL_Membership_With_No_Invoice = $@"
SELECT TOP 1 T2.""U_ST_INVOICE_NUMBER"" 
FROM ""@ST_CCI_CORP_CARD"" T0
INNER JOIN ""@ST_CORP_MEMBERSHIP"" T1 ON (T0.""Code"" = T1.""U_ST_MEMBER_CARD"")
INNER JOIN ""@ST_INDIV_MEMBERSHIP"" T2 ON ( T2.""U_ST_PARENT_MEMBERSHIP_ID"" = T1.""Code"" AND T2.""U_ST_PARENT_MEMBERSHIP_TYPE"" = 'C' )
WHERE T0.""Code"" = '{Corporate_Card_Code}' 
AND T2.""U_ST_MEMBERSHIP_STATUS"" <> 'C'
ORDER BY T1.""U_ST_END_DATE"" DESC
";

            Recordset RC_With_Invoice = Helper.Utility.Execute_Recordset_Query(company, SQL_Membership_With_No_Invoice);
            if (RC_With_Invoice.RecordCount > 0)
            {
                LastMembershipInvoice = RC_With_Invoice.Fields.Item(0).Value.ToString();
            }
            else if(RC_With_Invoice.RecordCount == 0)
            {
                LastMembershipInvoice = "NO-MEMBERSHIPS";
            }

            string SQL_Member = $@"
SELECT COUNT(*)
FROM ""@ST_CCI_CORP_CARD"" T0
INNER JOIN ""@ST_CORP_MEMBERSHIP"" T1 ON (T0.""Code"" = T1.""U_ST_MEMBER_CARD"")
INNER JOIN ""@ST_INDIV_MEMBERSHIP"" T2 ON ( T2.""U_ST_PARENT_MEMBERSHIP_ID"" = T1.""Code"" AND T2.""U_ST_PARENT_MEMBERSHIP_TYPE"" = 'C' )
WHERE T0.""Code"" = '{Corporate_Card_Code}' 
AND T2.""U_ST_INVOICE_NUMBER"" IS NOT NULL AND T2.""U_ST_INVOICE_NUMBER"" <> ''
";
            Recordset RC_Memberships = Helper.Utility.Execute_Recordset_Query(company, SQL_Member);
            if (RC_Memberships.RecordCount > 0)
            {
                int number_Of_memberships = Convert.ToInt32(RC_Memberships.Fields.Item(0).Value);
                if (number_Of_memberships > 0)
                    HasFinancialImpact = true;
            }
        }

        internal static string SQL_Parent_Memberships(string parentMembershipID, string parentMembershipType, string approvalValue, string membershipStatusValue)
        {
            string result = string.Empty;
            result =
$@"SELECT T0.""Code"", T0.""U_ST_MEMBER_NAME"",T0.""U_ST_MEMBERSHIP_STATUS"" 
FROM ""@ST_INDIV_MEMBERSHIP"" T0 WHERE T0.""U_ST_PARENT_MEMBERSHIP_ID"" = '{parentMembershipID}' AND T0.""U_ST_PARENT_MEMBERSHIP_TYPE"" = '{parentMembershipType}' ";
            if (!string.IsNullOrEmpty(membershipStatusValue))
                result += $@" AND T0.""U_ST_MEMBERSHIP_STATUS"" = '{membershipStatusValue}' ";
            if (!string.IsNullOrEmpty(approvalValue))
                result += $@" AND T0.""U_ST_APPROVAL_STATUS"" = '{approvalValue}' ";
            return result;
        }

        internal static bool Check_Membership_Children(Company company,string UDO_Code, string MemberCardType)
        {
            string SQL_Membership = $@"SELECT T0.""Code"", T0.""U_ST_MEMBER_NAME"",T0.""U_ST_START_DATE"", T0.""U_ST_END_DATE"", 
CASE WHEN T0.""U_ST_MEMBERSHIP_STATUS"" = 'C' Then 'Canceled' When  T0.""U_ST_MEMBERSHIP_STATUS"" = 'N' then 'New' When  T0.""U_ST_MEMBERSHIP_STATUS"" = 'P' then 'Past Renew' 
When  T0.""U_ST_MEMBERSHIP_STATUS"" = 'R' Then 'Renew' End As U_ST_MEMBERSHIP_STATUS FROM ""@ST_INDIV_MEMBERSHIP""  T0 WHERE T0.""U_ST_MEMBERSHIP_STATUS"" in ('N','R','P') And T0.""U_ST_APPROVAL_STATUS"" ='A'
            AND T0.""U_ST_PARENT_MEMBERSHIP_ID"" = '{UDO_Code}' AND T0.""U_ST_PARENT_MEMBERSHIP_TYPE"" = '{MemberCardType}' ";

            Recordset RC_Child = Helper.Utility.Execute_Recordset_Query(company, SQL_Membership);
            if (RC_Child.RecordCount > 0)
                return true;
            else
                return false;

        }


    }
}

