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
            Preconditions[ActivityRole.Initiator] = new Dictionary<string, bool> { { "AtHome", true } };
            Effects[ActivityRole.Initiator] = new Dictionary<string, bool> { { "IsTired", false } };
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
            var init = Participants[ActivityRole.Initiator];
            if (!init.GetState("AtHome")) 
                return (false, init.Name, "Not at home");
            return (true, "", "");
        }

        protected override void OnFulfill()
        {
            var brain = Participants[ActivityRole.Initiator].Children.OfType<NeedEntity>().FirstOrDefault(n => n.Name == "Energy");
            brain?.Reset();
        }
    }
}
