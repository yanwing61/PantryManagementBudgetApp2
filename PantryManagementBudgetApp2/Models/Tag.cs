using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PantryManagementBudgetApp2.Models
{
    public class Tag
    {
        [Key]
        public int TagID { get; set; }
        public string TagName { get; set; }

        //A tag can be applied to many pantry items
        public ICollection<PantryItem> PantryItems { get; set; }
    }

    public class TagDto
    {
        public int TagID { get; set; }
        public string TagName { get; set; }

    }
}