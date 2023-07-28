using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PantryManagementBudgetApp2.Models
{
    public class Inventory
    {
        [Key]
        public int InventoryID { get; set; }
        public int InventoryQty { get; set; }
        public DateTime InventoryLogDate { get; set; }

        //Each inventory record belongs to one pantry item
        [ForeignKey("PantryItem")]
        public int PantryItemID { get; set; }
        public virtual PantryItem PantryItem { get; set; }
    }

    public class InventoryDto
    {
        public int InventoryID { get; set; }
        public int InventoryQty { get; set; }
        public DateTime InventoryLogDate { get; set; }
        public int PantryItemID { get; set; }
        public string PantryItemName { get; set; }
    }
}