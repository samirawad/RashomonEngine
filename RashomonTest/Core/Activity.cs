using System;
using System.Collections.Generic;

namespace GoapRpgPoC.Core
{
    public abstract class Activity
    {
        public string Name { get; protected set; }
        public Dictionary<ActivityRole, NPC> Participants = new Dictionary<ActivityRole, NPC>();
        
        public Dictionary<ActivityRole, Dictionary<string, bool>> Preconditions = new Dictionary<ActivityRole, Dictionary<string, bool>>();
        public Dictionary<ActivityRole, Dictionary<string, bool>> Effects = new Dictionary<ActivityRole, Dictionary<string, bool>>();

        public virtual bool CompleteActivity(int currentTime)
        {
            Console.WriteLine($"\n[ACTION] Executing: {Name}...");
            
            // Apply Effects to World State
            foreach (var role in Effects.Keys)
            {
                foreach (var effect in Effects[role])
                {
                    Participants[role].SetState(effect.Key, effect.Value);
                    Console.WriteLine($"   -> {Participants[role].Name}'s state '{effect.Key}' is now {effect.Value}");
                }
            }

            // Flyweight Memory & Logging
            GlobalHistoryLog.Record(this);
            foreach (var npc in Participants.Values)
            {
                npc.Remember(this, currentTime);
            }

            return true;
        }
    }
}
