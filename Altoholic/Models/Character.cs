using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Altoholic.Models
{
    public class Character
    {
        public ulong Id { get; init; } = 0;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string HomeWorld { get; set; } = string.Empty;
        public string CurrentWorld { get; set; } = string.Empty;
        public string Datacenter {  get; set; } = string.Empty;
        public string CurrentDatacenter {  get; set; } = string.Empty;
        public string Region {  get; set; } = string.Empty;
        public string CurrentRegion {  get; set; } = string.Empty;
        public bool IsSprout { get; set; } = false;
        public uint LastJob { get; set; } = 0;
        public int LastJobLevel { get; set; } = 0;
        public string FCTag { get; set; } = string.Empty;
        public string FreeCompany { get; set; } = string.Empty;
        public long LastOnline { get; set; } = 0;
        public uint PlayTime { get; set; } = 0;
        public long LastPlayTimeUpdate {  get; set; } = 0;
        public Attributes? Attributes { get; set; }
        public PlayerCurrencies? Currencies { get; set; }
        public Jobs? Jobs { get; set; }
        public Profile? Profile { get; set; }
        public List<Quest> Quests { get; set; } = [];
        public List<Inventory> Inventory { get; set; } = [];
        public List<Inventory> Saddle { get; set; } = [];
        public List<Gear> Gear { get; set; } = [];
        public List<Retainer> Retainers { get; set; } = [];

        public bool HasAnyLevelJob(int level)
        {
            if (Jobs is null) return false;
            foreach (PropertyInfo prop in Jobs.GetType().GetProperties())
            {
                var value = prop.GetValue(Jobs);
                if (value != null)
                {
                    if (value is Job job)
                    {
                        int job_level = job.Level;
                        if (job_level >= level)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public bool HasQuest(int id)
        {
            return Quests.Exists(q => q.Id == id);
        }
        public bool IsQuestCompleted(int id)
        {
            if (HasQuest(id))
            {
                return Quests.First(q => q.Id == id).Completed;
            }
            else
            {
                return false;
            }
        }
    }
}
