using System;
using System.Collections.Generic;

namespace GoapRpgPoC.Core
{
    public class World
    {
        public int CurrentTick { get; private set; } = 0;
        public List<NPC> NPCs { get; private set; } = new List<NPC>();
        public List<Location> Locations { get; private set; } = new List<Location>();

        // This is our activity pool that NPCs can "choose" from
        public List<Activity> AvailableActivities { get; set; } = new List<Activity>();

        public void AddNPC(NPC npc) => NPCs.Add(npc);
        public void AddLocation(Location loc) => Locations.Add(loc);

        public void Tick()
        {
            CurrentTick++;
            Console.WriteLine($"\n--- WORLD TICK {CurrentTick} ---");

            // Tick all NPCs and their internal entities (Needs, etc.)
            foreach (var npc in NPCs)
            {
                npc.Tick(CurrentTick);
                // Console.WriteLine($"{npc.Name} is at {npc.Position}");
            }
        }

        public void Run(int duration)
        {
            for (int i = 0; i < duration; i++)
            {
                Tick();
            }
        }
    }
}
