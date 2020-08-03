// using System;
// using Xunit;
// using HabitTracker;
// using Npgsql;
// using HabitTracker.Database;
// using HabitTracker.HabitAggregate;
// using HabitTracker.Gainer;
// namespace HabitTrackerTest
// {
//     public class HabitRepoTest
//     {
//         private string connString;
//         public HabitRepoTest()
//         {
//             connString = "Host=localhost;Username=Habit;Password=revarino123;Database=HabitTracker;Port=5432";
//         }

//         [Fact]
//         public void CreateHabit()
//         {
//             NpgsqlConnection _connection = new NpgsqlConnection(connString);
//             _connection.Open();
//             IHabitRepository repo1 = new HabitRepository(_connection, null);
//             IUserRepository repo2 = new UserRepository(_connection, null);

//             User u = User.NewUser("Pulisic");
//             repo2.Create(u);
//             IGainer logGainer = new LogSuccess();
//             String[] days = { "sunday", "monday" };
//             Habit h = HabitFactory.Create("Cek streak", days, u.ID, logGainer);

//             repo1.CreateHabit(h, days);

//             repo1.AddLog(h.ID, h.Logs);
//             repo1.AddStreak(h.ID, h.getStreak());

//             _connection.Close();
//         }

//         // [Fact]
//         // public void cekLog()
//         // {
//         //     NpgsqlConnection _connection = new NpgsqlConnection(connString);
//         //     _connection.Open();
//         //     IHabitRepository repo1 = new HabitRepository(_connection, null);
//         //     Guid id = Guid.Parse("dcb833e9-9d60-4455-b650-6f44b225c7d6");


//         //     int cek = repo1.getLastLog(id);
//         //     Assert.Equal(1,cek);
//         // }
//     }
// }