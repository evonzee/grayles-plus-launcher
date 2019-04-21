namespace GraylesPlus
{
    public class ModpackVersion
    {
        public readonly string VersionNumber;
        public readonly string DownloadUrl;

        public ModpackVersion(string version, string url)
        {
            this.VersionNumber = version;
            this.DownloadUrl = url;
        }
    }
}