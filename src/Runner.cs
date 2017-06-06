using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;


namespace FreshMvvmRxUiInterop
{
    public class Runner
    {
        public static Runner<T> Create<T>(Func<T> func)
        {
            return new Runner<T>(func);
        }
    }

    public class Runner<T> : IObservable<T>
    {
        private readonly Func<T> _func;
        private readonly List<IObserver<T>> _observers = new List<IObserver<T>>();
        private T _currentValue;
        private bool _currentValueIsValid = false;
        private Subject<Exception> _thrownExceptions { get; set; }
        public IObservable<Exception> ThrownExceptions { get; set; }


        internal Runner(Func<T> func)
        {
            _func = func;
            _thrownExceptions = new Subject<Exception>();
            ThrownExceptions = _thrownExceptions.AsObservable();
        }

        public void Execute()
        {
            try
            {
                Task.Run(_func).ContinueWith(task =>
                {
                    var a = task.Result;
                    _currentValue = a;
                    _currentValueIsValid = true;
                    foreach (var observer in _observers)
                    {
                        try
                        {
                            observer.OnNext(a);

                        }
                        catch (Exception)
                        {
                            RemoveSubscriber(observer);
                        }
                    }
                });

            }
            catch (Exception e)
            {
                _thrownExceptions.OnNext(e);
            }
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            _observers.Add(observer);
            var d = new Disposer<T>(this, observer);
            if (_currentValueIsValid)
            {
                observer.OnNext(_currentValue);
            }
            return Disposable.Create(d.Dispose);
        }

        public void RemoveSubscriber(IObserver<T> observer)
        {
            try
            {
                _observers.Remove(observer);

            }
            catch
            {
            }
        }
    }

}