using Order_status.Domain.Entities;

namespace Order_status.Domain.Aggregates
{
    public class OrderStatus
    {
        public required Guid OrderId { get; set; }
        public required string CustomerName { get; set; }
        public required Status Status { get; set; }

        public string StatusDescription
        {
            get
            {
                return StatusDescriptionHelper.GetDescription(Status);
            }
        }

        public override string ToString()
        {
            return $"Hi {CustomerName}! {StatusDescription}.";
        }
    }
}
