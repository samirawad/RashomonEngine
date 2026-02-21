using System;
using System.Collections.Generic;
using GoapRpgPoC.Core;

namespace GoapRpgPoC.Activities
{
    public class WorkActivity : Activity
    {
        private Entity _workplace;
        private int _timer = 0;
        private const int WORK_DURATION = 5;

        public WorkActivity(Entity workplace)
        {
            _workplace = workplace;
            RequiredCapability = Tags.Hands;
            
            // Precondition: Must be at the workplace
            Preconditions[ActivityRole.Initiator] = new Dictionary<string, bool> { 
                { $"Near({_workplace.Name})", true } 
            };

            // Effect: Earn gold
            Effects[ActivityRole.Initiator] = new Dictionary<string, bool> { 
                { States.HasGold, true } 
            };
        }

        public override Activity Clone() => new WorkActivity(_workplace);

        protected override void UpdateName() => Name = $"{Participants[ActivityRole.Initiator].Name} is working at {_workplace.Name}";

        public override void Initialize() { base.Initialize(); _timer = 0; }

        public override void OnTick(int currentTick)
        {
            if (++_timer >= WORK_DURATION)
            {
                FinalizeActivity(currentTick);
            }
        }

        public override (bool valid, string blame, string reason) GetContractStatus()
        {
            var baseStatus = base.GetContractStatus();
            if (!baseStatus.valid) return baseStatus;

            var worker = Participants[ActivityRole.Initiator];
            if (Vector2.Distance(worker.Position, _workplace.Position) > 0)
                return (false, worker.Name, "Left the workplace");

            return (true, "", "");
        }

        protected override void OnFulfill()
        {
            // Create a physical gold pouch
            Item gold = new Item("Earned Gold");
            gold.SetState(States.HasGold, true);
            Participants[ActivityRole.Initiator].AddChild(gold);
        }
    }
}
