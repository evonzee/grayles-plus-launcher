using System;
using g = GraylesPlus;
using Xunit;
using System.IO;

namespace GraylesPlusTests
{
    public class StarboundTests : IDisposable
    {
        private string _graylesPath;
        private string _starboundPath;
        private g.Config _config;
        public StarboundTests() {
            this._graylesPath = Path.GetRandomFileName();
            this._starboundPath = Path.GetRandomFileName();
            Directory.CreateDirectory(this._graylesPath);
            Directory.CreateDirectory(this._starboundPath);
            this._config = new g.Config().With(graylesRoot: this._graylesPath, starboundRoot: this._starboundPath);
        }

        private void ConfigureSystem(string system){
            Directory.CreateDirectory(Path.Combine(this._starboundPath, system));
        }

        public void Dispose(){
            Directory.Delete(this._graylesPath, true);
            Directory.Delete(this._starboundPath, true);
        }

        [Theory]
        [InlineData("linux")]
        [InlineData("win64")]
        public void StarboundExists(string system)
        {
            this.ConfigureSystem(system);
            var starbound = new g.Starbound(config: this._config);
            Assert.NotNull(starbound);
        }

        [Fact]
        public void MissingSBConfigDoesntThrow(){
            var starbound = new g.Starbound(config: this._config);
            Assert.Null(starbound.StarboundProfile);

            starbound = new g.Starbound(config: new g.Config());
            Assert.Null(starbound.StarboundProfile);
        }
        
        [Theory]
        [InlineData("linux")]
        [InlineData("win64")]
        public void UnconfiguredReportsCorrectly(string system){
            this.ConfigureSystem(system);
            var starbound = new g.Starbound(config: this._config);
            Assert.False(starbound.Configured);
        }
        
        [Theory]
        [InlineData("linux")]
        [InlineData("win64")]
        public void ConfigurationDetectionWorks(string system){
            this.ConfigureSystem(system);
            var starbound = new g.Starbound(config: this._config);
            File.WriteAllText(Path.Combine(this._config.StarboundRoot, system, "grayles.config"), "test");
            Assert.True(starbound.Configured);
        }
        
        [Theory]
        [InlineData("linux")]
        [InlineData("win64")]
        public void ProfileWritten(string system){
            this.ConfigureSystem(system);
            var starbound = new g.Starbound(config: this._config);
  
            Assert.True(starbound.Configure());

            string text = File.ReadAllText(Path.Combine(this._config.StarboundRoot, system, "grayles.config"));
            Assert.Matches("defaultConfiguration", text);
            Assert.Matches("assetDirectories", text);
            Assert.Matches("storageDirectory", text);
            Assert.Matches(this._config.ModRoot, text);
            Assert.Matches(this._config.StorageRoot, text);
        }


    }
}
