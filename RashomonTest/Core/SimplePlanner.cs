using System.Collections.Generic;
using System.Linq;

namespace GoapRpgPoC.Core
{
    public class SimplePlanner
    {
        public List<Activity> BuildPlan(NPC plannerNPC, string goalState, bool goalValue)
        {
            plannerNPC.LogDebug($"[PLAN] Starting search for {goalState}={goalValue}");

            List<Activity> knownActivities = new List<Activity>();
            foreach (var relation in plannerNPC.Relationships.Values) knownActivities.AddRange(relation.Affordances);
            foreach (var child in plannerNPC.Children) knownActivities.AddRange(child.Affordances);
            knownActivities.AddRange(plannerNPC.Affordances);

            return BuildRecursivePlan(plannerNPC, goalState, goalValue, knownActivities.Distinct().ToList());
        }

        private List<Activity> BuildRecursivePlan(NPC plannerNPC, string goalState, bool goalValue, List<Activity> availableActivities)
        {
            List<Activity> plan = new List<Activity>();

            Activity finalAction = availableActivities.FirstOrDefault(a => 
                a.Effects.ContainsKey(ActivityRole.Initiator) &&
                a.Effects[ActivityRole.Initiator].ContainsKey(goalState) &&
                a.Effects[ActivityRole.Initiator][goalState] == goalValue);

            if (finalAction == null)
            {
                plannerNPC.LogDebug($"   [PLAN FAIL] No activity found to satisfy {goalState}={goalValue}");
                return null;
            }

            plannerNPC.LogDebug($"   [PLAN] Considering {finalAction.Name} to satisfy {goalState}={goalValue}");

            // --- 1. CHECK STATE PRECONDITIONS ---
            if (finalAction.Preconditions.ContainsKey(ActivityRole.Initiator))
            {
                foreach (var pre in finalAction.Preconditions[ActivityRole.Initiator])
                {
                    if (plannerNPC.GetState(pre.Key) != pre.Value)
                    {
                        plannerNPC.LogDebug($"      [PLAN] Precondition {pre.Key}={pre.Value} not met. Planning sub-task...");
                        var subPlan = BuildRecursivePlan(plannerNPC, pre.Key, pre.Value, availableActivities);
                        if (subPlan != null) plan.AddRange(subPlan);
                        else return null;
                    }
                }
            }

            // --- 2. CHECK TAG PRECONDITIONS ---
            if (finalAction.PreconditionTags.ContainsKey(ActivityRole.Initiator))
            {
                foreach (var tag in finalAction.PreconditionTags[ActivityRole.Initiator])
                {
                    if (!plannerNPC.HasTag(tag))
                    {
                        plannerNPC.LogDebug($"      [PLAN] Missing required tag '{tag}'. Planning sub-task...");
                        var subPlan = BuildRecursivePlan(plannerNPC, tag, true, availableActivities);
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
