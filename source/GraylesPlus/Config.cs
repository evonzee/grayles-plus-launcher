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
        private readonly string _updateUrl;

        public Config(): this(null, null, null) {}

        public Config(string graylesRoot, string starboundRoot, string updateUrl){
            this._graylesRoot = graylesRoot ?? AppDirectory;
            this._starboundRoot = starboundRoot ?? "";
            this._updateUrl = updateUrl ?? "https://raw.githubusercontent.com/evonzee/grayles-plus-launcher/master/.updates.json";
        }

        public Config With(string graylesRoot = null, string starboundRoot = null, string updateUrl = null) => new Config(
                graylesRoot: graylesRoot ?? this._graylesRoot,
                starboundRoot: starboundRoot ?? this._starboundRoot,
                updateUrl: updateUrl ?? this._updateUrl
            );

        #endregion

        #region Public properties

        public static string AppDirectory { get {
            return AppDomain.CurrentDomain.BaseDirectory;
        }}

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

        public string UpdateUrl { get; internal set; }


        #endregion

        #region Save and load from disk


        private static string ConfigFile { get {return Path.Combine(AppDirectory, "config.json"); } }

        public static Config Load() {
            var spec = new { graylesRoot = AppDirectory, starboundRoot = "", updateUrl = "" };
            if(File.Exists(ConfigFile)) {
                spec = JsonConvert.DeserializeAnonymousType(File.ReadAllText(ConfigFile), spec);
            }
            return new Config(spec.graylesRoot, spec.starboundRoot, spec.updateUrl);
        }

        public static void Save(Config config){
            var json = new { graylesRoot = config.GraylesRoot, starboundRoot = config.StarboundRoot, updateUrl = config.UpdateUrl };
            File.WriteAllText(ConfigFile, JsonConvert.SerializeObject(json));
        }

        #endregion

    }
}
