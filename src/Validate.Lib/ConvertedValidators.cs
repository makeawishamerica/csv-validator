
namespace FormatValidator
{
    using System.Collections.Generic;
    using Validate.Lib;
    using Validators;

    internal class ConvertedValidators
    {
        public ConvertedValidators()
        {
            Columns = new Dictionary<int, List<IValidator>>();
        }

        public Dictionary<int, List<IValidator>> Columns { get; private set; }

        public string RowSeperator { get; set; }

        public string ColumnSeperator { get; set; }

        public bool HasHeaderRow { get; set; }

        public ConnectionStrings ConnectionStrings { get; set; }

        public string ChapterId { get; set; }
    }
}
