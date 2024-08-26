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
        private GlobalCache _globalCache;
        private bool _isSpoilerEnabled;

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
        private bool _hasValueBeenSelected;

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
            Plugin.Log.Debug("ProgressWindow Clear() called");
            _currentCharacter = null;
            _selectedExpansion = _globalCache.AddonStorage.LoadAddonString(_currentLocale, 5752);
        }

        public override void Draw()
        {
            _currentLocale = _plugin.Configuration.Language;
            _isSpoilerEnabled = _plugin.Configuration.IsSpoilersEnabled;
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
                Plugin.Log.Debug("Altoholic CharactersProgressTable Exception : {0}", e);
            }
        }

        public void DrawTabs(Character selectedCharacter)
        {
            using var tab = ImRaii.TabBar($"###CharactersProgressTable#ProgressTabs#{selectedCharacter.CharacterId}#TabBar");
            if (!tab) return; 
            /*using (var msqTab = ImRaii.TabItem($"MSQ###Progress#Tabs#{selectedCharacter.CharacterId}#TabBar#MSQ"))
            {
                if (msqTab)
                {

                }
            }

            using (var eventTab =
                   ImRaii.TabItem(
                       $"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 665)}###Progress#Tabs#{selectedCharacter.CharacterId}#TabBar#Events"))
            {
                if (eventTab)
                {

                }
            }*/

            using (ImRaii.IEndObject reputationTab =
                   ImRaii.TabItem(
                       $"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 102512)}###CharactersProgressTable#ProgressTabs#{selectedCharacter.CharacterId}#TabBar#Tabs#Reputation"))
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

            if (!selectedCharacter.HasQuest(66754) &&
                !selectedCharacter.HasQuest(66789) &&
                !selectedCharacter.HasQuest(66857) &&
                !selectedCharacter.HasQuest(66911) &&
                !selectedCharacter.HasQuest(67023) &&
                !selectedCharacter.HasQuest(67700) &&
                !selectedCharacter.HasQuest(67700) &&
                !selectedCharacter.HasQuest(67791) &&
                !selectedCharacter.HasQuest(67856) &&
                !selectedCharacter.HasQuest(68509) &&
                !selectedCharacter.HasQuest(68572) &&
                !selectedCharacter.HasQuest(68633) &&
                !selectedCharacter.HasQuest(69219) &&
                !selectedCharacter.HasQuest(69330) &&
                !selectedCharacter.HasQuest(69432) &&
                !selectedCharacter.HasQuest(70081) &&
                !selectedCharacter.HasQuest(70137) &&
                !selectedCharacter.HasQuest(70217))
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
            if (selectedCharacter.HasQuest(66754) ||
                selectedCharacter.HasQuest(66789) ||
                selectedCharacter.HasQuest(66857) ||
                selectedCharacter.HasQuest(66911) ||
                selectedCharacter.HasQuest(67023)
               )
            {
                names.Add(_globalCache.AddonStorage.LoadAddonString(_currentLocale, 5752));
                arrUnlocked = true;
            }

            if (selectedCharacter.HasQuest(67700) ||
                selectedCharacter.HasQuest(67791) ||
                selectedCharacter.HasQuest(67856)
               )
            {
                names.Add(_globalCache.AddonStorage.LoadAddonString(_currentLocale, 5753));
                hwUnlocked = true;

                if (!_hasValueBeenSelected && !arrUnlocked)
                    _selectedExpansion = _globalCache.AddonStorage.LoadAddonString(_currentLocale, 5753);
            }

            if (selectedCharacter.HasQuest(68509) ||
                selectedCharacter.HasQuest(68572) ||
                selectedCharacter.HasQuest(68633)
               )
            {
                names.Add(_globalCache.AddonStorage.LoadAddonString(_currentLocale, 5754));
                sbUnlocked = true;

                if (!_hasValueBeenSelected && !hwUnlocked)
                    _selectedExpansion = _globalCache.AddonStorage.LoadAddonString(_currentLocale, 5754);
            }

            if (selectedCharacter.HasQuest(69219) ||
                selectedCharacter.HasQuest(69330) ||
                selectedCharacter.HasQuest(69432)
               )
            {
                shbUnlocked = true;
                names.Add(_globalCache.AddonStorage.LoadAddonString(_currentLocale, 8156));
                if (!_hasValueBeenSelected && !sbUnlocked)
                    _selectedExpansion = _globalCache.AddonStorage.LoadAddonString(_currentLocale, 8156);
            }

            if (selectedCharacter.HasQuest(70081) ||
                selectedCharacter.HasQuest(70137) ||
                selectedCharacter.HasQuest(70217)
               )
            {
                names.Add(_globalCache.AddonStorage.LoadAddonString(_currentLocale, 8160));
                ewUnlocked = true;
                if (!_hasValueBeenSelected && !shbUnlocked)
                    _selectedExpansion = _globalCache.AddonStorage.LoadAddonString(_currentLocale, 8160);
            }


            /*
             if (selectedCharacter.HasQuest() &&
                   selectedCharacter.HasQuest() &&
                   selectedCharacter.HasQuest())
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
                        _hasValueBeenSelected = true;
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
            ImGui.TableSetupColumn("###CharactersProgress#ReputationsTable", ImGuiTableColumnFlags.WidthFixed, 560);
            switch (_selectedExpansion)
            {
                case "A Realm Reborn":
                case "新生エオルゼア":
                    {
                        bool arrAllied = selectedCharacter.HasQuest(70324);
                        for (uint i = 1; i <= 5; i++)
                        {
                            if (
                                i == 1 && !selectedCharacter.HasQuest(66754) ||
                                i == 2 && !selectedCharacter.HasQuest(66789) ||
                                i == 3 && !selectedCharacter.HasQuest(66857) ||
                                i == 4 && !selectedCharacter.HasQuest(66911) ||
                                i == 5 && !selectedCharacter.HasQuest(67023)
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
                            DrawReward(selectedCharacter, i, b.Rank, arrAllied);
                        }
                    }
                    break;
                case "Heavensward":
                case "蒼天のイシュガルド":
                    {
                        bool hwAllied = selectedCharacter.HasQuest(67921);
                        for (uint i = 6; i <= 8; i++)
                        {
                            if (
                                i == 6 && !selectedCharacter.HasQuest(67700) ||
                                i == 7 && !selectedCharacter.HasQuest(67791) ||
                                i == 8 && !selectedCharacter.HasQuest(67856)
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
                            DrawReward(selectedCharacter, i, b.Rank, hwAllied);
                        }
                    }
                    break;
                case "Stormblood":
                case "紅蓮のリベレーター":
                    {
                        bool sbAllied = selectedCharacter.HasQuest(68700);
                        for (uint i = 9; i <= 11; i++)
                        {
                            if (
                                i == 9 && !selectedCharacter.HasQuest(68509) ||
                                i == 10 && !selectedCharacter.HasQuest(68572) ||
                                i == 11 && !selectedCharacter.HasQuest(68633)
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
                            DrawReward(selectedCharacter, i, b.Rank, sbAllied);
                        }
                    }
                    break;
                case "Shadowbringers":
                case "漆黒編":
                    {
                        for (uint i = 12; i <= 14; i++)
                        {
                            if (
                                i == 12 && !selectedCharacter.HasQuest(69219) ||
                                i == 13 && !selectedCharacter.HasQuest(69330) ||
                                i == 14 && !selectedCharacter.HasQuest(69432)
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
                            DrawReward(selectedCharacter, i, b.Rank, false);
                        }
                    }
                    break;
                case "Endwalker":
                case "暁月編":
                    {
                        bool ewAllied = selectedCharacter.HasQuest(70324);
                        for (uint i = 15; i <= 17; i++)
                        {
                            if (
                                i == 15 && !selectedCharacter.HasQuest(70081) ||
                                i == 16 && !selectedCharacter.HasQuest(70137) ||
                                i == 17 && !selectedCharacter.HasQuest(70217)
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
                            DrawReward(selectedCharacter, i, b.Rank, ewAllied);
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
            //Plugin.Log.Debug($"id: {id}, currentExp: {currentExp}, rank: {rank}, isAllied: {isAllied}");
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

            // Todo: if spoiler enabled, add a unique reward list (orchestrions, pets, framerkits) here and check it if brought/unlocked.
            // If spoiler is disabled, unveil each reward by reputation rank
        }

        private void DrawReward(Character currentCharacter, uint id, uint rank, bool isAllied)
        {
            if (rank < 3) return;
            //Plugin.Log.Debug($"DrawReward: id={id}, rank={rank}, isAllied={isAllied}");
            switch (id)
            {
                case 1: //Amaal'ja
                    {
                        if (ImGui.CollapsingHeader(
                                $"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 11429)}###Progress#BeastReputations#{id}#Reward"))
                        {
                            using var t = ImRaii.Table($"###Progress#BeastReputations#{id}#Reward#Table", 5);
                            if (!t) break;
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#Orchestrion1",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#FramerKit",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#Minion1",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#Mount1",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#Minion2",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableNextRow();
                            ImGui.TableSetColumnIndex(0);
                            DrawOrchestrion(85, currentCharacter.HasOrchestrionRoll(85));
                            ImGui.TableSetColumnIndex(1);
                            uint? fkId = _globalCache.FramerKitStorage.GetFramerKitIdFromItemId(43957);
                            if (fkId == null) return;
                            DrawFramerKit(fkId.Value, currentCharacter.HasFramerKit(fkId.Value));

                            if (rank >= 4)
                            {
                                ImGui.TableSetColumnIndex(2);
                                DrawMinion(58, currentCharacter.HasMinion(58));
                                ImGui.TableSetColumnIndex(3);
                                DrawMount(19, currentCharacter.HasMount(19));
                            }

                            if (isAllied)
                            {
                                ImGui.TableSetColumnIndex(4);
                                DrawMinion(124, currentCharacter.HasMinion(124));
                            }
                        }

                        break;
                    }
                case 2: //Sylph
                    {
                        if (ImGui.CollapsingHeader(
                                $"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 11429)}###Progress#BeastReputations#{id}#Reward"))
                        {
                            using var t = ImRaii.Table($"###Progress#BeastReputations#{id}#Reward#Table", 5);
                            if (!t) break;
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#Orchestrion1",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#FramerKit",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#Minion1",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#Mount1",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#Minion2",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableNextRow();
                            ImGui.TableSetColumnIndex(0);
                            DrawOrchestrion(117, currentCharacter.HasOrchestrionRoll(117));
                            ImGui.TableSetColumnIndex(1);
                            uint? fkId = _globalCache.FramerKitStorage.GetFramerKitIdFromItemId(43956);
                            if (fkId == null) return;
                            DrawFramerKit(fkId.Value, currentCharacter.HasFramerKit(fkId.Value));

                            if (rank >= 4)
                            {
                                ImGui.TableSetColumnIndex(2);
                                DrawMinion(50, currentCharacter.HasMinion(50));
                                ImGui.TableSetColumnIndex(3);
                                DrawMount(20, currentCharacter.HasMount(20));
                            }

                            if (isAllied)
                            {
                                ImGui.TableSetColumnIndex(4);
                                DrawMinion(123, currentCharacter.HasMinion(123));
                            }
                        }

                        break;
                    }
                case 3: //Kobold
                    {
                        if (ImGui.CollapsingHeader(
                                $"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 11429)}###Progress#BeastReputations#{id}#Reward"))
                        {
                            using var t = ImRaii.Table($"###Progress#BeastReputations#{id}#Reward#Table", 4);
                            if (!t) break;
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#FramerKit",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#Minion1",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#Mount1",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#Minion2",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableNextRow();
                            ImGui.TableSetColumnIndex(0);
                            uint? fkId = _globalCache.FramerKitStorage.GetFramerKitIdFromItemId(43955);
                            if (fkId == null) return;
                            DrawFramerKit(fkId.Value, currentCharacter.HasFramerKit(fkId.Value));
                            if (rank >= 4)
                            {
                                ImGui.TableSetColumnIndex(1);
                                DrawMinion(60, currentCharacter.HasMinion(60));
                                ImGui.TableSetColumnIndex(2);
                                DrawMount(27, currentCharacter.HasMount(27));
                            }

                            if (isAllied)
                            {
                                ImGui.TableSetColumnIndex(3);
                                DrawMinion(126, currentCharacter.HasMinion(126));
                            }
                        }

                        break;
                    }
                case 4: //Sahagin
                    {
                        if (rank < 4) return;
                        if (ImGui.CollapsingHeader(
                                $"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 11429)}###Progress#BeastReputations#{id}#Reward"))
                        {
                            using var t = ImRaii.Table($"###Progress#BeastReputations#{id}#Reward#Table", 4);
                            if (!t) break;
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#Minion1",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#Mount1",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#FramerKit",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#Minion2",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableNextRow();
                            ImGui.TableSetColumnIndex(0);
                            DrawMinion(61, currentCharacter.HasMinion(61));
                            ImGui.TableSetColumnIndex(1);
                            DrawMount(26, currentCharacter.HasMount(26));
                            ImGui.TableSetColumnIndex(2);
                            uint? fkId = _globalCache.FramerKitStorage.GetFramerKitIdFromItemId(43958);
                            if (fkId == null) return;
                            DrawFramerKit(fkId.Value, currentCharacter.HasFramerKit(fkId.Value));

                            if (isAllied)
                            {
                                ImGui.TableSetColumnIndex(3);
                                DrawMinion(127, currentCharacter.HasMinion(127));
                            }
                        }

                        break;
                    }
                case 5: //Ixal
                    {
                        if (rank < 4) return;
                        if (ImGui.CollapsingHeader(
                                $"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 11429)}###Progress#BeastReputations#{id}#Reward"))
                        {
                            using var t = ImRaii.Table($"###Progress#BeastReputations#{id}#Reward#Table", 4);
                            if (!t) break;
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#Minion1",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#Mount1",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#FramerKit",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#Minion2",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableNextRow();
                            ImGui.TableSetColumnIndex(0);
                            DrawMinion(59, currentCharacter.HasMinion(59));
                            ImGui.TableSetColumnIndex(1);
                            DrawMount(35, currentCharacter.HasMount(35));

                            if (rank >= 5)
                            {
                                ImGui.TableSetColumnIndex(2);
                                uint? fkId = _globalCache.FramerKitStorage.GetFramerKitIdFromItemId(43959);
                                if (fkId == null) return;
                                DrawFramerKit(fkId.Value, currentCharacter.HasFramerKit(fkId.Value));
                            }

                            if (isAllied)
                            {
                                ImGui.TableSetColumnIndex(3);
                                DrawMinion(125, currentCharacter.HasMinion(125));
                            }
                        }

                        break;
                    }
                case 6: //Vanu Vanu
                    {
                        if (rank < 4) return;
                        if (ImGui.CollapsingHeader(
                                $"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 11429)}###Progress#BeastReputations#{id}#Reward"))
                        {
                            using var t = ImRaii.Table($"###Progress#BeastReputations#{id}#Reward#Table", 6);
                            if (!t) break;
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#Orchestrion1",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#Minion1",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#Emote1",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#Minion2",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#Mount1",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#FramerKit",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableNextRow();
                            ImGui.TableSetColumnIndex(0);
                            DrawOrchestrion(86, currentCharacter.HasOrchestrionRoll(86));
                            ImGui.TableSetColumnIndex(1);
                            DrawMinion(135, currentCharacter.HasMinion(135));

                            if (rank >= 6)
                            {
                                ImGui.TableSetColumnIndex(2);
                                DrawEmote(120, currentCharacter.HasEmote(120));
                            }

                            if (rank >= 7)
                            {
                                ImGui.TableSetColumnIndex(3);
                                DrawMinion(172, currentCharacter.HasMinion(172));
                                ImGui.TableSetColumnIndex(4);
                                DrawMount(53, currentCharacter.HasMount(53));
                            }
                            if (rank == 8)
                            {
                                ImGui.TableSetColumnIndex(5);
                                uint? fkId = _globalCache.FramerKitStorage.GetFramerKitIdFromItemId(41371);
                                if (fkId == null) return;
                                DrawFramerKit(fkId.Value, currentCharacter.HasFramerKit(fkId.Value));
                            }
                        }

                        break;
                    }
                case 7: //Vath
                    {
                        if (rank < 4) return;
                        if (ImGui.CollapsingHeader(
                                $"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 11429)}###Progress#BeastReputations#{id}#Reward"))
                        {
                            using var t = ImRaii.Table($"###Progress#BeastReputations#{id}#Reward#Table", 5);
                            if (!t) break;
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#Orchestrion1",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#Minion1",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#Minion2",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#Mount1",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#FramerKit",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableNextRow();
                            ImGui.TableSetColumnIndex(0);
                            DrawOrchestrion(118, currentCharacter.HasOrchestrionRoll(118));
                            ImGui.TableSetColumnIndex(1);
                            DrawMinion(175, currentCharacter.HasMinion(175));

                            if (rank >= 7)
                            {
                                ImGui.TableSetColumnIndex(2);
                                DrawMinion(156, currentCharacter.HasMinion(156));
                                ImGui.TableSetColumnIndex(3);
                                DrawMount(72, currentCharacter.HasMount(72));
                            }
                            if (rank == 8)
                            {
                                ImGui.TableSetColumnIndex(4);
                                uint? fkId = _globalCache.FramerKitStorage.GetFramerKitIdFromItemId(41372);
                                if (fkId == null) return;
                                DrawFramerKit(fkId.Value, currentCharacter.HasFramerKit(fkId.Value));
                            }
                        }

                        break;
                    }
                case 8: //Moogle
                    {
                        if (rank < 7) return;
                        if (ImGui.CollapsingHeader(
                                $"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 11429)}###Progress#BeastReputations#{id}#Reward"))
                        {
                            using var t = ImRaii.Table($"###Progress#BeastReputations#{id}#Reward#Table", 5);
                            if (!t) break;
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#Orchestrion1",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#Minion1",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#Emote1",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#FramerKit",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#Minion2",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableNextRow();
                            ImGui.TableSetColumnIndex(0);
                            DrawEmote(126, currentCharacter.HasEmote(126));
                            ImGui.TableSetColumnIndex(1);
                            DrawMinion(184, currentCharacter.HasMinion(184));
                            ImGui.TableSetColumnIndex(2);
                            DrawMount(86, currentCharacter.HasMount(86));

                            if (rank == 8)
                            {
                                ImGui.TableSetColumnIndex(3);
                                uint? fkId = _globalCache.FramerKitStorage.GetFramerKitIdFromItemId(41373);
                                if (fkId == null) return;
                                DrawFramerKit(fkId.Value, currentCharacter.HasFramerKit(fkId.Value));
                            }

                            if (isAllied)
                            {
                                ImGui.TableSetColumnIndex(4);
                                DrawMinion(235, currentCharacter.HasMinion(235));
                            }
                        }

                        break;
                    }
                case 9: //Kojin
                    {
                        if (rank < 5) return;
                        if (ImGui.CollapsingHeader(
                                $"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 11429)}###Progress#BeastReputations#{id}#Reward"))
                        {
                            using var t = ImRaii.Table($"###Progress#BeastReputations#{id}#Reward#Table", 7);
                            if (!t) break;
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#Minion1",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#Orchestrion1",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#Emote1",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#Mount1",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#FramerKit",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#Minion2",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#Minion3",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableNextRow();
                            ImGui.TableSetColumnIndex(0);
                            DrawMinion(266, currentCharacter.HasMinion(266));

                            if (rank >= 6)
                            {
                                ImGui.TableSetColumnIndex(1);
                                DrawOrchestrion(179, currentCharacter.HasOrchestrionRoll(179));
                            }


                            if (rank >= 8)
                            {
                                ImGui.TableSetColumnIndex(2);
                                DrawEmote(167, currentCharacter.HasEmote(167));
                                ImGui.TableSetColumnIndex(3);
                                DrawMount(136, currentCharacter.HasMount(136));
                                ImGui.TableSetColumnIndex(4);
                                uint? fkId = _globalCache.FramerKitStorage.GetFramerKitIdFromItemId(40502);
                                if (fkId == null) return;
                                DrawFramerKit(fkId.Value, currentCharacter.HasFramerKit(fkId.Value));
                            }

                            if (isAllied)
                            {
                                ImGui.TableSetColumnIndex(5);
                                DrawMinion(323, currentCharacter.HasMinion(323));
                                ImGui.TableSetColumnIndex(6);
                                DrawMinion(328, currentCharacter.HasMinion(328));
                            }
                        }

                        break;
                    }
                case 10: //Ananta
                    {
                        if (rank < 4) return;
                        if (ImGui.CollapsingHeader(
                                $"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 11429)}###Progress#BeastReputations#{id}#Reward"))
                        {
                            using var t = ImRaii.Table($"###Progress#BeastReputations#{id}#Reward#Table", 7);
                            if (!t) break;
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#Minion1",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#Emote1",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#Orchestrion1",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#Mount1",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#Mount2",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#FramerKit",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#Minion2",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableNextRow();
                            ImGui.TableSetColumnIndex(0);
                            DrawMinion(277, currentCharacter.HasMinion(277));

                            if (rank >= 5)
                            {
                                ImGui.TableSetColumnIndex(1);
                                DrawEmote(64, currentCharacter.HasEmote(64));
                                ImGui.TableSetColumnIndex(2);
                                DrawOrchestrion(207, currentCharacter.HasOrchestrionRoll(207));
                            }

                            if (rank >= 7)
                            {
                                ImGui.TableSetColumnIndex(3);
                                DrawMount(146, currentCharacter.HasMount(146));
                            }

                            if (rank >= 8)
                            {
                                ImGui.TableSetColumnIndex(4);
                                DrawMount(148, currentCharacter.HasMount(148));
                                ImGui.TableSetColumnIndex(5);
                                uint? fkId = _globalCache.FramerKitStorage.GetFramerKitIdFromItemId(40503);
                                if (fkId == null) return;
                                DrawFramerKit(fkId.Value, currentCharacter.HasFramerKit(fkId.Value));
                            }

                            if (isAllied)
                            {
                                ImGui.TableSetColumnIndex(6);
                                DrawMinion(322, currentCharacter.HasMinion(322));
                            }
                        }

                        break;
                    }
                case 11: //Namazu
                    {
                        if (rank < 4) return;
                        if (ImGui.CollapsingHeader(
                                $"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 11429)}###Progress#BeastReputations#{id}#Reward"))
                        {
                            using var t = ImRaii.Table($"###Progress#BeastReputations#{id}#Reward#Table", 5);
                            if (!t) break;
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#Minion1",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#Emote1",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#Orchestrion1",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#Mount1",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#FramerKit",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableNextRow();
                            ImGui.TableSetColumnIndex(0);
                            DrawMinion(302, currentCharacter.HasMinion(302));

                            if (rank >= 6)
                            {
                                ImGui.TableSetColumnIndex(1);
                                DrawEmote(176, currentCharacter.HasEmote(176));
                                ImGui.TableSetColumnIndex(2);
                                DrawOrchestrion(231, currentCharacter.HasOrchestrionRoll(231));
                            }

                            if (rank >= 8)
                            {
                                ImGui.TableSetColumnIndex(3);
                                DrawMount(164, currentCharacter.HasMount(164));
                                ImGui.TableSetColumnIndex(4);
                                uint? fkId = _globalCache.FramerKitStorage.GetFramerKitIdFromItemId(40504);
                                if (fkId == null) return;
                                DrawFramerKit(fkId.Value, currentCharacter.HasFramerKit(fkId.Value));
                            }
                        }

                        break;
                    }
                case 12: //Pixie
                    {
                        if (rank < 6) return;
                        if (ImGui.CollapsingHeader(
                                $"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 11429)}###Progress#BeastReputations#{id}#Reward"))
                        {
                            using var t = ImRaii.Table($"###Progress#BeastReputations#{id}#Reward#Table", 4);
                            if (!t) break;
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#Minion1",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#Mount1",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#Orchestrion1",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#FramerKit",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableNextRow();
                            ImGui.TableSetColumnIndex(0);
                            DrawMinion(354, currentCharacter.HasMinion(354));

                            if (rank >= 7)
                            {
                                ImGui.TableSetColumnIndex(1);
                                DrawMount(201, currentCharacter.HasMount(201));
                            }

                            if (rank >= 8)
                            {
                                ImGui.TableSetColumnIndex(2);
                                DrawOrchestrion(345, currentCharacter.HasOrchestrionRoll(345));
                                ImGui.TableSetColumnIndex(3);
                                uint? fkId = _globalCache.FramerKitStorage.GetFramerKitIdFromItemId(39572);
                                if (fkId == null) return;
                                DrawFramerKit(fkId.Value, currentCharacter.HasFramerKit(fkId.Value));
                            }
                        }

                        break;
                    }
                case 13: //Qitari
                    {
                        if (rank < 4) return;
                        if (ImGui.CollapsingHeader(
                                $"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 11429)}###Progress#BeastReputations#{id}#Reward"))
                        {
                            using var t = ImRaii.Table($"###Progress#BeastReputations#{id}#Reward#Table", 5);
                            if (!t) break;
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#Minion1",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#Orchestrion1",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#Mount1",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#Minion2",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#FramerKit",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableNextRow();
                            ImGui.TableSetColumnIndex(0);
                            DrawMinion(369, currentCharacter.HasMinion(369));
                            
                            if (rank >= 6)
                            {
                                ImGui.TableSetColumnIndex(1);
                                DrawOrchestrion(371, currentCharacter.HasOrchestrionRoll(371));
                            }

                            if (rank >= 7)
                            {
                                ImGui.TableSetColumnIndex(2);
                                DrawMount(215, currentCharacter.HasMount(215));
                            }

                            if (rank >= 8)
                            {
                                ImGui.TableSetColumnIndex(3);
                                DrawMinion(370, currentCharacter.HasMinion(370));
                                ImGui.TableSetColumnIndex(4);
                                uint? fkId = _globalCache.FramerKitStorage.GetFramerKitIdFromItemId(39573);
                                if (fkId == null) return;
                                DrawFramerKit(fkId.Value, currentCharacter.HasFramerKit(fkId.Value));
                            }
                        }

                        break;
                    }
                case 14: //Dwarf
                    {
                        if (rank < 4) return;
                        if (ImGui.CollapsingHeader(
                                $"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 11429)}###Progress#BeastReputations#{id}#Reward"))
                        {
                            using var t = ImRaii.Table($"###Progress#BeastReputations#{id}#Reward#Table", 5);
                            if (!t) break;
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#Minion1",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#Orchestrion1",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#Mount1",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#Emote1",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#FramerKit",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableNextRow();
                            ImGui.TableSetColumnIndex(0);
                            DrawMinion(380, currentCharacter.HasMinion(380));
                            
                            if (rank >= 6)
                            {
                                ImGui.TableSetColumnIndex(1);
                                DrawOrchestrion(383, currentCharacter.HasOrchestrionRoll(383));
                            }

                            if (rank >= 7)
                            {
                                ImGui.TableSetColumnIndex(2);
                                DrawMount(223, currentCharacter.HasMount(223));
                            }

                            if (rank >= 8)
                            {
                                ImGui.TableSetColumnIndex(3);
                                DrawEmote(199, currentCharacter.HasEmote(199));
                                ImGui.TableSetColumnIndex(4);
                                uint? fkId = _globalCache.FramerKitStorage.GetFramerKitIdFromItemId(39574);
                                if (fkId == null) return;
                                DrawFramerKit(fkId.Value, currentCharacter.HasFramerKit(fkId.Value));
                            }
                        }

                        break;
                    }
                case 15: //Arkasodara
                    {
                        if (rank < 4) return;
                        if (ImGui.CollapsingHeader(
                                $"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 11429)}###Progress#BeastReputations#{id}#Reward"))
                        {
                            using var t = ImRaii.Table($"###Progress#BeastReputations#{id}#Reward#Table", 5);
                            if (!t) break;
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#Minion1",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#TTCard1",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#Mount1",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#Orchestrion1",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#FramerKit",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableNextRow();
                            ImGui.TableSetColumnIndex(0);
                            DrawMinion(444, currentCharacter.HasMinion(444));
                            
                            if (rank >= 5)
                            {
                                ImGui.TableSetColumnIndex(1);
                                DrawTripleTriadCard(349, currentCharacter.HasTTC(349));
                            }

                            if (rank >= 7)
                            {
                                ImGui.TableSetColumnIndex(2);
                                DrawMount(287, currentCharacter.HasMount(287));
                            }

                            if (rank >= 8)
                            {
                                ImGui.TableSetColumnIndex(3);
                                DrawOrchestrion(514, currentCharacter.HasOrchestrionRoll(514));
                                ImGui.TableSetColumnIndex(4);
                                uint? fkId = _globalCache.FramerKitStorage.GetFramerKitIdFromItemId(39575);
                                if (fkId == null) return;
                                DrawFramerKit(fkId.Value, currentCharacter.HasFramerKit(fkId.Value));
                            }
                        }

                        break;
                    }
                case 16: //Omicron
                    {
                        if (rank < 4) return;
                        if (ImGui.CollapsingHeader(
                                $"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 11429)}###Progress#BeastReputations#{id}#Reward"))
                        {
                            using var t = ImRaii.Table($"###Progress#BeastReputations#{id}#Reward#Table", 5);
                            if (!t) break;
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#TTCard1",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#Minion1",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#Mount1",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#FramerKit",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#Orchestrion1",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableNextRow();
                            ImGui.TableSetColumnIndex(0);
                            DrawTripleTriadCard(357, currentCharacter.HasTTC(357));
                            
                            if (rank >= 5)
                            {
                                ImGui.TableSetColumnIndex(1);
                                DrawMinion(457, currentCharacter.HasMinion(457));
                            }

                            if (rank >= 7)
                            {
                                ImGui.TableSetColumnIndex(2);
                                DrawMount(298, currentCharacter.HasMount(298));
                            }

                            if (rank >= 8)
                            {
                                ImGui.TableSetColumnIndex(3);
                                uint? fkId = _globalCache.FramerKitStorage.GetFramerKitIdFromItemId(38466);
                                if (fkId == null) return;
                                DrawFramerKit(fkId.Value, currentCharacter.HasFramerKit(fkId.Value));
                                ImGui.TableSetColumnIndex(4);
                                DrawOrchestrion(514, currentCharacter.HasOrchestrionRoll(514));
                            }
                        }

                        break;
                    }
                case 17: //Loporrit
                    {
                        if (rank < 5) return;
                        if (ImGui.CollapsingHeader(
                                $"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 11429)}###Progress#BeastReputations#{id}#Reward"))
                        {
                            using var t = ImRaii.Table($"###Progress#BeastReputations#{id}#Reward#Table", 6);
                            if (!t) break;
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#Minion1",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#Orchestrion1",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#Mount1",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#Emote",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#FramerKit",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableSetupColumn($"###Progress#BeastReputations#{id}#Reward#Orchestrion",
                                ImGuiTableColumnFlags.WidthFixed, 36);
                            ImGui.TableNextRow();
                            ImGui.TableSetColumnIndex(0);
                            DrawMinion(472, currentCharacter.HasMinion(472));
                            
                            if (rank >= 6)
                            {
                                ImGui.TableSetColumnIndex(1);
                                DrawOrchestrion(582, currentCharacter.HasOrchestrionRoll(582));
                            }

                            if (rank >= 7)
                            {
                                ImGui.TableSetColumnIndex(2);
                                DrawMount(285, currentCharacter.HasMount(285));
                            }

                            if (rank >= 8)
                            {
                                ImGui.TableSetColumnIndex(3);
                                DrawEmote(252, currentCharacter.HasEmote(252));
                                ImGui.TableSetColumnIndex(4);
                                uint? fkId = _globalCache.FramerKitStorage.GetFramerKitIdFromItemId(39576);
                                if (fkId == null) return;
                                DrawFramerKit(fkId.Value, currentCharacter.HasFramerKit(fkId.Value));
                                ImGui.TableSetColumnIndex(5);
                                DrawOrchestrion(566, currentCharacter.HasOrchestrionRoll(566));
                            }
                        }

                        break;
                    }
            }
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

                ImGui.SetCursorPos(new Vector2(p.X + 26, p.Y + 20));
                ImGui.TextUnformatted("\u2713");
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

                ImGui.SetCursorPos(new Vector2(p.X + 26, p.Y + 20));
                ImGui.TextUnformatted("\u2713");
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

                ImGui.SetCursorPos(new Vector2(p.X + 26, p.Y + 20));
                ImGui.TextUnformatted("\u2713");
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

                ImGui.SetCursorPos(new Vector2(p.X + 26, p.Y + 20));
                ImGui.TextUnformatted("\u2713");
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

                ImGui.SetCursorPos(new Vector2(p.X + 26, p.Y + 20));
                ImGui.TextUnformatted("\u2713");
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
                    Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(t.Icon), new Vector2(32, 32),
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
                Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(t.Icon), new Vector2(32, 32));
                if (ImGui.IsItemHovered())
                {
                    Utils.DrawTTCTooltip(_currentLocale, ref _globalCache, t);
                }

                ImGui.SetCursorPos(new Vector2(p.X + 26, p.Y + 20));
                ImGui.TextUnformatted("\u2713");
                ImGui.SetCursorPos(p);
            }
        }
    }
}
