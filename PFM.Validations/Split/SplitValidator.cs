namespace PFM.Validations.Split
{
    public class SplitValidator : ISplitValidator
    {
        public bool ValidateAmmount(double transactionAmmoun, double splitAmmount)
        {
            return transactionAmmoun == splitAmmount;
        }
    }
}
