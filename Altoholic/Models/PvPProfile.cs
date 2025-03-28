using System.Collections.Generic;

namespace Altoholic.Models
{
    public class PvPProfile
    {
        public byte CrystallineConflictCurrentRank { get; set; }
        public byte CrystallineConflictCurrentRiser { get; set; }
        public byte CrystallineConflictCurrentRisingStars { get; set; }
        public byte CrystallineConflictHighestRank { get; set; }
        public byte CrystallineConflictHighestRiser { get; set; }
        public byte CrystallineConflictHighestRisingStars { get; set; }
        public byte CrystallineConflictSeason { get; set; }
        public byte PreviousSeriesClaimedRank { get; set; }
        public byte PreviousSeriesRank { get; set; }
        public byte RankImmortalFlames { get; set; }
        public byte RankMaelstrom { get; set; }
        public byte RankTwinAdder { get; set; }
        public byte Series { get; set; }
        public byte SeriesClaimedRank { get; set; }
        public byte SeriesCurrentRank { get; set; }
        public uint ExperienceImmortalFlames { get; set; }
        public uint ExperienceMaelstrom { get; set; }
        public uint ExperienceTwinAdder { get; set; }
        public uint FrontlineTotalFirstPlace { get; set; }
        public uint FrontlineTotalMatches { get; set; }
        public uint FrontlineTotalSecondPlace { get; set; }
        public uint FrontlineTotalThirdPlace { get; set; }
        public uint RivalWingsTotalMatches { get; set; }
        public uint RivalWingsTotalMatchesWon { get; set; }
        public uint RivalWingsWeeklyMatches { get; set; }
        public uint RivalWingsWeeklyMatchesWon { get; set; }
        public ushort CrystallineConflictCasualMatches { get; set; }
        public ushort CrystallineConflictCasualMatchesWon { get; set; }
        public ushort CrystallineConflictCurrentCrystalCredit { get; set; }
        public ushort CrystallineConflictHighestCrystalCredit { get; set; }
        public ushort CrystallineConflictRankedMatches { get; set; }
        public ushort CrystallineConflictRankedMatchesWon { get; set; }
        public ushort FrontlineWeeklyFirstPlace { get; set; }
        public ushort FrontlineWeeklyMatches { get; set; }
        public ushort FrontlineWeeklySecondPlace { get; set; }
        public ushort FrontlineWeeklyThirdPlace { get; set; }
        public ushort SeriesExperience { get; set; }
        public Dictionary<uint, uint> SeriesPersonalRanks { get; set; } = [];
        public Dictionary<uint, uint> SeriesPersonalRanksClaimed { get; set; } = [];
    }
}
