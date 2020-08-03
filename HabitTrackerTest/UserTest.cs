using System;
using Xunit;
using HabitTracker;
namespace HabitTrackerTest
{
    public class UserTest
    {
        [Fact]
        public void UserLogs(){
            User u = User.NewUser("Herman");
        }
    }
}
