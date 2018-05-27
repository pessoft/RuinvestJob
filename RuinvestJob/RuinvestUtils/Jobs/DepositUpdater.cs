using Quartz;
using RuinvestLogic.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuinvestUtils.Jobs
{
    public class DepositUpdater : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                var deposits = DataWrapper.GetActiveDepost();
                var dateNow = DateTime.Now;
                var markFinishedIds = new List<int>();
                foreach(var deposit in deposits)
                {
                    if (deposit.EndDate <= dateNow)
                    {
                        markFinishedIds.Add(deposit.Id);
                    }
                }

                if (markFinishedIds != null && markFinishedIds.Any())
                {
                    DataWrapper.MarkDepositFinished(markFinishedIds);
                    deposits = deposits.Where(p => markFinishedIds.Contains(p.Id)).ToList();

                    foreach (var deposit in deposits)
                    {
                        DataWrapper.AddMoneyByUserId(deposit.UserId, deposit.EndAmount);
                    }
                }
            }
            catch (Exception ex)
            { }
        }
    }
}
