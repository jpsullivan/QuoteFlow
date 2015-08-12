namespace QuoteFlow.Api.Models.ViewModels.Customers
{
    public class CustomerShowModel
    {
        public CustomerShowModel(Customer customer)
        {
            Customer = customer;
        }
        
        /// <summary>
        /// The customer object itself.
        /// </summary>
        public Customer Customer { get; set; } 
    }
}