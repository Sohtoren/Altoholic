namespace Altoholic.Models
{
    public class Vista
    {
        public uint Id { get; set; }
        public string GermanName { get; set; } = string.Empty;
        public string EnglishName { get; set; } = string.Empty;
        public string FrenchName { get; set; } = string.Empty;
        public string JapaneseName { get; set; } = string.Empty;
        public uint ItemId { get; set; }
        public uint LevelId { get; set; }
        public int MinLevel { get; set; }
        public int MaxLevel { get; set; }
        public uint Emote { get; set; }
        public int MinTime { get; set; }
        public int MaxTime { get; set; }
        public uint PlaceNameId { get; set; }
        public int IconList { get; set; }
        public int IconDiscovered{ get; set; }
        public int IconUndiscovered { get; set; }
        public bool IsInitial { get; set; }
        public string GermanImpression { get; set; } = string.Empty;
        public string GermanDescription{ get; set; } = string.Empty;
        public string EnglishImpression { get; set; } = string.Empty;
        public string EnglishDescription { get; set; } = string.Empty;
        public string FrenchImpression { get; set; } = string.Empty;
        public string FrenchDescription { get; set; } = string.Empty;
        public string JapaneseImpression { get; set; } = string.Empty;
        public string JapaneseDescription { get; set; } = string.Empty;
    }
}
