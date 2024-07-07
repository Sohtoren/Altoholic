using Altoholic.Models;
using Dalamud.Interface;
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

        //private UldWrapper? _retainerIcons;
        private IDalamudTextureWrap? _retainerIconsTexture;
        //private readonly Dictionary<ClassJob, IDalamudTextureWrap?> _retainerTextures = [];

        public void Init()
        {
            Plugin.Log.Debug("IconStoage Init() called");
            _retainerIconsTexture = Plugin.TextureProvider.GetFromGame("ui/uld/Retainer_hr1.tex").RentAsync().Result;
            /*_retainerIcons = Plugin.PluginInterface.UiBuilder.LoadUld("ui/uld/Retainer.uld");
            if (_retainerIcons == null)
            {
                return;
            }

            _retainerTextures.Add(ClassJob.PLD, _retainerIcons.LoadTexturePart("ui/uld/Retainer_hr1.tex", 1));
            _retainerTextures.Add(ClassJob.MNK, _retainerIcons.LoadTexturePart("ui/uld/Retainer_hr1.tex", 2));
            _retainerTextures.Add(ClassJob.WAR, _retainerIcons.LoadTexturePart("ui/uld/Retainer_hr1.tex", 3));
            _retainerTextures.Add(ClassJob.DRG, _retainerIcons.LoadTexturePart("ui/uld/Retainer_hr1.tex", 4));
            _retainerTextures.Add(ClassJob.BRD, _retainerIcons.LoadTexturePart("ui/uld/Retainer_hr1.tex", 5));
            _retainerTextures.Add(ClassJob.WHM, _retainerIcons.LoadTexturePart("ui/uld/Retainer_hr1.tex", 6));
            _retainerTextures.Add(ClassJob.BLM, _retainerIcons.LoadTexturePart("ui/uld/Retainer_hr1.tex", 7));
            _retainerTextures.Add(ClassJob.SMN, _retainerIcons.LoadTexturePart("ui/uld/Retainer_hr1.tex", 8));
            _retainerTextures.Add(ClassJob.SCH, _retainerIcons.LoadTexturePart("ui/uld/Retainer_hr1.tex", 9));
            _retainerTextures.Add(ClassJob.NIN, _retainerIcons.LoadTexturePart("ui/uld/Retainer_hr1.tex", 10));
            _retainerTextures.Add(ClassJob.MCH, _retainerIcons.LoadTexturePart("ui/uld/Retainer_hr1.tex", 11));
            _retainerTextures.Add(ClassJob.DRK, _retainerIcons.LoadTexturePart("ui/uld/Retainer_hr1.tex", 12));
            _retainerTextures.Add(ClassJob.AST, _retainerIcons.LoadTexturePart("ui/uld/Retainer_hr1.tex", 13));
            _retainerTextures.Add(ClassJob.SAM, _retainerIcons.LoadTexturePart("ui/uld/Retainer_hr1.tex", 14));
            _retainerTextures.Add(ClassJob.RDM, _retainerIcons.LoadTexturePart("ui/uld/Retainer_hr1.tex", 15));
            _retainerTextures.Add(ClassJob.BLU, _retainerIcons.LoadTexturePart("ui/uld/Retainer_hr1.tex", 16));
            _retainerTextures.Add(ClassJob.GNB, _retainerIcons.LoadTexturePart("ui/uld/Retainer_hr1.tex", 17));
            _retainerTextures.Add(ClassJob.DNC, _retainerIcons.LoadTexturePart("ui/uld/Retainer_hr1.tex", 18));
            _retainerTextures.Add(ClassJob.SGE, _retainerIcons.LoadTexturePart("ui/uld/Retainer_hr1.tex", 19));
            _retainerTextures.Add(ClassJob.RPR, _retainerIcons.LoadTexturePart("ui/uld/Retainer_hr1.tex", 20));
            _retainerTextures.Add(ClassJob.VPR, _retainerIcons.LoadTexturePart("ui/uld/Retainer_hr1.tex", 21));
            _retainerTextures.Add(ClassJob.PCT, _retainerIcons.LoadTexturePart("ui/uld/Retainer_hr1.tex", 22));*/
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

        //public IDalamudTextureWrap? LoadRetainerJobIcon(uint id)
        public IDalamudTextureWrap? LoadRetainerJobIconTexture()
        {
            //Plugin.Log.Debug($"LoadRetainerJobIcon: id: {id}, classjob: {(ClassJob) id}");
            //return _retainerTextures[(ClassJob) id];
            return _retainerIconsTexture;
        }

        public void Dispose()
        {
            foreach (IDalamudTextureWrap icon in _icons.Values)
                icon.Dispose();

            //foreach (KeyValuePair<ClassJob, IDalamudTextureWrap?> loadedTexture in _retainerTextures) loadedTexture.Value?.Dispose();
            //_retainerIcons?.Dispose();
            _retainerIconsTexture?.Dispose();
        }
    }
}
