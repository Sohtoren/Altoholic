using System.Collections.Generic;

namespace Altoholic.Models
{
    public enum PvPRankEmblem
    {
        Bronze,
        Silver,
        Gold,
        Platinum,
        Diamond,
        Crystal

    }

    public class PvPRankTransient
    {
        public uint Id { get; set; }
        public Dictionary<int,string> GermanTransients { get; set; } = [];
        public Dictionary<int,string> EnglishTransients { get; set; } = [];
        public Dictionary<int,string> FrenchTransients { get; set; } = [];
        public Dictionary<int,string> JapaneseTransients { get; set; } = [];
    }
    public class PvPRank
    {
        public uint Id { get; init; }
        public uint ExpRequired { get; init; }
        public PvPRankTransient? Transients { get; init; }
    }
}
