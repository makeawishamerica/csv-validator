
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
                base.Errors.Add(new ValidationError(0, "Value is not Boolean (1/TRUE or 0/FALSE)"));
            }

            return isValid;
        }
    }
}
