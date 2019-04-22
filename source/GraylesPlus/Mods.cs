
using System.IO;

namespace GraylesPlus
{
    public class Mods
    {
        private readonly Config _config;
        private readonly ModpackVersion _targetVersion;


        #region Constructors and transformers

        public Mods(Config config, ModpackVersion targetVersion = null)
        {
            this._config = config;
            this._targetVersion = targetVersion;
        }

        public Mods With(Config config = null, ModpackVersion targetVersion = null) => new Mods(
                config: config ?? this._config,
                targetVersion: targetVersion ?? this._targetVersion
            );

        #endregion


        #region Public properties

        public bool Downloaded
        {
            get
            {
                return File.Exists(this.ModZip);
            }
        }

        public bool Installed
        {
            get
            {
                return this.InstalledVersion != null;
            }
        }

        public string InstalledVersion
        {
            get
            {
                string versionfile = Path.Combine(this._config.GraylesRoot, "version.txt");
                if (!File.Exists(versionfile))
                {
                    return null;
                }

                return File.ReadAllText(versionfile);
            }
            private set
            {
                File.WriteAllText(Path.Combine(this._config.GraylesRoot, "version.txt"), value);
            }
        }

        public ModpackVersion TargetVersion
        {
            get
            {
                if (this._targetVersion != null)
                {
                    return this._targetVersion;
                }
                return new ModpackVersion(InstalledVersion);
            }
        }

        public string ModZipPath
        {
            get
            {
                if (this.TargetVersion == null)
                {
                    return null;
                }
                return Path.Combine(this._config.GraylesRoot, "zip");
            }
        }

        public string ModZip
        {
            get
            {
                if (this.TargetVersion == null || this.TargetVersion.VersionNumber == null)
                {
                    return null;
                }
                return Path.Combine(this.ModZipPath, $"Grayles Modpack V{this.TargetVersion.VersionNumber}.zip");
            }
        }

        public string ModPath
        {
            get
            {
                return this._config.ModRoot;
            }
        }

        #endregion

        #region Activities this class can perform

        public bool Install()
        {
            if (!this.Downloaded)
            {
                return false; // user needs to download first
            }

            if (Directory.Exists(this.ModPath))
            {
                Directory.Delete(this.ModPath, true);
            }
            Directory.CreateDirectory(ModPath);

            System.IO.Compression.ZipFile.ExtractToDirectory(this.ModZip, this.ModPath);

            this.InstalledVersion = TargetVersion.VersionNumber;

            return this.Installed;
        }

        #endregion

    }
}