using System;
using HabitTracker.Gainer;
using HabitTracker.Database;
using Npgsql;
using System.Collections.Generic;
using NpgsqlTypes;
namespace HabitTracker
{
    public abstract class HabitResultHandler : IObserver<HabitResult>
    {
        protected string connString;
        protected IGainer _gainer;
        public HabitResultHandler(IGainer gainer)
        {
            _gainer = gainer;
            connString = "Host=localhost;Username=Habit;Password=revarino123;Database=HabitTracker;Port=5432";

        }
        public abstract void Update(HabitResult e);
    }

    public class SuccessHandler : HabitResultHandler
    {
        public SuccessHandler(IGainer gainer) : base(gainer)
        {
        }

        public override void Update(HabitResult e)
        {
            Success ev = e as Success;
            if (ev == null) return;
            NpgsqlConnection _connection = new NpgsqlConnection(connString);
            _connection.Open();
            IHabitRepository repo1 = new HabitRepository(_connection, null);

            if (repo1.getLastLog(ev._habit.ID) != 0)
            {
                repo1.AddStreak(ev._habit.ID, _gainer.Gain());

                repo1.GiveBadge(ev._habit.ID);

            }
            else
            {
                repo1.AddStreak(ev._habit.ID, 0);
            }

        }
    }

    public class FailHandler : HabitResultHandler
    {
        public FailHandler(IGainer gainer) : base(gainer)
        {
        }

        public override void Update(HabitResult e)
        {
            Fail ev = e as Fail;
            if (ev == null) return;
            ev._habit.AddStreak(_gainer.Gain());
        }
    }
}