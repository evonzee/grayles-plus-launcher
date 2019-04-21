namespace GraylesPlus
{
    public class ModpackVersion
    {
        public readonly string VersionNumber;
        public readonly string HashCode;
        public readonly string DownloadUrl;

        public ModpackVersion(string version, string hashcode, string url)
        {
            this.VersionNumber = version;
            this.HashCode = hashcode;
            this.DownloadUrl = url;
        }
    }
}