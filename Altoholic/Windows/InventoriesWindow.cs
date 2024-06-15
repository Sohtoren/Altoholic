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
using CheapLoc;
using System.Reflection.Emit;
using System.Reflection.Metadata;
using Lumina.Data.Structs;
using Dalamud.Interface.Internal;
using Dalamud.Interface.Utility;
using Dalamud.Interface;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Graphics.Render;
using System.Drawing;

namespace Altoholic.Windows;

public class InventoriesWindow : Window, IDisposable
{
    private Plugin plugin;
    private ClientLanguage currentLocale;

    public InventoriesWindow(
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
        armouryBoard = Plugin.PluginInterface.UiBuilder.LoadUld("ui/uld/ArmouryBoard.uld");
        armoryTabTextures.Add(InventoryType.ArmoryMainHand, armouryBoard.LoadTexturePart("ui/uld/ArmouryBoard_hr1.tex", 0));
        armoryTabTextures.Add(InventoryType.ArmoryHead, armouryBoard.LoadTexturePart("ui/uld/ArmouryBoard_hr1.tex", 1));
        armoryTabTextures.Add(InventoryType.ArmoryBody, armouryBoard.LoadTexturePart("ui/uld/ArmouryBoard_hr1.tex", 2));
        armoryTabTextures.Add(InventoryType.ArmoryHands, armouryBoard.LoadTexturePart("ui/uld/ArmouryBoard_hr1.tex", 3));
        armoryTabTextures.Add(InventoryType.ArmoryLegs, armouryBoard.LoadTexturePart("ui/uld/ArmouryBoard_hr1.tex", 5));
        armoryTabTextures.Add(InventoryType.ArmoryFeets, armouryBoard.LoadTexturePart("ui/uld/ArmouryBoard_hr1.tex", 6));

        armoryTabTextures.Add(InventoryType.ArmoryOffHand, armouryBoard.LoadTexturePart("ui/uld/ArmouryBoard_hr1.tex", 7));
        armoryTabTextures.Add(InventoryType.ArmoryEar, armouryBoard.LoadTexturePart("ui/uld/ArmouryBoard_hr1.tex", 8));
        armoryTabTextures.Add(InventoryType.ArmoryNeck, armouryBoard.LoadTexturePart("ui/uld/ArmouryBoard_hr1.tex", 9));
        armoryTabTextures.Add(InventoryType.ArmoryWrist, armouryBoard.LoadTexturePart("ui/uld/ArmouryBoard_hr1.tex", 10));
        armoryTabTextures.Add(InventoryType.ArmoryRings, armouryBoard.LoadTexturePart("ui/uld/ArmouryBoard_hr1.tex", 11));
        armoryTabTextures.Add(InventoryType.ArmorySoulCrystal, armouryBoard.LoadTexturePart("ui/uld/ArmouryBoard_hr1.tex", 12));
    }

    public Func<Character> GetPlayer { get; set; } = null!;
    public Func<List<Character>> GetOthersCharactersList { get; set; } = null!;
    private Character? current_character = null;
    private IEnumerable<Lumina.Excel.GeneratedSheets.Item>? current_items = null;
    private uint? current_item = null;
    private string searched_item = string.Empty;
    private Dictionary<string, List<Gear>>? selected_armory = null;

    private readonly UldWrapper armouryBoard;
    private readonly Dictionary<InventoryType, IDalamudTextureWrap?> armoryTabTextures = [];
    //private InventoryType selectedTab = InventoryType.ArmoryMainHand;
    private InventoryType? selectedTab = null;

    public override void OnClose()
    {
        Plugin.Log.Debug("InventoryWindow, OnClose() called");
        current_character = null;
        current_item = null;
        current_items = null;
        searched_item = string.Empty;
        selected_armory = null;
        selectedTab = null;
    }

