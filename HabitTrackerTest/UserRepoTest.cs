using System;
using Xunit;
using HabitTracker;
using Npgsql;
using HabitTracker.Database;

namespace HabitTrackerTest
{
    public class UserRepoTest
    {
        private string connString;
        public UserRepoTest()
        {
            connString = "Host=localhost;Username=Habit;Password=revarino123;Database=HabitTracker;Port=5432";
        }

        [Fact]
        public void CreateUser()
        {
            NpgsqlConnection _connection = new NpgsqlConnection(connString);
            _connection.Open();
            IUserRepository repo = new UserRepository(_connection, null);

            User u = User.NewUser("Kepa");
            repo.Create(u);
            
            _connection.Close();
        }
    }
}