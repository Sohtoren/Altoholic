using System;
using System.Collections.Generic;
using System.Linq;
using Altoholic.Models;
using Dalamud.Plugin.Services;
using LiteDB;

namespace Altoholic.Database
{
    public class Database
    {
        public static Character? GetCharacter(IPluginLog log, LiteDatabase db, ulong id)
        {
            Plugin.Log.Debug($"Database/GetCharacter entered db = {db}, Log = {log}, id = {id}");
            try
            {
                ILiteCollection<Character>? col = db.GetCollection<Character>();
                Character? character = col?.FindOne(cf => cf.Id == id);
                if (character != null)
                    character.CurrenciesHistory = db.GetCollection<CurrenciesHistory>().Find(c => c.CharacterId == id).ToList();
                return character;
            }
            catch (Exception ex)
            {
                // Todo: Add error handling to not crash game if db is opened in another program
                Console.WriteLine(ex.ToString());
                log.Error(ex.ToString());
                return null;
            }
        }

        public static List<Character> GetOthersCharacters(IPluginLog log, LiteDatabase db, ulong id)
        {
            try
            {
                ILiteCollection<Character>? col = db.GetCollection<Character>();
                if (col == null)
                {
                    return [];
                }

                List<Character> characters = col.Find(cf => cf.Id != id).ToList();
                return characters;

            }
            catch (Exception ex)
            {
                // Todo: Add error handling to not crash game if db is opened in another program
                Console.WriteLine(ex.ToString());
                log.Error(ex.ToString());
                return [];
            }
        }

        public static List<Character>? DeleteCharacter(IPluginLog log, LiteDatabase db, ulong id)
        {
            try
            {
                ILiteCollection<Character>? col = db.GetCollection<Character>();
                col?.Delete(id);
                ILiteCollection<Blacklist>? col2 = db.GetCollection<Blacklist>();
                col2?.Delete(id);

                return GetOthersCharacters(log, db, id);
            }
            catch (Exception ex)
            {
                // Todo: Add error handling to not crash game if db is opened in another program
                Console.WriteLine(ex.ToString());
                log.Error(ex.ToString());
                return null;
            }
        }

