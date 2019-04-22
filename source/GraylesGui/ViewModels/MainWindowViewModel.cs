using System;
using System.Linq;
using g = GraylesPlus;
using ReactiveUI;
using System.Reactive;
using Avalonia.Controls;
using System.Runtime.InteropServices;
using GraylesGui.Views;
using System.IO;
using System.Security.Cryptography;

namespace GraylesGui.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {

        public MainWindowViewModel()
        {
            this.Config = g.Config.Load();
            this.SetupCommands();
        }

        private g.Config _config;

        public Window Window { get; set; } // yuk

        public g.Config Config { 
            get  => this._config;
            private set { 
                this.RaiseAndSetIfChanged(ref _config, value);
                this.Mods = new g.Mods(value);
                this.Starbound = new g.Starbound(value);
                this.RaisePropertyChanged("StarboundFound");
                this.RaisePropertyChanged("GraylesFound");
                this.RaisePropertyChanged("ModsDownloaded");
                this.RaisePropertyChanged("ModsInstalled");
                this.RaisePropertyChanged("TargetVersion");
            }
        }

        private g.Mods _mods;
        public g.Mods Mods {
            get => this._mods;
            private set { 
                this.RaiseAndSetIfChanged(ref _mods, value);
                this.RaisePropertyChanged("TargetVersion");
            }
        }

        private g.Starbound _starbound;
        public g.Starbound Starbound {
            get => this._starbound;
            private set => this.RaiseAndSetIfChanged(ref this._starbound, value);
        }

        private string _status;
        public string Status
        {
            get => this._status;
            private set
            {
                Console.WriteLine(value);
                this.RaiseAndSetIfChanged(ref this._status, value);
            }
        }



        public string StarboundFound { get { return this.Starbound.StarboundExecutableFolder != null ? "Yes" : "No"; }}

        public string GraylesSet { get { return this.Config.GraylesRoot != null ? $"Yes" : "No"; }}

        public string ModsDownloaded { get { return this.Mods.Downloaded ? $"Yes, {Mods.TargetVersion.VersionNumber}" : "No"; }}

        public string ModsInstalled { get { return this.Mods.Installed ? $"Yes, {Mods.InstalledVersion}" : "No"; }}

        public string TargetVersion { get { return this.Mods.TargetVersion.VersionNumber; } }

        private void SetupCommands(){
            CheckForUpdates = ReactiveCommand.Create(RunCheckForUpdates);
            Launch = ReactiveCommand.Create(RunLaunch);
            Install = ReactiveCommand.Create(RunInstall);
            FindStarbound = ReactiveCommand.Create(RunFindStarbound);
            FindGrayles = ReactiveCommand.Create(RunFindGrayles);
        }

        public ReactiveCommand<Unit, Unit> CheckForUpdates { get; private set; }
        public ReactiveCommand<Unit, Unit> Launch { get; private set; }
        public ReactiveCommand<Unit, Unit> Install { get; private set; }
        public ReactiveCommand<Unit, Unit> FindStarbound { get; private set; }
        public ReactiveCommand<Unit, Unit> FindGrayles { get; private set; }

        void RunCheckForUpdates(){
            Status = "Checking for updates..";
            g.OnlineUpdate update = g.OnlineUpdate.Fetch(this.Config);
            if(update.UpdateUrl != this.Config.UpdateUrl)
            {
                this.Config = this.Config.With(updateUrl: update.UpdateUrl);
                g.Config.Save(this.Config);
            }
            if (update.AvailableVersions.Any())
            {
                this.Mods = this.Mods.With(targetVersion: update.AvailableVersions.First(v => v.Latest));
                Status = $"Latest available version is {this.Mods.TargetVersion.VersionNumber}";
            }
        }

        void RunLaunch()
        {
            Status = "Launching..";
            if (!Mods.Installed)
            {
                return;
            }
            if (!this.Starbound.Configured)
            {
                return;
            }
            Starbound.Launch();
        }

