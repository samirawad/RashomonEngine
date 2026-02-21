using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GoapRpgPoC.Core
{
    public class SimplePlanner
    {
        private StringBuilder _trace = new StringBuilder();

        public List<Activity> BuildPlan(NPC plannerNPC, string goalState, bool goalValue)
        {
            _trace.Clear();
            _trace.AppendLine($"[PLAN SEARCH] Goal: {goalState}={goalValue}");

            List<Activity> templates = new List<Activity>();
            foreach (var relation in plannerNPC.Relationships.Values) templates.AddRange(relation.Affordances);
            foreach (var child in plannerNPC.Children) templates.AddRange(child.Affordances);
            templates.AddRange(plannerNPC.Affordances);

            var plan = BuildRecursivePlan(plannerNPC, goalState, goalValue, templates.Distinct().ToList(), 1);
            plannerNPC.LogDebug(_trace.ToString().Trim());
            
            return plan;
        }

        private List<Activity> BuildRecursivePlan(NPC plannerNPC, string goalState, bool goalValue, List<Activity> templates, int depth)
        {
            string indent = new string(' ', depth * 3);
            List<Activity> plan = new List<Activity>();

            // 1. DISCOVERY: Find a template that matches
            Activity template = templates.FirstOrDefault(a => 
                a.Effects.ContainsKey(ActivityRole.Initiator) &&
                a.Effects[ActivityRole.Initiator].ContainsKey(goalState) &&
                a.Effects[ActivityRole.Initiator][goalState] == goalValue);

            if (template == null)
            {
                _trace.AppendLine($"{indent}[FAIL] No template satisfies {goalState}={goalValue}");
                return null;
            }

            // 2. INSTANTIATION: Clone and bind the roles
            Activity action = template.Clone();
            
            // Determine the target (If the affordance came from an external relationship entity)
            NPC target = null;
            var ownerEntity = plannerNPC.Relationships.Values.FirstOrDefault(e => e.Affordances.Contains(template));
            if (ownerEntity is NPC npcTarget) target = npcTarget;

            action.Bind(plannerNPC, target);
            _trace.AppendLine($"{indent}[MATCH] Created bound instance of {action.Name}");

            // 3. RECURSIVE VERIFICATION
            foreach (var role in action.Preconditions.Keys)
            {
                var participant = action.Participants[role];
                foreach (var pre in action.Preconditions[role])
                {
                    if (participant.GetState(pre.Key) != pre.Value)
                    {
                        if (role == ActivityRole.Initiator)
                        {
                            _trace.AppendLine($"{indent}   [NEED] My precondition {pre.Key}={pre.Value} not met. Sub-planning...");
                            var subPlan = BuildRecursivePlan(plannerNPC, pre.Key, pre.Value, templates, depth + 1);
                            if (subPlan != null) plan.AddRange(subPlan);
                            else return null;
                        }
                        else
                        {
                            _trace.AppendLine($"{indent}   [FAIL] Target {participant.Name} doesn't meet {pre.Key}={pre.Value}");
                            return null;
                        }
                    }
                }
            }

            foreach (var role in action.PreconditionTags.Keys)
            {
                var participant = action.Participants[role];
                foreach (var tag in action.PreconditionTags[role])
                {
                    if (!participant.HasTag(tag))
                    {
                        if (role == ActivityRole.Initiator)
                        {
                            _trace.AppendLine($"{indent}   [NEED] Missing tag '{tag}'. Sub-planning...");
                            var subPlan = BuildRecursivePlan(plannerNPC, tag, true, templates, depth + 1);
                            if (subPlan != null) plan.AddRange(subPlan);
                            else return null;
                        }
                        else
                        {
                            _trace.AppendLine($"{indent}   [FAIL] Target {participant.Name} missing tag '{tag}'");
                            return null;
                        }
                    }
                }
            }

            plan.Add(action);
            return plan;
        }
    }
}
