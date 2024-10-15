namespace Altoholic.Models
{
    public enum DutyType
    {
        Unknown,
        Savage,
        Ultimate,
        Extreme,
        Unreal,
        Criterion,
        Alliance,
    }
    public class Duty
    {
        public uint Id { get; set; }
        public string GermanName { get; set; } = string.Empty;
        public string GermanTransient { get; set; } = string.Empty;
        public string EnglishName { get; set; } = string.Empty;
        public string EnglishTransient { get; set; } = string.Empty;
        public string FrenchName { get; set; } = string.Empty;
        public string FrenchTransient { get; set; } = string.Empty;
        public string JapaneseName { get; set; } = string.Empty;
        public string JapaneseTransient { get; set; } = string.Empty;
        public uint Icon { get; set; }
        public uint Image { get; set; }
        public uint Content { get; set; }
        public uint ContentTypeId { get; set; }
        public int SortKey { get; set; }
        public int TransientKey { get; set; }
        public uint? ExVersion { get; set; }
        public bool HighEndDuty { get; set; }
        public bool AllowUndersized { get; set; }
        public uint ContentMemberType {get; set; }
    }
}
