using System;
using System.Collections.Generic;
using GoapRpgPoC.Core;

namespace GoapRpgPoC.Activities
{
    public class ChatActivity : Activity
    {
        private int _timer = 0;
        public override Activity Clone() => new ChatActivity();

        public ChatActivity()
        {
            RequiredCapability = Tags.Mouth;
            Effects[ActivityRole.Initiator] = new Dictionary<string, bool> { { States.Lonely, false } };
            Effects[ActivityRole.Target] = new Dictionary<string, bool> { { States.Lonely, false } };
        }

        public override void Bind(NPC initiator, NPC? target = null)
        {
            base.Bind(initiator, target);
            if (target != null) Preconditions[ActivityRole.Initiator] = new Dictionary<string, bool> { { $"Near({target.Name})", true } };
        }

        protected override void UpdateName() => Name = $"{Participants[ActivityRole.Initiator].Name} is chatting with {Participants[ActivityRole.Target].Name}";

        public override void Initialize() { base.Initialize(); _timer = 0; }

        public override void OnTick(int currentTick)
        {
            var init = Participants[ActivityRole.Initiator];
            var target = Participants[ActivityRole.Target];

            if (target.SubscribedScene != this)
            {
                init.LogDebug($"[CHAT] Waiting for {target.Name} to join...");
                target.ReceiveInvitation(new Invitation(this, ActivityRole.Target, init));
                return;
            }

            if (++_timer >= 3) FinalizeActivity(currentTick);
        }

        public override (bool valid, string blame, string reason) GetContractStatus()
        {
            var baseStatus = base.GetContractStatus();
            if (!baseStatus.valid) return baseStatus;

            var init = Participants[ActivityRole.Initiator];
            var target = Participants[ActivityRole.Target];
            if (Vector2.Distance(init.Position, target.Position) > 0) return (false, init.Name, "Walked away");
            if (target.SubscribedScene != this) return (false, target.Name, "Ended chat");
            return (true, "", "");
        }
    }
}
