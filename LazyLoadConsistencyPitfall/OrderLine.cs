namespace LazyLoadConsistencyPitfall
{
	public class OrderLine
	{
		public OrderLine(Order order, string productId, int quantity, double price)
		{
			Order = order;
			ProductId = productId;
			Quantity = quantity;
			Price = price;
		}

		private OrderLine()
		{
		}

		public int Id { get; private set; }

		public Order Order { get; private set; }

		public string ProductId { get; private set; }

		public int Quantity { get; private set; }

		public double Price { get; private set; }
	}
}