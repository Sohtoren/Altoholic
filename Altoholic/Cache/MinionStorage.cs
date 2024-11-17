using Altoholic.Models;
using Dalamud.Game;
using Lumina.Excel.Sheets;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Altoholic.Cache
{
    public class MinionStorage(int size = 120) : IDisposable
    {
        private readonly Dictionary<uint, Minion> _minions = new(size);

        public void Init(ClientLanguage currentLocale, GlobalCache globalCache)
        {
            List<Minion>? minions = Utils.GetAllMinions(currentLocale);
            if (minions == null || minions.Count == 0)
            {
                return;
            }

            foreach (Minion minion in minions)
            {
                globalCache.IconStorage.LoadIcon(minion.Icon);
                _minions.Add(minion.Id, minion);
            }
        }

        public Minion? GetMinion(ClientLanguage lang, uint id)
        {
            if (_minions.TryGetValue(id, out Minion? ret))
                return ret;

            Companion? companion = Utils.GetMinion(lang, id);
            if (companion is null)
            {
                return null;
            }

            ret = new Minion { Id = companion.Value.RowId, Transient = new Transient() };
            CompanionTransient? ct = Utils.GetCompanionTransient(lang, id);
            if (ct is null) return null;
            switch (lang)
            {
                case ClientLanguage.German:
                    ret.GermanName = companion.Value.Singular.ExtractText();
                    ret.Transient.GermanDescription = ct.Value.Description.ExtractText();
                    ret.Transient.GermanDescriptionEnhanced = ct.Value.DescriptionEnhanced.ExtractText();
                    ret.Transient.GermanTooltip = ct.Value.Tooltip.ExtractText();
                    break;
                case ClientLanguage.English:
                    ret.EnglishName = companion.Value.Singular.ExtractText();
                    ret.Transient.EnglishDescription = ct.Value.Description.ExtractText();
                    ret.Transient.EnglishDescriptionEnhanced = ct.Value.DescriptionEnhanced.ExtractText();
                    ret.Transient.EnglishTooltip = ct.Value.Tooltip.ExtractText();
                    break;
                case ClientLanguage.French:
                    ret.FrenchName = companion.Value.Singular.ExtractText();
                    ret.Transient.FrenchDescription = ct.Value.Description.ExtractText();
                    ret.Transient.FrenchDescriptionEnhanced = ct.Value.DescriptionEnhanced.ExtractText();
                    ret.Transient.FrenchTooltip = ct.Value.Tooltip.ExtractText();
                    break;
                case ClientLanguage.Japanese:
                    ret.JapaneseName = companion.Value.Singular.ExtractText();
                    ret.Transient.JapaneseDescription = ct.Value.Description.ExtractText();
                    ret.Transient.JapaneseDescriptionEnhanced = ct.Value.DescriptionEnhanced.ExtractText();
                    ret.Transient.JapaneseTooltip = ct.Value.Tooltip.ExtractText();
                    break;
            }

            ret.Icon = companion.Value.Icon;

            return ret;
        }

        public void Add(uint id, Minion m)
        {
            _minions.Add(id, m);
        }

        public int Count()
        {
            return _minions.Count;
        }
        public List<uint> Get()
        {
            return _minions.Keys.ToList();
        }
        public void Dispose()
        {
            _minions.Clear();
        }
    }
}
