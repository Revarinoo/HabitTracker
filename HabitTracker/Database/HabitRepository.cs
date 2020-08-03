using System;
using System.Collections.Generic;
using Npgsql;
using NpgsqlTypes;
using HabitTracker;
using HabitTracker.Gainer;
namespace HabitTracker.Database
{
    public class HabitRepository : IHabitRepository
    {
        private NpgsqlConnection _connection;
        private NpgsqlTransaction _transaction;

        public HabitRepository(NpgsqlConnection connection, NpgsqlTransaction transaction)
        {
            _connection = connection;
            _transaction = transaction;
        }

        public void CreateHabit(Habit hbt, String[] days)
        {
            string query = "INSERT INTO \"Habit\" (id, name, user_id) VALUES(@id, @name, @user_id)";
            using (var cmd = new NpgsqlCommand(query, _connection, _transaction))
            {
                cmd.Parameters.AddWithValue("id", hbt.ID);
                cmd.Parameters.AddWithValue("name", hbt.name);
                cmd.Parameters.AddWithValue("user_id", hbt.users);
                cmd.ExecuteNonQuery();
            }
            foreach (String x in days)
            {
                CreateDaysOff(hbt.ID, x);
            }
        }

        public void UpdateHabit(Guid habitID, Guid userID, string name, String[] days)
        {
            string query = "UPDATE \"Habit\" SET name = @name WHERE id = @habitID AND user_id = @user_id";
            using (var cmd = new NpgsqlCommand(query, _connection, _transaction))
            {
                cmd.Parameters.AddWithValue("habitID", habitID);
                cmd.Parameters.AddWithValue("name", name);
                cmd.Parameters.AddWithValue("user_id", userID);
                cmd.ExecuteNonQuery();
            }
            foreach (String x in days)
            {
                UpdateDaysOff(habitID, x);
            }
        }

        public void UpdateDaysOff(Guid habit_id, string days)
        {
            string query = "UPDATE \"Days_off\" SET days = @days WHERE habit_id = @habit_id";
            using (var cmd = new NpgsqlCommand(query, _connection, _transaction))
            {
                cmd.Parameters.AddWithValue("habit_id", habit_id);
                cmd.Parameters.AddWithValue("days", days);
                cmd.ExecuteNonQuery();
            }
        }

        public void CreateDaysOff(Guid habit_id, string days)
        {
            string query = "INSERT INTO \"Days_off\" (id,habit_id, days) VALUES(@id,@habit_id,@days)";
            using (var cmd = new NpgsqlCommand(query, _connection, _transaction))
            {
                cmd.Parameters.AddWithValue("id", Guid.NewGuid());
                cmd.Parameters.AddWithValue("habit_id", habit_id);
                cmd.Parameters.AddWithValue("days", days);
                cmd.ExecuteNonQuery();
            }
        }

        public Habit FindByID(Guid habitID, Guid userID)
        {
            string query = "SELECT name FROM \"Habit\" WHERE id = @habitID";
            string name = "";
            using (var cmd = new NpgsqlCommand(query, _connection, _transaction))
            {
                cmd.Parameters.AddWithValue("habitID", habitID);
                using (NpgsqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        name = reader.GetString(0);
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            IGainer logGainer = new LogSuccess();
            Habit h = new Habit(habitID, userID, name, new Logs(getLogCount(habitID)), new Streak(getCurrentStreak(habitID), getLong(habitID)), logGainer);
            foreach (String x in getDaysOff(habitID))
            {
                h.AddDaysOff(x);
            }
            return h;
        }


        public List<String> getDaysOff(Guid habitID)
        {
            List<String> days = new List<String>();
            string query = "SELECT substring(days from 1 for 3) FROM \"Days_off\" where habit_id = @habitID";
            using (var cmd = new NpgsqlCommand(query, _connection, _transaction))
            {
                cmd.Parameters.AddWithValue("habitID", habitID);
                using (NpgsqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        days.Add(reader.GetString(0));
                    }
                }
            }
            return days;
        }

        public Guid FindUser(Guid id)
        {
            string query = "SELECT id from \"Habit\" where user_id = @id";
            Guid habitID;
            using (var cmd = new NpgsqlCommand(query, _connection, _transaction))
            {
                cmd.Parameters.AddWithValue("id", id);
                using (NpgsqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        habitID = reader.GetGuid(0);
                    }
                }
            }
            return habitID;
        }

