using SAPbobsCOM;
using SAPbouiCOM;
using ST.KHCF.Customization.Forms.CCI;
using ST.KHCF.Customization.Logic;
using ST.KHCF.Customization.Logic.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Text;
using System.Threading.Tasks;

namespace ST.KHCF.Customization.Forms
{
    internal class Frm_Import_Data
    {
        internal static SAPbobsCOM.Company company;
        internal static Application SBO_Application;
        internal const string InternalKey_Field = "InternalKey";

        internal static Form Create_Form()
        {
            var form_params = (FormCreationParams)SBO_Application.CreateObject(BoCreatableObjectType.cot_FormCreationParams);
            form_params.XmlData = Properties.Resources.Frm_Import_Data;
            var form = SBO_Application.Forms.AddEx(form_params);
            //form.Visible = false;
            form.AutoManaged = true;
            form.SupportedModes = -1;
            form_params.Modality = BoFormModality.fm_Modal;

            //  Inv_Form_Data = Inv_Data;

            try
            {
                Initialize_Form(form);
            }
            catch (Exception ex)
            {
                SBO_Application.StatusBar.SetText(ex.Message, BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Error);
            }
            form.Visible = true;
            return form;
        }

        private static void Initialize_Form(Form form)
        {
            //KHCF_Objects[] Supported_UDOs = new KHCF_Objects[] { KHCF_Objects.CCI_Member_Card, KHCF_Objects.CCI_Corporate_Member_Card, KHCF_Objects.Individual_Membership, KHCF_Objects.Corporate_Membership, KHCF_Objects.Expected_Donations};
            KHCF_Objects[] Supported_UDOs = new KHCF_Objects[] { KHCF_Objects.CCI_Member_Card, KHCF_Objects.Individual_Membership  };
            ComboBox Cmb_Object_Type = (ComboBox)form.Items.Item("9").Specific;
            foreach (KHCF_Objects OneObj in Supported_UDOs)
            {
                UDO_Definition UDO_Info = Logic.Objects_Logic.All_UDO_Definition.FirstOrDefault(U => U.KHCF_Object == OneObj);
                Cmb_Object_Type.ValidValues.Add(((int)OneObj).ToString(), UDO_Info.Title);
            }
            Cmb_Object_Type.Select(0, BoSearchKey.psk_Index);
            //form.Items.Item("9").SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, -1, BoModeVisualBehavior.mvb_False);
            //form.Items.Item("13").Click();
        }

        internal static void SBO_Application_ItemEvent(string FormUID, ref ItemEvent pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            if (pVal.FormTypeEx != "ST_Import_Data")
            {
                return;
            }
            try
            {
                if (pVal.ItemUID == "6" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Preview(pVal);
                }
                if (pVal.ItemUID == "5" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Select_Header_Path(pVal);
                }
                if (pVal.ItemUID == "12" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Select_Lines_Path(pVal);
                }
                if (pVal.ItemUID == "7" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Import(pVal);
                }


            }
            catch (Exception ex)
            {
                Form form = SBO_Application.Forms.Item(pVal.FormUID);
                form.Freeze(false);
                SBO_Application.StatusBar.SetText(ex.Message, BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Error);
            }
        }

