using Altoholic.Cache;
using Altoholic.Models;
using CheapLoc;
using Dalamud.Bindings.ImGui;
using Dalamud.Game;
using Dalamud.Game.Text;
using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Windowing;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;

namespace Altoholic.Windows
{
    public class TimerWindow : Window, IDisposable
    {
        private readonly Plugin _plugin;
        private ClientLanguage _currentLocale;
        private readonly GlobalCache _globalCache;

        public TimerWindow(
            Plugin plugin,
            string name,
            GlobalCache globalCache)
            : base(name, ImGuiWindowFlags.AlwaysAutoResize | ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse | ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoBackground)
        {
            SizeConstraints = new WindowSizeConstraints
            {
                MinimumSize = new Vector2(40, 40),
                MaximumSize = new Vector2(float.MaxValue, float.MaxValue)
            };
            _plugin = plugin;
            _globalCache = globalCache;
        }

        public Func<Character>? GetPlayer { get; init; }
        public Func<List<Character>>? GetOthersCharactersList { get; init; }
        //public Vector2 OldWindowPosition { get; set; }

        public override void OnClose()
        {
        }

        public void Dispose()
        {
        }

        public override void Draw()
        {
            if (GetPlayer?.Invoke() == null) return;
            if (GetOthersCharactersList?.Invoke() == null) return;
            _currentLocale = _plugin.Configuration.Language;

            /*if (Position != null && (Position.Value.X != OldWindowPosition.X || Position.Value.Y != OldWindowPosition.Y))
            {
                Plugin.Log.Debug("New window position");
                _plugin.Configuration.TimerStandaloneWindowPositionX = Position.Value.X;
                _plugin.Configuration.TimerStandaloneWindowPositionY = Position.Value.Y;
                _plugin.Configuration.Save();

                OldWindowPosition = Position.Value;
            }*/

            List<Character> chars = [];
            chars.Insert(0, GetPlayer.Invoke());
            chars.AddRange(GetOthersCharactersList.Invoke());

            //Position = new Vector2(_plugin.Configuration.TimerStandaloneWindowPositionX, _plugin.Configuration.TimerStandaloneWindowPositionY);

            int iconSize = _plugin.Configuration.TimerStandaloneIcon is 0 or > 100 ? 48 : _plugin.Configuration.TimerStandaloneIcon;
            float iconAlpha = _plugin.Configuration.TimerStandaloneIconAlpha is 0f or > 1f ? 0.5f : _plugin.Configuration.TimerStandaloneIconAlpha;

            Utils.DrawIcon(_globalCache.IconStorage.LoadHighResIcon(91), new Vector2(iconSize, iconSize),
                new Vector4(1, 1, 1, iconAlpha));
            if (ImGui.IsItemHovered())
            {
                /*if (Position is not null)
                {
                    float winX = Position.Value.X;
                    float winY = Position.Value.Y;

                    if (Position?.X > ImGui.GetMainViewport().WorkSize.X * 0.80f)
                    {
                        ImGui.SetNextWindowPos(new Vector2(winX - 200, winY));
                    }
                }*/

                DrawTimers(chars, true);
            }

            //Position = null;
        }
        public void DrawNotHovered()
        {
            if (GetPlayer?.Invoke() == null) return;
            if (GetOthersCharactersList?.Invoke() == null) return;
            _currentLocale = _plugin.Configuration.Language;

            List<Character> chars = [];
            chars.Insert(0, GetPlayer.Invoke());
            chars.AddRange(GetOthersCharactersList.Invoke());
            DrawTimers(chars);
        }

