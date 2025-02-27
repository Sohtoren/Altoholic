using Altoholic.Cache;
using Altoholic.Models;
using Altoholic.Windows;
using CheapLoc;
using Dalamud.Game;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.Command;
using Dalamud.Game.Inventory.InventoryEventArgTypes;
using Dalamud.Game.Inventory;
using Dalamud.Game.Text;
using Dalamud.Hooking;
using Dalamud.Interface;
using Dalamud.Interface.ImGuiNotification;
using Dalamud.Interface.Windowing;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Application.Network.WorkDefinitions;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Client.Game.Control;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Client.System.String;
using FFXIVClientStructs.FFXIV.Client.UI;
using LiteDB;
using Lumina.Excel.Sheets;
using Lumina.Text;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using static FFXIVClientStructs.FFXIV.Client.UI.RaptureAtkModule;
using Character = Altoholic.Models.Character;
using FFXIVClientStructs.FFXIV.Client.UI.Info;
using System.Numerics;
using System.Diagnostics;

namespace Altoholic
{
    public sealed class Plugin : IDalamudPlugin
    {
        public static string Name => "Altoholic";
        private const string CommandName = "/altoholic";
        private readonly Array _questIds = Enum.GetValues(typeof(QuestIds));

        [PluginService] public static IDalamudPluginInterface PluginInterface { get; set; } = null!;
        [PluginService] public static IClientState ClientState { get; set; } = null!;
        [PluginService] public static ICommandManager CommandManager { get; set; } = null!;
        [PluginService] public static IFramework Framework { get; set; } = null!;
        [PluginService] public static IPluginLog Log { get; set; } = null!;
        [PluginService] public static IDataManager DataManager { get; set; } = null!;
        [PluginService] public static ITextureProvider TextureProvider { get; set; } = null!;
        [PluginService] public static INotificationManager NotificationManager { get; set; } = null!;
        [PluginService] public static ICondition Condition { get; set; } = null!;
        [PluginService] public static ISigScanner SigScanner { get; set; } = null!;
        [PluginService] public static IGameInteropProvider Hook { get; set; } = null!;
        [PluginService] public static IChatGui ChatGui { get; set; } = null!;
        [PluginService] public static IDutyState DutyState { get; set; } = null!;
        [PluginService] public static IGameInventory GameInventory { get; set; } = null!;

        private const string PlaytimeSig = "E8 ?? ?? ?? ?? B9 ?? ?? ?? ?? 48 8B D3";
        private delegate long PlaytimeDelegate(uint param1, long param2, uint param3);
        private readonly Hook<PlaytimeDelegate> _playtimeHook;

        public Configuration Configuration { get; set; }
        public WindowSystem WindowSystem = new("Altoholic");

        private ConfigWindow ConfigWindow { get; }
        private MainWindow MainWindow { get; }
        private CharactersWindow CharactersWindow { get; }
        private DetailsWindow DetailsWindow { get; }
        private JobsWindow JobsWindow { get; }
        private CurrenciesWindow CurrenciesWindow { get; }
        private InventoriesWindow InventoriesWindow { get; }
        private RetainersWindow RetainersWindow { get; }
        private CollectionWindow CollectionWindow { get; }
        private ProgressWindow ProgressWindow { get; }

        //private readonly LiteDatabase _db;
        private readonly SqliteConnection _db;

        private Character _localPlayer = new();
        private Utf8String? _localPlayerFreeCompanyTest;

        private readonly Stopwatch _autoSaveWatch = new();
        private readonly Service _altoholicService;
        public List<Character> OtherCharacters = [];
        public List<Blacklist> BlacklistedCharacters;
        private readonly Localization _localization = new();
        private readonly GlobalCache _globalCache;

        public Plugin()
        {
            _globalCache = new GlobalCache
            {
                IconStorage = new IconStorage(TextureProvider),
                ItemStorage = new ItemStorage(),
                JobStorage = new JobStorage(),
                AddonStorage = new AddonStorage(),
                StainStorage = new StainStorage(),
                MinionStorage = new MinionStorage(),
                MountStorage = new MountStorage(),
                TripleTriadCardStorage = new TripleTriadCardStorage(),
                EmoteStorage = new EmoteStorage(),
                BardingStorage = new BardingStorage(),
                FramerKitStorage = new FramerKitStorage(),
                OrchestrionRollStorage = new OrchestrionRollStorage(),
                OrnamentStorage = new OrnamentStorage(),
                GlassesStorage = new GlassesStorage(),
                BeastTribesStorage = new BeastTribesStorage(),
                QuestStorage = new QuestStorage(),
                DutyStorage = new DutyStorage(),
                PlaceNameStorage = new PlaceNameStorage(),
            };

            nint playtimePtr = SigScanner.ScanText(PlaytimeSig);
            _playtimeHook = Hook.HookFromAddress<PlaytimeDelegate>(playtimePtr, PlaytimePacket);
            _playtimeHook.Enable();

#if DEBUG
            string dbpath = Path.Combine(PluginInterface.AssemblyLocation.Directory?.FullName!, "altoholic.db");
            Log.Info($"dbpath = {dbpath}");
#else
            string dbpath = Path.Combine(PluginInterface.GetPluginConfigDirectory(), "altoholic.db");
#endif
            if (File.Exists(dbpath))
            {
                List<Character> characters = [];
                bool isSQliteDb = Database.Database.IsSqLiteDatabase(dbpath);
                if (!isSQliteDb)
                {
                    Log.Info("Database is not SQLite, starting migration");
                    LiteDatabase db = new(dbpath);
                    characters = Database.Database.GetDataFromLite(db);
                    db.Dispose();
                    File.Delete(dbpath);
                }

                _db = Database.Database.CreateDatabaseConnection(dbpath);
                _db.Open();
                Database.Database.CheckOrCreateDatabases(_db);
                if (!isSQliteDb)
                {
                    Database.Database.AddCharacters(_db, characters);
                }
                Log.Info("Migration finished");
            }
            else
            {
                _db = Database.Database.CreateDatabaseConnection(dbpath);
                _db.Open();
                Database.Database.CheckOrCreateDatabases(_db);
            }

            BlacklistedCharacters = Database.Database.GetBlacklists(_db);
#if DEBUG
            Log.Debug("BlacklistedCharacters.Count: ", BlacklistedCharacters.Count);
            foreach (Blacklist blacklistedCharacter in BlacklistedCharacters)
            {
                Log.Debug($"Blacklisted id: {blacklistedCharacter.CharacterId}");
            }
#endif
            Configuration = PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
            Configuration.Initialize(PluginInterface);

            if (Configuration.Language != ClientState.ClientLanguage)
            {
                Configuration.Language = ClientState.ClientLanguage;
                Configuration.Save();
            }
            ClientLanguage currentLocale = Configuration.Language;
            _localization.SetupWithLangCode(PluginInterface.UiLanguage);

            _globalCache.IconStorage.Init();
            _globalCache.ItemStorage.Init();
            _globalCache.BardingStorage.Init(currentLocale, _globalCache);
            _globalCache.EmoteStorage.Init(currentLocale, _globalCache);
            _globalCache.OrnamentStorage.Init(currentLocale, _globalCache);
            _globalCache.FramerKitStorage.Init(currentLocale, _globalCache);
            _globalCache.GlassesStorage.Init(currentLocale, _globalCache);
            _globalCache.MinionStorage.Init(currentLocale, _globalCache);
            _globalCache.MountStorage.Init(currentLocale, _globalCache);
            _globalCache.OrchestrionRollStorage.Init(currentLocale, _globalCache);
            _globalCache.TripleTriadCardStorage.Init(currentLocale, _globalCache);
            _globalCache.BeastTribesStorage.Init(currentLocale, _globalCache);
            _globalCache.QuestStorage.Init(_globalCache);
            _globalCache.DutyStorage.Init(_globalCache);
            _globalCache.PlaceNameStorage.Init(_globalCache);

            _altoholicService = new(
                () => _localPlayer,
                () => OtherCharacters,
                () => BlacklistedCharacters
            );

            ConfigWindow = new ConfigWindow(this, $"{Name} configuration", _globalCache);
            CharactersWindow = new CharactersWindow(this, $"{Name} characters", _db, _globalCache)
            {
                GetPlayer = () => _altoholicService.GetPlayer(),
                GetOthersCharactersList = () => _altoholicService.GetOthersCharacters(),
            };

            DetailsWindow = new DetailsWindow(this, $"{Name} characters details", _db, _globalCache)
            {
                GetPlayer = () => _altoholicService.GetPlayer(),
                GetOthersCharactersList = () => _altoholicService.GetOthersCharacters(),
            };

            JobsWindow = new JobsWindow(this, $"{Name} characters jobs", _globalCache)
            {
                GetPlayer = () => _altoholicService.GetPlayer(),
                GetOthersCharactersList = () => _altoholicService.GetOthersCharacters(),
            };

            CurrenciesWindow = new CurrenciesWindow(this, $"{Name} characters currencies", _globalCache)
            {
                GetPlayer = () => _altoholicService.GetPlayer(),
                GetOthersCharactersList = () => _altoholicService.GetOthersCharacters(),
            };

            InventoriesWindow = new InventoriesWindow(this, $"{Name} characters inventories", _globalCache)
            {
                GetPlayer = () => _altoholicService.GetPlayer(),
                GetOthersCharactersList = () => _altoholicService.GetOthersCharacters(),
            };

            RetainersWindow = new RetainersWindow(this, $"{Name} characters retainers", _globalCache)
            {
                GetPlayer = () => _altoholicService.GetPlayer(),
                GetOthersCharactersList = () => _altoholicService.GetOthersCharacters(),
            };

            CollectionWindow = new CollectionWindow(this, $"{Name} characters colletion", _globalCache)
            {
                GetPlayer = () => _altoholicService.GetPlayer(),
                GetOthersCharactersList = () => _altoholicService.GetOthersCharacters(),
            };

            ProgressWindow = new ProgressWindow(this, $"{Name} characters progress", _globalCache)
            {
                GetPlayer = () => _altoholicService.GetPlayer(),
                GetOthersCharactersList = () => _altoholicService.GetOthersCharacters(),
            };


            MainWindow = new MainWindow(
                this,
                $"{Name} characters",
                _globalCache,
                CharactersWindow,
                DetailsWindow,
                JobsWindow,
                CurrenciesWindow,
                InventoriesWindow,
                RetainersWindow,
                CollectionWindow,
                ProgressWindow,
                ConfigWindow);

            WindowSystem.AddWindow(ConfigWindow);
            WindowSystem.AddWindow(MainWindow);

            CommandManager.AddHandler(CommandName, new CommandInfo(OnCommand)
            {
                HelpMessage = "Show/hide Altoholic. use /altoholic help for more options"
            });

            PluginInterface.UiBuilder.Draw += DrawUI;
            PluginInterface.UiBuilder.OpenConfigUi += DrawConfigUI;
            PluginInterface.UiBuilder.OpenMainUi += DrawMainUI;
            PluginInterface.LanguageChanged += _localization.SetupWithLangCode;
            DutyState.DutyCompleted += OnDutyCompleted;
            GameInventory.ItemAdded += OnGameInventoryItemEvent;
            GameInventory.ItemChanged += OnGameInventoryItemEvent;
            GameInventory.ItemMerged += OnGameInventoryItemEvent;
            GameInventory.ItemRemoved += OnGameInventoryItemEvent;
            ClientState.Login += OnCharacterLogin;
            ClientState.Logout += OnCharacterLogout;
            Framework.Update += OnFrameworkUpdate;

            _autoSaveWatch.Start();
            Log.Info("Starting altoholic autosave timer");
        }

