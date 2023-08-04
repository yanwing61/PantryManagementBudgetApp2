using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PantryManagementBudgetApp2.Models.ViewModels
{
    public class UpdateInventory
    {
        //This viewmodel is a class which stores information that we need to present to /Inventory/Update/{id}

        //the existing inventory information

        public InventoryDto SelectedInventory { get; set; }

        // all pantry items to choose from when updating a specific inventory

        public IEnumerable<PantryItemDto> PantryItemOptions { get; set; }
    }
}