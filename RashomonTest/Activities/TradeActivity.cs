using System;
using System.Collections.Generic;
using System.Linq;
using GoapRpgPoC.Core;

namespace GoapRpgPoC.Activities
{
    public class TradeActivity : Activity
    {
        public TradeActivity()
        {
            RequiredCapability = Tags.Hands;
            Preconditions[ActivityRole.Initiator] = new Dictionary<string, bool> { { States.HasGold, true } };
            PreconditionTags[ActivityRole.Target] = new List<string> { Tags.Edible };

            Effects[ActivityRole.Initiator] = new Dictionary<string, bool> { { Tags.Edible, true }, { States.HasGold, false } };
            Effects[ActivityRole.Target] = new Dictionary<string, bool> { { States.HasGold, true }, { Tags.Edible, false } };
        }

        public override Activity Clone() => new TradeActivity();

        public override void Bind(NPC initiator, NPC? target = null)
        {
            base.Bind(initiator, target);
            if (target != null) Preconditions[ActivityRole.Initiator][$"Near({target.Name})"] = true;
        }

        protected override void UpdateName() 
        {
            var sellerName = Participants.ContainsKey(ActivityRole.Target) ? Participants[ActivityRole.Target].Name : "someone";
            Name = $"{Participants[ActivityRole.Initiator].Name} is trading with {sellerName}";
        }

        public override void OnTick(int currentTick)
        {
            var buyer = Participants[ActivityRole.Initiator];
            var seller = Participants[ActivityRole.Target];

            if (seller.SubscribedScene != this)
            {
                buyer.LogDebug($"[TRADE] Waiting for {seller.Name} to join trade...");
                seller.ReceiveInvitation(new Invitation(this, ActivityRole.Target, buyer));
                return;
            }

            FinalizeActivity(currentTick);
        }

        public override (bool valid, string blame, string reason) GetContractStatus()
        {
            var baseStatus = base.GetContractStatus();
            if (!baseStatus.valid) return baseStatus;

            var buyer = Participants[ActivityRole.Initiator];
            var seller = Participants[ActivityRole.Target];

            if (Vector2.Distance(buyer.Position, seller.Position) > 0) return (false, buyer.Name, "Too far from seller");
            if (!seller.HasTag(Tags.Edible)) return (false, seller.Name, "No food to sell");
            if (!buyer.GetState(States.HasGold)) return (false, buyer.Name, "No gold to buy");
            if (seller.SubscribedScene != this) return (false, seller.Name, "Not participating");

            return (true, "", "");
        }

        protected override void OnFulfill()
        {
            var buyer = Participants[ActivityRole.Initiator];
            var seller = Participants[ActivityRole.Target];

            var food = seller.Children.First(c => c.HasTag(Tags.Edible));
            var gold = buyer.Children.First(c => c.GetState(States.HasGold));

            seller.RemoveChild(food);
            buyer.AddChild(food);
            buyer.RemoveChild(gold);
            seller.AddChild(gold);
        }
    }
}
