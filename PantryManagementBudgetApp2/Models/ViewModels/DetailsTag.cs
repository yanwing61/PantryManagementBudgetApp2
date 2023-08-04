using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PantryManagementBudgetApp2.Models.ViewModels
{
    public class DetailsTag
    {
        public TagDto SelectedTag { get; set; }

        public IEnumerable<PantryItemDto> CarryTags { get; set; }
    }
}