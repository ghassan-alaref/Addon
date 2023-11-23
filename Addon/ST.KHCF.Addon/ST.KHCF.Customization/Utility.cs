using SAPbobsCOM;
using ST.KHCF.Customization.Logic;
using ST.KHCF.Customization.Logic.Classes;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
//using static ST.Helper.Service_Layer;

namespace ST.KHCF.Customization
{
    internal class Utility
    {
        internal class Item_UI
        {
            internal string Item_ID;
            internal int Width;
            internal int Top;
            internal int Height;
            internal int Left;
        }

        internal static string Get_Configuration(Company company, string ConfigCode, string Config_Description = "", string Default_Value = "")
        {
            string TableName = "ST_KHCF_CONFIG";

            return Helper.Utility.Get_Configuration(company, ConfigCode, Config_Description, Default_Value, TableName);
        }

        internal static string Get_Field_Configuration(Company company, string ConfigCode, string Config_Description = "", string Default_Value = "")
        {
            string TableName = "ST_CONFIG_VALUES";

            return Helper.Utility.Get_Configuration(company, ConfigCode, Config_Description, Default_Value, TableName);
        }

        internal static string Get_Full_Name(string firstName, string fatherName, string middleName, string surName)
        {
            string Result = firstName;
            if (fatherName != "")
            {
                Result += " " + fatherName;
            }
            if (middleName != "")
            {
                Result += " " + middleName;
            }
            if (surName != "")
            {
                Result += " " + surName;
            }
            return Result;
        }

        internal static string Get_New_UDO_Code(Company company, KHCF_Objects KHCF_Object)
        {
            UDO_Definition UDO_Def = Objects_Logic.All_UDO_Definition.FirstOrDefault(O => O.KHCF_Object == KHCF_Object);
            string Table_Name = UDO_Def.Table_Name;
            string SQL = $@"Select Max(CAST(""Code"" AS int)) from ""@{Table_Name}"" ";
            Recordset RC = Helper.Utility.Execute_Recordset_Query(company, SQL);
            if (RC.RecordCount == 0)
            {
                return "00000001";
            }
            return ((int)RC.Fields.Item(0).Value + 1).ToString().PadLeft(8, '0');
        }

        internal static int Get_Series_By_BP_Group(Company company, int BP_Groupe)
        {
            string SQL = $@"SELECT T0.""U_ST_SERIES_ID"" FROM OCRG T0 WHERE ""GroupCode"" = {BP_Groupe}";
            Recordset RC = Helper.Utility.Execute_Recordset_Query(company, SQL);
            if (RC.RecordCount == 0)
            {
                throw new Logic.Custom_Exception($"The BP Group[{BP_Groupe}] is not existing in B1");
            }
            int Result = (int)RC.Fields.Item("U_ST_SERIES_ID").Value;
            if (Result == 0)
            {
                throw new Logic.Custom_Exception($"The BP Group[{BP_Groupe}] is not mapped with a series");
            }
            return Result;
        }

        internal static string Create_BP(Company company, KHCF_BP BP_Data)
        {
            BusinessPartners BP = (BusinessPartners)company.GetBusinessObject(BoObjectTypes.oBusinessPartners);
            int Series_ID = Utility.Get_Series_By_BP_Group(company, BP_Data.BP_Group);
            BP.Series = Series_ID;
            BP.GroupCode = BP_Data.BP_Group;
            BP.CardName = BP_Data.CardName;
            BP.Currency = BP_Data.Currency;
            if (BP_Data.SalesPersonCode != 0)
            {
                BP.SalesPersonCode = BP_Data.SalesPersonCode;
            }
            BP.UserFields.Fields.Item("U_ST_DATA_SOURCE").Value = "008";

            BP.UserFields.Fields.Item("U_ST_MEMBER_CARD").Value = BP_Data.MemberCard_Code;
            if (BP_Data.Is_Lead)
            {
                BP.CardType = BoCardTypes.cLid;
            }
            if (BP_Data.Is_Vendor)
            {
                BP.CardType = BoCardTypes.cSupplier;
            }
            if (!string.IsNullOrEmpty(BP_Data.FatherCode))
            {
                BP.FatherCard = BP_Data.FatherCode;
                BP.FatherType = BP_Data.FatherType;
            }
            BP.EmailAddress = BP_Data.Email;
            BP.Cellular = BP_Data.Mobile;

            if (BP_Data.addresses.Count > 0)
            {
                for (int i = 0; i < BP_Data.addresses.Count; i++)
                {
                    BP.Addresses.AddressName = BP_Data.addresses[i].AddressName;
                    BP.Addresses.City = BP_Data.addresses[i].City;
                    BP.Addresses.Street = BP_Data.addresses[i].Street;
                    BP.Addresses.Country = BP_Data.addresses[i].Country;
                    if (BP_Data.addresses[i].AddressType == "S")
                    {
                        BP.Addresses.AddressType = BoAddressType.bo_ShipTo;
                    }
                    if (BP_Data.addresses[i].AddressType == "B")
                    {
                        BP.Addresses.AddressType = BoAddressType.bo_BillTo;
                    }
                    BP.Addresses.Block = BP_Data.addresses[i].Block;
                    BP.Addresses.ZipCode = BP_Data.addresses[i].ZipCode;
                    BP.Addresses.County = BP_Data.addresses[i].County;
                    BP.Addresses.BuildingFloorRoom = BP_Data.addresses[i].BuildingFloorRoom;
                    BP.Addresses.AddressName2 = BP_Data.addresses[i].AddressName2;
                    BP.Addresses.AddressName3 = BP_Data.addresses[i].AddressName3;
                    BP.Addresses.StreetNo = BP_Data.addresses[i].StreetNo;


                    BP.Addresses.Add();
                }
            }
            if (BP_Data.contacts.Count > 0)
            {
                for (int i = 0; i < BP_Data.contacts.Count; i++)
                {
                    if (string.IsNullOrEmpty(BP_Data.contacts[i].Name))
                        break;
                    BP.ContactEmployees.Name = BP_Data.contacts[i].Name;
                    BP.ContactEmployees.Position = BP_Data.contacts[i].Position;
                    BP.ContactEmployees.Address = BP_Data.contacts[i].Address;
                    BP.ContactEmployees.Phone1 = BP_Data.contacts[i].Tel_1;
                    BP.ContactEmployees.Phone2 = BP_Data.contacts[i].Tel_2;
                    BP.ContactEmployees.MobilePhone = BP_Data.contacts[i].Mobile;
                    BP.ContactEmployees.EmailGroupCode = "";
                    BP.ContactEmployees.E_Mail = BP_Data.contacts[i].Email;
                    BP.ContactEmployees.Pager = BP_Data.contacts[i].Pager;
                    BP.ContactEmployees.Remarks1 = BP_Data.contacts[i].Remarks_1;
                    BP.ContactEmployees.Remarks2 = BP_Data.contacts[i].Remarks_2;
                    BP.ContactEmployees.Password = BP_Data.contacts[i].Password;
                    BP.ContactEmployees.PlaceOfBirth = "";
                    if (BP_Data.contacts[i].Gender == "M")
                        BP.ContactEmployees.Gender = BoGenderTypes.gt_Male;
                    else
                        BP.ContactEmployees.Gender = BoGenderTypes.gt_Female;
                    BP.ContactEmployees.DateOfBirth = Convert.ToDateTime(BP_Data.contacts[i].DateOfBirth);
                    BP.ContactEmployees.Profession = BP_Data.contacts[i].Profession;
                    BP.ContactEmployees.CityOfBirth = BP_Data.contacts[i].CityOfBirth;
                    BP.ContactEmployees.ConnectedAddressName = BP_Data.contacts[i].ConnectedAddress;
                    BP.ContactEmployees.Fax = BP_Data.contacts[i].Mobile;

                    BP.ContactEmployees.Add();
                }
            }
            if (BP_Data.attatchments.Count > 0)
            {
                SAPbobsCOM.Attachments2 oATT = company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oAttachments2) as SAPbobsCOM.Attachments2;
                for (int i = 0; i < BP_Data.attatchments.Count; i++)
                {
                    if (string.IsNullOrEmpty(BP_Data.attatchments[i].FileName))
                        break;
                    oATT.Lines.Add();
                    oATT.Lines.FileName = BP_Data.attatchments[i].FileName;
                    oATT.Lines.FileExtension = System.IO.Path.GetExtension(BP_Data.attatchments[i].FileName).Substring(1);
                }
                int iAttEntry = -1;
                if (oATT.Add() == 0)
                {
                    iAttEntry = int.Parse(company.GetNewObjectKey());
                    BP.AttachmentEntry = iAttEntry;
                }
                else
                {
                    string s = company.GetLastErrorDescription();
                }

            }
            if (BP.Add() != 0)
            {
                throw new Logic.Custom_Exception($@"Business Partner[{company.GetLastErrorDescription()}]");
            }
            string BP_Code;
            company.GetNewObjectCode(out BP_Code);
            return BP_Code;
        }

        internal static string Chosse_From_List_For_Code_And_DBDataSource(SAPbouiCOM.ItemEvent pVal, string ItemUID, bool Is_User_DataSource = false, string DataSource_Tablename = "")
        {
            SAPbouiCOM.Form form = Loader.SBO_Application.Forms.Item(pVal.FormUID);
            SAPbouiCOM.ChooseFromListEvent Choos_Event = (SAPbouiCOM.ChooseFromListEvent)pVal;

            if (Choos_Event.SelectedObjects == null || Choos_Event.SelectedObjects.Rows.Count == 0)
            {
                return "";
            }

            string UDF_Name = Helper.Utility.Get_Item_DB_Datasource(form.Items.Item(ItemUID));
            string Code = Choos_Event.SelectedObjects.GetValue("Code", 0).ToString();
            if (Is_User_DataSource)
            {
                form.DataSources.UserDataSources.Item(UDF_Name).Value = Code;
            }
            else
            {
                string X;
                if (DataSource_Tablename == "")
                {
                    X = form.DataSources.DBDataSources.Item(0).TableName;
                }
                else
                {
                    X = DataSource_Tablename;
                }

                form.DataSources.DBDataSources.Item(X).SetValue(UDF_Name, 0, Code);
                if (form.Mode == SAPbouiCOM.BoFormMode.fm_OK_MODE)
                {
                    form.Mode = SAPbouiCOM.BoFormMode.fm_UPDATE_MODE;
                }
                //string Y = pVal.ItemUID;
                //((SAPbouiCOM.EditText)form.Items.Item(pVal.ItemUID).Specific).Value = Code;
            }


            return Code;
        }

        internal static string Chosse_From_List_For_Code_And_DBDataSource(SAPbouiCOM.ItemEvent pVal, string ItemUID, out SAPbouiCOM.DataTable CFL_DataTable, bool Is_User_DataSource = false, string DataSource_Tablename = "")
        {
            SAPbouiCOM.Form form = Loader.SBO_Application.Forms.Item(pVal.FormUID);
            SAPbouiCOM.ChooseFromListEvent Choos_Event = (SAPbouiCOM.ChooseFromListEvent)pVal;
            CFL_DataTable = Choos_Event.SelectedObjects;
            if (Choos_Event.SelectedObjects == null || Choos_Event.SelectedObjects.Rows.Count == 0)
            {
                return "";
            }

            string UDF_Name = Helper.Utility.Get_Item_DB_Datasource(form.Items.Item(ItemUID));
            string Code = Choos_Event.SelectedObjects.GetValue("Code", 0).ToString();
            if (Is_User_DataSource)
            {
                form.DataSources.UserDataSources.Item(UDF_Name).Value = Code;
            }
            else
            {
                string X;
                if (DataSource_Tablename == "")
                {
                    X = form.DataSources.DBDataSources.Item(0).TableName;
                }
                else
                {
                    X = DataSource_Tablename;
                }

                form.DataSources.DBDataSources.Item(X).SetValue(UDF_Name, 0, Code);
                //string Y = pVal.ItemUID;
                //((SAPbouiCOM.EditText)form.Items.Item(pVal.ItemUID).Specific).Value = Code;
            }


            return Code;
        }
        internal static void Fill_Relation_Grids(Company company, SAPbouiCOM.Form form, string UDO_Code, string[] Relations_Grid_IDs)
        {
            form.Freeze(true);
            foreach (string OneGrid in Relations_Grid_IDs)
            {
                SAPbouiCOM.Grid Grd = (SAPbouiCOM.Grid)form.Items.Item(OneGrid).Specific;
                SAPbouiCOM.DataTable DT = Grd.DataTable;
                DT.Rows.Clear();
                string Table_Name = DT.UniqueID;
                if (Table_Name.StartsWith("ST_"))
                {
                    Table_Name = "@" + Table_Name;
                }
                string SQL = $@"Select ""Code"", ""Name"" FROM ""{Table_Name}"" ORDER BY ""Name""";
                Recordset RC = Helper.Utility.Execute_Recordset_Query(company, SQL);
                DT.Rows.Add(RC.RecordCount);

                for (int i = 0; i < RC.RecordCount; i++)
                {
                    DT.SetValue("Code", i, RC.Fields.Item("Code").Value);
                    DT.SetValue("Name", i, RC.Fields.Item("Name").Value);
                    RC.MoveNext();
                }

                Grd.Columns.Item("Code").Visible = false;
                Grd.AutoResizeColumns();
            }
            form.Freeze(false);

        }

