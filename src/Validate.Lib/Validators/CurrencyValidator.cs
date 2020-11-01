
using System.Globalization;

namespace FormatValidator.Validators
{
    internal class CurrencyValidator : ValidationEntry
    {
        public override bool IsValid(string toCheck)
        {
            float parsed = 0;
            bool isValid = float.TryParse(toCheck, NumberStyles.Currency, CultureInfo.GetCultureInfo("en-US"), out parsed);

            if (!isValid)
            {
                base.Errors.Add(new ValidationError(0, string.Format("Could not convert '{0}' to a currency.", toCheck)));
            }

            return isValid;
        }
    }
}
