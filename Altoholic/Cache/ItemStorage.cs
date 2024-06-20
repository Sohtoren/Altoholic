using Dalamud.Interface.Internal;
using Dalamud.Plugin.Services;
using Lumina.Excel.GeneratedSheets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altoholic
{
    public class ItemStorage(int size = 0)
    {
        private readonly Dictionary<uint, Item> _Items = new(size);

        public Item this[uint id]
            => LoadItem(id);

        public Item this[int id]
            => LoadItem((uint)id);

        public Item LoadItem(uint id)
        {
            if (_Items.TryGetValue(id, out var ret))
                return ret;

            ret = Utils.GetItemFromId(id)!;
            _Items[id] = ret;
            return ret;
        }

        public void Dispose()
        {
            _Items.Clear();
        }
    }
}
