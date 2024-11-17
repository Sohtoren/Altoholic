using Altoholic.Models;
using Dalamud.Game;
using Lumina.Excel.Sheets;
using System;
using System.Collections.Generic;
using System.Linq;
using Mount = Altoholic.Models.Mount;

namespace Altoholic.Cache
{
    public class MountStorage(int size = 0) : IDisposable
    {
        private readonly Dictionary<uint, Mount> _mounts = new(size);

        public void Init(ClientLanguage currentLocale, GlobalCache globalCache)
        {
            List<Mount>? mounts = Utils.GetAllMounts(currentLocale);
            if (mounts == null || mounts.Count == 0)
            {
                return;
            }

            foreach (Mount mount in mounts)
            {
                globalCache.IconStorage.LoadIcon(mount.Icon);
                _mounts.Add(mount.Id, mount);
            }
        }

        public Mount? GetMount(ClientLanguage lang, uint id)
        {
            if (_mounts.TryGetValue(id, out Mount? ret))
                return ret;

            Lumina.Excel.Sheets.Mount? mount = Utils.GetMount(lang, id);
            if (mount is null)
            {
                return null;
            }

            ret = new Mount { Id = mount.Value.RowId, Transient = new Transient() };
            MountTransient? mt = Utils.GetMountTransient(lang, id);
            if (mt is null) return null;
            switch (lang)
            {
                case ClientLanguage.German:
                    ret.GermanName = mount.Value.Singular.ExtractText();
                    ret.Transient.GermanDescription = mt.Value.Description.ExtractText();
                    ret.Transient.GermanDescriptionEnhanced = mt.Value.DescriptionEnhanced.ExtractText();
                    ret.Transient.GermanTooltip = mt.Value.Tooltip.ExtractText();
                    break;
                case ClientLanguage.English:
                    ret.EnglishName = mount.Value.Singular.ExtractText();
                    ret.Transient.EnglishDescription = mt.Value.Description.ExtractText();
                    ret.Transient.EnglishDescriptionEnhanced = mt.Value.DescriptionEnhanced.ExtractText();
                    ret.Transient.EnglishTooltip = mt.Value.Tooltip.ExtractText();
                    break;
                case ClientLanguage.French:
                    ret.FrenchName = mount.Value.Singular.ExtractText();
                    ret.Transient.FrenchDescription = mt.Value.Description.ExtractText();
                    ret.Transient.FrenchDescriptionEnhanced = mt.Value.DescriptionEnhanced.ExtractText();
                    ret.Transient.FrenchTooltip = mt.Value.Tooltip.ExtractText();
                    break;
                case ClientLanguage.Japanese:
                    ret.JapaneseName = mount.Value.Singular.ExtractText();
                    ret.Transient.JapaneseDescription = mt.Value.Description.ExtractText();
                    ret.Transient.JapaneseDescriptionEnhanced = mt.Value.DescriptionEnhanced.ExtractText();
                    ret.Transient.JapaneseTooltip = mt.Value.Tooltip.ExtractText();
                    break;
            }

            ret.Icon = mount.Value.Icon;

            return ret;
        }

        public void Add(uint id, Mount m)
        {
            _mounts.Add(id, m);
        }

        public int Count()
        {
            return _mounts.Count;
        }
        public List<uint> Get()
        {
            return _mounts.Keys.ToList();
        }
        public void Dispose()
        {
            _mounts.Clear();
        }
    }
}
