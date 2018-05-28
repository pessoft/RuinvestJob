using Quartz;
using RuinvestLogic.Logic;
using RuinvestLogic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using RuinvestUtils.VK;


namespace RuinvestUtils.Jobs
{
    public class QiwiSenderMoney : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {

            try
            {
                var vk = VKLogic.GetInstance();
                vk.SendMessage($"QiwiSenderMoney Start<br>Date: {DateTime.Now.ToString()}");
                var date = DateTime.Now;
                var moneyOutOrders = DataWrapper.GetMoneyOrdersUpToDate(date)
                    .OrderBy(p => p.OrderDate)
                    .ToList();
                var qiwi = new QiwiWallet();
                var balance = qiwi.GetBalance();
                vk.SendMessage($"QiwiSenderMoney Balance: {balance}<br>Amount sum: {moneyOutOrders.Sum(p => p.Amount)} ");

                if (moneyOutOrders != null && moneyOutOrders.Any())
                {
                    foreach (var order in moneyOutOrders)
                    {
                        if ((order.Amount - order.AmountOut > 0) 
                            && (order.Amount - order.AmountOut <= balance))
                        {
                            var amounts = BreakIntoShares(order.Amount - order.AmountOut);

                            foreach (var amount in amounts)
                            {
                                var success = qiwi.SendMoney(order.NumberPurce.Replace("+", ""), amount);
                                if (success)
                                {
                                    order.AmountOut += amount;
                                    var upData = new List<OrderMoneyOut> { order };
                                    DataWrapper.UpdateOrderMoneyOutFinished(upData);
                                }
                                //что бы не долбиться без остановки на сервер qiwi
                                Thread.Sleep(50);
                            }
                        }

                        if (order.AmountOut >= order.Amount)
                        {
                            DataWrapper.MarkOrderMoneyOutFinished(order.OrderId);
                        }

                        balance = qiwi.GetBalance();
                    }
                }

                vk.SendMessage($"QiwiSenderMoney End<br>Date: {DateTime.Now.ToString()}<br>Balance: {qiwi.GetBalance()}");
            }
            catch (Exception ex)
            { }
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