using System;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Xunit;
using Orleans.Hosting;
using Orleans.TestingHost;
using EventStore.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace esxunit
{
    public class ClusterFixture : IDisposable
    {
        public TestCluster Cluster { get; private set; }

        public ClusterFixture()
        {
            this.Cluster = new TestClusterBuilder()
                .AddSiloBuilderConfigurator<TestSiloConfiguartion>()
                .Build();
            this.Cluster.Deploy();
        }

        public void Dispose()
        {
            this.Cluster.StopAllSilos();
        }
    }

    public class TestSiloConfiguartion : ISiloConfigurator, IHostConfigurator
    {
        public static string ESConnection = Environment.GetEnvironmentVariable("ES_CS") ?? "esdb://localhost:2113?tls=false";

        public void Configure(ISiloBuilder siloBuilder)
        {
            siloBuilder.ConfigureServices(services =>
            {
                services.Configure<EventStoreClientSettings>(opt =>
                {
                    opt.ConnectivitySettings.Address = new Uri("esdb://localhost:2113?tls=false");
                    opt.ConnectivitySettings.Insecure = true;
                });

                services.AddSingleton<EventStoreClient>(sp => new EventStoreClient(sp.GetRequiredService<IOptions<EventStoreClientSettings>>()));
            });
        }

        public void Configure(IHostBuilder hostBuilder)
        {
            hostBuilder.ConfigureLogging(log => log.AddConsole());
        }
    }

    [CollectionDefinition(nameof(ClusterCollection))]
    public class ClusterCollection : ICollectionFixture<ClusterFixture>
    {
    }
}