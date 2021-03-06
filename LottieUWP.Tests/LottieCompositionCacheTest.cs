using LottieUWP.Model;
using Moq;
using Xunit;

namespace LottieUWP.Tests
{
    public class LottieCompositionCacheTest
    {

        private LottieComposition composition;
        private LottieCompositionCache cache;

        public LottieCompositionCacheTest()
        {
            composition = new Mock<LottieComposition>().Object;
            cache = new LottieCompositionCache();
        }

        [Fact]
        public void TestEmpty()
        {
            Assert.Null(cache.Get("foo"));
        }

        [Fact]
        public void TestStrongAsset()
        {
            cache.Put("foo", composition);
            Assert.Equal(composition, cache.Get("foo"));
        }

        [Fact]
        public void TestWeakAsset()
        {
            cache.Put("foo", composition);
            Assert.Equal(composition, cache.Get("foo"));
        }
    }
}
