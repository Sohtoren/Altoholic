using Altoholic.Cache;
using Altoholic.Models;
using Dalamud.Game;
using Dalamud.Game.Text;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Windowing;
using Dalamud.Bindings.ImGui;
using Lumina.Excel.Sheets;
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

                using (var listbox =
                       ImRaii.ListBox("###CharactersCurrenciesTable#CharactersListBox", new Vector2(200, -1)))
                {
                    if (listbox)
                    {
                        if (ImGui.Selectable(
                                $"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 970)}###CharactersCurrenciesTable#CharactersListBox#All", _currentCharacter == null))
                        {
                            _currentCharacter = null;
                        }

#if DEBUG
                        for (int i = 0; i < 15; i++)
                        {
                            chars.Add(new Character()
                            {
                                FirstName = $"Dummy {i}",
                                LastName = $"LN {i}",
                                HomeWorld = $"Homeworld {i}",
                                Datacenter = $"EU",
                                FCTag = $"FC {i}",
                                Currencies = new PlayerCurrencies() { Gil = i },
                            });
                        }
#endif

                        foreach (Character currChar in chars.Where(currChar =>
                                     ImGui.Selectable(
                                         $"{currChar.FirstName} {currChar.LastName}{(char)SeIconChar.CrossWorld}{currChar.HomeWorld}",
                                         currChar == _currentCharacter)))
                        {
                            _currentCharacter = currChar;
                        }
                    }
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
                        Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(item.Value.Icon), new Vector2(24, 24));
                        ImGui.SameLine();
                        string n = item.Value.Name.ExtractText();
                        if (n.Contains("Legendary")) n = $"Yo-Kai {n}";
                        if (ImGui.Selectable(n, n == _selectedCurrency))
                        {
                            _selectedCurrency = n;
                            /*Plugin.Log.Debug($"n:{n}");
                            Plugin.Log.Debug($"selected_currency:{_selectedCurrency}");
                            Plugin.Log.Debug(
                                $"Currency selected : {Enum.Parse(typeof(Currencies), name)} {(uint)Enum.Parse(typeof(Currencies), name)}");
                            Plugin.Log.Debug($"name : {Utils.CapitalizeCurrency(name)}");*/
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
            Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(itm.Value.Icon), new Vector2(64, 64));

            long overallAmount = 0;
            using (var charactersCurrenciesAllCurrencyTable =
                   ImRaii.Table("###CharactersCurrencies#All#CurrencyTable", 2,
                       ImGuiTableFlags.Borders | ImGuiTableFlags.ScrollY, new Vector2(-1, 380)))
            {
                if (!charactersCurrenciesAllCurrencyTable) return;
                ImGui.TableSetupColumn("###CharactersCurrencies#All#CurrencyTable#CharacterName",
                    ImGuiTableColumnFlags.WidthFixed, 300);
                ImGui.TableSetupColumn("###CharactersCurrencies#All#CurrencyTable#CharacterCurrency",
                    ImGuiTableColumnFlags.WidthStretch);
#if DEBUG
                for (int i = 0; i < 15; i++)
                {
                    chars.Add(new Character()
                    {
                        FirstName = $"Dummy {i}",
                        LastName = $"LN {i}",
                        HomeWorld = $"Homeworld {i}",
                        Datacenter = $"EU",
                        FCTag = $"FC {i}",
                        Currencies = new PlayerCurrencies() { Gil = 999999999 },
                    });
                }
#endif
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

                    object? amount = p.GetValue(character.Currencies, null);
                    if (amount == null)
                    {
                        continue;
                    }
                    overallAmount += (int)amount;
                    //Plugin.Log.Debug($"v: {p.GetValue(character.Currencies, null):N0}");
                    ImGui.TableSetColumnIndex(1);
                    ImGui.TextUnformatted($"{amount:N0}");
                }
            }

            using var overallAmountTable = ImRaii.Table("###CharactersCurrencies#All#SearchCurrenciesTable#CharacterCurrencies#OverallAmountTable", 2);
            if (!overallAmountTable) return;
            ImGui.TableSetupColumn(
                "###CharactersCurrencies#All#SearchCurrenciesTable#CharacterCurrencies#OverallAmountTable#Text",
                ImGuiTableColumnFlags.WidthFixed, 305);
            ImGui.TableSetupColumn(
                "###CharactersCurrencies#All#SearchCurrenciesTable#CharacterCurrencies#OverallAmountTable#Amount",
                ImGuiTableColumnFlags.WidthStretch);
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            ImGui.TextUnformatted($"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 3501)}");
            ImGui.TableSetColumnIndex(1);
            ImGui.TextUnformatted($"{overallAmount:N0}");
        }

        private void DrawPc(Character selectedCharacter)
        {
            if (selectedCharacter.Currencies is null) return;

            using var tabBar = ImRaii.TabBar($"###CharactersCurrencies#CurrencyTabs#{selectedCharacter.CharacterId}");
            if (!tabBar) return;
            using (var commonTab = ImRaii.TabItem($"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 3662)}###CharactersCurrencies#CurrencyTabs#{selectedCharacter.CharacterId}#1"))
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
                    ImRaii.TabItem($"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 3663)}###CharactersCurrencies#CurrencyTabs#{selectedCharacter.CharacterId}#2");
                if (battleTab)
                {
                    DrawBattle(selectedCharacter);
                }
            }

            if (
                selectedCharacter.HasQuest((int)QuestIds.CURRENCY_UNLOCK_CRAFTING_SCRIPS) ||
                selectedCharacter.HasQuest((int)QuestIds.CURRENCY_UNLOCK_SKYBUILDERS_SCRIPS)
            )
            {
                using var othersTab =
                    ImRaii.TabItem($"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 3664)}###CharactersCurrencies#CurrencyTabs#{selectedCharacter.CharacterId}#3");
                {
                    if (othersTab)
                    {
                        DrawOthers(selectedCharacter);
                    }
                }
            }

            if (selectedCharacter.HasQuest((int)QuestIds.TRIBE_ARR_AMALJ_AA) ||
                selectedCharacter.HasQuest((int)QuestIds.TRIBE_ARR_SYLPHS) ||
                selectedCharacter.HasQuest((int)QuestIds.TRIBE_ARR_KOBOLDS) ||
                selectedCharacter.HasQuest((int)QuestIds.TRIBE_ARR_SAHAGIN) ||
                selectedCharacter.HasQuest((int)QuestIds.TRIBE_ARR_IXAL) ||
                selectedCharacter.HasQuest((int)QuestIds.TRIBE_HW_VANU_VANU) ||
                selectedCharacter.HasQuest((int)QuestIds.TRIBE_HW_VATH) ||
                selectedCharacter.HasQuest((int)QuestIds.TRIBE_HW_MOOGLES) ||
                selectedCharacter.HasQuest((int)QuestIds.TRIBE_SB_KOJIN) ||
                selectedCharacter.HasQuest((int)QuestIds.TRIBE_SB_ANANTA) ||
                selectedCharacter.HasQuest((int)QuestIds.TRIBE_SB_NAMAZU) ||
                selectedCharacter.HasQuest((int)QuestIds.TRIBE_SHB_PIXIES) ||
                selectedCharacter.HasQuest((int)QuestIds.TRIBE_SHB_QITARI) ||
                selectedCharacter.HasQuest((int)QuestIds.TRIBE_SHB_DWARVES) ||
                selectedCharacter.HasQuest((int)QuestIds.TRIBE_EW_ARKASODARA) ||
                selectedCharacter.HasQuest((int)QuestIds.TRIBE_EW_OMICRONS) ||
                selectedCharacter.HasQuest((int)QuestIds.TRIBE_EW_LOPORRITS)
               )
            {

                using var tribalTab =
                    ImRaii.TabItem(
                        $"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 5750)}###CharactersCurrencies#CurrencyTabs#{selectedCharacter.CharacterId}#4");
                {
                    if (tribalTab)
                    {
                        DrawTribal(selectedCharacter);
                    }
                }
            }

            if (!selectedCharacter.HasQuest((int)QuestIds.TRIBE_DT_PELUPELU) &&
                !selectedCharacter.HasQuest((int)QuestIds.TRIBE_DT_MAMOOL_JA) &&
                !selectedCharacter.HasQuest((int)QuestIds.TRIBE_DT_YOK_HUY))
            {
                return;
            }

            using var currentExpansionTribalTab =
                ImRaii.TabItem(
                    $"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 5777)}###CharactersCurrencies#CurrencyTabs#{selectedCharacter.CharacterId}#5");
            {
                if (currentExpansionTribalTab)
                {
                    DrawCurrentExpansionTribal(selectedCharacter);
                }
            }
        }

        private void DrawCommon(Character selectedCharacter)
        {
            if (selectedCharacter.Currencies is null || selectedCharacter.Profile is null) return;

            PlayerCurrencies pc = selectedCharacter.Currencies;

            using var charactersCurrenciesCommonCurrencyTable =
                ImRaii.Table($"###CharactersCurrencies#CommonCurrencyTable#{selectedCharacter.CharacterId}", 1);
            if (!charactersCurrenciesCommonCurrencyTable) return;
            ImGui.TableSetupColumn($"###CharactersCurrencies#CommonCurrencyTable#Currency#{selectedCharacter.CharacterId}",
                ImGuiTableColumnFlags.WidthStretch);
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            ImGui.TextUnformatted(_globalCache.AddonStorage.LoadAddonString(_currentLocale, 830));
            ImGui.Separator();
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            DrawCommonCurrency(pc.Gil, Currencies.GIL, 0);

            if (
                selectedCharacter.HasQuest((int)QuestIds.CURRENCY_UNLOCK_SEAL_TWIN_ADDER) ||
                selectedCharacter.HasQuest((int)QuestIds.CURRENCY_UNLOCK_SEAL_MAELSTROM) ||
                selectedCharacter.HasQuest((int)QuestIds.CURRENCY_UNLOCK_SEAL_IMMORTAL_FLAMES)
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
                .HasQuest(
                    (int)QuestIds.CURRENCY_UNLOCK_VENTURE) /* || //Ventures on the currencies window is unlocked with MSQ, not the venture quest... (checked on boosted char tho)
                    selected_character.HasQuest(66968) ||
                    selected_character.HasQuest(66969) ||
                    selected_character.HasQuest(66970)*/
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

            if (!selectedCharacter.HasQuest((int)QuestIds.CURRENCY_UNLOCK_GOLD_SAUCER)) //add quest unlocking. Check if you can get MGP without completing the GC intro quest
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

        private void DrawCommonCurrency(long currency, Currencies id, uint max)
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
            Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(item.Value.Icon), new Vector2(32, 32));
            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                ImGui.TextUnformatted($"{item.Value.Name}");
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
                _ => $"Reset in {Math.Floor(time.TotalHours)}h {time.Minutes:00}m [{nextTuesdayDate.Day}/{nextTuesdayDate.Month} {nextTuesdayDate.Hour:D2} {nextTuesdayDate.Minute:D2}]",
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
            DateTime nextTuesday8AmUtc = now.AddDays(daysUntilTuesday).Date.AddHours(8);
            double totalSeconds = (nextTuesday8AmUtc - now).TotalSeconds;
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
                    ImRaii.Table($"###CharactersCurrencies#BattleCurrencyTable#AllaganTable#{selectedCharacter.CharacterId}", 2);
                if (!charactersCurrenciesBattleCurrencyTableAllaganTable) return;
                ImGui.TableSetupColumn(
                    $"###CharactersCurrencies#BattleCurrencyTable#AllaganTable#Tomestone#{selectedCharacter.CharacterId}",
                    ImGuiTableColumnFlags.WidthStretch);
                ImGui.TableSetupColumn(
                    $"###CharactersCurrencies#BattleCurrencyTable#AllaganTable#Weekly#{selectedCharacter.CharacterId}",
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
                DrawBattleCurrency(pc.Allagan_Tomestone_Of_Heliometry, Currencies.ALLAGAN_TOMESTONE_OF_HELIOMETRY, 2000,
                    true);
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                DrawBattleCurrency(pc.Allagan_Tomestone_Of_Mathematics, Currencies.ALLAGAN_TOMESTONE_OF_MATHEMATICS, 2000, true);
                ImGui.TableSetColumnIndex(1);
                ImGui.TextUnformatted(_globalCache.AddonStorage.LoadAddonString(_currentLocale, 3502));
                ImGui.TextUnformatted($"{pc.Weekly_Acquired_Tomestone}/{pc.Weekly_Limit_Tomestone}");
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                ImGui.TextUnformatted(_globalCache.AddonStorage.LoadAddonString(_currentLocale, 5756));
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                DrawBattleCurrency(pc.Allagan_Tomestone_Of_Aesthetics, Currencies.ALLAGAN_TOMESTONE_OF_AESTHETICS, 2000,
                    true, true);
                ImGui.TableSetColumnIndex(1);
            }

            if (
                selectedCharacter.HasQuest((int)QuestIds.CURRENCY_UNLOCK_PVP_TWIN_ADDER) ||
                selectedCharacter.HasQuest((int)QuestIds.CURRENCY_UNLOCK_PVP_MAELSTROM) ||
                selectedCharacter.HasQuest((int)QuestIds.CURRENCY_UNLOCK_PVP_IMMORTAL_FLAMES) ||
                pc.Wolf_Mark > 0 ||
                pc.Trophy_Crystal > 0
            )
            {
                ImGui.TextUnformatted(_globalCache.AddonStorage.LoadAddonString(_currentLocale, 834));
                ImGui.Separator();
                using var charactersCurrenciesBattleCurrencyTablePvPTable = ImRaii.Table(
                    $"###CharactersCurrencies#BattleCurrencyTable#PvPTable#{selectedCharacter.CharacterId}", 2);
                if (!charactersCurrenciesBattleCurrencyTablePvPTable) return;
                ImGui.TableSetupColumn(
                    $"###CharactersCurrencies#BattleCurrencyTable#PvPTable#Wolf#{selectedCharacter.CharacterId}",
                    ImGuiTableColumnFlags.WidthStretch);
                ImGui.TableSetupColumn(
                    $"###CharactersCurrencies#BattleCurrencyTable#PvPTable#Trophy#{selectedCharacter.CharacterId}",
                    ImGuiTableColumnFlags.WidthStretch);
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                DrawBattleCurrency(pc.Wolf_Mark, Currencies.WOLF_MARK, 20000);
                ImGui.TableSetColumnIndex(1);
                DrawBattleCurrency(pc.Trophy_Crystal, Currencies.TROPHY_CRYSTAL, 20000);
            }

            if (
                selectedCharacter.HasQuest((int)QuestIds.CURRENCY_UNLOCK_HUNT_ARR_TWIN_ADDER) ||
                selectedCharacter.HasQuest((int)QuestIds.CURRENCY_UNLOCK_HUNT_ARR_MAELSTROM) ||
                selectedCharacter.HasQuest((int)QuestIds.CURRENCY_UNLOCK_HUNT_ARR_IMMORTAL_FLAMES) ||
                selectedCharacter.HasQuest((int)QuestIds.CURRENCY_UNLOCK_BLUE_CARNIVAL) ||
                pc.Allied_Seal > 0 ||
                pc.Centurio_Seal > 0 ||
                pc.Sack_of_Nuts > 0
            )
            {
                ImGui.TextUnformatted(_globalCache.AddonStorage.LoadAddonString(_currentLocale, 838));
                ImGui.Separator();
                using var charactersCurrenciesBattleCurrencyTableHuntTable =
                    ImRaii.Table($"###CharactersCurrencies#BattleCurrencyTable#HuntTable#{selectedCharacter.CharacterId}", 3);
                if (!charactersCurrenciesBattleCurrencyTableHuntTable) return;
                ImGui.TableSetupColumn(
                    $"###CharactersCurrencies#BattleCurrencyTable#HuntTable#FirstCol#{selectedCharacter.CharacterId}",
                    ImGuiTableColumnFlags.WidthStretch);
                ImGui.TableSetupColumn(
                    $"###CharactersCurrencies#BattleCurrencyTable#HuntTable#SecondCol#{selectedCharacter.CharacterId}",
                    ImGuiTableColumnFlags.WidthStretch);
                ImGui.TableSetupColumn(
                    $"###CharactersCurrencies#BattleCurrencyTable#HuntTable#ThirdCol#{selectedCharacter.CharacterId}",
                    ImGuiTableColumnFlags.WidthStretch);
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                if (
                    selectedCharacter.HasQuest((int)QuestIds.CURRENCY_UNLOCK_HUNT_ARR_TWIN_ADDER) ||
                    selectedCharacter.HasQuest((int)QuestIds.CURRENCY_UNLOCK_HUNT_ARR_MAELSTROM) ||
                    selectedCharacter.HasQuest((int)QuestIds.CURRENCY_UNLOCK_HUNT_ARR_IMMORTAL_FLAMES) ||
                    selectedCharacter.HasQuest((int)QuestIds.CURRENCY_UNLOCK_BLUE_CARNIVAL) ||
                    pc.Allied_Seal > 0
                )
                {
                    DrawBattleCurrency(pc.Allied_Seal, Currencies.ALLIED_SEAL, 4000);
                }

                if (
                    selectedCharacter.HasQuest((int)QuestIds.CURRENCY_UNLOCK_HUNT_HW) ||
                    selectedCharacter.HasQuest((int)QuestIds.CURRENCY_UNLOCK_HUNT_SB) ||
                    pc.Centurio_Seal > 0
                )
                {
                    ImGui.TableSetColumnIndex(1);
                    DrawBattleCurrency(pc.Centurio_Seal, Currencies.CENTURIO_SEAL, 4000);
                }

                if (
                    selectedCharacter.HasQuest((int)QuestIds.CURRENCY_UNLOCK_HUNT_SHB) ||
                    selectedCharacter.HasQuest((int)QuestIds.CURRENCY_UNLOCK_HUNT_EW) ||
                    selectedCharacter.HasQuest((int)QuestIds.CURRENCY_UNLOCK_HUNT_DT) ||
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
                ImRaii.Table($"###CharactersCurrencies#BattleCurrencyTable#FATETable#{selectedCharacter.CharacterId}", 1);
            if (!charactersCurrenciesBattleCurrencyTableFateTable) return;
            ImGui.TableSetupColumn(
                $"###CharactersCurrencies#BattleCurrencyTable#FATETable#Bicolor#{selectedCharacter.CharacterId}",
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
            Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(item.Value.Icon), new Vector2(32, 32), alpha);
            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                ImGui.TextUnformatted(
                    $"{item.Value.Name}{(discontinued ? "\r\n" + _globalCache.AddonStorage.LoadAddonString(_currentLocale, 5757) : string.Empty)}");
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
                ImRaii.Table($"###CharactersCurrencies#OthersCurrencyTable#{selectedCharacter.CharacterId}", 1);
            if (!charactersCurrenciesOthersCurrencyTable) return;
            if (selectedCharacter.HasQuest((int)QuestIds.CURRENCY_UNLOCK_CRAFTING_SCRIPS))
            {
                ImGui.TableSetupColumn($"###CharactersCurrencies#OthersCurrencyTable#Currency#{selectedCharacter.CharacterId}",
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

            if (!selectedCharacter.HasQuest((int)QuestIds.CURRENCY_UNLOCK_SKYBUILDERS_SCRIPS))
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
            Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(item.Value.Icon), new Vector2(32, 32), alpha);
            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                ImGui.TextUnformatted(
                    $"{item.Value.Name}{(discontinued ? "\r\n" + _globalCache.AddonStorage.LoadAddonString(_currentLocale, 5757) : string.Empty)}");
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
                ImRaii.Table($"###CharactersCurrencies#TribalCurrencyTable#{selectedCharacter.CharacterId}", 3);
            if (!charactersCurrenciesTribalCurrencyTable) return;
            ImGui.TableSetupColumn($"###CharactersCurrencies#TribalCurrencyTable#Col1#{selectedCharacter.CharacterId}",
                ImGuiTableColumnFlags.WidthStretch);
            ImGui.TableSetupColumn($"###CharactersCurrencies#TribalCurrencyTable#Col2#{selectedCharacter.CharacterId}",
                ImGuiTableColumnFlags.WidthStretch);
            ImGui.TableSetupColumn($"###CharactersCurrencies#TribalCurrencyTable#Col3#{selectedCharacter.CharacterId}",
                ImGuiTableColumnFlags.WidthStretch);
            if (selectedCharacter.HasQuest((int)QuestIds.TRIBE_ARR_AMALJ_AA) ||
                selectedCharacter.HasQuest((int)QuestIds.TRIBE_ARR_SYLPHS) ||
                selectedCharacter.HasQuest((int)QuestIds.TRIBE_ARR_KOBOLDS) ||
                selectedCharacter.HasQuest((int)QuestIds.TRIBE_ARR_SAHAGIN) ||
                selectedCharacter.HasQuest((int)QuestIds.TRIBE_ARR_IXAL)
               )
            {
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                ImGui.TextUnformatted(_globalCache.AddonStorage.LoadAddonString(_currentLocale, 5752));
                ImGui.TableNextRow();
                if (selectedCharacter.HasQuest((int)QuestIds.TRIBE_ARR_AMALJ_AA))
                {
                    //ImGui.TableSetColumnIndex(0);
                    ImGui.TableNextColumn();
                    DrawTribalCurrency(pc.Steel_Amaljok, Currencies.STEEL_AMALJOK, Tribal.AMALJ_AA);
                }

                if (selectedCharacter.HasQuest((int)QuestIds.TRIBE_ARR_SYLPHS))
                {
                    //ImGui.TableSetColumnIndex(1);
                    ImGui.TableNextColumn();
                    DrawTribalCurrency(pc.Sylphic_Goldleaf, Currencies.SYLPHIC_GOLDLEAF, Tribal.SYLPHS);
                }

                if (selectedCharacter.HasQuest((int)QuestIds.TRIBE_ARR_KOBOLDS))
                {
                    //ImGui.TableSetColumnIndex(2);
                    ImGui.TableNextColumn();
                    DrawTribalCurrency(pc.Titan_Cobaltpiece, Currencies.TITAN_COBALTPIECE, Tribal.KOBOLDS);
                }

                //ImGui.TableNextRow();
                if (selectedCharacter.HasQuest((int)QuestIds.TRIBE_ARR_SAHAGIN))
                {
                    //ImGui.TableSetColumnIndex(0);
                    ImGui.TableNextColumn();
                    DrawTribalCurrency(pc.Rainbowtide_Psashp, Currencies.RAINBOWTIDE_PSASHP, Tribal.SAHAGIN);
                }

                if (selectedCharacter.HasQuest((int)QuestIds.TRIBE_ARR_IXAL))
                {
                    //ImGui.TableSetColumnIndex(1);
                    ImGui.TableNextColumn();
                    DrawTribalCurrency(pc.Ixali_Oaknot, Currencies.IXALI_OAKNOT, Tribal.IXAL);
                }
            }

            if (selectedCharacter.HasQuest((int)QuestIds.TRIBE_HW_VANU_VANU) ||
                selectedCharacter.HasQuest((int)QuestIds.TRIBE_HW_VATH) ||
                selectedCharacter.HasQuest((int)QuestIds.TRIBE_HW_MOOGLES)
               )
            {
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                ImGui.TextUnformatted(_globalCache.AddonStorage.LoadAddonString(_currentLocale, 5753));
                ImGui.TableNextRow();
                if (selectedCharacter.HasQuest((int)QuestIds.TRIBE_HW_VANU_VANU))
                {
                    //ImGui.TableSetColumnIndex(0);
                    ImGui.TableNextColumn();
                    DrawTribalCurrency(pc.Vanu_Whitebone, Currencies.VANU_WHITEBONE, Tribal.VANU_VANU);
                }

                if (selectedCharacter.HasQuest((int)QuestIds.TRIBE_HW_VATH))
                {
                    //ImGui.TableSetColumnIndex(1);
                    ImGui.TableNextColumn();
                    DrawTribalCurrency(pc.Black_Copper_Gil, Currencies.BLACK_COPPER_GIL, Tribal.VATH);
                }

                if (selectedCharacter.HasQuest((int)QuestIds.TRIBE_HW_MOOGLES))
                {
                    //ImGui.TableSetColumnIndex(2);
                    ImGui.TableNextColumn();
                    DrawTribalCurrency(pc.Carved_Kupo_Nut, Currencies.CARVED_KUPO_NUT, Tribal.MOOGLES);
                }
            }

            if (selectedCharacter.HasQuest((int)QuestIds.TRIBE_SB_KOJIN) ||
                selectedCharacter.HasQuest((int)QuestIds.TRIBE_SB_ANANTA) ||
                selectedCharacter.HasQuest((int)QuestIds.TRIBE_SB_NAMAZU)
               )
            {
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                ImGui.TextUnformatted(_globalCache.AddonStorage.LoadAddonString(_currentLocale, 5754));
                ImGui.TableNextRow();
                if (selectedCharacter.HasQuest((int)QuestIds.TRIBE_SB_KOJIN))
                {
                    //ImGui.TableSetColumnIndex(0);
                    ImGui.TableNextColumn();
                    DrawTribalCurrency(pc.Kojin_Sango, Currencies.KOJIN_SANGO, Tribal.KOJIN);
                }

                if (selectedCharacter.HasQuest((int)QuestIds.TRIBE_SB_ANANTA))
                {
                    //ImGui.TableSetColumnIndex(1);
                    ImGui.TableNextColumn();
                    DrawTribalCurrency(pc.Ananta_Dreamstaff, Currencies.ANANTA_DREAMSTAFF, Tribal.ANANTA);
                }

                if (selectedCharacter.HasQuest((int)QuestIds.TRIBE_SB_NAMAZU))
                {
                    //ImGui.TableSetColumnIndex(2);
                    ImGui.TableNextColumn();
                    DrawTribalCurrency(pc.Namazu_Koban, Currencies.NAMAZU_KOBAN, Tribal.NAMAZU);
                }
            }

            if (selectedCharacter.HasQuest((int)QuestIds.TRIBE_SHB_PIXIES) ||
                selectedCharacter.HasQuest((int)QuestIds.TRIBE_SHB_QITARI) ||
                selectedCharacter.HasQuest((int)QuestIds.TRIBE_SHB_DWARVES)
               )
            {
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                ImGui.TextUnformatted(_globalCache.AddonStorage.LoadAddonString(_currentLocale, 8156));
                ImGui.TableNextRow();
                if (selectedCharacter.HasQuest((int)QuestIds.TRIBE_SHB_PIXIES))
                {
                    //ImGui.TableSetColumnIndex(0);
                    ImGui.TableNextColumn();
                    DrawTribalCurrency(pc.Fae_Fancy, Currencies.FAE_FANCY, Tribal.PIXIES);
                }

                if (selectedCharacter.HasQuest((int)QuestIds.TRIBE_SHB_QITARI))
                {
                    //ImGui.TableSetColumnIndex(1);
                    ImGui.TableNextColumn();
                    DrawTribalCurrency(pc.Qitari_Compliment, Currencies.QITARI_COMPLIMENT, Tribal.QITARI);
                }

                if (selectedCharacter.HasQuest((int)QuestIds.TRIBE_SHB_DWARVES))
                {
                    //ImGui.TableSetColumnIndex(2);
                    ImGui.TableNextColumn();
                    DrawTribalCurrency(pc.Hammered_Frogment, Currencies.HAMMERED_FROGMENT, Tribal.DWARVES);
                }
            }

            if (!selectedCharacter.HasQuest((int)QuestIds.TRIBE_EW_ARKASODARA) ||
                !selectedCharacter.HasQuest((int)QuestIds.TRIBE_EW_OMICRONS) ||
                !selectedCharacter.HasQuest((int)QuestIds.TRIBE_EW_LOPORRITS))
            {
                return;
            }

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            ImGui.TextUnformatted(_globalCache.AddonStorage.LoadAddonString(_currentLocale, 8160));
            ImGui.TableNextRow();
            if (selectedCharacter.HasQuest((int)QuestIds.TRIBE_EW_ARKASODARA))
            {
                ImGui.TableNextColumn();
                DrawTribalCurrency(pc.Arkasodara_Pana, Currencies.ARKASODARA_PANA, Tribal.ARKASODARA);
            }

            if (selectedCharacter.HasQuest((int)QuestIds.TRIBE_EW_OMICRONS))
            {
                ImGui.TableNextColumn();
                DrawTribalCurrency(pc.Omicron_Omnitoken, Currencies.OMICRON_OMNITOKEN, Tribal.OMICRONS);
            }

            if (!selectedCharacter.HasQuest((int)QuestIds.TRIBE_EW_LOPORRITS))
            {
                return;
            }
            ImGui.TableNextColumn();
            DrawTribalCurrency(pc.Loporrit_Carat, Currencies.LOPORRIT_CARAT, Tribal.LOPORRITS);
        }
        
        private void DrawCurrentExpansionTribal(Character selectedCharacter)
        {
            if (selectedCharacter.Currencies == null) return;

            PlayerCurrencies pc = selectedCharacter.Currencies;

            ImGui.TextUnformatted(_globalCache.AddonStorage.LoadAddonString(_currentLocale, 5751));
            ImGui.Separator();
            using var charactersCurrenciesTribalCurrencyTable =
                ImRaii.Table($"###CharactersCurrencies#CurrentExpansionTribalCurrencyTable#{selectedCharacter.CharacterId}", 3);
            if (!charactersCurrenciesTribalCurrencyTable) return;
            ImGui.TableSetupColumn($"###CharactersCurrencies#CurrentExpansionTribalCurrencyTable#Col1#{selectedCharacter.CharacterId}",
                ImGuiTableColumnFlags.WidthStretch);
            ImGui.TableSetupColumn($"###CharactersCurrencies#CurrentExpansionTribalCurrencyTable#Col2#{selectedCharacter.CharacterId}",
                ImGuiTableColumnFlags.WidthStretch);
            ImGui.TableSetupColumn($"###CharactersCurrencies#CurrentExpansionTribalCurrencyTable#Col3#{selectedCharacter.CharacterId}",
                ImGuiTableColumnFlags.WidthStretch);
            if (selectedCharacter.HasQuest((int)QuestIds.TRIBE_DT_PELUPELU)
               )
            {
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                ImGui.TextUnformatted(_globalCache.AddonStorage.LoadAddonString(_currentLocale, 8175));
                ImGui.TableNextRow();
                if (selectedCharacter.HasQuest((int)QuestIds.TRIBE_DT_PELUPELU))
                {
                    //ImGui.TableSetColumnIndex(0);
                    ImGui.TableNextColumn();
                    DrawTribalCurrency(pc.Pelu_Pelplume, Currencies.PELU_PELPLUME, Tribal.PELUPELU);
                }
            }
            if (selectedCharacter.HasQuest((int)QuestIds.TRIBE_DT_MAMOOL_JA)
               )
            {
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                ImGui.TextUnformatted(_globalCache.AddonStorage.LoadAddonString(_currentLocale, 8175));
                ImGui.TableNextRow();
                if (selectedCharacter.HasQuest((int)QuestIds.TRIBE_DT_MAMOOL_JA))
                {
                    //ImGui.TableSetColumnIndex(0);
                    ImGui.TableNextColumn();
                    DrawTribalCurrency(pc.Mamool_Ja_Nanook, Currencies.MAMOOL_JA_NANOOK, Tribal.MAMOOL_JA);
                }
            }
            if (selectedCharacter.HasQuest((int)QuestIds.TRIBE_DT_YOK_HUY)
               )
            {
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                ImGui.TextUnformatted(_globalCache.AddonStorage.LoadAddonString(_currentLocale, 8175));
                ImGui.TableNextRow();
                if (selectedCharacter.HasQuest((int)QuestIds.TRIBE_DT_YOK_HUY))
                {
                    //ImGui.TableSetColumnIndex(0);
                    ImGui.TableNextColumn();
                    DrawTribalCurrency(pc.Yok_Huy_Ward, Currencies.YOK_HUY_WARD, Tribal.YOK_HUY);
                }
            }
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
            Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(itm.Value.Icon), new Vector2(32, 32));
            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                ImGui.TextUnformatted(Utils.Capitalize(Utils.GetTribalCurrencyFromId(_currentLocale, (uint)tribalId)));
                ImGui.EndTooltip();
            }
            ImGui.TableSetColumnIndex(1);
            ImGui.TextUnformatted($"{currency}");
        }
    }
}
