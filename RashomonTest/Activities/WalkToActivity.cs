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
            if (_walker.Position.X < _target.Position.X) _walker.Position = new Vector2(_walker.Position.X + 1, _walker.Position.Y);
            else if (_walker.Position.X > _target.Position.X) _walker.Position = new Vector2(_walker.Position.X - 1, _walker.Position.Y);
            
            if (_walker.Position.Y < _target.Position.Y) _walker.Position = new Vector2(_walker.Position.X, _walker.Position.Y + 1);
            else if (_walker.Position.Y > _target.Position.Y) _walker.Position = new Vector2(_walker.Position.X, _walker.Position.Y - 1);

            string moveMsg = $"[MOVE] {_walker.Name} moved to {_walker.Position} (Target: {_target.Name} at {_target.Position})";
            Console.WriteLine($"   {moveMsg}");
            _walker.LogDebug(moveMsg);

            // If we've arrived, finish the activity
            if (Vector2.Distance(_walker.Position, _target.Position) == 0)
            {
                _walker.LogDebug($"[MOVE] Arrived at {_target.Name}.");
                ApplyEffects(currentTick);
            }
        }
    }
}
