using System;

namespace HabitTracker
{
    public abstract class HabitResult
    {
        public Habit _habit { get; set; }
        public HabitResult(Habit habit){
            this._habit = habit;
        }
    }

    public class Success : HabitResult
    {
        public Success(Habit habit) : base(habit){}
    }

    public class Fail : HabitResult
    {
        public Fail(Habit habit) : base(habit){}
    }
}