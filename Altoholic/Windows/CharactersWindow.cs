using Altoholic.Cache;
using Altoholic.Models;
using CheapLoc;
using Dalamud;
using Dalamud.Game.Text;
using Dalamud.Interface;
using Dalamud.Interface.Internal;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Windowing;
using ImGuiNET;
using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Altoholic.Windows
{
    public class CharactersWindow : Window, IDisposable
    {
        private readonly Plugin _plugin;
        private readonly LiteDatabase _db;
        private ClientLanguage _currentLocale;
        private readonly GlobalCache _globalCache;
        public CharactersWindow(
            Plugin plugin,
            string name,
            LiteDatabase db,
            GlobalCache globalCache
        )
            : base(name, ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse)
        {
            SizeConstraints = new WindowSizeConstraints
            {
                MinimumSize = new Vector2(1000, 450),
                MaximumSize = new Vector2(float.MaxValue, float.MaxValue)
            };
            _db = db;
            _plugin = plugin;
            _globalCache = globalCache;

            GilIcon = _globalCache.IconStorage.LoadIcon(065002);
        }

        public Func<Character> GetPlayer { get; init; } = null!;
        public Func<List<Character>> GetOthersCharactersList { get; set; } = null!;
        private long TotalGils { get; set; }
        private uint TotalPlayed { get; set; }
        private int TotalCharacters { get; set; }
        private int TotalWorlds { get; set; }

        //public IDalamudTextureWrap? GilIcon { get; set; } = Utils.LoadIcon(065002);
        private IDalamudTextureWrap? GilIcon { get; }

        //private Character? current_character_last_state {  get; set; }

        /*public override bool DrawConditions()
        {
            Character? current_character = GetPlayer.Invoke();
            if (current_character is null) return false;
            Plugin.Log.Debug($"Same char? {current_character_last_state == current_character}");
            if (current_character_last_state != null && current_character_last_state == current_character)
            {
                return false;
            }

            return true;
        }*/

        public override void OnClose()
        {
            Plugin.Log.Debug("CharactersWindow, OnClose() called");
        }

        public void Dispose()
        {
        }

        public override void Draw()
        {
            _currentLocale = _plugin.Configuration.Language;
            try
            {
                //if (ImGui.BeginTable("Characters", 10, ImGuiTableFlags.ScrollY))
                //if (ImGui.BeginTable("Characters", 10))
                using (var charactersTable = ImRaii.Table("###Characters", 9))
                {
                    if (!charactersTable) return;
                    ImGui.TableSetupColumn(_globalCache.AddonStorage.LoadAddonString(_currentLocale, 330),
                        ImGuiTableColumnFlags.WidthFixed, 100);
                    if (ImGui.IsItemHovered())
                        ImGui.Text("hover");
                    ImGui.TableSetupColumn(_globalCache.AddonStorage.LoadAddonString(_currentLocale, 331),
                        ImGuiTableColumnFlags.WidthFixed, 100);
                    ImGui.TableSetupColumn(_globalCache.AddonStorage.LoadAddonString(_currentLocale, 4728),
                        ImGuiTableColumnFlags.WidthFixed, 90);
                    ImGui.TableSetupColumn(Utils.GetDCString(), ImGuiTableColumnFlags.WidthFixed, 30);
                    ImGui.TableSetupColumn(
                        _globalCache.AddonStorage.LoadAddonString(_currentLocale,
                            464) /*_globalCache.AddonStorage.LoadAddonString(335)*/, ImGuiTableColumnFlags.WidthFixed,
                        30);
                    ImGui.TableSetupColumn("FC", ImGuiTableColumnFlags.WidthFixed, 50);
                    ImGui.TableSetupColumn(_globalCache.AddonStorage.LoadAddonString(_currentLocale, 2883),
                        ImGuiTableColumnFlags.WidthFixed, 110);
                    ImGui.TableSetupColumn("Last online", ImGuiTableColumnFlags.WidthFixed, 110);
                    ImGui.TableSetupColumn($"{Loc.Localize("Playtime", "Playtime")}###Characters#Playtime",
                        ImGuiTableColumnFlags.WidthStretch);
                    //ImGui.TableSetupColumn("Playtime", ImGuiTableColumnFlags.WidthFixed, 80);
                    //ImGui.TableSetupColumn("Actions", ImGuiTableColumnFlags.WidthStretch);
                    ImGui.TableHeadersRow();

                    List<Character> chars = [];
                    chars.Insert(0, GetPlayer.Invoke());
                    chars.AddRange(
                        GetOthersCharactersList.Invoke()
                        //.OrderByDescending(c => c.LastOnline)
                        /*.OrderBy(c => c.Datacenter)
                        .ThenBy(c => c.Datacenter == current.Datacenter)
                        .ThenBy(c => c.HomeWorld == current.HomeWorld)
                        .ThenBy(c => c.FirstName)*/
                    );
                    DrawCharacters(
                        chars
                            .ToList());
                }

                using var totalCharactersTable = ImRaii.Table("###TotalCharacters", 4);
                if (!totalCharactersTable) return;
                ImGui.TableSetupColumn("###TotalCharacters#Count", ImGuiTableColumnFlags.WidthFixed, 440);
                ImGui.TableSetupColumn("###TotalCharacters#Gils", ImGuiTableColumnFlags.WidthFixed, 110);
                ImGui.TableSetupColumn("###TotalCharacters#Empty", ImGuiTableColumnFlags.WidthFixed, 110);
                ImGui.TableSetupColumn("###TotalCharacters#Playtime", ImGuiTableColumnFlags.WidthStretch);
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                ImGui.Separator();
                ImGui.TextUnformatted($"Characters: {TotalCharacters}, Worlds: {TotalWorlds}");
                ImGui.TableSetColumnIndex(1);
                ImGui.Separator();
                using (var charactersrGils = ImRaii.Table("###TotalCharacters#GilsTable", 2))
                {
                    if (!charactersrGils) return;
                    ImGui.TableSetupColumn("###TotalCharacters#GilsTable#Icon", ImGuiTableColumnFlags.WidthFixed, 20);
                    ImGui.TableSetupColumn("###TotalCharacters#GilsTable#Amount", ImGuiTableColumnFlags.WidthFixed, 90);
                    ImGui.TableNextRow();
                    ImGui.TableSetColumnIndex(0);
                    Utils.DrawIcon(GilIcon, new Vector2(18, 18));
                    ImGui.TableSetColumnIndex(1);
                    string gilText = $"{TotalGils:N0}";
                    float posX = ImGui.GetCursorPosX() + ImGui.GetColumnWidth() - ImGui.CalcTextSize(gilText).X -
                                 ImGui.GetScrollX() - (2 * ImGui.GetStyle().ItemSpacing.X);
                    if (posX > ImGui.GetCursorPosX())
                        ImGui.SetCursorPosX(posX);
                    ImGui.TextUnformatted($"{gilText}");
                } // Ending Gils Table

                ImGui.TableSetColumnIndex(2);
                ImGui.Separator();
                ImGui.TableSetColumnIndex(3);
                ImGui.Separator();
                ImGui.TextUnformatted($"{GeneratePlaytime(TimeSpan.FromMinutes(TotalPlayed))}");
                if (ImGui.IsItemHovered())
                {
                    ImGui.BeginTooltip();
                    ImGui.TextUnformatted("as of the last auto /playtime check");
                    ImGui.EndTooltip();
                }
            }
            catch (Exception e)
            {
                Plugin.Log.Debug("Altoholic : Exception : {0}", e);
            }
        }
        private void DrawCharacter(int pos, Character character)
        {
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            ImGui.TextUnformatted($"{(character.IsSprout ? (char)SeIconChar.BotanistSprout : "")}{character.FirstName}");
            ImGui.TableSetColumnIndex(1);
            ImGui.TextUnformatted($"{character.LastName}");
            ImGui.TableSetColumnIndex(2);
            ImGui.TextUnformatted($"{character.HomeWorld}");
            ImGui.TableSetColumnIndex(3);
            ImGui.TextUnformatted($"{Utils.GetRegionFromWorld(character.HomeWorld)}");
            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                ImGui.TextUnformatted(Utils.GetDatacenterFromWorld(character.HomeWorld));
                ImGui.EndTooltip();
            }
            ImGui.TableSetColumnIndex(4);
            ImGui.TextUnformatted($"{character.LastJobLevel}");
            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                ImGui.TextUnformatted(_globalCache.JobStorage.GetName(_currentLocale, character.LastJob));
                ImGui.EndTooltip();
            }
            ImGui.TableSetColumnIndex(5);
            ImGui.TextUnformatted($"{Utils.GetFCTag(_currentLocale, _globalCache, character)}");

            ImGui.TableSetColumnIndex(6);
            using (var charactersCharacterGils = ImRaii.Table($"###Characters#Character#Gils#{character.Id}", 2))
            {
                if (!charactersCharacterGils) return;
                ImGui.TableSetupColumn($"###Characters#Character#Gils#Icon#{character.Id}", ImGuiTableColumnFlags.WidthFixed, 20);
                ImGui.TableSetupColumn($"###Characters#Character#Gils#Amount#{character.Id}", ImGuiTableColumnFlags.WidthFixed, 90);
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                Utils.DrawIcon(GilIcon, new Vector2(18, 18));
                ImGui.TableSetColumnIndex(1);
                if (character.Currencies is not null)
                {
                    string gilText = $"{character.Currencies.Gil:N0}";
                    float posX = ImGui.GetCursorPosX() + ImGui.GetColumnWidth() - ImGui.CalcTextSize(gilText).X - ImGui.GetScrollX() - (2 * ImGui.GetStyle().ItemSpacing.X);
                    if (posX > ImGui.GetCursorPosX())
                        ImGui.SetCursorPosX(posX);
                    ImGui.TextUnformatted($"{gilText}");
                }
            }// Ending Gils Table

            ImGui.TableSetColumnIndex(7);
            ImGui.TextUnformatted($"{Utils.GetLastOnlineFormatted(character.LastOnline/*, character.FirstName*/)}");
            if (pos > 0)
            {
                if (ImGui.IsItemHovered())
                {
                    ImGui.BeginTooltip();
                    ImGui.TextUnformatted(Utils.UnixTimeStampToDateTime(character.LastOnline));
                    ImGui.EndTooltip();
                }
            }
            ImGui.TableSetColumnIndex(8);
            ImGui.TextUnformatted($"{GeneratePlaytime(TimeSpan.FromMinutes(character.PlayTime))}");
            if (ImGui.IsItemHovered())
            {
                if (character.LastPlayTimeUpdate <= 0)
                {
                    return;
                }
                ImGui.BeginTooltip();
                ImGui.TextUnformatted($"Last updated on : {Utils.UnixTimeStampToDateTime(character.LastPlayTimeUpdate)} - {Utils.GetLastOnlineFormatted(character.LastPlayTimeUpdate)}");
                ImGui.EndTooltip();
            }

            if (character.LastPlayTimeUpdate <= 0 || Utils.GetLastPlayTimeUpdateDiff(character.LastPlayTimeUpdate) <= 7)
            {
                return;
            }

            ImGui.SameLine();
            ImGui.TextUnformatted($"{FontAwesomeIcon.Exclamation.ToIconString()}");
            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                ImGui.TextUnformatted("More than 7 days since the last update, consider using the /playtime command");
                ImGui.EndTooltip();
            }

            // Todo : Buttons
            // Todo : 1) Details button => Send to "profile", look like the character window from XIV
            // Todo : 2) Bag Button => Bag window, show all possible bags as grids (inventory, saddle, dresser, armoire)
            // Todo : 3) Retainers => List all char's retainers and their bag (like the old altoholic main window)
            // Todo : 4) Blacklist => Delete char from altoholic and add to blacklist
            // Todo : 5) Del => Delete char from altoholic
            // 4&5 could be same button with different modifier (alt BL /ctrl delete)
            //Put back actions code here
        }

        public enum TimeOptions
        {
            Normal = 0,
            Seconds = 1,
            Minutes = 2,
            Hours = 4,
            Days = 8,
        }
        public TimeOptions TimeOption { get; set; } = TimeOptions.Normal;
        private string GeneratePlaytime(TimeSpan time, bool withSeconds = false)
        {
            return TimeOption switch
            {
                TimeOptions.Normal => GeneratePlaytimeString(time, withSeconds),
                TimeOptions.Seconds => $"{time.TotalSeconds:n0} Seconds",
                TimeOptions.Minutes => $"{time.TotalMinutes:n0} Minutes",
                TimeOptions.Hours => $"{time.TotalHours:n2} Hours",
                TimeOptions.Days => $"{time.TotalDays:n2} Days",
                _ => GeneratePlaytimeString(time, withSeconds)
            };
        }

        private static string GeneratePlaytimeString(TimeSpan time, bool withSeconds = false)
        {
            if (time == TimeSpan.Zero)
            {
                return "No playtime found, use /playtime";
            }
            string formatted =
                $"{(time.Days > 0 ? $"{time.Days:n0} {(time.Days == 1 ? "Day" : "Days")}, " : string.Empty)}" +
                $"{(time.Hours > 0 ? $"{time.Hours:n0} {(time.Hours == 1 ? "Hour" : "Hours")}, " : string.Empty)}" +
                $"{(time.Minutes > 0 ? $"{time.Minutes:n0} {(time.Minutes == 1 ? "Minute": "Minutes")}, " : string.Empty)}";

            if (withSeconds)
                formatted += $"{time.Seconds:n0} {(time.Seconds == 1 ? "Second" :  "Seconds")}";

            if (formatted.EndsWith(", "))
                formatted = formatted[..^2];

            return formatted;
        }

        private void DrawCharacters(List<Character> characters)
        {
            if(characters.Count == 0) return;
            TotalGils = characters.Select(c => c.Currencies?.Gil ?? 0).Sum();
            TotalPlayed = 0;
            TotalCharacters = characters.Count;
            TotalWorlds = characters.Select(c => c.HomeWorld).Distinct().Count();

            Character currentCharacter = characters.First();
            DrawCharacter(0, currentCharacter);
            for (int i = 1; i < characters.Count; i++)
            {
                TotalPlayed += characters[i].PlayTime;
                DrawCharacter(i, characters[i]);
            }
        }

            
    }

    public static class IEnumerableExtensions
    {
        public static IEnumerable<(T item, int index)> WithIndex<T>(this IEnumerable<T> self)
            => self.Select((item, index) => (item, index));
    }
}