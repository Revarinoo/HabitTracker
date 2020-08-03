using System;
using System.Collections.Generic;
using HabitTracker.Gainer;
using HabitTracker;
namespace HabitTracker
{
    public class Habit : IObservable<HabitResult>
    {
        private Guid _id;
        private string _name;
        private List<String> _daysoff;
        private Logs _logs;
        private Streak _streak;
        private Guid _users;

        public Guid ID
        {
            get
            {
                return _id;
            }
        }

        public string name
        {
            get
            {
                return _name;
            }
        }

        public List<String> daysoff
        {
            get
            {
                return _daysoff;
            }
        }

        public Guid users
        {
            get
            {
                return _users;
            }
        }

        public int Logs
        {
            get
            {
                return _logs.value;
            }
        }

        public int current_streak
        {
            get
            {
                return _streak.current_streak;
            }
        }

        public int longest_streak{
            get{
                return _streak.longest_streak;
            }
        }
        protected IGainer _logGainer;
        public Habit(Guid id, Guid user, string name, IGainer logGainer) : this(id, user, name, new Logs(1), new Streak(0,0), logGainer) { }
        public Habit(Guid id, Guid user, string name, Logs count, Streak value, IGainer logGainer)
        {
            if (name == null) throw new Exception("Name cannot be null");
            if (name.Length < 2 || name.Length > 100)
            {
                throw new Exception("Name must between 2 and 100");
            }
            this._id = id;
            this._users = user;
            this._name = name;
            this._daysoff = new List<String>();
            this._logs = count;
            this._streak = value;
            this._logGainer = logGainer;
        }

        public static Habit NewHabit(string name, Guid userID, IGainer logGainer)
        {
            return new Habit(Guid.NewGuid(), userID, name, new Logs(), new Streak(), logGainer);
        }

        public static Habit UpdateHabit(Guid id, Guid userID, string name, IGainer logGainer){
            return new Habit(id,userID,name,logGainer);
        }

        public void AddStreak(int streak)
        {
            if (streak != 0)
            {
                this._streak = this._streak.Add(streak);
            }
            else
            {
                this._streak = this._streak.resetStreak();
            }
        }

        protected List<IObserver<HabitResult>> _observers = new List<IObserver<HabitResult>>();
        public void Attach(IObserver<HabitResult> obs)
        {
            _observers.Add(obs);
        }

        public void Broadcast(HabitResult e)
        {
            foreach (var obs in _observers)
            {
                obs.Update(e);
            }
        }

        public void AddLogs()
        {
            this._logs = this._logs.Add(_logGainer.Gain());
            Broadcast(new Success(this));
        }

        public override bool Equals(object obj)
        {
            var habit = obj as Habit;
            if (habit == null) return false;

            return true;
        }

        public int getStreak()
        {
            return _streak.current_streak;
        }

        public void AddDaysOff(String days)
        {
            
            _daysoff.Add(days);
        }

        public void UpdateDayOff(String[] days){
            foreach(String y in daysoff){
                _daysoff.Remove(y);
            }
            foreach(String item in days){
                if((item != "Mon") && (item != "Tue") && (item != "Wed") && (item != "Thu") && (item != "Fri") && (item != "Sat") && (item != "Sun")){
                    throw new Exception("Must be 3 words!");
                }
                _daysoff.Add(item);
            }
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + _id.GetHashCode();
                return hash;
            }
        }
    }
}