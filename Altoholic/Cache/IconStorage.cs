using Dalamud.Interface.Internal;
using Dalamud.Plugin.Services;
using System;
using System.Collections.Generic;
using static Dalamud.Plugin.Services.ITextureProvider;

namespace Altoholic.Cache
{
    public class IconStorage(ITextureProvider provider, int size = 0) : IDisposable
    {
        private readonly Dictionary<uint, IDalamudTextureWrap> _icons = new(size);

        public IDalamudTextureWrap this[uint id]
            => LoadIcon(id);

        public IDalamudTextureWrap this[int id]
            => LoadIcon((uint)id);

        public IDalamudTextureWrap LoadIcon(uint id, bool hq = false)
        {
            if (_icons.TryGetValue(id, out IDalamudTextureWrap? ret))
                return ret;

            ret = provider.GetIcon(id, hq ? IconFlags.ItemHighQuality : IconFlags.None)!;
            _icons[id] = ret;
            return ret;
        }
        public IDalamudTextureWrap LoadHighResIcon(uint id)
        {
            if (_icons.TryGetValue(id, out IDalamudTextureWrap? ret))
                return ret;

            ret = provider.GetIcon(id)!;
            _icons[id] = ret;
            return ret;
        }

        public void Dispose()
        {
            foreach (IDalamudTextureWrap icon in _icons.Values)
                icon.Dispose();
        }
    }
}
