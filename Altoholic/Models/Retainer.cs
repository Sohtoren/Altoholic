using System.Collections.Generic;

namespace Altoholic.Models
{
    public class Retainer
    {
        public uint Id { get; init; }
        public uint Owner { get; set; }
        public string Name { get; init; } = string.Empty;
        public int Level { get; init; } = 0;
        public int Gils { get; init; } = 0;
        public List<Item> Items { get; init; } = new List<Item>();
    }
}
