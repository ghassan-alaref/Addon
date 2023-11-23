using SAPbouiCOM;
using SAPbobsCOM;
using System;
using System.Collections.Generic;
using ST.KHCF.Customization.Logic.Classes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ST.KHCF.Customization.Logic;

namespace ST.KHCF.Customization.Forms.Fundraising
{
    internal class Frm_Machinery_Installation_Det : Parent_Form
    {

        internal static SAPbobsCOM.Company company;
        internal static Application SBO_Application;
        internal static Parent_Form Form_Obj;


        internal override void Initialize_Form(Form form)
        {
            base.Initialize_Form(form);
            
            Grid grid = (Grid)form.Items.Item("Item_12").Specific;
            grid.AutoResizeColumns();

            Matrix Mat_Line = (Matrix)form.Items.Item("33").Specific;
            Mat_Line.AutoResizeColumns();
            //SAPbouiCOM.ComboBox comboBox = (SAPbouiCOM.ComboBox)form.Items.Item("10").Specific;
            //comboBox.ValidValues.Add("A", "Available");
            //comboBox.ValidValues.Add("R", "Reserved");
            //comboBox.ValidValues.Add("P", "Purchased");
            //comboBox.ValidValues.Add("E", "Excluded");
            string SQL_Year = $@"SELECT T0.""Year"" As ""Code"" , T0.""Year"" As ""Name"" FROM OACP T0";
            Helper.Utility.Fill_One_ComboBoxBySQL(company, form, "Item_11", SQL_Year);

            form.Items.Item("31").Click();
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
                Form form = SBO_Application.Forms.Item(BusinessObjectInfo.FormUID);
                if (form.Mode == BoFormMode.fm_FIND_MODE)
                {
                    form.Items.Item("37").SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 4, BoModeVisualBehavior.mvb_False);
                    form.Items.Item("38").SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 4, BoModeVisualBehavior.mvb_False);
                    Form_Obj.UnSet_Mondatory_Fields_Color(form);
                }
                if ((BusinessObjectInfo.EventType == BoEventTypes.et_FORM_DATA_UPDATE || BusinessObjectInfo.EventType == BoEventTypes.et_FORM_DATA_ADD)
                    && BusinessObjectInfo.BeforeAction)
                {
                    double neededQty = Convert.ToDouble(form.DataSources.DBDataSources.Item(0).GetValue("U_ST_NEEDED_QUANTITY", 0));
                    double quantity = 0;
                    Matrix Mat = (Matrix)form.Items.Item("33").Specific;
                    int Count = Mat.RowCount;
                    for (int i = Count; i > 0; i--)
                    {
                        EditText qtyET = (EditText)Mat.Columns.Item("Col_0").Cells.Item(i).Specific;
                        if (!string.IsNullOrEmpty(qtyET.Value))
                        {
                            quantity += Convert.ToDouble(qtyET.Value);
                        }
                    }
                    if (quantity > neededQty)
                    {
                        throw new Custom_Exception("Installed Quantity Exceeds Needed Quantity");
                    }
                    BubbleEvent = Validate_Data(BusinessObjectInfo,null);
                    if (!BubbleEvent)
                    {
                        return BubbleEvent;
                    }
                    Before_Adding_UDO(BusinessObjectInfo);
                }

               


                if (BusinessObjectInfo.EventType == BoEventTypes.et_FORM_DATA_LOAD && !BusinessObjectInfo.BeforeAction)
                {
                    Form_Data_Load(BusinessObjectInfo);
                }

