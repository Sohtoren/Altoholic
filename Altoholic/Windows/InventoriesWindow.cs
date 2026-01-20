using Altoholic.Cache;
using Altoholic.Models;
using CheapLoc;
using Dalamud.Bindings.ImGui;
using Dalamud.Game;
using Dalamud.Game.Text;
using Dalamud.Interface;
using Dalamud.Interface.Textures.TextureWraps;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Windowing;
using FFXIVClientStructs.FFXIV.Client.Game;
using Lumina.Excel.Sheets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Altoholic.Windows
{
    public class InventoriesWindow : Window, IDisposable
    {
        private readonly Plugin _plugin;
        private ClientLanguage _currentLocale;
        private GlobalCache _globalCache;

        public InventoriesWindow(
            Plugin plugin,
            string name,
            GlobalCache globalCache
        )
            : base(
                name, ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse)
        {
            SizeConstraints = new WindowSizeConstraints
            {
                MinimumSize = new Vector2(1000, 450), MaximumSize = new Vector2(float.MaxValue, float.MaxValue)
            };
            _plugin = plugin;
            _globalCache = globalCache;

            _armouryBoard = Plugin.PluginInterface.UiBuilder.LoadUld("ui/uld/ArmouryBoard.uld");
            _armoryTabTextures.Add(InventoryType.ArmoryMainHand,
                _armouryBoard.LoadTexturePart("ui/uld/ArmouryBoard_hr1.tex", 0));
            _armoryTabTextures.Add(InventoryType.ArmoryHead,
                _armouryBoard.LoadTexturePart("ui/uld/ArmouryBoard_hr1.tex", 1));
            _armoryTabTextures.Add(InventoryType.ArmoryBody,
                _armouryBoard.LoadTexturePart("ui/uld/ArmouryBoard_hr1.tex", 2));
            _armoryTabTextures.Add(InventoryType.ArmoryHands,
                _armouryBoard.LoadTexturePart("ui/uld/ArmouryBoard_hr1.tex", 3));
            _armoryTabTextures.Add(InventoryType.ArmoryLegs,
                _armouryBoard.LoadTexturePart("ui/uld/ArmouryBoard_hr1.tex", 5));
            _armoryTabTextures.Add(InventoryType.ArmoryFeets,
                _armouryBoard.LoadTexturePart("ui/uld/ArmouryBoard_hr1.tex", 6));
            _armoryTabTextures.Add(InventoryType.ArmoryOffHand,
                _armouryBoard.LoadTexturePart("ui/uld/ArmouryBoard_hr1.tex", 7));
            _armoryTabTextures.Add(InventoryType.ArmoryEar,
                _armouryBoard.LoadTexturePart("ui/uld/ArmouryBoard_hr1.tex", 8));
            _armoryTabTextures.Add(InventoryType.ArmoryNeck,
                _armouryBoard.LoadTexturePart("ui/uld/ArmouryBoard_hr1.tex", 9));
            _armoryTabTextures.Add(InventoryType.ArmoryWrist,
                _armouryBoard.LoadTexturePart("ui/uld/ArmouryBoard_hr1.tex", 10));
            _armoryTabTextures.Add(InventoryType.ArmoryRings,
                _armouryBoard.LoadTexturePart("ui/uld/ArmouryBoard_hr1.tex", 11));
            _armoryTabTextures.Add(InventoryType.ArmorySoulCrystal,
                _armouryBoard.LoadTexturePart("ui/uld/ArmouryBoard_hr1.tex", 12));

            _miragePrismBoxSetIcon = Plugin.TextureProvider.GetFromGame("ui/uld/MiragePrismBoxIcon_hr1.tex").RentAsync().Result;
            (_miragePrismBoxSetIconUv0, _miragePrismBoxSetIconUv1) = Utils.GetTextureCoordinate(_miragePrismBoxSetIcon.Size, 96, 96, 36, 36);
            (_miragePrismBoxSetIconUv2, _miragePrismBoxSetIconUv3) = Utils.GetTextureCoordinate(_miragePrismBoxSetIcon.Size, 96, 132, 36, 36);

        }

        public Func<Character> GetPlayer { get; init; } = null!;
        public Func<List<Character>> GetOthersCharactersList { get; init; } = null!;
        private Character? _currentCharacter;
        private IEnumerable<Item>? _currentItems;
        private uint? _currentItem;
        private string _searchedItem = string.Empty;
        private string _lastSearchedItem = string.Empty;
        private Dictionary<string, List<Gear>>? _selectedArmory;

        private readonly UldWrapper _armouryBoard;
        private readonly Dictionary<InventoryType, IDalamudTextureWrap?> _armoryTabTextures = [];
        private readonly IDalamudTextureWrap _miragePrismBoxSetIcon;
        private readonly Vector2 _miragePrismBoxSetIconUv0;
        private readonly Vector2 _miragePrismBoxSetIconUv1;
        private readonly Vector2 _miragePrismBoxSetIconUv2;
        private readonly Vector2 _miragePrismBoxSetIconUv3;

        private InventoryType? _selectedTab;

        private bool _isSpoilerEnabled;

        private bool _obtainedArmoireOnly;
        /*public override void OnClose()
        {
            Plugin.Log.Debug("InventoryWindow, OnClose() called");
            _currentCharacter = null;
            _currentItem = null;
            _currentItems = null;
            _searchedItem = string.Empty;
            _lastSearchedItem = string.Empty;
            _selectedArmory = null;
            _selectedTab = null;
        }*/

        public void Dispose()
        {
            Plugin.Log.Info("InventoriesWindow, Dispose() called");
            _currentCharacter = null;
            _currentItem = null;
            _currentItems = null;
            _searchedItem = string.Empty;
            _lastSearchedItem = string.Empty;
            _selectedArmory = null;
            _selectedTab = null;
            _isSpoilerEnabled = false;
            _obtainedArmoireOnly = false;

            foreach (var loadedTexture in _armoryTabTextures) loadedTexture.Value?.Dispose();
            _armouryBoard.Dispose();
            _miragePrismBoxSetIcon.Dispose();
        }

        public void Clear()
        {
            Plugin.Log.Info("InventoriesWindow, Clear() called");
            _currentCharacter = null;
            _currentItem = null;
            _currentItems = null;
            _searchedItem = string.Empty;
            _lastSearchedItem = string.Empty;
            _selectedArmory = null;
            _selectedTab = null;
            _isSpoilerEnabled = false;
            _obtainedArmoireOnly = false;
        }

        public override void Draw()
        {
            _currentLocale = _plugin.Configuration.Language;
            _obtainedArmoireOnly = _plugin.Configuration.ObtainedOnly;
            _isSpoilerEnabled = _plugin.Configuration.IsSpoilersEnabled;
            List<Character> chars = [];
            chars.Insert(0, GetPlayer.Invoke());
            chars.AddRange(GetOthersCharactersList.Invoke());

            using var table = ImRaii.Table("###CharactersInventoryTable", 2);
            if (!table) return;

            ImGui.TableSetupColumn("###CharactersInventoryTable#CharactersListHeader",
                ImGuiTableColumnFlags.WidthFixed, 210);
            ImGui.TableSetupColumn("###CharactersInventoryTable#Inventories", ImGuiTableColumnFlags.WidthStretch);
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            using (var listBox =
                   ImRaii.ListBox("###CharactersInventoryTable#CharactersListBox", new Vector2(200, -1)))
            {
                if (listBox)
                {
                    if (chars.Count > 0)
                    {
                        if (ImGui.Selectable(
                                $"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 970)}###CharactersInventoryTable#CharactersListBox#All",
                                _currentCharacter == null))
                        {
                            _currentCharacter = null;
                        }

#if DEBUG
                            for (int i = 0; i < 15; i++)
                            {
                                chars.Add(new Character()
                                {
                                    FirstName = $"Dummy {i}",
                                    LastName = $"LN {i}",
                                    HomeWorld = $"Homeworld {i}",
                                    Datacenter = $"EU",
                                    FCTag = $"FC {i}",
                                    Currencies = new PlayerCurrencies() { Gil = i },
                                });
                            }
#endif

                        foreach (Character currChar in chars.Where(currChar =>
                                     ImGui.Selectable(
                                         $"{currChar.FirstName} {currChar.LastName}{(char)SeIconChar.CrossWorld}{currChar.HomeWorld}",
                                         currChar == _currentCharacter)))
                        {
                            _currentCharacter = currChar;
                            _currentItem = null;
                            _currentItems = null;
                            _searchedItem = string.Empty;
                            _lastSearchedItem = string.Empty;
                        }
                    }
                }
            }

            ImGui.TableSetColumnIndex(1);
            if (_currentCharacter is not null)
            {
                DrawInventories(_currentCharacter);
            }
            else
            {
                DrawAll(chars);
            }
        }

        private void DrawAll(List<Character> chars)
        {
            if (ImGui.InputText(Utils.Capitalize(_globalCache.AddonStorage.LoadAddonString(_currentLocale, 3635)),
                    ref _searchedItem, 512,
                    ImGuiInputTextFlags.EnterReturnsTrue))
            {
                switch (_searchedItem.Length)
                {
                    case >= 3 when _searchedItem is "Gil" or "MGP" or "MGF" || _lastSearchedItem == _searchedItem:
                        return;
                    case >= 3:
                        {
                            IEnumerable<Item>? items = Utils.GetItemsFromName(_currentLocale, _searchedItem);
                            if (items != null)
                            {
                                List<Item> currentItems = items.ToList();
                                if (currentItems.Count > 0)
                                {
                                    _currentItems = currentItems;
                                }
                            }

                            _lastSearchedItem = _searchedItem;
                            break;
                        }
                    case 0:
                        _currentItems = null;
                        break;
                }
            }

            if (_currentItems == null)
            {
                return;
            }

            List<Item> currentItemsList = _currentItems.ToList();

            using var searchItemsTable =
                ImRaii.Table("###CharactersInventory#All#SearchItemsTable", 2, ImGuiTableFlags.Borders);
            if (!searchItemsTable) return;
            ImGui.TableSetupColumn("###CharactersInventory#All#SearchItemsTable#Item", ImGuiTableColumnFlags.WidthFixed,
                300);
            ImGui.TableSetupColumn("###CharactersInventory#All#SearchItemsTable#CharacterItems",
                ImGuiTableColumnFlags.WidthStretch);
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            using (var itemlistbox = ImRaii.ListBox("###CharactersInventory#All#SearchItemsTable#Item#Listbox",
                       new Vector2(300, -1)))
            {
                if (itemlistbox)
                {
                    foreach (Item item in currentItemsList.Where(
                                 item => ImGui.Selectable(item.Name.ExtractText(), item.RowId == _currentItem)))
                    {
                        _currentItem = item.RowId;
                    }
                }
            }

            ImGui.TableSetColumnIndex(1);
            if (_currentItem is null)
            {
                return;
            }

            Item? itm = _globalCache.ItemStorage.LoadItem(_currentLocale, (uint)_currentItem);
            if (itm == null) return;
            using (var itemIconTable =
                   ImRaii.Table("###CharactersInventory#All#SearchItemsTable#CharacterItems#ItemIconName", 2,
                       ImGuiTableFlags.NoBordersInBody))
            {
                if (!itemIconTable) return;
                ImGui.TableSetupColumn("###CharactersInventory#All#SearchItemsTable#CharacterItems#ItemIconName#Icon",
                    ImGuiTableColumnFlags.WidthFixed, 50);
                ImGui.TableSetupColumn("###CharactersInventory#All#SearchItemsTable#CharacterItems#ItemIconName#Name",
                    ImGuiTableColumnFlags.WidthFixed, 300);
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(itm.Value.Icon), new Vector2(36, 36));

                ImGui.TableSetColumnIndex(1);
                ImGui.TextUnformatted($"{itm.Value.Name}");
            }

            List<CharacterInventories> inventories = [];
            foreach (Character character in chars)
            {
                //Plugin.Log.Debug($"{character.FirstName} {character.LastName}{(char)SeIconChar.CrossWorld}{character.HomeWorld}");
                bool dresser = false;
                CharacterInventories ci = new();
                uint inventoryCount = character.Inventory.FindAll(i => i.ItemId == _currentItem).Aggregate<Inventory, uint>(0, (current, inv) => current + inv.Quantity);
                //Plugin.Log.Debug($"inventory count: {inventoryCount}");
                if (inventoryCount > 0)
                {
                    ci.Inventory = new Tuple<bool, uint>(true, inventoryCount);
                }

                if (character.ArmoryInventory != null)
                {
                    uint armoryCount = 0;
                    uint armoryMainHandCount = character.ArmoryInventory.MainHand.FindAll(i => i.ItemId == _currentItem)
                        .Aggregate<Gear, uint>(0, (current, inv) => current + 1);
                    //Plugin.Log.Debug($"armory main hand count: {armoryMainHandCount}");
                    armoryCount += armoryMainHandCount;
                    uint armoryHeadCount = character.ArmoryInventory.Head.FindAll(i => i.ItemId == _currentItem)
                        .Aggregate<Gear, uint>(0, (current, inv) => current + 1);
                    //Plugin.Log.Debug($"armory head count: {armoryHeadCount}");
                    armoryCount += armoryHeadCount;
                    uint armoryBodyCount = character.ArmoryInventory.Body.FindAll(i => i.ItemId == _currentItem)
                        .Aggregate<Gear, uint>(0, (current, inv) => current + 1);
                    //Plugin.Log.Debug($"armory count: {armoryBodyCount}");
                    armoryCount += armoryBodyCount;
                    uint armoryHandsCount = character.ArmoryInventory.Hands.FindAll(i => i.ItemId == _currentItem)
                        .Aggregate<Gear, uint>(0, (current, inv) => current + 1);
                    //Plugin.Log.Debug($"armory hands count: {armoryHandsCount}");
                    armoryCount += armoryHandsCount;
                    uint armoryLegsCount = character.ArmoryInventory.Legs.FindAll(i => i.ItemId == _currentItem)
                        .Aggregate<Gear, uint>(0, (current, inv) => current + 1);
                    //Plugin.Log.Debug($"armory Legs count: {armoryLegsCount}");
                    armoryCount += armoryLegsCount;
                    uint armoryFeetsCount = character.ArmoryInventory.Feets.FindAll(i => i.ItemId == _currentItem)
                        .Aggregate<Gear, uint>(0, (current, inv) => current + 1);
                    //Plugin.Log.Debug($"armory Feets count: {armoryFeetsCount}");
                    armoryCount += armoryFeetsCount;
                    uint armoryOffHandCount = character.ArmoryInventory.OffHand.FindAll(i => i.ItemId == _currentItem)
                        .Aggregate<Gear, uint>(0, (current, inv) => current + 1);
                    //Plugin.Log.Debug($"armory OffHand count: {armoryOffHandCount}");
                    armoryCount += armoryOffHandCount;
                    uint armoryEarCount = character.ArmoryInventory.Ear.FindAll(i => i.ItemId == _currentItem)
                        .Aggregate<Gear, uint>(0, (current, inv) => current + 1);
                    //Plugin.Log.Debug($"armory Earrings count: {armoryEarCount}");
                    armoryCount += armoryEarCount;
                    uint armoryNeckCount = character.ArmoryInventory.Neck.FindAll(i => i.ItemId == _currentItem)
                        .Aggregate<Gear, uint>(0, (current, inv) => current + 1);
                    //Plugin.Log.Debug($"armory Neck count: {armoryNeckCount}");
                    armoryCount += armoryNeckCount;
                    uint armoryWristCount = character.ArmoryInventory.Wrist.FindAll(i => i.ItemId == _currentItem)
                        .Aggregate<Gear, uint>(0, (current, inv) => current + 1);
                    //Plugin.Log.Debug($"armory Wrist count: {armoryWristCount}");
                    armoryCount += armoryWristCount;
                    uint armoryRingsCount = character.ArmoryInventory.Rings.FindAll(i => i.ItemId == _currentItem)
                        .Aggregate<Gear, uint>(0, (current, inv) => current + 1);
                    //Plugin.Log.Debug($"armory Rings count: {armoryRingsCount}");
                    armoryCount += armoryRingsCount;

                    if (armoryCount > 0)
                    {
                        ci.Armory = new Tuple<bool, uint>(true, armoryCount);
                    }
                }

                uint retainerCount = 0;
                foreach (Retainer characterRetainer in character.Retainers)
                {
                    uint currentRetainerCount = characterRetainer.Inventory.FindAll(ri => ri.ItemId == _currentItem).Aggregate<Inventory, uint>(0, (current, inv) => current + inv.Quantity);
                    if (currentRetainerCount <= 0)
                    {
                        continue;
                    }

                    retainerCount += currentRetainerCount;
                    ci.Retainers.Add(new Tuple<string, uint>($"{characterRetainer.Name}", currentRetainerCount));
                }
                //Plugin.Log.Debug($"retainer count: {retainerCount}");

                foreach (GlamourItem glamourItem in character.GlamourDresser)
                {
                    if (glamourItem == null) //While this shouldn't be null, some weird case happen where it is
                    {
                        continue;
                    }
                    if (glamourItem.ItemId != _currentItem)
                    {
                        continue;
                    }

                    dresser = true;
                    ci.Dresser = true;
                }
                //Plugin.Log.Debug($"Dresser count: {(dresser ? 1 : 0)}");
                uint? armoireId = _globalCache.ArmoireStorage.GetArmoireIdFromItemId((uint)_currentItem);
                bool armoire = (armoireId != null) && character.HasArmoire((uint)armoireId);
                ci.Armoire = armoire;
                //Plugin.Log.Debug($"{_currentItem} Armoire count: {(armoire ? 1 : 0)}");

                if (inventoryCount <= 0 && retainerCount <= 0 && !dresser && !armoire)
                {
                    continue;
                }
                ci.FirstName = character.FirstName;
                ci.LastName = character.LastName;
                ci.World = character.HomeWorld;

                inventories.Add(ci);
            }
            if (inventories.Count == 0)
            {
                ImGui.TextUnformatted(
                    $"{Loc.Localize("NoItemOnAnyCharacter", "Item not found on any characters.\r\nCheck if inventories are available and updated.")}");
                return;
            }

            uint overallAmount = 0;
            using (var table = ImRaii.Table("###CharactersInventory#All#SearchItemsTable#CharacterItems#Item#Table", 2,
                       ImGuiTableFlags.Borders | ImGuiTableFlags.ScrollY, new Vector2(-1, 400)))
            {
                if (!table) return;
                ImGui.TableSetupColumn(
                    "###CharactersInventory#All#SearchItemsTable#CharacterItems#Item#Table#CharacterName",
                    ImGuiTableColumnFlags.WidthFixed, 300);
                ImGui.TableSetupColumn(
                    "###CharactersInventory#All#SearchItemsTable#CharacterItems#Item#Table#CharacterItem",
                    ImGuiTableColumnFlags.WidthStretch);

#if DEBUG
                for (int i = 0; i < 15; i++)
                {
                    characters.Add(new Character()
                    {
                        FirstName = $"Dummy {i}",
                        LastName = $"LN {i}",
                        HomeWorld = $"Homeworld {i}",
                    });
                }
#endif

                //foreach (Character character in characters)
                foreach (CharacterInventories characterInventories in inventories)
                {
                    uint totalAmount = 0;
                    if (characterInventories.Inventory.Item1)
                    {
                        totalAmount += characterInventories.Inventory.Item2;
                    }
                    if (characterInventories.Armory.Item1)
                    {
                        totalAmount += characterInventories.Inventory.Item2;
                    }

                    long retainerAmount = characterInventories.Retainers.Sum(c => c.Item2);
                    if (retainerAmount > 0)
                    {
                        totalAmount += (uint)retainerAmount;
                    }

                    totalAmount += characterInventories.Dresser ? 1 : (uint)0;
                    totalAmount += characterInventories.Armoire ? 1 : (uint)0;
                    /*uint totalAmount = character.Inventory.FindAll(i => i.ItemId == _currentItem)
                        .Aggregate<Inventory, uint>(0, (current, inv) => (uint)(current + inv.Quantity));*/
                    overallAmount += totalAmount;
                    ImGui.TableNextRow();
                    ImGui.TableSetColumnIndex(0);
                    ImGui.TextUnformatted(
                        $"{characterInventories.FirstName} {characterInventories.LastName}{(char)SeIconChar.CrossWorld}{characterInventories.World}");
                    ImGui.TableSetColumnIndex(1);
                    ImGui.TextUnformatted($"{totalAmount}");
                    if (totalAmount <= 0)
                    {
                        continue;
                    }

                    if (ImGui.IsItemHovered())
                    {
                        ImGui.BeginTooltip();
                        if (characterInventories.Inventory.Item1)
                        {
                            ImGui.TextUnformatted(
                                $"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 520)}");
                            ImGui.SameLine();
                            ImGui.TextUnformatted($"{characterInventories.Inventory.Item2:N0}");
                        }

                        if (characterInventories.Armory.Item1)
                        {
                            ImGui.TextUnformatted(
                                $"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 1370)}");
                            ImGui.SameLine();
                            ImGui.TextUnformatted($"{characterInventories.Armory.Item2:N0}");
                        }

                        if (retainerAmount > 0)
                        {
                            ImGui.TextUnformatted(
                                $"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 532)}");
                            ImGui.SameLine();
                            ImGui.TextUnformatted($"{retainerAmount:N0}");

                            foreach (Tuple<string, uint> characterInventoriesRetainer in characterInventories
                                         .Retainers)
                            {
                                ImGui.TextUnformatted(
                                    $"----- {characterInventoriesRetainer.Item1}: {characterInventoriesRetainer.Item2:N0}");
                            }
                        }

                        if (characterInventories.Dresser)
                        {
                            ImGui.TextUnformatted(
                                $"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 3735)}");
                            ImGui.SameLine();
                            ImGui.TextUnformatted($"{(characterInventories.Dresser ? 1 : 0)}");
                        }

                        if (characterInventories.Armoire)
                        {
                            ImGui.TextUnformatted(
                                $"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 3734)}");
                            ImGui.SameLine();
                            ImGui.TextUnformatted($"{(characterInventories.Armoire ? 1 : 0)}");
                        }

                        ImGui.EndTooltip();
                    }
                }
            }

            using var overallAmountTable =
                ImRaii.Table("###CharactersInventory#All#SearchItemsTable#CharacterItems#OverallAmountTable", 2);
            if (!overallAmountTable) return;
            ImGui.TableSetupColumn(
                "###CharactersInventory#All#SearchItemsTable#CharacterItems#OverallAmountTable#Text",
                ImGuiTableColumnFlags.WidthFixed, 300);
            ImGui.TableSetupColumn(
                "###CharactersInventory#All#SearchItemsTable#CharacterItems#OverallAmountTable#Amount",
                ImGuiTableColumnFlags.WidthStretch);
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            ImGui.TextUnformatted($"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 3501)}");
            ImGui.TableSetColumnIndex(1);
            ImGui.TextUnformatted($"{overallAmount}");
        }

        private void DrawInventories(Character selectedCharacter)
        {
            using var tab =
                ImRaii.TabBar($"###CharactersInventoryTable#Inventories#{selectedCharacter.CharacterId}#TabBar");
            if (!tab) return;
            using (var inventoriesTab =
                   ImRaii.TabItem(
                       $"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 520)}###CharactersInventoryTable#Inventories#{selectedCharacter.CharacterId}#TabBar#MainInventoriesTab"))
            {
                if (inventoriesTab)
                {
                    DrawMainInventories(selectedCharacter);
                }
            }

            using (var armoryTab =
                   ImRaii.TabItem(
                       $"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 1370)}###CharactersInventoryTable#Inventories#{selectedCharacter.CharacterId}#TabBar#ArmoryInventoryTab"))
            {
                if (armoryTab)
                {
                    using var table =
                        ImRaii.Table(
                            $"###CharactersInventoryTable#Inventories#ArmoryInventoryTable#{selectedCharacter.CharacterId}",
                            3);
                    if (!table) return;
                    ImGui.TableSetupColumn(
                        $"###CharactersInventoryTable#Inventories#ArmoryInventoryTable#{selectedCharacter.CharacterId}#Col1",
                        ImGuiTableColumnFlags.WidthStretch);
                    ImGui.TableSetupColumn(
                        $"###CharactersInventoryTable#Inventories#ArmoryInventoryTable#{selectedCharacter.CharacterId}#Col2",
                        ImGuiTableColumnFlags.WidthFixed, 600);
                    ImGui.TableSetupColumn(
                        $"###CharactersInventoryTable#Inventories#ArmoryInventoryTable#{selectedCharacter.CharacterId}#Col3",
                        ImGuiTableColumnFlags.WidthStretch);
                    ImGui.TableNextRow();
                    ImGui.TableSetColumnIndex(0);
                    ImGui.TableSetColumnIndex(1);
                    DrawArmory(_globalCache, selectedCharacter);
                    ImGui.TableSetColumnIndex(2);
                }
            }

            using (var glamourTab =
                   ImRaii.TabItem(
                       $"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 3735)}###CharactersInventoryTable#Inventories#{selectedCharacter.CharacterId}#TabBar#GlamourInventoryTab"))
            {
                if (glamourTab)
                {
                    DrawGlamourDresser(selectedCharacter.GlamourDresser);
                }
            }

            using (var armoireTab =
                   ImRaii.TabItem(
                       $"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 3734)}###CharactersInventoryTable#Inventories#{selectedCharacter.CharacterId}#TabBar#ArmoireInventoryTab"))
            {
                if (armoireTab)
                {
                    DrawArmoire(selectedCharacter);
                }
            }
        }

        private void DrawMainInventories(Character selectedCharacter)
        {
            using var table = ImRaii.Table($"###CharactersInventoryTable#Inventories#{selectedCharacter.CharacterId}",
                2, ImGuiTableFlags.ScrollY);
            if (!table) return;

            //if (selectedCharacter.Inventory.Count != 278) return;
            if (selectedCharacter.Inventory.Count != 282) return;
            List<Inventory> inventory =
                [.. selectedCharacter.Inventory[..140].OrderByDescending(i => i.Quantity)]; //Todo: Use ItemOrderModule
            List<Inventory> keysitems =
                [.. selectedCharacter.Inventory.Slice(141, 105).OrderByDescending(k => k.Quantity)];
            bool isKeyItemEmpty = (keysitems.FindAll(k => k.Quantity == 0).Count == 105);
            //List<Inventory> crystals = selected_character.Inventory.Slice(246, 32);
            List<Inventory> saddleBag = [.. selectedCharacter.Saddle.OrderByDescending(s => s.Quantity)];

            ImGui.TableSetupColumn($"###CharactersInventoryTable#Inventories#Bags_{selectedCharacter.CharacterId}",
                ImGuiTableColumnFlags.WidthFixed, 450);
            if (isKeyItemEmpty)
                ImGui.TableSetupColumn(
                    $"###CharactersInventoryTable#Inventories#KeyItems_{selectedCharacter.CharacterId}",
                    ImGuiTableColumnFlags.WidthFixed, 180);
            else
                ImGui.TableSetupColumn(
                    $"###CharactersInventoryTable#Inventories#KeyItems_{selectedCharacter.CharacterId}",
                    ImGuiTableColumnFlags.WidthStretch);

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            ImGui.TextUnformatted($"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 520)}");
            if (_currentLocale == ClientLanguage.Japanese)
            {
                ImGui.SameLine();
                ImGui.TextUnformatted($"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 521)}");
            }

            DrawInventory($"Inventory_{selectedCharacter.CharacterId}", inventory);
            using (var inventoryAmountTable =
                   ImRaii.Table(
                       $"###CharactersInventoryTable#Inventories#Inventory#Inventory_{selectedCharacter.CharacterId}#Amount",
                       2))
            {
                if (inventoryAmountTable)
                {
                    ImGui.TableSetupColumn(
                        $"###CharactersInventoryTable#Inventories#Inventory#Inventory_{selectedCharacter.CharacterId}#Amount#Col1",
                        ImGuiTableColumnFlags.WidthFixed, 390);
                    ImGui.TableSetupColumn(
                        $"###CharactersInventoryTable#Inventories#Inventory#Inventory_{selectedCharacter.CharacterId}#Amount#Col2",
                        ImGuiTableColumnFlags.WidthFixed, 80);
                    ImGui.TableNextRow();
                    ImGui.TableSetColumnIndex(0);
                    ImGui.Text("");
                    ImGui.TableSetColumnIndex(1);
                    ImGui.TextUnformatted($"{inventory.FindAll(i => i.ItemId != 0).Count}/140");
                }
            }

            ImGui.TableSetColumnIndex(1);
            ImGui.TextUnformatted($"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 536)}");
            //Plugin.Log.Debug($"{selected_character.FirstName} KeyItems: {keysitems.FindAll(k => k.Quantity == 0).Count}");
            if (isKeyItemEmpty)
            {
                ImGui.TextUnformatted($"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 1959)}");
            }
            else
            {
                DrawKeyInventory($"Keyitem_{selectedCharacter.CharacterId}", keysitems);
            }

            ImGui.TextUnformatted($"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 2882)}");
            DrawCrystals(selectedCharacter);

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            ImGui.TextUnformatted($"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 882)}");
            if (_currentLocale == ClientLanguage.Japanese)
            {
                ImGui.TextUnformatted($"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 12212)}");
            }

            //Plugin.Log.Debug($"{selected_character.FirstName} Saddle: {selected_character.Saddle.FindAll(k => k.Quantity == 0).Count}");
            int saddleCount = selectedCharacter.Saddle.FindAll(s => s.Quantity == 0).Count;
            if (saddleCount is 0 or 140)
            {
                ImGui.TextUnformatted(
                    $"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 5448)} {Loc.Localize("OpenSaddlebag", "The Saddlebag may be empty or open to update")}");
            }
            else
            {
                DrawInventory($"Saddle_{selectedCharacter.CharacterId}", saddleBag, true,
                    selectedCharacter.HasPremiumSaddlebag);
                using var saddleAmountTable =
                    ImRaii.Table(
                        $"###CharactersInventoryTable#Inventories#Inventory#Saddle_{selectedCharacter.CharacterId}#Amount",
                        2);
                if (!saddleAmountTable)
                {
                    return;
                }

                ImGui.TableSetupColumn(
                    $"###CharactersInventoryTable#Inventories#Inventory#Saddle_{selectedCharacter.CharacterId}#Amount#Col1",
                    ImGuiTableColumnFlags.WidthFixed, 390);
                ImGui.TableSetupColumn(
                    $"###CharactersInventoryTable#Inventories#Inventory#Saddle_{selectedCharacter.CharacterId}#Amount#Col2",
                    ImGuiTableColumnFlags.WidthFixed, 80);
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                ImGui.Text("");
                ImGui.TableSetColumnIndex(1);
                ImGui.TextUnformatted(
                    $"{saddleBag.FindAll(i => i.ItemId != 0).Count}/{((selectedCharacter.HasPremiumSaddlebag) ? "140" : "70")}");
            }

#if DEBUG
            List<Inventory> unknownBag = selectedCharacter.Inventory.Slice(246, 36);
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            ImGui.TextUnformatted("Unknown bag");
            //Plugin.Log.Debug($"{selected_character.FirstName} Saddle: {selected_character.Saddle.FindAll(k => k.Quantity == 0).Count}");
            DrawInventory($"unknown", unknownBag);
#endif
        }

        private void DrawInventory(string label, List<Inventory> inventory, bool saddle = false, bool premium = false)
        {
            using var table = ImRaii.Table($"###CharactersInventoryTable#Inventories#Inventory#{label}Table", 10,
                ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg | ImGuiTableFlags.BordersInner);
            if (!table) return;

            ImGui.TableSetupColumn($"###CharactersInventoryTable#Inventories#Inventory#{label}#Col1",
                ImGuiTableColumnFlags.WidthFixed, 36);
            ImGui.TableSetupColumn($"###CharactersInventoryTable#Inventories#Inventory#{label}#Col2",
                ImGuiTableColumnFlags.WidthFixed, 36);
            ImGui.TableSetupColumn($"###CharactersInventoryTable#Inventories#Inventory#{label}#Col3",
                ImGuiTableColumnFlags.WidthFixed, 36);
            ImGui.TableSetupColumn($"###CharactersInventoryTable#Inventories#Inventory#{label}#Col4",
                ImGuiTableColumnFlags.WidthFixed, 36);
            ImGui.TableSetupColumn($"###CharactersInventoryTable#Inventories#Inventory#{label}#Col5",
                ImGuiTableColumnFlags.WidthFixed, 36);
            ImGui.TableSetupColumn($"###CharactersInventoryTable#Inventories#Inventory#{label}#Col6",
                ImGuiTableColumnFlags.WidthFixed, 36);
            ImGui.TableSetupColumn($"###CharactersInventoryTable#Inventories#Inventory#{label}#Col7",
                ImGuiTableColumnFlags.WidthFixed, 36);
            ImGui.TableSetupColumn($"###CharactersInventoryTable#Inventories#Inventory#{label}#Col8",
                ImGuiTableColumnFlags.WidthFixed, 36);
            ImGui.TableSetupColumn($"###CharactersInventoryTable#Inventories#Inventory#{label}#Col9",
                ImGuiTableColumnFlags.WidthFixed, 36);
            ImGui.TableSetupColumn($"###CharactersInventoryTable#Inventories#Inventory#{label}#Col10",
                ImGuiTableColumnFlags.WidthFixed, 36);

            for (int i = 0; i < inventory.Count; i++)
            {
                Inventory item = inventory[i];
                if (saddle && !premium && i > 69) continue;
                if (i % 10 == 0)
                {
                    ImGui.TableNextRow();
                }

                ImGui.TableNextColumn();
                if (item.ItemId == 0)
                {
                    //Utils.DrawIcon(new Vector2(36, 36), false, 653);
                    ImGui.Text("");
                    /*var list = ImGui.GetWindowDrawList();
                    list.AddRect(new Vector2(0, 0), new Vector2(36, 36),(uint)i);*/
                }
                else
                {
                    Vector2 p = ImGui.GetCursorPos();
                    Item? itm = _globalCache.ItemStorage.LoadItem(_currentLocale, item.ItemId);
                    if (itm == null)
                    {
                        continue;
                    }

                    bool armoire = _globalCache.ArmoireStorage.CanBeInArmoireFromItemId(itm.Value.RowId);
                    var sets = _globalCache.MirageSetStorage.GetMirageSetItemLookup(item.ItemId);
                    bool canBeInASet = sets != null && sets.Count != 0;

                    Vector2 gcsp = ImGui.GetCursorScreenPos();
                    Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(itm.Value.Icon, item.HQ), new Vector2(36, 36));
                    if (item.Stain != 0)
                    {
                        Utils.DrawRound(gcsp, _globalCache.StainStorage.LoadStainWithColor(_currentLocale, item.Stain).Item2, 4.0f, 30.0f, 1.0f);
                    }

                    if (item.Stain2 != 0)
                    {
                        Utils.DrawRound(gcsp, _globalCache.StainStorage.LoadStainWithColor(_currentLocale, item.Stain2).Item2, 3.4f, 30.0f, 15.0f);
                    }
                    ImGui.SetCursorScreenPos(gcsp);
                    if (ImGui.IsItemHovered())
                    {
                        Utils.DrawInventoryItemTooltip(_currentLocale, ref _globalCache, item, armoire, canBeInASet);
                    }

                    if (itm.Value.StackSize > 1)
                    {
                        switch (item.Quantity)
                        {
                            case >= 100:
                                ImGui.SetCursorPos(new Vector2(p.X + 20, p.Y + 20));
                                break;
                            case > 9:
                                ImGui.SetCursorPos(new Vector2(p.X + 26, p.Y + 20));
                                break;
                            default:
                                ImGui.SetCursorPos(new Vector2(p.X + 30, p.Y + 20));
                                break;
                        }
                        ImGui.TextUnformatted($"{item.Quantity}");
                        ImGui.SetCursorPos(p);
                    }

                    if (canBeInASet)
                    {
                        DrawCanBeInASetIcon(p);
                    }

                    if (!armoire)
                    {
                        continue;
                    }

                    ImGui.SetCursorPos(p with { X = p.X + 20 });
                    Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(066460), new Vector2(16, 16));
                    ImGui.SetCursorPos(p);
                }
            }
        }

        private void DrawKeyInventory(string label, List<Inventory> inventory)
        {
            int itemCount = inventory.FindAll(i => i.ItemId != 0).Count;
            int rows = (int)Math.Ceiling(itemCount / (double)7);
            int height = rows * 36;

            using var table = ImRaii.Table($"###CharactersInventoryTable#Inventories#Inventory#{label}Table", 7,
                ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg | ImGuiTableFlags.BordersInner,
                new Vector2(310, height));
            if (!table) return;

            ImGui.TableSetupColumn($"###CharactersInventoryTable#Inventories#Inventory#{label}#Col1",
                ImGuiTableColumnFlags.WidthFixed, 36);
            ImGui.TableSetupColumn($"###CharactersInventoryTable#Inventories#Inventory#{label}#Col2",
                ImGuiTableColumnFlags.WidthFixed, 36);
            ImGui.TableSetupColumn($"###CharactersInventoryTable#Inventories#Inventory#{label}#Col3",
                ImGuiTableColumnFlags.WidthFixed, 36);
            ImGui.TableSetupColumn($"###CharactersInventoryTable#Inventories#Inventory#{label}#Col4",
                ImGuiTableColumnFlags.WidthFixed, 36);
            ImGui.TableSetupColumn($"###CharactersInventoryTable#Inventories#Inventory#{label}#Col5",
                ImGuiTableColumnFlags.WidthFixed, 36);
            ImGui.TableSetupColumn($"###CharactersInventoryTable#Inventories#Inventory#{label}#Col6",
                ImGuiTableColumnFlags.WidthFixed, 36);
            ImGui.TableSetupColumn($"###CharactersInventoryTable#Inventories#Inventory#{label}#Col7",
                ImGuiTableColumnFlags.WidthFixed, 36);
            for (int i = 0; i < inventory.Count; i++)
            {
                Inventory item = inventory[i];
                if (item.ItemId == 0) continue;
                if (i % 7 == 0)
                {
                    ImGui.TableNextRow();
                }

                ImGui.TableNextColumn();
                if (item.ItemId == 0)
                {
                    //Utils.DrawIcon(new Vector2(36, 36), false, 653);
                    ImGui.Text("");
                    /*var list = ImGui.GetWindowDrawList();
                    list.AddRect(new Vector2(0, 0), new Vector2(36, 36),(uint)i);*/
                }
                else
                {
                    Vector2 p = ImGui.GetCursorPos();
                    EventItem? itm = _globalCache.ItemStorage.LoadEventItem(_currentLocale, item.ItemId);
                    if (itm == null)
                    {
                        continue;
                    }

                    Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(itm.Value.Icon, item.HQ), new Vector2(36, 36));
                    if (ImGui.IsItemHovered())
                    {
                        Utils.DrawEventItemTooltip(_currentLocale, _globalCache, item);
                    }

                    if (itm.Value.StackSize <= 1)
                    {
                        continue;
                    }

                    ImGui.SetCursorPos(new Vector2(p.X + 26, p.Y + 20));
                    ImGui.TextUnformatted($"{item.Quantity}");
                    ImGui.SetCursorPos(p);
                }
            }
        }

        private void DrawCrystals(Character selectedCharacter)
        {
            if (selectedCharacter.Currencies is null) return;
            using var table = ImRaii.Table($"###CharactersInventoryTable#Inventories_{selectedCharacter.CharacterId}",
                4, ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg | ImGuiTableFlags.BordersInner,
                new Vector2(180, 180));
            if (!table) return;
            ImGui.TableSetupColumn(
                $"###CharactersInventoryTable#Inventories_{selectedCharacter.CharacterId}#Crystals#Icons",
                ImGuiTableColumnFlags.WidthFixed, 36);
            ImGui.TableSetupColumn(
                $"###CharactersInventoryTable#Inventories_{selectedCharacter.CharacterId}#Crystals#Col1",
                ImGuiTableColumnFlags.WidthFixed, 36);
            ImGui.TableSetupColumn(
                $"###CharactersInventoryTable#Inventories_{selectedCharacter.CharacterId}#Crystals#Col2",
                ImGuiTableColumnFlags.WidthFixed, 36);
            ImGui.TableSetupColumn(
                $"###CharactersInventoryTable#Inventories_{selectedCharacter.CharacterId}#Crystals#Col3",
                ImGuiTableColumnFlags.WidthFixed, 36);
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            ImGui.Text("");
            ImGui.TableSetColumnIndex(1);
            Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(20034), new Vector2(36, 36));
            ImGui.TableSetColumnIndex(2);
            Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(20019), new Vector2(36, 36));
            ImGui.TableSetColumnIndex(3);
            Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(20020), new Vector2(36, 36));

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(60651), new Vector2(24, 24));
            ImGui.TableSetColumnIndex(1);
            DrawCrystal(2, selectedCharacter.Currencies.Fire_Shard);
            ImGui.TableSetColumnIndex(2);
            DrawCrystal(8, selectedCharacter.Currencies.Fire_Crystal);
            ImGui.TableSetColumnIndex(3);
            DrawCrystal(14, selectedCharacter.Currencies.Fire_Cluster);

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(60652), new Vector2(24, 24));
            ImGui.TableSetColumnIndex(1);
            DrawCrystal(3, selectedCharacter.Currencies.Ice_Shard);
            ImGui.TableSetColumnIndex(2);
            DrawCrystal(9, selectedCharacter.Currencies.Ice_Crystal);
            ImGui.TableSetColumnIndex(3);
            DrawCrystal(15, selectedCharacter.Currencies.Ice_Cluster);

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(60653), new Vector2(24, 24));
            ImGui.TableSetColumnIndex(1);
            DrawCrystal(4, selectedCharacter.Currencies.Wind_Shard);
            ImGui.TableSetColumnIndex(2);
            DrawCrystal(10, selectedCharacter.Currencies.Wind_Crystal);
            ImGui.TableSetColumnIndex(3);
            DrawCrystal(16, selectedCharacter.Currencies.Wind_Cluster);

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(60654), new Vector2(24, 24));
            ImGui.TableSetColumnIndex(1);
            DrawCrystal(5, selectedCharacter.Currencies.Earth_Shard);
            ImGui.TableSetColumnIndex(2);
            DrawCrystal(11, selectedCharacter.Currencies.Earth_Crystal);
            ImGui.TableSetColumnIndex(3);
            DrawCrystal(17, selectedCharacter.Currencies.Earth_Cluster);

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(60655), new Vector2(24, 24));
            ImGui.TableSetColumnIndex(1);
            DrawCrystal(6, selectedCharacter.Currencies.Lightning_Shard);
            ImGui.TableSetColumnIndex(2);
            DrawCrystal(12, selectedCharacter.Currencies.Lightning_Crystal);
            ImGui.TableSetColumnIndex(3);
            DrawCrystal(18, selectedCharacter.Currencies.Lightning_Cluster);

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(60656), new Vector2(24, 24));
            ImGui.TableSetColumnIndex(1);
            DrawCrystal(7, selectedCharacter.Currencies.Water_Shard);
            ImGui.TableSetColumnIndex(2);
            DrawCrystal(13, selectedCharacter.Currencies.Water_Crystal);
            ImGui.TableSetColumnIndex(3);
            DrawCrystal(19, selectedCharacter.Currencies.Water_Cluster);
        }

        private void DrawCrystal(uint itemid, int amount)
        {
            ImGui.TextUnformatted($"{amount}");
            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                if (amount == 0)
                {
                    ImGui.TextUnformatted(Utils.GetCrystalName(_currentLocale, _globalCache, itemid));
                }
                else
                {
                    Utils.DrawCrystalTooltip(_currentLocale, ref _globalCache, itemid, amount);
                }

                ImGui.EndTooltip();
            }
        }

        private void DrawArmory(GlobalCache globalCache, Character selectedCharacter)
        {
            if (selectedCharacter.ArmoryInventory == null) return;
            //Plugin.Log.Debug($"{_armoryTabTextures.Count}");
            using var table = ImRaii.Table("###CharactersInventoryTable#Inventories#ArmoryInventoryTable", 3,
                ImGuiTableFlags.ScrollY);
            if (!table) return;

            ImGui.TableSetupColumn("###CharactersInventoryTable#Inventories#ArmoryInventoryTable#SelectedTable#Col1",
                ImGuiTableColumnFlags.WidthFixed, 44);
            ImGui.TableSetupColumn("###CharactersInventoryTable#Inventories#ArmoryInventoryTable#SelectedTable#Col2",
                ImGuiTableColumnFlags.WidthFixed, 275);
            ImGui.TableSetupColumn("###CharactersInventoryTable#Inventories#ArmoryInventoryTable#SelectedTable#Col3",
                ImGuiTableColumnFlags.WidthFixed, 44);
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            using (var armoryLeftTable =
                   ImRaii.Table("###CharactersInventoryTable#Inventories#ArmoryInventoryTable#SelectedTable#Col1Table",
                       1))
            {
                if (armoryLeftTable)
                {
                    ImGui.TableSetupColumn(
                        "###CharactersInventoryTable#Inventories#ArmoryInventoryTable#SelectedTable#Col1Table#Column",
                        ImGuiTableColumnFlags.WidthFixed, 44);
                    ImGui.TableNextRow();
                    ImGui.TableSetColumnIndex(0);
                    DrawArmouryIcon(_armoryTabTextures[InventoryType.ArmoryMainHand],
                        globalCache.AddonStorage.LoadAddonString(_currentLocale, 11524), new Vector2(44, 44),
                        selectedCharacter.ArmoryInventory.MainHand, InventoryType.ArmoryMainHand);

                    ImGui.TableNextRow();
                    ImGui.TableSetColumnIndex(0);
                    DrawArmouryIcon(_armoryTabTextures[InventoryType.ArmoryHead],
                        globalCache.AddonStorage.LoadAddonString(_currentLocale, 11525), new Vector2(44, 44),
                        selectedCharacter.ArmoryInventory.Head, InventoryType.ArmoryHead);

                    ImGui.TableNextRow();
                    ImGui.TableSetColumnIndex(0);
                    DrawArmouryIcon(_armoryTabTextures[InventoryType.ArmoryBody],
                        globalCache.AddonStorage.LoadAddonString(_currentLocale, 11526), new Vector2(44, 44),
                        selectedCharacter.ArmoryInventory.Body, InventoryType.ArmoryBody);

                    ImGui.TableNextRow();
                    ImGui.TableSetColumnIndex(0);
                    DrawArmouryIcon(_armoryTabTextures[InventoryType.ArmoryHands],
                        globalCache.AddonStorage.LoadAddonString(_currentLocale, 11527), new Vector2(44, 44),
                        selectedCharacter.ArmoryInventory.Hands, InventoryType.ArmoryHands);

                    ImGui.TableNextRow();
                    ImGui.TableSetColumnIndex(0);
                    DrawArmouryIcon(_armoryTabTextures[InventoryType.ArmoryLegs],
                        globalCache.AddonStorage.LoadAddonString(_currentLocale, 11528), new Vector2(44, 44),
                        selectedCharacter.ArmoryInventory.Legs, InventoryType.ArmoryLegs);

                    ImGui.TableNextRow();
                    ImGui.TableSetColumnIndex(0);
                    DrawArmouryIcon(_armoryTabTextures[InventoryType.ArmoryFeets],
                        globalCache.AddonStorage.LoadAddonString(_currentLocale, 11529), new Vector2(44, 44),
                        selectedCharacter.ArmoryInventory.Feets, InventoryType.ArmoryFeets);
                }
            }

            ImGui.TableSetColumnIndex(1);
            DrawArmoryInventory(globalCache);

            ImGui.TableSetColumnIndex(2);
            using var armoryRightTable =
                ImRaii.Table("###CharactersInventoryTable#Inventories#ArmoryInventoryTable#SelectedTable#Col3Table", 1);
            if (!armoryRightTable) return;

            ImGui.TableSetupColumn(
                "###CharactersInventoryTable#Inventories#ArmoryInventoryTable#SelectedTable#Col3Table#Column",
                ImGuiTableColumnFlags.WidthFixed, 44);
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            DrawArmouryIcon(_armoryTabTextures[InventoryType.ArmoryOffHand],
                globalCache.AddonStorage.LoadAddonString(_currentLocale, 11530), new Vector2(44, 44),
                selectedCharacter.ArmoryInventory.OffHand, InventoryType.ArmoryOffHand);

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            DrawArmouryIcon(_armoryTabTextures[InventoryType.ArmoryEar],
                globalCache.AddonStorage.LoadAddonString(_currentLocale, 11531), new Vector2(44, 44),
                selectedCharacter.ArmoryInventory.Ear, InventoryType.ArmoryEar);

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            DrawArmouryIcon(_armoryTabTextures[InventoryType.ArmoryNeck],
                globalCache.AddonStorage.LoadAddonString(_currentLocale, 11532), new Vector2(44, 44),
                selectedCharacter.ArmoryInventory.Neck, InventoryType.ArmoryNeck);

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            DrawArmouryIcon(_armoryTabTextures[InventoryType.ArmoryWrist],
                globalCache.AddonStorage.LoadAddonString(_currentLocale, 11533), new Vector2(44, 44),
                selectedCharacter.ArmoryInventory.Wrist, InventoryType.ArmoryWrist);

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            DrawArmouryIcon(_armoryTabTextures[InventoryType.ArmoryRings],
                globalCache.AddonStorage.LoadAddonString(_currentLocale, 11534), new Vector2(44, 44),
                selectedCharacter.ArmoryInventory.Rings, InventoryType.ArmoryRings);

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            DrawArmouryIcon(_armoryTabTextures[InventoryType.ArmorySoulCrystal],
                globalCache.AddonStorage.LoadAddonString(_currentLocale, 12238), new Vector2(44, 44),
                selectedCharacter.ArmoryInventory.SoulCrystal, InventoryType.ArmorySoulCrystal);
        }

        private void DrawArmouryIcon(IDalamudTextureWrap? texture, string tooltip, Vector2 size, List<Gear> gear,
            InventoryType type)
        {
            Vector4 inactiveColor = Vector4.One with { W = 0.33f };
            Vector4 activeColor = Vector4.One;
            if (texture is null) return;
            ImGui.Image(texture.Handle, size, Vector2.Zero, Vector2.One,
                (_selectedTab == type) ? activeColor : inactiveColor);
            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                ImGui.TextUnformatted(tooltip);
                ImGui.EndTooltip();
            }

            if (ImGui.IsItemClicked())
            {
                //Plugin.Log.Debug($"{tooltip} clicked");
                _selectedTab = type;
                _selectedArmory = new Dictionary<string, List<Gear>> { { tooltip, gear } };
            }

            ImGui.TextUnformatted($"{gear.FindAll(i => i.ItemId != 0).Count}");
        }

        private void DrawArmoryInventory(GlobalCache globalCache)
        {
            if (_selectedArmory == null || _selectedArmory.Count == 0) return;
            string? name = _selectedArmory.Keys.FirstOrDefault();
            if (name == null) return;
            List<Gear>? inventory = _selectedArmory.Values.FirstOrDefault();
            if (inventory == null) return;
            inventory = [.. inventory.OrderByDescending(i => i.ItemId != 0)];

            int armoryCount = inventory.FindAll(i => i.ItemId != 0).Count;
            int rows = (int)Math.Ceiling(armoryCount / (double)5);
            int height = rows * 48;

            using (var tableHeader =
                   ImRaii.Table($"###CharactersInventoryTable#Inventories#ArmoryInventoryTable#{name}##Amount", 3))
            {
                if (tableHeader)
                {
                    ImGui.TableSetupColumn(
                        $"###CharactersInventoryTable#Inventories#ArmoryInventoryTable#{name}#Header#Col1",
                        ImGuiTableColumnFlags.WidthStretch);
                    ImGui.TableSetupColumn(
                        $"###CharactersInventoryTable#Inventories#ArmoryInventoryTable#{name}#Header#Col2",
                        ImGuiTableColumnFlags.WidthFixed, 100);
                    ImGui.TableSetupColumn(
                        $"###CharactersInventoryTable#Inventories#ArmoryInventoryTable#{name}#Header#Col3",
                        ImGuiTableColumnFlags.WidthStretch);
                    ImGui.TableNextRow();
                    ImGui.TableSetColumnIndex(0);
                    ImGui.TableSetColumnIndex(1);
                    ImGui.TextUnformatted($"{Utils.Capitalize(name)}");
                    ImGui.TableSetColumnIndex(2);
                }
            }

            using (var table = ImRaii.Table(
                       "###CharactersInventoryTable#Inventories#ArmoryInventoryTable#SelectedTable", 5,
                       ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg | ImGuiTableFlags.BordersInner |
                       ImGuiTableFlags.ScrollY, new Vector2(266, height)))
            {
                if (table)
                {
                    ImGui.TableSetupColumn(
                        "###CharactersInventoryTable#Inventories#ArmoryInventoryTable#SelectedTable#Col1",
                        ImGuiTableColumnFlags.WidthFixed, 44);
                    ImGui.TableSetupColumn(
                        "###CharactersInventoryTable#Inventories#ArmoryInventoryTable#SelectedTable#Col2",
                        ImGuiTableColumnFlags.WidthFixed, 44);
                    ImGui.TableSetupColumn(
                        "###CharactersInventoryTable#Inventories#ArmoryInventoryTable#SelectedTable#Col3",
                        ImGuiTableColumnFlags.WidthFixed, 44);
                    ImGui.TableSetupColumn(
                        "###CharactersInventoryTable#Inventories#ArmoryInventoryTable#SelectedTable#Col4",
                        ImGuiTableColumnFlags.WidthFixed, 44);
                    ImGui.TableSetupColumn(
                        "###CharactersInventoryTable#Inventories#ArmoryInventoryTable#SelectedTable#Col5",
                        ImGuiTableColumnFlags.WidthFixed, 44);
                    for (int i = 0; i < inventory.Count; i++)
                    {
                        Gear gear = inventory[i];
                        if (gear.ItemId == 0) continue;
                        if (i % 5 == 0)
                        {
                            ImGui.TableNextRow();
                        }

                        ImGui.TableNextColumn();
                        if (gear.ItemId == 0)
                        {
                            //Utils.DrawIcon(new Vector2(36, 36), false, 653);
                            ImGui.Text("");
                            /*var list = ImGui.GetWindowDrawList();
                            list.AddRect(new Vector2(0, 0), new Vector2(36, 36),(uint)i);*/
                        }
                        else
                        {

                            /*Utils.DrawItemIcon(new Vector2(44, 44), gear.HQ, gear.ItemId);
                            if (ImGui.IsItemHovered())
                            {
                                Utils.DrawGearTooltip(gear);
                            }*/
                            ItemItemLevel? itl =
                                globalCache.ItemStorage.LoadItemWithItemLevel(_currentLocale, gear.ItemId);
                            if (itl == null) return;
                            Item? itm = itl.Item;
                            if (itm == null) return;
                            IDalamudTextureWrap icon = globalCache.IconStorage.LoadIcon(itm.Value.Icon, gear.HQ);
                            Utils.DrawIcon(icon, new Vector2(44, 44));
                            if (ImGui.IsItemHovered())
                            {
                                Utils.DrawGearTooltip(_currentLocale, ref globalCache, gear, itl);
                            }
                        }
                    }
                }
            }

            using var tableFooter =
                ImRaii.Table(
                    $"###CharactersInventoryTable#Inventories#ArmoryInventoryTable#SelectedTable#{name}##Amount", 2);
            if (!tableFooter)
            {
                return;
            }

            ImGui.TableSetupColumn(
                $"###CharactersInventoryTable#Inventories#ArmoryInventoryTable#{name}#Amount#Col1",
                ImGuiTableColumnFlags.WidthFixed, 220);
            ImGui.TableSetupColumn(
                $"###CharactersInventoryTable#Inventories#ArmoryInventoryTable#{name}#Amount#Col2",
                ImGuiTableColumnFlags.WidthFixed, 60);
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            ImGui.Text("");
            ImGui.TableSetColumnIndex(1);
            ImGui.TextUnformatted($"{inventory.FindAll(i => i.ItemId != 0).Count}/{inventory.Count}");

        }

        private void DrawArmoire(Character currentCharacter)
        {
            if (currentCharacter.Armoire.Count == 0)
            {
                ImGui.TextUnformatted($"{Loc.Localize("ArmoireNotLoaded", "No item found. You might need open the armoire to update")}");
                return;
            }

            using var armoireTable = ImRaii.Table("###armoireTable", 1, ImGuiTableFlags.ScrollY);
            if (!armoireTable) return;
            ImGui.TableSetupColumn($"###armoireTable#{currentCharacter.CharacterId}#Col1",
                ImGuiTableColumnFlags.WidthStretch);
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            if (ImGui.Checkbox($"{Loc.Localize("ObtainedOnly", "Obtained only")}", ref _obtainedArmoireOnly))
            {
                _plugin.Configuration.ObtainedOnly = _obtainedArmoireOnly;
                _plugin.Configuration.Save();
            }

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            DrawArmoireCollection(currentCharacter);
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            using var armoireTableAmount = ImRaii.Table("###ArmoireTableAmount", 2);
            if (!armoireTableAmount) return;
            int widthCol1 = 455;
            int widthCol2 = 145;
            if (_isSpoilerEnabled)
            {
                widthCol1 = 480;
                widthCol2 = 120;
            }

            ImGui.TableSetupColumn($"###ArmoireTableAmount#{currentCharacter.CharacterId}#Amount#Col1",
                ImGuiTableColumnFlags.WidthFixed, widthCol1);
            ImGui.TableSetupColumn($"###ArmoireTableAmount#{currentCharacter.CharacterId}#Amount#Col2",
                ImGuiTableColumnFlags.WidthFixed, widthCol2);
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            ImGui.Text("");
            ImGui.TableSetColumnIndex(1);
            string endStr = string.Empty;
            if (!_isSpoilerEnabled)
            {
                endStr += $"{Loc.Localize("ObtainedLowercase", " obtained")}";
            }
            else
            {
                endStr += $"/{_globalCache.ArmoireStorage.Count()}";
            }

            ImGui.TextUnformatted(
                $"{_globalCache.AddonStorage.LoadAddonString(_currentLocale, 3501)}: {currentCharacter.Armoire.Count}{endStr}");
        }

        private void DrawArmoireCollection(Character currentCharacter)
        {
            List<uint> armoireItems = (_obtainedArmoireOnly)
                ? [.. currentCharacter.Armoire]
                : _globalCache.ArmoireStorage.Get();
            int armoireCount = armoireItems.Count;
            if (armoireCount == 0) return;
            int rows = (int)Math.Ceiling(armoireCount / (double)10);
            int height = rows * 48 + 0;

            using var table = ImRaii.Table("###ArmoireItemsTable", 10,
                ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg | ImGuiTableFlags.BordersInner,
                new Vector2(572, height));
            if (!table) return;

            ImGui.TableSetupColumn("###ArmoireItemsTable#Col1",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn("###ArmoireItemsTable#Col2",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn("###ArmoireItemsTable#Col3",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn("###ArmoireItemsTable#Col4",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn("###ArmoireItemsTable#Col5",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn("###ArmoireItemsTable#Col6",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn("###ArmoireItemsTable#Col7",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn("###ArmoireItemsTable#Col8",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn("###ArmoireItemsTable#Col9",
                ImGuiTableColumnFlags.WidthFixed, 48);
            ImGui.TableSetupColumn("###ArmoireItemsTable#Col10",
                ImGuiTableColumnFlags.WidthFixed, 48);

            int i = 0;
            foreach (uint fkId in armoireItems)
            {
                if (i % 10 == 0)
                {
                    ImGui.TableNextRow();
                }

                ImGui.TableNextColumn();
                Armoire? a = _globalCache.ArmoireStorage.GetArmoire(fkId);
                if (a == null)
                {
                    continue;
                }

                Item? itm = _globalCache.ItemStorage.LoadItem(_currentLocale, a.ItemId);
                if (itm == null)
                {
                    continue;
                }

                Item item = itm.Value;

                if (currentCharacter.HasArmoire(fkId))
                {
                    Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(item.Icon), new Vector2(48, 48));
                }
                else
                {
                    if (_isSpoilerEnabled)
                    {
                        Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(item.Icon), new Vector2(48, 48),
                            new Vector4(1, 1, 1, 0.5f));
                    }
                    else
                    {
                        Utils.DrawIcon(_globalCache.IconStorage.LoadIcon(000786), new Vector2(48, 48));
                    }
                }

                if (ImGui.IsItemHovered())
                {
                    Utils.DrawItemTooltip(_currentLocale, ref _globalCache, item);
                }

                i++;
            }
        }

        private void DrawGlamourDresser(GlamourItem[] inventory)
        {
            int count = inventory.Where(i => i?.ItemId != 0 && i?.GlamourId == 0).ToList().Count;
            if (count == 0)
            {
                ImGui.TextUnformatted($"{Loc.Localize("GlamDresserNotLoaded", "No item found. You might need open the glamour dresser")}");

                return;
            }
            using (var tableFooter =
                   ImRaii.Table(
                       "###CharactersInventoryTable#Inventories#Inventory#GlamourDresser#Total##Amount", 2))
            {
                if (tableFooter)
                {
                    ImGui.TableSetupColumn(
                        "###CharactersInventoryTable#Inventories#ArmoryInventoryTable#Total#Amount#Col1",
                        ImGuiTableColumnFlags.WidthFixed, 640);
                    ImGui.TableSetupColumn(
                        "###CharactersInventoryTable#Inventories#ArmoryInventoryTable#Total#Amount#Col2",
                        ImGuiTableColumnFlags.WidthFixed, 60);
                    ImGui.TableNextRow();
                    ImGui.TableSetColumnIndex(0);
                    ImGui.Text("");
                    ImGui.TableSetColumnIndex(1);
                    ImGui.TextUnformatted($"{count}/800");
                }
            }

            using var table = ImRaii.Table("###CharactersInventoryTable#Inventories#Inventory#GlamourDresserTable", 15,
                ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg | ImGuiTableFlags.BordersInner |
                ImGuiTableFlags.ScrollY, new Vector2(690, -1));
            if (!table) return;

            ImGui.TableSetupColumn("###CharactersInventoryTable#Inventories#Inventory#GlamourDresser#Col1",
                ImGuiTableColumnFlags.WidthFixed, 36);
            ImGui.TableSetupColumn("###CharactersInventoryTable#Inventories#Inventory#GlamourDresser#Col2",
                ImGuiTableColumnFlags.WidthFixed, 36);
            ImGui.TableSetupColumn("###CharactersInventoryTable#Inventories#Inventory#GlamourDresser#Col3",
                ImGuiTableColumnFlags.WidthFixed, 36);
            ImGui.TableSetupColumn("###CharactersInventoryTable#Inventories#Inventory#GlamourDresser#Col4",
                ImGuiTableColumnFlags.WidthFixed, 36);
            ImGui.TableSetupColumn("###CharactersInventoryTable#Inventories#Inventory#GlamourDresser#Col5",
                ImGuiTableColumnFlags.WidthFixed, 36);
            ImGui.TableSetupColumn("###CharactersInventoryTable#Inventories#Inventory#GlamourDresser#Col6",
                ImGuiTableColumnFlags.WidthFixed, 36);
            ImGui.TableSetupColumn("###CharactersInventoryTable#Inventories#Inventory#GlamourDresser#Col7",
                ImGuiTableColumnFlags.WidthFixed, 36);
            ImGui.TableSetupColumn("###CharactersInventoryTable#Inventories#Inventory#GlamourDresser#Col8",
                ImGuiTableColumnFlags.WidthFixed, 36);
            ImGui.TableSetupColumn("###CharactersInventoryTable#Inventories#Inventory#GlamourDresser#Col9",
                ImGuiTableColumnFlags.WidthFixed, 36);
            ImGui.TableSetupColumn("###CharactersInventoryTable#Inventories#Inventory#GlamourDresser#Col10",
                ImGuiTableColumnFlags.WidthFixed, 36);
            ImGui.TableSetupColumn("###CharactersInventoryTable#Inventories#Inventory#GlamourDresser#Col11",
                ImGuiTableColumnFlags.WidthFixed, 36);
            ImGui.TableSetupColumn("###CharactersInventoryTable#Inventories#Inventory#GlamourDresser#Col12",
                ImGuiTableColumnFlags.WidthFixed, 36);
            ImGui.TableSetupColumn("###CharactersInventoryTable#Inventories#Inventory#GlamourDresser#Col13",
                ImGuiTableColumnFlags.WidthFixed, 36);
            ImGui.TableSetupColumn("###CharactersInventoryTable#Inventories#Inventory#GlamourDresser#Col14",
                ImGuiTableColumnFlags.WidthFixed, 36);
            ImGui.TableSetupColumn("###CharactersInventoryTable#Inventories#Inventory#GlamourDresser#Col15",
                ImGuiTableColumnFlags.WidthFixed, 36);

            int maxIndex = 0;
            bool setPassed = false;
            foreach (GlamourItem item in inventory)
            {
                if (item.ItemId == 0) continue;
                if (item.GlamourId != 0) continue;
                bool isInASet = _globalCache.MirageSetStorage.MirageSetLookup(item.ItemId);
                if (maxIndex % 15 == 0)
                {
                    ImGui.TableNextRow();
                }

                if (!setPassed && isInASet)
                {
                    ImGui.TableNextRow();
                    setPassed = true;
                    maxIndex = 0;
                }

                ImGui.TableNextColumn();
                Vector2 p = ImGui.GetCursorPos();
                ItemItemLevel? itl =
                    _globalCache.ItemStorage.LoadItemWithItemLevel(_currentLocale, item.ItemId);
                if (itl == null)
                {
                    continue;
                }
                Item? itm = itl.Item;
                if (itm == null)
                {
                    continue;
                }

                var sets = _globalCache.MirageSetStorage.GetMirageSetItemLookup(item.ItemId);
                bool canBeInASet = sets != null && sets.Count != 0;

                bool hq = item.Flags.HasFlag(InventoryItem.ItemFlags.HighQuality);
                Utils.DrawIcon(
                    _globalCache.IconStorage.LoadIcon(itm.Value.Icon,
                        hq), new Vector2(36, 36));
                if (ImGui.IsItemHovered())
                {
                    Utils.DrawGlamourDresserTooltip(_currentLocale, ref _globalCache,
                        item, itl, isInASet, _miragePrismBoxSetIcon, _miragePrismBoxSetIconUv0, _miragePrismBoxSetIconUv1, true, canBeInASet
                        );
                }

                if (isInASet && _miragePrismBoxSetIcon is not null)
                {
                    ImGui.SetCursorPos(p with { X = p.X + 25 });
                    ImGui.Image(_miragePrismBoxSetIcon.Handle, new Vector2(16, 16), _miragePrismBoxSetIconUv0, _miragePrismBoxSetIconUv1);
                    ImGui.SetCursorPos(p);
                }

                if (!isInASet && canBeInASet)
                {
                    DrawCanBeInASetIcon(p);
                }

                maxIndex++;
            }
        }

        private void DrawCanBeInASetIcon(Vector2 p)
        {
            if (_miragePrismBoxSetIcon is not null)
            {
                ImGui.SetCursorPos(p with { X = p.X + 20 });
                ImGui.Image(_miragePrismBoxSetIcon.Handle, new Vector2(20, 20), _miragePrismBoxSetIconUv2, _miragePrismBoxSetIconUv3);
                ImGui.SetCursorPos(p);
            }
        }

        private class CharacterInventories
        {
            public string FirstName = string.Empty;
            public string LastName = string.Empty;
            public string World = string.Empty;
            public Tuple<bool, uint> Inventory = new(false, 0);
            public Tuple<bool, uint> Armory = new(false, 0);
            public List<Tuple<string, uint>> Retainers = [];
            public bool Dresser = false;
            public bool Armoire = false;
        }
    }
}