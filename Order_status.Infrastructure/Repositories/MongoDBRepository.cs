using Order_status.Domain.Aggregates;

namespace Order_status.Infrastructure.Repositories
{
    public class MongoDBRepository : IOrderStatusRepository
    {
        public OrderStatus GetOrderStatus(Guid orderId)
        {
            throw new NotImplementedException();
        }

        public void UpdateOrderStatus(OrderStatus orderStatus)
        {
            throw new NotImplementedException();
        }
    }
}
