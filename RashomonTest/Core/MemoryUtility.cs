using System;
using System.IO;
using System.Collections.Generic;

namespace GoapRpgPoC.Core
{
    public static class MemoryUtility
    {
        public static void DumpMemories(List<NPC> npcs, string directoryName = "Logs")
        {
            // Ensure the directory exists
            if (!Directory.Exists(directoryName))
            {
                Directory.CreateDirectory(directoryName);
            }

            Console.WriteLine($"
[SYSTEM] Exporting memories to {Path.GetFullPath(directoryName)}...");

            foreach (var npc in npcs)
            {
                string filePath = Path.Combine(directoryName, $"{npc.Name}_Memories.txt");
                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    writer.WriteLine($"=== MEMORY LOG FOR {npc.Name.ToUpper()} ===");
                    writer.WriteLine($"Final Position: {npc.CurrentPosition}");
                    writer.WriteLine("--- Timeline ---");

                    if (npc.Memories.Count == 0)
                    {
                        writer.WriteLine("I have no memories of this day.");
                    }

                    foreach (var memory in npc.Memories)
                    {
                        writer.WriteLine($"[Tick {memory.Timestamp}] I remember: {memory.PastActivity.Name}");
                    }
                    
                    writer.WriteLine("
--- Final State ---");
                    foreach (var state in npc.State)
                    {
                        writer.WriteLine($"{state.Key}: {state.Value}");
                    }
                }
            }
            
            Console.WriteLine("[SYSTEM] Export complete!");
        }
    }
}
