using System;
using System.Collections.Generic;
using System.Text;

namespace Altoholic.Models
{
    public class Eureka
    {
        public uint CurrentLevel { get; set; }
        public uint MaxLevel { get; set; }
        public uint CurrentExperience { get; init; }
        public uint NeededExperience { get; init; }
        public byte[] Logos { get; init; } = new byte[100];

        public int GetCurrentLevel()
        {
            return Array.IndexOf(Utils.EUREKA_ELEMENTAL_LEVELS, NeededExperience);
        }
    }
}
