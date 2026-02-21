using System;
using System.Collections.Generic;
using System.Linq;
using GoapRpgPoC.Core;

namespace GoapRpgPoC.Activities
{
    public class EatActivity : Activity
    {
        public override Activity Clone() => new EatActivity();

        public override void Bind(NPC initiator, NPC target = null)
        {
            base.Bind(initiator, target);
            PreconditionTags[ActivityRole.Initiator] = new List<string> { "Edible" };
            Effects[ActivityRole.Initiator] = new Dictionary<string, bool> { { "IsHungry", false } };
        }

        protected override void UpdateName() => Name = $"{Participants[ActivityRole.Initiator].Name} is eating";

        public override void OnTick(int currentTick) => FinalizeActivity(currentTick);

        public override (bool valid, string blame, string reason) GetContractStatus()
        {
            var eater = Participants[ActivityRole.Initiator];
            if (!eater.Children.Any(c => c.HasTag("Edible"))) return (false, eater.Name, "No food found");
            return (true, "", "");
        }

        protected override void OnFulfill()
        {
            var eater = Participants[ActivityRole.Initiator];
            var food = eater.Children.First(c => c.HasTag("Edible"));
            eater.RemoveChild(food);
            var stomach = eater.Children.OfType<NeedEntity>().FirstOrDefault(n => n.Name == "Stomach");
            stomach?.Reset();
        }
    }
}
