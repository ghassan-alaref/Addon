using SAPbobsCOM;
using ST.KHCF.Customization.Logic.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ST.KHCF.Customization.Logic
{
    internal class General_Logic
    {
        internal static void Initialize()
        {
            Fields_Logic.Initialize();
            Objects_Logic.Initialize();

           Forms.Parent_Form.All_Form_Types = Objects_Logic.All_UDO_Definition.Where(O => O.Type == Object_Type.UDO_Main).Select(O => Utility.Get_Form_Type_ID(O.KHCF_Object)).ToArray();

        }
    }
}
