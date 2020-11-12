
namespace FormatValidator.Validators
{
    internal class NotNullableValidator : ValidationEntry
    {
        public override bool IsValid(string toCheck)
        {
            bool isValid = !string.IsNullOrEmpty(toCheck);
            if (!isValid)
            {
                base.Errors.Add(new ValidationError(0, "Field is Blank"));
            }

            return isValid;
        }
    }
}
