using System;
using System.Collections.Generic;

namespace GoapRpgPoC.Core
{
    public abstract class Activity
    {
        public string Name { get; protected set; }
        public bool IsFinished { get; protected set; } = false;
        
        public Dictionary<ActivityRole, NPC> Participants = new Dictionary<ActivityRole, NPC>();
        
        public Dictionary<ActivityRole, Dictionary<string, bool>> Preconditions = new Dictionary<ActivityRole, Dictionary<string, bool>>();
        public Dictionary<ActivityRole, Dictionary<string, bool>> Effects = new Dictionary<ActivityRole, Dictionary<string, bool>>();

        // Called once when the NPC starts the activity
        public virtual void Initialize() 
        {
            IsFinished = false;
        }

        // Called every tick by the World/NPC
        public abstract void OnTick(int currentTick);

        // The final "Handshake" that applies the world state changes
        protected virtual void ApplyEffects(int currentTime)
        {
            Console.WriteLine($"      [SUCCESS] {Name} is complete!");
            
            foreach (var role in Effects.Keys)
            {
                foreach (var effect in Effects[role])
                {
                    Participants[role].SetState(effect.Key, effect.Value);
                    Console.WriteLine($"         -> {Participants[role].Name}'s state '{effect.Key}' is now {effect.Value}");
                }
            }

            GlobalHistoryLog.Record(this);
            foreach (var npc in Participants.Values)
            {
                npc.Remember(this, currentTime);
            }

            IsFinished = true;
        }
    }
}
