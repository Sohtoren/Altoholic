using Altoholic.Models;
using Dalamud.Game;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Altoholic.Cache
{
    public class EmoteStorage(int size = 120) : IDisposable
    {
        private readonly Dictionary<uint, Emote> _emotes = new(size);

        public void Init(ClientLanguage currentLocale, GlobalCache globalCache)
        {
            List<Emote>? emotes = Utils.GetAllEmotes(currentLocale);
            if (emotes == null || emotes.Count == 0)
            {
                return;
            }

            foreach (Emote e in emotes)
            {
                if (e.Id is 88) continue;//Sleep emote is missing icon
                globalCache.IconStorage.LoadIcon(e.Icon);
                _emotes.Add(e.Id, e);
            }
        }

        public Emote? GetEmote(ClientLanguage lang, uint id)
        {
            if (_emotes.TryGetValue(id, out Emote? ret))
                return ret;

            Lumina.Excel.Sheets.Emote? emote = Utils.GetEmote(lang, id);
            if (emote is null)
            {
                return null;
            }

            ret = new Emote{Id = emote.Value.RowId ,TextCommand = new TextCommand()};
            Lumina.Excel.Sheets.TextCommand? tc = emote.Value.TextCommand.ValueNullable;
            if (tc is null) return null;
            switch (lang)
            {
                case ClientLanguage.German:
                    ret.GermanName = emote.Value.Name.ExtractText();
                    ret.TextCommand.GermanCommand = tc.Value.Command.ExtractText();
                    ret.TextCommand.GermanShortCommand = tc.Value.ShortCommand.ExtractText();
                    ret.TextCommand.GermanDescription = tc.Value.Description.ExtractText();
                    ret.TextCommand.GermanAlias = tc.Value.Alias.ExtractText();
                    ret.TextCommand.GermanShortAlias = tc.Value.ShortAlias.ExtractText();
                    break;
                case ClientLanguage.English:
                    ret.EnglishName = emote.Value.Name.ExtractText();
                    ret.TextCommand.EnglishCommand = tc.Value.Command.ExtractText();
                    ret.TextCommand.EnglishShortCommand = tc.Value.ShortCommand.ExtractText();
                    ret.TextCommand.EnglishDescription = tc.Value.Description.ExtractText();
                    ret.TextCommand.EnglishAlias = tc.Value.Alias.ExtractText();
                    ret.TextCommand.EnglishShortAlias = tc.Value.ShortAlias.ExtractText();
                    break;
                case ClientLanguage.French:
                    ret.FrenchName = emote.Value.Name.ExtractText();
                    ret.TextCommand.FrenchCommand = tc.Value.Command.ExtractText();
                    ret.TextCommand.FrenchShortCommand = tc.Value.ShortCommand.ExtractText();
                    ret.TextCommand.FrenchDescription = tc.Value.Description.ExtractText();
                    ret.TextCommand.FrenchAlias = tc.Value.Alias.ExtractText();
                    ret.TextCommand.FrenchShortAlias = tc.Value.ShortAlias.ExtractText();
                    break;
                case ClientLanguage.Japanese:
                    ret.JapaneseName = emote.Value.Name.ExtractText();
                    ret.TextCommand.JapaneseCommand = tc.Value.Command.ExtractText();
                    ret.TextCommand.JapaneseShortCommand = tc.Value.ShortCommand.ExtractText();
                    ret.TextCommand.JapaneseDescription = tc.Value.Description.ExtractText();
                    ret.TextCommand.JapaneseAlias = tc.Value.Alias.ExtractText();
                    ret.TextCommand.JapaneseShortAlias = tc.Value.ShortAlias.ExtractText();
                    break;
            }

            ret.Icon = emote.Value.Icon;

            return ret;
        }

        public void Add(uint id, Emote e)
        {
            _emotes.Add(id, e);
        }

        public int Count()
        {
            return _emotes.Count;
        }
        public List<uint> Get()
        {
            return _emotes.Keys.ToList();
        }
        public Dictionary<uint, Emote> GetAll()
        {
            return _emotes;
        }
        public void Dispose()
        {
            _emotes.Clear();
        }
    }
}
