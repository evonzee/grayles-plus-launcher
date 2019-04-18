using System;
using System.IO;
using Newtonsoft.Json;

namespace GraylesPlus
{
    public class Config
    {

        #region Constructors and transformers

        private readonly string _graylesRoot;
        private readonly string _starboundRoot;

        public Config(): this(Directory.GetCurrentDirectory(), "") {}

        public Config(string graylesRoot, string starboundRoot){
            this._graylesRoot = graylesRoot;
            this._starboundRoot = starboundRoot;
        }

        public Config With(string graylesRoot = null, string starboundRoot = null) => new Config(
                graylesRoot: graylesRoot ?? this._graylesRoot,
                starboundRoot: starboundRoot ?? this._starboundRoot
            );

        #endregion

        #region Public properties

        public string GraylesRoot { get {
            return this._graylesRoot;
        }}

        public string ModRoot { get {
            return Path.Combine(this.GraylesRoot, "mods");
        }}

        public string StorageRoot { get {
            return Path.Combine(this.GraylesRoot, "storage");
        }}

        public string StarboundRoot { get {
            return this._starboundRoot;
        }}

        #endregion

        #region Save and load from disk


        private static string ConfigFile { get {return Path.Combine(Directory.GetCurrentDirectory(), "config.json"); } }

        public static Config Load() {
            var spec = new { graylesRoot = Directory.GetCurrentDirectory(), starboundRoot = "" };
            if(File.Exists(ConfigFile)) {
                spec = JsonConvert.DeserializeAnonymousType(File.ReadAllText(ConfigFile), spec);
            }
            return new Config(spec.graylesRoot, spec.starboundRoot);
        }

        public static void Save(Config config){
            var json = new { graylesRoot = config.GraylesRoot, starboundRoot = config.StarboundRoot };
            File.WriteAllText(ConfigFile, JsonConvert.SerializeObject(json));
        }

        #endregion

    }
}
