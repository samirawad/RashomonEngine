using System;
using System.Collections.Generic;
using System.Linq;
using GoapRpgPoC.Core;

namespace GoapRpgPoC.Activities
{
    public class TradeActivity : Activity
    {
        public override Activity Clone() => new TradeActivity();

        public override void Bind(NPC initiator, NPC target = null)
        {
            base.Bind(initiator, target);
            
            // Preconditions for the Buyer (Initiator)
            Preconditions[ActivityRole.Initiator] = new Dictionary<string, bool> { 
                { "HasGold", true }, 
                { $"Near({target.Name})", true } 
            };
            
            // Requirements for the Seller (Target)
            PreconditionTags[ActivityRole.Target] = new List<string> { "Edible" };

            // Effects for both
            Effects[ActivityRole.Initiator] = new Dictionary<string, bool> { { "Edible", true }, { "HasGold", false } };
            Effects[ActivityRole.Target] = new Dictionary<string, bool> { { "HasGold", true }, { "Edible", false } };
        }

        protected override void UpdateName() 
        {
            Name = $"{Participants[ActivityRole.Initiator].Name} is trading with {Participants[ActivityRole.Target].Name}";
        }

        public override void OnTick(int currentTick)
        {
            var buyer = Participants[ActivityRole.Initiator];
            var seller = Participants[ActivityRole.Target];

            // 1. Check if Seller has accepted the invitation
            if (seller.SubscribedScene != this)
            {
                // Send/Resend Invitation
                buyer.LogDebug($"[TRADE] Sending invitation to {seller.Name}...");
                seller.ReceiveInvitation(new Invitation(this, ActivityRole.Target, buyer));
                
                // We wait until they subscribe
                return;
            }

            // 2. Both are here and subscribed! Execute the trade.
            FinalizeActivity(currentTick);
        }

        public override (bool valid, string blame, string reason) GetContractStatus()
        {
            var buyer = Participants[ActivityRole.Initiator];
            var seller = Participants[ActivityRole.Target];

            if (Vector2.Distance(buyer.Position, seller.Position) > 0) return (false, buyer.Name, "Too far from seller");
            if (!seller.HasTag("Edible")) return (false, seller.Name, "No food to sell");
            if (!buyer.GetState("HasGold")) return (false, buyer.Name, "No gold to buy");
            if (seller.SubscribedScene != this) return (false, seller.Name, "Refused or left the trade");

            return (true, "", "");
        }

        protected override void OnFulfill()
        {
            var buyer = Participants[ActivityRole.Initiator];
            var seller = Participants[ActivityRole.Target];

            var food = seller.Children.First(c => c.HasTag("Edible"));
            var gold = buyer.Children.First(c => c.GetState("HasGold"));

            seller.RemoveChild(food);
            buyer.AddChild(food);
            buyer.RemoveChild(gold);
            seller.AddChild(gold);
        }
    }
}
