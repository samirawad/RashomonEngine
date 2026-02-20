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
            Console.WriteLine("=== INITIALIZING FULLY COMPOSED RASHOMON TOWN ===\n");

            World town = new World();
            
            // 1. Create Bob (The Hub)
            NPC bob = new NPC("Bob", new Vector2(0, 0));
            town.AddNPC(bob);

            // 2. BUILD BOB'S BODY
            BodyPartEntity leftHand = new BodyPartEntity("Left Hand");
            BodyPartEntity rightHand = new BodyPartEntity("Right Hand");
            BodyPartEntity feet = new BodyPartEntity("Feet");
            
            bob.AddChild(leftHand);
            bob.AddChild(rightHand);
            bob.AddChild(feet);

            // 3. BUILD BOB'S NEEDS
            NeedEntity stomach = new NeedEntity("Stomach", 2, "IsHungry");
            bob.AddChild(stomach);

            // 4. GIVE BOB AN ITEM
            Item goldPouch = new Item("Pouch of Gold");
            goldPouch.SetState("HasGold", true);
            bob.AddChild(goldPouch);

            // 5. ATTACH CAPABILITIES (Affordances)
            // Note: Walking is a capability of the FEET!
            NPC alice = new NPC("Alice", new Vector2(3, 3));
            alice.SetState("HasApple", true); // Alice has the goods!
            town.AddNPC(alice);
            
            feet.AddAffordance(new WalkToActivity(bob, alice));
            rightHand.AddAffordance(new TradeActivity(bob, alice));

            // 6. RUN THE SIMULATION (Wait for Bob to get hungry!)
            Console.WriteLine("Bob is standing still. Waiting for his stomach to tick...\n");

            SimplePlanner planner = new SimplePlanner();
            List<Activity> bobsPlan = null;

            // Heartbeat loop: Bob will eventually get hungry and plan!
            while (bobsPlan == null && town.CurrentTick < 20)
            {
                town.Tick();
                bob.Tick(town.CurrentTick); // This makes Bob's needs grow!

                // Check if Bob is hungry and needs a plan
                if (bob.GetState("IsHungry"))
                {
                    Console.WriteLine("\n[GOAL] Bob realized he is hungry! Planning to trade for an apple...");
                    bobsPlan = planner.BuildPlan(bob, "HasApple", true);
                }
                
                System.Threading.Thread.Sleep(200);
            }

            // 7. EXECUTE THE DISCOVERED PLAN
            if (bobsPlan != null)
            {
                foreach (var action in bobsPlan)
                {
                    action.Initialize();
                    while (!action.IsFinished)
                    {
                        town.Tick();
                        bob.Tick(town.CurrentTick); // Bob still gets hungrier while walking!
                        action.OnTick(town.CurrentTick);
                    }
                }
            }

            MemoryUtility.DumpMemories(town.NPCs);
            Console.WriteLine("\n=== SIMULATION COMPLETE ===");
            Console.ReadLine();
        }
    }
}
