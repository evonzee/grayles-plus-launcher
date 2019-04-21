
using System.Diagnostics;
using System.IO;

namespace GraylesPlus
{
    public class Starbound
    {
        
        #region Constructors and transformers

        private readonly Config _config;
        
        public Starbound(Config config){
            this._config = config;
        }

        public Starbound With(Config config = null) => new Starbound(
                config: config ?? this._config
            );

        #endregion

        #region Public properties

        public string StarboundExecutableFolder { get {
            if(this._config.StarboundRoot == null || !Directory.Exists(this._config.StarboundRoot)){
                return null;
            }
            foreach(var dir in Directory.EnumerateDirectories(this._config.StarboundRoot)){
                if(dir.EndsWith("win64") || dir.EndsWith("linux") || dir.EndsWith("osx")){
                    return dir;
                }
            }
            return null;
        }}

        public string StarboundExecutable { get {
            if(this.StarboundExecutableFolder == null){
                return null;
            }
            return Path.Combine(this.StarboundExecutableFolder, "starbound");;
        }}

        public string StarboundProfile { get {
            if(this.StarboundExecutableFolder == null){
                return null;
            }
            return Path.Combine(this.StarboundExecutableFolder, "grayles.config");
        }}

        public bool Configured { get {
            if(this.StarboundProfile == null){
                return false;
            }
            return File.Exists(this.StarboundProfile);
        }}

        #endregion


        #region Activities this class can perform

        public bool Configure(){
            var json = new {
                defaultConfiguration = new {
                    rconServerBind = "*",
                    queryServerBind = "*",
                    gameServerBind = "*"
                },
                assetDirectories = new string[] {
                    "../assets",
                    this._config.ModRoot
                },
                storageDirectory = this._config.StorageRoot
            };

            File.WriteAllText(this.StarboundProfile, Newtonsoft.Json.JsonConvert.SerializeObject(json));

            return this.Configured;
        }

        public void Launch(){
            Directory.SetCurrentDirectory(this.StarboundExecutableFolder);
            var process = new Process();
            process.StartInfo.FileName = this.StarboundExecutable;
            process.StartInfo.Arguments = $"-bootconfig {this.StarboundProfile}";
            process.StartInfo.Environment["LD_LIBRARY_PATH"] = "./;" + System.Environment.GetEnvironmentVariable("LD_LIBRARY_PATH"); // may be needed on Linux
            process.Start();
        }

        #endregion

    }
}