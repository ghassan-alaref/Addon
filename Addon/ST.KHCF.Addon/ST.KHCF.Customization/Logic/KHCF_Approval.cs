using SAPbobsCOM;
using ST.KHCF.Customization.Logic.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ST.KHCF.Customization.Logic
{
    class KHCF_Approval
    {
        internal static void Approve_MemberCard(Company company, string UDO_Code, string Approval_Note, UDO_Definition UDO_Info)
        {

            try
            {
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
                string SQL_BP = $@"SELECT U_ST_BP_CODE, U_ST_CUSTOMER_GROUP,U_ST_CURRENCY,U_ST_ACCOUNT_MANAGER, {Name_Field} {Parent_Fields} FROM ""@{UDO_Info.Table_Name}"" WHERE ""Code"" = '{UDO_Code}' ";
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
                string value = RC_BP.Fields.Item("U_ST_CUSTOMER_GROUP").Value.ToString();
                if (string.IsNullOrEmpty(value))
                {
                    BP.BP_Group = 0;
                }
                else
                {
                    BP.BP_Group = int.Parse(RC_BP.Fields.Item("U_ST_CUSTOMER_GROUP").Value.ToString());
                }
               
                BP.CardName = RC_BP.Fields.Item(Name_Field).Value.ToString();
                BP.Currency = RC_BP.Fields.Item("U_ST_CURRENCY").Value.ToString();
                int.TryParse(RC_BP.Fields.Item("U_ST_ACCOUNT_MANAGER").Value.ToString(), out BP.SalesPersonCode);

                if (BP.BP_Group != 0)
                {
                    BusinessPartnerGroups bpGroup = (BusinessPartnerGroups)company.GetBusinessObject(BoObjectTypes.oBusinessPartnerGroups);
                    bpGroup.GetByKey(BP.BP_Group);
                    BP.Control_Account = bpGroup.UserFields.Fields.Item("U_ST_GL_ACCOUNT").Value.ToString();
                    string gg = bpGroup.UserFields.Fields.Item("U_ST_GL_ACCOUNT").Value.ToString(); 
                }

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
                string SQL = $@"Select U_ST_TEL1, U_ST_TEL2, U_ST_EMAIL, U_ST_FULL_NAME_AR,U_ST_ACCOUNT_MANAGER U_ST_FULL_NAME_EN
FROM ""@{UDO_Info.Table_Name}"" WHERE ""Code"" = '{UDO_Code}'";
                if (UDO_Info.KHCF_Object == KHCF_Objects.CCI_Corporate_Member_Card)
                {
                    //Name_Field = "U_ST_CORPORATE_ARABIC_NAME";
                    SQL = $@"Select U_ST_TEL1, U_ST_TEL_2, U_ST_EMAIL, U_ST_CORPORATE_ARABIC_NAME,U_ST_ACCOUNT_MANAGER U_ST_FULL_NAME_EN
FROM ""@{UDO_Info.Table_Name}"" WHERE ""Code"" = '{UDO_Code}'";
                }
                
                Recordset RC = Helper.Utility.Execute_Recordset_Query(company, SQL);

                // BP.CardForeignName = RC.Fields.Item("U_ST_FULL_NAME_EN").Value.ToString();
                //BP.Phone1 = RC.Fields.Item("U_ST_TEL1").Value.ToString();
                //BP.Phone2 = RC.Fields.Item("U_ST_TEL2").Value.ToString();
                BP.Mobile = RC.Fields.Item("U_ST_TEL1").Value.ToString();
                BP.Email = RC.Fields.Item("U_ST_EMAIL").Value.ToString();

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
                
                if (!UDO_Info.Table_Name.Contains("INDIV") && !UDO_Info.Table_Name.Contains("CORP"))
                {
                    string SQL_Contacts = $@"Select T0.""U_ST_CONTACT_ID"", T0.""U_ST_NAME"", T0.""U_ST_POSITION"", T0.""U_ST_ADDRESS"", T0.""U_ST_TELEPHONE_1"", T0.""U_ST_TELEPHONE_2"", T0.""U_ST_MOBILE_PHONE"", T0.""U_ST_E_MAIL"", T0.""U_ST_E_MAIL_GROUP"", T0.""U_ST_PAGER"", T0.""U_ST_REMARKS_1"", T0.""U_ST_REMARKS_2"", T0.""U_ST_PASSWORD"", T0.""U_ST_COUNTRY"",
T0.""U_ST_GENDER"", T0.""U_ST_PROFESSION"", T0.""U_ST_CITY_OF_BIRTH"", T0.""U_ST_CONNECTED_ADDRESS"", T0.""U_ST_DATE_OF_BIRTH"",  T0.""U_ST_FAX""  From ""@ST_CCI_CORP_CONT"" T0 where T0.""Code""='{UDO_Code}'";
                    Recordset RC_Contact = Helper.Utility.Execute_Recordset_Query(company,SQL_Contacts);
                    string SQL_ATT = $@"select T0.""LineId"", T0.""U_ST_FILE_NAME"", T0.""U_ST_DESCRIPTION"", T0.""U_ST_TYPE"" From ""@ST_CCI_CORP_ATT"" T0 Where T0.""Code""='{UDO_Code}'";
                    Recordset RC_Att = Helper.Utility.Execute_Recordset_Query(company, SQL_ATT);

                    if (RC_Contact.RecordCount > 0)
                    {
                        BP.contacts = new List<BpContact>();
                        for(int i =0;i<RC_Contact.RecordCount;i++) 
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
                if (BP_Code == "")
                {
                    BP_Code = Utility.Create_BP(company, BP);
                    Utility.Update_BP(company, BP_Code, UDO_Code, UDO_Info);
                    //BP_Code = Utility.Add_OR_Update_CCI_Card_BP(company, "", BP, UDO_Code, UDO_Info);
                    Field_Data Fld = new Field_Data() { Field_Name = "U_ST_BP_CODE", Value = BP_Code };
                    Utility.Update_UDO(company, UDO_Info, UDO_Code, new Field_Data[] { Fld });
                }
                else
                {
                    Utility.Update_BP(company, BP_Code, UDO_Code, UDO_Info);
                    Field_Data Fld = new Field_Data() { Field_Name = "U_ST_APPROVAL_STATUS", Value = "A" };
                    Field_Data Fld_Note = new Field_Data() { Field_Name = "U_ST_APPROVAL_NOTE", Value = Approval_Note };
                    Utility.Update_UDO(company, UDO_Info, UDO_Code, new Field_Data[] { Fld, Fld_Note });
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
                throw new Logic.Custom_Exception($"Error during Approve the Card[{UDO_Code}][{ex.Message}]");
            }
        }

        internal static void Reject_MemberCard(Company company, string UDO_Code, string Approval_Note, UDO_Definition UDO_Info)
        {
            Field_Data Fld = new Field_Data() { Field_Name = "U_ST_APPROVAL_STATUS", Value = "R" };
            Field_Data Fld_Note = new Field_Data() { Field_Name = "U_ST_APPROVAL_NOTE", Value = Approval_Note };
            Utility.Update_UDO(company, UDO_Info, UDO_Code, new Field_Data[] { Fld });
        }

        internal void Send_Alert_For_Approval_Creator(Company company, UDO_Definition UDO_Info, string UDO_Code, bool Is_Approved)
        {
            string Approval_Text;
            if (Is_Approved == true)
            {
                Approval_Text = "Approved";
            }
            else
            {
                Approval_Text = "Rejected";
            }
            string Message = $@"There {UDO_Info.Title}[{UDO_Code}] has been {Approval_Text}";
            // string Authorizer_Role = Configurations.Get_Authorizer_Role_Code(company_For_Parent_Form);
            string SQL_Users = $@"Select U_ST_CREATOR FROM ""@{UDO_Info.Table_Name}"" WHERE ""Code"" = '{UDO_Code}'";
            Recordset RC_Users = Helper.Utility.Execute_Recordset_Query(company, SQL_Users);
            for (int i = 0; i < RC_Users.RecordCount; i++)
            {
                string UserCode = RC_Users.Fields.Item("USER_CODE").Value.ToString();
                Helper.Utility.SendAlertMessage(company, UDO_Code, Message, UserCode, $"{UDO_Info.Title} need to Approve", "KHCF Object Code", $"{UDO_Info.Title} [{UDO_Code}]", Utility.Get_UDO_Type_ID(UDO_Info.KHCF_Object));

                RC_Users.MoveNext();
            }

        }
    }
}
