using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using Altoholic.Cache;
using Altoholic.Models;
using Dalamud;
using Dalamud.Game.Text;
using Dalamud.Interface;
using Dalamud.Interface.Internal;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game;
using ImGuiNET;

namespace Altoholic.Windows;

public class CollectionWindow : Window, IDisposable
{
    private Plugin plugin;
    private ClientLanguage currentLocale;
    private GlobalCache _globalCache;

    public CollectionWindow(
        Plugin plugin,
        string name,
        GlobalCache globalCache
        )
        : base(
        name, ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse)
    {
        this.SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(1000, 450),
            MaximumSize = new Vector2(float.MaxValue, float.MaxValue)
        };
        this.plugin = plugin;
        this._globalCache = globalCache;

        //rolesIcons = Plugin.PluginInterface.UiBuilder.LoadUld("ui/uld/fourth/ToggleButton.uld");
        //Plugin.Log.Debug($"Type: {rolesIcons.Uld?.FileInfo.Type}");
        //rolesTextures.Add(Role.Tank, rolesIcons.LoadTexturePart("ui/uld/fourth/ToggleButton_hr1.tex", 4));
        rolesTextureWrap = Plugin.TextureProvider.GetTextureFromGame("ui/uld/fourth/ToggleButton_hr1.tex");
    }

    public Func<Character>? GetPlayer { get; init; }
    public Func<List<Character>>? GetOthersCharactersList { get; set; }
    private Character? current_character = null;
    /*private readonly UldWrapper rolesIcons;
    private Dictionary<RoleIcon, IDalamudTextureWrap?> rolesTextures = [];*/
    private IDalamudTextureWrap? rolesTextureWrap = null;

    public override void OnClose()
    {
        Plugin.Log.Debug("CollectionWindow, OnClose() called");
        current_character = null;
    }

    public void Dispose()
    {
        current_character = null;
    }

    public override void Draw()
    {
        if (rolesTextureWrap is null) return;
        //if (characterIcons is null) return;
        currentLocale = plugin.Configuration.Language;
        //Utils.DrawEmptySlot(ref characterIcons, GearSlot.OH);
        using var table = ImRaii.Table("osefofthename", 1, ImGuiTableFlags.ScrollY);
        if (table == null) return;
        ImGui.TableSetupColumn("###osefofthename#Col1", ImGuiTableColumnFlags.WidthFixed, 210);
        Vector2 size = new(40, 40);
        ImGui.TableNextRow();
        ImGui.TableSetColumnIndex(0);
        Utils.DrawRoleTexture(ref rolesTextureWrap, RoleIcon.DoWDoM, size);
        ImGui.TableNextRow();
        ImGui.TableSetColumnIndex(0);
        Utils.DrawRoleTexture(ref rolesTextureWrap, RoleIcon.Tank, size);
        ImGui.TableNextRow();
        ImGui.TableSetColumnIndex(0);
        Utils.DrawRoleTexture(ref rolesTextureWrap, RoleIcon.Heal, size);
        ImGui.TableNextRow();
        ImGui.TableSetColumnIndex(0);
        Utils.DrawRoleTexture(ref rolesTextureWrap, RoleIcon.Melee, size);
        ImGui.TableNextRow();
        ImGui.TableSetColumnIndex(0);
        Utils.DrawRoleTexture(ref rolesTextureWrap, RoleIcon.Ranged, size);
        ImGui.TableNextRow();
        ImGui.TableSetColumnIndex(0);
        Utils.DrawRoleTexture(ref rolesTextureWrap, RoleIcon.Caster, size);        
        ImGui.TableNextRow();
        ImGui.TableSetColumnIndex(0);
        Utils.DrawRoleTexture(ref rolesTextureWrap, RoleIcon.DoHDoL, size);
        ImGui.TableNextRow();
        ImGui.TableSetColumnIndex(0);
        Utils.DrawRoleTexture(ref rolesTextureWrap, RoleIcon.Crafter, size);
        ImGui.TableNextRow();
        ImGui.TableSetColumnIndex(0);
        Utils.DrawRoleTexture(ref rolesTextureWrap, RoleIcon.Gatherer, size);

        /*var icon = rolesTextures[Role.Tank];
        if (icon == null)
        {
            Plugin.Log.Debug("icon is null");
        }
        else
        {
            Plugin.Log.Debug("icon is not null");
                ImGui.Image(icon.ImGuiHandle, new Vector2(64,64));
        }*/
    }
}