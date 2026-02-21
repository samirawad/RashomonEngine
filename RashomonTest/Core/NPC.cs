using System.Collections.Generic;
using System.Linq;

namespace GoapRpgPoC.Core
{
    public class NPC : Entity
    {
        public Dictionary<string, Entity> Relationships { get; private set; } = new Dictionary<string, Entity>();
        public List<Memory> Memories { get; private set; } = new List<Memory>();
        public List<string> DebugLog { get; private set; } = new List<string>();
        
        private string _lastLoggedMessage = "";
        private int _repeatCount = 1;

        public Queue<Activity> PlanQueue { get; private set; } = new Queue<Activity>();
        private int _totalPlanSteps = 0;
        private SimplePlanner _planner = new SimplePlanner();

        // Social Coordination
        public List<Invitation> IncomingInvitations { get; private set; } = new List<Invitation>();
        public Activity? SubscribedScene { get; private set; } = null;

        public NPC(string name, Vector2 startingPosition) : base(name, startingPosition) { }

        public void SetRelationship(string type, Entity entity) => Relationships[type] = entity;
        public Entity? GetRelationship(string type) => Relationships.ContainsKey(type) ? Relationships[type] : null;

        public void Remember(Activity activity, int time) => Memories.Add(new Memory(activity, time));

        public void LogDebug(string message)
        {
            string trimmedMsg = message.Trim();
            if (DebugLog.Count > 0 && trimmedMsg == _lastLoggedMessage)
            {
                _repeatCount++;
                if (trimmedMsg.Contains("\n")) DebugLog[DebugLog.Count - 1] = $"[REPEAT x{_repeatCount}]\n{trimmedMsg}";
                else DebugLog[DebugLog.Count - 1] = $"{trimmedMsg} (x{_repeatCount})";
            }
            else
            {
                DebugLog.Add(trimmedMsg);
                _lastLoggedMessage = trimmedMsg;
                _repeatCount = 1;
            }
        }

        // --- NEW: THE PERCEPTION HEARTBEAT ---
        public void Perceive()
        {
            // 1. Clear all external perceptual states
            // We find all keys starting with "Near(" or matching "AtHome"
            var perceptualKeys = State.Keys.Where(k => k.StartsWith("Near(") || k == States.AtHome).ToList();
            foreach (var key in perceptualKeys)
            {
                State[key] = false;
            }

            // 2. Scan relationships to update proximity
            foreach (var relation in Relationships)
            {
                var entity = relation.Value;
                if (Vector2.Distance(this.Position, entity.Position) == 0)
                {
                    // I am physically at this entity's location
                    SetState($"Near({entity.Name})", true);

                    // If this is my Home relationship, set AtHome
                    if (relation.Key == Relations.Home)
                    {
                        SetState(States.AtHome, true);
                    }
                }
            }
            
            // 3. Scan nearby NPCs who might not be in my relationships
            // (In a full engine, we'd loop through world.NPCs here)
        }

        // --- THE BRAIN UPDATE CYCLE ---
        public void Update(int currentTick)
        {
            // 0. REFRESH REALITY
            Perceive();

            // 1. EVALUATE INVITATIONS
            if (IncomingInvitations.Count > 0)
            {
                EvaluateInvitations();
            }

            // 2. PLANNING (If idle and not in a shared scene)
            if (PlanQueue.Count == 0 && SubscribedScene == null)
            {
                CheckNeedsAndPlan();
            }

            // 3. EXECUTION
            if (SubscribedScene != null)
            {
                LogDebug($"[BRAIN] Subscribed to Shared Scene: {SubscribedScene.Name}");
                if (SubscribedScene.IsFinished)
                {
                    SubscribedScene = null;
                    LogDebug("[BRAIN] Shared Scene finished. Resuming autonomy.");
                }
            }
            else if (PlanQueue.Count > 0)
            {
                var currentActivity = PlanQueue.Peek();
                int currentStep = _totalPlanSteps - PlanQueue.Count + 1;
                LogDebug($"[BRAIN] Executing Step {currentStep}/{_totalPlanSteps}: {currentActivity.Name}");

                currentActivity.OnTick(currentTick);

                if (currentActivity.IsFinished)
                {
                    PlanQueue.Dequeue();
                    if (!currentActivity.WasSuccessful)
                    {
                        LogDebug($"[BRAIN] Step {currentStep} FAILED. Aborting plan.");
                        PlanQueue.Clear();
                        _totalPlanSteps = 0;
                    }
                }
            }

            IncomingInvitations.Clear();
        }

        public void ReceiveInvitation(Invitation invite)
        {
            IncomingInvitations.Add(invite);
        }

        private void EvaluateInvitations()
        {
            foreach (var invite in IncomingInvitations)
            {
                var payoff = invite.GetPayoff();
                bool providesValue = false;
                foreach (var effect in payoff)
                {
                    if (GetState(effect.Key) != effect.Value)
                    {
                        providesValue = true;
                        break;
                    }
                }

                if (providesValue)
                {
                    LogDebug($"[BRAIN] Accepted invitation from {invite.Host.Name} to join {invite.Scene.Name}");
                    PlanQueue.Clear();
                    _totalPlanSteps = 0;
                    SubscribedScene = invite.Scene;
                    return; 
                }
            }
        }

        private void CheckNeedsAndPlan()
        {
            if (GetState(States.Hungry)) GeneratePlan(States.Hungry, false);
            else if (GetState(States.Tired)) GeneratePlan(States.Tired, false);
            else if (GetState(States.Lonely)) GeneratePlan(States.Lonely, false);
        }

        private void GeneratePlan(string goalState, bool goalValue)
        {
            var plan = _planner.BuildPlan(this, goalState, goalValue);
            if (plan != null)
            {
                _totalPlanSteps = plan.Count;
                foreach (var action in plan)
                {
                    action.Initialize();
                    PlanQueue.Enqueue(action);
                }
                LogDebug($"[BRAIN] New Plan: {string.Join(" -> ", plan.Select(a => a.Name))}");
            }
        }
    }
}