        public void Dispose()
        {
            _playtimeHook.Dispose();
            WindowSystem.RemoveAllWindows();

            PluginInterface.UiBuilder.Draw -= DrawUI;
            PluginInterface.UiBuilder.OpenConfigUi -= DrawConfigUI;
            PluginInterface.UiBuilder.OpenMainUi -= DrawMainUI;
            PluginInterface.LanguageChanged -= _localization.SetupWithLangCode;

            _globalCache.IconStorage.Dispose();
            _globalCache.ItemStorage.Dispose();
            _globalCache.JobStorage.Dispose();
            _globalCache.AddonStorage.Dispose();
            _globalCache.StainStorage.Dispose();
            _globalCache.MinionStorage.Dispose();
            _globalCache.MountStorage.Dispose();
            _globalCache.TripleTriadCardStorage.Dispose();
            _globalCache.EmoteStorage.Dispose();
            _globalCache.BardingStorage.Dispose();
            _globalCache.FramerKitStorage.Dispose();
            _globalCache.OrnamentStorage.Dispose();

            CollectionWindow.Dispose();
            RetainersWindow.Dispose();
            InventoriesWindow.Dispose();
            JobsWindow.Dispose();
            ConfigWindow.Dispose();
            CurrenciesWindow.Dispose();
            DetailsWindow.Dispose();
            CharactersWindow.Dispose();
            MainWindow.Dispose();

            CleanLastLocalCharacter();

            CommandManager.RemoveHandler(CommandName);

            GameInventory.ItemAdded -= OnGameInventoryItemEvent;
            GameInventory.ItemChanged -= OnGameInventoryItemEvent;
            GameInventory.ItemMerged -= OnGameInventoryItemEvent;
            GameInventory.ItemRemoved -= OnGameInventoryItemEvent;
            DutyState.DutyCompleted -= OnDutyCompleted;
            ClientState.Login -= OnCharacterLogin;
            ClientState.Logout -= OnCharacterLogout;
            Framework.Update -= OnFrameworkUpdate;
            _db.Close();
            _db.Dispose();
            SqliteConnection.ClearAllPools();
            _autoSaveWatch.Stop();
        }

        private void OnUnblacklistCommand()
        {
            Log.Debug("OnBlacklistCommand called");
            Blacklist? blacklist = BlacklistedCharacters.Find(b => b.CharacterId == ClientState.LocalContentId);
            if (blacklist is null)
            {
                Utils.ChatMessage("Character not found in the blacklist");
                return;
            }
            Database.Database.DeleteBlacklist(_db, blacklist.CharacterId);
            BlacklistedCharacters.Remove(blacklist);
            ChatGui.Print("Character removed from blacklist");
        }

        private void OnCommand(string command, string args)
        {
            Log.Debug($"OnCommand called {(args.Length > 0 ? args : "")}");

            switch (args.Replace("<", "").Replace(">", ""))
            {
                case "help":
                    SeStringBuilder builder = new();
                    builder.Append("[Altoholic] /altoholic ");
                    builder.PushColorRgba(KnownColor.LimeGreen.Vector());
                    builder.Append("option\n");
                    builder.PopColor();
                    builder.PushColorRgba(KnownColor.LimeGreen.Vector());
                    builder.Append("No option");
                    builder.PopColor();
                    builder.Append(": Show/hide Altoholic main window\n");
                    builder.Append("Options:\n");
                    builder.PushColorRgba(KnownColor.LimeGreen.Vector());
                    builder.Append("help");
                    builder.PopColor();
                    builder.Append(": Show this help\n");
#if DEBUG
                    builder.PushColorRgba(KnownColor.LimeGreen.Vector());
                    builder.Append("save");
                    builder.PopColor();
                    builder.Append(": Manually save current character\n");
#endif
                    builder.PushColorRgba(KnownColor.LimeGreen.Vector());
                    builder.Append("reset");
                    builder.PopColor();
                    builder.Append(" : Reset main window position");
                    builder.PushColorRgba(KnownColor.LimeGreen.Vector());
                    builder.Append("unbl");
                    builder.PopColor();
                    builder.Append("/");
                    builder.PushColorRgba(KnownColor.LimeGreen.Vector());
                    builder.Append("unblacklist");
                    builder.PopColor();
                    builder.Append(" : Remove current character from blacklist");

                    XivChatEntry chatEntry = new() { Message = builder.ToSeString().ToDalamudString(), Type = XivChatType.Echo };

                    ChatGui.Print(chatEntry);
                    break;
                case "reset":
                    Log.Debug($"Window position: {MainWindow.Position}");
                    MainWindow.Position = new Vector2(100, 100);
                    DrawMainUI();
                    break;
                case "save":
                    UpdateCharacter();
                    break;
                case "unblacklist":
                case "unbl":
                    OnUnblacklistCommand();
                    break;
                default:
                    if (MainWindow.IsOpen)
                    {
                        MainWindow.IsOpen = false;
                    }
                    else
                    {
                        Log.Debug("No argument, opening window");
                        DrawMainUI();
                    }
                    break;
            }
        }

        // ReSharper disable once InconsistentNaming
        private void DrawUI()
        {
            WindowSystem.Draw();
        }

        // ReSharper disable once InconsistentNaming
        private void DrawMainUI()
        {
            if (!ClientState.IsLoggedIn)
            {
                Log.Error("No character logged in, doing nothing");

                NotificationManager.AddNotification(new Notification
                {
                    Title = "Altoholic",
                    Content = "This plugin need a character to be logged in",
                    Type = NotificationType.Error,
                    Minimized = false,
                    InitialDuration = TimeSpan.FromSeconds(3)
                });
                return;
            }
            Plugin.Log.Debug($"localPlayerName : {_localPlayer.CharacterId} {_localPlayer.FirstName} {_localPlayer.LastName}");
            if (_localPlayer.CharacterId == 0 || string.IsNullOrEmpty(_localPlayer.FirstName))
            {
                Character? p = GetCharacterFromGameOrDatabase();
                if (p is null)
                {
                    if (_altoholicService.GetBlacklistedCharacters()
                        .Exists(b => b.CharacterId == _localPlayer.CharacterId))
                    {
                        PrintBlacklistedMessage();
                    }

                    return;
                };

                _localPlayer = p;
                //Plugin.Log.Debug($"localPlayerPlayTime : {localPlayerPlayTime}");
                GetCharacterPlaytime();

#if DEBUG
                Log.Debug($"Character localLastPlayTimeUpdate : {_localPlayer.LastPlayTimeUpdate}");

                /*foreach (Inventory inventory in _localPlayer.Inventory)
                {
                    Plugin.Log.Debug($"{inventory.ItemId} {lumina.Singular} {inventory.HQ} {inventory.Quantity}");
                }*/
#endif
            }

            if (_localPlayer.CharacterId != 0)
            {
                OtherCharacters = Database.Database.GetOthersCharacters(_db, _localPlayer.CharacterId);

                //Plugin.Log.Debug($"otherCharacters count {otherCharacters.Count}");
                if (_altoholicService.GetBlacklistedCharacters()
                    .Exists(b => b.CharacterId == _localPlayer.CharacterId))
                {
                    PrintBlacklistedMessage();
                    return;
                }

                GetCharacterPlaytime();
                if (_localPlayer.LastPlayTimeUpdate > 0 && Utils.GetLastPlayTimeUpdateDiff(_localPlayer.LastPlayTimeUpdate) >= 7)
                {
                    Utils.ChatMessage(Loc.Localize("LastPlaytimeOutdated", "More than 7 days since the last update, consider using the /playtime command"));
                }

                if (_localPlayer.PlayTime == 0)
                {
                    Utils.ChatMessage(Loc.Localize("NoPlaytimeFound", "No playtime found, use /playtime"));
                }

                //Plugin.Log.Debug($"localPlayer.Quests.Count: {localPlayer.Quests.Count}");
                if (_localPlayer.Quests.Count == 0)
                {
                    //Plugin.Log.Debug("No quest found, fetching from db");
                    Character? chara = Database.Database.GetCharacter(_db, _localPlayer.CharacterId);
                    if (chara != null)
                    {
                        _localPlayer.Quests = chara.Quests;
                    }
                }
                //Plugin.Log.Debug($"localPlayer.Quests.Count: {localPlayer.Quests.Count}");
                if (_localPlayer.Retainers.Count == 0)
                {
                    Character? chara = Database.Database.GetCharacter(_db, _localPlayer.CharacterId);
                    if (chara != null)
                    {
                        _localPlayer.Retainers = chara.Retainers;
                    }
                }

                Character? character = Database.Database.GetCharacter(_db, _localPlayer.CharacterId);
                if(character != null)
                {
                    foreach (Housing housing in character.Houses.Where(housing => !_localPlayer.Houses.Exists(h => h.Id == housing.Id)))
                    {
                        _localPlayer.Houses.Add(housing);
                    }
                }

#if DEBUG
                foreach (Gear i in _localPlayer.Gear)
                {
                    Log.Debug($"Gear: {i.ItemId} {Enum.GetName(typeof(GearSlot), i.Slot)} {i.Slot}");
                }

                /*Plugin.Log.Debug($"Title {localPlayer.Profile.Title}");
                Plugin.Log.Debug($"Title Length {localPlayer.Profile.Title.Length}");
                Plugin.Log.Debug($"Grand Company {localPlayer.Profile.Grand_Company}");
                Plugin.Log.Debug($"Grand Company Rank {localPlayer.Profile.Grand_Company_Rank}");
                Plugin.Log.Debug($"Gender {Utils.GetGender(localPlayer.Profile.Gender)}");
                Plugin.Log.Debug($"Race {Utils.GetRace(localPlayer.Profile.Gender, localPlayer.Profile.Race)}");
                Plugin.Log.Debug($"Tribe {Utils.GetTribe(localPlayer.Profile.Gender, localPlayer.Profile.Tribe)}");
                Plugin.Log.Debug($"City State {Utils.GetTown(localPlayer.Profile.City_State)}");
                Plugin.Log.Debug($"Nameday_Day {localPlayer.Profile.Nameday_Day}");
                Plugin.Log.Debug($"Nameday_Month {localPlayer.Profile.Nameday_Month}");
                Plugin.Log.Debug($"Guardian {Utils.GetGuardian(localPlayer.Profile.Guardian)}");*/
#endif
                IPlayerCharacter? lPlayer = ClientState.LocalPlayer;
                if (lPlayer != null)
                {
                    _localPlayer.Attributes = new Attributes
                    {
                        Hp = lPlayer.MaxHp,
                        Mp = lPlayer.MaxMp
                    };

                    GetPlayerAttributesProfileAndJobs();
                    GetPlayerEquippedGear();
                    GetPlayerInventory();
                    GetPlayerSaddleInventory();
                    GetPlayerArmoryInventory();
                    GetPlayerGlamourInventory();
                    GetPlayerArmoireInventory();
                    GetPlayerMail();
                    GetPlayerCompletedQuests();
                    GetPlayerRetainer();
                    GetPlayerBeastReputations();
                    GetDuties();

                    GetCollectionFromState();
                    /*
                    Log.Debug($"localPlayer.Inventory.Count : {localPlayer.Inventory.Count}");
                    Log.Debug($"localPlayer.Saddle.Count: {localPlayer.Saddle.Count}");
                    foreach (var inventory in localPlayer.Saddle)
                    {
                        Log.Debug($"{inventory.ItemId} {inventory.HQ} {inventory.Quantity}");
                    }
                    */
                    /*Log.Debug($"localPlayer retainers count : {localPlayer.Retainers.Count}");
                    foreach(Retainer retainer in localPlayer.Retainers)
                    {
                        Log.Debug($"{retainer.Name} job:{Enum.GetName(typeof(ClassJob), retainer.ClassJob)}, displayorder: {retainer.DisplayOrder}, items: {retainer.Inventory.Count}, gils: {retainer.Gils}");
                    }*/
#if DEBUG
                    if (_localPlayer.ArmoryInventory != null)
                    {
                        Log.Debug($"localPlayer.ArmoryInventory.MainHand.Count : {_localPlayer.ArmoryInventory.MainHand.FindAll(i => i.ItemId != 0).Count}");
                        Log.Debug($"localPlayer.ArmoryInventory.Head.Count : {_localPlayer.ArmoryInventory.Head.FindAll(i => i.ItemId != 0).Count}");
                        Log.Debug($"localPlayer.ArmoryInventory.Body.Count : {_localPlayer.ArmoryInventory.Body.FindAll(i => i.ItemId != 0).Count}");
                        Log.Debug($"localPlayer.ArmoryInventory.Hands.Count : {_localPlayer.ArmoryInventory.Hands.FindAll(i => i.ItemId != 0).Count}");
                        Log.Debug($"localPlayer.ArmoryInventory.Legs.Count : {_localPlayer.ArmoryInventory.Legs.FindAll(i => i.ItemId != 0).Count}");
                        Log.Debug($"localPlayer.ArmoryInventory.Feets.Count : {_localPlayer.ArmoryInventory.Feets.FindAll(i => i.ItemId != 0).Count}");
                        Log.Debug($"localPlayer.ArmoryInventory.OffHand.Count : {_localPlayer.ArmoryInventory.OffHand.FindAll(i => i.ItemId != 0).Count}");
                        Log.Debug($"localPlayer.ArmoryInventory.Ear.Count : {_localPlayer.ArmoryInventory.Ear.FindAll(i => i.ItemId != 0).Count}");
                        Log.Debug($"localPlayer.ArmoryInventory.Neck.Count : {_localPlayer.ArmoryInventory.Neck.FindAll(i => i.ItemId != 0).Count}");
                        Log.Debug($"localPlayer.ArmoryInventory.Wrist.Count : {_localPlayer.ArmoryInventory.Wrist.FindAll(i => i.ItemId != 0).Count}");
                        Log.Debug($"localPlayer.ArmoryInventory.Rings.Count : {_localPlayer.ArmoryInventory.Rings.FindAll(i => i.ItemId != 0).Count}");
                        Log.Debug($"localPlayer.ArmoryInventory.SoulCrystal.Count : {_localPlayer.ArmoryInventory.SoulCrystal.FindAll(i => i.ItemId != 0).Count}");
                        /*foreach (var inventory in localPlayer.ArmoryInventory.Hands)
                        {
                            Log.Debug($"{inventory.ItemId} {Utils.GetItemNameFromId(inventory.ItemId)} {inventory.HQ}");
                        }*/
                    }
#endif
                }

                if (ClientState.IsLoggedIn)
                {
                    _localPlayer.LastOnline = 0;
                }
#if DEBUG
                /*Plugin.Log.Debug($"Gladiator : {localPlayer.Jobs.Gladiator.Level}");
                Plugin.Log.Debug($"Pugilist : {localPlayer.Jobs.Pugilist.Level}");
                Plugin.Log.Debug($"Marauder : {localPlayer.Jobs.Marauder.Level}");
                Plugin.Log.Debug($"Lancer : {localPlayer.Jobs.Lancer.Level}");
                Plugin.Log.Debug($"Archer : {localPlayer.Jobs.Archer.Level}");
                Plugin.Log.Debug($"Conjurer : {localPlayer.Jobs.Conjurer.Level}");
                Plugin.Log.Debug($"Thaumaturge : {localPlayer.Jobs.Thaumaturge.Level}");
                Plugin.Log.Debug($"Carpenter : {localPlayer.Jobs.Carpenter.Level}");
                Plugin.Log.Debug($"Blacksmith : {localPlayer.Jobs.Blacksmith.Level}");
                Plugin.Log.Debug($"Armorer : {localPlayer.Jobs.Armorer.Level}");
                Plugin.Log.Debug($"Goldsmith : {localPlayer.Jobs.Goldsmith.Level}");
                Plugin.Log.Debug($"Leatherworker : {localPlayer.Jobs.Leatherworker.Level}");
                Plugin.Log.Debug($"Weaver : {localPlayer.Jobs.Weaver.Level}");
                Plugin.Log.Debug($"Alchemist : {localPlayer.Jobs.Alchemist.Level}");
                Plugin.Log.Debug($"Culinarian : {localPlayer.Jobs.Culinarian.Level}");
                Plugin.Log.Debug($"Miner : {localPlayer.Jobs.Miner.Level}");
                Plugin.Log.Debug($"Botanist : {localPlayer.Jobs.Botanist.Level}");
                Plugin.Log.Debug($"Fisher : {localPlayer.Jobs.Fisher.Level}");
                Plugin.Log.Debug($"Paladin : {localPlayer.Jobs.Paladin.Level}");
                Plugin.Log.Debug($"Monk : {localPlayer.Jobs.Monk.Level}");
                Plugin.Log.Debug($"Warrior : {localPlayer.Jobs.Warrior.Level}");
                Plugin.Log.Debug($"Dragoon : {localPlayer.Jobs.Dragoon.Level}");
                Plugin.Log.Debug($"Bard : {localPlayer.Jobs.Bard.Level}");
                Plugin.Log.Debug($"WhiteMage : {localPlayer.Jobs.WhiteMage.Level}");
                Plugin.Log.Debug($"BlackMage : {localPlayer.Jobs.BlackMage.Level}");
                Plugin.Log.Debug($"Arcanist : {localPlayer.Jobs.Arcanist.Level}");
                Plugin.Log.Debug($"Summoner : {localPlayer.Jobs.Summoner.Level}");
                Plugin.Log.Debug($"Scholar : {localPlayer.Jobs.Scholar.Level}");
                Plugin.Log.Debug($"Rogue : {localPlayer.Jobs.Rogue.Level}");
                Plugin.Log.Debug($"Ninja : {localPlayer.Jobs.Ninja.Level}");
                Plugin.Log.Debug($"Machinist : {localPlayer.Jobs.Machinist.Level}");
                Plugin.Log.Debug($"DarkKnight : {localPlayer.Jobs.DarkKnight.Level}");
                Plugin.Log.Debug($"Astrologian : {localPlayer.Jobs.Astrologian.Level}");
                Plugin.Log.Debug($"Samurai : {localPlayer.Jobs.Samurai.Level}");
                Plugin.Log.Debug($"RedMage : {localPlayer.Jobs.RedMage.Level}");
                Plugin.Log.Debug($"BlueMage : {localPlayer.Jobs.BlueMage.Level}");
                Plugin.Log.Debug($"Gunbreaker : {localPlayer.Jobs.Gunbreaker.Level}");
                Plugin.Log.Debug($"Dancer : {localPlayer.Jobs.Dancer.Level}");
                Plugin.Log.Debug($"Reaper : {localPlayer.Jobs.Reaper.Level}");
                Plugin.Log.Debug($"Sage : {localPlayer.Jobs.Sage.Level}");*/
#endif
            }
            MainWindow.IsOpen = true;
        }

