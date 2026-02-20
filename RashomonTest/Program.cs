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
            Console.WriteLine("=== INITIALIZING RELATIONSHIP-BASED RASHOMON TOWN ===\n");

            World town = new World();
            
            // 1. Create Locations
            Location tavern = new Location("The Rusty Tankard", 3, 3);
            town.AddLocation(tavern);

            // 2. Create NPCs
            NPC bob = new NPC("Bob", new Vector2(0, 0));
            NPC alice = new NPC("Alice", tavern.Position);
            town.AddNPC(bob);
            town.AddNPC(alice);

            // 3. Setup Relationships & Affordances
            // Alice "Affords" a Chat activity
            Activity chatWithAlice = new ChatActivity(bob, alice);
            Activity walkToAlice = new WalkToActivity(bob, alice);
            
            alice.AddAffordance(chatWithAlice);
            alice.AddAffordance(walkToAlice);

            // Bob knows Alice! This is the "Knowledge Link"
            bob.SetRelationship("Friend", alice);

            // 4. Run the Discovery-Based Planner
            Console.WriteLine($"Bob is at {bob.Position}. He knows Alice is at {alice.Position}.");
            Console.WriteLine("Bob's Goal: Socialized = true (He will search his relationships...)\n");

            SimplePlanner planner = new SimplePlanner();
            List<Activity> bobsPlan = planner.BuildPlan(bob, "Socialized", true);

            if (bobsPlan != null)
            {
                Console.WriteLine("=== DISCOVERY PLAN GENERATED ===");
                foreach (var action in bobsPlan)
                {
                    Console.WriteLine($"- {action.Name}");
                }

                foreach (var action in bobsPlan)
                {
                    action.Initialize();
                    while (!action.IsFinished)
                    {
                        town.Tick();
                        action.OnTick(town.CurrentTick);
                    }
                }
            }
            else
            {
                Console.WriteLine("Bob doesn't know anyone who affords socializing!");
            }

            MemoryUtility.DumpMemories(town.NPCs);
            Console.WriteLine("\n=== SIMULATION COMPLETE ===");
            Console.ReadLine();
        }
    }
}
