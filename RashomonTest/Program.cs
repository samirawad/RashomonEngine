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
            Console.WriteLine("=== INITIALIZING VOCABULARY-BASED TOWN v2.6 ===\n");

            World town = new World();
            
            // 1. LOCATIONS
            Location bobsHome = new Location("Bob's Cabin", 0, 0);
            Location alicesHome = new Location("Alice's Cottage", 5, 5);
            Location orchard = new Location("Greenwood Orchard", 8, 8);
            Location tavern = new Location("The Rusty Tankard", 2, 2);
            
            town.AddLocation(bobsHome);
            town.AddLocation(alicesHome);
            town.AddLocation(orchard);
            town.AddLocation(tavern);

            // 2. CREATE NPCs
            NPC bob = CreateNPC("Bob", bobsHome.Position);
            NPC alice = CreateNPC("Alice", alicesHome.Position);
            town.AddNPC(bob);
            town.AddNPC(alice);

            // 3. SET RELATIONSHIPS
            bob.SetRelationship(Relations.Home, bobsHome);
            bob.SetRelationship(Relations.Friend, alice);
            bob.SetRelationship(Relations.LocalTavern, tavern);

            alice.SetRelationship(Relations.Home, alicesHome);
            alice.SetRelationship(Relations.Work, orchard);
            alice.SetRelationship(Relations.LocalTavern, tavern);

            // 4. ATTACH AFFORDANCES
            bobsHome.AddAffordance(new WalkToActivity(bobsHome));
            alice.AddAffordance(new WalkToActivity(alice));
            alicesHome.AddAffordance(new WalkToActivity(alicesHome));
            orchard.AddAffordance(new WalkToActivity(orchard));

            alice.AddAffordance(new TradeActivity());
            bobsHome.AddAffordance(new SleepActivity());
            alicesHome.AddAffordance(new SleepActivity());
            orchard.AddAffordance(new HarvestActivity(orchard));

            // Biological tools afford their own verbs
            foreach (var npc in town.NPCs)
            {
                npc.Children.Find(c => c.Name == "Mouth")?.AddAffordance(new EatActivity());
            }

            // 5. STARTING ITEMS
            Item gold = new Item("Gold Pouch");
            gold.SetState(States.HasGold, true);
            bob.AddChild(gold);

            // 6. RUN
            town.Run(100, 50);

            MemoryUtility.DumpMemories(town.NPCs);
            Console.WriteLine("\n=== SIMULATION COMPLETE ===");
            Console.ReadLine();
        }

        static NPC CreateNPC(string name, Vector2 startPos)
        {
            NPC npc = new NPC(name, startPos);
            
            var hands = new BodyPartEntity("Hands");
            hands.AddTag(Tags.Hands);
            
            var feet = new BodyPartEntity("Feet");
            feet.AddTag(Tags.Feet);
            
            var mouth = new BodyPartEntity("Mouth");
            mouth.AddTag(Tags.Mouth);
            
            npc.AddChild(hands);
            npc.AddChild(feet);
            npc.AddChild(mouth);
            
            npc.AddChild(new NeedEntity("Stomach", 5, States.Hungry));
            npc.AddChild(new NeedEntity("Energy", 15, States.Tired));
            npc.AddChild(new NeedEntity("Heart", 25, States.Lonely));
            return npc;
        }
    }
}