        // ReSharper disable once InconsistentNaming
        public void DrawConfigUI()
        {
            ConfigWindow.IsOpen = true;
        }

        private void GetCharacterPlaytime()
        {
            if (_localPlayer.PlayTime != 0)
            {
                return;
            }

            Character? chara = Database.Database.GetCharacter(_db, _localPlayer.CharacterId);
            if (chara is null || chara.PlayTime == 0)
            {
                return;
            }

            _localPlayer.PlayTime = chara.PlayTime;
            _localPlayer.LastPlayTimeUpdate = chara.LastPlayTimeUpdate;
        }

        private void OnFrameworkUpdate(IFramework framework)
        {
            IPlayerCharacter? lPlayer = ClientState.LocalPlayer;
            if (lPlayer == null)
            {
                return;
            }

            if (_localPlayer.CharacterId == 0)
            {
                _localPlayer = new Character { CharacterId = ClientState.LocalContentId };
            }

            if (BlacklistedCharacters.Exists(b => b.CharacterId == _localPlayer.CharacterId)) return;

            string name = lPlayer.Name.TextValue;
            if (string.IsNullOrEmpty(name)) return;
            string[] names = name.Split(" ");
            if (names.Length == 2)
            {
                _localPlayer.FirstName = names[0];
                _localPlayer.LastName = names[1];
            }
            World? hw = lPlayer.HomeWorld.ValueNullable;
            if (hw != null)
            {
                _localPlayer.HomeWorld = hw.Value.Name.ExtractText();
                _localPlayer.Datacenter = Utils.GetDatacenterFromWorld(_localPlayer.HomeWorld);
                _localPlayer.Region = Utils.GetRegionFromWorld(_localPlayer.HomeWorld);
            }
            
            World? cw = lPlayer.CurrentWorld.ValueNullable;
            if (cw != null)
            {
                _localPlayer.CurrentWorld = cw.Value.Name.ExtractText();
                _localPlayer.CurrentDatacenter = Utils.GetDatacenterFromWorld(_localPlayer.CurrentWorld);
                _localPlayer.CurrentRegion = Utils.GetRegionFromWorld(_localPlayer.CurrentWorld);
            }


            uint job = lPlayer.ClassJob.RowId;
            if (job != _localPlayer.LastJob)
            {
                GetPlayerEquippedGear();
            }
            _localPlayer.LastJob = job;
            _localPlayer.LastJobLevel = lPlayer.Level;
            _localPlayer.FCTag = lPlayer.CompanyTag.TextValue;
            _localPlayer.Attributes = new Attributes
            {
                Hp = lPlayer.MaxHp,
                Mp = lPlayer.MaxMp
            };

            GetPlayerAttributesProfileAndJobs();
            GetPlayerRetainer();
            GetPlayerMail();
            GetHousing();

            if (_autoSaveWatch.Elapsed.Minutes < Configuration.AutoSaveTimer)
            {
                return;
            }

            UpdateCharacter();
            _autoSaveWatch.Restart();
        }

