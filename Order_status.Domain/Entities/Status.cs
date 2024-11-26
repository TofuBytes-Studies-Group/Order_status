namespace Order_status.Domain.Entities
{
    public enum Status
    {
        Accepted,
        Rejected,
        BeingPrepared,
        ReadyForPickUp,
        PickedUp, 
        Delivered,
    }
}
