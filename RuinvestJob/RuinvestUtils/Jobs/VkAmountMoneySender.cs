using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Quartz;
using RuinvestLogic.Logic;
using RuinvestUtils.VK;

namespace RuinvestUtils.Jobs
{
    public class VkAmountMoneySender : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            var yesterday = DateTime.Now.AddDays(-1);
            var moneyOutOrders = DataWrapper.GetMoneyOrdersByDate(yesterday);
            var sumYandex = moneyOutOrders.Where(p => p.TypePurce == MoneyOutEnum.Yandex).Sum(p => p.Amount);
            var sumQiwi = moneyOutOrders.Where(p => p.TypePurce == MoneyOutEnum.Qiwi).Sum(p => p.Amount);
            var message = $"По состоянию на <br>«{ DateTime.Now.ToString("d MMMM yyyy")} г.» нужно пополнить кошельки<br>Qiwi: {sumQiwi} руб.<br>Yandex: {sumYandex} руб.";

            var vk = VKLogic.GetInstance();
            vk.SendMessage(message);
        }
    }
}