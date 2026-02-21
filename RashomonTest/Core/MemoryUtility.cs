using System;
using System.IO;
using System.Collections.Generic;

namespace GoapRpgPoC.Core
{
    public static class MemoryUtility
    {
        public static void DumpMemories(List<NPC> npcs, string directoryName = "Logs")
        {
            if (!Directory.Exists(directoryName)) Directory.CreateDirectory(directoryName);

            Console.WriteLine($"\n[SYSTEM] Exporting logs to {Path.GetFullPath(directoryName)}...");

            foreach (var npc in npcs)
            {
                // 1. Export Memories (Personal perspective)
                string memPath = Path.Combine(directoryName, $"{npc.Name}_Memories.txt");
                using (StreamWriter writer = new StreamWriter(memPath))
                {
                    writer.WriteLine($"=== MEMORY LOG FOR {npc.Name.ToUpper()} ===");
                    writer.WriteLine($"Final Position: {npc.Position}");
                    
                    writer.WriteLine("\n--- Inventory & Parts ---");
                    foreach (var child in npc.Children)
                    {
                        writer.Write($"- {child.Name}");
                        if (child.State.Count > 0)
                        {
                            writer.Write(" [");
                            foreach (var s in child.State) writer.Write($"{s.Key}:{s.Value} ");
                            writer.Write("]");
                        }
                        writer.WriteLine();
                    }

                    writer.WriteLine("\n--- Timeline ---");
                    foreach (var memory in npc.Memories)
                    {
                        string status = memory.PastActivity.WasSuccessful ? "Success" : "FAILED";
                        writer.WriteLine($"[Tick {memory.Timestamp}] [{status}] I remember: {memory.PastActivity.Name}");
                    }
                    
                    writer.WriteLine("\n--- Recursive State (Search) ---");
                    string[] interestingKeys = { "Edible", "HasGold", "IsHungry" };
                    foreach (var key in interestingKeys)
                        writer.WriteLine($"{key}: {npc.GetState(key)}");
                }

                // 2. Export Debug Log (Technical perspective)
                string debugPath = Path.Combine(directoryName, $"{npc.Name}_Debug.txt");
                using (StreamWriter writer = new StreamWriter(debugPath))
                {
                    writer.WriteLine($"=== TECHNICAL DEBUG LOG FOR {npc.Name.ToUpper()} ===");
                    foreach (var log in npc.DebugLog)
                    {
                        writer.WriteLine(log);
                    }
                }
            }
            
            Console.WriteLine("[SYSTEM] Export complete!");
        }
    }
}
