using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PersonalFinanceApplication.Models.ViewModels
{
    public class DetailsPeriod
    {
        public PeriodDto SelectedPeriod { get; set; }
        public IEnumerable<BalanceDto> RelatedBalances { get; set;}
        public IEnumerable<CashflowDto> RelatedCashflows { get; set; }
    }
}
