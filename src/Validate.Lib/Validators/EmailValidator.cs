
using System;
using System.Globalization;

namespace FormatValidator.Validators
{
    internal class EmailValidator : ValidationEntry
    {
        public override bool IsValid(string toCheck)
        {
            bool isValid = true;

            try
            {
                var addr = new System.Net.Mail.MailAddress(toCheck);
                if (!addr.Address.Equals(toCheck))
                {
                    base.Errors.Add(new ValidationError(0, string.Format("'{0}' is not a valid email.", toCheck)));
                    isValid = false;
                }
            }
            catch (Exception ex)
            {
                base.Errors.Add(new ValidationError(0, string.Format("'{0}' is not a valid email.", toCheck)));
                isValid = false;
            }

            return isValid;
        }
    }
}
