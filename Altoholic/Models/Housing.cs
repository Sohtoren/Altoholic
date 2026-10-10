using System;

namespace Altoholic.Models
{
    public class Housing
    {
        public ulong Id { get; init; }
        public uint MapId { get; set; }
        public uint TerritoryId { get; init; }
        public sbyte Ward { get; init; }
        public sbyte Plot { get; init; }
        public byte Division { get; init; }
        public short Room { get; set; }
        public bool IsFreeCompany { get; set; }
        public DateTime? LastCheck { get; set; }
    }
}