        public List<Guid> cekUser(Guid id)
        {
            string query = "SELECT id from \"Habit\" where user_id = @id";
            List<Guid> habitID = new List<Guid>();
            using (var cmd = new NpgsqlCommand(query, _connection, _transaction))
            {
                cmd.Parameters.AddWithValue("id", id);
                using (NpgsqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        habitID.Add(reader.GetGuid(0));
                    }
                }
            }
            return habitID;
        }

        public int getLogCount(Guid habitID)
        {
            string query = "SELECT COUNT(*) FROM \"Logs\" WHERE habit_id = @habitID";
            int count = 0;
            using (var cmd = new NpgsqlCommand(query, _connection, _transaction))
            {
                cmd.Parameters.AddWithValue("habitID", habitID);
                using (NpgsqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        count = reader.GetInt32(0);
                    }
                }

            }
            return count;
        }

        public void AddLog(Guid habitID)
        {
            string query = "INSERT INTO \"Logs\" (id,habit_id) VALUES(@id,@habit_id)";
            using (var cmd = new NpgsqlCommand(query, _connection, _transaction))
            {
                cmd.Parameters.AddWithValue("id", Guid.NewGuid());
                cmd.Parameters.AddWithValue("habit_id", habitID);

                cmd.ExecuteNonQuery();
            }

            if (checkLogDays(habitID) == 1 && checkLogEpic(habitID) == 1)
            {
                StreakCheck(GetUserID(habitID), habitID, 1, 1);
            }
            else if (checkLogDays(habitID) == 1 && checkLogEpic(habitID) == 0)
            {
                StreakCheck(GetUserID(habitID), habitID, 1, 0);
            }
            else if (checkLogDays(habitID) != 1 && checkLogEpic(habitID) == 1)
            {
                StreakCheck(GetUserID(habitID), habitID, 0, 1);
            }

        }

        public int checkLog(Guid habitID, Guid userID)
        {
            int cek = 0;
            int data = 0;
            try
            {
                string query = "SELECT sum(log_libur) FROM \"StreakCheck\" WHERE user_id = @userID";
                using (var cmd = new NpgsqlCommand(query, _connection, _transaction))
                {
                    cmd.Parameters.AddWithValue("userID", userID);
                    using (NpgsqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            data = reader.GetInt32(0);
                        }
                    }
                }
            }
            catch
            {
                data = 0;
            }

            if (data >= 10)
            {
                cek = 1;
            }
            return cek;
        }

        public int checkLogEpic(Guid habitID)
        {
            int data2 = 0;
            string query = "SELECT log_date FROM \"Logs\" WHERE habit_id = @habitID and log_date < (current_date-interval '10 day') order by log_date desc limit 1";
            using (var cmd = new NpgsqlCommand(query, _connection, _transaction))
            {
                cmd.Parameters.AddWithValue("habitID", habitID);
                using (NpgsqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        data2 = 1;
                    }
                }
            }
            return data2;
        }

        public int checkLogDays(Guid habitID)
        {
            int cek = 0;
            string query = "SELECT log_date FROM \"Logs\" l JOIN \"Days_off\" d ON l.habit_id = d.habit_id WHERE to_char(l.log_date,'Dy') = d.days AND l.habit_id = @habitID";
            using (var cmd = new NpgsqlCommand(query, _connection, _transaction))
            {
                cmd.Parameters.AddWithValue("habitID", habitID);
                using (NpgsqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        cek = 1;
                    }
                }
            }
            return cek;
        }

        public int GetStreakCheck(Guid habitID)
        {
            int data = 0;
            string query = "SELECT SUM(log_epic) FROM \"StreakCheck\" WHERE habit_id = @habitID";
            using (var cmd = new NpgsqlCommand(query, _connection, _transaction))
            {
                cmd.Parameters.AddWithValue("habitID", habitID);
                using (NpgsqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        data = reader.GetInt32(0);
                    }
                }
            }
            return data;
        }

        public void AddBadge(Guid userID, string name, string description)
        {
            string query = "INSERT INTO \"Badge\" (id,name,description,user_id) VALUES(@id,@name,@description,@user_id)";
            using (var cmd = new NpgsqlCommand(query, _connection, _transaction))
            {
                cmd.Parameters.AddWithValue("id", Guid.NewGuid());
                cmd.Parameters.AddWithValue("name", name);
                cmd.Parameters.AddWithValue("description", description);
                cmd.Parameters.AddWithValue("user_id", userID);
                cmd.ExecuteNonQuery();
            }
        }

        public void AddStreak(Guid habitID, int currentStreak)
        {
            int longest_streak = 0;

            string query = "SELECT longest_streak FROM \"Streak\" WHERE habit_id = @habit_id ORDER BY created_at DESC LIMIT 1";
            using (var cmd = new NpgsqlCommand(query, _connection, _transaction))
            {
                cmd.Parameters.AddWithValue("habit_id", habitID);
                using (NpgsqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        longest_streak = reader.GetInt32(0);
                    }
                    reader.Close();
                }

            }
            if (currentStreak > longest_streak)
            {
                longest_streak = currentStreak;
            }

            query = "INSERT INTO \"Streak\" (id,habit_id,current_streak,longest_streak) VALUES(@id,@habit_id,@current_streak,@longest_streak)";
            using (var cmd = new NpgsqlCommand(query, _connection, _transaction))
            {
                cmd.Parameters.AddWithValue("id", Guid.NewGuid());
                cmd.Parameters.AddWithValue("habit_id", habitID);
                cmd.Parameters.AddWithValue("current_streak", currentStreak);
                cmd.Parameters.AddWithValue("longest_streak", longest_streak);

                cmd.ExecuteNonQuery();
            }

            query = "SELECT count(1) FROM \"Streak\" WHERE habit_id = @habitID";
            int count = 0;
            using (var cmd = new NpgsqlCommand(query, _connection, _transaction))
            {
                cmd.Parameters.AddWithValue("habitID", habitID);
                using (NpgsqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        count = reader.GetInt32(0);
                    }
                    reader.Close();
                }
            }
            if (count % 100 == 0)
            {
                createStreakSnapshot(habitID);
            }
            

        }

        public void GiveBadge(Guid habitID)
        {
            string Badge1 = "Dominating";
            string descBadge1 = "4+ streak";

            string Badge2 = "Workaholic";
            string descBadge2 = "Doing some works on daysoff";

            string Badge3 = "Epic Comeback";
            string descBadge3 = "10 streak after 10 days without logging";
            if (getLong(habitID) > 4)
            {
                if (CheckBadge(GetUserID(habitID), Badge1) == 0)
                {
                    AddBadge(GetUserID(habitID), Badge1, descBadge1);
                }
            }
            if (checkLog(habitID, GetUserID(habitID)) == 1)
            {
                if (CheckBadge(GetUserID(habitID), Badge2) == 0)
                {
                    AddBadge(GetUserID(habitID), Badge2, descBadge2);
                }
            }
            if (GetStreakCheck(habitID) != 0)
            {
                StreakCheck(GetUserID(habitID), habitID, 0, 1);
                if (GetStreakCheck(habitID) >= 10)
                {
                    if (CheckBadge(GetUserID(habitID), Badge3) == 0)
                    {
                        AddBadge(GetUserID(habitID), Badge3, descBadge3);
                    }
                }
            }
        }

        public void StreakCheck(Guid userID, Guid habitID, int log_libur, int log_epic)
        {

            string query = "INSERT INTO \"StreakCheck\" (id,log_libur,log_epic,user_id,habit_id) values(@id,@log_libur,@log_epic,@user_id,@habit_id)";
            using (var cmd = new NpgsqlCommand(query, _connection, _transaction))
            {
                cmd.Parameters.AddWithValue("id", Guid.NewGuid());
                cmd.Parameters.AddWithValue("log_libur", log_libur);
                cmd.Parameters.AddWithValue("log_epic", log_epic);
                cmd.Parameters.AddWithValue("user_id", userID);
                cmd.Parameters.AddWithValue("habit_id", habitID);
                cmd.ExecuteNonQuery();
            }
        }

        public Guid GetUserID(Guid habitID)
        {
            Guid userID;
            string query = "SELECT user_id FROM \"Habit\" WHERE id = @habitID";
            using (var cmd = new NpgsqlCommand(query, _connection, _transaction))
            {
                cmd.Parameters.AddWithValue("habitID", habitID);
                using (NpgsqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        userID = reader.GetGuid(0);
                    }
                }
            }
            return userID;
        }

        public List<Guid> GetAllBadge(Guid userID)
        {
            List<Guid> data = new List<Guid>();
            string query = "SELECT id FROM \"Badge\" WHERE user_id = @userID";
            using (var cmd = new NpgsqlCommand(query, _connection, _transaction))
            {
                cmd.Parameters.AddWithValue("userID", userID);
                using (NpgsqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        data.Add(reader.GetGuid(0));
                    }
                }
            }
            return data;
        }

        public Badge FindBadge(Guid userID, Guid badgeID)
        {

            string name = "";
            string desc = "";
            DateTime created_at = new DateTime();
            string query = "SELECT name,description,created_at FROM \"Badge\" WHERE user_id = @userID AND id = @badgeID";
            using (var cmd = new NpgsqlCommand(query, _connection, _transaction))
            {
                cmd.Parameters.AddWithValue("userID", userID);
                cmd.Parameters.AddWithValue("badgeID", badgeID);
                using (NpgsqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        name = reader.GetString(0);
                        desc = reader.GetString(1);
                        created_at = reader.GetDateTime(2);
                    }
                }
            }
            Badge b = new Badge(badgeID, name, desc, userID, created_at);
            return b;
        }

        private int getCurrentStreak(Guid habitID)
        {
            NpgsqlDateTime lastStreakCreatedAt = new NpgsqlDateTime(0);
            int sumCurrentStreak = 0;

            string query = "SELECT current_streak, last_streak_created_at FROM \"Streak_snapshot\" WHERE habit_id = @habitID ORDER BY created_at DESC LIMIT 1";
            using (var cmd = new NpgsqlCommand(query, _connection, _transaction))
            {
                cmd.Parameters.AddWithValue("habitID", @habitID);
                using (NpgsqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        sumCurrentStreak = reader.GetInt32(0);
                        lastStreakCreatedAt = reader.GetTimeStamp(1);
                    }
                    reader.Close();
                }
            }

            query = "SELECT coalesce(sum(current_streak),0) FROM \"Streak\" WHERE habit_id = @habitID AND created_at > @lastStreakCreatedAt";
            using (var cmd = new NpgsqlCommand(query, _connection, _transaction))
            {
                cmd.Parameters.AddWithValue("habitID", habitID);
                cmd.Parameters.AddWithValue("lastStreakCreatedAt", lastStreakCreatedAt);
                using (NpgsqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        int current = reader.GetInt32(0);
                        sumCurrentStreak += current;
                    }
                    reader.Close();
                }
            }

            if (getLastLog(habitID) == 0)
            {
                sumCurrentStreak = 0;
                query = "UPDATE \"Streak\" SET current_streak = @sumCurrentStreak where habit_id = @habitID";
                using (var cmd = new NpgsqlCommand(query, _connection, _transaction))
                {
                    cmd.Parameters.AddWithValue("sumCurrentStreak", sumCurrentStreak);
                    cmd.Parameters.AddWithValue("habitID", habitID);
                    cmd.ExecuteNonQuery();
                }
                NpgsqlDateTime CreatedAt = new NpgsqlDateTime(0);

                query = "SELECT created_at FROM \"Streak\" WHERE habit_id = @habitID ORDER BY created_at DESC LIMIT 1";
                using (var cmd = new NpgsqlCommand(query, _connection, _transaction))
                {
                    cmd.Parameters.AddWithValue("habitID", habitID);
                    using (NpgsqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            CreatedAt = reader.GetTimeStamp(0);
                        }
                        reader.Close();
                    }
                }

                query = "UPDATE \"Streak\" SET longest_streak = @sumCurrentStreak WHERE habit_id = @habitID and created_at = @created";
                using (var cmd = new NpgsqlCommand(query, _connection, _transaction))
                {
                    cmd.Parameters.AddWithValue("sumCurrentStreak", sumCurrentStreak);
                    cmd.Parameters.AddWithValue("habitID", habitID);
                    cmd.Parameters.AddWithValue("created", CreatedAt);
                    cmd.ExecuteNonQuery();
                }
            }
            return sumCurrentStreak;
        }

        public int getLong(Guid habitID)
        {
            int longest_streak = 0;
            int temp = 0;
            string query = "SELECT longest_streak FROM \"Streak\" WHERE habit_id = @habitID ORDER BY created_at DESC";
            using (var cmd = new NpgsqlCommand(query, _connection, _transaction))
            {
                cmd.Parameters.AddWithValue("habitID", habitID);
                using (NpgsqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader.GetInt32(0) == 1)
                        {
                            int sum = reader.GetInt32(0);
                            longest_streak += sum;
                        }
                        else
                        {
                            if (longest_streak > temp)
                            {
                                temp = longest_streak;
                            }

                            longest_streak = 0;
                        }
                    }
                }
            }
            if (temp == 0)
            {
                temp = getCurrentStreak(habitID);
            }
            return temp;
        }

        public int CheckBadge(Guid userID, string name)
        {
            int cek = 0;
            string query = "SELECT name FROM \"Badge\" WHERE user_id = @userID AND name = @name";
            using (var cmd = new NpgsqlCommand(query, _connection, _transaction))
            {
                cmd.Parameters.AddWithValue("userID", userID);
                cmd.Parameters.AddWithValue("name", name);
                using (NpgsqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        cek = 1;
                    }
                }
            }
            return cek;
        }

        public void createStreakSnapshot(Guid habitID)
        {
            string query = "SELECT id,created_at FROM \"Streak\" WHERE habit_id = @habitID ORDER BY created_at DESC LIMIT 1";
            Guid lastStreakId;
            int longestStreak = 0;
            int currentStreak = 0;
            NpgsqlDateTime lastStreakCreatedAt;
            using (var cmd = new NpgsqlCommand(query, _connection, _transaction))
            {
                cmd.Parameters.AddWithValue("habitID", habitID);
                using (NpgsqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        lastStreakId = reader.GetGuid(0);
                        lastStreakCreatedAt = reader.GetTimeStamp(1);
                    }
                    else
                    {
                        throw new Exception("last current streak not found");
                    }
                    reader.Close();
                }
            }

            query = "SELECT current_streak FROM \"Streak\" WHERE habit_id = @habitID ORDER BY created_at DESC LIMIT 1";
            using (var cmd = new NpgsqlCommand(query, _connection, _transaction))
            {
                cmd.Parameters.AddWithValue("habitID", habitID);
                using (NpgsqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        currentStreak = reader.GetInt32(0);
                    }
                    reader.Close();
                }
            }
            if (currentStreak != 0)
            {
                currentStreak = getCurrentStreak(habitID);
            }
            longestStreak = getLong(habitID);
            if (currentStreak > longestStreak)
            {
                longestStreak = currentStreak;
            }

            query = "INSERT INTO \"Streak_snapshot\" (id,habit_id,current_streak,longest_streak,last_streak_id,last_streak_created_at) VALUES(@id,@habit_id,@current_streak,@longest_streak,@last_streak_id,@last_streak_created_at)";
            using (var cmd = new NpgsqlCommand(query, _connection, _transaction))
            {
                cmd.Parameters.AddWithValue("id", Guid.NewGuid());
                cmd.Parameters.AddWithValue("habit_id", habitID);
                cmd.Parameters.AddWithValue("current_streak", currentStreak);
                cmd.Parameters.AddWithValue("longest_streak", longestStreak);
                cmd.Parameters.AddWithValue("last_streak_id", lastStreakId);
                cmd.Parameters.AddWithValue("last_streak_created_at", lastStreakCreatedAt);
                cmd.ExecuteNonQuery();
            }
        }

        public void DeleteHabit(Guid habitID, Guid userID)
        {
            string query = "DELETE FROM \"Habit\" WHERE id = @habitID AND user_id = @userID";
            using (var cmd = new NpgsqlCommand(query, _connection, _transaction))
            {
                cmd.Parameters.AddWithValue("habitID", habitID);
                cmd.Parameters.AddWithValue("userID", userID);

                cmd.ExecuteNonQuery();
            }
        }

        public int getLastLog(Guid habitID)
        {
            int data = 0;
            string query = "SELECT log_date from \"Logs\" where habit_id = @habitID and ((to_char(log_date,'DD')= to_char(current_date-1,'dd')) OR (to_char(log_date,'DD')= to_char(current_date,'dd'))) ORDER BY log_date DESC LIMIT 1";
            using (var cmd = new NpgsqlCommand(query, _connection, _transaction))
            {
                cmd.Parameters.AddWithValue("habitID", habitID);
                NpgsqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    data = 1;
                }
                reader.Close();
            }
            if (data == 0)
            {
                query = "SELECT days from \"Days_off\" where habit_id = @habitID and to_char(current_date-1,'Dy') = days";
                using (var cmd = new NpgsqlCommand(query, _connection, _transaction))
                {
                    cmd.Parameters.AddWithValue("habitID", habitID);
                    NpgsqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        data = 1;
                    }
                    reader.Close();
                }
            }

            return data;
        }


    }
}