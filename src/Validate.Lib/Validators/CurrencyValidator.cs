
using System.Globalization;

namespace FormatValidator.Validators
{
    internal class CurrencyValidator : ValidationEntry
    {
        public override bool IsValid(string toCheck)
        {
            float parsed = 0;
            bool isValid = float.TryParse(toCheck, NumberStyles.Currency, CultureInfo.GetCultureInfo("en-US"), out parsed);


            // Above expression didn't check to see if dollar sign is present, adding below condition to check for this:
            if (isValid && toCheck[0] != '$')
            {
                isValid = false;
            }


            if (!isValid)
            {
                base.Errors.Add(new ValidationError(0, "Invalid value: value must be currency"));
            }

            return isValid;
        }
    }
}
