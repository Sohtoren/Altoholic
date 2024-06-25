using Altoholic.Models;
using Dalamud;
using System;
using System.Collections.Generic;
using System.Linq;
using OrchestrionRoll = Altoholic.Models.OrchestrionRoll;

namespace Altoholic.Cache
{
    public class OrchestrionRollStorage(int size = 120) : IDisposable
    {
        private readonly Dictionary<uint, OrchestrionRoll> _orchestrionRolls = new(size);

        public void Init(ClientLanguage currentLocale)
        {
            List<OrchestrionRoll>? orchestrions = Utils.GetAllOrchestrionRolls(currentLocale);
            if (orchestrions == null || orchestrions.Count == 0)
            {
                return;
            }

            foreach (OrchestrionRoll tt in orchestrions)
            {
                _orchestrionRolls.Add(tt.Id, tt);
            }
        }

        public OrchestrionRoll? GetOrchestrionRoll(ClientLanguage lang, uint id)
        {
            if (_orchestrionRolls.TryGetValue(id, out OrchestrionRoll? ret))
                return ret;

            Lumina.Excel.GeneratedSheets.Orchestrion? or = Utils.GetOrchestrionRoll(lang, id);
            if (or is null)
            {
                return null;
            }

            ret = new OrchestrionRoll { Id = or.RowId };
            switch (lang)
            {
                case ClientLanguage.German:
                    ret.GermanName = or.Name;
                    break;
                case ClientLanguage.English:
                    ret.EnglishName = or.Name;
                    break;
                case ClientLanguage.French:
                    ret.FrenchName = or.Name;
                    break;
                case ClientLanguage.Japanese:
                    ret.JapaneseName = or.Name;
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
