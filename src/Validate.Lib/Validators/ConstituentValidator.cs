
using System.Data;
using System.Globalization;
using Validate.Lib;

namespace FormatValidator.Validators
{
    internal class ConstituentValidator : ValidationEntry
    {
        public DbaseManager DbaseManager { get; set; }

        public ConstituentValidator(string connectionString) : base()
        {
            DbaseManager = new DbaseManager(connectionString);
            DbaseManager.IsDebug = true;
            DbaseManager.Table = "[LO].[Constituent]";
        }

        public override bool IsValid(string toCheck)
        {
            int parsed = 0;
            bool isNumeric = int.TryParse(toCheck, out parsed);
            string[] fields = new string[] { "PrimaryEmail" };
            string[] wheres = new string[] { "Active = 'REMOVED'", string.Format("PrimaryEmail = '{0}'", toCheck) };

            if (isNumeric)
            {
                fields = new string[] { "ConsId" };
                wheres = new string[] { "Active = 'REMOVED'", string.Format("ConsId = '{0}'", toCheck) };
            }

            DataTable table = DbaseManager.Download(fields, wheres);

            if (table.Rows.Count > 0)
            {
                base.Errors.Add(new ValidationError(0, string.Format("Inactive constitutent '{0}'.", toCheck)));
                return false;
            }

            return true;
        }
    }
}
