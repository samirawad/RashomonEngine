using System;
using System.Collections.Generic;

namespace GoapRpgPoC.Core
{
    public class World
    {
        public int CurrentTick { get; private set; } = 0;
        public List<NPC> NPCs { get; private set; } = new List<NPC>();
        public List<Location> Locations { get; private set; } = new List<Location>();

        public void AddNPC(NPC npc) => NPCs.Add(npc);
        public void AddLocation(Location loc) => Locations.Add(loc);

        public void Tick()
        {
            CurrentTick++;
            Console.WriteLine($"\n--- WORLD TICK {CurrentTick} ---");

            // 1. Tick all Entities (Needs, Stomachs, etc. grow in parallel)
            foreach (var npc in NPCs)
            {
                npc.Tick(CurrentTick);
            }

            // 2. Update all NPCs (They perceive, plan, and act in parallel)
            foreach (var npc in NPCs)
            {
                npc.Update(CurrentTick);
            }
        }

        public void Run(int duration, int sleepMs = 100)
        {
            for (int i = 0; i < duration; i++)
            {
                Tick();
                System.Threading.Thread.Sleep(sleepMs);
            }
        }
    }
}
