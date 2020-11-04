using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("ValidateTests")]
namespace FormatValidator
{
    using System;
    using System.Collections.Generic;
    using Validate.Lib;
    using Validators;

    /// <summary>
    /// A fast configurable file Validor.
    /// </summary>
    /// <remarks>
    /// The intention is that instances of Validator can be reused across
    /// multiple sources.
    /// </remarks>
    public class Validator
    {
        private RowValidator _rowValidator;
        private ColumnValidator _colValidator;
        private string _rowSeperator;
        private int _totalRowsChecked;
        private bool _hasHeaderRow;
        private ConnectionStrings _connectionStrings;
        private string _chapterId;

        /// <summary>
        /// Gets or Sets the header.
        /// </summary>
        public string[] Header { get; set; }

        /// <summary>
        /// Initialises a new instance of Validator
        /// </summary>
        internal Validator()
        {
            _rowValidator = new RowValidator();
            _colValidator = new ColumnValidator();
            _rowSeperator = "\r\n";
        }

        /// <summary>
        /// Creates a new instance of Validator from the <paramref name="json" /> configuration.
        /// </summary>
        /// <param name="json">The validation configuration.</param>
        /// <returns>A new configured Validator</returns>
        public static Validator FromJson(string json)
        {
            ValidatorConfiguration configuration = new JsonReader().Read(json);

            return FromConfiguration(configuration);
        }

        /// <summary>
        /// Creates a new Validator instance from the <paramref name="configuration" />.
        /// </summary>
        /// <param name="configuration">The validation configuration</param>
        /// <returns>A new configured validator</returns>
        public static Validator FromConfiguration(ValidatorConfiguration configuration)
        {
            ConfigurationConvertor converter = new ConfigurationConvertor(configuration);
            ConvertedValidators converted = converter.Convert();

            Validator validator = new Validator();
            validator.SetColumnSeperator(converted.ColumnSeperator);
            validator.SetRowSeperator(converted.RowSeperator);
            validator.SetConnectionStrings(converted.ConnectionStrings);
            validator.SetChapterId(converted.ChapterId);
            validator.TransferConvertedColumns(converted);
            validator._hasHeaderRow = converted.HasHeaderRow;

            return validator;
        }

        /// <summary>
        /// Validate the provided <paramref name="reader" />.
        /// </summary>
        /// <param name="reader">The data source to validate</param>
        /// <returns>An enumerable of <see cref="RowValidationError" /> </returns>
        public IEnumerable<RowValidationError> Validate(ISourceReader reader)
        {
            foreach (string line in reader.ReadLines(_rowSeperator))
            {
                _totalRowsChecked++;

                if (IsHeaderRow())
                {
                }
                else if (!_rowValidator.IsValid(line))
                {
                    RowValidationError error = _rowValidator.GetError();
                    error.Row = _totalRowsChecked;
                    _rowValidator.ClearErrors();

                    yield return error;
                }
            }
        }

        /// <summary>
        /// Validate the provided <paramref name="reader" />.
        /// </summary>
        /// <param name="reader">The data source to validate</param>
        /// <returns>An enumerable of <see cref="ColumnValidationError" /> </returns>
        public IEnumerable<ColumnValidationError> ValidateCols(ISourceReader reader)
        {
            foreach (string line in reader.ReadLines(_rowSeperator))
            {
                Header = _rowValidator.GetHeader(line);

                if (!_colValidator.IsValid(line))
                {
                    ColumnValidationError error = _colValidator.GetError();
                    _colValidator.ClearErrors();

                    yield return error;
                }

                break; // only process the first line
            }
        }

        /// <summary>
        /// Change the column seperator
        /// </summary>
        /// <param name="seperator">The seperator</param>
        public void SetColumnSeperator(string seperator)
        {
            if (string.IsNullOrEmpty(seperator))
            {
                _rowValidator.ColumnSeperator = ",";
                _colValidator.ColumnSeperator = ",";
            }
            else
            {
                _rowValidator.ColumnSeperator = seperator;
                _colValidator.ColumnSeperator = seperator;
            }
        }

        /// <summary>
        /// Change the row seperator
        /// </summary>
        /// <param name="rowSeperator"></param>
        public void SetRowSeperator(string rowSeperator)
        {
            if (!string.IsNullOrEmpty(rowSeperator))
            {
                _rowSeperator = rowSeperator;
            }
        }

        /// <summary>
        /// Change the connection strings
        /// </summary>
        /// <param name="seperator">The seperator</param>
        public void SetConnectionStrings(ConnectionStrings conn)
        {
            if (conn != null)
            {
                _connectionStrings = conn;
            }
        }

        /// <summary>
        /// The set chapter id.
        /// </summary>
        /// <param name="chapterId">The chapter id.</param>
        public void SetChapterId(string chapterId)
        {
            if (!String.IsNullOrEmpty(chapterId))
            {
                _chapterId = chapterId;
            }
        }

        internal List<ValidatorGroup> GetColumnValidators()
        {
            return _rowValidator.GetColumnValidators();
        }

        private void TransferConvertedColumns(ConvertedValidators converted)
        {
            foreach (KeyValuePair<int, List<IValidator>> column in converted.Columns)
            {
                foreach (IValidator columnValidator in column.Value)
                {
                    if (columnValidator.GetType().Equals(typeof(NameValidator)))
                    {
                        _colValidator.AddColumnValidator(column.Key, columnValidator);
                    }
                    else
                    {
                        _rowValidator.AddColumnValidator(column.Key, columnValidator);
                    }
                }
            }
        }

        private bool IsHeaderRow() => _hasHeaderRow && _totalRowsChecked == 1;

        /// <summary>
        /// Total number of rows that were checked in the last validation.
        /// </summary>
        public int TotalRowsChecked
        {
            get { return _totalRowsChecked; }
        }
    }
}
