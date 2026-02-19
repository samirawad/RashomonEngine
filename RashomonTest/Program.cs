using System;
using System.Collections.Generic;
using System.Linq;

namespace GoapRpgPoC
{
    // --- 1. CORE ENUMS & DATA STRUCTURES ---
    public enum ActivityRole { Initiator, Target }

    public class Memory
    {
        public Activity PastActivity { get; private set; }
        public int Timestamp { get; private set; }

        public Memory(Activity activity, int time)
        {
            PastActivity = activity;
            Timestamp = time;
        }
    }

    public static class GlobalHistoryLog
    {
        public static List<Activity> History = new List<Activity>();

        public static void Record(Activity activity)
        {
            History.Add(activity);
        }
    }

    // --- 2. THE NPC (AGENT) ---
    public class NPC
    {
        public string Name { get; private set; }
        public Dictionary<string, bool> State { get; private set; } = new Dictionary<string, bool>();
        public List<Memory> Memories { get; private set; } = new List<Memory>();

        public NPC(string name)
        {
            Name = name;
        }

        public void SetState(string key, bool value) => State[key] = value;
        public bool GetState(string key) => State.ContainsKey(key) && State[key];

        public void Remember(Activity activity, int time)
        {
            Memories.Add(new Memory(activity, time));
        }
    }

    // --- 3. THE SHARED ACTIVITY BASE CLASS ---
    public abstract class Activity
    {
        public string Name { get; protected set; }
        public Dictionary<ActivityRole, NPC> Participants = new Dictionary<ActivityRole, NPC>();
        
        public Dictionary<ActivityRole, Dictionary<string, bool>> Preconditions = new Dictionary<ActivityRole, Dictionary<string, bool>>();
        public Dictionary<ActivityRole, Dictionary<string, bool>> Effects = new Dictionary<ActivityRole, Dictionary<string, bool>>();

        public bool CompleteActivity(int currentTime)
        {
            Console.WriteLine($"\n[ACTION] Executing: {Name}...");
            
            // Apply Effects to World State
            foreach (var role in Effects.Keys)
            {
                foreach (var effect in Effects[role])
                {
                    Participants[role].SetState(effect.Key, effect.Value);
                    Console.WriteLine($"   -> {Participants[role].Name}'s state '{effect.Key}' is now {effect.Value}");
                }
            }

            // Flyweight Memory & Logging
            GlobalHistoryLog.Record(this);
            foreach (var npc in Participants.Values)
            {
                npc.Remember(this, currentTime);
            }

            return true;
        }
    }

    // --- 4. CONCRETE ACTIVITIES ---
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

    // --- 5. THE PLANNER ---
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

    // --- 6. MAIN SIMULATION LOOP ---
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== INITIALIZING RPG WORLD ===\n");

            // Setup NPCs
            NPC bob = new NPC("Bob");
            bob.SetState("HasGold", true);
            bob.SetState("NearTarget", false);
            bob.SetState("HasApple", false);

            NPC alice = new NPC("Alice");
            alice.SetState("HasApple", true);

            // Give the world a list of possible activities
            List<Activity> worldActivities = new List<Activity>
            {
                new WalkToActivity(bob, alice),
                new TradeActivity(bob, alice)
            };

            // Bob's Goal: Get an Apple
            Console.WriteLine("Bob's Goal: HasApple = true\n");
            
            SimplePlanner planner = new SimplePlanner();
            List<Activity> bobsPlan = planner.BuildPlan(bob, "HasApple", true, worldActivities);

            if (bobsPlan != null)
            {
                Console.WriteLine("=== PLAN GENERATED ===");
                foreach (var step in bobsPlan)
                {
                    Console.WriteLine($"- {step.Name}");
                }

                // Execute the plan (Simulating game time)
                int gameClock = 100; 
                foreach (var action in bobsPlan)
                {
                    action.CompleteActivity(gameClock);
                    gameClock += 10; // Time passes
                }
            }
            else
            {
                Console.WriteLine("Bob could not find a valid plan.");
            }

            // --- QUERYING THE LOGS ---
            Console.WriteLine("\n=== GLOBAL HISTORY LOG ===");
            foreach (var act in GlobalHistoryLog.History)
            {
                Console.WriteLine($"Occurred: {act.Name}");
            }

            Console.WriteLine("\n=== ALICE'S MEMORIES ===");
            foreach (var mem in alice.Memories)
            {
                Console.WriteLine($"Timestamp {mem.Timestamp}: I remember participating in '{mem.PastActivity.Name}'");
            }
            
            Console.WriteLine("\n=== BOB'S CURRENT STATE ===");
            Console.WriteLine($"Has Apple: {bob.GetState("HasApple")}");
            Console.WriteLine($"Has Gold: {bob.GetState("HasGold")}");
            
            Console.ReadLine();
        }
    }
}