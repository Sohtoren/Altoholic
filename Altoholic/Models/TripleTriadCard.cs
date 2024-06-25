namespace Altoholic.Models
{
    public class TripleTriadCard
    {
        public uint Id { get; set; }
        public string GermanName { get; set; } = string.Empty;
        public string GermanDescription { get; set; } = string.Empty;
        public string EnglishName { get; set; } = string.Empty;
        public string EnglishDescription { get; set; } = string.Empty;
        public string FrenchName { get; set; } = string.Empty;
        public string FrenchDescription { get; set; } = string.Empty;
        public string JapaneseName { get; set; } = string.Empty;
        public string JapaneseDescription { get; set; } = string.Empty;
        public uint Icon { get; set; }
    }
}
