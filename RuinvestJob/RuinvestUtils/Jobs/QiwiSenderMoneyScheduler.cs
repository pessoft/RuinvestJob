using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RuinvestUtils.Jobs
{
    public class QiwiSenderMoneyScheduler
    {
        public static async void Start()
        {
            IScheduler scheduler = await StdSchedulerFactory.GetDefaultScheduler();
            await scheduler.Start();

            IJobDetail job = JobBuilder.Create<QiwiSenderMoney>().Build();

            ITrigger trigger = TriggerBuilder.Create()
               .WithIdentity("trigger2", "group1")
               .StartNow()
               .WithSimpleSchedule(x => x
                   .WithIntervalInHours(1)
                   .RepeatForever())
               .Build();

            await scheduler.ScheduleJob(job, trigger);
        }
    }
}