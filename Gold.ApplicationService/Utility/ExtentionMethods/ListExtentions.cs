using Gold.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gold.ApplicationService.Utility.ExtentionMethods
{
    public static class ListExtentions
    {
        public static long PaymentsAmount(this List<Installment> installments)
        {
            long amount = 0;

            foreach (var item in installments)
            {
                if (item.Payments !=null)
                {
                    foreach (var payment in item.Payments)
                    {
                        amount += payment.Amount;
                    }
                }
            }
            return amount;
        }
    }
}
