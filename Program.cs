using System;
using System.Collections.Generic;
using System.Transactions;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using FluentNHibernate.Mapping;
using NHibernate;
using Environment = NHibernate.Cfg.Environment;

namespace StatelessBatchFlush
{
    class Program
    {
        private static int _numEntities = 3;
        private static string _connectionString = "CONNECTION_STRING_HERE";

        static void Main(string[] args)
        {
            Console.WriteLine($"Saving {_numEntities} entities using System.Transaction and UseConnectionOnSystemTransactionPrepare true...");
            ValidateBatching(BuildSessionFactory(1, true));

            Console.WriteLine($"Saving {_numEntities} entities using System.Transaction and UseConnectionOnSystemTransactionPrepare false with batch size 1...");
            ValidateBatching(BuildSessionFactory(1, false));
            
            Console.WriteLine($"Saving {_numEntities} entities using System.Transaction and UseConnectionOnSystemTransactionPrepare false with batch size 2...");
            ValidateBatching(BuildSessionFactory(2, false));
        }

        private static ISessionFactory BuildSessionFactory(int batchSize, bool useConnectionOnSystemTransactionPrepare)
        {
            return Fluently.Configure()
                .Database(MsSqlConfiguration.MsSql2012.ConnectionString(_connectionString))
                .ExposeConfiguration(config =>
                {
                    config.SetProperty(Environment.BatchSize, batchSize.ToString());
                    config.SetProperty(Environment.UseConnectionOnSystemTransactionPrepare, useConnectionOnSystemTransactionPrepare.ToString());
                })
                .Mappings(mapping => mapping.FluentMappings.AddFromAssemblyOf<Program>())
                .BuildSessionFactory();
        }

        private static void ValidateBatching(ISessionFactory sessionFactory)
        {
            try
            {
                using (var session = sessionFactory.OpenStatelessSession())
                {
                    session.CreateSQLQuery("DELETE FROM TestEntities").ExecuteUpdate();
                }

                var entities = new List<TestEntity>(_numEntities);
                for (int i = 0; i < _numEntities; i++)
                {
                    entities.Add(new TestEntity { Id = Guid.NewGuid() });
                }

                using (var transaction = new TransactionScope())
                {
                    using (var session = sessionFactory.OpenStatelessSession())
                    {
                        foreach (var entity in entities)
                        {
                            session.Insert(entity);
                        }
                    }
                    transaction.Complete();
                }

                using (var session = sessionFactory.OpenStatelessSession())
                {
                    var results = session.QueryOver<TestEntity>().List<TestEntity>();

                    Console.WriteLine($"Found {results.Count} entities out of expected {entities.Count}.");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }

    public class TestEntity
    {
        public virtual Guid Id { get; set; }
    }

    public class TestEntityMap : ClassMap<TestEntity>
    {
        public TestEntityMap()
        {
            Table("TestEntities");
            Id(c => c.Id).GeneratedBy.Assigned();
        }
    }
}
