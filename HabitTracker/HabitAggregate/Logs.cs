using System;

namespace HabitTracker
{
    public class Logs
    {
        private int _value;
        public int value
        {
            get
            {
                return _value;
            }
        }

        public Logs()
        {
            _value = 0;
        }

        public Logs(int count)
        {
            if (count < 0)
            {
                throw new Exception("Count must > 0");
            }
            this._value = count;
        }

        public Logs Add(int count)
        {
            if (count < 0)
            {
                throw new Exception("count cannot be negative");
            }
            return new Logs(this._value + count);
        }

        public override bool Equals(object obj)
        {
            var count = obj as Logs;
            if(count == null) return false;

            return true;
        }
        
        public override int GetHashCode()
        {
            return 1;
        }
    }
}