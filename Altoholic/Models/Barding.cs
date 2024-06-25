namespace Altoholic.Models
{
    public class Barding
    {
        public uint Id { get; set; }
        public string GermanName { get; set; } = string.Empty;
        public string EnglishName { get; set; } = string.Empty;
        public string FrenchName { get; set; } = string.Empty;
        public string JapaneseName { get; set; } = string.Empty;
        public uint Icon { get; set; }
        public uint IconHead { get; set; }
        public uint IconBody { get; set; }
        public uint IconLegs { get; set; }
    }
}
