using Altoholic.Models;
using System;
using System.Collections.Generic;

namespace Altoholic
{
    internal class JobName
    {
        public string GermanName { get; set; } = string.Empty;
        public string EnglishName { get; set; } = string.Empty;
        public string FrenchName { get; set; } = string.Empty;
        public string JapaneseName { get; set; } = string.Empty;
    }
    public class JobStorage : IDisposable
    {
        private readonly Dictionary<uint, JobName> _Jobs = new(40);
        private readonly Dictionary<int, int> _Level = new(101);

        public JobStorage()
        {
            for (uint i = 0; i <= 40; i++)
            {
                if (Plugin.PluginInterface.GetPluginConfig() is Configuration config)
                {
                    var jobde = Utils.GetClassJobFromId(i, Dalamud.ClientLanguage.German);
                    if (jobde == null) continue;
                    string de = jobde.Name;

                    var joben = Utils.GetClassJobFromId(i, Dalamud.ClientLanguage.English);
                    if (joben == null) continue;
                    string en = joben.Name;

                    var jobfr = Utils.GetClassJobFromId(i, Dalamud.ClientLanguage.French);
                    if (jobfr == null) continue;
                    string fr = jobfr.Name;

                    var jobja = Utils.GetClassJobFromId(i, Dalamud.ClientLanguage.Japanese);
                    if (jobja == null) continue;
                    string ja = jobja.Name;
                    _Jobs.Add(i, new JobName()
                    {
                        GermanName = de,
                        EnglishName = en,
                        FrenchName = fr,
                        JapaneseName = ja,
                    });
                }
            }
            for (var i = 0; i <= 100; i++)
            {
                var exp = Utils.GetJobNextLevelExp(i);
                _Level.Add(i, exp);
            }
        }

        public string GetName(uint job)
        {
            Dalamud.ClientLanguage loc = Utils.GetLocale();
            return loc switch
            {
                Dalamud.ClientLanguage.German => _Jobs[job].GermanName,
                Dalamud.ClientLanguage.English => _Jobs[job].EnglishName,
                Dalamud.ClientLanguage.French => _Jobs[job].FrenchName,
                Dalamud.ClientLanguage.Japanese => _Jobs[job].JapaneseName,
                _ => _Jobs[job].EnglishName,
            };
        }
        public int GetNextLevelExp(int level)
        {
            return _Level[level];
        }

        public void Dispose()
        {
            _Jobs.Clear();
            _Level.Clear();
        }
    }
}
