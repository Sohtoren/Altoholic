using System;
using System.Collections.Generic;
using System.Text;

namespace Altoholic.Models
{
    public class Bozja
    {
        public uint CurrentLevel { get; init; }
        public uint MaxLevel { get; init; }
        public uint CurrentExperience { get; init; }
        public uint NeededExperience { get; init; }
        public byte[] HolsterActions { get; init; } = new byte[100];

        public int GetCurrentRank()
        {
            return Array.IndexOf(Utils.BOZJA_RANKS, NeededExperience);
        }
    }
}
