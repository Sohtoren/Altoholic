using Altoholic.Models;
using Dalamud.Game;
using Lumina.Excel.GeneratedSheets;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Altoholic.Cache
{
    public class MinionStorage(int size = 120) : IDisposable
    {
        private readonly Dictionary<uint, Minion> _minions = new(size);

        public void Init(Dalamud.Game.ClientLanguage currentLocale)
        {
            List<Minion>? minions = Utils.GetAllMinions(currentLocale);
            if (minions == null || minions.Count == 0)
            {
                return;
            }

            foreach (Minion minion in minions)
            {
                _minions.Add(minion.Id, minion);
            }
        }

        public Minion? GetMinion(Dalamud.Game.ClientLanguage lang, uint id)
        {
            if (_minions.TryGetValue(id, out Minion? ret))
                return ret;

            Companion? companion = Utils.GetMinion(lang, id);
            if (companion is null)
            {
                return null;
            }

            ret = new Minion { Id = companion.RowId, Transient = new Transient() };
            CompanionTransient? ct = Utils.GetCompanionTransient(lang, id);
            if (ct is null) return null;
            switch (lang)
            {
                case Dalamud.Game.ClientLanguage.German:
                    ret.GermanName = companion.Singular;
                    ret.Transient.GermanDescription = ct.Description;
                    ret.Transient.GermanDescriptionEnhanced = ct.DescriptionEnhanced;
                    ret.Transient.GermanTooltip = ct.Tooltip;
                    break;
                case Dalamud.Game.ClientLanguage.English:
                    ret.EnglishName = companion.Singular;
                    ret.Transient.EnglishDescription = ct.Description;
                    ret.Transient.EnglishDescriptionEnhanced = ct.DescriptionEnhanced;
                    ret.Transient.EnglishTooltip = ct.Tooltip;
                    break;
                case Dalamud.Game.ClientLanguage.French:
                    ret.FrenchName = companion.Singular;
                    ret.Transient.FrenchDescription = ct.Description;
                    ret.Transient.FrenchDescriptionEnhanced = ct.DescriptionEnhanced;
                    ret.Transient.FrenchTooltip = ct.Tooltip;
                    break;
                case Dalamud.Game.ClientLanguage.Japanese:
                    ret.JapaneseName = companion.Singular;
                    ret.Transient.JapaneseDescription = ct.Description;
                    ret.Transient.JapaneseDescriptionEnhanced = ct.DescriptionEnhanced;
                    ret.Transient.JapaneseTooltip = ct.Tooltip;
                    break;
            }

            ret.Icon = companion.Icon;

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
