using System;

namespace FreshMvvmRxUiInterop
{
    public class Disposer<T>
    {
        private readonly Runner<T> _runner;
        private readonly IObserver<T> _observer;

        public Disposer(Runner<T> runner, IObserver<T> observer)
        {
            _runner = runner;
            _observer = observer;
        }

        public void Dispose()
        {
            _runner.RemoveSubscriber(_observer);
        }
    }
}