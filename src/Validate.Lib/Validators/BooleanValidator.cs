
using System;
using System.Globalization;

namespace FormatValidator.Validators
{
    internal class BooleanValidator : ValidationEntry
    {
        public override bool IsValid(string toCheck)
        {
            bool temp;
            bool isValid = Boolean.TryParse(toCheck, out temp) || toCheck.Equals("1") || toCheck.Equals("0");

            if (!isValid)
            {
                base.Errors.Add(new ValidationError(0, string.Format("Could not convert '{0}' to a boolean.", toCheck)));
            }

            return isValid;
        }
    }
}
