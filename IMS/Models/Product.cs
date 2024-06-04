using System;
using System.Collections.Generic;

namespace IMS.Models;

public partial class Product
{
    public int ProductId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public int? CategoryId { get; set; }

    public int Quantity { get; set; }

    public int Price { get; set; }

    public virtual Category? Category { get; set; }

    public virtual ICollection<InventoryTransaction> InventoryTransactions { get; set; }

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } 

    public virtual ICollection<Sale> Sales { get; set; } 
}
