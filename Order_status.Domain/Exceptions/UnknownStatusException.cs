namespace Order_status.Domain.Exceptions
{
    public class UnknownStatusException : Exception
    {
        public UnknownStatusException() : base("Unknown status") { }
    }
}
