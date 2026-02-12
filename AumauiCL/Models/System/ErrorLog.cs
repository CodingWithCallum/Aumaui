using AumauiCL.Interfaces;
using AumauiCL.Models.Core;
using SQLite;

namespace AumauiCL.Models.System
{
    public class ErrorLog : BaseAggregate, IEntity
    {
        private int _id;
        private string _errorMessage = string.Empty;
        private string _stackTrace = string.Empty;
        private DateTime _timestamp;
        private string _user = string.Empty;

        [PrimaryKey, AutoIncrement]
        public int ID
        {
            get => _id;
            set => SetField(ref _id, value);
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetField(ref _errorMessage, value);
        }

        public string StackTrace
        {
            get => _stackTrace;
            set => SetField(ref _stackTrace, value);
        }

        public DateTime Timestamp
        {
            get => _timestamp;
            set => SetField(ref _timestamp, value);
        }

        public string User
        {
            get => _user;
            set => SetField(ref _user, value);
        }
    }
}
