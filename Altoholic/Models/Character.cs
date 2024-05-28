using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Altoholic.Models
{
    public class Character
    {
        public ulong Id { get; init; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string HomeWorld { get; set; } = string.Empty;
        public string Datacenter {  get; set; } = string.Empty;
        public string Region {  get; set; } = string.Empty;
        public uint LastJob { get; set; } = 0;
        public int LastJobLevel { get; set; } = 0;
        public string FCTag { get; set; } = string.Empty;
        public string FreeCompany { get; set; } = string.Empty;
        public long LastOnline { get; set; } = 0;
        public uint PlayTime { get; set; }
        public long LastPlayTimeUpdate {  get; set; } = 0;
        public Attributes Attributes { get; set; } = null!;
        public PlayerCurrencies Currencies { get; set; } = null!;
        public Jobs Jobs { get; set; } = null!;
        public Profile Profile { get; set; } = null!;
        public List<Quest> Quests { get; set; } = [];
        public List<Inventory> Inventory { get; set; } = [];
        public List<Gear> Gear { get; set; } = [];
        public List<Retainer> Retainers { get; set; } = [];

        public bool HasAnyLevelJob(int level)
        {
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
            return this.Quests.Exists(q => q.Id == id);
        }
        public bool IsQuestCompleted(int id)
        {
            if (this.HasQuest(id))
            {
                return this.Quests.First(q => q.Id == id).Completed;
            }
            else
            {
                return false;
            }
        }
    }
}
