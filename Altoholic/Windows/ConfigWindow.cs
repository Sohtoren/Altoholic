using Altoholic.Cache;
using CheapLoc;
using Dalamud.Bindings.ImGui;
using Dalamud.Game;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Windowing;
using System;
using System.Numerics;

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
            Size = new Vector2(1050, 565);
            SizeCondition = ImGuiCond.Always;

            _configuration = plugin.Configuration;
            _plugin = plugin;
            _globalCache = globalCache;
        }

        private ClientLanguage _selectedLanguage;
        private int _selectedDateFormat;

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
                    _configuration.TrySave();
                }
            }

            bool isObtainedOnlyEnabled = _configuration.ObtainedOnly;
            if (ImGui.Checkbox("Obtained Only###ObtainedOnly", ref isObtainedOnlyEnabled))
            {
                _configuration.ObtainedOnly = isObtainedOnlyEnabled;
                _configuration.TrySave();
            }

            if (ImGui.IsItemHovered())
            {
                using var tooltip = ImRaii.Tooltip();
                if (!tooltip) return;
                ImGui.TextUnformatted(
                    $"{Loc.Localize("ConfigObtainedOnly", "If checked, will only display collected items in collections")}");
            }

            bool isSpoilersEnabled = _configuration.IsSpoilersEnabled;
            if (ImGui.Checkbox($"{Loc.Localize("ConfigEnableSpoilers", "Enable Spoilers")}####EnableSpoilers",
                    ref isSpoilersEnabled))
            {
                _configuration.IsSpoilersEnabled = isSpoilersEnabled;
                _configuration.TrySave();
            }

            if (ImGui.IsItemHovered())
            {
                using var tooltip = ImRaii.Tooltip();
                if (!tooltip) return;
                ImGui.TextUnformatted(
                    $"{Loc.Localize("ConfigSpoilersMessage", "Display unobtained icons instead of non spoilery placeholder")}");
            }

            bool isPlaytimeNotificationEnabled = _configuration.IsPlaytimeNotificationEnabled;
            if (ImGui.Checkbox(
                    $"{Loc.Localize("ConfigEnablePlaytimeNotification", "Enable playtime notification")}####EnableSpoilers",
                    ref isPlaytimeNotificationEnabled))
            {
                _configuration.IsPlaytimeNotificationEnabled = isPlaytimeNotificationEnabled;
                _configuration.TrySave();
            }

            if (ImGui.IsItemHovered())
            {
                using var tooltip = ImRaii.Tooltip();
                if (!tooltip) return;
                ImGui.TextUnformatted(
                    $"{Loc.Localize("ConfigPlaytimeNotificationMessage", "Display a notification in chat when /playtime hasn't been used for more than 7 days")}");
            }

            bool isAutoSaveChatMessageEnabled = _configuration.IsAutoSaveChatMessageEnabled;
            if (ImGui.Checkbox(
                    $"{Loc.Localize("ConfigEnableAutosave", "Enable autosave chat message####EnableAutoSaveChatMessage")}",
                    ref isAutoSaveChatMessageEnabled))
            {
                _configuration.IsAutoSaveChatMessageEnabled = isAutoSaveChatMessageEnabled;
                _configuration.TrySave();
            }

            if (ImGui.IsItemHovered())
            {
                using var tooltip = ImRaii.Tooltip();
                if (!tooltip) return;
                ImGui.TextUnformatted(
                    $"{Loc.Localize("ConfigAutoSaveChatMessage", "Display a chat message when the character is (auto) saved")}");
            }

            int autoSaveTimer = _configuration.AutoSaveTimer;
            ImGui.PushItemWidth(200);
            if (ImGui.SliderInt("Auto save Timer (default 5 mins)###AutoSaveTimer", ref autoSaveTimer, 1, 60))
            {
                _configuration.AutoSaveTimer = autoSaveTimer;
                _configuration.TrySave();
                
            }
            ImGui.PopItemWidth();

            switch (_selectedLanguage)
            {
                case ClientLanguage.German:
                    ImGui.TextUnformatted(" Datumsformat");
                    break;
                case ClientLanguage.English:
                    ImGui.TextUnformatted("Date format");
                    break;
                case ClientLanguage.French:
                    ImGui.TextUnformatted("Format date");
                    break;
                case ClientLanguage.Japanese:
                    ImGui.TextUnformatted("日付形式（にっぷしき）");
                    break;
                default:
                    ImGui.TextUnformatted("Date format");
                    break;
            }
            _selectedDateFormat = _configuration.DateFormat;
            DateTime dt = DateTime.Now;
            ImGui.SetNextItemWidth(200);
            using (var dateFormatCombo =
                   ImRaii.Combo($"###DateFormatCombo",
                       Utils.FormatDateString(_selectedDateFormat, dt)))
            {
                if (dateFormatCombo.Success)
                {
                    if (ImGui.Selectable($"{dt: yyyyMMdd HH:ss} (yyyymmdd)",
                            0 == _selectedDateFormat))
                    {
                        _selectedDateFormat = 0;
                    }
                    if (ImGui.Selectable($"{dt: yyyy-MM-dd HH:ss} (yyyy-mm-dd)",
                            0 == _selectedDateFormat))
                    {
                        _selectedDateFormat = 1;
                    }
                    if (ImGui.Selectable($"{dt: yyyy/MM/dd HH:ss} (yyyy/mm/dd)",
                            0 == _selectedDateFormat))
                    {
                        _selectedDateFormat = 2;
                    }
                    if (ImGui.Selectable($"{dt: ddMMyyyy HH:ss} (ddmmyyyy)",
                            0 == _selectedDateFormat))
                    {
                        _selectedDateFormat = 3;
                    }
                    if (ImGui.Selectable($"{dt: dd-MM-yyyy HH:ss}(dd-mm-yyyy)",
                            0 == _selectedDateFormat))
                    {
                        _selectedDateFormat = 4;
                    }
                    if (ImGui.Selectable($"{dt: dd/MM/yyyy HH:ss}(dd/mm/yyyy)",
                            0 == _selectedDateFormat))
                    {
                        _selectedDateFormat = 5;
                    }
                    if (ImGui.Selectable($"{dt: MMddyyyy HH:ss tt} (mmddyyyy)",
                            0 == _selectedDateFormat))
                    {
                        _selectedDateFormat = 6;
                    }
                    if (ImGui.Selectable($"{dt: MM-dd-yyyy HH:ss tt} (mm-dd-yyyy)",
                            0 == _selectedDateFormat))
                    {
                        _selectedDateFormat = 7;
                    }
                    if (ImGui.Selectable($"{dt: MM/dd/yyyy HH:ss tt} (mm/dd/yyyy)",
                            0 == _selectedDateFormat))
                    {
                        _selectedDateFormat = 8;
                    }

                    if (_selectedLanguage == ClientLanguage.Japanese)
                    {
                        if (ImGui.Selectable($"{dt: yyyy年MM月ddD日 HH時mm分}",
                                0 == _selectedDateFormat))
                        {
                            _selectedDateFormat = 20;
                        }
                    }

                    _configuration.DateFormat = _selectedDateFormat;
                    _configuration.TrySave();
                }
            }
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

                _configuration.TrySave();
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

                _configuration.TrySave();
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

                _configuration.TrySave();
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

                    _configuration.TrySave();
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

                    _configuration.TrySave();
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

                    _configuration.TrySave();
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

                    _configuration.TrySave();
                }
            }

            ImGui.Separator();
            ImGui.TextUnformatted("Timer Standalone Icon");
            int iconSize = _configuration.TimerStandaloneIcon is 0 or > 100 ? 48 : _configuration.TimerStandaloneIcon;
            if (ImGui.SliderInt("Size (default 48)###TimerIconSize", ref iconSize, 15, 100))
            {
                _configuration.TimerStandaloneIcon = iconSize;
                _configuration.TrySave();
            }
            float iconAlpha = _configuration.TimerStandaloneIconAlpha is 0f or > 1f ? 0.5f : _configuration.TimerStandaloneIconAlpha;
            if (ImGui.SliderFloat("Transparency (default 0.5)###TimerIconAlpha", ref iconAlpha, 0.1f, 1f, "%.1f"))
            {
                _configuration.TimerStandaloneIconAlpha = iconAlpha;
                _configuration.TrySave();
            }
            ImGui.TextUnformatted($"{_globalCache.AddonStorage.LoadAddonString(_selectedLanguage, 14051)}:");
            Utils.DrawIcon(_globalCache.IconStorage.LoadHighResIcon(91), new Vector2(iconSize, iconSize),
                new Vector4(1, 1, 1, iconAlpha));

            bool isTimerStandaloneAtStart = _configuration.TimerStandaloneShowAtStartup;
            if (ImGui.Checkbox($"{Loc.Localize("TimerStandaloneShowAtStart", "Show timer standalone icon on login")}###TimerIconStartup",
                    ref isTimerStandaloneAtStart))
            {
                _configuration.TimerStandaloneShowAtStartup = isTimerStandaloneAtStart;
                _configuration.TrySave();
            }
            /*float iconPosX = _configuration.TimerStandaloneWindowPositionX;
            ImGui.TextUnformatted("Icon position");
            if (ImGui.SliderFloat("X###TimerIconPosX", ref iconPosX, 20, ImGui.GetMainViewport().WorkSize.X - iconSize, "%.0f"))
            {
                _configuration.TimerStandaloneWindowPositionX = iconPosX;
                _configuration.TrySave();
            }
            float iconPosY = _configuration.TimerStandaloneWindowPositionY;
            if (ImGui.SliderFloat("Y###TimerIconPosY", ref iconPosY, 20, ImGui.GetMainViewport().WorkSize.Y - iconSize, "%.0f"))
            {
                _configuration.TimerStandaloneWindowPositionY = iconPosY;
                _configuration.TrySave();
            }*/
        }
    }
}