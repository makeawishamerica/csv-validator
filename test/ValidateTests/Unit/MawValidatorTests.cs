
namespace FormatValidatorTests.Unit
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net.Mail;
    using FormatValidator;
    using FormatValidator.Validators;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Newtonsoft.Json;

    [TestClass]
    public class MawValidatorTests
    {
        [TestMethod]
        public void Validator_ReturnsAValidator()
        {
            string[] files = new string[] { @"Data/045-Interest-sallen@wish.org.csv" };

            foreach (var file in files)
            {
                List<RowValidationError> rowErrors = new List<RowValidationError>();
                List<ColumnValidationError> colErrors = new List<ColumnValidationError>();

                string[] parts = Path.GetFileName(file).Replace(".csv", "").Split('-');

                if (parts.Length == 3)
                {
                    string chid = parts[0];
                    string function = parts[1];
                    string email = parts[2];
                    string[] header = new string[] { };

                    try
                    {
                        string JSON = System.IO.File.ReadAllText(@"Data/Configuration/maw-" + function.ToLower() + "-config.json");

                        // Add the chapter id to the json string
                        var config = JsonConvert.DeserializeObject<IDictionary<string, object>>(JSON);
                        config.Add("chapterId", chid);
                        JSON = JsonConvert.SerializeObject(config);

                        Validator validator = Validator.FromJson(JSON);
                        FileSourceReader reader = new FileSourceReader(file);

                        colErrors = new List<ColumnValidationError>(validator.ValidateCols(reader));
                        rowErrors = new List<RowValidationError>(validator.Validate(reader));

                        header = validator.Header;

                        string emailFrom = "no-reply@apps.wish.org";
                        string emailTo = email;
                        string subject = String.Format("LO Import {0} File Validation", function);
                        string content = "";

                        MailMessage message = new MailMessage();
                        message.From = new MailAddress(emailFrom);
                        message.To.Add(emailTo);
                        message.Subject = subject;

                        if (rowErrors.Count == 0 && colErrors.Count == 0)
                        {
                            content = String.Format("The attached file {0}-{1}-{2}.csv has been validated successfully with no errors.  " +
                                "Please proceed with the import into LO and remember to check the count of records " +
                                "imported in LO with the count of records in the file.", chid, function, email);

                            message.Body = content;
                            message.Attachments.Add(new Attachment(file));
                        }
                        else
                        {
                            string standardsErrorLog = GenerateStandardsErrorLog(rowErrors, header);
                            string exceptionErrorLog = GenerateExceptionErrorLog(colErrors);


                            content = String.Format("The attached file {0}-{1}-{2}.csv has been validated and there are errors.  " +
                                "The details of the errors that need to be corrected are attached.  " +
                                "If you need help resolving these errors, please raise a ticket at help.wish.org for the integrated fundraising team.  " +
                                "Once the errors are corrected, please drop the updated file in the validation location.", chid, function, email);

                            message.Body = content;
                            message.Attachments.Add(new Attachment(file));

                            if (!String.IsNullOrEmpty(standardsErrorLog))
                            {
                                string standardsFileName = String.Format(@"Data/{0}-{1}-{2}-Standards.csv", chid, function, email);
                                File.WriteAllText(standardsFileName, standardsErrorLog);
                                message.Attachments.Add(new Attachment(standardsFileName));
                            }
                            if (!String.IsNullOrEmpty(exceptionErrorLog))
                            {
                                string exceptionFileName = String.Format(@"Data/{0}-{1}-{2}-ColumnExceptions.csv", chid, function, email);
                                File.WriteAllText(exceptionFileName, exceptionErrorLog);
                                message.Attachments.Add(new Attachment(exceptionFileName));
                            }
                        }

                        //SmtpClient smtp = new SmtpClient("wish-org.mail.protection.outlook.com");
                        SmtpClient smtp = new SmtpClient("192.168.100.34");
                        smtp.Send(message);
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
        }

        /// <summary>
        /// The generate standards error log.
        /// </summary>
        /// <param name="errors">The errors.</param>
        /// <returns>The result.</returns>
        private string GenerateStandardsErrorLog(List<RowValidationError> errors, string[] header)
        {
            string content = "COLUMN,Row,Rule Code\n";

            for (var i = 0; i < errors.Count; i++)
            {
                string row = errors[i].Row.ToString();
                foreach (var error in errors[0].Errors)
                {
                    string colName = header[error.Column - 1];
                    content += String.Format("{0},{1},{2}\n", colName, row, error.Message);
                }
            }

            return content;
        }

        /// <summary>
        /// The generate column exception error log.
        /// </summary>
        /// <param name="errors">The errors.</param>
        /// <returns>The result.</returns>
        private string GenerateExceptionErrorLog(List<ColumnValidationError> errors)
        {
            string content = "Column,Expected Header Value,Exception Header Value\n";

            foreach (var error in errors[0].Errors)
            {
                string colName = GetExcelColEquiv(error.Column);
                string[] parts = error.Message.Split('|');
                content += String.Format("{0},{1},{2}\n", colName, parts[0], parts[1]);
            }

            return content;
        }

        /// <summary>
        /// Get Excel Column Equivalent
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        private string GetExcelColEquiv(int column)
        {
            int dividend = column;
            string columnName = String.Empty;
            int modulo;

            while (dividend > 0)
            {
                modulo = (dividend - 1) % 26;
                columnName = Convert.ToChar(65 + modulo).ToString() + columnName;
                dividend = (int)((dividend - modulo) / 26);
            }

            return columnName;
        }
    }
}
