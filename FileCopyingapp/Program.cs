using FileCopyingapp;
using System;
using System.Diagnostics;
using System.Linq;
using System.Security.Principal;
using System.Windows.Forms;

namespace FileCopyDesktopApp
{
    static class Program
    {

        [STAThread]
        static void Main(string[] args)
        {
            if (!IsRunningAsAdmin())
            {
                RelaunchAsAdmin(args);
                return; // Exit the current (non-admin) process
            }
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm(args));
        }
        static bool IsRunningAsAdmin()
        {
            var identity = WindowsIdentity.GetCurrent();
            var principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        static void RelaunchAsAdmin(string[] args)
        {
             string exeName = Process.GetCurrentProcess().MainModule.FileName;
            string argString = string.Join(" ", args.Select(a => $"\"{a}\""));

            var startInfo = new ProcessStartInfo(exeName, argString)
            {
                Verb = "runas", // Triggers UAC prompt
                UseShellExecute = true
            };

            try
            {
                Process.Start(startInfo); // Launch elevated version
            }
            catch
            {
                MessageBox.Show("This application requires administrator privileges.", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            Environment.Exit(0); // Exit the non-elevated version
        }
    }
}