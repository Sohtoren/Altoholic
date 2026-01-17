namespace Altoholic.Models
{
    public class GlamourPlate
    {
        public byte Number { get; set; }
        public uint[] GearsIds { get; set; } = [];
        public byte[] Stain0Ids { get; set; } = [];
        public byte[] Stain1Ids { get; set; } = [];
    }
}