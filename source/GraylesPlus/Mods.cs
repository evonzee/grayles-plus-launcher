
using System.IO;

namespace GraylesPlus
{
    public class Mods
    {
        private readonly Config _config;
        private readonly string _targetVersion;

        public Mods(): this(new Config(), null) {}

        public Mods(Config config, string targetVersion = null){
            this._config = config;
            this._targetVersion = targetVersion;
        }

        public Mods With(Config config = null, string targetVersion = null) => new Mods(
                config: config ?? this._config,
                targetVersion: targetVersion ?? this._targetVersion
            );

        public bool Downloaded { get {
            return false;
        }}

        public bool Installed { get {
            return this.InstalledVersion != null;
        }}

        public string InstalledVersion { get {
            string versionfile = Path.Combine(this._config.GraylesRoot, "version.txt");
            if(!File.Exists(versionfile)){
                return null;
            }

            return File.ReadAllText(versionfile);
        }}

        public string TargetVersion { get {
            if(this._targetVersion != null){
                return this._targetVersion;
            }
            return InstalledVersion;
        }}

        public string ModZip { get {
            if(this.TargetVersion == null){
                return null;
            }
            return Path.Combine(this._config.GraylesRoot, $"Grayles Modpack V{this.TargetVersion}.zip");
        }}

        public string ModPath { get {
            return this._config.ModRoot;
        }}

        public string GetLatestVersion() {
            return "4.0.0";  // eventually, get this from a service on grayles.net
        }

        public bool Install(){
            return this.Installed;
        }


    }
}