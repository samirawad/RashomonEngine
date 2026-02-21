using System.Collections.Generic;

namespace GoapRpgPoC.Core
{
    public abstract class Entity
    {
        public string Name { get; protected set; }
        public Vector2 Position { get; set; }
        
        // --- 1. HIERARCHY (The Container Logic) ---
        public Entity? Parent { get; set; }
        public List<Entity> Children { get; protected set; } = new List<Entity>();

        // --- 2. AFFORDANCES (What can I do?) ---
        public List<Activity> Affordances { get; protected set; } = new List<Activity>();

        // --- 3. STATE (What am I?) ---
        public Dictionary<string, bool> State { get; private set; } = new Dictionary<string, bool>();

        // --- 4. TAGS (Intrinsic properties like "Edible", "Sharp") ---
        public HashSet<string> Tags { get; private set; } = new HashSet<string>();

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

        public void RemoveChild(Entity child)
        {
            if (Children.Contains(child))
            {
                child.Parent = null;
                Children.Remove(child);
            }
        }

        public void AddAffordance(Activity activity) => Affordances.Add(activity);
        
        public void SetState(string key, bool value) => State[key] = value;
        
        // --- 3. RECURSIVE STATE CHECK ---
        public bool GetState(string key)
        {
            // 1. Check myself
            if (State.ContainsKey(key)) return State[key];

            // 2. Check my children (Inventory, Body Parts, etc.)
            foreach (var child in Children)
            {
                if (child.GetState(key)) return true;
            }

            return false;
        }

        // --- 4. RECURSIVE TAG CHECK ---
        public void AddTag(string tag) => Tags.Add(tag);
        public bool HasTag(string? tag)
        {
            if (string.IsNullOrEmpty(tag)) return true; // Empty requirement is always met

            // 1. Check myself
            if (Tags.Contains(tag)) return true;

            // 2. Check my children
            foreach (var child in Children)
            {
                if (child.HasTag(tag)) return true;
            }

            return false;
        }

        // --- 5. HEARTBEAT (Living Entities) ---
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
