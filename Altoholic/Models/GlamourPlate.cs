namespace Altoholic.Models
{
    public class GlamourPlate
    {
        public byte Number { get; set; }
        public uint[] GearsIds { get; init; } = [];
        public byte[] Stain0Ids { get; init; } = [];
        public byte[] Stain1Ids { get; init; } = [];
    }
}