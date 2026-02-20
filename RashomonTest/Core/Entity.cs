using System.Collections.Generic;

namespace GoapRpgPoC.Core
{
    public abstract class Entity
    {
        public string Name { get; protected set; }
        public Vector2 Position { get; set; }
        
        // The Activities this entity "offers" to the world
        public List<Activity> Affordances { get; protected set; } = new List<Activity>();

        public Entity(string name, Vector2 position)
        {
            Name = name;
            Position = position;
        }

        public void AddAffordance(Activity activity) => Affordances.Add(activity);
    }
}
