using System;
using System.Collections.Generic;
using System.Linq;
using GoapRpgPoC.Core;

namespace GoapRpgPoC.Activities
{
    public class SleepActivity : Activity
    {
        private int _sleepTimer = 0;
        private const int SLEEP_DURATION = 5;

        public SleepActivity(NPC sleeper)
        {
            Name = $"{sleeper.Name} is sleeping";
            Participants[ActivityRole.Initiator] = sleeper;

            // Preconditions: Must be at home (this is handled by the planner moving them)
            Preconditions[ActivityRole.Initiator] = new Dictionary<string, bool> { { "AtHome", true } };

            // Effects: No longer tired
            Effects[ActivityRole.Initiator] = new Dictionary<string, bool> { { "IsTired", false } };
        }

        public override void OnTick(int currentTick)
        {
            _sleepTimer++;
            var sleeper = Participants[ActivityRole.Initiator];

            if (_sleepTimer < SLEEP_DURATION)
            {
                Console.WriteLine($"   [SLEEP] {sleeper.Name} is snoring... (Tick {_sleepTimer}/{SLEEP_DURATION})");
                sleeper.LogDebug($"[SLEEP] Snoring... {_sleepTimer}/{SLEEP_DURATION}");
            }
            else
            {
                // Reset the Sleep Need entity
                var brain = sleeper.Children.OfType<NeedEntity>().FirstOrDefault(n => n.Name == "Energy");
                brain?.Reset();

                ApplyEffects(currentTick);
            }
        }
    }
}
