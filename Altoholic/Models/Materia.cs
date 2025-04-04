namespace Altoholic.Models
{
    public class Materia
    {
        public uint Id { get; init; }
        public uint[] Grades { get; set; } = [16];
        public short[] Values { get; set; } = [16];
        public uint BaseParamId { get; set; }
    }
}
