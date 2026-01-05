using System;
using System.Numerics;
using Altoholic.Cache;
using CheapLoc;
using Dalamud.Game;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Windowing;
using Dalamud.Bindings.ImGui;

namespace Altoholic.Windows
{
    public class ConfigWindow : Window, IDisposable
    {
        private readonly Plugin _plugin;
        private readonly Configuration _configuration;
        private readonly GlobalCache _globalCache;

        public ConfigWindow(
            Plugin plugin,
            string name,
            GlobalCache globalCache
        ) : base(
            name,
            ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoScrollbar |
            ImGuiWindowFlags.NoScrollWithMouse)
        {
            Size = new Vector2(600, 500);
            SizeCondition = ImGuiCond.Always;

            _configuration = plugin.Configuration;
            _plugin = plugin;
            _globalCache = globalCache;
        }

        private ClientLanguage _selectedLanguage;

        public void Dispose() { }

        public override void Draw()
        {
            using var table = ImRaii.Table("###ConfigTable", 2);
            if (!table) return;

            ImGui.TableSetupColumn("###ConfigTable#Config", ImGuiTableColumnFlags.WidthStretch);
            ImGui.TableSetupColumn("###ConfigTable#Credits", ImGuiTableColumnFlags.WidthStretch);
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            DrawConfig();
            ImGui.TableSetColumnIndex(1);
            DrawCredits();
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            DrawTimerConfig();
            ImGui.TableSetColumnIndex(1);
        }

        private void DrawConfig()
        {
            _selectedLanguage = _configuration.Language;
            ImGui.SetNextItemWidth(200);
            using (var langCombo =
                   ImRaii.Combo($"{_globalCache.AddonStorage.LoadAddonString(_selectedLanguage, 338)}###LangCombo",
                       _selectedLanguage.ToString()))
            {
                if (langCombo.Success)
                {
                    if (ImGui.Selectable(nameof(ClientLanguage.German),
                            nameof(ClientLanguage.German) == _selectedLanguage.ToString()))
                    {
                        _selectedLanguage = ClientLanguage.German;
                    }

                    if (ImGui.Selectable(nameof(ClientLanguage.English),
                            nameof(ClientLanguage.English) == _selectedLanguage.ToString()))
                    {
                        _selectedLanguage = ClientLanguage.English;
                    }

                    if (ImGui.Selectable(nameof(ClientLanguage.French),
                            nameof(ClientLanguage.French) == _selectedLanguage.ToString()))
                    {
                        _selectedLanguage = ClientLanguage.French;
                    }

                    if (ImGui.Selectable("日本語", nameof(ClientLanguage.Japanese) == _selectedLanguage.ToString()))
                    {
                        _selectedLanguage = ClientLanguage.Japanese;
                    }

                    _configuration.Language = _selectedLanguage;
                    _plugin.ChangeLanguage(_selectedLanguage);
                    try
                    {
                        _configuration.Save();
                    }
                    catch (Exception e)
                    {
                        Plugin.Log.Debug($"Config save error: {e}");
                    }
                }
            }

            bool isObtainedOnlyEnabled = _configuration.ObtainedOnly;
            if (ImGui.Checkbox("Obtained Only###ObtainedOnly", ref isObtainedOnlyEnabled))
            {
                _configuration.ObtainedOnly = isObtainedOnlyEnabled;
                _configuration.Save();
            }

            if (ImGui.IsItemHovered())
            {
                using var tooltip = ImRaii.Tooltip();
                if (!tooltip) return;
                ImGui.TextUnformatted(
                    $"{Loc.Localize("ObtainedOnly", "Display unobtained items, mounts, minions, etc with a non spoiler icon")}");
            }

            bool isEnabled = _configuration.IsSpoilersEnabled;
            if (ImGui.Checkbox("Enable Spoilers####EnableSpoilers", ref isEnabled))
            {
                _configuration.IsSpoilersEnabled = isEnabled;
                _configuration.Save();
            }

            if (ImGui.IsItemHovered())
            {
                using var tooltip = ImRaii.Tooltip();
                if (!tooltip) return;
                ImGui.TextUnformatted($"{Loc.Localize("Spoilers", "Display unobtained icons instead of placeholder")}");
            }

            bool isAutoSaveChatMessageEnabled = _configuration.IsAutoSaveChatMessageEnabled;
            if (ImGui.Checkbox("Enable autosave chat message####EnableAutoSaveChatMessage",
                    ref isAutoSaveChatMessageEnabled))
            {
                _configuration.IsAutoSaveChatMessageEnabled = isAutoSaveChatMessageEnabled;
                _configuration.Save();
            }

            if (ImGui.IsItemHovered())
            {
                using var tooltip = ImRaii.Tooltip();
                if (!tooltip) return;
                ImGui.TextUnformatted(
                    $"{Loc.Localize("AutoSave Chat Message", "Display a chat message when the character is (auto) saved")}");
            }

            int autoSaveTimer = _configuration.AutoSaveTimer;
            ImGui.PushItemWidth(200);
            ImGui.InputInt("Auto save Timer (max 60 mins)####AutoSaveTimer", ref autoSaveTimer);
            ImGui.PopItemWidth();
            if (autoSaveTimer > 60)
            {
                autoSaveTimer = 60;
            }

            if (autoSaveTimer < 1)
            {
                autoSaveTimer = 1;
            }

            _configuration.AutoSaveTimer = autoSaveTimer;
            _configuration.Save();
        }

