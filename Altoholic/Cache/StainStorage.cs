using Dalamud;
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
                Lumina.Excel.GeneratedSheets.Stain? stainde = Utils.GetStainFromId(i, ClientLanguage.German);
                if (stainde == null) continue;
                string de = stainde.Name;

                Lumina.Excel.GeneratedSheets.Stain? stainen = Utils.GetStainFromId(i, ClientLanguage.English);
                if (stainen == null) continue;
                string en = stainen.Name;

                Lumina.Excel.GeneratedSheets.Stain? stainfr = Utils.GetStainFromId(i, ClientLanguage.French);
                if (stainfr == null) continue;
                string fr = stainfr.Name;

                Lumina.Excel.GeneratedSheets.Stain? stainja = Utils.GetStainFromId(i, ClientLanguage.Japanese);
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

        /*public string this[uint id]
            => LoadStain(id);

        public string this[int id]
            => LoadStain((uint)id);*/

        public string LoadStain(ClientLanguage currentLocale, uint id)
        {
            return currentLocale switch
            {
                ClientLanguage.German => _stains[id].German,
                ClientLanguage.English => _stains[id].English,
                ClientLanguage.French => _stains[id].French,
                ClientLanguage.Japanese => _stains[id].Japanese,
                _ => _stains[id].English,
            };
        }

        public void Dispose()
        {
            _stains.Clear();
        }
    }
}
