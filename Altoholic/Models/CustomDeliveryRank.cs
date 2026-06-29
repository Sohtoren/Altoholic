using System;

namespace Altoholic.Models
{
    public class CustomDeliveryRank

    {
        public uint Id { get; init; }
        public byte HeartCount { get; set; }
        public ushort UsedAllowance { get; set; }
        public DateTime LastCheck { get; set; }
    }
}
