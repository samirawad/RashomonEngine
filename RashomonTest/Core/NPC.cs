using System.Collections.Generic;

namespace GoapRpgPoC.Core
{
    public class NPC : Entity
    {
        // Knowledge Graph: Links to EXTERNAL entities
        public Dictionary<string, Entity> Relationships { get; private set; } = new Dictionary<string, Entity>();
        
        // Sentience: Personal Timeline
        public List<Memory> Memories { get; private set; } = new List<Memory>();

        // Debugging: Technical log of simulation events
        public List<string> DebugLog { get; private set; } = new List<string>();

        public NPC(string name, Vector2 startingPosition) : base(name, startingPosition)
        {
        }

        // External Relationships Management
        public void SetRelationship(string type, Entity entity) => Relationships[type] = entity;
        public Entity GetRelationship(string type) => Relationships.ContainsKey(type) ? Relationships[type] : null;

        public void Remember(Activity activity, int time)
        {
            Memories.Add(new Memory(activity, time));
        }

        public void LogDebug(string message)
        {
            DebugLog.Add(message);
        }
    }
}
