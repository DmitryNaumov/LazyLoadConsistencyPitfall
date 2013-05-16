namespace LazyLoadConsistencyPitfall
{
	using System;
	using FluentNHibernate.Cfg;
	using FluentNHibernate.Cfg.Db;
	using NHibernate;
	using NHibernate.Cfg;
	using NHibernate.Tool.hbm2ddl;

	class Program
	{
		static void Main(string[] args)
		{
			var sessionFactory = CreateSessionFactory();

			new Application(sessionFactory).Run();

			Console.WriteLine("Press any key to exit...");
			Console.ReadKey();
		}

		static ISessionFactory CreateSessionFactory()
		{
			return Fluently.Configure()
				.Database(MsSqlConfiguration.MsSql2008
					.ConnectionString("Data Source=.;Initial Catalog=AggregateRoot;Integrated Security=True")
				)
				.Mappings(m => m.FluentMappings.AddFromAssemblyOf<Program>())
				.ExposeConfiguration(BuildSchema)
				.BuildSessionFactory();
		}

		static void BuildSchema(Configuration config)
		{
			new SchemaExport(config).Create(false, true);
		}
	}
}
