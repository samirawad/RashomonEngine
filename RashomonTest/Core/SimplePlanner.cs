using System.Collections.Generic;
using System.Linq;

namespace GoapRpgPoC.Core
{
    public class SimplePlanner
    {
        public List<Activity> BuildPlan(NPC plannerNPC, string goalState, bool goalValue)
        {
            // DISCOVERY: Gather all activities the NPC "knows" about through relationships
            List<Activity> knownActivities = new List<Activity>();
            
            foreach (var relation in plannerNPC.Relationships.Values)
            {
                knownActivities.AddRange(relation.Affordances);
            }

            // Also include self-affordances (if the NPC can do something independently)
            knownActivities.AddRange(plannerNPC.Affordances);

            return BuildRecursivePlan(plannerNPC, goalState, goalValue, knownActivities.Distinct().ToList());
        }

        private List<Activity> BuildRecursivePlan(NPC plannerNPC, string goalState, bool goalValue, List<Activity> availableActivities)
        {
            List<Activity> plan = new List<Activity>();

            // Find an activity that satisfies the goal
            Activity finalAction = availableActivities.FirstOrDefault(a => 
                a.Effects.ContainsKey(ActivityRole.Initiator) &&
                a.Effects[ActivityRole.Initiator].ContainsKey(goalState) &&
                a.Effects[ActivityRole.Initiator][goalState] == goalValue);

            if (finalAction == null) return null;

            // Check preconditions recursively
            if (finalAction.Preconditions.ContainsKey(ActivityRole.Initiator))
            {
                foreach (var pre in finalAction.Preconditions[ActivityRole.Initiator])
                {
                    if (plannerNPC.GetState(pre.Key) != pre.Value)
                    {
                        var subPlan = BuildRecursivePlan(plannerNPC, pre.Key, pre.Value, availableActivities);
                        if (subPlan != null) plan.AddRange(subPlan);
                        else return null; // Precondition impossible to meet
                    }
                }
            }

            plan.Add(finalAction);
            return plan;
        }
    }
}
