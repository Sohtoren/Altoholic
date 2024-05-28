namespace Altoholic.Models
{
    public class Item
    {
        public uint Id { get; init; }
        public int ItemID { get; init; }
        public bool HQ { get; init; } = false;
        public int Amount { get; init; } = 0;
    }
}
