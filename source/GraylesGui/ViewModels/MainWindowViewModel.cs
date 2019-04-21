using System;
using g = GraylesPlus;
using ReactiveUI;
using System.Reactive;
using Avalonia.Controls;
using System.Runtime.InteropServices;
using GraylesGui.Views;

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
            }
        }

        private g.Mods _mods;
        public g.Mods Mods {
            get => this._mods;
            private set => this.RaiseAndSetIfChanged(ref _mods, value);
        }

        private g.Starbound _starbound;
        public g.Starbound Starbound {
            get => this._starbound;
            private set => this.RaiseAndSetIfChanged(ref this._starbound, value);
        }


        public string StarboundFound { get { return this.Starbound.StarboundExecutableFolder != null ? "Yes" : "No"; }}

        public string GraylesSet { get { return this.Config.GraylesRoot != null ? $"Yes" : "No"; }}

        public string ModsDownloaded { get { return this.Mods.Downloaded ? $"Yes, {Mods.TargetVersion}" : "No"; }}

        public string ModsInstalled { get { return this.Mods.Installed ? $"Yes, {Mods.InstalledVersion}" : "No"; }}

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
            Console.WriteLine("Checking for updates..");
            this.Mods = this.Mods.With(targetVersion: g.Mods.GetLatestVersion());
        }

        void RunLaunch()
        {
            Console.WriteLine("Launching..");
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
        
        void RunInstall()
        {
            Console.WriteLine("Installing..");
            // eventually, download mods here
            this.RaisePropertyChanged("ModsDownloaded");
            if (!Mods.Downloaded)
            {
                Console.WriteLine("Showing dialog");
                var model = new DownloadAdviceViewModel()
                {
                    Mods = this.Mods.With(targetVersion: g.Mods.GetLatestVersion())
                };
                try
                {
                    var window = new DownloadAdvice
                    {
                        DataContext = model,
                    };
                    window.Show();
                } catch(Exception e)
                {
                    Console.WriteLine("Error! " + e);
                }
                Console.WriteLine("done");
                return;
            }

            Mods.Install();
            this.RaisePropertyChanged("ModsInstalled");
            Starbound.Configure();
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
