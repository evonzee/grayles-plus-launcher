namespace GraylesPlus
{
    public class ModpackVersion
    {
        public readonly string VersionNumber;
        public readonly string HashCode;
        public readonly string DownloadUrl;
        public readonly bool Latest;

        public ModpackVersion(string version, string hashcode = null, string url = null, bool latest = false)
        {
            this.VersionNumber = version;
            this.HashCode = hashcode;
            this.DownloadUrl = url;
            this.Latest = latest;
        }
    }
}