                SBO_Application.StatusBar.SetText("Loading", BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_None);

            }
            catch (Exception ex)
            {
                Loader.New_Msg = ex.Message;

                BubbleEvent = false;
            }

            return BubbleEvent;
        }

        private static void Form_Data_Load(BusinessObjectInfo businessObjectInfo)
        {
            Form form = SBO_Application.Forms.Item(businessObjectInfo.FormUID);
            string Machine_ID = form.DataSources.DBDataSources.Item("@ST_MACHIN_DET").GetValue("U_ST_MACHINE_ID", 0);
            if (!string.IsNullOrEmpty(Machine_ID))
            {
                string SQL_Machine = $@"Select T0.""U_ST_MACHINE_NAME_EN"",T0.""U_ST_MACHINE_NAME_AR"" From ""@ST_MACHINERY"" T0 where T0.""Code"" = '{Machine_ID}'";
                Recordset RC_Machine = Helper.Utility.Execute_Recordset_Query(company,SQL_Machine);
                if (RC_Machine.RecordCount > 0)
                {
                    form.DataSources.UserDataSources.Item("NAME_EN").Value = RC_Machine.Fields.Item("U_ST_MACHINE_NAME_EN").Value.ToString();
                    form.DataSources.UserDataSources.Item("NAME_AR").Value = RC_Machine.Fields.Item("U_ST_MACHINE_NAME_AR").Value.ToString();
                }
            }
           
             Load_Summary(form);
            Form_Obj.Set_Fields(form);
            //string SQL_Invoice = $@"Select ""DocEntry""  AS ""Code"", ""DocNum"" AS ""Name"" FROM OINV Where ""DocEntry"" = 0{form.DataSources.DBDataSources.Item(0).GetValue("U_ST_INVOICE_NUMBER", 0)}";
            //Helper.Utility.Fill_One_ComboBoxBySQL(company, form, "87", SQL_Invoice, false);


        }

        internal static void Add_Line(ItemEvent pVal, string Line_DataSource_Table, string Datatable_Id, string Matrix_Id, string Code_Col, bool isDBTable)
        {
            Form form = SBO_Application.Forms.Item(pVal.FormUID);
            DataTable DT = form.DataSources.DataTables.Item(Datatable_Id);
            form.Freeze(true);
            DBDataSource DS_Lines = form.DataSources.DBDataSources.Item(Line_DataSource_Table);
            Matrix Mat_Lines = (Matrix)form.Items.Item(Matrix_Id).Specific;
            Mat_Lines.FlushToDataSource();
            int Count = DS_Lines.Size;
            if (isDBTable)
            {
                if (Count == 1)
                {
                    if (DS_Lines.GetValue(Code_Col, Count - 1) != "")
                    {
                        DS_Lines.InsertRecord(Count);
                    }
                    else
                    {
                        Count = 0;

                        DS_Lines.InsertRecord(Count);
                        Mat_Lines.LoadFromDataSource();
                        Mat_Lines.DeleteRow(1);
                        Mat_Lines.FlushToDataSource();
                        Mat_Lines.LoadFromDataSource();
                    }
                }
                else
                {
                    DS_Lines.InsertRecord(Count);
                }
                if (DS_Lines.Size > DT.Rows.Count)
                {
                    DT.Rows.Add();
                }
            }
            else
            {
                if (Count == 1)
                {
                    if (DT.GetValue(Code_Col, Count - 1) != "")
                    {
                        DS_Lines.InsertRecord(Count);
                    }
                    else
                    {
                        Count = 0;

                        DS_Lines.InsertRecord(Count);
                        Mat_Lines.LoadFromDataSource();
                        Mat_Lines.DeleteRow(1);
                        Mat_Lines.FlushToDataSource();
                        Mat_Lines.LoadFromDataSource();
                    }
                }
                else
                {
                    DS_Lines.InsertRecord(Count);
                }
                if (DS_Lines.Size > DT.Rows.Count)
                {
                    DT.Rows.Add();
                }

            }

            Mat_Lines.LoadFromDataSource();
            //Set_Default_Value_Before_Adding(form);

            if (form.Mode == BoFormMode.fm_OK_MODE)
            {
                form.Mode = BoFormMode.fm_UPDATE_MODE;
            }
            Mat_Lines.AutoResizeColumns();
            form.Freeze(false);

        }

        internal static void Remove_Selected_Lines(ItemEvent pVal, string Datatabe_Id, string Matrix_Id)
        {
            Form form = SBO_Application.Forms.Item(pVal.FormUID);
            DataTable DT_Orphans_Details = form.DataSources.DataTables.Item(Datatabe_Id);
            Matrix Mat = (Matrix)form.Items.Item(Matrix_Id).Specific;
            int Count = Mat.RowCount;
            for (int i = Count; i > 0; i--)
            {
                //CheckBox Chk_Selected = (CheckBox)Mat.GetCellSpecific("SELECTED", i);
                CheckBox Chk_Selected = (CheckBox)Mat.Columns.Item("SELECTED").Cells.Item(i).Specific;
                if (Chk_Selected.Checked)
                {
                    Mat.DeleteRow(i);
                    DT_Orphans_Details.Rows.Remove(i - 1);
                    // form.DataSources.DBDataSources.Item(1).RemoveRecord(Grid_Deleted_Row +1);
                    Mat.FlushToDataSource();
                    Mat.LoadFromDataSource();

                }
            }
            if (form.Mode == BoFormMode.fm_OK_MODE)
            {
                form.Mode = BoFormMode.fm_UPDATE_MODE;
            }
        }

        private static void Before_Adding_UDO(BusinessObjectInfo businessObjectInfo)
        {
            Form form = SBO_Application.Forms.Item(businessObjectInfo.FormUID);
            if (form.Mode == BoFormMode.fm_ADD_MODE)
            {
                Set_Default_Value_Before_Adding(form);
                //Update_Machine(form);
            }
        }

        private static void Set_Default_Value_Before_Adding(Form form)
        {
            if (form.Mode == BoFormMode.fm_ADD_MODE)
            {
                string New_UDO_Code = Utility.Get_New_UDO_Code(company, Form_Obj.KHCF_Object);
                form.DataSources.DBDataSources.Item("@ST_MACHIN_DET").SetValue("Code", 0, New_UDO_Code);
            }

        }

        private static bool Validate_Data(BusinessObjectInfo businessObjectInfo,ItemEvent pVal)
        {
            Form form = null;
            if (pVal == null)
              form = SBO_Application.Forms.Item(businessObjectInfo.FormUID);
            else
              form = SBO_Application.Forms.Item(pVal.FormUID);
           
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
                if ((pVal.ItemUID == "30") && pVal.EventType == BoEventTypes.et_CHOOSE_FROM_LIST && !pVal.BeforeAction)
                {
                    Frm_Expected_Donations.Choose_General_Item_ID(pVal);
                }
                if (pVal.ItemUID == "33" && pVal.ColUID == "Col_3" && pVal.EventType == BoEventTypes.et_CHOOSE_FROM_LIST && !pVal.BeforeAction)
                {
                    Choose_Area(pVal);
                }
                if (pVal.ItemUID == "Item_9"  && pVal.EventType == BoEventTypes.et_CHOOSE_FROM_LIST && !pVal.BeforeAction)
                {
                    Choose_Machine(pVal);
                }

                if (pVal.ItemUID == "60" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Add_Attachment(pVal);
                }
                if (pVal.ItemUID == "61" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Remove_Attachment(pVal);
                }
                if (pVal.ItemUID == "62" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Open_Attachment(pVal);
                }

                if (pVal.ItemUID == "37" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Add_Line(pVal, "@ST_MACHIN_DET_L", "Info_Details", "33", "U_ST_INSTALLED_QUANTITY", true);
                }
                if (pVal.ItemUID == "38" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Remove_Selected_Lines(pVal, "Info_Details", "33");
                }
                //if (pVal.ItemUID == "1" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                //{
                //    Validate_Data(null, pVal);
                //    Add_UDO(pVal);
                //}

            }
            catch (Exception ex)
            {
                Form form = SBO_Application.Forms.Item(pVal.FormUID);
                form.Freeze(false);
                SBO_Application.StatusBar.SetText(ex.Message, BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Error);
            }
        }

        private static void Add_Attachment(ItemEvent pVal)
        {
            Form form = SBO_Application.Forms.Item(pVal.FormUID);

            ST.Helper.Dialog.BrowseFile BF = new Helper.Dialog.BrowseFile(Helper.Dialog.DialogType.OpenFile);

            BF.ShowDialog();
            if (BF.FileName == "")
            {
                return;
            }
            form.Freeze(true);
            DBDataSource DS_Attachment = form.DataSources.DBDataSources.Item("@ST_MACHIN_ATT");
            Matrix Mat_Add = (Matrix)form.Items.Item("63").Specific;
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
            DS_Attachment.SetValue("LineId", Count, "-1");

            //DS_Address.SetValue("U_ST_COUNTRY", Count, "JO");

            Mat_Add.LoadFromDataSource();
            Mat_Add.AutoResizeColumns();
            form.Freeze(false);

        }

        private static void Remove_Attachment(ItemEvent pVal)
        {
            Form form = SBO_Application.Forms.Item(pVal.FormUID);

            Form_Obj.Remove_Matrix_Row(form, "63");

        }

        private static void Open_Attachment(ItemEvent pVal)
        {
            Form form = SBO_Application.Forms.Item(pVal.FormUID);


            Matrix Mat = (Matrix)form.Items.Item("63").Specific;
            for (int i = 0; i < Mat.RowCount; i++)
            {
                CheckBox Chk_Selected = (CheckBox)Mat.GetCellSpecific("SELECTED", i + 1);
                if (Chk_Selected.Checked)
                {
                    EditText Txt_FileName = (EditText)Mat.GetCellSpecific("FileName", i + 1);
                    System.Diagnostics.Process.Start(Txt_FileName.Value);
                }
            }

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
            if (SBO_Application.Forms.ActiveForm.Mode != BoFormMode.fm_FIND_MODE)
            {
                Form_Obj.Set_Fields(SBO_Application.Forms.ActiveForm);
            }
            else if (SBO_Application.Forms.ActiveForm.Mode == BoFormMode.fm_FIND_MODE)
            {
                Form_Obj.UnSet_Mondatory_Fields_Color(SBO_Application.Forms.ActiveForm);
                SBO_Application.Forms.ActiveForm.DataSources.UserDataSources.Item("NAME_EN").Value = string.Empty;
                SBO_Application.Forms.ActiveForm.DataSources.UserDataSources.Item("NAME_AR").Value = string.Empty;
            }
            if (SBO_Application.Forms.ActiveForm.Mode == BoFormMode.fm_FIND_MODE)
            {
                Form form = SBO_Application.Forms.ActiveForm;
                DisableMatrixButtons(form);
            }
        }

        private static void Choose_Area(ItemEvent pVal)
        {
            SAPbouiCOM.Form form = SBO_Application.Forms.Item(pVal.FormUID);
            ChooseFromListEvent Chos_Event = (ChooseFromListEvent)pVal;
            string Area = "";
            if ((Chos_Event.SelectedObjects == null ? false : Chos_Event.SelectedObjects.Rows.Count != 0))
            {
                Area = Chos_Event.SelectedObjects.GetValue("U_ST_AREA_NAME", 0).ToString();
                Matrix Mat_Line = (Matrix)form.Items.Item(pVal.ItemUID).Specific;
                Mat_Line.SetCellWithoutValidation(pVal.Row, pVal.ColUID, Area);
            }

        }

        private static void Choose_Machine(ItemEvent pVal)
        {
            SAPbouiCOM.Form form = SBO_Application.Forms.Item(pVal.FormUID);
            ChooseFromListEvent Chos_Event = (ChooseFromListEvent)pVal;
            string Code = "", Name_EN = "", Status = "", Name_AR = "";
            if ((Chos_Event.SelectedObjects == null ? false : Chos_Event.SelectedObjects.Rows.Count != 0))
            {
                Code = Chos_Event.SelectedObjects.GetValue("Code", 0).ToString();
                Name_EN = Chos_Event.SelectedObjects.GetValue("U_ST_MACHINE_NAME_EN", 0).ToString();
                Status = Chos_Event.SelectedObjects.GetValue("U_ST_MACHINE_STATUS", 0).ToString();
                Name_AR = Chos_Event.SelectedObjects.GetValue("U_ST_MACHINE_NAME_AR", 0).ToString();  //
                form.DataSources.DBDataSources.Item("@ST_MACHIN_DET").SetValue("U_ST_MACHINE_ID", 0, Code);
                form.DataSources.DBDataSources.Item("@ST_MACHIN_DET").SetValue("U_ST_MACHINE_STATUS", 0, Status);
                form.DataSources.UserDataSources.Item("NAME_EN").Value = Name_EN;
                form.DataSources.UserDataSources.Item("NAME_AR").Value = Name_AR;

            }

        }

        private static void Add_UDO(ItemEvent pVal)
        {
            Form form = SBO_Application.Forms.Item(pVal.FormUID);
            int index = -1;
            if (form.Mode == BoFormMode.fm_ADD_MODE)
            {
                DBDataSource DS_Lines = form.DataSources.DBDataSources.Item("@ST_MACHIN_INST_INFO");
                Matrix Mat_Lines = (Matrix)form.Items.Item("33").Specific;

                Field_Data[] Field_Datas = new Field_Data[7];
                string Code = form.DataSources.UserDataSources.Item("Code").Value.ToString();
                string Location = form.DataSources.UserDataSources.Item("Loc").Value.ToString();
                string E_installation_date = form.DataSources.UserDataSources.Item("EDate").ValueEx.ToString();
                E_installation_date = E_installation_date.Replace(".", "/");
                string A_installation_date = form.DataSources.UserDataSources.Item("ADate").ValueEx.ToString();
                A_installation_date = A_installation_date.Replace(".", "/");


                for (int i = 0; i < Mat_Lines.RowCount; i++)
                {
                    EditText Quantity = (EditText)Mat_Lines.GetCellSpecific("Col_0", i + 1);
                    EditText Price_JOD = (EditText)Mat_Lines.GetCellSpecific("Col_1", i + 1);
                    EditText Price_USD = (EditText)Mat_Lines.GetCellSpecific("Col_2", i + 1);
                    ComboBox Loc = (ComboBox)Mat_Lines.GetCellSpecific("Col_3", i + 1);

                    Field_Datas[0] = new Field_Data() { Field_Name = "U_ST_NEEDED_QUANTITY", Value = Quantity.Value };
                    Field_Datas[1] = new Field_Data() { Field_Name = "U_ST_MACHINE_PRICE_JOD", Value = Price_JOD.Value };
                    Field_Datas[2] = new Field_Data() { Field_Name = "U_ST_MACHINE_PRICE_USD", Value = Price_USD.Value };
                    Field_Datas[3] = new Field_Data() { Field_Name = "U_ST_MACHINE_LOCATION", Value = Loc.Value };
                    Field_Datas[4] = new Field_Data() { Field_Name = "U_ST_MACHINE_LOCATION_IN_KHCC", Value = Location };
                    Field_Datas[5] = new Field_Data() { Field_Name = "U_ST_MACHINE_EXPECTED_INSTALLATION_DATE", Value = E_installation_date };
                    Field_Datas[6] = new Field_Data() { Field_Name = "U_ST_MACHINE_ACTUAL_INSTALLATION_DATE", Value = A_installation_date };

                    //Add_Entry(DS_Lines.GetValue("Code", i).ToString(), UDO_Info, Field_Datas);

                }

                if (form.Mode == BoFormMode.fm_ADD_MODE)
                {
                    form.Mode = BoFormMode.fm_OK_MODE;
                }
            }
            else if (form.Mode == BoFormMode.fm_FIND_MODE)
            {
                string Code = form.DataSources.UserDataSources.Item("Code").Value;
                EditText edit = (EditText)form.Items.Item("5").Specific;
                string Value = edit.Value;
                Code = Value;
                if (!string.IsNullOrEmpty(Code))
                {
                    string SQL_Machinery = $@"Select T0.""U_ST_MACHINE_NAME_EN"", T0.""U_ST_MACHINE_NAME_AR"", T0.""U_ST_MACHINE_STATUS"", T1.""U_ST_NEEDED_QUANTITY"", T1.""U_ST_MACHINE_PRICE_JOD"", T1.""U_ST_MACHINE_PRICE_USD"", T1.""U_ST_MACHINE_LOCATION"", T1.""U_ST_MACHINE_LOCATION_IN_KHCC"", T1.""U_ST_MACHINE_EXPECTED_INSTALLATION_DATE"", T1.""U_ST_MACHINE_ACTUAL_INSTALLATION_DATE"" from ""@ST_MACHINERY"" T0 inner join ""@ST_MACHIN_INST_INFO""  T1 on T0.""Code"" = T1.""Code"" where T0.""Code""= '{Code}'";
                    DBDataSource DS_Lines = form.DataSources.DBDataSources.Item("@ST_MACHIN_INST_INFO");
                    Matrix Mat_Lines = (Matrix)form.Items.Item("33").Specific;
                    Recordset RC = Helper.Utility.Execute_Recordset_Query(company,SQL_Machinery);
                    if (RC.RecordCount > 0)
                    {
                        form.DataSources.UserDataSources.Item("Status").Value = RC.Fields.Item("U_ST_MACHINE_STATUS").Value.ToString();
                        form.DataSources.UserDataSources.Item("NAME_EN").Value = RC.Fields.Item("U_ST_MACHINE_NAME_EN").Value.ToString();
                        form.DataSources.UserDataSources.Item("NAME_AR").Value = RC.Fields.Item("U_ST_MACHINE_NAME_AR").Value.ToString();
                        form.DataSources.UserDataSources.Item("Loc").Value = RC.Fields.Item("U_ST_MACHINE_LOCATION_IN_KHCC").Value.ToString();
                    }
                    for (int i = 0; i < RC.RecordCount; i++)
                    {
                        string Q = RC.Fields.Item("U_ST_NEEDED_QUANTITY").Value.ToString();
                        if (!string.IsNullOrEmpty(Q) && Q !="0")
                        {
                            DateTime E_Date, A_Date;
                            DateTime.TryParse(RC.Fields.Item("U_ST_MACHINE_EXPECTED_INSTALLATION_DATE").Value.ToString(), out E_Date);
                            DateTime.TryParse(RC.Fields.Item("U_ST_MACHINE_ACTUAL_INSTALLATION_DATE").Value.ToString(), out A_Date);
                            if(E_Date.Year != 1899 && E_Date.Year != 0001)
                             form.DataSources.UserDataSources.Item("EDate").Value = E_Date.ToString("yyyyMMdd");
                            if (A_Date.Year != 1899 && A_Date.Year != 0001)
                                form.DataSources.UserDataSources.Item("ADate").Value = E_Date.ToString("yyyyMMdd");
                            //form.DataSources.UserDataSources.Item("ADate").Value = A_Date.ToString("yyyyMMdd");
                        }
                        DS_Lines.Clear();
                        if (!string.IsNullOrEmpty(Q) && Q != "0")
                        {
                            index++;
                            DS_Lines.InsertRecord(index);
                            DS_Lines.SetValue("U_ST_NEEDED_QUANTITY", index, RC.Fields.Item("U_ST_NEEDED_QUANTITY").Value.ToString());
                            DS_Lines.SetValue("U_ST_MACHINE_PRICE_JOD", index, RC.Fields.Item("U_ST_MACHINE_PRICE_JOD").Value.ToString());
                            DS_Lines.SetValue("U_ST_MACHINE_PRICE_USD", index, RC.Fields.Item("U_ST_MACHINE_PRICE_USD").Value.ToString());
                            DS_Lines.SetValue("U_ST_MACHINE_LOCATION", index, RC.Fields.Item("U_ST_MACHINE_LOCATION").Value.ToString());
                        }
                        RC.MoveNext();
                    }

                    if (index != -1)
                    {
                        Mat_Lines.LoadFromDataSource();
                        //Mat_Lines.DeleteRow(Mat_Lines.RowCount-1);
                        Mat_Lines.FlushToDataSource();
                        Mat_Lines.LoadFromDataSource();
                    }
                }
            }
        }

        internal static void Add_Entry(string Code, UDO_Definition UDO_Info, Field_Data[] Field_Datas)
        {
            try
            {
                CompanyService oCmpSrv = company.GetCompanyService();
                GeneralService oGeneralService = oCmpSrv.GetGeneralService(Utility.Get_UDO_Type_ID(UDO_Info.KHCF_Object));
                GeneralDataParams oGeneralParams = (GeneralDataParams)oGeneralService.GetDataInterface(GeneralServiceDataInterfaces.gsGeneralDataParams);
                oGeneralParams.SetProperty("Code", Code);
                GeneralData oGeneralData = oGeneralService.GetByParams(oGeneralParams);
                SAPbobsCOM.GeneralDataCollection Installation_Children = oGeneralData.Child("ST_MACHIN_INST_INFO");
                SAPbobsCOM.GeneralData oChild = Installation_Children.Add();

                string UDO_Unique_ID;
                foreach (Field_Data OneField in Field_Datas.Where(x => !string.IsNullOrEmpty(x.Value.ToString()) || !x.Value.ToString().Contains('?')))
                {
                    if (OneField.Is_Temp == true)
                    {
                        continue;
                    }
                    if (OneField.Value != null)
                        oChild.SetProperty(OneField.Field_Name, OneField.Value.ToString());
                }

                oGeneralService.Update(oGeneralData);

            }
            catch (Exception ex)
            {
                throw new Logic.Custom_Exception($"Error during add the UDO[{UDO_Info.Table_Name}], [{ex.Message}]");
            }
        }

        private static void Update_Machine(Form form)
        {
            string Code = form.DataSources.DBDataSources.Item("@ST_MACHIN_INST_DET").GetValue("U_ST_MACHINE_ID", 0);
            UDO_Definition UDO_Info = Logic.Objects_Logic.All_UDO_Definition.FirstOrDefault(U => U.KHCF_Object == KHCF_Objects.Machinery);

            Field_Data Fld = new Field_Data() { Field_Name = "U_ST_MACHINE_STATUS", Value = form.DataSources.UserDataSources.Item("Status").Value };
            Utility.Update_UDO(company, UDO_Info, Code, new[] { Fld });
        }

        private static void Load_Summary(Form form)
        {
            string Code = form.DataSources.DBDataSources.Item(0).GetValue("Code", 0);
            string Machine_ID = form.DataSources.DBDataSources.Item(0).GetValue("U_ST_MACHINE_ID", 0);

            if (!string.IsNullOrEmpty(Code))
            {
                DataTable DT_Summery = form.DataSources.DataTables.Item("Summery");
                DT_Summery.Rows.Clear();
                string SUM_SQL = $@"SELECT Sum(T2.""U_ST_QUANTITY"") as ""SQ"" ,Sum(T3.""U_ST_NEEDED_QUANTITY"") as ""NQ"" FROM  ""@ST_ACTUAL_DONATIONS""  T0 inner join ""@ST_FUND_TARGET"" T1 on  T0.""U_ST_TARGET"" = T1.""Code""  inner join ""@ST_TARGET_MACHINES""  T2 on T1.""Code"" = T2.""Code"" inner join   ""@ST_MACHIN_DET""  T3  on T3.""U_ST_MACHINE_ID"" = T2.""U_ST_MACHINE_ID"" where YEAR(T0.""U_ST_DONATION_DATE"") = T3.""U_ST_YEAR"" And T3.""Code""='{Code}' And T3.""U_ST_MACHINE_STATUS"" = 'P'";
                string SUM_SQL_A = $@"SELECT Sum(T2.""U_ST_QUANTITY"") as ""SQ"" ,Sum(T3.""U_ST_NEEDED_QUANTITY"") as ""NQ"" FROM  ""@ST_EXPEC_DONATION""  T0 inner join ""@ST_FUND_TARGET"" T1 on  T0.""U_ST_TARGET"" = T1.""Code""  inner join ""@ST_TARGET_MACHINES""  T2 on T1.""Code"" = T2.""Code"" inner join   ""@ST_MACHIN_DET""  T3  on T3.""U_ST_MACHINE_ID"" = T2.""U_ST_MACHINE_ID"" where YEAR(T0.""U_ST_DONATION_DATE"") = T3.""U_ST_YEAR"" And T3.""Code""='{Code}' And T3.""U_ST_MACHINE_STATUS"" = 'R'";
                string SQL_Summery = $@"SELECT  T2.""U_ST_QUANTITY"",T3.""U_ST_NEEDED_QUANTITY"",T3.""U_ST_MACHINE_PRICE_JOD"",T3.""U_ST_MACHINE_PRICE_USD""  FROM  ""@ST_ACTUAL_DONATIONS""  T0 inner join ""@ST_FUND_TARGET"" T1 on  T0.""U_ST_TARGET"" = T1.""Code""  inner join ""@ST_TARGET_MACHINES""  T2 on T1.""Code"" = T2.""Code"" inner join   ""@ST_MACHIN_DET""  T3  on T3.""U_ST_MACHINE_ID"" = T2.""U_ST_MACHINE_ID"" where YEAR(T0.""U_ST_DONATION_DATE"") = T3.""U_ST_YEAR"" And T3.""Code""='{Code}' AND T3.""U_ST_MACHINE_STATUS"" = 'P'";
                Recordset RC_Summery = Helper.Utility.Execute_Recordset_Query(company, SQL_Summery);
                Recordset RC_Summery_P = Helper.Utility.Execute_Recordset_Query(company, SUM_SQL);
                Recordset RC_Summery_R = Helper.Utility.Execute_Recordset_Query(company, SUM_SQL_A);
                DT_Summery.Rows.Add(1);

                string Full_SQL = $@"SELECT sum(T0.""U_ST_NEEDED_QUANTITY"") As ""NQ"" FROM ""@ST_MACHIN_DET""  T0 WHERE T0.""U_ST_MACHINE_ID"" ='{Machine_ID}'";
                string Price_SQL = $@"SELECT T0.""U_ST_MACHINE_PRICE_JOD"" , T0.""U_ST_MACHINE_PRICE_USD"" FROM ""@ST_MACHIN_DET""  T0 WHERE T0.""U_ST_MACHINE_ID"" ='{Machine_ID}'";
                Recordset Full_RC = Helper.Utility.Execute_Recordset_Query(company, Full_SQL);
                Recordset Price_RC = Helper.Utility.Execute_Recordset_Query(company, Price_SQL);
                int Quantity = Convert.ToInt32(RC_Summery_P.Fields.Item("NQ").Value);
                double Price_JOD = Convert.ToDouble(RC_Summery.Fields.Item("U_ST_MACHINE_PRICE_JOD").Value);
                double Price_USD = Convert.ToDouble(RC_Summery.Fields.Item("U_ST_MACHINE_PRICE_USD").Value);

                if (Full_RC.RecordCount > 0)
                {
                    Quantity = Convert.ToInt32(Full_RC.Fields.Item(0).Value.ToString());
                    Price_JOD = Convert.ToDouble(Price_RC.Fields.Item("U_ST_MACHINE_PRICE_JOD").Value.ToString());
                    Price_USD = Convert.ToDouble(Price_RC.Fields.Item("U_ST_MACHINE_PRICE_USD").Value.ToString());
                }
               
                int SQuantity = Convert.ToInt32(RC_Summery_P.Fields.Item("SQ").Value);
                int RQuantity = Convert.ToInt32(RC_Summery_R.Fields.Item("SQ").Value);
                
                double S_Result_JOD = SQuantity * Price_JOD;
                double S_Result_USD = SQuantity * Price_USD;
                double R_Result_JOD = RQuantity * Price_JOD;
                double R_Result_USD = RQuantity * Price_USD;
                double Result_JOD = Quantity * Price_JOD;
                double Result_USD = Quantity * Price_USD;
                DT_Summery.SetValue("Quantity", 0,Quantity.ToString());
                //DT_Summery.SetValue("Location", i, RC_Summery.Fields.Item("U_ST_MACHINE_LOCATION").Value);
                DT_Summery.SetValue("Total_Price_JOD", 0, Result_JOD.ToString("N03"));
                DT_Summery.SetValue("Total_Price_USD", 0, Result_USD.ToString("N03"));
                DT_Summery.SetValue("S_Quantity", 0, RC_Summery_P.Fields.Item("SQ").Value);
                DT_Summery.SetValue("S_Price_JOD", 0, S_Result_JOD.ToString("N03"));
                DT_Summery.SetValue("S_Price_USD", 0, S_Result_USD.ToString("N03"));
                DT_Summery.SetValue("R_Quantity", 0, RC_Summery_R.Fields.Item("SQ").Value);
                DT_Summery.SetValue("R_Price_JOD", 0, R_Result_JOD.ToString("N03"));
                DT_Summery.SetValue("R_Price_USD", 0, R_Result_USD.ToString("N03"));

                //for (int i = 0; i < RC_Summery.RecordCount; i++)
                //{
                //    int Quantity = Convert.ToInt32(RC_Summery.Fields.Item("U_ST_NEEDED_QUANTITY").Value);
                //    int SQuantity = Convert.ToInt32(RC_Summery.Fields.Item("U_ST_QUANTITY").Value);
                //    double Price_JOD = Convert.ToDouble(RC_Summery.Fields.Item("U_ST_MACHINE_PRICE_JOD").Value);
                //    double Price_USD = Convert.ToDouble(RC_Summery.Fields.Item("U_ST_MACHINE_PRICE_USD").Value);
                //    double S_Result_JOD = SQuantity * Price_JOD;
                //    double S_Result_USD = SQuantity * Price_USD;
                //    double Result_JOD = Quantity * Price_JOD;
                //    double Result_USD = Quantity * Price_USD;
                //    DT_Summery.SetValue("Needed Quantity", i, RC_Summery.Fields.Item("U_ST_NEEDED_QUANTITY").Value);
                //    //DT_Summery.SetValue("Location", i, RC_Summery.Fields.Item("U_ST_MACHINE_LOCATION").Value);
                //    DT_Summery.SetValue("Total_Price_JOD", i, Result_JOD.ToString("N03"));
                //    DT_Summery.SetValue("Total_Price_USD", i, Result_USD.ToString("N03"));
                //    DT_Summery.SetValue("S_Quantity", i, RC_Summery.Fields.Item("U_ST_QUANTITY").Value);
                //    DT_Summery.SetValue("S_Price_JOD", i, S_Result_JOD.ToString("N03"));
                //    DT_Summery.SetValue("S_Price_USD", i,S_Result_USD.ToString("N03"));
                //    RC_Summery.MoveNext();
                //}
            }
        }

        private static void DisableMatrixButtons(Form form)
        {
            List<string> GridBtnsIds = new List<string>() { "37", "38" };// "116", "115", "119", "118", "129", "128", "601", "600" };
            for (int i = 0; i < GridBtnsIds.Count; i++)
            {
                form.Items.Item(GridBtnsIds[i]).SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 4, BoModeVisualBehavior.mvb_False);



            }
        }
    }
}
