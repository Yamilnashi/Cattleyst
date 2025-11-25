namespace CattleystData.Models.Enums
{
    public enum EEventType : byte
    {
        Cattle_Received = 1,
        Cattle_Status_Updated = 2,
        Cattle_Processed = 3,
        Product_Created = 4,
        Product_Status_Updated = 5,
        Stock_Updated = 6,
        Order_Placed = 7,
        Order_Status_Updated = 8,
        Shipping_Updated = 9,
        Error_Ocurred = 10,
        Inventory_Low = 11,
        Compliance_Alert = 12
    }
}
