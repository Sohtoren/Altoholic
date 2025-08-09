﻿using Dalamud.Interface;
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

        private IDalamudTextureWrap? _retainerIconsTextureWrap;
        private IDalamudTextureWrap? _rolesTextureWrap;
        private readonly Dictionary<int, IDalamudTextureWrap?> _itemDetailsTextures = [];
        private UldWrapper? _itemDetailsUld;

        public void Init()
        {
            Plugin.Log.Debug("IconStorage Init() called");
            _retainerIconsTextureWrap = Plugin.TextureProvider.GetFromGame("ui/uld/Retainer_hr1.tex").RentAsync().Result;
            _rolesTextureWrap = Plugin.TextureProvider.GetFromGame("ui/uld/img03/ToggleButton_hr1.tex").RentAsync().Result;
            _itemDetailsUld = Plugin.PluginInterface.UiBuilder.LoadUld("ui/uld/ItemDetail.uld");
            if (_itemDetailsUld is null) return;

            for (int i = 0; i <= 36; i++)
            {
                _itemDetailsTextures.Add(i,
                    _itemDetailsUld.LoadTexturePart("ui/uld/ItemDetail_hr1.tex", i));
            }
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
            return _retainerIconsTextureWrap;
        }

        public IDalamudTextureWrap? LoadRoleIconTexture()
        {
            return _rolesTextureWrap;
        }
        public IDalamudTextureWrap? LoadItemDetailsTexture(int i)
        {
            return _itemDetailsTextures[i];
        }

        public void Dispose()
        {
            foreach (IDalamudTextureWrap icon in _icons.Values)
                icon.Dispose();

            _retainerIconsTextureWrap?.Dispose();
            _rolesTextureWrap?.Dispose();
            foreach (var loadedTexture in _itemDetailsTextures) loadedTexture.Value?.Dispose();
            _itemDetailsUld?.Dispose();
        }
    }
}