        private void DrawCredits()
        {
            ImGui.TextUnformatted("Special Thanks:");
            ImGui.Separator();
            ImGui.TextUnformatted("NebulousByte (https://github.com/NebulousByte)");
            ImGui.TextUnformatted("Dalamud discord server");
        }

        private void DrawTimerConfig()
        {
            _selectedLanguage = _configuration.Language;
            ImGui.Separator();
            ImGui.TextUnformatted(Loc.Localize("ConfigEnabledTimer", "Enabled Timers"));

            /*bool isMiniCacpopEnabled = _configuration.EnabledTimers.Contains(TimersStatus.MiniCacpot);
            if (ImGui.Checkbox($"{_globalCache.AddonStorage.LoadAddonString(_selectedLanguage, 9260)}###MiniCacpot", ref isMiniCacpopEnabled))
            {
                if (isMiniCacpopEnabled)
                {
                    _configuration.EnabledTimers.Add(TimersStatus.MiniCacpot);
                }
                else
                {
                    _configuration.EnabledTimers.Remove(TimersStatus.MiniCacpot);
                }

                _configuration.Save();
            }

            bool isJumboCacpopEnabled = _configuration.EnabledTimers.Contains(TimersStatus.JumboCacpot);
            if (ImGui.Checkbox($"{_globalCache.AddonStorage.LoadAddonString(_selectedLanguage, 9272)}###JumboCacpot", ref isJumboCacpopEnabled))
            {
                if (isJumboCacpopEnabled)
                {
                    _configuration.EnabledTimers.Add(TimersStatus.JumboCacpot);
                }
                else
                {
                    _configuration.EnabledTimers.Remove(TimersStatus.JumboCacpot);
                }

                _configuration.Save();
            }

            bool isFashionReportEnabled = _configuration.EnabledTimers.Contains(TimersStatus.FashionReport);
            if (ImGui.Checkbox($"{_globalCache.AddonStorage.LoadAddonString(_selectedLanguage, 8819)}###FashionReport", ref isFashionReportEnabled))
            {
                if (isJumboCacpopEnabled)
                {
                    _configuration.EnabledTimers.Add(TimersStatus.FashionReport);
                }
                else
                {
                    _configuration.EnabledTimers.Remove(TimersStatus.FashionReport);
                }

                _configuration.Save();
            }*/

            if (_configuration.EnabledTimers is not null)
            {
                bool isCustomDeliveriesEnabled = _configuration.EnabledTimers.Contains(TimersStatus.CustomDeliveries);
                if (ImGui.Checkbox(
                        $"{_globalCache.AddonStorage.LoadAddonString(_selectedLanguage, 5700)}###CustomDeliveries",
                        ref isCustomDeliveriesEnabled))
                {
                    if (isCustomDeliveriesEnabled)
                    {
                        _configuration.EnabledTimers.Add(TimersStatus.CustomDeliveries);
                    }
                    else
                    {
                        _configuration.EnabledTimers.Remove(TimersStatus.CustomDeliveries);
                    }

                    _configuration.Save();
                }

                bool isDomanEnclaveEnabled = _configuration.EnabledTimers.Contains(TimersStatus.DomanEnclave);
                if (ImGui.Checkbox(
                        $"{_globalCache.AddonStorage.LoadAddonString(_selectedLanguage, 8821)}###DomanEnclave",
                        ref isDomanEnclaveEnabled))
                {
                    if (isDomanEnclaveEnabled)
                    {
                        _configuration.EnabledTimers.Add(TimersStatus.DomanEnclave);
                    }
                    else
                    {
                        _configuration.EnabledTimers.Remove(TimersStatus.DomanEnclave);
                    }

                    _configuration.Save();
                }

                bool isMaskedCarnivaleEnabled = _configuration.EnabledTimers.Contains(TimersStatus.MaskedCarnivale);
                if (ImGui.Checkbox(
                        $"{_globalCache.AddonStorage.LoadAddonString(_selectedLanguage, 8832)}###MaskedCarnivale",
                        ref isMaskedCarnivaleEnabled))
                {
                    if (isMaskedCarnivaleEnabled)
                    {
                        _configuration.EnabledTimers.Add(TimersStatus.MaskedCarnivale);
                    }
                    else
                    {
                        _configuration.EnabledTimers.Remove(TimersStatus.MaskedCarnivale);
                    }

                    _configuration.Save();
                }

                bool isTribeEnabled = _configuration.EnabledTimers.Contains(TimersStatus.Tribes);
                if (ImGui.Checkbox($"{_globalCache.AddonStorage.LoadAddonString(_selectedLanguage, 102515)}###Tribes",
                        ref isTribeEnabled))
                {
                    if (isTribeEnabled)
                    {
                        _configuration.EnabledTimers.Add(TimersStatus.Tribes);
                    }
                    else
                    {
                        _configuration.EnabledTimers.Remove(TimersStatus.Tribes);
                    }

                    _configuration.Save();
                }
            }

            ImGui.Separator();
            ImGui.TextUnformatted("Timer Standalone Icon");
            int iconSize = _configuration.TimerStandaloneIcon is 0 or > 100 ? 48 : _configuration.TimerStandaloneIcon;
            if (ImGui.SliderInt("Size (default 48)###TimerIconSize", ref iconSize, 15, 100))
            {
                _configuration.TimerStandaloneIcon = iconSize;
                _configuration.Save();
            }
            float iconAlpha = _configuration.TimerStandaloneIconAlpha is 0f or > 1f ? 0.5f : _configuration.TimerStandaloneIconAlpha;
            if (ImGui.SliderFloat("Transparency (default 0.5)###TimerIconAlpha", ref iconAlpha, 0.1f, 1f, "%.1f"))
            {
                _configuration.TimerStandaloneIconAlpha = iconAlpha;
                _configuration.Save();
            }
            ImGui.TextUnformatted($"{_globalCache.AddonStorage.LoadAddonString(_selectedLanguage, 14051)}:");
            Utils.DrawIcon(_globalCache.IconStorage.LoadHighResIcon(91), new Vector2(iconSize, iconSize),
                new Vector4(1, 1, 1, iconAlpha));

            /*float iconPosX = _configuration.TimerStandaloneWindowPositionX;
            ImGui.TextUnformatted("Icon position");
            if (ImGui.SliderFloat("X###TimerIconPosX", ref iconPosX, 20, ImGui.GetMainViewport().WorkSize.X - iconSize, "%.0f"))
            {
                _configuration.TimerStandaloneWindowPositionX = iconPosX;
                _configuration.Save();
            }
            float iconPosY = _configuration.TimerStandaloneWindowPositionY;
            if (ImGui.SliderFloat("Y###TimerIconPosY", ref iconPosY, 20, ImGui.GetMainViewport().WorkSize.Y - iconSize, "%.0f"))
            {
                _configuration.TimerStandaloneWindowPositionY = iconPosY;
                _configuration.Save();
            }*/
        }
    }
}