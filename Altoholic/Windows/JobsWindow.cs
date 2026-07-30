using Altoholic.Cache;
using Altoholic.Models;
using Dalamud.Bindings.ImGui;
using Dalamud.Game;
using Dalamud.Game.Text;
using Dalamud.Interface.Textures.TextureWraps;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Windowing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Altoholic.Windows
{
    public class JobsWindow : Window, IDisposable
    {
        private readonly Plugin _plugin;
        private ClientLanguage _currentLocale;
        private readonly GlobalCache _globalCache;
        public JobsWindow(
            Plugin plugin,
            string name,
            GlobalCache globalCache) 
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
            //_currentLocale = currentLocale;

            _rolesTextureWrap = _globalCache.IconStorage.LoadRoleIconTexture();
            _phantomJobTexture = _globalCache.IconStorage.LoadPhantomJobsTexture();
            _phantomJobsIconsTexture = _globalCache.IconStorage.LoadPhantomJobsIconsTexture();
        }

        public Func<Character> GetPlayer { get; init; } = null!;
        public Func<List<Character>> GetOthersCharactersList { get; set; } = null!;
        private Character? _currentCharacter;
        private IDalamudTextureWrap? _rolesTextureWrap;
        private IDalamudTextureWrap? _phantomJobTexture;
        private IDalamudTextureWrap? _phantomJobsIconsTexture;

        public void Dispose()
        {
            Utils.LogMessage(LogLevel.Debug, _plugin.Configuration.EnableDebugMessages, "JobsWindow, Dispose() called");
            _currentCharacter = null;
        }
        public void Clear()
        {
            Utils.LogMessage(LogLevel.Debug, _plugin.Configuration.EnableDebugMessages, "JobsWindow, Clear() called");
            _currentCharacter = null;
        }

        public override void Draw()
        {
            _currentLocale = _plugin.Configuration.Language;
            List<Character> chars = [];
            chars.Insert(0, GetPlayer.Invoke());
            chars.AddRange(GetOthersCharactersList.Invoke());

            using var charactersJobsTable = ImRaii.Table("###CharactersJobsTable", 2);
            if (!charactersJobsTable) return;
            ImGui.TableSetupColumn("###CharactersJobsTable#CharactersListHeader", ImGuiTableColumnFlags.WidthFixed,
                210);
            ImGui.TableSetupColumn("###CharactersJobsTable#Jobs", ImGuiTableColumnFlags.WidthStretch);
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            using (var listbox = ImRaii.ListBox("###CharactersJobsTable#CharactersListBox", new Vector2(200, -1)))
            {
                if (listbox)
                {
                    if (chars.Count > 0)
                    {
                        if (ImGui.Selectable(
                                $"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 970)}###CharactersJobsTable#CharactersListBox#All",
                                _currentCharacter == null))
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
                                });
                            }
#endif

                        foreach (var currChar in chars.Where(currChar =>
                                     ImGui.Selectable(
                                         $"{currChar.FirstName} {currChar.LastName}{(char)SeIconChar.CrossWorld}{currChar.HomeWorld}",
                                         currChar == _currentCharacter)))
                        {
                            _currentCharacter = currChar;
                        }
                    }
                }
            }

            ImGui.TableSetColumnIndex(1);
            if (_currentCharacter is not null)
            {
                DrawJobs(_currentCharacter);
            }
            else
            {
                DrawAll(chars);
            }
        }

        private void DrawAll(List<Character> chars)
        {
            if (chars.Count == 0) return;
            uint[] jobsIdsOrder = [1, 19, 3, 21, 32, 37, 6, 24, 28, 33, 40, 2, 20, 4, 22, 29, 30, 34, 39, 41,43, 5, 31, 23, 38, 7, 25, 26, 27, 35, 42, 36, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18];
            int columns = chars.Count + 1;
            using var charactersHildibrandQuestAll = ImRaii.Table("###CharactersJobs#All", columns,
                ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg | ImGuiTableFlags.BordersInner |
                ImGuiTableFlags.ScrollX | ImGuiTableFlags.ScrollY);
            if (!charactersHildibrandQuestAll) return;
            ImGui.TableSetupColumn($"###CharactersJobs#All#Job", ImGuiTableColumnFlags.WidthFixed, 32);
            foreach (Character c in chars)
            {
                ImGui.TableSetupColumn($"###CharactersJobs#All#Name#{c.CharacterId}",
                    ImGuiTableColumnFlags.WidthFixed, ImGui.CalcTextSize("100").X + 5);
            }
            ImGui.TableSetupScrollFreeze(columns, 1);//Freeze header so it shows while scrolling
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            ImGui.TextUnformatted(_globalCache.AddonStorage.LoadAddonString(_currentLocale, 1898));
            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                ImGui.TextUnformatted(_globalCache.AddonStorage.LoadAddonString(_currentLocale, 294));
                ImGui.EndTooltip();
            }

            foreach (Character currChar in chars)
            {
                ImGui.TableNextColumn();
                ImGui.TextUnformatted($"{currChar.FirstName[0]}.{currChar.LastName[0]}");
                if (ImGui.IsItemHovered())
                {
                    ImGui.BeginTooltip();
                    ImGui.TextUnformatted(
                        $"{currChar.FirstName} {currChar.LastName}{(char)SeIconChar.CrossWorld}{currChar.HomeWorld}");
                    ImGui.EndTooltip();
                }
            }

            foreach (uint i in jobsIdsOrder)
            {
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(Utils.GetJobIconWithCorner(i)), new Vector2(30, 30));
                HoverJobName(i);

                foreach (Character currChar in chars)
                {
                    ImGui.TableNextColumn();
                    ImGui.TextUnformatted($"{Helpers.Jobs.GetCharacterJob(i, currChar)?.Level}");
                    HoverCharNameJobName(currChar, i);
                }
            }
        }

        private void HoverJobName(uint job)
        {
            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                ImGui.TextUnformatted(_globalCache.JobStorage.GetName(_currentLocale, job));
                ImGui.EndTooltip();
            }
        }
        private void HoverCharNameJobName(Character character, uint job)
        {
            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                ImGui.TextUnformatted($"{character.FirstName} {character.LastName}{(char)SeIconChar.CrossWorld}{character.HomeWorld}");
                ImGui.TextUnformatted(_globalCache.JobStorage.GetName(_currentLocale, job));
                ImGui.EndTooltip();
            }
        }
        private void DrawJobs(Character selectedCharacter)
        {
            using var tabBar = ImRaii.TabBar($"###CharactersJobs#JobsTabs#{selectedCharacter.CharacterId}");
            if (!tabBar.Success) return;
            using (var DoWDoMTab = ImRaii.TabItem($"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 1080)}###CharactersJobs#JobsTabs#DoWDoM#{selectedCharacter.CharacterId}"))
            {
                if (DoWDoMTab)
                {
                    DrawDoWDoMJobs(selectedCharacter);
                }
            }
            using (var DoHDoLTab = ImRaii.TabItem($"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 1081)}###CharactersJobs#JobsTabs#DoHDoL#{selectedCharacter.CharacterId}"))
            {
                if (DoHDoLTab)
                {
                    DrawDoHDoLJobs(selectedCharacter);
                }
            }
            if(selectedCharacter.HasQuest((int)QuestIds.OCCULT_CRESCENT_UNFAMILIAR_TERRITORY) && selectedCharacter.OccultCrescent is not null)
            using (var phantomJobsTab = ImRaii.TabItem($"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 16611)}###CharactersJobs#JobsTabs#Phantom#{selectedCharacter.CharacterId}"))
            {
                if (phantomJobsTab)
                {
                    Helpers.Jobs.DrawPhantomJobs(_phantomJobTexture, _phantomJobsIconsTexture, _globalCache, _currentLocale, selectedCharacter);
                }
            }
        }

        private void DrawDoWDoMJobs(Character selectedCharacter)
        {
            if (_rolesTextureWrap is null) return;
            using var charactersJobsDoWDoMJobs = ImRaii.Table($"###CharactersJobs#DoWDoMJobs#{selectedCharacter.CharacterId}", 2,
                ImGuiTableFlags.ScrollY);
            if (!charactersJobsDoWDoMJobs) return;
            ImGui.TableSetupColumn($"###CharactersJobs#DoW#{selectedCharacter.CharacterId}", ImGuiTableColumnFlags.WidthStretch);
            ImGui.TableSetupColumn($"###CharactersJobs#DoM#{selectedCharacter.CharacterId}", ImGuiTableColumnFlags.WidthStretch);
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            using (var charactersJobsDoWRoleTank = ImRaii.Table("###CharactersJobs#DoW#RoleTank", 2))
            {
                if (!charactersJobsDoWRoleTank) return;
                ImGui.TableSetupColumn("###CharactersJobs#DoW#RoleTank#Icon", ImGuiTableColumnFlags.WidthFixed, 22);
                ImGui.TableSetupColumn("###CharactersJobs#DoW#RoleTank#Name", ImGuiTableColumnFlags.WidthStretch);
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                Utils.DrawRoleTexture(ref _rolesTextureWrap, RoleIcon.Tank, new Vector2(20, 20));
                ImGui.TableSetColumnIndex(1);
                ImGui.TextUnformatted($"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 1082)}"); // Tank
            }

            ImGui.Separator();

            ImGui.TableSetColumnIndex(1);
            using (var charactersJobsDoMRoleHealer = ImRaii.Table("###CharactersJobs#DoM#RoleHealer", 2))
            {
                if (!charactersJobsDoMRoleHealer) return;
                ImGui.TableSetupColumn("###CharactersJobs#DoM#RoleHealer#Icon", ImGuiTableColumnFlags.WidthFixed, 22);
                ImGui.TableSetupColumn("###CharactersJobs#DoM#RoleHealer#Name", ImGuiTableColumnFlags.WidthStretch);
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                Utils.DrawRoleTexture(ref _rolesTextureWrap, RoleIcon.Heal, new Vector2(20, 20));
                ImGui.TableSetColumnIndex(1);
                ImGui.TextUnformatted($"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 1083)}");
            }

            ImGui.Separator();

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            DrawJobLine(selectedCharacter, ClassJob.GLA);
            ImGui.TableSetColumnIndex(1);
            DrawJobLine(selectedCharacter, ClassJob.CNJ);

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            DrawJobLine(selectedCharacter, ClassJob.MRD);
            ImGui.TableSetColumnIndex(1);
            DrawJobLine(selectedCharacter, ClassJob.SCH);

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            DrawJobLine(selectedCharacter, ClassJob.DRK);
            ImGui.TableSetColumnIndex(1);
            DrawJobLine(selectedCharacter, ClassJob.AST);

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            DrawJobLine(selectedCharacter, ClassJob.GNB);
            ImGui.TableSetColumnIndex(1);
            DrawJobLine(selectedCharacter, ClassJob.SGE);

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            using (var charactersJobsDoWRoleMelee = ImRaii.Table("###CharactersJobs#DoW#RoleMelee", 2))
            {
                if (!charactersJobsDoWRoleMelee) return;
                ImGui.TableSetupColumn("###CharactersJobs#DoW#RoleMelee#Icon", ImGuiTableColumnFlags.WidthFixed, 22);
                ImGui.TableSetupColumn("###CharactersJobs#DoW#RoleMelee#Name", ImGuiTableColumnFlags.WidthStretch);
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                Utils.DrawRoleTexture(ref _rolesTextureWrap, RoleIcon.Melee, new Vector2(20, 20));
                ImGui.TableSetColumnIndex(1);
                ImGui.TextUnformatted($"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 1084)}");
            }

            ImGui.Separator();

            ImGui.TableSetColumnIndex(1);
            using (var charactersJobsDoWRolePhysicalRanged =
                   ImRaii.Table("###CharactersJobs#DoW#RolePhysicalRanged", 2))
            {
                if (!charactersJobsDoWRolePhysicalRanged) return;
                ImGui.TableSetupColumn("###CharactersJobs#DoW#RolePhysicalRanged#Icon",
                    ImGuiTableColumnFlags.WidthFixed, 22);
                ImGui.TableSetupColumn("###CharactersJobs#DoW#RolePhysicalRanged#Name",
                    ImGuiTableColumnFlags.WidthStretch);
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                Utils.DrawRoleTexture(ref _rolesTextureWrap, RoleIcon.Ranged, new Vector2(20, 20));
                ImGui.TableSetColumnIndex(1);
                ImGui.TextUnformatted($"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 1085)}");
            }

            ImGui.Separator();

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            DrawJobLine(selectedCharacter, ClassJob.PGL);
            ImGui.TableSetColumnIndex(1);
            DrawJobLine(selectedCharacter, ClassJob.ARC);

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            DrawJobLine(selectedCharacter, ClassJob.LNC);
            ImGui.TableSetColumnIndex(1);
            DrawJobLine(selectedCharacter, ClassJob.MCH);

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            DrawJobLine(selectedCharacter, ClassJob.ROG);
            ImGui.TableSetColumnIndex(1);
            DrawJobLine(selectedCharacter, ClassJob.DNC);

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            DrawJobLine(selectedCharacter, ClassJob.SAM);

            ImGui.TableSetColumnIndex(1);
            using (var charactersJobsDoMRoleMagicalRanged = ImRaii.Table("###CharactersJobs#DoM#RoleMagicalRanged", 2))
            {
                if (!charactersJobsDoMRoleMagicalRanged) return;
                ImGui.TableSetupColumn("###CharactersJobs#DoM#RoleMagicalRanged#Icon", ImGuiTableColumnFlags.WidthFixed,
                    22);
                ImGui.TableSetupColumn("###CharactersJobs#DoM#RoleMagicalRanged#Name",
                    ImGuiTableColumnFlags.WidthStretch);
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                Utils.DrawRoleTexture(ref _rolesTextureWrap, RoleIcon.Caster, new Vector2(20, 20));
                ImGui.TableSetColumnIndex(1);
                ImGui.TextUnformatted($"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 1086)}");
            }

            ImGui.Separator();

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            DrawJobLine(selectedCharacter, ClassJob.RPR);
            ImGui.TableSetColumnIndex(1);
            DrawJobLine(selectedCharacter, ClassJob.THM);

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            DrawJobLine(selectedCharacter, ClassJob.VPR);
            ImGui.TableSetColumnIndex(1);
            DrawJobLine(selectedCharacter, ClassJob.SMN);

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            DrawJobLine(selectedCharacter, ClassJob.BST);
            ImGui.TableSetColumnIndex(1);
            DrawJobLine(selectedCharacter, ClassJob.RDM);
            
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(1);
            DrawJobLine(selectedCharacter, ClassJob.PCT);

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(1);
            DrawJobLine(selectedCharacter, ClassJob.BLU);
        }

        private void DrawDoHDoLJobs(Character selectedCharacter)
        {
            if (_rolesTextureWrap is null) return;
            using var charactersJobsDoHDoLJobs =
                ImRaii.Table($"###CharactersJobs#DoHDoLJobs#{selectedCharacter.CharacterId}", 2);
            if (!charactersJobsDoHDoLJobs) return;
            ImGui.TableSetupColumn($"###CharactersJobs#DoH#{selectedCharacter.CharacterId}", ImGuiTableColumnFlags.WidthStretch);
            ImGui.TableSetupColumn($"###CharactersJobs#DoL#{selectedCharacter.CharacterId}", ImGuiTableColumnFlags.WidthStretch);
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            using (var charactersJobsDoHRoleDoH = ImRaii.Table("###CharactersJobs#DoH#RoleDoH", 2))
            {
                if (!charactersJobsDoHRoleDoH) return;
                ImGui.TableSetupColumn("###CharactersJobs#DoH#RoleDoH#Icon", ImGuiTableColumnFlags.WidthFixed, 22);
                ImGui.TableSetupColumn("###CharactersJobs#DoH#RoleDoH#Name", ImGuiTableColumnFlags.WidthStretch);
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                Utils.DrawRoleTexture(ref _rolesTextureWrap, RoleIcon.Crafter, new Vector2(20, 20));
                ImGui.TableSetColumnIndex(1);
                ImGui.TextUnformatted($"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 802)}");
            }

            ImGui.Separator();

            ImGui.TableSetColumnIndex(1);
            using (var charactersJobsDoLRoleDoL = ImRaii.Table("###CharactersJobs#DoL#RoleDoL", 2))
            {
                if (!charactersJobsDoLRoleDoL) return;
                ImGui.TableSetupColumn("###CharactersJobs#DoL#RoleDoL#Icon", ImGuiTableColumnFlags.WidthFixed, 22);
                ImGui.TableSetupColumn("###CharactersJobs#DoL#RoleDoL#Name", ImGuiTableColumnFlags.WidthStretch);
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                Utils.DrawRoleTexture(ref _rolesTextureWrap, RoleIcon.Gatherer, new Vector2(20, 20));
                ImGui.TableSetColumnIndex(1);
                ImGui.TextUnformatted($"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 803)}");
            }

            ImGui.Separator();

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            DrawJobLine(selectedCharacter, ClassJob.CRP);
            ImGui.TableSetColumnIndex(1);
            DrawJobLine(selectedCharacter, ClassJob.MIN);

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            DrawJobLine(selectedCharacter, ClassJob.BSM);
            ImGui.TableSetColumnIndex(1);
            DrawJobLine(selectedCharacter, ClassJob.BTN);

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            DrawJobLine(selectedCharacter, ClassJob.ARM);
            ImGui.TableSetColumnIndex(1);
            DrawJobLine(selectedCharacter, ClassJob.FSH);

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            DrawJobLine(selectedCharacter, ClassJob.GSM);

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            DrawJobLine(selectedCharacter, ClassJob.LTW);

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            DrawJobLine(selectedCharacter, ClassJob.WVR);

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            DrawJobLine(selectedCharacter, ClassJob.ALC);

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            DrawJobLine(selectedCharacter, ClassJob.CUL);
        }

        private void DrawJobLine(Character selectedCharacter, ClassJob job)
        {
            if (selectedCharacter.Jobs is null) return;
            switch (job)
            {
                case ClassJob.GLA:
                case ClassJob.PLD:
                    {
                        if (selectedCharacter.Jobs.Gladiator.Level >= 30)
                        {
                            DrawJob(selectedCharacter.Jobs.Paladin, ClassJob.PLD, $"{_globalCache.JobStorage.GetName(_currentLocale,(uint)ClassJob.PLD)} / {_globalCache.JobStorage.GetName(_currentLocale,(uint)ClassJob.GLA)}", true);
                        }
                        else
                        {
                            bool active = (selectedCharacter.Jobs.Gladiator.Level > 0);
                            DrawJob(selectedCharacter.Jobs.Gladiator, ClassJob.GLA, _globalCache.JobStorage.GetName(_currentLocale,(uint)ClassJob.GLA), active);
                        }

                        break;
                    }
                case ClassJob.MRD:
                case ClassJob.WAR:
                    {
                        if (selectedCharacter.Jobs.Marauder.Level >= 30)
                        {
                            DrawJob(selectedCharacter.Jobs.Warrior, ClassJob.WAR, $"{_globalCache.JobStorage.GetName(_currentLocale,(uint)ClassJob.WAR)} / {_globalCache.JobStorage.GetName(_currentLocale,(uint)ClassJob.MRD)}", true);
                        }
                        else
                        {
                            bool active = (selectedCharacter.Jobs.Marauder.Level > 0);
                            DrawJob(selectedCharacter.Jobs.Marauder, ClassJob.MRD, _globalCache.JobStorage.GetName(_currentLocale,(uint)ClassJob.MRD), active);
                        }

                        break;
                    }
                case ClassJob.DRK:
                    {
                        bool active = (selectedCharacter.Jobs.DarkKnight.Level >= 30);
                        DrawJob(selectedCharacter.Jobs.DarkKnight, ClassJob.DRK, _globalCache.JobStorage.GetName(_currentLocale,(uint)ClassJob.DRK), active);

                        break;
                    }
                case ClassJob.GNB:
                    {
                        bool active = (selectedCharacter.Jobs.Gunbreaker.Level >= 60);
                        DrawJob(selectedCharacter.Jobs.Gunbreaker, ClassJob.GNB, _globalCache.JobStorage.GetName(_currentLocale,(uint)ClassJob.GNB), active);

                        break;
                    }
                case ClassJob.PGL:
                case ClassJob.MNK:
                    {
                        if (selectedCharacter.Jobs.Pugilist.Level >= 30)
                        {
                            DrawJob(selectedCharacter.Jobs.Monk, ClassJob.MNK, $"{_globalCache.JobStorage.GetName(_currentLocale,(uint)ClassJob.MNK)} / {_globalCache.JobStorage.GetName(_currentLocale,(uint)ClassJob.PGL)}", true);
                        }
                        else
                        {
                            bool active = (selectedCharacter.Jobs.Pugilist.Level > 0);
                            DrawJob(selectedCharacter.Jobs.Pugilist, ClassJob.PGL, _globalCache.JobStorage.GetName(_currentLocale,(uint)ClassJob.PGL), active);
                        }

                        break;
                    }
                case ClassJob.LNC:
                case ClassJob.DRG:
                    {
                        if (selectedCharacter.Jobs.Lancer.Level >= 30)
                        {
                            DrawJob(selectedCharacter.Jobs.Dragoon, ClassJob.DRG, $"{_globalCache.JobStorage.GetName(_currentLocale,(uint)ClassJob.DRG)} / {_globalCache.JobStorage.GetName(_currentLocale,(uint)ClassJob.LNC)}", true);
                        }
                        else
                        {
                            bool active = (selectedCharacter.Jobs.Lancer.Level > 0);
                            DrawJob(selectedCharacter.Jobs.Lancer, ClassJob.LNC, _globalCache.JobStorage.GetName(_currentLocale,(uint)ClassJob.LNC), active);
                        }

                        break;
                    }
                case ClassJob.ROG:
                case ClassJob.NIN:
                    {
                        if (selectedCharacter.Jobs.Rogue.Level >= 30)
                        {
                            DrawJob(selectedCharacter.Jobs.Ninja, ClassJob.NIN, $"{_globalCache.JobStorage.GetName(_currentLocale,(uint)ClassJob.NIN)} / {_globalCache.JobStorage.GetName(_currentLocale,(uint)ClassJob.ROG)}", true);
                        }
                        else
                        {
                            bool active = (selectedCharacter.Jobs.Rogue.Level > 0);
                            DrawJob(selectedCharacter.Jobs.Rogue, ClassJob.ROG, _globalCache.JobStorage.GetName(_currentLocale,(uint)ClassJob.ROG), active);
                        }

                        break;
                    }
                case ClassJob.SAM:
                    {
                        bool active = (selectedCharacter.Jobs.Samurai.Level >= 50);
                        DrawJob(selectedCharacter.Jobs.Samurai, ClassJob.SAM, _globalCache.JobStorage.GetName(_currentLocale,(uint)ClassJob.SAM), active);

                        break;
                    }
                case ClassJob.RPR:
                    {
                        bool active = (selectedCharacter.Jobs.Reaper.Level >= 70);
                        DrawJob(selectedCharacter.Jobs.Reaper, ClassJob.RPR, _globalCache.JobStorage.GetName(_currentLocale,(uint)ClassJob.RPR), active);

                        break;
                    }
                case ClassJob.VPR:
                    {
                        bool active = (selectedCharacter.Jobs.Viper.Level >= 70);
                        DrawJob(selectedCharacter.Jobs.Viper, ClassJob.VPR, _globalCache.JobStorage.GetName(_currentLocale,(uint)ClassJob.VPR), active);

                        break;
                    }
                case ClassJob.BST:
                    {
                        bool active = (selectedCharacter.Jobs.Beastmaster.Level >= 70);
                        DrawJob(selectedCharacter.Jobs.Beastmaster, ClassJob.BST, _globalCache.JobStorage.GetName(_currentLocale, (uint)ClassJob.BST), active);

                        break;
                    }
                case ClassJob.CNJ:
                case ClassJob.WHM:
                    {
                        if (selectedCharacter.Jobs.Conjurer.Level >= 30)
                        {
                            DrawJob(selectedCharacter.Jobs.WhiteMage, ClassJob.WHM, $"{_globalCache.JobStorage.GetName(_currentLocale,(uint)ClassJob.WHM)} / {_globalCache.JobStorage.GetName(_currentLocale,(uint)ClassJob.CNJ)}", true);
                        }
                        else
                        {
                            bool active = (selectedCharacter.Jobs.Conjurer.Level > 0);
                            DrawJob(selectedCharacter.Jobs.Conjurer, ClassJob.CNJ, _globalCache.JobStorage.GetName(_currentLocale,(uint)ClassJob.CNJ), active);
                        }

                        break;
                    }
                case ClassJob.ACN:
                case ClassJob.SCH:
                case ClassJob.SMN:
                    {
                        if (selectedCharacter.Jobs.Arcanist.Level >= 30)
                        {
                            if (job == ClassJob.SCH)
                            {
                                DrawJob(selectedCharacter.Jobs.Scholar, ClassJob.SCH,
                                    $"{_globalCache.JobStorage.GetName(_currentLocale, (uint)ClassJob.SCH)}", true);
                            }
                            else if (job == ClassJob.SMN)
                            {
                                DrawJob(selectedCharacter.Jobs.Summoner, ClassJob.SMN,
                                    $"{_globalCache.JobStorage.GetName(_currentLocale, (uint)ClassJob.SMN)} / {_globalCache.JobStorage.GetName(_currentLocale, (uint)ClassJob.ACN)}",
                                    true);
                            }
                        }
                        else
                        {
                            bool active = (selectedCharacter.Jobs.Arcanist.Level > 0);
                            DrawJob(selectedCharacter.Jobs.Arcanist, ClassJob.ACN, _globalCache.JobStorage.GetName(_currentLocale,(uint)ClassJob.ACN), active);
                        }
                        break;
                    }
                case ClassJob.AST:
                    {
                        bool active = (selectedCharacter.Jobs.Astrologian.Level >= 30) ;
                        DrawJob(selectedCharacter.Jobs.Astrologian, ClassJob.AST, _globalCache.JobStorage.GetName(_currentLocale,(uint)ClassJob.AST), active);

                        break;
                    }
                case ClassJob.SGE:
                    {
                        bool active = (selectedCharacter.Jobs.Sage.Level >= 70);
                        DrawJob(selectedCharacter.Jobs.Sage, ClassJob.SGE, _globalCache.JobStorage.GetName(_currentLocale,(uint)ClassJob.SGE), active);

                        break;
                    }
                case ClassJob.ARC:
                case ClassJob.BRD:
                    {
                        if (selectedCharacter.Jobs.Archer.Level >= 30)
                        {
                            DrawJob(selectedCharacter.Jobs.Bard, ClassJob.BRD, $"{_globalCache.JobStorage.GetName(_currentLocale,(uint)ClassJob.BRD)} / {_globalCache.JobStorage.GetName(_currentLocale,(uint)ClassJob.ARC)}", true);
                        }
                        else
                        {
                            bool active = (selectedCharacter.Jobs.Archer.Level > 0);
                            DrawJob(selectedCharacter.Jobs.Archer, ClassJob.ARC, _globalCache.JobStorage.GetName(_currentLocale,(uint)ClassJob.ARC), active);
                        }

                        break;
                    }
                case ClassJob.MCH:
                    {
                        bool active = (selectedCharacter.Jobs.Machinist.Level >= 30);
                        DrawJob(selectedCharacter.Jobs.Machinist, ClassJob.MCH, _globalCache.JobStorage.GetName(_currentLocale,(uint)ClassJob.MCH), active);

                        break;
                    }
                case ClassJob.DNC:
                    {
                        bool active = (selectedCharacter.Jobs.Dancer.Level >= 60);
                        DrawJob(selectedCharacter.Jobs.Dancer, ClassJob.DNC, _globalCache.JobStorage.GetName(_currentLocale,(uint)ClassJob.DNC), active);

                        break;
                    }
                case ClassJob.THM:
                case ClassJob.BLM:
                    {
                        if (selectedCharacter.Jobs.Thaumaturge.Level >= 30)
                        {
                            DrawJob(selectedCharacter.Jobs.BlackMage, ClassJob.BLM, $"{_globalCache.JobStorage.GetName(_currentLocale,(uint)ClassJob.BLM)} / {_globalCache.JobStorage.GetName(_currentLocale,(uint)ClassJob.THM)}", true);
                        }
                        else
                        {
                            bool active = (selectedCharacter.Jobs.Thaumaturge.Level > 0);
                            DrawJob(selectedCharacter.Jobs.Thaumaturge, ClassJob.THM, _globalCache.JobStorage.GetName(_currentLocale,(uint)ClassJob.THM), active);
                        }

                        break;
                    }
                case ClassJob.RDM:
                    {
                        bool active = (selectedCharacter.Jobs.RedMage.Level >= 50);
                        DrawJob(selectedCharacter.Jobs.RedMage, ClassJob.RDM, _globalCache.JobStorage.GetName(_currentLocale,(uint)ClassJob.RDM), active);

                        break;
                    }
                case ClassJob.PCT:
                    {
                        bool active = (selectedCharacter.Jobs.Pictomancer.Level >= 50);
                        DrawJob(selectedCharacter.Jobs.Pictomancer, ClassJob.PCT, _globalCache.JobStorage.GetName(_currentLocale,(uint)ClassJob.PCT), active);

                        break;
                    }
                case ClassJob.BLU:
                    {
                        bool active = (selectedCharacter.Jobs.BlueMage.Level > 0);
                        DrawJob(selectedCharacter.Jobs.BlueMage, ClassJob.BLU, _globalCache.JobStorage.GetName(_currentLocale,(uint)ClassJob.BLU), active);

                        break;
                    }
                case ClassJob.CRP:
                    {
                        bool active = (selectedCharacter.Jobs.Carpenter.Level > 0);
                        DrawJob(selectedCharacter.Jobs.Carpenter, ClassJob.CRP, _globalCache.JobStorage.GetName(_currentLocale,(uint)ClassJob.CRP), active);

                        break;
                    }
                case ClassJob.BSM:
                    {
                        bool active = (selectedCharacter.Jobs.Blacksmith.Level > 0);
                        DrawJob(selectedCharacter.Jobs.Blacksmith, ClassJob.BSM, _globalCache.JobStorage.GetName(_currentLocale,(uint)ClassJob.BSM), active);

                        break;
                    }
                case ClassJob.ARM:
                    {
                        bool active = (selectedCharacter.Jobs.Armorer.Level > 0);
                        DrawJob(selectedCharacter.Jobs.Armorer, ClassJob.ARM, _globalCache.JobStorage.GetName(_currentLocale,(uint)ClassJob.ARM), active);

                        break;
                    }
                case ClassJob.GSM:
                    {
                        bool active = (selectedCharacter.Jobs.Goldsmith.Level > 0);
                        DrawJob(selectedCharacter.Jobs.Goldsmith, ClassJob.GSM, _globalCache.JobStorage.GetName(_currentLocale,(uint)ClassJob.GSM), active);

                        break;
                    }
                case ClassJob.LTW:
                    {
                        bool active = (selectedCharacter.Jobs.Leatherworker.Level > 0);
                        DrawJob(selectedCharacter.Jobs.Leatherworker, ClassJob.LTW, _globalCache.JobStorage.GetName(_currentLocale,(uint)ClassJob.LTW), active);

                        break;
                    }
                case ClassJob.WVR:
                    {
                        bool active = (selectedCharacter.Jobs.Weaver.Level > 0);
                        DrawJob(selectedCharacter.Jobs.Weaver, ClassJob.WVR, _globalCache.JobStorage.GetName(_currentLocale,(uint)ClassJob.WVR), active);

                        break;
                    }
                case ClassJob.ALC:
                    {
                        bool active = (selectedCharacter.Jobs.Alchemist.Level > 0);
                        DrawJob(selectedCharacter.Jobs.Alchemist, ClassJob.ALC, _globalCache.JobStorage.GetName(_currentLocale,(uint)ClassJob.ALC), active);

                        break;
                    }
                case ClassJob.CUL:
                    {
                        bool active = (selectedCharacter.Jobs.Culinarian.Level > 0);
                        DrawJob(selectedCharacter.Jobs.Culinarian, ClassJob.CUL, _globalCache.JobStorage.GetName(_currentLocale,(uint)ClassJob.CUL), active);

                        break;
                    }
                case ClassJob.MIN:
                    {
                        bool active = (selectedCharacter.Jobs.Miner.Level > 0);
                        DrawJob(selectedCharacter.Jobs.Miner, ClassJob.MIN, _globalCache.JobStorage.GetName(_currentLocale,(uint)ClassJob.MIN), active);

                        break;
                    }
                case ClassJob.BTN:
                    {
                        bool active = (selectedCharacter.Jobs.Botanist.Level > 0);
                        DrawJob(selectedCharacter.Jobs.Botanist, ClassJob.BTN, _globalCache.JobStorage.GetName(_currentLocale,(uint)ClassJob.BTN), active);

                        break;
                    }
                case ClassJob.FSH:
                    {
                        bool active = (selectedCharacter.Jobs.Fisher.Level > 0);
                        DrawJob(selectedCharacter.Jobs.Fisher, ClassJob.FSH, _globalCache.JobStorage.GetName(_currentLocale,(uint)ClassJob.FSH), active);

                        break;
                    }
                case ClassJob.ADV:
                    break;
            }
        }

        private void DrawJob(Job job, ClassJob jobId, string tooltip, bool active)
        {
            //Plugin.Log.Debug($"{job_id} {tooltip} {Utils.GetJobIconWithCornerSmall((uint)job_id)}");
            Vector4 alpha = active switch
            {
                true => new Vector4(1, 1, 1, 1),
                false => new Vector4(1, 1, 1, 0.5f),
            };
            using var charactersJobsJobLine = ImRaii.Table("###CharactersJobs#JobLine", 2);
            if (!charactersJobsJobLine) return;
                ImGui.TableSetupColumn("###CharactersJobs#Icon", ImGuiTableColumnFlags.WidthFixed, 36);
                ImGui.TableSetupColumn("###CharactersJobs#LevelNameExp", ImGuiTableColumnFlags.WidthStretch);
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(Utils.GetJobIconWithCornerSmall((uint)jobId)), new Vector2(36, 36), alpha);
                ImGui.TableSetColumnIndex(1);
                using (var charactersJobsJobLevelNameExp = ImRaii.Table("###CharactersJobs#JobLevelNameExp", 2))
                {
                    if (!charactersJobsJobLevelNameExp) return;
                    ImGui.TableSetupColumn("###CharactersJobs#JobLevelNameExp#Level", ImGuiTableColumnFlags.WidthFixed, 20);
                    ImGui.TableSetupColumn("###CharactersJobs#JobLevelNameExp#NameExp", ImGuiTableColumnFlags.WidthStretch);
                    ImGui.TableNextRow();
                    ImGui.TableSetColumnIndex(0);
                    if (active)
                        ImGui.TextUnformatted($"{job.Level}");
                    else
                    {
                        ImGui.PushStyleVar(ImGuiStyleVar.Alpha, 0.5f);
                        ImGui.TextUnformatted($"{job.Level}");
                        ImGui.PopStyleVar();
                    }
                    ImGui.TableSetColumnIndex(1);
                    if (active)
                        ImGui.TextUnformatted($"{Utils.Capitalize(_globalCache.JobStorage.GetName(_currentLocale,(uint)jobId))}");
                    else
                    {
                        ImGui.PushStyleVar(ImGuiStyleVar.Alpha, 0.5f);
                        ImGui.TextUnformatted($"{Utils.Capitalize(_globalCache.JobStorage.GetName(_currentLocale,(uint)jobId))}");
                        ImGui.PopStyleVar();
                    }
                    ImGui.TableNextRow();
                    ImGui.TableSetColumnIndex(1);
                    bool maxLevel = (jobId == ClassJob.BLU) ? job.Level == 80 : job.Level == 100;
                    Utils.DrawLevelProgressBar(job.Exp, _globalCache.JobStorage.GetNextLevelExp(job.Level), tooltip, active, maxLevel);
                }
            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                ImGui.TextUnformatted(tooltip);
                ImGui.EndTooltip();
            }
        }
    }
}
