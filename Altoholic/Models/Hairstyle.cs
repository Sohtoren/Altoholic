namespace Altoholic.Models
{
    public class Hairstyle
    {
        public uint Id { get; set; }
        public string GermanName { get; set; } = string.Empty;
        public string EnglishName { get; set; } = string.Empty;
        public string FrenchName { get; set; } = string.Empty;
        public string JapaneseName { get; set; } = string.Empty;
        public bool IsPurchasable { get; set; }
        public uint SortKey { get; set; }
        public uint Icon { get; set; }
        public uint UnlockLink { get; set; }
        public uint ItemId { get; set; }
        public uint FeatureId { get; set; }
    }
}
