using Quartz;
using RuinvestLogic.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System.Web;

namespace RuinvestUtils.Jobs
{
    public class QiwiSenderMoney : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {

            var yesterday = DateTime.Now.AddDays(-1);
            var moneyOutOrders = DataWrapper.GetMoneyOrdersUpToDate(yesterday)
                .OrderBy(p => p.OrderDate)
                .ToList();
            var qiwi = new QiwiWallet();
            var balance = qiwi.GetBalance();


            if (moneyOutOrders != null && moneyOutOrders.Any())
            {
                foreach (var order in moneyOutOrders)
                {
                    if (order.Amount <= balance)
                    {
                        var amounts = BreakIntoShares(order.Amount);

                        foreach (var amount in amounts)
                        {
                            qiwi.SendMoney(order.NumberPurce.Replace("+", ""), amount);
                            Thread.Sleep(30);//что бы не долбиться без остановки на сервер qiwi
                        }
                    }

                    balance = qiwi.GetBalance();
                }
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