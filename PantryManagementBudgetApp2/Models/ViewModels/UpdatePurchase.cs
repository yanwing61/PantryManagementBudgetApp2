using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PantryManagementBudgetApp2.Models.ViewModels
{
    public class UpdatePurchase
    {
        //This viewmodel is a class which stores information that we need to present to /Purchase/Update/{id}

        //the existing Purchase information

        public PurchaseDto SelectedPurchase { get; set; }

        // all pantry items to choose from when updating a specific purchase
        public IEnumerable<PantryItemDto> PantryItemOptions { get; set; }

        // all periods to choose from when updating a specific purchase
        public IEnumerable<PeriodDto> PeriodOptions { get; set; }
    }
}