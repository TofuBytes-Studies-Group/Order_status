using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using Order_status.Domain.Entities;

namespace Order_status.Infrastructure.Models
{
    public class OrderStatusDTO
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public required Guid OrderId { get; set; }
        public required string CustomerName { get; set; }
        public required Status Status { get; set; }
    }
}
