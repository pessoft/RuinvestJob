using RuinvestUtils.Jobs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RuinvestJob
{
    static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            var currentProcess = Process.GetCurrentProcess();
            var proceses = Process.GetProcessesByName(currentProcess.ProcessName);

            if (proceses != null && proceses.Length > 1)
            {
                Application.Exit();
                return;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            VkAmountMoneyScheduler.Start();
            QiwiSenderMoneyScheduler.Start();

            var form = new RuinvestJob();
            using (NotifyIcon icon = new NotifyIcon())
            {
                icon.Icon = form.Icon;
                icon.ContextMenu = new ContextMenu(
                    new[]
                    {
                        new MenuItem("Exit", (s, e) =>
                        {
                            form.Close();
                            Application.Exit();
                        }),
                    });
                icon.Visible = true;

                Application.Run(form);
                icon.Visible = false;
            }
        }
    }
}
