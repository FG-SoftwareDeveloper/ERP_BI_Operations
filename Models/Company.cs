﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ERP_BI_Operations.Models;

[Table("Company")]
public partial class Company
{
    [Key]
    [Column("CompanyID")]
    public int CompanyId { get; set; }

    [Required]
    [StringLength(255)]
    public string CompanyName { get; set; }

    [StringLength(255)]
    public string Address { get; set; }

    [StringLength(255)]
    public string ContactInfo { get; set; }

    [InverseProperty("Company")]
    public virtual ICollection<Account> Accounts { get; set; } = new List<Account>();

    [InverseProperty("Company")]
    public virtual ICollection<BankAccount> BankAccounts { get; set; } = new List<BankAccount>();

    [InverseProperty("Company")]
    public virtual ICollection<Customer> Customers { get; set; } = new List<Customer>();

    [InverseProperty("Company")]
    public virtual ICollection<Department> Departments { get; set; } = new List<Department>();

    [InverseProperty("Company")]
    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();

    [InverseProperty("Company")]
    public virtual ICollection<Equipment> Equipment { get; set; } = new List<Equipment>();

    [InverseProperty("Company")]
    public virtual ICollection<FinancialSnapshot> FinancialSnapshots { get; set; } = new List<FinancialSnapshot>();

    [InverseProperty("Company")]
    public virtual ICollection<ForecastedFinancial> ForecastedFinancials { get; set; } = new List<ForecastedFinancial>();

    [InverseProperty("Company")]
    public virtual ICollection<JournalEntry> JournalEntries { get; set; } = new List<JournalEntry>();

    [InverseProperty("Company")]
    public virtual ICollection<Kpi> Kpis { get; set; } = new List<Kpi>();

    [InverseProperty("Company")]
    public virtual ICollection<Material> Materials { get; set; } = new List<Material>();

    [InverseProperty("Company")]
    public virtual ICollection<PayrollRun> PayrollRuns { get; set; } = new List<PayrollRun>();

    [InverseProperty("Company")]
    public virtual ICollection<Project> Projects { get; set; } = new List<Project>();

    [InverseProperty("Company")]
    public virtual ICollection<Vendor> Vendors { get; set; } = new List<Vendor>();
}