        private unsafe void GetPlayerAttributesProfileAndJobs()
        {
            if (_localPlayer.CharacterId == 0) return;
            string title = string.Empty;
            bool prefixTitle = false;
            RaptureAtkModule* raptureAtkModule = FFXIVClientStructs.FFXIV.Client.System.Framework.Framework.Instance()->UIModule->GetRaptureAtkModule();
            if (raptureAtkModule != null)
            {
                Span<NamePlateInfo> npi = raptureAtkModule->NamePlateInfoEntries;
                if (npi != null)
                {
                    for (int i = 0; i < 50 && i < raptureAtkModule->NameplateInfoCount; i++)
                    {
                        ref NamePlateInfo namePlateInfo = ref npi[i];
                        if (ClientState.LocalPlayer == null) continue;
                        if (namePlateInfo.ObjectId.ObjectId != ClientState.LocalPlayer.EntityId)
                        {
                            continue;
                        }

                        string t = namePlateInfo.Title.ToString();//this sometime get player name??? to recheck
                        //Plugin.Log.Debug($"t: {t}");
                        if (t == $"{_localPlayer.FirstName} {_localPlayer.LastName}")
                        {
                            continue;
                        }

                        title = t;
                        prefixTitle = namePlateInfo.IsPrefixTitle;
                    }
                }
            }
            ref readonly UIState uistate = ref *UIState.Instance();//nullcheck?
            PlayerState player = uistate.PlayerState;
            _localPlayer.IsSprout = player.IsNovice();
            _localPlayer.IsBattleMentor = player.IsBattleMentor();
            _localPlayer.IsTradeMentor = player.IsTradeMentor();
            _localPlayer.IsReturner = player.IsReturner();
            _localPlayer.HasPremiumSaddlebag = player.HasPremiumSaddlebag;
            _localPlayer.PlayerCommendations = player.PlayerCommendations;
            _localPlayer.Profile = new Profile
            {
                Title = title,
                TitleIsPrefix = prefixTitle,
                GrandCompany = player.GrandCompany,
                GrandCompanyRank = player.GetGrandCompanyRank(),
                Race = player.Race,
                Tribe = player.Tribe,
                Gender = player.Sex,
                CityState = player.StartTown,
                NamedayDay = player.BirthDay,
                NamedayMonth = player.BirthMonth,
                Guardian = player.GuardianDeity
            };
            _localPlayer.Jobs = new Jobs
            {
                //Adventurer = new Job { Level = player.ClassJobLevels[-1], Exp = player.ClassJobExperience[-1] },
                Gladiator = new Job { Level = player.ClassJobLevels[1], Exp = player.ClassJobExperience[1] },
                Pugilist = new Job { Level = player.ClassJobLevels[0], Exp = player.ClassJobExperience[0] },
                Marauder = new Job { Level = player.ClassJobLevels[2], Exp = player.ClassJobExperience[2] },
                Lancer = new Job { Level = player.ClassJobLevels[4], Exp = player.ClassJobExperience[4] },
                Archer = new Job { Level = player.ClassJobLevels[3], Exp = player.ClassJobExperience[3] },
                Conjurer = new Job { Level = player.ClassJobLevels[6], Exp = player.ClassJobExperience[6] },
                Thaumaturge = new Job { Level = player.ClassJobLevels[5], Exp = player.ClassJobExperience[5] },
                Carpenter = new Job { Level = player.ClassJobLevels[7], Exp = player.ClassJobExperience[7] },
                Blacksmith = new Job { Level = player.ClassJobLevels[8], Exp = player.ClassJobExperience[8] },
                Armorer = new Job { Level = player.ClassJobLevels[9], Exp = player.ClassJobExperience[9] },
                Goldsmith = new Job { Level = player.ClassJobLevels[10], Exp = player.ClassJobExperience[10] },
                Leatherworker = new Job { Level = player.ClassJobLevels[11], Exp = player.ClassJobExperience[11] },
                Weaver = new Job { Level = player.ClassJobLevels[12], Exp = player.ClassJobExperience[12] },
                Alchemist = new Job { Level = player.ClassJobLevels[13], Exp = player.ClassJobExperience[13] },
                Culinarian = new Job { Level = player.ClassJobLevels[14], Exp = player.ClassJobExperience[14] },
                Miner = new Job { Level = player.ClassJobLevels[15], Exp = player.ClassJobExperience[15] },
                Botanist = new Job { Level = player.ClassJobLevels[16], Exp = player.ClassJobExperience[16] },
                Fisher = new Job { Level = player.ClassJobLevels[17], Exp = player.ClassJobExperience[17] },
                Paladin = new Job { Level = player.ClassJobLevels[1], Exp = player.ClassJobExperience[1] },
                Monk = new Job { Level = player.ClassJobLevels[0], Exp = player.ClassJobExperience[0] },
                Warrior = new Job { Level = player.ClassJobLevels[2], Exp = player.ClassJobExperience[2] },
                Dragoon = new Job { Level = player.ClassJobLevels[4], Exp = player.ClassJobExperience[4] },
                Bard = new Job { Level = player.ClassJobLevels[3], Exp = player.ClassJobExperience[3] },
                WhiteMage = new Job { Level = player.ClassJobLevels[6], Exp = player.ClassJobExperience[6] },
                BlackMage = new Job { Level = player.ClassJobLevels[5], Exp = player.ClassJobExperience[5] },
                Arcanist = new Job { Level = player.ClassJobLevels[18], Exp = player.ClassJobExperience[18] },
                Summoner = new Job { Level = player.ClassJobLevels[18], Exp = player.ClassJobExperience[18] },
                Scholar = new Job { Level = player.ClassJobLevels[18], Exp = player.ClassJobExperience[18] },
                Ninja = new Job { Level = player.ClassJobLevels[19], Exp = player.ClassJobExperience[19] },
                Rogue = new Job { Level = player.ClassJobLevels[19], Exp = player.ClassJobExperience[19] },
                Machinist = new Job { Level = player.ClassJobLevels[20], Exp = player.ClassJobExperience[20] },
                DarkKnight = new Job { Level = player.ClassJobLevels[21], Exp = player.ClassJobExperience[21] },
                Astrologian = new Job { Level = player.ClassJobLevels[22], Exp = player.ClassJobExperience[22] },
                Samurai = new Job { Level = player.ClassJobLevels[23], Exp = player.ClassJobExperience[23] },
                RedMage = new Job { Level = player.ClassJobLevels[24], Exp = player.ClassJobExperience[24] },
                BlueMage = new Job { Level = player.ClassJobLevels[25], Exp = player.ClassJobExperience[25] },
                Gunbreaker = new Job { Level = player.ClassJobLevels[26], Exp = player.ClassJobExperience[26] },
                Dancer = new Job { Level = player.ClassJobLevels[27], Exp = player.ClassJobExperience[27] },
                Reaper = new Job { Level = player.ClassJobLevels[28], Exp = player.ClassJobExperience[28] },
                Sage = new Job { Level = player.ClassJobLevels[29], Exp = player.ClassJobExperience[29] },
                Viper = new Job { Level = player.ClassJobLevels[30], Exp = player.ClassJobExperience[30] },
                Pictomancer = new Job { Level = player.ClassJobLevels[31], Exp = player.ClassJobExperience[31] },
            };

            ref readonly BattleChara lp = ref *Control.GetLocalPlayer();
            _localPlayer.CurrentFacewear[0] = lp.DrawData.GlassesIds[0];
            _localPlayer.CurrentFacewear[1] = lp.DrawData.GlassesIds[1];
            _localPlayer.CurrentOrnament = lp.OrnamentData.OrnamentId;

            /*foreach (uint i in Enumerable.Range(1001,79))
            {
                Log.Debug($"uistate.IsUnlockLinkUnlocked(i): {uistate.IsUnlockLinkUnlocked(i)}");
            }*/
        }

        private unsafe void GetCollectionFromState()
        {
            ref readonly UIState uistate = ref *UIState.Instance();//nullcheck?
            PlayerState player = uistate.PlayerState;
            foreach (uint i in _globalCache.MinionStorage.Get().Where(i => !_localPlayer.HasMinion(i)))
            {
                if (uistate.IsCompanionUnlocked(i))
                {
                    _localPlayer.Minions.Add(i);
                }
            }
            foreach (uint i in _globalCache.TripleTriadCardStorage.Get().Where(i => !_localPlayer.HasTTC(i)))
            {
                if (uistate.IsTripleTriadCardUnlocked((ushort)i))
                {
                    _localPlayer.TripleTriadCards.Add(i);
                }
            }
            foreach (uint i in _globalCache.EmoteStorage.Get().Where(i => !_localPlayer.HasEmote(i)))
                //foreach (KeyValuePair<uint, Models.Emote> i in _globalCache.EmoteStorage.GetAll().Where(i => !_localPlayer.HasEmote(i.Key)))
            {
                // Todo: Use UnlockLink instead of EmoteID
                if (uistate.IsEmoteUnlocked((ushort)i))
                    //if(uistate.IsUnlockLinkUnlocked(i.Value.UnlockLink))
                {
                    //_localPlayer.Emotes.Add(i.Key);
                    _localPlayer.Emotes.Add(i);
                }
            }
            foreach (uint i in _globalCache.BardingStorage.Get().Where(i => !_localPlayer.HasBarding(i)))
            {
                if (uistate.Buddy.CompanionInfo.IsBuddyEquipUnlocked(i))
                {
                    _localPlayer.Bardings.Add(i);
                }
            }

            GetMountFromState(player);
            GetFramerKitsFromState(player);
            GetOrchestrionRollFromState(player);
            GetOrnamentFromState(player);
            GetGlassesFromState(player);
        }

        private void GetMountFromState(PlayerState player)
        {
            foreach (uint i in _globalCache.MountStorage.Get().Where(i => !_localPlayer.HasMount(i))
                         .Where(i => player.IsMountUnlocked(i)))
            {
                _localPlayer.Mounts.Add(i);
            }
        }
        private void GetFramerKitsFromState(PlayerState player)
        {
            foreach (uint i in _globalCache.FramerKitStorage.Get().Where(i => !_localPlayer.HasFramerKit(i)).Where(i => player.IsFramersKitUnlocked(i)))
            {
                    _localPlayer.FramerKits.Add(i);
            }
        }
        private void GetOrchestrionRollFromState(PlayerState player)
        {
            foreach (uint i in _globalCache.OrchestrionRollStorage.Get().Where(i => !_localPlayer.HasOrchestrionRoll(i)).Where(i => player.IsOrchestrionRollUnlocked(i)))
            {
                    _localPlayer.OrchestrionRolls.Add(i);
            }
        }
        private void GetOrnamentFromState(PlayerState player)
        {
            foreach (uint i in _globalCache.OrnamentStorage.Get().Where(i => !_localPlayer.HasOrnament(i)).Where(i => player.IsOrnamentUnlocked(i)))
            {
                    _localPlayer.Ornaments.Add(i);
            }
        }
        private void GetGlassesFromState(PlayerState player)
        {
            foreach (uint i in _globalCache.GlassesStorage.Get().Where(i => !_localPlayer.HasGlasses(i)).Where(i => player.IsGlassesUnlocked((ushort)i)))
            {
                _localPlayer.Glasses.Add(i);
            }
        }

        /*private void GetBeastTribeRepFromState(PlayerState player)
        {
            player.GetBeastTribeRank(i);
        }*/

