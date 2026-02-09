using Lumina.Excel.Sheets;

namespace Altoholic.Models
{
    public class Roulette
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
        public string Category { get; set; } = string.Empty;
        public string DutyType { get; set; } = string.Empty;
        public uint Icon { get; set; }
        public ushort ItemLevelRequired { get; set; }
        public ushort ItemLevelSync { get; set; }
        public int RewardTomeA { get; set; }
        public int RewardTomeB { get; set; }
        public int RewardTomeC { get; set; }
        public uint? InstanceContent { get; set; }
        public uint? RequiredExVersion { get; set; }
        public uint OpenRule { get; set; }
        public byte RequiredLevel { get; set; }
        public byte SyncedFromLevel { get; set; }
        public uint ContentRouletteRoleBonus { get; set; }
        public int SortKey { get; set; }
        public uint ClassJobCategory { get; set; }
        public uint? ContentMemberType { get; set; }
        public byte QueueMaxPlayers { get; set; }
        public uint? ContentType { get; set; }
        public byte TimeLimit { get; set; }
        public byte TimeLimitMax { get; set; }
        public uint? LootModeType { get; set; }
        public byte PenaltyTimestampArrayIndex { get; set; }
        public sbyte CompletionArrayIndex { get; set; }
        public bool IsGoldSaucer { get; set; }
        public bool IsInDutyFinder{ get; set; }
        public bool IsPvP { get; set; }       
        public bool AppliesHighestAverageDutyItemLevel { get; set; }
        public bool AllowConsumableItems { get; set; }
        public bool AllowPhoenixDown { get; set; }
        public bool AllowReplacement { get; set; }
        public bool RatedMatch { get; set; }
        public bool Rated { get; set; }
        public bool IsRegistrationAllowedFromAnyDataCenter { get; set; }
    }
}
