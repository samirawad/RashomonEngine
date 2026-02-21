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

            // Gather EVERY potential template from the NPC's world-view
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

            // 1. Find all templates that satisfy the effect
            var potentialMatches = templates.Where(a => 
                a.Effects.ContainsKey(ActivityRole.Initiator) &&
                a.Effects[ActivityRole.Initiator].ContainsKey(goalState) &&
                a.Effects[ActivityRole.Initiator][goalState] == goalValue);

            foreach (var template in potentialMatches)
            {
                // 2. CAPABILITY CHECK: Can I physically do this?
                if (!string.IsNullOrEmpty(template.RequiredCapability) && !plannerNPC.HasTag(template.RequiredCapability))
                {
                    _trace.AppendLine($"{indent}[SKIP] I lack capability '{template.RequiredCapability}' for {template.GetType().Name}");
                    continue;
                }

                // 3. BINDING & RECURSION
                Activity action = template.Clone();
                NPC target = null;
                var ownerEntity = plannerNPC.Relationships.Values.FirstOrDefault(e => e.Affordances.Contains(template));
                if (ownerEntity is NPC npcTarget) target = npcTarget;

                action.Bind(plannerNPC, target);
                _trace.AppendLine($"{indent}[MATCH] Considering {action.Name}");

                bool subPlanSuccess = true;

                // Check States
                foreach (var role in action.Preconditions.Keys)
                {
                    var participant = action.Participants[role];
                    foreach (var pre in action.Preconditions[role])
                    {
                        if (participant.GetState(pre.Key) != pre.Value)
                        {
                            if (role == ActivityRole.Initiator)
                            {
                                _trace.AppendLine($"{indent}   [NEED] {pre.Key}={pre.Value}. Sub-planning...");
                                var subPlan = BuildRecursivePlan(plannerNPC, pre.Key, pre.Value, templates, depth + 1);
                                if (subPlan != null) plan.AddRange(subPlan);
                                else { subPlanSuccess = false; break; }
                            }
                            else
                            {
                                _trace.AppendLine($"{indent}   [FAIL] {participant.Name} missing state {pre.Key}");
                                subPlanSuccess = false; break;
                            }
                        }
                    }
                    if (!subPlanSuccess) break;
                }

                if (!subPlanSuccess) continue;

                // Check Tags
                foreach (var role in action.PreconditionTags.Keys)
                {
                    var participant = action.Participants[role];
                    foreach (var tag in action.PreconditionTags[role])
                    {
                        if (!participant.HasTag(tag))
                        {
                            if (role == ActivityRole.Initiator)
                            {
                                _trace.AppendLine($"{indent}   [NEED] Tag '{tag}'. Sub-planning...");
                                var subPlan = BuildRecursivePlan(plannerNPC, tag, true, templates, depth + 1);
                                if (subPlan != null) plan.AddRange(subPlan);
                                else { subPlanSuccess = false; break; }
                            }
                            else
                            {
                                _trace.AppendLine($"{indent}   [FAIL] {participant.Name} missing tag '{tag}'");
                                subPlanSuccess = false; break;
                            }
                        }
                    }
                    if (!subPlanSuccess) break;
                }

                if (subPlanSuccess)
                {
                    plan.Add(action);
                    return plan;
                }
            }

            _trace.AppendLine($"{indent}[FAIL] No valid path for {goalState}={goalValue}");
            return null;
        }
    }
}
