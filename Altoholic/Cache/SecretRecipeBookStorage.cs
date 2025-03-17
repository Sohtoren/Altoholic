using Dalamud.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using SecretRecipeBook = Altoholic.Models.SecretRecipeBook;

namespace Altoholic.Cache
{
    public class SecretRecipeBookStorage(int size = 120) : IDisposable
    {
        private readonly Dictionary<uint, SecretRecipeBook> _secretRecipeBooks = new(size);

        public void Init(ClientLanguage currentLocale, GlobalCache globalCache)
        {
            List<SecretRecipeBook>? secretRecipeBooks = Utils.GetAllSecretRecipeBook(currentLocale);
            if (secretRecipeBooks == null || secretRecipeBooks.Count == 0)
            {
                return;
            }

            foreach (SecretRecipeBook s in secretRecipeBooks)
            {
                globalCache.IconStorage.LoadIcon(s.Icon);
                _secretRecipeBooks.Add(s.Id, s);
            }
        }

        public SecretRecipeBook? GetSecretRecipeBook(ClientLanguage lang, uint id)
        {
            if (_secretRecipeBooks.TryGetValue(id, out SecretRecipeBook? ret))
                return ret;

            Lumina.Excel.Sheets.SecretRecipeBook? srb = Utils.GetSecretRecipeBook(lang, id);
            if (srb is null)
            {
                return null;
            }

            ret = new SecretRecipeBook { Id = srb.Value.RowId };
            switch (lang)
            {
                case ClientLanguage.German:
                    ret.GermanName = srb.Value.Name.ExtractText();
                    break;
                case ClientLanguage.English:
                    ret.EnglishName = srb.Value.Name.ExtractText();
                    break;
                case ClientLanguage.French:
                    ret.FrenchName = srb.Value.Name.ExtractText();
                    break;
                case ClientLanguage.Japanese:
                    ret.JapaneseName = srb.Value.Name.ExtractText();
                    break;
            }

            return ret;
        }

        public void Add(uint id, SecretRecipeBook or)
        {
            _secretRecipeBooks.Add(id, or);
        }

        public int Count()
        {
            return _secretRecipeBooks.Count;
        }
        public List<uint> Get()
        {
            return _secretRecipeBooks.Keys.ToList();
        }
        public void Dispose()
        {
            _secretRecipeBooks.Clear();
        }
    }
}
