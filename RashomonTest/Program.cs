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

            World town = new World();
            Location home = new Location("Bob's Home", 0, 0);
            Location tavern = new Location("The Rusty Tankard", 3, 3);
            town.AddLocation(home);
            town.AddLocation(tavern);

            NPC bob = new NPC("Bob", home.Position);
            NPC alice = new NPC("Alice", tavern.Position); // Alice is waiting at the tavern

            town.AddNPC(bob);
            town.AddNPC(alice);

            // New Goal: Bob wants to be socialized
            town.AvailableActivities = new List<Activity>
            {
                new WalkToActivity(bob, alice),
                new ChatActivity(bob, alice)
            };

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
                        System.Threading.Thread.Sleep(200); 
                    }
                }
            }

            Console.WriteLine("\n=== SIMULATION COMPLETE ===");
            Console.WriteLine($"Bob is socialized: {bob.GetState("Socialized")}");
            Console.WriteLine($"Alice is socialized: {alice.GetState("Socialized")}");
            
            Console.ReadLine();
        }
    }
}
