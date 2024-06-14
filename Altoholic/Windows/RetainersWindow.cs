using Altoholic.Models;
using CheapLoc;
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
using static FFXIVClientStructs.FFXIV.Client.Game.CurrencyManager;

namespace Altoholic.Windows;

public class RetainersWindow : Window, IDisposable
{
    private Plugin plugin;
    private ClientLanguage currentLocale;

    public RetainersWindow(
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
    private Retainer? current_retainer = null;
    private IEnumerable<Lumina.Excel.GeneratedSheets.Item>? current_items = null;
    private uint? current_item = null;
    private string searched_item = string.Empty;

    public override void OnClose()
    {
        Plugin.Log.Debug("RetainerWindow, OnClose() called");
        current_character = null;
        current_retainer = null;
        current_item = null;
        current_items = null;
        searched_item = string.Empty;
    }

    public void Dispose()
    {
        current_character = null;
        current_retainer = null;
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
            using var table = ImRaii.Table($"###CharactersRetainerTable", 2);
            if (!table) return;

            ImGui.TableSetupColumn("###CharactersRetainerTable#CharactersListHeader", ImGuiTableColumnFlags.WidthFixed, 210);
            ImGui.TableSetupColumn("###CharactersRetainerTable#Inventories", ImGuiTableColumnFlags.WidthStretch);
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            using (var listBox = ImRaii.ListBox("###CharactersRetainerTable#CharactersListBox", new Vector2(200, -1)))
            {
                if (listBox)
                {
                    ImGui.SetScrollY(0);
                    if (chars.Count > 0)
                    {
                        foreach (Character currChar in chars)
                        {
                            if (ImGui.Selectable($"{currChar.FirstName} {currChar.LastName}{(char)SeIconChar.CrossWorld}{currChar.HomeWorld}", currChar == current_character))
                            {
                                current_character = currChar;
                                current_retainer = null;
                            }
                        }
                    }
                }
            }
            ImGui.TableSetColumnIndex(1);
            if (current_character is not null)
            {
                DrawRetainers(current_character);
            }
        }
        catch (Exception e)
        {
            Plugin.Log.Debug("Altoholic CharactersRetainerTable Exception : {0}", e);
        }    
    }

    private void DrawAll(List<Retainer> retainers)
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
            using var searchItemsTable = ImRaii.Table($"###CharactersRetainer#All#SearchItemsTable", 2, ImGuiTableFlags.Borders);
            if (!searchItemsTable) return;
            ImGui.TableSetupColumn("###CharactersRetainer#All#SearchItemsTable#Item", ImGuiTableColumnFlags.WidthFixed, 300);
            ImGui.TableSetupColumn("###CharactersRetainer#All#SearchItemsTable#CharacterItems", ImGuiTableColumnFlags.WidthFixed, 350);
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            using (var itemlist = ImRaii.ListBox("###CharactersRetainer#All#SearchItemsTable#Item#Listbox", new Vector2(300, -1)))
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
                using (var item_icon_table = ImRaii.Table($"###CharactersRetainer#All#SearchItemsTable#CharacterItems#ItemIconName", 2, ImGuiTableFlags.NoBordersInBody))
                {
                    ImGui.TableSetupColumn("###CharactersRetainer#All#SearchItemsTable#CharacterItems#ItemIconName#Icon", ImGuiTableColumnFlags.WidthFixed, 50);
                    ImGui.TableSetupColumn("###CharactersRetainer#All#SearchItemsTable#CharacterItems#ItemIconName#Name", ImGuiTableColumnFlags.WidthFixed, 300);
                    ImGui.TableNextRow();
                    ImGui.TableSetColumnIndex(0);
                    Utils.DrawItemIcon(new Vector2(36, 36), false, (uint)current_item);
                    ImGui.TableSetColumnIndex(1);
                    ImGui.TextUnformatted($"{Utils.GetItemNameFromId((uint)current_item)}");
                }

