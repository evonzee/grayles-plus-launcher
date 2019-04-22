using System;
using System.Collections.Generic;

namespace GraylesPlus
{
    public class OnlineUpdate
    {


        private readonly Config _config;

        #region Constructors and transformers

        public OnlineUpdate(Config config, IEnumerable<ModpackVersion> versions = null, string updateUrl = null)
        {
            this._config = config;
            List<ModpackVersion> newVersions = new List<ModpackVersion>();
            newVersions.AddRange(versions ?? new List<ModpackVersion>());
            this.AvailableVersions = newVersions.AsReadOnly();
            this.UpdateUrl = updateUrl ?? this._config.UpdateUrl;
        }

        public OnlineUpdate With(Config config = null, List<ModpackVersion> versions = null, string updateUrl = null) => new OnlineUpdate(
                config: config ?? this._config,
                versions: versions ?? this.AvailableVersions,
                updateUrl: updateUrl ?? this.UpdateUrl
            );

        #endregion


        #region Public properties

        public IEnumerable<ModpackVersion> AvailableVersions { get; private set; }

        public string UpdateUrl { get; private set; }

        #endregion



        #region Activities this class can perform

        public static OnlineUpdate Fetch(Config config)
        {
            using (var webClient = new System.Net.WebClient())
            {
                OnlineUpdate update = new OnlineUpdate(config);
                do
                {
                    Console.WriteLine($"Checking for updates at {update.UpdateUrl}");
                    config = config.With(updateUrl: update.UpdateUrl); // keep following until the url stabilizes
                    try
                    {
                        string json = webClient.DownloadString(config.UpdateUrl);
                        update = Parse(config, json);
                    } catch (System.Net.WebException e)
                    {
                        Console.WriteLine($"Failed to get updates from {update.UpdateUrl}: {e}");
                        return update;
                    }

                } while (update.UpdateUrl != config.UpdateUrl);
                Console.WriteLine($"Found updates: {update.AvailableVersions}");
                return update;
            }
        }

        public static OnlineUpdate Parse(Config config, string json)
        {

            var versionSpec = new { name = "", url = "", hashcode = "", latest = false };
            var spec = new { updateUrl = "", versions = new[] { versionSpec } };
            var results = Newtonsoft.Json.JsonConvert.DeserializeAnonymousType(json, spec);

            List<ModpackVersion> versions = new List<ModpackVersion>();
            foreach (var version in results.versions)
            {
                versions.Add(new ModpackVersion(
                    version: version.name,
                    hashcode: version.hashcode,
                    url: version.url,
                    latest: version.latest
                    ));
            }

            return new OnlineUpdate(
                config: config,
                versions: versions,
                updateUrl: results.updateUrl
                );
        }

        #endregion

    }
}
