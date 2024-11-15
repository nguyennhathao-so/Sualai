using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CNPM
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new LogIn());

            //Application.Run(new SignUp());
            //Application.Run(new TrangChu());
            //Application.Run(new ChiTiet());
            //Application.Run(new ThemKhoHang());
        }
    }
}
