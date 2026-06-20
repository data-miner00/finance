namespace Core.Models
{
    public enum AccountType
    {
        Savings,

        EWallet,

        Cash,

        [Obsolete("Use EWallet instead.")]
        App,

        CreditCard,
    }
}