        private static void Preview(ItemEvent pVal)
        {
            Form form = SBO_Application.Forms.Item(pVal.FormUID);
            Grid HeadersGrid = (Grid)form.Items.Item("15").Specific;
            Grid LinesGrid = (Grid)form.Items.Item("16").Specific;
            form.Freeze(true);
            DataTable DT_Header_Result = form.DataSources.DataTables.Item("RESULT");
            DT_Header_Result.Clear();

            KHCF_Objects Selected_Object = (KHCF_Objects)int.Parse(form.DataSources.UserDataSources.Item("9").Value);
            UDO_Definition Obj_Info = Logic.Objects_Logic.All_UDO_Definition.FirstOrDefault(U => U.KHCF_Object == Selected_Object);
            string CSV_Delimiter = Configurations.Get_File_Delimiter(company);
            string File_Header_Path = form.DataSources.UserDataSources.Item("4").Value;
            string[] Header_Lines = System.IO.File.ReadAllLines(File_Header_Path);
            if (Header_Lines.Length <= 2)
            {
                throw new Logic.Custom_Exception($@"The File[{File_Header_Path}] is Empty");
            }

            string[] Header_Columns = Header_Lines[0].Split(CSV_Delimiter.ToCharArray()).Select(H => H.Trim()).ToArray();
            if (Obj_Info.External_Key != "" && !Header_Columns.Contains(Obj_Info.External_Key))
            {
                throw new Logic.Custom_Exception($"The {Obj_Info.External_Key} column is mandatory in the file");
            }
            DT_Header_Result.Columns.Add("Existing", BoFieldsType.ft_AlphaNumeric, 1);
            DT_Header_Result.Columns.Add("Import_Result", BoFieldsType.ft_Text);
            foreach (string OneCol in Header_Columns)
            {
                DT_Header_Result.Columns.Add(OneCol, BoFieldsType.ft_AlphaNumeric);
            }
            if (Header_Columns.Contains("Code") == false)
            {
                DT_Header_Result.Columns.Add("Code", BoFieldsType.ft_AlphaNumeric, 50);
            }
            //DT_Header_Result.Columns.Add("Existing", BoFieldsType.ft_AlphaNumeric, 1);
            //DT_Header_Result.Columns.Add("Import_Result", BoFieldsType.ft_Text);

            DT_Header_Result.Rows.Add(Header_Lines.Length - 2);
            for (int i = 2  ; i < Header_Lines.Length; i++)
            {
                int Table_Row_Index = i - 2;
                string[] One_LineDate = Header_Lines[i].Split(CSV_Delimiter.ToCharArray()).Select(H => H.Trim()).ToArray();
                List<Field_Data> Line_Fields_Data = new List<Field_Data>();
                for (int j = 0; j < One_LineDate.Length; j++)
                {
                    DT_Header_Result.SetValue(j + 2, Table_Row_Index, One_LineDate[j]);
                    Field_Data Fld = new Field_Data() {  Column_Name_In_DB = DT_Header_Result.Columns.Item(j + 2).Name, Data_Type = BoFieldTypes.db_Alpha, Value = One_LineDate[j] };
                    Line_Fields_Data.Add(Fld);
                }

                //if (Obj_Info.KHCF_Object != KHCF_Objects.Individual_Membership)
                //{
                    var Existing_Data = Utility.National_ID_IsExisting(company, Line_Fields_Data, Obj_Info);

                if (Existing_Data.IsExising == true)
                {
                    DT_Header_Result.SetValue("Existing", Table_Row_Index, "Y");
                    DT_Header_Result.SetValue("Import_Result", Table_Row_Index, $"The {Obj_Info.Title} is already existing in SAP with code[{Existing_Data.Code}]");
                    DT_Header_Result.SetValue("Code", Table_Row_Index, Existing_Data.Code);
                }
                else
                {
                    DT_Header_Result.SetValue("Existing", Table_Row_Index, "N");
                }
                //}
                //else
                //{
                //    DT_Header_Result.SetValue("Existing", Table_Row_Index, "N");
                //}
            }

            ((Grid)form.Items.Item("16").Specific).AutoResizeColumns();


            DataTable DT_Lines_Result = form.DataSources.DataTables.Item("RESULT_LINES1");
            DT_Lines_Result.Clear();
            string File_Lines_Path = form.DataSources.UserDataSources.Item("11").Value;
            if (File_Lines_Path == "")
            {
                form.Freeze(false);
                return;
            }
            string[] Lines1_Lines = System.IO.File.ReadAllLines(File_Lines_Path);
            if (Lines1_Lines.Length <= 2)
            {
                form.Freeze(false);
                return;
            }
            string[] Lines1_Columns = Lines1_Lines[0].Split(CSV_Delimiter.ToCharArray());
            if (!Lines1_Columns.Contains(InternalKey_Field))
            {
                throw new Logic.Custom_Exception($"The {InternalKey_Field} column is mandatory in the lines file");
            }
            foreach (string OneCol in Lines1_Columns)
            {
                DT_Lines_Result.Columns.Add(OneCol, BoFieldsType.ft_AlphaNumeric);
            }
            DT_Lines_Result.Rows.Add(Lines1_Lines.Length - 2);
            for (int i = 2; i < Lines1_Lines.Length; i++)
            {
                int Table_Row_Index = i - 2;
                string[] One_LineDate = Lines1_Lines[i].Split(CSV_Delimiter.ToCharArray());
                for (int j = 0; j < One_LineDate.Length; j++)
                {
                    DT_Lines_Result.SetValue(j, Table_Row_Index, One_LineDate[j]);
                }

                //if (Utility.National_ID_IsExisting(company, DT_Header_Result.GetValue(Obj_Info.External_Key, Table_Row_Index), Obj_Info))
                //{
                //    DT_Lines_Result.SetValue("Existing", Table_Row_Index, "Y");
                //}
                //else
                //{
                //    DT_Header_Result.SetValue("Existing", Table_Row_Index, "N");
                //}

            }

            ((Grid)form.Items.Item("15").Specific).AutoResizeColumns();
            HeadersGrid.AutoResizeColumns();
            LinesGrid.AutoResizeColumns();

            form.Freeze(false);

        }

