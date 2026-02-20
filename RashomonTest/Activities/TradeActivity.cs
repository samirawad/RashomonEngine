using System;
using System.Collections.Generic;
using System.Linq;
using GoapRpgPoC.Core;

namespace GoapRpgPoC.Activities
{
    public class TradeActivity : Activity
    {
        public TradeActivity(NPC buyer, NPC seller)
        {
            Name = $"{buyer.Name} is trading gold for {seller.Name}'s food";
            Participants[ActivityRole.Initiator] = buyer;
            Participants[ActivityRole.Target] = seller;

            // Preconditions: Buyer needs gold, Seller needs something edible
            Preconditions[ActivityRole.Initiator] = new Dictionary<string, bool> { 
                { "HasGold", true }, 
                { "NearTarget", true } 
            };
            Preconditions[ActivityRole.Target] = new Dictionary<string, bool> { 
                { "Edible", true } 
            };

            // Effects: Buyer gets food, loses gold. Seller gets gold, loses food.
            Effects[ActivityRole.Initiator] = new Dictionary<string, bool> { 
                { "Edible", true }, 
                { "HasGold", false } 
            };
            Effects[ActivityRole.Target] = new Dictionary<string, bool> { 
                { "HasGold", true }, 
                { "Edible", false } 
            };
        }

        public override void OnTick(int currentTick)
        {
            var buyer = Participants[ActivityRole.Initiator];
            var seller = Participants[ActivityRole.Target];

            // --- PHYSICAL ENTITY TRANSFER ---
            // 1. Find the food (by TAG) and the gold entities (by STATE)
            var foodItem = seller.Children.FirstOrDefault(c => c.HasTag("Edible"));
            var goldItem = buyer.Children.FirstOrDefault(c => c.GetState("HasGold"));

            if (foodItem != null && goldItem != null)
            {
                string tradeMsg = $"[TRADE] {buyer.Name} traded {goldItem.Name} for {seller.Name}'s {foodItem.Name}";
                Console.WriteLine($"   {tradeMsg}");
                buyer.LogDebug(tradeMsg);
                seller.LogDebug(tradeMsg);

                // 2. Move the entities in the hierarchy
                seller.RemoveChild(foodItem);
                buyer.AddChild(foodItem);

                buyer.RemoveChild(goldItem);
                seller.AddChild(goldItem);

                ApplyEffects(currentTick);
            }
            else
            {
                string failMsg = $"[TRADE FAIL] Exchange between {buyer.Name} and {seller.Name} aborted.";
                buyer.LogDebug(failMsg);
                seller.LogDebug(failMsg);

                if (foodItem == null) buyer.LogDebug($"   -> Reason: {seller.Name} has no items tagged 'Edible'");
                if (goldItem == null) buyer.LogDebug($"   -> Reason: {buyer.Name} has no items with state 'HasGold'");

                IsFinished = true;
            }
        }
    }
}
