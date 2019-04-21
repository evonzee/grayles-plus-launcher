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
            Directory.CreateDirectory(Path.Combine(this._path, "zip"));
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

            var targetVersion = new g.ModpackVersion(version: version);
            var mods2 = mods.With(targetVersion: targetVersion);

            Assert.NotSame(mods2, mods);

            Assert.Equal(mods2.TargetVersion, version);
            Assert.Equal(mods2.ModZip, Path.Combine(this._path, "zip", $"Grayles Modpack V{version}.zip"));
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

        [Theory]
        [InlineData("3.0.2")]
        [InlineData("4.0.0")]
        public void ZipFileDetected(string version){
            // not a real zipfile, but this test doesn't care about that
            using(var file = File.CreateText(Path.Combine(this._path,"zip",$"Grayles Modpack V{version}.zip"))){
                file.Write(version);
            }

            var targetVersion = new g.ModpackVersion(version: version);
            var mods = new g.Mods(config: this._config).With(targetVersion: targetVersion);
            Assert.True(mods.Downloaded);

            targetVersion = new g.ModpackVersion(version: "another version");
            mods = mods.With(targetVersion: targetVersion);
            Assert.False(mods.Downloaded);
        }
        
        [Theory]
        [InlineData("3.0.2")]
        [InlineData("4.0.0")]
        public void ModsCanUnzip(string version){
            string modsToZip = Path.Combine(this._path, "zipmods");
            Directory.CreateDirectory(modsToZip);
            using(var file = File.CreateText(Path.Combine(modsToZip,"testfile.txt"))){
                file.Write(version);
            }
            System.IO.Compression.ZipFile.CreateFromDirectory(modsToZip, Path.Combine(this._path,"zip",$"Grayles Modpack V{version}.zip"));

            var targetVersion = new g.ModpackVersion(version: version);
            var mods = new g.Mods(config: this._config).With(targetVersion: targetVersion);
            Assert.True(mods.Install());
            Assert.True(mods.Installed);
            Assert.Equal(version, mods.InstalledVersion);

            Assert.Equal(version, File.ReadAllText(Path.Combine(this._config.ModRoot, "testfile.txt")));
        }


        // test that latest version can be determined (future version)

        // test that modpack can be downloaded correctly (future version)

    }
}
