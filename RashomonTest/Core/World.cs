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
            Console.WriteLine($"
--- WORLD TICK {CurrentTick} ---");

            // In a real system, this is where we'd iterate through NPCs
            // and let them Think, Plan, and Act.
            foreach (var npc in NPCs)
            {
                // For now, we'll just log their position
                // Console.WriteLine($"{npc.Name} is at {npc.CurrentPosition}");
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
