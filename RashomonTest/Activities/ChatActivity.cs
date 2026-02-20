using System;
using System.Collections.Generic;
using GoapRpgPoC.Core;

namespace GoapRpgPoC.Activities
{
    public class ChatActivity : Activity
    {
        private int _chatTimer = 0;
        private const int CHAT_DURATION = 3; // How many ticks the chat lasts

        public ChatActivity(NPC initiator, NPC target)
        {
            Name = $"{initiator.Name} is having a friendly chat with {target.Name}";
            Participants[ActivityRole.Initiator] = initiator;
            Participants[ActivityRole.Target] = target;

            // Preconditions: Must be near each other
            Preconditions[ActivityRole.Initiator] = new Dictionary<string, bool> { { "NearTarget", true } };

            // Effects: Both feel socialized!
            Effects[ActivityRole.Initiator] = new Dictionary<string, bool> { { "Socialized", true } };
            Effects[ActivityRole.Target] = new Dictionary<string, bool> { { "Socialized", true } };
        }

        public override void OnTick(int currentTick)
        {
            _chatTimer++;

            if (_chatTimer < CHAT_DURATION)
            {
                Console.WriteLine($"   [CHAT] {Participants[ActivityRole.Initiator].Name} and {Participants[ActivityRole.Target].Name} are talking... (Tick {_chatTimer}/{CHAT_DURATION})");
            }
            else
            {
                // The "Director" decides the scene is over
                ApplyEffects(currentTick);
            }
        }
    }
}
