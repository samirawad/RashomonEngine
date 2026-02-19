using System.Collections.Generic;
using GoapRpgPoC.Core;

namespace GoapRpgPoC.Activities
{
    public class WalkToActivity : Activity
    {
        public WalkToActivity(NPC walker, NPC target)
        {
            Name = $"{walker.Name} walks to {target.Name}";
            Participants[ActivityRole.Initiator] = walker;
            Participants[ActivityRole.Target] = target;

            // Effect: Walker is now near the target
            Effects[ActivityRole.Initiator] = new Dictionary<string, bool> { { "NearTarget", true } };
            // Optional: The Target is also near the Walker
            Effects[ActivityRole.Target] = new Dictionary<string, bool> { { "NearTarget", true } };
        }
    }
}
