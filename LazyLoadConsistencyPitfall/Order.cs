namespace LazyLoadConsistencyPitfall
{
	using System.Collections.Generic;

	public class Order
	{
		public Order(int customerId)
		{
			CustomerId = customerId;

			OrderLines = new List<OrderLine>();
		}

		private Order()
		{
		}

		public int Id { get; private set; }

		public int Version { get; private set; }

		public int CustomerId { get; private set; }

		public double Total { get; private set; }

		public IList<OrderLine> OrderLines { get; private set; }

		public void BuyProduct(string productId, int quantity, double price)
		{
			OrderLines.Add(new OrderLine(this, productId, quantity, price));

			Total += quantity * price;
		}
	}
}