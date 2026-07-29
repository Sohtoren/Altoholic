using System;
using System.Collections.Generic;
using System.Text;

namespace Altoholic.Models
{
    public class OccultCrescent
    {
        public uint KnowledgeLevel { get; set; }
        public uint KnowledgeExperience { get; set; }
        public int EnlightenmentSilverPieces { get; set; }
        public int EnlightenmentSilverObols { get; set; }
        public int EnlightenmentGoldPieces { get; set; }
        public int EnlightenmentGoldObols { get; set; }
        public int SanguineCiphers { get; set; }
        public byte[] Jobs { get; set; } = new byte[24];
        public uint[] JobsExperiences { get; set; } = new uint[24];
        public uint[] LoreBooks { get; set; } = [];
    }
}
