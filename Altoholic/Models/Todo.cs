using System;
using System.Collections.Generic;
using System.Text;

namespace Altoholic.Models
{
    public class TodoCategory
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool State { get; set; } //Active/Disabled
        public DateTime? CreationDate { get; set; }
    }
    public class Todo
    {
        public int Id { get; set; }
        public uint CharacterId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool State { get; set; } //Active/Disabled
        public bool Completion { get; set; } //Done/Not done
        public DateTime? CreationDate { get; set; }
    }
}