        internal static void Load_All_Relation_Data(Company company, SAPbouiCOM.Form form, string UDO_Code, string[] Relations_Grid_IDs, KHCF_Objects KHCF_Object)
        {
            string KHCF_Object_Type = ((int)KHCF_Object).ToString();
            string[] Basic_Cols = new string[] { "SELECTED", "Code", "Name" };

            foreach (string OneGrid in Relations_Grid_IDs)
            {
                SAPbouiCOM.Grid Grd = (SAPbouiCOM.Grid)form.Items.Item(OneGrid).Specific;
                SAPbouiCOM.DataTable DT = Grd.DataTable;
                for (int i = 0; i < DT.Rows.Count; i++)
                {
                    DT.SetValue("SELECTED", i, "N");
                }
                List<string> Additional_UDFs = new List<string>();
                for (int i = 0; i < DT.Columns.Count; i++)
                {
                    string Col = DT.Columns.Item(i).Name;
                    if (!Basic_Cols.Contains(Col))
                    {
                        Additional_UDFs.Add(Col);
                    }
                }
                string SQL_Relation_Codes = $@"SELECT * FROM ""@ST_REL_OBJECTS""  T0 
WHERE T0.U_ST_KHCF_OBJECT_TYPE ='{KHCF_Object_Type}' AND  T0.U_ST_KHCF_OBJECT_CODE = '{UDO_Code}' 
AND  T0.U_ST_KHCF_TABLE_NAME = '{DT.UniqueID}' ";
                Recordset RC_Code = Helper.Utility.Execute_Recordset_Query(company, SQL_Relation_Codes);
                for (int i = 0; i < RC_Code.RecordCount; i++)
                {
                    string UDT_Code = RC_Code.Fields.Item("U_ST_KHCF_TABLE_CODE").Value.ToString();
                    for (int J = 0; J < DT.Rows.Count; J++)
                    {
                        if (DT.GetValue("Code", J).ToString() == UDT_Code)
                        {
                            DT.SetValue("SELECTED", J, "Y");
                            foreach (string Add_Col in Additional_UDFs)
                            {
                                DT.SetValue(Add_Col, J, RC_Code.Fields.Item(Add_Col).Value);
                            }
                            break;
                        }
                    }

                    RC_Code.MoveNext();
                }

                for (int J = 0; J < DT.Rows.Count; J++)
                {
                    if (DT.GetValue("SELECTED", J).ToString() != "Y")
                    {
                        foreach (string Add_Col in Additional_UDFs)
                        {
                            DT.SetValue(Add_Col, J,0);
                        }
                    }
                }


            }

        }

        internal static void Update_Relation_Table(Company company, SAPbouiCOM.Form form, string UDO_Code, string[] Relations_Grid_IDs, KHCF_Objects KHCF_Object)
        {
            string KHCF_Object_Type = ((int)KHCF_Object).ToString();
            Update_Relation_Table(company, form, UDO_Code, Relations_Grid_IDs, KHCF_Object_Type);
        }

        internal static void Update_Relation_Table(Company company, SAPbouiCOM.Form form, string UDO_Code, string[] Relations_Grid_IDs, string KHCF_Object_Type, bool With_Selected_Column = true)
        {
            UserTable UDT_Rel = company.UserTables.Item("ST_REL_OBJECTS");
            string[] Basic_Cols = new string[] { "SELECTED", "Code", "Name" };

            foreach (string OneGrid in Relations_Grid_IDs)
            {
                SAPbouiCOM.Grid Grd = (SAPbouiCOM.Grid)form.Items.Item(OneGrid).Specific;
                SAPbouiCOM.DataTable DT = Grd.DataTable;
                List<string> Additional_UDFs = new List<string>();
                for (int i = 0; i < DT.Columns.Count; i++)
                {
                    string Col = DT.Columns.Item(i).Name;
                    if (!Basic_Cols.Contains(Col))
                    {
                        Additional_UDFs.Add(Col);
                    }
                }

                if (With_Selected_Column == true)
                {
                    for (int i = 0; i < DT.Rows.Count; i++)
                    {
                        if (DT.GetValue("SELECTED", i).ToString() != "Y")
                        {
                            string Code = DT.GetValue("Code", i).ToString();

                            string SQL_Relation_Code = $@"SELECT T0.""Code"" FROM ""@ST_REL_OBJECTS""  T0 
WHERE T0.U_ST_KHCF_OBJECT_TYPE ='{KHCF_Object_Type}' AND  T0.U_ST_KHCF_OBJECT_CODE = '{UDO_Code}' 
AND  T0.U_ST_KHCF_TABLE_NAME = '{DT.UniqueID}' AND  T0.U_ST_KHCF_TABLE_CODE ='{Code}'";
                            Recordset RC_Code = Helper.Utility.Execute_Recordset_Query(company, SQL_Relation_Code);

                            if (RC_Code.RecordCount != 0)
                            {
                                string REL_Code = RC_Code.Fields.Item("Code").Value.ToString();
                                UDT_Rel.GetByKey(REL_Code);
                                if (UDT_Rel.Remove() != 0)
                                {
                                    throw new Logic.Custom_Exception($"Error during remove the row[{REL_Code}][{company.GetLastErrorDescription()}]");
                                }
                            }
                        }
                    }
                }
                else
                {
                    string SQL_Relation_Code_List = $@"SELECT T0.""U_ST_KHCF_TABLE_CODE"" FROM ""@ST_REL_OBJECTS""  T0 
WHERE T0.U_ST_KHCF_OBJECT_TYPE ='{KHCF_Object_Type}' AND  T0.U_ST_KHCF_OBJECT_CODE = '{UDO_Code}' 
AND  T0.U_ST_KHCF_TABLE_NAME = '{DT.UniqueID}' ";
                    Recordset RC_Code_List = Helper.Utility.Execute_Recordset_Query(company, SQL_Relation_Code_List);
                    List<string> DB_Code_List = new List<string>();
                    for (int i = 0; i < RC_Code_List.RecordCount; i++)
                    {
                        DB_Code_List.Add(RC_Code_List.Fields.Item("U_ST_KHCF_TABLE_CODE").Value.ToString());
                        RC_Code_List.MoveNext();
                    }

                    List<string> DT_Code_List = new List<string>();
                    for (int i = 0; i < DT.Rows.Count; i++)
                    {
                        DT_Code_List.Add(DT.GetValue("Code", i).ToString());
                    }
                    foreach (string One_DB_Code in DB_Code_List)
                    {
                        bool Is_Existing = false;
                        foreach (string One_DT_Code in DT_Code_List)
                        {
                            if (One_DB_Code == One_DT_Code)
                            {
                                Is_Existing = true;
                                break;
                            }
                        }
                        if (Is_Existing == false)
                        {
                            string SQL_Relation_Code = $@"SELECT T0.""Code"" FROM ""@ST_REL_OBJECTS""  T0 
WHERE T0.U_ST_KHCF_OBJECT_TYPE ='{KHCF_Object_Type}' AND  T0.U_ST_KHCF_OBJECT_CODE = '{UDO_Code}' 
AND  T0.U_ST_KHCF_TABLE_NAME = '{DT.UniqueID}' AND  T0.U_ST_KHCF_TABLE_CODE ='{One_DB_Code}'";
                            Recordset RC_Code = Helper.Utility.Execute_Recordset_Query(company, SQL_Relation_Code);

                            if (RC_Code.RecordCount != 0)
                            {
                                string REL_Code = RC_Code.Fields.Item("Code").Value.ToString();
                                UDT_Rel.GetByKey(REL_Code);
                                if (UDT_Rel.Remove() != 0)
                                {
                                    throw new Logic.Custom_Exception($"Error during remove the row[{REL_Code}][{company.GetLastErrorDescription()}]");
                                }
                            }

                        }
                    }
                }

                for (int i = 0; i < DT.Rows.Count; i++)
                {
                    if (DT.GetValue("SELECTED", i).ToString() == "Y" || With_Selected_Column == false)
                    {
                        string Code = DT.GetValue("Code", i).ToString();

                        string SQL_Relation_Code = $@"SELECT T0.""Code"" FROM ""@ST_REL_OBJECTS""  T0 
WHERE T0.U_ST_KHCF_OBJECT_TYPE ='{KHCF_Object_Type}' AND  T0.U_ST_KHCF_OBJECT_CODE = '{UDO_Code}' 
AND  T0.U_ST_KHCF_TABLE_NAME = '{DT.UniqueID}' AND  T0.U_ST_KHCF_TABLE_CODE ='{Code}'";
                        Recordset RC_Code = Helper.Utility.Execute_Recordset_Query(company, SQL_Relation_Code);

                        if (RC_Code.RecordCount == 0)
                        {
                            UDT_Rel.Code = Helper.Utility.getCode("@ST_REL_OBJECTS", company);
                            UDT_Rel.Name = UDT_Rel.Code;

                            UDT_Rel.UserFields.Fields.Item("U_ST_KHCF_OBJECT_TYPE").Value = KHCF_Object_Type;
                            UDT_Rel.UserFields.Fields.Item("U_ST_KHCF_OBJECT_CODE").Value = UDO_Code;
                            UDT_Rel.UserFields.Fields.Item("U_ST_KHCF_TABLE_NAME").Value = DT.UniqueID;
                            UDT_Rel.UserFields.Fields.Item("U_ST_KHCF_TABLE_CODE").Value = Code;
                            foreach (string Add_Col in Additional_UDFs)
                            {
                                UDT_Rel.UserFields.Fields.Item(Add_Col).Value = DT.GetValue(Add_Col, i);
                            }

                            if (UDT_Rel.Add() != 0)
                            {
                                string X = company.GetLastErrorDescription();
                                throw new Logic.Custom_Exception($"Error during add a new row on the table[{DT.UniqueID}][{company.GetLastErrorDescription()}]");
                            }
                        }
                        else
                        {
                            UDT_Rel.GetByKey(RC_Code.Fields.Item("Code").Value.ToString());
                            foreach (string Add_Col in Additional_UDFs)
                            {
                                UDT_Rel.UserFields.Fields.Item(Add_Col).Value = DT.GetValue(Add_Col, i);
                            }

                            if (UDT_Rel.Update() != 0)
                            {
                                string X = company.GetLastErrorDescription();
                                throw new Logic.Custom_Exception($"Error during Update a row[{Code}] on the table[{DT.UniqueID}][{company.GetLastErrorDescription()}]");
                            }

                        }
                    }
                }


            }
        }

        internal static string Get_Current_User_Role(Company company)
        {
            string SQL = $@"Select U_ST_ROLE from OUSR WHERE USER_CODE = '{company.UserName}'";
            Recordset RC = Helper.Utility.Execute_Recordset_Query(company, SQL);

            return RC.Fields.Item("U_ST_ROLE").Value.ToString();
        }

        internal static string Get_UDO_Type_ID(KHCF_Objects KHCF_Enum)
        {
            return Objects_Logic.All_UDO_Definition.FirstOrDefault(U => U.KHCF_Object == KHCF_Enum).Table_Name;
            //return KHCF_Enum.ToString();
        }

        internal static string Get_Form_Type_ID(KHCF_Objects KHCF_Enum)
        {
            return "ST_" + KHCF_Enum.ToString();
        }

