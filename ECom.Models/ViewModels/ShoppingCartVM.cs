namespace ECom.Models.ViewModels
{
    public class ShoppingCartVM
    {
        public IEnumerable<ShoppingCart> shoppingCartsList { get; set; }

        public OrderHeader orderHeader { get; set; }
        //public double? shoppingCartTotal { get; set; }
    }
}
