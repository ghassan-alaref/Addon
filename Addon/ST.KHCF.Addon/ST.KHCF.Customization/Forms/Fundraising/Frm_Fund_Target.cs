using SAPbobsCOM;
using SAPbouiCOM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ST.KHCF.Customization.Forms.Fundraising
{
    class Frm_Fund_Target:Parent_Form
    {
        internal static SAPbobsCOM.Company company;
        internal static Application SBO_Application;
        internal static Parent_Form Form_Obj;
        internal static string[] Matrix_Item_IDs = new string[] { "112", "117", "120", "130", "602" };


        internal override void Initialize_Form(Form form)
        {
            base.Initialize_Form(form);

            SAPbouiCOM.ChooseFromList CFL_Doner = form.ChooseFromLists.Item("CFL_Areas");
            Conditions Doner_Cons = CFL_Doner.GetConditions();
            Condition Doner_Con = Doner_Cons.Add();
            Doner_Con.Alias = "U_ST_STATUS";
            Doner_Con.CondVal = "A";
            Doner_Con.Operation = BoConditionOperation.co_EQUAL;
            CFL_Doner.SetConditions(Doner_Cons);

            string wishItemGroup = Utility.Get_Configuration(company, "Wish_Item_Group", "Wish Item Group Code", "115");
            SAPbouiCOM.ChooseFromList CFL_Wish = form.ChooseFromLists.Item("CFL_Wishes");
            Conditions wish_Cons = CFL_Wish.GetConditions();
            Condition wish_Con = wish_Cons.Add();
            wish_Con.Alias = "ItmsGrpCod";
            wish_Con.CondVal = wishItemGroup;
            wish_Con.Operation = BoConditionOperation.co_EQUAL;
            CFL_Wish.SetConditions(wish_Cons);


            SAPbouiCOM.ChooseFromList CFL_Orph = form.ChooseFromLists.Item("CFL_Orphans");
            Conditions orph_Cons = CFL_Orph.GetConditions();
            Condition orph_con = orph_Cons.Add();
            orph_con.Alias = "GroupCode";
            orph_con.CondVal = "110";
            orph_con.Operation = BoConditionOperation.co_EQUAL;
            CFL_Orph.SetConditions(orph_Cons);

            Matrix Mat_Lines = (Matrix)form.Items.Item("602").Specific;
            if (Mat_Lines.RowCount > 0)
            {
                string SQL = $@"SELECT T0.""U_ST_CAN_EXTEND_RESERVATION"" FROM OUSR T0 WHERE T0.""USER_CODE"" = '{company.UserName}'";
                Recordset RC = Helper.Utility.Execute_Recordset_Query(company, SQL);
                if (RC.Fields.Item("U_ST_CAN_EXTEND_RESERVATION").Value.ToString() == "Y")
                {
                    Mat_Lines.Columns.Item("Date").Editable = true;
                }
                else
                {
                  Mat_Lines.Columns.Item("Date").Editable = false;
                }
            }


            form.Items.Item("Item_38").Click();
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

                Code_value = SBO_Application.Forms.ActiveForm.TypeEx.Replace("ST", "Frm");
                if ((BusinessObjectInfo.EventType == BoEventTypes.et_FORM_DATA_UPDATE || BusinessObjectInfo.EventType == BoEventTypes.et_FORM_DATA_ADD)
                    && BusinessObjectInfo.BeforeAction)
                {

                    BubbleEvent = Validate_Data(BusinessObjectInfo);
                    if (!BubbleEvent)
                    {
                        return BubbleEvent;
                    }
                    Before_Adding_UDO(BusinessObjectInfo);
                }

                if ((BusinessObjectInfo.EventType == BoEventTypes.et_FORM_DATA_UPDATE || BusinessObjectInfo.EventType == BoEventTypes.et_FORM_DATA_ADD) && BusinessObjectInfo.ActionSuccess && !BusinessObjectInfo.BeforeAction)
                {
                    ADD_Update_UDO(BusinessObjectInfo);
                }
                if (BusinessObjectInfo.EventType == BoEventTypes.et_FORM_DATA_LOAD && !BusinessObjectInfo.BeforeAction)
                {
                    Form_Data_Load(BusinessObjectInfo);
                }
                Form form = SBO_Application.Forms.Item(BusinessObjectInfo.FormUID);
                if (form.Mode == BoFormMode.fm_FIND_MODE)
                {
                    Form_Obj.UnSet_Mondatory_Fields_Color(form);
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

        private static void ADD_Update_UDO(BusinessObjectInfo businessObjectInfo)
        {
            Form form = SBO_Application.Forms.Item(businessObjectInfo.FormUID);
            System.Xml.XmlDocument XML_Doc = new System.Xml.XmlDocument();
            string XML_Text = businessObjectInfo.ObjectKey.Replace(" ", "").Replace("=", " = ");
            XML_Doc.LoadXml(XML_Text);
            string UDO_Code = XML_Doc.GetElementsByTagName("Code")[0].InnerText;

            string Source_Form_ID = form.DataSources.UserDataSources.Item("FORM_ID").Value;
            if (Source_Form_ID != "")
            {
                string Source_Item_ID = form.DataSources.UserDataSources.Item("ITEM_ID").Value;
                Form Source_Form = SBO_Application.Forms.Item(Source_Form_ID);
                string Source_Field_Name = Helper.Utility.Get_Item_DB_Datasource(Source_Form.Items.Item(Source_Item_ID));
                string Source_Object_Id = form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).GetValue("U_ST_SOURCE_OBJECT_ID", 0);

                Source_Form.DataSources.DBDataSources.Item(Source_Object_Id).SetValue(Source_Field_Name, 0, UDO_Code);

                if (Source_Form.Mode == BoFormMode.fm_OK_MODE)
                {
                    Source_Form.Mode = BoFormMode.fm_UPDATE_MODE;
                }

                System.Threading.Thread t = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(Parent_Form.Run_Close_Form_Thread));
                t.Start(form.UniqueID);

            }


        }


        private static void Form_Data_Load(BusinessObjectInfo businessObjectInfo)
        {
            Form form = SBO_Application.Forms.Item(businessObjectInfo.FormUID);
            string UDO_Code = form.DataSources.DBDataSources.Item(0).GetValue("Code", 0);
            //string SQL_Invoice = $@"Select ""DocEntry""  AS ""Code"", ""DocNum"" AS ""Name"" FROM OINV Where ""DocEntry"" = 0{form.DataSources.DBDataSources.Item(0).GetValue("U_ST_INVOICE_NUMBER", 0)}";
            //Helper.Utility.Fill_One_ComboBoxBySQL(company, form, "87", SQL_Invoice, false);
            Matrix Mat_Lines = (Matrix)form.Items.Item("602").Specific;
            if (Mat_Lines.RowCount > 0)
            {
                string SQL = $@"SELECT T0.""U_ST_CAN_EXTEND_RESERVATION"" FROM OUSR T0 WHERE T0.""USER_CODE"" = '{company.UserName}'";
                Recordset RC = Helper.Utility.Execute_Recordset_Query(company, SQL);
                if (RC.Fields.Item("U_ST_CAN_EXTEND_RESERVATION").Value.ToString() == "Y")
                {
                    Mat_Lines.Columns.Item("Date").Editable = true;
                }
                else
                {
                    Mat_Lines.Columns.Item("Date").Editable = false;
                }
            }

            Form_Obj.Set_Fields(form);

            form.Freeze(true);

            form.Freeze(false);
        }

        private static void Before_Adding_UDO(BusinessObjectInfo businessObjectInfo)
        {
            Form form = SBO_Application.Forms.Item(businessObjectInfo.FormUID);
            if (form.Mode == BoFormMode.fm_ADD_MODE)
            {
                Set_Default_Value_Before_Adding(form);
            }
        }

        private static void Set_Default_Value_Before_Adding(Form form)
        {
            if (form.Mode == BoFormMode.fm_ADD_MODE)
            {
                string New_UDO_Code = Utility.Get_New_UDO_Code(company, Form_Obj.KHCF_Object);
                form.DataSources.DBDataSources.Item(0).SetValue("Code", 0, New_UDO_Code);
                Update_Amount();

                //form.DataSources.DBDataSources.Item(0).SetValue("U_ST_CREATOR", 0, company.UserName);
                //form.DataSources.DBDataSources.Item(0).SetValue("U_ST_PARENT_TYPE", 0, "N");
            }

        }

        private static bool Validate_Data(BusinessObjectInfo businessObjectInfo)
        {
            Form form = SBO_Application.Forms.Item(businessObjectInfo.FormUID);

            if (!Form_Obj.Validate_Data(form))
            {
                return false;
            }

            foreach (string OneMat in Matrix_Item_IDs)
            {
                Matrix Mat = (Matrix)form.Items.Item(OneMat).Specific;
                string Code_Field = Mat.Columns.Item("CODE").DataBind.Alias;
                string Table = Mat.Columns.Item("CODE").DataBind.TableName;

                DBDataSource DB_Line = form.DataSources.DBDataSources.Item(Table);
                for (int i = 0; i < DB_Line.Size; i++)
                {
                    if (DB_Line.GetValue(Code_Field, i) != "")
                    {
                        double Amount;
                        double.TryParse(DB_Line.GetValue("U_ST_AMOUNT", i), out Amount);
                        if (Amount == 0)
                        {
                            throw new Logic.Custom_Exception($"The Amount is zero in the Table[{Table}], Line[{i+1}]");
                        }
                    }
                }                
            }


            DBDataSource DB_Machin_Line = form.DataSources.DBDataSources.Item("@ST_TARGET_MACHINES");
            for (int i = 0; i < DB_Machin_Line.Size; i++)
            {
                if (DB_Machin_Line.GetValue("U_ST_MACHINE_ID", i) != "")
                {
                    double Don_Qty;
                    double.TryParse(DB_Machin_Line.GetValue("U_ST_QUANTITY", i), out Don_Qty);
                    double Remain;
                    double.TryParse(DB_Machin_Line.GetValue("U_ST_REMAINING_AMOUNT", i), out Remain);
                    if (Don_Qty > Remain )
                    {
                        throw new Logic.Custom_Exception($"The donated quantity is greater than the remaining , Line[{i + 1}]");
                    }
                }
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
                if (pVal.ItemUID == "110" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Add_Line(pVal, "@ST_TARGET_PATS", "Patients_Dets", "112", "U_ST_PATIENT_CODE", true);
                }
                if (pVal.ItemUID == "111" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Remove_Selected_Lines(pVal, "Patients_Dets", "112");
                }
                if (pVal.ItemUID == "112" && pVal.ColUID == "CODE" && pVal.EventType == BoEventTypes.et_CHOOSE_FROM_LIST && !pVal.BeforeAction)
                {
                    Patients_choose_From_List(pVal, "@ST_TARGET_PATS");
                }
                if (pVal.ItemUID == "115" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Add_Line(pVal, "@ST_TARGET_WISH", "Wish_Dets", "117", "U_ST_WISH_CODE", true);
                }
                if (pVal.ItemUID == "116" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Remove_Selected_Lines(pVal, "Wish_Dets", "117");
                }
                if (pVal.ItemUID == "117" && pVal.ColUID == "CODE" && pVal.EventType == BoEventTypes.et_CHOOSE_FROM_LIST && !pVal.BeforeAction)
                {
                    Wish_choose_From_List(pVal, "@ST_TARGET_WISH");
                }
                if (pVal.ItemUID == "118" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Add_Line(pVal, "@ST_TARGET_ORPH", "Orphan_Dets", "120", "U_ST_ORPHAN_CODE", true);
                }
                if (pVal.ItemUID == "119" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Remove_Selected_Lines(pVal, "Orphan_Dets", "120");
                }
                if (pVal.ItemUID == "120" && pVal.ColUID == "CODE" && pVal.EventType == BoEventTypes.et_CHOOSE_FROM_LIST && !pVal.BeforeAction)
                {
                    Orphans_Choos_From_List(pVal, "@ST_TARGET_ORPH");
                }
                if (pVal.ItemUID == "128" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Add_Line(pVal, "@ST_TARGET_MACHINES", "Machines_Dets", "130", "U_ST_MACHINE_ID", true);
                }
                if (pVal.ItemUID == "129" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Remove_Selected_Lines(pVal, "Machines_Dets", "130");
                }
                if (pVal.ItemUID == "130" && pVal.ColUID == "CODE" && pVal.EventType == BoEventTypes.et_CHOOSE_FROM_LIST && !pVal.BeforeAction)
                {
                    Machines_Choos_From_List(pVal, "@ST_TARGET_MACHINES");
                }
                if (pVal.ItemUID == "600" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Add_Line(pVal, "@ST_TARGET_AREAS", "Area_Dets", "602", "U_ST_AREA_ID", true);
                }
                if (pVal.ItemUID == "601" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    Remove_Selected_Lines(pVal, "Area_Dets", "602");
                }
                if (pVal.ItemUID == "602" && pVal.ColUID == "CODE" && pVal.EventType == BoEventTypes.et_CHOOSE_FROM_LIST && !pVal.BeforeAction)
                {
                    Areas_Choos_From_List(pVal, "@ST_TARGET_AREAS");
                }

                if (Matrix_Item_IDs.Contains(pVal.ItemUID) && pVal.ColUID == "Amount" && pVal.ItemChanged)
                {
                    Form form = SBO_Application.Forms.Item(pVal.FormUID);
                    ((Matrix)form.Items.Item(pVal.ItemUID).Specific).FlushToDataSource();
                    Calculate_Sum_Amount(form);
                }

            }
            catch (Exception ex)
            {
                Form form = SBO_Application.Forms.Item(pVal.FormUID);
                form.Freeze(false);
                SBO_Application.StatusBar.SetText(ex.Message, BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Error);
            }
        }

        private static void Calculate_Sum_Amount(Form form)
        {
            string[] Lines_Datasource = new string[] { "@ST_TARGET_PATS", "@ST_TARGET_WISH", "@ST_TARGET_ORPH", "@ST_TARGET_MACHINES", "@ST_TARGET_AREAS" };

            double Sum_Amount = 0;

            foreach (string One_Datasource in Lines_Datasource)
            {
                DBDataSource DB_Line = form.DataSources.DBDataSources.Item(One_Datasource);
                for (int i = 0; i < DB_Line.Size; i++)
                {
                    double Amount;
                    double.TryParse(DB_Line.GetValue("U_ST_AMOUNT", i), out Amount);
                    Sum_Amount += Amount;
                }
            }

            form.DataSources.DBDataSources.Item(Form_Obj.UDO_Database_Table_Name).SetValue("U_ST_TOTAL", 0, Sum_Amount.ToString());   

        }

        //private static void SetRequiredFields(string type,Form form)
        //{
        //    try
        //    {
        //        form.Freeze(true);
        //        List<string> fund = new List<string>() { "14", "15", "16", "18", "20", "21", "24", "36", "37", "39", "40", "41", "43", "44", "55", "45", "46" };
        //        List<string> grant = new List<string>() { "100", "101", "104", "105", "106", "107", "108", "109", "Item_42", "Item_7", "Item_12" };
        //        List<string> pledge = new List<string>() { "140", "141", "142", "143", "145", "146", "Item_33", "Item_4", "Item_5" };

        //        if (type == "F")
        //        {
        //            for (int i = 0; i < fund.Count; i++)
        //            {
        //                form.Items.Item(fund[i]).SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 2, BoModeVisualBehavior.mvb_True);
        //                form.Items.Item(fund[i]).SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 3, BoModeVisualBehavior.mvb_True);
        //                if ((fund[i] == "15" || fund[i] == "16") && (form.Items.Item(fund[i]).BackColor == Color.FromKnownColor(KnownColor.White).ToArgb()))
        //                {
        //                    form.Items.Item(fund[i]).BackColor = Color.Pink.R | (Color.Pink.G << 8) | (Color.Pink.B << 16);
        //                }
        //            }
        //            for (int i = 0; i < grant.Count; i++)
        //            {
        //                form.Items.Item(grant[i]).SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 2, BoModeVisualBehavior.mvb_False);
        //                form.Items.Item(grant[i]).SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 3, BoModeVisualBehavior.mvb_False);

        //                if ((grant[i] == "105" || grant[i] == "106"))
        //                {
        //                    form.Items.Item(grant[i]).BackColor = Color.FromKnownColor(KnownColor.White).ToArgb();
        //                }
        //            }
        //            for (int i = 0; i < pledge.Count; i++)
        //            {
        //                form.Items.Item(pledge[i]).SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 2, BoModeVisualBehavior.mvb_False);
        //                form.Items.Item(pledge[i]).SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 3, BoModeVisualBehavior.mvb_False);

        //                if (pledge[i] == "143")
        //                {
        //                    form.Items.Item(pledge[i]).BackColor = Color.FromKnownColor(KnownColor.White).ToArgb();
        //                }

        //            }
        //        }
        //        else if (type == "G")
        //        {
        //            for (int i = 0; i < fund.Count; i++)
        //            {
        //                form.Items.Item(fund[i]).SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 2, BoModeVisualBehavior.mvb_True);
        //                form.Items.Item(fund[i]).SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 3, BoModeVisualBehavior.mvb_True);

        //                if ((fund[i] == "15" || fund[i] == "16") && (form.Items.Item(fund[i]).BackColor == Color.FromKnownColor(KnownColor.White).ToArgb()))
        //                {
        //                    form.Items.Item(fund[i]).BackColor = Color.Pink.R | (Color.Pink.G << 8) | (Color.Pink.B << 16);
        //                }
        //            }
        //            for (int i = 0; i < grant.Count; i++)
        //            {
        //                form.Items.Item(grant[i]).SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 2, BoModeVisualBehavior.mvb_True);
        //                form.Items.Item(grant[i]).SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 3, BoModeVisualBehavior.mvb_True);

        //                if ((grant[i] == "105" || grant[i] == "106") && (form.Items.Item(grant[i]).BackColor == Color.FromKnownColor(KnownColor.White).ToArgb()))
        //                {
        //                    form.Items.Item(grant[i]).BackColor = Color.Pink.R | (Color.Pink.G << 8) | (Color.Pink.B << 16);
        //                }
        //            }
        //            for (int i = 0; i < pledge.Count; i++)
        //            {
        //                form.Items.Item(pledge[i]).SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 2, BoModeVisualBehavior.mvb_False);
        //                form.Items.Item(pledge[i]).SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 3, BoModeVisualBehavior.mvb_False);

        //                if (pledge[i] == "143")
        //                {
        //                    form.Items.Item(pledge[i]).BackColor = Color.FromKnownColor(KnownColor.White).ToArgb();
        //                }
        //            }
        //        }
        //        else if (type == "P")
        //        {
        //            for (int i = 0; i < fund.Count; i++)
        //            {
        //                form.Items.Item(fund[i]).SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 2, BoModeVisualBehavior.mvb_True);
        //                form.Items.Item(fund[i]).SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 3, BoModeVisualBehavior.mvb_True);

        //                if ((fund[i] == "15" || fund[i] == "16") && (form.Items.Item(fund[i]).BackColor == Color.FromKnownColor(KnownColor.White).ToArgb()))
        //                {
        //                    form.Items.Item(fund[i]).BackColor = Color.Pink.R | (Color.Pink.G << 8) | (Color.Pink.B << 16);
        //                }
        //            }
        //            for (int i = 0; i < grant.Count; i++)
        //            {
        //                form.Items.Item(grant[i]).SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 2, BoModeVisualBehavior.mvb_False);
        //                form.Items.Item(grant[i]).SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 3, BoModeVisualBehavior.mvb_False);

        //                if ((grant[i] == "105" || grant[i] == "106"))
        //                {
        //                    form.Items.Item(grant[i]).BackColor = Color.FromKnownColor(KnownColor.White).ToArgb();
        //                }
        //            }
        //            for (int i = 0; i < pledge.Count; i++)
        //            {
        //                form.Items.Item(pledge[i]).SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 2, BoModeVisualBehavior.mvb_True);
        //                form.Items.Item(pledge[i]).SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 3, BoModeVisualBehavior.mvb_True);

        //                if (pledge[i] == "143" && (form.Items.Item(pledge[i]).BackColor == Color.FromKnownColor(KnownColor.White).ToArgb()))
        //                {
        //                    form.Items.Item(pledge[i]).BackColor = Color.Pink.R | (Color.Pink.G << 8) | (Color.Pink.B << 16);
        //                }
        //            }
        //        }
        //        form.Freeze(false);
        //    }
        //    catch (Exception ex)
        //    {
        //        form.Freeze(false);
        //    }

        //}

        internal static void SBO_Application_MenuEvent(ref MenuEvent pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            if (pVal.BeforeAction)
                return;
            if (Form_Obj == null || SBO_Application.Forms.ActiveForm.TypeEx != Form_Obj.Form_Type)
            {
                return;
            }
            Code_value = SBO_Application.Forms.ActiveForm.TypeEx.Replace("ST", "Frm");
            if (pVal.MenuUID == "1282" || pVal.MenuUID == "1281")//Add || Find
            {
                Form form = SBO_Application.Forms.ActiveForm;
                //Form_Obj.Set_ReadOnly(form, Form_Obj.UDO_Info);
                //form.DataSources.UserDataSources.Item("27").Value = "0";
                //form.DataSources.UserDataSources.Item("172").Value = "0";
                string UDO_Code = form.DataSources.DBDataSources.Item(0).GetValue("Code", 0);
            }
            if (SBO_Application.Forms.ActiveForm.Mode != BoFormMode.fm_FIND_MODE)
            {
                Form_Obj.Set_Fields(SBO_Application.Forms.ActiveForm);
            }
            if (SBO_Application.Forms.ActiveForm.Mode == BoFormMode.fm_FIND_MODE)
            {
                Form form = SBO_Application.Forms.ActiveForm;
                DisableMatrixButtons(form);
                Form_Obj.UnSet_Mondatory_Fields_Color(SBO_Application.Forms.ActiveForm);
            }
        }
        private static void DisableMatrixButtons(Form form)
        {
            List<string> GridBtnsIds = new List<string>() { "111", "110", "116", "115", "119", "118", "129", "128", "601", "600" };
            for (int i = 0; i < GridBtnsIds.Count; i++)
            {
                form.Items.Item(GridBtnsIds[i]).SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 4, BoModeVisualBehavior.mvb_False);

            }
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
                        if (Matrix_Id == "602")
                        {
                            string SQL = $@"SELECT T0.""U_ST_CAN_EXTEND_RESERVATION"" FROM OUSR T0 WHERE T0.""USER_CODE"" = '{company.UserName}'";
                            Recordset RC = Helper.Utility.Execute_Recordset_Query(company, SQL);
                            if (RC.Fields.Item("U_ST_CAN_EXTEND_RESERVATION").Value.ToString() == "Y")
                            {
                                Mat_Lines.Columns.Item("Date").Editable = true;

                            }
                        }
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
                    //DT_Orphans_Details.Rows.Remove(i - 1);
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
        private static void Patients_choose_From_List(ItemEvent pVal, string Line_DataSource_Table)
        {
            SAPbouiCOM.Form form = Loader.SBO_Application.Forms.Item(pVal.FormUID);
            SAPbouiCOM.ChooseFromListEvent Choos_Event = (SAPbouiCOM.ChooseFromListEvent)pVal;

            if (Choos_Event.SelectedObjects == null || Choos_Event.SelectedObjects.Rows.Count == 0)
            {
                return;
            }
            Matrix Mat = (Matrix)form.Items.Item("112").Specific;

            string Code = Choos_Event.SelectedObjects.GetValue("Code", 0).ToString();
            int Index = pVal.Row - 1;
            form.DataSources.DBDataSources.Item(Line_DataSource_Table).SetValue("U_ST_PATIENT_CODE", Index, Code);


            Set_Patient_Data(form, Index, Line_DataSource_Table);
            Mat.LoadFromDataSource();
            Mat.AutoResizeColumns();
            if (form.Mode == BoFormMode.fm_OK_MODE)
            {
                form.Mode = BoFormMode.fm_UPDATE_MODE;
            }
        }
        internal static void Set_Patient_Data(Form form, int DT_Row_Index, string Line_DataSource_Table)
        {
            string Code = form.DataSources.DBDataSources.Item(Line_DataSource_Table).GetValue("U_ST_PATIENT_CODE", DT_Row_Index);
            string SQL = $@"SELECT T0.U_ST_FULL_ARABIC_NAME, T0.U_ST_GENDER, T0.U_ST_DATE_OF_BIRTH, T0.U_ST_NATIONAL_ID
FROM ""@ST_PATIENTS_CARD""  T0 WHERE T0.""Code"" ='{Code}'";
            Recordset RC = Helper.Utility.Execute_Recordset_Query(company, SQL);
            string dob = string.Empty;
            string tempDate = RC.Fields.Item("U_ST_DATE_OF_BIRTH").Value.ToString();
            if (!string.IsNullOrEmpty(tempDate))
            {
                DateTime temp = Convert.ToDateTime(tempDate);
                dob = temp.ToString("dd/MM/yyyy");
            }
            form.DataSources.DBDataSources.Item(Line_DataSource_Table).SetValue("U_ST_NAME", DT_Row_Index, RC.Fields.Item("U_ST_FULL_ARABIC_NAME").Value.ToString());
            form.DataSources.DBDataSources.Item(Line_DataSource_Table).SetValue("U_ST_GENDER", DT_Row_Index, RC.Fields.Item("U_ST_GENDER").Value.ToString());
            form.DataSources.DBDataSources.Item(Line_DataSource_Table).SetValue("U_ST_DATE_OF_BIRTH", DT_Row_Index, dob);
            form.DataSources.DBDataSources.Item(Line_DataSource_Table).SetValue("U_ST_NATIONAL_ID", DT_Row_Index, RC.Fields.Item("U_ST_NATIONAL_ID").Value.ToString());
        }
        private static void Wish_choose_From_List(ItemEvent pVal, string Line_DataSource_Table)
        {
            SAPbouiCOM.Form form = Loader.SBO_Application.Forms.Item(pVal.FormUID);
            SAPbouiCOM.ChooseFromListEvent Choos_Event = (SAPbouiCOM.ChooseFromListEvent)pVal;

            if (Choos_Event.SelectedObjects == null || Choos_Event.SelectedObjects.Rows.Count == 0)
            {
                return;
            }
            Matrix Mat = (Matrix)form.Items.Item("117").Specific;

            string Code = Choos_Event.SelectedObjects.GetValue("ItemCode", 0).ToString();
            int Index = pVal.Row - 1;
            form.DataSources.DBDataSources.Item(Line_DataSource_Table).SetValue("U_ST_WISH_CODE", Index, Code);
            Set_Wish_Data(form, Index, Line_DataSource_Table);
            Mat.LoadFromDataSource();
            Mat.AutoResizeColumns();
        }
        internal static void Set_Wish_Data(Form form, int DT_Row_Index, string Line_DataSource_Table)
        {
            string Code = form.DataSources.DBDataSources.Item(Line_DataSource_Table).GetValue("U_ST_WISH_CODE", DT_Row_Index);
            string SQL = $@"SELECT T0.""ItemName"" FROM OITM  T0 WHERE T0.""ItemCode"" = '{Code}'";
            Recordset RC = Helper.Utility.Execute_Recordset_Query(company, SQL);
            form.DataSources.DBDataSources.Item(Line_DataSource_Table).SetValue("U_ST_NAME", DT_Row_Index, RC.Fields.Item("ItemName").Value.ToString());
        }
        internal static void Orphans_Choos_From_List(ItemEvent pVal, string Line_DataSource_Table)
        {
            SAPbouiCOM.Form form = Loader.SBO_Application.Forms.Item(pVal.FormUID);
            SAPbouiCOM.ChooseFromListEvent Choos_Event = (SAPbouiCOM.ChooseFromListEvent)pVal;

            if (Choos_Event.SelectedObjects == null || Choos_Event.SelectedObjects.Rows.Count == 0)
            {
                return;
            }
            Matrix Mat = (Matrix)form.Items.Item("120").Specific;

            string Code = Choos_Event.SelectedObjects.GetValue("CardCode", 0).ToString();
            int Index = pVal.Row - 1;
            form.DataSources.DBDataSources.Item(Line_DataSource_Table).SetValue("U_ST_ORPHAN_CODE", Index, Code);
            Set_Orphan_Data(form, Index, Line_DataSource_Table);
            Mat.LoadFromDataSource();

            Mat.AutoResizeColumns();
        }
        internal static void Set_Orphan_Data(Form form, int DT_Row_Index, string Line_DataSource_Table)
        {
            string Code = form.DataSources.DBDataSources.Item(Line_DataSource_Table).GetValue("U_ST_ORPHAN_CODE", DT_Row_Index);
            string SQL = $@"SELECT T0.""CardName"", T0.U_ST_GENDER, T0.U_ST_DATE_OF_BIRTH, T0.U_ST_NATIONAL_ID FROM OCRD T0 WHERE T0.""CardCode"" = '{Code}'";
            Recordset RC = Helper.Utility.Execute_Recordset_Query(company, SQL);
            string dob = string.Empty;
            string tempDate = RC.Fields.Item("U_ST_DATE_OF_BIRTH").Value.ToString();
            if (!string.IsNullOrEmpty(tempDate))
            {
                DateTime temp = Convert.ToDateTime(tempDate);
                dob = temp.ToString("dd/MM/yyyy");
            }
            form.DataSources.DBDataSources.Item(Line_DataSource_Table).SetValue("U_ST_NAME", DT_Row_Index, RC.Fields.Item("CardName").Value.ToString());
            form.DataSources.DBDataSources.Item(Line_DataSource_Table).SetValue("U_ST_GENDER", DT_Row_Index, RC.Fields.Item("U_ST_GENDER").Value.ToString());
            form.DataSources.DBDataSources.Item(Line_DataSource_Table).SetValue("U_ST_DATE_OF_BIRTH", DT_Row_Index, dob);
            form.DataSources.DBDataSources.Item(Line_DataSource_Table).SetValue("U_ST_NATIONAL_ID", DT_Row_Index, RC.Fields.Item("U_ST_NATIONAL_ID").Value.ToString());
        }
        internal static void Machines_Choos_From_List(ItemEvent pVal, string Line_DataSource_Table)
        {
            SAPbouiCOM.Form form = Loader.SBO_Application.Forms.Item(pVal.FormUID);
            SAPbouiCOM.ChooseFromListEvent Choos_Event = (SAPbouiCOM.ChooseFromListEvent)pVal;

            if (Choos_Event.SelectedObjects == null || Choos_Event.SelectedObjects.Rows.Count == 0)
            {
                return;
            }
            Matrix Mat = (Matrix)form.Items.Item("130").Specific;

            string Code = Choos_Event.SelectedObjects.GetValue("Code", 0).ToString();
            int Index = pVal.Row - 1;
            form.DataSources.DBDataSources.Item(Line_DataSource_Table).SetValue("U_ST_MACHINE_ID", Index, Code);
            Set_Machine_Data(form, Index, Line_DataSource_Table);
            Mat.LoadFromDataSource();

            Mat.AutoResizeColumns();

            if (form.Mode == BoFormMode.fm_OK_MODE)
            {
                form.Mode = BoFormMode.fm_UPDATE_MODE;
            }
        }
        internal static void Set_Machine_Data(Form form, int DT_Row_Index, string Line_DataSource_Table)
        {
            string Code = form.DataSources.DBDataSources.Item(Line_DataSource_Table).GetValue("U_ST_MACHINE_ID", DT_Row_Index);
            string SQL = $@"SELECT T0.""U_ST_MACHINE_NAME_AR"" FROM ""@ST_MACHINERY"" T0 WHERE T0.""Code"" = '{Code}'";
            Recordset RC = Helper.Utility.Execute_Recordset_Query(company, SQL);
            form.DataSources.DBDataSources.Item(Line_DataSource_Table).SetValue("U_ST_NAME", DT_Row_Index, RC.Fields.Item("U_ST_MACHINE_NAME_AR").Value.ToString());
            string qtySQL = $@"SELECT T0.""U_ST_NEEDED_QUANTITY"" FROM ""@ST_MACHIN_DET"" T0 WHERE T0.""U_ST_MACHINE_ID"" = '{Code}' AND U_ST_YEAR = '{DateTime.Today.Year}'";
            Recordset qtyRC = Helper.Utility.Execute_Recordset_Query(company, qtySQL);
            int Need_Qty = (int) qtyRC.Fields.Item("U_ST_NEEDED_QUANTITY").Value;
            string SQL_Actual_Qty = $@"SELECT SUM(T0.""U_ST_QUANTITY"") FROM ""@ST_TARGET_MACHINES"" T0 
INNER JOIN ""@ST_MACHIN_DET""  T1 ON T0.""U_ST_MACHINE_ID"" = T1.U_ST_MACHINE_ID 
 WHERE T0.""Code"" in(Select ""U_ST_TARGET"" FROM ""@ST_ACTUAL_DONATIONS"" T3 WHERE  T1.U_ST_YEAR = TO_Varchar(Year(T3.U_ST_DONATION_DATE)))
 AND T0.""U_ST_MACHINE_ID"" = '{Code}'";

            Recordset RC_Actual_Qty = Helper.Utility.Execute_Recordset_Query(company, SQL_Actual_Qty);
            int Actual_Qty = (int)RC_Actual_Qty.Fields.Item(0).Value;
            double Remain = Need_Qty - Actual_Qty;

            form.DataSources.DBDataSources.Item(Line_DataSource_Table).SetValue("U_ST_QUANTITY", DT_Row_Index, Remain.ToString());
            form.DataSources.DBDataSources.Item(Line_DataSource_Table).SetValue("U_ST_REMAINING_AMOUNT", DT_Row_Index, Remain.ToString());

        }
        internal static void Areas_Choos_From_List(ItemEvent pVal, string Line_DataSource_Table)
        {
            SAPbouiCOM.Form form = Loader.SBO_Application.Forms.Item(pVal.FormUID);
            SAPbouiCOM.ChooseFromListEvent Choos_Event = (SAPbouiCOM.ChooseFromListEvent)pVal;

            if (Choos_Event.SelectedObjects == null || Choos_Event.SelectedObjects.Rows.Count == 0)
            {
                return;
            }
            Matrix Mat = (Matrix)form.Items.Item("602").Specific;

            string Code = Choos_Event.SelectedObjects.GetValue("Code", 0).ToString();
            int Index = pVal.Row - 1;
            form.DataSources.DBDataSources.Item(Line_DataSource_Table).SetValue("U_ST_AREA_ID", Index, Code);
            Set_Area_Data(form, Index, Line_DataSource_Table);
            Mat.LoadFromDataSource();

            Mat.AutoResizeColumns();
        }
        internal static void Set_Area_Data(Form form, int DT_Row_Index, string Line_DataSource_Table)
        {
            string Code = form.DataSources.DBDataSources.Item(Line_DataSource_Table).GetValue("U_ST_AREA_ID", DT_Row_Index);
            string SQL = $@"SELECT T0.""U_ST_AREA_NAME"" FROM ""@ST_NAMING"" T0 WHERE T0.""Code"" = '{Code}'";
            Recordset RC = Helper.Utility.Execute_Recordset_Query(company, SQL);
            int reservationPeriod = Convert.ToInt32(Utility.Get_Configuration(company, "Allocation_Reservation_Period", "Allocation Reservation Period in Days", "14"));
            DateTime reservationDate = DateTime.Now.AddDays(reservationPeriod);
            form.DataSources.DBDataSources.Item(Line_DataSource_Table).SetValue("U_ST_NAME", DT_Row_Index, RC.Fields.Item("U_ST_AREA_NAME").Value.ToString());
            form.DataSources.DBDataSources.Item(Line_DataSource_Table).SetValue("U_ST_RES_DATE", DT_Row_Index, reservationDate.ToString("yyyyMMdd"));

        }
        private static void Update_Amount()
        {
            double totalAmount = 0;
            List<string> Matrices = new List<string>() {"112","117","120","130","602" };
            Form form = SBO_Application.Forms.ActiveForm;
            foreach (string matrix in Matrices)
            {
                Matrix Mat = (Matrix)form.Items.Item(matrix).Specific;
                if (Mat.RowCount == 0)
                {
                    continue;
                }
                else
                {
                    for (int i = Mat.RowCount; i > 0; i--)
                    {

                        EditText amountET = (EditText)Mat.Columns.Item("Amount").Cells.Item(i).Specific;
                        if (!string.IsNullOrEmpty(amountET.Value))
                        {
                            totalAmount += Convert.ToDouble(amountET.Value);
                        }

                    }
                }
            }
            form.DataSources.DBDataSources.Item(0).SetValue("U_ST_TOTAL", 0, totalAmount.ToString());
        }
    }
}
