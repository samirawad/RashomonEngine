using System;
using System.Collections.Generic;
using System.Linq;
using GoapRpgPoC.Core;

namespace GoapRpgPoC.Activities
{
    public class SleepActivity : Activity
    {
        private int _timer = 0;
        public override Activity Clone() => new SleepActivity();

        public override void Bind(NPC initiator, NPC target = null)
        {
            base.Bind(initiator, target);
            Preconditions[ActivityRole.Initiator] = new Dictionary<string, bool> { { "AtHome", true } };
            Effects[ActivityRole.Initiator] = new Dictionary<string, bool> { { "IsTired", false } };
        }

        protected override void UpdateName() => Name = $"{Participants[ActivityRole.Initiator].Name} is sleeping";

        public override void Initialize() { base.Initialize(); _timer = 0; }

        public override void OnTick(int currentTick)
        {
            if (++_timer >= 5) FinalizeActivity(currentTick);
        }

        public override (bool valid, string blame, string reason) GetContractStatus()
        {
            if (!Participants[ActivityRole.Initiator].GetState("AtHome")) 
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
