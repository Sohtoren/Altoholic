using Altoholic.Models;
using Dalamud.Game;
using Lumina.Excel.GeneratedSheets2;
using System;
using System.Collections.Generic;
using System.Linq;
using Ornament = Altoholic.Models.Ornament;

namespace Altoholic.Cache
{
    public class OrnamentStorage(int size = 120) : IDisposable
    {
        private readonly Dictionary<uint, Ornament> _ornaments = new(size);

        public void Init(ClientLanguage currentLocale, GlobalCache globalCache)
        {
            List<Ornament>? ornaments = Utils.GetAllOrnaments(currentLocale);
            if (ornaments == null || ornaments.Count == 0)
            {
                return;
            }

            foreach (Ornament ornament in ornaments.Where(ornament => ornament.Id is not (22 or 25 or 26 or 32))) //Those ids moved to facewear
            {
                globalCache.IconStorage.LoadIcon(ornament.Icon);
                _ornaments.Add(ornament.Id, ornament);
            }
        }

        public Ornament? GetOrnament(ClientLanguage lang, uint id)
        {
            if (_ornaments.TryGetValue(id, out Ornament? ret))
                return ret;

            Lumina.Excel.GeneratedSheets.Ornament? ornament = Utils.GetOrnament(lang, id);
            if (ornament is null)
            {
                return null;
            }

            ret = new Ornament { Id = ornament.RowId, Transient = new Transient() };
            OrnamentTransient? mt = Utils.GetOrnamentTransient(lang, id);
            if (mt is null) return null;
            switch (lang)
            {
                case ClientLanguage.German:
                    ret.GermanName = ornament.Singular;
                    ret.Transient.GermanDescription = mt.Unknown0;
                    break;
                case ClientLanguage.English:
                    ret.EnglishName = ornament.Singular;
                    ret.Transient.EnglishDescription = mt.Unknown0;
                    break;
                case ClientLanguage.French:
                    ret.FrenchName = ornament.Singular;
                    ret.Transient.FrenchDescription = mt.Unknown0;
                    break;
                case ClientLanguage.Japanese:
                    ret.JapaneseName = ornament.Singular;
                    ret.Transient.JapaneseDescription = mt.Unknown0;
                    break;
            }

            ret.Icon = ornament.Icon;

            return ret;
        }

        public void Add(uint id, Ornament m)
        {
            _ornaments.Add(id, m);
        }

        public int Count()
        {
            return _ornaments.Count;
        }
        public List<uint> Get()
        {
            return _ornaments.Keys.ToList();
        }
        public void Dispose()
        {
            _ornaments.Clear();
        }
    }
}
