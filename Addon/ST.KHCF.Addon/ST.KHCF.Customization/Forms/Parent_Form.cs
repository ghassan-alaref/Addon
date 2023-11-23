using SAPbobsCOM;
using SAPbouiCOM;
using ST.KHCF.Customization.Logic;
using ST.KHCF.Customization.Logic.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
//using System.Windows.Forms;

namespace ST.KHCF.Customization.Forms
{
    public class Parent_Form
    {
        internal KHCF_Objects KHCF_Object;
        internal static KHCF_Objects[] Objects_Need_Approval = new KHCF_Objects[] { KHCF_Objects.CCI_Corporate_Member_Card, KHCF_Objects.CCI_Member_Card, KHCF_Objects.Individual_Membership, KHCF_Objects.Corporate_Membership };
        //internal static KHCF_Objects[] Objects_Need_Approval = new KHCF_Objects[] { KHCF_Objects.CCI_Corporate_Member_Card, KHCF_Objects.CCI_Member_Card, KHCF_Objects.Expected_Donations};
        internal static string[] KHCF_Additional_Forms_Type = new string[] { "ST_Revenue_Realization", "ST_Cards_List" , "ST_Membership_Renewal"
            , "ST_Cards_Actions_From_File", "ST_Memberships_Need_To_Active", "ST_Waiting_List" };
        internal SAPbobsCOM.Company company_For_Parent_Form;
        internal Application SBO_Application_For_Parent_Form;
        internal static string[] All_Form_Types;
        //private static string UDO_Type_For_Open;
        private static bool Application_Will_Open;
        private static bool Application_Add_New_Open;
        private static string Before_CFL_Form_ID;
        private static string Before_CFL_Item_ID;
        internal UDO_Definition UDO_Info;
        protected static string Code_value;
        protected static string Desc_value = string.Empty, Man_fields  = string.Empty;

        internal class Depends_List
        {

            internal string Item_ID;
            internal string Parent_Item_ID;
            internal string SQL;

        }


        internal int RunTimeMondatoryColor { get { return Color.Pink.R | (Color.Pink.G << 8) | (Color.Pink.B << 16); } }
        internal int FindColor { get { return  255 | 255 | 192 ; } }
        //internal int RunTimeFindColor { get { return Color.LightYellow. | (Color.Pink.G << 8) | (Color.Pink.B << 16); } }

        public string Form_Type { get { return "ST_" + KHCF_Object.ToString(); } }
        public string UDO_Database_Table_Name
        {
            get
            {
                return "@" + UDO_Info.Table_Name;
            }
        }

        internal Form Create_Form(Application SBO_Application, string XML_Form)
        {
            SBO_Application.StatusBar.SetText("Loading Form", BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Warning);
            FormCreationParams form_params = null;
            try 
            {
                form_params = (FormCreationParams)SBO_Application.CreateObject(BoCreatableObjectType.cot_FormCreationParams);
            }
            catch (Exception ex) 
            {
                
            }
            form_params.XmlData = XML_Form;
            var form = SBO_Application.Forms.AddEx(form_params);
            //form.Visible = false;
            form.AutoManaged = true;
            form.SupportedModes = -1;
            form.Mode = BoFormMode.fm_FIND_MODE;
            form.DataBrowser.BrowseBy = "5";

            try
            {
                Initialize_Form(form);
            }
            catch (Exception ex)
            {
                SBO_Application.StatusBar.SetText(ex.Message, BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Error);
            }
            form.Visible = true;
            SBO_Application.StatusBar.SetText("Loading Form", BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_None);
            return form;
        }

