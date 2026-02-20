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
            Console.WriteLine("=== INITIALIZING RASHOMON ENGINE TOWN ===\n");

            // 1. Setup the World
            World town = new World();
            
            // 2. Define More Locations
            Location home = new Location("Bob's Home", 0, 0);
            Location market = new Location("Market Square", 4, 4);
            Location blacksmith = new Location("Iron Hearth Forge", 5, 0);
            Location forest = new Location("Greenwood Forest", 0, 5);
            Location tavern = new Location("The Rusty Tankard", 3, 3);

            town.AddLocation(home);
            town.AddLocation(market);
            town.AddLocation(blacksmith);
            town.AddLocation(forest);
            town.AddLocation(tavern);

            // 3. Setup More NPCs
            NPC bob = new NPC("Bob", home.Position);
            NPC alice = new NPC("Alice", market.Position);
            NPC charlie = new NPC("Charlie", blacksmith.Position);
            NPC diana = new NPC("Diana", tavern.Position); // Diana is resting at the tavern

            town.AddNPC(bob);
            town.AddNPC(alice);
            town.AddNPC(charlie);
            town.AddNPC(diana);

            // 4. Register Activities (Bob still wants to chat with Alice!)
            town.AvailableActivities = new List<Activity>
            {
                new WalkToActivity(bob, alice),
                new ChatActivity(bob, alice)
            };

            // 5. Run the Simulation
            Console.WriteLine($"[TOWN MAP] {town.Locations.Count} locations and {town.NPCs.Count} NPCs ready.\n");
            Console.WriteLine("Bob's Goal: Socialized = true\n");

            SimplePlanner planner = new SimplePlanner();
            List<Activity> bobsPlan = planner.BuildPlan(bob, "Socialized", true, town.AvailableActivities);

            if (bobsPlan != null)
            {
                foreach (var action in bobsPlan)
                {
                    Console.WriteLine($"\n[STARTING] {action.Name}");
                    action.Initialize();
                    while (!action.IsFinished)
                    {
                        town.Tick();
                        action.OnTick(town.CurrentTick);
                        System.Threading.Thread.Sleep(100); 
                    }
                }
            }

            // 6. DUMP THE RASHOMON FILES!
            MemoryUtility.DumpMemories(town.NPCs);

            Console.WriteLine("\n=== SIMULATION COMPLETE ===");
            Console.WriteLine("Memories exported. Press Enter to exit.");
            Console.ReadLine();
        }
    }
}
