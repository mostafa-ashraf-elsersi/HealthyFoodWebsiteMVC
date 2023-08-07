using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthyFoodWebsite.Models
{
    public class ShoppingBagItem
    {
        [Key]
        public int Id { get; set; } = 0;

        public string Name { get; set; } = string.Empty;

        public float UnitPrice { get; set; }

        public int Quantity { get; set; } = 1;

        public float SubTotalPrice { get; set; }

        public string Status { get; set; } = "Active";

        [ForeignKey("Logger")]
        public int LoggerId { get; set; }

        public bool IsDeleted { get; set; } = false;


        // Navigation Properties
        public Logger Logger { get; set; } = null!;
    }
}
