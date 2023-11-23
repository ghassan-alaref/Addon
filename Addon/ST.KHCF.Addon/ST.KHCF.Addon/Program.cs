using System;

namespace ST.KHCF.Addon
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {

            Customization.Loader loader = new Customization.Loader();
            System.Windows.Forms.Application.Run();
        }
    }
}
