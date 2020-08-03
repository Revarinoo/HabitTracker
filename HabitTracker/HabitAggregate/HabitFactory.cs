using System;
using HabitTracker.Gainer;
using HabitTracker;
namespace HabitTracker.HabitAggregate
{
    public class HabitFactory
    {
        public static Habit Create(String habit_name, String[] days, Guid user, IGainer logGainer)
        {
            if (!checkDays(days)) return null;
            Habit habit = Habit.NewHabit(habit_name, user, logGainer);
            foreach (String item in days)
            {
                if((item != "Mon") && (item != "Tue") && (item != "Wed") && (item != "Thu") && (item != "Fri") && (item != "Sat") && (item != "Sun")){
                    throw new Exception("Must be 3 words!");
                }
            }
            foreach (String x in days)
            {
                habit.AddDaysOff(x);
            }

            habit.AddLogs();
            return habit;
        }

        public static Habit Update(Guid habitID, Guid userID, String habit_name, String[] days)
        {
            IGainer logGainer = new LogSuccess();
            if (!checkDays(days)) return null;

            Habit h = Habit.UpdateHabit(habitID, userID, habit_name, logGainer);
            h.UpdateDayOff(days);
            return h;
        }

        public static Habit AddLog(Habit h){
            HabitResultHandler success = new SuccessHandler(new StreakSuccess());
            HabitResultHandler fail = new FailHandler(new StreakFail());
            h.Attach(success);
            h.Attach(fail);
            h.AddLogs();
            return h;
        }
        private static bool checkDays(String[] days)
        {
            int cek = 0;
            for (int x = 0; x < days.Length; x++)
            {
                for (int k = x + 1; k < days.Length; k++)
                {
                    if (days[x] == days[k])
                    {

                        return false;
                    }
                }

                if (cek == 0)
                {
                    if (days.Length == 7)
                    {
                        return false;
                    }

                }
            }
            return true;

        }
    }
}