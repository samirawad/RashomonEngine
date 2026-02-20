using System.Collections.Generic;
using System.Linq;

namespace GoapRpgPoC.Core
{
    public class SimplePlanner
    {
        public List<Activity> BuildPlan(NPC plannerNPC, string goalState, bool goalValue)
        {
            // --- UNIFIED DISCOVERY ---
            List<Activity> knownActivities = new List<Activity>();
            
            // 1. Scan External Relationships (Friends, Locations)
            foreach (var relation in plannerNPC.Relationships.Values)
            {
                knownActivities.AddRange(relation.Affordances);
            }

            // 2. Scan Internal Hierarchy (Body Parts, Inventory, Needs)
            foreach (var child in plannerNPC.Children)
            {
                knownActivities.AddRange(child.Affordances);
            }

            // 3. Scan Self-Affordances (Internal logic)
            knownActivities.AddRange(plannerNPC.Affordances);

            return BuildRecursivePlan(plannerNPC, goalState, goalValue, knownActivities.Distinct().ToList());
        }

        private List<Activity> BuildRecursivePlan(NPC plannerNPC, string goalState, bool goalValue, List<Activity> availableActivities)
        {
            List<Activity> plan = new List<Activity>();

            // The goal state could be on the NPC itself, OR one of its children (e.g., Stomach needs to be "Full")
            // For now, we search for activities that satisfy the goal for the Initiator
            Activity finalAction = availableActivities.FirstOrDefault(a => 
                a.Effects.ContainsKey(ActivityRole.Initiator) &&
                a.Effects[ActivityRole.Initiator].ContainsKey(goalState) &&
                a.Effects[ActivityRole.Initiator][goalState] == goalValue);

            if (finalAction == null) return null;

            if (finalAction.Preconditions.ContainsKey(ActivityRole.Initiator))
            {
                foreach (var pre in finalAction.Preconditions[ActivityRole.Initiator])
                {
                    // Check if NPC meets the precondition
                    if (plannerNPC.GetState(pre.Key) != pre.Value)
                    {
                        var subPlan = BuildRecursivePlan(plannerNPC, pre.Key, pre.Value, availableActivities);
                        if (subPlan != null) plan.AddRange(subPlan);
                        else return null;
                    }
                }
            }

            plan.Add(finalAction);
            return plan;
        }
    }
}
