using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Altoholic.Models
{
    public class Housing
    {
        public long Id { get; set; }
        public uint MapId { get; set; }
        public uint TerritoryId { get; set; }
        public sbyte Ward { get; set; }
        public sbyte Plot { get; set; }
        public byte Division { get; set; }
        public short Room { get; set; }
        public bool IsFreeCompany { get; set; }
    }
}