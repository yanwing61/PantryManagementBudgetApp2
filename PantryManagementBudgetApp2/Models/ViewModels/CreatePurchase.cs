using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PantryManagementBudgetApp2.Models.ViewModels
{
    public class CreatePurchase
    {

        public IEnumerable<PeriodDto> PeriodOptions { get; set; }
        public IEnumerable<PantryItemDto> PantryItemOptions { get; set; }
    }
}