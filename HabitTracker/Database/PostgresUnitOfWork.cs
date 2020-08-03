using System;
using System.Collections.Generic;

using Npgsql;
using NpgsqlTypes;

using System.Text.Json;
using System.Text.Json.Serialization;

namespace HabitTracker.Database{
    public class PostgresUnitOfWork : UnitOfWork
  {
    private NpgsqlConnection _connection;
    private NpgsqlTransaction _transaction;

    private HabitRepository _habitRepository;
    private UserRepository _userRepository;

    public HabitRepository HabitRepo
    {
      get
      {
        if (_habitRepository == null)
        {
          _habitRepository = new HabitRepository(_connection, _transaction);
        }
        return _habitRepository;
      }
    }

    public UserRepository UserRepo
    {
      get
      {
        if (_userRepository == null)
        {
          _userRepository = new UserRepository(_connection, _transaction);
        }
        return _userRepository;
      }
    }

    public PostgresUnitOfWork()
    {
      _connection = new NpgsqlConnection("Host=localhost;Username=Habit;Password=revarino123;Database=HabitTracker;Port=5432");
      _connection.Open();
      _transaction = _connection.BeginTransaction();
    }

    public void Commit()
    {
      _transaction.Commit();
    }

    public void Rollback()
    {
      _transaction.Rollback();
    }

    private bool disposed = false;
    public void Dispose()
    {
      Dispose(true);
      GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
      if (!this.disposed)
      {
        if (disposing)
        {
          _connection.Close();
        }

        disposed = true;
      }
    }


  }
}