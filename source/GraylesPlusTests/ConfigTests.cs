using System;
using g = GraylesPlus;
using Xunit;
using System.IO;

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
            var config2 = config.With(graylesRoot: grayles, starboundRoot: sb);

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

        [Fact]
        public void ConfigCanBeRead(){
            var configFile = Path.Combine(g.Config.AppDirectory, "config.json");
            File.WriteAllText(configFile, "{graylesRoot: 'bob', starboundRoot: 'joe'}");

            var config = g.Config.Load();
            Assert.Equal("bob", config.GraylesRoot);
            Assert.Equal("joe", config.StarboundRoot);

            File.Delete(configFile);
        }

        [Fact]
        public void ConfigCanBeWritten(){
            var config = new g.Config().With(graylesRoot: "Testing", starboundRoot: "SB Root");
            g.Config.Save(config);
            
            var newConfig = g.Config.Load();
            Assert.Equal("Testing", newConfig.GraylesRoot);
            Assert.Equal("SB Root", newConfig.StarboundRoot);

            var configFile = Path.Combine(g.Config.AppDirectory, "config.json");
            File.Delete(configFile);
        }

    }
}
