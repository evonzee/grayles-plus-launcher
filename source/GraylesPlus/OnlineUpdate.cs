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

        public static OnlineUpdate Fetch(string url)
        {
            throw new NotImplementedException();
        }



        #endregion

    }
}
