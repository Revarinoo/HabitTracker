using System;

namespace HabitTracker.Gainer
{
    public class StreakSuccess : IGainer
    {
        public int Gain(){
            return 1;
        }
    }

    public class StreakFail : IGainer
    {
        public int Gain(){
            return 0;
        }
    }
}