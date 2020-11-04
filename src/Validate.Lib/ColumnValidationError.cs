
namespace FormatValidator
{
    using System.Collections.Generic;

    public class ColumnValidationError
    {
        private int _col;
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

        public int Column
        {
            get { return _col; }
            set { _col = value; }
        }

        public string Content
        {
            get { return _content; }
            set { _content = value; }
        }
    }
}
