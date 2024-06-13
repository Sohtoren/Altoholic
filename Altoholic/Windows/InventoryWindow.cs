using Altoholic.Models;
using Dalamud;
using Dalamud.Game.Text;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin.Services;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Xml.Linq;

namespace Altoholic.Windows;

public class InventoryWindow : Window, IDisposable
{
    private Plugin plugin;
    private ClientLanguage currentLocale;

    public InventoryWindow(
        Plugin plugin,
        string name
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
    }

    public Func<Character> GetPlayer { get; set; } = null!;
    public Func<List<Character>> GetOthersCharactersList { get; set; } = null!;
    private Character? current_character = null;
    private IEnumerable<Lumina.Excel.GeneratedSheets.Item>? current_items = null;
    private uint? current_item = null;
    private string searched_item = string.Empty;

    public override void OnClose()
    {
        Plugin.Log.Debug("InventoryWindow, OnClose() called");
        current_character = null;
        current_item = null;
        current_items = null;
        searched_item = string.Empty;
    }

    public void Dispose()
    {
        current_character = null;
        current_item = null;
        current_items = null;
        searched_item = string.Empty;
    }

    public override void Draw()
    {
        currentLocale = plugin.Configuration.Language;
        var chars = new List<Character>();
        chars.Insert(0, GetPlayer.Invoke());
        chars.AddRange(GetOthersCharactersList.Invoke());

        try
        {
            currentLocale = plugin.Configuration.Language;
            using var table = ImRaii.Table($"###CharactersInventoryTable", 2);
            if (!table) return;

            ImGui.TableSetupColumn("###CharactersInventoryTable#CharactersListHeader", ImGuiTableColumnFlags.WidthFixed, 210);
            ImGui.TableSetupColumn("###CharactersInventoryTable#Inventories", ImGuiTableColumnFlags.WidthStretch);
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            using (var listBox = ImRaii.ListBox("###CharactersInventoryTable#CharactersListBox", new Vector2(200, -1)))
            {
                if (listBox)
                {

                    ImGui.SetScrollY(0);
                    if (chars.Count > 0)
                    {
                        if (ImGui.Selectable($"{Utils.GetAddonString(970)}###CharactersInventoryTable#CharactersListBox#All"))
                        {
                            current_character = null;
                        }

                        foreach (Character currChar in chars)
                        {
                            if (ImGui.Selectable($"{currChar.FirstName} {currChar.LastName}{(char)SeIconChar.CrossWorld}{currChar.HomeWorld}", currChar == current_character))
                            {
                                current_character = currChar;
                            }
                        }
                    }
                }
            }
            ImGui.TableSetColumnIndex(1);
            if (current_character is not null)
            {
                DrawInventories(current_character);
            }
            else
            {
                DrawAll(chars);
            }
        }
        catch (Exception e)
        {
            Plugin.Log.Debug("Altoholic CharactersInventoryTable Exception : {0}", e);
        }
    }

