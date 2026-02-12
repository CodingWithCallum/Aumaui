using AumauiCL.Models.Core;

namespace AumauiCL.Models.System
{
    public class EmailTemplate : BaseAggregate
    {
        private string _subject = string.Empty;
        private string _body = string.Empty;
        private string _recipient = string.Empty;

        public string Subject
        {
            get => _subject;
            set => SetField(ref _subject, value);
        }

        public string Body
        {
            get => _body;
            set => SetField(ref _body, value);
        }

        public string Recipient
        {
            get => _recipient;
            set => SetField(ref _recipient, value);
        }
    }
}
