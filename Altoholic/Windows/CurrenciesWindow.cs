using Altoholic.Models;
using Dalamud;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin.Services;
using ImGuiNET;
using Lumina.Data.Files.Excel;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace Altoholic.Windows;

public class CurrenciesWindow : Window, IDisposable
{
    private Plugin plugin;

    private readonly IPluginLog pluginLog;
    private readonly IDataManager dataManager;
    private readonly ITextureProvider textureProvider;

    private readonly ClientLanguage currentLocale;

    public CurrenciesWindow(
        Plugin plugin,
        string name,
        IPluginLog pluginLog,
        IDataManager dataManager,
        ITextureProvider textureProvider,
        ClientLanguage currentLocale
        )
        : base(
        name, ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse)
    {
        this.SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(1000, 450),
            MaximumSize = new Vector2(float.MaxValue, float.MaxValue)
        };
        this.plugin = plugin;
        this.pluginLog = pluginLog;
        this.dataManager = dataManager;
        this.textureProvider = textureProvider;
        this.currentLocale = currentLocale;

        this.selected_currency = Utils.GetAddonString(dataManager, pluginLog, currentLocale, 761);
    }

    public Func<Character> GetPlayer { get; init; } = null!;
    public Func<List<Character>> GetOthersCharactersList { get; set; } = null!;
    private Character? current_character = null;
    private string current_currency = string.Empty;
    private string selected_currency = string.Empty;

    public void Dispose()
    {

    }

    public override void Draw()
    {;
        var chars = new List<Character>();
        chars.Insert(0, GetPlayer.Invoke());
        chars.AddRange(GetOthersCharactersList.Invoke());

        try
        {
            if (ImGui.BeginTable("CharactersCurrencies", 2))
            {
                ImGui.TableSetupColumn(string.Empty, ImGuiTableColumnFlags.WidthFixed, 210);
                ImGui.TableSetupColumn(string.Empty, ImGuiTableColumnFlags.WidthStretch);
                ImGui.TableNextRow();
                ImGui.TableNextColumn();
                if (ImGui.BeginListBox(string.Empty, new Vector2(200, -1)))
                {
                    ImGui.SetScrollY(0);
                    if (ImGui.Selectable("All"))
                    {
                        current_character = null;
                    }
                    foreach (Character currChar in chars)
                    {
                        if (ImGui.Selectable($"{currChar.FirstName} {currChar.LastName}{(char)SpecialIcon.E05D}{currChar.HomeWorld}"))
                        {
                            current_character = currChar;
                        }
                    }

                    ImGui.EndListBox();
                }
                ImGui.TableNextColumn();

                if (current_character is not null)
                {
                    /*if (ImGui.BeginTable("Currencies", 3))
                    {
                        ImGui.TableSetupColumn("Currency", ImGuiTableColumnFlags.WidthFixed, 400);
                        ImGui.TableSetupColumn("Amount", ImGuiTableColumnFlags.WidthFixed, 70);
                        ImGui.TableSetupColumn(string.Empty, ImGuiTableColumnFlags.WidthStretch);

                        ImGui.TableHeadersRow();
                        DrawPcList(current_pc);

                        /* Use same tabs as Currency in game
                         * 
                         * 
                         * Split Common and Other in two vertically to add more
                         * */

                    // Ending Currencies table
                    /*ImGui.EndTable();
                }  */
                    DrawPc(current_character);
                }
                else
                {
                    DrawAll(chars);
                }
                ImGui.EndTable();
            }
        }
        catch (Exception e)
        {
            pluginLog.Debug("Altoholic : Exception : {0}", e);
        }
    }

    private void DrawAll(List<Character> chars)
    {

        //pluginLog.Debug("DrawAll called");
        if (ImGui.BeginCombo(string.Empty, selected_currency))
        {
            //pluginLog.Debug("BeginCombo");
            List<string> names = [.. Enum.GetNames(typeof(Currencies))];
            names.Sort();
            foreach (string name in names)
            {
                Utils.DrawItemIcon(textureProvider, dataManager, pluginLog, currentLocale, new Vector2(24, 24), false, (uint)Enum.Parse(typeof(Currencies), name));
                ImGui.SameLine();
                string n = Utils.GetItemNameFromId(dataManager, pluginLog, currentLocale, (uint)Enum.Parse(typeof(Currencies), name));
                if (n.Contains("Legendary")) n = string.Format("{0}{1}", "Yo-Kai ", n);
                if (ImGui.Selectable(n, n == selected_currency))
                {
                    selected_currency = n;
                    pluginLog.Debug($"n:{n}");
                    pluginLog.Debug($"selected_currency:{selected_currency}");
                    pluginLog.Debug($"Currency selected : {Enum.Parse(typeof(Currencies),name)} {(uint)Enum.Parse(typeof(Currencies), name)}");
                    pluginLog.Debug($"name : {Utils.CapitalizeCurrency(name)}");
                    current_currency = Utils.CapitalizeCurrency(name);
                }
            }

            ImGui.EndCombo();
        }

        //pluginLog.Debug($"current_currency: {current_currency}");
        if (!string.IsNullOrEmpty(current_currency))
        {
            Utils.DrawItemIcon(textureProvider, dataManager, pluginLog, currentLocale, new Vector2(64, 64), false, (uint)Enum.Parse(typeof(Currencies), current_currency.ToUpper()));
            if (ImGui.BeginTable("CommonCurrencyTable", 2, ImGuiTableFlags.Borders))
            {
                ImGui.TableSetupColumn(string.Empty, ImGuiTableColumnFlags.WidthFixed, 300);
                ImGui.TableSetupColumn(string.Empty, ImGuiTableColumnFlags.WidthFixed, 50);
                foreach (Character character in chars)
                {
                    //pluginLog.Debug($"{character.Currencies.Gil}");
                    ImGui.TableNextRow();
                    ImGui.TableSetColumnIndex(0);
                    ImGui.TextUnformatted($"{character.FirstName} {character.LastName}{(char)SpecialIcon.E05D}{character.HomeWorld}");
                    if (character.Currencies is not null)
                    {
                        var p = character.Currencies.GetType().GetProperty(current_currency);
                        //pluginLog.Debug($"p: {p}");
                        if (p != null)
                        {
                            //pluginLog.Debug($"v: {p.GetValue(character.Currencies, null):N0}");
                            ImGui.TableSetColumnIndex(1);
                            ImGui.TextUnformatted($"{p.GetValue(character.Currencies, null):N0}");
                        }
                    }
                }
                ImGui.EndTable();
            }
        }
    }

    private void DrawPc(Character current_character)
    {
        if (current_character.Currencies is null) return;

        if (ImGui.BeginTabBar($"#tabs"))
        {
            if (ImGui.BeginTabItem($"{Utils.GetAddonString(dataManager, pluginLog, currentLocale, 3662)}"))
            {
                DrawCommon(current_character);
                ImGui.EndTabItem();
            }

            if (
                current_character.Currencies.Bicolor_Gemstone > 0 ||
                current_character.HasAnyLevelJob(50)
            )
            {
                if (ImGui.BeginTabItem($"{Utils.GetAddonString(dataManager, pluginLog, currentLocale, 3663)}"))
                {
                    DrawBattle(current_character);
                    ImGui.EndTabItem();
                }
            }

            if (
                current_character.IsQuestCompleted(67631) ||
                current_character.IsQuestCompleted(69208)
            )
            {
                if (ImGui.BeginTabItem($"{Utils.GetAddonString(dataManager, pluginLog, currentLocale, 3664)}"))
                {
                    DrawOthers(current_character);
                    ImGui.EndTabItem();
                }
            }

            if (current_character.IsQuestCompleted(66754) ||
                current_character.IsQuestCompleted(66789) ||
                current_character.IsQuestCompleted(66857) ||
                current_character.IsQuestCompleted(66911) ||
                current_character.IsQuestCompleted(67023) ||
                current_character.IsQuestCompleted(67700) ||
                current_character.IsQuestCompleted(67700) ||
                current_character.IsQuestCompleted(67791) ||
                current_character.IsQuestCompleted(67856) ||
                current_character.IsQuestCompleted(68509) ||
                current_character.IsQuestCompleted(68572) ||
                current_character.IsQuestCompleted(68633) ||
                current_character.IsQuestCompleted(69219) ||
                current_character.IsQuestCompleted(69330) ||
                current_character.IsQuestCompleted(69432) ||
                current_character.IsQuestCompleted(70081) ||
                current_character.IsQuestCompleted(70137) ||
                current_character.IsQuestCompleted(70217)
            )
            {
                if (ImGui.BeginTabItem($"{Utils.GetAddonString(dataManager, pluginLog, currentLocale, 5750)}"))
                {
                    DrawTribal(current_character);
                    ImGui.EndTabItem();
                }
            }

            ImGui.EndTabBar();
        }
    }

    private void DrawCommon(Character current_character)
    {
        if (current_character.Currencies is null || current_character.Profile is null) return;

        PlayerCurrencies pc = current_character.Currencies;

        if (ImGui.BeginTable("CommonCurrencyTable", 1))
        {
            ImGui.TableSetupColumn(string.Empty, ImGuiTableColumnFlags.WidthStretch);
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            ImGui.TextUnformatted(Utils.GetAddonString(dataManager, pluginLog, currentLocale, 830));
            ImGui.Separator();
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            DrawCommonCurrency(pc.Gil, Currencies.GIL, 0);

            if (
                current_character.IsQuestCompleted(66216) ||
                current_character.IsQuestCompleted(66217) ||
                current_character.IsQuestCompleted(66218)
            )
            {
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                ImGui.TextUnformatted(Utils.GetAddonString(dataManager, pluginLog, currentLocale, 831));
                ImGui.Separator();
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                int val = 0;
                Currencies c = 0;
                if (current_character.Profile.Grand_Company == 1)
                {
                    val = pc.Storm_Seal;
                    c = Currencies.STORM_SEAL;
                }
                else if (current_character.Profile.Grand_Company == 2)
                {
                    val = pc.Serpent_Seal;
                    c = Currencies.SERPENT_SEAL;
                }
                else if (current_character.Profile.Grand_Company == 3)
                {
                    val = pc.Flame_Seal;
                    c = Currencies.FLAME_SEAL;
                }

                /*(int val, Currencies c) = current_character.Profile.Grand_Company switch
                {
                    1 => (pc.Storm_Seal, Currencies.STORM_SEAL),
                    2 => (pc.Serpent_Seal, Currencies.SERPENT_SEAL),
                    3 => (pc.Flame_Seal, Currencies.FLAME_SEAL),
                };*/
                DrawCommonCurrency(val, c, Utils.GetGrandCompanyRankMaxSeals(dataManager, pluginLog, currentLocale, current_character.Profile.Grand_Company_Rank));
            }

            if (
                current_character.IsQuestCompleted(66045)/* || //Ventures on the currencies window is unlocked with MSQ, not the venture quest... (checked on boosted char tho)
                current_character.IsQuestCompleted(66968) ||
                current_character.IsQuestCompleted(66969) ||
                current_character.IsQuestCompleted(66970)*/
            )
            {
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                ImGui.TextUnformatted(Utils.GetAddonString(dataManager, pluginLog, currentLocale, 5755));
                ImGui.Separator();
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                DrawCommonCurrency(pc.Venture, Currencies.VENTURE, 0);
            }

            if (current_character.IsQuestCompleted(65970))//add quest unlocking. Check if you can get MGP without completing the GC intro quest
            {
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                ImGui.TextUnformatted(Utils.GetAddonString(dataManager, pluginLog, currentLocale, 3667));
                ImGui.Separator();
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                DrawCommonCurrency(pc.MGP, Currencies.MGP, 0);
            }
            ImGui.EndTable();
        }
    }

    private void DrawCommonCurrency(int currency, Currencies id, uint max)
    {
        if (ImGui.BeginTable("CommonCurrencyTable", 2))
        {
            ImGui.TableSetupColumn(string.Empty, ImGuiTableColumnFlags.WidthFixed, 50);
            ImGui.TableSetupColumn(string.Empty, ImGuiTableColumnFlags.WidthStretch);
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            Utils.DrawItemIcon(textureProvider, dataManager, pluginLog, currentLocale, new Vector2(32, 32), false, (uint)id);
            if (ImGui.IsItemHovered())
                ImGui.SetTooltip(
                    Utils.GetItemNameFromId(dataManager, pluginLog, currentLocale, (uint)id)
                );
            ImGui.TableSetColumnIndex(1);
            if (max != 0)
            {
                ImGui.TextUnformatted($"{currency:N0}/{max:N0}");
            }
            else
            {
                ImGui.TextUnformatted($"{currency:N0}");
            }

            ImGui.EndTable();
        }
    }
  
    private DateTime GetNextThuesday()
    {
        DateTime today = DateTime.Today;
        // The (... + 7) % 7 ensures we end up with a value in the range [0, 6]
        int daysUntilTuesday = ((int)DayOfWeek.Tuesday - (int)today.DayOfWeek + 7) % 7;
        DateTime nextTuesday = today.AddDays(daysUntilTuesday).AddHours(8);
        return nextTuesday;
    }

    private void DrawBattle(Character current_character)
    {
        if (current_character.Currencies is null) return;
        PlayerCurrencies pc = current_character.Currencies;

        //Todo: Dunno when this unlock, maybe ARR done?
        if (current_character.HasAnyLevelJob(50))
        {
            ImGui.TextUnformatted(Utils.GetAddonString(dataManager, pluginLog, currentLocale, 3500));
            ImGui.Separator();
            if (ImGui.BeginTable("BattleCurrencyAllaganTable", 2))
            {
                ImGui.TableSetupColumn(string.Empty, ImGuiTableColumnFlags.WidthStretch);
                ImGui.TableSetupColumn(string.Empty, ImGuiTableColumnFlags.WidthStretch);
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(1);
                ImGui.TextUnformatted($"{Utils.GetAddonString(dataManager, pluginLog, currentLocale, 3668)} {GetNextThuesday()}");
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                DrawBattleCurrency(pc.Allagan_Tomestone_Of_Poetics, Currencies.ALLAGAN_TOMESTONE_OF_POETICS, 2000, true);
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                DrawBattleCurrency(pc.Allagan_Tomestone_Of_Causality, Currencies.ALLAGAN_TOMESTONE_OF_CAUSALITY, 2000, true);
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                DrawBattleCurrency(pc.Allagan_Tomestone_Of_Comedy, Currencies.ALLAGAN_TOMESTONE_OF_COMEDY, 2000, true);
                ImGui.TableSetColumnIndex(1);
                ImGui.TextUnformatted(Utils.GetAddonString(dataManager, pluginLog, currentLocale, 3502));
                ImGui.TextUnformatted($"?/900");// Todo: Find a way to get weekly amount
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                ImGui.TextUnformatted(Utils.GetAddonString(dataManager, pluginLog, currentLocale, 5756));
                DrawBattleCurrency(pc.Allagan_Tomestone_Of_Astronomy, Currencies.ALLAGAN_TOMESTONE_OF_ASTRONOMY, 2000, true);

                ImGui.EndTable();
            }
        }

        if (
            current_character.IsQuestCompleted(66640) ||
            current_character.IsQuestCompleted(66641) ||
            current_character.IsQuestCompleted(66642) ||
            pc.Wolf_Mark > 0 ||
            pc.Trophy_Crystal > 0
        )
        {
            ImGui.TextUnformatted(Utils.GetAddonString(dataManager, pluginLog, currentLocale, 834));
            ImGui.Separator();
            if (ImGui.BeginTable("BattleCurrencyPvPTable", 2))
            {
                ImGui.TableSetupColumn(string.Empty, ImGuiTableColumnFlags.WidthStretch);
                ImGui.TableSetupColumn(string.Empty, ImGuiTableColumnFlags.WidthStretch);
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                DrawBattleCurrency(pc.Wolf_Mark, Currencies.WOLF_MARK, 20000);
                ImGui.TableSetColumnIndex(1);
                DrawBattleCurrency(pc.Trophy_Crystal, Currencies.TROPHY_CRYSTAL, 20000);

                ImGui.EndTable();
            }
        }

        if (
            current_character.IsQuestCompleted(67099) ||
            current_character.IsQuestCompleted(67100) ||
            current_character.IsQuestCompleted(67101) ||
            current_character.IsQuestCompleted(68734) ||
            pc.Allied_Seal > 0 ||
            pc.Centurio_Seal > 0 ||
            pc.Sack_of_Nuts > 0
        )
        {
            ImGui.TextUnformatted(Utils.GetAddonString(dataManager, pluginLog, currentLocale, 838));
            ImGui.Separator();
            if (ImGui.BeginTable("BattleCurrencyHuntTable", 2))
            {
                ImGui.TableSetupColumn(string.Empty, ImGuiTableColumnFlags.WidthStretch);
                ImGui.TableSetupColumn(string.Empty, ImGuiTableColumnFlags.WidthStretch);
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                if (
                    current_character.IsQuestCompleted(67099) ||
                    current_character.IsQuestCompleted(67100) ||
                    current_character.IsQuestCompleted(67101) ||
                    current_character.IsQuestCompleted(68734) ||
                    pc.Allied_Seal > 0
                )
                {
                    DrawBattleCurrency(pc.Allied_Seal, Currencies.ALLIED_SEAL, 4000);
                }
                if (
                    current_character.IsQuestCompleted(67658) ||
                    pc.Centurio_Seal > 0
                )
                {
                    ImGui.TableSetColumnIndex(1);
                    DrawBattleCurrency(pc.Centurio_Seal, Currencies.CENTURIO_SEAL, 4000);
                }
                if (
                    current_character.IsQuestCompleted(69133) ||
                    pc.Sack_of_Nuts > 0
                )
                {
                    ImGui.TableNextRow();
                    ImGui.TableSetColumnIndex(0);
                    DrawBattleCurrency(pc.Sack_of_Nuts, Currencies.SACK_OF_NUTS, 4000);
                }

                ImGui.EndTable();
            }
        }

        if (
           pc.Bicolor_Gemstone > 0
           //SHB starting quest doesn't work, boosted chars have this done but no gems unlocked in currency
            //Todo: check if can get list of completed FATEs then check for at least one in either SHB or EW
        )
        {
            ImGui.TextUnformatted(Utils.GetAddonString(dataManager, pluginLog, currentLocale, 5768));
            ImGui.Separator();
            if (ImGui.BeginTable("BattleCurrencyFATETable", 1))
            {
                ImGui.TableSetupColumn(string.Empty, ImGuiTableColumnFlags.WidthStretch);
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                DrawBattleCurrency(pc.Bicolor_Gemstone, Currencies.BICOLOR_GEMSTONE, 1000);

                ImGui.EndTable();
            }
        }
    }

    private void DrawBattleCurrency(int currency, Currencies id, uint max, bool total = false, bool discontinued = true)
    {
        if (ImGui.BeginTable("BattleCurrencyTable", 2))
        {
            ImGui.TableSetupColumn(string.Empty, ImGuiTableColumnFlags.WidthFixed, 50);
            ImGui.TableSetupColumn(string.Empty, ImGuiTableColumnFlags.WidthStretch);
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            Utils.DrawItemIcon(textureProvider, dataManager, pluginLog, currentLocale, new Vector2(32, 32), false, (uint)id);
            if (ImGui.IsItemHovered())
                ImGui.SetTooltip(
                    string.Format("{0}{1}", Utils.GetItemNameFromId(dataManager, pluginLog, currentLocale, (uint)id), (discontinued ? "\r\n" + Utils.GetAddonString(dataManager, pluginLog, currentLocale, 5757) : string.Empty))
                );
            ImGui.TableSetColumnIndex(1);
            if (max != 0)
            {
                if (total)
                {
                    ImGui.TextUnformatted($"{Utils.GetAddonString(dataManager, pluginLog, currentLocale, 3501)}");
                }
                ImGui.TextUnformatted($"{currency:N0}/{max:N0}");
            }
            else
            {
                ImGui.TextUnformatted($"{currency:N0}");
            }

            ImGui.EndTable();
        }
    }

    private void DrawOthers(Character current_character)
    {
        if (current_character.Currencies is null) return;
        PlayerCurrencies pc = current_character.Currencies;

        if (ImGui.BeginTable("OthersCurrencyTable", 1))
        {
            if (current_character.IsQuestCompleted(67631))
            {
                ImGui.TableSetupColumn(string.Empty, ImGuiTableColumnFlags.WidthStretch);
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                ImGui.TextUnformatted(Utils.GetAddonString(dataManager, pluginLog, currentLocale, 3665));
                ImGui.Separator();
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                DrawOtherCurrency(pc.White_Crafters_Scrip, Currencies.WHITE_CRAFTERS_SCRIP, 4000, true);
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                DrawOtherCurrency(pc.Purple_Crafters_Scrip, Currencies.PURPLE_CRAFTERS_SCRIP, 4000, true);
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                ImGui.TextUnformatted(Utils.GetAddonString(dataManager, pluginLog, currentLocale, 5756));
                DrawOtherCurrency(pc.Yellow_Crafters_Scrip, Currencies.YELLOW_CRAFTERS_SCRIP, 4000, true, true);

                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                ImGui.TextUnformatted(Utils.GetAddonString(dataManager, pluginLog, currentLocale, 3666));
                ImGui.Separator();
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                DrawOtherCurrency(pc.White_Gatherers_Scrip, Currencies.WHITE_GATHERERS_SCRIP, 4000, true);
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                DrawOtherCurrency(pc.Purple_Gatherers_Scrip, Currencies.PURPLE_GATHERERS_SCRIP, 4000, true);
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                ImGui.TextUnformatted(Utils.GetAddonString(dataManager, pluginLog, currentLocale, 5756));
                DrawOtherCurrency(pc.Yellow_Gatherers_Scrip, Currencies.YELLOW_GATHERERS_SCRIP, 4000, true, true);
            }

            if (current_character.IsQuestCompleted(69208))
            {
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                ImGui.TextUnformatted(Utils.GetAddonString(dataManager, pluginLog, currentLocale, 5758));
                ImGui.Separator();
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                DrawOtherCurrency(pc.Skybuilders_Scrip, Currencies.SKYBUILDERS_SCRIP, 4000, true);
            }
            ImGui.EndTable();
        }
    }

    private void DrawOtherCurrency(int currency, Currencies id, uint max, bool total = false, bool discontinued = false)
    {
        if (ImGui.BeginTable("BattleCurrencyTable", 2))
        {
            ImGui.TableSetupColumn(string.Empty, ImGuiTableColumnFlags.WidthFixed, 50);
            ImGui.TableSetupColumn(string.Empty, ImGuiTableColumnFlags.WidthStretch);
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            Utils.DrawItemIcon(textureProvider, dataManager, pluginLog, currentLocale, new Vector2(32, 32), false, (uint)id);
            if (ImGui.IsItemHovered())
                ImGui.SetTooltip(
                    string.Format("{0}{1}", Utils.GetItemNameFromId(dataManager, pluginLog, currentLocale, (uint)id), (discontinued ? "\r\n" + Utils.GetAddonString(dataManager, pluginLog, currentLocale, 5757) : string.Empty))                    
                );
            ImGui.TableSetColumnIndex(1);
            if (max != 0)
            {
                if (total)
                {
                    ImGui.TextUnformatted($"{Utils.GetAddonString(dataManager, pluginLog, currentLocale, 3501)}");
                }
                ImGui.TextUnformatted($"{currency:N0}/{max:N0}");
            }
            else
            {
                ImGui.TextUnformatted($"{currency:N0}");
            }

            ImGui.EndTable();
        }
    }

    private void DrawTribal(Character current_character)
    {
        if (current_character.Currencies == null) return;

        PlayerCurrencies pc = current_character.Currencies;

        ImGui.TextUnformatted(Utils.GetAddonString(dataManager, pluginLog, currentLocale, 5751));
        ImGui.Separator();
        if (ImGui.BeginTable("TribalCurrencyTable", 3))
        {
            ImGui.TableSetupColumn(string.Empty, ImGuiTableColumnFlags.WidthStretch);
            ImGui.TableSetupColumn(string.Empty, ImGuiTableColumnFlags.WidthStretch);
            ImGui.TableSetupColumn(string.Empty, ImGuiTableColumnFlags.WidthStretch);
            if (current_character.IsQuestCompleted(66754) ||
                current_character.IsQuestCompleted(66789) ||
                current_character.IsQuestCompleted(66857) ||
                current_character.IsQuestCompleted(66911) ||
                current_character.IsQuestCompleted(67023)
            )
            {
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                ImGui.TextUnformatted("A Realm Reborn");
                ImGui.TableNextRow();
                if (current_character.IsQuestCompleted(66754))
                {
                    //ImGui.TableSetColumnIndex(0);
                    ImGui.TableNextColumn();
                    DrawTribalCurrency(pc.Steel_Amaljok, Currencies.STEEL_AMALJOK, Tribal.AMALJ_AA);
                }
                if (current_character.IsQuestCompleted(66789))
                {
                    //ImGui.TableSetColumnIndex(1);
                    ImGui.TableNextColumn();
                    DrawTribalCurrency(pc.Sylphic_Goldleaf, Currencies.SYLPHIC_GOLDLEAF, Tribal.SYLPHS);
                }
                if (current_character.IsQuestCompleted(66857))
                {
                    //ImGui.TableSetColumnIndex(2);
                    ImGui.TableNextColumn();
                    DrawTribalCurrency(pc.Titan_Cobaltpiece, Currencies.TITAN_COBALTPIECE, Tribal.KOBOLDS);
                }

                //ImGui.TableNextRow();
                if (current_character.IsQuestCompleted(66911))
                {
                    //ImGui.TableSetColumnIndex(0);
                    ImGui.TableNextColumn();
                    DrawTribalCurrency(pc.Rainbowtide_Psashp, Currencies.RAINBOWTIDE_PSASHP, Tribal.SAHAGIN);
                }
                if (current_character.IsQuestCompleted(67023))
                {
                    //ImGui.TableSetColumnIndex(1);
                    ImGui.TableNextColumn();
                    DrawTribalCurrency(pc.Ixali_Oaknot, Currencies.IXALI_OAKNOT, Tribal.IXAL);
                }
            }

            if (current_character.IsQuestCompleted(67700) ||
                current_character.IsQuestCompleted(67791) ||
                current_character.IsQuestCompleted(67856)
            )
            {
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                ImGui.TextUnformatted("Heavensward");
                ImGui.TableNextRow();
                if (current_character.IsQuestCompleted(67700))
                {
                    //ImGui.TableSetColumnIndex(0);
                    ImGui.TableNextColumn();
                    DrawTribalCurrency(pc.Vanu_Whitebone, Currencies.VANU_WHITEBONE, Tribal.VANU_VANU);
                }
                if (current_character.IsQuestCompleted(67791))
                {
                    //ImGui.TableSetColumnIndex(1);
                    ImGui.TableNextColumn();
                    DrawTribalCurrency(pc.Black_Copper_Gil, Currencies.BLACK_COPPER_GIL, Tribal.VATH);
                }
                if (current_character.IsQuestCompleted(67856))
                {
                    //ImGui.TableSetColumnIndex(2);
                    ImGui.TableNextColumn();
                    DrawTribalCurrency(pc.Carved_Kupo_Nut, Currencies.CARVED_KUPO_NUT, Tribal.MOOGLES);
                }
            }

            if (current_character.IsQuestCompleted(68509) ||
                current_character.IsQuestCompleted(68572) ||
                current_character.IsQuestCompleted(68633)
            )
            {
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                ImGui.TextUnformatted("Stormblood");
                ImGui.TableNextRow();
                if (current_character.IsQuestCompleted(68509))
                {
                    //ImGui.TableSetColumnIndex(0);
                    ImGui.TableNextColumn();
                    DrawTribalCurrency(pc.Kojin_Sango, Currencies.KOJIN_SANGO, Tribal.KOJIN);
                }
                if (current_character.IsQuestCompleted(68572))
                {
                    //ImGui.TableSetColumnIndex(1);
                    ImGui.TableNextColumn();
                    DrawTribalCurrency(pc.Ananta_Dreamstaff, Currencies.ANANTA_DREAMSTAFF, Tribal.ANANTA);
                }
                if (current_character.IsQuestCompleted(68633))
                {
                    //ImGui.TableSetColumnIndex(2);
                    ImGui.TableNextColumn();
                    DrawTribalCurrency(pc.Namazu_Koban, Currencies.NAMAZU_KOBAN, Tribal.NAMAZU);
                }
            }

            if (current_character.IsQuestCompleted(69219) ||
                current_character.IsQuestCompleted(69330) ||
                current_character.IsQuestCompleted(69432)
            )
            {
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                ImGui.TextUnformatted("Shadowbringers");
                ImGui.TableNextRow();
                if (current_character.IsQuestCompleted(69219))
                {
                    //ImGui.TableSetColumnIndex(0);
                    ImGui.TableNextColumn();
                    DrawTribalCurrency(pc.Fae_Fancy, Currencies.FAE_FANCY, Tribal.PIXIES);
                }
                if (current_character.IsQuestCompleted(69330))
                {
                    //ImGui.TableSetColumnIndex(1);
                    ImGui.TableNextColumn();
                    DrawTribalCurrency(pc.Qitari_Compliment, Currencies.QITARI_COMPLIMENT, Tribal.QITARI);
                }
                if (current_character.IsQuestCompleted(69432))
                {
                    //ImGui.TableSetColumnIndex(2);
                    ImGui.TableNextColumn();
                    DrawTribalCurrency(pc.Hammered_Frogment, Currencies.HAMMERED_FROGMENT, Tribal.DWARVES);
                }
            }

            if (current_character.IsQuestCompleted(70081) ||
                current_character.IsQuestCompleted(70137) ||
                current_character.IsQuestCompleted(70217)
            )
            {
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                ImGui.TextUnformatted("Endwalker");
                ImGui.TableNextRow();
                if (current_character.IsQuestCompleted(70081))
                {
                    //ImGui.TableSetColumnIndex(0);
                    ImGui.TableNextColumn();
                    DrawTribalCurrency(pc.Arkasodara_Pana, Currencies.ARKASODARA_PANA, Tribal.ARKASODARA);
                }
                if (current_character.IsQuestCompleted(70137))
                {
                    //ImGui.TableSetColumnIndex(1);
                    ImGui.TableNextColumn();
                    DrawTribalCurrency(pc.Omicron_Omnitoken, Currencies.OMICRON_OMNITOKEN, Tribal.OMICRONS);
                }
                if (current_character.IsQuestCompleted(70217))
                {
                    //ImGui.TableSetColumnIndex(2);
                    ImGui.TableNextColumn();
                    DrawTribalCurrency(pc.Loporrit_Carat, Currencies.LOPORRIT_CARAT, Tribal.LOPORRITS);
                }
            }

            ImGui.EndTable();
        }
    }

    private void DrawTribalCurrency(int currency, Currencies id, Tribal tribal_id)
    {
        if (ImGui.BeginTable("TribalCurrencyTable", 2))
        {
            ImGui.TableSetupColumn(string.Empty, ImGuiTableColumnFlags.WidthFixed, 50);
            ImGui.TableSetupColumn(string.Empty, ImGuiTableColumnFlags.WidthFixed, 20);
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            Utils.DrawItemIcon(textureProvider, dataManager, pluginLog, currentLocale, new Vector2(32, 32), false, (uint)id);
            if (ImGui.IsItemHovered())
                ImGui.SetTooltip(
                    Utils.GetTribalCurrencyFromId(dataManager, pluginLog, currentLocale, (uint)tribal_id)
                );
            ImGui.TableSetColumnIndex(1);
            ImGui.TextUnformatted($"{currency}");

            ImGui.EndTable();
        }
    }
}
