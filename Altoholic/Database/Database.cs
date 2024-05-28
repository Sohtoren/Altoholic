using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Altoholic.Models;
using Dalamud.Logging;
using Dalamud.Plugin.Services;
using LiteDB;

namespace Altoholic.Database
{
    public class Database
    {
        public static Character? GetCharacter(LiteDatabase db, IPluginLog pluginLog, ulong id)
        {
            pluginLog.Debug($"Database/GetCharacter entered db = {db}, pluginLog = {pluginLog}, id = {id}");
            try
            {
                var col = db.GetCollection<Character>();
                if (col != null)
                {
                    //var character = col.FindOne(cf => cf.FirstName == firstname && cf.LastName == lastname && cf.HomeWorld == homeworld);
                    var character = col.FindOne(cf => cf.Id == id);

                    /*if (character is not null)
                    {
                        character.Retainers = GetCharacterRetainers(db, pluginLog, character.Id);
                    }*/

                    return character;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                // Todo: Add error handling to not crash game if db is opened in another program
                Console.WriteLine(ex.ToString());
                pluginLog.Error(ex.ToString());
                return null;
            }
        }

        public static List<Character> GetOthersCharacters(LiteDatabase db, IPluginLog pluginLog, ulong id)
        {
            try
            {
                var col = db.GetCollection<Character>();
                if (col != null)
                {
                    //var characters = col.Find(cf => cf.FirstName != firstname && cf.LastName != lastname/*&& cf.HomeWorld != homeworld*/).ToList();
                    //var characters = col.Find(cf => cf.Uuid != uuid).ToList();
                    var characters = col.Find(cf => cf.Id != id).ToList();
                    /*foreach (var c in characters)
                    {
                        c.Retainers = GetCharacterRetainers(db, pluginLog, c.Id);
                    }*/

                    return characters;
                }
                else
                {
                    return [];
                }
            }
            catch (Exception ex)
            {
                // Todo: Add error handling to not crash game if db is opened in another program
                Console.WriteLine(ex.ToString());
                pluginLog.Error(ex.ToString());
                return [];
            }
        }

        public static List<Character>? DeleteCharacter(LiteDatabase db, IPluginLog pluginLog, ulong id)
        {
            try
            {
                var col = db.GetCollection<Character>();
                col?.Delete(id);
                var col2 = db.GetCollection<Blacklist>();
                col2?.Delete(id);

                return GetOthersCharacters(db, pluginLog, id);
            }
            catch (Exception ex)
            {
                // Todo: Add error handling to not crash game if db is opened in another program
                Console.WriteLine(ex.ToString());
                pluginLog.Error(ex.ToString());
                return null;
            }
}

        public static void UpdateCharacter(LiteDatabase db, IPluginLog pluginLog, Character character)
        {
            pluginLog.Debug($"Entering UpdateCharacter with character : id = {character.Id}, FirstName = {character.FirstName}, LastName = {character.LastName}, HomeWorld = {character.HomeWorld}, DataCenter = {character.Datacenter}, LastJob = {character.LastJob}, LastJobLevel = {character.LastJobLevel}, FCTag = {character.FCTag}, FreeCompany = {character.FreeCompany}, LastOnline = {character.LastOnline}, PlayTime = {character.PlayTime}, LastPlayTimeUpdate = {character.LastPlayTimeUpdate}");
            if (character.Id == 0) return;

            try
            {
                var col = db.GetCollection<Character>();
                if (col != null)
                {
                    var c = new Character();
                    //var char_exist = col.FindOne(cf => cf.FirstName == character.FirstName && cf.LastName == character.LastName && cf.HomeWorld == character.HomeWorld);
                    var char_exist = col.FindOne(cf => cf.Id == character.Id);
                    if (char_exist != null)
                    {
                        c = char_exist;
                        c.FirstName = character.FirstName;
                        c.LastName = character.LastName;
                        c.HomeWorld = character.HomeWorld;
                        c.Datacenter = character.Datacenter;
                        c.Region = character.Region;
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
                        c.Attributes = character.Attributes;
                        c.Currencies = character.Currencies;
                        c.Jobs = character.Jobs;
                        c.Quests = character.Quests;
                        c.Profile = character.Profile;
                        c.Retainers = character.Retainers;
                        c.Inventory = character.Inventory;
                        c.Gear = character.Gear;                        
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
                            LastJob = character.LastJob,
                            LastJobLevel = character.LastJobLevel,
                            FCTag = character.FCTag,
                            FreeCompany = character.FreeCompany,
                            LastOnline = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                            Attributes = character.Attributes,
                            Currencies = character.Currencies,
                            Jobs = character.Jobs,
                            Quests = character.Quests,
                            Profile = character.Profile,
                            Retainers = character.Retainers,
                            Inventory = character.Inventory,
                            Gear = character.Gear,
                        };
                        if (character.PlayTime > 0)
                        {
                            c.PlayTime = character.PlayTime;
                            c.LastPlayTimeUpdate = character.LastPlayTimeUpdate;
                        }
                    }

                    pluginLog.Debug($"Updating character with c : id = {c.Id}, FirstName = {c.FirstName}, LastName = {c.LastName}, HomeWorld = {c.HomeWorld}, DataCenter = {c.Datacenter}, LastJob = {c.LastJob}, LastJobLevel = {c.LastJobLevel}, FCTag = {c.FCTag}, FreeCompany = {c.FreeCompany}, LastOnline = {c.LastOnline}, PlayTime = {c.PlayTime}, LastPlayTimeUpdate = {c.LastPlayTimeUpdate}, Quests = {c.Quests.Count}, Inventory = {c.Inventory}, Gear {c.Gear.Count}, Retainers = {c.Retainers.Count}");
                    col.Upsert(c);
                }
            }
            catch (Exception ex)
            {
                // Todo: Add error handling to not crash game if db is opened in another program
                Console.WriteLine(ex.ToString());
                pluginLog.Error(ex.ToString());
            }
        }
        
