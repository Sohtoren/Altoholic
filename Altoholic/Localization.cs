using System;
using System.IO;
using System.Reflection;
using CheapLoc;

namespace Altoholic
{
    public class Localization
    {
        private static readonly string[] ApplicableLangCodes = ["de", "ja", "fr"];

        private const string FallbackLangCode = "en";
        private const string LocResourceDirectory = "loc";

        private readonly Assembly _assembly = Assembly.GetCallingAssembly();

        public void ExportLocalizable() => Loc.ExportLocalizableForAssembly(_assembly);
        private void SetupWithFallbacks() => Loc.SetupWithFallbacks(_assembly);

        public void SetupWithLangCode(string langCode)
        {
            if (langCode.ToLower() == FallbackLangCode || !ApplicableLangCodes.Contains(langCode.ToLower()))
            {
                SetupWithFallbacks();
                return;
            }

            try
            {
                Loc.Setup(ReadLocData(langCode), _assembly);
            }
            catch (Exception)
            {
                Plugin.Log.Warning($"Could not load loc {langCode}. Setting up fallbacks.");
                SetupWithFallbacks();
            }
        }

        private static string ReadLocData(string langCode)
        {
            return File.ReadAllText(Path.Combine(Plugin.PluginInterface.AssemblyLocation.DirectoryName!, LocResourceDirectory, $"{langCode}.json"));
        }
    }
}
