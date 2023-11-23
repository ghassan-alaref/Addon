using SAPbobsCOM;
using System;
using System.Collections.Generic;

namespace ST.KHCF.Customization.Logic.Classes
{

    public class Field_Definition
    {
        internal string Field_Name;
        internal BoFieldTypes Data_Type;
        internal string Field_Title;

        internal bool Is_Original_System_Field;
        internal KHCF_Objects KHCF_Object;
        internal string Linked_Table;
        internal string Item_ID;
        internal string Valid_Values_Text;
        internal int Length;
        internal bool Is_Temp;
        private string Column_Name;
        internal string Column_Name_In_DB
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Column_Name))
                {
                    Column_Name = "U_" + Field_Name; ;
                }
                return Column_Name;
            }
            set { Column_Name = value; }

        }
    }

    internal class Field_Data : Field_Definition
    {
        internal object Value;
    }

    internal class Inoice_Data
    {
        internal string Source_Code;
        internal double Premium_Amount;
        internal double Discount_Value;
        internal double Discount_Percentage;
        internal int Waiting_Period;
        internal bool Is_One_Installment;
        internal int Payment_Terms;

        internal Installment_Line[] Installment_Lines;
        internal string Currency;
    }

    internal class Installment_Line
    {
        internal double Amount;
        internal DateTime DueDate;
    }

    internal enum KHCF_Modules
    {
        Patient = 1,
        CCI = 2,
        Fundraising = 3,
      

    }

    internal enum KHCF_Objects
    {
        Fundraising_Individual_Card = 1,
        Fundraising_Individual_Address = 2,
        Fundraising_Individual_Contacts = 3,
        Patients_Card = 4,
        Social_Study = 5,
        Patients_Address = 6,
        Social_Study_Address = 7,
        Coverage_Request = 8,
        Coverage_Transaction = 9,
        Accommodation = 10,
        Treatment_Plan_Details = 11,
        Areas_and_Transportation_Types = 12,
        Sales_Target = 13,
        CCI_Member_Card = 14,
        CCI_Member_Card_Address = 15,
        CCI_Corporate_Member_Card = 16,
        CCI_Corporate_Member_Card_Address = 17,
        CCI_Corporate_Member_Card_Contacts = 18,
        Individual_Membership = 19,
        Coverage_Rules = 20,
        Fundraising_Corporate_Card = 21,
        Fundraising_Corporate_Address = 22,
        Fundraising_Corporate_Contacts = 23,
        Expected_Donations = 24,
        Actual_Donations = 25,
        Booth_Commission = 26,
        Plaque_Wall = 27,
        Plaque_Wall_Lines = 28,
        Tracking_Recognitions = 29,
        Recommending_Recognitions = 30,
        Naming = 31,
        Machinery = 32,
        Monthly_Giving = 33,
        Goodwill_Funds = 34,
        //Grants = 35,
        //Potential_Grants = 36,
        Won_Grants = 37,
        Pledges = 38,
        Dreams_Come_True = 39,
        Patient_Activity = 40,
        CCI_Corporate_Member_Card_Attachment = 41,
        CCI_Member_Card_Attachment = 42,
        Fundraising_Corporate_Member_Card_Attachment = 43,
        Fundraising_Member_Card_Attachment = 44,
        Coverage_Rules_Lines = 45,
        Communication_Log = 46,
        Corporate_Membership = 47,
        Sales_Target_Lines = 48,
        Commission_Rules = 49,
        Commission_Rules_Lines = 50,
        Expected_Donations_Patients_Lines = 51,
        Expected_Donations_Orphans_Lines = 52,
        Actual_Donations_Patients_Lines = 53,
        Actual_Donations_Orphans_Lines = 54,
        Tracking_Recognitions_Attachment = 55,
        Naming_Attachment = 56,
        Machinery_Installation_Info = 57,
        Pledges_Patients = 58,
        Monthly_Giving_Accounting = 59,
        Recommending_Recognitions_Types = 60,
        //Grants_Lines = 61,
        Goodwill_Funds_Attachment = 62,
        Dreams_Come_True_Attachment = 63,
        Patient_Activity_Patients_Lines = 64,
        Patient_Activity_Attachment = 65,
        Won_Grants_Wishes = 66,
        Won_Grants_Orph = 67,
        Won_Grants_Patients = 68,
        Won_Grants_Donation_Amount = 69,
        Patients_Card_Attachment = 70,
        Social_Study_Attachment = 71,
        Social_Study_Special_Needs = 72,
        Social_Study_Health_Issue = 73,
        Coverage_Request_Attachment = 74,
        Expected_Donations_Naming_Lines = 75,
        Expected_Donations_Machinery_Lines = 76,
        Actual_Donations_Naming_Lines = 77,
        Actual_Donations_Machinery_Lines = 78,
        Schools_Universites = 79,
        Schools_And_Universites_Details = 80,
        Pledges_Machinery_Lines = 81,
        Pledges_Participant_Lines = 82,
        Ambassador = 83,
        Ambassador_Team_Members = 84,
        CCI_Individual_Member_Card_Contacts = 85,
        CCI_Individual_Membership_Attachment = 86,
        CCI_Corporate_Membership_Attachment = 87,
        Goodwill_Funds_Participants = 88,
        Goodwill_Funds_Departments = 89,
        Goodwill_Funds_Co_Departments = 90,
        Goodwill_Funds_Report = 91,
        Goodwill_Funds_Nationality = 96,
        Goodwill_Funds_Sub_Nationality = 93,
        Goodwill_Funds_Cancer_Type = 94,
        Goodwill_Funds_Support_Type = 95,
        Social_Previous_Support = 92,
        Coverage_Request_Treatment = 97,
        Machinery_Attachment = 98,
        Expected_Donations_Department_Lines = 99,
        Fund_Target = 100,
        Fund_Target_Machines = 101,
        Fund_Target_Areas = 102,
        Fund_Rules = 103,
        Fund_Target_Wishes = 104,
        Fund_Target_Orph = 105,
        Fund_Target_Pats = 106,
        Machinery_Installation_Det = 107,
        Machinery_Installation_L = 108,
        Won_Grants_Post_Dates = 109,
        Expected_Donations_Payment_Lines = 110,
        Fund_Rules_Attachment = 111,
        Fund_Rules_Age = 112,
        Schools_And_Universites_Activity = 113,
        Schools_And_Universites_Item = 114,
        Schools_And_Universites_Grade = 115,
        Won_Grants_Reports_Req = 116
    }

    internal enum Object_Type
    {
        UDO_Main = 1,
        UDO_Child = 2,
        UDT = 3
    }

    internal class UDO_Data
    {
        internal UDO_Definition UDO_Info;
        internal List< List<Field_Data>> Fields_Data_Lines = new List<List<Field_Data>>();
        internal string Foreign_Key_Field;
        internal string Primary_Key;
        internal List<Field_Definition> Fields_Definition;
    }

    public class UDO_Definition
    {
        internal KHCF_Objects KHCF_Object;
        internal string Table_Name;
        internal string Title;
        internal Object_Type Type;
        internal KHCF_Objects[] Childs;
        internal bool Has_Draft;
        internal KHCF_Modules UDO_Modules;
        /// <summary>
        /// As Example U_National_ID
        /// </summary>
        internal string External_Key = "";
        internal string SQL_Existing_Query = "";
        internal bool Has_BP = false;

    }


    internal class KHCF_BP
    {
        internal int BP_Group;
        internal string CardName;
        internal string Currency;
        internal string FatherCode;
        internal string Email;
        internal string Mobile;
        internal BoFatherCardTypes FatherType;
        internal string MemberCard_Code;
        internal string GLAccount;
        internal bool Is_Lead;
        internal bool Is_Vendor;
        internal string Control_Account;
        internal List<BpAddress> addresses;
        internal List<BpContact> contacts = new List<BpContact>();
        internal List<BPAttatchment> attatchments = new List<BPAttatchment>();
        internal int SalesPersonCode;
    }

    internal class BpAddress
    {
        internal string AddressName;
        internal string AddressType;
        internal string Street;
        internal string City;
        internal string Country;
        internal string ZipCode;
        internal string County;
        internal string State;
        internal string Block;
        internal string BuildingFloorRoom;
        internal string AddressName2;
        internal string AddressName3;
        internal string StreetNo;
    }

    internal class BpContact
    {
        internal string ContactID;
        internal string Name;
        internal string Position;
        internal string Address;
        internal string Tel_1;
        internal string Tel_2;
        internal string Mobile;
        internal string Extention;
        internal string Email;
        internal string EmailGroup;
        internal string Pager;
        internal string Remarks_1;
        internal string Remarks_2;
        internal string Password;
        internal string PlaceOfBirth;
        internal string DateOfBirth;
        internal string Gender;
        internal string Profession;
        internal string CityOfBirth;
        internal string ConnectedAddress;
    }

    internal class BPAttatchment
    {
        internal string LinId;
        internal string FileName;
        internal string Description;
        internal string Type;
    }
}
