using Altoholic.Cache;
using Altoholic.Models;
using Dalamud.Game;
using Dalamud.Game.Text;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Windowing;
using ImGuiNET;
using Lumina.Excel.GeneratedSheets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;

namespace Altoholic.Windows
{
    public class CurrenciesWindow : Window, IDisposable
    {
        private readonly Plugin _plugin;
        private ClientLanguage _currentLocale;
        private readonly GlobalCache _globalCache;

        public CurrenciesWindow(
            Plugin plugin,
            string name,
            GlobalCache globalCache
        )
            : base(
                name, ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse)
        {
            SizeConstraints = new WindowSizeConstraints
            {
                MinimumSize = new Vector2(1000, 450),
                MaximumSize = new Vector2(float.MaxValue, float.MaxValue)
            };
            _plugin = plugin;
            _globalCache = globalCache;

            _selectedCurrency = _globalCache.AddonStorage.LoadAddonString(plugin.Configuration.Language, 761);
        }

        public Func<Character> GetPlayer { get; init; } = null!;
        public Func<List<Character>> GetOthersCharactersList { get; set; } = null!;
        private Character? _currentCharacter;
        private string _currentCurrency = string.Empty;
        private string _selectedCurrency;

        /*public override void OnClose()
        {
            Plugin.Log.Debug("DetailsWindow, OnClose() called");
            _currentCharacter = null;
            _currentCurrency = string.Empty;
            _selectedCurrency = string.Empty;
        }*/

        public void Dispose()
        {
            Plugin.Log.Info("CurrenciesWindow, Dispose() called");
            _currentCharacter = null;
            _currentCurrency = string.Empty;
            _selectedCurrency = string.Empty;
        }

        public void Clear()
        {
            Plugin.Log.Info("CurrenciesWindow, Clear() called");
            _currentCharacter = null;
            _currentCurrency = string.Empty;
            _selectedCurrency = string.Empty;
        }

        public override void Draw()
        {
            _currentLocale = _plugin.Configuration.Language;
            if(_selectedCurrency == "Currency" && _currentLocale != ClientLanguage.English) 
                _selectedCurrency = _globalCache.AddonStorage.LoadAddonString(_currentLocale, 761);
            List<Character> chars = [];
            chars.Insert(0, GetPlayer.Invoke());
            chars.AddRange(GetOthersCharactersList.Invoke());

            try
            {
                using var charactersCurrenciesTable = ImRaii.Table("###CharactersCurrenciesTable", 2);
                if (!charactersCurrenciesTable) return;
                ImGui.TableSetupColumn("###CharactersCurrenciesTable#CharactersList", ImGuiTableColumnFlags.WidthFixed,
                    210);
                ImGui.TableSetupColumn("###CharactersCurrenciesTable#Currencies", ImGuiTableColumnFlags.WidthStretch);
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                if (ImGui.BeginListBox("###CharactersCurrenciesTable#CharactersListBox", new Vector2(200, -1)))
                {
                    ImGui.SetScrollY(0);
                    if (ImGui.Selectable(
                            $"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 970)}###CharactersCurrenciesTable#CharactersListBox#All"))
                    {
                        _currentCharacter = null;
                    }

                    foreach (Character currChar in chars.Where(currChar =>
                                 ImGui.Selectable(
                                     $"{currChar.FirstName} {currChar.LastName}{(char)SeIconChar.CrossWorld}{currChar.HomeWorld}",
                                     currChar == _currentCharacter)))
                    {
                        _currentCharacter = currChar;
                    }

                    ImGui.EndListBox();
                }

                ImGui.TableSetColumnIndex(1);
                if (_currentCharacter is not null)
                {
                    DrawPc(_currentCharacter);
                }
                else
                {
                    DrawAll(chars);
                }
            }
            catch (Exception e)
            {
                Plugin.Log.Debug("Altoholic : Exception : {0}", e);
            }
        }

        private void DrawAll(List<Character> chars)
        {
            //Plugin.Log.Debug("DrawAll called");
            using (var combo = ImRaii.Combo("###CharactersCurrencies#All#Combo", _selectedCurrency))
            {
                if (combo)
                {
                    //Plugin.Log.Debug("BeginCombo");
                    List<string> names = [.. Enum.GetNames(typeof(Currencies))];
                    names.Sort();
                    foreach (string name in names)
                    {
                        Item? item =
                            _globalCache.ItemStorage.LoadItem(_currentLocale,
                                (uint)Enum.Parse(typeof(Currencies), name));
                        if (item == null) continue;
                        Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(item.Icon), new Vector2(24, 24));
                        ImGui.SameLine();
                        string n = item.Name;
                        if (n.Contains("Legendary")) n = $"Yo-Kai {n}";
                        if (ImGui.Selectable(n, n == _selectedCurrency))
                        {
                            _selectedCurrency = n;
                            Plugin.Log.Debug($"n:{n}");
                            Plugin.Log.Debug($"selected_currency:{_selectedCurrency}");
                            Plugin.Log.Debug(
                                $"Currency selected : {Enum.Parse(typeof(Currencies), name)} {(uint)Enum.Parse(typeof(Currencies), name)}");
                            Plugin.Log.Debug($"name : {Utils.CapitalizeCurrency(name)}");
                            _currentCurrency = Utils.CapitalizeCurrency(name);
                        }
                    }
                }
            }

