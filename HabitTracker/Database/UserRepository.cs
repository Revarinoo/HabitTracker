using System;

using Npgsql;
using NpgsqlTypes;

namespace HabitTracker.Database
{
    public class UserRepository : IUserRepository
    {
        private NpgsqlConnection _connection;
        private NpgsqlTransaction _transaction;

        public UserRepository(NpgsqlConnection connection, NpgsqlTransaction transaction)
        {
            _connection = connection;
            _transaction = transaction;
        }

        public User FindById(Guid id)
        {
            string query = @"select name from ""user"" where id = @id";

            using (var cmd = new NpgsqlCommand(query, _connection, _transaction))
            {
                cmd.Parameters.AddWithValue("id", id);
                using (NpgsqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        string name = reader.GetString(0);
                        
                        User u = new User(id, name);
                        return u;
                    }
                }
            }

            return null;
        }


        public void Create(User user)
        {
            string query = "INSERT INTO \"user\" (id, name) VALUES(@id, @name)";
            using (var cmd = new NpgsqlCommand(query, _connection, _transaction))
            {
                cmd.Parameters.AddWithValue("id", user.ID);
                cmd.Parameters.AddWithValue("name", user.Name);
                cmd.ExecuteNonQuery();
            }
        }
    }
}