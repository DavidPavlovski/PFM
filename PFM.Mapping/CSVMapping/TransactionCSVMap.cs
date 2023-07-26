using ISO._4217;
using PFM.DataAccess.Entities;
using PFM.Enums;
using PFM.Helpers.CSVParser;

namespace PFM.Mapping.CSVMapping
{
    public class TransactionCSVMap : CustomClassMap<Transaction>
    {
        public TransactionCSVMap() : base()
        {

            Map(x => x.Id).Name("id");

            Map(x => x.BeneficiaryName).Name("beneficiary-name");

            Map(x => x.Date).Name("date").Validate(field =>
            {
                if (string.IsNullOrEmpty(field.Field))
                {
                    Errors.Add($"Date is requiered. Problem occured at Row : {field.Row.Parser.Row} , Column : 'date'");
                }
                else if (!DateTime.TryParse(field.Field, out DateTime res))
                {
                    Errors.Add($"'{field.Field}' is not a valid date. Problem occured at Row : {field.Row.Parser.Row} , Column : 'date'");
                }
                return true;
            }).Default(new DateTime(), useOnConversionFailure: true);

            Map(x => x.Direction).Name("direction").Validate(field =>
            {

                if (string.IsNullOrEmpty(field.Field))
                {
                    Errors.Add($"Direction is requiered. Problem occured at Row : {field.Row.Parser.Row} , Column : 'direction'");
                }
                else if (!Enum.IsDefined(typeof(Direction), field.Field))
                {
                    Errors.Add($"'{field.Field}' is not a valid transaction direction. Problem occured at Row : {field.Row.Parser.Row} , Column : 'direction'");
                }
                return true;
            }).Default(Direction.d, useOnConversionFailure: true);

            Map(x => x.Ammount).Name("amount").Validate(field =>
            {
                if (string.IsNullOrEmpty(field.Field))
                {
                    Errors.Add($"Ammount is requiered. Problem occured at Row : {field.Row.Parser.Row} , Column : 'amount'");
                }
                else if (!double.TryParse(field.Field, out double ammount))
                {
                    Errors.Add($"Value for Ammount : '{field.Field}' is not a valid number. Problem occured at Row : {field.Row.Parser.Row} , Column : 'amount'");
                }
                else if (ammount <= 0)
                {
                    Errors.Add($"Ammount cannot be 0 or less. Problem occured at Row : {field.Row.Parser.Row} , Column : 'amount'");
                }
                return true;
            }).Default(1, useOnConversionFailure: true);

            Map(x => x.Description).Name("description").Validate(field =>
            {
                if (string.IsNullOrEmpty(field.Field))
                {
                    Errors.Add($"Description is requiered. Problem occured at Row : {field.Row.Parser.Row} , Column : 'description'");
                }
                return true;
            }).Default(string.Empty, useOnConversionFailure: true);

            Map(x => x.Currency).Name("currency").Validate(field =>
            {
                if (string.IsNullOrEmpty(field.Field))
                {
                    Errors.Add($"Currency is requiered. Problem occured at Row : {field.Row.Parser.Row} , Column : 'currency'");
                }
                else if (!CurrencyCodesResolver.Codes.Any(x => x.Code == field.Field))
                {
                    Errors.Add($"'{field.Field}' is not a valid ISO 4217 currency code. Problem occured at Row : {field.Row.Parser.Row} , Column : 'currency'");
                }
                return true;
            }).Default("USD", useOnConversionFailure: true);

            Map(x => x.Mcc).Name("mcc");

            Map(x => x.Kind).Name("kind").Validate(field =>
            {
                if (string.IsNullOrEmpty(field.Field))
                {
                    Errors.Add($"Transaction kind is requiered. Problem occured at Row : {field.Row.Parser.Row} , Column : 'kind'");
                }
                else if (!Enum.IsDefined(typeof(TransactionKind), field.Field))
                {
                    Errors.Add($"'{field.Field}' is not a valid transaction kind. Problem occured at Row : {field.Row.Parser.Row} , Column : 'kind'");
                }
                return true;

            }).Default(TransactionKind.dep, useOnConversionFailure: true);


        }
    }
}