        //        internal static string Get_BP_Code(Company company, string Membership_Code)
        //        {
        //            string SQL_Mem_Ship = $@"SELECT T0.U_ST_START_DATE, T1.U_ST_BP_CODE, T1.""CreateDate""
        //FROM ""@ST_INDIV_MEMBERSHIP""  T0 INNER JOIN ""@ST_CCI_INDIV_CARD""  T1 ON T0.U_ST_MEMBER_CARD = T1.""Code"" 
        //WHERE T0.""Code"" = '{Membership_Code}'";
        //            Recordset RC_Mem_Ship = Helper.Utility.Execute_Recordset_Query(company, SQL_Mem_Ship);

        //            if (RC_Mem_Ship.RecordCount == 0)
        //            {
        //                throw new  Logic.Custom_Exception($"There is no Member Card Data for the Membership[{Membership_Code}]");
        //            }
        //            return RC_Mem_Ship.Fields.Item("U_ST_BP_CODE").Value.ToString();
        //        }

        internal static void Update_BP(Company company, string BP_Code, string UDO_Code, UDO_Definition UDO_Info, string Control_Account = null, bool Is_Lead = false)
        {
            if (BP_Code == "")
            {
                return;
            }
            BusinessPartners BP = (BusinessPartners)company.GetBusinessObject(BoObjectTypes.oBusinessPartners);
            BP.GetByKey(BP_Code);

            if (BP.CardType == BoCardTypes.cLid && Is_Lead == false)
            {
                BP.CardType = BoCardTypes.cCustomer;
                BP.UserFields.Fields.Item("U_ST_MEMBER_CARD").Value = UDO_Code;
            }

            string SQL_Group = $@"SELECT T0.""U_ST_GL_ACCOUNT""  FROM OCRG T0 where T0.""GroupCode""='{BP.GroupCode}'";
            Recordset RC_Group = Helper.Utility.Execute_Recordset_Query(company, SQL_Group);
            if (RC_Group.RecordCount > 0)
            {
                BP.DebitorAccount = RC_Group.Fields.Item("U_ST_GL_ACCOUNT").Value.ToString();
            }

            if (UDO_Info.KHCF_Object == KHCF_Objects.CCI_Member_Card)
            {
                string UDF = "U_ST_DATE_OF_BIRTH,U_ST_SUB_CHANNEL,U_ST_BROKER1,U_ST_NATIONAL_ID,U_ST_PERSONAL_ID,U_ST_PASSPORT_ID,U_ST_FATHER_NAME_AR,U_ST_MIDDLE_NAME_AR,U_ST_SURNAME_AR,U_ST_FIRST_NAME_EN,U_ST_FATHER_NAME_EN,U_ST_MIDDLE_NAME_EN,U_ST_SURNAME_EN,U_ST_FULL_NAME_EN,U_ST_ACCOUNT_MANAGER,U_ST_NATIONALITY";

                string SQL = $@"Select U_ST_TEL1, U_ST_CURRENCY, U_ST_TEL2, U_ST_EMAIL, U_ST_FULL_NAME_AR,U_ST_ACCOUNT_MANAGER U_ST_FULL_NAME_EN, U_ST_MOBILE, U_ST_CUSTOMER_GROUP ,{UDF}
FROM ""@{UDO_Info.Table_Name}"" WHERE ""Code"" = '{UDO_Code}'";
                Recordset RC = Helper.Utility.Execute_Recordset_Query(company, SQL);

                BP.CardName = RC.Fields.Item("U_ST_FULL_NAME_AR").Value.ToString();
                BP.CardForeignName = RC.Fields.Item("U_ST_FULL_NAME_EN").Value.ToString();
                BP.Phone1 = RC.Fields.Item("U_ST_TEL1").Value.ToString();
                BP.Phone2 = RC.Fields.Item("U_ST_TEL2").Value.ToString();
                BP.Cellular = RC.Fields.Item("U_ST_MOBILE").Value.ToString();
                BP.EmailAddress = RC.Fields.Item("U_ST_EMAIL").Value.ToString();
                BP.SalesPersonCode = Convert.ToInt32(RC.Fields.Item("U_ST_ACCOUNT_MANAGER").Value);
                BP.GroupCode = Convert.ToInt32(RC.Fields.Item("U_ST_CUSTOMER_GROUP").Value);
                BP.Currency = RC.Fields.Item("U_ST_CURRENCY").Value.ToString();
                BP.UserFields.Fields.Item("U_ST_CUSTOMER_TYPE").Value = "C";

                foreach (string OneFld in UDF.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries))
                {
                    if (OneFld == "U_ST_TITLE")
                        BP.UserFields.Fields.Item("U_ST_JOB_TITLE").Value = RC.Fields.Item("U_ST_TITLE").Value.ToString();
                    else
                        BP.UserFields.Fields.Item(OneFld).Value = RC.Fields.Item(OneFld).Value.ToString();
                }
            }
            else if (UDO_Info.KHCF_Object == KHCF_Objects.CCI_Corporate_Member_Card)
            {
                string UDF = "U_ST_CHANNEL,U_ST_SUB_CHANNEL,U_ST_BROKER,U_ST_CORPORATE_NATIONAL_ID,U_ST_ACCOUNT_MANAGER,U_ST_SECTOR,U_ST_GENERAL_MANAGER";
                string SQL = $@"Select U_ST_TEL1, U_ST_CURRENCY , U_ST_TEL_2, U_ST_EMAIL, U_ST_CORPORATE_ARABIC_NAME, U_ST_CORPORATE_ENGLISH_NAME,U_ST_REMARK,{UDF}
FROM ""@{UDO_Info.Table_Name}"" WHERE ""Code"" = '{UDO_Code}'";
                Recordset RC = Helper.Utility.Execute_Recordset_Query(company, SQL);

                BP.CardName = RC.Fields.Item("U_ST_CORPORATE_ARABIC_NAME").Value.ToString();
                BP.CardForeignName = RC.Fields.Item("U_ST_CORPORATE_ENGLISH_NAME").Value.ToString();
                BP.Phone1 = RC.Fields.Item("U_ST_TEL1").Value.ToString();
                BP.Phone2 = RC.Fields.Item("U_ST_TEL_2").Value.ToString();
                BP.EmailAddress = RC.Fields.Item("U_ST_EMAIL").Value.ToString();
                BP.FreeText = RC.Fields.Item("U_ST_REMARK").Value.ToString();
                BP.UserFields.Fields.Item("U_ST_BROKER1").Value = RC.Fields.Item("U_ST_BROKER").Value.ToString();
                BP.UserFields.Fields.Item("U_ST_MAIN_SECTOR").Value = RC.Fields.Item("U_ST_SECTOR").Value.ToString();
                BP.UserFields.Fields.Item("U_ST_CHANNEL").Value = RC.Fields.Item("U_ST_CHANNEL").Value.ToString();
                BP.UserFields.Fields.Item("U_ST_SUB_CHANNEL").Value = RC.Fields.Item("U_ST_SUB_CHANNEL").Value.ToString();
                BP.UserFields.Fields.Item("U_ST_CORPORATE_NATIONAL_ID").Value = RC.Fields.Item("U_ST_CORPORATE_NATIONAL_ID").Value.ToString();
                BP.UserFields.Fields.Item("U_ST_GENERAL_MANAGER").Value = RC.Fields.Item("U_ST_GENERAL_MANAGER").Value.ToString();
                BP.UserFields.Fields.Item("U_ST_ACCOUNT_MANAGER").Value = RC.Fields.Item("U_ST_ACCOUNT_MANAGER").Value.ToString();
                BP.Currency = RC.Fields.Item("U_ST_CURRENCY").Value.ToString();
                BP.UserFields.Fields.Item("U_ST_CUSTOMER_TYPE").Value = "C";

            }
            else if (UDO_Info.KHCF_Object == KHCF_Objects.Fundraising_Individual_Card)
            {
                string UDF = $@"U_ST_DATE_OF_BIRTH,U_ST_FIRST_NAME_AR,U_ST_FATHER_NAME_AR,U_ST_MIDDLE_NAME_AR,U_ST_SURNAME_AR,U_ST_FIRST_NAME_EN,U_ST_FATHER_NAME_EN,U_ST_MIDDLE_NAME_EN,U_ST_SURNAME_EN,U_ST_ACCOUNT_MANAGER,U_ST_NATIONALITY,U_ST_JOB_TITLE,U_ST_RESIDENCY";

                string SQL = $@"Select {UDF}, U_ST_TEL1, U_ST_MOBILE_1, U_ST_MOBILE_2, U_ST_EMAIL,U_ST_FULL_NAME_AR,U_ST_FULL_NAME_EN,
U_ST_LEAD_ADD_UPDATE, U_ST_DONOR_ADD_UPDATE
FROM ""@{UDO_Info.Table_Name}"" WHERE ""Code"" = '{UDO_Code}'";

                Recordset RC = Helper.Utility.Execute_Recordset_Query(company, SQL);

                BP.CardName = RC.Fields.Item("U_ST_FULL_NAME_AR").Value.ToString();
                BP.CardForeignName = RC.Fields.Item("U_ST_FULL_NAME_EN").Value.ToString();
                BP.Phone1 = RC.Fields.Item("U_ST_TEL1").Value.ToString();
                BP.Phone2 = RC.Fields.Item("U_ST_MOBILE_2").Value.ToString();
                BP.Cellular = RC.Fields.Item("U_ST_MOBILE_1").Value.ToString();
                BP.EmailAddress = RC.Fields.Item("U_ST_EMAIL").Value.ToString();
                BP.UserFields.Fields.Item("U_ST_CUSTOMER_TYPE").Value = "F";
                BP.UserFields.Fields.Item("U_ST_DATA_SOURCE").Value = "008";

                bool isLead = RC.Fields.Item("U_ST_LEAD_ADD_UPDATE").Value.ToString() == "Y";
                bool isDonor = RC.Fields.Item("U_ST_DONOR_ADD_UPDATE").Value.ToString() == "Y";
                if (isDonor)
                {
                    BP.CardType = BoCardTypes.cCustomer;
                }
                else if (isLead)
                {
                    BP.CardType = BoCardTypes.cLid;
                }
                


                string X = RC.Fields.Item("U_ST_RESIDENCY").Value.ToString();
                foreach (string OneFld in UDF.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries))
                {
                    if (OneFld == "U_ST_RESIDENCY" && X == "")
                    {
                        continue;
                    }
                    BP.UserFields.Fields.Item(OneFld).Value = RC.Fields.Item(OneFld).Value.ToString();
                }

            }
            else if (UDO_Info.KHCF_Object == KHCF_Objects.Fundraising_Corporate_Card)
            {
                string SQL = $@"Select U_ST_TEL_1, U_ST_TEL_2, U_ST_MOBILE_1, U_ST_EMAIL, U_ST_COMPANY_ARABIC_NAME, U_ST_COMPANY_ENGLISH_NAME
FROM ""@{UDO_Info.Table_Name}"" WHERE ""Code"" = '{UDO_Code}'";
                Recordset RC = Helper.Utility.Execute_Recordset_Query(company, SQL);

                BP.CardName = RC.Fields.Item("U_ST_COMPANY_ARABIC_NAME").Value.ToString();
                BP.CardForeignName = RC.Fields.Item("U_ST_COMPANY_ENGLISH_NAME").Value.ToString();
                BP.Phone1 = RC.Fields.Item("U_ST_TEL_1").Value.ToString();
                BP.Phone2 = RC.Fields.Item("U_ST_TEL_2").Value.ToString();
                BP.Cellular = RC.Fields.Item("U_ST_MOBILE_1").Value.ToString();
                BP.EmailAddress = RC.Fields.Item("U_ST_EMAIL").Value.ToString();
                BP.UserFields.Fields.Item("U_ST_CUSTOMER_TYPE").Value = "F";
                BP.UserFields.Fields.Item("U_ST_DATA_SOURCE").Value = "008";


            }
            else if (UDO_Info.KHCF_Object == KHCF_Objects.Patients_Card)
            {
                string SQL = $@"Select U_ST_TEL1, U_ST_TEL2, U_ST_FULL_ARABIC_NAME, U_ST_FULL_ENGLISH_NAME
FROM ""@{UDO_Info.Table_Name}"" WHERE ""Code"" = '{UDO_Code}'";
                Recordset RC = Helper.Utility.Execute_Recordset_Query(company, SQL);

                BP.CardName = RC.Fields.Item("U_ST_FULL_ARABIC_NAME").Value.ToString();
                BP.CardForeignName = RC.Fields.Item("U_ST_FULL_ENGLISH_NAME").Value.ToString();
                BP.Phone1 = RC.Fields.Item("U_ST_TEL1").Value.ToString();
                BP.Phone2 = RC.Fields.Item("U_ST_TEL2").Value.ToString();
                //BP.Cellular = RC.Fields.Item("U_ST_MOBILE_1").Value.ToString();
                //BP.EmailAddress = RC.Fields.Item("U_ST_EMAIL").Value.ToString();

            }
            else
            {
                throw new Logic.Custom_Exception($"The Card category[{UDO_Info.KHCF_Object}] is not supported to update the BP");
            }
            string Address_Table_Name;
            if (UDO_Info.Table_Name.Contains("CCI_INDIV"))
            {
                Address_Table_Name = "ST_CCI_INDIV_ADDR";
            }
            else if (UDO_Info.Table_Name.Contains("FUND_INDIV"))
            {
                Address_Table_Name = "ST_FUND_INDIV_ADDR";
            }
            else if (UDO_Info.Table_Name.Contains("FUND_CORP"))
            {
                Address_Table_Name = "ST_FUND_CORP_ADDR";
            }
            else
            {
                Address_Table_Name = "ST_CCI_CORP_ADDR";
            }

            string SQL_Address = $@"SELECT T0.""U_ST_ADDRESS_NAME"", T0.""U_ST_STREET"", T0.""U_ST_BLOCK"", T0.""U_ST_ZIP_CODE"", T0.""U_ST_CITY""
