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
            Console.WriteLine("=== INITIALIZING UNIFORM CONTAINER WORLD ===\n");

            World town = new World();
            
            // 1. Create the NPCs
            NPC bob = new NPC("Bob", new Vector2(0, 0));
            NPC alice = new NPC("Alice", new Vector2(5, 5));
            town.AddNPC(bob);
            town.AddNPC(alice);

            // 2. Create an Inventory Item (An "Old Phone")
            Item phone = new Item("Rusty Old Phone");
            
            // The PHONE affords chatting!
            // (Notice: This chat can be remote!)
            Activity remoteChat = new ChatActivity(bob, alice); 
            phone.AddAffordance(remoteChat);

            // 3. The "PICKUP" Logic
            // Bob now "CONTAINS" the phone as a child
            bob.AddChild(phone);

            // 4. Run the Planner
            Console.WriteLine($"Bob is holding a {phone.Name}.");
            Console.WriteLine("Bob's Goal: Socialized = true\n");

            SimplePlanner planner = new SimplePlanner();
            List<Activity> bobsPlan = planner.BuildPlan(bob, "Socialized", true);

            if (bobsPlan != null)
            {
                Console.WriteLine("=== PLAN DISCOVERED THROUGH INVENTORY ===");
                foreach (var action in bobsPlan)
                {
                    Console.WriteLine($"- {action.Name}");
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
                Console.WriteLine("Bob couldn't find a way to socialize.");
            }

            MemoryUtility.DumpMemories(town.NPCs);
            Console.WriteLine("\n=== SIMULATION COMPLETE ===");
            Console.ReadLine();
        }
    }
}
