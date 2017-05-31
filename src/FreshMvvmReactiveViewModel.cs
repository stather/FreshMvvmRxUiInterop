using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using FreshMvvm;
using ReactiveUI;

namespace FreshMvvmRxUiInterop
{
    public abstract class FreshMvvmReactiveViewModel : FreshBasePageModel, IReactiveNotifyPropertyChanged<IReactiveObject>, IHandleObservableErrors, IReactiveObject
    {
        readonly FreshMvvmReactiveObject _reactiveObject = new FreshMvvmReactiveObject();

        public IDisposable SuppressChangeNotifications()
        {
            var suppressor = _reactiveObject.SuppressChangeNotifications();

            return new DisposableAction(() =>
            {
                suppressor.Dispose();
            });
        }

        public IObservable<IReactivePropertyChangedEventArgs<IReactiveObject>> Changing => _reactiveObject.Changing;

        public IObservable<IReactivePropertyChangedEventArgs<IReactiveObject>> Changed => _reactiveObject.Changed;

        public IObservable<Exception> ThrownExceptions => _reactiveObject.ThrownExceptions;

        public event PropertyChangingEventHandler PropertyChanging
        {
            add => _reactiveObject.PropertyChanging += value;
            remove => _reactiveObject.PropertyChanging -= value;
        }
        public void RaisePropertyChanging(PropertyChangingEventArgs args)
        {
            _reactiveObject.RaisePropertyChanging(propertyName: args.PropertyName);
        }

        public void RaisePropertyChanged(PropertyChangedEventArgs args)
        {
            _reactiveObject.RaisePropertyChanged(propertyName: args.PropertyName);
            base.RaisePropertyChanged(args.PropertyName);
        }

        public bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            var original = storage;
            this.RaiseAndSetIfChanged(ref storage, value, propertyName);

            return !EqualityComparer<T>.Default.Equals(original, value);
        }
    }
}