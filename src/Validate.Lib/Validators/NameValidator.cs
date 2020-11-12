
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
            bool isValid = string.IsNullOrWhiteSpace(toCheck) || Name.Equals(toCheck);
            if (!isValid)
            {
                base.Errors.Add(new ValidationError(0, string.Format("{0}|{1}", Name, toCheck)));
            }
            return isValid;
        }
    }
}
