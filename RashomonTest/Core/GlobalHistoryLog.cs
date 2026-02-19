using System.Collections.Generic;

namespace GoapRpgPoC.Core
{
    public static class GlobalHistoryLog
    {
        public static List<Activity> History = new List<Activity>();

        public static void Record(Activity activity)
        {
            History.Add(activity);
        }
    }
}
