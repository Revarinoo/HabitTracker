using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using HabitTracker;
using HabitTracker.Database;
using Npgsql;
using HabitTracker.Gainer;
using HabitTracker.HabitAggregate;
namespace Abc.HabitTracker.Api.Controllers
{
    [ApiController]
    public class HabitsController : ControllerBase
    {
        private readonly ILogger<HabitsController> _logger;
        private string connString;
        public HabitsController(ILogger<HabitsController> logger)
        {
            _logger = logger;
            connString = "Host=localhost;Username=Habit;Password=revarino123;Database=HabitTracker;Port=5432";
        }

        [HttpGet("api/v1/users/{userID}/habits")]
        public ActionResult<IEnumerable<Habits>> All(Guid userID)
        {
            NpgsqlConnection _connection = new NpgsqlConnection(connString);
            _connection.Open();
            IHabitRepository repo1 = new HabitRepository(_connection, null);
            List<Habits> habits = new List<Habits>();
            List<Guid> userHabit = new List<Guid>();
            List<Habit> habit = new List<Habit>();
            try
            {
                foreach (Guid x in repo1.cekUser(userID))
                {
                    userHabit.Add(x);
                }
                foreach (Guid y in userHabit)
                {
                    habit.Add(repo1.FindByID(y, userID));
                }

                foreach (Habit x in habit)
                {
                    Habits ht = new Habits()
                    {
                        ID = x.ID,
                        name = x.name,
                        user_id = x.users,
                        days = x.daysoff,
                        Log_count = x.Logs,
                        current_streak = x.current_streak,
                        longest_streak = x.longest_streak
                    };
                    habits.Add(ht);
                }
                return habits;
            }
            catch
            {
                return NotFound("user not found");
            }

        }

        [HttpGet("api/v1/users/{userID}/habits/{id}")]
        public ActionResult<Habits> Get(Guid userID, Guid id)
        {
            try
            {
                NpgsqlConnection _connection = new NpgsqlConnection(connString);
                _connection.Open();
                IHabitRepository repo1 = new HabitRepository(_connection, null);
                Habit h = repo1.FindByID(id, userID);
                return new Habits()
                {
                    ID = h.ID,
                    name = h.name,
                    user_id = h.users,
                    days = h.daysoff,
                    Log_count = h.Logs,
                    current_streak = h.current_streak,
                    longest_streak = h.longest_streak
                };
            }
            catch
            {
                return NotFound("user not found");
            }
        }

        [HttpPost("api/v1/users/{userID}/habits")]
        public ActionResult<Habits> AddNewHabit(Guid userID, [FromBody] RequestData data)
        {
            NpgsqlConnection _connection = new NpgsqlConnection(connString);
            _connection.Open();
            IHabitRepository repo1 = new HabitRepository(_connection, null);

            IGainer logGainer = new LogSuccess();
            try
            {
                Habit h = HabitFactory.Create(data.Name, data.days, userID, logGainer);

                repo1.CreateHabit(h, data.days);

                repo1.AddLog(h.ID);

                repo1.AddStreak(h.ID, h.getStreak());
                return new Habits()
                {
                    ID = h.ID,
                    name = h.name,
                    user_id = h.users,
                    Log_count = h.Logs,
                    days = h.daysoff,
                    current_streak = h.current_streak,
                    longest_streak = h.longest_streak
                };
            }
            catch
            {
                return NotFound("user not found");
            }
        }

        [HttpPut("api/v1/users/{userID}/habits/{id}")]
        public ActionResult<Habits> UpdateHabit(Guid userID, Guid id, [FromBody] RequestData data)
        {
            try
            {
                NpgsqlConnection _connection = new NpgsqlConnection(connString);
                _connection.Open();
                IHabitRepository repo1 = new HabitRepository(_connection, null);
                Habit h = HabitFactory.Update(id, userID, data.Name, data.days);
                repo1.UpdateHabit(h.ID, h.users, h.name, data.days);

                return new Habits()
                {
                    ID = h.ID,
                    name = h.name,
                    user_id = h.users,
                    Log_count = h.Logs,
                    days = h.daysoff,
                    current_streak = h.current_streak,
                    longest_streak = h.longest_streak
                };
            }
            catch
            {
                return NotFound("error");
            }
        }

        [HttpDelete("api/v1/users/{userID}/habits/{id}")]
        public ActionResult<Habits> DeleteHabit(Guid userID, Guid id)
        {
            NpgsqlConnection _connection = new NpgsqlConnection(connString);
            _connection.Open();
            IHabitRepository repo1 = new HabitRepository(_connection, null);
            try
            {
                Habit h = repo1.FindByID(id, userID);
                repo1.DeleteHabit(id, userID);
                return new Habits()
                {
                    ID = h.ID,
                    name = h.name,
                    user_id = h.users,
                    days = h.daysoff,
                    Log_count = h.Logs,
                    current_streak = h.current_streak,
                    longest_streak = h.longest_streak
                };
            }
            catch
            {
                return NotFound("Failed!");
            }
        }

        [HttpPost("api/v1/users/{userID}/habits/{id}/logs")]
        public ActionResult<Habits> Log(Guid userID, Guid id)
        {
            NpgsqlConnection _connection = new NpgsqlConnection(connString);
            _connection.Open();
            IHabitRepository repo1 = new HabitRepository(_connection, null);

            try
            {
                Habit h = repo1.FindByID(id, userID);

                repo1.AddLog(id);
                Habit habit = HabitFactory.AddLog(h);

                _connection.Close();
                return new Habits()
                {
                    ID = habit.ID,
                    name = habit.name,
                    user_id = habit.users,
                    days = habit.daysoff,
                    Log_count = habit.Logs,
                    current_streak = habit.current_streak,
                    longest_streak = habit.longest_streak
                };
            }
            catch
            {
                return NotFound("Failed");
            }
        }
    }
}
