using Altoholic.Models;
using Dalamud.Game;
using Lumina.Excel.GeneratedSheets;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Altoholic.Cache
{
    public class BeastTribesStorage(int size = 120) : IDisposable
    {
        private readonly Dictionary<uint, BeastTribes> _beastTribes = new(size);
        private readonly Dictionary<uint, BeastReputationRank> _beastTribeRanks = new(size);

        public void Init(ClientLanguage currentLocale, GlobalCache globalCache)
        {
            List<BeastTribes>? beastTribes = Utils.GetAllBeastTribes(currentLocale);
            if (beastTribes == null || beastTribes.Count == 0)
            {
                return;
            }

            foreach (BeastTribes beastTribe in beastTribes)
            {
                globalCache.IconStorage.LoadIcon(beastTribe.Icon);
                _beastTribes.Add(beastTribe.Id, beastTribe);
            }

            List<BeastReputationRank>? beastReputationRanks = Utils.GetBeastReputationRanks(currentLocale);
            if (beastReputationRanks == null || beastReputationRanks.Count == 0)
            {
                return;
            }

            foreach (BeastReputationRank beastReputationRank in beastReputationRanks)
            {
                _beastTribeRanks.Add(beastReputationRank.RowId, beastReputationRank);
            }
        }

        public BeastTribes? GetBeastTribe(ClientLanguage lang, uint id)
        {
            if (_beastTribes.TryGetValue(id, out BeastTribes? ret))
                return ret;

            BeastTribe? bt = Utils.GetBeastTribe(lang, id);
            if (bt is null)
            {
                return null;
            }

            BeastTribes b = new() { Id = bt.RowId, Icon = bt.Icon, MaxRank = bt.MaxRank, DisplayOrder = bt.DisplayOrder };
            switch (lang)
            {
                case ClientLanguage.German:
                    b.GermanName = bt.Name;
                    break;
                case ClientLanguage.English:
                    b.EnglishName = bt.Name;
                    break;
                case ClientLanguage.French:
                    b.FrenchName = bt.Name;
                    break;
                case ClientLanguage.Japanese:
                    b.JapaneseName = bt.Name;
                    break;
            }

            return b;
        }
        public BeastReputationRank? GetRank(ClientLanguage lang, uint id)
        {
            if (_beastTribeRanks.TryGetValue(id, out BeastReputationRank? ret))
                return ret;

            ret = Utils.GetBeastReputationRank(lang, id);
            return ret;
        }

        public List<uint> Get()
        {
            return _beastTribes.Keys.ToList();
        }
        public int Count()
        {
            return _beastTribes.Count;
        }
        public List<uint> GetRanks()
        {
            return _beastTribeRanks.Keys.ToList();
        }
        public void Dispose()
        {
            _beastTribes.Clear();
        }
    }
}