        private static void Import(ItemEvent pVal)
        {
            if (SBO_Application.MessageBox("Are you sure you want to import the data", 1, "Yes", "No") != 1)
            {
                return;
            }
            Form form = SBO_Application.Forms.Item(pVal.FormUID);
            Grid HeadersGrid = (Grid)form.Items.Item("16").Specific;
            Grid LinesGrid = (Grid)form.Items.Item("15").Specific;
            try
            {
                form.Freeze(true);
                bool authorized = false;
                string SQL_User_Auth = $@"SELECT T0.""U_ST_CAN_OVERWRITE_DATA"" FROM OUSR T0 WHERE T0.""USER_CODE""='{company.UserName}'";
                Recordset RC_User_Auth = Helper.Utility.Execute_Recordset_Query(company, SQL_User_Auth);
                if (RC_User_Auth.RecordCount > 0)
                {
                    if (RC_User_Auth.Fields.Item(0).Value.ToString() == "Y")
                    {
                        authorized = true;
                    }
                }
                DataTable DT_Result = form.DataSources.DataTables.Item("RESULT");
                DataTable DT_Result_Line1 = form.DataSources.DataTables.Item("RESULT_LINES1");
                string Object_ID = form.DataSources.UserDataSources.Item("9").Value;
                UDO_Definition UDO_Info = Logic.Objects_Logic.All_UDO_Definition.FirstOrDefault(U => U.KHCF_Object == (KHCF_Objects)(int.Parse(Object_ID)));
                UDO_Data UDO_Line1_Data = null;
                string[] Excluded_Filed = new string[] { "Existing", "Import_Result" };
                List<Field_Definition> Fields_Table = new List<Field_Definition>();
                for (int i = 0; i < DT_Result.Columns.Count; i++)
                {
                    string Col_Name = DT_Result.Columns.Item(i).Name;
                    string Field_Name = Utility.Get_Field_Definition_Name(Col_Name);
                    if (Excluded_Filed.Contains(Col_Name))
                    {
                        continue;
                    }
                    if (Col_Name == "Code")
                    {
                        Field_Definition Fld_Code = new Field_Definition() { Field_Name = "Code", Column_Name_In_DB = "Code", Data_Type = BoFieldTypes.db_Alpha , Is_Temp = true};
                        Fields_Table.Add(Fld_Code);
                        continue;
                    }
                    if ((UDO_Info.KHCF_Object == KHCF_Objects.Corporate_Membership || UDO_Info.KHCF_Object == KHCF_Objects.Individual_Membership) && Col_Name == "U_ST_NATIONAL_ID")
                    {
                        Fields_Table.Add(new Field_Definition() { Field_Name = "ST_MEMBER_CARD", Column_Name_In_DB = "U_ST_MEMBER_CARD", Data_Type = BoFieldTypes.db_Alpha });
                        continue;
                    }
                    if (Col_Name == InternalKey_Field)
                    {
                        Fields_Table.Add(new Field_Definition() { Field_Name = InternalKey_Field, Column_Name_In_DB = InternalKey_Field, Data_Type = BoFieldTypes.db_Alpha, Is_Temp = true });
                        continue;
                    }
                    Field_Definition OneFld = Logic.Fields_Logic.All_Field_Definition.FirstOrDefault(F => F.KHCF_Object == UDO_Info.KHCF_Object && F.Field_Name == Field_Name);
                    if (OneFld == null)
                    {
                        throw new Logic.Custom_Exception($"The Field[{Col_Name}] is not supported");
                    }
                    OneFld.Column_Name_In_DB = Col_Name;
                    Fields_Table.Add(OneFld);
                }

                if (UDO_Info.KHCF_Object == KHCF_Objects.CCI_Member_Card)
                {
                    Fields_Table.Add(new Field_Definition() { Field_Name = InternalKey_Field, Column_Name_In_DB = InternalKey_Field, Data_Type = BoFieldTypes.db_Alpha, Is_Temp = true });
                }

                UDO_Definition UDO_MemberCard_Info = null;
                string Key_UDF = "";
                if (UDO_Info.KHCF_Object == KHCF_Objects.Individual_Membership)
                {
                    UDO_MemberCard_Info = Logic.Objects_Logic.All_UDO_Definition.FirstOrDefault(U => U.KHCF_Object == KHCF_Objects.CCI_Member_Card);
                    Key_UDF = "Code";
                }
                else if (UDO_Info.KHCF_Object == KHCF_Objects.Corporate_Membership)
                {
                    UDO_MemberCard_Info = Logic.Objects_Logic.All_UDO_Definition.FirstOrDefault(U => U.KHCF_Object == KHCF_Objects.CCI_Corporate_Member_Card);
                    Key_UDF = "U_ST_CORPORATE_NATIONAL_ID";
                }
                else if (UDO_Info.KHCF_Object == KHCF_Objects.CCI_Member_Card)
                {
                    var UDO_Line_Info = Logic.Objects_Logic.All_UDO_Definition.FirstOrDefault(U => U.KHCF_Object == KHCF_Objects.CCI_Member_Card_Address);
                    UDO_Line1_Data = new UDO_Data() { UDO_Info = UDO_Line_Info, Foreign_Key_Field = InternalKey_Field, Primary_Key = "U_ST_ADDRESS_NAME" };
                    Key_UDF = InternalKey_Field;
                }
                else if (UDO_Info.KHCF_Object == KHCF_Objects.CCI_Corporate_Member_Card)
                {
                    var UDO_Line_Info = Logic.Objects_Logic.All_UDO_Definition.FirstOrDefault(U => U.KHCF_Object == KHCF_Objects.CCI_Corporate_Member_Card_Address);
                    UDO_Line1_Data = new UDO_Data() { UDO_Info = UDO_Line_Info, Foreign_Key_Field = "U_ST_CORPORATE_NATIONAL_ID", Primary_Key = "U_ST_ADDRESS_NAME" };
                    Key_UDF = "U_ST_CORPORATE_NATIONAL_ID";
                }

                List<List<Field_Data>> All_Line_Lines_Data = new List<List<Field_Data>>();
                List<Field_Definition> Line1_Fields_Table = new List<Field_Definition>();

                if (UDO_Line1_Data != null)
                {
                    for (int i = 0; i < DT_Result_Line1.Columns.Count; i++)
                    {
                        string Col_Name = DT_Result_Line1.Columns.Item(i).Name;
                        string Field_Name = Utility.Get_Field_Definition_Name(Col_Name);
                        //if (Excluded_Filed.Contains(Col_Name))
                        //{
                        //    continue;
                        //}
                        //if ((UDO_Info.KHCF_Object == KHCF_Objects.Corporate_Membership || UDO_Info.KHCF_Object == KHCF_Objects.Individual_Membership) && Col_Name == "U_ST_NATIONAL_ID")
                        //{
                        //    Fields_Table.Add(new Field_Definition() { Field_Name = "ST_MEMBER_CARD", Column_Name_In_DB = "U_ST_MEMBER_CARD", Data_Type = BoFieldTypes.db_Alpha });
                        //    continue;
                        //}

                        Field_Definition OneFld = Logic.Fields_Logic.All_Field_Definition.FirstOrDefault(F => F.KHCF_Object == UDO_Line1_Data.UDO_Info.KHCF_Object && F.Field_Name == Field_Name);
                        if (OneFld == null)
                        {
                            if (Col_Name == UDO_Line1_Data.Foreign_Key_Field)
                            {

                                OneFld = new Field_Definition() { Field_Name = InternalKey_Field, Column_Name_In_DB = InternalKey_Field, Data_Type = BoFieldTypes.db_Alpha, Is_Temp = true };
                            }
                            else
                            {
                                throw new Logic.Custom_Exception($"The Field[{Col_Name}] is not supported");
                            }
                        }
                        //OneFld.Column_Name_In_DB = Col_Name;
                        Line1_Fields_Table.Add(OneFld);
                    }


                    UDO_Line1_Data.Fields_Definition = Line1_Fields_Table;
                    for (int i = 0; i < DT_Result_Line1.Rows.Count; i++)
                    {
                        List<Field_Data> One_Line_Data = new List<Field_Data>();
                        try
                        {
                            foreach (Field_Definition One_Table_Field in Line1_Fields_Table)
                            {
                                object Value;
                                //if ((UDO_Info.KHCF_Object == KHCF_Objects.Corporate_Membership || UDO_Info.KHCF_Object == KHCF_Objects.Individual_Membership)
                                //    && One_Table_Field.Column_Name_In_DB == "U_ST_MEMBER_CARD")
                                //{
                                //    string Table_Value = DT_Result_Line1.GetValue(Key_UDF, i).ToString();
                                //    Value = Utility.Get_Code_Per_National_ID(company, Table_Value, UDO_MemberCard_Info);
                                //    if (Value.ToString() == "")
                                //    {
                                //        throw new Logic.Custom_Exception($"We can't find Member Card for the National ID[{Table_Value}]");
                                //    }
                                //}
                                //else
                                //{
                                string Table_Value = DT_Result_Line1.GetValue(One_Table_Field.Column_Name_In_DB, i).ToString();
                                Value = Get_Real_Value(i, One_Table_Field, Table_Value);
                                //}

                                Field_Data One_Field = new Field_Data() { Field_Name = One_Table_Field.Field_Name, Value = Value };
                                One_Line_Data.Add(One_Field);
                            }

                            All_Line_Lines_Data.Add(One_Line_Data);
                        }
                        catch (Exception ex)
                        {
                            throw new Logic.Custom_Exception($"Error during gathering The Lines 1 data[{ex.Message}]");
                        }
                    }
                    UDO_Line1_Data.Fields_Data_Lines = All_Line_Lines_Data;
                }

                //List<UDO_Data> Lines_Data = new List<UDO_Data>();
                //if (UDO_Line1_Data != null)
                //{
                //    Lines_Data.Add(UDO_Line1_Data);
                //}

                for (int i = 0; i < DT_Result.Rows.Count; i++)
                {
                    List<Field_Data> One_Line_Data = new List<Field_Data>();
                    try
                    {
                        foreach (Field_Definition One_Table_Field in Fields_Table)
                        {
                            object Value;
                            if ((UDO_Info.KHCF_Object == KHCF_Objects.Corporate_Membership)
                                && One_Table_Field.Column_Name_In_DB == Key_UDF)
                            {
                                string Table_Value = DT_Result.GetValue(Key_UDF, i).ToString();
                                Value = Utility.Get_Code_Per_National_ID(company, Table_Value, UDO_MemberCard_Info);
                                if (Value.ToString() == "")
                                {
                                    throw new Logic.Custom_Exception($"We can't find Member Card for the National ID[{Table_Value}]");
                                }
                            }
                            else
                            {
                                string Table_Value = DT_Result.GetValue(One_Table_Field.Column_Name_In_DB, i).ToString();
                                Value = Get_Real_Value(i, One_Table_Field, Table_Value);
                            }

                            Field_Data One_Field = new Field_Data() { Field_Name = One_Table_Field.Column_Name_In_DB, Value = Value, Is_Temp = One_Table_Field.Is_Temp };
                            if (One_Field.Field_Name == InternalKey_Field)
                            {
                                One_Field.Is_Temp = true;
                            }
                            One_Line_Data.Add(One_Field);
                        }

                        string Update_Type = string.Empty;
                        string Key_ID = DT_Result.GetValue(Key_UDF, i).ToString();
                        List<UDO_Data> Lines_Data = new List<UDO_Data>();
                        if (UDO_Line1_Data != null)
                        {
                            List<List<Field_Data>> UDO_Line_Lines_Data = new List<List<Field_Data>>();

                            foreach (var OneLine in All_Line_Lines_Data)
                            {
                                if (OneLine.FirstOrDefault(L => L.Field_Name == UDO_Line1_Data.Foreign_Key_Field).Value.ToString() == Key_ID)
                                {
                                    UDO_Line_Lines_Data.Add(OneLine);
                                }
                            }
                            UDO_Line1_Data.Fields_Data_Lines = UDO_Line_Lines_Data;
                            Lines_Data.Add(UDO_Line1_Data);
                        }

                        bool isRecordInDB = DT_Result.GetValue("Existing", i).ToString() == "Y";
                        if (isRecordInDB)
                        {
                            //continue;
                            if (isRecordInDB && authorized)
                            {
                                //string UDO_Code = Utility.Get_Code_Per_National_ID(company, Key_ID, UDO_Info);
                                var NationalID_IsExistingFunctionReturnObject = Utility.National_ID_IsExisting(company, One_Line_Data, UDO_Info);
                                string UDO_Code = NationalID_IsExistingFunctionReturnObject.Code;
                                //if (UDO_Info.KHCF_Object == KHCF_Objects.CCI_Member_Card)
                                //{
                                //    Field_Data Parent_ID_Field = One_Line_Data.FirstOrDefault(C => C.Column_Name_In_DB == "U_ST_PARENT_ID");
                                //    if (Parent_ID_Field != null)
                                //    {
                                //        string SQL_Parent = $@"SELECT T0.U_ST_PARENT_ID FROM ""@ST_CCI_INDIV_CARD"" T0 WHERE T0.""Code"" = '{UDO_Code}'";
                                //        Recordset RC_Parent = Helper.Utility.Execute_Recordset_Query(company, SQL_Parent);
                                //        string Old_Parent_ID = RC_Parent.Fields.Item("U_ST_PARENT_ID").Value.ToString();
                                //        Field_Data Parent_Type_Field = null;
                                //        string Parent_Name = "";
                                //        if (Old_Parent_ID != Parent_ID_Field.Value.ToString())
                                //        {
                                //            Parent_Type_Field = One_Line_Data.FirstOrDefault(C => C.Column_Name_In_DB == "U_ST_PARENT_TYPE");
                                //            if (Parent_Type_Field == null)
                                //            {
                                //                throw new Logic.Custom_Exception("We have Parent ID field but the Parent Type field is missing");
                                //            }
                                //            Parent_Name = KHCF_Logic_Utility.Get_MemberCard_Name(company, Parent_Type_Field.Value.ToString(), Parent_ID_Field.Value.ToString());
                                //        }
                                //        //Utility.Update_UDO(company, UDO_Info, UDO_Code, One_Line_Data.ToArray());
                                //        //Update_Type = "Updated";
                                //        KHCF_Logic_Utility.Check_MemberCard_Parent_Logic(company, UDO_Code, UDO_Info, Old_Parent_ID, Parent_ID_Field.Value.ToString(), Parent_Type_Field.Value.ToString(), Parent_Name, NationalID_IsExistingFunctionReturnObject);
                                //        //goto Set_Msg;
                                //    }
                                //}
                                Utility.Update_UDO(company, UDO_Info, UDO_Code, One_Line_Data.ToArray());
                                Update_Type = "Updated";
                            }
                            else if (isRecordInDB && !authorized)
                            {
                                continue;
                            }
                        }
                        else
                        {
                            if (UDO_Info.KHCF_Object == KHCF_Objects.Individual_Membership)
                            {
                                Field_Data Start_Field = One_Line_Data.FirstOrDefault(F => F.Field_Name == "U_ST_START_DATE");
                                Field_Data End_Field = One_Line_Data.FirstOrDefault(F => F.Field_Name == "U_ST_END_DATE");
                                if (Start_Field == null || End_Field == null)
                                {
                                    throw new Logic.Custom_Exception("The Start Date and End Date is Mandatory");
                                }

                                bool isPast;
                                Membership.Get_New_Renewal_StartDate(company, (DateTime)End_Field.Value, out isPast, (DateTime)Start_Field.Value);
                                if (isPast)
                                {
                                    Field_Data Status_Field = One_Line_Data.FirstOrDefault(F => F.Field_Name == "U_ST_MEMBERSHIP_STATUS");
                                    if (Status_Field != null)
                                    {
                                        Status_Field.Value = "P";
                                    }
                                }

                            }
                            Add_Missing_Columns(company, UDO_Info, One_Line_Data);
                            string Code = Utility.Add_UDO_Entry(company, UDO_Info, One_Line_Data.ToArray(), Lines_Data.ToArray());

                            if (UDO_Info.KHCF_Object == KHCF_Objects.Individual_Membership)
                            {
                                Membership.Create_Invoice(company, Code, UDO_Info);
                            }
                            if (UDO_Info.KHCF_Object == KHCF_Objects.CCI_Member_Card)
                            {
                                KHCF_Approval.Approve_MemberCard(company, Code, "Approved Automatically", UDO_Info);
                            }
                            Update_Type = "Created";
                        }

                        //Set_Msg:
                        SBO_Application.StatusBar.SetText($"The Card with National ID[{Key_ID}] has been {Update_Type} successfully!", BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Success);
                        //if(isRecordInDB)
                        //    DT_Result.SetValue("Import_Result", i, "Member already exists in the database");
                        //else
                        DT_Result.SetValue("Import_Result", i, $"{UDO_Info.Title} {Update_Type} successfully");
                    }
                    catch (Exception ex)
                    {
                        DT_Result.SetValue("Import_Result", i, ex.Message);
                    }
                }
            }
            finally
            {
                HeadersGrid.AutoResizeColumns();
                if (LinesGrid.Columns.Count != 0)
                {
                    LinesGrid.AutoResizeColumns();
                }
                form.Freeze(false);
            }
        }

