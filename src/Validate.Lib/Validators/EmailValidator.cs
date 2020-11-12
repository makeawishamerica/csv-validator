
using System;
using System.Globalization;

namespace FormatValidator.Validators
{
    internal class EmailValidator : ValidationEntry
    {
        public override bool IsValid(string toCheck)
        {
            bool isValid = true;
            string code = "Invalid value: Field is not a valid email " +
                "('@' not present, '.' not present or leading/trailing spaces present";

            try
            {
                var addr = new System.Net.Mail.MailAddress(toCheck);
                if (!addr.Address.Equals(toCheck))
                {
                    base.Errors.Add(new ValidationError(0, code));
                    isValid = false;
                }
            }
            catch (Exception ex)
            {
                base.Errors.Add(new ValidationError(0, code));
                isValid = false;
            }

            return isValid;
        }
    }
}
