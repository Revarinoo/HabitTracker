using System;

namespace HabitTracker.Gainer
{
    public class Bonus : IGainer
    {
        private int _multiple;
        private IGainer _wrappee;

        public Bonus(int multiple, IGainer wrappee)
        {
            _multiple = multiple;
            _wrappee = wrappee;
        }

        public int Gain()
        {
            return _wrappee.Gain() * _multiple;
        }
    }
}