                using var table = ImRaii.Table($"###CharactersRetainer#All#SearchItemsTable#CharacterItems#Item#Table", 2, ImGuiTableFlags.Borders);
                if (!table) return;
                ImGui.TableSetupColumn("###CharactersRetainer#All#SearchItemsTable#CharacterItems#Item#Table#CharacterName", ImGuiTableColumnFlags.WidthFixed, 300);
                ImGui.TableSetupColumn("###CharactersRetainer#All#SearchItemsTable#CharacterItems#Item#Table#CharacterItem", ImGuiTableColumnFlags.WidthFixed, 50);
                List<Retainer> retainers_with_items = retainers.FindAll(r => r.Inventory.FindAll(ri => ri.ItemId == current_item).Count > 0);
                Plugin.Log.Debug($"retainers_with_items: {retainers_with_items}");
                foreach (Retainer retainer in retainers_with_items)
                {
                    uint total_amount = 0;
                    foreach (Inventory inv in retainer.Inventory.FindAll(i => i.ItemId == current_item))
                    {
                        total_amount += inv.Quantity;
                    }

                    ImGui.TableNextRow();
                    ImGui.TableSetColumnIndex(0);
                    ImGui.TextUnformatted($"{retainer.Name}");
                    ImGui.TableSetColumnIndex(1);
                    ImGui.TextUnformatted($"{total_amount}");
                }
            }
        }
    }

    public void DrawRetainers(Character current_character)
    {
        currentLocale = plugin.Configuration.Language;

        try
        {
            if (current_character.Retainers.FindAll(r => r.Name != "RETAINER").Count > 0)
            {
                using var table = ImRaii.Table($"###CharactersRetainerTable", 2);
                if (!table) return;

                ImGui.TableSetupColumn("###CharactersRetainerTable#RetainersListHeader", ImGuiTableColumnFlags.WidthFixed, 110);
                ImGui.TableSetupColumn("###CharactersRetainerTable#Inventories", ImGuiTableColumnFlags.WidthStretch);
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                using (var listBox = ImRaii.ListBox("###CharactersRetainerTable#RetainerssListBox", new Vector2(200, -1)))
                {
                    if (listBox)
                    {
                        ImGui.SetScrollY(0);
                        if (current_character.Retainers.FindAll(r => r.Name != "RETAINER").Count > 0)
                        {
                            if (ImGui.Selectable($"{Utils.GetAddonString(970)}###CharactersRetainerTable#RetainerssListBox#All", current_retainer == null))
                            {
                                current_retainer = null;
                            }

                            foreach (Retainer currRetainer in current_character.Retainers)
                            {
                                if (currRetainer.Name == "RETAINER") continue;
                                if (ImGui.Selectable($"{currRetainer.Name}", currRetainer == current_retainer))
                                {
                                    current_retainer = currRetainer;
                                }
                            }
                        }
                    }
                }
                ImGui.TableSetColumnIndex(1);
                if (current_retainer is not null)
                {
                    DrawRetainer(current_retainer);
                }
                else
                {
                    if (current_character is not null)
                        DrawAll(current_character.Retainers);
                }
            }
            else
            {
                ImGui.TextUnformatted("No retainer found, please visit the retainer bell");//Todo: localization
            }
        }
        catch (Exception e)
        {
            Plugin.Log.Debug("Altoholic CharactersRetainerTable Exception : {0}", e);
        }
    }

    private void DrawRetainer(Retainer selected_retainer)
    {
        using var tab = ImRaii.TabBar($"###Retainer#{selected_retainer.Id}#TabBar");
        if (!tab) return;
        using (var detailsTab = ImRaii.TabItem($"{Utils.GetAddonString(6361)}###Retainer#{selected_retainer.Id}#TabBar#DetailsTab"))
        {
            if (detailsTab.Success)
            {
                DrawRetainerDetails(selected_retainer);
            }
        };
        using (var inventoryTab = ImRaii.TabItem($"{Utils.GetAddonString(520)}###Retainer#{selected_retainer.Id}#TabBar#InventoryTab"))
        {
            if (inventoryTab.Success)
            {
                DrawInventories(selected_retainer);
            }
        };
        if (selected_retainer.MarketItemCount > 0)
        {
            using (var marketTab = ImRaii.TabItem($"{Utils.GetAddonString(6556)}###Retainer#{selected_retainer.Id}#TabBar#MarketTab"))
            {
                if (marketTab.Success)
                {
                    DrawMarket(selected_retainer);
                }
            };
        }
    }
    private void DrawRetainerDetails(Retainer selected_retainer)
    {
        using var charactersRetainerTable = ImRaii.Table($"###CharactersRetainerTable#RetainerTable{selected_retainer.Id}", 2, ImGuiTableFlags.None, new Vector2(-1, 300));
        if (charactersRetainerTable)
        {
            ImGui.TableSetupColumn($"###CharactersRetainerTable#RetainerTable#Info_{selected_retainer.Id}", ImGuiTableColumnFlags.WidthFixed, 200);
            ImGui.TableSetupColumn($"###CharactersRetainerTable#RetainerTable#Gear_{selected_retainer.Id}", ImGuiTableColumnFlags.WidthStretch);
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            ImGui.TextUnformatted($"{selected_retainer.Name}");
            Utils.DrawIcon(new Vector2(18, 18), false, 065002); ImGui.SameLine(); ImGui.TextUnformatted($"{selected_retainer.Gils}");
            ImGui.TableSetColumnIndex(1);

            using var ventureTable = ImRaii.Table($"###CharactersRetainerTable#RetainerTable#Gear_{selected_retainer.Id}#Venture", 3, ImGuiTableFlags.None, new Vector2(-1, 50));
            if (ventureTable)
            {
                ImGui.TableSetupColumn($"###CharactersRetainerTable#RetainerTable#Gear_{selected_retainer.Id}#Venture#Col1", ImGuiTableColumnFlags.WidthStretch);
                ImGui.TableSetupColumn($"###CharactersRetainerTable#RetainerTable#Gear_{selected_retainer.Id}#Venture#Col2", ImGuiTableColumnFlags.WidthFixed, 150);
                ImGui.TableSetupColumn($"###CharactersRetainerTable#RetainerTable#Gear_{selected_retainer.Id}#Venture#Col3", ImGuiTableColumnFlags.WidthStretch);
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                ImGui.TableSetColumnIndex(1);
                ImGui.TextUnformatted($"{Utils.GetAddonString(2322)}");
                var venture = Utils.GetRetainerTask(selected_retainer.VentureID);
                if (venture != null)
                {
                    if (venture.RetainerLevel > 0)
                    {
                        ImGui.TextUnformatted($"{Utils.GetAddonString(464)} {venture.RetainerLevel}");
                    }
                    if (!venture.IsRandom)
                    {
                        var task = Utils.GetRetainerTaskNormal(venture.Task);
                        if (task != null)
                        {
                            ImGui.SameLine();
                            var item = task.Item.Value;
                            if (item != null)
                                ImGui.TextUnformatted($"{Utils.GetItemNameFromId(item.RowId)}");
                        }
                    }
                    else
                    {
                        var task = Utils.GetRetainerTaskRandom(venture.Task);
                        if (task != null)
                        {
                            ImGui.SameLine();
                            ImGui.TextUnformatted($"{task.Name}");
                        }
                    }
                }
                ImGui.TableSetColumnIndex(2);
            }
            ventureTable.Dispose();

            using var gearTable = ImRaii.Table($"###CharactersRetainerTable#RetainerTable#Gear_{selected_retainer.Id}#Gear", 3, ImGuiTableFlags.None, new Vector2(-1, 50));
            if (gearTable)
            {
                ImGui.TableSetupColumn($"###CharactersRetainerTable#RetainerTable#Gear_{selected_retainer.Id}#Gear#Col1", ImGuiTableColumnFlags.WidthStretch);
                ImGui.TableSetupColumn($"###CharactersRetainerTable#RetainerTable#Gear_{selected_retainer.Id}#Gear#Col2", ImGuiTableColumnFlags.WidthFixed, 300);
                ImGui.TableSetupColumn($"###CharactersRetainerTable#RetainerTable#Gear_{selected_retainer.Id}#Gear#Col3", ImGuiTableColumnFlags.WidthStretch);
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                ImGui.TableSetColumnIndex(1);
                if (selected_retainer.ClassJob != 0 && selected_retainer.Gear.Count > 0)
                {
                    Utils.DrawGear(selected_retainer.Gear, selected_retainer.ClassJob, selected_retainer.Level, 200, 180, true, Utils.GetRetainerJobMaxLevel((uint)selected_retainer.ClassJob,GetPlayer.Invoke()));
                }
                else
                {
                    ImGui.TextUnformatted($"{Utils.GetAddonString(5448)} {Loc.Localize("OpenRetainer", "Open the retainer to update the gear")}");
                }

            }
            gearTable.Dispose();
        }
        charactersRetainerTable.Dispose();
    }

    private void DrawInventories(Retainer selected_retainer)
    {
        if (selected_retainer.Inventory.Count != 193)
        {
            ImGui.TextUnformatted($"{Utils.GetAddonString(5448)} {Loc.Localize("OpenRetainer", "Open the retainer to update")}");
        }
        else
        {
            List<Inventory> inventory = [.. selected_retainer.Inventory[..175].OrderByDescending(i => i.Quantity)];//Todo: Use ItemOrderModule
            using var table = ImRaii.Table($"###CharactersRetainerTable#Inventories#{selected_retainer.Id}", 2, ImGuiTableFlags.ScrollY);
            if (!table) return;
            ImGui.TableSetupColumn($"###CharactersRetainerTable#Inventories#Bags_{selected_retainer.Id}", ImGuiTableColumnFlags.WidthFixed, 450);
            ImGui.TableSetupColumn($"###CharactersRetainerTable#Inventories#Crystals_{selected_retainer.Id}", ImGuiTableColumnFlags.WidthFixed, 180);
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            ImGui.TextUnformatted($"{Utils.GetAddonString(520)}");

            DrawRetainerInventory($"Retainer_{selected_retainer.Id}", inventory);
            if (ImGui.BeginTable($"###CharactersRetainerTable#Inventories#Bags_{selected_retainer.Id}#Amount", 2))
            {
                ImGui.TableSetupColumn($"###CharactersRetainerTable#Inventories#Bags_{selected_retainer.Id}#Amount#Col1", ImGuiTableColumnFlags.WidthFixed, 390);
                ImGui.TableSetupColumn($"###CharactersRetainerTable#Inventories#Bags_{selected_retainer.Id}#Amount#Col2", ImGuiTableColumnFlags.WidthFixed, 80);
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                ImGui.Text("");
                ImGui.TableSetColumnIndex(1);
                ImGui.TextUnformatted($"{inventory.FindAll(i => i.ItemId != 0).Count}/175");
                ImGui.EndTable();
            }

            ImGui.TableSetColumnIndex(1);
            ImGui.TextUnformatted($"{Utils.GetAddonString(2882)}");
            DrawCrystals(selected_retainer);
        }
    }

    private void DrawRetainerInventory(string label, List<Inventory> inventory)
    {
        using var table = ImRaii.Table($"###CharactersRetainerTable#Inventories#Inventory#{label}Table", 10, ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg | ImGuiTableFlags.BordersInner);
        if (!table) return;

        ImGui.TableSetupColumn($"###CharactersRetainerTable#Inventories#Inventory#{label}#Col1", ImGuiTableColumnFlags.WidthFixed, 36);
        ImGui.TableSetupColumn($"###CharactersRetainerTable#Inventories#Inventory#{label}#Col2", ImGuiTableColumnFlags.WidthFixed, 36);
        ImGui.TableSetupColumn($"###CharactersRetainerTable#Inventories#Inventory#{label}#Col3", ImGuiTableColumnFlags.WidthFixed, 36);
        ImGui.TableSetupColumn($"###CharactersRetainerTable#Inventories#Inventory#{label}#Col4", ImGuiTableColumnFlags.WidthFixed, 36);
        ImGui.TableSetupColumn($"###CharactersRetainerTable#Inventories#Inventory#{label}#Col5", ImGuiTableColumnFlags.WidthFixed, 36);
        ImGui.TableSetupColumn($"###CharactersRetainerTable#Inventories#Inventory#{label}#Col6", ImGuiTableColumnFlags.WidthFixed, 36);
        ImGui.TableSetupColumn($"###CharactersRetainerTable#Inventories#Inventory#{label}#Col7", ImGuiTableColumnFlags.WidthFixed, 36);
        ImGui.TableSetupColumn($"###CharactersRetainerTable#Inventories#Inventory#{label}#Col8", ImGuiTableColumnFlags.WidthFixed, 36);
        ImGui.TableSetupColumn($"###CharactersRetainerTable#Inventories#Inventory#{label}#Col9", ImGuiTableColumnFlags.WidthFixed, 36);
        ImGui.TableSetupColumn($"###CharactersRetainerTable#Inventories#Inventory#{label}#Col10", ImGuiTableColumnFlags.WidthFixed, 36);
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
                i == 130 ||
                i == 140 ||
                i == 150 ||
                i == 160 ||
                i == 170
            )
            {
                ImGui.TableNextRow();
            }
            ImGui.TableNextColumn();
            if (item == null || item.ItemId == 0)
            {
                Utils.DrawIcon(new Vector2(36, 36), false, 0);
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
        ImGui.TableSetColumnIndex(6);
        ImGui.TableSetColumnIndex(7);
        ImGui.TableSetColumnIndex(8);
        ImGui.TableSetColumnIndex(9);
        ImGui.TableSetColumnIndex(10);
    }

    private void DrawCrystals(Retainer selected_retainer)
    {
        using var table = ImRaii.Table($"###CharactersRetainerTable#Inventories_{selected_retainer.Id}", 4, ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg | ImGuiTableFlags.BordersInner, new Vector2(180, 180));
        if (!table) return;
        ImGui.TableSetupColumn($"###CharactersRetainerTable#Inventories_{selected_retainer.Id}#Crystals#Icons", ImGuiTableColumnFlags.WidthFixed, 36);
        ImGui.TableSetupColumn($"###CharactersRetainerTable#Inventories_{selected_retainer.Id}#Crystals#Col1", ImGuiTableColumnFlags.WidthFixed, 36);
        ImGui.TableSetupColumn($"###CharactersRetainerTable#Inventories_{selected_retainer.Id}#Crystals#Col2", ImGuiTableColumnFlags.WidthFixed, 36);
        ImGui.TableSetupColumn($"###CharactersRetainerTable#Inventories_{selected_retainer.Id}#Crystals#Col3", ImGuiTableColumnFlags.WidthFixed, 36);
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
        Utils.DrawIcon(new Vector2(40, 40), false, 60651);
        ImGui.TableSetColumnIndex(1);
        DrawCrystal(2, selected_retainer.Inventory[175].Quantity);
        ImGui.TableSetColumnIndex(2);
        DrawCrystal(8, selected_retainer.Inventory[181].Quantity);
        ImGui.TableSetColumnIndex(3);
        DrawCrystal(14, selected_retainer.Inventory[187].Quantity);

        ImGui.TableNextRow();
        ImGui.TableSetColumnIndex(0);
        Utils.DrawIcon(new Vector2(40, 40), false, 60652);
        ImGui.TableSetColumnIndex(1);
        DrawCrystal(3, selected_retainer.Inventory[176].Quantity);
        ImGui.TableSetColumnIndex(2);
        DrawCrystal(9, selected_retainer.Inventory[182].Quantity);
        ImGui.TableSetColumnIndex(3);
        DrawCrystal(15, selected_retainer.Inventory[188].Quantity);

        ImGui.TableNextRow();
        ImGui.TableSetColumnIndex(0);
        Utils.DrawIcon(new Vector2(40, 40), false, 60653);
        ImGui.TableSetColumnIndex(1);
        DrawCrystal(4, selected_retainer.Inventory[177].Quantity);
        ImGui.TableSetColumnIndex(2);
        DrawCrystal(10, selected_retainer.Inventory[183].Quantity);
        ImGui.TableSetColumnIndex(3);
        DrawCrystal(16, selected_retainer.Inventory[189].Quantity);

        ImGui.TableNextRow();
        ImGui.TableSetColumnIndex(0);
        Utils.DrawIcon(new Vector2(40, 40), false, 60654);
        ImGui.TableSetColumnIndex(1);
        DrawCrystal(5, selected_retainer.Inventory[178].Quantity);
        ImGui.TableSetColumnIndex(2);
        DrawCrystal(11, selected_retainer.Inventory[184].Quantity);
        ImGui.TableSetColumnIndex(3);
        DrawCrystal(17, selected_retainer.Inventory[190].Quantity);

        ImGui.TableNextRow();
        ImGui.TableSetColumnIndex(0);
        Utils.DrawIcon(new Vector2(40, 40), false, 60655);
        ImGui.TableSetColumnIndex(1);
        DrawCrystal(6, selected_retainer.Inventory[179].Quantity);
        ImGui.TableSetColumnIndex(2);
        DrawCrystal(12, selected_retainer.Inventory[185].Quantity);
        ImGui.TableSetColumnIndex(3);
        DrawCrystal(18, selected_retainer.Inventory[191].Quantity);

        ImGui.TableNextRow();
        ImGui.TableSetColumnIndex(0);
        Utils.DrawIcon(new Vector2(40, 40), false, 60656);
        ImGui.TableSetColumnIndex(1);
        DrawCrystal(7, selected_retainer.Inventory[180].Quantity);
        ImGui.TableSetColumnIndex(2);
        DrawCrystal(13, selected_retainer.Inventory[186].Quantity);
        ImGui.TableSetColumnIndex(3);
        DrawCrystal(19, selected_retainer.Inventory[192].Quantity);
    }

    private void DrawCrystal(uint itemid, uint amount)
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
                Utils.DrawCrystalTooltip(itemid, (int)amount);
            }
            ImGui.EndTooltip();
        }
    }
    private void DrawMarket(Retainer selected_retainer)
    {
        if (selected_retainer.MarketItemCount == 0) return;
        if (selected_retainer.MarketInventory.Count > 0)
        {
            ImGui.TextUnformatted($"On the market until: {Utils.UnixTimeStampToDateTime(selected_retainer.MarketExpire)}");
            foreach (Inventory item in selected_retainer.MarketInventory)
            {
                if (item.ItemId != 0)
                {
                    ImGui.TextUnformatted($"{Utils.GetItemNameFromId(item.ItemId)}: {item.Quantity}");
                }
            }
        }
        else
        {
            ImGui.TextUnformatted($"{selected_retainer.MarketItemCount} found. {Loc.Localize("OpenRetainerToUpdateList", "Open the retainer to list")}");
        }
    }
}