using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthyFoodWebsite.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; } = 0;

        public float TotalCost { get; set; }

        public string Status { get; set; } = "Active";

        public DateTime InitiatingDateAndTime { get; set; }

        public bool StartedPreparing { get; set; } = false;

        public bool StartedDelivering { get; set; } = false;

        [ForeignKey("Logger")]
        public int LoggerId { get; set; }

        public bool IsDeleted { get; set; } = false;


        // Navigation Properties
        public Logger Logger { get; set; } = null!;

        public ICollection<ShoppingBagItem> ShoppingBagItems { get; set; } = new List<ShoppingBagItem>();
    }
}
