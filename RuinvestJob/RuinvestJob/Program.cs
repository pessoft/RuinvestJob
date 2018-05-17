using RuinvestUtils.Jobs;
using System;
using System.Collections.Generic;
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
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            VkAmountMoneyScheduler.Start();
            QiwiSenderMoneyScheduler.Start();

            var form = new RuinvestJob();
            using (NotifyIcon icon = new NotifyIcon())
            {
                icon.Icon = Icon.ExtractAssociatedIcon("title-ico.ico");
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
