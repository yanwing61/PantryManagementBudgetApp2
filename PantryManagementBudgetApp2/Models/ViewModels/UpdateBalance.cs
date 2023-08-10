using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PersonalFinanceApplication.Models.ViewModels
{
    public class UpdateBalance
    {
        // This viewmodel is a class which stores info we need to present to /Balance/Update/{}

        // The existing balance info
        public BalanceDto SelectedBalance { get; set; }

        // All periods to choose from when updating this balance record
        public IEnumerable<PeriodDto> PeriodOptions { get; set; }
    }
}
