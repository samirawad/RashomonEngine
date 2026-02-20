using System;
using System.Collections.Generic;
using GoapRpgPoC.Core;

namespace GoapRpgPoC.Activities
{
    public class TradeActivity : Activity
    {
        public TradeActivity(NPC buyer, NPC seller)
        {
            Name = $"{buyer.Name} is trading gold for {seller.Name}'s apple";
            Participants[ActivityRole.Initiator] = buyer;
            Participants[ActivityRole.Target] = seller;

            // Preconditions
            Preconditions[ActivityRole.Initiator] = new Dictionary<string, bool> { 
                { "HasGold", true }, 
                { "NearTarget", true } 
            };
            Preconditions[ActivityRole.Target] = new Dictionary<string, bool> { 
                { "HasApple", true } 
            };

            // Effects
            Effects[ActivityRole.Initiator] = new Dictionary<string, bool> { 
                { "HasApple", true }, 
                { "HasGold", false } 
            };
            Effects[ActivityRole.Target] = new Dictionary<string, bool> { 
                { "HasGold", true }, 
                { "HasApple", false } 
            };
        }

        public override void OnTick(int currentTick)
        {
            // Trading is instant once started!
            ApplyEffects(currentTick);
        }
    }
}
