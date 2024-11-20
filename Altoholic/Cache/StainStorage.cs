using Altoholic.Models;
using Dalamud.Game;
using Lumina.Excel.Sheets;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace Altoholic.Cache
{
    internal class Stain
    {
        public string German { get; set; } = string.Empty;
        public string English { get; set; } = string.Empty;
        public string French { get; set; } = string.Empty;
        public string Japanese { get; set; } = string.Empty;
        public Vector4 Color { get; set; }
    }
    public class StainStorage : IDisposable
    {
        private readonly Dictionary<uint, Stain> _stains;

        public StainStorage(int size = 120)
        {
            _stains = new Dictionary<uint, Stain>(size);
            for (uint i = 0; i <= 120; i++)
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
                    Color = Utils.ConvertColorToVector4(Utils.ConvertColorToAbgr(stainen.Value.Color))
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

        public (string, Vector4) LoadStainWithColor(ClientLanguage currentLocale, uint id)
        {
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
