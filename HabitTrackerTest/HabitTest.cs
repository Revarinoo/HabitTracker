// using System;
// using Xunit;
// using HabitTracker;
// using HabitTracker.Gainer;
// using System.Collections.Generic;
// using HabitTracker.HabitAggregate;
// namespace HabitTrackerTest
// {
//     public class HabitTest
//     {
//         User Reva, Rino;
//         Habit habit;

//         [Fact]
//         public void createHabit(){
//             Reva = User.NewUser("Reva");
//             Rino = User.NewUser("Rino");
//             IGainer logGainer = new LogSuccess();
//             String[] days = {"sunday","monday"};
//             habit = HabitFactory.Create("Tidur yang cukup",days,Reva.ID,logGainer);
            
//             Assert.Equal(1, habit.getStreak());
//             Assert.Equal(1,habit.Logs);
//         }
//     }
// }