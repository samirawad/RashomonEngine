using System.Collections.Generic;

namespace GoapRpgPoC.Core
{
    public class NPC
    {
        public string Name { get; private set; }
        public Vector2 CurrentPosition { get; set; }
        public Dictionary<string, bool> State { get; private set; } = new Dictionary<string, bool>();
        public List<Memory> Memories { get; private set; } = new List<Memory>();

        public NPC(string name, Vector2 startingPosition)
        {
            Name = name;
            CurrentPosition = startingPosition;
        }

        public void SetState(string key, bool value) => State[key] = value;
        public bool GetState(string key) => State.ContainsKey(key) && State[key];

        public void Remember(Activity activity, int time)
        {
            Memories.Add(new Memory(activity, time));
        }
    }
}
