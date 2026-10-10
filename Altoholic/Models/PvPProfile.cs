using System.Collections.Generic;

namespace Altoholic.Models
{
    public class PvPProfile
    {
        public byte CrystallineConflictCurrentRank { get; init; }
        public byte CrystallineConflictCurrentRiser { get; init; }
        public byte CrystallineConflictCurrentRisingStars { get; init; }
        public byte CrystallineConflictHighestRank { get; init; }
        public byte CrystallineConflictHighestRiser { get; init; }
        public byte CrystallineConflictHighestRisingStars { get; init; }
        public byte CrystallineConflictSeason { get; init; }
        public byte PreviousSeriesClaimedRank { get; init; }
        public byte PreviousSeriesRank { get; init; }
        public byte RankImmortalFlames { get; init; }
        public byte RankMaelstrom { get; init; }
        public byte RankTwinAdder { get; init; }
        public byte Series { get; init; }
        public byte SeriesClaimedRank { get; init; }
        public byte SeriesCurrentRank { get; init; }
        public uint ExperienceImmortalFlames { get; init; }
        public uint ExperienceMaelstrom { get; init; }
        public uint ExperienceTwinAdder { get; init; }
        public uint FrontlineTotalFirstPlace { get; init; }
        public uint FrontlineTotalMatches { get; init; }
        public uint FrontlineTotalSecondPlace { get; init; }
        public uint FrontlineTotalThirdPlace { get; init; }
        public uint RivalWingsTotalMatches { get; init; }
        public uint RivalWingsTotalMatchesWon { get; init; }
        public uint RivalWingsWeeklyMatches { get; init; }
        public uint RivalWingsWeeklyMatchesWon { get; init; }
        public ushort CrystallineConflictCasualMatches { get; init; }
        public ushort CrystallineConflictCasualMatchesWon { get; init; }
        public ushort CrystallineConflictCurrentCrystalCredit { get; init; }
        public ushort CrystallineConflictHighestCrystalCredit { get; init; }
        public ushort CrystallineConflictRankedMatches { get; init; }
        public ushort CrystallineConflictRankedMatchesWon { get; init; }
        public ushort FrontlineWeeklyFirstPlace { get; init; }
        public ushort FrontlineWeeklyMatches { get; init; }
        public ushort FrontlineWeeklySecondPlace { get; init; }
        public ushort FrontlineWeeklyThirdPlace { get; init; }
        public ushort SeriesExperience { get; init; }
        public Dictionary<uint, uint> SeriesPersonalRanks { get; init; } = [];
        public Dictionary<uint, uint> SeriesPersonalRanksClaimed { get; init; } = [];
    }
}
