using Altoholic.Cache;
using Altoholic.Models;
using CheapLoc;
using Dalamud.Game;
using Dalamud.Game.Text;
using Dalamud.Interface;
using Dalamud.Interface.Colors;
using Dalamud.Interface.Textures.TextureWraps;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Windowing;
using Dalamud.Bindings.ImGui;
using Lumina.Excel.Sheets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Emote = Altoholic.Models.Emote;
using Mount = Altoholic.Models.Mount;
using Ornament = Altoholic.Models.Ornament;
using PvPRank = Altoholic.Models.PvPRank;
using TripleTriadCard = Altoholic.Models.TripleTriadCard;

namespace Altoholic.Windows
{
    public class PvPWindow : Window, IDisposable
    {
        private readonly Plugin _plugin;
        private ClientLanguage _currentLocale;
        private GlobalCache _globalCache;

        public PvPWindow(
            Plugin plugin,
            string name,
            GlobalCache globalCache
        )
            : base(
                name, ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse)
        {
            SizeConstraints = new WindowSizeConstraints
            {
                MinimumSize = new Vector2(1000, 450), MaximumSize = new Vector2(float.MaxValue, float.MaxValue)
            };
            _plugin = plugin;
            _globalCache = globalCache;

            _companyIcons = Plugin.PluginInterface.UiBuilder.LoadUld("ui/uld/PVP.uld");
            _companyTextures.Add(1, _companyIcons.LoadTexturePart("ui/uld/PVP_hr1.tex", 9));
            _companyTextures.Add(2, _companyIcons.LoadTexturePart("ui/uld/PVP_hr1.tex", 10));
            _companyTextures.Add(3, _companyIcons.LoadTexturePart("ui/uld/PVP_hr1.tex", 11));
            _companyTextures.Add(4, _companyIcons.LoadTexturePart("ui/uld/PVP_hr1.tex", 12));

            _pvpRanksIcon = Plugin.PluginInterface.UiBuilder.LoadUld("ui/uld/PVPColosseum.uld");
            _pvpRanksTextures.Add(PvPRankEmblem.Diamond, _pvpRanksIcon.LoadTexturePart("ui/uld/PVPColosseum_hr1.tex", 3));
            _pvpRanksTextures.Add(PvPRankEmblem.Platinum, _pvpRanksIcon.LoadTexturePart("ui/uld/PVPColosseum_hr1.tex", 4));
            _pvpRanksTextures.Add(PvPRankEmblem.Gold, _pvpRanksIcon.LoadTexturePart("ui/uld/PVPColosseum_hr1.tex", 5));
            _pvpRanksTextures.Add(PvPRankEmblem.Silver, _pvpRanksIcon.LoadTexturePart("ui/uld/PVPColosseum_hr1.tex", 6));
            _pvpRanksTextures.Add(PvPRankEmblem.Bronze, _pvpRanksIcon.LoadTexturePart("ui/uld/PVPColosseum_hr1.tex", 7));
            _pvpRanksTextures.Add(PvPRankEmblem.Crystal, _pvpRanksIcon.LoadTexturePart("ui/uld/PVPColosseum_hr1.tex", 9));
        }

        public Func<Character>? GetPlayer { get; init; }
        public Func<List<Character>>? GetOthersCharactersList { get; init; }
        private Character? _currentCharacter;
        private bool _isSpoilerEnabled;

        private readonly UldWrapper _companyIcons;
        private readonly Dictionary<uint, IDalamudTextureWrap?> _companyTextures = [];
        private readonly UldWrapper _pvpRanksIcon;
        private readonly Dictionary<PvPRankEmblem, IDalamudTextureWrap?> _pvpRanksTextures = [];

        public void Clear()
        {
            Plugin.Log.Info("DetailsWindow, Clear() called");
            _currentCharacter = null;
            _isSpoilerEnabled = false;
        }

        public void Dispose()
        {
            Plugin.Log.Info("DetailsWindow, Dispose() called");
            _currentCharacter = null;
            _isSpoilerEnabled = false;
            foreach (KeyValuePair<uint, IDalamudTextureWrap?> loadedTexture in _companyTextures) loadedTexture.Value?.Dispose();
            _companyIcons.Dispose();
            foreach (KeyValuePair<PvPRankEmblem, IDalamudTextureWrap?> loadedTexture in _pvpRanksTextures) loadedTexture.Value?.Dispose();
            _pvpRanksIcon.Dispose();
        }

