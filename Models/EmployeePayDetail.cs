﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ERP_BI_Operations.Models;

public partial class EmployeePayDetail
{
    [Key]
    [Column("PayDetailID")]
    public int PayDetailId { get; set; }

    [Column("EmployeeID")]
    public int EmployeeId { get; set; }

    [Column("PayrollRunID")]
    public int PayrollRunId { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? GrossPay { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? NetPay { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? TaxesWithheld { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? Deductions { get; set; }

    [ForeignKey("EmployeeId")]
    [InverseProperty("EmployeePayDetails")]
    public virtual Employee Employee { get; set; }

    [ForeignKey("PayrollRunId")]
    [InverseProperty("EmployeePayDetails")]
    public virtual PayrollRun PayrollRun { get; set; }
}