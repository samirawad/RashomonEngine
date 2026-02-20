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
            
            // 1. Create Bob
            NPC bob = new NPC("Bob", new Vector2(0, 0));
            town.AddNPC(bob);

            // 2. Build Bob's Body & Needs
            bob.AddChild(new BodyPartEntity("Feet"));
            bob.AddChild(new BodyPartEntity("Right Hand"));
            bob.AddChild(new NeedEntity("Stomach", 2, "IsHungry"));

            // 3. Give Bob a PHYSICAL Gold Pouch
            Item gold = new Item("Pouch of Gold");
            gold.SetState("HasGold", true);
            bob.AddChild(gold);

            // 4. Create Alice and her PHYSICAL Apple
            NPC alice = new NPC("Alice", new Vector2(3, 3));
            town.AddNPC(alice);

            Item apple = new Item("Juicy Red Apple");
            apple.AddTag("Edible"); // Now a tag, not a state!
            alice.AddChild(apple);

            // 5. ATTACH CAPABILITIES
            // The FEET child provides the walking affordance
            var feet = bob.Children.Find(c => c.Name == "Feet");
            feet.AddAffordance(new WalkToActivity(bob, alice));

            // The HAND child provides the trading affordance
            var rightHand = bob.Children.Find(c => c.Name == "Right Hand");
            rightHand.AddAffordance(new TradeActivity(bob, alice));
            
            // Bob himself knows how to eat! (Self-affordance)
            bob.AddAffordance(new EatActivity(bob));

            // 6. RUN THE SIMULATION
            Console.WriteLine("Bob is standing still. Alice is holding an apple.");
            Console.WriteLine("Wait for Bob to get hungry...\n");

            SimplePlanner planner = new SimplePlanner();
            List<Activity> bobsPlan = null;

            while (bobsPlan == null && town.CurrentTick < 20)
            {
                town.Tick();
                bob.Tick(town.CurrentTick);

                if (bob.GetState("IsHungry"))
                {
                    Console.WriteLine("\n[GOAL] Bob is hungry! Finding a way to satisfy it...");
                    bobsPlan = planner.BuildPlan(bob, "IsHungry", false);
                }
                
                System.Threading.Thread.Sleep(200);
            }

            if (bobsPlan != null)
            {
                foreach (var action in bobsPlan)
                {
                    action.Initialize();
                    while (!action.IsFinished)
                    {
                        town.Tick();
                        bob.Tick(town.CurrentTick);
                        action.OnTick(town.CurrentTick);
                    }
                }
            }

            // 7. FINAL RASHOMON INSPECTION!
            MemoryUtility.DumpMemories(town.NPCs);

            Console.WriteLine("\n=== SIMULATION COMPLETE ===");
            Console.WriteLine($"Does Bob have an apple? {bob.GetState("Edible")}");
            Console.WriteLine($"Is Bob still hungry? {bob.GetState("IsHungry")}");
            Console.ReadLine();
        }
    }
}
