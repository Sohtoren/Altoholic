namespace Altoholic.Models
{
    public class Inventory
    {
        public int Id { get; set; }
        public uint ItemId { get; set; }
        public uint Quantity { get; set; }
        public bool HQ { get; set; }
        public ushort Spiritbond { get; set; }
        public ushort Condition { get; set; }
        public ulong CrafterContentID { get; set; }
        public ushort[] Materia { get; set; } = [];
        public byte[] MateriaGrade { get; set; } = [];
        public byte Stain { get; set; }
        public byte Stain2 { get; set; }
        public uint GlamourID { get; set; }
    }
}
