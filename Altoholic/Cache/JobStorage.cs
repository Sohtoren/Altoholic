using Altoholic.Models;
using Dalamud.Game;
using System;
using System.Collections.Generic;
using ClassJob = Lumina.Excel.Sheets.ClassJob;

namespace Altoholic.Cache
{
    public class JobStorage : IDisposable
    {
        private readonly Dictionary<uint, Models.JobName> _jobs;
        private readonly Dictionary<int, int> _level = new(101);
        private readonly Dictionary<uint, Models.PhantomJob> _phantomJobs;
        private readonly Dictionary<uint, uint[]> _phantomJobLevelExperiences = new(51);

        public JobStorage(int size = 120)
        {
            _jobs = new Dictionary<uint, Models.JobName>(size);
            for (uint i = 0; i <= 43; i++)
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

                _jobs.Add(i, new Models.JobName
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

            _phantomJobs = Helpers.Jobs.GetPhantomJobs();
            _phantomJobLevelExperiences = Helpers.Jobs.GetPhantomJobsLevelExperience();
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

        public PhantomJob GetPhantomJob(uint id)
        {
            return _phantomJobs[id];
        }

        public uint GetPhantomJobExperience(uint id, uint level)
        {
            return _phantomJobLevelExperiences[id][level];
        }

        public void Dispose()
        {
            _jobs.Clear();
            _level.Clear();
        }
    }
}
