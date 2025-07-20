#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERP_BI_Operations.Models;

public partial class Account
{
    [Key]
    [Column("AccountID")]
    public int AccountId { get; set; }

    [Required(ErrorMessage = "Account Name is required")]
    [StringLength(255)]
    public string AccountName { get; set; }

    [StringLength(100)]
    public string AccountType { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    [Range(0, double.MaxValue, ErrorMessage = "Balance must be a positive number")]
    public decimal Balance { get; set; }

    [Column("CompanyID")]
    [Display(Name = "Company")]
    public int CompanyId { get; set; }

    [ForeignKey("CompanyId")]
    [InverseProperty("Accounts")]
    public virtual Company Company { get; set; }

    [InverseProperty("Account")]
    public virtual ICollection<JournalEntryLine> JournalEntryLines { get; set; } = new List<JournalEntryLine>();
}
