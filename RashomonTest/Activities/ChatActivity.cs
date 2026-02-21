using System;
using System.Collections.Generic;
using GoapRpgPoC.Core;

namespace GoapRpgPoC.Activities
{
    public class ChatActivity : Activity
    {
        private int _timer = 0;
        public override Activity Clone() => new ChatActivity();

        public override void Bind(NPC initiator, NPC target = null)
        {
            base.Bind(initiator, target);
            Preconditions[ActivityRole.Initiator] = new Dictionary<string, bool> { { $"Near({target.Name})", true } };
            Effects[ActivityRole.Initiator] = new Dictionary<string, bool> { { "IsLonely", false } };
            Effects[ActivityRole.Target] = new Dictionary<string, bool> { { "IsLonely", false } };
        }

        protected override void UpdateName() => Name = $"{Participants[ActivityRole.Initiator].Name} is chatting with {Participants[ActivityRole.Target].Name}";

        public override void Initialize() { base.Initialize(); _timer = 0; }

        public override void OnTick(int currentTick)
        {
            var init = Participants[ActivityRole.Initiator];
            var target = Participants[ActivityRole.Target];

            // 1. Handshake check
            if (target.SubscribedScene != this)
            {
                init.LogDebug($"[CHAT] Waiting for {target.Name} to join...");
                target.ReceiveInvitation(new Invitation(this, ActivityRole.Target, init));
                return;
            }

            // 2. Scene Progress
            if (++_timer >= 3) FinalizeActivity(currentTick);
        }

        public override (bool valid, string blame, string reason) GetContractStatus()
        {
            var init = Participants[ActivityRole.Initiator];
            var target = Participants[ActivityRole.Target];
            if (Vector2.Distance(init.Position, target.Position) > 0) return (false, init.Name, "Walked away");
            if (target.SubscribedScene != this) return (false, target.Name, "Ended the chat");
            return (true, "", "");
        }
    }
}
