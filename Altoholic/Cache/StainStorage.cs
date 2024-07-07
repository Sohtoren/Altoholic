using Dalamud.Game;
using System;
using System.Collections.Generic;

namespace Altoholic.Cache
{
    internal class Stain
    {
        public string German { get; set; } = string.Empty;
        public string English { get; set; } = string.Empty;
        public string French { get; set; } = string.Empty;
        public string Japanese { get; set; } = string.Empty;
    }
    public class StainStorage : IDisposable
    {
        private readonly Dictionary<uint, Stain> _stains;

        public StainStorage(int size = 120)
        {
            _stains = new Dictionary<uint, Stain>(size);
            for (uint i = 0; i <= 120; i++)
            {
                Lumina.Excel.GeneratedSheets.Stain? stainde = Utils.GetStainFromId(i, Dalamud.Game.ClientLanguage.German);
                if (stainde == null) continue;
                string de = stainde.Name;

                Lumina.Excel.GeneratedSheets.Stain? stainen = Utils.GetStainFromId(i, Dalamud.Game.ClientLanguage.English);
                if (stainen == null) continue;
                string en = stainen.Name;

                Lumina.Excel.GeneratedSheets.Stain? stainfr = Utils.GetStainFromId(i, Dalamud.Game.ClientLanguage.French);
                if (stainfr == null) continue;
                string fr = stainfr.Name;

                Lumina.Excel.GeneratedSheets.Stain? stainja = Utils.GetStainFromId(i, Dalamud.Game.ClientLanguage.Japanese);
                if (stainja == null) continue;
                string ja = stainja.Name;

                _stains.Add(i, new Stain
                {
                    German = de,
                    English = en,
                    French = fr,
                    Japanese = ja,
                });
            }
        }

        public string LoadStain(Dalamud.Game.ClientLanguage currentLocale, uint id)
        {
            return currentLocale switch
            {
                Dalamud.Game.ClientLanguage.German => _stains[id].German,
                Dalamud.Game.ClientLanguage.English => _stains[id].English,
                Dalamud.Game.ClientLanguage.French => _stains[id].French,
                Dalamud.Game.ClientLanguage.Japanese => _stains[id].Japanese,
                _ => _stains[id].English,
            };
        }

        public void Dispose()
        {
            _stains.Clear();
        }
    }
}
