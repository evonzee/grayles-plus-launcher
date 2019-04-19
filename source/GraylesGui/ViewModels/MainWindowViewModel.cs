using System;
using System.Collections.Generic;
using System.Text;
using g = GraylesPlus;

namespace GraylesGui.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public g.Config Config => g.Config.Load();

        public g.Mods Mods => new g.Mods(this.Config);

        public g.Starbound Starbound => new g.Starbound(this.Config);

        public string StarboundFound => this.Starbound.StarboundExecutableFolder != null ? "Yes" : "No";

        public string GraylesSet => this.Config.GraylesRoot != null ? $"Yes" : "No";

        public string ModsDownloaded => this.Mods.Downloaded ? $"Yes, {Mods.TargetVersion}" : "No";

        public string ModsInstalled => this.Mods.Installed ? $"Yes, {Mods.InstalledVersion}" : "No";
    }
}
