using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PantryManagementBudgetApp2.Models
{
    public class PantryItem
    {
        [Key]
        public int PantryItemID { get; set; }
        public string PantryItemName { get; set; }
        public int PantryItemCurrentQty { get; set; }

        //A pantry item can have many tags
        public ICollection<Tag> Tags { get; set; }
    }
}