        internal void Fill_Address_ComboBox(Matrix Mat_Add)
        {
            string SQL = $@"SELECT T0.""Code"", T0.""Name"" FROM OCRY T0";
            Recordset RC = (Recordset)company_For_Parent_Form.GetBusinessObject(BoObjectTypes.BoRecordset);
            //RC.DoQuery(SQL);
            //for (int i = 0; i < RC.RecordCount; i++)
            //{
            //    Mat_Add.Columns.Item("Country").ValidValues.Add(RC.Fields.Item("Code").Value.ToString(), RC.Fields.Item("Name").Value.ToString());
            //    RC.MoveNext();
            //}

           /* SQL = $@"SELECT T0.""Code"", T0.""Name"" FROM ""@ST_CITY_AREA""  T0";
            RC = (Recordset)company_For_Parent_Form.GetBusinessObject(BoObjectTypes.BoRecordset);
            RC.DoQuery(SQL);
            for (int i = 0; i < RC.RecordCount; i++)
            {
                Mat_Add.Columns.Item("City").ValidValues.Add(RC.Fields.Item("Code").Value.ToString(), RC.Fields.Item("Name").Value.ToString());
                RC.MoveNext();
            }*/

            //Mat_Add.Columns.Item("ZipCode").Visible = false;
            //Mat_Add.Columns.Item("County").Visible = false;
            //Mat_Add.Columns.Item("State").Visible = false;
            //Mat_Add.Columns.Item("Address3").Visible = false;
            //Mat_Add.Columns.Item("StreetNo").Visible = false;
            //Mat_Add.Columns.Item("Block").Visible = false;


        }

        
        internal virtual void Initialize_Form(Form form)
        {
            UDO_Info = Objects_Logic.All_UDO_Definition.FirstOrDefault(U => U.KHCF_Object == KHCF_Object);
            string[] depends_Lists_Item_IDs = this.Get_Depends_List_List().Select(D => D.Item_ID).ToArray();

            Field_Definition[] UDFs_Linked_Table = Fields_Logic.All_Field_Definition.Where(F => F.KHCF_Object == this.KHCF_Object && F.Linked_Table != "").ToArray();
            if (form.TypeEx != "ST_Schools_Universites")
            {
                foreach (Field_Definition OneUDF in UDFs_Linked_Table)
                {
                    if (depends_Lists_Item_IDs.Contains(OneUDF.Item_ID))
                    {
                        continue;
                    }
                    string SQL = $@"Select ""Code"", ""Name"" from ""@{OneUDF.Linked_Table}""";
                    if (OneUDF.Linked_Table.Equals("ST_COVERAGE"))
                        SQL += $@"Order by ""Code""";
                    Helper.Utility.Fill_One_ComboBoxBySQL(company_For_Parent_Form, form, OneUDF.Item_ID, SQL, true);
                }
            }

            Set_Fields(form);
            //form.EnableMenu("1283", false);
        }

        internal void Set_Fields(Form form) 
        {
            try
            {
                form.Freeze(true);
                Set_ReadOnly(form, UDO_Info);
                Set_Mondatory_Fields_Color(form);
                Set_Editable_Fields(form);
            }
            catch(Exception ex)
            { }
            finally
            {
                form.Freeze(false);
            }
        }

        internal string[] Get_Depends_Parent_Item_IDs_List()
        {
            string[] Result = this.Get_Depends_List_List().Select(D => D.Parent_Item_ID).ToArray();

            return Result;
        }

        internal bool Set_ReadOnly(Form form, UDO_Definition UDO_Info)
        {
            string itemId = "";
            Code_value = form.TypeEx.Replace("ST", "Frm");
            try
            {
                bool Need_ReadOnly;
                //bool Is_Approval = false;
                if (!Objects_Need_Approval.Contains(UDO_Info.KHCF_Object))
                {
                    return false;
                }
                string[] Approval_Items = Get_Approval_Items_List(); 
                bool Is_Approved = Get_Is_Approval_Status(form);
                bool Can_Approve = Utility.User_Can_Approve(Loader.company, Loader.company.UserName, UDO_Info);
                //if (form.Mode == BoFormMode.fm_ADD_MODE || form.Mode == BoFormMode.fm_FIND_MODE)
                //{
                //    Need_ReadOnly = false;
                //}
                //else
                //{
                //    if (Is_Approved)
                //    {
                //        Need_ReadOnly = false;
                //    }
                //    else
                //    {
                //        if (Loader.company.UserName == form.DataSources.DBDataSources.Item("@" + UDO_Info.Table_Name).GetValue("U_ST_CREATOR", 0))
                //        {
                //            Need_ReadOnly = false;
                //        }
                //        //else if (Can_Approve)
                //        //{
                //        //    Need_ReadOnly = true;
                //        //    Is_Approval = true;
                //        //}
                //        else
                //        {
                //            Need_ReadOnly = true;
                //        }
                //    }
                //}
                
                //form.Freeze(true);

                //string[] Editable_Item_List = Get_Always_Editable_Item_List();
                //if (!Need_ReadOnly)
                //{
                //    for (int i = 0; i < form.Items.Count; i++)
                //    {
                //        if (form.Items.Item(i).Type != BoFormItemTypes.it_STATIC || form.Items.Item(i).Type != BoFormItemTypes.it_BUTTON_COMBO || form.Items.Item(i).Type != BoFormItemTypes.it_FOLDER)
                //        {
                //            string ItemId = form.Items.Item(i).UniqueID;
                //            itemId = ItemId;
                //            if (Editable_Item_List.Contains(ItemId))
                //            {
                //                continue;
                //            }
                //            if (Approval_Items.Contains(ItemId))
                //            {
                //                form.Items.Item(i).Enabled = false;
                //            }
                //            else
                //            {
                //                form.Items.Item(i).SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, -1, BoModeVisualBehavior.mvb_True);
                //            }
                //        }
                        
                //    }
                //}
                //else
                //{
                //    for (int i = 0; i < form.Items.Count; i++)
                //    {
                //        if (form.Items.Item(i).Type != BoFormItemTypes.it_STATIC || form.Items.Item(i).Type != BoFormItemTypes.it_BUTTON_COMBO || form.Items.Item(i).Type != BoFormItemTypes.it_FOLDER)
                //        {
                //            string ItemId = form.Items.Item(i).UniqueID;
                //            itemId = ItemId;
                //            if (Editable_Item_List.Contains(ItemId))
                //            {
                //                continue;
                //            }
                //            if (Is_Approval)
                //            {
                //                if (Approval_Items.Contains(ItemId))
                //                {
                //                    form.Items.Item(i).Enabled = true;
                //                }
                //                else
                //                {
                //                    form.Items.Item(i).SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, -1, BoModeVisualBehavior.mvb_False);
                //                    form.Items.Item(i).SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 4, BoModeVisualBehavior.mvb_True);
                //                }
                //            }
                //            else
                //            {
                //                form.Items.Item(i).SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, -1, BoModeVisualBehavior.mvb_False);
                //                form.Items.Item(i).SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 4, BoModeVisualBehavior.mvb_True);
                //            }
                //        }

                //    }
                //}
                //Set_Editable_Fields(form);
                string[] items = new string[] { "161", "162" };
                if(form.Mode == BoFormMode.fm_FIND_MODE)
                {
                    Change_Visiblity(form, Approval_Items, false);
                    Change_Visiblity(form, items, true);
                    try
                    {
                        form.Items.Item("162").Enabled = true;
                        form.Items.Item("162").SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 4, BoModeVisualBehavior.mvb_True);
                    }
                    catch (Exception) { }
                }
                if (Can_Approve && !Is_Approved)
                {
                    Change_Visiblity(form, Approval_Items, true);
                    Change_Visiblity(form, items, true);
                    try
                    {
                        form.Items.Item("162").Enabled = true;
                        form.Items.Item("162").SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 4, BoModeVisualBehavior.mvb_True);
                    }
                    catch (Exception) { }
                }
                else 
                {
                    Change_Visiblity(form, Approval_Items, false);
                }