        private void DrawTimers(List<Character> chars, bool drawBg = false)
        {
            if (_plugin.Configuration.EnabledTimers is null) return;
            int dateFormat = _plugin.Configuration.DateFormat;
            HashSet<TimersStatus> enabledTimers = _plugin.Configuration.EnabledTimers;
            if (chars.Count == 0) return;

            if (drawBg)
            {
                ImGui.PushStyleColor(ImGuiCol.ChildBg, new Vector4(0.15f, 0.15f, 0.15f, 1.0f));
            }

            int columns = enabledTimers.Count;

            using var charactersTimers = ImRaii.Table("###CharactersTimers#All", 1 + columns,
                ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg | ImGuiTableFlags.BordersInner |
                ImGuiTableFlags.ScrollX | ImGuiTableFlags.ScrollY);
            if (!charactersTimers) return;

            string miniCacpot = _globalCache.AddonStorage.LoadAddonString(_currentLocale, 9260).Replace(" ", "\n");
            string jumboCacpot = _globalCache.AddonStorage.LoadAddonString(_currentLocale, 9272).Replace(" ", "\n");
            string fashionReport = _globalCache.AddonStorage.LoadAddonString(_currentLocale, 8819).Replace(" ", "\n");
            string customDelivery = _globalCache.AddonStorage.LoadAddonString(_currentLocale, 5700).Replace(" ", "\n");
            string domanEnclave = (_currentLocale switch
            {
                ClientLanguage.German => "Domanischen Enklave",
                ClientLanguage.English => "Doman Enclave",
                ClientLanguage.French => "Quartier Enclavé",
                ClientLanguage.Japanese => "ドマ町人地復興",
                _ => "Doman Enclave"
            }).Replace(" ", "\n");
            string maskedFestival = (_currentLocale switch
            {
                ClientLanguage.German => "Große Maskerade",
                ClientLanguage.English => "Masked Carnivale",
                ClientLanguage.French => "Grande Mascarade",
                ClientLanguage.Japanese => "マスクカーニバル",
                _ => "Masked Carnivale"
            }).Replace(" ", "\n");
            string societalRelations = (_currentLocale switch
            {
                ClientLanguage.German => "\nStämme",
                ClientLanguage.English => "\nTribes",
                ClientLanguage.French => "\nTribus",
                ClientLanguage.Japanese => "\nTribes".ToUpper(),
                _ => "Tribes"
            }).Replace(" ", "\n");

            string longestName = chars
                .Select(c => $"{c.FirstName} {c.LastName}{(char)SeIconChar.CrossWorld}{c.HomeWorld}")
                .Aggregate("", (max, cur) => max.Length > cur.Length ? max : cur);

            ImGui.TableSetupColumn("###CharactersTimers#All#Names", ImGuiTableColumnFlags.WidthFixed,
                ImGui.CalcTextSize(longestName).X + 10);
            if (enabledTimers.Contains(TimersStatus.MiniCacpot))
            {
                ImGui.TableSetupColumn($"###CharactersTimers#All#Timer_0", ImGuiTableColumnFlags.WidthFixed,
                    ImGui.CalcTextSize(miniCacpot).X);
            }

            if (enabledTimers.Contains(TimersStatus.JumboCacpot))
            {
                ImGui.TableSetupColumn($"###CharactersTimers#All#Timer_1", ImGuiTableColumnFlags.WidthFixed,
                    ImGui.CalcTextSize(jumboCacpot).X);
            }

            if (enabledTimers.Contains(TimersStatus.FashionReport))
            {
                ImGui.TableSetupColumn($"###CharactersTimers#All#Timer_2", ImGuiTableColumnFlags.WidthFixed,
                    ImGui.CalcTextSize(fashionReport).X);
            }

            if (enabledTimers.Contains(TimersStatus.CustomDeliveries))
            {
                ImGui.TableSetupColumn($"###CharactersTimers#All#Timer_3", ImGuiTableColumnFlags.WidthFixed,
                    ImGui.CalcTextSize(customDelivery).X);
            }

            if (enabledTimers.Contains(TimersStatus.DomanEnclave))
            {
                ImGui.TableSetupColumn($"###CharactersTimers#All#Timer_4", ImGuiTableColumnFlags.WidthFixed,
                    ImGui.CalcTextSize(domanEnclave).X);
            }

            if (enabledTimers.Contains(TimersStatus.MaskedCarnivale))
            {
                ImGui.TableSetupColumn($"###CharactersTimers#All#Timer_5", ImGuiTableColumnFlags.WidthFixed,
                    ImGui.CalcTextSize(maskedFestival).X);
            }

            if (enabledTimers.Contains(TimersStatus.Tribes))
            {
                ImGui.TableSetupColumn($"###CharactersTimers#All#Timer_6", ImGuiTableColumnFlags.WidthFixed,
                    ImGui.CalcTextSize(societalRelations).X);
            }

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            if (enabledTimers.Contains(TimersStatus.MiniCacpot))
            {
                ImGui.TableNextColumn();
                ImGui.TextUnformatted(miniCacpot);
            }

            if (enabledTimers.Contains(TimersStatus.JumboCacpot))
            {
                ImGui.TableNextColumn();
                ImGui.TextUnformatted(jumboCacpot);
            }

            if (enabledTimers.Contains(TimersStatus.FashionReport))
            {
                ImGui.TableNextColumn();
                ImGui.TextUnformatted(fashionReport);
            }

            if (enabledTimers.Contains(TimersStatus.CustomDeliveries))
            {
                ImGui.TableNextColumn();
                ImGui.TextUnformatted(customDelivery);
            }

            if (enabledTimers.Contains(TimersStatus.DomanEnclave))
            {
                ImGui.TableNextColumn();
                ImGui.TextUnformatted(domanEnclave);
            }

            if (enabledTimers.Contains(TimersStatus.MaskedCarnivale))
            {
                ImGui.TableNextColumn();
                ImGui.TextUnformatted(maskedFestival);
            }

            if (enabledTimers.Contains(TimersStatus.Tribes))
            {
                ImGui.TableNextColumn();
                ImGui.TextUnformatted(societalRelations);
            }

            foreach (Character currChar in chars)
            {
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                ImGui.TextUnformatted(
                    $"{currChar.FirstName} {currChar.LastName}{(char)SeIconChar.CrossWorld}{currChar.HomeWorld}");
                if (enabledTimers.Contains(TimersStatus.MiniCacpot))
                {
                    ImGui.TableNextColumn();
                }

                if (enabledTimers.Contains(TimersStatus.JumboCacpot))
                {
                    ImGui.TableNextColumn();
                }

                if (enabledTimers.Contains(TimersStatus.FashionReport))
                {
                    ImGui.TableNextColumn();
                }

                if (enabledTimers.Contains(TimersStatus.CustomDeliveries))
                {
                    ImGui.TableNextColumn();
                    if (currChar.Timers is
                        {
                            CustomDeliveriesAllowances: not null,
                            CustomDeliveriesLastCheck: not null
                        } && currChar.Timers.CustomDeliveriesLastCheck > GetLastWeeklyReset())
                    {
                        if (currChar.Timers.CustomDeliveriesAllowances == 0)
                        {
                            ImGui.PushFont(UiBuilder.IconFont);
                            ImGui.TextUnformatted($"{FontAwesomeIcon.Check.ToIconString()}");
                            ImGui.PopFont();
                        }
                        else
                        {
                            ImGui.TextUnformatted($"{currChar.Timers.CustomDeliveriesAllowances}");
                            if (ImGui.IsItemHovered())
                            {
                                ImGui.BeginTooltip();
                                ImGui.TextUnformatted($"{Loc.Localize("LastCheck", "Last check:")} {Utils.FormatDate(dateFormat, currChar.Timers.CustomDeliveriesLastCheck.Value)}");
                                ImGui.EndTooltip();
                            }
                        }
                    }
                }

                if (enabledTimers.Contains(TimersStatus.DomanEnclave))
                {
                    ImGui.TableNextColumn();
                    if (currChar.Timers is
                        {
                            DomanEnclaveWeeklyDonation: not null, DomanEnclaveWeeklyAllowances: not null,
                            DomanEnclaveLastCheck: not null
                        } && currChar.Timers.DomanEnclaveLastCheck > GetLastWeeklyReset())
                    {
                        if (currChar.Timers.DomanEnclaveWeeklyDonation == currChar.Timers.DomanEnclaveWeeklyAllowances)
                        {
                            ImGui.PushFont(UiBuilder.IconFont);
                            ImGui.TextUnformatted($"{FontAwesomeIcon.Check.ToIconString()}");
                            ImGui.PopFont();
                        }
                        else
                        {
                            int donation = currChar.Timers.DomanEnclaveWeeklyDonation.Value;
                            string donationStr = (donation > 1000) ? $"{donation / 1_000}k" : donation.ToString();
                            int allowance = currChar.Timers.DomanEnclaveWeeklyAllowances.Value;
                            string allowanceStr = (allowance > 1000) ? $"{allowance / 1_000}k" : allowance.ToString();
                            ImGui.TextUnformatted(
                                $"{donationStr}/{allowanceStr}");
                            if (ImGui.IsItemHovered())
                            {
                                ImGui.BeginTooltip();
                                ImGui.TextUnformatted($"{donation}/{allowance}");
                                ImGui.TextUnformatted($"{Loc.Localize("LastCheck", "Last check:")} {Utils.FormatDate(dateFormat, currChar.Timers.DomanEnclaveLastCheck.Value)}");
                                ImGui.EndTooltip();
                            }
                        }
                    }
                }

                if (enabledTimers.Contains(TimersStatus.MaskedCarnivale))
                {
                    ImGui.TableNextColumn();
                    if (currChar.Timers.MaskedFestivalLastCheck is not null &&
                        currChar.Timers.MaskedFestivalLastCheck > GetLastWeeklyReset())
                    {
                        if (currChar.Timers is
                            {
                                MaskedCarnivaleNoviceChallenge: true, MaskedCarnivaleModerateChallenge: true,
                                MaskedCarnivaleAdvancedChallenge: true
                            })
                        {
                            ImGui.PushFont(UiBuilder.IconFont);
                            ImGui.TextUnformatted($"{FontAwesomeIcon.Check.ToIconString()}");
                            ImGui.PopFont();
                        }
                        else
                        {
                            ImGui.BeginGroup();
                            ImGui.PushStyleColor(ImGuiCol.Text, (currChar.Timers.MaskedCarnivaleNoviceChallenge
                                ? KnownColor.LimeGreen.Vector()
                                : KnownColor.Red.Vector()));
                            ImGui.TextUnformatted("N");
                            ImGui.PopStyleColor();
                            ImGui.SameLine();
                            ImGui.PushStyleColor(ImGuiCol.Text, (currChar.Timers.MaskedCarnivaleModerateChallenge
                                ? KnownColor.LimeGreen.Vector()
                                : KnownColor.Red.Vector()));
                            ImGui.TextUnformatted("M");
                            ImGui.PopStyleColor();
                            ImGui.SameLine();
                            ImGui.PushStyleColor(ImGuiCol.Text, (currChar.Timers.MaskedCarnivaleAdvancedChallenge
                                ? KnownColor.LimeGreen.Vector()
                                : KnownColor.Red.Vector()));
                            ImGui.TextUnformatted("A");
                            ImGui.PopStyleColor();
                            ImGui.EndGroup();
                            if (ImGui.IsItemHovered())
                            {
                                ImGui.BeginTooltip();
                                ImGui.TextUnformatted(
                                    $"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 12449)}: ");
                                if (currChar.Timers.MaskedCarnivaleNoviceChallenge)
                                {
                                    ImGui.SameLine();
                                    ImGui.TextUnformatted(
                                        $"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 12441)}");
                                }
                                else
                                {
                                    ImGui.SameLine();
                                    ImGui.TextUnformatted(
                                        $"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 12442)}");
                                }

                                ImGui.TextUnformatted(
                                    $"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 12448)}: ");
                                if (currChar.Timers.MaskedCarnivaleModerateChallenge)
                                {
                                    ImGui.SameLine();
                                    ImGui.TextUnformatted(
                                        $"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 12441)}");
                                }
                                else
                                {
                                    ImGui.SameLine();
                                    ImGui.TextUnformatted(
                                        $"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 12442)}");
                                }

                                ImGui.TextUnformatted(
                                    $"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 12447)}: ");
                                if (currChar.Timers.MaskedCarnivaleAdvancedChallenge)
                                {
                                    ImGui.SameLine();
                                    ImGui.TextUnformatted(
                                        $"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 12441)}");
                                }
                                else
                                {
                                    ImGui.SameLine();
                                    ImGui.TextUnformatted(
                                        $"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 12442)}");
                                }

                                ImGui.TextUnformatted($"{Loc.Localize("LastCheck", "Last check:")} {Utils.FormatDate(dateFormat, currChar.Timers.MaskedFestivalLastCheck.Value)}");

                                ImGui.EndTooltip();
                            }
                        }
                    }
                }

                if (enabledTimers.Contains(TimersStatus.Tribes))
                {
                    ImGui.TableNextColumn();
                    if (currChar.Timers is
                            not
                            {
                                TribeRemainingAllowances: not null,
                                TribeLastCheck: not null
                            } || !(currChar.Timers.TribeLastCheck > GetLastDailyReset()))
                    {
                        continue;
                    }

                    if (currChar.Timers.TribeRemainingAllowances == 0)
                    {
                        ImGui.PushFont(UiBuilder.IconFont);
                        ImGui.TextUnformatted($"{FontAwesomeIcon.Check.ToIconString()}");
                        ImGui.PopFont();
                    }
                    else
                    {
                        ImGui.TextUnformatted($"{currChar.Timers.TribeRemainingAllowances}");
                        if (ImGui.IsItemHovered())
                        {
                            ImGui.BeginTooltip();
                            ImGui.TextUnformatted($"{Loc.Localize("LastCheck", "Last check:")} {Utils.FormatDate(dateFormat, currChar.Timers.TribeLastCheck.Value)}");
                            ImGui.EndTooltip();
                        }
                    }
                }
            }

            //Todo: Add weekly raid for current tier
            if (drawBg)
            {
                ImGui.PopStyleColor();
            }
        }

        private static DateTime GetLastWeeklyReset()
        {
            DateTime todayUtc = DateTime.Today;
            DateTime today = DateTime.SpecifyKind(todayUtc, DateTimeKind.Utc).ToLocalTime();
            int daysSinceTuesday = ((int)today.DayOfWeek - (int)DayOfWeek.Tuesday + 7) % 7;
            DateTime lastTuesday = today.Date.AddDays(-daysSinceTuesday);
            DateTime lastTuesdayReset = lastTuesday.AddHours(8);

            return lastTuesdayReset;
        }

        private static DateTime GetLastDailyReset()
        {
            DateTime nowUtc = DateTime.Now;
            DateTime now = DateTime.SpecifyKind(nowUtc, DateTimeKind.Utc).ToLocalTime();

            DateTime todayFourPm = new(
                now.Year, now.Month, now.Day, 16, 0, 0, DateTimeKind.Utc);

            return now >= todayFourPm ? todayFourPm : todayFourPm.AddDays(-1);
        }
    }
}
