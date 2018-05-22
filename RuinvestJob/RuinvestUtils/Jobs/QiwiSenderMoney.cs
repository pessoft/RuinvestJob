using Quartz;
using RuinvestLogic.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System.Web;
using RuinvestUtils.VK;
using NLog;

namespace RuinvestUtils.Jobs
{
    public class QiwiSenderMoney : IJob
    {
        private readonly Logger logger = LogManager.GetCurrentClassLogger();

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                logger.Info("Start QiwiSenderMoney");

                var vk = VKLogic.GetInstance();
                vk.SendMessage($"QiwiSenderMoney Start<br>Date: {DateTime.Now.ToString()}");
                var yesterday = DateTime.Now.AddDays(-1);
                var moneyOutOrders = DataWrapper.GetMoneyOrdersUpToDate(yesterday)
                    .OrderBy(p => p.OrderDate)
                    .ToList();
                var qiwi = new QiwiWallet();
                var balance = qiwi.GetBalance();
                vk.SendMessage($"QiwiSenderMoney Balance: {balance}<br>Amount sum: {moneyOutOrders.Sum(p => p.Amount)} ");

                if (moneyOutOrders != null && moneyOutOrders.Any())
                {
                    foreach (var order in moneyOutOrders)
                    {
                        if (order.Amount <= balance)
                        {
                            var amounts = BreakIntoShares(order.Amount);

                            foreach (var amount in amounts)
                            {
                                var success =  qiwi.SendMoney(order.NumberPurce.Replace("+", ""), amount);
                                if (success)
                                {
                                    order.Amount -= amount;
                                    DataWrapper.UpdateOrderMoneyOutFinished(new List<RuinvestLogic.Models.OrderMoneyOut> { order });
                                }
                                Thread.Sleep(50);//что бы не долбиться без остановки на сервер qiwi
                            }
                        }

                        balance = qiwi.GetBalance();
                    }
                }

                vk.SendMessage($"QiwiSenderMoney End<br>Date: {DateTime.Now.ToString()}<br>Balance: {qiwi.GetBalance()}");

                logger.Info("End QiwiSenderMoney");
            }
            catch (Exception ex)
            {
                logger.Error("Error QiwiSenderMoney", ex);
            }
        }

        private List<double> BreakIntoShares(double amount)
        {
            var maxAmount = 10000.00;
            var tmpAmount = amount;
            var result = new List<double>();

            if (maxAmount >= amount)
            {
                result.Add(amount);
            }
            else
            {
                while (tmpAmount > 0)
                {
                    if (maxAmount >= tmpAmount)
                    {
                        result.Add(tmpAmount);
                    }
                    else
                    {
                        result.Add(maxAmount);
                    }

                    tmpAmount -= maxAmount;
                }
            }

            return result;
        }
    }
}