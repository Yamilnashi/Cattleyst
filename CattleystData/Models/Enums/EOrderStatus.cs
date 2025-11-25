namespace CattleystData.Models.Enums
{
    public enum EOrderStatus : byte
    {
        Submitted = 1,
        Preparing_Order = 2,
        Shipping = 3,
        Shipped = 4,
        Delivered = 5
    }
}