        async void RunInstall()
        {
            Status = "Installing..";

            if (!Mods.Downloaded)
            {
                Directory.CreateDirectory(this.Mods.ModZipPath);
                using (var webClient = new System.Net.WebClient())
                {
                    webClient.DownloadProgressChanged += (sender, e) => { this.Status = $"Downloaded {e.ProgressPercentage}% ({e.BytesReceived}/{e.TotalBytesToReceive})"; };

                    this.Status = "Starting download...";
                    var dl = webClient.DownloadFileTaskAsync(new Uri(this.Mods.TargetVersion.DownloadUrl), this.Mods.ModZip);
                    try
                    {
                        await dl;
                        this.Status = "Finished downloading.";
                        if (!dl.IsCompletedSuccessfully)
                        {
                            Status = "The download failed.";
                            return;
                        }
                    }
                    catch (System.Net.WebException)
                    {
                        this.Status = "Error while downloading.  Please try manually placing the file in the zip directory.";
                        return;
                    }
                }
            }

            this.RaisePropertyChanged("ModsDownloaded");
            if (!Mods.Downloaded)
            {
                Status = "Something went wrong while downloading.  Please check the file in the Grayles zip directory";
                return;
            }

            // check checksum
            if (this.Mods.TargetVersion.HashCode != null)
            {
                Status = "Checking Checksum...";
                using (FileStream fs = File.OpenRead(this.Mods.ModZip))
                using (HashAlgorithm alg = SHA256.Create())
                {
                    byte[] hash = alg.ComputeHash(fs);
                    string hashString = BitConverter.ToString(hash).Replace("-", "").ToLower();
                    if (hashString != this.Mods.TargetVersion.HashCode)
                    {
                        string newname = Path.GetRandomFileName();
                        File.Move(this.Mods.ModZip, $"{this.Mods.ModZip}.{newname}.zip");
                        Status = $"The download was corrupted.  We have renamed it to include {newname} for diagnostics.";
                        this.RaisePropertyChanged("ModsDownloaded");
                    }
                }
            }

            Status = "Installing...";
            Mods.Install();
            this.RaisePropertyChanged("ModsInstalled");

            Status = "Configuring Starbound...";
            Starbound.Configure();

            Status = "Install process complete.";
        }

        async void RunFindStarbound()
        {
            Console.WriteLine("Finding Starbound..");

            var folder = this.Config.StarboundRoot;
            if(folder == null || this.Starbound.StarboundExecutableFolder == null)
            {
                // default to a good folder depending on platform
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    Console.WriteLine("Windows detected");
                    folder = "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Starbound";
                } else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    Console.WriteLine("MacOS detected");
                    folder = Environment.GetEnvironmentVariable("HOME") + "/Library/Application Support/Steam/steamapps/common/Starbound";
                } else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    Console.WriteLine("Linux detected");
                    folder = "~/";
                }
            }

            var task = new OpenFolderDialog()
            {
                InitialDirectory = folder,
                Title = "Select Starbound root directory (should contain assets/ and doc/)"
            }.ShowAsync(this.Window);

            folder = await task;
            if (task.IsCompletedSuccessfully && !task.IsCanceled)
            {
                Console.WriteLine("Setting folder!");
                folder = System.Net.WebUtility.UrlDecode(folder);
                this.Config = this.Config.With(starboundRoot: folder);
                g.Config.Save(this.Config);
            }
        }

        async void RunFindGrayles()
        {
            Console.WriteLine("Finding Grayles..");
            var folder = this.Config.GraylesRoot;
            if (folder == null || this.Mods.ModPath == null)
            {
                folder = AppDomain.CurrentDomain.BaseDirectory;
            }

            var task = new OpenFolderDialog()
            {
                InitialDirectory = folder
            }.ShowAsync(this.Window);

            folder = await task;
            if (task.IsCompletedSuccessfully && !task.IsCanceled)
            {
                Console.WriteLine("Setting folder!");
                folder = System.Net.WebUtility.UrlDecode(folder);
                this.Config = this.Config.With(graylesRoot: folder);
                g.Config.Save(this.Config);
            }
        }


    }
}