        private unsafe PlayerCurrencies GetPlayerCurrencies()
        {
            ref InventoryManager inventoryManager = ref *InventoryManager.Instance();
            return new PlayerCurrencies
            {
                Achievement_Certificate = inventoryManager.GetInventoryItemCount((uint)Currencies.ACHIEVEMENT_CERTIFICATE, false, false, false),
                Allagan_Tomestone_Of_Aesthetics = inventoryManager.GetInventoryItemCount((uint)Currencies.ALLAGAN_TOMESTONE_OF_AESTHETICS, false, false, false),
                Allagan_Tomestone_Of_Allegory = inventoryManager.GetInventoryItemCount((uint)Currencies.ALLAGAN_TOMESTONE_OF_ALLEGORY, false, false, false),
                Allagan_Tomestone_Of_Aphorism = inventoryManager.GetInventoryItemCount((uint)Currencies.ALLAGAN_TOMESTONE_OF_APHORISM, false, false, false),
                Allagan_Tomestone_Of_Astronomy = inventoryManager.GetInventoryItemCount((uint)Currencies.ALLAGAN_TOMESTONE_OF_ASTRONOMY, false, false, false),
                Allagan_Tomestone_Of_Causality = inventoryManager.GetInventoryItemCount((uint)Currencies.ALLAGAN_TOMESTONE_OF_CAUSALITY, false, false, false),
                Allagan_Tomestone_Of_Comedy = inventoryManager.GetInventoryItemCount((uint)Currencies.ALLAGAN_TOMESTONE_OF_COMEDY, false, false, false),
                Allagan_Tomestone_Of_Creation = inventoryManager.GetInventoryItemCount((uint)Currencies.ALLAGAN_TOMESTONE_OF_CREATION, false, false, false),
                Allagan_Tomestone_Of_Esoterics = inventoryManager.GetInventoryItemCount((uint)Currencies.ALLAGAN_TOMESTONE_OF_ESOTERICS, false, false, false),
                Allagan_Tomestone_Of_Genesis = inventoryManager.GetInventoryItemCount((uint)Currencies.ALLAGAN_TOMESTONE_OF_GENESIS, false, false, false),
                Allagan_Tomestone_Of_Goetia = inventoryManager.GetInventoryItemCount((uint)Currencies.ALLAGAN_TOMESTONE_OF_GOETIA, false, false, false),
                Allagan_Tomestone_Of_Heliometry = inventoryManager.GetInventoryItemCount((uint)Currencies.ALLAGAN_TOMESTONE_OF_HELIOMETRY, false, false, false),
                Allagan_Tomestone_Of_Law = inventoryManager.GetInventoryItemCount((uint)Currencies.ALLAGAN_TOMESTONE_OF_LAW, false, false, false),
                Allagan_Tomestone_Of_Lore = inventoryManager.GetInventoryItemCount((uint)Currencies.ALLAGAN_TOMESTONE_OF_LORE, false, false, false),
                Allagan_Tomestone_Of_Mendacity = inventoryManager.GetInventoryItemCount((uint)Currencies.ALLAGAN_TOMESTONE_OF_MENDACITY, false, false, false),
                Allagan_Tomestone_Of_Mythology = inventoryManager.GetInventoryItemCount((uint)Currencies.ALLAGAN_TOMESTONE_OF_MYTHOLOGY, false, false, false),
                Allagan_Tomestone_Of_Phantasmagoria = inventoryManager.GetInventoryItemCount((uint)Currencies.ALLAGAN_TOMESTONE_OF_PHANTASMAGORIA, false, false, false),
                Allagan_Tomestone_Of_Philosophy = inventoryManager.GetInventoryItemCount((uint)Currencies.ALLAGAN_TOMESTONE_OF_PHILOSOPHY, false, false, false),
                Allagan_Tomestone_Of_Poetics = inventoryManager.GetInventoryItemCount((uint)Currencies.ALLAGAN_TOMESTONE_OF_POETICS, false, false, false),
                Allagan_Tomestone_Of_Revelation = inventoryManager.GetInventoryItemCount((uint)Currencies.ALLAGAN_TOMESTONE_OF_REVELATION, false, false, false),
                Allagan_Tomestone_Of_Scripture = inventoryManager.GetInventoryItemCount((uint)Currencies.ALLAGAN_TOMESTONE_OF_SCRIPTURE, false, false, false),
                Allagan_Tomestone_Of_Soldiery = inventoryManager.GetInventoryItemCount((uint)Currencies.ALLAGAN_TOMESTONE_OF_SOLDIERY, false, false, false),
                Allagan_Tomestone_Of_Verity = inventoryManager.GetInventoryItemCount((uint)Currencies.ALLAGAN_TOMESTONE_OF_VERITY, false, false, false),
                Allied_Seal = inventoryManager.GetInventoryItemCount((uint)Currencies.ALLIED_SEAL, false, false, false),
                Ananta_Dreamstaff = inventoryManager.GetInventoryItemCount((uint)Currencies.ANANTA_DREAMSTAFF, false, false, false),
                Arkasodara_Pana = inventoryManager.GetInventoryItemCount((uint)Currencies.ARKASODARA_PANA, false, false, false),
                Bicolor_Gemstone = inventoryManager.GetInventoryItemCount((uint)Currencies.BICOLOR_GEMSTONE, false, false, false),
                Black_Copper_Gil = inventoryManager.GetInventoryItemCount((uint)Currencies.BLACK_COPPER_GIL, false, false, false),
                Bozjan_Cluster = inventoryManager.GetInventoryItemCount((uint)Currencies.BOZJAN_CLUSTER, false, false, false),
                Carved_Kupo_Nut = inventoryManager.GetInventoryItemCount((uint)Currencies.CARVED_KUPO_NUT, false, false, false),
                Centurio_Seal = inventoryManager.GetInventoryItemCount((uint)Currencies.CENTURIO_SEAL, false, false, false),
                Earth_Cluster = inventoryManager.GetInventoryItemCount((uint)Currencies.EARTH_CLUSTER, false, false, false),
                Earth_Crystal = inventoryManager.GetInventoryItemCount((uint)Currencies.EARTH_CRYSTAL, false, false, false),
                Earth_Shard = inventoryManager.GetInventoryItemCount((uint)Currencies.EARTH_SHARD, false, false, false),
                Fae_Fancy = inventoryManager.GetInventoryItemCount((uint)Currencies.FAE_FANCY, false, false, false),
                Faux_Leaf = inventoryManager.GetInventoryItemCount((uint)Currencies.FAUX_LEAF, false, false, false),
                Felicitous_Token = inventoryManager.GetInventoryItemCount((uint)Currencies.FELICITOUS_TOKEN, false, false, false),
                Fire_Cluster = inventoryManager.GetInventoryItemCount((uint)Currencies.FIRE_CLUSTER, false, false, false),
                Fire_Crystal = inventoryManager.GetInventoryItemCount((uint)Currencies.FIRE_CRYSTAL, false, false, false),
                Fire_Shard = inventoryManager.GetInventoryItemCount((uint)Currencies.FIRE_SHARD, false, false, false),
                Flame_Seal = inventoryManager.GetInventoryItemCount((uint)Currencies.FLAME_SEAL, false, false, false),
                Gil = inventoryManager.GetInventoryItemCount((uint)Currencies.GIL, false, false, false),
                Hammered_Frogment = inventoryManager.GetInventoryItemCount((uint)Currencies.HAMMERED_FROGMENT, false, false, false),
                Ice_Cluster = inventoryManager.GetInventoryItemCount((uint)Currencies.ICE_CLUSTER, false, false, false),
                Ice_Crystal = inventoryManager.GetInventoryItemCount((uint)Currencies.ICE_CRYSTAL, false, false, false),
                Ice_Shard = inventoryManager.GetInventoryItemCount((uint)Currencies.ICE_SHARD, false, false, false),
                Irregular_Tomestone_Of_Creation = inventoryManager.GetInventoryItemCount((uint)Currencies.IRREGULAR_TOMESTONE_OF_CREATION, false, false, false),
                Irregular_Tomestone_Of_Esoterics = inventoryManager.GetInventoryItemCount((uint)Currencies.IRREGULAR_TOMESTONE_OF_ESOTERICS, false, false, false),
                Irregular_Tomestone_Of_Genesis_i = inventoryManager.GetInventoryItemCount((uint)Currencies.IRREGULAR_TOMESTONE_OF_GENESIS_I, false, false, false),
                Irregular_Tomestone_Of_Genesis_ii = inventoryManager.GetInventoryItemCount((uint)Currencies.IRREGULAR_TOMESTONE_OF_GENESIS_II, false, false, false),
                Irregular_Tomestone_Of_Goetia = inventoryManager.GetInventoryItemCount((uint)Currencies.IRREGULAR_TOMESTONE_OF_GOETIA, false, false, false),
                Irregular_Tomestone_Of_Law = inventoryManager.GetInventoryItemCount((uint)Currencies.IRREGULAR_TOMESTONE_OF_LAW, false, false, false),
                Irregular_Tomestone_Of_Lore = inventoryManager.GetInventoryItemCount((uint)Currencies.IRREGULAR_TOMESTONE_OF_LORE, false, false, false),
                Irregular_Tomestone_Of_Mendacity = inventoryManager.GetInventoryItemCount((uint)Currencies.IRREGULAR_TOMESTONE_OF_MENDACITY, false, false, false),
                Irregular_Tomestone_Of_Mythology = inventoryManager.GetInventoryItemCount((uint)Currencies.IRREGULAR_TOMESTONE_OF_MYTHOLOGY, false, false, false),
                Irregular_Tomestone_Of_Pageantry = inventoryManager.GetInventoryItemCount((uint)Currencies.IRREGULAR_TOMESTONE_OF_PAGEANTRY, false, false, false),
                Irregular_Tomestone_Of_Phantasmagoria = inventoryManager.GetInventoryItemCount((uint)Currencies.IRREGULAR_TOMESTONE_OF_PHANTASMAGORIA, false, false, false),
                Irregular_Tomestone_Of_Philosophy = inventoryManager.GetInventoryItemCount((uint)Currencies.IRREGULAR_TOMESTONE_OF_PHILOSOPHY, false, false, false),
                Irregular_Tomestone_Of_Scripture = inventoryManager.GetInventoryItemCount((uint)Currencies.IRREGULAR_TOMESTONE_OF_SCRIPTURE, false, false, false),
                Irregular_Tomestone_Of_Soldiery = inventoryManager.GetInventoryItemCount((uint)Currencies.IRREGULAR_TOMESTONE_OF_SOLDIERY, false, false, false),
                Irregular_Tomestone_Of_Tenfold_pageantry = inventoryManager.GetInventoryItemCount((uint)Currencies.IRREGULAR_TOMESTONE_OF_TENFOLD_PAGEANTRY, false, false, false),
                Irregular_Tomestone_Of_Verity = inventoryManager.GetInventoryItemCount((uint)Currencies.IRREGULAR_TOMESTONE_OF_VERITY, false, false, false),
                Islanders_Cowrie = inventoryManager.GetInventoryItemCount((uint)Currencies.ISLANDERS_COWRIE, false, false, false),
                Ixali_Oaknot = inventoryManager.GetInventoryItemCount((uint)Currencies.IXALI_OAKNOT, false, false, false),
                Kojin_Sango = inventoryManager.GetInventoryItemCount((uint)Currencies.KOJIN_SANGO, false, false, false),
                Lightning_Cluster = inventoryManager.GetInventoryItemCount((uint)Currencies.LIGHTNING_CLUSTER, false, false, false),
                Lightning_Crystal = inventoryManager.GetInventoryItemCount((uint)Currencies.LIGHTNING_CRYSTAL, false, false, false),
                Lightning_Shard = inventoryManager.GetInventoryItemCount((uint)Currencies.LIGHTNING_SHARD, false, false, false),
                Loporrit_Carat = inventoryManager.GetInventoryItemCount((uint)Currencies.LOPORRIT_CARAT, false, false, false),
                MGF = inventoryManager.GetInventoryItemCount((uint)Currencies.MGF, false, false, false),
                MGP = inventoryManager.GetInventoryItemCount((uint)Currencies.MGP, false, false, false),
                Namazu_Koban = inventoryManager.GetInventoryItemCount((uint)Currencies.NAMAZU_KOBAN, false, false, false),
                Omicron_Omnitoken = inventoryManager.GetInventoryItemCount((uint)Currencies.OMICRON_OMNITOKEN, false, false, false),
                Orange_Crafters_Scrip = inventoryManager.GetInventoryItemCount((uint)Currencies.ORANGE_CRAFTERS_SCRIP, false, false, false),
                Orange_Gatherers_Scrip = inventoryManager.GetInventoryItemCount((uint)Currencies.ORANGE_GATHERERS_SCRIP, false, false, false),
                Pelu_Pelplume = inventoryManager.GetInventoryItemCount((uint)Currencies.PELU_PELPLUME, false, false, false),
                Purple_Crafters_Scrip = inventoryManager.GetInventoryItemCount((uint)Currencies.PURPLE_CRAFTERS_SCRIP, false, false, false),
                Purple_Gatherers_Scrip = inventoryManager.GetInventoryItemCount((uint)Currencies.PURPLE_GATHERERS_SCRIP, false, false, false),
                Qitari_Compliment = inventoryManager.GetInventoryItemCount((uint)Currencies.QITARI_COMPLIMENT, false, false, false),
                Rainbowtide_Psashp = inventoryManager.GetInventoryItemCount((uint)Currencies.RAINBOWTIDE_PSASHP, false, false, false),
                Sack_of_Nuts = inventoryManager.GetInventoryItemCount((uint)Currencies.SACK_OF_NUTS, false, false, false),
                Seafarers_Cowrie = inventoryManager.GetInventoryItemCount((uint)Currencies.SEAFARERS_COWRIE, false, false, false),
                Serpent_Seal = inventoryManager.GetInventoryItemCount((uint)Currencies.SERPENT_SEAL, false, false, false),
                Skybuilders_Scrip = inventoryManager.GetInventoryItemCount((uint)Currencies.SKYBUILDERS_SCRIP, false, false, false),
                Steel_Amaljok = inventoryManager.GetInventoryItemCount((uint)Currencies.STEEL_AMALJOK, false, false, false),
                Storm_Seal = inventoryManager.GetInventoryItemCount((uint)Currencies.STORM_SEAL, false, false, false),
                Sylphic_Goldleaf = inventoryManager.GetInventoryItemCount((uint)Currencies.SYLPHIC_GOLDLEAF, false, false, false),
                Titan_Cobaltpiece = inventoryManager.GetInventoryItemCount((uint)Currencies.TITAN_COBALTPIECE, false, false, false),
                Trophy_Crystal = inventoryManager.GetInventoryItemCount((uint)Currencies.TROPHY_CRYSTAL, false, false, false),
                Vanu_Whitebone = inventoryManager.GetInventoryItemCount((uint)Currencies.VANU_WHITEBONE, false, false, false),
                Venture = inventoryManager.GetInventoryItemCount((uint)Currencies.VENTURE, false, false, false),
                Water_Cluster = inventoryManager.GetInventoryItemCount((uint)Currencies.WATER_CLUSTER, false, false, false),
                Water_Crystal = inventoryManager.GetInventoryItemCount((uint)Currencies.WATER_CRYSTAL, false, false, false),
                Water_Shard = inventoryManager.GetInventoryItemCount((uint)Currencies.WATER_SHARD, false, false, false),
                White_Crafters_Scrip = inventoryManager.GetInventoryItemCount((uint)Currencies.WHITE_CRAFTERS_SCRIP, false, false, false),
                White_Gatherers_Scrip = inventoryManager.GetInventoryItemCount((uint)Currencies.WHITE_GATHERERS_SCRIP, false, false, false),
                Wind_Cluster = inventoryManager.GetInventoryItemCount((uint)Currencies.WIND_CLUSTER, false, false, false),
                Wind_Crystal = inventoryManager.GetInventoryItemCount((uint)Currencies.WIND_CRYSTAL, false, false, false),
                Wind_Shard = inventoryManager.GetInventoryItemCount((uint)Currencies.WIND_SHARD, false, false, false),
                Wolf_Mark = inventoryManager.GetInventoryItemCount((uint)Currencies.WOLF_MARK, false, false, false),
                Yellow_Crafters_Scrip = inventoryManager.GetInventoryItemCount((uint)Currencies.YELLOW_CRAFTERS_SCRIP, false, false, false),
                Yellow_Gatherers_Scrip = inventoryManager.GetInventoryItemCount((uint)Currencies.YELLOW_GATHERERS_SCRIP, false, false, false),
                Yo_Kai_Legendary_Jibanyan_Medal = inventoryManager.GetInventoryItemCount((uint)Currencies.YO_KAI_LEGENDARY_JIBANYAN_MEDAL, false, false, false),
                Yo_Kai_Legendary_Komasan_Medal = inventoryManager.GetInventoryItemCount((uint)Currencies.YO_KAI_LEGENDARY_KOMASAN_MEDAL, false, false, false),
                Yo_Kai_Legendary_Whisper_Medal = inventoryManager.GetInventoryItemCount((uint)Currencies.YO_KAI_LEGENDARY_WHISPER_MEDAL, false, false, false),
                Yo_Kai_Legendary_Blizzaria_Medal = inventoryManager.GetInventoryItemCount((uint)Currencies.YO_KAI_LEGENDARY_BLIZZARIA_MEDAL, false, false, false),
                Yo_Kai_Legendary_Kyubi_Medal = inventoryManager.GetInventoryItemCount((uint)Currencies.YO_KAI_LEGENDARY_KYUBI_MEDAL, false, false, false),
                Yo_Kai_Legendary_Komajiro_Medal = inventoryManager.GetInventoryItemCount((uint)Currencies.YO_KAI_LEGENDARY_KOMAJIRO_MEDAL, false, false, false),
                Yo_Kai_Legendary_Manjimutt_Medal = inventoryManager.GetInventoryItemCount((uint)Currencies.YO_KAI_LEGENDARY_MANJIMUTT_MEDAL, false, false, false),
                Yo_Kai_Legendary_Noko_Medal = inventoryManager.GetInventoryItemCount((uint)Currencies.YO_KAI_LEGENDARY_NOKO_MEDAL, false, false, false),
                Yo_Kai_Legendary_Venoct_Medal = inventoryManager.GetInventoryItemCount((uint)Currencies.YO_KAI_LEGENDARY_VENOCT_MEDAL, false, false, false),
                Yo_Kai_Legendary_Shogunyan_Medal = inventoryManager.GetInventoryItemCount((uint)Currencies.YO_KAI_LEGENDARY_SHOGUNYAN_MEDAL, false, false, false),
                Yo_Kai_Legendary_Hovernyan_Medal = inventoryManager.GetInventoryItemCount((uint)Currencies.YO_KAI_LEGENDARY_HOVERNYAN_MEDAL, false, false, false),
                Yo_Kai_Legendary_Robonyan_f_type_Medal = inventoryManager.GetInventoryItemCount((uint)Currencies.YO_KAI_LEGENDARY_ROBONYAN_F_TYPE_MEDAL, false, false, false),
                Yo_Kai_Legendary_Usapyon_Medal = inventoryManager.GetInventoryItemCount((uint)Currencies.YO_KAI_LEGENDARY_USAPYON_MEDAL, false, false, false),
                Yo_Kai_Legendary_Zazel_Medal = inventoryManager.GetInventoryItemCount((uint)Currencies.YO_KAI_LEGENDARY_ZAZEL_MEDAL, false, false, false),
                Yo_Kai_Legendary_lord_ananta_Medal = inventoryManager.GetInventoryItemCount((uint)Currencies.YO_KAI_LEGENDARY_LORD_ANANTA_MEDAL, false, false, false),
                Yo_Kai_Legendary_Lord_enma_Medal = inventoryManager.GetInventoryItemCount((uint)Currencies.YO_KAI_LEGENDARY_LORD_ENMA_MEDAL, false, false, false),
                Yo_Kai_Legendary_Damona_Medal = inventoryManager.GetInventoryItemCount((uint)Currencies.YO_KAI_LEGENDARY_DAMONA_MEDAL, false, false, false),
                Yo_Kai_Medal = inventoryManager.GetInventoryItemCount((uint)Currencies.YO_KAI_MEDAL, false, false, false),
                Weekly_Acquired_Tomestone = inventoryManager.GetWeeklyAcquiredTomestoneCount(),
                Weekly_Limit_Tomestone = InventoryManager.GetLimitedTomestoneWeeklyLimit(),
            };
        }

