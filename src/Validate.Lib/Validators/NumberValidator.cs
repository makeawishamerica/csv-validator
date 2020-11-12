
namespace FormatValidator.Validators
{
    internal class NumberValidator : ValidationEntry
    {
        public override bool IsValid(string toCheck)
        {
            int parsed = 0;
            bool isValid = string.IsNullOrWhiteSpace(toCheck) || int.TryParse(toCheck, out parsed);

            if (!isValid)
            {
                base.Errors.Add(new ValidationError(0, "Invalid value: value must be a whole number"));
            }

            return isValid;
        }
    }
}
