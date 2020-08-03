using System;
using HabitTracker.Gainer;
namespace HabitTracker
{
    public class Streak
    {
    
        private int _currentStreak;
        private int _longestStreak;

        public int current_streak
        {
            get
            {
                return _currentStreak;
            }
        }
        public int longest_streak
        {
            get
            {
                return _longestStreak;
            }
        }

        public Streak()
        {
            _currentStreak = 0;
            _longestStreak = _currentStreak;
        }

        public Streak(int current_streak, int longest_streak)
        {
            if (current_streak < 0 || longest_streak < 0)
            {
                throw new Exception("Streak must > 0");
            }
            
            this._longestStreak = longest_streak;
            
            this._currentStreak = current_streak;
        }

        public Streak Add(int streak)
        {
            if (streak < 0)
            {
                throw new Exception("Streak must > 0");
            }
            
            return new Streak(this._currentStreak + streak, this._longestStreak);
        }

        public Streak resetStreak(){
            return new Streak(0,this._longestStreak);
        }

        public override bool Equals(object obj)
        {
            var streak = obj as Streak;
            if (streak == null) return false;

            return true;
        }

        public override int GetHashCode()
        {
            return 1;
        }
    }
}