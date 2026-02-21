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
            Console.WriteLine("=== INITIALIZING CAPABILITY-AFFORDANCE BRIDGE v2.5 ===\n");

            World town = new World();
            
            // 1. LOCATIONS
            Location bobsHome = new Location("Bob's Cabin", 0, 0);
            Location alicesHome = new Location("Alice's Cottage", 5, 5);
            Location orchard = new Location("Greenwood Orchard", 8, 8);
            
            town.AddLocation(bobsHome);
            town.AddLocation(alicesHome);
            town.AddLocation(orchard);

            // 2. CREATE NPCs
            NPC bob = CreateNPC("Bob", bobsHome.Position);
            NPC alice = CreateNPC("Alice", alicesHome.Position);
            town.AddNPC(bob);
            town.AddNPC(alice);

            // 3. SET RELATIONSHIPS
            bob.SetRelationship("Home", bobsHome);
            bob.SetRelationship("Friend", alice);
            alice.SetRelationship("Home", alicesHome);
            alice.SetRelationship("Work", orchard);

            // 4. ATTACH AFFORDANCES (Living on the OBJECT)
            
            // Destinations afford walking
            bobsHome.AddAffordance(new WalkToActivity(bobsHome));
            alice.AddAffordance(new WalkToActivity(alice));
            alicesHome.AddAffordance(new WalkToActivity(alicesHome));
            orchard.AddAffordance(new WalkToActivity(orchard));

            // Social/Economic objects afford interactions
            alice.AddAffordance(new TradeActivity());
            
            // Locations afford biological survival
            bobsHome.AddAffordance(new SleepActivity());
            alicesHome.AddAffordance(new SleepActivity());
            orchard.AddAffordance(new HarvestActivity(orchard));

            // Resources afford consumption
            // We can't add this to a specific item yet because they don't exist,
            // so we add it to the NPCs as a "Universal Rule" for items they might hold.
            // Actually, for this PoC, we'll let the Mouth afford eating.
            bob.Children.Find(c => c.Name == "Mouth").AddAffordance(new EatActivity());
            alice.Children.Find(c => c.Name == "Mouth").AddAffordance(new EatActivity());

            // 5. STARTING ITEMS
            Item gold = new Item("Gold Pouch");
            gold.SetState("HasGold", true);
            bob.AddChild(gold);

            // 6. RUN THE SIMULATION
            Console.WriteLine("Simulation Running. NPCs will match their Tags against world Affordances.\n");

            town.Run(100, 50);

            MemoryUtility.DumpMemories(town.NPCs);
            Console.WriteLine("\n=== SIMULATION COMPLETE ===");
            Console.ReadLine();
        }

        static NPC CreateNPC(string name, Vector2 startPos)
        {
            NPC npc = new NPC(name, startPos);
            
            var hands = new BodyPartEntity("Hands");
            hands.AddTag("Hands");
            
            var feet = new BodyPartEntity("Feet");
            feet.AddTag("Feet");
            
            var mouth = new BodyPartEntity("Mouth");
            mouth.AddTag("Mouth");
            
            npc.AddChild(hands);
            npc.AddChild(feet);
            npc.AddChild(mouth);
            
            npc.AddChild(new NeedEntity("Stomach", 5, "IsHungry"));
            npc.AddChild(new NeedEntity("Energy", 15, "IsTired"));
            return npc;
        }
    }
}
