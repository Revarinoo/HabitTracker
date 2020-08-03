using System;

namespace HabitTracker
{
    public interface UnitOfWork : IDisposable
    {
        void Commit();
        void Rollback();
    }
}