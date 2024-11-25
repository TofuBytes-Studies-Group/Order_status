namespace Order_status.Domain.Entities
{
    public enum Status
    {
        Accepted,
        BeingPrepared,
        ReadyForPickUp,
        PickedUp, 
        Delivered,
    }
}
