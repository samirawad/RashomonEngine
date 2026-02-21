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
            Console.WriteLine("=== INITIALIZING PURE DISCOVERY TOWN v2.7 ===\n");

            World town = new World();
            
            // 1. LOCATIONS
            Location bobsHome = new Location("Bob's Cabin", 0, 0);
            Location alicesHome = new Location("Alice's Cottage", 5, 5);
            Location forge = new Location("Iron Hearth Forge", 5, 0);
            Location orchard = new Location("Greenwood Orchard", 8, 8);
            Location tavern = new Location("The Rusty Tankard", 2, 2);
            
            town.AddLocation(bobsHome);
            town.AddLocation(alicesHome);
            town.AddLocation(forge);
            town.AddLocation(orchard);
            town.AddLocation(tavern);

            // 2. CREATE NPCs
            NPC bob = CreateNPC("Bob", bobsHome.Position);
            NPC alice = CreateNPC("Alice", alicesHome.Position);
            NPC charlie = CreateNPC("Charlie", forge.Position);
            NPC diana = CreateNPC("Diana", tavern.Position);

            town.AddNPC(bob);
            town.AddNPC(alice);
            town.AddNPC(charlie);
            town.AddNPC(diana);

            // 3. SET RELATIONSHIPS
            bob.SetRelationship(Relations.Home, bobsHome);
            bob.SetRelationship(Relations.Friend, alice);

            alice.SetRelationship(Relations.Home, alicesHome);
            alice.SetRelationship(Relations.Work, orchard);

            charlie.SetRelationship(Relations.Home, forge); // Blacksmith lives at work
            charlie.SetRelationship(Relations.Work, forge);
            charlie.SetRelationship(Relations.Friend, alice); // Needs to know where to buy food!

            diana.SetRelationship(Relations.Home, tavern); // Innkeeper lives at work
            diana.SetRelationship(Relations.Work, tavern);
            diana.SetRelationship(Relations.Friend, alice); // Needs food too!

            // 4. ATTACH AFFORDANCES
            
            // Movement affordances on destinations
            foreach (var loc in town.Locations)
            {
                loc.AddAffordance(new WalkToActivity(loc));
            }
            // NPCs also afford being walked to
            foreach (var npc in town.NPCs)
            {
                npc.AddAffordance(new WalkToActivity(npc));
            }

            // Biological affordances on body parts
            foreach (var npc in town.NPCs)
            {
                npc.Children.Find(c => c.Name == "Mouth")?.AddAffordance(new EatActivity());
                npc.Children.Find(c => c.Name == "Mouth")?.AddAffordance(new SleepActivity()); // Sleep is biological too
            }

            // Environmental & Job affordances
            orchard.AddAffordance(new HarvestActivity(orchard));
            forge.AddAffordance(new WorkActivity(forge));
            tavern.AddAffordance(new WorkActivity(tavern));

            // Social affordances
            alice.AddAffordance(new TradeActivity());
            diana.AddAffordance(new ChatActivity());

            // 5. STARTING ITEMS
            Item bobsGold = new Item("Bob's Initial Gold");
            bobsGold.SetState(States.HasGold, true);
            bob.AddChild(bobsGold);

            // 6. RUN THE SIMULATION (Increase ticks to allow economic cycles)
            Console.WriteLine("Simulation Running. Charlie and Diana must work to buy Alice's apples.\n");

            town.Run(150, 50);

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
            
            // Vary hunger slightly to stagger activity
            var random = new Random();
            npc.AddChild(new NeedEntity("Stomach", 5, States.Hungry));
            npc.AddChild(new NeedEntity("Energy", 15, States.Tired));
            npc.AddChild(new NeedEntity("Heart", 25, States.Lonely));
            return npc;
        }
    }
}
