using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PantryManagementBudgetApp2.Models
{
    public class Purchase
    {
        [Key]
        public int PurchaseID { get; set; }

        public decimal UnitPrice { get; set; }
        public int Qty { get; set; }

        [ForeignKey("Period")]
        public int PeriodId { get; set; }
        public virtual Period Period { get; set; }

        [ForeignKey("PantryItem")]
        public int PantryItemID { get; set; }
        public virtual PantryItem PantryItem { get; set; }

    }
}