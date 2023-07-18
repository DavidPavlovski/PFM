namespace PFM.Validations.Split
{
    public interface ISplitValidator
    {
        bool ValidateAmmount(double transactionAmmoun, double splitAmmount);
    }
}
