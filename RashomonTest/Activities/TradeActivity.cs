using System.Collections.Generic;
using GoapRpgPoC.Core;

namespace GoapRpgPoC.Activities
{
    public class TradeActivity : Activity
    {
        public TradeActivity(NPC buyer, NPC seller)
        {
            Name = $"{buyer.Name} trades gold for {seller.Name}'s apple";
            Participants[ActivityRole.Initiator] = buyer;
            Participants[ActivityRole.Target] = seller;

            // Preconditions
            Preconditions[ActivityRole.Initiator] = new Dictionary<string, bool> { 
                { "HasGold", true }, 
                { "NearTarget", true } // Must be nearby!
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
    }
}
