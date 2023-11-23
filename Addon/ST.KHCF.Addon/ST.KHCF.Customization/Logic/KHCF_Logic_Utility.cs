using SAPbobsCOM;
using ST.KHCF.Customization.Forms;
using ST.KHCF.Customization.Logic.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace ST.KHCF.Customization.Logic
{
    internal class KHCF_Logic_Utility
    {
        internal enum Patient_Type
        {
            CCI = 1,
            Goodwill = 2,
            Other_CCI = 3

        }
        internal static string Get_Created_UserCode(Company company, string UDO_Code, UDO_Definition UDO_Info)
        {
            string SQL = $@"SELECT T1.USER_CODE FROM ""@{UDO_Info.Table_Name}""  T0 INNER JOIN OUSR T1 ON T0.""UserSign"" = USERID 
WHERE T0.""Code"" = '{UDO_Code}'";
            Recordset RC = Helper.Utility.Execute_Recordset_Query(company, SQL);
            return RC.Fields.Item("USER_CODE").Value.ToString();
        }

        internal static void Link(Company company, string UDO_Code, string Parent_Type, string Parent_ID, string Parent_Name, UDO_Definition UDO_Info)
        {
            if (Parent_ID == "")
            {
                throw new Logic.Custom_Exception("The Parent ID is empty");
            }


            try
            {

                company.StartTransaction();

                string TableName;
                UDO_Definition UDO_Parent_Info;
                if (UDO_Info.UDO_Modules == KHCF_Modules.CCI)
                {
                    if (Parent_Type == "I")
                    {
                        UDO_Parent_Info = Objects_Logic.All_UDO_Definition.FirstOrDefault(U => U.KHCF_Object == KHCF_Objects.CCI_Member_Card);
                    }
                    else
                    {
                        UDO_Parent_Info = Objects_Logic.All_UDO_Definition.FirstOrDefault(U => U.KHCF_Object == KHCF_Objects.CCI_Corporate_Member_Card);
                    }
                }
                else
                {
                    if (Parent_Type == "I")
                    {
                        UDO_Parent_Info = Objects_Logic.All_UDO_Definition.FirstOrDefault(U => U.KHCF_Object == KHCF_Objects.Fundraising_Individual_Card);
                    }
                    else
                    {
                        UDO_Parent_Info = Objects_Logic.All_UDO_Definition.FirstOrDefault(U => U.KHCF_Object == KHCF_Objects.Fundraising_Corporate_Card);
                    }
                }
                TableName = "@" + UDO_Parent_Info.Table_Name;
                //if (Parent_Type == "I")
                //{
                //    TableName = "@" + Objects_Logic.All_UDO_Definition.FirstOrDefault(U => U.KHCF_Object == KHCF_Objects.CCI_Member_Card).Table_Name;
                //}
                //else
                //{
                //    TableName = "@" + Objects_Logic.All_UDO_Definition.FirstOrDefault(U => U.KHCF_Object == KHCF_Objects.CCI_Corporate_Member_Card).Table_Name;
                //}
                string SQL_Parent = $@"SELECT U_ST_BP_CODE, U_ST_FATHER_TYPE, U_ST_CUSTOMER_GROUP  FROM ""{TableName}"" WHERE ""Code"" = '{Parent_ID}' ";
                Recordset RC_Parent = Helper.Utility.Execute_Recordset_Query(company, SQL_Parent);
                if (RC_Parent.RecordCount == 0)
                {
                    throw new Logic.Custom_Exception($"We can't find the card for the Parent ID[{Parent_ID}]");
                }
                string Parent_BP = RC_Parent.Fields.Item("U_ST_BP_CODE").Value.ToString();
                if (Parent_BP == "")
                {
                    throw new Logic.Custom_Exception($"There is no Business Partner for the Parent ID[{Parent_ID}]");
                }

                bool hasMemberships; string lastMembershipInvoice;
                Membership.IndividualCardApprovalInfo(company, UDO_Code, out lastMembershipInvoice, out hasMemberships);

                string Parent_Customer_Group = RC_Parent.Fields.Item("U_ST_CUSTOMER_GROUP").Value.ToString();
                Field_Data Fld_Parent_ID = new Field_Data() { Field_Name = "U_ST_PARENT_ID", Value = Parent_ID };
                Field_Data Fld_Parent_Name = new Field_Data() { Field_Name = "U_ST_PARENT_NAME", Value = Parent_Name };
                Field_Data Fld_Parent_Type = new Field_Data() { Field_Name = "U_ST_PARENT_TYPE", Value = Parent_Type };
                Field_Data Fld_Parent_Group = new Field_Data() { Field_Name = "U_ST_CUSTOMER_GROUP", Value = Parent_Customer_Group };
                Utility.Update_UDO(company, UDO_Info, UDO_Code, new Field_Data[] { Fld_Parent_ID, Fld_Parent_Type, Fld_Parent_Name, Fld_Parent_Group });
                string BP_Code = BP_Code = Utility.Get_BP_Code(company, UDO_Code, UDO_Info);

                //if (Parent_Type == "I")
                //    BP_Code = Utility.Get_BP_Code(company, UDO_Code, UDO_Info);
                if (BP_Code != "")
                {
                    BusinessPartners BP = (BusinessPartners)company.GetBusinessObject(BoObjectTypes.oBusinessPartners);
                    BP.GetByKey(BP_Code);
                    BP.GroupCode = int.Parse(Parent_Customer_Group);
                    BP.FatherCard = Parent_BP;
                    BusinessPartnerGroups bpGroup = (BusinessPartnerGroups)company.GetBusinessObject(BoObjectTypes.oBusinessPartnerGroups);
                    bpGroup.GetByKey(BP.GroupCode);
                    string s = bpGroup.UserFields.Fields.Item("U_ST_GL_ACCOUNT").Value.ToString();
                    BP.DebitorAccount = bpGroup.UserFields.Fields.Item("U_ST_GL_ACCOUNT").Value.ToString();
                    if (RC_Parent.Fields.Item("U_ST_FATHER_TYPE").Value.ToString() == "P")
                    {
                        BP.FatherType = BoFatherCardTypes.cPayments_sum;
                    }
                    else
                    {
                        BP.FatherType = BoFatherCardTypes.cDelivery_sum;
                    }

                    if (BP.Update() != 0)
                    {
                        throw new Logic.Custom_Exception($"Error during update the BP[{BP_Code}][{company.GetLastErrorDescription()}]");
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
                {

                }
                throw new Logic.Custom_Exception($"Error during link the Card[{UDO_Code}][{ex.Message}]");
            }
        }

        internal static void Remove_UDO_Entry(Company company, string UDO_Code, UDO_Definition UDO_Info, bool With_DB_Transaction = true, bool With_Validation = true)
        {
            if (With_Validation == true)
            {
                Remove_UDO_Validation(company, UDO_Code, UDO_Info, true);
            }

            if (With_DB_Transaction == true)
            {
                company.StartTransaction();
            }
            try
            {
                if (UDO_Info.KHCF_Object == KHCF_Objects.CCI_Corporate_Member_Card || UDO_Info.KHCF_Object == KHCF_Objects.CCI_Member_Card)
                {
                    UDO_Definition Sub_UDO_Info = Logic.Objects_Logic.All_UDO_Definition.FirstOrDefault(U => U.KHCF_Object == KHCF_Objects.CCI_Member_Card);
                    Tuple<string, string>[] Child_Codes = Get_Member_Child_Codes(company, UDO_Code, UDO_Info);

                    foreach (Tuple<string, string> OneCode in Child_Codes)
                    {
                        //Unlink(company, OneCode.Item1, OneCode.Item2, Sub_UDO_Info, false);
                        if (Remove_UDO_Validation(company, OneCode.Item1, Sub_UDO_Info, false) == true)
                        {
                            Remove_UDO_Entry(company, OneCode.Item1, Sub_UDO_Info, false, false);
                        }
                    }
                }

                if (UDO_Info.Has_BP)
                {
                    string BP_Code = Utility.Get_BP_Code(company, UDO_Code, UDO_Info);
                    if (BP_Code != "")
                    {
                        BusinessPartners BP = (BusinessPartners)company.GetBusinessObject(BoObjectTypes.oBusinessPartners);
                        BP.GetByKey(BP_Code);
                        if (BP.Remove() != 0)
                        {
                            throw new Logic.Custom_Exception($"Error during remove the BP[{BP_Code}][{company.GetLastErrorDescription()}]");
                        }
                    }
                }
                CompanyService oCmpSrv = company.GetCompanyService();
                //string UDO_Code = Utility.Get_UDO_Type_ID(UDO_Info.KHCF_Object);
                int DocEntry = Utility.Get_UDO_DocEntry(company, UDO_Code, UDO_Info);
                GeneralService oGeneralService = oCmpSrv.GetGeneralService(Utility.Get_UDO_Type_ID(UDO_Info.KHCF_Object));
                GeneralDataParams oGeneralParams = (GeneralDataParams)oGeneralService.GetDataInterface(GeneralServiceDataInterfaces.gsGeneralDataParams);
                //oGeneralParams.SetProperty("DocEntry", DocEntry.ToString());
                oGeneralParams.SetProperty("Code", UDO_Code);
                //oGeneralParams.SetProperty("Code", UDO_Code);
                //GeneralData oGeneralData = oGeneralService.GetByParams(oGeneralParams);

                oGeneralService.Delete(oGeneralParams);

                if (With_DB_Transaction == true)
                {
                    company.EndTransaction(BoWfTransOpt.wf_Commit);
                }

            }
            catch (Exception ex)
            {
                if (With_DB_Transaction == true)
                {
                    try
                    {
                        company.EndTransaction(BoWfTransOpt.wf_RollBack);
                    }
                    catch (Exception)
                    { }
                }
                throw new Logic.Custom_Exception($"Error during remove the UDO entry[{UDO_Code}][{ex.Message}]");
            }
        }

        private static Tuple<string, string>[] Get_Member_Child_Codes(Company company, string UDO_Code, UDO_Definition UDO_Info)
        {
            string Parent_Type;
            if (UDO_Info.KHCF_Object == KHCF_Objects.CCI_Corporate_Member_Card)
            {
                Parent_Type = "C";
            }
            else
            {
                Parent_Type = "I";
            }
            string SQL = $@"SELECT ""Code"", U_ST_BP_CODE FROM ""@ST_CCI_INDIV_CARD""  T1 
                WHERE T1.""U_ST_PARENT_ID"" = '{UDO_Code}' AND  T1.""U_ST_PARENT_TYPE"" = '{Parent_Type}'";
            Recordset RC = Helper.Utility.Execute_Recordset_Query(company, SQL);

            List<Tuple<string, string>> Result = new List<Tuple<string, string>>();

            for (int i = 0; i < RC.RecordCount; i++)
            {
                Result.Add(new Tuple<string, string>(RC.Fields.Item("Code").Value.ToString(), RC.Fields.Item("U_ST_BP_CODE").Value.ToString()));
                RC.MoveNext();
            }

            return Result.ToArray();
        }

        private static bool Remove_UDO_Validation(Company company, string UDO_Code, UDO_Definition UDO_Info, bool Throw_Error)
        {
            if (UDO_Info.KHCF_Object == KHCF_Objects.Individual_Membership)
            {
                string SQL = $@"SELECT T0.U_ST_INVOICE_NUMBER FROM ""@ST_INDIV_MEMBERSHIP"" T0 WHERE T0.""Code"" = '{UDO_Code}'";
                Recordset RC = Helper.Utility.Execute_Recordset_Query(company, SQL);
                if (RC.Fields.Item("U_ST_INVOICE_NUMBER").Value.ToString() != "")
                {
                    if (Throw_Error)
                    {
                        throw new Logic.Custom_Exception($"Membership[{UDO_Code}] cannot be removed because it is related with invoice[{RC.Fields.Item("U_ST_INVOICE_NUMBER").Value}]");
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            if (UDO_Info.KHCF_Object == KHCF_Objects.Corporate_Membership)
            {
                string SQL = $@"SELECT T0.U_ST_INVOICE_NUMBER FROM ""@ST_CORP_MEMBERSHIP"" T0 WHERE T0.""Code"" = '{UDO_Code}'";
                string relatedIndividualMembershipsSQL = $@"SELECT T0.""Code"" FROM  ""@ST_INDIV_MEMBERSHIP"" 
T0 WHERE T0.""U_ST_PARENT_MEMBERSHIP_ID"" = '{UDO_Code}' AND T0.""ST_PARENT_MEMBERSHIP_TYPE"" = 'C' ";
                Recordset RC = Helper.Utility.Execute_Recordset_Query(company, SQL);
                Recordset RC2 = Helper.Utility.Execute_Recordset_Query(company, SQL);
                if (RC.Fields.Item("U_ST_INVOICE_NUMBER").Value.ToString() != "" || RC2.Fields.Item("U_ST_INVOICE_NUMBER").Value.ToString() != "")
                {
                    if (Throw_Error)
                    {
                        throw new Logic.Custom_Exception($"Membership[{UDO_Code}] cannot be removed because it is related with invoice(s).");
                    }
                    else
                    {
                        return false;
                    }
                }
                    
            }

            if (UDO_Info.KHCF_Object == KHCF_Objects.CCI_Member_Card)
            {
                string SQL = $@"SELECT COUNT(*) FROM ""@ST_INDIV_MEMBERSHIP""  T0 WHERE T0.""U_ST_MEMBER_CARD"" = '{UDO_Code}'";
                Recordset RC = Helper.Utility.Execute_Recordset_Query(company, SQL);
                if ((int)RC.Fields.Item(0).Value != 0)
                {
                    if (Throw_Error)
                    {
                        throw new Logic.Custom_Exception($"We can't remove the Member Card[{UDO_Code}] because it is related with Memberships");
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            //            if (UDO_Info.KHCF_Object == KHCF_Objects.CCI_Corporate_Member_Card)
            //            {
            //                string SQL = $@"SELECT COUNT(*) FROM ""@ST_CCI_INDIV_CARD""  T1 
            //WHERE T1.""U_ST_PARENT_ID"" = '{UDO_Code}' AND  T1.""U_ST_PARENT_TYPE"" = 'C'";
            //                Recordset RC = Helper.Utility.Execute_Recordset_Query(company, SQL);
            //                if ((int)RC.Fields.Item(0).Value != 0)
            //                {
            //                    throw new Logic.Custom_Exception($"We can't remove the Member Card[{UDO_Code}] because it is related with Child Member Card");
            //                }
            //            }
            return true;

        }

        internal static void Create_Revenue_Realization_Cancellation(Company company, int CM_DocEntry, int CM_DocNum, int Inv_DocNum)
        {
            string SQL = $@"SELECT T0.""TransId"" FROM OJDT T0 WHERE T0.U_ST_REVENUE_REALIZATION_INV_NUM = {Inv_DocNum}";
            Recordset RC = Helper.Utility.Execute_Recordset_Query(company, SQL);
            string Allowance_Account = Configurations.Get_Allowance_Account(company);
            //company.StartTransaction();
            try
            {
                for (int i = 0; i < RC.RecordCount; i++)
                {
                    int JE_ID = (int)RC.Fields.Item("TransId").Value;
                    JournalEntries JE = (JournalEntries)company.GetBusinessObject(BoObjectTypes.oJournalEntries);
                    JE.GetByKey(JE_ID);
                    string Currency = JE.Lines.FCCurrency;

                    JournalEntries JE_Cancel = (JournalEntries)company.GetBusinessObject(BoObjectTypes.oJournalEntries);
                    // JE_Cancel.ReferenceDate = End_Date;
                    JE_Cancel.ReferenceDate = JE_Cancel.TaxDate = JE.TaxDate = DateTime.Today;
                    string Remarks = $@"Created for Revenue Realization for Invoice Cancellation for Credit Note[{CM_DocNum}]";
                    JE_Cancel.Memo = Remarks;
                    JE_Cancel.Series = Configurations.GEt_Revenue_Realization_JE_Series(company);
                    JE.UserFields.Fields.Item("U_ST_TYPE").Value = "CR";

                    double Orig_Premium = 0, Orig_Allowance = 0, Orig_Revenue = 0;
                    string Orig_Premium_Account = "", Orig_Allowance_Account = "", Orig_Revenue_Account = "";
                    for (int J = 0; J < JE.Lines.Count; J++)
                    {
                        JE.Lines.SetCurrentLine(J);

                        if (JE.Lines.Debit != 0)
                        {
                            if (Currency != "")
                            {
                                Orig_Premium = JE.Lines.FCDebit;
                            }
                            else
                            {
                                Orig_Premium = JE.Lines.Debit;
                            }
                            Orig_Premium_Account = JE.Lines.ShortName;
                        }
                        else
                        {
                            if (Currency != "")
                            {
                                if (JE.Lines.ShortName == Allowance_Account)
                                {
                                    Orig_Allowance = JE.Lines.FCCredit;
                                    Orig_Allowance_Account = JE.Lines.ShortName;
                                }
                                else
                                {
                                    Orig_Revenue = JE.Lines.FCCredit;
                                    Orig_Revenue_Account = JE.Lines.ShortName;
                                }
                            }
                            else
                            {
                                if (JE.Lines.ShortName == Allowance_Account)
                                {
                                    Orig_Allowance = JE.Lines.Credit;
                                    Orig_Allowance_Account = JE.Lines.ShortName;
                                }
                                else
                                {
                                    Orig_Revenue = JE.Lines.Credit;
                                    Orig_Revenue_Account = JE.Lines.ShortName;
                                }
                            }
                        }
                    }

                    double Rate = Orig_Allowance / Orig_Premium;
                    double Cancel_Premium = (double)RC.Fields.Item("LineTotal").Value;

                    if (Currency != "")
                    {
                        JE_Cancel.Lines.ShortName = Orig_Premium_Account;
                        JE_Cancel.Lines.FCCredit = Cancel_Premium;
                        JE_Cancel.Lines.FCCurrency = Currency;
                        JE_Cancel.Lines.Add();


                        JE_Cancel.Lines.ShortName = Orig_Allowance_Account;
                        JE_Cancel.Lines.FCDebit = Cancel_Premium * Rate;
                        JE_Cancel.Lines.FCCurrency = Currency;
                        JE_Cancel.Lines.Add();

                        JE_Cancel.Lines.ShortName = Orig_Revenue_Account;
                        JE_Cancel.Lines.FCDebit = Cancel_Premium - (Cancel_Premium * Rate);
                        JE_Cancel.Lines.FCCurrency = Currency;
                        JE_Cancel.Lines.Add();

                    }
                    else
                    {
                        JE_Cancel.Lines.ShortName = Orig_Premium_Account;
                        JE_Cancel.Lines.Credit = Cancel_Premium;
                        JE_Cancel.Lines.Add();


                        JE_Cancel.Lines.ShortName = Orig_Allowance_Account;
                        JE_Cancel.Lines.Debit = Cancel_Premium * Rate;
                        JE_Cancel.Lines.Add();

                        JE_Cancel.Lines.ShortName = Orig_Revenue_Account;
                        JE_Cancel.Lines.Debit = Cancel_Premium - (Cancel_Premium * Rate);
                        JE_Cancel.Lines.Add();
                    }
                    Orig_Premium_Account = JE.Lines.ShortName;

                    if (JE_Cancel.Add() != 0)
                    {
                        throw new Logic.Custom_Exception($"Error during create the cancellation Journal Entry[{company.GetLastErrorDescription()}]");
                    }
                    else
                    {
                        string New_ID;
                        company.GetNewObjectCode(out New_ID);
                        //Documents Doc_CN = (Documents)company.GetBusinessObject(BoObjectTypes.oCreditNotes);
                        //Doc_CN.GetByKey(DocEntry);

                        //for (int J = 0; J < Doc_CN.Lines.Count; J++)
                        //{
                        //    Doc_CN.Lines.SetCurrentLine(J);
                        //    if (Doc_CN.Lines.LineNum == (int)RC.Fields.Item("LineNum").Value)
                        //    {
                        //        Doc_CN.Lines.UserFields.Fields.Item("U_ST_CANCEL_REVENUE_REALIZATION_JE").Value = int.Parse(New_ID);
                        //        break;
                        //    }
                        //}
                        //if (Doc_CN.Update() != 0)
                        //{
                        //    throw new Exception($"Error during update the Credit Note[{company.GetLastErrorDescription()}]");
                        //}

                        string SQL_Update = $@"Update RIN1 SET U_ST_CANCEL_REVENUE_REALIZATION_JE = {New_ID} WHERE ""DocEntry"" = {CM_DocEntry} AND ""LineNum"" = {RC.Fields.Item("LineNum").Value}";
                        Recordset RC_Update = Helper.Utility.Execute_Recordset_Query(company, SQL_Update);

                    }

                    RC.MoveNext();
                }
                //company.EndTransaction(BoWfTransOpt.wf_Commit);
            }
            catch (Exception ex)
            {
                //try
                //{
                //    company.EndTransaction(BoWfTransOpt.wf_RollBack);
                //}
                //catch (Exception)
                //{ }
                throw new Logic.Custom_Exception($"Error during create the Revenue Realization Cancellation[{ex.Message}]");
            }
        }

        internal static void Unlink(Company company, string Sub_UDO_Code, string Sub_BP_CardCode, UDO_Definition Sub_UDO_Info, bool With_DB_Transaction = true)
        {
            int Default_Cust_Group;
            if (Sub_UDO_Info.UDO_Modules == KHCF_Modules.CCI)
            {
            Default_Cust_Group= Configurations.Get_Default_CCI_Individual_Customer_Group(company, true);
            }
            else if (Sub_UDO_Info.UDO_Modules == KHCF_Modules.Fundraising)
            {
                Default_Cust_Group = Configurations.Get_Default_Fundraising_Individual_Customer_Group(company, true);
            }
            else
            {
                throw new Logic.Custom_Exception($"The UDO [{Sub_UDO_Info.KHCF_Object}] is not supported to Unlink ");
            }
            Field_Data Fld_Parent_ID = new Field_Data() { Field_Name = "U_ST_PARENT_ID", Value = "" };
            Field_Data Fld_Parent_Name = new Field_Data() { Field_Name = "U_ST_PARENT_NAME", Value = string.Empty };
            Field_Data Fld_Parent_Type = new Field_Data() { Field_Name = "U_ST_PARENT_TYPE", Value = "N" };
            Field_Data Fld_Customer_Group = new Field_Data() { Field_Name = "U_ST_CUSTOMER_GROUP", Value = Default_Cust_Group.ToString() };
            try
            {
                if (With_DB_Transaction == true)
                {
                    company.StartTransaction();
                }
                Utility.Update_UDO(company, Sub_UDO_Info, Sub_UDO_Code, new Field_Data[] { Fld_Parent_ID, Fld_Parent_Type, Fld_Parent_Name, Fld_Customer_Group });

                if (Sub_BP_CardCode != "")
                {
                    BusinessPartners BP = (BusinessPartners)company.GetBusinessObject(BoObjectTypes.oBusinessPartners);
                    BP.GetByKey(Sub_BP_CardCode);

                    BP.FatherCard = "";
                    BP.GroupCode = Default_Cust_Group;
                    if (BP.Update() != 0)
                    {
                        throw new Logic.Custom_Exception($"Error during update the BP[{Sub_BP_CardCode}][{company.GetLastErrorDescription()}]");
                    }
                }
                if (With_DB_Transaction == true)
                {
                    company.EndTransaction(BoWfTransOpt.wf_Commit);
                }
            }
            catch (Exception ex)
            {
                if (With_DB_Transaction == true)
                {
                    try
                    {
                        company.EndTransaction(BoWfTransOpt.wf_RollBack);
                    }
                    catch (Exception)
                    {

                    }
                }
                throw new Logic.Custom_Exception($"Error during Unlink the Card[{Sub_UDO_Code}][{ex.Message}]");
            }
        }

        internal static void Create_KFCC_Invoice_JE(Company company, int AP_Invoice_DocEntry, int AP_Invoice_DocNum, string Patient_Code)
        {
            Patient_Type Type = Get_Patient_Type(company, Patient_Code);

            string Another_Account = "";

            Documents AP_Doc = (Documents)company.GetBusinessObject(BoObjectTypes.oPurchaseInvoices);
            AP_Doc.GetByKey(AP_Invoice_DocEntry);

            switch (Type)
            {
                case Patient_Type.CCI:
                    Another_Account = Configurations.Get_Treatment_Revenue_Account(company);
                    break;
                case Patient_Type.Goodwill:
                    string Tarnsaction_Code = AP_Doc.UserFields.Fields.Item("U_ST_COVERAGE_TRANSACTION_CODE").Value.ToString();
                    if (Tarnsaction_Code == "")
                    {
                        throw new Logic.Custom_Exception("The Coverage Transaction is missing in the AP Invoice");
                    }
                    string SQL_Fund_Account = $@"SELECT T2.""U_ST_REVENUE_ACCOUNT_CODE"" 
FROM ""@ST_COVERAGE_TRANS""  T0 INNER JOIN ""@ST_GOOD_WILL_FUNDS""  T1 ON T0.U_ST_FUND_BOX = T1.""Code"" 
INNER JOIN ""@ST_GODWIL_FUNDS_TYP"" T2 ON T1.U_ST_TYPE = T2.""Code"" 
WHERE T0.""Code"" = '{Tarnsaction_Code}'";
                    Recordset RC_Fund_Account = Helper.Utility.Execute_Recordset_Query(company, SQL_Fund_Account);
                    Another_Account = RC_Fund_Account.Fields.Item("U_ST_REVENUE_ACCOUNT_CODE").Value.ToString();
                    if (Another_Account == "")
                    {
                        throw new Logic.Custom_Exception("We can't find the Revenue Account in the table[@ST_GODWIL_FUNDS_TYP]");
                    }
                    break;
                case Patient_Type.Other_CCI:
                    Another_Account = Configurations.Get_Other_CCI_Patient_Clearing_Account(company);
                    break;
                default:
                    break;
            }

            string SQL_Patient = $@"SELECT T0.U_ST_BP_CODE FROM ""@ST_PATIENTS_CARD""  T0 WHERE T0.""Code"" = '{Patient_Code}'";
            Recordset RC_Patient = Helper.Utility.Execute_Recordset_Query(company, SQL_Patient);
            string Patient_BP_CardCode = RC_Patient.Fields.Item("U_ST_BP_CODE").Value.ToString();
            double Amount = AP_Doc.DocTotal;
            try
            {
                company.StartTransaction();
                JournalEntries JE = (JournalEntries)company.GetBusinessObject(BoObjectTypes.oJournalEntries);

                string Remark = $@"Created for KFCC AP Invoice[{AP_Doc.DocNum}]";
                JE.ReferenceDate = JE.DueDate = JE.VatDate = AP_Doc.DocDate;
                JE.Memo = Remark;

                JE.Lines.ShortName = Patient_BP_CardCode;
                JE.Lines.Debit = Amount;
                JE.Lines.LineMemo = Remark;
                JE.Lines.Add();

                JE.Lines.ShortName = Another_Account;
                JE.Lines.Credit = Amount;
                JE.Lines.LineMemo = Remark;
                JE.Lines.Add();

                if (JE.Add() != 0)
                {
                    throw new Logic.Custom_Exception($"Error during add the Journal Entry[{company.GetLastErrorDescription()}]");
                }
                string New_ID;
                company.GetNewObjectCode(out New_ID);
                AP_Doc.UserFields.Fields.Item("U_ST_TARGET_JE_ID").Value = int.Parse(New_ID);
                if (AP_Doc.Update() != 0)
                {
                    throw new Logic.Custom_Exception($"Error during Update the AP Invoice[{company.GetLastErrorDescription()}]");
                }
                company.EndTransaction(BoWfTransOpt.wf_Commit);
            }
            catch (Exception)
            {
                try
                {
                    company.EndTransaction(BoWfTransOpt.wf_RollBack);
                }
                catch (Exception) { }
                throw;
            }

        }

        private static Patient_Type Get_Patient_Type(Company company, string Patient_Code)
        {
            string SQL = $@"SELECT T0.U_ST_BP_CODE, T0.U_ST_MEMBER_CARD, T0.U_ST_PATIENT_VENDOR_GROUP_GW, T0.U_ST_PATIENT_VENDOR_GROUP_CCI 
FROM ""@ST_PATIENTS_CARD""  T0 WHERE T0.""Code"" ='{Patient_Code}'";
            Recordset RC = Helper.Utility.Execute_Recordset_Query(company, SQL);
            if (RC.Fields.Item("U_ST_MEMBER_CARD").Value.ToString() != "")
            {
                return Patient_Type.CCI;
            }
            else if (RC.Fields.Item("U_ST_PATIENT_VENDOR_GROUP_GW").Value.ToString() != "" && RC.Fields.Item("U_ST_PATIENT_VENDOR_GROUP_GW").Value.ToString() != "-")
            {
                return Patient_Type.Goodwill;
            }
            else if (RC.Fields.Item("U_ST_PATIENT_VENDOR_GROUP_CCI").Value.ToString() != "" && RC.Fields.Item("U_ST_PATIENT_VENDOR_GROUP_CCI").Value.ToString() != "-")
            {
                return Patient_Type.Other_CCI;
            }
            else
            {
                throw new Logic.Custom_Exception($"The Patient type is unknown for the Patient[{Patient_Code}]");
            }

        }

        internal static void Stop_Coverage_Request(Company company, string Coverage_Request_Code, string Stop_Reason, DateTime Stop_Date)
        {

            string SQL_AP_Invoices = $@"SELECT T0.""DocEntry"" 
FROM OPCH T0 INNER JOIN""@ST_COVERAGE_TRANS""  T1 ON T0.U_ST_COVERAGE_TRANSACTION_CODE = T1.""Code"" 
WHERE T1.""U_ST_COVERAGE_REQUEST_CODE"" = '{Coverage_Request_Code}' AND T0.""DocStatus"" = 'O'";
            Recordset RC_AP_Invoice = Helper.Utility.Execute_Recordset_Query(company, SQL_AP_Invoices);

            try
            {
                company.StartTransaction();
                for (int i = 0; i < RC_AP_Invoice.RecordCount; i++)
                {
                    int AP_DocEntry = (int)RC_AP_Invoice.Fields.Item("DocEntry").Value;
                    Documents AP_Doc = (Documents)company.GetBusinessObject(BoObjectTypes.oPurchaseInvoices);
                    AP_Doc.GetByKey(AP_DocEntry);
                    Documents Cancel_Doc = AP_Doc.CreateCancellationDocument();
                    if (Cancel_Doc.Add() != 0)
                    {
                        throw new Logic.Custom_Exception($"Error during cancel the AP Invoice[{AP_Doc.DocNum}][{company.GetLastErrorDescription()}]");
                    }

                    RC_AP_Invoice.MoveNext();
                }

                Field_Data Fld_Status = new Field_Data() { Field_Name = "U_ST_STATUS", Value = "S" };
                Field_Data Fld_Date = new Field_Data() { Field_Name = "U_ST_STOP_DATE", Value = Stop_Date };
                Field_Data Fld_Reason = new Field_Data() { Field_Name = "U_ST_STATUS", Value = Stop_Reason };
                UDO_Definition UDO_Info = Objects_Logic.All_UDO_Definition.FirstOrDefault(U => U.KHCF_Object == KHCF_Objects.Coverage_Request);
                Utility.Update_UDO(company, UDO_Info, Coverage_Request_Code, new Field_Data[] { Fld_Status, Fld_Date, Fld_Reason });

                company.EndTransaction(BoWfTransOpt.wf_Commit);
            }
            catch (Exception)
            {
                company.EndTransaction(BoWfTransOpt.wf_RollBack);
            }

        }

        internal static void Stop_Coverage_Transaction(Company company, string Coverage_Transaction_Code, string Stop_Reason, DateTime Stop_Date, SAPbouiCOM.Application SBO_Application)
        {

            string SQL_AP_Invoices = $@"SELECT T0.""DocEntry"" 
FROM OPCH T0 
WHERE T0.""U_ST_COVERAGE_TRANSACTION_CODE"" = '{Coverage_Transaction_Code}' AND T0.""DocStatus"" = 'O'";
            Recordset RC_AP_Invoice = Helper.Utility.Execute_Recordset_Query(company, SQL_AP_Invoices);

            try
            {
                company.StartTransaction();
                for (int i = 0; i < RC_AP_Invoice.RecordCount; i++)
                {
                    int AP_DocEntry = (int)RC_AP_Invoice.Fields.Item("DocEntry").Value;
                    Documents AP_Doc = (Documents)company.GetBusinessObject(BoObjectTypes.oPurchaseInvoices);
                    AP_Doc.GetByKey(AP_DocEntry);
                    Documents Cancel_Doc = AP_Doc.CreateCancellationDocument();
                    if (Cancel_Doc.Add() != 0)
                    {
                        throw new Logic.Custom_Exception($"Error during cancel the AP Invoice[{AP_Doc.DocNum}][{company.GetLastErrorDescription()}]");
                    }
                    if (SBO_Application != null)
                    {
                        SBO_Application.StatusBar.SetText($"The AP Invoice[{AP_Doc.DocNum}] has been canceled", SAPbouiCOM.BoMessageTime.bmt_Medium, SAPbouiCOM.BoStatusBarMessageType.smt_Success);
                    }

                    RC_AP_Invoice.MoveNext();
                }

                //Field_Data Fld_Status = new Field_Data() { Field_Name = "U_ST_STATUS", Value = "S" };
                Field_Data Fld_Date = new Field_Data() { Field_Name = "U_ST_STOP_DATE", Value = Stop_Date };
                Field_Data Fld_Reason = new Field_Data() { Field_Name = "U_ST_STOP_REASON", Value = Stop_Reason };
                UDO_Definition UDO_Info = Objects_Logic.All_UDO_Definition.FirstOrDefault(U => U.KHCF_Object == KHCF_Objects.Coverage_Transaction);
                //Utility.Update_UDO(company, UDO_Info, Coverage_Transaction_Code, new Field_Data[] { Fld_Status, Fld_Date, Fld_Reason });
                Utility.Update_UDO(company, UDO_Info, Coverage_Transaction_Code, new Field_Data[] { Fld_Date, Fld_Reason });

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
                throw new Logic.Custom_Exception($@"Error during stop the Coverage Transaction[{Coverage_Transaction_Code}],[{ex.Message}]");
            }

        }

        internal static string[] Get_Children_MemberCard_Codes(Company company, string Parent_MemberCard_Code, UDO_Definition UDO_Info)
        {
            string Parent_Type;
            if (UDO_Info.KHCF_Object == KHCF_Objects.CCI_Member_Card)
            {
                Parent_Type = "I";
            }
            else if (UDO_Info.KHCF_Object == KHCF_Objects.CCI_Corporate_Member_Card)
            {
                Parent_Type = "C";
            }
            else
            {
                throw new Exception($"The type[{UDO_Info.KHCF_Object}] not supported for hierarchy structure.");
            }

            string SQL = $@"SELECT T0.""Code"" FROM ""@ST_CCI_INDIV_CARD""  T0 WHERE T0.U_ST_PARENT_TYPE = '{Parent_Type}' AND  T0.""U_ST_PARENT_ID"" ='{Parent_MemberCard_Code}'";
            Recordset RC = Helper.Utility.Execute_Recordset_Query(company, SQL);

            List<string> children = new List<string>();
            for (int i = 0; i < RC.RecordCount; i++)
            {
                children.Add(RC.Fields.Item("Code").Value.ToString());

                RC.MoveNext();
            }

            return children.ToArray();
        }
        internal static string[] Get_Children_Membership_Codes(Company company, string Parent_Membership_Code, UDO_Definition UDO_Info)
        {
            string Parent_Type;
            if (UDO_Info.KHCF_Object == KHCF_Objects.Individual_Membership)
            {
                Parent_Type = "I";
            }
            else if (UDO_Info.KHCF_Object == KHCF_Objects.Corporate_Membership)
            {
                Parent_Type = "C";
            }
            else
            {
                throw new Exception($"The type[{UDO_Info.KHCF_Object}] not supported for hierarchy structure.");
            }

            string SQL = $@"SELECT T0.""Code"" FROM ""@ST_INDIV_MEMBERSHIP""  T0 
WHERE T0.U_ST_PARENT_MEMBERSHIP_TYPE = '{Parent_Type}' AND  T0.""U_ST_PARENT_MEMBERSHIP_ID"" ='{Parent_Membership_Code}'";
            Recordset RC = Helper.Utility.Execute_Recordset_Query(company, SQL);

            List<string> children = new List<string>();
            for (int i = 0; i < RC.RecordCount; i++)
            {
                children.Add(RC.Fields.Item("Code").Value.ToString());

                RC.MoveNext();
            }

            return children.ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="company"></param>
        /// <param name="MemberCard_Type">[I]for Individual and [C]for Corporate</param>
        /// <param name="v2"></param>
        /// <returns></returns>
        internal static string Get_MemberCard_Name(Company company, string MemberCard_Type, string UDO_Code)
        {
            string Table_Name, Name_Field;

            if (MemberCard_Type == "I")
            {
                Table_Name = "@ST_CCI_INDIV_CARD";
                Name_Field = "U_ST_FULL_NAME_AR";
            }
            else
            {
                Table_Name = "@ST_CCI_CORP_CARD";
                Name_Field = "U_ST_CORPORATE_ARABIC_NAME";
            }
            string SQL = $@"SELECT ""{Name_Field}"" ,""U_ST_CUSTOMER_GROUP"" from ""{Table_Name}"" WHERE ""Code"" = '{UDO_Code}'";
            Recordset RC = Helper.Utility.Execute_Recordset_Query(company, SQL);
            return RC.Fields.Item(Name_Field).Value.ToString();

        }

        internal static void Check_MemberCard_Parent_Logic(Company company, string UDO_Code, UDO_Definition UDO_Info, string Old_Parent_ID, string New_Parent_ID
, string Parent_Type, string Parent_Name, (bool IsExising, string Code, bool Is_Lead, string Lead_CardCode) Existing)
        {

            if (Existing.Is_Lead)
            {
                UDO_Code = Forms.System_Forms.Convert_Lead_To_MemberCard(Existing.Lead_CardCode, false);
            }

            string SQL_Membership = $@"SELECT *  FROM ""@ST_INDIV_MEMBERSHIP""  T0 WHERE T0.""U_ST_MEMBER_CARD""  = '{UDO_Code}'";

            Recordset RC_Membership = Helper.Utility.Execute_Recordset_Query(company, SQL_Membership);
            if (RC_Membership.RecordCount != 0)
            {
                if (Old_Parent_ID != "")
                {
                    if (Old_Parent_ID != New_Parent_ID)
                    {
                        throw new Logic.Custom_Exception($"The Member Card[{UDO_Code}] is already assigned to another parent[{Old_Parent_ID}]");
                    }
                }

                string Membership_Staus = RC_Membership.Fields.Item("U_ST_MEMBERSHIP_STATUS").Value.ToString();
                DateTime Start_Date = (DateTime)RC_Membership.Fields.Item("U_ST_START_DATE").Value;
                DateTime End_Date = (DateTime)RC_Membership.Fields.Item("U_ST_END_DATE").Value;

                if (!(new string[] { "C", "S" }).Contains(Membership_Staus) && DateTime.Today >= Start_Date && DateTime.Today <= End_Date)
                {
                    throw new Logic.Custom_Exception($"The Member Card[{UDO_Code}] already has an active membership, you need to stop it before linking with the Parent Card");
                }

            }


            if (New_Parent_ID != "")
            {
                Link(company, UDO_Code, Parent_Type, New_Parent_ID, Parent_Name, UDO_Info);
            }

        }

        internal static string Get_Control_Account_Per_BPGroup(Company company, int BP_Group)
        {
            string SQL = $@"SELECT T0.""U_ST_GL_ACCOUNT"" FROM OCRG T0 WHERE ""GroupCode"" = {BP_Group}";
            Recordset RC = Helper.Utility.Execute_Recordset_Query(company, SQL);
            return RC.Fields.Item("U_ST_GL_ACCOUNT").Value.ToString();
        }

        internal static void Set_Corporate_Fund_Chosse_From_List_Basic_Condition(SAPbouiCOM.Form form, string CFL_ID)
        {
            SAPbouiCOM.ChooseFromList CFL = form.ChooseFromLists.Item(CFL_ID);
            SAPbouiCOM.Conditions Cons = CFL.GetConditions();
            SAPbouiCOM.Condition Con = Cons.Add();
            Con.Alias = "U_ST_IS_BLACKLISTED";
            Con.CondVal = "Y";
            Con.Operation = SAPbouiCOM.BoConditionOperation.co_NOT_EQUAL;

            CFL.SetConditions(Cons);
        }
        internal static void Set_Individua_Fund_Chosse_From_List_Basic_Condition(SAPbouiCOM.Form form, string CFL_ID)
        {
            SAPbouiCOM.ChooseFromList CFL = form.ChooseFromLists.Item(CFL_ID);
            SAPbouiCOM.Conditions Cons = CFL.GetConditions();
            SAPbouiCOM.Condition Con = Cons.Add();
            Con.Alias = "U_ST_BLACKLISTED_ADD_UPDATE";
            Con.CondVal = "Y";
            Con.Operation = SAPbouiCOM.BoConditionOperation.co_NOT_EQUAL;

            CFL.SetConditions(Cons);
        }

        internal static void Select_Allocation(SAPbouiCOM.ItemEvent pVal, Parent_Form Form_Obj, string Item_ID)
        {
           SAPbouiCOM.Form Source_Form = Loader.SBO_Application.Forms.Item(pVal.FormUID);
            string Source_Field_Name = Helper.Utility.Get_Item_DB_Datasource(Source_Form.Items.Item(Item_ID));
            if (Source_Form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).GetValue(Source_Field_Name, 0) != "")
            {
                Loader.SBO_Application.StatusBar.SetText("The Allocation is already assigned", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                return;
            }
            UDO_Definition Obj_Info = Objects_Logic.All_UDO_Definition.FirstOrDefault(O => O.KHCF_Object == KHCF_Objects.Fund_Target);
            SAPbouiCOM.Form KHCF_UDO_Form = Loader.Open_UDO_Form(Obj_Info.KHCF_Object);
            System.Threading.Thread.Sleep(500);
            Loader.SBO_Application.Menus.Item("1282").Activate();

            KHCF_UDO_Form.DataSources.DBDataSources.Item("@ST_FUND_TARGET").SetValue("U_ST_SOURCE_OBJECT_ID", 0, Form_Obj.UDO_Database_Table_Name);
            KHCF_UDO_Form.DataSources.UserDataSources.Item("FORM_ID").Value = pVal.FormUID;
            KHCF_UDO_Form.DataSources.UserDataSources.Item("ITEM_ID").Value = Item_ID;

            if (Form_Obj.KHCF_Object == KHCF_Objects.Expected_Donations)
            {
                SAPbouiCOM.ChooseFromList CFL = KHCF_UDO_Form.ChooseFromLists.Item("CFL_MACHINES");
                SAPbouiCOM.Conditions Cons = CFL.GetConditions();
                SAPbouiCOM.Condition Con = Cons.Add();
                Con.Alias = "U_ST_MACHINE_STATUS";
                Con.CondVal = "A";
                Con.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL;

                CFL.SetConditions(Cons);
            }
            if (Form_Obj.KHCF_Object == KHCF_Objects.Actual_Donations)
            {
                SAPbouiCOM.ChooseFromList CFL = KHCF_UDO_Form.ChooseFromLists.Item("CFL_MACHINES");
                SAPbouiCOM.Conditions Cons = CFL.GetConditions();
                SAPbouiCOM.Condition Con = Cons.Add();
                Con.Alias = "U_ST_MACHINE_STATUS";
                Con.CondVal = "A";
                Con.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL;
                Con.Relationship = SAPbouiCOM.BoConditionRelationship.cr_OR;
                Con = Cons.Add();
                Con.Alias = "U_ST_MACHINE_STATUS";
                Con.CondVal = "R";
                Con.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL;

                CFL.SetConditions(Cons);
            }

        }
    }
}
