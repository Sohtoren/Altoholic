namespace Altoholic.Models
{
    public class Hairstyle
    {
        public uint Id { get; init; }
        public string GermanName { get; set; } = string.Empty;
        public string EnglishName { get; set; } = string.Empty;
        public string FrenchName { get; set; } = string.Empty;
        public string JapaneseName { get; set; } = string.Empty;
        public bool IsPurchasable { get; init; }
        public uint SortKey { get; set; }
        public uint Icon { get; init; }
        public uint UnlockLink { get; init; }
        public uint ItemId { get; init; }
        public uint FeatureId { get; set; }
    }
}
