using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuinvestUtils.Jobs
{
    public class DepositUpdaterScheduler
    {
        public static async void Start()
        {
            IScheduler scheduler = await StdSchedulerFactory.GetDefaultScheduler();
            await scheduler.Start();

            IJobDetail job = JobBuilder.Create<DepositUpdater>().Build();

            ITrigger trigger = TriggerBuilder.Create()
               .WithIdentity("depositUpdaterScheduler", "group1")
               .StartNow()
               .WithSimpleSchedule(x => x
                   .WithIntervalInMinutes(15)
                   .RepeatForever())
               .Build();

            await scheduler.ScheduleJob(job, trigger);
        }
    }
}
