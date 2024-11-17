
using Dalamud.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using OrchestrionRoll = Altoholic.Models.OrchestrionRoll;

namespace Altoholic.Cache
{
    public class OrchestrionRollStorage(int size = 120) : IDisposable
    {
        private readonly Dictionary<uint, OrchestrionRoll> _orchestrionRolls = new(size);

        public void Init(ClientLanguage currentLocale, GlobalCache globalCache)
        {
            List<OrchestrionRoll>? orchestrions = Utils.GetAllOrchestrionRolls(currentLocale);
            if (orchestrions == null || orchestrions.Count == 0)
            {
                return;
            }

            foreach (OrchestrionRoll o in orchestrions)
            {
                globalCache.IconStorage.LoadIcon(o.Icon);
                _orchestrionRolls.Add(o.Id, o);
            }
        }

        public OrchestrionRoll? GetOrchestrionRoll(ClientLanguage lang, uint id)
        {
            if (_orchestrionRolls.TryGetValue(id, out OrchestrionRoll? ret))
                return ret;

            Lumina.Excel.Sheets.Orchestrion? or = Utils.GetOrchestrionRoll(lang, id);
            if (or is null)
            {
                return null;
            }

            ret = new OrchestrionRoll { Id = or.Value.RowId };
            switch (lang)
            {
                case ClientLanguage.German:
                    ret.GermanName = or.Value.Name.ExtractText();
                    break;
                case ClientLanguage.English:
                    ret.EnglishName = or.Value.Name.ExtractText();
                    break;
                case ClientLanguage.French:
                    ret.FrenchName = or.Value.Name.ExtractText();
                    break;
                case ClientLanguage.Japanese:
                    ret.JapaneseName = or.Value.Name.ExtractText();
                    break;
            }

            return ret;
        }

        public void Add(uint id, OrchestrionRoll or)
        {
            _orchestrionRolls.Add(id, or);
        }

        public int Count()
        {
            return _orchestrionRolls.Count;
        }
        public List<uint> Get()
        {
            return _orchestrionRolls.Keys.ToList();
        }
        public void Dispose()
        {
            _orchestrionRolls.Clear();
        }
    }
}
