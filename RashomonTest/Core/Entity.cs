using System.Collections.Generic;

namespace GoapRpgPoC.Core
{
    public abstract class Entity
    {
        public string Name { get; protected set; }
        public Vector2 Position { get; set; }
        
        // --- 1. HIERARCHY (The Container Logic) ---
        public Entity Parent { get; set; }
        public List<Entity> Children { get; protected set; } = new List<Entity>();

        // --- 2. AFFORDANCES (What can I do?) ---
        public List<Activity> Affordances { get; protected set; } = new List<Activity>();

        // --- 3. STATE (What am I?) ---
        public Dictionary<string, bool> State { get; private set; } = new Dictionary<string, bool>();

        public Entity(string name, Vector2 position)
        {
            Name = name;
            Position = position;
        }

        public void AddChild(Entity child)
        {
            child.Parent = this;
            Children.Add(child);
        }

        public void AddAffordance(Activity activity) => Affordances.Add(activity);
        
        public void SetState(string key, bool value) => State[key] = value;
        public bool GetState(string key) => State.ContainsKey(key) && State[key];

        // --- 4. HEARTBEAT (Living Entities) ---
        public virtual void Tick(int currentTick)
        {
            // Entities can update their internal states here
            foreach (var child in Children)
            {
                child.Tick(currentTick);
            }
        }
    }
}
