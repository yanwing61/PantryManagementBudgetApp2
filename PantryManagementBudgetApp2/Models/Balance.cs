using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PantryManagementBudgetApp2.Models
{
    public class Balance
    {
        [Key]
        public int BalanceId { get; set; }

        // 1 period record corresponds to 1 balance & 1 cashflow record
        [ForeignKey("Period")]
        public int PeriodId { get; set; }
        public virtual Period Period { get; set; }

        // Balance of assets e.g. chequing & savings accounts
        public decimal ChequingAcct { get; set; }
        public decimal SavingsAcct { get; set; }

        // Balance of liabilities e.g. revolving & instalment credit
        public decimal RevolvingCrdt { get; set; }
        public decimal InstalmentCrdt { get; set; }
    }

    public class BalanceDto
    {
        public int BalanceId { get; set; }
        public int PeriodYear { get; set; }
        public int PeriodMonth { get; set; }
        public int PeriodId { get; set; }
        public decimal ChequingAcct { get; set; }
        public decimal SavingsAcct { get; set; }
        public decimal RevolvingCrdt { get; set; }
        public decimal InstalmentCrdt { get; set; }
    }
}