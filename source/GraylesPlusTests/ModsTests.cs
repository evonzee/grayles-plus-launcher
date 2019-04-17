using System;
using g = GraylesPlus;
using Xunit;
using System.IO;

namespace GraylesPlusTests
{
    public class ModsTests : IDisposable
    {
        private string _path;
        private g.Config _config;
        public ModsTests() {
            this._path = Path.GetRandomFileName();
            Directory.CreateDirectory(this._path);
            this._config = new g.Config().With(graylesRoot: this._path);
        }

        public void Dispose(){
            Directory.Delete(this._path, true);
        }

        [Fact]
        public void ModsExists()
        {
            var mods = new g.Mods(config: this._config);
            Assert.True(mods != null);
        }
        
        [Fact]
        public void EmptyModVersionReportsUninstalled(){
            var mods = new g.Mods(config: this._config);
            Assert.False(mods.Downloaded);
            Assert.False(mods.Installed);
            Assert.Null(mods.InstalledVersion);
            Assert.Null(mods.TargetVersion);
            Assert.Null(mods.ModZip);
        }

        [Theory]
        [InlineData("3.0.2")]
        [InlineData("4.0.0")]
        public void TargetVersionSettable(string version){
            var mods = new g.Mods(config: this._config);
            var mods2 = mods.With(targetVersion: version);

            Assert.NotSame(mods2, mods);

            Assert.Equal(mods2.TargetVersion, version);
            Assert.Equal(mods2.ModZip, Path.Combine(this._path, $"Grayles Modpack V{version}.zip"));
            Assert.Null(mods2.InstalledVersion);

        }

        [Theory]
        [InlineData("3.0.2")]
        [InlineData("4.0.0")]
        public void InstalledVersionRead(string version){
            using(var file = File.CreateText(Path.Combine(this._path,"version.txt"))){
                file.Write(version);
            }

            var mods = new g.Mods(config: this._config);

            Assert.Equal(mods.InstalledVersion, version);
        }

        // test that downloaded file presence is correctly detected

        // test that modpack can be unzipped correctly

        // test that latest version can be determined (future version)

        // test that modpack can be downloaded correctly (future version)

    }
}
