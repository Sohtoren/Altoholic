namespace Altoholic.Models
{
    public class Inventory
    {
        public int Id { get; set; }
        public uint ItemId { get; init; }
        public uint Quantity { get; init; }
        public bool HQ { get; init; }
        public ushort Spiritbond { get; set; }
        public ushort Condition { get; set; }
        public ulong CrafterContentID { get; set; }
        public ushort[] Materia { get; init; } = [];
        public byte[] MateriaGrade { get; init; } = [];
        public byte Stain { get; init; }
        public byte Stain2 { get; init; }
        public uint GlamourID { get; set; }
    }
}
