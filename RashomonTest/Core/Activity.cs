using System;
using System.Collections.Generic;

namespace GoapRpgPoC.Core
{
    public abstract class Activity
    {
        public string Name { get; protected set; }
        public bool IsFinished { get; protected set; } = false;
        public bool WasSuccessful { get; protected set; } = false;
        
        // Roles to be filled (e.g., Initiator, Target)
        public Dictionary<ActivityRole, NPC> Participants = new Dictionary<ActivityRole, NPC>();
        
        public Dictionary<ActivityRole, Dictionary<string, bool>> Preconditions = new Dictionary<ActivityRole, Dictionary<string, bool>>();
        public Dictionary<ActivityRole, Dictionary<string, bool>> Effects = new Dictionary<ActivityRole, Dictionary<string, bool>>();
        public Dictionary<ActivityRole, List<string>> PreconditionTags = new Dictionary<ActivityRole, List<string>>();

        // --- NEW: CLONING FOR LATE BINDING ---
        // Every activity must be able to create a fresh copy of itself
        public abstract Activity Clone();

        public virtual void Bind(NPC initiator, NPC target = null)
        {
            Participants[ActivityRole.Initiator] = initiator;
            if (target != null) Participants[ActivityRole.Target] = target;
            
            // Re-generate the name based on the actual participants
            UpdateName();
        }

        protected virtual void UpdateName() { }

        public virtual void Initialize() 
        {
            IsFinished = false;
            WasSuccessful = false;
        }

        public abstract void OnTick(int currentTick);

        public virtual (bool valid, string blame, string reason) GetContractStatus()
        {
            return (true, "", "");
        }

        protected void FinalizeActivity(int currentTime)
        {
            var status = GetContractStatus();

            if (!status.valid)
            {
                WasSuccessful = false;
                string failMsg = $"[CONTRACT FAILED] {Name} aborted. Blame: {status.blame}. Reason: {status.reason}";
                foreach (var npc in Participants.Values)
                {
                    npc.LogDebug(failMsg);
                    npc.Remember(this, currentTime);
                }
            }
            else
            {
                WasSuccessful = true;
                OnFulfill();

                foreach (var role in Effects.Keys)
                {
                    foreach (var effect in Effects[role])
                    {
                        Participants[role].SetState(effect.Key, effect.Value);
                    }
                }

                foreach (var npc in Participants.Values)
                {
                    npc.LogDebug($"[CONTRACT SUCCESS] {Name} fulfilled.");
                    npc.Remember(this, currentTime);
                }
            }

            GlobalHistoryLog.Record(this);
            IsFinished = true;
        }

        protected virtual void OnFulfill() { }
    }
}