    private void DrawAll(List<Character> chars)
    {
        if(ImGui.InputText(Utils.Capitalize(Utils.GetAddonString(3635)), ref searched_item, 512, ImGuiInputTextFlags.EnterReturnsTrue))
        {
            if (searched_item.Length > 3)
            {
                IEnumerable<Lumina.Excel.GeneratedSheets.Item>? items = Utils.GetItemsFromName(searched_item);
                if (items != null && items.Any())
                {
                    current_items = items;
                }
            }
        }

        if (current_items != null && current_items.Any())
        {
            using var searchItemsTable = ImRaii.Table($"###CharactersInventory#All#SearchItemsTable", 2, ImGuiTableFlags.Borders);
            if (!searchItemsTable) return;
            ImGui.TableSetupColumn("###CharactersInventory#All#SearchItemsTable#Item", ImGuiTableColumnFlags.WidthFixed, 300);
            ImGui.TableSetupColumn("###CharactersInventory#All#SearchItemsTable#CharacterItems", ImGuiTableColumnFlags.WidthFixed, 350);
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            using (var itemlist = ImRaii.ListBox("###CharactersInventory#All#SearchItemsTable#Item#Listbox", new Vector2(300, -1)))
            {
                ImGui.SetScrollY(0);
                foreach (var item in current_items)
                {
                    if (ImGui.Selectable(item.Name, item.RowId == current_item))
                    {
                        current_item = item.RowId;
                    }
                }
            };

            ImGui.TableSetColumnIndex(1);
            if (current_item is not null)
            {
                using (var item_icon_table = ImRaii.Table($"###CharactersInventory#All#SearchItemsTable#CharacterItems#ItemIconName", 2, ImGuiTableFlags.NoBordersInBody))
                {
                    ImGui.TableSetupColumn("###CharactersInventory#All#SearchItemsTable#CharacterItems#ItemIconName#Icon", ImGuiTableColumnFlags.WidthFixed, 50);
                    ImGui.TableSetupColumn("###CharactersInventory#All#SearchItemsTable#CharacterItems#ItemIconName#Name", ImGuiTableColumnFlags.WidthFixed, 300);
                    ImGui.TableNextRow();
                    ImGui.TableSetColumnIndex(0);
                    Utils.DrawItemIcon(new Vector2(36, 36), false, (uint)current_item);
                    ImGui.TableSetColumnIndex(1);
                    ImGui.TextUnformatted($"{Utils.GetItemNameFromId((uint)current_item)}");
                }

                using var table = ImRaii.Table($"###CharactersInventory#All#SearchItemsTable#CharacterItems#Item#Table", 2, ImGuiTableFlags.Borders);
                if (!table) return;
                ImGui.TableSetupColumn("###CharactersInventory#All#SearchItemsTable#CharacterItems#Item#Table#CharacterName", ImGuiTableColumnFlags.WidthFixed, 300);
                ImGui.TableSetupColumn("###CharactersInventory#All#SearchItemsTable#CharacterItems#Item#Table#CharacterItem", ImGuiTableColumnFlags.WidthFixed, 50);
                List<Character> characters = chars.FindAll(c => c.Inventory.FindAll(ci => ci.ItemId == current_item).Count > 0);
                foreach (Character character in characters)
                {
                    uint total_amount = 0;
                    foreach (Inventory inv in character.Inventory.FindAll(i => i.ItemId == current_item))
                    {
                        total_amount += inv.Quantity;
                    }

                    ImGui.TableNextRow();
                    ImGui.TableSetColumnIndex(0);
                    ImGui.TextUnformatted($"{character.FirstName} {character.LastName}{(char)SeIconChar.CrossWorld}{character.HomeWorld}");
                    ImGui.TableSetColumnIndex(1);
                    ImGui.TextUnformatted($"{total_amount}");
                }
            }
        }
    }

    private void DrawInventories(Character current_character)
    {
        using var table = ImRaii.Table($"###CharactersInventoryTable#Inventories#{current_character.Id}", 3, ImGuiTableFlags.ScrollX | ImGuiTableFlags.ScrollX);
        if (!table) return;

        if (current_character.Inventory.Count != 278) return;
        List<Inventory> inventory = [.. current_character.Inventory[..140].OrderByDescending(i => i.Quantity)];//Todo: Use ItemOrderModule
        List<Inventory> keysitems = [.. current_character.Inventory.Slice(141, 105).OrderByDescending(k => k.Quantity)];
        bool isKeyItemEmpty = (keysitems.FindAll(k => k.Quantity == 0).Count == 105);
        //List<Inventory> crystals = current_character.Inventory.Slice(246, 32);

        ImGui.TableSetupColumn($"###CharactersInventoryTable#Inventories#Bags_{current_character.Id}", ImGuiTableColumnFlags.WidthFixed, 450);
        if (isKeyItemEmpty)
            ImGui.TableSetupColumn($"###CharactersInventoryTable#Inventories#KeyItems_{current_character.Id}", ImGuiTableColumnFlags.WidthFixed, 100);
        else
            ImGui.TableSetupColumn($"###CharactersInventoryTable#Inventories#KeyItems_{current_character.Id}", ImGuiTableColumnFlags.WidthStretch);

        ImGui.TableSetupColumn($"###CharactersInventoryTable#Inventories#Crystals_{current_character.Id}", ImGuiTableColumnFlags.WidthFixed, 180);
        ImGui.TableNextRow();
        ImGui.TableSetColumnIndex(0);
        ImGui.TextUnformatted($"{Utils.GetAddonString(520)}");
        DrawInventory($"Inventory_{current_character.Id}", inventory);
        ImGui.TableSetColumnIndex(1);
        ImGui.TextUnformatted($"{Utils.GetAddonString(536)}");
        Plugin.Log.Debug($"{current_character.FirstName} KeyItems: {keysitems.FindAll(k => k.Quantity == 0).Count}");
        if (isKeyItemEmpty)
        {
            ImGui.TextUnformatted($"{Utils.GetAddonString(1959)}");
        }
        else
        {
            DrawInventory($"Keyitem_{current_character.Id}", keysitems, true);
        }
        ImGui.TableSetColumnIndex(2);
        ImGui.TextUnformatted($"{Utils.GetAddonString(2882)}");
        DrawCrystals(current_character);

        ImGui.TableNextRow();
        ImGui.TableSetColumnIndex(0);
        ImGui.TextUnformatted($"{Utils.GetAddonString(12212)}");
        Plugin.Log.Debug($"{current_character.FirstName} Saddle: {current_character.Saddle.FindAll(k => k.Quantity == 0).Count}");
        int SaddleCount = current_character.Saddle.FindAll(s => s.Quantity == 0).Count;
        if (SaddleCount == 0 || SaddleCount == 140)
        {
            ImGui.TextUnformatted($"{Utils.GetAddonString(1959)}");
        }
        else
        {
            DrawInventory($"Saddle_{current_character.Id}", [..current_character.Saddle.OrderByDescending(s => s.Quantity)]);
        }
    }

