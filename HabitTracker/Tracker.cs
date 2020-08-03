using System;
using HabitTracker.Gainer;
using System.Collections.Generic;

namespace HabitTracker
{
    public class Tracker
    {
        private Habit _habit;
        protected IGainer _StreakGainer;
        public Tracker(IGainer streak, Habit habit){
            this._StreakGainer = streak;
            this._habit = habit;
        }

        
        protected void giveStreak()
        {
         _habit.AddStreak(_StreakGainer.Gain());
        }
    }
}