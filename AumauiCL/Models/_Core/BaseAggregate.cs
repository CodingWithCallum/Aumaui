using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AumauiCL.Models.Core
{
    public abstract class BaseAggregate : INotifyPropertyChanged
    {
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
