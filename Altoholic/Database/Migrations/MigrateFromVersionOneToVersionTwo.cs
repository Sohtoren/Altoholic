using Dapper;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Altoholic.Database.Migrations
{
    public static class MigrateFromVersionOneToVersionTwo
    {
    #pragma warning disable CA1812
        // ReSharper disable once ClassNeverInstantiated.Local
        private class Gear
        {
            public int Id { get; set; }
            public uint ItemId { get; set; }
            public bool HQ { get; set; }
            public bool CompanyCrestApplied { get; set; }
            public short Slot { get; set; }
            public ushort Spiritbond { get; set; }
            public ushort Condition { get; set; }
            public ulong CrafterContentID { get; set; }
            public ushort Materia { get; set; }
            public byte MateriaGrade { get; set; }
            public byte Stain { get; set; }
            public byte Stain2 { get; set; }
            public uint GlamourID { get; set; }
        }
        private class ArmoryGear
        {
            public required List<Gear> MainHand { get; init; }
            public required List<Gear> Head { get; init; }
            public required List<Gear> Body { get; init; }
            public required List<Gear> Hands { get; init; }
            public required List<Gear> Legs { get; init; }
            public required List<Gear> Feets { get; init; }
            public required List<Gear> OffHand { get; init; }
            public required List<Gear> Ear { get; init; }
            public required List<Gear> Neck { get; init; }
            public required List<Gear> Wrist { get; init; }
            public required List<Gear> Rings { get; init; }
            public required List<Gear> SoulCrystal { get; init; }
        }
        // ReSharper disable once ClassNeverInstantiated.Local
        private class Retainer
        {
            public ulong Id { get; init; }
            public bool Available { get; init; } = false;
            public string Name { get; set; } = string.Empty;
            public byte ClassJob { get; set; }
            public int Level { get; set; } = 0;
            public uint Gils { get; set; } = 0;
            public FFXIVClientStructs.FFXIV.Client.Game.RetainerManager.RetainerTown Town { get; set; } = 0;
            public uint MarketItemCount { get; set; } = 0;
            public uint MarketExpire { get; set; } = 0;
            public uint VentureID { get; set; } = 0;
            public uint VentureComplete { get; set; } = 0;
            public long LastUpdate { get; set; } = 0;
            public int DisplayOrder { get; set; } = 0;
            public List<Gear> Gear { get; set; } = [];
            public List<Models.Inventory> Inventory { get; set; } = [];
            public List<Models.Inventory> MarketInventory { get; set; } = [];
        }
#pragma warning restore CA1812
        private class Character
        {
            public ulong CharacterId { get; init; } = 0;
            public Models.ArmoryGear? ArmoryInventory { get; set; } = null;
            public List<Models.Retainer> Retainers { get; set; } = [];
            public List<Models.Gear> Gear { get; set; } = [];
        }

        public static bool Do(SqliteConnection db, string characterTableName)
        {
            try
            {
                string sql = $"SELECT * FROM {characterTableName}";
                IEnumerable<Models.DatabaseCharacter> dbCharacters = db.Query<Models.DatabaseCharacter>(sql);
                foreach (Models.DatabaseCharacter dbCharacter in dbCharacters)
                {
                    Character character = MigrateByteToByteArray(dbCharacter);

                    Update(db, characterTableName, character);
                }
                return true;
            }
            catch (Exception ex)
            {
                Plugin.Log.Error(ex.ToString());
                return false;
            }
        }
        private static Character MigrateByteToByteArray(Models.DatabaseCharacter databaseCharacter)
        {
            Character character = new() { CharacterId = databaseCharacter.CharacterId };

            List<Gear> gears = string.IsNullOrEmpty(databaseCharacter.Gear)
                ? []
                : System.Text.Json.JsonSerializer.Deserialize<List<Gear>>(databaseCharacter.Gear) ?? [];

            character.Gear.AddRange(gears.Select(gear => new Models.Gear()
            {
                Id = gear.Id,
                ItemId = gear.ItemId,
                HQ = gear.HQ,
                CompanyCrestApplied = gear.CompanyCrestApplied,
                Slot = gear.Slot,
                Spiritbond = gear.Spiritbond,
                Condition = gear.Condition,
                CrafterContentID = gear.CrafterContentID,
                Materia = [gear.Materia],
                MateriaGrade = [gear.MateriaGrade],
                Stain = gear.Stain,
                Stain2 = gear.Stain2,
                GlamourID = gear.GlamourID
            }));

            ArmoryGear? armoryInventory = string.IsNullOrEmpty(databaseCharacter.ArmoryInventory)
                ? null
                : System.Text.Json.JsonSerializer.Deserialize<ArmoryGear>(databaseCharacter.ArmoryInventory);

            if (armoryInventory != null)
            {
                foreach (Gear g in armoryInventory.MainHand)
                {
                    character.ArmoryInventory?.MainHand.Add(new Models.Gear()
                    {
                        Id = g.Id,
                        ItemId = g.ItemId,
                        HQ = g.HQ,
                        CompanyCrestApplied = g.CompanyCrestApplied,
                        Slot = g.Slot,
                        Spiritbond = g.Spiritbond,
                        Condition = g.Condition,
                        CrafterContentID = g.CrafterContentID,
                        Materia = [g.Materia],
                        MateriaGrade = [g.MateriaGrade],
                        Stain = g.Stain,
                        Stain2 = g.Stain2,
                        GlamourID = g.GlamourID
                    });
                }
                foreach (Gear g in armoryInventory.OffHand)
                {
                    character.ArmoryInventory?.OffHand.Add(new Models.Gear()
                    {
                        Id = g.Id,
                        ItemId = g.ItemId,
                        HQ = g.HQ,
                        CompanyCrestApplied = g.CompanyCrestApplied,
                        Slot = g.Slot,
                        Spiritbond = g.Spiritbond,
                        Condition = g.Condition,
                        CrafterContentID = g.CrafterContentID,
                        Materia = [g.Materia],
                        MateriaGrade = [g.MateriaGrade],
                        Stain = g.Stain,
                        Stain2 = g.Stain2,
                        GlamourID = g.GlamourID
                    });
                }
                foreach (Gear g in armoryInventory.Head)
                {
                    character.ArmoryInventory?.Head.Add(new Models.Gear()
                    {
                        Id = g.Id,
                        ItemId = g.ItemId,
                        HQ = g.HQ,
                        CompanyCrestApplied = g.CompanyCrestApplied,
                        Slot = g.Slot,
                        Spiritbond = g.Spiritbond,
                        Condition = g.Condition,
                        CrafterContentID = g.CrafterContentID,
                        Materia = [g.Materia],
                        MateriaGrade = [g.MateriaGrade],
                        Stain = g.Stain,
                        Stain2 = g.Stain2,
                        GlamourID = g.GlamourID
                    });
                }
                foreach (Gear g in armoryInventory.Body)
                {
                    character.ArmoryInventory?.Body.Add(new Models.Gear()
                    {
                        Id = g.Id,
                        ItemId = g.ItemId,
                        HQ = g.HQ,
                        CompanyCrestApplied = g.CompanyCrestApplied,
                        Slot = g.Slot,
                        Spiritbond = g.Spiritbond,
                        Condition = g.Condition,
                        CrafterContentID = g.CrafterContentID,
                        Materia = [g.Materia],
                        MateriaGrade = [g.MateriaGrade],
                        Stain = g.Stain,
                        Stain2 = g.Stain2,
                        GlamourID = g.GlamourID
                    });
                }
                foreach (Gear g in armoryInventory.Hands)
                {
                    character.ArmoryInventory?.Hands.Add(new Models.Gear()
                    {
                        Id = g.Id,
                        ItemId = g.ItemId,
                        HQ = g.HQ,
                        CompanyCrestApplied = g.CompanyCrestApplied,
                        Slot = g.Slot,
                        Spiritbond = g.Spiritbond,
                        Condition = g.Condition,
                        CrafterContentID = g.CrafterContentID,
                        Materia = [g.Materia],
                        MateriaGrade = [g.MateriaGrade],
                        Stain = g.Stain,
                        Stain2 = g.Stain2,
                        GlamourID = g.GlamourID
                    });
                }
                foreach (Gear g in armoryInventory.Legs)
                {
                    character.ArmoryInventory?.Legs.Add(new Models.Gear()
                    {
                        Id = g.Id,
                        ItemId = g.ItemId,
                        HQ = g.HQ,
                        CompanyCrestApplied = g.CompanyCrestApplied,
                        Slot = g.Slot,
                        Spiritbond = g.Spiritbond,
                        Condition = g.Condition,
                        CrafterContentID = g.CrafterContentID,
                        Materia = [g.Materia],
                        MateriaGrade = [g.MateriaGrade],
                        Stain = g.Stain,
                        Stain2 = g.Stain2,
                        GlamourID = g.GlamourID
                    });
                }
                foreach (Gear g in armoryInventory.Feets)
                {
                    character.ArmoryInventory?.Feets.Add(new Models.Gear()
                    {
                        Id = g.Id,
                        ItemId = g.ItemId,
                        HQ = g.HQ,
                        CompanyCrestApplied = g.CompanyCrestApplied,
                        Slot = g.Slot,
                        Spiritbond = g.Spiritbond,
                        Condition = g.Condition,
                        CrafterContentID = g.CrafterContentID,
                        Materia = [g.Materia],
                        MateriaGrade = [g.MateriaGrade],
                        Stain = g.Stain,
                        Stain2 = g.Stain2,
                        GlamourID = g.GlamourID
                    });
                }
                foreach (Gear g in armoryInventory.OffHand)
                {
                    character.ArmoryInventory?.OffHand.Add(new Models.Gear()
                    {
                        Id = g.Id,
                        ItemId = g.ItemId,
                        HQ = g.HQ,
                        CompanyCrestApplied = g.CompanyCrestApplied,
                        Slot = g.Slot,
                        Spiritbond = g.Spiritbond,
                        Condition = g.Condition,
                        CrafterContentID = g.CrafterContentID,
                        Materia = [g.Materia],
                        MateriaGrade = [g.MateriaGrade],
                        Stain = g.Stain,
                        Stain2 = g.Stain2,
                        GlamourID = g.GlamourID
                    });
                }
                foreach (Gear g in armoryInventory.Ear)
                {
                    character.ArmoryInventory?.Ear.Add(new Models.Gear()
                    {
                        Id = g.Id,
                        ItemId = g.ItemId,
                        HQ = g.HQ,
                        CompanyCrestApplied = g.CompanyCrestApplied,
                        Slot = g.Slot,
                        Spiritbond = g.Spiritbond,
                        Condition = g.Condition,
                        CrafterContentID = g.CrafterContentID,
                        Materia = [g.Materia],
                        MateriaGrade = [g.MateriaGrade],
                        Stain = g.Stain,
                        Stain2 = g.Stain2,
                        GlamourID = g.GlamourID
                    });
                }
                foreach (Gear g in armoryInventory.Neck)
                {
                    character.ArmoryInventory?.Neck.Add(new Models.Gear()
                    {
                        Id = g.Id,
                        ItemId = g.ItemId,
                        HQ = g.HQ,
                        CompanyCrestApplied = g.CompanyCrestApplied,
                        Slot = g.Slot,
                        Spiritbond = g.Spiritbond,
                        Condition = g.Condition,
                        CrafterContentID = g.CrafterContentID,
                        Materia = [g.Materia],
                        MateriaGrade = [g.MateriaGrade],
                        Stain = g.Stain,
                        Stain2 = g.Stain2,
                        GlamourID = g.GlamourID
                    });
                }
                foreach (Gear g in armoryInventory.Wrist)
                {
                    character.ArmoryInventory?.Wrist.Add(new Models.Gear()
                    {
                        Id = g.Id,
                        ItemId = g.ItemId,
                        HQ = g.HQ,
                        CompanyCrestApplied = g.CompanyCrestApplied,
                        Slot = g.Slot,
                        Spiritbond = g.Spiritbond,
                        Condition = g.Condition,
                        CrafterContentID = g.CrafterContentID,
                        Materia = [g.Materia],
                        MateriaGrade = [g.MateriaGrade],
                        Stain = g.Stain,
                        Stain2 = g.Stain2,
                        GlamourID = g.GlamourID
                    });
                }
                foreach (Gear g in armoryInventory.Rings)
                {
                    character.ArmoryInventory?.Rings.Add(new Models.Gear()
                    {
                        Id = g.Id,
                        ItemId = g.ItemId,
                        HQ = g.HQ,
                        CompanyCrestApplied = g.CompanyCrestApplied,
                        Slot = g.Slot,
                        Spiritbond = g.Spiritbond,
                        Condition = g.Condition,
                        CrafterContentID = g.CrafterContentID,
                        Materia = [g.Materia],
                        MateriaGrade = [g.MateriaGrade],
                        Stain = g.Stain,
                        Stain2 = g.Stain2,
                        GlamourID = g.GlamourID
                    });
                }
                foreach (Gear g in armoryInventory.SoulCrystal)
                {
                    character.ArmoryInventory?.SoulCrystal.Add(new Models.Gear()
                    {
                        Id = g.Id,
                        ItemId = g.ItemId,
                        HQ = g.HQ,
                        CompanyCrestApplied = g.CompanyCrestApplied,
                        Slot = g.Slot,
                        Spiritbond = g.Spiritbond,
                        Condition = g.Condition,
                        CrafterContentID = g.CrafterContentID,
                        Materia = [g.Materia],
                        MateriaGrade = [g.MateriaGrade],
                        Stain = g.Stain,
                        Stain2 = g.Stain2,
                        GlamourID = g.GlamourID
                    });
                }
            }


            List<Models.Retainer> newRetainers = [];
            List<Retainer> retainers = string.IsNullOrEmpty(databaseCharacter.Retainers)
                ? []
                : System.Text.Json.JsonSerializer.Deserialize<List<Retainer>>(databaseCharacter.Retainers) ?? [];

            
            foreach (Retainer retainer in retainers)
            {
                Models.Retainer newRetainer = new()
                {
                    Id = retainer.Id,
                    Available = retainer.Available,
                    Name = retainer.Name,
                    ClassJob = retainer.ClassJob,
                    Level  = retainer.Level,
                    Gils = retainer.Gils,
                    Town = retainer.Town,
                    MarketItemCount = retainer.MarketItemCount,
                    MarketExpire = retainer.MarketExpire,
                    VentureID = retainer.VentureID,
                    VentureComplete = retainer.VentureComplete,
                    LastUpdate = retainer.LastUpdate,
                    DisplayOrder = retainer.DisplayOrder,
                    Inventory = retainer.Inventory,
                    MarketInventory = retainer.MarketInventory,
                };

                newRetainer.Gear.AddRange(retainer.Gear.Select(gear => new Models.Gear()
                {
                    Id = gear.Id,
                    ItemId = gear.ItemId,
                    HQ = gear.HQ,
                    CompanyCrestApplied = gear.CompanyCrestApplied,
                    Slot = gear.Slot,
                    Spiritbond = gear.Spiritbond,
                    Condition = gear.Condition,
                    CrafterContentID = gear.CrafterContentID,
                    Materia = [gear.Materia],
                    MateriaGrade = [gear.MateriaGrade],
                    Stain = gear.Stain,
                    Stain2 = gear.Stain2,
                    GlamourID = gear.GlamourID
                }));

                newRetainers.Add(newRetainer);
            }

            character.Retainers = newRetainers;
            return character;
        }
        private static void Update(SqliteConnection db, string characterTableName, Character character)
        {
            string armoryInventory = System.Text.Json.JsonSerializer.Serialize(character.ArmoryInventory);
            string retainers = System.Text.Json.JsonSerializer.Serialize(character.Retainers);
            string gear = System.Text.Json.JsonSerializer.Serialize(character.Gear);
            ulong characterId = character.CharacterId;

            try
            {
                string updateSql = $"UPDATE {characterTableName} SET [Gear] = @gear, [ArmoryInventory] = @armoryInventory, [Retainers] = @retainers WHERE [CharacterId] = @characterId";
                db.Execute(updateSql, new { gear, armoryInventory, retainers, characterId });

            }
            catch (Exception ex)
            {
                Plugin.Log.Error(ex.ToString());
            }
        }
    }
}