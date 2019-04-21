using System;
using System.Collections.Generic;
using Xunit;
using g = GraylesPlus;

namespace GraylesPlusTests
{
    public class OnlineUpdateTests : IDisposable
    {

        private g.Config _config;

        public OnlineUpdateTests()
        {
            this._config = new g.Config().With(updateUrl: "none");
        }

        public void Dispose()
        {
        }

        [Fact]
        void OnlineUpdaterExists()
        {
            g.OnlineUpdate update = new g.OnlineUpdate(this._config);
            Assert.NotNull(update);
        }

        [Fact]
        void UpdateCanParseEmptyVersion()
        {
            string json = "{versions: [], updateUrl: 'test'}";
            g.OnlineUpdate updates = g.OnlineUpdate.Parse(this._config, json);
            Assert.Equal("test", updates.UpdateUrl);
            Assert.Empty(updates.AvailableVersions);
        }

        [Fact]
        void UpdateCanParseAVersion()
        {
            string json = "{versions: [{name:'bob', url: 'test', hashcode: 'asdf', latest: true}], updateUrl: 'test'}";
            g.OnlineUpdate updates = g.OnlineUpdate.Parse(this._config, json);

            Assert.NotEmpty(updates.AvailableVersions);
            Assert.Single(updates.AvailableVersions);

            var versions = new List<g.ModpackVersion>();
            versions.AddRange(updates.AvailableVersions);
            Assert.Equal("bob", versions[0].VersionNumber);
        }
    }
}
