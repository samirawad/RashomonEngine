using System;
using System.Collections.Generic;
using System.Linq;
using GoapRpgPoC.Core;

namespace GoapRpgPoC.Activities
{
    public class SleepActivity : Activity
    {
        private int _timer = 0;

        public SleepActivity()
        {
            RequiredCapability = Tags.Mouth; // Biological check (placeholder)
            Preconditions[ActivityRole.Initiator] = new Dictionary<string, bool> { { States.AtHome, true } };
            Effects[ActivityRole.Initiator] = new Dictionary<string, bool> { { States.Tired, false } };
        }

        public override Activity Clone() => new SleepActivity();

        protected override void UpdateName() => Name = $"{Participants[ActivityRole.Initiator].Name} is sleeping";

        public override void Initialize() { base.Initialize(); _timer = 0; }

        public override void OnTick(int currentTick)
        {
            if (++_timer >= 5) FinalizeActivity(currentTick);
        }

        public override (bool valid, string blame, string reason) GetContractStatus()
        {
            var baseStatus = base.GetContractStatus();
            if (!baseStatus.valid) return baseStatus;

            if (!Participants[ActivityRole.Initiator].GetState(States.AtHome)) 
                return (false, Participants[ActivityRole.Initiator].Name, "Not at home");
            return (true, "", "");
        }

        protected override void OnFulfill()
        {
            var brain = Participants[ActivityRole.Initiator].Children.OfType<NeedEntity>().FirstOrDefault(n => n.Name == "Energy");
            brain?.Reset();
        }
    }
}
