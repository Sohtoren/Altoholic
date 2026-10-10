namespace Altoholic.Models
{
    public class Materia
    {
        public uint Id { get; init; }
        public uint[] Grades { get; init; } = [16];
        public short[] Values { get; init; } = [16];
        public uint BaseParamId { get; init; }
    }
}
