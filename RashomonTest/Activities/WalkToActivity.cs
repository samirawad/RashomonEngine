using System;
using System.Collections.Generic;
using GoapRpgPoC.Core;

namespace GoapRpgPoC.Activities
{
    public class WalkToActivity : Activity
    {
        private Entity _target;

        // Constructor for template
        public WalkToActivity(Entity target) { _target = target; }

        public override Activity Clone() => new WalkToActivity(_target);

        public override void Bind(NPC initiator, NPC target = null)
        {
            base.Bind(initiator, target);
            
            // Dynamic Effects based on the specific target
            Effects[ActivityRole.Initiator] = new Dictionary<string, bool> { 
                { $"Near({_target.Name})", true } 
            };

            if (_target.Name.Contains("Home")) Effects[ActivityRole.Initiator]["AtHome"] = true;
        }

        protected override void UpdateName() 
        {
            Name = $"{Participants[ActivityRole.Initiator].Name} is walking to {_target.Name}";
        }

        public override void OnTick(int currentTick)
        {
            var walker = Participants[ActivityRole.Initiator];
            if (walker.Position.X < _target.Position.X) walker.Position = new Vector2(walker.Position.X + 1, walker.Position.Y);
            else if (walker.Position.X > _target.Position.X) walker.Position = new Vector2(walker.Position.X - 1, walker.Position.Y);
            
            if (walker.Position.Y < _target.Position.Y) walker.Position = new Vector2(walker.Position.X, walker.Position.Y + 1);
            else if (walker.Position.Y > _target.Position.Y) walker.Position = new Vector2(walker.Position.X, walker.Position.Y - 1);

            if (Vector2.Distance(walker.Position, _target.Position) == 0) FinalizeActivity(currentTick);
        }

        public override (bool valid, string blame, string reason) GetContractStatus()
        {
            if (Vector2.Distance(Participants[ActivityRole.Initiator].Position, _target.Position) > 0)
                return (false, Participants[ActivityRole.Initiator].Name, "Not at destination");
            return (true, "", "");
        }
    }
}
