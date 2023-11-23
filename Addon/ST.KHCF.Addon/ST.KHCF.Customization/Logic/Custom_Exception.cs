using SAPbobsCOM;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ST.KHCF.Customization.Logic
{
    [Serializable]
    public class Custom_Exception : Exception
    {
        public Custom_Exception()
        {
           // Update_Log_File("There is no Message , its a defualt Constructor");
        }

        public Custom_Exception(string message) : base(message)
        {
           // Update_Log_File(message);
        }

        internal void Update_Log_File(string Message)
        {
            //return;
            string FilePath = "..\\..\\Log_File.txt";
            
            if (!File.Exists(FilePath))
            {
                // Create a file to write to.
                using (StreamWriter sw = File.CreateText(FilePath))
                {
                    sw.WriteLine("=======================Mesaage======================");
                    sw.WriteLine(DateTime.Now.ToString() + ":" + Message);
                    sw.WriteLine("=======================Stack Trace===================");
                    sw.WriteLine(this.StackTrace);
                    sw.WriteLine("=====================================================");
                }
                return;
            }

            using (StreamWriter sw = File.AppendText(FilePath))
            {
                sw.WriteLine("============Mesaage============");
                sw.WriteLine(DateTime.Now.ToString() + ":" + Message);
            }
        }
    }
}
