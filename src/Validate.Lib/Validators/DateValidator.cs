
using System;
using System.Globalization;

namespace FormatValidator.Validators
{
    internal class DateValidator : ValidationEntry
    {
        public override bool IsValid(string toCheck)
        {
            DateTime temp;
            bool isValid = DateTime.TryParse(toCheck, out temp);

            if (!isValid)
            {
                base.Errors.Add(new ValidationError(0, string.Format("Could not convert '{0}' to a date.", toCheck)));
            }

            return isValid;
        }
    }
}