    private void DrawInventory(string label, List<Inventory> inventory, bool event_item = false)
    {
        using var table = ImRaii.Table($"###CharactersInventoryTable#Inventories#Inventory#{label}Table", 10, ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg | ImGuiTableFlags.BordersInner);
        if (!table) return;

        ImGui.TableSetupColumn($"###CharactersInventoryTable#Inventories#Inventory#{label}#Col1", ImGuiTableColumnFlags.WidthFixed, 36);
        ImGui.TableSetupColumn($"###CharactersInventoryTable#Inventories#Inventory#{label}#Col2", ImGuiTableColumnFlags.WidthFixed, 36);
        ImGui.TableSetupColumn($"###CharactersInventoryTable#Inventories#Inventory#{label}#Col3", ImGuiTableColumnFlags.WidthFixed, 36);
        ImGui.TableSetupColumn($"###CharactersInventoryTable#Inventories#Inventory#{label}#Col4", ImGuiTableColumnFlags.WidthFixed, 36);
        ImGui.TableSetupColumn($"###CharactersInventoryTable#Inventories#Inventory#{label}#Col5", ImGuiTableColumnFlags.WidthFixed, 36);
        ImGui.TableSetupColumn($"###CharactersInventoryTable#Inventories#Inventory#{label}#Col6", ImGuiTableColumnFlags.WidthFixed, 36);
        ImGui.TableSetupColumn($"###CharactersInventoryTable#Inventories#Inventory#{label}#Col7", ImGuiTableColumnFlags.WidthFixed, 36);
        ImGui.TableSetupColumn($"###CharactersInventoryTable#Inventories#Inventory#{label}#Col8", ImGuiTableColumnFlags.WidthFixed, 36);
        ImGui.TableSetupColumn($"###CharactersInventoryTable#Inventories#Inventory#{label}#Col9", ImGuiTableColumnFlags.WidthFixed, 36);
        ImGui.TableSetupColumn($"###CharactersInventoryTable#Inventories#Inventory#{label}#Col10", ImGuiTableColumnFlags.WidthFixed, 36);
        /*ImGui.TableSetupColumn($"###CharactersInventoryTable#Inventories#Inventory#Col11", ImGuiTableColumnFlags.WidthStretch);
        ImGui.TableSetupColumn($"###CharactersInventoryTable#Inventories#Inventory#Col12", ImGuiTableColumnFlags.WidthStretch);
        ImGui.TableSetupColumn($"###CharactersInventoryTable#Inventories#Inventory#Col13", ImGuiTableColumnFlags.WidthStretch);
        ImGui.TableSetupColumn($"###CharactersInventoryTable#Inventories#Inventory#Col14", ImGuiTableColumnFlags.WidthStretch);*/
        for (int i = 0; i < inventory.Count; i++)
        {
            Inventory item = inventory[i];
            if (i == 0 ||
                i == 10 ||
                i == 20 ||
                i == 30 ||
                i == 40 ||
                i == 50 ||
                i == 60 ||
                i == 70 ||
                i == 80 ||
                i == 90 ||
                i == 100 ||
                i == 110 ||
                i == 120 ||
                i == 130
            )
            {
                ImGui.TableNextRow();
            }
            ImGui.TableNextColumn();
            if (item == null || item.ItemId == 0)
            {
                //Utils.DrawIcon(new Vector2(36, 36), false, 653);
                ImGui.Text("");
                /*var list = ImGui.GetWindowDrawList();
                list.AddRect(new Vector2(0, 0), new Vector2(36, 36),(uint)i);*/
            }
            else
            {
                if(event_item)
                    Utils.DrawEventItemIcon(new Vector2(36, 36), item.HQ, item.ItemId);
                else
                    Utils.DrawItemIcon(new Vector2(36, 36), item.HQ, item.ItemId);
                if (ImGui.IsItemHovered())
                {
                    Utils.DrawItemTooltip(item);
                }
            }
        }
    }