        public static void UpdatePlaytime(LiteDatabase db, IPluginLog pluginLog, ulong id, uint PlayTime, long PlayTimeUpdate)
        {
            if (PlayTime == 0) return;
            pluginLog.Debug($"UpdatePlayTime {id} {PlayTime} {PlayTimeUpdate}");
            try
            {
                var col = db.GetCollection<Character>();
                if (col != null)
                {
                    var c = new Character();
                    var char_exist = col.FindOne(ce => ce.Id == id);
                    if (char_exist != null)
                    {
                        c = char_exist;
                        c.PlayTime = PlayTime;
                        c.LastPlayTimeUpdate = PlayTimeUpdate;
                    }
                    else
                    {
                        c = new Character()
                        {
                            Id = id,
                            PlayTime = PlayTime,
                            LastPlayTimeUpdate = PlayTimeUpdate,
                        };
                    }

                    col.Upsert(c);
                }
            }
            catch (Exception ex)
            {
                // Todo: Add error handling to not crash game if db is opened in another program
                Console.WriteLine(ex.ToString());
                pluginLog.Error(ex.ToString());
            }
        }

        public static List<Character>? BlacklistCharacter(LiteDatabase db, IPluginLog pluginLog, ulong id)
        {
            pluginLog.Debug($"Database/BlacklistCharacter entered db = {db}, pluginLog = {pluginLog}, id = {id}");
            try
            {
                var col = db.GetCollection<Blacklist>();
                if (col != null)
                {
                    var character = col.FindOne(cf => cf.Id == id);
                    if (character == null)
                    {
                        Blacklist blacklist = new()
                        {
                            Id = id,
                        };
                        col.Insert(blacklist);
                    }
                    DeleteCharacter(db, pluginLog, id);
                    return GetOthersCharacters(db, pluginLog, id);
                }
                return GetOthersCharacters(db, pluginLog, id);
            }
            catch (Exception ex)
            {
                // Todo: Add error handling to not crash game if db is opened in another program
                Console.WriteLine(ex.ToString());
                pluginLog.Error(ex.ToString());
                return null;
            }
        }
    }
}
