using System;
using System.Collections.Generic;
using GoapRpgPoC.Core;

namespace GoapRpgPoC.Activities
{
    public class HarvestActivity : Activity
    {
        private Entity _orchard;
        private int _timer = 0;

        public HarvestActivity(Entity orchard) { _orchard = orchard; }

        public override Activity Clone() => new HarvestActivity(_orchard);

        public override void Bind(NPC initiator, NPC target = null)
        {
            base.Bind(initiator, target);
            Preconditions[ActivityRole.Initiator] = new Dictionary<string, bool> { { $"Near({_orchard.Name})", true } };
            Effects[ActivityRole.Initiator] = new Dictionary<string, bool> { { "Edible", true } };
        }

        protected override void UpdateName() => Name = $"{Participants[ActivityRole.Initiator].Name} is harvesting at {_orchard.Name}";

        public override void Initialize() { base.Initialize(); _timer = 0; }

        public override void OnTick(int currentTick)
        {
            if (++_timer >= 3) FinalizeActivity(currentTick);
        }

        public override (bool valid, string blame, string reason) GetContractStatus()
        {
            if (Vector2.Distance(Participants[ActivityRole.Initiator].Position, _orchard.Position) > 0)
                return (false, Participants[ActivityRole.Initiator].Name, "Not at orchard");
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