, T0.""U_ST_COUNTY"",  T0.""U_ST_STATE"", T0.""U_ST_BUILDING"", T0.""U_ST_ADDRESS_TYPE"", T0.""U_ST_ADDRESS_NAME_2""
, T0.""U_ST_ADDRESS_NAME_3"", T0.""U_ST_STREET_NO""
,(Select T1.""Code"" From OCRY T1 Where T1.""Name"" = U_ST_COUNTRY) As U_ST_COUNTRY 
FROM ""@{Address_Table_Name}"" T0 WHERE ""Code"" = '{UDO_Code}' ";
            Recordset RC_Addr = Helper.Utility.Execute_Recordset_Query(company, SQL_Address);

            List<BpAddress> UDO_Addresses = new List<BpAddress>();
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

                UDO_Addresses.Add(address);

                RC_Addr.MoveNext();
            }
            Tuple<string, string>[] UDO_AddressNames = UDO_Addresses.Select(X => new Tuple<string, string>(X.AddressName, X.AddressType)).ToArray();
            List<Tuple<string, string>> BP_AddressNames = new List<Tuple<string, string>>();
            for (int i = BP.Addresses.Count - 1; i >= 0; i--)
            {
                BP.Addresses.SetCurrentLine(i);

                string Address_Name = BP.Addresses.AddressName;
                string Address_Type = BP.Addresses.AddressType == BoAddressType.bo_ShipTo ? "S" : "B";
                if (UDO_AddressNames.Count(A => A.Item1 == Address_Name && A.Item2 == Address_Type) == 0)
                {
                    BP.Addresses.Delete();
                }

                BP_AddressNames.Add(new Tuple<string, string>(Address_Name, Address_Type));
            }
            foreach (var One_UDO_Address in UDO_Addresses)
            {
                bool Is_New = false;
                if (BP_AddressNames.Count(A => A.Item1 == One_UDO_Address.AddressName && A.Item2 == One_UDO_Address.AddressType) == 0)
                {
                    if (BP.Addresses.AddressName != "")
                    {
                        BP.Addresses.Add();
                    }
                    Is_New = true;
                }
                else
                {
                    for (int i = 0; i < BP.Addresses.Count; i++)
                    {
                        BP.Addresses.SetCurrentLine(i);
                        if (One_UDO_Address.AddressName == BP.Addresses.AddressName && One_UDO_Address.AddressType ==(BP.Addresses.AddressType == BoAddressType.bo_ShipTo ? "S" : "B"))
                        {
                            break;
                        }
                    }
                }
                if (Is_New == true)
                {
                    BP.Addresses.AddressName = One_UDO_Address.AddressName;
                    if (One_UDO_Address.AddressType == "S")
                    {
                        BP.Addresses.AddressType = BoAddressType.bo_ShipTo;
                    }
                    if (One_UDO_Address.AddressType == "B")
                    {
                        BP.Addresses.AddressType = BoAddressType.bo_BillTo;
                    }
                }
                BP.Addresses.City = One_UDO_Address.City;
                BP.Addresses.Street = One_UDO_Address.Street;
                BP.Addresses.Country = One_UDO_Address.Country;
                BP.Addresses.Block = One_UDO_Address.Block;
                //BP.Addresses.ZipCode = One_UDO_Address.ZipCode;
                //BP.Addresses.County = One_UDO_Address.County;
                BP.Addresses.BuildingFloorRoom = One_UDO_Address.BuildingFloorRoom;
                //BP.Addresses.AddressName2 = One_UDO_Address.AddressName2;
                //BP.Addresses.AddressName3 = One_UDO_Address.AddressName3;
                BP.Addresses.StreetNo = One_UDO_Address.StreetNo;
            }
            //var x = BP.CardType;
            if (BP.Update() != 0)
            {
                throw new Logic.Custom_Exception($"Business Partner[{BP_Code}][{company.GetLastErrorDescription()}]");
            }
        }

        internal static string Add_OR_Update_CCI_Card_BP(Company company, string BP_Code, KHCF_BP BP_Data, string UDO_Code, UDO_Definition UDO_Info, string Control_Account = null)
        {
            BusinessPartners BP = (BusinessPartners)company.GetBusinessObject(BoObjectTypes.oBusinessPartners);
            bool IsBPNew = true;
            if (string.IsNullOrEmpty(BP_Code))
            {
                int Series_ID = Utility.Get_Series_By_BP_Group(company, BP_Data.BP_Group);
                BP.Series = Series_ID;
                BP.GroupCode = BP_Data.BP_Group;
                BP.CardName = BP_Data.CardName;
                BP.Currency = BP_Data.Currency;
                BP.UserFields.Fields.Item("U_ST_MEMBER_CARD").Value = BP_Data.MemberCard_Code;
                if (BP_Data.Is_Lead)
                {
                    BP.CardType = BoCardTypes.cLid;
                }
                if (BP_Data.Is_Vendor)
                {
                    BP.CardType = BoCardTypes.cSupplier;
                }
                if (!string.IsNullOrEmpty(BP_Data.FatherCode))
                {
                    BP.FatherCard = BP_Data.FatherCode;
                    BP.FatherType = BP_Data.FatherType;
                }
                BP.EmailAddress = BP_Data.Email;
                BP.Cellular = BP_Data.Mobile;
                if (BP_Data.addresses.Count > 0)
                {
                    for (int i = 0; i < BP_Data.addresses.Count; i++)
                    {
                        BP.Addresses.AddressName = BP_Data.addresses[i].AddressName;
                        BP.Addresses.City = BP_Data.addresses[i].City;
                        BP.Addresses.Street = BP_Data.addresses[i].Street;
                        BP.Addresses.Country = BP_Data.addresses[i].Country;
                        if (BP_Data.addresses[i].AddressType == "S")
                        {
                            BP.Addresses.AddressType = BoAddressType.bo_ShipTo;
                        }
                        if (BP_Data.addresses[i].AddressType == "B")
                        {
                            BP.Addresses.AddressType = BoAddressType.bo_BillTo;
                        }
                        BP.Addresses.Block = BP_Data.addresses[i].Block;
                        BP.Addresses.ZipCode = BP_Data.addresses[i].ZipCode;
                        BP.Addresses.County = BP_Data.addresses[i].County;
                        BP.Addresses.BuildingFloorRoom = BP_Data.addresses[i].BuildingFloorRoom;
                        BP.Addresses.AddressName2 = BP_Data.addresses[i].AddressName2;
                        BP.Addresses.AddressName3 = BP_Data.addresses[i].AddressName3;
                        BP.Addresses.StreetNo = BP_Data.addresses[i].StreetNo;
                        BP.Addresses.Add();
                    }
                }
                if (BP_Data.contacts.Count > 0)
                {
                    for (int i = 0; i < BP_Data.contacts.Count; i++)
                    {
                        if (string.IsNullOrEmpty(BP_Data.contacts[i].Name))
                            break;
                        BP.ContactEmployees.Name = BP_Data.contacts[i].Name;
                        BP.ContactEmployees.Position = BP_Data.contacts[i].Position;
                        BP.ContactEmployees.Address = BP_Data.contacts[i].Address;
                        BP.ContactEmployees.Phone1 = BP_Data.contacts[i].Tel_1;
                        BP.ContactEmployees.Phone2 = BP_Data.contacts[i].Tel_2;
                        BP.ContactEmployees.MobilePhone = BP_Data.contacts[i].Mobile;
                        BP.ContactEmployees.EmailGroupCode = "";
                        BP.ContactEmployees.E_Mail = BP_Data.contacts[i].Email;
                        BP.ContactEmployees.Pager = BP_Data.contacts[i].Pager;
                        BP.ContactEmployees.Remarks1 = BP_Data.contacts[i].Remarks_1;
                        BP.ContactEmployees.Remarks2 = BP_Data.contacts[i].Remarks_2;
                        BP.ContactEmployees.Password = BP_Data.contacts[i].Password;
                        BP.ContactEmployees.PlaceOfBirth = "";
                        if (BP_Data.contacts[i].Gender == "M")
                            BP.ContactEmployees.Gender = BoGenderTypes.gt_Male;
                        else
                            BP.ContactEmployees.Gender = BoGenderTypes.gt_Female;
                        BP.ContactEmployees.DateOfBirth = Convert.ToDateTime(BP_Data.contacts[i].DateOfBirth);
                        BP.ContactEmployees.Profession = BP_Data.contacts[i].Profession;
                        BP.ContactEmployees.CityOfBirth = BP_Data.contacts[i].CityOfBirth;
                        BP.ContactEmployees.ConnectedAddressName = BP_Data.contacts[i].ConnectedAddress;
                        BP.ContactEmployees.Fax = BP_Data.contacts[i].Mobile;

                        BP.ContactEmployees.Add();
                    }
                }
                if (BP_Data.attatchments.Count > 0)
                {
                    SAPbobsCOM.Attachments2 oATT = company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oAttachments2) as SAPbobsCOM.Attachments2;
                    for (int i = 0; i < BP_Data.attatchments.Count; i++)
                    {
                        if (string.IsNullOrEmpty(BP_Data.attatchments[i].FileName))
                            break;
                        oATT.Lines.Add();
                        oATT.Lines.FileName = BP_Data.attatchments[i].FileName;
                        oATT.Lines.FileExtension = System.IO.Path.GetExtension(BP_Data.attatchments[i].FileName).Substring(1);
                    }
                    int iAttEntry = -1;
                    if (oATT.Add() == 0)
                    {
                        iAttEntry = int.Parse(company.GetNewObjectKey());
                        BP.AttachmentEntry = iAttEntry;
                    }
                    else
                    {
                        string s = company.GetLastErrorDescription();
                    }
                }
            }
            if (!string.IsNullOrEmpty(BP_Code))
            {
                BP.GetByKey(BP_Code);
                IsBPNew = false;
            }

            if (UDO_Info.KHCF_Object == KHCF_Objects.CCI_Member_Card)
            {
                string UDF = "U_ST_DATE_OF_BIRTH,U_ST_SUB_CHANNEL,U_ST_BROKER1,U_ST_NATIONAL_ID,U_ST_PERSONAL_ID,U_ST_PASSPORT_ID,U_ST_FATHER_NAME_AR,U_ST_MIDDLE_NAME_AR,U_ST_SURNAME_AR,U_ST_FIRST_NAME_EN,U_ST_FATHER_NAME_EN,U_ST_MIDDLE_NAME_EN,U_ST_SURNAME_EN,U_ST_FULL_NAME_EN,U_ST_ACCOUNT_MANAGER,U_ST_NATIONALITY";

                string SQL = $@"Select U_ST_TEL1, U_ST_TEL2, U_ST_EMAIL, U_ST_FULL_NAME_AR,U_ST_ACCOUNT_MANAGER U_ST_FULL_NAME_EN, U_ST_MOBILE, U_ST_CUSTOMER_GROUP ,{UDF}
FROM ""@{UDO_Info.Table_Name}"" WHERE ""Code"" = '{UDO_Code}'";
                Recordset RC = Helper.Utility.Execute_Recordset_Query(company, SQL);

                BP.CardName = RC.Fields.Item("U_ST_FULL_NAME_AR").Value.ToString();
                BP.CardForeignName = RC.Fields.Item("U_ST_FULL_NAME_EN").Value.ToString();
                BP.Phone1 = RC.Fields.Item("U_ST_TEL1").Value.ToString();
                BP.Phone2 = RC.Fields.Item("U_ST_TEL2").Value.ToString();
                BP.Cellular = RC.Fields.Item("U_ST_MOBILE").Value.ToString();
                BP.EmailAddress = RC.Fields.Item("U_ST_EMAIL").Value.ToString();
                BP.SalesPersonCode = Convert.ToInt32(RC.Fields.Item("U_ST_ACCOUNT_MANAGER").Value);
                BP.GroupCode = Convert.ToInt32(RC.Fields.Item("U_ST_CUSTOMER_GROUP").Value);
                BP.UserFields.Fields.Item("U_ST_CUSTOMER_TYPE").Value = "C";

                string SQL_Group = $@"SELECT T0.""U_ST_GL_ACCOUNT""  FROM OCRG T0 where T0.""GroupCode""='{BP.GroupCode}'";
                Recordset RC_Group = Helper.Utility.Execute_Recordset_Query(company, SQL_Group);
                if (RC_Group.RecordCount > 0)
                {
                    BP.DebitorAccount = RC_Group.Fields.Item("U_ST_GL_ACCOUNT").Value.ToString();
                }

                foreach (string OneFld in UDF.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries))
                {
                    if (OneFld == "U_ST_TITLE")
                        BP.UserFields.Fields.Item("U_ST_JOB_TITLE").Value = RC.Fields.Item("U_ST_TITLE").Value.ToString();
                    else
                        BP.UserFields.Fields.Item(OneFld).Value = RC.Fields.Item(OneFld).Value.ToString();
                }

            }
            else if (UDO_Info.KHCF_Object == KHCF_Objects.CCI_Corporate_Member_Card)
            {
                string UDF = "U_ST_CHANNEL,U_ST_SUB_CHANNEL,U_ST_BROKER,U_ST_CORPORATE_NATIONAL_ID,U_ST_ACCOUNT_MANAGER,U_ST_SECTOR,U_ST_GENERAL_MANAGER";
                string SQL = $@"Select U_ST_TEL1, U_ST_TEL_2, U_ST_EMAIL, U_ST_CORPORATE_ARABIC_NAME, U_ST_CORPORATE_ENGLISH_NAME,U_ST_REMARK,{UDF}
FROM ""@{UDO_Info.Table_Name}"" WHERE ""Code"" = '{UDO_Code}'";
                Recordset RC = Helper.Utility.Execute_Recordset_Query(company, SQL);

                BP.CardName = RC.Fields.Item("U_ST_CORPORATE_ARABIC_NAME").Value.ToString();
                BP.CardForeignName = RC.Fields.Item("U_ST_CORPORATE_ENGLISH_NAME").Value.ToString();
                BP.Phone1 = RC.Fields.Item("U_ST_TEL1").Value.ToString();
                BP.Phone2 = RC.Fields.Item("U_ST_TEL_2").Value.ToString();
                BP.EmailAddress = RC.Fields.Item("U_ST_EMAIL").Value.ToString();
                BP.FreeText = RC.Fields.Item("U_ST_REMARK").Value.ToString();

                foreach (string OneFld in UDF.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries))
                {
                    if (OneFld.Contains("U_ST_BROKER"))
                    {
                        BP.UserFields.Fields.Item("U_ST_BROKER1").Value = RC.Fields.Item("U_ST_BROKER").Value.ToString();
                    }
                    else if (OneFld.Contains("U_ST_SECTOR"))
                    {
                        BP.UserFields.Fields.Item("U_ST_MAIN_SECTOR").Value = RC.Fields.Item("U_ST_SECTOR").Value.ToString();
                    }
                    else
                    {
                        BP.UserFields.Fields.Item(OneFld).Value = RC.Fields.Item(OneFld).Value.ToString();
                    }
                }
                BP.UserFields.Fields.Item("U_ST_CUSTOMER_TYPE").Value = "C";
            }
            else if (UDO_Info.KHCF_Object == KHCF_Objects.Fundraising_Individual_Card)
            {
                string SQL = $@"Select U_ST_TEL1, U_ST_MOBILE_1, U_ST_MOBILE_2, U_ST_EMAIL, U_ST_FULL_NAME_AR, U_ST_FULL_NAME_EN
FROM ""@{UDO_Info.Table_Name}"" WHERE ""Code"" = '{UDO_Code}'";
                Recordset RC = Helper.Utility.Execute_Recordset_Query(company, SQL);

                BP.CardName = RC.Fields.Item("U_ST_FULL_NAME_AR").Value.ToString();
                BP.CardForeignName = RC.Fields.Item("U_ST_FULL_NAME_EN").Value.ToString();
                BP.Phone1 = RC.Fields.Item("U_ST_TEL1").Value.ToString();
                BP.Phone2 = RC.Fields.Item("U_ST_MOBILE_2").Value.ToString();
                BP.Cellular = RC.Fields.Item("U_ST_MOBILE_1").Value.ToString();
                BP.EmailAddress = RC.Fields.Item("U_ST_EMAIL").Value.ToString();

            }
            else if (UDO_Info.KHCF_Object == KHCF_Objects.Fundraising_Corporate_Card)
            {
                string SQL = $@"Select U_ST_TEL1, U_ST_TEL2, U_ST_MOBILE_1, U_ST_EMAIL, U_ST_COMMERCIAL_ARABIC_NAME, U_ST_COMMERCIAL_ENGLISH_NAME
FROM ""@{UDO_Info.Table_Name}"" WHERE ""Code"" = '{UDO_Code}'";
                Recordset RC = Helper.Utility.Execute_Recordset_Query(company, SQL);

                BP.CardName = RC.Fields.Item("U_ST_COMMERCIAL_ARABIC_NAME").Value.ToString();
                BP.CardForeignName = RC.Fields.Item("U_ST_COMMERCIAL_ENGLISH_NAME").Value.ToString();
                BP.Phone1 = RC.Fields.Item("U_ST_TEL1").Value.ToString();
                BP.Phone2 = RC.Fields.Item("U_ST_TEL2").Value.ToString();
                BP.Cellular = RC.Fields.Item("U_ST_MOBILE_1").Value.ToString();
                BP.EmailAddress = RC.Fields.Item("U_ST_EMAIL").Value.ToString();

            }
            else if (UDO_Info.KHCF_Object == KHCF_Objects.Patients_Card)
            {
                string SQL = $@"Select U_ST_TEL1, U_ST_TEL2, U_ST_FULL_ARABIC_NAME, U_ST_FULL_ENGLISH_NAME
FROM ""@{UDO_Info.Table_Name}"" WHERE ""Code"" = '{UDO_Code}'";
                Recordset RC = Helper.Utility.Execute_Recordset_Query(company, SQL);

                BP.CardName = RC.Fields.Item("U_ST_FULL_ARABIC_NAME").Value.ToString();
                BP.CardForeignName = RC.Fields.Item("U_ST_FULL_ENGLISH_NAME").Value.ToString();
                BP.Phone1 = RC.Fields.Item("U_ST_TEL1").Value.ToString();
                BP.Phone2 = RC.Fields.Item("U_ST_TEL2").Value.ToString();
                //BP.Cellular = RC.Fields.Item("U_ST_MOBILE_1").Value.ToString();
                //BP.EmailAddress = RC.Fields.Item("U_ST_EMAIL").Value.ToString();

            }
            else
            {
                throw new Logic.Custom_Exception($"The Card category[{UDO_Info.KHCF_Object}] is not supported to update the BP.");
            }
            string Address_Table_Name;
            if (UDO_Info.Table_Name.Contains("INDIV"))
            {
                Address_Table_Name = "ST_CCI_INDIV_ADDR";
            }
            else
            {
                Address_Table_Name = "ST_CCI_CORP_ADDR";
            }
            string SQL_Address = $@"SELECT T0.""U_ST_ADDRESS_NAME"", T0.""U_ST_STREET"", T0.""U_ST_BLOCK"", T0.""U_ST_ZIP_CODE"", T0.""U_ST_CITY""
