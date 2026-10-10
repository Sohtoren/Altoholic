namespace Altoholic.Models
{
    public class Armoire
    {
        public uint Id { get; init; }
        public uint ItemId { get; init; }
        public int Order { get; set; }
        public uint ArmoireCategory { get; set; }
        public uint ArmoireSubcategory { get; set; }
    }
}
