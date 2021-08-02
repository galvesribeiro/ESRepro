using System;
using Microsoft.Extensions.Hosting;
using Orleans;
using Microsoft.Extensions.DependencyInjection;
using Orleans.Hosting;
using EventStore.Client;
using Microsoft.Extensions.Options;
using Xunit;

namespace esxunit
{
    public class RealClusterFixture : IDisposable
    {
        public IHost Silo { get; set; }

        public RealClusterFixture()
        {
            this.Silo = Host.CreateDefaultBuilder()
                .UseOrleans((ctx, siloBuilder) =>
                {
                    siloBuilder.UseLocalhostClustering();
                })
                .ConfigureServices(services =>
                {
                    services.Configure<EventStoreClientSettings>(opt =>
                    {
                        opt.ConnectivitySettings.Address = new Uri("esdb://localhost:2113?tls=false");
                        opt.ConnectivitySettings.Insecure = true;
                    });

                    services.AddSingleton<EventStoreClient>(sp => new EventStoreClient(sp.GetRequiredService<IOptions<EventStoreClientSettings>>()));
                })
                .Build();
            
            this.Silo.Start();
        }

        public void Dispose()
        {
            this.Silo?.Dispose();
        }
    }

    [CollectionDefinition(nameof(RealClusterCollection))]
    public class RealClusterCollection : ICollectionFixture<RealClusterFixture>
    {
    }
}