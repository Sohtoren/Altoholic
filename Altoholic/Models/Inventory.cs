namespace Altoholic.Models
{
    public class Inventory
    {
        public int Id { get; set; }
        public uint ItemId { get; set; }
        public uint Quantity { get; set; }
        public bool HQ { get; set; }
    }
}
