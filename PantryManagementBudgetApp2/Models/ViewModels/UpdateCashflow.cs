using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PersonalFinanceApplication.Models.ViewModels
{
    public class UpdateCashflow
    {
        // This viewmodel is a class which stores info we need to present to /Cashflow/Update/{}

        // The existing cashflow info
        public CashflowDto SelectedCashflow { get; set; }

        // All periods to choose from when updating this cashflow record
        public IEnumerable<PeriodDto> PeriodOptions { get; set; }
    }
}
