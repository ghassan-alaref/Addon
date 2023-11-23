using SAPbobsCOM;
using SAPbouiCOM;
using ST.KHCF.Customization.Forms;
using ST.KHCF.Customization.Logic.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LinqToDB;
using System.Globalization;

namespace ST.KHCF.Customization.Logic
{
    internal class Member_Cards_UI
    {
        private static Application SBO_Application = Loader.SBO_Application;
        private static SAPbobsCOM.Company company = Loader.company;


        #region Communication Log

        internal static void Add_Communication_Log(ItemEvent pVal, string MemberCardType)
        {
            SAPbouiCOM.Form form = SBO_Application.Forms.Item(pVal.FormUID);
            if (form.Mode == BoFormMode.fm_ADD_MODE || form.Mode == BoFormMode.fm_FIND_MODE)
            {
                throw new Logic.Custom_Exception("Action cannot be completed in ADD or Find modes.");
            }

            UDO_Definition KHCF_Object = Objects_Logic.All_UDO_Definition.FirstOrDefault(O => O.KHCF_Object == KHCF_Objects.Communication_Log);
            SAPbouiCOM.Form KHCF_UDO_Form = Loader.Open_UDO_Form(KHCF_Object.KHCF_Object);
            KHCF_UDO_Form.Mode = BoFormMode.fm_ADD_MODE;
            KHCF_UDO_Form.DataSources.DBDataSources.Item(0).SetValue("U_ST_MEMBER_CARD_CODE", 0, form.DataSources.DBDataSources.Item(0).GetValue("Code", 0));
            KHCF_UDO_Form.DataSources.DBDataSources.Item(0).SetValue("U_ST_NUMBER_CARD_TYPE", 0, MemberCardType);
            KHCF_UDO_Form.DataSources.DBDataSources.Item(0).SetValue("U_ST_AUTOMATIC_DATE", 0, DateTime.Today.ToString("yyyyMMdd"));
            KHCF_UDO_Form.DataSources.DBDataSources.Item(0).SetValue("U_ST_TIME", 0, DateTime.Now.ToString("HH:mm:ss"));

            KHCF_UDO_Form.Items.Item("172").SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, -1, BoModeVisualBehavior.mvb_False);
            KHCF_UDO_Form.Items.Item("24").SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, -1, BoModeVisualBehavior.mvb_False);
            KHCF_UDO_Form.Items.Item("22").SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, -1, BoModeVisualBehavior.mvb_False);
        }

        internal static void Load_Communication_Log(Form form, string MemberCardType, string Card_ID)
        {
            DataTable DT_Communication_Log = form.DataSources.DataTables.Item("Communication_Log");
            DT_Communication_Log.Rows.Clear();
            //            string SQL_Comm_Log = $@"SELECT T0.""Code"", T0.U_ST_AUTOMATIC_DATE, T0.U_ST_MANUAL_DATE, T0.U_ST_TIME, T0.U_ST_REMARKS
            //, T0.U_ST_STATUS, T0.U_ST_SUB_STATUS, T0.U_ST_TYPE, T0.U_ST_SOURCE
            //FROM ""@ST_COMMUNICAT_LOG""  T0 WHERE T0.U_ST_MEMBER_CARD_CODE ='{Card_ID}' AND T0.U_ST_NUMBER_CARD_TYPE = 'C'";
            string SQL_Comm_Log = $@"SELECT T0.""Code"", T0.""U_ST_AUTOMATIC_DATE"", T0.""U_ST_MANUAL_DATE"", T0.""U_ST_TIME"", T0.""U_ST_REMARKS""
, T1.""Name"" as ""Status_Name"", T2.""Name"" as ""Sub_Status_Name"", T3.""Name"" as ""Type_Name"", T4.""Name"" as ""Source_Name"" 
FROM ""@ST_COMMUNICAT_LOG""  T0 
LEFT OUTER JOIN ""@ST_COMM_STATUS""  T1 ON T0.""U_ST_STATUS"" = T1.""Code"" 
LEFT OUTER JOIN ""@ST_COMM_SUB_STATUS""  T2 ON T0.""U_ST_SUB_STATUS"" = T2.""Code"" 
LEFT OUTER JOIN ""@ST_COMM_TYPE""  T3 ON T0.""U_ST_TYPE"" = T3.""Code"" 
LEFT OUTER JOIN ""@ST_COMM_SOURCE""  T4 ON T0.""U_ST_SOURCE"" = T4.""Code""
WHERE T0.U_ST_MEMBER_CARD_CODE ='{Card_ID}' AND T0.U_ST_NUMBER_CARD_TYPE = '{MemberCardType}'";
            Recordset RC_Comm_Log = Helper.Utility.Execute_Recordset_Query(company, SQL_Comm_Log);
            DT_Communication_Log.Rows.Add(RC_Comm_Log.RecordCount);

            for (int i = 0; i < RC_Comm_Log.RecordCount; i++)
            {
                for (int J = 0; J < DT_Communication_Log.Columns.Count; J++)
                {
                    string Col_Name = DT_Communication_Log.Columns.Item(J).Name;
                    string UDF_Name;

                    UDF_Name = Col_Name;
                    if (!UDF_Name.ToLower().Contains("select"))
                        DT_Communication_Log.SetValue(Col_Name, i, RC_Comm_Log.Fields.Item(UDF_Name).Value);
                }
                RC_Comm_Log.MoveNext();
            }
            Grid Grd_Membership = (Grid)form.Items.Item("621").Specific;
            Grd_Membership.AutoResizeColumns();

        }

        #endregion

        #region Addresses

        internal static void Remove_Address_Row(ItemEvent pVal)
        {
            SAPbouiCOM.Form form = SBO_Application.Forms.Item(pVal.FormUID);
            Matrix Mat = (Matrix)form.Items.Item("20").Specific;
            int Count = Mat.RowCount;
            for (int i = Count; i > 0; i--)
            {
                SAPbouiCOM.CheckBox Chk_Selected = (SAPbouiCOM.CheckBox)Mat.Columns.Item("SELECTED").Cells.Item(i).Specific;
                if (Chk_Selected.Checked)
                {
                    Mat.DeleteRow(i);
                    Mat.FlushToDataSource();
                    Mat.LoadFromDataSource();
                }
            }
        }

        internal static void Add_Address_Row(ItemEvent pVal, string MemberCardType)
        {
            SAPbouiCOM.Form form = SBO_Application.Forms.Item(pVal.FormUID);
            try
            {
                form.Freeze(true);
                DBDataSource DS_Address = form.DataSources.DBDataSources.Item(MemberCardType == "I" ? "@ST_CCI_INDIV_ADDR" : "@ST_CCI_CORP_ADDR");
                Matrix Mat_Add = (Matrix)form.Items.Item("20").Specific;
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

                DS_Address.SetValue("U_ST_ADDRESS_TYPE", Count, "B");
                if (MemberCardType == "I")
                {
                    string Residancy = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_RESIDENCY", 0);
                    if (!string.IsNullOrEmpty(Residancy))
                        DS_Address.SetValue("U_ST_COUNTRY", Count, Residancy);
                }

                Mat_Add.LoadFromDataSource();
                Mat_Add.AutoResizeColumns();
            }
            finally
            {
                form.Freeze(false);
            }

        }

        internal static void Check_Address(SAPbouiCOM.BusinessObjectInfo businessObjectInfo, string Addres_UDO_Table)
        {
            SAPbouiCOM.Form form = Loader.SBO_Application.Forms.Item(businessObjectInfo.FormUID);
            SAPbouiCOM.DBDataSource DS_Address = form.DataSources.DBDataSources.Item(Addres_UDO_Table);

            int Count = DS_Address.Size;
            SAPbouiCOM.Matrix Mat_Add = (SAPbouiCOM.Matrix)form.Items.Item("20").Specific;
            if (DS_Address.Size == 1 && string.IsNullOrEmpty(DS_Address.GetValue("U_ST_ADDRESS_NAME", Count - 1)))
            {
                throw new Custom_Exception($"Please add at least one address at the addresses tab.");
            }

            string[] Mandatory_Columns = new string[] { "Country", "City" };
            string r = DS_Address.GetValue("U_ST_ADDRESS_TYPE", Count - 1);

            for (int i = 0; i < Count; i++)
            {
                //SAPbouiCOM.ComboBox City = (SAPbouiCOM.ComboBox)Mat_Add.Columns.Item("City").Cells.Item(i + 1).Specific;
                SAPbouiCOM.EditText City = (SAPbouiCOM.EditText)Mat_Add.Columns.Item("City").Cells.Item(i + 1).Specific;
                SAPbouiCOM.EditText Country = (SAPbouiCOM.EditText)Mat_Add.Columns.Item("Country").Cells.Item(i + 1).Specific;
                string Country_Value = Country.Value;
                string City_Value = "hh";
                if (City != null)
                    City_Value = City.Value;
                if (string.IsNullOrEmpty(Country_Value) || string.IsNullOrEmpty(City_Value))
                {
                    throw new Custom_Exception($"Please Fill the City And Country at the Addresses tab at row [{i + 1}]");
                }
            }

        }

        #endregion

        #region Sub Members

        internal static void Remove_Member_Row(SAPbouiCOM.ItemEvent pVal)
        {
            SAPbouiCOM.Form form = Loader.SBO_Application.Forms.Item(pVal.FormUID);
            SAPbouiCOM.DataTable DT_Members = form.DataSources.DataTables.Item("MEMBERS");

            for (int i = 0; i < DT_Members.Rows.Count; i++)
            {
                if (DT_Members.GetValue("SELECTED", i).ToString() != "Y")
                {
                    continue;
                }
                string Sub_Code = DT_Members.GetValue("Code", i).ToString();
                if (Sub_Code != "")
                {
                    if (form.Mode != SAPbouiCOM.BoFormMode.fm_OK_MODE)
                    {
                        throw new Logic.Custom_Exception("We can unlink the member if the form in the OK mode only");
                    }
                    UDO_Definition UDO_Info = Logic.Objects_Logic.All_UDO_Definition.FirstOrDefault(U => U.KHCF_Object == KHCF_Objects.CCI_Member_Card);
                    string BP_Sub_Card = Utility.Get_BP_Code(Loader.company, Sub_Code, UDO_Info);
                    KHCF_Logic_Utility.Unlink(Loader.company, Sub_Code, BP_Sub_Card, UDO_Info);
                    Loader.SBO_Application.StatusBar.SetText($"The Card[{Sub_Code}] has been unlinked successfully!", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Success);
                }
                DT_Members.Rows.Remove(i);
            }
            //   SBO_Application.Menus.Item("1304").Activate();
        }
        internal static void Check_Fundraising_Members(SAPbouiCOM.BusinessObjectInfo businessObjectInfo, string MemberCardType)
        {
            SAPbouiCOM.Form form = Loader.SBO_Application.Forms.Item(businessObjectInfo.FormUID);

            SAPbouiCOM.DataTable DT_Members = form.DataSources.DataTables.Item("MEMBERS");
            string[] Mandatory_Columns = new string[] { "ST_RELATIONSHIP_TO_FATHER", "ST_MOBILE_1", "ST_NATIONALITY", "ST_CUSTOMER_GROUP", "ST_GENDER", "ST_TITLE", "ST_REGION" };
            if (DT_Members.Rows.Count > 0)
            {
                for (int i = 0; i < DT_Members.Rows.Count; i++)
                {
                    if (DT_Members.GetValue("ST_CUSTOMER_GROUP", i) == null)
                        DT_Members.Rows.Remove(i);
                    else if (string.IsNullOrEmpty(DT_Members.GetValue("ST_CUSTOMER_GROUP", i).ToString()))
                        DT_Members.Rows.Remove(i);
                }
            }
            for (int i = 0; i < DT_Members.Rows.Count; i++)
            {
                //string nationality = DT_Members.GetValue("ST_NATIONALITY", i) == null ? string.Empty : DT_Members.GetValue("ST_NATIONALITY", i).ToString();
                // string nationality = "";
                string Phone = DT_Members.GetValue("ST_MOBILE_1", i).ToString();

                string FirstNameAR = DT_Members.GetValue("ST_FIRST_NAME_AR", i).ToString();
                string FatherNameAR = DT_Members.GetValue("ST_FATHER_NAME_AR", i).ToString();
                //string MiddleNameAR = DT_Members.GetValue("ST_MIDDLE_NAME_AR", i).ToString();
                string SurNameAR = DT_Members.GetValue("ST_SURNAME_AR", i).ToString();

                string FirstNameEN = DT_Members.GetValue("ST_FIRST_NAME_EN", i).ToString();
                //string FatherNameEN = DT_Members.GetValue("ST_FATHER_NAME_EN", i).ToString();
                //string MiddleNameEN = DT_Members.GetValue("ST_MIDDLE_NAME_EN", i).ToString();
                string SurNameEN = DT_Members.GetValue("ST_SURNAME_EN", i).ToString();


                if (string.IsNullOrEmpty(FirstNameAR) || string.IsNullOrEmpty(FatherNameAR) || string.IsNullOrEmpty(SurNameAR))
                {
                    throw new Custom_Exception($@"Please fill ""All Name's syllables"" in line [{i + 1}] ");
                }


                if (string.IsNullOrEmpty(FirstNameEN) || string.IsNullOrEmpty(SurNameEN))
                {
                    throw new Custom_Exception($@" Please fill ""First Name and Last Name"" in line {i + 1}");
                }
                Grid Grd_Members = (Grid)form.Items.Item("138").Specific;
                for (int J = 0; J < DT_Members.Columns.Count; J++)
                {
                    string Col_Name = DT_Members.Columns.Item(J).Name;

                    if (Mandatory_Columns.Contains(Col_Name))
                    {
                        object obj = DT_Members.GetValue(Col_Name, i);
                        string objString = string.Empty;
                        if (obj != null)
                            objString = obj.ToString();

                        if (string.IsNullOrEmpty(objString))
                        {
                            TextInfo cultInfo = new CultureInfo("en-US", false).TextInfo;
                            //Col_Name = Col_Name.Substring(3).Replace("_", " ");
                            //string output = cultInfo.ToTitleCase(Col_Name);
                            string output = Grd_Members.Columns.Item(Col_Name).TitleObject.Caption;
                            throw new Custom_Exception($" Please fill [{output}] in line {i + 1} in Relations Contacts tab");
                        }
                    }
                }
                if (string.IsNullOrEmpty(Phone) || Phone.Substring(0, 5) != "00962" || Phone.Length != 14)
                {
                    string p = Phone.Substring(0, 4);
                    int ph = Phone.Length;
                    throw new Custom_Exception($@"Mobile Number must start with 00962 and with 9 length in line [{i + 1}] ");
                }
            }


            /* if (MemberCardType == "C")
             {
                 for (int i = 0; i < DT_Members.Rows.Count; i++)
                 {
                     string Residency = DT_Members.GetValue("ST_RESIDENCY", i) == null ? string.Empty : DT_Members.GetValue("ST_RESIDENCY", i).ToString();
                     string Mobile = DT_Members.GetValue("ST_MOBILE", i) == null ? string.Empty : DT_Members.GetValue("ST_MOBILE", i).ToString();
                     string Title = DT_Members.GetValue("ST_TITLE", i) == null ? string.Empty : DT_Members.GetValue("ST_TITLE", i).ToString();
                     string Prefix = DT_Members.GetValue("ST_MOBILE", i) == null ? string.Empty : DT_Members.GetValue("ST_PREFIX", i).ToString();

                     if (string.IsNullOrEmpty(Residency))
                     {
                         throw new Custom_Exception($@"Please fill ""Residency"" in line {i + 1}");
                     }
                     if (string.IsNullOrEmpty(Mobile))
                     {
                         throw new Custom_Exception($@"Please fill ""Mobile"" in line {i + 1}");
                     }
                     if (string.IsNullOrEmpty(Mobile))
                     {
                         throw new Custom_Exception($@"Please fill ""Title"" in line {i + 1}");
                     }
                     if (string.IsNullOrEmpty(Mobile))
                     {
                         throw new Custom_Exception($@"Please fill ""Prefix"" in line {i + 1}");
                     }
                 }
             }*/


        }

        internal static void Check_Members(SAPbouiCOM.BusinessObjectInfo businessObjectInfo, string MemberCardType)
        {
            SAPbouiCOM.Form form = Loader.SBO_Application.Forms.Item(businessObjectInfo.FormUID);

            SAPbouiCOM.DataTable DT_Members = form.DataSources.DataTables.Item("MEMBERS");
            string[] Mandatory_Columns = new string[] { "ST_NATIONALITY", "ST_GENDER", "ST_DATE_OF_BIRTH", "ST_CUSTOMER_GROUP" };
            if (DT_Members.Rows.Count > 0)
            {
                for (int i = 0; i < DT_Members.Rows.Count; i++)
                {
                    if (DT_Members.GetValue("ST_CUSTOMER_GROUP", i) == null)
                        DT_Members.Rows.Remove(i);
                    else if (string.IsNullOrEmpty(DT_Members.GetValue("ST_CUSTOMER_GROUP", i).ToString()))
                        DT_Members.Rows.Remove(i);
                }
            }
            for (int i = 0; i < DT_Members.Rows.Count; i++)
            {
                string national_id = DT_Members.GetValue("ST_NATIONAL_ID", i) == null ? string.Empty : DT_Members.GetValue("ST_NATIONAL_ID", i).ToString();
                string nationality = DT_Members.GetValue("ST_NATIONALITY", i) == null ? string.Empty : DT_Members.GetValue("ST_NATIONALITY", i).ToString();
                string Personal_ID = DT_Members.GetValue("ST_PERSONAL_ID", i) == null ? string.Empty : DT_Members.GetValue("ST_PERSONAL_ID", i).ToString();
                string Passport_ID = DT_Members.GetValue("ST_PASSPORT_ID", i) == null ? string.Empty : DT_Members.GetValue("ST_PASSPORT_ID", i).ToString();

                string FirstNameAR = DT_Members.GetValue("ST_FIRST_NAME_AR", i).ToString();
                string FatherNameAR = DT_Members.GetValue("ST_FATHER_NAME_AR", i).ToString();
                string MiddleNameAR = DT_Members.GetValue("ST_MIDDLE_NAME_AR", i).ToString();
                string SurNameAR = DT_Members.GetValue("ST_SURNAME_AR", i).ToString();

                string FirstNameEN = DT_Members.GetValue("ST_FIRST_NAME_EN", i).ToString();
                string FatherNameEN = DT_Members.GetValue("ST_FATHER_NAME_EN", i).ToString();
                string MiddleNameEN = DT_Members.GetValue("ST_MIDDLE_NAME_EN", i).ToString();
                string SurNameEN = DT_Members.GetValue("ST_SURNAME_EN", i).ToString();

                if (nationality == "JO" || nationality == "Jordan")
                {
                    if (string.IsNullOrEmpty(national_id))
                        throw new Logic.Custom_Exception($@"Please fill ""National ID"" in line {i + 1}");

                    else if (national_id.ToString().Length != 10)
                        throw new Custom_Exception($"National ID must be 10 digits in line {i + 1}");
                    if (string.IsNullOrEmpty(FirstNameAR) || string.IsNullOrEmpty(FatherNameAR) || string.IsNullOrEmpty(MiddleNameAR) || string.IsNullOrEmpty(SurNameAR))
                    {
                        throw new Custom_Exception($@"Please fill ""All Name's syllables"" in line [{i + 1}] ");
                    }
                }
                else if (nationality != "JO" && nationality != "Jordan")
                {
                    if (string.IsNullOrEmpty(Personal_ID) && string.IsNullOrEmpty(Passport_ID))
                        throw new Custom_Exception($"Please fill Passport ID or Personal ID for non-Jordanian in line {i + 1}");
                    if (string.IsNullOrEmpty(FirstNameAR) || string.IsNullOrEmpty(SurNameAR))
                    {
                        throw new Custom_Exception($@" Please fill ""First Name and Last Name"" in line {i + 1}");
                    }
                }
                for (int J = 0; J < DT_Members.Columns.Count; J++)
                {
                    string Col_Name = DT_Members.Columns.Item(J).Name;
                    if (Col_Name != "ST_DATE_OF_BIRTH")
                    {
                        if (Mandatory_Columns.Contains(Col_Name) && string.IsNullOrEmpty(DT_Members.GetValue(Col_Name, i).ToString()))
                        {
                            string Title = "";
                            if (Col_Name == "ST_GENDER")
                                Title = "Gender";
                            else if (Col_Name == "ST_CUSTOMER_GROUP")
                                Title = "Customer Group";
                            throw new Custom_Exception($" Please fill {Title} in line {i + 1}");
                        }
                    }
                    else if (Col_Name == "ST_DATE_OF_BIRTH" && DT_Members.GetValue(Col_Name, i) == null)
                    {
                        throw new Custom_Exception($@"Please fill ""Date of Birth"" in line {i + 1}");
                    }
                }
            }


            if (MemberCardType == "C")
            {
                for (int i = 0; i < DT_Members.Rows.Count; i++)
                {
                    string Residency = DT_Members.GetValue("ST_RESIDENCY", i) == null ? string.Empty : DT_Members.GetValue("ST_RESIDENCY", i).ToString();
                    string Mobile = DT_Members.GetValue("ST_MOBILE", i) == null ? string.Empty : DT_Members.GetValue("ST_MOBILE", i).ToString();
                    string Title = DT_Members.GetValue("ST_TITLE", i) == null ? string.Empty : DT_Members.GetValue("ST_TITLE", i).ToString();
                    string Prefix = DT_Members.GetValue("ST_MOBILE", i) == null ? string.Empty : DT_Members.GetValue("ST_PREFIX", i).ToString();

                    if (string.IsNullOrEmpty(Residency))
                    {
                        throw new Custom_Exception($@"Please fill ""Residency"" in line {i + 1}");
                    }
                    if (string.IsNullOrEmpty(Mobile))
                    {
                        throw new Custom_Exception($@"Please fill ""Mobile"" in line {i + 1}");
                    }
                    if (string.IsNullOrEmpty(Mobile))
                    {
                        throw new Custom_Exception($@"Please fill ""Title"" in line {i + 1}");
                    }
                    if (string.IsNullOrEmpty(Mobile))
                    {
                        throw new Custom_Exception($@"Please fill ""Prefix"" in line {i + 1}");
                    }
                }
            }
        }

        internal static void Add_Member_Row(SAPbouiCOM.ItemEvent pVal)
        {
            SAPbouiCOM.Form form = Loader.SBO_Application.Forms.Item(pVal.FormUID);
            SAPbouiCOM.DataTable DT_Members = form.DataSources.DataTables.Item("MEMBERS");

            for (int i = 0; i < DT_Members.Rows.Count; i++)
            {
                if (string.IsNullOrEmpty(DT_Members.GetValue("ST_CUSTOMER_GROUP", i).ToString()))
                {
                    DT_Members.Rows.Remove(i);
                }
            }

            int Count = DT_Members.Rows.Count;
            DT_Members.Rows.Add();
            //DT_Members.SetValue("ST_GENDER", DT_Members.Rows.Count - 1, "M");
            DT_Members.SetValue("ST_APPROVAL_STATUS", DT_Members.Rows.Count - 1, "A");
            DT_Members.SetValue("ST_CUSTOMER_GROUP", DT_Members.Rows.Count - 1, form.DataSources.DBDataSources.Item(0).GetValue("U_ST_CUSTOMER_GROUP", 0).ToString());
            DT_Members.SetValue("ST_PASSPORT_ID", DT_Members.Rows.Count - 1, string.Empty);
            DT_Members.SetValue("ST_PERSONAL_ID", DT_Members.Rows.Count - 1, string.Empty);
            DT_Members.SetValue("ST_NATIONAL_ID", DT_Members.Rows.Count - 1, string.Empty);

        }

        internal static void Load_Sub_Members(SAPbouiCOM.Form form, string MemberCardType, string Card_ID, string searchPhrase = null)
        {
            try
            {
                DataTable DT_Members = form.DataSources.DataTables.Item("MEMBERS");
                form.Freeze(true);

                DT_Members.Rows.Clear();

                string SQL_Members = $@"SELECT *  FROM ""@ST_CCI_INDIV_CARD""  T0 
WHERE T0.""U_ST_PARENT_ID"" = '{Card_ID}' AND  T0.""U_ST_PARENT_TYPE"" = '{MemberCardType}' ";
                if (!string.IsNullOrEmpty(searchPhrase))
                {
                    SQL_Members += $@" AND (T0.""U_ST_NATIONAL_ID"" = '{searchPhrase}' OR T0.""U_ST_PERSONAL_ID"" = '{searchPhrase}' 
OR T0.""U_ST_PASSPORT_ID"" = '{searchPhrase}' OR T0.""U_ST_FULL_NAME_AR"" LIKE '%{searchPhrase}%' OR T0.""U_ST_FULL_NAME_EN"" LIKE '%{searchPhrase}%') ";
                }
                Recordset RC_Members = Helper.Utility.Execute_Recordset_Query(company, SQL_Members);

                DT_Members.Rows.Add(RC_Members.RecordCount);

                for (int i = 0; i < RC_Members.RecordCount; i++)
                {
                    for (int J = 1; J < DT_Members.Columns.Count; J++)
                    {
                        string Col_Name = DT_Members.Columns.Item(J).Name;
                        string UDF_Name = string.Empty;
                        try
                        {
                            if (Col_Name == "Code")
                            {
                                UDF_Name = Col_Name;
                            }
                            else
                            {
                                UDF_Name = "U_" + Col_Name;
                            }
                            DT_Members.SetValue(Col_Name, i, RC_Members.Fields.Item(UDF_Name).Value);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(Col_Name + UDF_Name);
                        }
                    }
                    RC_Members.MoveNext();
                }
                Grid Grd_Members = (Grid)form.Items.Item("138").Specific;
                for (int i = 0; i < DT_Members.Rows.Count; i++)
                {
                    for (int j = 2; j < Grd_Members.Columns.Count + 1; j++)
                    {
                        Grd_Members.CommonSetting.SetCellEditable(i + 1, j, false);
                    }
                }
                Grd_Members.AutoResizeColumns();

                if (MemberCardType == "I")
                {
                    string p_id = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_PARENT_ID", 0);
                    if (!string.IsNullOrEmpty(p_id))
                        form.Items.Item("138").Enabled = false;
                    else
                        form.Items.Item("138").Enabled = true;
                }
            }
            finally
            {
                form.Freeze(false);
            }
        }

        #endregion

        #region Attachments

        internal static void Add_Attachment(ItemEvent pVal, string MemberCardType)
        {
            SAPbouiCOM.Form form = SBO_Application.Forms.Item(pVal.FormUID);

            ST.Helper.Dialog.BrowseFile BF = new Helper.Dialog.BrowseFile(Helper.Dialog.DialogType.OpenFile);
            // BF.Filter = "CSV Files (*.csv)|*.csv";

            BF.ShowDialog();
            if (BF.FileName == "")
            {
                return;
            }
            if (BF.FileName.Split('.').Length >= 2)
            {
                int index = BF.FileName.Split('.').Length;
                if (BF.FileName.Split('.')[index - 1] == "exe")
                    return;
            }
            form.Freeze(true);
            DBDataSource DS_Attachment = form.DataSources.DBDataSources.Item(MemberCardType == "I" ? "@ST_CCI_INDIV_ATT" : "@ST_CCI_CORP_ATT");
            Matrix Mat_Add = (Matrix)form.Items.Item("500").Specific;
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
            DS_Attachment.SetValue("LineId", Count, (Count + 1).ToString());

            Mat_Add.LoadFromDataSource();
            Mat_Add.AutoResizeColumns();
            form.Freeze(false);
            if (form.Mode == BoFormMode.fm_OK_MODE)
                form.Mode = BoFormMode.fm_UPDATE_MODE;

        }

        internal static void Open_Attachment(ItemEvent pVal)
        {
            SAPbouiCOM.Form form = SBO_Application.Forms.Item(pVal.FormUID);

            Matrix Mat = (Matrix)form.Items.Item("500").Specific;
            for (int i = 0; i < Mat.RowCount; i++)
            {
                SAPbouiCOM.CheckBox Chk_Selected = (SAPbouiCOM.CheckBox)Mat.GetCellSpecific("SELECTED", i + 1);
                if (Chk_Selected.Checked)
                {
                    EditText Txt_FileName = (EditText)Mat.GetCellSpecific("FileName", i + 1);
                    System.Diagnostics.Process.Start(Txt_FileName.Value);
                }
            }
        }

        internal static void Remove_Attachment(ItemEvent pVal)
        {
            SAPbouiCOM.Form form = SBO_Application.Forms.Item(pVal.FormUID);

            Remove_Matrix_Row(form, "500");

        }

        #endregion

        internal static void Update_Card_BP(Form form, string UDO_Code, UDO_Definition UDO_Info)
        {
            try
            {
                //string UDO_Code = form.DataSources.DBDataSources.Item(0).GetValue("Code", 0).ToString();
                company.StartTransaction();
                string Address_Table_Name = "";
                string Name_Field = "";
                string Parent_Fields = "";
                if (UDO_Info.KHCF_Object == KHCF_Objects.CCI_Corporate_Member_Card)
                {
                    Name_Field = "U_ST_CORPORATE_ARABIC_NAME";
                }
                else
                {
                    Name_Field = "U_ST_FULL_NAME_AR";
                    Parent_Fields = ", U_ST_PARENT_ID, U_ST_PARENT_TYPE";
                }
                string SQL_BP = $@"SELECT U_ST_BP_CODE, U_ST_CUSTOMER_GROUP,U_ST_CURRENCY, {Name_Field} {Parent_Fields} FROM ""@{UDO_Info.Table_Name}"" WHERE ""Code"" = '{UDO_Code}' ";
                Recordset RC_BP = Helper.Utility.Execute_Recordset_Query(company, SQL_BP);
                if (UDO_Info.Table_Name.Contains("INDIV"))
                {
                    Address_Table_Name = "ST_CCI_INDIV_ADDR";
                }
                else
                {
                    Address_Table_Name = "ST_CCI_CORP_ADDR";
                }
                string BP_Code = RC_BP.Fields.Item("U_ST_BP_CODE").Value.ToString();

                KHCF_BP BP = new KHCF_BP();

                if (BP_Code == "")
                {
                    return;
                }


                string value = RC_BP.Fields.Item("U_ST_CUSTOMER_GROUP").Value.ToString();
                if (string.IsNullOrEmpty(value))
                    BP.BP_Group = 0;
                else
                    BP.BP_Group = int.Parse(RC_BP.Fields.Item("U_ST_CUSTOMER_GROUP").Value.ToString());
                BP.CardName = RC_BP.Fields.Item(Name_Field).Value.ToString();
                BP.Currency = RC_BP.Fields.Item("U_ST_CURRENCY").Value.ToString();
                if (UDO_Info.KHCF_Object == KHCF_Objects.CCI_Member_Card)
                {
                    string Parent_ID = RC_BP.Fields.Item("U_ST_PARENT_ID").Value.ToString();
                    if (Parent_ID != "")
                    {
                        string TableName;
                        if (RC_BP.Fields.Item("U_ST_PARENT_TYPE").Value.ToString() == "I")
                        {
                            TableName = "@" + Objects_Logic.All_UDO_Definition.FirstOrDefault(U => U.KHCF_Object == KHCF_Objects.CCI_Member_Card).Table_Name;
                        }
                        else
                        {
                            TableName = "@" + Objects_Logic.All_UDO_Definition.FirstOrDefault(U => U.KHCF_Object == KHCF_Objects.CCI_Corporate_Member_Card).Table_Name;
                        }
                        string SQL_Parent = $@"SELECT U_ST_BP_CODE, U_ST_FATHER_TYPE  FROM ""{TableName}"" WHERE ""Code"" = '{Parent_ID}' ";
                        Recordset RC_Parent = Helper.Utility.Execute_Recordset_Query(company, SQL_Parent);
                        if (RC_Parent.RecordCount == 0)
                        {
                            throw new Logic.Custom_Exception($"We can't find the card for the Parent ID[{Parent_ID}]");
                        }
                        string Parent_BP = RC_Parent.Fields.Item("U_ST_BP_CODE").Value.ToString();
                        if (Parent_BP == "")
                        {
                            throw new Logic.Custom_Exception($"There is no Business Partner for the parent ID[{Parent_ID}]");
                        }
                        BP.FatherCode = Parent_BP;
                        if (RC_Parent.Fields.Item("U_ST_FATHER_TYPE").Value.ToString() == "P")
                        {
                            BP.FatherType = BoFatherCardTypes.cPayments_sum;
                        }
                        else
                        {
                            BP.FatherType = BoFatherCardTypes.cDelivery_sum;
                        }
                    }

                }
                BP.MemberCard_Code = UDO_Code;

                string SQL_Addr = $@"SELECT T0.""U_ST_ADDRESS_NAME"", T0.""U_ST_STREET"", T0.""U_ST_BLOCK"", T0.""U_ST_ZIP_CODE"", T0.""U_ST_CITY""
, T0.""U_ST_COUNTY"",  T0.""U_ST_STATE"", T0.""U_ST_BUILDING"", T0.""U_ST_ADDRESS_TYPE"", T0.""U_ST_ADDRESS_NAME_2""
, T0.""U_ST_ADDRESS_NAME_3"", T0.""U_ST_STREET_NO""
,(Select T1.""Code"" From OCRY T1 Where T1.""Name"" =U_ST_COUNTRY)As U_ST_COUNTRY 
FROM ""@{Address_Table_Name}"" T0 WHERE ""Code"" = '{UDO_Code}' ";
                Recordset RC_Addr = Helper.Utility.Execute_Recordset_Query(company, SQL_Addr);
                BP.addresses = new List<BpAddress>();
                if (RC_Addr.RecordCount > 0)
                {
                    BP.addresses = new List<BpAddress>();
                    for (int i = 0; i < RC_Addr.RecordCount; i++)
                    {
                        BpAddress address = new BpAddress();
                        address.Street = RC_Addr.Fields.Item("U_ST_STREET").Value.ToString();
                        address.AddressName = RC_Addr.Fields.Item("U_ST_ADDRESS_NAME").Value.ToString();
                        address.City = RC_Addr.Fields.Item("U_ST_CITY").Value.ToString();
                        address.AddressType = RC_Addr.Fields.Item("U_ST_ADDRESS_TYPE").Value.ToString();
                        address.Country = RC_Addr.Fields.Item("U_ST_COUNTRY").Value.ToString();
                        address.Block = RC_Addr.Fields.Item("U_ST_BLOCK").Value.ToString();
                        address.ZipCode = RC_Addr.Fields.Item("U_ST_ZIP_CODE").Value.ToString();
                        address.County = RC_Addr.Fields.Item("U_ST_COUNTY").Value.ToString();
                        address.State = RC_Addr.Fields.Item("U_ST_STATE").Value.ToString();
                        address.BuildingFloorRoom = RC_Addr.Fields.Item("U_ST_BUILDING").Value.ToString();
                        address.AddressName2 = RC_Addr.Fields.Item("U_ST_ADDRESS_NAME_2").Value.ToString();
                        address.AddressName3 = RC_Addr.Fields.Item("U_ST_ADDRESS_NAME_3").Value.ToString();
                        address.StreetNo = RC_Addr.Fields.Item("U_ST_STREET_NO").Value.ToString();

                        BP.addresses.Add(address);

                        RC_Addr.MoveNext();
                    }
                }

                if (!UDO_Info.Table_Name.Contains("INDIV"))
                {
                    string SQL_Contacts = $@"Select T0.""U_ST_CONTACT_ID"", T0.""U_ST_NAME"", T0.""U_ST_POSITION"", T0.""U_ST_ADDRESS"", T0.""U_ST_TELEPHONE_1"", T0.""U_ST_TELEPHONE_2"", T0.""U_ST_MOBILE_PHONE"", T0.""U_ST_E_MAIL"", T0.""U_ST_E_MAIL_GROUP"", T0.""U_ST_PAGER"", T0.""U_ST_REMARKS_1"", T0.""U_ST_REMARKS_2"", T0.""U_ST_PASSWORD"", T0.""U_ST_COUNTRY"",
T0.""U_ST_GENDER"", T0.""U_ST_PROFESSION"", T0.""U_ST_CITY_OF_BIRTH"", T0.""U_ST_CONNECTED_ADDRESS"", T0.""U_ST_DATE_OF_BIRTH"",  T0.""U_ST_FAX""  From ""@ST_CCI_CORP_CONT"" T0 where T0.""Code""='{UDO_Code}'";
                    Recordset RC_Contact = Helper.Utility.Execute_Recordset_Query(company, SQL_Contacts);
                    string SQL_ATT = $@"select T0.""LineId"", T0.""U_ST_FILE_NAME"", T0.""U_ST_DESCRIPTION"", T0.""U_ST_TYPE"" From ""@ST_CCI_CORP_ATT"" T0 Where T0.""Code""='{UDO_Code}'";
                    Recordset RC_Att = Helper.Utility.Execute_Recordset_Query(company, SQL_ATT);

                    if (RC_Contact.RecordCount > 0)
                    {
                        BP.contacts = new List<BpContact>();
                        for (int i = 0; i < RC_Contact.RecordCount; i++)
                        {
                            BpContact Contact = new BpContact();
                            Contact.ContactID = RC_Contact.Fields.Item("U_ST_CONTACT_ID").Value.ToString();
                            Contact.Name = RC_Contact.Fields.Item("U_ST_NAME").Value.ToString();
                            Contact.Position = RC_Contact.Fields.Item("U_ST_POSITION").Value.ToString();
                            Contact.Address = RC_Contact.Fields.Item("U_ST_ADDRESS").Value.ToString();
                            Contact.Tel_1 = RC_Contact.Fields.Item("U_ST_TELEPHONE_1").Value.ToString();
                            Contact.Tel_2 = RC_Contact.Fields.Item("U_ST_TELEPHONE_2").Value.ToString();
                            Contact.Mobile = RC_Contact.Fields.Item("U_ST_MOBILE_PHONE").Value.ToString();
                            Contact.Email = RC_Contact.Fields.Item("U_ST_E_MAIL").Value.ToString();
                            Contact.EmailGroup = RC_Contact.Fields.Item("U_ST_E_MAIL_GROUP").Value.ToString();
                            Contact.Pager = RC_Contact.Fields.Item("U_ST_PAGER").Value.ToString();
                            Contact.Remarks_1 = RC_Contact.Fields.Item("U_ST_REMARKS_1").Value.ToString();
                            Contact.Remarks_2 = RC_Contact.Fields.Item("U_ST_REMARKS_2").Value.ToString();
                            Contact.Password = RC_Contact.Fields.Item("U_ST_PASSWORD").Value.ToString();
                            //Contact.PlaceOfBirth = RC_Contact.Fields.Item("U_ST_COUNTRY").Value.ToString();
                            Contact.Gender = RC_Contact.Fields.Item("U_ST_GENDER").Value.ToString();
                            Contact.Profession = RC_Contact.Fields.Item("U_ST_PROFESSION").Value.ToString();
                            Contact.CityOfBirth = RC_Contact.Fields.Item("U_ST_CITY_OF_BIRTH").Value.ToString();
                            Contact.ConnectedAddress = RC_Contact.Fields.Item("U_ST_CONNECTED_ADDRESS").Value.ToString();
                            Contact.DateOfBirth = RC_Contact.Fields.Item("U_ST_DATE_OF_BIRTH").Value.ToString();
                            Contact.Extention = RC_Contact.Fields.Item("U_ST_FAX").Value.ToString();
                            BP.contacts.Add(Contact);
                        }
                    }
                    if (RC_Att.RecordCount > 0)
                    {
                        BP.attatchments = new List<BPAttatchment>();
                        for (int i = 0; i < RC_Att.RecordCount; i++)
                        {
                            BPAttatchment attachment = new BPAttatchment();
                            attachment.LinId = RC_Att.Fields.Item("LineId").Value.ToString();
                            attachment.FileName = RC_Att.Fields.Item("U_ST_FILE_NAME").Value.ToString();
                            attachment.Description = RC_Att.Fields.Item("U_ST_DESCRIPTION").Value.ToString();
                            attachment.Type = RC_Att.Fields.Item("U_ST_TYPE").Value.ToString();
                            BP.attatchments.Add(attachment);
                        }
                    }
                }
                Utility.Update_BP(company, BP_Code, UDO_Code, UDO_Info);

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
                throw new Logic.Custom_Exception($"Error during Approving the Card[{UDO_Code}][{ex.Message}]");
            }
        }

        internal static void Remove_Matrix_Row(Form form, string Matrix_ID)
        {
            Matrix Mat = (Matrix)form.Items.Item(Matrix_ID).Specific;
            int Count = Mat.RowCount;
            for (int i = Count; i > 0; i--)
            {
                CheckBox Chk_Selected = (CheckBox)Mat.Columns.Item("SELECTED").Cells.Item(i).Specific;
                if (Chk_Selected.Checked)
                {
                    Mat.DeleteRow(i);
                    Mat.FlushToDataSource();
                    Mat.LoadFromDataSource();
                }
            }
        }

        internal static bool Process_Individual_Update_Approval(Form form, Parent_Form Form_Obj)
        {
            string Member_Card = form.DataSources.DBDataSources.Item(0).GetValue("Code", 0);
            string Approval_Status = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_APPROVAL_STATUS", 0);
            string lastMembershipInvoice; bool hasFinancialImpact;

            Membership.IndividualCardApprovalInfo(company, Member_Card, out lastMembershipInvoice, out hasFinancialImpact);

            bool Need_Approve = false;
            if (Approval_Status == "R")
            {
                if (SBO_Application.MessageBox($@"The card has been updated.
Do you want to send it to the Approval process?", 1, "Yes", "No") == 1)
                {
                    Need_Approve = true;
                }
                else
                {
                    return false;
                }
                if (Need_Approve)
                {
                    form.DataSources.DBDataSources.Item(0).SetValue("U_ST_APPROVAL_STATUS", 0, "P");
                }
            }
            string Membership_Code = form.DataSources.DBDataSources.Item(0).GetValue("Code", 0);
            string[] Approval_Fields_List = Configurations.Get_Individual_Card_Fields_For_Approval(company);
            string Code = form.DataSources.DBDataSources.Item(0).GetValue("Code", 0);
            string SQL_Member_DB_Data = $@"Select * FROM ""{Form_Obj.UDO_Database_Table_Name}"" WHERE ""Code"" = '{Code}'";
            Recordset RC_Member_DB_Data = Helper.Utility.Execute_Recordset_Query(company, SQL_Member_DB_Data);

            if (lastMembershipInvoice == string.Empty)
            {
                foreach (string OneField in Approval_Fields_List)
                {
                    Field_Definition Field_Info = Fields_Logic.All_Field_Definition.FirstOrDefault(F => F.KHCF_Object == Form_Obj.KHCF_Object && F.Field_Name == OneField.Replace("U_", ""));
                    if (Field_Info == null)
                    {
                        throw new Logic.Custom_Exception($"The Field[{OneField}] is not supported, please check the configuration");
                    }
                    if (Field_Info.Data_Type == BoFieldTypes.db_Date)
                    {
                        if (DateTime.ParseExact(form.DataSources.DBDataSources.Item(0).GetValue(OneField, 0), "yyyyMMdd", null) != (DateTime)RC_Member_DB_Data.Fields.Item(OneField).Value)
                        {
                            Need_Approve = true;
                        }
                    }
                    else
                    {
                        if (form.DataSources.DBDataSources.Item(0).GetValue(OneField, 0) != RC_Member_DB_Data.Fields.Item(OneField).Value.ToString())
                        {
                            Need_Approve = true;
                        }
                    }
                    if (Need_Approve)
                    {
                        throw new Custom_Exception("Please Remove the linked memberships first; premium calculation may be affected.");
                    }
                }
            }
            else if(hasFinancialImpact)
            {
                foreach (string OneField in Approval_Fields_List)
                {
                    Field_Definition Field_Info = Fields_Logic.All_Field_Definition.FirstOrDefault(F => F.KHCF_Object == Form_Obj.KHCF_Object && F.Field_Name == OneField.Replace("U_", ""));
                    if (Field_Info == null)
                    {
                        throw new Logic.Custom_Exception($"The Field[{OneField}] is not supported, please check the configuration");
                    }
                    if (Field_Info.Data_Type == BoFieldTypes.db_Date)
                    {
                        if (DateTime.ParseExact(form.DataSources.DBDataSources.Item(0).GetValue(OneField, 0), "yyyyMMdd", null) != (DateTime)RC_Member_DB_Data.Fields.Item(OneField).Value)
                        {
                            Need_Approve = true;
                        }
                    }
                    else
                    {
                        if (form.DataSources.DBDataSources.Item(0).GetValue(OneField, 0) != RC_Member_DB_Data.Fields.Item(OneField).Value.ToString())
                        {
                            Need_Approve = true;
                        }
                    }
                    if (Need_Approve)
                    {
                        if (SBO_Application.MessageBox($@"The field[{Field_Info.Field_Title}] has been changed.
Do you want to send it to the Approval process?", 1, "Yes", "No") == 1)
                        {
                            Need_Approve = true;
                            break;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
                if (Need_Approve)
                {
                    form.DataSources.DBDataSources.Item(0).SetValue("U_ST_APPROVAL_STATUS", 0, "P");
                }
            }
            return true;
        }

        internal static bool Process_Corporate_Update_Approval(Form form, Parent_Form Form_Obj)
        {
            string Member_Card = form.DataSources.DBDataSources.Item(0).GetValue("Code", 0);
            string Approval_Status = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_APPROVAL_STATUS", 0);
            bool hasFinancialImpact; string lastMembershipInvoice;

            Membership.CorporateCardApprovalInfo(company, Member_Card, out lastMembershipInvoice, out hasFinancialImpact);

            bool Need_Approve = false;
            if (Approval_Status == "R")
            {
                if (SBO_Application.MessageBox($@"The card has been updated.
Do you want to send it to the Approval process?", 1, "Yes", "No") == 1)
                {
                    Need_Approve = true;
                }
                else
                {
                    return false;
                }
                if (Need_Approve)
                {
                    form.DataSources.DBDataSources.Item(0).SetValue("U_ST_APPROVAL_STATUS", 0, "P");
                }
            }

            string Membership_Code = form.DataSources.DBDataSources.Item(0).GetValue("Code", 0);
            string[] Approval_Fields_List = Configurations.Get_Corporate_Card_Fields_For_Approval_Before_Update(company);
            string Code = form.DataSources.DBDataSources.Item(0).GetValue("Code", 0);
            string SQL_Member_DB_Data = $@"Select * FROM ""{Form_Obj.UDO_Database_Table_Name}"" WHERE ""Code"" = '{Code}'";
            Recordset RC_Member_DB_Data = Helper.Utility.Execute_Recordset_Query(company, SQL_Member_DB_Data);

            if (lastMembershipInvoice == string.Empty)
            {
                foreach (string OneField in Approval_Fields_List)
                {
                    Field_Definition Field_Info = Fields_Logic.All_Field_Definition.FirstOrDefault(F => F.KHCF_Object == Form_Obj.KHCF_Object && F.Field_Name == OneField.Replace("U_", ""));
                    if (Field_Info == null)
                    {
                        throw new Logic.Custom_Exception($"The Field[{OneField}] is not supported, please check the configuration");
                    }
                    if (Field_Info.Data_Type == BoFieldTypes.db_Date)
                    {
                        if (DateTime.ParseExact(form.DataSources.DBDataSources.Item(0).GetValue(OneField, 0), "yyyyMMdd", null) != (DateTime)RC_Member_DB_Data.Fields.Item(OneField).Value)
                        {
                            Need_Approve = true;
                        }
                    }
                    else
                    {
                        if (form.DataSources.DBDataSources.Item(0).GetValue(OneField, 0) != RC_Member_DB_Data.Fields.Item(OneField).Value.ToString())
                        {
                            Need_Approve = true;
                        }
                    }
                    if (Need_Approve)
                    {
                        throw new Custom_Exception("Please Remove the linked memberships first; premium calculation may be affected.");
                    }
                }
            }
            else if (hasFinancialImpact)
            {
                foreach (string OneField in Approval_Fields_List)
                {
                    Field_Definition Field_Info = Fields_Logic.All_Field_Definition.FirstOrDefault(F => F.KHCF_Object == Form_Obj.KHCF_Object && F.Field_Name == OneField.Replace("U_", ""));
                    if (Field_Info == null)
                    {
                        throw new Logic.Custom_Exception($"The Field[{OneField}] is not supported, please check the configuration.");
                    }
                    if (Field_Info.Data_Type == BoFieldTypes.db_Date)
                    {
                        if (DateTime.ParseExact(form.DataSources.DBDataSources.Item(0).GetValue(OneField, 0), "yyyyMMdd", null) != (DateTime)RC_Member_DB_Data.Fields.Item(OneField).Value)
                        {
                            Need_Approve = true;
                        }
                    }
                    else
                    {
                        if (form.DataSources.DBDataSources.Item(0).GetValue(OneField, 0) != RC_Member_DB_Data.Fields.Item(OneField).Value.ToString())
                        {
                            Need_Approve = true;
                        }
                    }
                    if (Need_Approve)
                    {
                        if (SBO_Application.MessageBox($@"The field[{Field_Info.Field_Title}] has been changed.
Do you want to send it to the Approval process?", 1, "Yes", "No") == 1)
                        {
                            Need_Approve = true;
                            break;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
                if (Need_Approve)
                {
                    form.DataSources.DBDataSources.Item(0).SetValue("U_ST_APPROVAL_STATUS", 0, "P");
                }
            }
            return true;
        }
    }
}




// ProcessUpdateApproval
//            else if (hasFinancialImpact && !lastMembershipHasInvoice)
//            {
//                string Membership_Code = form.DataSources.DBDataSources.Item(0).GetValue("Code", 0);
//string[] Approval_Fields_List = Configurations.Get_Individual_Card_Fields_For_Approval(company);
//string Code = form.DataSources.DBDataSources.Item(0).GetValue("Code", 0);
//string SQL_Current_Date = $@"Select * FROM ""{Form_Obj.UDO_Database_Table_Name}"" WHERE ""Code"" = '{Code}'";
//Recordset RC_Current_Date = Helper.Utility.Execute_Recordset_Query(company, SQL_Current_Date);

//                foreach (string OneField in Approval_Fields_List)
//                {
//                    Field_Definition Field_Info = Fields_Logic.All_Field_Definition.FirstOrDefault(F => F.KHCF_Object == Form_Obj.KHCF_Object && F.Field_Name == OneField.Replace("U_", ""));
//                    if (Field_Info == null)
//                    {
//                        throw new Logic.Custom_Exception($"The Field[{OneField}] is not supported, please check the configuration");
//                    }
//                    if (Field_Info.Data_Type == BoFieldTypes.db_Date)
//                    {
//                        if (DateTime.ParseExact(form.DataSources.DBDataSources.Item(0).GetValue(OneField, 0), "yyyyMMdd", null) != (DateTime) RC_Current_Date.Fields.Item(OneField).Value)
//                        {
//                            Need_Approve = true;
//                        }
//                    }
//                    else
//                    {
//                        if (form.DataSources.DBDataSources.Item(0).GetValue(OneField, 0) != RC_Current_Date.Fields.Item(OneField).Value.ToString())
//                        {
//                            Need_Approve = true;
//                        }
//                    }
//                    if (Need_Approve)
//                    {
//                        throw new Custom_Exception("Please Rremove the linked memberships first; premium calculation may be affected.");
//                    }
//                    else
//                    {
//                        Need_Approve = true;
//                    }
//                }

//            }
//            else if ((lastMembershipHasInvoice))
//            {
//                string Membership_Code = form.DataSources.DBDataSources.Item(0).GetValue("Code", 0);
//string[] Approval_Fields_List = Configurations.Get_Individual_Card_Fields_For_Approval(company);
//string Code = form.DataSources.DBDataSources.Item(0).GetValue("Code", 0);
//string SQL_Current_Date = $@"Select * FROM ""{Form_Obj.UDO_Database_Table_Name}"" WHERE ""Code"" = '{Code}'";
//Recordset RC_Current_Date = Helper.Utility.Execute_Recordset_Query(company, SQL_Current_Date);

//                foreach (string OneField in Approval_Fields_List)
//                {
//                    Field_Definition Field_Info = Fields_Logic.All_Field_Definition.FirstOrDefault(F => F.KHCF_Object == Form_Obj.KHCF_Object && F.Field_Name == OneField.Replace("U_", ""));
//                    if (Field_Info == null)
//                    {
//                        throw new Logic.Custom_Exception($"The Field[{OneField}] is not supported, please check the configuration");
//                    }
//                    if (Field_Info.Data_Type == BoFieldTypes.db_Date)
//                    {
//                        if (DateTime.ParseExact(form.DataSources.DBDataSources.Item(0).GetValue(OneField, 0), "yyyyMMdd", null) != (DateTime) RC_Current_Date.Fields.Item(OneField).Value)
//                        {
//                            Need_Approve = true;
//                        }
//                    }
//                    else
//                    {
//                        if (form.DataSources.DBDataSources.Item(0).GetValue(OneField, 0) != RC_Current_Date.Fields.Item(OneField).Value.ToString())
//                        {
//                            Need_Approve = true;
//                        }
//                    }
//                    if (Need_Approve)
//                    {
//                        if (SBO_Application.MessageBox($@"The field[{Field_Info.Field_Title}] has been changed and you need to send the Card to Approval
//Do you want to send it to the Approval process?", 1, "Yes", "No") == 1)
//                        {
//                            Need_Approve = true;
//                            break;
//                        }
//                        else
//                        {
//                            return false;
//                        }
//                    }
//                }
//                if (Need_Approve)
//                {
//                    form.DataSources.DBDataSources.Item(0).SetValue("U_ST_APPROVAL_STATUS", 0, "P");
//                }
//                return true;
//            }
