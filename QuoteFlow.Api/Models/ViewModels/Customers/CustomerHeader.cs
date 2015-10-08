namespace QuoteFlow.Api.Models.ViewModels.Customers
{
    public class CustomerHeader
    {
        public CustomerHeader(string customerName)
        {
            CustomerName = customerName;
        }

        /// <summary>
        /// The customers full name.
        /// </summary>
        public string CustomerName { get; set; } 
    }
}