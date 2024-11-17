using Dalamud.Game;
using System;
using System.Collections.Generic;
using ClassJob = Lumina.Excel.Sheets.ClassJob;

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
                ClassJob? jobde = Utils.GetClassJobFromId(i, ClientLanguage.German);
                if (jobde == null) continue;
                string de = jobde.Value.Name.ExtractText();
                string abbde = jobde.Value.Abbreviation.ExtractText();

                ClassJob? joben = Utils.GetClassJobFromId(i, ClientLanguage.English);
                if (joben == null) continue;
                string en = joben.Value.Name.ExtractText();
                string abben = joben.Value.Abbreviation.ExtractText();

                ClassJob? jobfr = Utils.GetClassJobFromId(i, ClientLanguage.French);
                if (jobfr == null) continue;
                string fr = jobfr.Value.Name.ExtractText();
                string abbfr = jobfr.Value.Abbreviation.ExtractText();

                ClassJob? jobja = Utils.GetClassJobFromId(i, ClientLanguage.Japanese);
                if (jobja == null) continue;
                string ja = jobja.Value.Name.ExtractText();
                string abbja = jobja.Value.Abbreviation.ExtractText();

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
        
        public string GetName(ClientLanguage lang, uint job, bool abbreviation = false)
        {
            return lang switch
            {
                ClientLanguage.German => (abbreviation) ? _jobs[job].GermanAbbreviation : Utils.Capitalize(_jobs[job].GermanName),
                ClientLanguage.English => (abbreviation) ? _jobs[job].EnglishAbbreviation : Utils.Capitalize(_jobs[job].EnglishName),
                ClientLanguage.French => (abbreviation) ? _jobs[job].FrenchAbbreviation : Utils.Capitalize(_jobs[job].FrenchName),
                ClientLanguage.Japanese => (abbreviation) ? _jobs[job].JapaneseAbbreviation : Utils.Capitalize(_jobs[job].JapaneseName),
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
