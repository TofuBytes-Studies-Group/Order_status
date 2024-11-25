using Order_status.Domain.Exceptions;

namespace Order_status.Domain.Entities
{
    public static class StatusDescriptionHelper
    {
        public static string GetDescription(Status status)
        {
            return status switch
            {
                Status.Accepted => "Your order has been accepted by the restaurant",
                Status.BeingPrepared => "Your food is being prepared",
                Status.ReadyForPickUp => "An order is ready for pickup",
                Status.PickedUp => "Your order has been picked up",
                Status.Delivered => "Your order has been delivered. Thank you for ordering from MTOGO!",
                _ => throw new UnknownStatusException()
            };
        }
    }
}
