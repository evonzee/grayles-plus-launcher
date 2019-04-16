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
    }
}
