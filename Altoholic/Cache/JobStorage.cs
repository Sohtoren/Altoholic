using Dalamud.Game;
using System;
using System.Collections.Generic;
using ClassJob = Lumina.Excel.GeneratedSheets.ClassJob;

namespace Altoholic.Cache
{
    internal class JobName
    {
        public string GermanName { get; set; } = string.Empty;
        public string GermanAbbreviation { get; set; } = string.Empty;
        public string EnglishName { get; set; } = string.Empty;
        public string EnglishAbbreviation { get; set; } = string.Empty;
        public string FrenchName { get; set; } = string.Empty;
        public string FrenchAbbreviation { get; set; } = string.Empty;
        public string JapaneseName { get; set; } = string.Empty;
        public string JapaneseAbbreviation { get; set; } = string.Empty;
    }
    public class JobStorage : IDisposable
    {
        private readonly Dictionary<uint, JobName> _jobs;
        private readonly Dictionary<int, int> _level = new(101);

        public JobStorage(int size = 120)
        {
            _jobs = new Dictionary<uint, JobName>(size);
            for (uint i = 0; i <= 42; i++)
            {
                ClassJob? jobde = Utils.GetClassJobFromId(i, Dalamud.Game.ClientLanguage.German);
                if (jobde == null) continue;
                string de = jobde.Name;
                string abbde = jobde.Abbreviation;

                ClassJob? joben = Utils.GetClassJobFromId(i, Dalamud.Game.ClientLanguage.English);
                if (joben == null) continue;
                string en = joben.Name;
                string abben = joben.Abbreviation;

                ClassJob? jobfr = Utils.GetClassJobFromId(i, Dalamud.Game.ClientLanguage.French);
                if (jobfr == null) continue;
                string fr = jobfr.Name;
                string abbfr = jobfr.Abbreviation;

                ClassJob? jobja = Utils.GetClassJobFromId(i, Dalamud.Game.ClientLanguage.Japanese);
                if (jobja == null) continue;
                string ja = jobja.Name;
                string abbja = jobja.Abbreviation;

                _jobs.Add(i, new JobName
                {
                    GermanName = de,
                    GermanAbbreviation = abbde,
                    EnglishName = en,
                    EnglishAbbreviation = abben,
                    FrenchName = fr,
                    FrenchAbbreviation = abbfr,
                    JapaneseName = ja,
                    JapaneseAbbreviation = abbja,
                });
            }
            for (int i = 0; i <= 100; i++)
            {
                int exp = Utils.GetJobNextLevelExp(i);
                _level.Add(i, exp);
            }
        }
        
        public string GetName(Dalamud.Game.ClientLanguage lang, uint job, bool abbreviation = false)
        {
            return lang switch
            {
                Dalamud.Game.ClientLanguage.German => (abbreviation) ? _jobs[job].GermanAbbreviation : Utils.Capitalize(_jobs[job].GermanName),
                Dalamud.Game.ClientLanguage.English => (abbreviation) ? _jobs[job].EnglishAbbreviation : Utils.Capitalize(_jobs[job].EnglishName),
                Dalamud.Game.ClientLanguage.French => (abbreviation) ? _jobs[job].FrenchAbbreviation : Utils.Capitalize(_jobs[job].FrenchName),
                Dalamud.Game.ClientLanguage.Japanese => (abbreviation) ? _jobs[job].JapaneseAbbreviation : Utils.Capitalize(_jobs[job].JapaneseName),
                _ => (abbreviation) ? _jobs[job].EnglishAbbreviation : Utils.Capitalize(_jobs[job].EnglishName),
            };
        }
        public int GetNextLevelExp(int level)
        {
            return _level[level];
        }

        public void Dispose()
        {
            _jobs.Clear();
            _level.Clear();
        }
    }
}
