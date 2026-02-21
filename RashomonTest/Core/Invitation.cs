using System.Collections.Generic;

namespace GoapRpgPoC.Core
{
    public class Invitation
    {
        public Activity Scene { get; private set; }
        public ActivityRole Role { get; private set; }
        public NPC Host { get; private set; }

        public Invitation(Activity scene, ActivityRole role, NPC host)
        {
            Scene = scene;
            Role = role;
            Host = host;
        }

        // Returns the payoff (Effects) this role provides
        public Dictionary<string, bool> GetPayoff()
        {
            if (Scene.Effects.ContainsKey(Role))
                return Scene.Effects[Role];
            return new Dictionary<string, bool>();
        }
    }
}
