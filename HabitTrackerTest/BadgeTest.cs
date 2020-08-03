using System;
using Xunit;
using HabitTracker;

namespace HabitTrackerTest
{
    public class BadgeTest
    {
        User Revarino;
        Badge badge;

        [Fact]
        public void CreateBadge()
        {
            Revarino = User.NewUser("Revarino");
            badge = Badge.NewBadge("Dominating", "4++ Streak",Revarino.ID);
        }
    }
}