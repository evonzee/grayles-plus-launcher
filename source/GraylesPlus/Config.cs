using System;
using System.IO;

namespace GraylesPlus
{
    public class Config
    {
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
    }
}
