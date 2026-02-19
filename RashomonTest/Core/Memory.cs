namespace GoapRpgPoC.Core
{
    public class Memory
    {
        public Activity PastActivity { get; private set; }
        public int Timestamp { get; private set; }

        public Memory(Activity activity, int time)
        {
            PastActivity = activity;
            Timestamp = time;
        }
    }
}
