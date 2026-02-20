using System;
using System.Collections.Generic;
using GoapRpgPoC.Core;

namespace GoapRpgPoC.Activities
{
    public class WalkToActivity : Activity
    {
        private NPC _walker;
        private NPC _target;

        public WalkToActivity(NPC walker, NPC target)
        {
            _walker = walker;
            _target = target;
            Name = $"{_walker.Name} is walking to {_target.Name}";
            
            Participants[ActivityRole.Initiator] = _walker;
            Participants[ActivityRole.Target] = _target;

            // Effect: Walker is now near the target
            Effects[ActivityRole.Initiator] = new Dictionary<string, bool> { { "NearTarget", true } };
            Effects[ActivityRole.Target] = new Dictionary<string, bool> { { "NearTarget", true } };
        }

        public override void OnTick(int currentTick)
        {
            // Simple logic: Move Bob 1 step closer to Alice
            if (_walker.CurrentPosition.X < _target.CurrentPosition.X) _walker.CurrentPosition = new Vector2(_walker.CurrentPosition.X + 1, _walker.CurrentPosition.Y);
            else if (_walker.CurrentPosition.X > _target.CurrentPosition.X) _walker.CurrentPosition = new Vector2(_walker.CurrentPosition.X - 1, _walker.CurrentPosition.Y);
            
            if (_walker.CurrentPosition.Y < _target.CurrentPosition.Y) _walker.CurrentPosition = new Vector2(_walker.CurrentPosition.X, _walker.CurrentPosition.Y + 1);
            else if (_walker.CurrentPosition.Y > _target.CurrentPosition.Y) _walker.CurrentPosition = new Vector2(_walker.CurrentPosition.X, _walker.CurrentPosition.Y - 1);

            Console.WriteLine($"   [MOVE] {_walker.Name} moved to {_walker.CurrentPosition}...");

            // If we've arrived, finish the activity
            if (Vector2.Distance(_walker.CurrentPosition, _target.CurrentPosition) == 0)
            {
                ApplyEffects(currentTick);
            }
        }
    }
}
