namespace Altoholic.Models
{
    public class Ornament
    {
        public uint Id { get; set; }
        public string GermanName { get; set; } = string.Empty;
        public string EnglishName { get; set; } = string.Empty;
        public string FrenchName { get; set; } = string.Empty;
        public string JapaneseName { get; set; } = string.Empty;
        public uint Icon { get; set; }
        public Transient? Transient { get; set; }
    }
}
