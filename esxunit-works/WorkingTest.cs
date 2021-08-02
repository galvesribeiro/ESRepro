using System.Threading.Tasks;
using EventStore.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Xunit;

namespace esxunit
{
    [Collection(nameof(RealClusterCollection))]
    public class WorkingTest
    {
        private readonly IHost _silo;

        public WorkingTest(RealClusterFixture realClusterFixture)
        {
            this._silo = realClusterFixture.Silo;
        }

        [Fact]
        public async Task Test()
        {
            var es = this._silo.Services.GetRequiredService<EventStoreClient>();

            var result = await es.AppendToStreamAsync("Test12222", StreamRevision.None, new EventData[] { new EventData(Uuid.NewUuid(), "Test", System.ReadOnlyMemory<byte>.Empty) });

            Assert.NotNull(result);
        }
    }
}