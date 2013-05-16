namespace LazyLoadConsistencyPitfall
{
	using FluentNHibernate.Mapping;

	public class OrderMap : ClassMap<Order>
	{
		public OrderMap()
		{
			Id(x => x.Id);

			Not.LazyLoad();

			Version(x => x.Version);
			
			Map(x => x.CustomerId);
			Map(x => x.Total);

			HasMany(x => x.OrderLines)
				.Inverse()
				.Cascade.All()
				.LazyLoad();
				// Option #2 to achieve consistency: avoid lazy loads
				// .Not.LazyLoad().Fetch.Join();
		}
	}
}