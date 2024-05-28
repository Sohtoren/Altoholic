using Altoholic.Models;
using Dalamud;
using Dalamud.Interface;
using Dalamud.Interface.Internal;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game.Fate;
using ImGuiNET;
using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Altoholic.Windows;

public class CharactersWindow : Window, IDisposable
{
    private readonly Plugin plugin;
    private readonly IPluginLog pluginLog;
    private readonly ITextureProvider textureProvider;
    private readonly ClientLanguage currentLocale;
    private readonly LiteDatabase db;

    public CharactersWindow(
        Plugin plugin,
        string name,
        IPluginLog pluginLog,
        ITextureProvider textureProvider,
        ClientLanguage currentLocale,
        LiteDatabase db
    )
        : base(name, ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse)
    {
        this.SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(1000, 450),
            MaximumSize = new Vector2(float.MaxValue, float.MaxValue)
        };
        this.db = db;
        this.plugin = plugin;
        this.pluginLog = pluginLog;
        this.textureProvider = textureProvider;
        this.currentLocale = currentLocale;
    }

    public Func<Character> GetPlayer { get; init; } = null!;
    public Func<List<Character>> GetOthersCharactersList { get; set; } = null!;
    public long TotalGils { get; set; } = 0;
    public uint TotalPlayed { get; set; } = 0;
    public int TotalCharacters { get; set; } = 0;
    public int TotalWorlds { get; set; } = 0;

    public override void Draw()
    {
        ClientStateExample();
    }

    public void Dispose()
    {
    }

    public void ClientStateExample()
    {
        try
        {
            //if (ImGui.BeginTable("Characters", 10, ImGuiTableFlags.ScrollY))
            //if (ImGui.BeginTable("Characters", 10))
            if (ImGui.BeginTable("Characters", 9))
            {
                ImGui.TableSetupColumn("Firstname", ImGuiTableColumnFlags.WidthFixed, 100);
                ImGui.TableSetupColumn("Lastname", ImGuiTableColumnFlags.WidthFixed, 100);
                ImGui.TableSetupColumn("Homeworld", ImGuiTableColumnFlags.WidthFixed, 90);
                ImGui.TableSetupColumn("DC", ImGuiTableColumnFlags.WidthFixed, 30);
                ImGui.TableSetupColumn("Level", ImGuiTableColumnFlags.WidthFixed, 30);
                ImGui.TableSetupColumn("FC", ImGuiTableColumnFlags.WidthFixed, 50);
                ImGui.TableSetupColumn("Gils", ImGuiTableColumnFlags.WidthFixed, 110);
                ImGui.TableSetupColumn("Last online", ImGuiTableColumnFlags.WidthFixed, 110);
                ImGui.TableSetupColumn("Playtime", ImGuiTableColumnFlags.WidthStretch);
                //ImGui.TableSetupColumn("Playtime", ImGuiTableColumnFlags.WidthFixed, 80);
                //ImGui.TableSetupColumn("Actions", ImGuiTableColumnFlags.WidthStretch);
                ImGui.TableHeadersRow();

                var chars = new List<Character>();
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

                ImGui.EndTable();
            }
            if (ImGui.BeginTable("Characters", 4))
            {
                ImGui.TableSetupColumn(string.Empty, ImGuiTableColumnFlags.WidthFixed, 440);
                ImGui.TableSetupColumn(string.Empty, ImGuiTableColumnFlags.WidthFixed, 110);
                ImGui.TableSetupColumn(string.Empty, ImGuiTableColumnFlags.WidthFixed, 110);
                ImGui.TableSetupColumn(string.Empty, ImGuiTableColumnFlags.WidthStretch);
                ImGui.TableNextRow();
                ImGui.TableNextColumn();
                ImGui.Separator();
                ImGui.Text($"Characters: {TotalCharacters}, Worlds: {TotalWorlds} ");
                ImGui.TableNextColumn();
                ImGui.Separator();
                ImGui.BeginTable("Gils", 2);
                    ImGui.TableSetupColumn(string.Empty, ImGuiTableColumnFlags.WidthFixed, 20);
                    ImGui.TableSetupColumn(string.Empty, ImGuiTableColumnFlags.WidthFixed, 90);
                    ImGui.TableNextRow();
                    ImGui.TableNextColumn();
                    Utils.DrawIcon(textureProvider, pluginLog, new Vector2(18, 18), false, 065002);
                    ImGui.TableNextColumn();
                    var gilText = $"{TotalGils:N0}";
                    var posX = ImGui.GetCursorPosX() + ImGui.GetColumnWidth() - ImGui.CalcTextSize(gilText.ToString()).X - ImGui.GetScrollX() - (2 * ImGui.GetStyle().ItemSpacing.X);
                    if (posX > ImGui.GetCursorPosX())
                        ImGui.SetCursorPosX(posX);
                    ImGui.Text($"{gilText}");

                // Ending Gils Table
                ImGui.EndTable();

                ImGui.TableNextColumn();
                ImGui.Separator();
                ImGui.TableNextColumn();
                ImGui.Separator();
                ImGui.Text($"{GeneratePlaytime(TimeSpan.FromMinutes(TotalPlayed))}");
                if (ImGui.IsItemHovered())
                    ImGui.SetTooltip(
                        "as of the last auto /playtime check"
                    );
                ImGui.EndTable();
            }

        }
        catch (Exception e)
        {
            pluginLog.Debug("Altoholic : Exception : {0}", e);
        }
    }
    private void DrawCharacter(int pos, Character character)
    {
        ImGui.TableNextRow();
        ImGui.TableNextColumn();
        ImGui.Text($"{character.FirstName}");
        ImGui.TableNextColumn();
        ImGui.Text($"{character.LastName}");
        ImGui.TableNextColumn();
        ImGui.Text($"{character.HomeWorld}");
        ImGui.TableNextColumn();
        ImGui.Text($"{Utils.GetRegionFromWorld(character.HomeWorld)}");
        if (ImGui.IsItemHovered())
            ImGui.SetTooltip(
                Utils.GetDatacenterFromWorld(character.HomeWorld)
            );
        ImGui.TableNextColumn();
        ImGui.Text($"{character.LastJobLevel}");
        if (ImGui.IsItemHovered())
            ImGui.SetTooltip(
                Enum.GetName(typeof(ClassJob), character.LastJob)
            );
        ImGui.TableNextColumn();
        ImGui.Text($"{character.FCTag}");
        ImGui.TableNextColumn();

            ImGui.BeginTable("Gils", 2);
                ImGui.TableSetupColumn(string.Empty, ImGuiTableColumnFlags.WidthFixed, 20);
                ImGui.TableSetupColumn(string.Empty, ImGuiTableColumnFlags.WidthFixed, 90);
                ImGui.TableNextColumn();
                Utils.DrawIcon(textureProvider, pluginLog, new Vector2(18, 18), false, 065002);
                ImGui.TableNextColumn();
                var gilText = $"{character.Currencies.Gil:N0}";
                var posX = ImGui.GetCursorPosX() + ImGui.GetColumnWidth() - ImGui.CalcTextSize(gilText.ToString()).X - ImGui.GetScrollX() - (2 * ImGui.GetStyle().ItemSpacing.X);
                if (posX > ImGui.GetCursorPosX())
                    ImGui.SetCursorPosX(posX);
                ImGui.Text($"{gilText}");
            ImGui.EndTable();// Ending Gils Table

        ImGui.TableNextColumn();
        ImGui.Text($"{GetLastOnlineFormatted(character.LastOnline/*, character.FirstName*/)}");
        if(pos > 0)
            if (ImGui.IsItemHovered())
                ImGui.SetTooltip(
                    UnixTimeStampToDateTime(character.LastOnline)
                );
        ImGui.TableNextColumn();
        ImGui.Text($"{GeneratePlaytime(TimeSpan.FromMinutes(character.PlayTime))}");
        if (character.LastPlayTimeUpdate > 0)
        {
            if (ImGui.IsItemHovered())
                ImGui.SetTooltip(
                    $"Last updated on : {UnixTimeStampToDateTime(character.LastPlayTimeUpdate)}"

                    );
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
            TimeOptions.Seconds => $"{time.TotalSeconds:n0} {"Seconds"}",
            TimeOptions.Minutes => $"{time.TotalMinutes:n0} {"Minutes"}",
            TimeOptions.Hours => $"{time.TotalHours:n2} {"Hours"}",
            TimeOptions.Days => $"{time.TotalDays:n2} {"Days"}",
            _ => GeneratePlaytimeString(time, withSeconds)
        };
    }

    private static string GeneratePlaytimeString(TimeSpan time, bool withSeconds = false)
    {
        if (time == TimeSpan.Zero)
        {
            return "No playtime found, use /playtime";
        }
        var formatted =
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
        TotalGils = characters.Select(c => c.Currencies.Gil).Sum();
        TotalPlayed = 0;
        TotalCharacters = characters.Count;
        TotalWorlds = characters.Select(c => c.HomeWorld).Distinct().Count();
        for(var i = 0; i < characters.Count;i++)
        {
            TotalPlayed += characters[i].PlayTime;
            DrawCharacter(i, characters[i]);
        }
    }

    private static string UnixTimeStampToDateTime(long lastOnline)
    {
        DateTime dateTime = new(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        dateTime = dateTime.AddSeconds(lastOnline).ToLocalTime();
        return dateTime.ToString();
    }
    private string GetLastOnlineFormatted(long lastOnline/*, string firstname*/)
    {
        var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        long diff = now - lastOnline;
        var diffDays = Math.Abs(diff / 86400);
        var diffHours = Math.Abs(diff / 3600);
        var diffMins = Math.Abs(diff / 60);

        string? time;

        //pluginLog.Debug($"{firstname} diffDays: {diffDays}, diffHours: {diffHours}, diffMins: {diffMins}");

        if (lastOnline == 0)
        {
            time = string.Format("now");
        }
        else if (diffDays > 365)
        {
            time = string.Format("Over a year ago");
        }
        else if (diffDays < 365 && diffDays > 30)
        {
            double tdiff = diffDays / 30;
            time = string.Format("{0} months ago", Math.Floor(tdiff));
        }
        else if (diffDays < 30 && diffDays > 1)
        {
            time = string.Format("{0} days ago", diffDays);
        }
        else if (diffDays == 1)
        {
            time = string.Format("A day ago");
        }
        else if (diffDays == 0 && diffHours < 24 && diffHours > 1)
        {
            time = string.Format("{0} hours ago", diffHours);
        }
        /*else if (diffDays == 0 && diffHours == 1 && diffMins > 0)
        {
            time = string.Format("An hour and {diffMins} mins")
        }*/
        //else if (diffDays == 0 && diffHours == 1 && diffMins == 0)
        else if (diffDays == 0 && diffHours == 1)
        {
            time = string.Format("One hour ago");
        }
        else if (diffDays == 0 && diffHours == 0 && diffMins < 60 && diffMins > 1)
        {
            time = string.Format("{0} minutes ago", diffMins);
        }
        else if (diffDays == 0 && diffHours == 0 && diffMins == 1 && diff == 0)
        {
            time = string.Format("One minutes ago");
        }
        else if (diffDays == 0 && diffHours == 0 && diffMins <= 1)
        {
            time = string.Format("A few seconds ago");
        }
        else
        {
            time = "Unknown";
        }

        return time;
    }     
}

public static class IEnumerableExtensions
{
    public static IEnumerable<(T item, int index)> WithIndex<T>(this IEnumerable<T> self)
       => self.Select((item, index) => (item, index));
}