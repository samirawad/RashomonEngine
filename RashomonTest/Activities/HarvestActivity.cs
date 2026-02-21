using System;
using System.Collections.Generic;
using GoapRpgPoC.Core;

namespace GoapRpgPoC.Activities
{
    public class HarvestActivity : Activity
    {
        private Entity _orchard;
        private int _timer = 0;

        public HarvestActivity(Entity orchard) 
        { 
            _orchard = orchard; 
            RequiredCapability = "Hands";
            Preconditions[ActivityRole.Initiator] = new Dictionary<string, bool> { { $"Near({_orchard.Name})", true } };
            Effects[ActivityRole.Initiator] = new Dictionary<string, bool> { { "Edible", true } };
        }

        public override Activity Clone() => new HarvestActivity(_orchard);

        protected override void UpdateName() => Name = $"{Participants[ActivityRole.Initiator].Name} is harvesting at {_orchard.Name}";

        public override void Initialize() { base.Initialize(); _timer = 0; }

        public override void OnTick(int currentTick)
        {
            if (++_timer >= 3) FinalizeActivity(currentTick);
        }

        public override (bool valid, string blame, string reason) GetContractStatus()
        {
            var baseStatus = base.GetContractStatus();
            if (!baseStatus.valid) return baseStatus;

            var init = Participants[ActivityRole.Initiator];
            if (Vector2.Distance(init.Position, _orchard.Position) > 0)
                return (false, init.Name, "Not at orchard");
            return (true, "", "");
        }

        protected override void OnFulfill()
        {
            Item apple = new Item("Apple");
            apple.AddTag("Edible");
            Participants[ActivityRole.Initiator].AddChild(apple);
        }
    }
}