            //Plugin.Log.Debug($"current_currency: {current_currency}");
            if (string.IsNullOrEmpty(_currentCurrency))
            {
                return;
            }

            Item? itm = _globalCache.ItemStorage.LoadItem(_currentLocale,
                (uint)Enum.Parse(typeof(Currencies), _currentCurrency.ToUpper()));
            if (itm == null) return;
            Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(itm.Icon), new Vector2(64, 64));
            using var charactersCurrenciesAllCurrencyTable =
                ImRaii.Table("###CharactersCurrencies#All#CurrencyTable", 2, ImGuiTableFlags.Borders);
            if (!charactersCurrenciesAllCurrencyTable) return;
            ImGui.TableSetupColumn("###CharactersCurrencies#All#CurrencyTable#CharacterName",
                ImGuiTableColumnFlags.WidthFixed, 300);
            ImGui.TableSetupColumn("###CharactersCurrencies#All#CurrencyTable#CharacterCurrency",
                ImGuiTableColumnFlags.WidthFixed, 50);
            foreach (Character character in chars)
            {
                //Plugin.Log.Debug($"{character.Currencies.Gil}");
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                ImGui.TextUnformatted(
                    $"{character.FirstName} {character.LastName}{(char)SeIconChar.CrossWorld}{character.HomeWorld}");
                if (character.Currencies is null)
                {
                    continue;
                }

                PropertyInfo? p = character.Currencies.GetType().GetProperty(_currentCurrency);
                //Plugin.Log.Debug($"p: {p}");
                if (p == null)
                {
                    continue;
                }

                //Plugin.Log.Debug($"v: {p.GetValue(character.Currencies, null):N0}");
                ImGui.TableSetColumnIndex(1);
                ImGui.TextUnformatted($"{p.GetValue(character.Currencies, null):N0}");
            }
        }

        private void DrawPc(Character selectedCharacter)
        {
            if (selectedCharacter.Currencies is null) return;

            using var tabBar = ImRaii.TabBar($"###CharactersCurrencies#CurrencyTabs#{selectedCharacter.Id}");
            if (!tabBar) return;
            using (var commonTab = ImRaii.TabItem($"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 3662)}"))
            {
                //if (!) return;
                if (commonTab)
                {
                    DrawCommon(selectedCharacter);
                }
            }

            if (
                selectedCharacter.Currencies.Bicolor_Gemstone > 0 ||
                selectedCharacter.HasAnyLevelJob(50)
            )
            {
                using var battleTab =
                    ImRaii.TabItem($"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 3663)}");
                if (battleTab)
                {
                    DrawBattle(selectedCharacter);
                }
            }

            if (
                selectedCharacter.IsQuestCompleted(67631) ||
                selectedCharacter.IsQuestCompleted(69208)
            )
            {
                using var othersTab =
                    ImRaii.TabItem($"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 3664)}");
                {
                    if (othersTab)
                    {
                        DrawOthers(selectedCharacter);
                    }
                }
            }

            if (!selectedCharacter.IsQuestCompleted(66754) &&
                !selectedCharacter.IsQuestCompleted(66789) &&
                !selectedCharacter.IsQuestCompleted(66857) &&
                !selectedCharacter.IsQuestCompleted(66911) &&
                !selectedCharacter.IsQuestCompleted(67023) &&
                !selectedCharacter.IsQuestCompleted(67700) &&
                !selectedCharacter.IsQuestCompleted(67700) &&
                !selectedCharacter.IsQuestCompleted(67791) &&
                !selectedCharacter.IsQuestCompleted(67856) &&
                !selectedCharacter.IsQuestCompleted(68509) &&
                !selectedCharacter.IsQuestCompleted(68572) &&
                !selectedCharacter.IsQuestCompleted(68633) &&
                !selectedCharacter.IsQuestCompleted(69219) &&
                !selectedCharacter.IsQuestCompleted(69330) &&
                !selectedCharacter.IsQuestCompleted(69432) &&
                !selectedCharacter.IsQuestCompleted(70081) &&
                !selectedCharacter.IsQuestCompleted(70137) &&
                !selectedCharacter.IsQuestCompleted(70217))
            {
                return;
            }

            using var tribalTab =
                ImRaii.TabItem($"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 5750)}");
            {
                if (tribalTab)
                {
                    DrawTribal(selectedCharacter);
                }
            }
        }

        private void DrawCommon(Character selectedCharacter)
        {
            if (selectedCharacter.Currencies is null || selectedCharacter.Profile is null) return;

            PlayerCurrencies pc = selectedCharacter.Currencies;

            using var charactersCurrenciesCommonCurrencyTable =
                ImRaii.Table($"###CharactersCurrencies#CommonCurrencyTable#{selectedCharacter.Id}", 1);
            if (!charactersCurrenciesCommonCurrencyTable) return;
            ImGui.TableSetupColumn($"###CharactersCurrencies#CommonCurrencyTable#Currency#{selectedCharacter.Id}",
                ImGuiTableColumnFlags.WidthStretch);
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            ImGui.TextUnformatted(_globalCache.AddonStorage.LoadAddonString(_currentLocale, 830));
            ImGui.Separator();
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            DrawCommonCurrency(pc.Gil, Currencies.GIL, 0);

            if (
                selectedCharacter.IsQuestCompleted(66216) ||
                selectedCharacter.IsQuestCompleted(66217) ||
                selectedCharacter.IsQuestCompleted(66218)
            )
            {
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                ImGui.TextUnformatted(_globalCache.AddonStorage.LoadAddonString(_currentLocale, 831));
                ImGui.Separator();
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                int val = 0;
                Currencies c = 0;
                switch (selectedCharacter.Profile.GrandCompany)
                {
                    case 1:
                        val = pc.Storm_Seal;
                        c = Currencies.STORM_SEAL;
                        break;
                    case 2:
                        val = pc.Serpent_Seal;
                        c = Currencies.SERPENT_SEAL;
                        break;
                    case 3:
                        val = pc.Flame_Seal;
                        c = Currencies.FLAME_SEAL;
                        break;
                }

                DrawCommonCurrency(val, c, Utils.GetGrandCompanyRankMaxSeals(_currentLocale, selectedCharacter.Profile.GrandCompanyRank));
            }

            if (
                selectedCharacter
                .IsQuestCompleted(
                    66045) /* || //Ventures on the currencies window is unlocked with MSQ, not the venture quest... (checked on boosted char tho)
                    selected_character.IsQuestCompleted(66968) ||
                    selected_character.IsQuestCompleted(66969) ||
                    selected_character.IsQuestCompleted(66970)*/
            )
            {
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                ImGui.TextUnformatted(_globalCache.AddonStorage.LoadAddonString(_currentLocale, 5755));
                ImGui.Separator();
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                DrawCommonCurrency(pc.Venture, Currencies.VENTURE, 0);
            }

            if (!selectedCharacter.IsQuestCompleted(65970)) //add quest unlocking. Check if you can get MGP without completing the GC intro quest
            {
                return;
            }

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            ImGui.TextUnformatted(_globalCache.AddonStorage.LoadAddonString(_currentLocale, 3667));
            ImGui.Separator();
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            DrawCommonCurrency(pc.MGP, Currencies.MGP, 0);
        }

        private void DrawCommonCurrency(int currency, Currencies id, uint max)
        {
            using var charactersCurrenciesCommonCurrencyTableCurrencyTable =
                ImRaii.Table("###CharactersCurrencies#CommonCurrencyTable#CurrencyTable", 2);
            if (!charactersCurrenciesCommonCurrencyTableCurrencyTable) return;
            ImGui.TableSetupColumn("###CharactersCurrencies#CommonCurrencyTable#CurrencyTable#Icon",
                ImGuiTableColumnFlags.WidthFixed, 50);
            ImGui.TableSetupColumn("###CharactersCurrencies#CommonCurrencyTable#CurrencyTable#Amount",
                ImGuiTableColumnFlags.WidthStretch);
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            Item? item =
                _globalCache.ItemStorage.LoadItem(_currentLocale, (uint)id);
            if (item == null) return;
            Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(item.Icon), new Vector2(32, 32));
            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                ImGui.TextUnformatted($"{item.Name}");
                ImGui.EndTooltip();
            }

            ImGui.TableSetColumnIndex(1);
            ImGui.TextUnformatted(max != 0 ? $"{currency:N0}/{max:N0}" : $"{currency:N0}");
        }

        private static string GetTurnIn(ClientLanguage currentLocale)
        {
            DateTime nextTuesdayDateUtc = GetNextTuesday();
            DateTime nextTuesdayDate = DateTime.SpecifyKind(nextTuesdayDateUtc, DateTimeKind.Utc).ToLocalTime();
            TimeSpan time = TimeSpan.FromSeconds(GetNextTuesdayRemainingTime());
            string v = currentLocale switch
            {
                ClientLanguage.German =>
                    $"Zurücksetzung: {time.TotalHours:00} Std. {time.Minutes:00} Min. ({nextTuesdayDate.Day}.{nextTuesdayDate.Month}., {nextTuesdayDate.Hour:D2}:{nextTuesdayDate.Minute:D2} Uhr)",
                ClientLanguage.English =>
                    $"Reset in {Math.Floor(time.TotalHours)}h {time.Minutes:00}m [{nextTuesdayDate.Month}/{nextTuesdayDate.Day} {nextTuesdayDate.Hour:D2} {nextTuesdayDate.Minute:D2}]",
                ClientLanguage.French =>
                    $"Remise à zéro   : {time.TotalHours:00} {((nextTuesdayDate.Hour > 1) ? "heures" : "heure")} {time.Minutes:00} {((nextTuesdayDate.Minute > 1) ? "minutes" : "minute")} [{nextTuesdayDate.Day}.{nextTuesdayDate.Month} {nextTuesdayDate.Hour:D2}h{nextTuesdayDate.Minute:D2}]",
                ClientLanguage.Japanese =>
                    $"リセット日時 : {time.TotalHours:00}時間{time.Minutes:00}分後[{nextTuesdayDate.Day}/{nextTuesdayDate.Month} {nextTuesdayDate.Hour}:{nextTuesdayDate.Minute}]",
                _ => $"Reset in {Math.Floor(time.TotalHours)}h {time.Minutes:00}m [{nextTuesdayDate.Month}/{nextTuesdayDate.Day} {nextTuesdayDate.Hour:D2} {nextTuesdayDate.Minute:D2}]",
            };
            return v;
        }
        private static double GetNextTuesdayRemainingTime()
        {
            DateTime now = DateTime.UtcNow;
            DateTime today = DateTime.Today;
            int daysUntilTuesday;
            if (today.DayOfWeek == DayOfWeek.Tuesday)
            {
                daysUntilTuesday = (((int)DayOfWeek.Monday - (int)today.DayOfWeek + 7) % 7) + 1;
            }
            else
            {
                daysUntilTuesday = ((int)DayOfWeek.Tuesday - (int)today.DayOfWeek + 7) % 7;
            }
            DateTime nextThuesday8AmUtc = now.AddDays(daysUntilTuesday).Date.AddHours(8);
            double totalSeconds = (nextThuesday8AmUtc - now).TotalSeconds;
            return totalSeconds;
        }
        private static DateTime GetNextTuesday()
        {
            DateTime today = DateTime.Today;
            // The (... + 7) % 7 ensures we end up with a value in the range [0, 6]
            int daysUntilTuesday;
            if (today.DayOfWeek == DayOfWeek.Tuesday)
            {
                daysUntilTuesday = (((int)DayOfWeek.Monday - (int)today.DayOfWeek + 7) % 7) + 1;
            }
            else
            {
                daysUntilTuesday = ((int)DayOfWeek.Tuesday - (int)today.DayOfWeek + 7) % 7;
            }

            DateTime nextTuesday = today.AddDays(daysUntilTuesday).AddHours(8);
            return nextTuesday;
        }

        private void DrawBattle(Character selectedCharacter)
        {
            if (selectedCharacter.Currencies is null) return;
            PlayerCurrencies pc = selectedCharacter.Currencies;

            //Todo: Dunno when this unlock, maybe ARR done?
            if (selectedCharacter.HasAnyLevelJob(50))
            {
                ImGui.TextUnformatted(_globalCache.AddonStorage.LoadAddonString(_currentLocale, 3500));
                ImGui.Separator();
                using var charactersCurrenciesBattleCurrencyTableAllaganTable =
                    ImRaii.Table($"###CharactersCurrencies#BattleCurrencyTable#AllaganTable#{selectedCharacter.Id}", 2);
                if (!charactersCurrenciesBattleCurrencyTableAllaganTable) return;
                ImGui.TableSetupColumn(
                    $"###CharactersCurrencies#BattleCurrencyTable#AllaganTable#Tomestone#{selectedCharacter.Id}",
                    ImGuiTableColumnFlags.WidthStretch);
                ImGui.TableSetupColumn(
                    $"###CharactersCurrencies#BattleCurrencyTable#AllaganTable#Weekly#{selectedCharacter.Id}",
                    ImGuiTableColumnFlags.WidthStretch);
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(1);
                ImGui.TextUnformatted(
                    $"{GetTurnIn(_currentLocale)}");
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                DrawBattleCurrency(pc.Allagan_Tomestone_Of_Poetics, Currencies.ALLAGAN_TOMESTONE_OF_POETICS, 2000,
                    true);
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                DrawBattleCurrency(pc.Allagan_Tomestone_Of_Aesthetics, Currencies.ALLAGAN_TOMESTONE_OF_AESTHETICS, 2000,
                    true);
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                DrawBattleCurrency(pc.Allagan_Tomestone_Of_Heliometry, Currencies.ALLAGAN_TOMESTONE_OF_HELIOMETRY, 2000, true);
                ImGui.TableSetColumnIndex(1);
                ImGui.TextUnformatted(_globalCache.AddonStorage.LoadAddonString(_currentLocale, 3502));
                ImGui.TextUnformatted($"{pc.Weekly_Acquired_Tomestone}/{pc.Weekly_Limit_Tomestone}");
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                ImGui.TextUnformatted(_globalCache.AddonStorage.LoadAddonString(_currentLocale, 5756));
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                DrawBattleCurrency(pc.Allagan_Tomestone_Of_Causality, Currencies.ALLAGAN_TOMESTONE_OF_CAUSALITY, 2000,
                    true, true);
                ImGui.TableSetColumnIndex(1);
                DrawBattleCurrency(pc.Allagan_Tomestone_Of_Comedy, Currencies.ALLAGAN_TOMESTONE_OF_COMEDY, 2000,
                    true, true);
            }

            if (
                selectedCharacter.IsQuestCompleted(66640) ||
                selectedCharacter.IsQuestCompleted(66641) ||
                selectedCharacter.IsQuestCompleted(66642) ||
                pc.Wolf_Mark > 0 ||
                pc.Trophy_Crystal > 0
            )
            {
                ImGui.TextUnformatted(_globalCache.AddonStorage.LoadAddonString(_currentLocale, 834));
                ImGui.Separator();
                using var charactersCurrenciesBattleCurrencyTablePvPTable = ImRaii.Table(
                    $"###CharactersCurrencies#BattleCurrencyTable#PvPTable#{selectedCharacter.Id}", 2);
                if (!charactersCurrenciesBattleCurrencyTablePvPTable) return;
                ImGui.TableSetupColumn(
                    $"###CharactersCurrencies#BattleCurrencyTable#PvPTable#Wolf#{selectedCharacter.Id}",
                    ImGuiTableColumnFlags.WidthStretch);
                ImGui.TableSetupColumn(
                    $"###CharactersCurrencies#BattleCurrencyTable#PvPTable#Trophy#{selectedCharacter.Id}",
                    ImGuiTableColumnFlags.WidthStretch);
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                DrawBattleCurrency(pc.Wolf_Mark, Currencies.WOLF_MARK, 20000);
                ImGui.TableSetColumnIndex(1);
                DrawBattleCurrency(pc.Trophy_Crystal, Currencies.TROPHY_CRYSTAL, 20000);
            }

            if (
                selectedCharacter.IsQuestCompleted(67099) ||
                selectedCharacter.IsQuestCompleted(67100) ||
                selectedCharacter.IsQuestCompleted(67101) ||
                selectedCharacter.IsQuestCompleted(68734) ||
                pc.Allied_Seal > 0 ||
                pc.Centurio_Seal > 0 ||
                pc.Sack_of_Nuts > 0
            )
            {
                ImGui.TextUnformatted(_globalCache.AddonStorage.LoadAddonString(_currentLocale, 838));
                ImGui.Separator();
                using var charactersCurrenciesBattleCurrencyTableHuntTable =
                    ImRaii.Table($"###CharactersCurrencies#BattleCurrencyTable#HuntTable#{selectedCharacter.Id}", 3);
                if (!charactersCurrenciesBattleCurrencyTableHuntTable) return;
                ImGui.TableSetupColumn(
                    $"###CharactersCurrencies#BattleCurrencyTable#HuntTable#FirstCol#{selectedCharacter.Id}",
                    ImGuiTableColumnFlags.WidthStretch);
                ImGui.TableSetupColumn(
                    $"###CharactersCurrencies#BattleCurrencyTable#HuntTable#SecondCol#{selectedCharacter.Id}",
                    ImGuiTableColumnFlags.WidthStretch);
                ImGui.TableSetupColumn(
                    $"###CharactersCurrencies#BattleCurrencyTable#HuntTable#ThirdCol#{selectedCharacter.Id}",
                    ImGuiTableColumnFlags.WidthStretch);
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                if (
                    selectedCharacter.IsQuestCompleted(67099) ||
                    selectedCharacter.IsQuestCompleted(67100) ||
                    selectedCharacter.IsQuestCompleted(67101) ||
                    selectedCharacter.IsQuestCompleted(68734) ||
                    pc.Allied_Seal > 0
                )
                {
                    DrawBattleCurrency(pc.Allied_Seal, Currencies.ALLIED_SEAL, 4000);
                }

                if (
                    selectedCharacter.IsQuestCompleted(67658) ||
                    pc.Centurio_Seal > 0
                )
                {
                    ImGui.TableSetColumnIndex(1);
                    DrawBattleCurrency(pc.Centurio_Seal, Currencies.CENTURIO_SEAL, 4000);
                }

                if (
                    selectedCharacter.IsQuestCompleted(69133) ||
                    pc.Sack_of_Nuts > 0
                )
                {
                    ImGui.TableSetColumnIndex(2);
                    DrawBattleCurrency(pc.Sack_of_Nuts, Currencies.SACK_OF_NUTS, 4000);
                }
            }

            if (pc.Bicolor_Gemstone <= 0)
            {
                return;
            }

            //SHB starting quest doesn't work, boosted chars have this done but no gems unlocked in currency
            //Todo: check if can get list of completed FATEs then check for at least one in either SHB or EW
            ImGui.TextUnformatted(_globalCache.AddonStorage.LoadAddonString(_currentLocale, 5768));
            ImGui.Separator();
            using var charactersCurrenciesBattleCurrencyTableFateTable =
                ImRaii.Table($"###CharactersCurrencies#BattleCurrencyTable#FATETable#{selectedCharacter.Id}", 1);
            if (!charactersCurrenciesBattleCurrencyTableFateTable) return;
            ImGui.TableSetupColumn(
                $"###CharactersCurrencies#BattleCurrencyTable#FATETable#Bicolor#{selectedCharacter.Id}",
                ImGuiTableColumnFlags.WidthStretch);
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            DrawBattleCurrency(pc.Bicolor_Gemstone, Currencies.BICOLOR_GEMSTONE, 1000);
        }

        private void DrawBattleCurrency(int currency, Currencies id, uint max, bool total = false,
            bool discontinued = false)
        {
            using var charactersCurrenciesBattleCurrencyTableCurrencyTable =
                ImRaii.Table("###CharactersCurrencies#BattleCurrencyTable#CurrencyTable", 2);
            if (!charactersCurrenciesBattleCurrencyTableCurrencyTable) return;
            ImGui.TableSetupColumn("###CharactersCurrencies#BattleCurrencyTable#CurrencyTable#Icon",
                ImGuiTableColumnFlags.WidthFixed, 50);
            ImGui.TableSetupColumn("###CharactersCurrencies#BattleCurrencyTable#CurrencyTable#Amount",
                ImGuiTableColumnFlags.WidthStretch);
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            Item? item = _globalCache.ItemStorage.LoadItem(_currentLocale, (uint)id);
            if (item == null) return;
            Vector4 alpha = !discontinued switch
            {
                true => new Vector4(1, 1, 1, 1),
                false => new Vector4(1, 1, 1, 0.5f),
            };
            Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(item.Icon), new Vector2(32, 32), alpha);
            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                ImGui.TextUnformatted(
                    $"{item.Name}{(discontinued ? "\r\n" + _globalCache.AddonStorage.LoadAddonString(_currentLocale, 5757) : string.Empty)}");
                ImGui.EndTooltip();
            }

            ImGui.TableSetColumnIndex(1);
            if (max != 0)
            {
                if (total)
                    ImGui.TextUnformatted($"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 3501)}");
                ImGui.TextUnformatted($"{currency:N0}/{max:N0}");
            }
            else
            {
                ImGui.TextUnformatted($"{currency:N0}");
            }
        }

        private void DrawOthers(Character selectedCharacter)
        {
            if (selectedCharacter.Currencies is null) return;
            PlayerCurrencies pc = selectedCharacter.Currencies;

            using var charactersCurrenciesOthersCurrencyTable =
                ImRaii.Table($"###CharactersCurrencies#OthersCurrencyTable#{selectedCharacter.Id}", 1);
            if (!charactersCurrenciesOthersCurrencyTable) return;
            if (selectedCharacter.IsQuestCompleted(67631))
            {
                ImGui.TableSetupColumn($"###CharactersCurrencies#OthersCurrencyTable#Currency#{selectedCharacter.Id}",
                    ImGuiTableColumnFlags.WidthStretch);
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                ImGui.TextUnformatted(_globalCache.AddonStorage.LoadAddonString(_currentLocale, 3665));
                ImGui.Separator();
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                DrawOtherCurrency(pc.Purple_Crafters_Scrip, Currencies.PURPLE_CRAFTERS_SCRIP, 4000, true);
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                DrawOtherCurrency(pc.Orange_Crafters_Scrip, Currencies.ORANGE_CRAFTERS_SCRIP, 4000, true);
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                ImGui.TextUnformatted(_globalCache.AddonStorage.LoadAddonString(_currentLocale, 5756));
                DrawOtherCurrency(pc.White_Crafters_Scrip, Currencies.WHITE_CRAFTERS_SCRIP, 4000, true, true);

                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                ImGui.TextUnformatted(_globalCache.AddonStorage.LoadAddonString(_currentLocale, 3666));
                ImGui.Separator();
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                DrawOtherCurrency(pc.Purple_Gatherers_Scrip, Currencies.PURPLE_CRAFTERS_SCRIP, 4000, true);
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                DrawOtherCurrency(pc.Orange_Gatherers_Scrip, Currencies.ORANGE_GATHERERS_SCRIP, 4000, true);
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                ImGui.TextUnformatted(_globalCache.AddonStorage.LoadAddonString(_currentLocale, 5756));
                DrawOtherCurrency(pc.White_Gatherers_Scrip, Currencies.WHITE_GATHERERS_SCRIP, 4000, true, true);
            }

            if (!selectedCharacter.IsQuestCompleted(69208))
            {
                return;
            }

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            ImGui.TextUnformatted(_globalCache.AddonStorage.LoadAddonString(_currentLocale, 5758));
            ImGui.Separator();
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            DrawOtherCurrency(pc.Skybuilders_Scrip, Currencies.SKYBUILDERS_SCRIP, 4000, true);
        }

        private void DrawOtherCurrency(int currency, Currencies id, uint max, bool total = false,
            bool discontinued = false)
        {
            using var charactersCurrenciesOtherCurrencyTableCurrencyTable =
                ImRaii.Table("###CharactersCurrencies#OtherCurrencyTable#CurrencyTable", 2);
            if (!charactersCurrenciesOtherCurrencyTableCurrencyTable) return;
            ImGui.TableSetupColumn("###CharactersCurrencies#OtherCurrencyTable#CurrencyTable#Icon",
                ImGuiTableColumnFlags.WidthFixed, 50);
            ImGui.TableSetupColumn("###CharactersCurrencies#OtherCurrencyTable#CurrencyTable#Amount",
                ImGuiTableColumnFlags.WidthStretch);
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            Item? item = _globalCache.ItemStorage.LoadItem(_currentLocale, (uint)id);
            if (item == null) return;
            Vector4 alpha = !discontinued switch
            {
                true => new Vector4(1, 1, 1, 1),
                false => new Vector4(1, 1, 1, 0.5f),
            };
            Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(item.Icon), new Vector2(32, 32), alpha);
            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                ImGui.TextUnformatted(
                    $"{item.Name}{(discontinued ? "\r\n" + _globalCache.AddonStorage.LoadAddonString(_currentLocale, 5757) : string.Empty)}");
                ImGui.EndTooltip();
            }

            ImGui.TableSetColumnIndex(1);
            if (max != 0)
            {
                if (total)
                {
                    ImGui.TextUnformatted($"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 3501)}");
                }

                ImGui.TextUnformatted($"{currency:N0}/{max:N0}");
            }
            else
            {
                ImGui.TextUnformatted($"{currency:N0}");
            }
        }

        private void DrawTribal(Character selectedCharacter)
        {
            if (selectedCharacter.Currencies == null) return;

            PlayerCurrencies pc = selectedCharacter.Currencies;

            ImGui.TextUnformatted(_globalCache.AddonStorage.LoadAddonString(_currentLocale, 5751));
            ImGui.Separator();
            using var charactersCurrenciesTribalCurrencyTable =
                ImRaii.Table($"###CharactersCurrencies#TribalCurrencyTable#{selectedCharacter.Id}", 3);
            if (!charactersCurrenciesTribalCurrencyTable) return;
            ImGui.TableSetupColumn($"###CharactersCurrencies#TribalCurrencyTable#Col1#{selectedCharacter.Id}",
                ImGuiTableColumnFlags.WidthStretch);
            ImGui.TableSetupColumn($"###CharactersCurrencies#TribalCurrencyTable#Col2#{selectedCharacter.Id}",
                ImGuiTableColumnFlags.WidthStretch);
            ImGui.TableSetupColumn($"###CharactersCurrencies#TribalCurrencyTable#Col3#{selectedCharacter.Id}",
                ImGuiTableColumnFlags.WidthStretch);
            if (selectedCharacter.IsQuestCompleted(66754) ||
                selectedCharacter.IsQuestCompleted(66789) ||
                selectedCharacter.IsQuestCompleted(66857) ||
                selectedCharacter.IsQuestCompleted(66911) ||
                selectedCharacter.IsQuestCompleted(67023)
               )
            {
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                ImGui.TextUnformatted("A Realm Reborn");
                ImGui.TableNextRow();
                if (selectedCharacter.IsQuestCompleted(66754))
                {
                    //ImGui.TableSetColumnIndex(0);
                    ImGui.TableNextColumn();
                    DrawTribalCurrency(pc.Steel_Amaljok, Currencies.STEEL_AMALJOK, Tribal.AMALJ_AA);
                }

                if (selectedCharacter.IsQuestCompleted(66789))
                {
                    //ImGui.TableSetColumnIndex(1);
                    ImGui.TableNextColumn();
                    DrawTribalCurrency(pc.Sylphic_Goldleaf, Currencies.SYLPHIC_GOLDLEAF, Tribal.SYLPHS);
                }

                if (selectedCharacter.IsQuestCompleted(66857))
                {
                    //ImGui.TableSetColumnIndex(2);
                    ImGui.TableNextColumn();
                    DrawTribalCurrency(pc.Titan_Cobaltpiece, Currencies.TITAN_COBALTPIECE, Tribal.KOBOLDS);
                }

                //ImGui.TableNextRow();
                if (selectedCharacter.IsQuestCompleted(66911))
                {
                    //ImGui.TableSetColumnIndex(0);
                    ImGui.TableNextColumn();
                    DrawTribalCurrency(pc.Rainbowtide_Psashp, Currencies.RAINBOWTIDE_PSASHP, Tribal.SAHAGIN);
                }

                if (selectedCharacter.IsQuestCompleted(67023))
                {
                    //ImGui.TableSetColumnIndex(1);
                    ImGui.TableNextColumn();
                    DrawTribalCurrency(pc.Ixali_Oaknot, Currencies.IXALI_OAKNOT, Tribal.IXAL);
                }
            }

            if (selectedCharacter.IsQuestCompleted(67700) ||
                selectedCharacter.IsQuestCompleted(67791) ||
                selectedCharacter.IsQuestCompleted(67856)
               )
            {
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                ImGui.TextUnformatted("Heavensward");
                ImGui.TableNextRow();
                if (selectedCharacter.IsQuestCompleted(67700))
                {
                    //ImGui.TableSetColumnIndex(0);
                    ImGui.TableNextColumn();
                    DrawTribalCurrency(pc.Vanu_Whitebone, Currencies.VANU_WHITEBONE, Tribal.VANU_VANU);
                }

                if (selectedCharacter.IsQuestCompleted(67791))
                {
                    //ImGui.TableSetColumnIndex(1);
                    ImGui.TableNextColumn();
                    DrawTribalCurrency(pc.Black_Copper_Gil, Currencies.BLACK_COPPER_GIL, Tribal.VATH);
                }

                if (selectedCharacter.IsQuestCompleted(67856))
                {
                    //ImGui.TableSetColumnIndex(2);
                    ImGui.TableNextColumn();
                    DrawTribalCurrency(pc.Carved_Kupo_Nut, Currencies.CARVED_KUPO_NUT, Tribal.MOOGLES);
                }
            }

            if (selectedCharacter.IsQuestCompleted(68509) ||
                selectedCharacter.IsQuestCompleted(68572) ||
                selectedCharacter.IsQuestCompleted(68633)
               )
            {
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                ImGui.TextUnformatted("Stormblood");
                ImGui.TableNextRow();
                if (selectedCharacter.IsQuestCompleted(68509))
                {
                    //ImGui.TableSetColumnIndex(0);
                    ImGui.TableNextColumn();
                    DrawTribalCurrency(pc.Kojin_Sango, Currencies.KOJIN_SANGO, Tribal.KOJIN);
                }

                if (selectedCharacter.IsQuestCompleted(68572))
                {
                    //ImGui.TableSetColumnIndex(1);
                    ImGui.TableNextColumn();
                    DrawTribalCurrency(pc.Ananta_Dreamstaff, Currencies.ANANTA_DREAMSTAFF, Tribal.ANANTA);
                }

                if (selectedCharacter.IsQuestCompleted(68633))
                {
                    //ImGui.TableSetColumnIndex(2);
                    ImGui.TableNextColumn();
                    DrawTribalCurrency(pc.Namazu_Koban, Currencies.NAMAZU_KOBAN, Tribal.NAMAZU);
                }
            }

            if (selectedCharacter.IsQuestCompleted(69219) ||
                selectedCharacter.IsQuestCompleted(69330) ||
                selectedCharacter.IsQuestCompleted(69432)
               )
            {
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                ImGui.TextUnformatted("Shadowbringers");
                ImGui.TableNextRow();
                if (selectedCharacter.IsQuestCompleted(69219))
                {
                    //ImGui.TableSetColumnIndex(0);
                    ImGui.TableNextColumn();
                    DrawTribalCurrency(pc.Fae_Fancy, Currencies.FAE_FANCY, Tribal.PIXIES);
                }

                if (selectedCharacter.IsQuestCompleted(69330))
                {
                    //ImGui.TableSetColumnIndex(1);
                    ImGui.TableNextColumn();
                    DrawTribalCurrency(pc.Qitari_Compliment, Currencies.QITARI_COMPLIMENT, Tribal.QITARI);
                }

                if (selectedCharacter.IsQuestCompleted(69432))
                {
                    //ImGui.TableSetColumnIndex(2);
                    ImGui.TableNextColumn();
                    DrawTribalCurrency(pc.Hammered_Frogment, Currencies.HAMMERED_FROGMENT, Tribal.DWARVES);
                }
            }

            if (!selectedCharacter.IsQuestCompleted(70081) &&
                !selectedCharacter.IsQuestCompleted(70137) &&
                !selectedCharacter.IsQuestCompleted(70217))
            {
                return;
            }

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            ImGui.TextUnformatted("Endwalker");
            ImGui.TableNextRow();
            if (selectedCharacter.IsQuestCompleted(70081))
            {
                //ImGui.TableSetColumnIndex(0);
                ImGui.TableNextColumn();
                DrawTribalCurrency(pc.Arkasodara_Pana, Currencies.ARKASODARA_PANA, Tribal.ARKASODARA);
            }

            if (selectedCharacter.IsQuestCompleted(70137))
            {
                //ImGui.TableSetColumnIndex(1);
                ImGui.TableNextColumn();
                DrawTribalCurrency(pc.Omicron_Omnitoken, Currencies.OMICRON_OMNITOKEN, Tribal.OMICRONS);
            }

            if (!selectedCharacter.IsQuestCompleted(70217))
            {
                return;
            }

            //ImGui.TableSetColumnIndex(2);
            ImGui.TableNextColumn();
            DrawTribalCurrency(pc.Loporrit_Carat, Currencies.LOPORRIT_CARAT, Tribal.LOPORRITS);
        }

        private void DrawTribalCurrency(int currency, Currencies id, Tribal tribalId)
        {
            using var charactersCurrenciesTribalCurrencyTableCurrencyTable =
                ImRaii.Table("###CharactersCurrencies#TribalCurrencyTable#CurrencyTable", 2);
            if (!charactersCurrenciesTribalCurrencyTableCurrencyTable) return;
            ImGui.TableSetupColumn("###CharactersCurrencies#TribalCurrencyTable#CurrencyTable#Icon",
                ImGuiTableColumnFlags.WidthFixed, 50);
            ImGui.TableSetupColumn("###CharactersCurrencies#TribalCurrencyTable#CurrencyTable#Amount",
                ImGuiTableColumnFlags.WidthFixed, 20);
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            Item? itm = _globalCache.ItemStorage.LoadItem(_currentLocale, (uint)id);
            if (itm == null) return;
            Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(itm.Icon), new Vector2(32, 32));
            if (ImGui.IsItemHovered())
                ImGui.SetTooltip(
                    Utils.GetTribalCurrencyFromId(_currentLocale, (uint)tribalId)
                );
            ImGui.TableSetColumnIndex(1);
            ImGui.TextUnformatted($"{currency}");
        }
    }
}
