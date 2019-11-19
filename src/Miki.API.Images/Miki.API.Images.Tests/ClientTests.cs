using Miki.API.Images.Models;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace Miki.API.Images.Tests
{
    public class ClientTests
    {
        [Fact]
        public void ConstructClientTest()
        {
            var i = new ImghoardClient();

            Assert.NotNull(i);
            Assert.Equal(Config.Default().Endpoint, i.GetEndpoint());
        }

        [Fact]
        public async Task GetSingleImageAsync()
        {
            var mock = new Mock<IImghoardClient>();

            mock.Setup(x => x.GetImageAsync(It.IsAny<ulong>()))
                .Returns(Task.FromResult(new Image
                {
                    Id = 1171190856858734592,
                    Tags = new[] { "animal", "cat" },
                    Url = "https://cdn.miki.ai/ext/imgh/1ciajYwALX.jpeg"
                }));

            var response = await mock.Object.GetImageAsync(1171190856858734592);

            Assert.Equal<ulong>(1171190856858734592, response.Id);
            Assert.NotNull(response.Tags);
            Assert.NotNull(response.Url);
        }
    }
}
