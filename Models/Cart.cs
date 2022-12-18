using Team2_DotNetCA.Models;

namespace Team2_DotNetCA.Models
{
    public class Cart
    {
        // 1 cart = 1 userId = 1 set of products & qty = 1 total amount

        public int UserId { get; set; }

        public Dictionary<Product, int> CartItems { get; set; }

        public Cart()
        {
            CartItems = new Dictionary<Product, int>();
        }

        // FOR AJAX 
        public int ProductId { get; set; }

        public int Quantity { get; set; }

    }
}
