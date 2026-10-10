namespace Altoholic.Models
{
    public class FramerKit
    {
        public uint Id { get; init; }
        public string GermanName { get; set; } = string.Empty;
        public string EnglishName { get; set; } = string.Empty;
        public string FrenchName { get; set; } = string.Empty;
        public string JapaneseName { get; set; } = string.Empty;
        public uint Icon { get; set; }
        public uint ItemId {get; init; }
    }
}
