using Altoholic.Models;
using Dalamud.Game;
using Lumina.Excel.GeneratedSheets;
using System;
using System.Collections.Generic;
using System.Linq;
using Mount = Altoholic.Models.Mount;

namespace Altoholic.Cache
{
    public class MountStorage(int size = 120) : IDisposable
    {
        private readonly Dictionary<uint, Mount> _mounts = new(size);

        public void Init(Dalamud.Game.ClientLanguage currentLocale)
        {
            List<Mount>? mounts = Utils.GetAllMounts(currentLocale);
            if (mounts == null || mounts.Count == 0)
            {
                return;
            }

            foreach (Mount mount in mounts)
            {
                _mounts.Add(mount.Id, mount);
            }
        }

        public Mount? GetMount(Dalamud.Game.ClientLanguage lang, uint id)
        {
            if (_mounts.TryGetValue(id, out Mount? ret))
                return ret;

            Lumina.Excel.GeneratedSheets.Mount? mount = Utils.GetMount(lang, id);
            if (mount is null)
            {
                return null;
            }

            ret = new Mount { Id = mount.RowId, Transient = new Transient() };
            MountTransient? mt = Utils.GetMountTransient(lang, id);
            if (mt is null) return null;
            switch (lang)
            {
                case Dalamud.Game.ClientLanguage.German:
                    ret.GermanName = mount.Singular;
                    ret.Transient.GermanDescription = mt.Description;
                    ret.Transient.GermanDescriptionEnhanced = mt.DescriptionEnhanced;
                    ret.Transient.GermanTooltip = mt.Tooltip;
                    break;
                case Dalamud.Game.ClientLanguage.English:
                    ret.EnglishName = mount.Singular;
                    ret.Transient.EnglishDescription = mt.Description;
                    ret.Transient.EnglishDescriptionEnhanced = mt.DescriptionEnhanced;
                    ret.Transient.EnglishTooltip = mt.Tooltip;
                    break;
                case Dalamud.Game.ClientLanguage.French:
                    ret.FrenchName = mount.Singular;
                    ret.Transient.FrenchDescription = mt.Description;
                    ret.Transient.FrenchDescriptionEnhanced = mt.DescriptionEnhanced;
                    ret.Transient.FrenchTooltip = mt.Tooltip;
                    break;
                case Dalamud.Game.ClientLanguage.Japanese:
                    ret.JapaneseName = mount.Singular;
                    ret.Transient.JapaneseDescription = mt.Description;
                    ret.Transient.JapaneseDescriptionEnhanced = mt.DescriptionEnhanced;
                    ret.Transient.JapaneseTooltip = mt.Tooltip;
                    break;
            }

            ret.Icon = mount.Icon;

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
