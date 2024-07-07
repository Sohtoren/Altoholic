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

        public string LoadAddonString(Dalamud.Game.ClientLanguage lang, int id)
        {
            if (_addons.TryGetValue(id, out AddonString? ret))
            {
                string str = lang switch
                {
                    Dalamud.Game.ClientLanguage.German => ret.German,
                    Dalamud.Game.ClientLanguage.English => ret.English,
                    Dalamud.Game.ClientLanguage.French => ret.French,
                    Dalamud.Game.ClientLanguage.Japanese => ret.Japanese,
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
                    case Dalamud.Game.ClientLanguage.German:
                        ret.German = newstr;
                        break;
                    case Dalamud.Game.ClientLanguage.English:
                        ret.English = newstr;
                        break;
                    case Dalamud.Game.ClientLanguage.French:
                        ret.French = newstr;
                        break;
                    case Dalamud.Game.ClientLanguage.Japanese:
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
                case Dalamud.Game.ClientLanguage.German:
                    a.German = newaddonstr;
                    break;
                case Dalamud.Game.ClientLanguage.English:
                    a.English = newaddonstr;
                    break;
                case Dalamud.Game.ClientLanguage.French:
                    a.French = newaddonstr;
                    break;
                case Dalamud.Game.ClientLanguage.Japanese:
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