        public override void Draw()
        {
            if (GetPlayer?.Invoke() == null) return;
            if (GetOthersCharactersList?.Invoke() == null) return;
            _currentLocale = _plugin.Configuration.Language;
            _isSpoilerEnabled = _plugin.Configuration.IsSpoilersEnabled;
            //Plugin.Log.Debug($"DrawDetails character with c : id = {character.CharacterId}, FirstName = {character.FirstName}, LastName = {character.LastName}, HomeWorld = {character.HomeWorld}, DataCenter = {character.Datacenter}, LastJob = {character.LastJob}, LastJobLevel = {character.LastJobLevel}, FCTag = {character.FCTag}, FreeCompany = {character.FreeCompany}, LastOnline = {character.LastOnline}, PlayTime = {character.PlayTime}, LastPlayTimeUpdate = {character.LastPlayTimeUpdate}, Quests = {character.Quests.Count}, Inventory = {character.Inventory.Count}, Gear {character.Gear.Count}, Retainers = {character.Retainers.Count}");
            List<Character> chars = [];
            chars.Insert(0, GetPlayer.Invoke());
            chars.AddRange(GetOthersCharactersList.Invoke());

            using var characterDetailsTable = ImRaii.Table("###CharactersDetailsTable", 2);
            if (!characterDetailsTable) return;
            ImGui.TableSetupColumn("###CharactersDetailsTable#CharacterLists", ImGuiTableColumnFlags.WidthFixed,
                200);
            ImGui.TableSetupColumn("###CharactersDetailsTable#Details", ImGuiTableColumnFlags.WidthStretch);
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            using (var listBox =
                   ImRaii.ListBox("###CharactersDetailsTable#CharactersListBox", new Vector2(200, -1)))
            {
                if (listBox)
                {
#if DEBUG
                        for (int i = 0; i < 15; i++)
                        {
                            chars.Add(new Character()
                            {
                                FirstName = $"Dummy {i}",
                                LastName = $"LN {i}",
                                HomeWorld = $"Homeworld {i}",
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
                DrawPvP(_currentCharacter);
            }
        }

        private void DrawPvP(Character currentCharacter)
        {
            if (currentCharacter.Profile is null) return;
            if (currentCharacter.Profile.GrandCompany == 0)
            {
                ImGui.TextUnformatted(Loc.Localize("PvPProfileNotUnlocked", "This character doesn't have PvP unlocked"));
                return;
            }
            if (currentCharacter.PvPProfile == null)
            {
                ImGui.TextUnformatted(Loc.Localize("PvPProfileNotLoaded", "This character PvP profile wasn't loaded"));
                return;
            }

            uint pvpRank = currentCharacter.Profile.GrandCompany switch
            {
                1 => currentCharacter.PvPProfile.RankMaelstrom,
                2 => currentCharacter.PvPProfile.RankTwinAdder,
                3 => currentCharacter.PvPProfile.RankImmortalFlames,
                _ => 0
            };
            uint pvpExp = currentCharacter.Profile.GrandCompany switch
            {
                1 => currentCharacter.PvPProfile.ExperienceMaelstrom,
                2 => currentCharacter.PvPProfile.ExperienceTwinAdder,
                3 => currentCharacter.PvPProfile.ExperienceImmortalFlames,
                _ => 0
            };

            PvPRank? currentRank = _globalCache.PvPStorage.GetRank(pvpRank);
            if (currentRank is null) return;

            string rankName = _currentLocale switch
            {
                ClientLanguage.German => currentRank.Transients?.GermanTransients[
                    currentCharacter.Profile.GrandCompany - 1] ?? string.Empty,
                ClientLanguage.English => currentRank.Transients?.EnglishTransients[
                    currentCharacter.Profile.GrandCompany - 1] ?? string.Empty,
                ClientLanguage.French => currentRank.Transients?.FrenchTransients[
                    currentCharacter.Profile.GrandCompany - 1] ?? string.Empty,
                ClientLanguage.Japanese => currentRank.Transients?.JapaneseTransients[
                    currentCharacter.Profile.GrandCompany - 1] ?? string.Empty,
                _ => currentRank.Transients?.EnglishTransients[currentCharacter.Profile.GrandCompany - 1] ?? string.Empty
            };

            IDalamudTextureWrap? companyBanner = _companyTextures[(uint)currentCharacter.Profile.GrandCompany];
            if (companyBanner is null) return;
            using (var pvpProfileTabTable =
                   ImRaii.Table("###pvpProfileTabTable ", 2))
            {
                if (pvpProfileTabTable)
                {
                    ImGui.TableSetupColumn(
                        $"###pvpProfileTabTable #{currentCharacter.CharacterId}#Col1",
                        ImGuiTableColumnFlags.WidthFixed, companyBanner.Width + 10);
                    ImGui.TableSetupColumn(
                        $"###pvpProfileTabTable #{currentCharacter.CharacterId}#Col2",
                        ImGuiTableColumnFlags.WidthStretch);
                    ImGui.TableNextRow();
                    ImGui.TableSetColumnIndex(0);
                    ImGui.Image(companyBanner.Handle, new Vector2(companyBanner.Width, companyBanner.Height));
                    ImGui.TableSetColumnIndex(1);
                    ImGui.TextUnformatted(rankName);
                    ImGui.TextUnformatted("");
                    ImGui.TextUnformatted($"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 102454).ToUpper()} {pvpRank}");
                    DrawPvPExperience(pvpExp, currentRank.ExpRequired, _globalCache.PvPStorage.GetRankExperience(pvpRank + 1));
                    DrawSerieExperience(currentCharacter.PvPProfile);
                }
            }

            using var tabBar = ImRaii.TabBar("###PvPWindow#Tabs");
            if (!tabBar.Success) return;
            using (var crystallineConflictTab =
                   ImRaii.TabItem($"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 14863)}"))
            {
                if (crystallineConflictTab.Success)
                {
                    string victories = _globalCache.AddonStorage.LoadAddonString(_currentLocale, 14867);
                    ImGui.TextUnformatted(_globalCache.AddonStorage.LoadAddonString(_currentLocale, 14864));
                    ImGui.Separator();
                    using (var crystallineConflictCasualTabTable =
                           ImRaii.Table("###crystallineConflictCasualTabTable", 2))
                    {
                        if (crystallineConflictCasualTabTable)
                        {
                            ImGui.TableSetupColumn(
                                $"###crystallineConflictCasualTabTable#{currentCharacter.CharacterId}#Col1",
                                ImGuiTableColumnFlags.WidthStretch);
                            ImGui.TableSetupColumn(
                                $"###crystallineConflictCasualTabTable#{currentCharacter.CharacterId}#Col2",
                                ImGuiTableColumnFlags.WidthStretch);
                            ImGui.TableNextRow();
                            ImGui.TableSetColumnIndex(0);
                            ImGui.TextUnformatted(_globalCache.AddonStorage.LoadAddonString(_currentLocale, 102460));
                            ImGui.TableSetColumnIndex(1);
                            ImGui.TextUnformatted(
                                $"{currentCharacter.PvPProfile.CrystallineConflictCasualMatches:N0}");
                            ImGui.TableNextRow();
                            ImGui.TableSetColumnIndex(0);
                            ImGui.TextUnformatted(victories);
                            ImGui.TableSetColumnIndex(1);
                            ImGui.TextUnformatted(
                                $"{currentCharacter.PvPProfile.CrystallineConflictCasualMatchesWon:N0} ({GetPercent(currentCharacter.PvPProfile.CrystallineConflictCasualMatches, currentCharacter.PvPProfile.CrystallineConflictCasualMatchesWon)}%)");
                        }
                    }
                    ImGui.TextUnformatted(_globalCache.AddonStorage.LoadAddonString(_currentLocale, 14865));
                    ImGui.Separator();
                    using (var crystallineConflictRankedTabTable =
                           ImRaii.Table("###crystallineConflictRankedTabTable", 2))
                    {
                        if (crystallineConflictRankedTabTable)
                        {
                            ImGui.TableSetupColumn(
                                $"###crystallineConflictRankedTabTable#{currentCharacter.CharacterId}#Col1",
                                ImGuiTableColumnFlags.WidthStretch);
                            ImGui.TableSetupColumn(
                                $"###crystallineConflictRankedTabTable#{currentCharacter.CharacterId}#Col2",
                                ImGuiTableColumnFlags.WidthStretch);
                            ImGui.TableNextRow();
                            ImGui.TableSetColumnIndex(0);
                            ImGui.TextUnformatted(_globalCache.AddonStorage.LoadAddonString(_currentLocale, 102460));
                            ImGui.TableSetColumnIndex(1);
                            ImGui.TextUnformatted(
                                $"{currentCharacter.PvPProfile.CrystallineConflictRankedMatches:N0}");
                            ImGui.TableNextRow();
                            ImGui.TableSetColumnIndex(0);
                            ImGui.TextUnformatted(victories);
                            ImGui.TableSetColumnIndex(1);
                            ImGui.TextUnformatted(
                                $"{currentCharacter.PvPProfile.CrystallineConflictRankedMatchesWon:N0} ({GetPercent(currentCharacter.PvPProfile.CrystallineConflictRankedMatches, currentCharacter.PvPProfile.CrystallineConflictRankedMatchesWon)}%)");
                        }
                    }

                    ImGui.TextUnformatted(_globalCache.AddonStorage.LoadAddonString(_currentLocale, 5412));
                    ImGui.Separator();
                    using (var crystallineConflictRankingTabTable =
                           ImRaii.Table("###crystallineConflictRankingTabTable", 4))
                    {
                        if (crystallineConflictRankingTabTable)
                        {
                            ImGui.TableSetupColumn(
                                $"###crystallineConflictRankingTabTable#{currentCharacter.CharacterId}#Col1",
                                ImGuiTableColumnFlags.WidthStretch);
                            ImGui.TableSetupColumn(
                                $"###crystallineConflictRankingTabTable#{currentCharacter.CharacterId}#Col2",
                                ImGuiTableColumnFlags.WidthStretch);
                            ImGui.TableSetupColumn(
                                $"###crystallineConflictRankingTabTable#{currentCharacter.CharacterId}#Col3",
                                ImGuiTableColumnFlags.WidthStretch);
                            ImGui.TableSetupColumn(
                                $"###crystallineConflictRankingTabTable#{currentCharacter.CharacterId}#Col4",
                                ImGuiTableColumnFlags.WidthStretch);
                            ImGui.TableNextRow();
                            ImGui.TableSetColumnIndex(0);
                            ImGui.TextUnformatted(_globalCache.AddonStorage.LoadAddonString(_currentLocale, 11753));
                            ImGui.TableSetColumnIndex(1);
                            ImGui.TextUnformatted($"{GetCystallineConflictRankName(currentCharacter.PvPProfile.CrystallineConflictCurrentRank)}");
                            ImGui.TableSetColumnIndex(2);
                            ImGui.TextUnformatted($"{(currentCharacter.PvPProfile.CrystallineConflictCurrentRank == 0 ? "-" : currentCharacter.PvPProfile.CrystallineConflictCurrentRank)}");
                            ImGui.TableSetColumnIndex(3);
                            ImGui.TextUnformatted($"{GetCystallineConflictStars(currentCharacter.PvPProfile.CrystallineConflictCurrentRank, currentCharacter.PvPProfile.CrystallineConflictCurrentRisingStars)}");

                            ImGui.TableNextRow();
                            ImGui.TableSetColumnIndex(0);
                            ImGui.TextUnformatted(_globalCache.AddonStorage.LoadAddonString(_currentLocale, 11755));
                            ImGui.TableSetColumnIndex(1);
                            ImGui.TextUnformatted($"{GetCystallineConflictRankName(currentCharacter.PvPProfile.CrystallineConflictHighestRank)}");
                            ImGui.TableSetColumnIndex(2);
                            ImGui.TextUnformatted($"{(currentCharacter.PvPProfile.CrystallineConflictHighestRank == 0 ? "-" : currentCharacter.PvPProfile.CrystallineConflictHighestRank)}");
                            ImGui.TableSetColumnIndex(3);
                            ImGui.TextUnformatted($"{GetCystallineConflictStars(currentCharacter.PvPProfile.CrystallineConflictHighestRank, currentCharacter.PvPProfile.CrystallineConflictHighestRisingStars)}");
                        }
                    }


                    using (var crystallineConflictEmblemTabTable =
                           ImRaii.Table("###crystallineConflictEmblemTabTable", 3, ImGuiTableFlags.None, new Vector2(850, 100)))
                    {
                        if (crystallineConflictEmblemTabTable)
                        {
                            ImGui.TableSetupColumn(
                                $"###crystallineConflictEmblemTabTable#{currentCharacter.CharacterId}#Col1",
                                ImGuiTableColumnFlags.WidthFixed, 365);
                            ImGui.TableSetupColumn(
                                $"###crystallineConflictEmblemTabTable#{currentCharacter.CharacterId}#Col2",
                                ImGuiTableColumnFlags.WidthFixed, 120);
                            ImGui.TableSetupColumn(
                                $"###crystallineConflictEmblemTabTable#{currentCharacter.CharacterId}#Col3",
                                ImGuiTableColumnFlags.WidthFixed, 365);
                            ImGui.TableNextRow();
                            ImGui.TableSetColumnIndex(0);
                            ImGui.TextUnformatted("");
                            ImGui.TableSetColumnIndex(1);
                            if (currentCharacter.PvPProfile.CrystallineConflictCurrentRank > 0)
                            {
                                IDalamudTextureWrap? emblem =
                                    GetCystallineConflictRankEmblem(currentCharacter.PvPProfile.CrystallineConflictCurrentRank);
                                if (emblem is not null)
                                {
                                    ImGui.Image(emblem.Handle, new Vector2((float)emblem.Width / 2, (float)emblem.Height / 2));
                                }
                            }
                            ImGui.TableSetColumnIndex(2);
                            ImGui.TextUnformatted("");
                        }
                    }
                }
            }

            string overallPerformance = _globalCache.AddonStorage.LoadAddonString(_currentLocale, 102459);
            string weeklyPerformance = _globalCache.AddonStorage.LoadAddonString(_currentLocale, 102462);
            using (var frontlineTab =
                   ImRaii.TabItem($"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 10043)}"))
            {
                if (frontlineTab.Success)
                {
                    ImGui.TextUnformatted(overallPerformance);
                    ImGui.Separator();
                    using (var frontlineOverallTabTable =
                           ImRaii.Table("###frontlineOverallTabTable", 2))
                    {
                        if (frontlineOverallTabTable)
                        {
                            ImGui.TableSetupColumn(
                                $"###frontlineOverallTabTable#{currentCharacter.CharacterId}#Col1",
                                ImGuiTableColumnFlags.WidthStretch);
                            ImGui.TableSetupColumn(
                                $"###frontlineOverallTabTable#{currentCharacter.CharacterId}#Col2",
                                ImGuiTableColumnFlags.WidthStretch);
                            ImGui.TableNextRow();
                            ImGui.TableSetColumnIndex(0);
                            ImGui.TextUnformatted(_globalCache.AddonStorage.LoadAddonString(_currentLocale, 102489));
                            ImGui.TableSetColumnIndex(1);
                            ImGui.TextUnformatted($"{currentCharacter.PvPProfile.FrontlineTotalMatches:N0}");
                            ImGui.TableNextRow();
                            ImGui.TableSetColumnIndex(0);
                            ImGui.TextUnformatted(_globalCache.AddonStorage.LoadAddonString(_currentLocale, 102490));
                            ImGui.TableSetColumnIndex(1);
                            ImGui.TextUnformatted($"{currentCharacter.PvPProfile.FrontlineTotalFirstPlace:N0} ({GetPercent(currentCharacter.PvPProfile.FrontlineTotalMatches, currentCharacter.PvPProfile.FrontlineTotalFirstPlace)}%)");
                            ImGui.TableNextRow();
                            ImGui.TableSetColumnIndex(0);
                            ImGui.TextUnformatted(_globalCache.AddonStorage.LoadAddonString(_currentLocale, 102491));
                            ImGui.TableSetColumnIndex(1);
                            ImGui.TextUnformatted($"{currentCharacter.PvPProfile.FrontlineTotalSecondPlace:N0} ({GetPercent(currentCharacter.PvPProfile.FrontlineTotalMatches, currentCharacter.PvPProfile.FrontlineTotalSecondPlace)}%)");
                            ImGui.TableNextRow();
                            ImGui.TableSetColumnIndex(0);
                            ImGui.TextUnformatted(_globalCache.AddonStorage.LoadAddonString(_currentLocale, 102492));
                            ImGui.TableSetColumnIndex(1);
                            ImGui.TextUnformatted($"{currentCharacter.PvPProfile.FrontlineTotalThirdPlace:N0} ({GetPercent(currentCharacter.PvPProfile.FrontlineTotalMatches, currentCharacter.PvPProfile.FrontlineTotalThirdPlace)}%)");

                        }
                    }

                    ImGui.TextUnformatted(weeklyPerformance);
                    ImGui.Separator();
                    using (var frontlineWeeklyTabTable =
                           ImRaii.Table("###frontlineWeeklyTabTable", 2))
                    {
                        if (frontlineWeeklyTabTable)
                        {
                            ImGui.TableSetupColumn(
                                $"###frontlineWeeklyTabTable#{currentCharacter.CharacterId}#Col1",
                                ImGuiTableColumnFlags.WidthStretch);
                            ImGui.TableSetupColumn(
                                $"###frontlineWeeklyTabTable#{currentCharacter.CharacterId}#Col2",
                                ImGuiTableColumnFlags.WidthStretch);
                            ImGui.TableNextRow();
                            ImGui.TableSetColumnIndex(0);
                            ImGui.TextUnformatted(_globalCache.AddonStorage.LoadAddonString(_currentLocale, 102489));
                            ImGui.TableSetColumnIndex(1);
                            ImGui.TextUnformatted($"{currentCharacter.PvPProfile.FrontlineWeeklyMatches:N0}");
                            ImGui.TableNextRow();
                            ImGui.TableSetColumnIndex(0);
                            ImGui.TextUnformatted(_globalCache.AddonStorage.LoadAddonString(_currentLocale, 102490));
                            ImGui.TableSetColumnIndex(1);
                            ImGui.TextUnformatted($"{currentCharacter.PvPProfile.FrontlineWeeklyFirstPlace:N0} ({GetPercent(currentCharacter.PvPProfile.FrontlineWeeklyMatches, currentCharacter.PvPProfile.FrontlineWeeklyFirstPlace)}%)");
                            ImGui.TableNextRow();
                            ImGui.TableSetColumnIndex(0);
                            ImGui.TextUnformatted(_globalCache.AddonStorage.LoadAddonString(_currentLocale, 102491));
                            ImGui.TableSetColumnIndex(1);
                            ImGui.TextUnformatted($"{currentCharacter.PvPProfile.FrontlineWeeklySecondPlace:N0} ({GetPercent(currentCharacter.PvPProfile.FrontlineWeeklyMatches, currentCharacter.PvPProfile.FrontlineWeeklySecondPlace)}%)");
                            ImGui.TableNextRow();
                            ImGui.TableSetColumnIndex(0);
                            ImGui.TextUnformatted(_globalCache.AddonStorage.LoadAddonString(_currentLocale, 102492));
                            ImGui.TableSetColumnIndex(1);
                            ImGui.TextUnformatted($"{currentCharacter.PvPProfile.FrontlineWeeklyThirdPlace:N0} ({GetPercent(currentCharacter.PvPProfile.FrontlineWeeklyMatches, currentCharacter.PvPProfile.FrontlineWeeklyThirdPlace)}%)");
                        }
                    }
                }
            }

            string winStr = _globalCache.AddonStorage.LoadAddonString(_currentLocale, 9350);
            using (var rivalWingsTab =
                   ImRaii.TabItem($"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 11393)}"))
            {
                if (rivalWingsTab.Success)
                {
                    ImGui.TextUnformatted(overallPerformance);
                    ImGui.Separator();
                    using (var rivalWingsOverallTabTable =
                           ImRaii.Table("###rivalWingsOverallTabTable", 2))
                    {
                        if (rivalWingsOverallTabTable)
                        {
                            ImGui.TableSetupColumn(
                                $"###rivalWingsOverallTabTable#{currentCharacter.CharacterId}#Col1",
                                ImGuiTableColumnFlags.WidthStretch);
                            ImGui.TableSetupColumn(
                                $"###rivalWingsOverallTabTable#{currentCharacter.CharacterId}#Col2",
                                ImGuiTableColumnFlags.WidthStretch);
                            ImGui.TableNextRow();
                            ImGui.TableSetColumnIndex(0);
                            ImGui.TextUnformatted(_globalCache.AddonStorage.LoadAddonString(_currentLocale, 11396));
                            ImGui.TableSetColumnIndex(1);
                            ImGui.TextUnformatted($"{currentCharacter.PvPProfile.RivalWingsTotalMatches:N0}");
                            ImGui.TableNextRow();
                            ImGui.TableSetColumnIndex(0);
                            ImGui.TextUnformatted(_globalCache.AddonStorage.LoadAddonString(_currentLocale, 11397));
                            ImGui.TableSetColumnIndex(1);
                            ImGui.TextUnformatted($"{currentCharacter.PvPProfile.RivalWingsTotalMatchesWon:N0} ({winStr}{GetPercent(currentCharacter.PvPProfile.RivalWingsTotalMatches, currentCharacter.PvPProfile.RivalWingsTotalMatchesWon)}%)");

                        }
                    }

                    ImGui.TextUnformatted(weeklyPerformance);
                    ImGui.Separator();
                    using (var rivalWingsWeeklyTabTable =
                           ImRaii.Table("###rivalWingsWeeklyTabTable", 2))
                    {
                        if (rivalWingsWeeklyTabTable)
                        {
                            ImGui.TableSetupColumn(
                                $"###rivalWingsWeeklyTabTable#{currentCharacter.CharacterId}#Col1",
                                ImGuiTableColumnFlags.WidthStretch);
                            ImGui.TableSetupColumn(
                                $"###rivalWingsWeeklyTabTable#{currentCharacter.CharacterId}#Col2",
                                ImGuiTableColumnFlags.WidthStretch);
                            ImGui.TableNextRow();
                            ImGui.TableSetColumnIndex(0);
                            ImGui.TextUnformatted(_globalCache.AddonStorage.LoadAddonString(_currentLocale, 102489));
                            ImGui.TableSetColumnIndex(1);
                            ImGui.TextUnformatted($"{currentCharacter.PvPProfile.RivalWingsWeeklyMatches:N0}");
                            ImGui.TableNextRow();
                            ImGui.TableSetColumnIndex(0);
                            ImGui.TextUnformatted(_globalCache.AddonStorage.LoadAddonString(_currentLocale, 102490));
                            ImGui.TableSetColumnIndex(1);
                            ImGui.TextUnformatted($"{currentCharacter.PvPProfile.RivalWingsWeeklyMatchesWon:N0} ({winStr}{GetPercent(currentCharacter.PvPProfile.RivalWingsWeeklyMatches, currentCharacter.PvPProfile.RivalWingsWeeklyMatchesWon)}%)");
                        }
                    }
                }
            }

            using (var seriesRewardTab =
                   ImRaii.TabItem($"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 14866)}"))
            {
                if (seriesRewardTab.Success)
                {
                    using var pvpProfileTabSeriesTable =
                        ImRaii.Table("###pvpProfileTabSeriesTable ", 2, ImGuiTableFlags.None, new Vector2(-1, -1));
                    if (pvpProfileTabSeriesTable)
                    {
                        ImGui.TableSetupColumn(
                            $"###pvpProfileSeriesCurrentExperienceTabTable #{currentCharacter.CharacterId}#Col1",
                            ImGuiTableColumnFlags.WidthStretch);
                        ImGui.TableSetupColumn(
                            $"###pvpProfileSeriesCurrentExperienceTabTable #{currentCharacter.CharacterId}#Col2",
                            ImGuiTableColumnFlags.WidthStretch);
                        ImGui.TableNextRow();
                        ImGui.TableSetColumnIndex(0);
                        ImGui.TextUnformatted(
                            $"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 14900).ToUpper()} 10");
                        using (var pvpProfileSeriesCurrentExperienceTabTable =
                               ImRaii.Table("###pvpProfileSeriesCurrentExperienceTabTable ", 2,
                                   ImGuiTableFlags.None, new Vector2(400, 40)))
                        {
                            if (pvpProfileSeriesCurrentExperienceTabTable)
                            {
                                ImGui.TableSetupColumn(
                                    $"###pvpProfileSeriesCurrentExperienceTabTable #{currentCharacter.CharacterId}#Col1",
                                    ImGuiTableColumnFlags.WidthStretch);
                                ImGui.TableSetupColumn(
                                    $"###pvpProfileSeriesCurrentExperienceTabTable #{currentCharacter.CharacterId}#Col2",
                                    ImGuiTableColumnFlags.WidthFixed, 310);
                                ImGui.TableNextRow();
                                ImGui.TableSetColumnIndex(0);
                                ImGui.TextUnformatted("");
                                ImGui.TableSetColumnIndex(1);

                                DrawSerieExperience(currentCharacter.PvPProfile);
                            }
                        }

                        ImGui.TextUnformatted(
                            $"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 16432)}");
                        using (ImRaii.IEndObject t = ImRaii.Table("###Series#Rewards#10#Table", 5))
                        {
                            if (t)
                            {
                                ImGui.TableSetupColumn("###Series#Rewards#10#Table#Reward#Emote",
                                    ImGuiTableColumnFlags.WidthFixed, 36);
                                ImGui.TableSetupColumn("###Series#Rewards#10#Table#Reward#FramerKit",
                                    ImGuiTableColumnFlags.WidthFixed, 36);
                                ImGui.TableSetupColumn($"###Series#Rewards#10#Table#Reward#Minion",
                                    ImGuiTableColumnFlags.WidthFixed, 36);
                                ImGui.TableSetupColumn($"###Series#Rewards#10#Table#Reward#Minion2",
                                    ImGuiTableColumnFlags.WidthFixed, 36);
                                ImGui.TableSetupColumn($"###Series#Rewards#01#Table#Reward#Framerkit2",
                                    ImGuiTableColumnFlags.WidthFixed, 36);
                                ImGui.TableNextRow();
                                ImGui.TableSetColumnIndex(0);
                                DrawEmote(317, currentCharacter.HasEmote(317));
                                uint? fkId = _globalCache.FramerKitStorage.GetFramerKitIdFromItemId(49172);
                                if (fkId == null) return;
                                ImGui.TableSetColumnIndex(1);
                                DrawFramerKit(fkId.Value, currentCharacter.HasFramerKit(fkId.Value));
                                ImGui.TableSetColumnIndex(2);
                                DrawMinion(556, currentCharacter.HasMinion(556));
                                ImGui.TableSetColumnIndex(3);
                                uint? fkId2 = _globalCache.FramerKitStorage.GetFramerKitIdFromItemId(49173);
                                if (fkId2 == null) return;
                                DrawFramerKit(fkId2.Value, currentCharacter.HasFramerKit(fkId2.Value));
                                ImGui.TableSetColumnIndex(4);
                                DrawMount(394, currentCharacter.HasMount(394));
                            }
                        }

                        ImGui.TableSetColumnIndex(1);
                        using (var pvpProfileTabSeriesPreviousTable =
                               ImRaii.Table("###pvpProfileTabSeriesPreviousTable ", 2, ImGuiTableFlags.ScrollY,
                                   new Vector2(700, -1)))
                        {
                            if (!pvpProfileTabSeriesPreviousTable)
                            {
                                return;
                            }

                            ImGui.TableSetupColumn(
                                $"###pvpProfileSeriesCurrentExperienceTabTable #{currentCharacter.CharacterId}#Col1",
                                ImGuiTableColumnFlags.WidthStretch);

                            ImGui.TableNextRow();
                            ImGui.TableSetColumnIndex(0);
                            if (ImGui.CollapsingHeader(
                                $"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 14900)} 9###Series#Rewards#9"))
                            {
                                uint? series9Rank =
                                    currentCharacter.PvPProfile.SeriesPersonalRanks.GetValueOrDefault((uint)9);
                                uint? series9Claimed =
                                    currentCharacter.PvPProfile.SeriesPersonalRanksClaimed.GetValueOrDefault(
                                        (uint)7);
                                if (series9Rank is not null)
                                {
                                    ImGui.TextUnformatted(
                                        $"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 14860).ToUpper()} {series9Rank.Value}");
                                }

                                ImGui.TextUnformatted(
                                $"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 16432)}");
                                using ImRaii.IEndObject t = ImRaii.Table("###Series#Rewards#9#Table", 5);
                                if (t)
                                {
                                    ImGui.TableSetupColumn("###Series#Rewards#9#Table#Reward#Emote",
                                        ImGuiTableColumnFlags.WidthFixed, 36);
                                    ImGui.TableSetupColumn("###Series#Rewards#9#Table#Reward#FramerKit",
                                        ImGuiTableColumnFlags.WidthFixed, 36);
                                    ImGui.TableSetupColumn($"###Series#Rewards#9#Table#Reward#Minion",
                                        ImGuiTableColumnFlags.WidthFixed, 36);
                                    ImGui.TableSetupColumn($"###Series#Rewards#9#Table#Reward#Minion2",
                                        ImGuiTableColumnFlags.WidthFixed, 36);
                                    ImGui.TableSetupColumn($"###Series#Rewards#9#Table#Reward#Framerkit2",
                                        ImGuiTableColumnFlags.WidthFixed, 36);
                                    ImGui.TableNextRow();
                                    ImGui.TableSetColumnIndex(0);
                                    DrawEmote(307, currentCharacter.HasEmote(307));
                                    uint? fkId = _globalCache.FramerKitStorage.GetFramerKitIdFromItemId(46736);
                                    if (fkId == null) return;
                                    ImGui.TableSetColumnIndex(1);
                                    DrawFramerKit(fkId.Value, currentCharacter.HasFramerKit(fkId.Value));
                                    ImGui.TableSetColumnIndex(2);
                                    DrawMinion(556, currentCharacter.HasMinion(556));
                                    ImGui.TableSetColumnIndex(3);
                                    DrawMinion(555, currentCharacter.HasMinion(555));
                                    ImGui.TableSetColumnIndex(4);
                                    uint? fkId2 = _globalCache.FramerKitStorage.GetFramerKitIdFromItemId(46737);
                                    if (fkId2 == null) return;
                                    DrawFramerKit(fkId2.Value, currentCharacter.HasFramerKit(fkId2.Value));
                                }
                            }

                            ImGui.TableNextRow();
                            ImGui.TableSetColumnIndex(0);
                            if (ImGui.CollapsingHeader(
                                $"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 14900)} 8###Series#Rewards#8"))
                            {
                                uint? series8Rank =
                                    currentCharacter.PvPProfile.SeriesPersonalRanks.GetValueOrDefault((uint)8);
                                uint? series8Claimed =
                                    currentCharacter.PvPProfile.SeriesPersonalRanksClaimed.GetValueOrDefault(
                                        (uint)7);
                                if (series8Rank is not null)
                                {
                                    ImGui.TextUnformatted(
                                        $"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 14860).ToUpper()} {series8Rank.Value}");
                                }

                                ImGui.TextUnformatted(
                                    $"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 16432)}");
                                using ImRaii.IEndObject t = ImRaii.Table("###Series#Rewards#8#Table", 5);
                                if (t)
                                {
                                    ImGui.TableSetupColumn("###Series#Rewards#8#Table#Reward#Emote",
                                    ImGuiTableColumnFlags.WidthFixed, 36);
                                    ImGui.TableSetupColumn("###Series#Rewards#8#Table#Reward#FramerKit",
                                        ImGuiTableColumnFlags.WidthFixed, 36);
                                    ImGui.TableSetupColumn($"###Series#Rewards#8#Table#Reward#Minion",
                                        ImGuiTableColumnFlags.WidthFixed, 36);
                                    ImGui.TableSetupColumn($"###Series#Rewards#8#Table#Reward#Framerkit2",
                                        ImGuiTableColumnFlags.WidthFixed, 36);
                                    ImGui.TableSetupColumn($"###Series#Rewards#8#Table#Reward#Mount",
                                        ImGuiTableColumnFlags.WidthFixed, 36);
                                    ImGui.TableNextRow();
                                    ImGui.TableSetColumnIndex(0);
                                    DrawEmote(293, currentCharacter.HasEmote(293));
                                    uint? fkId = _globalCache.FramerKitStorage.GetFramerKitIdFromItemId(46335);
                                    if (fkId == null) return;
                                    ImGui.TableSetColumnIndex(1);
                                    DrawFramerKit(fkId.Value, currentCharacter.HasFramerKit(fkId.Value));
                                    ImGui.TableSetColumnIndex(2);
                                    DrawMinion(544, currentCharacter.HasMinion(544));
                                    ImGui.TableSetColumnIndex(3);
                                    uint? fkId2 = _globalCache.FramerKitStorage.GetFramerKitIdFromItemId(46336);
                                    if (fkId2 == null) return;
                                    DrawFramerKit(fkId2.Value, currentCharacter.HasFramerKit(fkId2.Value));
                                    ImGui.TableSetColumnIndex(4);
                                    DrawMount(390, currentCharacter.HasMount(390));
                                }
                            }
                            ImGui.TableNextRow();
                            ImGui.TableSetColumnIndex(0);
                            if (ImGui.CollapsingHeader(
                                    $"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 14900)} 7###Series#Rewards#7"))
                            {
                                uint? series7Rank =
                                    currentCharacter.PvPProfile.SeriesPersonalRanks.GetValueOrDefault((uint)7);
                                uint? series7Claimed =
                                    currentCharacter.PvPProfile.SeriesPersonalRanksClaimed.GetValueOrDefault(
                                        (uint)7);
                                if (series7Rank is not null)
                                {
                                    ImGui.TextUnformatted(
                                        $"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 14860).ToUpper()} {series7Rank.Value}");
                                }

                                ImGui.TextUnformatted(
                                    $"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 16432)}");
                                using ImRaii.IEndObject t = ImRaii.Table("###Series#Rewards#7#Table", 5);
                                if (t)
                                {
                                    ImGui.TableSetupColumn("###Series#Rewards#7#Table#Reward#Emote",
                                        ImGuiTableColumnFlags.WidthFixed, 36);
                                    ImGui.TableSetupColumn("###Series#Rewards#7#Table#Reward#FramerKit",
                                        ImGuiTableColumnFlags.WidthFixed, 36);
                                    ImGui.TableSetupColumn($"###Series#Rewards#7#Table#Reward#Minion",
                                        ImGuiTableColumnFlags.WidthFixed, 36);
                                    ImGui.TableSetupColumn($"###Series#Rewards#7#Table#Reward#Framerkit2",
                                        ImGuiTableColumnFlags.WidthFixed, 36);
                                    ImGui.TableSetupColumn($"###Series#Rewards#7#Table#Reward#Item",
                                        ImGuiTableColumnFlags.WidthFixed, 36);
                                    ImGui.TableNextRow();
                                    ImGui.TableSetColumnIndex(0);
                                    DrawEmote(279, currentCharacter.HasEmote(279));
                                    uint? fkId = _globalCache.FramerKitStorage.GetFramerKitIdFromItemId(44350);
                                    if (fkId == null) return;
                                    ImGui.TableSetColumnIndex(1);
                                    DrawFramerKit(fkId.Value, currentCharacter.HasFramerKit(fkId.Value));
                                    ImGui.TableSetColumnIndex(2);
                                    DrawMinion(530, currentCharacter.HasMinion(530));
                                    ImGui.TableSetColumnIndex(3);
                                    uint? fkId2 = _globalCache.FramerKitStorage.GetFramerKitIdFromItemId(44351);
                                    if (fkId2 == null) return;
                                    DrawFramerKit(fkId2.Value, currentCharacter.HasFramerKit(fkId2.Value));
                                    ImGui.TableSetColumnIndex(4);
                                    DrawItem(44352, 25, series7Claimed);
                                }
                            }

                            ImGui.TableNextRow();
                            ImGui.TableSetColumnIndex(0);
                            if (ImGui.CollapsingHeader(
                                    $"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 14900)} 6###Series#Rewards#6"))
                            {
                                uint? series6Rank =
                                    currentCharacter.PvPProfile.SeriesPersonalRanks.GetValueOrDefault((uint)6);
                                uint? series6Claimed =
                                    currentCharacter.PvPProfile.SeriesPersonalRanksClaimed.GetValueOrDefault(
                                        (uint)6);
                                ImGui.TextUnformatted(
                                    $"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 14860).ToUpper()} {(series6Rank.Value > 0 ? series6Rank.Value
                                            : _globalCache.AddonStorage.LoadAddonString(_currentLocale, 369)
                                        )}");

                                ImGui.TextUnformatted(
                                    $"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 16432)}");
                                using ImRaii.IEndObject t = ImRaii.Table("###Series#Rewards#6#Table", 5);
                                if (t)
                                {
                                    ImGui.TableSetupColumn("###Series#Rewards#6#Table#Reward#Ornament",
                                        ImGuiTableColumnFlags.WidthFixed, 36);
                                    ImGui.TableSetupColumn("###Series#Rewards#6#Table#Reward#FramerKit",
                                        ImGuiTableColumnFlags.WidthFixed, 36);
                                    ImGui.TableSetupColumn($"###Series#Rewards#6#Table#Reward#Minion",
                                        ImGuiTableColumnFlags.WidthFixed, 36);
                                    ImGui.TableSetupColumn($"###Series#Rewards#6#Table#Reward#Framerkit2",
                                        ImGuiTableColumnFlags.WidthFixed, 36);
                                    ImGui.TableSetupColumn($"###Series#Rewards#6#Table#Reward#Mount",
                                        ImGuiTableColumnFlags.WidthFixed, 36);
                                    ImGui.TableNextRow();
                                    ImGui.TableSetColumnIndex(0);
                                    DrawOrnament(41, currentCharacter.HasOrnament(41));
                                    uint? fkId = _globalCache.FramerKitStorage.GetFramerKitIdFromItemId(43565);
                                    if (fkId == null) return;
                                    ImGui.TableSetColumnIndex(1);
                                    DrawFramerKit(fkId.Value, currentCharacter.HasFramerKit(fkId.Value));
                                    ImGui.TableSetColumnIndex(2);
                                    DrawMinion(514, currentCharacter.HasMinion(514));
                                    ImGui.TableSetColumnIndex(3);
                                    uint? fkId2 = _globalCache.FramerKitStorage.GetFramerKitIdFromItemId(43566);
                                    if (fkId2 == null) return;
                                    DrawFramerKit(fkId2.Value, currentCharacter.HasFramerKit(fkId2.Value));
                                    ImGui.TableSetColumnIndex(4);
                                    DrawMount(331, currentCharacter.HasMount(331));
                                }
                            }

                            ImGui.TableNextRow();
                            ImGui.TableSetColumnIndex(0);
                            if (ImGui.CollapsingHeader(
                                    $"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 14900)} 5###Series#Rewards#5"))
                            {
                                uint? series5Rank =
                                    currentCharacter.PvPProfile.SeriesPersonalRanks.GetValueOrDefault((uint)5);
                                uint? series5Claimed =
                                    currentCharacter.PvPProfile.SeriesPersonalRanksClaimed.GetValueOrDefault(
                                        (uint)5);
                                ImGui.TextUnformatted(
                                    $"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 14860).ToUpper()} {(series5Rank.Value > 0 ? series5Rank.Value
                                            : _globalCache.AddonStorage.LoadAddonString(_currentLocale, 369)
                                        )}");

                                ImGui.TextUnformatted(
                                    $"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 16432)}");
                                using ImRaii.IEndObject t = ImRaii.Table("###Series#Rewards#5#Table", 5);
                                if (t)
                                {
                                    ImGui.TableSetupColumn("###Series#Rewards#5#Table#Reward#Emote",
                                        ImGuiTableColumnFlags.WidthFixed, 36);
                                    ImGui.TableSetupColumn("###Series#Rewards#5#Table#Reward#FramerKit",
                                        ImGuiTableColumnFlags.WidthFixed, 36);
                                    ImGui.TableSetupColumn($"###Series#Rewards#5#Table#Reward#Minion",
                                        ImGuiTableColumnFlags.WidthFixed, 36);
                                    ImGui.TableSetupColumn($"###Series#Rewards#5#Table#Reward#Framerkit2",
                                        ImGuiTableColumnFlags.WidthFixed, 36);
                                    ImGui.TableSetupColumn($"###Series#Rewards#5#Table#Reward#Item",
                                        ImGuiTableColumnFlags.WidthFixed, 36);
                                    ImGui.TableNextRow();
                                    ImGui.TableSetColumnIndex(0);
                                    DrawEmote(273, currentCharacter.HasEmote(273));
                                    uint? fkId = _globalCache.FramerKitStorage.GetFramerKitIdFromItemId(41367);
                                    if (fkId == null) return;
                                    ImGui.TableSetColumnIndex(1);
                                    DrawFramerKit(fkId.Value, currentCharacter.HasFramerKit(fkId.Value));
                                    ImGui.TableSetColumnIndex(2);
                                    DrawMinion(493, currentCharacter.HasMinion(493));
                                    ImGui.TableSetColumnIndex(3);
                                    uint? fkId2 = _globalCache.FramerKitStorage.GetFramerKitIdFromItemId(41368);
                                    if (fkId2 == null) return;
                                    DrawFramerKit(fkId2.Value, currentCharacter.HasFramerKit(fkId2.Value));
                                    ImGui.TableSetColumnIndex(4);
                                    DrawItem(41523, 25, series5Claimed);
                                }
                            }

                            ImGui.TableNextRow();
                            ImGui.TableSetColumnIndex(0);
                            if (ImGui.CollapsingHeader(
                                    $"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 14900)} 4###Series#Rewards#4"))
                            {
                                uint? series4Rank =
                                    currentCharacter.PvPProfile.SeriesPersonalRanks.GetValueOrDefault((uint)4);
                                uint? series4Claimed =
                                    currentCharacter.PvPProfile.SeriesPersonalRanksClaimed.GetValueOrDefault(
                                        (uint)4);
                                ImGui.TextUnformatted(
                                    $"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 14860).ToUpper()} {(series4Rank.Value > 0 ? series4Rank.Value
                                            : _globalCache.AddonStorage.LoadAddonString(_currentLocale, 369)
                                        )}");

                                ImGui.TextUnformatted(
                                    $"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 16432)}");
                                using ImRaii.IEndObject t = ImRaii.Table("###Series#Rewards#4#Table", 5);
                                if (t)
                                {
                                    ImGui.TableSetupColumn("###Series#Rewards#4#Table#Reward#Emote",
                                        ImGuiTableColumnFlags.WidthFixed, 36);
                                    ImGui.TableSetupColumn("###Series#Rewards#4#Table#Reward#FramerKit",
                                        ImGuiTableColumnFlags.WidthFixed, 36);
                                    ImGui.TableSetupColumn($"###Series#Rewards#4#Table#Reward#Minion",
                                        ImGuiTableColumnFlags.WidthFixed, 36);
                                    ImGui.TableSetupColumn($"###Series#Rewards#4#Table#Reward#Framerkit2",
                                        ImGuiTableColumnFlags.WidthFixed, 36);
                                    ImGui.TableSetupColumn($"###Series#Rewards#4#Table#Reward#Mount",
                                        ImGuiTableColumnFlags.WidthFixed, 36);
                                    ImGui.TableNextRow();
                                    ImGui.TableSetColumnIndex(0);
                                    DrawEmote(263, currentCharacter.HasEmote(263));
                                    uint? fkId = _globalCache.FramerKitStorage.GetFramerKitIdFromItemId(40498);
                                    if (fkId == null) return;
                                    ImGui.TableSetColumnIndex(1);
                                    DrawFramerKit(fkId.Value, currentCharacter.HasFramerKit(fkId.Value));
                                    ImGui.TableSetColumnIndex(2);
                                    DrawMinion(482, currentCharacter.HasMinion(482));
                                    ImGui.TableSetColumnIndex(3);
                                    uint? fkId2 = _globalCache.FramerKitStorage.GetFramerKitIdFromItemId(40499);
                                    if (fkId2 == null) return;
                                    DrawFramerKit(fkId2.Value, currentCharacter.HasFramerKit(fkId2.Value));
                                    ImGui.TableSetColumnIndex(4);
                                    DrawMount(297, currentCharacter.HasMount(297));
                                }
                            }


                            ImGui.TableNextRow();
                            ImGui.TableSetColumnIndex(0);
                            if (ImGui.CollapsingHeader(
                                    $"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 14900)} 3###Series#Rewards#3"))
                            {
                                uint? series3Rank =
                                    currentCharacter.PvPProfile.SeriesPersonalRanks.GetValueOrDefault((uint)3);
                                uint? series3Claimed =
                                    currentCharacter.PvPProfile.SeriesPersonalRanksClaimed.GetValueOrDefault(
                                        (uint)3);
                                ImGui.TextUnformatted(
                                    $"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 14860).ToUpper()} {(series3Rank.Value > 0 ? series3Rank.Value
                                            : _globalCache.AddonStorage.LoadAddonString(_currentLocale, 369)
                                        )}");

                                ImGui.TextUnformatted(
                                    $"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 16432)}");
                                using ImRaii.IEndObject t = ImRaii.Table("###Series#Rewards#3#Table", 5);
                                if (t)
                                {
                                    ImGui.TableSetupColumn("###Series#Rewards#3#Table#Reward#Emote",
                                        ImGuiTableColumnFlags.WidthFixed, 36);
                                    ImGui.TableSetupColumn("###Series#Rewards#3#Table#Reward#FramerKit",
                                        ImGuiTableColumnFlags.WidthFixed, 36);
                                    ImGui.TableSetupColumn($"###Series#Rewards#3#Table#Reward#Minion",
                                        ImGuiTableColumnFlags.WidthFixed, 36);
                                    ImGui.TableSetupColumn($"###Series#Rewards#3#Table#Reward#Framerkit2",
                                        ImGuiTableColumnFlags.WidthFixed, 36);
                                    ImGui.TableSetupColumn($"###Series#Rewards#3#Table#Reward#Item",
                                        ImGuiTableColumnFlags.WidthFixed, 36);
                                    ImGui.TableNextRow();
                                    ImGui.TableSetColumnIndex(0);
                                    DrawEmote(251, currentCharacter.HasEmote(251));
                                    uint? fkId = _globalCache.FramerKitStorage.GetFramerKitIdFromItemId(39570);
                                    if (fkId == null) return;
                                    ImGui.TableSetColumnIndex(1);
                                    DrawFramerKit(fkId.Value, currentCharacter.HasFramerKit(fkId.Value));
                                    ImGui.TableSetColumnIndex(2);
                                    DrawMount(167, currentCharacter.HasMount(167));
                                    ImGui.TableSetColumnIndex(3);
                                    uint? fkId2 = _globalCache.FramerKitStorage.GetFramerKitIdFromItemId(39571);
                                    if (fkId2 == null) return;
                                    DrawFramerKit(fkId2.Value, currentCharacter.HasFramerKit(fkId2.Value));
                                    ImGui.TableSetColumnIndex(4);
                                    DrawItem(39364, 25, series3Claimed);
                                }
                            }


                            ImGui.TableNextRow();
                            ImGui.TableSetColumnIndex(0);
                            if (ImGui.CollapsingHeader(
                                    $"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 14900)} 2###Series#Rewards#2"))
                            {
                                uint? series2Rank =
                                    currentCharacter.PvPProfile.SeriesPersonalRanks.GetValueOrDefault((uint)2);
                                uint? series2Claimed =
                                    currentCharacter.PvPProfile.SeriesPersonalRanksClaimed.GetValueOrDefault(
                                        (uint)2);
                                ImGui.TextUnformatted(
                                    $"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 14860).ToUpper()} {(series2Rank.Value > 0 ? series2Rank.Value
                                            : _globalCache.AddonStorage.LoadAddonString(_currentLocale, 369)
                                        )}");

                                ImGui.TextUnformatted(
                                    $"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 16432)}");
                                using ImRaii.IEndObject t = ImRaii.Table("###Series#Rewards#2#Table", 6);
                                if (t)
                                {
                                    ImGui.TableSetupColumn("###Series#Rewards#2#Table#Reward#Emote",
                                        ImGuiTableColumnFlags.WidthFixed, 36);
                                    ImGui.TableSetupColumn("###Series#Rewards#2#Table#Reward#FramerKit",
                                        ImGuiTableColumnFlags.WidthFixed, 36);
                                    ImGui.TableSetupColumn($"###Series#Rewards#2#Table#Reward#Minion",
                                        ImGuiTableColumnFlags.WidthFixed, 36);
                                    ImGui.TableSetupColumn($"###Series#Rewards#2#Table#Reward#Minion2",
                                        ImGuiTableColumnFlags.WidthFixed, 36);
                                    ImGui.TableSetupColumn($"###Series#Rewards#2#Table#Reward#Framerkit2",
                                        ImGuiTableColumnFlags.WidthFixed, 36);
                                    ImGui.TableSetupColumn($"###Series#Rewards#2#Table#Reward#Mount",
                                        ImGuiTableColumnFlags.WidthFixed, 36);
                                    ImGui.TableNextRow();
                                    ImGui.TableSetColumnIndex(0);
                                    DrawEmote(248, currentCharacter.HasEmote(248));
                                    uint? fkId = _globalCache.FramerKitStorage.GetFramerKitIdFromItemId(38348);
                                    if (fkId == null) return;
                                    ImGui.TableSetColumnIndex(1);
                                    DrawFramerKit(fkId.Value, currentCharacter.HasFramerKit(fkId.Value));
                                    ImGui.TableSetColumnIndex(2);
                                    DrawMinion(459, currentCharacter.HasMinion(459));
                                    ImGui.TableSetColumnIndex(3);
                                    DrawMinion(458, currentCharacter.HasMinion(458));
                                    ImGui.TableSetColumnIndex(4);
                                    uint? fkId2 = _globalCache.FramerKitStorage.GetFramerKitIdFromItemId(38349);
                                    if (fkId2 == null) return;
                                    DrawFramerKit(fkId2.Value, currentCharacter.HasFramerKit(fkId2.Value));
                                    ImGui.TableSetColumnIndex(5);
                                    DrawMount(280, currentCharacter.HasMount(280));
                                }
                            }


                            ImGui.TableNextRow();
                            ImGui.TableSetColumnIndex(0);
                            if (ImGui.CollapsingHeader(
                                    $"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 14900)} 1###Series#Rewards#1"))
                            {
                                uint? series1Rank =
                                    currentCharacter.PvPProfile.SeriesPersonalRanks.GetValueOrDefault((uint)1);
                                uint? series1Claimed =
                                    currentCharacter.PvPProfile.SeriesPersonalRanksClaimed.GetValueOrDefault(
                                        (uint)1);
                                ImGui.TextUnformatted(
                                    $"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 14860).ToUpper()} {(series1Rank.Value > 0 ? series1Rank.Value
                                            : _globalCache.AddonStorage.LoadAddonString(_currentLocale, 369)
                                        )}");

                                ImGui.TextUnformatted(
                                    $"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 16432)}");
                                using ImRaii.IEndObject t = ImRaii.Table("###Series#Rewards#1#Table", 5);
                                if (t)
                                {
                                    ImGui.TableSetupColumn("###Series#Rewards#1#Table#Reward#Ornament",
                                        ImGuiTableColumnFlags.WidthFixed, 36);
                                    ImGui.TableSetupColumn("###Series#Rewards#1#Table#Reward#FramerKit",
                                        ImGuiTableColumnFlags.WidthFixed, 36);
                                    ImGui.TableSetupColumn($"###Series#Rewards#1#Table#Reward#Ornament",
                                        ImGuiTableColumnFlags.WidthFixed, 36);
                                    ImGui.TableSetupColumn($"###Series#Rewards#1#Table#Reward#Framerkit1",
                                        ImGuiTableColumnFlags.WidthFixed, 36);
                                    ImGui.TableSetupColumn($"###Series#Rewards#1#Table#Reward#Item",
                                        ImGuiTableColumnFlags.WidthFixed, 36);
                                    ImGui.TableNextRow();
                                    ImGui.TableSetColumnIndex(0);
                                    DrawEmote(240, currentCharacter.HasEmote(240));
                                    uint? fkId = _globalCache.FramerKitStorage.GetFramerKitIdFromItemId(37266);
                                    if (fkId == null) return;
                                    ImGui.TableSetColumnIndex(1);
                                    DrawFramerKit(fkId.Value, currentCharacter.HasFramerKit(fkId.Value));
                                    ImGui.TableSetColumnIndex(2);
                                    DrawFacewear(13, currentCharacter.HasGlasses(13));
                                    ImGui.TableSetColumnIndex(3);
                                    uint? fkId1 = _globalCache.FramerKitStorage.GetFramerKitIdFromItemId(37267);
                                    if (fkId1 == null) return;
                                    DrawFramerKit(fkId1.Value, currentCharacter.HasFramerKit(fkId1.Value));
                                    ImGui.TableSetColumnIndex(4);
                                    DrawItem(37268, 25, series1Claimed);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void DrawPvPExperience(uint totalExperience, uint currentRankRequiredExperience, uint nextRequiredExperience)
        {
            uint left = totalExperience - currentRankRequiredExperience;
            uint right = nextRequiredExperience - currentRankRequiredExperience;
            Utils.DrawPvPRankBar(left, right, 250, ImGuiColors.ParsedGreen, ImGuiColors.DalamudGrey3);
            using var pvpRankTable =
                ImRaii.Table("###pvpRankTable", 3, ImGuiTableFlags.None, new Vector2(300, 30));
            if (!pvpRankTable)
            {
                return;
            }

            ImGui.TableSetupColumn(
                $"###pvpRankTable#Col1",
                ImGuiTableColumnFlags.WidthFixed, 150);
            ImGui.TableSetupColumn(
                $"###pvpRankTable#Col2",
                ImGuiTableColumnFlags.WidthStretch);
            ImGui.TableSetupColumn(
                $"###pvpRankTable#Col2",
                ImGuiTableColumnFlags.WidthFixed, 150);
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            ImGui.TextUnformatted($"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 102455)}{totalExperience}");
            ImGui.TableSetColumnIndex(1);
            ImGui.TextUnformatted("");
            ImGui.TableSetColumnIndex(2);
            ImGui.TextUnformatted($"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 102456)} {left}/{right}");
        }
        private void DrawSerieExperience(PvPProfile pvPProfile)
        {
            ImGui.TextUnformatted($"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 14860).ToUpper()} {pvPProfile.SeriesCurrentRank}");
            PvPSeriesLevel? previousSerie = _globalCache.PvPStorage.GetSeriesExperience((uint)pvPProfile.SeriesCurrentRank - 1);
            if (previousSerie is null) return;
            PvPSeriesLevel? serie = _globalCache.PvPStorage.GetSeriesExperience(pvPProfile.SeriesCurrentRank);
            if (serie is null) return;
            //uint left = (uint)pvPProfile.SeriesExperience/* - previousSerie.Value.ExpToNext*/;
            uint left = pvPProfile.SeriesExperience;
            uint right = serie.Value.ExpToNext;
            Utils.DrawPvPRankBar(left, right, 250, ImGuiColors.ParsedPurple, ImGuiColors.DalamudGrey3);
            using ImRaii.IEndObject pvpRankTable =
                ImRaii.Table("###pvpSerieTable", 3, ImGuiTableFlags.None, new Vector2(300, 30));
            if (!pvpRankTable)
            {
                return;
            }

            ImGui.TableSetupColumn(
                $"###pvpSerieTable#Col1",
                ImGuiTableColumnFlags.WidthFixed, 150);
            ImGui.TableSetupColumn(
                $"###pvpSerieTable#Col2",
                ImGuiTableColumnFlags.WidthStretch);
            ImGui.TableSetupColumn(
                $"###pvpSerieTable#Col2",
                ImGuiTableColumnFlags.WidthFixed, 150);
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            ImGui.TextUnformatted("");
            ImGui.TableSetColumnIndex(1);
            ImGui.TextUnformatted("");
            ImGui.TableSetColumnIndex(2);
            ImGui.TextUnformatted($"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 102456)} {left}/{right}");
        }
        private string GetPercent(uint total, uint win)
        {
            return (total == 0) ? "0.0" : ((win /(double)total) * 100).ToString("0.0");
        }
        private string GetCystallineConflictRankName(byte rank)
        {
            return rank switch
            {
                1 => _globalCache.AddonStorage.LoadAddonString(_currentLocale, 14894),
                2 => _globalCache.AddonStorage.LoadAddonString(_currentLocale, 14895),
                3 => _globalCache.AddonStorage.LoadAddonString(_currentLocale, 14896),
                4 => _globalCache.AddonStorage.LoadAddonString(_currentLocale, 14897),
                5 => _globalCache.AddonStorage.LoadAddonString(_currentLocale, 14898),
                _ => ""
            };
        }
        private IDalamudTextureWrap? GetCystallineConflictRankEmblem(byte rank)
        {
            return rank switch
            {
                1 => _pvpRanksTextures[PvPRankEmblem.Bronze],
                2 => _pvpRanksTextures[PvPRankEmblem.Silver],
                3 => _pvpRanksTextures[PvPRankEmblem.Gold],
                4 => _pvpRanksTextures[PvPRankEmblem.Platinum],
                5 => _pvpRanksTextures[PvPRankEmblem.Diamond],
                6 => _pvpRanksTextures[PvPRankEmblem.Crystal],
                _ => null
            };
        }
        private string GetCystallineConflictStars(byte rank, byte stars)
        {
            return rank switch
            {
                0 or 1 => stars switch
                {
                    0 => "\u2606\u2606\u2606",
                    1 => "\u2605\u2606\u2606",
                    2 => "\u2605\u2605\u2606",
                    3 => "\u2605\u2605\u2605",
                    _ => "\u2606\u2606\u2606",
                },
                2 or 3 => stars switch
                {
                    0 => "\u2606\u2606\u2606\u2606",
                    1 => "\u2605\u2606\u2606\u2606",
                    2 => "\u2605\u2605\u2606\u2606",
                    3 => "\u2605\u2605\u2605\u2606",
                    4 => "\u2605\u2605\u2605\u2605",
                    _ => "\u2606\u2606\u2606",
                },
                4 => stars switch
                {
                    0 => "\u2606\u2606\u2606\u2606\u2606",
                    1 => "\u2605\u2606\u2606\u2606\u2606",
                    2 => "\u2605\u2605\u2606\u2606\u2606",
                    3 => "\u2605\u2605\u2605\u2605\u2606",
                    4 => "\u2605\u2605\u2605\u2605\u2605",
                    _ => "\u2606\u2606\u2606",
                },
                _ => ""
            };
        }

        private void DrawMinion(uint id, bool hasMinion)
        {
            Vector2 p = ImGui.GetCursorPos();
            Minion? m = _globalCache.MinionStorage.GetMinion(_currentLocale, id);
            if (m == null)
            {
                return;
            }

            if (!hasMinion)
            {
                if (_isSpoilerEnabled)
                {
                    Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(m.Icon), new Vector2(32, 32),
                        new Vector4(1, 1, 1, 0.5f));
                    if (ImGui.IsItemHovered())
                    {
                        Utils.DrawMinionTooltip(_currentLocale, ref _globalCache, m);
                    }
                }
                else
                {
                    Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(000786), new Vector2(32, 32));
                }
            }
            else
            {
                Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(m.Icon), new Vector2(32, 32));
                if (ImGui.IsItemHovered())
                {
                    Utils.DrawMinionTooltip(_currentLocale, ref _globalCache, m);
                }

                ImGui.SetCursorPos(new Vector2(p.X + 20, p.Y + 20));
                ImGui.PushFont(UiBuilder.IconFont);
                ImGui.TextUnformatted(FontAwesomeIcon.Check.ToIconString());
                ImGui.PopFont();
                ImGui.SetCursorPos(p);
            }
        }

        private void DrawMount(uint id, bool hasMount)
        {
            Vector2 p = ImGui.GetCursorPos();
            Mount? m = _globalCache.MountStorage.GetMount(_currentLocale, id);
            if (m == null)
            {
                return;
            }

            if (!hasMount)
            {
                if (_isSpoilerEnabled)
                {
                    Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(m.Icon), new Vector2(32, 32),
                        new Vector4(1, 1, 1, 0.5f));
                    if (ImGui.IsItemHovered())
                    {
                        Utils.DrawMountTooltip(_currentLocale, ref _globalCache, m);
                    }
                }
                else
                {
                    Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(000786), new Vector2(32, 32));
                }
            }
            else
            {
                Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(m.Icon), new Vector2(32, 32));
                if (ImGui.IsItemHovered())
                {
                    Utils.DrawMountTooltip(_currentLocale, ref _globalCache, m);
                }

                ImGui.SetCursorPos(new Vector2(p.X + 20, p.Y + 20));
                ImGui.PushFont(UiBuilder.IconFont);
                ImGui.TextUnformatted(FontAwesomeIcon.Check.ToIconString());
                ImGui.PopFont();
                ImGui.SetCursorPos(p);
            }
        }

