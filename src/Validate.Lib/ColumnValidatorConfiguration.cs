
namespace FormatValidator
{
    public class ColumnValidatorConfiguration
    {
        public string Name { get; set; }

        public bool Unique { get; set; }

        public int MaxLength { get; set; }

        public string Pattern { get; set; }

        public bool IsNumeric { get; set; }

        public bool IsRequired { get; set; }

        public string Code { get; set; }

        public int ReferenceCol { get; set; }

        public bool IsUnique { get; set; }

        public bool IsCurrency { get; set; }

        public bool IsDate { get; set; }

        public bool IsBoolean { get; set; }

        public bool IsEmail { get; set; }

        public bool IsConstituentLookup { get; set; }

        public bool IsInterestLookup { get; set; }
    }
}
