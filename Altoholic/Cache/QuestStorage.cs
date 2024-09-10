using Altoholic.Database;
using Altoholic.Models;
using Dalamud.Game;
using Lumina.Excel.GeneratedSheets;
using System;
using System.Collections.Generic;
using System.Linq;
using Quest = Altoholic.Models.Quest;

namespace Altoholic.Cache
{
    public class QuestStorage(int size = 120) : IDisposable
    {
        private readonly Dictionary<uint, Quest> _quests = new(size);

        public void Init(GlobalCache globalCache)
        {
            foreach (int id in Enum.GetValues(typeof(QuestIds)))
            {
                Quest? q = Utils.GetQuest((uint)id);
                if (q == null) continue;

                globalCache.IconStorage.LoadIcon(q.Icon);
                _quests.TryAdd(q.Id, q);
            }
        }

        public Quest? LoadQuest(uint id)
        {
            if (_quests.TryGetValue(id, out Quest? ret))
                return ret;

            ret = Utils.GetQuest(id);
            return ret;
        }

        public void Add(uint id, Quest q)
        {
            _quests.Add(id, q);
        }

        public int Count()
        {
            return _quests.Count;
        }
        public List<uint> Get()
        {
            return _quests.Keys.ToList();
        }
        public void Dispose()
        {
            _quests.Clear();
        }

        public string GetQuestName(ClientLanguage currentLocale, int id)
        {
            Quest? q = LoadQuest((uint)id);
            if (q == null) return string.Empty;
            return currentLocale switch
            {
                ClientLanguage.German => q.GermanName,
                ClientLanguage.English => q.EnglishName,
                ClientLanguage.French => q.FrenchName,
                ClientLanguage.Japanese => q.JapaneseName,
                _ => q.EnglishName
            };
        }
    }
}