        private static void Add_Missing_Columns(SAPbobsCOM.Company company, UDO_Definition UDO_Info, List<Field_Data> One_Line_Data)
        {
            Field_Data Creator_Field = One_Line_Data.FirstOrDefault(F => F.Field_Name == "U_ST_CREATOR");
            if (Creator_Field == null)
            {
                One_Line_Data.Add(new Field_Data() { Field_Name = "U_ST_CREATOR", Data_Type = BoFieldTypes.db_Alpha, Value = company.UserName });
            }
            if (UDO_Info.KHCF_Object == KHCF_Objects.Individual_Membership)
            {
                Field_Data Create_Date_Field = One_Line_Data.FirstOrDefault(F => F.Field_Name == "U_ST_CREATION_DATE");
                if (Create_Date_Field == null)
                {
                    One_Line_Data.Add(new Field_Data() { Field_Name = "U_ST_CREATION_DATE", Data_Type = BoFieldTypes.db_Alpha, Value = DateTime.Today});
                }

                Field_Data MemberCard_Field = One_Line_Data.FirstOrDefault(F => F.Field_Name == "U_ST_MEMBER_CARD");
                string MemberCard_Code = MemberCard_Field.Value.ToString();

                var Customer_Group = Utility.Get_Member_Card_Customer_Group(company, MemberCard_Code);
                One_Line_Data.Add(new Field_Data() { Field_Name = "U_ST_CUSTOMER_GROUP", Data_Type = BoFieldTypes.db_Alpha, Value = Customer_Group.Code_Name });

            }

        }

