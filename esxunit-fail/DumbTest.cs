using System.Threading.Tasks;
using EventStore.Client;
using Microsoft.Extensions.DependencyInjection;
using Orleans.TestingHost;
using Xunit;

namespace esxunit
{
    [Collection(nameof(TestClusterCollection))]
    public class DumbTest
    {
        private readonly TestCluster _cluster;

        public DumbTest(TestClusterFixture clusterFixture)
        {
            this._cluster = clusterFixture.Cluster;
        }

        [Fact]
        public async Task Test()
        {
            var es = this._cluster.ServiceProvider.GetRequiredService<EventStoreClient>();

            var result = await es.AppendToStreamAsync("Test12222", StreamRevision.None, new EventData[] { new EventData(Uuid.NewUuid(), "Test", System.ReadOnlyMemory<byte>.Empty) });

            Assert.NotNull(result);
        }
    }
}