namespace LazyLoadConsistencyPitfall
{
	using FluentNHibernate.Mapping;

	public class OrderLineMap : ClassMap<OrderLine>
	{
		public OrderLineMap()
		{
			Id(x => x.Id);

			Not.LazyLoad();

			References(x => x.Order);

			Map(x => x.ProductId);
			Map(x => x.Quantity);
			Map(x => x.Price);
		}
	}
}