    private void DrawCrystals(Character current_character)
    {
        if (current_character.Currencies is null) return;
        using var table = ImRaii.Table($"###CharactersInventoryTable#Inventories_{current_character.Id}", 4, ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg | ImGuiTableFlags.BordersInner);
        if (!table) return;
        ImGui.TableSetupColumn($"###CharactersInventoryTable#Inventories_{current_character.Id}#Crystals#Icons", ImGuiTableColumnFlags.WidthFixed, 36);
        ImGui.TableSetupColumn($"###CharactersInventoryTable#Inventories_{current_character.Id}#Crystals#Col1", ImGuiTableColumnFlags.WidthFixed, 36);
        ImGui.TableSetupColumn($"###CharactersInventoryTable#Inventories_{current_character.Id}#Crystals#Col2", ImGuiTableColumnFlags.WidthFixed, 36);
        ImGui.TableSetupColumn($"###CharactersInventoryTable#Inventories_{current_character.Id}#Crystals#Col3", ImGuiTableColumnFlags.WidthFixed, 36);
        ImGui.TableNextRow();
        ImGui.TableSetColumnIndex(0);
        ImGui.Text("");
        ImGui.TableSetColumnIndex(1);
        Utils.DrawIcon(new Vector2(36, 36), false, 20034);
        ImGui.TableSetColumnIndex(2);
        Utils.DrawIcon(new Vector2(36, 36), false, 20019);
        ImGui.TableSetColumnIndex(3);
        Utils.DrawIcon(new Vector2(36, 36), false, 20020);

        ImGui.TableNextRow();
        ImGui.TableSetColumnIndex(0);
        Utils.DrawIcon(new Vector2(24,24), false, 60651);
        ImGui.TableSetColumnIndex(1);
        DrawCrystal(2, current_character.Currencies.Fire_Shard);
        ImGui.TableSetColumnIndex(2);
        DrawCrystal(8, current_character.Currencies.Fire_Crystal);
        ImGui.TableSetColumnIndex(3);
        DrawCrystal(14, current_character.Currencies.Fire_Cluster);

        ImGui.TableNextRow();
        ImGui.TableSetColumnIndex(0);
        Utils.DrawIcon(new Vector2(24,24), false, 60652);
        ImGui.TableSetColumnIndex(1);
        DrawCrystal(3, current_character.Currencies.Ice_Shard);
        ImGui.TableSetColumnIndex(2);
        DrawCrystal(9, current_character.Currencies.Ice_Crystal);
        ImGui.TableSetColumnIndex(3);
        DrawCrystal(15, current_character.Currencies.Ice_Cluster);

        ImGui.TableNextRow();
        ImGui.TableSetColumnIndex(0);
        Utils.DrawIcon(new Vector2(24,24), false, 60653);
        ImGui.TableSetColumnIndex(1);
        DrawCrystal(4, current_character.Currencies.Wind_Shard);
        ImGui.TableSetColumnIndex(2);
        DrawCrystal(10, current_character.Currencies.Wind_Crystal);
        ImGui.TableSetColumnIndex(3);
        DrawCrystal(16, current_character.Currencies.Wind_Cluster);

        ImGui.TableNextRow();
        ImGui.TableSetColumnIndex(0);
        Utils.DrawIcon(new Vector2(24,24), false, 60654);
        ImGui.TableSetColumnIndex(1);
        DrawCrystal(5, current_character.Currencies.Earth_Shard);
        ImGui.TableSetColumnIndex(2);
        DrawCrystal(11, current_character.Currencies.Earth_Crystal);
        ImGui.TableSetColumnIndex(3);
        DrawCrystal(17, current_character.Currencies.Earth_Cluster);

        ImGui.TableNextRow();
        ImGui.TableSetColumnIndex(0);
        Utils.DrawIcon(new Vector2(24,24), false, 60655);
        ImGui.TableSetColumnIndex(1);
        DrawCrystal(6, current_character.Currencies.Lightning_Shard);
        ImGui.TableSetColumnIndex(2);
        DrawCrystal(12, current_character.Currencies.Lightning_Crystal);
        ImGui.TableSetColumnIndex(3);
        DrawCrystal(18, current_character.Currencies.Lightning_Cluster);

        ImGui.TableNextRow();
        ImGui.TableSetColumnIndex(0);
        Utils.DrawIcon(new Vector2(24,24), false, 60656);
        ImGui.TableSetColumnIndex(1);
        DrawCrystal(7, current_character.Currencies.Water_Shard);
        ImGui.TableSetColumnIndex(2);
        DrawCrystal(13, current_character.Currencies.Water_Crystal);
        ImGui.TableSetColumnIndex(3);
        DrawCrystal(19, current_character.Currencies.Water_Cluster);
    }
    private void DrawCrystal(uint itemid, int amount)
    {
        ImGui.TextUnformatted($"{amount}");
        if (ImGui.IsItemHovered())
        {
            ImGui.BeginTooltip();
            if (amount == 0)
            {
                ImGui.TextUnformatted(Utils.GetItemNameFromId(itemid));
            }
            else
            {
                Utils.DrawCrystalTooltip(itemid, amount);
            }
            ImGui.EndTooltip();
        }
    }
}