        public static void UpdateCharacter(IPluginLog log, LiteDatabase db, Character character)
        {
            Plugin.Log.Debug($"Entering UpdateCharacter with character : id = {character.Id}, FirstName = {character.FirstName}, LastName = {character.LastName}, HomeWorld = {character.HomeWorld}, DataCenter = {character.Datacenter}, LastJob = {character.LastJob}, LastJobLevel = {character.LastJobLevel}, FCTag = {character.FCTag}, FreeCompany = {character.FreeCompany}, LastOnline = {character.LastOnline}, PlayTime = {character.PlayTime}, LastPlayTimeUpdate = {character.LastPlayTimeUpdate}");
            if (character.Id == 0) return;

            try
            {
                ILiteCollection<Character>? col = db.GetCollection<Character>();
                if (col == null)
                {
                    return;
                }

                Character c;
                Character? charExist = col.FindOne(cf => cf.Id == character.Id);
                if (charExist != null)
                {
                    c = charExist;
                    c.FirstName = character.FirstName;
                    c.LastName = character.LastName;
                    c.HomeWorld = character.HomeWorld;
                    c.Datacenter = character.Datacenter;
                    c.Region = character.Region;
                    c.IsSprout = character.IsSprout;
                    c.IsBattleMentor = character.IsBattleMentor;
                    c.IsTradeMentor = character.IsTradeMentor;
                    c.IsReturner = character.IsReturner;
                    c.LastJob = character.LastJob;
                    c.LastJobLevel = character.LastJobLevel;
                    c.FCTag = character.FCTag;
                    c.FreeCompany = character.FreeCompany;
                    c.LastOnline = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                    if (character.PlayTime > 0)
                    {
                        c.PlayTime = character.PlayTime;
                        c.LastPlayTimeUpdate = character.LastPlayTimeUpdate;
                    }
                    c.HasPremiumSaddlebag = character.HasPremiumSaddlebag;
                    c.Attributes = character.Attributes;
                    c.Currencies = character.Currencies;
                    c.Jobs = character.Jobs;
                    c.Profile = character.Profile;
                    c.Quests = character.Quests;
                    c.Inventory = character.Inventory;
                    c.ArmoryInventory = character.ArmoryInventory;
                    c.Saddle = character.Saddle;
                    c.Gear = character.Gear;
                    c.Retainers = character.Retainers;
                    c.Minions = character.Minions;
                    c.Mounts = character.Mounts;
                }
                else
                {
                    c = new Character()
                    {
                        Id = character.Id,
                        FirstName = character.FirstName,
                        LastName = character.LastName,
                        HomeWorld = character.HomeWorld,
                        Datacenter = character.Datacenter,
                        Region = character.Region,
                        IsSprout = character.IsSprout,
                        IsBattleMentor = character.IsBattleMentor,
                        IsTradeMentor = character.IsTradeMentor,
                        IsReturner = character.IsReturner,
                        LastJob = character.LastJob,
                        LastJobLevel = character.LastJobLevel,
                        FCTag = character.FCTag,
                        FreeCompany = character.FreeCompany,
                        LastOnline = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                        HasPremiumSaddlebag = character.HasPremiumSaddlebag,
                        Attributes = character.Attributes,
                        Currencies = character.Currencies,
                        Jobs = character.Jobs,
                        Profile = character.Profile,
                        Quests = character.Quests,
                        Inventory = character.Inventory,
                        ArmoryInventory = character.ArmoryInventory,
                        Saddle = character.Saddle,
                        Gear = character.Gear,
                        Retainers = character.Retainers,
                        Minions = character.Minions,
                        Mounts = character.Mounts,
                    };
                    if (character.PlayTime > 0)
                    {
                        c.PlayTime = character.PlayTime;
                        c.LastPlayTimeUpdate = character.LastPlayTimeUpdate;
                    }
                }

                Plugin.Log.Debug($"Updating character with c : id = {c.Id}, FirstName = {c.FirstName}, LastName = {c.LastName}, HomeWorld = {c.HomeWorld}, DataCenter = {c.Datacenter}, LastJob = {c.LastJob}, LastJobLevel = {c.LastJobLevel}, FCTag = {c.FCTag}, FreeCompany = {c.FreeCompany}, LastOnline = {c.LastOnline}, PlayTime = {c.PlayTime}, LastPlayTimeUpdate = {c.LastPlayTimeUpdate}, Quests = {c.Quests.Count}, Inventory = {c.Inventory.Count}, Gear {c.Gear.Count}, Retainers = {c.Retainers.Count}");
                col.Upsert(c);

                ILiteCollection<CurrenciesHistory>? chCol = db.GetCollection<CurrenciesHistory>();
                if (chCol == null)
                {
                    return;
                }
                chCol.Insert(new CurrenciesHistory()
                {
                    CharacterId = character.Id,
                    Datetime = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                    Currencies = character.Currencies
                });
            }
            catch (Exception ex)
            {
                // Todo: Add error handling to not crash game if db is opened in another program
                Console.WriteLine(ex.ToString());
                log.Error(ex.ToString());
            }
        }
        
        public static void UpdatePlaytime(IPluginLog log, LiteDatabase db, ulong id, uint playTime, long playTimeUpdate)
        {
            if (playTime == 0) return;
            Plugin.Log.Debug($"UpdatePlayTime {id} {playTime} {playTimeUpdate}");
            try
            {
                ILiteCollection<Character>? col = db.GetCollection<Character>();
                if (col == null)
                {
                    return;
                }

                Character c;
                Character? charExist = col.FindOne(ce => ce.Id == id);
                if (charExist != null)
                {
                    c = charExist;
                    c.PlayTime = playTime;
                    c.LastPlayTimeUpdate = playTimeUpdate;
                }
                else
                {
                    c = new Character()
                    {
                        Id = id,
                        PlayTime = playTime,
                        LastPlayTimeUpdate = playTimeUpdate,
                    };
                }

                col.Upsert(c);
            }
            catch (Exception ex)
            {
                // Todo: Add error handling to not crash game if db is opened in another program
                Console.WriteLine(ex.ToString());
                log.Error(ex.ToString());
            }
        }

        public static List<Character>? BlacklistCharacter(IPluginLog log, LiteDatabase db, ulong id)
        {
            Plugin.Log.Debug($"Database/BlacklistCharacter entered db = {db}, Log = {log}, id = {id}");
            try
            {
                ILiteCollection<Blacklist>? col = db.GetCollection<Blacklist>();
                if (col == null)
                {
                    return GetOthersCharacters(log, db, id);
                }

                Blacklist? character = col.FindOne(cf => cf.Id == id);
                if (character == null)
                {
                    Blacklist blacklist = new()
                    {
                        Id = id,
                    };
                    col.Insert(blacklist);
                }
                DeleteCharacter(log, db, id);
                return GetOthersCharacters(log, db, id);
            }
            catch (Exception ex)
            {
                // Todo: Add error handling to not crash game if db is opened in another program
                Console.WriteLine(ex.ToString());
                log.Error(ex.ToString());
                return null;
            }
        }
    }
}
