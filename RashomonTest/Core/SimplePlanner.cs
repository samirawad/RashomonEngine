using System.Collections.Generic;
using System.Linq;

namespace GoapRpgPoC.Core
{
    public class SimplePlanner
    {
        // A very basic recursive backward-chaining planner
        public List<Activity> BuildPlan(NPC plannerNPC, string goalState, bool goalValue, List<Activity> availableActivities)
        {
            List<Activity> plan = new List<Activity>();

            // Find an activity that satisfies the main goal
            Activity finalAction = availableActivities.FirstOrDefault(a => 
                a.Participants[ActivityRole.Initiator] == plannerNPC &&
                a.Effects.ContainsKey(ActivityRole.Initiator) &&
                a.Effects[ActivityRole.Initiator].ContainsKey(goalState) &&
                a.Effects[ActivityRole.Initiator][goalState] == goalValue);

            if (finalAction == null) return null; // No way to solve this!

            // Check if preconditions are met. If not, plan for them first!
            if (finalAction.Preconditions.ContainsKey(ActivityRole.Initiator))
            {
                foreach (var pre in finalAction.Preconditions[ActivityRole.Initiator])
                {
                    if (plannerNPC.GetState(pre.Key) != pre.Value)
                    {
                        // We need to satisfy this precondition first (Recursive call)
                        var subPlan = BuildPlan(plannerNPC, pre.Key, pre.Value, availableActivities);
                        if (subPlan != null) plan.AddRange(subPlan);
                    }
                }
            }

            plan.Add(finalAction);
            return plan;
        }
    }
}
