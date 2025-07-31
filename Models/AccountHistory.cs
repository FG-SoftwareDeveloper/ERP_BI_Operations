using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERP_BI_Operations.Models
{
    // Add the [Table] attribute with the exact singular table name from your database
    [Table("AccountHistory")]
    public class AccountHistory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int AccountId { get; set; }

        [Required]
        [Column(TypeName = "date")] // Ensures it's stored as a DATE type in SQL
        public DateTime DateRecorded { get; set; }

        [Required]
        [Column(TypeName = "decimal(18, 2)")] // Ensure precise decimal storage for currency
        public decimal Balance { get; set; }

        // Navigation property to the Account (optional, but good for relationships)
        public virtual Account Account { get; set; }
    }
}