    public void Dispose()
    {
        current_character = null;
        current_item = null;
        current_items = null;
        searched_item = string.Empty;
        selected_armory = null;
        selectedTab = null;

        foreach (var loadedTexture in armoryTabTextures) loadedTexture.Value?.Dispose();
        armouryBoard.Dispose();
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
            table.Dispose();
        }
        catch (Exception e)
        {
            Plugin.Log.Debug("Altoholic CharactersInventoryTable Exception : {0}", e);
        }
    }

    private void DrawAll(List<Character> chars)
    {
        if (ImGui.InputText(Utils.Capitalize(Utils.GetAddonString(3635)), ref searched_item, 512, ImGuiInputTextFlags.EnterReturnsTrue))
        {
            if (searched_item.Length >= 3)
            {
                if (searched_item == "Gil" || searched_item == "MGP" || searched_item == "MGF") return;
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
                table.Dispose();
            }
            searchItemsTable.Dispose();
        }
    }

    private void DrawInventories(Character selected_character)
    {
        using var tab = ImRaii.TabBar($"###CharactersInventoryTable#Inventories#{selected_character.Id}#TabBar");
        if (!tab) return;
        using (var inventoriesTab = ImRaii.TabItem($"{Utils.GetAddonString(520)}###CharactersInventoryTable#Inventories#{selected_character.Id}#TabBar#MainInventoriesTab"))
        {
            if (inventoriesTab.Success)
            {
                DrawMainInventories(selected_character);
            }
        };
        using (var armoryTab = ImRaii.TabItem($"{Utils.GetAddonString(1370)}###CharactersInventoryTable#Inventories#{selected_character.Id}#TabBar#ArmoryInventoryTab"))
        {
            if (armoryTab.Success)
            {
                using var table = ImRaii.Table($"###CharactersInventoryTable#Inventories#ArmoryInventoryTable#{selected_character.Id}", 3);
                if (table)
                {
                    ImGui.TableSetupColumn($"###CharactersInventoryTable#Inventories#ArmoryInventoryTable#{selected_character.Id}#Col1", ImGuiTableColumnFlags.WidthStretch);
                    ImGui.TableSetupColumn($"###CharactersInventoryTable#Inventories#ArmoryInventoryTable#{selected_character.Id}#Col2", ImGuiTableColumnFlags.WidthFixed, 600);
                    ImGui.TableSetupColumn($"###CharactersInventoryTable#Inventories#ArmoryInventoryTable#{selected_character.Id}#Col3", ImGuiTableColumnFlags.WidthStretch);
                    ImGui.TableNextRow();
                    ImGui.TableSetColumnIndex(0);
                    ImGui.TableSetColumnIndex(1);
                    DrawArmory(selected_character);
                    ImGui.TableSetColumnIndex(2);
                }
                table.Dispose();
            }
        };

        using (var glamourTab = ImRaii.TabItem($"{Utils.GetAddonString(3735)}###CharactersInventoryTable#Inventories#{selected_character.Id}#TabBar#GlamourInventoryTab"))
        {
            if (glamourTab.Success)
            {
            }
        };

        using (var armoireTab = ImRaii.TabItem($"{Utils.GetAddonString(3734)}###CharactersInventoryTable#Inventories#{selected_character.Id}#TabBar#ArmoireInventoryTab"))
        {
            if (armoireTab.Success)
            {

            }
        };

        tab.Dispose();
    }
    private void DrawMainInventories(Character selected_character)
    {
        using var table = ImRaii.Table($"###CharactersInventoryTable#Inventories#{selected_character.Id}", 2, ImGuiTableFlags.ScrollY);
        if (!table) return;

        if (selected_character.Inventory.Count != 278) return;
        List<Inventory> inventory = [.. selected_character.Inventory[..140].OrderByDescending(i => i.Quantity)];//Todo: Use ItemOrderModule
        List<Inventory> keysitems = [.. selected_character.Inventory.Slice(141, 105).OrderByDescending(k => k.Quantity)];
        bool isKeyItemEmpty = (keysitems.FindAll(k => k.Quantity == 0).Count == 105);
        //List<Inventory> crystals = selected_character.Inventory.Slice(246, 32);
        List<Inventory> saddleBag = [.. selected_character.Saddle.OrderByDescending(s => s.Quantity)];

        ImGui.TableSetupColumn($"###CharactersInventoryTable#Inventories#Bags_{selected_character.Id}", ImGuiTableColumnFlags.WidthFixed, 450);
        if (isKeyItemEmpty)
            ImGui.TableSetupColumn($"###CharactersInventoryTable#Inventories#KeyItems_{selected_character.Id}", ImGuiTableColumnFlags.WidthFixed, 180);
        else
            ImGui.TableSetupColumn($"###CharactersInventoryTable#Inventories#KeyItems_{selected_character.Id}", ImGuiTableColumnFlags.WidthStretch);

        //ImGui.TableSetupColumn($"###CharactersInventoryTable#Inventories#Crystals_{selected_character.Id}", ImGuiTableColumnFlags.WidthFixed, 180);
        ImGui.TableNextRow();
        ImGui.TableSetColumnIndex(0);
        ImGui.TextUnformatted($"{Utils.GetAddonString(520)}");
        DrawInventory($"Inventory_{selected_character.Id}", inventory);
        using var inventory_amount_table = ImRaii.Table($"###CharactersInventoryTable#Inventories#Inventory#Inventory_{selected_character.Id}#Amount", 2);
        if(inventory_amount_table)
        {
            ImGui.TableSetupColumn($"###CharactersInventoryTable#Inventories#Inventory#Inventory_{selected_character.Id}#Amount#Col1", ImGuiTableColumnFlags.WidthFixed, 390);
            ImGui.TableSetupColumn($"###CharactersInventoryTable#Inventories#Inventory#Inventory_{selected_character.Id}#Amount#Col2", ImGuiTableColumnFlags.WidthFixed, 80);
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            ImGui.Text("");
            ImGui.TableSetColumnIndex(1);
            ImGui.TextUnformatted($"{inventory.FindAll(i => i.ItemId != 0).Count}/140");
        }
        inventory_amount_table.Dispose();
        ImGui.TableSetColumnIndex(1);
        ImGui.TextUnformatted($"{Utils.GetAddonString(536)}");
        //Plugin.Log.Debug($"{selected_character.FirstName} KeyItems: {keysitems.FindAll(k => k.Quantity == 0).Count}");
        if (isKeyItemEmpty)
        {
            ImGui.TextUnformatted($"{Utils.GetAddonString(1959)}");
        }
        else
        {
            DrawKeyInventory($"Keyitem_{selected_character.Id}", keysitems);
        }
        ImGui.TextUnformatted($"{Utils.GetAddonString(2882)}");
        DrawCrystals(selected_character);

        ImGui.TableNextRow();
        ImGui.TableSetColumnIndex(0);
        ImGui.TextUnformatted($"{Utils.GetAddonString(12212)}");
        //Plugin.Log.Debug($"{selected_character.FirstName} Saddle: {selected_character.Saddle.FindAll(k => k.Quantity == 0).Count}");
        int SaddleCount = selected_character.Saddle.FindAll(s => s.Quantity == 0).Count;
        if (SaddleCount == 0 || SaddleCount == 140)
        {
            ImGui.TextUnformatted($"{Utils.GetAddonString(5448)} {Loc.Localize("OpenSaddlebag", "The Saddlebag may be empty or open to update")}");
        }
        else
        {
            DrawInventory($"Saddle_{selected_character.Id}", saddleBag, true, selected_character.HasPremiumSaddlebag);
            using var saddle_amount_table = ImRaii.Table($"###CharactersInventoryTable#Inventories#Inventory#Saddle_{selected_character.Id}#Amount", 2);
            if(saddle_amount_table)
            {
                ImGui.TableSetupColumn($"###CharactersInventoryTable#Inventories#Inventory#Saddle_{selected_character.Id}#Amount#Col1", ImGuiTableColumnFlags.WidthFixed, 390);
                ImGui.TableSetupColumn($"###CharactersInventoryTable#Inventories#Inventory#Saddle_{selected_character.Id}#Amount#Col2", ImGuiTableColumnFlags.WidthFixed, 80);
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                ImGui.Text("");
                ImGui.TableSetColumnIndex(1);
                ImGui.TextUnformatted($"{saddleBag.FindAll(i => i.ItemId != 0).Count}/{((selected_character.HasPremiumSaddlebag) ? "140" : "70")}");
            }
            saddle_amount_table.Dispose();
        }

        table.Dispose();
    }

    private void DrawInventory(string label, List<Inventory> inventory, bool saddle = false, bool premium = false)
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

        for (int i = 0; i < inventory.Count; i++)
        {
            Inventory item = inventory[i];
            if (saddle && !premium && i > 69) continue;
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
                Utils.DrawItemIcon(new Vector2(36, 36), item.HQ, item.ItemId);
                if (ImGui.IsItemHovered())
                {
                    Utils.DrawItemTooltip(item);
                }
            }
        }
        table.Dispose();
    }

    private void DrawKeyInventory(string label, List<Inventory> inventory)
    {
        int height = inventory.FindAll(i => i.ItemId != 0).ToList().Count switch
        {
            int i when i >= 0 && i <= 7 => 36,
            int i when i >= 8 && i <= 14 => 72,
            int i when i >= 15 && i <= 21 => 108,
            int i when i >= 22 && i <= 28 => 144,
            int i when i >= 29 && i <= 35 => 180,
            int i when i >= 36 && i <= 42 => 216,
            int i when i >= 43 && i <= 49 => 252,
            int i when i >= 44 && i <= 56 => 288,
            int i when i >= 57 && i <= 63 => 234,
            int i when i >= 64 && i <= 70 => 360,
            int i when i >= 71 && i <= 77 => 396,
            int i when i >= 78 && i <= 84 => 432,
            int i when i >= 85 && i <= 91 => 468,
            int i when i >= 92 && i <= 98 => 504,
            int i when i >= 99 && i <= 105 => 540,
            _ => -1
        };
        using var table = ImRaii.Table($"###CharactersInventoryTable#Inventories#Inventory#{label}Table", 7, ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg | ImGuiTableFlags.BordersInner, new Vector2(310, height));
        if (!table) return;

        ImGui.TableSetupColumn($"###CharactersInventoryTable#Inventories#Inventory#{label}#Col1", ImGuiTableColumnFlags.WidthFixed, 36);
        ImGui.TableSetupColumn($"###CharactersInventoryTable#Inventories#Inventory#{label}#Col2", ImGuiTableColumnFlags.WidthFixed, 36);
        ImGui.TableSetupColumn($"###CharactersInventoryTable#Inventories#Inventory#{label}#Col3", ImGuiTableColumnFlags.WidthFixed, 36);
        ImGui.TableSetupColumn($"###CharactersInventoryTable#Inventories#Inventory#{label}#Col4", ImGuiTableColumnFlags.WidthFixed, 36);
        ImGui.TableSetupColumn($"###CharactersInventoryTable#Inventories#Inventory#{label}#Col5", ImGuiTableColumnFlags.WidthFixed, 36);
        ImGui.TableSetupColumn($"###CharactersInventoryTable#Inventories#Inventory#{label}#Col6", ImGuiTableColumnFlags.WidthFixed, 36);
        ImGui.TableSetupColumn($"###CharactersInventoryTable#Inventories#Inventory#{label}#Col7", ImGuiTableColumnFlags.WidthFixed, 36);
        for (int i = 0; i < inventory.Count; i++)
        {
            Inventory item = inventory[i];
            if (item.ItemId == 0) continue;
            if (i == 0 ||
                i == 7 ||
                i == 14 ||
                i == 21 ||
                i == 28 ||
                i == 35 ||
                i == 42 ||
                i == 49 ||
                i == 56 ||
                i == 63 ||
                i == 70 ||
                i == 77 ||
                i == 84 ||
                i == 91 ||
                i == 98
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
                Utils.DrawEventItemIcon(new Vector2(36, 36), item.HQ, item.ItemId);
                if (ImGui.IsItemHovered())
                {
                    Utils.DrawEventItemTooltip(item);
                }
            }
        }
    }

    private void DrawCrystals(Character selected_character)
    {
        if (selected_character.Currencies is null) return;
        using var table = ImRaii.Table($"###CharactersInventoryTable#Inventories_{selected_character.Id}", 4, ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg | ImGuiTableFlags.BordersInner, new Vector2(180, 180));
        if (!table) return;
        ImGui.TableSetupColumn($"###CharactersInventoryTable#Inventories_{selected_character.Id}#Crystals#Icons", ImGuiTableColumnFlags.WidthFixed, 36);
        ImGui.TableSetupColumn($"###CharactersInventoryTable#Inventories_{selected_character.Id}#Crystals#Col1", ImGuiTableColumnFlags.WidthFixed, 36);
        ImGui.TableSetupColumn($"###CharactersInventoryTable#Inventories_{selected_character.Id}#Crystals#Col2", ImGuiTableColumnFlags.WidthFixed, 36);
        ImGui.TableSetupColumn($"###CharactersInventoryTable#Inventories_{selected_character.Id}#Crystals#Col3", ImGuiTableColumnFlags.WidthFixed, 36);
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
        Utils.DrawIcon(new Vector2(24, 24), false, 60651);
        ImGui.TableSetColumnIndex(1);
        DrawCrystal(2, selected_character.Currencies.Fire_Shard);
        ImGui.TableSetColumnIndex(2);
        DrawCrystal(8, selected_character.Currencies.Fire_Crystal);
        ImGui.TableSetColumnIndex(3);
        DrawCrystal(14, selected_character.Currencies.Fire_Cluster);

        ImGui.TableNextRow();
        ImGui.TableSetColumnIndex(0);
        Utils.DrawIcon(new Vector2(24, 24), false, 60652);
        ImGui.TableSetColumnIndex(1);
        DrawCrystal(3, selected_character.Currencies.Ice_Shard);
        ImGui.TableSetColumnIndex(2);
        DrawCrystal(9, selected_character.Currencies.Ice_Crystal);
        ImGui.TableSetColumnIndex(3);
        DrawCrystal(15, selected_character.Currencies.Ice_Cluster);

        ImGui.TableNextRow();
        ImGui.TableSetColumnIndex(0);
        Utils.DrawIcon(new Vector2(24, 24), false, 60653);
        ImGui.TableSetColumnIndex(1);
        DrawCrystal(4, selected_character.Currencies.Wind_Shard);
        ImGui.TableSetColumnIndex(2);
        DrawCrystal(10, selected_character.Currencies.Wind_Crystal);
        ImGui.TableSetColumnIndex(3);
        DrawCrystal(16, selected_character.Currencies.Wind_Cluster);

        ImGui.TableNextRow();
        ImGui.TableSetColumnIndex(0);
        Utils.DrawIcon(new Vector2(24, 24), false, 60654);
        ImGui.TableSetColumnIndex(1);
        DrawCrystal(5, selected_character.Currencies.Earth_Shard);
        ImGui.TableSetColumnIndex(2);
        DrawCrystal(11, selected_character.Currencies.Earth_Crystal);
        ImGui.TableSetColumnIndex(3);
        DrawCrystal(17, selected_character.Currencies.Earth_Cluster);

        ImGui.TableNextRow();
        ImGui.TableSetColumnIndex(0);
        Utils.DrawIcon(new Vector2(24, 24), false, 60655);
        ImGui.TableSetColumnIndex(1);
        DrawCrystal(6, selected_character.Currencies.Lightning_Shard);
        ImGui.TableSetColumnIndex(2);
        DrawCrystal(12, selected_character.Currencies.Lightning_Crystal);
        ImGui.TableSetColumnIndex(3);
        DrawCrystal(18, selected_character.Currencies.Lightning_Cluster);

        ImGui.TableNextRow();
        ImGui.TableSetColumnIndex(0);
        Utils.DrawIcon(new Vector2(24, 24), false, 60656);
        ImGui.TableSetColumnIndex(1);
        DrawCrystal(7, selected_character.Currencies.Water_Shard);
        ImGui.TableSetColumnIndex(2);
        DrawCrystal(13, selected_character.Currencies.Water_Crystal);
        ImGui.TableSetColumnIndex(3);
        DrawCrystal(19, selected_character.Currencies.Water_Cluster);

        table.Dispose();
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

    private void DrawArmory(Character selected_character)
    {
        if (selected_character.ArmoryInventory == null) return;
        using var table = ImRaii.Table($"###CharactersInventoryTable#Inventories#ArmoryInventoryTable", 3, ImGuiTableFlags.ScrollY);
        if (!table) return;

        ImGui.TableSetupColumn($"###CharactersInventoryTable#Inventories#ArmoryInventoryTable#SelectedTable#Col1", ImGuiTableColumnFlags.WidthFixed, 44);
        ImGui.TableSetupColumn($"###CharactersInventoryTable#Inventories#ArmoryInventoryTable#SelectedTable#Col2", ImGuiTableColumnFlags.WidthFixed, 275);
        ImGui.TableSetupColumn($"###CharactersInventoryTable#Inventories#ArmoryInventoryTable#SelectedTable#Col3", ImGuiTableColumnFlags.WidthFixed, 44);
        ImGui.TableNextRow();
        ImGui.TableSetColumnIndex(0);
        using var armory_left_table = ImRaii.Table("###CharactersInventoryTable#Inventories#ArmoryInventoryTable#SelectedTable#Col1Table", 1);
        if(armory_left_table)
        {
            ImGui.TableSetupColumn("###CharactersInventoryTable#Inventories#ArmoryInventoryTable#SelectedTable#Col1Table#Column", ImGuiTableColumnFlags.WidthFixed, 44);
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            DrawArmouryIcon(armoryTabTextures[InventoryType.ArmoryMainHand], Utils.GetAddonString(11524), new Vector2(44, 44), selected_character.ArmoryInventory.MainHand, InventoryType.ArmoryMainHand);

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            DrawArmouryIcon(armoryTabTextures[InventoryType.ArmoryHead], Utils.GetAddonString(11525), new Vector2(44, 44), selected_character.ArmoryInventory.Head, InventoryType.ArmoryHead);

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            DrawArmouryIcon(armoryTabTextures[InventoryType.ArmoryBody], Utils.GetAddonString(11526), new Vector2(44, 44), selected_character.ArmoryInventory.Body, InventoryType.ArmoryBody);

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            DrawArmouryIcon(armoryTabTextures[InventoryType.ArmoryHands], Utils.GetAddonString(11527), new Vector2(44, 44), selected_character.ArmoryInventory.Hands, InventoryType.ArmoryHands);

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            DrawArmouryIcon(armoryTabTextures[InventoryType.ArmoryLegs], Utils.GetAddonString(11528), new Vector2(44, 44), selected_character.ArmoryInventory.Legs, InventoryType.ArmoryLegs);

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            DrawArmouryIcon(armoryTabTextures[InventoryType.ArmoryFeets], Utils.GetAddonString(11529), new Vector2(44, 44), selected_character.ArmoryInventory.Feets, InventoryType.ArmoryFeets);
        }
        armory_left_table.Dispose();

        ImGui.TableSetColumnIndex(1);
        DrawArmoryInventory();

        ImGui.TableSetColumnIndex(2);
        using var armory_right_table = ImRaii.Table("###CharactersInventoryTable#Inventories#ArmoryInventoryTable#SelectedTable#Col3Table", 1);
        if (armory_right_table)
        {
            ImGui.TableSetupColumn("###CharactersInventoryTable#Inventories#ArmoryInventoryTable#SelectedTable#Col3Table#Column", ImGuiTableColumnFlags.WidthFixed, 44);
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            DrawArmouryIcon(armoryTabTextures[InventoryType.ArmoryOffHand], Utils.GetAddonString(11530), new Vector2(44, 44), selected_character.ArmoryInventory.OffHand, InventoryType.ArmoryOffHand);

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            DrawArmouryIcon(armoryTabTextures[InventoryType.ArmoryEar], Utils.GetAddonString(11531), new Vector2(44, 44), selected_character.ArmoryInventory.Ear, InventoryType.ArmoryEar);

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            DrawArmouryIcon(armoryTabTextures[InventoryType.ArmoryNeck], Utils.GetAddonString(11532), new Vector2(44, 44), selected_character.ArmoryInventory.Neck, InventoryType.ArmoryNeck);

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            DrawArmouryIcon(armoryTabTextures[InventoryType.ArmoryWrist], Utils.GetAddonString(11533), new Vector2(44, 44), selected_character.ArmoryInventory.Wrist, InventoryType.ArmoryWrist);

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            DrawArmouryIcon(armoryTabTextures[InventoryType.ArmoryRings], Utils.GetAddonString(11534), new Vector2(44, 44), selected_character.ArmoryInventory.Rings, InventoryType.ArmoryRings);

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            DrawArmouryIcon(armoryTabTextures[InventoryType.ArmorySoulCrystal], Utils.GetAddonString(12238), new Vector2(44, 44), selected_character.ArmoryInventory.SoulCrystal, InventoryType.ArmorySoulCrystal);
        }
        armory_right_table.Dispose();

        table.Dispose();
    }
    private void DrawArmouryIcon(IDalamudTextureWrap? texture, string tooltip, Vector2 size, List<Gear> gear, InventoryType type)
    {
        var inactiveColor = Vector4.One with { W = 0.33f };
        var activeColor = Vector4.One;
        if (texture is null) return;
        ImGui.Image(texture.ImGuiHandle, size, Vector2.Zero, Vector2.One, (selectedTab == type) ? activeColor : inactiveColor);
        if (ImGui.IsItemHovered())
        {
            ImGui.BeginTooltip();
            ImGui.TextUnformatted(tooltip);
            ImGui.EndTooltip();
        }
        if (ImGui.IsItemClicked())
        {
            Plugin.Log.Debug($"{tooltip} clicked");
            selectedTab = type;
            selected_armory = new Dictionary<string, List<Gear>> { { tooltip, gear } };
        }
        ImGui.TextUnformatted($"{gear.FindAll(i => i.ItemId != 0).Count}");
    }
    private void DrawArmoryInventory()
    {
        if (selected_armory == null || selected_armory.Count == 0) return;
        var name = selected_armory.Keys.FirstOrDefault();
        if (name == null) return;
        var inventory = selected_armory.Values.FirstOrDefault();
        if (inventory == null) return;
        inventory = [.. inventory.OrderByDescending(i => i.ItemId != 0)];
        int height = inventory.FindAll(i => i.ItemId != 0).ToList().Count switch
        {
            int i when i >= 0 && i <= 5 => 48,
            int i when i >= 6 && i <= 10 => 96,
            int i when i >= 11 && i <= 15 => 144,
            int i when i >= 16 && i <= 20 => 192,
            int i when i >= 21 && i <= 25 => 240,
            int i when i >= 26 && i <= 30 => 288,
            int i when i >= 31 && i <= 35 => 336,
            int i when i >= 36 && i <= 40 => 384,
            int i when i >= 41 && i <= 45 => 432,
            int i when i >= 46 && i <= 50 => 480,
            _ => -1
        };

        using var tableHeader = ImRaii.Table($"###CharactersInventoryTable#Inventories#ArmoryInventoryTable#{name}##Amount", 3);
        if (tableHeader)
        {
            ImGui.TableSetupColumn($"###CharactersInventoryTable#Inventories#ArmoryInventoryTable#{name}#Header#Col1", ImGuiTableColumnFlags.WidthStretch);
            ImGui.TableSetupColumn($"###CharactersInventoryTable#Inventories#ArmoryInventoryTable#{name}#Header#Col2", ImGuiTableColumnFlags.WidthFixed, 100);
            ImGui.TableSetupColumn($"###CharactersInventoryTable#Inventories#ArmoryInventoryTable#{name}#Header#Col3", ImGuiTableColumnFlags.WidthStretch);
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            ImGui.TableSetColumnIndex(1);
            ImGui.TextUnformatted($"{Utils.Capitalize(name)}");
            ImGui.TableSetColumnIndex(2);
        }
        tableHeader.Dispose();


        using var table = ImRaii.Table($"###CharactersInventoryTable#Inventories#ArmoryInventoryTable#SelectedTable", 5, ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg | ImGuiTableFlags.BordersInner | ImGuiTableFlags.ScrollY, new Vector2(266, height));
        if (table)
        {
            ImGui.TableSetupColumn($"###CharactersInventoryTable#Inventories#ArmoryInventoryTable#SelectedTable#Col1", ImGuiTableColumnFlags.WidthFixed, 44);
            ImGui.TableSetupColumn($"###CharactersInventoryTable#Inventories#ArmoryInventoryTable#SelectedTable#Col2", ImGuiTableColumnFlags.WidthFixed, 44);
            ImGui.TableSetupColumn($"###CharactersInventoryTable#Inventories#ArmoryInventoryTable#SelectedTable#Col3", ImGuiTableColumnFlags.WidthFixed, 44);
            ImGui.TableSetupColumn($"###CharactersInventoryTable#Inventories#ArmoryInventoryTable#SelectedTable#Col4", ImGuiTableColumnFlags.WidthFixed, 44);
            ImGui.TableSetupColumn($"###CharactersInventoryTable#Inventories#ArmoryInventoryTable#SelectedTable#Col5", ImGuiTableColumnFlags.WidthFixed, 44);
            for (int i = 0; i < inventory.Count; i++)
            {
                Gear gear = inventory[i];
                if (gear.ItemId == 0) continue;
                if (i == 0 ||
                    i == 5 ||
                    i == 10 ||
                    i == 15 ||
                    i == 20 ||
                    i == 25 ||
                    i == 30 ||
                    i == 35 ||
                    i == 40 ||
                    i == 45
                )
                {
                    ImGui.TableNextRow();
                }
                ImGui.TableNextColumn();
                if (gear == null || gear.ItemId == 0)
                {
                    //Utils.DrawIcon(new Vector2(36, 36), false, 653);
                    ImGui.Text("");
                    /*var list = ImGui.GetWindowDrawList();
                    list.AddRect(new Vector2(0, 0), new Vector2(36, 36),(uint)i);*/
                }
                else
                {
                    Utils.DrawItemIcon(new Vector2(44, 44), gear.HQ, gear.ItemId);
                    if (ImGui.IsItemHovered())
                    {
                        Utils.DrawGearTooltip(gear);
                    }
                }
            }
        }
        table.Dispose();

        using var tableFooter = ImRaii.Table($"###CharactersInventoryTable#Inventories#ArmoryInventoryTable#SelectedTable#{name}##Amount", 2);
        if (tableFooter)
        {
            ImGui.TableSetupColumn($"###CharactersInventoryTable#Inventories#ArmoryInventoryTable#{name}#Amount#Col1", ImGuiTableColumnFlags.WidthFixed, 220);
            ImGui.TableSetupColumn($"###CharactersInventoryTable#Inventories#ArmoryInventoryTable#{name}#Amount#Col2", ImGuiTableColumnFlags.WidthFixed, 60);
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            ImGui.Text("");
            ImGui.TableSetColumnIndex(1);
            ImGui.TextUnformatted($"{inventory.FindAll(i => i.ItemId != 0).Count}/{inventory.Count}");
        }
        tableFooter.Dispose();
    }
}