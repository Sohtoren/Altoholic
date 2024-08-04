using Altoholic.Cache;
using Altoholic.Models;
using Dalamud.Game;
using Dalamud.Game.Text;
using Dalamud.Interface.Textures.TextureWraps;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Windowing;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Altoholic.Windows
{
    public class ProgressWindow : Window, IDisposable
    {
        private readonly Plugin _plugin;
        private ClientLanguage _currentLocale;
        private readonly GlobalCache _globalCache;

        public ProgressWindow(
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

            //_rolesIcons = Plugin.PluginInterface.UiBuilder.LoadUld("ui/uld/Retainer.uld");
            //Plugin.Log.Debug($"Type: {rolesIcons.Uld?.FileInfo.Type}");
            //rolesTextures.Add(Role.Tank, rolesIcons.LoadTexturePart("ui/uld/fourth/ToggleButton_hr1.tex", 4));
            //_rolesTextureWrap = Plugin.TextureProvider.GetFromGame("ui/uld/Retainer_hr1.tex").RentAsync().Result;

            _commendationIcon = Plugin.TextureProvider.GetFromGame("ui/uld/Character_hr1.tex").RentAsync().Result;
            _chevronTexture = Plugin.TextureProvider.GetFromGame("ui/uld/fourth/ListItemB_hr1.tex").RentAsync().Result;

            _currentLocale = _plugin.Configuration.Language;
            _selectedExpansion = _globalCache.AddonStorage.LoadAddonString(_currentLocale, 5752);
        }

        public Func<Character> GetPlayer { get; set; } = null!;
        public Func<List<Character>> GetOthersCharactersList { get; set; } = null!;
        private Character? _currentCharacter;

        private string _selectedExpansion;

        private readonly IDalamudTextureWrap? _commendationIcon;
        private readonly IDalamudTextureWrap? _chevronTexture;

        private bool _rightChevron = true;
        private bool _downChevron;

        public override void OnClose()
        {
        }

        public void Dispose()
        {
            _currentCharacter = null;
            _selectedExpansion = string.Empty;
            _commendationIcon?.Dispose();
            _chevronTexture?.Dispose();
        }

        public void Clear()
        {
            _currentCharacter = null;
            _selectedExpansion = _globalCache.AddonStorage.LoadAddonString(_currentLocale, 5752);
        }

        public override void Draw()
        {
            _currentLocale = _plugin.Configuration.Language;
            List<Character> chars = [];
            chars.Insert(0, GetPlayer.Invoke());
            chars.AddRange(GetOthersCharactersList.Invoke());

            try
            {
                using ImRaii.IEndObject table = ImRaii.Table("###CharactersProgressTable", 2);
                if (!table) return;

                ImGui.TableSetupColumn("###CharactersProgressTable#CharactersListHeader",
                    ImGuiTableColumnFlags.WidthFixed, 210);
                ImGui.TableSetupColumn("###CharactersProgressTable#ProgressTabs", ImGuiTableColumnFlags.WidthStretch);
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                using (ImRaii.IEndObject listBox =
                       ImRaii.ListBox("###CharactersProgressTable#CharactersListBox", new Vector2(200, -1)))
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
                    DrawTabs(_currentCharacter);
                }
            }
            catch (Exception e)
            {
                Plugin.Log.Debug("Altoholic CharactersInventoryTable Exception : {0}", e);
            }
        }

        public void DrawTabs(Character selectedCharacter)
        {
            using var tab = ImRaii.TabBar($"###CharactersProgressTable#ProgressTabs#{selectedCharacter.Id}#TabBar");
            if (!tab) return; 
            /*using (var msqTab = ImRaii.TabItem($"MSQ###Progress#Tabs#{selectedCharacter.Id}#TabBar#MSQ"))
            {
                if (msqTab)
                {

                }
            }

            using (var eventTab =
                   ImRaii.TabItem(
                       $"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 665)}###Progress#Tabs#{selectedCharacter.Id}#TabBar#Events"))
            {
                if (eventTab)
                {

                }
            }*/

            using (ImRaii.IEndObject reputationTab =
                   ImRaii.TabItem(
                       $"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 102512)}###CharactersProgressTable#ProgressTabs#{selectedCharacter.Id}#TabBar#Tabs#Reputation"))
            {
                if (reputationTab.Success)
                {
                    DrawReputations(selectedCharacter);
                }
            }
        }

        private void DrawReputations(Character selectedCharacter)
        {
            ImGui.TextUnformatted($"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 102513)}");
            ImGui.Separator();
            if (_commendationIcon != null)
            {
                (Vector2 uv0, Vector2 uv1) = Utils.GetTextureCoordinate(_commendationIcon.Size, 320, 208, 64, 64);
                ImGui.Image(_commendationIcon.ImGuiHandle, new Vector2(32, 32), uv0, uv1);
            }

            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                ImGui.TextUnformatted($"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 102520)}");
                ImGui.EndTooltip();
            }

            ImGui.SameLine();
            ImGui.TextUnformatted($"{selectedCharacter.PlayerCommendations}");

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

            ImGui.TextUnformatted($"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 102515)}");
            ImGui.Separator();
            if (_chevronTexture != null)
            {
                if (_rightChevron)
                {
                    _downChevron = false;
                    (Vector2 uv0, Vector2 uv1) = Utils.GetTextureCoordinate(_chevronTexture.Size, 0, 0, 48, 48);
                    ImGui.Image(_chevronTexture.ImGuiHandle, new Vector2(24, 24), uv0, uv1);
                }

                if (_downChevron)
                {
                    _rightChevron = false;
                    (Vector2 uv0, Vector2 uv1) = Utils.GetTextureCoordinate(_chevronTexture.Size, 48, 0, 48, 48);
                    ImGui.Image(_chevronTexture.ImGuiHandle, new Vector2(24, 24), uv0, uv1);
                }
            }

            ImGui.SameLine();
            bool arrUnlocked = false;
            bool hwUnlocked = false;
            bool sbUnlocked = false;
            bool shbUnlocked = false;
            bool ewUnlocked = false;
            List<string> names = [];
            if (selectedCharacter.IsQuestCompleted(66754) ||
                selectedCharacter.IsQuestCompleted(66789) ||
                selectedCharacter.IsQuestCompleted(66857) ||
                selectedCharacter.IsQuestCompleted(66911) ||
                selectedCharacter.IsQuestCompleted(67023)
               )
            {
                names.Add(_globalCache.AddonStorage.LoadAddonString(_currentLocale, 5752));
                arrUnlocked = true;
            }

            if (selectedCharacter.IsQuestCompleted(67700) ||
                selectedCharacter.IsQuestCompleted(67791) ||
                selectedCharacter.IsQuestCompleted(67856)
               )
            {
                names.Add(_globalCache.AddonStorage.LoadAddonString(_currentLocale, 5753));
                hwUnlocked = true;

                if (!arrUnlocked)
                    _selectedExpansion = _globalCache.AddonStorage.LoadAddonString(_currentLocale, 5753);
            }

            if (selectedCharacter.IsQuestCompleted(68509) ||
                selectedCharacter.IsQuestCompleted(68572) ||
                selectedCharacter.IsQuestCompleted(68633)
               )
            {
                names.Add(_globalCache.AddonStorage.LoadAddonString(_currentLocale, 5754));
                sbUnlocked = true;

                if (!hwUnlocked)
                    _selectedExpansion = _globalCache.AddonStorage.LoadAddonString(_currentLocale, 5754);
            }

            if (selectedCharacter.IsQuestCompleted(69219) ||
                selectedCharacter.IsQuestCompleted(69330) ||
                selectedCharacter.IsQuestCompleted(69432)
               )
            {
                shbUnlocked = true;
                names.Add(_globalCache.AddonStorage.LoadAddonString(_currentLocale, 8156));
                if (!sbUnlocked)
                    _selectedExpansion = _globalCache.AddonStorage.LoadAddonString(_currentLocale, 8156);
            }

            if (selectedCharacter.IsQuestCompleted(70081) ||
                selectedCharacter.IsQuestCompleted(70137) ||
                selectedCharacter.IsQuestCompleted(70217)
            )
            {
                names.Add(_globalCache.AddonStorage.LoadAddonString(_currentLocale, 8160));
                ewUnlocked = true;
                if (!shbUnlocked) 
                    _selectedExpansion = _globalCache.AddonStorage.LoadAddonString(_currentLocale, 8160);
            }

            /*
             if (selectedCharacter.IsQuestCompleted() &&
                   selectedCharacter.IsQuestCompleted() &&
                   selectedCharacter.IsQuestCompleted())
               {
                   names.Add(_globalCache.AddonStorage.LoadAddonString(_currentLocale, 8175));//DT
                    dtUnlocked = true;
                    if (!ewUnlocked) 
                        _selectedExpansion = _globalCache.AddonStorage.LoadAddonString(_currentLocale, 8175)
             */

            using (var combo = ImRaii.Combo("###CharactersProgress#Reputations#Combo", _selectedExpansion))
            {
                if (combo)
                {
                    foreach (string name in names.Where(name => ImGui.Selectable(name, name == _selectedExpansion)))
                    {
                        _selectedExpansion = name;
                        _rightChevron = true;
                        _downChevron = false;
                    }
                }
            }
            if (ImGui.IsItemClicked())
            {
                _rightChevron = false;
                _downChevron = true;
            }

            using ImRaii.IEndObject charactersReputationTable = ImRaii.Table("###CharactersProgress#Reputations##Reputation", 1, ImGuiTableFlags.ScrollY);
            if (!charactersReputationTable) return;
            ImGui.TableSetupColumn("###CharactersProgress#ReputationsTable", ImGuiTableColumnFlags.WidthStretch);
            switch (_selectedExpansion)
            {
                case "A Realm Reborn":
                case "新生エオルゼア":
                    {
                        bool arrAllied = selectedCharacter.IsQuestCompleted(70324);
                        for (uint i = 1; i <= 5; i++)
                        {
                            if (
                                i == 1 && !selectedCharacter.IsQuestCompleted(66754) ||
                                i == 2 && !selectedCharacter.IsQuestCompleted(66789) ||
                                i == 3 && !selectedCharacter.IsQuestCompleted(66857) ||
                                i == 4 && !selectedCharacter.IsQuestCompleted(66911) ||
                                i == 5 && !selectedCharacter.IsQuestCompleted(67023)
                            )
                            {
                                continue;
                            }

                            BeastTribeRank? b = selectedCharacter.GetBeastReputation(i);
                            if (b == null)
                            {
                                continue;
                            }

                            ImGui.TableNextRow();
                            ImGui.TableSetColumnIndex(0);
                            DrawReputationLine(i, b.Value, b.Rank, arrAllied);
                        }
                    }
                    break;
                case "Heavensward":
                case "蒼天のイシュガルド":
                    {
                        bool hwAllied = selectedCharacter.IsQuestCompleted(67921);
                        for (uint i = 6; i <= 8; i++)
                        {
                            if (
                                i == 6 && !selectedCharacter.IsQuestCompleted(67700) ||
                                i == 7 && !selectedCharacter.IsQuestCompleted(67791) ||
                                i == 8 && !selectedCharacter.IsQuestCompleted(67856)
                            )
                            {
                                continue;
                            }

                            BeastTribeRank? b = selectedCharacter.GetBeastReputation(i);
                            if (b == null)
                            {
                                continue;
                            }

                            ImGui.TableNextRow();
                            ImGui.TableSetColumnIndex(0);
                            DrawReputationLine(i, b.Value, b.Rank, hwAllied);
                        }
                    }
                    break;
                case "Stormblood":
                case "紅蓮のリベレーター":
                    {
                        bool sbAllied = selectedCharacter.IsQuestCompleted(68700);
                        for (uint i = 9; i <= 11; i++)
                        {
                            if (
                                i == 9 && !selectedCharacter.IsQuestCompleted(68509) ||
                                i == 10 && !selectedCharacter.IsQuestCompleted(68572) ||
                                i == 11 && !selectedCharacter.IsQuestCompleted(68633)
                            )
                            {
                                continue;
                            }

                            BeastTribeRank? b = selectedCharacter.GetBeastReputation(i);
                            if (b == null)
                            {
                                continue;
                            }

                            ImGui.TableNextRow();
                            ImGui.TableSetColumnIndex(0);
                            DrawReputationLine(i, b.Value, b.Rank, sbAllied);
                        }
                    }
                    break;
                case "Shadowbringers":
                case "漆黒編":
                    {
                        for (uint i = 12; i <= 14; i++)
                        {
                            if (
                                i == 12 && !selectedCharacter.IsQuestCompleted(69219) ||
                                i == 13 && !selectedCharacter.IsQuestCompleted(69330) ||
                                i == 14 && !selectedCharacter.IsQuestCompleted(69432)
                            )
                            {
                                continue;
                            }

                            BeastTribeRank? b = selectedCharacter.GetBeastReputation(i);
                            if (b == null)
                            {
                                continue;
                            }

                            ImGui.TableNextRow();
                            ImGui.TableSetColumnIndex(0);
                            DrawReputationLine(i, b.Value, b.Rank, false);
                        }
                    }
                    break;
                case "Endwalker":
                case "暁月編":
                    {
                        bool ewAllied = selectedCharacter.IsQuestCompleted(70324);
                        for (uint i = 15; i <= 17; i++)
                        {
                            if (
                                i == 15 && !selectedCharacter.IsQuestCompleted(70081) ||
                                i == 16 && !selectedCharacter.IsQuestCompleted(70137) ||
                                i == 17 && !selectedCharacter.IsQuestCompleted(70217)
                            )
                            {
                                continue;
                            }

                            BeastTribeRank? b = selectedCharacter.GetBeastReputation(i);
                            if (b == null)
                            {
                                continue;
                            }

                            ImGui.TableNextRow();
                            ImGui.TableSetColumnIndex(0);
                            DrawReputationLine(i, b.Value, b.Rank, ewAllied);
                        }
                    }
                    break;
                case "Dawntrail":
                case "黄金編":
                    {

                    }
                    break;
            }
        }

        private void DrawReputationLine(uint id, uint currentExp, uint rank, bool isAllied)
        {
            BeastTribes? beastTribe = _globalCache.BeastTribesStorage.GetBeastTribe(_currentLocale, id);
            if (beastTribe == null) return;

            using (var charactersReputationsReputationLine = ImRaii.Table($"###CharactersProgress#Reputations##Reputation#ReputationLine#{id}", 2))
            {
                if (!charactersReputationsReputationLine) return;
                ImGui.TableSetupColumn($"###CharactersProgress#Reputations##Reputation#ReputationLine#{id}Icon", ImGuiTableColumnFlags.WidthFixed, 36);
                ImGui.TableSetupColumn($"###CharactersProgress#Reputations##Reputation#ReputationLine#{id}#RankNameExp", ImGuiTableColumnFlags.WidthStretch);
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(beastTribe.Icon), new Vector2(36, 36));
                ImGui.TableSetColumnIndex(1);
                string name = _currentLocale switch
                {
                    ClientLanguage.German => beastTribe.GermanName,
                    ClientLanguage.English => beastTribe.EnglishName,
                    ClientLanguage.French => beastTribe.FrenchName,
                    ClientLanguage.Japanese => beastTribe.JapaneseName,
                    _ => beastTribe.EnglishName
                };
                ImGui.TextUnformatted($"{Utils.Capitalize(name)}");
            }

            Utils.DrawReputationProgressBar(_currentLocale, _globalCache, currentExp, beastTribe.MaxRank == rank, rank, isAllied);
        }
    }
}
