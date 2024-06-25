using Altoholic.Models;
using Dalamud;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Altoholic.Cache
{
    public class EmoteStorage(int size = 120) : IDisposable
    {
        private readonly Dictionary<uint, Models.Emote> _emotesStorage = new(size);

        public void Init(ClientLanguage currentLocale)
        {
            List<Models.Emote>? emotes = Utils.GetAllEmotes(currentLocale);
            if (emotes == null || emotes.Count == 0)
            {
                return;
            }

            foreach (Models.Emote e in emotes)
            {
                _emotesStorage.Add(e.Id, e);
            }
        }

        public Emote? GetEmote(ClientLanguage lang, uint id)
        {
            if (_emotesStorage.TryGetValue(id, out Emote? ret))
                return ret;

            Lumina.Excel.GeneratedSheets.Emote? emote = Utils.GetEmote(lang, id);
            if (emote is null)
            {
                return null;
            }

            ret = new Emote{TextCommand = new TextCommand()};
            Lumina.Excel.GeneratedSheets.TextCommand? tc = Utils.GetTextCommand(lang, emote.TextCommand.Row);
            if (tc is null) return null;
            switch (lang)
            {
                case ClientLanguage.German:
                    ret.GermanName = emote.Name;
                    ret.TextCommand.GermanCommand = tc.Command;
                    ret.TextCommand.GermanShortCommand = tc.ShortCommand;
                    ret.TextCommand.GermanDescription = tc.Description;
                    ret.TextCommand.GermanAlias = tc.Alias;
                    ret.TextCommand.GermanShortAlias = tc.ShortAlias;
                    break;
                case ClientLanguage.English:
                    ret.EnglishName = emote.Name;
                    ret.TextCommand.EnglishCommand = tc.Command;
                    ret.TextCommand.EnglishShortCommand = tc.ShortCommand;
                    ret.TextCommand.EnglishDescription = tc.Description;
                    ret.TextCommand.EnglishAlias = tc.Alias;
                    ret.TextCommand.EnglishShortAlias = tc.ShortAlias;
                    break;
                case ClientLanguage.French:
                    ret.FrenchName = emote.Name;
                    ret.TextCommand.FrenchCommand = tc.Command;
                    ret.TextCommand.FrenchShortCommand = tc.ShortCommand;
                    ret.TextCommand.FrenchDescription = tc.Description;
                    ret.TextCommand.FrenchAlias = tc.Alias;
                    ret.TextCommand.FrenchShortAlias = tc.ShortAlias;
                    break;
                case ClientLanguage.Japanese:
                    ret.JapaneseName = emote.Name;
                    ret.TextCommand.JapaneseCommand = tc.Command;
                    ret.TextCommand.JapaneseShortCommand = tc.ShortCommand;
                    ret.TextCommand.JapaneseDescription = tc.Description;
                    ret.TextCommand.JapaneseAlias = tc.Alias;
                    ret.TextCommand.JapaneseShortAlias = tc.ShortAlias;
                    break;
            }

            ret.Icon = emote.Icon;

            return ret;
        }

        public void Add(uint id, Emote e)
        {
            _emotesStorage.Add(id, e);
        }

        public int Count()
        {
            return _emotesStorage.Count;
        }
        public List<uint> Get()
        {
            return _emotesStorage.Keys.ToList();
        }
        public void Dispose()
        {
            _emotesStorage.Clear();
        }
    }
}
