using System;

namespace GoapRpgPoC.Core
{
    public class NeedEntity : Entity
    {
        public int Value { get; private set; } = 0;
        private int _tickRate;
        private string _stateKey;

        public NeedEntity(string name, int tickRate, string stateKey) : base(name, new Vector2(0,0))
        {
            _tickRate = tickRate;
            _stateKey = stateKey;
        }

        public override void Tick(int currentTick)
        {
            if (currentTick % _tickRate == 0)
            {
                Value++;
                // If value is high, set the parent's state to "Hungry" or "Tired"
                if (Value > 5 && Parent != null)
                {
                    Parent.SetState(_stateKey, true);
                    Console.WriteLine($"      [NEED] {Parent.Name}'s {Name} is increasing... ({_stateKey}=true)");
                }
            }
            base.Tick(currentTick);
        }

        public void Reset()
        {
            Value = 0;
            Parent?.SetState(_stateKey, false);
        }
    }
}
