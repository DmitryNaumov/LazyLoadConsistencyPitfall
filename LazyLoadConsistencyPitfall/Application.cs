namespace LazyLoadConsistencyPitfall
{
	using System;
	using System.Data;
	using System.Linq;
	using System.Threading;
	using NHibernate;

	class Application
	{
		private readonly ISessionFactory _sessionFactory;

		public Application(ISessionFactory sessionFactory)
		{
			_sessionFactory = sessionFactory;
		}

		public void Run()
		{
			var orderId = CreateOrder();

			using (var session = _sessionFactory.OpenSession())
			using (var transaction = session.BeginTransaction())
			// Option #1 to achieve consistency: use RepeatableRead isolation level, but be prepared for exceptions!
			//using (var transaction = session.BeginTransaction(IsolationLevel.RepeatableRead))
			{
				// SELECT order0_.Id as Id1_0_, order0_.Version as Version1_0_, order0_.CustomerId as CustomerId1_0_, order0_.Total as Total1_0_ FROM [Order] order0_ WHERE order0_.Id=1
				var order = session.Get<Order>(orderId);

				Console.WriteLine(order.Total);

				ConcurrentWriter(orderId);

				// SELECT orderlines0_.Order_id as Order5_1_, orderlines0_.Id as Id1_, orderlines0_.Id as Id0_0_, orderlines0_.ProductId as ProductId0_0_, orderlines0_.Quantity as Quantity0_0_, orderlines0_.Price as Price0_0_, orderlines0_.Order_id as Order5_0_0_ FROM [OrderLine] orderlines0_ WHERE orderlines0_.Order_id=1
				Console.WriteLine(order.OrderLines.Sum(oi => oi.Quantity * oi.Price));

				/*
					It wouldn't require RepeatableRead if NH generate slightly modified query and validate Order.Version:
					SELECT o.Version, ol.* FROM [OrderLine] ol INNER JOIN [Order] o ON ol.Order_id = o.Id WHERE ol.Order_id = 1
				*/

				transaction.Commit();
			}

		}

		private void ConcurrentWriter(int orderId)
		{
			var thread = new Thread(() =>
			{
				using (var session = _sessionFactory.OpenSession())
				using (var transaction = session.BeginTransaction())
				{
					var order = session.Get<Order>(orderId);

					order.BuyProduct("Whiskey", 1, 10.12);

					transaction.Commit();
				}
			});

			thread.Start();

			// give it chance to complete
			thread.Join(1000);
		}

		private int CreateOrder()
		{
			using (var session = _sessionFactory.OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				var order = new Order(1);
				order.BuyProduct("Milk", 1, 1.23);
				order.BuyProduct("Bread", 2, 4.56);

				session.Save(order);

				transaction.Commit();

				return order.Id;
			}
		}
	}
}
