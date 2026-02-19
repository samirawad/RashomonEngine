using System;
using System.Collections.Generic;
using GoapRpgPoC.Core;
using GoapRpgPoC.Activities;

namespace GoapRpgPoC
{
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
