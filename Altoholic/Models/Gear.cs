namespace Altoholic.Models
{
    public class Gear
    {
        public int Id { get; set; }
        public uint ItemId { get; set; }
        public bool HQ { get; set; }
        public bool CompanyCrestApplied { get; set; }
        public short Slot { get; init; }
        public ushort Spiritbond { get; init; }
        public ushort Condition { get; init; }
        public ulong CrafterContentID { get; init; }
        public ushort[] Materia { get; init; } = [];
        public byte[] MateriaGrade { get; init; } = [];
        public byte Stain { get; init; }
        public byte Stain2 { get; init; }
        public uint GlamourID { get; init; }
    }
}
