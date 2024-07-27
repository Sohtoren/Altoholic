using Altoholic.Models;
using Dalamud.Game;
using Lumina.Excel.GeneratedSheets2;
using System;
using System.Collections.Generic;
using System.Linq;
using Glasses = Altoholic.Models.Glasses;

namespace Altoholic.Cache
{
    public class GlassesStorage(int size = 120) : IDisposable
    {
        private readonly Dictionary<uint, Glasses> _glassess = new(size);

        public void Init(ClientLanguage currentLocale, GlobalCache globalCache)
        {
            List<Glasses>? glassess = Utils.GetAllGlasses(currentLocale);
            if (glassess == null || glassess.Count == 0)
            {
                return;
            }

            foreach (Glasses glasses in glassess.Where(glasses => glasses.Id is 1 or 13 or 25 or 37 or 49 or 61 or 73 or 85 or 97 or 109))
            {
                globalCache.IconStorage.LoadIcon(glasses.Icon);
                _glassess.Add(glasses.Id, glasses);
            }
        }

        public Glasses? GetGlasses(ClientLanguage lang, uint id)
        {
            if (_glassess.TryGetValue(id, out Glasses? ret))
                return ret;

            Lumina.Excel.GeneratedSheets.Glasses? glasses = Utils.GetGlasses(lang, id);
            if (glasses is null)
            {
                return null;
            }

            ret = new Glasses { Id = glasses.RowId };
            switch (lang)
            {
                case ClientLanguage.German:
                    ret.GermanName = glasses.Singular;
                    ret.GermanDescription = glasses.Description;
                    break;
                case ClientLanguage.English:
                    ret.EnglishName = glasses.Singular;
                    ret.EnglishDescription = glasses.Description;
                    break;
                case ClientLanguage.French:
                    ret.FrenchName = glasses.Singular;
                    ret.FrenchDescription = glasses.Description;
                    break;
                case ClientLanguage.Japanese:
                    ret.JapaneseName = glasses.Singular;
                    ret.JapaneseDescription = glasses.Description;
                    break;
            }

            ret.Icon = (uint)glasses.Icon;

            return ret;
        }

        public void Add(uint id, Glasses m)
        {
            _glassess.Add(id, m);
        }

        public int Count()
        {
            return _glassess.Count;
        }
        public List<uint> Get()
        {
            return _glassess.Keys.ToList();
        }
        public void Dispose()
        {
            _glassess.Clear();
        }
    }
}
