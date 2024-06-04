namespace IMS.Models.DTOs
{
    public class InventoryTransactionDTO
    {
        public int? ProductId { get; set; }

        public int Quantity { get; set; }

        public DateOnly TransactionDate { get; set; }
    }
}
