using FFXIVClientStructs.FFXIV.Client.System.String;
using System.Collections.Generic;

namespace Altoholic.Models
{
    public class Retainer
    {
        public ulong Id { get; init; }
        public bool Available { get; set; } = false;
        public string Name { get; set; } = string.Empty;
        public byte ClassJob { get; set; }
        public int Level { get; set; } = 0;
        public uint Gils { get; set; } = 0;
        public FFXIVClientStructs.FFXIV.Client.Game.RetainerManager.RetainerTown Town { get; set; } = 0;
        public uint MarketItemCount { get; set; } = 0;
        public uint MarketExpire { get; set; } = 0;
        public uint VentureID { get; set; } = 0;
        public uint VentureComplete { get; set; } = 0;
        public long LastUpdate { get; set; } = 0;
        public int DisplayOrder { get; set; } = 0;
        public List<Gear> Gear { get; set; } = [];
        public List<Inventory> Inventory { get; set; } = [];
        public List<Inventory> MarketInventory { get; set; } = [];
    }
}
