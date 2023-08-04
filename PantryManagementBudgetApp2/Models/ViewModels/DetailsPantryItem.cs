using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PantryManagementBudgetApp2.Models.ViewModels
{
    public class DetailsPantryItem
    {
        public PantryItemDto SelectedPantryItem { get; set; }

        public IEnumerable<InventoryDto> RelatedInventories { get; set; }

        public IEnumerable<TagDto> AssociatedTags { get; set; }

        public IEnumerable<TagDto> NotAssociatedTags { get; set; }
    }
}