                //form.Freeze(false);

                //return Need_ReadOnly;
                return true;
            }
            catch (Exception ex) 
            {
                throw new Logic.Custom_Exception($"Error during set Fields[{ex.Message}] at "+ itemId);
            }
        }

        internal virtual bool Get_Is_Approval_Status(Form form)
        {
            //string Code = form.DataSources.DBDataSources.Item(UDO_Database_Table_Name).GetValue("U_ST_APPROVAL_STATUS", 0) == "A";
            return form.DataSources.DBDataSources.Item(UDO_Database_Table_Name).GetValue("U_ST_APPROVAL_STATUS", 0) != "P";
        }

        internal string[] Get_Approval_Items_List()
        {
            string Field_str = Utility.Get_Field_Configuration(company_For_Parent_Form, Code_value + "_Approval", Desc_value + " Form , please sperate each value with comma [,]", Man_fields);
            string[] Field_Array;
            if (Field_str!="" && !string.IsNullOrEmpty(Field_str))
             Field_Array = Field_str.Split(',');
            else 
                Field_Array = new string[] { };
            //List<string> Result = new List<string>();
            //Result.AddRange(Field_Array);
            return Field_Array;
        }

        private void Set_Editable_Fields(Form form)
        {
            try
            {
                Code_value = this.Form_Type.Replace("ST", "Frm");
                string[] Fix_ReadOnly_Fields_List = Get_Fix_ReadOnly_Fields_List();
                //if (form.Mode != BoFormMode.fm_FIND_MODE)
                //{
                foreach (string OneItem in Fix_ReadOnly_Fields_List)
                {
                    if (form.Items.Item(OneItem).Enabled)
                    {
                        form.Items.Item(OneItem).SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, -1, BoModeVisualBehavior.mvb_False);
                        form.Items.Item(OneItem).SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 4, BoModeVisualBehavior.mvb_True);
                    }
                }
                //}
                //else
                //{
                //    foreach (string OneItem in Fix_ReadOnly_Fields_List)
                //    {
                //        if (!form.Items.Item(OneItem).Enabled)
                //        {
                //            form.Items.Item(OneItem).SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 4, BoModeVisualBehavior.mvb_True);
                //        }
                //    }
                //}
            }
            catch (Exception ex)
            {
                throw new Logic.Custom_Exception("Exception in Set_Editable_Fields");
            }
        }

        private void Set_Mondatory_Fields_Color(Form form)
        {
            Code_value = this.Form_Type.Replace("ST", "Frm");
            string[] Fix_Mondatory_Fields_List = Get_Mondatory_Fields_List().ToArray();
            //form.Freeze(true);
            foreach (string OneItem in Fix_Mondatory_Fields_List)
            {
                if(form.Items.Item(OneItem).BackColor != RunTimeMondatoryColor)
                    form.Items.Item(OneItem).BackColor = RunTimeMondatoryColor;
            }
        }

        internal void UnSet_Mondatory_Fields_Color(Form form)
        {
            try
            {
                Code_value = this.Form_Type.Replace("ST", "Frm");
                string[] Fix_Mondatory_Fields_List = Get_Mondatory_Fields_List().ToArray();
                string[] Fix_Read_Only = Get_Fix_ReadOnly_Fields_List().ToArray();
                form.Freeze(true);
                foreach (string OneItem in Fix_Mondatory_Fields_List)
                {
                    int c = form.Items.Item("5").BackColor;
                    form.Items.Item(OneItem).BackColor = -1;
                }
                foreach (string OneItem in Fix_Read_Only)
                {
                    form.Items.Item(OneItem).BackColor = -1;
                    if (form.Items.Item(OneItem).Type != BoFormItemTypes.it_STATIC || form.Items.Item(OneItem).Type != BoFormItemTypes.it_BUTTON_COMBO || form.Items.Item(OneItem).Type != BoFormItemTypes.it_FOLDER)
                    {
                        form.Items.Item(OneItem).SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 4, BoModeVisualBehavior.mvb_True);
                    }
                    else
                    {
                        form.Items.Item(OneItem).Enabled = true;
                    }
                }
            }
            finally
            {
                form.Freeze(false);
            }
        }

        protected List<string> Get_Mondatory_Fields_List( )
        {
            string Field_str = Utility.Get_Field_Configuration(company_For_Parent_Form, Code_value+"_Mandatory",Desc_value+" Form , please sperate each value with comma [,]", Man_fields);
            string[] Field_Array;
            if (Field_str != "" && !string.IsNullOrEmpty(Field_str))
                Field_Array = Field_str.Split(',');
            else
                Field_Array = new string[] { };
            
            List<string> Result = new List<string>();
            Result.AddRange(Field_Array);
            return Result;
           // return new string[] { };
        }

        //internal virtual string[] Get_Mondatory_Fields_List()
        //{
        //    return new string[] { "" };
        //}
        
        protected string[] Get_Fix_ReadOnly_Fields_List()
        {
            string Field_str = Utility.Get_Field_Configuration(company_For_Parent_Form, Code_value + "_ReadOnly", Desc_value + " Form , please sperate each value with comma [,]", Man_fields);
            string[] Field_Array;
            if (Field_str != "" && !string.IsNullOrEmpty(Field_str))
                Field_Array = Field_str.Trim().Split(',');
            else
                Field_Array = new string[] { };

            //List<string> Result = new List<string>();
            //Result.AddRange(Field_Array);
            return Field_Array;
        }

        public  string[] Get_Tab_Item_List()
        {
            string Field_str = Utility.Get_Field_Configuration(company_For_Parent_Form, Code_value + "_Tab", Desc_value + " Form , please sperate each value with comma [,]", Man_fields);
            string[] Field_Array;
            if (Field_str != "" && !string.IsNullOrEmpty(Field_str))
                Field_Array = Field_str.Split(',');
            else
                Field_Array = new string[] { };
            //List<string> Result = new List<string>();
            //Result.AddRange(Field_Array);
            return Field_Array;
        }

        internal string[] Get_Always_Editable_Item_List()
        {
            
            string Field_str = Utility.Get_Field_Configuration(company_For_Parent_Form, "Parent_Form" + "_Editable_Item", Desc_value + " Form , please sperate each value with comma [,]", Man_fields);
            string[] Field_Array;
            if (Field_str != "" && !string.IsNullOrEmpty(Field_str))
                Field_Array = Field_str.Split(',');
            else
                Field_Array = new string[] { };
            List<string> Result = new List<string>();// { "1", "2" };
            //List<string> Result = new List<string>();
            Result.AddRange(Field_Array);

            Result.AddRange(Get_Tab_Item_List());
            Result.AddRange(Get_Approval_Items_List());
            return Result.ToArray();
        }

        internal virtual Depends_List[] Get_Depends_List_List()
        {
            return new Depends_List[] { };
        }

        public virtual bool Validate_Data(Form form)
        {
            SBO_Application_For_Parent_Form.StatusBar.SetText("Validating data..", BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Warning);

            foreach (Item OneItem in form.Items)
            {
                if (OneItem.BackColor == RunTimeMondatoryColor)
                {
                    string Field_Name = Helper.Utility.Get_Item_DB_Datasource(OneItem);
                    string Value = form.DataSources.DBDataSources.Item(this.UDO_Database_Table_Name).GetValue(Field_Name, 0);
                    
                    if (Value == "")
                    {
                        if (Field_Name != "U_ST_NATIONAL_ID")
                        {
                            Field_Definition Field = Fields_Logic.All_Field_Definition.FirstOrDefault(F => "U_" + F.Field_Name == Field_Name);
                            if (Field == null)
                            {
                                throw new Logic.Custom_Exception($"[{Field_Name}] is not defined. Contact System Admin");
                            }
                            //OneItem.Click();
                            Loader.New_Msg = $"[{Field.Field_Title}] is required.";
                            string X = OneItem.UniqueID;
                            return false;
                        }
                        else
                        {
                            string v_n = form.DataSources.DBDataSources.Item(this.UDO_Database_Table_Name).GetValue("U_ST_NATIONALITY", 0);
                            if (v_n == "Jordan")
                            {
                                Field_Definition Field = Fields_Logic.All_Field_Definition.FirstOrDefault(F => "U_" + F.Field_Name == Field_Name);
                                if (Field == null)
                                {
                                    throw new Logic.Custom_Exception($"[{Field_Name}] is not defined. Contact System Admin");
                                }
                                //OneItem.Click();
                                Loader.New_Msg = $"[{Field.Field_Title}] is required.";
                                return false;
                            }
                        }
                    }
                }
            }
            return true;
        }

        internal static void SBO_Application_MenuEvent_For_Parent(ref MenuEvent pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;

            if (pVal.BeforeAction)
                return;

            if (pVal.MenuUID == "1282" || pVal.MenuUID == "1281")//Add || Find
            {
                Form form = Loader.SBO_Application.Forms.ActiveForm;
                if (All_Form_Types.Contains(form.TypeEx))
                {
                    if (pVal.MenuUID == "1281")
                    {
                        KHCF_Objects Form_UDO = Get_KHCF_Object(form).KHCF_Object;
                        Parent_Form Form_Obj = Loader.Get_Form_Object(Form_UDO.ToString());
                        Form_Obj.company_For_Parent_Form = Loader.company;
                        Form_Obj.KHCF_Object = Form_UDO;
                        Form_Obj.UnSet_Mondatory_Fields_Color(form);
                    }
                }
            }
            if (pVal.MenuUID == "1282" || pVal.MenuUID == "1289" || pVal.MenuUID == "1291" || pVal.MenuUID == "1290")
            {
                Form form = Loader.SBO_Application.Forms.ActiveForm;
                if (All_Form_Types.Contains(form.TypeEx))
                {
                    UDO_Definition Obj_Info = Get_KHCF_Object(form);
                    Parent_Form Form_Obj = Loader.Get_Form_Object(Obj_Info.KHCF_Object.ToString());
                    Form_Obj.company_For_Parent_Form = Loader.company;
                    Form_Obj.KHCF_Object = Obj_Info.KHCF_Object;
                    Form_Obj.UDO_Info = Obj_Info;
                    Form_Obj.Set_Fields(form);
                }
            }


        }

        private static UDO_Definition Get_KHCF_Object(Form form)
        {
            string UDO_Name = form.BusinessObject.Type;
            if (string.IsNullOrEmpty(UDO_Name))
            {
                throw new Logic.Custom_Exception($"The Form Type[{form.TypeEx}] is not related with UDO");
            }
            UDO_Definition UDO_Info = Objects_Logic.All_UDO_Definition.FirstOrDefault(U => U.Table_Name == UDO_Name);
            if (UDO_Info == null)
            {
                throw new Logic.Custom_Exception($"The UDO [{UDO_Name}] is not supported as KHCF objects");
            }
            return UDO_Info;
        }

        internal static void SBO_Application_ItemEvent_For_Parent(string FormUID, ref ItemEvent pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            if ((pVal.FormTypeEx == "9999" && (pVal.ItemUID == "7" || pVal.ItemUID == "5")) ||(pVal.FormTypeEx == "-149" && pVal.ItemUID == "1000011")  && pVal.EventType == BoEventTypes.et_CLICK && pVal.BeforeAction)
            {
                Application_Will_Open = true;
            }
            if ((pVal.FormTypeEx == "9999" && (pVal.ItemUID == "5"))  && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && pVal.BeforeAction)
            {
                Application_Add_New_Open = true;

            }


            if (pVal.FormTypeEx.StartsWith("UDO_FT_ST_") && pVal.EventType == BoEventTypes.et_FORM_ACTIVATE && !pVal.BeforeAction && Application_Will_Open)
            {
                //Application_Will_Open = false;
                Form form = Loader.SBO_Application.Forms.Item(pVal.FormUID);
                //form.Visible = false;
                System.Threading.Thread t = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(Run_Close_Form_Thread));
                t.Start(form.UniqueID);
            }

            if (pVal.FormTypeEx.StartsWith("UDO_FT_ST_") && pVal.EventType == BoEventTypes.et_FORM_VISIBLE && !pVal.BeforeAction && Application_Add_New_Open)
            {
                Form form = Loader.SBO_Application.Forms.Item(pVal.FormUID);

                if (form.Mode == BoFormMode.fm_ADD_MODE)
                {
                    Application_Add_New_Open = false;
                    form.Visible = false;
                    form.Resize(100, 100);
                
                    
                    System.Threading.Thread t = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(Run_Close_Form_Thread));
                    t.Start(form.UniqueID);
                    string TableName = form.DataSources.DBDataSources.Item(0).TableName.Replace("@", "");
                    UDO_Definition Obj_Info = Objects_Logic.All_UDO_Definition.FirstOrDefault(O => O.Table_Name == TableName);
                    Form KHCF_UDO_Form = Loader.Open_UDO_Form(Obj_Info.KHCF_Object);

                    System.Threading.Thread.Sleep(500);
                 Loader.SBO_Application.Menus.Item("1282").Activate();
                   // KHCF_UDO_Form.Mode = BoFormMode.fm_ADD_MODE;

                    //Parent_Form Form_Obj = Loader.Get_Form_Object(Obj_Info.KHCF_Object.ToString());
                    //Form_Obj.company_For_Parent_Form = Loader.company;
                    //Form_Obj.KHCF_Object = Obj_Info.KHCF_Object;
                    //Form_Obj.UDO_Info = Obj_Info;
                    //Form_Obj.Set_Fields(KHCF_UDO_Form);


                }
            }

            if (pVal.FormTypeEx == "198")
            {
                if (pVal.EventType == BoEventTypes.et_MATRIX_LINK_PRESSED && pVal.BeforeAction)
                {
                    Form form = Loader.SBO_Application.Forms.Item(pVal.FormUID);
                    Matrix Mtx = (Matrix)form.Items.Item(pVal.ItemUID).Specific;

                    if (Mtx.Columns.Item(pVal.ColUID).Title == "KHCF Object Code")
                    {
                        Application_Will_Open = true;
                    }
                }
            }
            //if (pVal.EventType == BoEventTypes.et_ITEM_PRESSED)
            //{

            //}
            if (!All_Form_Types.Contains(pVal.FormTypeEx) && !KHCF_Additional_Forms_Type.Contains(pVal.FormTypeEx))
            {
                return;
            }
            try
            {
                if (pVal.EventType == BoEventTypes.et_MATRIX_LINK_PRESSED && pVal.BeforeAction)
                {
                    Form form = Loader.SBO_Application.Forms.Item(pVal.FormUID);
                    string UDO = "";
                    try
                    {
                        Grid Grd = (Grid)form.Items.Item(pVal.ItemUID).Specific;
                        UDO = ((EditTextColumn)Grd.Columns.Item(pVal.ColUID)).LinkedObjectType;
                    }
                    catch (Exception)
                    {

                    }
                    try
                    {
                        if (UDO == "")
                        {
                            Matrix Mat = (Matrix)form.Items.Item(pVal.ItemUID).Specific;
                            var x = Mat.Columns.Item(pVal.ColUID).Type;
                            // UDO = ((EditTextColumn)Mat.Columns.Item(pVal.ColUID)).LinkedObjectType;
                            SAPbouiCOM.LinkedButton oLink = ((SAPbouiCOM.LinkedButton)(Mat.Columns.Item(pVal.ColUID).ExtendedObject));
                            UDO = oLink.LinkedObjectType;

                        }
                    }
                    catch (Exception ex)
                    {

                    }
                    if (UDO == "")
                    {
                        return;
                    }
                    //UDO_Type_For_Open = UDO;
                    Application_Will_Open = true;
                }

                if (pVal.EventType == BoEventTypes.et_ITEM_PRESSED && pVal.BeforeAction)
                {
                    Form form = Loader.SBO_Application.Forms.Item(pVal.FormUID);
                    string UDO = "";
                    try
                    {
                        if (form.Items.Item(pVal.ItemUID).Type == BoFormItemTypes.it_LINKED_BUTTON)
                        {
                            LinkedButton Linked_Butt = (LinkedButton)form.Items.Item(pVal.ItemUID).Specific;
                            if (Linked_Butt == null)
                            {
                                return;
                            }
                            UDO = Linked_Butt.LinkedObjectType;
                        }
                    }
                    catch (Exception)
                    {
                        return;
                    }

                    if (UDO == "")
                    {
                        return;
                    }
                    //UDO_Type_For_Open = UDO;
                    Application_Will_Open = true;
                }
                
                if (pVal.EventType == BoEventTypes.et_CHOOSE_FROM_LIST && pVal.BeforeAction)
                {
                    if (Before_CFL_Form_ID == "")
                    {
                        Before_CFL_Form_ID = pVal.FormUID;
                        Before_CFL_Item_ID = pVal.ItemUID;
                    }     
                }

                if (pVal.EventType == BoEventTypes.et_FORM_CLOSE && !pVal.BeforeAction)
                {
                    Before_CFL_Form_ID = "";
                    Before_CFL_Item_ID = "";
                }

            }
            catch (Exception ex)
            {
                Loader.SBO_Application.StatusBar.SetText(ex.Message, BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Error);
            }
        }

        internal void Send_Alert_For_Approve(string UDO_Code)
        {
            string Message = $@"Please approve the {UDO_Info.Title}[{UDO_Code}]";
            string Can_Approve_UDF_Name = Objects_Logic.Get_Can_Approve_UDF_Name(UDO_Info);
           // string Authorizer_Role = Configurations.Get_Authorizer_Role_Code(company_For_Parent_Form);
            string SQL_Users = $@"Select USER_CODE FROM OUSR WHERE ""{Can_Approve_UDF_Name}"" = 'Y'";
            Recordset RC_Users = Helper.Utility.Execute_Recordset_Query(company_For_Parent_Form, SQL_Users);
            for (int i = 0; i < RC_Users.RecordCount; i++)
            {
                string UserCode = RC_Users.Fields.Item("USER_CODE").Value.ToString();
                Helper.Utility.SendAlertMessage(company_For_Parent_Form, UDO_Code, Message, UserCode, $"{UDO_Info.Title} Approval Request ", "KHCF Object Code", $"{UDO_Info.Title} [{UDO_Code}]", Utility.Get_UDO_Type_ID(UDO_Info.KHCF_Object));

                RC_Users.MoveNext();
            }

        }

        internal void Send_Alert_To_Creator(string UDO_Code,string Title,string Creator_Code)
        {
            string Message = $@"the {UDO_Info.Title}[{UDO_Code}] has been {Title}ed";
            Helper.Utility.SendAlertMessage(company_For_Parent_Form, UDO_Code, Message, Creator_Code, $"{UDO_Info.Title} {Title} Alert ", "KHCF Object Code", $"{UDO_Info.Title} [{UDO_Code}]", Utility.Get_UDO_Type_ID(UDO_Info.KHCF_Object));
             
        }

        internal static bool SBO_Application_FormDataEvent_For_Parent(ref BusinessObjectInfo BusinessObjectInfo)
        {
            bool BubbleEvent = true;
            try
            {
                if (BusinessObjectInfo.FormTypeEx.StartsWith("UDO_FT_ST_") && BusinessObjectInfo.EventType == BoEventTypes.et_FORM_DATA_LOAD && !BusinessObjectInfo.BeforeAction && Application_Will_Open)
                {
                    Application_Will_Open = false;
                    Form form = Loader.SBO_Application.Forms.Item(BusinessObjectInfo.FormUID);
                    form.Resize(100, 100);
                    string TableName = form.DataSources.DBDataSources.Item(0).TableName.Replace("@", "");
                    UDO_Definition KHCF_Object = Objects_Logic.All_UDO_Definition.FirstOrDefault(O => O.Table_Name == TableName);
                    Form KHCF_UDO_Form = Loader.Open_UDO_Form(KHCF_Object.KHCF_Object);

                    string Code = form.DataSources.DBDataSources.Item(0).GetValue("Code", 0).ToString();
                    
                    form.Visible = false;

                    ((EditText)KHCF_UDO_Form.Items.Item("5").Specific).Value = Code;
                    string f = KHCF_UDO_Form.Items.Item("1").UniqueID;
                    KHCF_UDO_Form.Items.Item("1").Click();
                    
                    System.Threading.Thread t = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(Run_Close_Form_Thread));
                    t.Start(form.UniqueID);
                }

                if (!All_Form_Types.Contains(BusinessObjectInfo.FormTypeEx) )
                {
                    return true;
                }

                //if (BusinessObjectInfo.EventType == BoEventTypes.et_FORM_DATA_ADD && !BusinessObjectInfo.BeforeAction)
                //{
                //    Before_CFL_Form_ID = "";
                //    Before_CFL_Item_ID = "";

                //}

                if (BusinessObjectInfo.EventType == BoEventTypes.et_FORM_DATA_ADD && !BusinessObjectInfo.BeforeAction && BusinessObjectInfo.ActionSuccess)
                {
                    if (!string.IsNullOrEmpty(Before_CFL_Form_ID))
                    {
                        string Form_ID = Before_CFL_Form_ID;
                        string Item_ID = Before_CFL_Item_ID;

                        try
                        {
                            //SAPbouiCOM.Form form = Loader.SBO_Application.Forms.Item(BusinessObjectInfo.FormUID);

                            SAPbouiCOM.Form Original_form = Loader.SBO_Application.Forms.Item(Form_ID);
                            string Item_Datasource = Helper.Utility.Get_Item_DB_Datasource(Original_form.Items.Item(Item_ID));

                            System.Xml.XmlDocument XML_Doc = new System.Xml.XmlDocument();
                            string XML_Text = BusinessObjectInfo.ObjectKey.Replace(" ", "").Replace("=", " = ");
                            XML_Doc.LoadXml(XML_Text);

                            string UDO_Code = XML_Doc.GetElementsByTagName("Code")[0].InnerText;
                            UDO_Definition Obj_Info = Get_KHCF_Object(Original_form);
                            Original_form.DataSources.DBDataSources.Item("@" + Obj_Info.Table_Name).SetValue(Item_Datasource, 0, UDO_Code);

                            SAPbouiCOM.Form form = Loader.SBO_Application.Forms.Item(BusinessObjectInfo.FormUID);
                            //form.Close();
                            Before_CFL_Form_ID = "";
                            Before_CFL_Item_ID = "";

                        }
                        catch (Exception ex)
                        {
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Loader.SBO_Application.StatusBar.SetText(ex.Message, BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Error);
                Loader.New_Msg = ex.Message;
                BubbleEvent = false;
            }

            return BubbleEvent;
        }

        internal static void Run_Close_Form_Thread(object obj)
        {
            Form form = Loader.SBO_Application.Forms.Item(obj.ToString());
            System.Threading.Thread.Sleep(300);
            form.Visible = false;
            System.Threading.Thread.Sleep(5000);

            try
            {
                Application_Will_Open = false;
                form.Close();
            }
            catch (Exception)
            {

            }
        }

        internal void Fill_Attachment_ComboBox(Matrix Mat_Att)
        {
            string SQL = $@"SELECT T0.""Code"", T0.""Name"" FROM ""@ST_ATTACHMENT_TYPE"" T0";
            Recordset RC = (Recordset)company_For_Parent_Form.GetBusinessObject(BoObjectTypes.BoRecordset);
            RC.DoQuery(SQL);
            Helper.Utility.FillMatrixComboBoxForSQL(company_For_Parent_Form, Mat_Att, "Type", SQL, true);

        }

        internal void Remove_Matrix_Row(Form form, string Matrix_ID)
        {
            Matrix Mat = (Matrix)form.Items.Item(Matrix_ID).Specific;
            int Count = Mat.RowCount;
            for (int i = Count; i > 0; i--)
            {
                //CheckBox Chk_Selected = (CheckBox)Mat.GetCellSpecific("SELECTED", i);
                CheckBox Chk_Selected = (CheckBox)Mat.Columns.Item("SELECTED").Cells.Item(i).Specific;
                if (Chk_Selected.Checked)
                {
                    Mat.DeleteRow(i);
                    // form.DataSources.DBDataSources.Item(1).RemoveRecord(Grid_Deleted_Row +1);
                    Mat.FlushToDataSource();
                    Mat.LoadFromDataSource();
                }
            }
        }

        internal void Load_Depends_Items(Form form)
        {
            foreach (Depends_List One_Item in Get_Depends_List_List())
            {
                string UDF_Parent_Name = "U_" + Utility.Get_Field_Name(KHCF_Object, One_Item.Parent_Item_ID);
                string Parent_Value = form.DataSources.DBDataSources.Item( UDO_Database_Table_Name).GetValue(UDF_Parent_Name, 0);
                string SQL = string.Format(One_Item.SQL, Parent_Value);
                Helper.Utility.Fill_One_ComboBoxBySQL(Loader.company, form,One_Item.Item_ID, SQL, true);
            }
        }

        internal void Load_One_Depends_Parent_Item(Form form, string Parent_Item_ID, bool Set_Empty = true)
        {
            Depends_List One_Item = Get_Depends_List_List().FirstOrDefault(F => F.Parent_Item_ID == Parent_Item_ID);
            if (One_Item == null)
            {
                throw new Logic.Custom_Exception($"There is no Depends list for the Parent Item ID[{Parent_Item_ID}]");
            }
            else
            {
                string UDF_Parent_Name = "U_" + Utility.Get_Field_Name(KHCF_Object, One_Item.Parent_Item_ID);
                string Parent_Value = form.DataSources.DBDataSources.Item(UDO_Database_Table_Name).GetValue(UDF_Parent_Name, 0);
                string SQL = string.Format(One_Item.SQL, Parent_Value);
                Helper.Utility.Fill_One_ComboBoxBySQL(Loader.company, form, One_Item.Item_ID, SQL, true);
                string UDF_Name = "U_" + Utility.Get_Field_Name(KHCF_Object, One_Item.Item_ID);
                if (Set_Empty == true)
                {
                    form.DataSources.DBDataSources.Item(UDO_Database_Table_Name).SetValue(UDF_Name, 0, "");
                }
            }

        }

        internal void Change_Visiblity(Form form, string[] Items, bool Visiblity)
        {
            foreach (string OneItem in Items)
            {
                try
                {
                    //if (form.Items.Item(OneItem).Type == BoFormItemTypes.it_EDIT)
                    //{
                        form.Items.Item(OneItem).SetAutoManagedAttribute(BoAutoManagedAttr.ama_Visible, -1,
                            Visiblity ? BoModeVisualBehavior.mvb_True : BoModeVisualBehavior.mvb_False);

                    //}
                    //else
                    //{
                    //    form.Items.Item(OneItem).Visible = Visiblity;
                    //}
                }
                catch (Exception) { }
            }
        }       

    }
}
