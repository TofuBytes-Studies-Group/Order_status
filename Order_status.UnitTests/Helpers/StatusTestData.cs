﻿using System.Collections;
using Order_status.Domain.Entities;

namespace Order_status.UnitTests.Helpers
{
    public class StatusTestData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[] { Status.Accepted, "Your order has been accepted by the restaurant" };
            yield return new object[] { Status.Rejected, "Unfortunately, the restaurant cannot fulfil your order at this time. Please try one of our many other delicious restaurants!" };
            yield return new object[] { Status.BeingPrepared, "Your food is being prepared" };
            yield return new object[] { Status.ReadyForPickUp, "An order is ready for pickup" };
            yield return new object[] { Status.PickedUp, "Your order has been picked up" };
            yield return new object[] { Status.Delivered, "Your order has been delivered. Thank you for ordering from MTOGO!" };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