        private void GetPlayerCompletedQuests()
        {
            foreach (int id in _questIds)
            {
                Log.Debug($"Checking quest {id}");
                /*if(localPlayer.HasQuest(id) && localPlayer.IsQuestCompleted(id))
                    Plugin.Log.Debug($"{id} is completed");*/

                if (_localPlayer.HasQuest(id)/* && _localPlayer.IsQuestCompleted(id)*/)
                {
#if DEBUG
                    Log.Debug($"Has quest {id}");
#endif
                    continue;
                }

#if DEBUG
                Log.Debug($"Quest not in store or not completed, checking if quest {id} is completed");
#endif
                bool complete = Utils.IsQuestCompleted(id);
                if (!complete)
                {
#if DEBUG
                    Log.Debug($"Quest {id} not completed");
#endif
                    continue;
                }
#if DEBUG
                Log.Debug($"Quest {id} completed, adding");
#endif
                    _localPlayer.Quests.Add(id);
            }
        }

        private unsafe void GetPlayerBeastReputations()
        {
            Log.Debug("GetPlayerBeastReputations entered");
            int btCount = _globalCache.BeastTribesStorage.Count();
            if (_localPlayer.BeastReputations.Count > btCount) _localPlayer.BeastReputations.Clear();

            ref readonly QuestManager qm = ref *QuestManager.Instance();
            //QuestManager qm = new QuestManager();
            for (uint i = 1; i <= btCount; i++)
            {
                BeastTribeRank? btr = _localPlayer.GetBeastReputation(i);
                if (btr != null && btr.Rank ==
                    _globalCache.BeastTribesStorage.GetBeastTribe(ClientLanguage.English, i)?.MaxRank)
                {
                    continue;
                }

                //BeastReputationWork t = *QuestManager.Instance()->GetBeastReputationById((ushort)i);
                BeastReputationWork t = *qm.GetBeastReputationById((ushort)i);
                //BeastReputationWork t = *qm.GetBeastReputationById(1);
                ushort val = t.Value;
                //byte rank = t.Rank;
                byte rank = (byte)(t.Rank & 0x7F);
#if DEBUG
                //Log.Debug($"GetPlayerBeastReputations: id: {i}, val: {val}, rank: {rank}, t.rank: {t.Rank}");
#endif
                BeastTribeRank? b = _localPlayer.BeastReputations.Find(br => br.Id == i);
                if (b == null)
                {
                    _localPlayer.BeastReputations.Add(new BeastTribeRank
                    {
                        Id = i,
                        Value = val,
                        Rank = rank
                    });
                }
                else
                {
                    if (rank != 0 && (val == 0 && b.Value != 0)) continue;
                    b.Value = val;
                    b.Rank = rank;
                }
            }
        }//Todo: Replace this with the playerstate function once rep has been merge to CS

        private unsafe void GetPlayerEquippedGear()
        {
            //var inv = InventoryManager.Instance()->GetInventoryContainer(InventoryType.EquippedItems);
            ref readonly InventoryManager inventoryManager = ref *InventoryManager.Instance();
            InventoryContainer inv = *inventoryManager.GetInventoryContainer(InventoryType.EquippedItems);
            List<Gear> gearItems = [];
            //for (var i = 0; i < inv->Size; i++)
            for (int i = 0; i < inv.Size; i++)
            {
                //var ii = inv->Items[i];
                InventoryItem ii = inv.Items[i];
                InventoryItem.ItemFlags flags = ii.Flags;
                Gear currGear = new()
                {
                    ItemId = ii.ItemId,
                    HQ = flags.HasFlag(InventoryItem.ItemFlags.HighQuality),
                    CompanyCrestApplied = flags.HasFlag(InventoryItem.ItemFlags.CompanyCrestApplied),
                    Slot = ii.Slot,
                    Spiritbond = ii.Spiritbond,
                    Condition = ii.Condition,
                    CrafterContentID = ii.CrafterContentId,
                    Materia = ii.Materia.GetPinnableReference(),
                    MateriaGrade = ii.MateriaGrades.GetPinnableReference(),
                    Stain = ii.Stains[0],
                    Stain2 = ii.Stains[1],
                    GlamourID = ii.GlamourId,
                };
                gearItems.Add(currGear);
            }
            _localPlayer.Gear = gearItems;
        }

        private unsafe void GetPlayerInventory()
        {
            InventoryType[] invs =
            [
                InventoryType.Inventory1,
                InventoryType.Inventory2,
                InventoryType.Inventory3,
                InventoryType.Inventory4,
                InventoryType.KeyItems,
                InventoryType.Crystals
            ];
            List<Inventory> items = [];
            ref InventoryManager inventoryManager = ref *InventoryManager.Instance();
            foreach (InventoryType kind in invs)
            {
                //var inv = InventoryManager.Instance()->GetInventoryContainer(kind)
                //ref readonly InventoryManager inventoryManager = ref *InventoryManager.Instance();
                InventoryContainer inv = *inventoryManager.GetInventoryContainer(kind);
                //for (var i = 0; i < inv->Size; i++)
                for (int i = 0; i < inv.Size; i++)
                {
                    //var ii = inv->Items[i];
                    InventoryItem ii = inv.Items[i];
                    InventoryItem.ItemFlags flags = ii.Flags;
                    Inventory currInv = new()
                    {
                        ItemId = ii.ItemId,
                        HQ = flags.HasFlag(InventoryItem.ItemFlags.HighQuality),
                        Quantity = (uint)ii.Quantity,
                    };
                    //Plugin.Log.Debug($"{currInv.ItemId}");
                    items.Add(currInv);
                }
            }
            _localPlayer.Inventory = items;

            _localPlayer.Currencies = GetPlayerCurrencies();
        }

        private unsafe void GetPlayerSaddleInventory()
        {
            if (
                Condition[ConditionFlag.BoundByDuty] ||
                Condition[ConditionFlag.BoundByDuty56] ||
                Condition[ConditionFlag.BoundByDuty95]
            )
            {
                return;
            }
            InventoryType[] invs =
            [
                InventoryType.SaddleBag1,
                InventoryType.SaddleBag2,
                InventoryType.PremiumSaddleBag1,
                InventoryType.PremiumSaddleBag2
            ];
            List<Inventory> items = [];
            ref readonly InventoryManager inventoryManager = ref *InventoryManager.Instance();
            foreach (InventoryType kind in invs)
            {
                //var inv = InventoryManager.Instance()->GetInventoryContainer(kind)
                //ref readonly InventoryManager inventoryManager = ref *InventoryManager.Instance();
                InventoryContainer inv = *inventoryManager.GetInventoryContainer(kind);
                //for (var i = 0; i < inv->Size; i++)
                for (int i = 0; i < inv.Size; i++)
                {
                    //var ii = inv->Items[i];
                    InventoryItem ii = inv.Items[i];
                    InventoryItem.ItemFlags flags = ii.Flags;
                    Inventory currInv = new()
                    {
                        ItemId = ii.ItemId,
                        HQ = flags.HasFlag(InventoryItem.ItemFlags.HighQuality),
                        Quantity = (uint)ii.Quantity,
                    };
                    //Plugin.Log.Debug($"{currInv.ItemId}");
                    items.Add(currInv);
                }
            }
            _localPlayer.Saddle = items;
        }

