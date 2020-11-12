
namespace FormatValidator.Validators
{
    using System.Collections.Generic;

    // this class needs a persitant error management, so that it can last through multiple
    // row validations.

    internal class UniqueValidator : ValidationEntry
    {
        private List<string> _entries;

        private int? Column { get; set; }

        public UniqueValidator(int? refCol = null)
        {
            _entries = new List<string>();
            Column = refCol;
        }

        public override bool IsValid(string toCheck)
        {
            bool isValid = !_entries.Contains(toCheck);

            if (isValid)
            {
                _entries.Add(toCheck);
            }
            else
            {
                ValidationError error = new ValidationError(0, string.Format("{0}|{1}", Column, toCheck));
                error.ErrorType = ErrorType.Duplicate;

                base.Errors.Add(error);
            }

            return isValid;
        }
    }
}
