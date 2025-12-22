using Dalamud.Game;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace Altoholic.Cache
{
    internal class Stain
    {
        public string German { get; init; } = string.Empty;
        public string English { get; init; } = string.Empty;
        public string French { get; init; } = string.Empty;
        public string Japanese { get; init; } = string.Empty;
        public uint Color { get; init; }
    }
    public class StainStorage : IDisposable
    {
        private readonly Dictionary<uint, Stain> _stains;

        public StainStorage(int size = 125)
        {
            _stains = new Dictionary<uint, Stain>(size);
            for (uint i = 0; i <= size; i++)
            {
                Lumina.Excel.Sheets.Stain? stainde = Utils.GetStainFromId(i, ClientLanguage.German);
                if (stainde == null) continue;
                string de = stainde.Value.Name.ExtractText();

                Lumina.Excel.Sheets.Stain? stainen = Utils.GetStainFromId(i, ClientLanguage.English);
                if (stainen == null) continue;
                string en = stainen.Value.Name.ExtractText();

                Lumina.Excel.Sheets.Stain? stainfr = Utils.GetStainFromId(i, ClientLanguage.French);
                if (stainfr == null) continue;
                string fr = stainfr.Value.Name.ExtractText();

                Lumina.Excel.Sheets.Stain? stainja = Utils.GetStainFromId(i, ClientLanguage.Japanese);
                if (stainja == null) continue;
                string ja = stainja.Value.Name.ExtractText();

                _stains.Add(i, new Stain
                {
                    German = de,
                    English = en,
                    French = fr,
                    Japanese = ja,
                    Color = stainen.Value.Color
                });
            }
        }

        public string LoadStainName(ClientLanguage currentLocale, uint id)
        {
            Stain s = _stains[id];
            return currentLocale switch
            {
                ClientLanguage.German => s.German,
                ClientLanguage.English => s.English,
                ClientLanguage.French => s.French,
                ClientLanguage.Japanese => s.Japanese,
                _ => s.English,
            };
        }

        public (string, uint) LoadStainWithColor(ClientLanguage currentLocale, uint id)
        {
            if (id > _stains.Count) return (string.Empty, 0);
            Stain s = _stains[id];
            string name = currentLocale switch
            {
                ClientLanguage.German => s.German,
                ClientLanguage.English => s.English,
                ClientLanguage.French => s.French,
                ClientLanguage.Japanese => s.Japanese,
                _ => s.English,
            };
            return (name, s.Color);
        }

        public void Dispose()
        {
            _stains.Clear();
        }
    }
}
