using System;
using System.Windows.Forms;

namespace Project_Game
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
            Application.Run(new FormMenu()); // Khởi động bằng menu thay vì Form1
        }
    }
}
