using System;
using g = GraylesPlus;
using Xunit;


namespace GraylesPlusTests
{
    public class ConfigTests
    {
        [Fact]
        public void ConfigExists()
        {
            var config = new g.Config();
            Assert.True(config != null);
        }

        [Theory]
        [InlineData(null, null)]
        [InlineData(null,"D")]
        [InlineData("C",null)]
        [InlineData("C","D")]
        public void TestDirectoryChanges(string grayles, string sb){
            var config = new g.Config();
            var config2 = config.With(grayles,sb);

            Assert.NotSame(config, config2);

            if(grayles == null){
                Assert.Equal(config.GraylesRoot, config2.GraylesRoot);
            } else {
                Assert.Equal(grayles, config2.GraylesRoot);
                Assert.NotEqual(config.GraylesRoot, config2.GraylesRoot);
            }

            if(sb == null){
                Assert.Equal(config.StarboundRoot, config2.StarboundRoot);
            } else {
                Assert.Equal(sb, config2.StarboundRoot);
                Assert.NotEqual(config.StarboundRoot, config2.StarboundRoot);
            }

        }

    }
}
