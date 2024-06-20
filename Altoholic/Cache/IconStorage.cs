using Dalamud.Interface.FontIdentifier;
using Dalamud.Interface.Internal;
using Dalamud.Plugin.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Dalamud.Plugin.Services.ITextureProvider;

namespace Altoholic.Cache
{
    public class IconStorage(ITextureProvider provider, int size = 0)
    {
        private readonly Dictionary<uint, IDalamudTextureWrap> _icons = new(size);

        public IDalamudTextureWrap this[uint id]
            => LoadIcon(id);

        public IDalamudTextureWrap this[int id]
            => LoadIcon((uint)id);

        public IDalamudTextureWrap LoadIcon(uint id, bool hq = false)
        {
            if (_icons.TryGetValue(id, out var ret))
                return ret;

            //ret = provider.GetIcon(id)!;
            ret = provider.GetIcon(id, hq ? IconFlags.ItemHighQuality : IconFlags.None)!;
            _icons[id] = ret;
            return ret;
        }

        public void Dispose()
        {
            foreach (var icon in _icons.Values)
                icon.Dispose();
        }
    }
}