        private unsafe void GetPlayerArmoryInventory()
        {
            InventoryType[] invs =
            [
                InventoryType.ArmoryMainHand,
                InventoryType.ArmoryHead,
                InventoryType.ArmoryBody,
                InventoryType.ArmoryHands,
                InventoryType.ArmoryLegs,
                InventoryType.ArmoryFeets,
                InventoryType.ArmoryOffHand,
                InventoryType.ArmoryEar,
                InventoryType.ArmoryNeck,
                InventoryType.ArmoryWrist,
                InventoryType.ArmoryRings,
                InventoryType.ArmorySoulCrystal
            ];
            List<Gear> mainHand = [];
            List<Gear> head = [];
            List<Gear> body = [];
            List<Gear> hands = [];
            List<Gear> legs = [];
            List<Gear> feets = [];
            List<Gear> oh = [];
            List<Gear> ear = [];
            List<Gear> neck = [];
            List<Gear> wrist = [];
            List<Gear> rings = [];
            List<Gear> soulCrystal = [];
            ref readonly InventoryManager inventoryManager = ref *InventoryManager.Instance();
            foreach (InventoryType kind in invs)
            {
                InventoryContainer inv = *inventoryManager.GetInventoryContainer(kind);
                for (int i = 0; i < inv.Size; i++)
                {
                    InventoryItem ii = inv.Items[i];
                    InventoryItem.ItemFlags flags = ii.Flags;
                    Gear currGear = new()
                    {
                        ItemId = ii.ItemId,
                        HQ = flags.HasFlag(InventoryItem.ItemFlags.HighQuality),
                        CompanyCrestApplied = flags.HasFlag(InventoryItem.ItemFlags.CompanyCrestApplied),
                        Slot = ii.Slot,
                        Spiritbond = ii.Spiritbond,
                        Condition = ii.Condition,
                        CrafterContentID = ii.CrafterContentId,
                        Materia = ii.Materia.GetPinnableReference(),
                        MateriaGrade = ii.MateriaGrades.GetPinnableReference(),
                        Stain = ii.Stains.GetPinnableReference(),
                        GlamourID = ii.GlamourId,
                    };

                    switch(kind)
                    {
                        case InventoryType.ArmoryMainHand: mainHand.Add(currGear);break;
                        case InventoryType.ArmoryHead: head.Add(currGear); break;
                        case InventoryType.ArmoryBody: body.Add(currGear); break;
                        case InventoryType.ArmoryHands: hands.Add(currGear); break;
                        case InventoryType.ArmoryLegs: legs.Add(currGear); break;
                        case InventoryType.ArmoryFeets: feets.Add(currGear); break;
                        case InventoryType.ArmoryOffHand: oh.Add(currGear); break;
                        case InventoryType.ArmoryEar: ear.Add(currGear); break;
                        case InventoryType.ArmoryNeck: neck.Add(currGear); break;
                        case InventoryType.ArmoryWrist: wrist.Add(currGear); break;
                        case InventoryType.ArmoryRings: rings.Add(currGear); break;
                        case InventoryType.ArmorySoulCrystal: soulCrystal.Add(currGear);break;
                    }
                }
            }

            _localPlayer.ArmoryInventory = new ArmoryGear
            {
                MainHand = mainHand,
                Head = head,
                Body = body,
                Hands = hands,
                Legs = legs,
                Feets = feets,
                OffHand = oh,
                Ear = ear,
                Neck = neck,
                Wrist = wrist,
                Rings = rings,
                SoulCrystal = soulCrystal,
            };
        }
        private void GetPlayerGlamourInventory()
        {
            
        }
        private void GetPlayerArmoireInventory()
        {
            
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct InfoProxyLetterCount
        {
            [FieldOffset(0x26)] public byte NumLetters;
        }
        private unsafe void GetPlayerMail()
        {
            if (
                Condition[ConditionFlag.BoundByDuty] ||
                Condition[ConditionFlag.BoundByDuty56] ||
                Condition[ConditionFlag.BoundByDuty95]
            )
            {
                return;
            }

            InfoProxyLetterCount* ipl = (InfoProxyLetterCount*)InfoModule.Instance()->GetInfoProxyById(InfoProxyId.Letter);
            _localPlayer.UnreadLetters = ipl->NumLetters;
        }

        private unsafe void GetPlayerRetainer()
        {
            if (
                Condition[ConditionFlag.BoundByDuty] ||
                Condition[ConditionFlag.BoundByDuty56] ||
                Condition[ConditionFlag.BoundByDuty95]
            )
            {
                return;
            }

            ref readonly RetainerManager retainerManager = ref *RetainerManager.Instance();
            byte retainersCount = retainerManager.GetRetainerCount();
            if (retainersCount == 0) return;
            //Log.Debug($"Retainers count {retainersCount}");
            for (uint i = 0; i < 10; i++)
            {
                RetainerManager.Retainer* currentRetainer = retainerManager.GetRetainerBySortedIndex(i);
                //if (!current_retainer->Available) continue;
                ulong retainerId = currentRetainer->RetainerId;
                string name = currentRetainer->NameString;

                //Log.Debug($"current_retainer name: {name} id: {retainerId}");
                if (name == "RETAINER") continue;

                Retainer? r = _localPlayer.Retainers.Find(r => r.Id == retainerId);
                r ??= new Retainer
                {
                    Id = currentRetainer->RetainerId
                };

                r.Available = currentRetainer->Available;
                r.Name = name;
                r.ClassJob = currentRetainer->ClassJob;
                r.Level = currentRetainer->Level;
                r.Gils = currentRetainer->Gil;
                r.Town = currentRetainer->Town;
                r.MarketItemCount = currentRetainer->MarketItemCount;
                r.MarketExpire = currentRetainer->MarketExpire;
                r.VentureID = currentRetainer->VentureId;
                r.LastUpdate = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                r.DisplayOrder = retainerManager.DisplayOrder.GetPinnableReference();

                if (r.Id == retainerManager.LastSelectedRetainerId)
                {
                    List<Inventory> inventory = GetPlayerRetainerInventory();
                    if (inventory.Count > 0)
                    {
                        r.Inventory = inventory;
                    }
                    r.Gear = GetPlayerRetainerEquippedGear();
                    r.MarketInventory = GetPlayerRetainerMarketInventory();
                }

                if (_localPlayer.Retainers.Find(ret => ret.Id == retainerId) == null)
                {
                    _localPlayer.Retainers.Add(r);
                }
            }
        }
        private unsafe List<Inventory> GetPlayerRetainerInventory()
        {
            InventoryType[] invs =
            [
                InventoryType.RetainerPage1,
                InventoryType.RetainerPage2,
                InventoryType.RetainerPage3,
                InventoryType.RetainerPage4,
                InventoryType.RetainerPage5,
                InventoryType.RetainerPage6,
                InventoryType.RetainerPage7,
                InventoryType.RetainerCrystals
            ];
            List<Inventory> items = [];
            ref readonly InventoryManager inventoryManager = ref *InventoryManager.Instance();
            foreach (InventoryType kind in invs)
            {
                //var inv = InventoryManager.Instance()->GetInventoryContainer(kind)
                //ref readonly InventoryManager inventoryManager = ref *InventoryManager.Instance();
                InventoryContainer inv = *inventoryManager.GetInventoryContainer(kind);
                //for (var i = 0; i < inv->Size; i++)
                for (int i = 0; i < inv.Size; i++)
                {
                    //var ii = inv->Items[i];
                    InventoryItem ii = inv.Items[i];
                    InventoryItem.ItemFlags flags = ii.Flags;
                    Inventory currInv = new()
                    {
                        ItemId = ii.ItemId,
                        HQ = flags.HasFlag(InventoryItem.ItemFlags.HighQuality),
                        Quantity = (uint)ii.Quantity,
                    };
                    //Plugin.Log.Debug($"{currInv.ItemId}");
                    items.Add(currInv);
                }
            }
            return items;

            /*
             * 
                RetainerMarket = 12002,
            */
        }
        
        private unsafe List<Inventory> GetPlayerRetainerMarketInventory()
        {
            //var inv = InventoryManager.Instance()->GetInventoryContainer(InventoryType.EquippedItems);
            ref readonly InventoryManager inventoryManager = ref *InventoryManager.Instance();
            InventoryContainer inv = *inventoryManager.GetInventoryContainer(InventoryType.RetainerMarket);
            List<Inventory> marketItems = [];
            //for (var i = 0; i < inv->Size; i++)
            for (int i = 0; i < inv.Size; i++)
            {
                //var ii = inv->Items[i];
                InventoryItem ii = inv.Items[i];
                InventoryItem.ItemFlags flags = ii.Flags;
                Inventory currInv = new()
                {
                    ItemId = ii.ItemId,
                    HQ = flags.HasFlag(InventoryItem.ItemFlags.HighQuality),
                    Quantity = (uint)ii.Quantity,
                };
                //Plugin.Log.Debug($"{currInv.ItemId}");
                marketItems.Add(currInv);
            }
            return marketItems;
        }
        
        private unsafe List<Gear> GetPlayerRetainerEquippedGear()
        {
            //var inv = InventoryManager.Instance()->GetInventoryContainer(InventoryType.EquippedItems);
            ref readonly InventoryManager inventoryManager = ref *InventoryManager.Instance();
            InventoryContainer inv = *inventoryManager.GetInventoryContainer(InventoryType.RetainerEquippedItems);
            List<Gear> gearItems = [];
            //for (var i = 0; i < inv->Size; i++)
            for (int i = 0; i < inv.Size; i++)
            {
                //var ii = inv->Items[i];
                InventoryItem ii = inv.Items[i];
                InventoryItem.ItemFlags flags = ii.Flags;
                Gear currGear = new()
                {
                    ItemId = ii.ItemId,
                    HQ = flags.HasFlag(InventoryItem.ItemFlags.HighQuality),
                    CompanyCrestApplied = flags.HasFlag(InventoryItem.ItemFlags.CompanyCrestApplied),
                    Slot = ii.Slot,
                    Spiritbond = ii.Spiritbond,
                    Condition = ii.Condition,
                    CrafterContentID = ii.CrafterContentId,
                    Materia = ii.Materia.GetPinnableReference(),
                    MateriaGrade = ii.MateriaGrades.GetPinnableReference(),
                    Stain = ii.Stains[0],
                    Stain2 = ii.Stains[1],
                    GlamourID = ii.GlamourId,
                };
                gearItems.Add(currGear);
            }
            return gearItems;
        }

        private unsafe void GetHousing()
        {
            if (
                Condition[ConditionFlag.BoundByDuty] ||
                Condition[ConditionFlag.BoundByDuty56] ||
                Condition[ConditionFlag.BoundByDuty95]
            )
            {
                return;
            }

            if (_localPlayer.HomeWorld != _localPlayer.CurrentWorld)
            {
                return;
            }

            ref readonly HousingManager housingManager = ref *HousingManager.Instance();

            if (!housingManager.IsInside())
            {
                return;
            }

            if (!housingManager.HasHousePermissions())
            {
                return;
            }

            sbyte ward = housingManager.GetCurrentWard();

            // 1 for Main Division, 2 for Subdivision
            byte division = housingManager.GetCurrentDivision();

            // -128 for Apartments in Main Division, -127 for Apartments in Subdivision
            sbyte plot = housingManager.GetCurrentPlot();
            short room = housingManager.GetCurrentRoom();
            long id = housingManager.GetCurrentIndoorHouseId();
            /*HousingTerritoryType ct = housingManager.CurrentTerritory->GetTerritoryType();
            HousingTerritoryType ot = (HousingTerritoryType)0;
            if (housingManager.OutdoorTerritory != null)
            {
                ot = housingManager.OutdoorTerritory->GetTerritoryType();
            }
            HousingTerritoryType it = housingManager.IndoorTerritory->GetTerritoryType();*/
            int p = plot is -127 or -128 ? 0 : plot;

            ushort tt = ClientState.TerritoryType;
            uint mId = GameMain.Instance()->CurrentMapId;

            Housing? house = _localPlayer.Houses.Find(h => h.Id == id);
            if (house != null)
            {
                if (house.Room == 0 && room != 0)
                {
                    house.Room = room;
                }
            }
            else
            {
                _localPlayer.Houses.Add(new Housing()
                {
                    Id = id,
                    Ward = ward,
                    Division = division,
                    Plot = plot,
                    Room = room,
                    MapId = mId,
                    TerritoryId = tt
                });
            }

            //Log.Debug($"Add house: tt:{tt}, mId:{mId}, w:{ward + 1}, d:{division}, plot: {p}, room: {room}, id: {id}");
        }

        private void CleanLastLocalCharacter()
        {
            _localPlayer = new Character();
            _localPlayerFreeCompanyTest = null;
            OtherCharacters = [];
        }

        private void OnCharacterLogin()
        {
            Log.Info("Altoholic : OnCharacterLogin called");
            OtherCharacters = Database.Database.GetOthersCharacters(_db, _localPlayer.CharacterId);

            Character? p = GetCharacterFromGameOrDatabase();
            if (p is null)
            {
                return;
            }

            _localPlayer = p;
            //Log.Info($"Character id is : {localPlayer.Id}");

            //Log.Info("Altoholic : Found {0} others players", otherCharacters.Count);
            //Todo: start timer after /playtime command

            _autoSaveWatch.Reset();
            _autoSaveWatch.Start();
            if (Configuration.IsAutoSaveChatMessageEnabled)
            {
                Utils.ChatMessage("Starting altoholic autosave timer");
            }

            GetPlayerBeastReputations();
            GetCollectionFromState();
            GetDuties();
        }

        private void GetDuties()
        {
            foreach (Duty i in _globalCache.DutyStorage.GetAll())
            {
                if (!_localPlayer.IsDutyUnlocked(i.Id))
                {
                    if (Utils.IsDutyUnlocked(i.Content))
                    {
                        Log.Debug($"Duty {i.Id}:{i.EnglishName} unlocked");
                        _localPlayer.DutiesUnlocked.Add(i.Id);
                    }
                    else
                    {
                        Log.Debug($"Duty {i.Id}:{i.EnglishName} not unlocked");
                    }
                }

                if (_localPlayer.IsDutyCompleted(i.Id))
                {
                    continue;
                }

                if (Utils.IsDutyCompleted(i.Content))
                {
                    Log.Debug($"Duty {i.Id}:{i.EnglishName} completed");
                    _localPlayer.Duties.Add(i.Id);
                }
                else
                {
                    Log.Debug($"Duty {i.Id}:{i.EnglishName} not completed");
                }
            }
        }

        private void OnCharacterLogout(int type, int code)
        {
            Log.Debug("Altoholic : OnCharacterLogout called");
            GetPlayerCompletedQuests();
            UpdateCharacter();
            CleanLastLocalCharacter();

            ProgressWindow.IsOpen = false;
            ProgressWindow.Clear();
            CollectionWindow.IsOpen = false;
            CollectionWindow.Clear();
            RetainersWindow.IsOpen = false;
            RetainersWindow.Clear();
            InventoriesWindow.IsOpen = false;
            InventoriesWindow.Clear();
            JobsWindow.IsOpen = false;
            JobsWindow.Clear();
            ConfigWindow.IsOpen = false;
            CurrenciesWindow.IsOpen = false;
            CurrenciesWindow.Clear();
            DetailsWindow.IsOpen = false;
            DetailsWindow.Clear();
            CharactersWindow.IsOpen = false;
            MainWindow.IsOpen = false;
            MainWindow.Clear();
            _autoSaveWatch.Stop();
        }

        private void OnDutyCompleted(object? sender, ushort e)
        {
            Log.Debug($"OnDutyCompleted entered with e: {e}");
            Duty? d = _globalCache.DutyStorage.GetFromTerritory(e);
            if (d == null || d.Id == 0)
            {
                return;
            }

            if (_localPlayer.IsDutyCompleted(d.Id)) return;
            Log.Debug($"Duty {d.Id}:<{d.EnglishName}> not completed");
            _localPlayer.Duties.Add(d.Id);
        }

        private void OnGameInventoryItemEvent(GameInventoryEvent type, InventoryEventArgs data)
        {
            //Log.Debug($"Condition[ConditionFlag.LoggingOut]: {Condition[ConditionFlag.LoggingOut]}");
            if (Condition[ConditionFlag.LoggingOut]) return;
            Log.Debug($"OnGameInventoryItemEvent entered, item {type}: {data.Item.ItemId}");
            GameInventoryItem i = data.Item;
            if (i.ItemId == 0 || i.ItemId == 1) return;
            switch (i.ContainerType)
            {
                case GameInventoryType.Inventory1:
                case GameInventoryType.Inventory2:
                case GameInventoryType.Inventory3:
                case GameInventoryType.Inventory4:
                case GameInventoryType.KeyItems:
                case GameInventoryType.Crystals:
                    /*Inventory item = new()
                    {
                        ItemId = i.ItemId,
                        HQ = i.IsHq,
                        Quantity = i.Quantity,
                    };
                    _localPlayer.Inventory.Add(item);*/
                    GetPlayerInventory();
                    break;
                case GameInventoryType.EquippedItems:
                    GetPlayerEquippedGear();
                    break;
                case GameInventoryType.SaddleBag1:
                case GameInventoryType.SaddleBag2:
                case GameInventoryType.PremiumSaddleBag1:
                case GameInventoryType.PremiumSaddleBag2:
                    GetPlayerSaddleInventory();
                    break;
                case GameInventoryType.ArmoryMainHand:
                case GameInventoryType.ArmoryHead:
                case GameInventoryType.ArmoryBody:
                case GameInventoryType.ArmoryHands:
                case GameInventoryType.ArmoryLegs:
                case GameInventoryType.ArmoryFeets:
                case GameInventoryType.ArmoryOffHand:
                case GameInventoryType.ArmoryEar:
                case GameInventoryType.ArmoryNeck:
                case GameInventoryType.ArmoryWrist:
                case GameInventoryType.ArmoryRings:
                case GameInventoryType.ArmorySoulCrystal:
                    GetPlayerArmoryInventory();
                    break;
            }
            
            /*GetPlayerGlamourInventory();
            GetPlayerArmoireInventory();*/
        }

        public void ReloadConfig()
        {
            Configuration = PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
            Configuration.Initialize(PluginInterface);
        }

        private long PlaytimePacket(uint param1, long param2, uint param3)
        {
            long result = _playtimeHook.Original(param1, param2, param3);
            if (param1 != 11)
                return result;

            (string, string, string) player = GetLocalPlayerNameWorldRegion();
            if (player.Item1.Length == 0)
                return result;

            Log.Debug($"Extracted Player Name: {player.Item1}{(char)SeIconChar.CrossWorld}{player.Item2}");

            uint totalPlaytime = (uint)Marshal.ReadInt32((nint)param2 + 0x10);
            Log.Debug($"Value from address {totalPlaytime}");
            TimeSpan playtime = TimeSpan.FromMinutes(totalPlaytime);
            Log.Debug($"Value from timespan {playtime}");

            string[] names = player.Item1.Split(' ');
            if (names.Length == 0)
                return result;

            _localPlayer.PlayTime = totalPlaytime;

            ulong id = ClientState.LocalContentId;

            long newPlayTimeUpdate = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            _localPlayer.LastPlayTimeUpdate = newPlayTimeUpdate;
            Log.Debug($"Updating playtime with {player.Item1}, {player.Item2}, {totalPlaytime}, {newPlayTimeUpdate}.");
            Database.Database.UpdatePlaytime(_db, id, totalPlaytime, newPlayTimeUpdate);
            return result;
        }

        public (string, string, string) GetLocalPlayerNameWorldRegion()
        {
            IPlayerCharacter? local = ClientState.LocalPlayer;
            if (local == null || local.HomeWorld.ValueNullable == null)
                return (string.Empty, string.Empty, string.Empty);
            string homeworld = local.HomeWorld.Value.Name.ExtractText();

            return ($"{local.Name}", $"{homeworld}", $"{Utils.GetRegionFromWorld(homeworld)}");
        }

        public void UpdateCharacter()
        {
            if (_localPlayer.CharacterId == 0 || _localPlayer.FirstName.Length == 0) return;

            Log.Debug($"Updating characters with {_localPlayer.CharacterId} {_localPlayer.FirstName} {_localPlayer.LastName}{(char)SeIconChar.CrossWorld}{_localPlayer.HomeWorld}, {Utils.GetRegionFromWorld(_localPlayer.HomeWorld)}.");
            Blacklist? b = Database.Database.GetBlacklist(_db, _localPlayer.CharacterId);
            if (b != null) return;
            Character? charExist = Database.Database.GetCharacter(_db, _localPlayer.CharacterId);
            int returnedRows = charExist == null ? Database.Database.AddCharacter(_db, _localPlayer) : Database.Database.UpdateCharacter(_db, _localPlayer);
            if (returnedRows == 1)
            {
                if (Configuration.IsAutoSaveChatMessageEnabled)
                {
                    Utils.ChatMessage("Character has been saved");
                }
            }
        }

        public void PrintBlacklistedMessage()
        {
            SeStringBuilder builder = new();
            builder.Append("This character is blacklisted, use ");
            builder.PushColorRgba(KnownColor.LimeGreen.Vector());
            builder.Append("/altoholic unbl");
            builder.PopColor();
            builder.Append(" to remove it from the list");

            XivChatEntry chatEntry = new() { Message = builder.ToSeString().ToDalamudString(), Type = XivChatType.Echo };
            Log.Debug("Character is blacklisted");
            ChatGui.Print(chatEntry);
        }

        public Character? GetCharacterFromGameOrDatabase()
        {
            if (BlacklistedCharacters.Exists(b => b.CharacterId == ClientState.LocalContentId)) return null;
            (string, string, string) player = GetLocalPlayerNameWorldRegion();
            if (string.IsNullOrEmpty(player.Item1) || string.IsNullOrEmpty(player.Item2) || string.IsNullOrEmpty(player.Item3)) return null;

            //Plugin.Log.Debug($"Altoholic : Character names => 0: {player.Item1}{(char)SeIconChar.CrossWorld}{player.Item2});
            string[] names = player.Item1.Split(' ');
            if (names.Length != 2)
            {
                return null;
            }

            //Plugin.Log.Debug("Altoholic : Character names : 0 : {0}, 1: {1}, 2: {2}, 3: {3}", names[0], names[1], player.Item2, player.Item3);
            Character? chara = Database.Database.GetCharacter(_db, ClientState.LocalContentId);
            if (chara != null)
            {
                //Plugin.Log.Debug($"GetCharacterFromDB : id = {chara.Id}, FirstName = {chara.FirstName}, LastName = {chara.LastName}, HomeWorld = {chara.HomeWorld}, DataCenter = {chara.Datacenter}, LastJob = {chara.LastJob}, LastJobLevel = {chara.LastJobLevel}, FCTag = {chara.FCTag}, FreeCompany = {chara.FreeCompany}, LastOnline = {chara.LastOnline}, PlayTime = {chara.PlayTime}, LastPlayTimeUpdate = {chara.LastPlayTimeUpdate}");
                return chara;
            }

            return new Character
            {
                CharacterId = ClientState.LocalContentId,
                FirstName = names[0],
                LastName = names[1],
                HomeWorld = player.Item2,
                CurrentWorld = player.Item2,
                Datacenter = Utils.GetDatacenterFromWorld(player.Item2),
                CurrentDatacenter = Utils.GetDatacenterFromWorld(player.Item2),
                Region = player.Item3,
                CurrentRegion = player.Item3,
                IsSprout = _localPlayer.IsSprout,
                IsBattleMentor = _localPlayer.IsBattleMentor,
                IsTradeMentor = _localPlayer.IsTradeMentor,
                IsReturner = _localPlayer.IsReturner,
                LastJob = _localPlayer.LastJob,
                LastJobLevel = _localPlayer.LastJobLevel,
                FCTag = _localPlayer.FCTag,
                FreeCompany = _localPlayer.FreeCompany,
                LastOnline = _localPlayer.LastOnline,
                PlayTime = _localPlayer.PlayTime,
                LastPlayTimeUpdate = _localPlayer.LastPlayTimeUpdate,
                HasPremiumSaddlebag = _localPlayer.HasPremiumSaddlebag,
                PlayerCommendations = _localPlayer.PlayerCommendations,
                Attributes = _localPlayer.Attributes,
                Currencies = _localPlayer.Currencies,
                Jobs = _localPlayer.Jobs,
                Profile = _localPlayer.Profile,
                Quests = _localPlayer.Quests,
                Inventory = _localPlayer.Inventory,
                Saddle = _localPlayer.Saddle,
                Gear = _localPlayer.Gear,
                Retainers = _localPlayer.Retainers,
                Minions = _localPlayer.Minions,
                Mounts = _localPlayer.Mounts,
                TripleTriadCards = _localPlayer.TripleTriadCards,
                Emotes = _localPlayer.Emotes,
                Bardings = _localPlayer.Bardings,
                FramerKits = _localPlayer.FramerKits,
                OrchestrionRolls = _localPlayer.OrchestrionRolls,
                Ornaments = _localPlayer.Ornaments,
                Glasses = _localPlayer.Glasses,
                CurrenciesHistory = _localPlayer.CurrenciesHistory,
                BeastReputations = _localPlayer.BeastReputations,
            };

        }
        public void ChangeLanguage(ClientLanguage newLanguage)
        {
            string isoLang = newLanguage switch
            {
                ClientLanguage.German => "de",
                ClientLanguage.English => "en",
                ClientLanguage.French => "fr",
                ClientLanguage.Japanese => "ja",
                _ => "en",
            };

            _localization.SetupWithLangCode(isoLang);
        }
    }
}

