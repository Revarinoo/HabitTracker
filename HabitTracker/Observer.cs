using System;

namespace HabitTracker
{
    public interface IObserver<T>
    {
        void Update(T e);
    }

    public interface IObservable<T>
    {
        void Attach(IObserver<T> obs);
        void Broadcast(T e);
    }
}