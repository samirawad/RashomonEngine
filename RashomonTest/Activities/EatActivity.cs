using System;
using System.Collections.Generic;
using System.Linq;
using GoapRpgPoC.Core;

namespace GoapRpgPoC.Activities
{
    public class EatActivity : Activity
    {
        public EatActivity(NPC eater)
        {
            Name = $"{eater.Name} is eating something delicious";
            Participants[ActivityRole.Initiator] = eater;

            // Precondition TAG: Initiator must have something with the "Edible" TAG
            PreconditionTags[ActivityRole.Initiator] = new List<string> { "Edible" };

            // Effects: No longer hungry AND no longer has food
            Effects[ActivityRole.Initiator] = new Dictionary<string, bool> { 
                { "IsHungry", false },
                { "Edible", false } 
            };
        }

        public override void OnTick(int currentTick)
        {
            var eater = Participants[ActivityRole.Initiator];

            // Logic: Find the child entity that has the "Edible" tag
            var foodItem = eater.Children.FirstOrDefault(c => c.Tags.Contains("Edible"));

            if (foodItem != null)
            {
                string eatMsg = $"[EAT] {eater.Name} consumes the {foodItem.Name}.";
                Console.WriteLine($"   {eatMsg}");
                eater.LogDebug(eatMsg);
                
                eater.RemoveChild(foodItem);
                
                var stomach = eater.Children.OfType<NeedEntity>().FirstOrDefault(n => n.Name == "Stomach");
                stomach?.Reset();

                ApplyEffects(currentTick);
            }
            else
            {
                eater.LogDebug("[EAT FAIL] Tried to eat but could not find an item with 'Edible' tag.");
                IsFinished = true;
            }
        }
    }
}
