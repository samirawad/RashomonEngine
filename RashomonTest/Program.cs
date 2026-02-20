using System;
using System.Collections.Generic;
using System.Linq;
using GoapRpgPoC.Core;
using GoapRpgPoC.Activities;

namespace GoapRpgPoC
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== INITIALIZING FULL TOWN SIMULATION ===\n");

            World town = new World();
            
            // 1. LOCATIONS
            Location bobsHome = new Location("Bob's Cabin", 0, 0);
            Location alicesHome = new Location("Alice's Cottage", 5, 5);
            Location blacksmith = new Location("Iron Hearth Forge", 5, 0);
            Location tavern = new Location("The Rusty Tankard", 2, 2);
            
            town.AddLocation(bobsHome);
            town.AddLocation(alicesHome);
            town.AddLocation(blacksmith);
            town.AddLocation(tavern);

            // 2. CREATE NPCs
            NPC bob = CreateStandardNPC("Bob", bobsHome.Position);
            NPC alice = CreateStandardNPC("Alice", alicesHome.Position);
            NPC charlie = CreateStandardNPC("Charlie", blacksmith.Position);
            NPC diana = CreateStandardNPC("Diana", tavern.Position);

            town.AddNPC(bob);
            town.AddNPC(alice);
            town.AddNPC(charlie);
            town.AddNPC(diana);

            // 3. SET RELATIONSHIPS & KNOWLEDGE
            bob.SetRelationship("Friend", alice);
            bob.SetRelationship("LocalTavern", tavern);
            alice.SetRelationship("LocalTavern", tavern);
            charlie.SetRelationship("LocalTavern", tavern);
            diana.SetRelationship("Workplace", tavern);

            // 4. GIVE STARTING ITEMS
            Item bobsGold = new Item("Bob's Gold");
            bobsGold.SetState("HasGold", true);
            bob.AddChild(bobsGold);

            Item alicesApple = new Item("Alice's Apple");
            alicesApple.AddTag("Edible");
            alice.AddChild(alicesApple);

            // 5. ATTACH AFFORDANCES
            SetupStandardAffordances(bob, town);
            SetupStandardAffordances(alice, town);
            SetupStandardAffordances(charlie, town);
            SetupStandardAffordances(diana, town);

            // Specialized activities
            var bobFeet = bob.Children.Find(c => c.Name == "Feet");
            var bobHand = bob.Children.Find(c => c.Name == "Hands");
            bobFeet.AddAffordance(new WalkToActivity(bob, alice));
            bobHand.AddAffordance(new TradeActivity(bob, alice));

            // 6. RUN THE SIMULATION
            Console.WriteLine("Town simulation running... NPCs will wait for needs to trigger plans.\n");

            SimplePlanner planner = new SimplePlanner();
            int maxTicks = 50;

            while (town.CurrentTick < maxTicks)
            {
                town.Tick();

                foreach (var npc in town.NPCs)
                {
                    // Check needs and generate plans if necessary
                    if (npc.GetState("IsHungry"))
                    {
                        var plan = planner.BuildPlan(npc, "IsHungry", false);
                        ExecutePlan(npc, plan, town);
                    }
                    else if (npc.GetState("IsTired"))
                    {
                        var plan = planner.BuildPlan(npc, "IsTired", false);
                        ExecutePlan(npc, plan, town);
                    }
                    else if (npc.GetState("IsLonely"))
                    {
                        var plan = planner.BuildPlan(npc, "IsLonely", false);
                        ExecutePlan(npc, plan, town);
                    }
                }

                System.Threading.Thread.Sleep(100);
            }

            MemoryUtility.DumpMemories(town.NPCs);
            Console.WriteLine("\n=== SIMULATION COMPLETE ===");
            Console.ReadLine();
        }

        static NPC CreateStandardNPC(string name, Vector2 startPos)
        {
            NPC npc = new NPC(name, startPos);
            
            // Body
            npc.AddChild(new BodyPartEntity("Hands"));
            npc.AddChild(new BodyPartEntity("Feet"));
            
            // Needs (different rates for variation)
            npc.AddChild(new NeedEntity("Stomach", 5, "IsHungry"));
            npc.AddChild(new NeedEntity("Energy", 10, "IsTired"));
            npc.AddChild(new NeedEntity("Heart", 15, "IsLonely"));

            return npc;
        }

        static void SetupStandardAffordances(NPC npc, World town)
        {
            // Self-affordances
            npc.AddAffordance(new EatActivity(npc));
            npc.AddAffordance(new SleepActivity(npc));
            
            // Note: Socialization usually requires a target, 
            // in a real system we'd discover this target dynamically.
        }

        static void ExecutePlan(NPC npc, List<Activity> plan, World town)
        {
            if (plan == null) return;

            npc.LogDebug($"[EXECUTE] Starting plan with {plan.Count} steps.");
            foreach (var action in plan)
            {
                action.Initialize();
                while (!action.IsFinished)
                {
                    town.Tick();
                    foreach (var n in town.NPCs) n.Tick(town.CurrentTick);
                    action.OnTick(town.CurrentTick);
                }
            }
        }
    }
}
