using Dalamud.Game;
using System;
using System.Collections.Generic;

namespace Altoholic.Cache
{
    internal class AddonString
    {
        public string German { get; set; } = string.Empty;
        public string English { get; set; } = string.Empty;
        public string French { get; set; } = string.Empty;
        public string Japanese { get; set; } = string.Empty;
    }
    public class AddonStorage : IDisposable
    {
        private readonly Dictionary<int, AddonString> _addons = [];

        public string LoadAddonString(ClientLanguage lang, int id)
        {
            if (_addons.TryGetValue(id, out AddonString? ret))
            {
                string str = lang switch
                {
                    ClientLanguage.German => ret.German,
                    ClientLanguage.English => ret.English,
                    ClientLanguage.French => ret.French,
                    ClientLanguage.Japanese => ret.Japanese,
                    _ => ret.English,
                };
                if (!string.IsNullOrEmpty(str))
                {
                    return str;
                }

                string newstr = Utils.GetAddonString(lang, id);
                if (string.IsNullOrEmpty(newstr))
                {
                    return str;
                }

                switch (lang)
                {
                    case ClientLanguage.German:
                        ret.German = newstr;
                        break;
                    case ClientLanguage.English:
                        ret.English = newstr;
                        break;
                    case ClientLanguage.French:
                        ret.French = newstr;
                        break;
                    case ClientLanguage.Japanese:
                        ret.Japanese = newstr;
                        break;
                }

                return newstr;

            }

            string newaddonstr = Utils.GetAddonString(lang, id);
            AddonString a = new();
            if (string.IsNullOrEmpty(newaddonstr))
            {
                return string.Empty;
            }

            switch (lang)
            {
                case ClientLanguage.German:
                    a.German = newaddonstr;
                    break;
                case ClientLanguage.English:
                    a.English = newaddonstr;
                    break;
                case ClientLanguage.French:
                    a.French = newaddonstr;
                    break;
                case ClientLanguage.Japanese:
                    a.Japanese = newaddonstr;
                    break;
            }

            ret = a;
            _addons[id] = ret;
            return newaddonstr;
        }

        public void Dispose()
        {
            _addons.Clear();
        }
    }
}
