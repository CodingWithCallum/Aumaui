using System.ComponentModel;
using System.Runtime.CompilerServices;
using AumauiCL.Interfaces;

namespace AumauiCL.Models.Core
{
    public abstract class BaseAggregate : INotifyPropertyChanged, ISyncable
    {
        private bool _isSynced;
        private bool _wasFailed;
        private string _failReason = string.Empty;

        public bool IsSynced
        {
            get => _isSynced;
            set => SetField(ref _isSynced, value);
        }

        public bool WasFailed
        {
            get => _wasFailed;
            set => SetField(ref _wasFailed, value);
        }

        public string FailReason
        {
            get => _failReason;
            set => SetField(ref _failReason, value);
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected virtual void InvalidateComputedProperty(string? propertyName)
        {
            // Override in derived classes to handle specific computed properties
        }

        protected bool SetField<T>(ref T field, T value, string? relatedComputedProperty = null, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;

            field = value;
            OnPropertyChanged(propertyName);

            if (!string.IsNullOrEmpty(relatedComputedProperty))
            {
                InvalidateComputedProperty(relatedComputedProperty);
            }

            return true;
        }
    }
}
