using System;
using System.Collections.Generic;
namespace HabitTracker {
    public interface IHabitRepository{
        
        void CreateHabit(Habit hbt,String[] days);
        void AddLog(Guid habitID);
        void AddStreak(Guid habitID, int currentStreak);
        void DeleteHabit(Guid habitID, Guid userID);
        int getLastLog(Guid habitID);
        List<String> getDaysOff(Guid habitID);
        Guid FindUser(Guid id);
        List<Guid> cekUser(Guid id);
        void UpdateHabit(Guid habitID, Guid userID, string name, String[] days);
        Habit FindByID(Guid habitID, Guid userID);
        List<Guid> GetAllBadge(Guid userID);
        Badge FindBadge(Guid userID,Guid badgeID);
        int getLong(Guid habitID);
        void GiveBadge(Guid habitID);
    }
}