using Configuration;
using SmartTechnologiesM.Activation;
using System;
using System.Windows.Forms;

namespace Keygen
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var config = Config.GetConfig();

            Application.Run(new KeygenForm(config.Key, config.IV));
        }
    }
}
