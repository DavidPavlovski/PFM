using CsvHelper.Configuration;
using ISO._4217;
using LanguageExt.Common;
using PFM.DataAccess.Entities;
using PFM.Enums;
using System.Globalization;
using System.Text.RegularExpressions;

namespace PFM.Mapping.CSVMapping
{
    public class TransactionCSVMap : ClassMap<Transaction>
    {
        public TransactionCSVMap() : base()
        {
            Map(x => x.Id).Name("id");

            Map(x => x.BeneficiaryName).Name("beneficiary-name");

            Map(x => x.Date).Name("date").Validate(field =>
            {
                if (string.IsNullOrEmpty(field.Field))
                {
                    throw new ArgumentException($"Date is requiered. Problem occured at Row : {field.Row.Parser.Row} , Column : 'date'");
                }
                if (!DateTime.TryParse(field.Field, out DateTime res))
                {
                    throw new ArgumentException($"'{field.Field}' is not a valid date. Problem occured at Row : {field.Row.Parser.Row} , Column : 'date'");
                }

                return true;

            });

            Map(x => x.Direction).Name("direction").Validate(field =>
            {
                if (string.IsNullOrEmpty(field.Field))
                {
                    throw new ArgumentException($"Direction is requiered. Problem occured at Row : {field.Row.Parser.Row} , Column : 'direction'");
                }
                if (!Enum.IsDefined(typeof(Direction), field.Field))
                {
                    throw new ArgumentException($"'{field.Field}' is not a valid transaction direction. Problem occured at Row : {field.Row.Parser.Row} , Column : 'direction'");
                }
                return true;
            });

            Map(x => x.Ammount).Name("amount").Validate(field =>
            {
                if (string.IsNullOrEmpty(field.Field))
                {
                    throw new ArgumentException($"Ammount is requiered. Problem occured at Row : {field.Row.Parser.Row} , Column : 'amount'");
                }
                double ammount;
                if (!double.TryParse(field.Field, out ammount))
                {
                    throw new ArgumentException($"Value for Ammount : '{field.Field}' is not a valid number. Problem occured at Row : {field.Row.Parser.Row} , Column : 'amount'");
                }
                if (ammount <= 0)
                {
                    throw new ArgumentException($"Ammount cannot be 0 or less. Problem occured at Row : {field.Row.Parser.Row} , Column : 'amount'");
                }
                return true;
            });

            Map(x => x.Description).Name("description").Validate(field =>
            {
                if (string.IsNullOrEmpty(field.Field))
                {
                    throw new ArgumentException($"Description is requiered. Problem occured at Row : {field.Row.Parser.Row} , Column : 'description'");
                }
                return true;
            });

            Map(x => x.Currency).Name("currency").Validate(field =>
            {
                if (string.IsNullOrEmpty(field.Field))
                {
                    throw new ArgumentException($"Currency is requiered. Problem occured at Row : {field.Row.Parser.Row} , Column : 'currency'");
                }
                if (!CurrencyCodesResolver.Codes.Any(x => x.Code == field.Field))
                {
                    throw new ArgumentException($"'{field.Field}' is not a valid ISO 4217 currency code. Problem occured at Row : {field.Row.Parser.Row} , Column : 'currency'");
                }
                return true;
            });

            Map(x => x.Mcc).Name("mcc");

            Map(x => x.Kind).Name("kind").Validate(field =>
            {
                if (string.IsNullOrEmpty(field.Field))
                {
                    throw new ArgumentException($"Transaction kind is requiered. Problem occured at Row : {field.Row.Parser.Row} , Column : 'kind'");
                }
                if (!Enum.IsDefined(typeof(TransactionKind), field.Field))
                {
                    throw new ArgumentException($"'{field.Field}' is not a valid transaction kind. Problem occured at Row : {field.Row.Parser.Row} , Column : 'kind'");
                }
                return true;

            });
        }
    }
}
