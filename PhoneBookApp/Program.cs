using System;
using System.Windows.Forms;
using PhoneBookApp.Forms;

namespace PhoneBookApp
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {  
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new LoginForm());
        }
    }
}