        private static object Get_Real_Value(int i, Field_Definition One_Table_Field, string Table_Value)
        {
            object Value;
            try
            {
                switch (One_Table_Field.Data_Type)
                {
                    case BoFieldTypes.db_Alpha:
                    case BoFieldTypes.db_Memo:
                        Value = Table_Value;
                        break;
                    case BoFieldTypes.db_Numeric:
                        Value = int.Parse(Table_Value);
                        break;
                    case BoFieldTypes.db_Date:
                        Value = DateTime.ParseExact(Table_Value, "yyyyMMdd", null);
                        break;
                    case BoFieldTypes.db_Float:
                        Value = double.Parse(Table_Value);
                        break;
                    default:
                        throw new Logic.Custom_Exception($"The data type[{One_Table_Field.Data_Type}] is not supported");
                        //break;
                }

            }
            catch (Exception ex)
            {
                throw new Logic.Custom_Exception($"The value[{Table_Value}] in line [{i}], column [{One_Table_Field.Field_Name}] cacnot be converted to data type[{One_Table_Field.Data_Type}][{ex.Message}]");
            }

            return Value;
        }

        private static void Select_Header_Path(ItemEvent pVal)
        {
            Form form = SBO_Application.Forms.Item(pVal.FormUID);

            ST.Helper.Dialog.BrowseFile BF = new Helper.Dialog.BrowseFile(Helper.Dialog.DialogType.OpenFile);
            BF.Filter = "CSV Files (*.csv)|*.csv";

            BF.ShowDialog();

            form.DataSources.UserDataSources.Item("4").Value = BF.FileName;

        }

        private static void Select_Lines_Path(ItemEvent pVal)
        {
            Form form = SBO_Application.Forms.Item(pVal.FormUID);

            ST.Helper.Dialog.BrowseFile BF = new Helper.Dialog.BrowseFile(Helper.Dialog.DialogType.OpenFile);
            BF.Filter = "CSV Files (*.csv)|*.csv";

            BF.ShowDialog();

            form.DataSources.UserDataSources.Item("11").Value = BF.FileName;

        }
    }
}
