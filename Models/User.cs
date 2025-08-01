﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ERP_BI_Operations.Models;

[Index("Username", Name = "UQ__Users__536C85E4D10D51BE", IsUnique = true)]
public partial class User
{
    [Key]
    [Column("UserID")]
    public int UserId { get; set; }

    [Required]
    [StringLength(255)]
    public string Username { get; set; }

    [Required]
    [StringLength(255)]
    public string PasswordHash { get; set; }

    [StringLength(100)]
    public string Role { get; set; }

    [Column("EmployeeID")]
    public int? EmployeeId { get; set; }

    [ForeignKey("EmployeeId")]
    [InverseProperty("Users")]
    public virtual Employee Employee { get; set; }
}