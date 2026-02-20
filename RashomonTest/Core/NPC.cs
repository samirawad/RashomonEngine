using System.Collections.Generic;

namespace GoapRpgPoC.Core
{
    public class NPC : Entity
    {
        // Knowledge Graph: Subjective links to other world entities
        public Dictionary<string, Entity> Relationships { get; private set; } = new Dictionary<string, Entity>();
        
        // Internal beliefs/state
        public Dictionary<string, bool> State { get; private set; } = new Dictionary<string, bool>();
        
        // Personal Timeline
        public List<Memory> Memories { get; private set; } = new List<Memory>();

        public NPC(string name, Vector2 startingPosition) : base(name, startingPosition)
        {
            Name = name;
            Position = startingPosition;
        }

        // Methods to manage the Knowledge Graph
        public void SetRelationship(string type, Entity entity) => Relationships[type] = entity;
        public Entity GetRelationship(string type) => Relationships.ContainsKey(type) ? Relationships[type] : null;

        // State Management
        public void SetState(string key, bool value) => State[key] = value;
        public bool GetState(string key) => State.ContainsKey(key) && State[key];

        public void Remember(Activity activity, int time)
        {
            Memories.Add(new Memory(activity, time));
        }
    }
}
