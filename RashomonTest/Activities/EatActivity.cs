using System;
using System.Collections.Generic;
using System.Linq;
using GoapRpgPoC.Core;

namespace GoapRpgPoC.Activities
{
    public class EatActivity : Activity
    {
        public EatActivity()
        {
            RequiredCapability = Tags.Mouth;
            PreconditionTags[ActivityRole.Initiator] = new List<string> { Tags.Edible };
            Effects[ActivityRole.Initiator] = new Dictionary<string, bool> { { States.Hungry, false } };
        }

        public override Activity Clone() => new EatActivity();

        protected override void UpdateName() => Name = $"{Participants[ActivityRole.Initiator].Name} is eating";

        public override void OnTick(int currentTick) => FinalizeActivity(currentTick);

        public override (bool valid, string blame, string reason) GetContractStatus()
        {
            var baseStatus = base.GetContractStatus();
            if (!baseStatus.valid) return baseStatus;

            var eater = Participants[ActivityRole.Initiator];
            if (!eater.Children.Any(c => c.HasTag(Tags.Edible))) return (false, eater.Name, "No food found");
            return (true, "", "");
        }

        protected override void OnFulfill()
        {
            var eater = Participants[ActivityRole.Initiator];
            var food = eater.Children.First(c => c.HasTag(Tags.Edible));
            eater.RemoveChild(food);
            var stomach = eater.Children.OfType<NeedEntity>().FirstOrDefault(n => n.Name == "Stomach");
            stomach?.Reset();
        }
    }
}
