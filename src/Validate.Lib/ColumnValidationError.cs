
namespace FormatValidator
{
    using System.Collections.Generic;

    public class ColumnValidationError
    {
        private string _content;
        private List<ValidationError> _errors;

        public ColumnValidationError()
        {
            _errors = new List<ValidationError>();
        }

        public List<ValidationError> Errors
        {
            get { return _errors; }
        }

        public string Content
        {
            get { return _content; }
            set { _content = value; }
        }
    }
}