        private void DrawEmote(uint id, bool hasEmote)
        {
            Vector2 p = ImGui.GetCursorPos();
            Emote? e = _globalCache.EmoteStorage.GetEmote(_currentLocale, id);
            if (e == null)
            {
                return;
            }

            if (!hasEmote)
            {
                if (_isSpoilerEnabled)
                {
                    Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(e.Icon), new Vector2(32, 32),
                        new Vector4(1, 1, 1, 0.5f));
                    if (ImGui.IsItemHovered())
                    {
                        Utils.DrawEmoteTooltip(_currentLocale, ref _globalCache, e);
                    }
                }
                else
                {
                    Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(000786), new Vector2(32, 32));
                }
            }
            else
            {
                Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(e.Icon), new Vector2(32, 32));
                if (ImGui.IsItemHovered())
                {
                    Utils.DrawEmoteTooltip(_currentLocale, ref _globalCache, e);
                }

                ImGui.SetCursorPos(new Vector2(p.X + 20, p.Y + 20));
                //ImGui.TextUnformatted("\u2713");
                ImGui.PushFont(UiBuilder.IconFont);
                ImGui.TextUnformatted(FontAwesomeIcon.Check.ToIconString());
                ImGui.PopFont();
                ImGui.SetCursorPos(p);
            }
        }

        private void DrawOrchestrion(uint id, bool hasOrchestrion)
        {
            Vector2 p = ImGui.GetCursorPos();
            OrchestrionRoll? o = _globalCache.OrchestrionRollStorage.GetOrchestrionRoll(_currentLocale, id);
            if (o == null)
            {
                return;
            }

            if (!hasOrchestrion)
            {
                if (_isSpoilerEnabled)
                {
                    Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(o.Icon), new Vector2(32, 32),
                        new Vector4(1, 1, 1, 0.5f));
                    if (ImGui.IsItemHovered())
                    {
                        Utils.DrawOrchestrionRollTooltip(_currentLocale, ref _globalCache, o);
                    }
                }
                else
                {
                    Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(000786), new Vector2(32, 32));
                }
            }
            else
            {
                Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(o.Icon), new Vector2(32, 32));
                if (ImGui.IsItemHovered())
                {
                    Utils.DrawOrchestrionRollTooltip(_currentLocale, ref _globalCache, o);
                }

                ImGui.SetCursorPos(new Vector2(p.X + 20, p.Y + 20));
                //ImGui.TextUnformatted("\u2713");
                ImGui.PushFont(UiBuilder.IconFont);
                ImGui.TextUnformatted(FontAwesomeIcon.Check.ToIconString());
                ImGui.PopFont();
                ImGui.SetCursorPos(p);
            }
        }

        private void DrawFramerKit(uint id, bool hasFramerKit)
        {
            Vector2 p = ImGui.GetCursorPos();
            FramerKit? f = _globalCache.FramerKitStorage.LoadItem(_currentLocale, id);
            if (f == null)
            {
                return;
            }
            //Plugin.Log.Debug($"fk: {f.Id} name:{f.EnglishName}, icon:{f.Icon} itemId:{f.ItemId}");
            if (!hasFramerKit)
            {
                if (_isSpoilerEnabled)
                {
                    Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(f.Icon), new Vector2(32, 32),
                        new Vector4(1, 1, 1, 0.5f));
                    if (ImGui.IsItemHovered())
                    {
                        Utils.DrawFramerKitTooltip(_currentLocale, ref _globalCache, f);
                    }
                }
                else
                {
                    Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(000786), new Vector2(32, 32));
                }
            }
            else
            {
                Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(f.Icon), new Vector2(32, 32));
                if (ImGui.IsItemHovered())
                {
                    Utils.DrawFramerKitTooltip(_currentLocale, ref _globalCache, f);
                }

                ImGui.SetCursorPos(new Vector2(p.X + 20, p.Y + 20));
                //ImGui.TextUnformatted("\u2713");
                ImGui.PushFont(UiBuilder.IconFont);
                ImGui.TextUnformatted(FontAwesomeIcon.Check.ToIconString());
                ImGui.PopFont();
                ImGui.SetCursorPos(p);
            }
        }

        private void DrawTripleTriadCard(uint id, bool hasTripleTriadCard)
        {
            Vector2 p = ImGui.GetCursorPos();
            TripleTriadCard? t = _globalCache.TripleTriadCardStorage.GetTripleTriadCard(_currentLocale, id);
            if (t == null)
            {
                return;
            }
            //Plugin.Log.Debug($"fk: {f.Id} name:{f.EnglishName}, icon:{f.Icon} itemId:{f.ItemId}");
            if (!hasTripleTriadCard)
            {
                if (_isSpoilerEnabled)
                {
                    //Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(t.Icon), new Vector2(32, 32),
                    Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(027672), new Vector2(32, 32),
                        new Vector4(1, 1, 1, 0.5f));
                    if (ImGui.IsItemHovered())
                    {
                        Utils.DrawTTCTooltip(_currentLocale, ref _globalCache, t);
                    }
                }
                else
                {
                    Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(000786), new Vector2(32, 32));
                }
            }
            else
            {
                //Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(t.Icon), new Vector2(32, 32));
                Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(027672), new Vector2(32, 32));
                if (ImGui.IsItemHovered())
                {
                    Utils.DrawTTCTooltip(_currentLocale, ref _globalCache, t);
                }

                ImGui.SetCursorPos(new Vector2(p.X + 20, p.Y + 20));
                //ImGui.TextUnformatted("\u2713");
                ImGui.PushFont(UiBuilder.IconFont);
                ImGui.TextUnformatted(FontAwesomeIcon.Check.ToIconString());
                ImGui.PopFont();
                ImGui.SetCursorPos(p);
            }
        }

        private void DrawOrnament(uint id, bool hasOrnament)
        {
            Vector2 p = ImGui.GetCursorPos();
            Ornament? o = _globalCache.OrnamentStorage.GetOrnament(_currentLocale, id);
            if (o == null)
            {
                return;
            }

            if (!hasOrnament)
            {
                if (_isSpoilerEnabled)
                {
                    Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(o.Icon), new Vector2(32, 32),
                        new Vector4(1, 1, 1, 0.5f));
                    if (ImGui.IsItemHovered())
                    {
                        Utils.DrawOrnamentTooltip(_currentLocale, ref _globalCache, o);
                    }
                }
                else
                {
                    Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(000786), new Vector2(32, 32));
                }
            }
            else
            {
                Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(o.Icon), new Vector2(32, 32));
                if (ImGui.IsItemHovered())
                {
                    Utils.DrawOrnamentTooltip(_currentLocale, ref _globalCache, o);
                }

                ImGui.SetCursorPos(new Vector2(p.X + 20, p.Y + 20));
                //ImGui.TextUnformatted("\u2713");
                ImGui.PushFont(UiBuilder.IconFont);
                ImGui.TextUnformatted(FontAwesomeIcon.Check.ToIconString());
                ImGui.PopFont();
                ImGui.SetCursorPos(p);
            }
        }
        private void DrawFacewear(uint id, bool hasFacewear)
        {
            Vector2 p = ImGui.GetCursorPos();
            Models.Glasses? g = _globalCache.GlassesStorage.GetGlasses(_currentLocale, id);
            if (g == null)
            {
                return;
            }

            if (!hasFacewear)
            {
                if (_isSpoilerEnabled)
                {
                    Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(g.Icon), new Vector2(32, 32),
                        new Vector4(1, 1, 1, 0.5f));
                    if (ImGui.IsItemHovered())
                    {
                        Utils.DrawGlassesTooltip(_currentLocale, ref _globalCache, g);
                    }
                }
                else
                {
                    Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(000786), new Vector2(32, 32));
                }
            }
            else
            {
                Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(g.Icon), new Vector2(32, 32));
                if (ImGui.IsItemHovered())
                {
                    Utils.DrawGlassesTooltip(_currentLocale, ref _globalCache, g);
                }

                ImGui.SetCursorPos(new Vector2(p.X + 20, p.Y + 20));
                //ImGui.TextUnformatted("\u2713");
                ImGui.PushFont(UiBuilder.IconFont);
                ImGui.TextUnformatted(FontAwesomeIcon.Check.ToIconString());
                ImGui.PopFont();
                ImGui.SetCursorPos(p);
            }
        }

        private void DrawItem(uint id, uint requiredRank, uint? claimedRank)
        {
            bool? hasUnlocked = (claimedRank is null) ? null : claimedRank > requiredRank;
            Vector2 p = ImGui.GetCursorPos();
            Item? i = _globalCache.ItemStorage.LoadItem(_currentLocale, id);
            if (i == null)
            {
                return;
            }

            if (!hasUnlocked != null && hasUnlocked == false)
            {
                if (_isSpoilerEnabled)
                {
                    Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(i.Value.Icon), new Vector2(32, 32),
                        new Vector4(1, 1, 1, 0.5f));
                    if (ImGui.IsItemHovered())
                    {
                        Utils.DrawItemTooltip(_currentLocale, ref _globalCache, i.Value);
                    }
                }
                else
                {
                    Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(000786), new Vector2(32, 32));
                }
            }
            else
            {
                Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(i.Value.Icon), new Vector2(32, 32));
                if (ImGui.IsItemHovered())
                {
                    Utils.DrawItemTooltip(_currentLocale, ref _globalCache, i.Value);
                }

                ImGui.SetCursorPos(new Vector2(p.X + 20, p.Y + 20));
                ImGui.PushFont(UiBuilder.IconFont);
                ImGui.TextUnformatted(hasUnlocked is null
                    ? FontAwesomeIcon.Check.ToIconString()
                    : FontAwesomeIcon.Question.ToIconString());
                ImGui.PopFont();
                ImGui.SetCursorPos(p);
            }
        }

    }
}