, T0.""U_ST_COUNTY"",  T0.""U_ST_STATE"", T0.""U_ST_BUILDING"", T0.""U_ST_ADDRESS_TYPE"", T0.""U_ST_ADDRESS_NAME_2""
, T0.""U_ST_ADDRESS_NAME_3"", T0.""U_ST_STREET_NO""
,(Select T1.""Code"" From OCRY T1 Where T1.""Name"" = T0.""U_ST_COUNTRY"") As ""U_ST_COUNTRY"" 
FROM ""@{Address_Table_Name}"" T0 WHERE ""Code"" = '{UDO_Code}' ";
            Recordset RC_Addr = Helper.Utility.Execute_Recordset_Query(company, SQL_Address);

            List<BpAddress> UDO_Addresses = new List<BpAddress>();
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
                UDO_Addresses.Add(address);
                RC_Addr.MoveNext();
            }
            Tuple<string, string>[] UDO_AddressNames = UDO_Addresses.Select(X => new Tuple<string, string>(X.AddressName, X.AddressType)).ToArray();
            List<Tuple<string, string>> BP_AddressNames = new List<Tuple<string, string>>();
            for (int i = BP.Addresses.Count - 1; i >= 0; i--)
            {
                BP.Addresses.SetCurrentLine(i);
                string Address_Name = BP.Addresses.AddressName;
                string Address_Type = BP.Addresses.AddressType == BoAddressType.bo_ShipTo ? "S" : "B";
                if (UDO_AddressNames.Count(A => A.Item1 == Address_Name && A.Item2 == Address_Type) == 0)
                {
                    BP.Addresses.Delete();
                }
                BP_AddressNames.Add(new Tuple<string, string>(Address_Name, Address_Type));
            }
            foreach (var One_UDO_Address in UDO_Addresses)
            {
                bool Is_New = false;
                if (BP_AddressNames.Count(A => A.Item1 == One_UDO_Address.AddressName && A.Item2 == One_UDO_Address.AddressType) == 0)
                {
                    if (BP.Addresses.AddressName != "")
                    {
                        BP.Addresses.Add();
                    }
                    Is_New = true;
                }
                else
                {
                    for (int i = 0; i < BP.Addresses.Count; i++)
                    {
                        BP.Addresses.SetCurrentLine(i);
                        if (One_UDO_Address.AddressName == BP.Addresses.AddressName && One_UDO_Address.AddressType == (BP.Addresses.AddressType == BoAddressType.bo_ShipTo ? "S" : "B"))
                        {
                            break;
                        }
                    }
                }
                if (Is_New == true)
                {
                    BP.Addresses.AddressName = One_UDO_Address.AddressName;
                    if (One_UDO_Address.AddressType == "S")
                    {
                        BP.Addresses.AddressType = BoAddressType.bo_ShipTo;
                    }
                    if (One_UDO_Address.AddressType == "B")
                    {
                        BP.Addresses.AddressType = BoAddressType.bo_BillTo;
                    }
                }
                BP.Addresses.City = One_UDO_Address.City;
                BP.Addresses.Street = One_UDO_Address.Street;
                BP.Addresses.Country = One_UDO_Address.Country;
                BP.Addresses.Block = One_UDO_Address.Block;
                //BP.Addresses.ZipCode = One_UDO_Address.ZipCode;
                //BP.Addresses.County = One_UDO_Address.County;
                BP.Addresses.BuildingFloorRoom = One_UDO_Address.BuildingFloorRoom;
                //BP.Addresses.AddressName2 = One_UDO_Address.AddressName2;
                //BP.Addresses.AddressName3 = One_UDO_Address.AddressName3;
                BP.Addresses.StreetNo = One_UDO_Address.StreetNo;
            }

            if (IsBPNew)
                if (BP.Add() != 0)
                    throw new Logic.Custom_Exception($"Error during creating the Business Partner[{BP_Code}][{company.GetLastErrorDescription()}]");
                else
                {
                    string BPCode;
                    company.GetNewObjectCode(out BPCode);
                    return BPCode;
                }
            else
            {
                if (BP.Update() != 0)
                    throw new Logic.Custom_Exception($"Error during updating the Business Partner[{BP_Code}][{company.GetLastErrorDescription()}]");
                return BP_Code;
            }
        }

        internal static void Update_UDO(Company company, UDO_Definition UDO_Info, string UDO_Entry_Code, Field_Data[] Field_Datas)
        {
            try
            {
                CompanyService oCmpSrv = company.GetCompanyService();
                GeneralService oGeneralService = oCmpSrv.GetGeneralService(Utility.Get_UDO_Type_ID(UDO_Info.KHCF_Object));
                GeneralDataParams oGeneralParams = (GeneralDataParams)oGeneralService.GetDataInterface(GeneralServiceDataInterfaces.gsGeneralDataParams);
                oGeneralParams.SetProperty("Code", UDO_Entry_Code);
                GeneralData oGeneralData = oGeneralService.GetByParams(oGeneralParams);
                foreach (Field_Data OneField in Field_Datas)
                {
                    if (OneField.Is_Temp == true)
                    {
                        continue;
                    }
                    oGeneralData.SetProperty(OneField.Field_Name, OneField.Value);
                }

                oGeneralService.Update(oGeneralData);

            }
            catch (Exception ex)
            {
                throw new Logic.Custom_Exception($"Error during update the UDO[{UDO_Info.Table_Name}], Code[{UDO_Entry_Code}][{ex.Message}]");
            }
        }

        internal static string Add_UDO_Entry(Company company, UDO_Definition UDO_Info, Field_Data[] Field_Datas, UDO_Data[] Lines_Data = null)
        {
            try
            {
                CompanyService oCmpSrv = company.GetCompanyService();
                string UDO_Code = Utility.Get_UDO_Type_ID(UDO_Info.KHCF_Object); 
                GeneralService oGeneralService = oCmpSrv.GetGeneralService(Utility.Get_UDO_Type_ID(UDO_Info.KHCF_Object));
                GeneralDataParams oGeneralParams = (GeneralDataParams)oGeneralService.GetDataInterface(GeneralServiceDataInterfaces.gsGeneralDataParams);
                string UDO_Entry_Code = Utility.Get_New_UDO_Code(company, UDO_Info.KHCF_Object);
                //oGeneralParams.SetProperty("Code", UDO_Entry_Code);
                //GeneralData oGeneralData = oGeneralService.GetByParams(oGeneralParams);
                GeneralData oGeneralData = (GeneralData)oGeneralService.GetDataInterface(SAPbobsCOM.GeneralServiceDataInterfaces.gsGeneralData);
                oGeneralData.SetProperty("Code", UDO_Entry_Code);
                //oGeneralData.SetProperty("Name", UDO_Entry_Code);
                string UDO_Unique_ID;
                foreach (Field_Data OneField in Field_Datas.Where(x => !string.IsNullOrEmpty(x.Value.ToString()) || !x.Value.ToString().Contains('?')))
                {
                    if (OneField.Is_Temp == true)
                    {
                        continue;
                    }
                    if(OneField.Value != null)
                        oGeneralData.SetProperty(OneField.Field_Name, OneField.Value.ToString());
                }

                if (Lines_Data != null)
                {
                    foreach (var OneLine in Lines_Data)
                    {
                        SAPbobsCOM.GeneralDataCollection oChildren = oGeneralData.Child(OneLine.UDO_Info.Table_Name);
                        for (int J = 0; J < OneLine.Fields_Data_Lines.Count; J++)
                        {
                            List < Field_Data > OneLineData = OneLine.Fields_Data_Lines[J];
                            SAPbobsCOM.GeneralData oChild = oChildren.Add();
                            foreach (Field_Definition OneField in OneLine.Fields_Definition)
                            {
                                if (OneField.Is_Temp == true)
                                {
                                    continue;
                                }
                                oChild.SetProperty(OneField.Column_Name_In_DB, OneLineData.FirstOrDefault(F => F.Column_Name_In_DB == OneField.Column_Name_In_DB).Value);
                            }
                        }

                    }
                }

                GeneralDataParams New_Entry = oGeneralService.Add(oGeneralData);
                string NewCode = New_Entry.GetProperty("Code").ToString();
                // company.GetNewObjectCode(out NewCode);
                return NewCode;
            }
            catch (Exception ex)
            {
                throw new Logic.Custom_Exception($"Error during add the UDO[{UDO_Info.Table_Name}], [{ex.Message}]");
            }
        }

        internal static (bool IsExising, string Code, bool Is_Lead, string Lead_CardCode) National_ID_IsExisting(Company company, List<Field_Data> Line_Fields_Data, UDO_Definition Obj_Info)
        {
            if (Obj_Info.KHCF_Object == KHCF_Objects.CCI_Member_Card)
            {
                //U_ST_NATIONAL_ID
                Field_Data U_ST_NATIONALITY = Line_Fields_Data.FirstOrDefault(N => N.Column_Name_In_DB.Contains("U_ST_NATIONALITY"));
                if (U_ST_NATIONALITY == null)
                {
                    Field_Data Field_Code = Line_Fields_Data.FirstOrDefault(N => N.Column_Name_In_DB.Contains("Code"));
                    if (Field_Code == null)
                    {
                        throw new Logic.Custom_Exception("There is no Nationality or Code field");
                    }
                    else
                    {
                        string SQL_Code = $@"SELECT ""Code"" FROM ""@{Obj_Info.Table_Name}"" WHERE ""Code"" = '{Field_Code.Value}'";
                        Recordset RC_Code = Helper.Utility.Execute_Recordset_Query(company, SQL_Code);
                        return (RC_Code.RecordCount > 0, RC_Code.RecordCount > 0 ? RC_Code.Fields.Item("Code").Value.ToString() : "", false, "");
                    }
                    //throw new Logic.Custom_Exception("The U_ST_NATIONALITY field is missing");
                }
                string SQL;
                if (U_ST_NATIONALITY.Value.ToString() == "Jordan")
                {
                    Field_Data U_ST_NATIONAL_ID = Line_Fields_Data.FirstOrDefault(N => N.Column_Name_In_DB.Contains("U_ST_NATIONAL_ID"));
                    if (U_ST_NATIONAL_ID == null)
                    {
                        throw new Logic.Custom_Exception("The U_ST_NATIONAL_ID field is missing");
                    }
                    SQL = $@"SELECT ""Code"" FROM ""@ST_CCI_INDIV_CARD"" T0 
WHERE LOWER(T0.""U_ST_NATIONALITY"")='jordan' AND U_ST_NATIONAL_ID = '{U_ST_NATIONAL_ID.Value.ToString()}'";
                }
                else
                {
                    Field_Data U_ST_PERSONAL_ID = Line_Fields_Data.FirstOrDefault(N => N.Column_Name_In_DB.Contains("U_ST_PERSONAL_ID"));
                    if (U_ST_PERSONAL_ID == null)
                    {
                        throw new Logic.Custom_Exception("The U_ST_PERSONAL_ID field is missing");
                    }
                    Field_Data U_ST_PASSPORT_ID = Line_Fields_Data.FirstOrDefault(N => N.Column_Name_In_DB.Contains("U_ST_PASSPORT_ID"));
                    if (U_ST_PASSPORT_ID == null)
                    {
                        throw new Logic.Custom_Exception("The U_ST_PASSPORT_ID field is missing");
                    }
                    string foreignerID;
                    if (!string.IsNullOrEmpty(U_ST_PASSPORT_ID.Value.ToString()))
                        foreignerID = U_ST_PASSPORT_ID.Value.ToString();
                    else
                        foreignerID = U_ST_PERSONAL_ID.Value.ToString();

                    SQL = $@"SELECT ""Code"" FROM ""@ST_CCI_INDIV_CARD"" T0 
WHERE LOWER(T0.""U_ST_NATIONALITY"") <> 'jordan'
AND ( T0.U_ST_PERSONAL_ID = '{foreignerID}' OR T0.U_ST_PASSPORT_ID = '{foreignerID}' )";
                }

                Recordset RC = Helper.Utility.Execute_Recordset_Query(company, SQL);

                string SQL_Lead;
                if (U_ST_NATIONALITY.Value.ToString() == "Jordan")
                {
                    Field_Data U_ST_NATIONAL_ID = Line_Fields_Data.FirstOrDefault(N => N.Column_Name_In_DB.Contains("U_ST_NATIONAL_ID"));
                    SQL_Lead = $@"SELECT T0.""CardType"", T0.""CardCode"" FROM OCRD T0 
WHERE LOWER(T0.""U_ST_NATIONALITY"")='jordan' AND U_ST_NATIONAL_ID = '{U_ST_NATIONAL_ID.Value.ToString()}'";
                }
                else
                {
                    string foreignerID;
                    Field_Data U_ST_PERSONAL_ID = Line_Fields_Data.FirstOrDefault(N => N.Column_Name_In_DB.Contains("U_ST_PERSONAL_ID"));
                    Field_Data U_ST_PASSPORT_ID = Line_Fields_Data.FirstOrDefault(N => N.Column_Name_In_DB.Contains("U_ST_PASSPORT_ID"));

                    if (!string.IsNullOrEmpty(U_ST_PASSPORT_ID.Value.ToString()))
                        foreignerID = U_ST_PASSPORT_ID.Value.ToString();
                    else
                        foreignerID = U_ST_PERSONAL_ID.Value.ToString();

                    SQL_Lead = $@"SELECT T0.""CardType"", T0.""CardCode"" FROM OCRD T0 
WHERE LOWER(T0.""U_ST_NATIONALITY"") <> 'jordan'
AND ( T0.U_ST_PERSONAL_ID = '{foreignerID}' OR T0.U_ST_PASSPORT_ID = '{foreignerID}' )";
                }

                Recordset RC_Lead = Helper.Utility.Execute_Recordset_Query(company, SQL_Lead);
                int count1 = RC.RecordCount; int count2 = RC_Lead.RecordCount;
                return (RC.RecordCount > 0,
                    RC.RecordCount > 0 ? RC.Fields.Item(0).Value.ToString() : string.Empty,
                    RC_Lead.RecordCount > 0,
                    RC_Lead.RecordCount > 0 ? RC_Lead.Fields.Item("CardCode").Value.ToString() : string.Empty);
            }
            else if (Obj_Info.KHCF_Object == KHCF_Objects.Individual_Membership)
            {
                Field_Data Field_Code = Line_Fields_Data.FirstOrDefault(N => N.Column_Name_In_DB.Contains("Code"));
                if (Field_Code == null)
                {
                    return (false, "", false, "");
                }
                else
                {
                    string SQL = $@"SELECT ""Code"" FROM ""@{Obj_Info.Table_Name}"" WHERE ""Code"" = '{Field_Code.Value}'";
                    Recordset RC = Helper.Utility.Execute_Recordset_Query(company, SQL);
                    return (RC.RecordCount > 0, RC.RecordCount > 0 ? RC.Fields.Item("Code").Value.ToString() : "", false, "");
                }
            }

            Field_Data External_Key = Line_Fields_Data.FirstOrDefault(N => N.Column_Name_In_DB == Obj_Info.External_Key);
            if (External_Key == null)
            {
                throw new Logic.Custom_Exception($"The {Obj_Info.External_Key} field is missing");
            }


            if (Obj_Info.SQL_Existing_Query == "")
            {
                string SQL = $@"SELECT ""Code"" FROM ""@{Obj_Info.Table_Name}"" WHERE {Obj_Info.External_Key} = '{External_Key.Value}'";
                Recordset RC = Helper.Utility.Execute_Recordset_Query(company, SQL);

                return (RC.RecordCount > 0,
                    RC.RecordCount > 0 ? RC.Fields.Item(0).Value.ToString() : string.Empty, false, "");
            }
            else
            {
                string SQL = string.Format(Obj_Info.SQL_Existing_Query, External_Key.Value);
                Recordset RC = Helper.Utility.Execute_Recordset_Query(company, SQL);

                return (RC.RecordCount > 0,
                    RC.RecordCount > 0 ? RC.Fields.Item(0).Value.ToString() : string.Empty, false, "");
            }
        }

        internal static string Get_Code_Per_National_ID(Company company, object National_ID, UDO_Definition Obj_Info)
        {
            string SQL;
            if (Obj_Info.SQL_Existing_Query == "")
            {
                SQL = $@"SELECT ""Code"" FROM ""@{Obj_Info.Table_Name}"" WHERE {Obj_Info.External_Key} = '{National_ID}'";
            }
            else
            {
                SQL = string.Format(Obj_Info.SQL_Existing_Query, National_ID);
            }

            Recordset RC = Helper.Utility.Execute_Recordset_Query(company, SQL);

            return RC.Fields.Item("Code").Value.ToString();
        }

        internal static string Get_Field_Definition_Name(string Col_Name)
        {
            if (Col_Name.StartsWith("U_"))
            {
                return Col_Name.Substring(2);
            }
            else
            {
                return Col_Name;
            }

        }

        internal static string Get_BP_Code(Company company, string UDO_Code, UDO_Definition UDO_Info)
        {
            string SQL_BP = $@"SELECT U_ST_BP_CODE FROM ""@{UDO_Info.Table_Name}"" WHERE ""Code"" = '{UDO_Code}' ";
            Recordset RC_BP = Helper.Utility.Execute_Recordset_Query(company, SQL_BP);

            string BP_Code = RC_BP.Fields.Item("U_ST_BP_CODE").Value.ToString();

            return BP_Code;
        }

        internal static SAPbobsCOM.Documents Get_Membership_Invoice(Company company, string UDO_Code)
        {
            SAPbobsCOM.Documents Doc = null;
            string SQL_Invoice_Number = $@"SELECT T0.U_ST_INVOICE_NUMBER FROM ""@ST_INDIV_MEMBERSHIP""  T0 WHERE T0.""Code"" ='{UDO_Code}'";
            Recordset RC_Invoice_Number = Helper.Utility.Execute_Recordset_Query(company, SQL_Invoice_Number);
            if (RC_Invoice_Number.Fields.Item("U_ST_INVOICE_NUMBER").Value.ToString() == "")
            {
                return null;
            }
            string SQL = $@"SELECT T0.""DocEntry"" FROM OINV T0 WHERE T0.""U_ST_MEMBERSHIP_CODE"" = '{UDO_Code}'";
            Recordset RC = Helper.Utility.Execute_Recordset_Query(company, SQL);

            int DocEntry = 0;
            if (RC.RecordCount != 0)
            {
                Doc = (SAPbobsCOM.Documents)company.GetBusinessObject(BoObjectTypes.oInvoices);
                DocEntry = (int)RC.Fields.Item("DocEntry").Value;
            }
            else
            {
                SQL = $@"SELECT T0.""DocEntry"" FROM ODPI T0 WHERE T0.""U_ST_MEMBERSHIP_CODE"" = '{UDO_Code}'";
                RC = Helper.Utility.Execute_Recordset_Query(company, SQL);
                if (RC.RecordCount == 0)
                {
                    throw new Logic.Custom_Exception($"There is no Invoice/Down Payment for the Membership[{UDO_Code}]");
                }
                Doc = (SAPbobsCOM.Documents)company.GetBusinessObject(BoObjectTypes.oDownPayments);
                DocEntry = (int)RC.Fields.Item("DocEntry").Value;
            }

            Doc.GetByKey(DocEntry);

            return Doc;
        }

        internal static SAPbobsCOM.Documents Get_Corporate_Invoice(Company company, string UDO_Code)
        {
            SAPbobsCOM.Documents Doc = null;
            //string SQL_Invoice_Number = $@"SELECT T0.U_ST_INVOICE_NUMBER FROM ""@ST_CORP_MEMBERSHIP""  T0 WHERE T0.""Code"" ='{UDO_Code}'";
            string SQL_Invoice_Number = $@"SELECT T0.""Code"" ,  T0.U_ST_INVOICE_NUMBER FROM ""@ST_INDIV_MEMBERSHIP"" T0 
WHERE T0.""U_ST_PARENT_MEMBERSHIP_ID"" ='{UDO_Code}' AND T0.""U_ST_PARENT_MEMBERSHIP_TYPE"" = 'C'  ";
            Recordset RC_Invoice_Number = Helper.Utility.Execute_Recordset_Query(company, SQL_Invoice_Number);
            if (RC_Invoice_Number.RecordCount == 0)
                return null;

            string indivMembershipCode = RC_Invoice_Number.Fields.Item("Code").Value.ToString();
            string SQL = $@"SELECT T0.""DocEntry"" FROM OINV T0 WHERE T0.""U_ST_MEMBERSHIP_CODE"" = '{indivMembershipCode}'";
            Recordset RC = Helper.Utility.Execute_Recordset_Query(company, SQL);

            int DocEntry = 0;
            if (RC.RecordCount != 0)
            {
                Doc = (SAPbobsCOM.Documents)company.GetBusinessObject(BoObjectTypes.oInvoices);
                DocEntry = (int)RC.Fields.Item("DocEntry").Value;
            }
            else
            {
                SQL = $@"SELECT T0.""DocEntry"" FROM ODPI T0 WHERE T0.""U_ST_MEMBERSHIP_CODE"" = '{indivMembershipCode}'";
                RC = Helper.Utility.Execute_Recordset_Query(company, SQL);
                if (RC.RecordCount == 0)
                {
                    throw new Logic.Custom_Exception($"There is no Invoice/Down Payment for the Membership[{indivMembershipCode}]");
                }
                Doc = (SAPbobsCOM.Documents)company.GetBusinessObject(BoObjectTypes.oDownPayments);
                DocEntry = (int)RC.Fields.Item("DocEntry").Value;
            }

            //int DocEntry = Convert.ToInt32(RC_Invoice_Number.Fields.Item("U_ST_INVOICE_NUMBER").Value.ToString());
            Doc.GetByKey(DocEntry);

            return Doc;
        }

        internal static string Get_Last_Individual_Membership_Per_Card(Company company, string UDO_Card_Code, UDO_Definition UDO_Membership_Info, bool With_Error = true)
        {
            string SQL = $@"SELECT top 1 T0.""Code"" FROM ""@{UDO_Membership_Info.Table_Name}""  T0 
WHERE T0.U_ST_MEMBER_CARD = '{UDO_Card_Code}' 
ORDER BY U_ST_END_DATE DESC";
            Recordset RC = Helper.Utility.Execute_Recordset_Query(company, SQL);

            if (RC.RecordCount == 0)
            {
                if (With_Error == true)
                {
                    throw new Logic.Custom_Exception($"There is no active Membership for the Member Card[{UDO_Card_Code}]");
                }
                else
                {
                    return "";
                }
            }
            else
            {
                return RC.Fields.Item("Code").Value.ToString();
            }
        }

        internal static Field_Data[] Get_UDO_Data(Company company, string UDO_Code, UDO_Definition UDO_Info)
        {
            string SQL_Data = $@"Select * FROM ""@{UDO_Info.Table_Name}"" WHERE ""Code"" = '{UDO_Code}'";
            Recordset RC_Data = Helper.Utility.Execute_Recordset_Query(company, SQL_Data);

            Field_Definition[] UDO_Fields = Fields_Logic.All_Field_Definition.Where(F => F.KHCF_Object == UDO_Info.KHCF_Object).ToArray();
            List<Field_Data> Result_Data = new List<Field_Data>();
            foreach (Field_Definition OneField in UDO_Fields)
            {
                Result_Data.Add(new Field_Data() { Field_Name = OneField.Column_Name_In_DB, Value = RC_Data.Fields.Item(OneField.Column_Name_In_DB).Value });
            }
            return Result_Data.ToArray();
        }

        internal static int Get_UDO_DocEntry(Company company, string UDO_Code, UDO_Definition UDO_Info)
        {
            string SQL = $@"SELECT T0.""DocEntry"" FROM ""@{UDO_Info.Table_Name}""  T0 WHERE T0.""Code"" = '{UDO_Code}'";
            Recordset RC = Helper.Utility.Execute_Recordset_Query(company, SQL);
            return (int)RC.Fields.Item("DocEntry").Value;
        }

        internal static bool User_Can_Approve(Company company, string User_Name, UDO_Definition UDO_Info)
        {
            string SQL = $@"Select {Logic.Objects_Logic.Get_Can_Approve_UDF_Name(UDO_Info)} from OUSR 
WHERE USER_CODE = '{User_Name}'";

            Recordset RC = Helper.Utility.Execute_Recordset_Query(company, SQL);

            return RC.Fields.Item(0).Value.ToString() == "Y";
        }

        internal static Helper.MetaDataOperater.UserFields.Valid_Value[] Get_Valid_Values_List(Company company, UDO_Definition UDO_Info, string DB_Field_Name)
        {
            List<Helper.MetaDataOperater.UserFields.Valid_Value> Result = new List<Helper.MetaDataOperater.UserFields.Valid_Value>();
            Field_Definition Fld = Fields_Logic.All_Field_Definition.FirstOrDefault(F => F.KHCF_Object == UDO_Info.KHCF_Object && F.Column_Name_In_DB == DB_Field_Name);
            if (Fld == null)
            {
                throw new Logic.Custom_Exception($"The Field [{DB_Field_Name}] is not supported in the UDO[{UDO_Info.Table_Name}]");
            }
            string Valid_Values_Text = Fld.Valid_Values_Text;

            if (Valid_Values_Text.Contains("|"))
            {
                string[] Val_Desc = Valid_Values_Text.Split("|".ToCharArray());
                foreach (string One_Val_Desc in Val_Desc)
                {
                    if (One_Val_Desc.Contains(","))
                    {
                        throw new Logic.Custom_Exception($"The Valid Values[{Valid_Values_Text}] for the Field[{Fld.Field_Name}], Table[{Fld}] is not valid as Valid Values");
                    }
                    string[] One_Vale_Desc_Array = One_Val_Desc.Split(",".ToCharArray());
                    Helper.MetaDataOperater.UserFields.Valid_Value One_Value = new Helper.MetaDataOperater.UserFields.Valid_Value();
                    One_Value.Value = One_Vale_Desc_Array[0];
                    One_Value.Name = One_Vale_Desc_Array[1];
                    Result.Add(One_Value);
                }
            }
            else
            {
                string[] Values_Text = Valid_Values_Text.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Select(S => S.Trim()).ToArray();
                if (Values_Text.Length == 2 && (Values_Text.Contains("No") && Values_Text.Contains("Yes")))
                {
                    Helper.MetaDataOperater.UserFields.Valid_Value One_Value = new Helper.MetaDataOperater.UserFields.Valid_Value();
                    One_Value.Value = Values_Text[0].Substring(0, 1);
                    One_Value.Name = Values_Text[0];
                    Result.Add(One_Value);

                    One_Value.Value = Values_Text[1].Substring(0, 1);
                    One_Value.Name = Values_Text[1];
                    Result.Add(One_Value);

                }
                else
                {
                    throw new Logic.Custom_Exception("This Valid Values case is not supported");
                    //Helper.MetaDataOperater.UserFields.Valid_Value One_Value = new Helper.MetaDataOperater.UserFields.Valid_Value();
                    //One_Value.Value = Values_Text;
                    //One_Value.Name =
                    //Values.AddRange(Values_Text);
                    //Descriptions.AddRange(Values_Text);
                }
            }

            return Result.ToArray();
        }

        internal static void Set_UDF_Value_on_Form(string Value, SAPbouiCOM.Form UDF_Form, string UDF_Name, bool Is_Text)
        {
            UDF_Form.Items.Item(UDF_Name).Enabled = true;
            if (Is_Text)
            {
                ((SAPbouiCOM.EditText)UDF_Form.Items.Item(UDF_Name).Specific).Value = Value;
            }
            else
            {
                ((SAPbouiCOM.ComboBox)UDF_Form.Items.Item(UDF_Name).Specific).Select(Value);
            }

        }

        internal static string Get_Field_Name(KHCF_Objects KHCF_Object_Value, string Item_ID)
        {
            var Field = Fields_Logic.All_Field_Definition.FirstOrDefault(F => F.KHCF_Object == KHCF_Object_Value && F.Item_ID == Item_ID);
            var Field2 = Fields_Logic.All_Field_Definition.FirstOrDefault(F => F.KHCF_Object == KHCF_Object_Value);//&& F.Item_ID == Item_ID);

            if (Field == null)
            {
                throw new Logic.Custom_Exception($"There is no Field value for the Object[{KHCF_Object_Value}] and Item[{Item_ID}]");
            }
            else
            {
                return Field.Field_Name;
            }
        }

        internal static string Get_BP_Group_Name(Company company, int BP_Group_Code)
        {
            string SQL = $@"SELECT T0.""GroupName"" FROM OCRG T0 WHERE ""GroupCode"" = {BP_Group_Code}";
            Recordset RC = Helper.Utility.Execute_Recordset_Query(company, SQL);
            if (RC.RecordCount == 0)
            {
                throw new Logic.Custom_Exception($"The BP Group[{BP_Group_Code}] is not existing on B1");
            }
            return RC.Fields.Item("GroupName").Value.ToString();
        }

        internal static void Set_Item_Editable(SAPbouiCOM.Form form, string One_Item, bool Is_Editable, bool Set_Mondatory = false)
        {
            if (Is_Editable)
            {
                form.Items.Item(One_Item).SetAutoManagedAttribute(SAPbouiCOM.BoAutoManagedAttr.ama_Editable, -1, SAPbouiCOM.BoModeVisualBehavior.mvb_True);
                form.Items.Item(One_Item).SetAutoManagedAttribute(SAPbouiCOM.BoAutoManagedAttr.ama_Editable, 4, SAPbouiCOM.BoModeVisualBehavior.mvb_True);
                if (Set_Mondatory == true)
                {
                    form.Items.Item(One_Item).BackColor = Color.Pink.R | (Color.Pink.G << 8) | (Color.Pink.B << 16);
                }
            }
            else
            {
                form.Items.Item(One_Item).SetAutoManagedAttribute(SAPbouiCOM.BoAutoManagedAttr.ama_Editable, -1, SAPbouiCOM.BoModeVisualBehavior.mvb_False);
                form.Items.Item(One_Item).SetAutoManagedAttribute(SAPbouiCOM.BoAutoManagedAttr.ama_Editable, 4, SAPbouiCOM.BoModeVisualBehavior.mvb_False);
                if (Set_Mondatory == true)
                {
                    form.Items.Item(One_Item).BackColor = Color.FromKnownColor(KnownColor.White).ToArgb();

                }
            }
        }

        internal static Field_Data Get_Field_Value(Company company, UDO_Definition UDO_Info, string UDO_Code, string Field_Name)
        {
            string SQL = $@"Select ""{Field_Name}"" from ""@{UDO_Info.Table_Name}"" WHERE ""Code"" = '{UDO_Code}'";
            Recordset RC = Helper.Utility.Execute_Recordset_Query(company, SQL);

            Field_Data Result = new Field_Data() { Field_Name = Field_Name, Value = RC.Fields.Item(Field_Name).Value };

            return Result;
        }

        internal static DateTime Add_Time_Log(string type, string content, DateTime previous_Time, bool last = false)
        {
            TimeSpan timeSpan = DateTime.Now.Subtract(previous_Time);
            string contents = string.Empty;
            if (last)
            {
                contents = "The Entire Initialize Form Took:" + timeSpan.ToString("ss':'ff") + Environment.NewLine + "-------------------------------------------------------";
            }
            else
            {
                contents = $@"Fill {content} took {timeSpan.ToString("ss':'ff")}" + Environment.NewLine;
            }
            if (type != "C")
            {
                if (type != "F")
                {
                    if (type == "P")
                    {
                        File.AppendAllText("C:\\Users\\wsaleh\\Desktop\\Patient_TimeLog.txt", contents);
                    }
                }
                else
                {
                    File.AppendAllText("C:\\Users\\wsaleh\\Desktop\\Fund_TimeLog.txt", contents);
                }
            }
            else
            {
                File.AppendAllText("C:\\Users\\wsaleh\\Desktop\\CCI_TimeLog.txt", contents);
            }
            return DateTime.Now;
        }

        internal static bool Check_Text(string Checked_Name)
        {
            // true if Arabic, false if English
            bool result = true;
            char[] Name = Checked_Name.ToCharArray();
            for (var i = 0; i < Name.Length; i++)
            {
                if (Name[i] > 64 && Name[i] < 123)
                {
                    result = false;
                    break;
                }
            }
            return result;
        }

        internal static bool IsDigitsOnly(string str)
        {
            foreach (char c in str)
            {
                if (c < '0' || c > '9')
                    return false;
            }

            return true;
        }

        internal static int GetMonthDifference(DateTime startDate, DateTime endDate)
        {
            int monthsApart = 12 * (startDate.Year - endDate.Year) + startDate.Month - endDate.Month;
            return monthsApart;
        }

        internal static double Get_Exchange_Rate(SAPbobsCOM.Company company, string Currency, DateTime Date)
        {
            if (Currency == "JOD")
            {
                return 1;
            }
            try
            {
                SBObob bob = (SBObob)company.GetBusinessObject(BoObjectTypes.BoBridge);
                double Result = (double)bob.GetCurrencyRate(Currency, Date).Fields.Item(0).Value;
                return Result;
            }
            catch (Exception)
            {
                throw new Logic.Custom_Exception($"Error during get the Exchange Rate for the Currency[{Currency}] on Date[{Date}]");
            }
        }

        internal static bool IndvCardHasValidMembership(Company company, string MemberCardCode)
        {
            string SQL_History = $@"SELECT COUNT(*) AS ""MEMBERSHIPS"" FROM ""@ST_INDIV_MEMBERSHIP"" T0 WHERE T0.""U_ST_MEMBER_CARD"" ='{MemberCardCode}' 
AND ( T0.""U_ST_MEMBERSHIP_STATUS"" IN ('N','R','P') ) AND CURRENT_DATE BETWEEN T0.""U_ST_START_DATE""  AND  T0.""U_ST_END_DATE"" ";
            Recordset RC_History = Helper.Utility.Execute_Recordset_Query(company, SQL_History);
            if (RC_History.RecordCount > 0)
            {
                int number = Convert.ToInt32(RC_History.Fields.Item("MEMBERSHIPS").Value.ToString());
                if (number > 0)
                    return true;
            }
            return false;
        }

        internal static string Get_Booth_Employee(Company company)
        {
            string SQL_Booth = $@"SELECT T1.""Code"" ,(T1.""firstName"" || ' ' || T1.""lastName"") AS ""Name"" FROM OHEM T1 INNER JOIN OUSR T2 ON T1.""userId"" = T2.""USERID"" WHERE T2.""USER_CODE"" ='{company.UserName}' AND ""U_ST_EMPLOYEE_TYPE"" = 'B'";

//            string SQL_Booth = $@"SELECT T0.""empID"" AS ""Code"", (T0.""firstName"" || ' ' || T0.""lastName"") AS ""Name"" FROM OHEM T0 
//Where U_ST_EMPLOYEE_TYPE = 'B' ";
            Recordset RC = Helper.Utility.Execute_Recordset_Query(company, SQL_Booth);
            if (RC.RecordCount == 0)
            {
                return string.Empty;
            }
            else
                return
                    RC.Fields.Item("Code").Value.ToString();
           
        }

        #region GUI Utility

        internal static Documents Copy_Document(Company company, Documents Source_Doc, BoObjectTypes Target_Type)
        {
            Documents Target_Doc = (Documents)company.GetBusinessObject(Target_Type);
            Target_Doc.CardCode = Source_Doc.CardCode;
            Target_Doc.DocType = Source_Doc.DocType;

            for (int i = 0; i < Source_Doc.Lines.Count; i++)
            {
                Source_Doc.Lines.SetCurrentLine(i);

                Target_Doc.Lines.BaseEntry = Source_Doc.DocEntry;
                Target_Doc.Lines.BaseLine = Source_Doc.Lines.LineNum;
                Target_Doc.Lines.BaseType = (int)Source_Doc.DocObjectCode;
                if (Source_Doc.DocType == BoDocumentTypes.dDocument_Service)
                {
                    Target_Doc.Lines.LineTotal = Source_Doc.Lines.LineTotal;
                }

                Target_Doc.Lines.Add();
            }

            if (Target_Doc.Add() !=0)
            {
                throw new Exception($@"Error during copy the document[{Source_Doc.DocNum}][{company.GetLastErrorDescription()}] ");
            }
            else
            {
                string NewEntry;
                company.GetNewObjectCode(out NewEntry);
                Target_Doc.GetByKey(int.Parse(NewEntry));

                return Target_Doc;
            }
        }

        internal static string Get_Country_Name(Company company, string Country_Code)
        {
            string SQL = $@"SELECT T0.""Name"" FROM OCRY T0 WHERE T0.""Code"" = '{Country_Code}'";
            Recordset RC = Helper.Utility.Execute_Recordset_Query(company, SQL);

            if (RC.RecordCount == 0)
            {
                throw new Logic.Custom_Exception($"The Country Code[{Country_Code}] is not existing");
            }
            return RC.Fields.Item("Name").Value.ToString();
        }

        internal static (int Group_Code, string Group_Name, string Code_Name) Get_Member_Card_Customer_Group(Company company, string MemberCard_Code)
        {
            string SQL = $@"SELECT T1.""GroupCode"" , T1.""GroupName"" 
FROM ""@ST_CCI_INDIV_CARD""  T0 INNER JOIN OCRG T1 ON TO_Varchar(T1.""GroupCode"" )= T0.U_ST_CUSTOMER_GROUP 
WHERE T0.""Code"" = '{MemberCard_Code}'";
            Recordset RC = Helper.Utility.Execute_Recordset_Query(company, SQL);

            return ((int)RC.Fields.Item("GroupCode").Value, RC.Fields.Item("GroupName").Value.ToString(), RC.Fields.Item("GroupCode").Value.ToString() + "-" + RC.Fields.Item("GroupName").Value.ToString());
        }

        internal static void Remove_BP(Company company, string CardCode)
        {
            if (string.IsNullOrWhiteSpace(CardCode))
            {
                return;
            }

            BusinessPartners BP = (BusinessPartners)company.GetBusinessObject(BoObjectTypes.oBusinessPartners);
            BP.GetByKey(CardCode);

            if (BP.Remove() != 0)
            {
                throw new Logic.Custom_Exception($"Error during remove the BP[{CardCode}][{company.GetLastErrorDescription()}]");
            }
        }

        internal static string Get_Date_Datasource_ValueEX(DateTime Date_Value)
        {
            if (Date_Value.Year == 1899)
            {
                return "";
            }
            else
            {
                return Date_Value.ToString("yyyyMMdd");
            }
        }

        internal static List<Item_UI> Get_Grids_UI(SAPbouiCOM.Form form, string[] Grid_IDs)
        {
            List<Item_UI> Result = new List<Item_UI>();
            foreach (string  One_Item in Grid_IDs)
            {
                SAPbouiCOM.Item Itm = form.Items.Item(One_Item);
                Item_UI Temp = new Item_UI() { Item_ID = One_Item, Height = Itm.Height, Left = Itm.Left, Top = Itm.Top, Width = Itm.Width };
                Result.Add(Temp);
            }
            return Result;
        }

        internal static void Resize_Grids(SAPbouiCOM.Form form, List<Item_UI> Item_List)
        {
            try
            {
                form.Freeze(true);
                foreach (Item_UI One_Item in Item_List)
                {
                    SAPbouiCOM.Item Itm = form.Items.Item(One_Item.Item_ID);

                    Itm.Width = One_Item.Width;
                    Itm.Height = One_Item.Height;
                    Itm.Left = One_Item.Left;
                    if (Itm.Type == SAPbouiCOM.BoFormItemTypes.it_GRID)
                    {
                        ((SAPbouiCOM.Grid)form.Items.Item(One_Item.Item_ID).Specific).AutoResizeColumns();
                    }
                    else if (Itm.Type == SAPbouiCOM.BoFormItemTypes.it_MATRIX)
                    {
                        ((SAPbouiCOM.Matrix)form.Items.Item(One_Item.Item_ID).Specific).AutoResizeColumns();
                    }
                }
                form.Freeze(false);
            }
            catch (Exception)
            {
                form.Freeze(false);

            }
        }

        internal static void Check_Relation_Table_Mandatory(SAPbouiCOM.DataTable DT, string Table_Name)
        {
            bool Is_Selected = false;
            for (int i = 0; i < DT.Rows.Count; i++)
            {
                if (DT.GetValue("SELECTED", i).ToString() == "Y")
                {
                    Is_Selected = true;
                    break;
                }
            }
            if (!Is_Selected)
            {
                throw new Logic.Custom_Exception($"You need to select at least one row from the table[{Table_Name}]");
            }
        }

        internal static void Add_Grid_Row(SAPbouiCOM.Form form, string Datatable_Name)
        {
            SAPbouiCOM.DataTable DT = form.DataSources.DataTables.Item(Datatable_Name);
            DT.Rows.Add();
        }

        internal static void Remove_Grid_Row(SAPbouiCOM.Form form, string Datatable_Name)
        {
            SAPbouiCOM.DataTable DT = form.DataSources.DataTables.Item(Datatable_Name);

            for (int i = DT.Rows.Count - 1; i >= 0; i--)
            {
                if (DT.GetValue("SELECTED", i).ToString() != "Y")
                {
                    continue;
                }
                DT.Rows.Remove(i);
            }
        }

        #endregion


    }
}
