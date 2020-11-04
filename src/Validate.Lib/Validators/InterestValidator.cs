
using System.Data;
using System.Globalization;
using Validate.Lib;

namespace FormatValidator.Validators
{
    internal class InterestValidator : ValidationEntry
    {
        public DbaseManager DbaseManager { get; set; }

        public string ChapterId { get; set; }

        public InterestValidator(string connectionString, string chid) : base()
        {
            DbaseManager = new DbaseManager(connectionString);
            DbaseManager.IsDebug = true;
            DbaseManager.Table = "[LO].[Interest]";

            ChapterId = chid;
        }

        public override bool IsValid(string toCheck)
        {
            bool isValidGroup = true;
            bool isValidChapterGroup = true;

            string[] fields = new string[] { "ID", "Description", "SUBSTRING(Description,6,3) As ChapterId" };
            string[] wheres = new string[] { "LEFT(Description,4) = 'SFMC'", string.Format("ID = '{0}'", toCheck) };

            DataTable table = DbaseManager.Download(fields, wheres);

            if (table.Rows.Count == 0)
            {
                base.Errors.Add(new ValidationError(0, "This is not a valid Interest Group"));
                isValidGroup = false;
            }
            else
            {
                if (!ChapterId.Equals(table.Rows[0]["ChapterId"]))
                {
                    base.Errors.Add(new ValidationError(0, "This is not a valid Interest Group for your chapter"));
                    isValidChapterGroup = false;
                }
            }

            return (isValidGroup && isValidChapterGroup);

        }
    }
}
