namespace Altoholic.Models
{
    public class Emote
    {
        public uint Id { get; init; }
        public string GermanName { get; set; } = string.Empty;
        public string EnglishName { get; set; } = string.Empty;
        public string FrenchName { get; set; } = string.Empty;
        public string JapaneseName { get; set; } = string.Empty;
        public TextCommand? TextCommand { get; init; }
        public uint Icon { get; set; }
        public uint UnlockLink { get; set; }
    }
}
