using Altoholic.Cache;
using Altoholic.Models;
using CheapLoc;
using Dalamud.Game;
using Dalamud.Game.Text;
using Dalamud.Interface;
using Dalamud.Interface.Textures.TextureWraps;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Windowing;
using ImGuiNET;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Altoholic.Windows
{
    public class CharactersWindow : Window, IDisposable
    {
        private readonly Plugin _plugin;
        private readonly SqliteConnection _db;
        private ClientLanguage _currentLocale;
        private readonly GlobalCache _globalCache;
        public CharactersWindow(
            Plugin plugin,
            string name,
            SqliteConnection db,
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
        private long TotalPlayed { get; set; }
        private int TotalCharacters { get; set; }
        private int TotalWorlds { get; set; }

        //public IDalamudTextureWrap? GilIcon { get; set; } = Utils.LoadIcon(065002);
        private IDalamudTextureWrap? GilIcon { get; }

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
                using (ImRaii.IEndObject charactersTable = ImRaii.Table("###Characters", 10, ImGuiTableFlags.ScrollY, new Vector2(-1, 470)))
                //using (ImRaii.IEndObject charactersTable = ImRaii.Table("###Characters", 10))
                {
                    if (!charactersTable) return;
                    ImGui.TableSetupColumn(_globalCache.AddonStorage.LoadAddonString(_currentLocale, 330),
                        ImGuiTableColumnFlags.WidthFixed, 100);
                    ImGui.TableSetupColumn(_globalCache.AddonStorage.LoadAddonString(_currentLocale, 331),
                        ImGuiTableColumnFlags.WidthFixed, 100);
                    ImGui.TableSetupColumn(_globalCache.AddonStorage.LoadAddonString(_currentLocale, 4728),
                        ImGuiTableColumnFlags.WidthFixed, 90);
                    ImGui.TableSetupColumn(Utils.GetDCString(), ImGuiTableColumnFlags.WidthFixed, 30);
                    ImGui.TableSetupColumn(
                        _globalCache.AddonStorage.LoadAddonString(_currentLocale,
                            464) /*_globalCache.AddonStorage.LoadAddonString(335)*/, ImGuiTableColumnFlags.WidthFixed,
                        30);
                    ImGui.TableSetupColumn("FC", ImGuiTableColumnFlags.WidthFixed, 70);
                    ImGui.TableSetupColumn(_globalCache.AddonStorage.LoadAddonString(_currentLocale, 2883),
                        ImGuiTableColumnFlags.WidthFixed, 150);
                    ImGui.TableSetupColumn("Last online", ImGuiTableColumnFlags.WidthFixed, 110);
                    //ImGui.TableSetupColumn("Last online", ImGuiTableColumnFlags.WidthFixed, 80);
                    ImGui.TableSetupColumn($"{Loc.Localize("Playtime", "Playtime")}###Characters#Playtime", ImGuiTableColumnFlags.WidthStretch);
                    //ImGui.TableSetupColumn($"{Loc.Localize("Playtime", "Playtime")}###Characters#Playtime", ImGuiTableColumnFlags.WidthFixed, 200);
                    ImGui.TableSetupColumn($"{Loc.Localize("Action", "Action")}###Characters#Action", ImGuiTableColumnFlags.WidthFixed, 40);
                    ImGui.TableHeadersRow();

                    List<Character> chars = [];
                    Character p = GetPlayer.Invoke();
                    if (p.CharacterId != 0)
                    {
                        chars.Insert(0, p);
                    }
                    chars.AddRange(
                        GetOthersCharactersList.Invoke()
                    //.OrderByDescending(c => c.LastOnline)
                    /*.OrderBy(c => c.Datacenter)
                    .ThenBy(c => c.Datacenter == current.Datacenter)
                    .ThenBy(c => c.HomeWorld == current.HomeWorld)
                    .ThenBy(c => c.FirstName)*/
                    );

#if DEBUG
                    //Dummy generation
                    for (int i = 0; i < 100; i++)
                    {
                        chars.Add(new Character()
                        {
                            FirstName = $"Dummy {i}",
                            LastName = $"LN {i}",
                            HomeWorld = $"Homeworld {i}",
                            Datacenter = $"EU",
                            FCTag = $"FC {i}",
                            Currencies = new PlayerCurrencies(){ Gil = 999999999 },
                            LastOnline = 0,
                            PlayTime = 0,
                        });
                    }
#endif
                    DrawCharacters(
                        chars
                            .ToList());
                }

                using ImRaii.IEndObject totalCharactersTable = ImRaii.Table("###TotalCharacters", 4);
                if (!totalCharactersTable) return;
                ImGui.TableSetupColumn("###TotalCharacters#Count", ImGuiTableColumnFlags.WidthFixed, 440);
                ImGui.TableSetupColumn("###TotalCharacters#Gils", ImGuiTableColumnFlags.WidthFixed, 150);
                ImGui.TableSetupColumn("###TotalCharacters#Empty", ImGuiTableColumnFlags.WidthFixed, 110);
                ImGui.TableSetupColumn("###TotalCharacters#Playtime", ImGuiTableColumnFlags.WidthStretch);
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                ImGui.Separator();
                ImGui.TextUnformatted($"Characters: {TotalCharacters}, Worlds: {TotalWorlds}");
                ImGui.TableSetColumnIndex(1);
                ImGui.Separator();
                using (ImRaii.IEndObject charactersrGils = ImRaii.Table("###TotalCharacters#GilsTable", 2))
                {
                    if (!charactersrGils) return;
                    ImGui.TableSetupColumn("###TotalCharacters#GilsTable#Icon", ImGuiTableColumnFlags.WidthFixed, 20);
                    ImGui.TableSetupColumn("###TotalCharacters#GilsTable#Amount", ImGuiTableColumnFlags.WidthFixed, 130);
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
                /*if (ImGui.IsItemHovered())
                {
                    ImGui.BeginTooltip();
                    ImGui.TextUnformatted("as of the last auto /playtime check");
                    ImGui.EndTooltip();
                }*/
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
            ImGui.TextUnformatted(
                $"{(character.IsSprout ? (char)SeIconChar.BotanistSprout : "")}{character.FirstName}");
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
            using (ImRaii.IEndObject charactersCharacterGils = ImRaii.Table($"###Characters#Character#Gils#{character.CharacterId}", 2))
            {
                if (!charactersCharacterGils) return;
                ImGui.TableSetupColumn($"###Characters#Character#Gils#Icon#{character.CharacterId}",
                    ImGuiTableColumnFlags.WidthFixed, 20);
                ImGui.TableSetupColumn($"###Characters#Character#Gils#Amount#{character.CharacterId}",
                    ImGuiTableColumnFlags.WidthFixed, 130);
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                Utils.DrawIcon(GilIcon, new Vector2(18, 18));
                ImGui.TableSetColumnIndex(1);
                if (character.Currencies is not null)
                {
                    string gilText = $"{character.Currencies.Gil:N0}";
                    float posX = ImGui.GetCursorPosX() + ImGui.GetColumnWidth() - ImGui.CalcTextSize(gilText).X -
                                 ImGui.GetScrollX() - (2 * ImGui.GetStyle().ItemSpacing.X);
                    if (posX > ImGui.GetCursorPosX())
                        ImGui.SetCursorPosX(posX);
                    ImGui.TextUnformatted($"{gilText}");
                }
            } // Ending Gils Table

            ImGui.TableSetColumnIndex(7);
            ImGui.TextUnformatted($"{Utils.GetLastOnlineFormatted(character.LastOnline /*, character.FirstName*/)}");
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
            if (character.PlayTime > 0)
            {
                if (ImGui.IsItemHovered())
                {
                    if (character.LastPlayTimeUpdate <= 0)
                    {
                        return;
                    }

                    ImGui.BeginTooltip();
                    ImGui.TextUnformatted(
                        $"Last updated on : {Utils.UnixTimeStampToDateTime(character.LastPlayTimeUpdate)} - {Utils.GetLastOnlineFormatted(character.LastPlayTimeUpdate)}");
                    ImGui.EndTooltip();
                }
            }
            if (character.LastPlayTimeUpdate > 0 && Utils.GetLastPlayTimeUpdateDiff(character.LastPlayTimeUpdate) >= 7)
            {
                ImGui.SameLine();
                ImGui.PushFont(UiBuilder.IconFont);
                ImGui.TextUnformatted($"{FontAwesomeIcon.ExclamationTriangle.ToIconString()}");
                ImGui.PopFont();
                if (ImGui.IsItemHovered())
                {
                    ImGui.BeginTooltip();
                    ImGui.TextUnformatted(
                        "More than 7 days since the last update, consider using the /playtime command");
                    ImGui.EndTooltip();
                }
            }

            // Todo : Buttons
            // Todo : 1) Details button => Send to "profile", look like the character window from XIV
            // Todo : 2) Bag Button => Bag window, show all possible bags as grids (inventory, saddle, dresser, armoire)
            // Todo : 3) Retainers => List all char's retainers and their bag (like the old altoholic main window)
            // Todo : 4) Blacklist => Delete char from altoholic and add to blacklist
            // Todo : 5) Del => Delete char from altoholic
            // 4&5 could be same button with different modifier (alt BL /ctrl delete)
            //Put back actions code here

            ImGui.TableSetColumnIndex(9);

            /*using ImRaii.IEndObject characterActions = ImRaii.Table($"###CharacterActions_{character.CharacterId}", 2);
            if (!characterActions) return;
            ImGui.TableSetupColumn($"###CharacterActions_{character.CharacterId}#Blacklist", ImGuiTableColumnFlags.WidthFixed,
                20);
            ImGui.TableSetupColumn($"###CharacterActions_{character.CharacterId}#Delete", ImGuiTableColumnFlags.WidthFixed, 20);
            ImGui.TableNextRow();*/
            /**************************Blacklist**************************/
            //ImGui.TableSetColumnIndex(0);
            if (pos == 0)
            {
                return;
            }

            ImGui.PushFont(UiBuilder.IconFont);
            ImGui.TextUnformatted(FontAwesomeIcon.Ban.ToIconString());
            if (ImGui.IsItemClicked())
            {
                ImGui.OpenPopup(
                    $"Blacklist {character.FirstName} {character.LastName}{(char)SeIconChar.CrossWorld}{character.HomeWorld}###BLModal_{character.CharacterId}");
                // Todo: Add or find trigger to notify logic to delete character
                // Todo: Add or find trigger to notify logic to update others character
                Plugin.Log.Debug(
                    $"Altoholic : Blacklist button for char {character.FirstName} {character.LastName}{(char)SeIconChar.CrossWorld}{character.HomeWorld} hitted");
            }

            ImGui.PopFont();
            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                ImGui.TextUnformatted(
                    $"Blacklist {character.FirstName} {character.LastName}{(char)SeIconChar.CrossWorld}{character.HomeWorld}");
                ImGui.EndTooltip();
            }

            using (ImRaii.IEndObject blacklist = ImRaii.PopupModal($"###BLModal_{character.CharacterId}"))
            {
                if (!blacklist) return;
                ImGui.TextUnformatted("Are you sure you want to blacklist this character?");
                ImGui.TextUnformatted("This will prevent this character to be added in the future");
                ImGui.Separator();

                if (ImGui.Button("OK", new Vector2(120, 0)))
                {
                    int result = Database.Database.BlacklistCharacter(_db, character.CharacterId);
                    //SetBlacklistedCharacter(character.CharacterId);
                    //this.SetOthersCharactersList(oC);
                    Utils.ChatMessage(
                        $"{character.FirstName} {character.LastName}{(char)SeIconChar.CrossWorld}{character.HomeWorld} has been blacklisted.");
                    Utils.ChatMessage("Use /altoholicbl on this character to remove it from the blacklist");
                    Utils.ChatMessage("Close and re-open the window to refresh list");
                    ImGui.CloseCurrentPopup();
                }

                ImGui.SetItemDefaultFocus();
                ImGui.SameLine();
                if (ImGui.Button("Cancel", new Vector2(120, 0))) { ImGui.CloseCurrentPopup(); }
            }

            /**************************Delete**************************/
            /*ImGui.TableSetColumnIndex(1);
            ImGui.PushFont(UiBuilder.IconFont);
            if (ImGui.Button(FontAwesomeIcon.Times.ToIconString()))
            {
                ImGui.OpenPopup($"Delete {character.CharacterId}");

                // Todo: Add or find trigger to notify logic to delete character
                // Todo: Add or find trigger to notify logic to update others character
                // Todo: Improve this shit bc this is ugly AF
                Plugin.Log.Debug("Altoholic : Delete button for char {0} hitted", character.FirstName);
            }

            ImGui.PopFont();
            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                ImGui.TextUnformatted("Delete this character");
                ImGui.EndTooltip();
            }

            using ImRaii.IEndObject delete = ImRaii.PopupModal($"Delete {character.CharacterId}");
            if (!delete) return;
            ImGui.TextUnformatted("Are you sure you want to delete this char?");
            ImGui.TextUnformatted("It will be added again the next time you log in on this character");
            ImGui.Separator();

            if (ImGui.Button("OK", new Vector2(120, 0)))
            {
                int result = Database.Database.DeleteCharacter(_db, character.CharacterId);
                ImGui.CloseCurrentPopup();
            }

            ImGui.SetItemDefaultFocus();
            ImGui.SameLine();
            if (ImGui.Button("Cancel", new Vector2(120, 0))) { ImGui.CloseCurrentPopup(); }
            */
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
                $"{(time.Minutes > 0 ? $"{time.Minutes:n0} {(time.Minutes == 1 ? "Minute" : "Minutes")}, " : string.Empty)}";

            if (withSeconds)
                formatted += $"{time.Seconds:n0} {(time.Seconds == 1 ? "Second" : "Seconds")}";

            if (formatted.EndsWith(", "))
                formatted = formatted[..^2];

            return formatted;
        }

        private void DrawCharacters(List<Character> characters)
        {
            if (characters.Count == 0) return;
            TotalGils = characters.Select(c => c.Currencies?.Gil ?? 0).ToArray().Sum(g => (long)g);
            TotalCharacters = characters.Count;
            TotalWorlds = characters.Select(c => c.HomeWorld).Distinct().Count();
            TotalPlayed = characters.Sum(c => c.PlayTime);

            Character currentCharacter = characters.First();
            DrawCharacter(0, currentCharacter);
            for (int i = 1; i < characters.Count; i++)
            {
                DrawCharacter(i, characters[i]);
            }
        }
    }
}
