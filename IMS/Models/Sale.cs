using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace IMS.Models;

public partial class Sale
{
    public int Oid { get; set; }
    [Required]
    [NotNull]
    public string? CustomerName { get; set; }
    [Required]
    [NotNull]
    public int? CustomerContact { get; set; }

    public int? ProductId { get; set; } 

    public int ProductQuantinty { get; set; }
    [Required]
    [NotNull]
    public string? GstNo { get; set; }

    public DateOnly? SellDate { get; set; }

    public int? TotalAmount { get; set; }

    public string? Status { get; set; }

    public virtual Product? Product { get; set; }
}
