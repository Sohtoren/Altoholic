using Dalamud.Interface.Textures;
using Dalamud.Interface.Textures.TextureWraps;
using Dalamud.Plugin.Services;
using System;
using System.Collections.Generic;

namespace Altoholic.Cache
{
    public class IconStorage(ITextureProvider provider, int size = 0) : IDisposable
    {
        private readonly Dictionary<uint, IDalamudTextureWrap> _icons = new(size);

        private IDalamudTextureWrap? _retainerIconsTexture;

        public void Init()
        {
            Plugin.Log.Debug("IconStoage Init() called");
            _retainerIconsTexture = Plugin.TextureProvider.GetFromGame("ui/uld/Retainer_hr1.tex").RentAsync().Result;
        }

        public IDalamudTextureWrap this[uint id]
            => LoadIcon(id);

        public IDalamudTextureWrap this[int id]
            => LoadIcon((uint)id);

        public IDalamudTextureWrap LoadIcon(uint id, bool hq = false)
        {
            if (_icons.TryGetValue(id, out IDalamudTextureWrap? ret))
                return ret;

            ret = provider.GetFromGameIcon(new GameIconLookup(id, hq)).RentAsync().Result;
            _icons[id] = ret;
            return ret;
        }
        public IDalamudTextureWrap LoadHighResIcon(uint id)
        {
            if (_icons.TryGetValue(id, out IDalamudTextureWrap? ret))
                return ret;

            ret = provider.GetFromGameIcon(new GameIconLookup(id)).RentAsync().Result;
            _icons[id] = ret;
            return ret;
        }

        public IDalamudTextureWrap? LoadRetainerJobIconTexture()
        {
            return _retainerIconsTexture;
        }

        public void Dispose()
        {
            foreach (IDalamudTextureWrap icon in _icons.Values)
                icon.Dispose();

            _retainerIconsTexture?.Dispose();
        }
    }
}
