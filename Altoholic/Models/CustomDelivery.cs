namespace Altoholic.Models
{
    public class CustomDelivery

    {
        public uint Id { get; init; }
        public uint Icon { get; set; }
        public short GlamourIndex { get; set; }
        public short LevelUnlock { get; set; }
        public ushort DeliveriesPerWeek { get; set; }
        public uint QuestRequired { get; set; }
        public string GermanName { get; set; } = string.Empty;
        public string EnglishName { get; set; } = string.Empty;
        public string FrenchName { get; set; } = string.Empty;
        public string JapaneseName { get; set; } = string.Empty;
    }
}
