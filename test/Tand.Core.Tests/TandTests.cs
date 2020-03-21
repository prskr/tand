using Xunit;

namespace Tand.Core.Tests
{
    public class TandTests
    {

        [Fact]
        public void GenerateTand()
        {
            var handleCallCounter = 0;
            var resolver = new ResolverMock(new LogTarget<ITandSample>(tandSample => handleCallCounter++));
            var tand = new Tand(resolver);

            var sample = tand.DecorateWithTand<ITandSample, TandSample>(new TandSample());
            var result = sample.LogMyParams("Hello, World", 42);
            Assert.Equal(1, result);
            Assert.Equal(2, handleCallCounter);
            Assert.Equal(1, resolver.ResolvingCounter);
        }
    }
}