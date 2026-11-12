using System;
using System.Windows.Forms;
using GestionMagasin.Forms;

namespace GestionMagasin
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            Application.Run(new LoginForm());
        }
    }
}
