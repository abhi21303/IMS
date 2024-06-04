using System;
using System.Collections.Generic;

namespace IMS.Models;

public partial class InventoryTransaction
{
    public int TransactionId { get; set; }

    public int? ProductId { get; set; }

    public int Quantity { get; set; }

    public DateOnly TransactionDate { get; set; }

    public virtual Product? Product { get; set; }
}
