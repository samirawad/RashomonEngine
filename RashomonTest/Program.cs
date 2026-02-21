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
            Console.WriteLine("=== INITIALIZING SCENE & ROLE SIMULATION v2.3 ===\n");

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

            // 4. ATTACH TEMPLATES (The Rule of Discovery)
            
            // --- INTERNAL AFFORDANCES (Capabilities) ---
            var bobFeet = bob.Children.Find(c => c.Name == "Feet");
            var bobMouth = bob.Children.Find(c => c.Name == "Mouth");
            var aliceFeet = alice.Children.Find(c => c.Name == "Feet");
            var aliceMouth = alice.Children.Find(c => c.Name == "Mouth");

            // Feet afford movement to known entities
            bobFeet.AddAffordance(new WalkToActivity(bobsHome));
            bobFeet.AddAffordance(new WalkToActivity(alice));
            aliceFeet.AddAffordance(new WalkToActivity(alicesHome));
            aliceFeet.AddAffordance(new WalkToActivity(orchard));

            // Mouths afford eating
            bobMouth.AddAffordance(new EatActivity());
            aliceMouth.AddAffordance(new EatActivity());

            // --- EXTERNAL AFFORDANCES (Environment) ---
            bobsHome.AddAffordance(new SleepActivity());
            alicesHome.AddAffordance(new SleepActivity());
            orchard.AddAffordance(new HarvestActivity(orchard));

            // --- SOCIAL AFFORDANCES (Relationships) ---
            // Alice affords trading to those who know her
            alice.AddAffordance(new TradeActivity());

            // 5. STARTING ITEMS
            Item gold = new Item("Gold Pouch");
            gold.SetState("HasGold", true);
            bob.AddChild(gold);

            // 6. RUN THE SIMULATION
            Console.WriteLine("Simulation Running. Every action is now a unique scene instance.\n");

            town.Run(100, 50);

            MemoryUtility.DumpMemories(town.NPCs);
            Console.WriteLine("\n=== SIMULATION COMPLETE ===");
            Console.ReadLine();
        }

        static NPC CreateNPC(string name, Vector2 startPos)
        {
            NPC npc = new NPC(name, startPos);
            npc.AddChild(new BodyPartEntity("Hands"));
            npc.AddChild(new BodyPartEntity("Feet"));
            npc.AddChild(new BodyPartEntity("Mouth"));
            npc.AddChild(new NeedEntity("Stomach", 5, "IsHungry"));
            npc.AddChild(new NeedEntity("Energy", 15, "IsTired"));
            return npc;
        }
    }
}
