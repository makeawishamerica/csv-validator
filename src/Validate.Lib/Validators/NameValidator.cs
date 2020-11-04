
namespace FormatValidator.Validators
{
    internal class NameValidator : ValidationEntry
    {
        private string Name { get; set; }

        public NameValidator(string name)
        {
            Name = name;
        }

        public override bool IsValid(string toCheck)
        {
            bool isValid = !string.IsNullOrWhiteSpace(toCheck) || Name.Equals(toCheck);
            if (!isValid)
            {
                base.Errors.Add(new ValidationError(0, string.Format("Column name '{0}' was not equal to {1}.", toCheck, Name)));
            }
            return isValid;
        }
    }
}
